using NUnit.Framework;
using System.IO;
using QuestomAssets;
using System.Collections.Generic;
using System.Linq;
using QuestomAssets.Utils;
using BeatmapAssetMaker;

namespace Tests
{
    public class QuestomAssetsEngineTests
    {
        private const string BS_APK_FILE = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\platform-tools\1.1.0baseoriginal.apk";
        private const string TEST_SONG_FOLDER = @"TestSong";
        public const string COVER_ART_FILE = @"TestCover.png";
        private const string _apkFile = "beatsaber_TESTS.apk";
        private const string TestSongName = "Testing Song!";
        private const string TestSongSubName = "An ode to test";
        private const string TestSongAuthorName = "Emulamer";
        private const string PlaylistIDFormat = "someplaylist{0}";
        private const string PlaylistNameFormat = "A Playlist {0}";
        private string SongIDFormat = "{0:D4}-{1:D4}";

        private void SetupPlaylistsWithSongs(int playlistCount, int songCount)
        {
            BeatSaberQuestomConfig config = null;
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/");

                config = qae.GetCurrentConfig();

                for (int p = 0; p < playlistCount; p++)
                {
                    var playlist = new BeatSaberPlaylist()
                    {
                        PlaylistID = string.Format(PlaylistIDFormat, p),
                        PlaylistName = string.Format(PlaylistNameFormat, p),
                        CoverArtBytes = File.ReadAllBytes(COVER_ART_FILE)
                    };
                    for (int i = 0; i < songCount; i++)
                    {
                        var song = new BeatSaberSong()
                        {
                            SongID = string.Format(SongIDFormat, p, i),
                            CustomSongFolder = TEST_SONG_FOLDER
                        };

                        playlist.SongList.Add(song);
                    }
                    config.Playlists.Add(playlist);
                }
                qae.UpdateConfig(config);
            }
        }

        private BeatSaberQuestomConfig CopyIDs(BeatSaberQuestomConfig config)
        {
            var newConfig = new BeatSaberQuestomConfig();
            foreach (var p in config.Playlists)
            {
                var playlist = new BeatSaberPlaylist()
                {
                    PlaylistID = p.PlaylistID
                };
                foreach (var s in p.SongList)
                {
                    var song = new BeatSaberSong()
                    {
                        SongID = s.SongID
                    };
                    playlist.SongList.Add(song);
                }
                newConfig.Playlists.Add(playlist);
            }
            return newConfig;
        }

        [SetUp]
        public void Setup()
        {
            if (!File.Exists(BS_APK_FILE))
                throw new System.Exception("Beat Saber APK file doesn't exist.  Make sure it is set to the proper location in the BS_APK_FILE constant.");
            if (!Directory.Exists(TEST_SONG_FOLDER))
                throw new System.Exception("Custom songs folder doesn't exist.  Make sure it is set in the TEST_SONG_FOLDER constant.");
            if (!File.Exists(COVER_ART_FILE))
                throw new System.Exception("Cover art file doesn't exist.  Make sure it is set in the COVER_ART_FILE constant.");

            File.Copy(BS_APK_FILE, _apkFile, true);
            ImageUtils.Instance = new ImageUtilsWin();
        }

        [Test]
        public void LoadsConfig()
        {
            BeatSaberQuestomConfig config = null;
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, true))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/", true);

                config = qae.GetCurrentConfig(false);
            }
            
            Assert.IsNotNull(config, "Didn't load current config");
            Assert.Pass();
        }

        [Test]
        public void LoadsConfigImages()
        {
            SetupPlaylistsWithSongs(1, 1);
            BeatSaberQuestomConfig config = null;
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, true))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/", true);

                config = qae.GetCurrentConfig();

                Assert.IsNotNull(config, "Didn't load current config");
                Assert.IsNotNull(config.Playlists[0].CoverArtBytes);
                Assert.IsNotNull(config.Playlists[0].SongList[0].CoverArtBytes);

                //todo:
                //Assert.AreEqual(1024, config.Playlists[0].CoverArt.Width);
                //Assert.AreEqual(1024, config.Playlists[0].CoverArt.Height);
                //Assert.AreEqual(256, config.Playlists[0].SongList[0].CoverArt.Width);
                //Assert.AreEqual(256, config.Playlists[0].SongList[0].CoverArt.Height);
            }
            Assert.Pass();
        }



        [Test]
        public void AddsPlaylistWithSong()
        {
            SetupPlaylistsWithSongs(1, 1);
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, false))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/", false);

                var newConfig = qae.GetCurrentConfig(false);
                Assert.AreEqual(1, newConfig.Playlists.Count, "Playlist count should be 1!");
                var playlist = newConfig.Playlists[0];
                Assert.AreEqual(string.Format(PlaylistIDFormat, 0), playlist.PlaylistID);
                Assert.AreEqual(string.Format(PlaylistNameFormat, 0), playlist.PlaylistName);
                Assert.IsNotNull(playlist.CoverArtBytes, "Playlist cover art didn't reload!");
                //Assert.AreEqual(1024, playlist.CoverArt.Width, "Playlist cover art is not 1024 width!");
                //Assert.AreEqual(1024, playlist.CoverArt.Height, "Playlist cover art is not 1024 height!");

                Assert.AreEqual(1, playlist.SongList.Count, "Songs count should be 1!");
                var song = playlist.SongList[0];
                Assert.AreEqual(string.Format(SongIDFormat, 0, 0), song.SongID);
                Assert.AreEqual(TestSongName, song.SongName);
                Assert.AreEqual(TestSongSubName, song.SongSubName);
                Assert.AreEqual(TestSongAuthorName, song.SongAuthorName);
                Assert.IsNotNull(song.CoverArtBytes, "Cover art didn't load!");
                //Assert.AreEqual(256, song.CoverArt.Width, "Song cover art is not 256 width!");
                //Assert.AreEqual(256, song.CoverArt.Height, "Song cover art is not 256 height!");
            }
            Assert.Pass();
        }

        [Test]
        public void MovesSongToNewPlaylist()
        {
            SetupPlaylistsWithSongs(2, 2);
            BeatSaberQuestomConfig oldConfig = null;
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, false))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/", false);

                oldConfig = qae.GetCurrentConfig(false);
                var config = CopyIDs(oldConfig);
                var song = config.Playlists[0].SongList[0];
                config.Playlists[1].SongList.Add(song);
                config.Playlists[0].SongList.Remove(song);
                qae.UpdateConfig(config);
            }
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, true))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/", true);

                var testConfig = qae.GetCurrentConfig(false);
                Assert.AreEqual(2, testConfig.Playlists.Count);
                Assert.AreEqual(1, testConfig.Playlists[0].SongList.Count());
                Assert.AreEqual(3, testConfig.Playlists[1].SongList.Count());
                Assert.AreEqual(string.Format(SongIDFormat, 0, 1), testConfig.Playlists[0].SongList[0].SongID);
                Assert.AreEqual(string.Format(SongIDFormat, 1, 0), testConfig.Playlists[1].SongList[0].SongID);
                Assert.AreEqual(string.Format(SongIDFormat, 1, 1), testConfig.Playlists[1].SongList[1].SongID);
                Assert.AreEqual(string.Format(SongIDFormat, 0, 0), testConfig.Playlists[1].SongList[2].SongID);
                Assert.AreEqual(oldConfig.Playlists[0].PlaylistName, testConfig.Playlists[0].PlaylistName);
                Assert.AreEqual(oldConfig.Playlists[1].PlaylistName, testConfig.Playlists[1].PlaylistName);
            }
            Assert.Pass();
        }

        //[Test]
        //public void RemovesSong()
        //{

        //}

        //[Test]
        //public void RemovesPlaylist()
        //{

        //}

        //[Test]
        //public void ReordersPlaylist()
        //{

        //}

        [Test]
        public void DoesNotDuplicateSong()
        {
            SetupPlaylistsWithSongs(2, 2);
            BeatSaberQuestomConfig oldConfig = null;
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, false))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/", false);

                oldConfig = qae.GetCurrentConfig(false);
                var config = CopyIDs(oldConfig);
                var song = new BeatSaberSong()
                {
                    SongID = config.Playlists[0].SongList[0].SongID,
                    CustomSongFolder = TEST_SONG_FOLDER
                };
                oldConfig.Playlists[0].SongList.Add(song);
                qae.UpdateConfig(config);
            }
            using (var apk = new ApkAssetsFileProvider(_apkFile, FileCacheMode.Memory, true))
            {
                QuestomAssetsEngine qae = new QuestomAssetsEngine(apk, "assets/bin/Data/", true);

                var testConfig = qae.GetCurrentConfig(false);
                Assert.AreEqual(2, testConfig.Playlists[0].SongList.Count());
                Assert.AreEqual(2, testConfig.Playlists[1].SongList.Count());
            }
            Assert.Pass();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_apkFile);
        }
    }
}