using SCME.InterfaceImplementations.SQLite;
using SCME.Types;
using SCME.Types.Database;
using SCME.Types.DatabaseServer;
using SCME.Types.Profiles;
using SCME.UIServiceConfig.Properties;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SCME.UI.IO
{
    internal class IoDbSync
    {
        private readonly BroadcastCommunication _communication;
        private readonly MonitoringSender _monitoringSender;
        private string _mmeCode;
        private SQLiteDbService _sqLiteDbService;
        private IDbService _msSqlDbService;

        public IoDbSync(BroadcastCommunication communication, MonitoringSender monitoringSender)
        {
            _communication = communication;
            _monitoringSender = monitoringSender;
        }

        internal void Initialize(string databasePath, string databaseOptions, string mmeCode = null)
        {
            try
            {
                var connectionString = $"data source={databasePath};{databaseOptions}";
                _mmeCode = mmeCode;
                _sqLiteDbService = new SQLiteDbService(new SQLiteConnection(connectionString));
                _msSqlDbService = new DatabaseProxy(Settings.Default.CentralDatabase);
                _communication.PostDbSyncState(DeviceConnectionState.ConnectionInProcess, string.Empty);
            }
            catch (Exception e)
            {
                SystemHost.AppendLog(ComplexParts.Sync, LogMessageType.Error, $"Local database not synced with a central database. Reason: {e.ToString()}");
            }
        }

        public Task<InitializationResponse> SyncProfilesAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                var initializationResponse = new InitializationResponse()
                {
                    MmeCode = _mmeCode
                };

                try
                {
                    if (Settings.Default.IsLocal)
                        initializationResponse.SyncMode = SyncMode.Local;
                    else
                    {
                        _monitoringSender.Sync(SyncProfiles());
                        SendUnSendedResult();
                        initializationResponse.SyncMode = SyncMode.Sync;
                    }
                }
                catch (Exception e)
                {
                    initializationResponse.SyncMode = SyncMode.NotSync;
                    _communication.PostDbSyncState(DeviceConnectionState.ConnectionFailed, e.Message);
                }

                _communication.PostDbSyncState(DeviceConnectionState.ConnectionSuccess, string.Empty);

                return initializationResponse;
            });
        }

        private int SyncProfiles()
        {
            try
            {
                var localProfiles = _sqLiteDbService.GetProfilesSuperficially(_mmeCode);
                var centralProfiles = _msSqlDbService.GetProfilesDeepByMmeCode(_mmeCode);
                if (!_sqLiteDbService.GetMmeCodes().ContainsKey(_mmeCode))
                    _sqLiteDbService.InsertMmeCode(_mmeCode);

                List<MyProfile> deletingProfiles;
                List<MyProfile> addingProfiles;

                if (_sqLiteDbService.Migrate())
                {
                    deletingProfiles = localProfiles;
                    addingProfiles = centralProfiles;
                }
                else
                {
                    deletingProfiles = localProfiles.Except(centralProfiles, new MyProfile.ProfileByVersionTimeEqualityComparer()).ToList();
                    addingProfiles = centralProfiles.Except(localProfiles, new MyProfile.ProfileByVersionTimeEqualityComparer()).ToList();
                }

                foreach (var i in deletingProfiles)
                    _sqLiteDbService.RemoveProfile(i, _mmeCode);

                foreach (var i in addingProfiles)
                {
                    _sqLiteDbService.InsertUpdateProfile(null, i, _mmeCode);
                }

                return localProfiles.Count - deletingProfiles.Count + addingProfiles.Count;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while syncing profiles from local database with a master: {0}", ex.ToString()));
            }
        }

        private void SendUnSendedResult()
        {
            int UnsentCount = 0;
            DateTime UnsentStartDate = DateTime.Now;
            using (var centralDbClient = new CentralDatabaseServiceClient(Settings.Default.CentralDatabaseService))
                foreach (var unSendedDevice in _sqLiteDbService.SqLiteResultsServiceLocal.GetUnsendedDevices())
                    if (centralDbClient.SendResultToServer(unSendedDevice))
                        _sqLiteDbService.SqLiteResultsServiceLocal.SetResultSended(unSendedDevice.Id);
                    //Подсчет неотправленных
                    else
                    {
                        UnsentCount++;
                        if (unSendedDevice.Timestamp <= UnsentStartDate)
                            UnsentStartDate = unSendedDevice.Timestamp;
                    }
            if (UnsentCount != 0)
                MessageBox.Show(string.Format("Количество неотправленных результатов: {0}\nРанняя дата: {1:dd.MM.yyyy HH:mm:ss}", UnsentCount, UnsentStartDate), "Информация о неотправленных результатах");
        }

        public (MyProfile profile, bool IsInMmeCode) SyncProfile(MyProfile profile)
        {
            var (centralProfile, isInMmeCode) = _msSqlDbService.GetTopProfileByName(_mmeCode, profile.Name);
            
            if (!isInMmeCode)
                return (null, false);
            
            if (!new MyProfile.ProfileByVersionTimeEqualityComparer().Equals(profile, centralProfile))
            {
                centralProfile.DeepData = _msSqlDbService.LoadProfileDeepData(centralProfile);
                _sqLiteDbService.InsertUpdateProfile(profile, centralProfile, _mmeCode);
                return (centralProfile, true);
            }

            return (null, true);
        }
    }
}