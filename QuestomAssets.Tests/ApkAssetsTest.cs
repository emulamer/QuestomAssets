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

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            if (!File.Exists(BS_APK_FILE))
                throw new System.Exception("Beat Saber APK file doesn't exist.  Make sure it is set to the proper location in the BS_APK_FILE constant.");
            if (!Directory.Exists(TEST_SONG_FOLDER))
                throw new System.Exception("Custom songs folder doesn't exist.  Make sure it is set in the TEST_SONG_FOLDER constant.");
            if (!File.Exists(COVER_ART_FILE))
                throw new System.Exception("Cover art file doesn't exist.  Make sure it is set in the COVER_ART_FILE constant.");

            File.Copy(BS_APK_FILE, _apkFile, true);
        }

        [TearDown]
        public override void TearDown()
        {
            File.Delete(_apkFile);
            base.TearDown();
        }

        protected override QaeConfig GetQaeConfig(IAssetsFileProvider prov)
        {
            return new QaeConfig() { AssetsPath = "assets/bin/Data/", SongsPath = "", FileProvider = prov, SongFileProvider = new FolderFileProvider(".\\", false) };
        }

        protected override IAssetsFileProvider GetProvider()
        {
            return new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, false);
        }
    }
}