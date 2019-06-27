using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QuestomAssets
{
    public class LogTiming : IDisposable
    {
        private string _logTitle;
        private Stopwatch _stopwatch;
        public LogTiming(string logTitle)
        {
            _logTitle = logTitle;
            _stopwatch = new Stopwatch();
            Log.LogMsg($"Starting {logTitle}...");
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            Log.LogMsg($"Finished {_logTitle} in {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
