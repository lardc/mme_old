using NLog;
using NLog.Config;
using NLog.Targets;
using SCME.Types;
using SCME.UI.Logger;
using SCME.UIServiceConfig.Properties;
using System;
using System.IO;
using System.Text;
using NLogger = NLog.Logger;

namespace SCME.UI
{
    internal static class SystemHost
    {
        private static BroadcastCommunication m_Communication;
        private static LogicContainer _IoMain;

        //Логер
        private static NLogger Logger;
        //Месторасположения файлов логов
        private static string LogPath;
        private static string DirectoryLogPath;

        internal static ResultsJournal Results
        {
            get; private set;
        }

        internal static void Initialize()
        {
            m_Communication = new BroadcastCommunication();
            SetCommunication(m_Communication);
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
                AppendLog(ComplexParts.Service, LogMessageType.Milestone, "Application started");
                
                Results = new ResultsJournal();
                Results.Open(Settings.Default.ResultsDatabasePath, Settings.Default.DBOptionsResults, Settings.Default.MMECode);
                if (!Settings.Default.IsLocal)
                    AppendLog(ComplexParts.Service, LogMessageType.Info, "Result journal opened");

                _IoMain = new LogicContainer(m_Communication);
                _IoMain.Initialize(Cache.Main.Param);
            }
            catch (Exception ex)
            {
                File.AppendAllText("SCME.UI error.txt", $"\n\n{DateTime.Now}\nEXCEPTION: {ex}\nINNER EXCEPTION: {ex.InnerException ?? new Exception("No additional information - InnerException is null")}\n");
            }
        }

        internal static void SetCommunication(BroadcastCommunication Communication)
        {
            m_Communication = Communication;
        }

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
    }
}