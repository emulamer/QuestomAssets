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
    public class FolderAssetsTestCombined : QuestomAssetsEngineTestBase
    {
        private const string BS_EXTRACTED_ASSETS = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\platform-tools\1.1.0baseoriginal\assets\bin\Data\";
        protected int TestRandomNum = (new Random()).Next(5000);

        [SetUp]
        public override void Setup()
        {
            if (!Directory.Exists(BS_EXTRACTED_ASSETS))
                throw new System.Exception("Beat Saber APK file doesn't exist.  Make sure it is set to the proper location in the BS_APK_FILE constant.");
            if (!Directory.Exists(TEST_SONG_FOLDER))
                throw new System.Exception("Custom songs folder doesn't exist.  Make sure it is set in the TEST_SONG_FOLDER constant.");
            if (!File.Exists(COVER_ART_FILE))
                throw new System.Exception("Cover art file doesn't exist.  Make sure it is set in the COVER_ART_FILE constant.");

            FileUtils.DirectoryCopy(BS_EXTRACTED_ASSETS, $".\\TestAssets{TestRandomNum}", true);
            if (!Directory.Exists(ModLibTestFolder))
                Directory.CreateDirectory(ModLibTestFolder);

            base.Setup();
        }

        private List<string> testSongDirs = new List<string>();
        protected override string MakeTestSongDir()
        {
            string dir = $".\\{TestRandomNum}TestSongInstance{testSongDirs.Count}";
            FileUtils.DirectoryCopy(TEST_SONG_FOLDER, dir, true);
            testSongDirs.Add(dir);
            return dir;
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
            Directory.Delete($".\\TestAssets{TestRandomNum}",true);
            foreach (string testDir in testSongDirs)
                if (Directory.Exists(testDir))
                    Directory.Delete(testDir, true);
            base.TearDown();
        }

        protected override QaeConfig GetQaeConfig(IFileProvider prov)
        {
            return new QaeConfig() { AssetsPath = "", SongsPath = "", RootFileProvider = prov, SongFileProvider = new FolderFileProvider(".\\", false), ModLibsFileProvider = new FolderFileProvider(ModLibTestFolder, false, false) };
        }

        protected override IFileProvider GetProvider()
        {
            return new FolderFileProvider($".\\TestAssets{TestRandomNum}\\", false, true);
        }

    }
}
