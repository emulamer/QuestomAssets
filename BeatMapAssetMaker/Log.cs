using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker
{
    public static class Log
    {
        public static void LogErr(string error, params object[] o)
        {
            Console.WriteLine($"ERROR: {error}", o);
        }

        public static void LogMsg(string message, params object[] o)
        {
            Console.WriteLine(message, o);
        }
    }
}
