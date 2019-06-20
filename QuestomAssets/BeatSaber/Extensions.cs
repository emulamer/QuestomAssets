using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.AssetsChanger;
using System.IO;
using System.Linq;
using QuestomAssets.Utils;

namespace QuestomAssets.BeatSaber
{
    public static class Extensions
    {
        private static string FindFirstOfSplit(IAssetsFileProvider fp, string assetsFile)
        {
            int lastDot = assetsFile.LastIndexOf('.');
            if (lastDot > 0)
            {
                string afterDot = assetsFile.Substring(lastDot, assetsFile.Length - lastDot);
                string noSplit;
                if (afterDot.ToLower().StartsWith(".split"))
                {
                    noSplit = assetsFile.Substring(0, lastDot);
                    if (fp.FileExists(noSplit))
                        return noSplit;
                }
                else
                {
                    noSplit = assetsFile;
                }
                if (fp.FileExists(noSplit))
                    return noSplit;

                var split0 = noSplit + ".split0";
                if (fp.FileExists(split0))
                    return split0;
            }
            else if (fp.FileExists(assetsFile))
            {
                return assetsFile;
            }
            else if (fp.FileExists(assetsFile + ".split0"))
            {
                return assetsFile + ".split0";
            }

            return null;            
        }

        public static string CorrectAssetFilename(this IAssetsFileProvider fp, string assetsFile)
        {
            var correctName = FindFirstOfSplit(fp, assetsFile);
            if (correctName != null)
                return correctName;

            //some of the files in ExternalFiles have library/ on them, but they're actually in Resources/
            if ( assetsFile.Contains("library/"))
            {
                string whyUnity = assetsFile.Replace("library/", "Resources/");
                correctName = FindFirstOfSplit(fp, whyUnity);
                if (correctName != null)
                    return correctName;

                //whyUnity = assetsFile.Replace("library/", "");
                //correctName = FindFirstOfSplit(fp, whyUnity);
                //if (correctName != null)
                //    return correctName;

                //whyUnity = assetsFile.Replace("/library/", "");
                //correctName = FindFirstOfSplit(fp, whyUnity);
                //if (correctName != null)
                //    return correctName;
            }

            //some of the files in ExternalFiles have library/ on them, but they're actually in the root path
            var splitPath = assetsFile.Split('/').ToList();
            if (splitPath.Count() > 1)
            {
                splitPath.RemoveAt(splitPath.Count - 2);
                correctName = String.Join("/", splitPath);
                correctName = FindFirstOfSplit(fp, correctName);
                if (correctName != null)
                    return correctName;
            }

            


            throw new ArgumentException($"The assets file {assetsFile} doesn't exist in with any known name variations!");
        }

        public static Stream ReadCombinedAssets(this IAssetsFileProvider fp, string assetsFilePath, out bool wasCombined)
        {
            string actualName = fp.CorrectAssetFilename(assetsFilePath);

            List<string> assetFiles = new List<string>();
            if (actualName.ToLower().EndsWith("split0"))
            {
                assetFiles.AddRange(fp.FindFiles(actualName.Replace(".split0", ".split*"))
                    .OrderBy(x => Convert.ToInt32(x.Split(new string[] { ".split" }, StringSplitOptions.None).Last())));
            }
            else
            {
                wasCombined = false;
                return fp.GetReadStream(actualName);
            }
            wasCombined = true;
            //TODO: property or something on the file provider interface letting this code know if it should use the combined stream
            //      I think combined stream may perform horribly on zip files or cause other issues.

            if (true)
            {
                return new CombinedStream(assetFiles, fp);
            }
            else
            {
                MemoryStream msFullFile = new MemoryStream();
                foreach (string assetsFile in assetFiles)
                {
                    byte[] fileBytes = fp.Read(assetsFile);
                    msFullFile.Write(fileBytes, 0, fileBytes.Length);
                }

                return msFullFile;
            }
        }

    }
}
