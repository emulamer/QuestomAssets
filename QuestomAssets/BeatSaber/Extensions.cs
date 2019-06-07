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
        private static string FindFirstOfSplit(IApkFileIO apk, string assetsFile)
        {
            int lastDot = assetsFile.LastIndexOf('.');
            if (lastDot > 0)
            {

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
            }
            if (apk.FileExists(assetsFile))
            {
                return assetsFile;
            }
            return null;

            
        }

        public static string CorrectAssetFilename(this IApkFileIO apk, string assetsFile)
        {
            var correctName = FindFirstOfSplit(apk, assetsFile);
            if (correctName != null)
                return correctName;
            //some of the files in ExternalFiles have library/ on them, but they're actually in the root path
            var splitPath = assetsFile.Split('/').ToList();
            if (splitPath.Count() > 1)
            {
                splitPath.RemoveAt(splitPath.Count - 2);
                correctName = String.Join("/", splitPath);
                correctName = FindFirstOfSplit(apk, correctName);
                if (correctName != null)
                    return correctName;
            }

            throw new ArgumentException("The file doesn't exist in the APK with any name!");
        }
        public static void WriteCombinedAssets(this IApkFileIO apk, AssetsFile assetsFile, string assetsFilePath)
        {
            if (assetsFilePath.EndsWith("split0"))
                throw new ArgumentException("Don't pass in filenames with split0, pass in the original.");
            apk.Delete(assetsFilePath + ".split*");
            using (var ws = apk.GetWriteStream(assetsFilePath, true, true))
                assetsFile.Write(ws);            
        }

        public static Stream ReadCombinedAssets(this IApkFileIO apk, string assetsFilePath)
        {
            string actualName = apk.CorrectAssetFilename(assetsFilePath);

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
