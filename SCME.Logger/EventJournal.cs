using NLogger = NLog.Logger;
using NLog;
using NLog.Config;
using NLog.Targets;
using SCME.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Text;

namespace SCME.Logger
{
    public class EventJournal
    {
        //private static readonly object ms_Locker = new object();
        //private ConcurrentQueue<LogItem> m_MessageList;
        //private readonly AutoResetEvent m_ListEvent;
        //private SQLiteConnection m_Connection;
        //private SQLiteCommand m_InsertCommand;
        //private Thread m_LogWriteThread;
        //private bool m_Detailed, m_ThreadStarted;
        //private volatile bool m_IsThreadClosed, m_ShutdownRequest;

        //Логер
        private readonly NLogger Logger;
        //Месторасположения файлов логов
        private string LogPath;
        private string DirectoryLogPath;

        /// <summary>Инициализирует новый экземпляр класса EventJournal</summary>
        public EventJournal()
        {
            //m_MessageList = new ConcurrentQueue<LogItem>();
            //m_ListEvent = new AutoResetEvent(false);

            Logger = LogManager.GetCurrentClassLogger();
    }

        /// <summary>Открытие конфигурации логера</summary>
        /// <param name="logDatabasePath">База данных</param>
        /// <param name="logDatabaseOptions">Дополнительные настройки базы данных</param>
        /// <param name="logTracePath">Перезаписываемый файл логов</param>
        /// <param name="forceLogTraceFlush">Принудительная операция записи в файл</param>
        /// <param name="detailed">Детализированные логи</param>
        public void Open(string logDatabasePath, string logDatabaseOptions, string logTracePath, bool forceLogTraceFlush = true, bool detailed = false)
        {
            //m_Detailed = detailed;
            //if (m_LogWriteThread != null)
            //    m_LogWriteThread.Abort();
            //m_LogWriteThread = new Thread(ThreadPoolCallback) { IsBackground = true, Priority = ThreadPriority.Lowest };

            //if (!string.IsNullOrWhiteSpace(logDatabasePath))
            //{
            //    m_Connection = new SQLiteConnection(String.Format("data source={0};{1}", logDatabasePath, logDatabaseOptions), false);
            //    m_Connection.Open();
            //    m_InsertCommand = m_Connection.CreateCommand();
            //}
            //Trace.Listeners.Clear();
            //var listener = new TextWriterTraceListener(m_LogTracePath, @"FileLog") { IndentSize = 8 };
            //Trace.Listeners.Add(listener);
            //Trace.AutoFlush = m_ForceLogTraceFlush;
            //m_IsThreadClosed = false;
            //m_LogWriteThread.Start();
            //m_ThreadStarted = true;

            LogPath = logTracePath;
            DirectoryLogPath = Path.GetDirectoryName(LogPath);
            //Перезаписываемый файл логов
            FileTarget RollingLogFile = new FileTarget("RollingFileLog")
            {
                Encoding = Encoding.UTF8,
                Layout = "${message}",
                FileName = logTracePath,
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
        }

        /// <summary>Закрытие конфигурации логера</summary>
        public void Close()
        {
            //try
            //{
            //    m_ShutdownRequest = true;
            //    m_ListEvent.Set();
            //    if (m_ThreadStarted)
            //        while (!m_IsThreadClosed)
            //            Thread.Sleep(10);
            //    Trace.Flush();
            //    if (m_Connection != null && m_Connection.State == ConnectionState.Open)
            //        m_Connection.Close();
            //}
            //catch { }
        }

        /// <summary>Добавление записи в лог</summary>
        /// <param name="device">Устройство, вызвавшее событие</param>
        /// <param name="messageType">Тип сообщения</param>
        /// <param name="message">Текст сообщения</param>
        public void AppendLog(ComplexParts device, LogMessageType messageType, string message)
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
            
            //m_MessageList.Enqueue(new LogItem
            //    {
            //        Timestamp = DateTime.Now,
            //        Source = Device,
            //        MessageType = Type,
            //        Message = Message
            //    });
            
            //m_ListEvent.Set();
        }

        /// <summary>Чтение логов с конца</summary>
        public List<LogItem> ReadFromEnd(long Tail, long Count)
        {
            return new List<LogItem>();

            //lock (ms_Locker)
            //{
            //    var logs = new List<LogItem>();

            //    if (m_Connection == null || m_Connection.State != ConnectionState.Open)
            //        return logs;

            //    try
            //    {
            //        var cmd = m_Connection.CreateCommand();
            //        cmd.CommandText =
            //            string.Format(
            //                "SELECT * FROM MainTable WHERE MainTable.ID < {0} ORDER BY MainTable.ID DESC LIMIT {1}",
            //                Tail, Count);

            //        using (var reader = cmd.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                logs.Add(new LogItem
            //                    {
            //                        ID = (Int64) reader[0],
            //                        Timestamp = DateTime.Parse((string) reader[1], CultureInfo.InvariantCulture),
            //                        Source = (ComplexParts)(byte) reader[2],
            //                        MessageType = (LogMessageType)(byte)reader[3],
            //                        Message = (string) reader[4]
            //                    });
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        logs.Add(new LogItem
            //            {
            //                ID = 0,
            //                Timestamp = DateTime.Now,
            //                Source = 0,
            //                MessageType = 0,
            //                Message = ex.Message
            //            });
            //    }

            //    return logs;
            //}
        }
        
        //private void ThreadPoolCallback(object Parameter)
        //{
        //    try
        //    {
        //        while (!m_ShutdownRequest || (m_MessageList.Count > 0))
        //        {
        //            LogItem item;

        //            while (m_MessageList.TryDequeue(out item))
        //            {
        //                if (!m_Detailed && item.MessageType == LogMessageType.Note)
        //                    continue;

        //                InsertIntoDatabase(item.Timestamp, item.Source,
        //                                   item.MessageType, item.Message);

        //                switch (item.MessageType)
        //                {
        //                    case LogMessageType.Note:
        //                    case LogMessageType.Info:
        //                        Trace.WriteLine(String.Format("{0} INFO - {1}: {2}", item.Timestamp,
        //                                                      item.Source, item.Message));
        //                        break;
        //                    case LogMessageType.Error:
        //                    case LogMessageType.Error:
        //                    case LogMessageType.Error:
        //                        Trace.WriteLine(String.Format("{0} CRITICAL - {1}: {2}", item.Timestamp,
        //                                                      item.Source, item.Message));
        //                        break;
        //                }
        //            }
        //            if(!m_ShutdownRequest)
        //                m_ListEvent.WaitOne();
        //        }
        //    }
        //    finally
        //    {
        //        m_IsThreadClosed = true;
        //    }
        //}

        //private void InsertIntoDatabase(DateTime Timestamp, ComplexParts Source, LogMessageType Type, string Message)
        //{
        //    lock (ms_Locker)
        //    {
        //        if (m_Connection == null || m_Connection.State != ConnectionState.Open)
        //            return;
                
        //        try
        //        {
        //            m_InsertCommand.CommandText =
        //                string.Format(
        //                    "INSERT INTO MainTable(ID, DateTimeStamp, SourceID, TypeID, Message) VALUES(NULL, '{0}', {1}, {2}, '{3}')",
        //                    Timestamp.ToString(@"yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), (byte)Source, (byte)Type, Message);

        //            m_InsertCommand.ExecuteNonQuery();
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //}
    }
}