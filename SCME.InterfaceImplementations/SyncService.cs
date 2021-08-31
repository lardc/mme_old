﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using SCME.InterfaceImplementations.SQLite;
using SCME.Types;
using SCME.Types.DatabaseServer;
using SCME.Types.Interfaces;
using SCME.Types.Profiles;
using SCME.Types.Utils;

namespace SCME.InterfaceImplementations
{
    public class SyncService : ISyncService
    {
        private readonly string _mmeCode;
        private readonly SQLiteConnection _connection;
        private readonly IProfilesService _saveProfileService;
        private readonly IResultsService _resultsService;

        private AfterSyncResultsRoutine FAfterSyncResultsRoutine;
        private AfterSyncProfilesRoutine FAfterSyncProfilesRoutine;
        
        public SyncService(string databasePath, string mmeCode)
        {
            //_sqLiteDbService = new SQLiteDbService(new SQLiteConnection(databasePath));
            _mmeCode = mmeCode;
            _connection = new SQLiteConnection(databasePath, false);
            _connection.Open();

            _saveProfileService = new SQLiteProfilesService(databasePath);
            _resultsService = new SQLiteResultsServiceLocal(databasePath);
        }

        private void SyncResultsWorkerHandler(InputWorkerParameters WorkerParameters)
        {
            try
            {
                var unsendedDevices = _resultsService.GetUnsendedDevices();
                using (var centralDbClient = new CentralDatabaseServiceClient())
                {
                    foreach (var unsendedDevice in unsendedDevices)
                    {
                        var sended = centralDbClient.SendResultToServer(unsendedDevice);
                        if (sended)
                            _resultsService.SetResultSended(unsendedDevice.Id);
                    }
                }
            }
            catch (Exception ex)
            {               
                throw new Exception(string.Format("Error while syncing results from local database with a master:{0}", ex.Message));
            }
        }

        private void SyncResultsCompletedHandler(string Error)
        {
            //этот код выполняется в потоке диспетчера, поэтому как обычно обращаемся к разделяемым данным и пользовательскому интерфейсу, не порождая никаких проблем
            //в принятом Error передаётся описание ошибки синхронизации профилей, если Error пуст - значит синхронизация была успешной
            if (this.FAfterSyncResultsRoutine != null)
                this.FAfterSyncResultsRoutine(Error);
        }

        public void SyncResults(AfterSyncResultsRoutine afterSyncResultsRoutine)
        {
            //синхронизация результатов измерений выполняется продолжительное время, поэтому чтобы не получить ContextSwitchDeadlock выполняем её в отдельном потоке, при успешном завершению которого будет вызвана реализация afterSyncResultsRoutine
            LongTimeRoutineWorker Worker = new LongTimeRoutineWorker(SyncResultsWorkerHandler, SyncResultsCompletedHandler);

            //запоминаем что нам надо вызвать когда LongTimeRoutineWorker успешно выполнит свою работу
            this.FAfterSyncResultsRoutine = afterSyncResultsRoutine;

            //стартуем фоновый поток, нужды во входных параметрах нет - поэтому будем передавать null
            Worker.Run(null);
        }

        private void SyncProfilesWorkerHandler(InputWorkerParameters WorkerParameters)
        {
//            try
//            {
//                using (var msSqlDbService = new DatabaseProxy("SCME.CentralDatabase"))
//                {
//                    var localProfiles = _sqLiteDbService.GetProfilesSuperficially(_mmeCode);
//                    var centralProfiles = msSqlDbService.GetProfilesSuperficially(_mmeCode);
//                    if(!_sqLiteDbService.GetMmeCodes().ContainsKey(_mmeCode))
//                        _sqLiteDbService.InsertMmeCode(_mmeCode);
//
//                    List<MyProfile> deletingProfiles;
//                    List<MyProfile> addingProfiles;
//                    
//                    if (_sqLiteDbService.Migrate())
//                    {
//                        deletingProfiles = localProfiles;
//                        addingProfiles = centralProfiles;
//                    }
//                    else
//                    {
//                        deletingProfiles = localProfiles.Except(centralProfiles, new MyProfile.ProfileByVersionTimeEqualityComparer()).ToList();
//                        addingProfiles = centralProfiles.Except(localProfiles, new MyProfile.ProfileByVersionTimeEqualityComparer()).ToList();
//                    }
//                    
//                    foreach (var i in deletingProfiles)
//                        _sqLiteDbService.RemoveProfile(i, _mmeCode);
//
//                    foreach (var i in addingProfiles)
//                    {
//                        i.DeepData = msSqlDbService.LoadProfileDeepData(i);
//                        _sqLiteDbService.InsertUpdateProfile(null, i, _mmeCode);
//                    }
//
//                    return;
//                }
//                using (var centralDbClient = new CentralDatabaseServiceClient())
//                {
////                    Thread.Sleep(5000);
////                    throw new Exception();
//                    
//                    var serverProfiles = centralDbClient.GetProfileItemsByMme(_mmeCode);
//                    serverProfiles.ForEach(m => m.NextGenerationKey = m.ProfileKey);
//
//                    SaveProfiles(serverProfiles);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(string.Format("Error while syncing profiles from local database with a master: {0}", ex.Message));
//            }
        }

        private void SyncProfilesCompletedHandler(string Error)
        {
            //этот код выполняется в потоке диспетчера, поэтому как обычно обращаемся к разделяемым данным и пользовательскому интерфейсу, не порождая никаких проблем
            //в принятом Error передаётся описание ошибки синхронизации профилей, если Error пуст - значит синхронизация была успешной
            if (this.FAfterSyncProfilesRoutine != null)
                this.FAfterSyncProfilesRoutine(Error);
        }

        public void SyncProfiles(AfterSyncProfilesRoutine afterSyncProfilesRoutine)
        {
            //синхронизация профилей выполняется продолжительное время, поэтому чтобы не получить ContextSwitchDeadlock выполняем её в отдельном потоке
            LongTimeRoutineWorker Worker = new LongTimeRoutineWorker(SyncProfilesWorkerHandler, SyncProfilesCompletedHandler);

            //запоминаем что нам надо вызвать когда LongTimeRoutineWorker закончит свою работу
            this.FAfterSyncProfilesRoutine = afterSyncProfilesRoutine;

            //стартуем фоновый поток, нужды во входных параметрах нет - поэтому будем передавать null
            Worker.Run(null);
        }

        public MyProfile SyncProfile(MyProfile profile)
        {
//            using (var msSqlDbService = new DatabaseProxy("SCME.CentralDatabase"))
//            {
//                var centralProfile = msSqlDbService.GetProfileByKey(profile.Key);
//                if (!new MyProfile.ProfileByVersionTimeEqualityComparer().Equals(profile, centralProfile))
//                {
//                    _sqLiteDbService.InsertUpdateProfile(profile, centralProfile, _mmeCode);
//                    return centralProfile;
//                }
//
//                return null;
//            }
            return null;
        }

        public void SyncProfiles(ICentralDatabaseService centralDatabaseService)
        {
            var serverProfiles = centralDatabaseService.GetProfileItemsByMme(_mmeCode);
            SaveProfiles(serverProfiles);
        }

        private void SaveProfiles(List<ProfileItem> serverProfiles)
        {
            _saveProfileService.SaveProfilesFromMme(serverProfiles, _mmeCode);
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }
}
