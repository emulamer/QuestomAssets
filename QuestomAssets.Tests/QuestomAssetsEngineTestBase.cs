using BeatmapAssetMaker;
using NUnit.Framework;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Models;
using QuestomAssets.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace QuestomAssets.Tests
{
    public abstract class QuestomAssetsEngineTestBase
    {
        public const string TEST_SONG_FOLDER = @"TestSong";
        public const string COVER_ART_FILE = @"TestCover.png";
        public const string _apkFile = "beatsaber_TESTS.apk";
        public const string TestSongName = "Testing Song!";
        public const string TestSongSubName = "An ode to test";
        public const string TestSongAuthorName = "Emulamer";
        public const string PlaylistIDFormat = "someplaylist{0}";
        public const string PlaylistNameFormat = "A Playlist {0}";
        public string SongIDFormat = "{0:D4}-{1:D4}";

        protected void SetupPlaylistsWithSongs(int playlistCount, int songCount)
        {
            BeatSaberQuestomConfig config = null;
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    config = qae.GetCurrentConfig();

                    for (int p = 0; p < playlistCount; p++)
                    {
                        var playlist = new BeatSaberPlaylist()
                        {
                            PlaylistID = string.Format(PlaylistIDFormat, p),
                            PlaylistName = string.Format(PlaylistNameFormat, p)

                        };
                        for (int i = 0; i < songCount; i++)
                        {
                            var song = new BeatSaberSong()
                            {
                                SongID = string.Format(SongIDFormat, p, i),
                                CustomSongPath = TEST_SONG_FOLDER
                            };

                            playlist.SongList.Add(song);
                        }
                        config.Playlists.Add(playlist);
                    }
                    qae.UpdateConfig(config);
                }
            }
        }

        protected BeatSaberQuestomConfig CopyIDs(BeatSaberQuestomConfig config)
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
        public virtual void Setup()
        {

            ImageUtils.Instance = new ImageUtilsWin();
        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        protected abstract QaeConfig GetQaeConfig(IAssetsFileProvider prov);

        protected abstract IAssetsFileProvider GetProvider();

        [Test]
        public void LoadsConfig()
        {
            BeatSaberQuestomConfig config = null;
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    config = qae.GetCurrentConfig();
                }
            }

            Assert.IsNotNull(config, "Didn't load current config");
            Assert.Pass();
        }

        [Test]
        public void LoadsConfigImages()
        {
            SetupPlaylistsWithSongs(1, 1);
            BeatSaberQuestomConfig config = null;
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {

                    config = qae.GetCurrentConfig();

                    Assert.IsNotNull(config, "Didn't load current config");
                    // Assert.IsNotNull(config.Playlists[0].CoverArtBytes);
                    //  Assert.IsNotNull(config.Playlists[0].SongList[0].CoverArtBytes);

                    //todo:
                    //Assert.AreEqual(1024, config.Playlists[0].CoverArt.Width);
                    //Assert.AreEqual(1024, config.Playlists[0].CoverArt.Height);
                    //Assert.AreEqual(256, config.Playlists[0].SongList[0].CoverArt.Width);
                    //Assert.AreEqual(256, config.Playlists[0].SongList[0].CoverArt.Height);
                }
            }
            Assert.Pass();
        }

        [Test]
        public void AddsPlaylistWithSong()
        {
            SetupPlaylistsWithSongs(1, 1);
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {

                    var newConfig = qae.GetCurrentConfig();
                    Assert.AreEqual(1, newConfig.Playlists.Count, "Playlist count should be 1!");
                    var playlist = newConfig.Playlists[0];
                    Assert.AreEqual(string.Format(PlaylistIDFormat, 0), playlist.PlaylistID);
                    Assert.AreEqual(string.Format(PlaylistNameFormat, 0), playlist.PlaylistName);
                    //  Assert.IsNotNull(playlist.CoverArtBytes, "Playlist cover art didn't reload!");
                    //Assert.AreEqual(1024, playlist.CoverArt.Width, "Playlist cover art is not 1024 width!");
                    //Assert.AreEqual(1024, playlist.CoverArt.Height, "Playlist cover art is not 1024 height!");

                    Assert.AreEqual(1, playlist.SongList.Count, "Songs count should be 1!");
                    var song = playlist.SongList[0];
                    Assert.AreEqual(string.Format(SongIDFormat, 0, 0), song.SongID);
                    Assert.AreEqual(TestSongName, song.SongName);
                    Assert.AreEqual(TestSongSubName, song.SongSubName);
                    Assert.AreEqual(TestSongAuthorName, song.SongAuthorName);
                    //   Assert.IsNotNull(song.CoverArtBytes, "Cover art didn't load!");
                    //Assert.AreEqual(256, song.CoverArt.Width, "Song cover art is not 256 width!");
                    //Assert.AreEqual(256, song.CoverArt.Height, "Song cover art is not 256 height!");
                }
            }
            Assert.Pass();
        }

        [Test]
        public void MovesSongToNewPlaylist()
        {
            SetupPlaylistsWithSongs(2, 2);
            BeatSaberQuestomConfig oldConfig = null;
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {

                    oldConfig = qae.GetCurrentConfig();
                    var config = CopyIDs(oldConfig);
                    var song = config.Playlists[0].SongList[0];
                    config.Playlists[1].SongList.Add(song);
                    config.Playlists[0].SongList.Remove(song);
                    qae.UpdateConfig(config);
                }
            }
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    var testConfig = qae.GetCurrentConfig();
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
            }
            Assert.Pass();
        }

        [Test]
        public void ThreadSafeImageReadTest()
        {
            SetupPlaylistsWithSongs(5, 5);
            int tnum = 0;
            object tlock = new object();
            ParameterizedThreadStart pts = new ParameterizedThreadStart((x) =>
            {
                int mynum;
                lock (tlock)
                {
                    mynum = tnum++;
                }
                var tqae = x as QuestomAssetsEngine;
                var tcfg = tqae.GetCurrentConfig();
                if (mynum % 4 == 0)
                {
                    for (int i = 0; i < tcfg.Playlists.Count; i++)
                    {
                        var png = tcfg.Playlists[i].TryGetCoverPngBytes();

                    }
                }
                else if (mynum % 3 == 0)
                {
                    for (int i = tcfg.Playlists.Count-1; i >=0; i--)
                    {
                        var png = tcfg.Playlists[i].TryGetCoverPngBytes();
                    }
                }
                else
                {
                    for (int i = 0; i < 20; i++)
                    {
                        tcfg = tqae.GetCurrentConfig();
                    }
                }
            });

            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    List<Thread> threads = new List<Thread>();
                    for (int i = 0; i < 16; i++)
                    {
                        Thread t = new Thread(pts);
                        threads.Add(t);
                    }
                    threads.ForEach(x => x.Start(qae));

                    while (threads.Any(x=> x.ThreadState == ThreadState.Running))
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                   
                }
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
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {

                    oldConfig = qae.GetCurrentConfig();
                    var config = CopyIDs(oldConfig);
                    var song = new BeatSaberSong()
                    {
                        SongID = config.Playlists[0].SongList[0].SongID,
                        CustomSongPath = TEST_SONG_FOLDER
                    };
                    oldConfig.Playlists[0].SongList.Add(song);
                    qae.UpdateConfig(config);
                }
            }
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    var testConfig = qae.GetCurrentConfig();
                    Assert.AreEqual(2, testConfig.Playlists[0].SongList.Count());
                    Assert.AreEqual(2, testConfig.Playlists[1].SongList.Count());
                }
            }
            Assert.Pass();
        }
    }
}
