using System;
using System.Diagnostics;

namespace Goblintools
{
    public static class Log
    {
        private static EventLog _ApplicationLog { get; set; }
        private static EventLog ApplicationLog
        {
            get
            {
                if (_ApplicationLog == null)
                    _ApplicationLog = new EventLog { Source = "Goblintools" };

                return _ApplicationLog;
            }
        }

        public static void Information(string message)
        {
            ApplicationLog.WriteEntry(message, EventLogEntryType.Information);
        }

        public static void Warning(string message)
        {
            ApplicationLog.WriteEntry(message, EventLogEntryType.Warning);
        }

        public static void Error(Exception ex)
        {
            ApplicationLog.WriteEntry($"{ex.Message}\r\n{ex.StackTrace}", EventLogEntryType.Error);
        }
    }
}
