using Emulamer.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.AssetsChanger;
using System.IO;
using System.Linq;

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

        public static Stream ReadCombinedAssets(this Apkifier apk, string assetsFilePath)
        {
            string actualName = apk.FindFirstOfSplit(assetsFilePath);

            List<string> assetFiles = new List<string>();
            if (actualName.ToLower().EndsWith("split0"))
            {
                assetFiles.AddRange(apk.FindFiles(actualName.Replace(".split0", ".split*"))
                    .OrderBy(x => Convert.ToInt32(x.Split(new string[] { ".split" }, StringSplitOptions.None).Last())));
            }
            else
            {
                return apk.Read(actualName).ToStream();
            }
            MemoryStream msFullFile = new MemoryStream();
            foreach (string assetsFile in assetFiles)
            {
                byte[] fileBytes = apk.Read(assetsFile);
                msFullFile.Write(fileBytes, 0, fileBytes.Length);
            }

            return msFullFile;
        }

    }
}
