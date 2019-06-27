using BeatmapAssetMaker;
using NUnit.Framework;
using QuestomAssets.AssetOps;
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
    [NonParallelizable]
    public abstract class QuestomAssetsEngineTestBase
    {
        public const string TEST_SONG_FOLDER = @"TestSong";
        public const string COVER_ART_FILE = @"TestCover.png";
        
        public const string TestSongName = "Testing Song!";
        public const string TestSongSubName = "An ode to test";
        public const string TestSongAuthorName = "Emulamer";
        public const string PlaylistIDFormat = "someplaylist{0}";
        public const string PlaylistNameFormat = "A Playlist {0}";
        public string SongIDFormat = "{0:D4}-{1:D4}";


        protected abstract string MakeTestSongDir();

        public class DebugLogger : ILog
        {
            public void LogErr(string message, Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{message} {ex.Message} {ex.StackTrace} {ex?.InnerException?.Message} {ex?.InnerException?.StackTrace}");
            }

            public void LogErr(string message, params object[] args)
            {
                System.Diagnostics.Debug.WriteLine(String.Format(message, args));
            }

            public void LogMsg(string message, params object[] args)
            {
                string logStr = message;
                if (args.Length > 0 && !(args[0] is object[] && ((object[])args[0]).Length < 1))
                {
                    try
                    {
                        logStr = string.Format(logStr, args);
                    }
                    catch
                    { }
                }
                System.Diagnostics.Debug.WriteLine(logStr);
            }
        }
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
                                CustomSongPath = MakeTestSongDir()
                            };

                            playlist.SongList.Add(song);
                        }
                        config.Playlists.Add(playlist);
                    }
                    qae.UpdateConfig(config);
                    qae.Save();
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
            Log.ClearLogSinks();
            Log.SetLogSink(new DebugLogger());
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
                    qae.Save();
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

        [Test]
        public void BasicAddNewSongOpWorks()
        {
            SetupPlaylistsWithSongs(1, 1);

            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    var song = new BeatSaberSong()
                    {
                        SongID = "TESTSONG2",
                        CustomSongPath = MakeTestSongDir()
                    };
                    bool calledStatusChangeStarted = false;
                    bool calledStatusChangeComplete = false;
                    var newSongOp = new AddNewSongToPlaylistOp(song, "someplaylist0");
                    qae.OpManager.OpStatusChanged += (sender, op) =>
                     {
                         if (op.Status == OpStatus.Started)
                             calledStatusChangeStarted = true;
                         if (op.Status == OpStatus.Complete)
                             calledStatusChangeComplete = true;
                     };
                    qae.OpManager.QueueOp(newSongOp);
                    while (qae.OpManager.IsProcessing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    //give it an extra bit of time to make sure events get fired
                    System.Threading.Thread.Sleep(200);
                    Assert.IsTrue(calledStatusChangeStarted, "Did not get OpStatusChanged event for status Started!");
                    Assert.IsTrue(calledStatusChangeComplete, "Did not get OpStatusChanged event for status Complete!");
                    qae.Save();
                }
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    var cfg = qae.GetCurrentConfig();

                    var song = cfg.Playlists.FirstOrDefault(x => x.PlaylistID == "someplaylist0")?.SongList?.FirstOrDefault(x => x.SongID == "TESTSONG2");
                    Assert.NotNull(song, "Couldn't find the song the op was supposed to add!");
                    //todo: more tests on this

                }
                Assert.Pass();
            }
        }

        [Test]
        public void BasicAddPlaylistOpWorks()
        {
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    var newPlaylist = new BeatSaberPlaylist()
                    {
                        PlaylistID = "TESTPLAYLIST1",
                        PlaylistName = "Test Playlist 1"
                    };

                    bool calledStatusChangeStarted = false;
                    bool calledStatusChangeComplete = false;
                    var newSongOp = new AddOrUpdatePlaylistOp(newPlaylist);
                    qae.OpManager.OpStatusChanged += (sender, op) =>
                    {
                        if (op.Status == OpStatus.Started)
                            calledStatusChangeStarted = true;
                        if (op.Status == OpStatus.Complete)
                            calledStatusChangeComplete = true;
                    };
                    qae.OpManager.QueueOp(newSongOp);
                    while (qae.OpManager.IsProcessing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Assert.IsTrue(calledStatusChangeStarted, "Did not get OpStatusChanged event for status Started!");
                    Assert.IsTrue(calledStatusChangeComplete, "Did not get OpStatusChanged event for status Complete!");
                    qae.Save();
                }
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    var cfg = qae.GetCurrentConfig();

                    var playlist = cfg.Playlists.FirstOrDefault(x => x.PlaylistID == "TESTPLAYLIST1");
                    Assert.NotNull(playlist, "Couldn't find the song the op was supposed to add!");
                    Assert.AreEqual("Test Playlist 1", playlist.PlaylistName, "Playlist name was not set correctly!");
                    //todo: more tests on this

                }
                Assert.Pass();
            }
        }

        [Test]
        public void BasicDeleteSongOpWorks()
        {
            SetupPlaylistsWithSongs(5, 5);
            using (var fp = GetProvider())
            {
                var q = GetQaeConfig(fp);
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {

                    bool calledStatusChangeStarted = false;
                    bool calledStatusChangeComplete = false;
                    var deleteSongOp = new DeleteSongOp(string.Format(SongIDFormat, 2, 2));
                    qae.OpManager.OpStatusChanged += (sender, op) =>
                    {
                        if (op.Status == OpStatus.Started)
                            calledStatusChangeStarted = true;
                        if (op.Status == OpStatus.Complete)
                            calledStatusChangeComplete = true;
                    };
                    qae.OpManager.QueueOp(deleteSongOp);
                    while (qae.OpManager.IsProcessing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    Assert.IsTrue(calledStatusChangeStarted, "Did not get OpStatusChanged event for status Started!");
                    Assert.IsTrue(calledStatusChangeComplete, "Did not get OpStatusChanged event for status Complete!");
                    qae.Save();
                }
                using (QuestomAssetsEngine qae = new QuestomAssetsEngine(q))
                {
                    var cfg = qae.GetCurrentConfig();

                    Assert.AreEqual(5, cfg.Playlists.Count, "Playlist count is incorrect after song delete");
                    Assert.AreEqual(5, cfg.Playlists[0].SongList.Count, "Song came out of the wrong playlist");
                    Assert.AreEqual(5, cfg.Playlists[1].SongList.Count, "Song came out of the wrong playlist");
                    Assert.AreEqual(4, cfg.Playlists[2].SongList.Count, "Song did not come out of the right playlist");
                    Assert.AreEqual(5, cfg.Playlists[3].SongList.Count, "Song came out of the wrong playlist");
                    Assert.AreEqual(5, cfg.Playlists[4].SongList.Count, "Song came out of the wrong playlist");

                    var pl = cfg.Playlists[2];
                    Assert.IsFalse(pl.SongList.Any(x => x.SongID == string.Format(SongIDFormat, 2, 2)), "Expected target song to be deleted.");
                    
                    //todo: more tests on this

                }
                Assert.Pass();
            }
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
                        CustomSongPath = MakeTestSongDir()
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
