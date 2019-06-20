using BeatmapAssetMaker;
using NUnit.Framework;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Models;
using QuestomAssets.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace QuestomAssets.Tests
{
    public class FolderAssetsTestNotCombined : QuestomAssetsEngineTestBase
    {
        private const string BS_EXTRACTED_ASSETS = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\platform-tools\1.1.0baseoriginal\assets\bin\Data\";

        [SetUp]
        public override void Setup()
        {
            if (!Directory.Exists(BS_EXTRACTED_ASSETS))
                throw new System.Exception("Beat Saber APK file doesn't exist.  Make sure it is set to the proper location in the BS_APK_FILE constant.");
            if (!Directory.Exists(TEST_SONG_FOLDER))
                throw new System.Exception("Custom songs folder doesn't exist.  Make sure it is set in the TEST_SONG_FOLDER constant.");
            if (!File.Exists(COVER_ART_FILE))
                throw new System.Exception("Cover art file doesn't exist.  Make sure it is set in the COVER_ART_FILE constant.");

            DirectoryCopy(BS_EXTRACTED_ASSETS, ".\\TestAssets", true);

            base.Setup();
        }

        //[Test]
        //public void CombinedStreamWorks()
        //{

        //    List<string> files = Directory.GetFiles(BS_EXTRACTED_ASSETS, "sharedassets18.assets.split*").OrderBy(x => Convert.ToInt32(x.Split(new string[] { ".split" }, StringSplitOptions.None).Last())).ToList();
        //    int totalLen = (int)files.Sum(x => new FileInfo(x).Length);


        //    byte[] bActual8 = new byte[8];
        //    using (var fs = File.OpenRead(files[0]))
        //    {
        //        fs.Seek(1048572, SeekOrigin.Begin);
        //        fs.Read(bActual8, 0, 4);
        //    }
        //    using (var fs = File.OpenRead(files[1]))
        //    {
        //        fs.Read(bActual8, 4, 4);
        //    }

        //    int numFiles = 3;
        //    byte[] bActualBig = new byte[1024 * 1024 * numFiles];
        //    int actualCtr = 0;
        //    for (int i = 0; i < numFiles; i++)
        //    {
        //        using (var fs = File.OpenRead(files[i]))
        //        {
        //            actualCtr += fs.Read(bActualBig, actualCtr, (int)fs.Length);
        //        }
        //    }

        //    CombinedStream cs = new CombinedStream(files);
        //    Assert.AreEqual(totalLen, cs.Length, "Combined length isn't right.");
        //    cs.Seek(1048572, SeekOrigin.Begin);
        //    Assert.AreEqual(1048572, cs.Position, "Seek position isn't right!");
        //    byte[] bRead = new byte[8];
        //    int actualRead = cs.Read(bRead, 0, 8);
        //    Assert.AreEqual(8, actualRead, "Cross file read didn't return the right amount of bytes.");
        //    Assert.AreEqual(1048580, cs.Position, "Read didn't advance position properly reading across split files.");
        //    Assert.AreEqual(bActual8, bRead, "Cross file read data didn't match.");
        //    byte[] bBigRead = new byte[bActualBig.Length];
        //    cs.Seek(0, SeekOrigin.Begin);
        //    actualRead = cs.Read(bBigRead, 0, bBigRead.Length);
        //    Assert.AreEqual(bActualBig.Length, actualRead, "Big multi cross file read didn't return the correct value from Read.");
        //    Assert.AreEqual(bActualBig, bBigRead, "Big multi cross file read data didn't match expected.");

        //    Assert.Pass();
        //}

        [TearDown]
        public override void TearDown()
        {
            Directory.Delete(".\\TestAssets",true);
            base.TearDown();
        }

        protected override QaeConfig GetQaeConfig(IAssetsFileProvider prov)
        {
            return new QaeConfig() { AssetsPath = "", SongsPath = "", FileProvider = prov, SongFileProvider = new FolderFileProvider(".\\", false) };
        }

        protected override IAssetsFileProvider GetProvider()
        {
            return new FolderFileProvider(".\\TestAssets\\", false, false);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
