using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public static class Log
    {
        private static ILog _logSink = new ConsoleSink();
        public static void SetLogSink(ILog logSink)
        {
            _logSink = logSink;
        }

        public static void LogMsg(string message, params object[] args)
        {
            if (_logSink != null)
            {
                try
                {
                    _logSink.LogMsg(message, args);
                }
                catch
                { }
            }
        }

        public static void LogErr(string message, Exception ex)
        {
            if (_logSink != null)
            {
                try
                {
                    _logSink.LogErr(message, ex);
                }
                catch
                { }
            }
        }

        public static void LogErr(string message, params object[] args)
        {
            if (_logSink != null)
            {
                try
                {
                    _logSink.LogErr(message, args);
                }
                catch
                { }
            }
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
