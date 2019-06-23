using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public static class Log
    {
        private static List<ILog> _logSinks = new List<ILog>();

        public static void ClearLogSinks()
        {
            _logSinks.Clear();
        }

        public static void SetLogSink(ILog logSink)
        {
            _logSinks.Add(logSink);
        }

        public static void RemoveLogSink(ILog logSink)
        {
            if (_logSinks.Contains(logSink))
                _logSinks.Remove(logSink);
        }

        public static void LogMsg(string message, params object[] args)
        {
            _logSinks.ForEach(x =>
            {
                try
                { x.LogMsg(message, args); }
                catch { }
            });
        }

        public static void LogErr(string message, Exception ex)
        {
            _logSinks.ForEach(x =>
            {
                try
                { x.LogErr(message, ex); }
                catch { }
            });
        }

        public static void LogErr(string message)
        {
            _logSinks.ForEach(x =>
            {
                try
                { x.LogErr(message); }
                catch { }
            });
        }
    }

    public class ConsoleSink : ILog
    {
        public void LogErr(string message, Exception ex)
        {
            Console.WriteLine($"{message} ({ex.Message} {ex.StackTrace}");
        }

        public void LogErr(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void LogMsg(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}
