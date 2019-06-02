using Emulamer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public static class Extensions
    {
        public static string FindFirstOfSplit(this Apkifier apk, string assetsFile)
        {
            int lastDot = assetsFile.LastIndexOf('.');
            string afterDot = assetsFile.Substring(lastDot, assetsFile.Length - lastDot);
            string noSplit;
            if (afterDot.ToLower().StartsWith(".split"))
            {
                noSplit = assetsFile.Substring(0, lastDot);
                if (apk.FileExists(noSplit))
                    return noSplit;
                
            }
            else
            {
                noSplit = assetsFile;
            }
            var split0 = noSplit + ".split0";
            if (apk.FileExists(split0))
                return split0;
            if (apk.FileExists(assetsFile))
            {
                return assetsFile;
            }
            throw new ArgumentException("The file doesn't exist in the APK with any name!");
        }

    }
}
