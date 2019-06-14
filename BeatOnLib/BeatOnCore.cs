using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using QuestomAssets;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using QuestomAssets.Utils;

namespace BeatOnLib
{

    public class BeatOnCore 
    {
        private string _rootAssetsFolder;

        public BeatOnCore(string rootAssetsFolder)
        {
            _logger = new StringLogger();
            Log.SetLogSink(_logger);
            _rootAssetsFolder = rootAssetsFolder;
            _fileProvider = new FolderFileProvider(rootAssetsFolder, false);
            ImageUtils.Instance = new ImageUtilsDroid();
        }

        private StringLogger _logger;
        private IAssetsFileProvider _fileProvider;
        private QuestomAssetsEngine _engine;
        private QuestomAssetsEngine Engine
        {
            get
            {
                if (_engine == null)
                    InitEngine();

                return _engine;
            }
        }
        
        private void InitEngine()
        {
            _engine = new QuestomAssetsEngine(_fileProvider, _rootAssetsFolder, false);
        }

        public string GetLog()
        {
            return _logger.GetString();
        }
        public void ClearLog()
        {
            _logger.ClearLog();
        }
        public string GetConfigJson()
        {
            try
            {
                return JsonConvert.SerializeObject(Engine.GetCurrentConfig(false));
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception in BeatOnCore.GetConfig", ex);
            }
            return null;
        }

        public bool UpdateConfig(string configJson)
        {
            try
            {
                var config = JsonConvert.DeserializeObject<BeatSaberQuestomConfig>(configJson);
                Engine.UpdateConfig(config);
                return true;
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception in BeatOnCore.UpdateConfig", ex);
            }
            return false;
        }

        //public string Test()
        //{

        //    try
        //    {                
        //        QuestomAssets.Log.SetLogSink(l);
        //        var customSongsFolders = GetCustomSongsFromPath("/sdcard/BS/ToConvert");

        //        // QuestomAssets.Utils.ImageUtils.Instance = new ImageUtilsDroid();
        //        // var file = info.ActivityInfo.ApplicationInfo.PublicSourceDir;
        //        using (var fp = new FolderFileProvider("/sdcard/Android/data/com.beatgames.beatsaber/files/assets/", false))// file, FileCacheMode.Memory, false, System.IO.Path.GetTempPath()))
        //        {
        //            QuestomAssetsEngine qae = new QuestomAssetsEngine(fp, "");
        //            var cfg = qae.GetCurrentConfig();


        //            BeatSaberPlaylist playlist = cfg.Playlists.FirstOrDefault(x => x.PlaylistID == "CustomSongs");
        //            if (playlist == null)
        //            {
        //                Log.LogMsg("Playlist doesn't already exist, creating it");
        //                playlist = new BeatSaberPlaylist()
        //                {
        //                    PlaylistID = "CustomSongs",
        //                    PlaylistName = "Custom Songs"
        //                };
        //                cfg.Playlists.Add(playlist);
        //            }


        //            Log.LogMsg($"Attempting to load {customSongsFolders.Count} custom songs...");
        //            foreach (var cs in customSongsFolders)
        //            {
        //                playlist.SongList.Add(new BeatSaberSong()
        //                {
        //                    CustomSongFolder = cs
        //                });
        //            }
        //            Log.LogMsg("Applying new configuration...");
        //            qae.UpdateConfig(cfg);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.LogErr("ERRROR: ", ex);
        //    }

        //}


        public bool SignApk(string apkFile)
        {
            try
            {
                using (var fp = new ApkAssetsFileProvider(apkFile, FileCacheMode.Memory))
                {
                    ApkSigner signer = new ApkSigner();
                    signer.Sign(fp);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.LogErr("Error signing APK", ex);
            }
            return false;
        }

        public string GetBase64FileDataFromApk(string apkFile, string filenameToGet)
        {
            try
            {
                using (var fp = new ApkAssetsFileProvider(apkFile, FileCacheMode.Memory))
                {
                    return Convert.ToBase64String(fp.Read(filenameToGet));
                }
            }
            catch (Exception ex)
            {
                Log.LogErr($"Error getting file {filenameToGet} from apk {apkFile}", ex);
            }
            return null;
        }

        public bool SaveFileToApk(string apkFile, string filenameToSave, string base64FileData)
        {
            try
            {
                byte[] fileData = Convert.FromBase64String(base64FileData);
                using (var fp = new ApkAssetsFileProvider(apkFile, FileCacheMode.Memory))
                {
                    fp.Write(filenameToSave, fileData, true, true);
                    fp.Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.LogErr($"Error saving file {filenameToSave} to apk {apkFile}", ex);
            }
            return false;
        }


        //static List<string> GetCustomSongsFromPath(string path)
        //{
        //    List<string> customSongsFolders = new List<string>();

        //    if (File.Exists(Path.Combine(path, "Info.dat")))
        //    {
        //        //do one
        //        Log.LogErr("Found Info.dat in customSongsFolder, injecting one custom song.");
        //        customSongsFolders.Add(path);
        //    }
        //    else
        //    {
        //        //do many
        //        List<string> foundSongs = Directory.EnumerateDirectories(path).Where(y => File.Exists(Path.Combine(y, "Info.dat"))).ToList();
        //        Log.LogMsg($"Found {foundSongs.Count()} custom songs to inject");
        //        customSongsFolders.AddRange(foundSongs);
        //    }
        //    return customSongsFolders;
        //}


    }

    //something screwy with embeddenator-4000, it won't handle classes that aren't part of this namespace


   
}