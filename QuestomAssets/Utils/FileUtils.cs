using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Utils
{
    public class FileUtils
    {
        public static Func<string> GetTempDirectoryOverride;
        public static string GetTempDirectory()
        {
            if (GetTempDirectoryOverride != null)
                return GetTempDirectoryOverride();

            return System.IO.Path.GetTempPath();
        }
    }
}
