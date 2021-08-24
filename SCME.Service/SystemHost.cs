using NLog;
using NLog.Config;
using NLog.Targets;
using SCME.InterfaceImplementations.NewImplement.SQLite;
using SCME.Logger;
using SCME.Service.Properties;
using SCME.Types;
using SCME.Types.Database;
using SCME.UIServiceConfig.Properties;
using System;
using System.Data.SQLite;
using System.IO;
using System.ServiceModel;
using System.Text;
using NLogger = NLog.Logger;

namespace SCME.Service
{
    internal static class SystemHost
    {
        private static ExternalControlServer ms_ControlService;
        private static ServiceHost ms_ControlServiceHost;
        private static ServiceHost ms_DatabaseServiceHost;
        private static ServiceHost ms_MaintenanceServiceHost;
        private static ServiceHost _SQLiteDbServiceHost;
        private static BroadcastCommunication m_Communication;
        private static IDbService _dbService;

        //Логер
        private static NLogger Logger;
        //Месторасположения файлов логов
        private static string LogPath;
        private static string DirectoryLogPath;

        public static void AppendLog(ComplexParts device, LogMessageType messageType, string message)
        {
            string Message = message.Replace("'", string.Empty);
            switch (messageType)
            {
                //Информационное сообщение
                case LogMessageType.Info:
                    Logger.Info(string.Format("{0} INFO - {1}: {2}", DateTime.Now, device, Message));
                    break;
                //Важная веха
                case LogMessageType.Milestone:
                    Logger.Warn(string.Format("{0} MILESTONE - {1}: {2}", DateTime.Now, device, Message));
                    break;
                //Критическое сообщение
                case LogMessageType.Error:
                    Logger.Fatal(string.Format("{0} CRITICAL - {1}: {2}", DateTime.Now, device, Message));
                    //Создание копии при возникновении критического события
                    string CriticalLogsPath = Path.Combine(DirectoryLogPath, "Critical");
                    Directory.CreateDirectory(CriticalLogsPath);
                    File.Copy(LogPath, Path.Combine(CriticalLogsPath, string.Format("LogsService {0:dd.MM.yyyy HH_mm}.log", DateTime.Now)), true);
                    break;
            }
        }

        internal static bool? IsSyncedWithServer { get; set; }

        private static void AfterSyncWithServerRoutineHandler(string notSyncedReason)
        {
            //данная реализация будет вызвана после того, как будет пройдена стадия синхронизации данных SQLLite базы данных с данными центральной базы
            switch (notSyncedReason == String.Empty)
            {
                case true:
                    //если принятый notSyncedReason пуст - синхронизация успешно выполнена
                    IsSyncedWithServer = true;
                    AppendLog(ComplexParts.Service, LogMessageType.Milestone, "Local database was successfully synced with a central database");
                    break;

                default:
                    //синхронизация не выполнена. описание причины содержится в notSyncedReason
                    IsSyncedWithServer = false;

                    LogMessageType logMessageType;
                    switch (Settings.Default.IsLocal)
                    {
                        case true:
                            //синхронизация не выполнена потому, что отключена параметром в конфигурационном файле
                            logMessageType = LogMessageType.Info;
                            break;

                        default:
                            //синхронизация не выполнена потому, что в процессе синхронизации произошла ошибка
                            logMessageType = LogMessageType.Error;
                            break;

                    }

                    AppendLog(ComplexParts.Service, logMessageType, string.Format("Local database not synced with a central database. Reason: {0}", notSyncedReason));
                    break;
            }

            //процесс синхронизации завершён, сообщаем об этом UI
            FireSyncDBAreProcessedEvent();
        }

        internal static void SetCommunication(BroadcastCommunication Communication)
        {
            m_Communication = Communication;
        }

        private static void HostDbService()
        {
            _SQLiteDbServiceHost = new ServiceHost( _dbService = new SQLiteDbService(new SQLiteConnection(new SQLiteConnectionStringBuilder()
            {
                DataSource = Settings.Default.ResultsDatabasePath,
                SyncMode = SynchronizationModes.Full,
                JournalMode = SQLiteJournalModeEnum.Truncate,
                FailIfMissing = true
            }.ToString())));
             var behaviour = _SQLiteDbServiceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                behaviour.InstanceContextMode = InstanceContextMode.Single;

            if (SCME.UIServiceConfig.Properties.Settings.Default.IsLocal)
            
            if (UIServiceConfig.Properties.Settings.Default.IsLocal)
                _dbService.Migrate();
            
            
    
            try
            {
                _SQLiteDbServiceHost.Open();
            }
            catch (Exception ex)
            {
                AppendLog(ComplexParts.Service, LogMessageType.Error, $"New implement db local service not host: {ex.Message}");
            }
            
            AppendLog(ComplexParts.Service, LogMessageType.Info, "New implement db local service run");
        }
        
        internal static bool Initialize()
        {
            try
            {
                Logger = LogManager.GetCurrentClassLogger();
                LogPath = Settings.Default.LogsTracePathTemplate;
                DirectoryLogPath = Path.GetDirectoryName(LogPath);
                //Перезаписываемый файл логов
                FileTarget RollingLogFile = new FileTarget("RollingFileLog")
                {
                    Encoding = Encoding.UTF8,
                    Layout = "${message}",
                    FileName = Settings.Default.LogsTracePathTemplate,
                    ArchiveFileName = Path.Combine(DirectoryLogPath, "Archived", "LogsArchived.log"),
                    ArchiveAboveSize = 100000,
                    MaxArchiveFiles = 1
                };
                //Файл критических логов
                FileTarget CriticalLogFile = new FileTarget("CriticalFileLog")
                {
                    Encoding = Encoding.UTF8,
                    Layout = "${message}",
                    FileName = Path.Combine(DirectoryLogPath, "LogsServiceCritical.log")
                };
                //Конфигурация NLog
                LoggingConfiguration Configuration = new LoggingConfiguration();
                Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, RollingLogFile);
                Configuration.AddRule(LogLevel.Warn, LogLevel.Fatal, CriticalLogFile);
                LogManager.Configuration = Configuration;
                AppendLog(ComplexParts.Service, LogMessageType.Milestone, Resources.Log_SystemHost_Application_started);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"SCME.Service error.txt",
                    $"\n\n{DateTime.Now}\nEXCEPTION: {ex}\nINNER EXCEPTION: {ex.InnerException ?? new Exception("No additional information - InnerException is null")}\n");

                return false;
            }

//            try
//            {
//                SQLiteDatabaseService dbForMigration = new SQLiteDatabaseService(Settings.Default.ResultsDatabasePath);
//                dbForMigration.Open();
//                dbForMigration.Migrate();
//                dbForMigration.Close();
//            }
//            catch (Exception ex)
//            {
//                AppendLog(ComplexParts.Service, LogMessageType.Error, String.Format("Migrate database error: {0}", ex.Message));
//                return false;
//            }

            try
            {
                Results = new ResultsJournal();
                ///??
                //Results.Open(Settings.Default.DisableResultDB ? String.Empty : Settings.Default.ResultsDatabasePath, Settings.Default.DBOptionsResults, Settings.Default.MMECode);
                Results.Open(Settings.Default.ResultsDatabasePath, Settings.Default.DBOptionsResults, Settings.Default.MMECode);

                if (!Settings.Default.IsLocal)
                    AppendLog(ComplexParts.Service, LogMessageType.Info, Resources.Log_SystemHost_Result_journal_opened);

                //нам ещё не известно как завершится процесс синхронизации данных, поэтому
                IsSyncedWithServer = null;

//                switch (Settings.Default.IsLocal)
//                {
//                    case true:
//                        //синхронизация отключена, уведомляем UI, что стадия синхронизации баз данных пройдена
//                        AfterSyncWithServerRoutineHandler("Synchronization of the local database with a central database is prohibited by parameter DisableResultDB");
//                        break;
//
//                    default:
//                        //запускаем в потоке синхронизацию результатов измерений и профилей 
//                        Results.SyncWithServer(AfterSyncWithServerRoutineHandler);
//                        break;
//                }
                   
                try
                {
                    ms_ControlService = new ExternalControlServer();
                    ms_ControlServiceHost = new ServiceHost(ms_ControlService);
                    ms_ControlServiceHost.Open();
                    AppendLog(ComplexParts.Service, LogMessageType.Info, String.Format(Resources.Log_SystemHost_Control_service_is_listening));

                    ms_DatabaseServiceHost = new ServiceHost(typeof(DatabaseServer));

                    HostDbService();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(@"SCME.Service error.txt",
                    $"\n\n{DateTime.Now}\nEXCEPTION: {ex}\nINNER EXCEPTION: {ex.InnerException ?? new Exception("No additional information - InnerException is null")}\n");
                    AppendLog(ComplexParts.Service, LogMessageType.Error, $"SQLite database error: {ex?.InnerException?.ToString() ?? ex.ToString()}");
                    return false;
                }
               
                
                try
                {
                    ms_DatabaseServiceHost.AddServiceEndpoint(typeof(IDatabaseCommunicationService), new NetTcpBinding("DefaultTcpBinding"), Settings.Default.DBServiceExternalEndpoint);
                }
                catch (Exception ex)
                {
                    AppendLog(ComplexParts.Service, LogMessageType.Error, String.Format("Can't open external database service port: {0}", ex.Message));
                }

                ms_DatabaseServiceHost.Open();
                AppendLog(ComplexParts.Service, LogMessageType.Info, String.Format(Resources.Log_SystemHost_Database_service_is_listening));

                ms_MaintenanceServiceHost = new ServiceHost(typeof(MaintenanceServer));

                try
                {
                    ms_MaintenanceServiceHost.AddServiceEndpoint(typeof(IDatabaseMaintenanceService), new NetTcpBinding("DefaultTcpBinding"), Settings.Default.MaintenanceServiceExternalEndpoint);
                }
                catch (Exception ex)
                {
                    AppendLog(ComplexParts.Service, LogMessageType.Error, String.Format("Can't open external maintenance service port: {0}", ex.Message));
                }

                ms_MaintenanceServiceHost.Open();
                AppendLog(ComplexParts.Service, LogMessageType.Info, String.Format(Resources.Log_SystemHost_Maintenance_service_is_listening));

                return true;
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"SCME.Service error.txt",
                    $"\n\n{DateTime.Now}\nEXCEPTION: {ex}\nINNER EXCEPTION: {ex.InnerException ?? new Exception("No additional information - InnerException is null")}\n");
                AppendLog(ComplexParts.Service, LogMessageType.Error, ex.Message);
                Journal.Close();
                return false;
            }
        }

        internal static EventJournal Journal { get; private set; }
        internal static ResultsJournal Results { get; private set; }

        internal static void Close()
        {
            try
            {
                if (ms_ControlServiceHost != null)
                {
                    try
                    {
                        (ms_ControlService as IExternalControl).Deinitialize();
                    }
                    finally
                    {
                        if (ms_ControlServiceHost.State == CommunicationState.Faulted)
                            ms_ControlServiceHost.Abort();
                        else
                            ms_ControlServiceHost.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Journal != null)
                    AppendLog(ComplexParts.Service, LogMessageType.Error,
                                        String.Format(Resources.Log_SystemHost_Error_while_closing, @"Control host", ex.Message));
            }

            try
            {
                if (ms_DatabaseServiceHost != null)
                    if (ms_DatabaseServiceHost.State == CommunicationState.Faulted)
                        ms_DatabaseServiceHost.Abort();
                    else
                        ms_DatabaseServiceHost.Close();
            }
            catch (Exception ex)
            {
                if (Journal != null)
                    AppendLog(ComplexParts.Service, LogMessageType.Error,
                                        String.Format(Resources.Log_SystemHost_Error_while_closing, @"DB host", ex.Message));
            }

            try
            {
                if (ms_MaintenanceServiceHost != null)
                    if (ms_MaintenanceServiceHost.State == CommunicationState.Faulted)
                        ms_MaintenanceServiceHost.Abort();
                    else
                        ms_MaintenanceServiceHost.Close();
            }
            catch (Exception ex)
            {
                if (Journal != null)
                    AppendLog(ComplexParts.Service, LogMessageType.Error,
                                        String.Format(Resources.Log_SystemHost_Error_while_closing, @"Maintenance host", ex.Message));
            }

            try
            {
                Results.Close();
            }
            catch (Exception ex)
            {
                if (Journal != null)
                    AppendLog(ComplexParts.Database, LogMessageType.Error,
                                        String.Format(Resources.Log_SystemHost_Error_while_closing, @"Result journal", ex.Message));
            }

            if (Journal != null)
            {
                AppendLog(ComplexParts.Service, LogMessageType.Milestone, Resources.Log_SystemHost_Application_closed);
                Journal.Close();
            }
        }

        private static void FireSyncDBAreProcessedEvent()
        {
            string sCommunicationLive = String.Empty;

            switch (m_Communication == null)
            {
                case false:
                    m_Communication.PostSyncDBAreProcessedEvent();
                    break;

                default:
                    sCommunicationLive = "comunication object is not live (null value)";
                    break;
            }

            if (Journal != null)
            {
                string Mess = "Sync DataBases are processed";

                if (sCommunicationLive != String.Empty)
                    Mess = Mess + ", " + sCommunicationLive;

                AppendLog(ComplexParts.Service, LogMessageType.Info, Mess);
            }
        }
    }
}