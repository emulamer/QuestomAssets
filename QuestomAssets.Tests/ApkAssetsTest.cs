using NUnit.Framework;
using System.IO;
using QuestomAssets;
using System.Collections.Generic;
using System.Linq;
using QuestomAssets.Utils;
using QuestomAssets.Models;
using BeatmapAssetMaker;
using System;
using static QuestomAssets.BeatSaber.Extensions;
using QuestomAssets.Tests;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.Tests
{
    public class ApkAssetsTest : QuestomAssetsEngineTestBase
    {
        private const string BS_APK_FILE = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\platform-tools\1.1.0baseoriginal.apk";
        protected int TestRandomNum = (new Random()).Next(5000);
        private string _apkFile = null;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _apkFile = $"beatsaber_TESTS{TestRandomNum}.apk";
            if (!File.Exists(BS_APK_FILE))
                throw new System.Exception("Beat Saber APK file doesn't exist.  Make sure it is set to the proper location in the BS_APK_FILE constant.");
            if (!Directory.Exists(TEST_SONG_FOLDER))
                throw new System.Exception("Custom songs folder doesn't exist.  Make sure it is set in the TEST_SONG_FOLDER constant.");
            if (!File.Exists(COVER_ART_FILE))
                throw new System.Exception("Cover art file doesn't exist.  Make sure it is set in the COVER_ART_FILE constant.");

            File.Copy(BS_APK_FILE, _apkFile, true);
        }
        private List<string> testSongDirs = new List<string>();
        protected override string MakeTestSongDir()
        {
            string dir = $".\\{TestRandomNum}TestSongInstance{testSongDirs.Count}";
            FileUtils.DirectoryCopy(TEST_SONG_FOLDER, dir, true);
            testSongDirs.Add(dir);
            return dir;
        }

        [TearDown]
        public override void TearDown()
        {
            File.Delete(_apkFile);
            foreach (string testDir in testSongDirs)
                Directory.Delete(testDir, true);
            base.TearDown();
        }

        protected override QaeConfig GetQaeConfig(IFileProvider prov)
        {
            return new QaeConfig() { AssetsPath = "assets/bin/Data/", SongsPath = "", RootFileProvider = prov, SongFileProvider = new FolderFileProvider(".\\", false) };
        }

        protected override IFileProvider GetProvider()
        {
            return new ZipFileProvider(_apkFile, FileCacheMode.Memory, false);
        }
    }
}