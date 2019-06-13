using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Content.PM;
using System.Collections.Generic;
using Android.Content;
using Java.Interop;
using QuestomAssets;
using System.IO;
using System.Linq;
using System;

namespace BeatOn
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Button button = FindViewById<Button>(Resource.Id.button1);
            button.Click += Button_Click;
        }

        private void Button_Click(object sender, System.EventArgs e)
        {

            Intent mainIntent = new Intent(Intent.ActionMain, null);
  
            var apps = PackageManager.QueryIntentActivities(mainIntent, 0);
            foreach (var info in apps)
            {
                if (info.ActivityInfo.TaskAffinity == "com.beatgames.beatsaber")
                {
                    var customSongsFolders = GetCustomSongsFromPath("/sdcard/BS/ToConvert");

                    QuestomAssets.Utils.ImageUtils.Instance = new ImageUtilsDroid();
                    var file = info.ActivityInfo.ApplicationInfo.PublicSourceDir;
                    using (var fp = new FolderFileProvider("/sdcard/Android/data/com.beatgames.beatsaber/files/assets", false))// file, FileCacheMode.Memory, false, System.IO.Path.GetTempPath()))
                    {
                        QuestomAssets.QuestomAssetsEngine qae = new QuestomAssets.QuestomAssetsEngine();
                        var cfg = qae.GetCurrentConfig(fp, "/sdcard/Android/data/com.beatgames.beatsaber/files/assets");

                        
                        BeatSaberPlaylist playlist = cfg.Playlists.FirstOrDefault(x => x.PlaylistID == "CustomSongs");
                        if (playlist == null)
                        {
                            Log.LogMsg("Playlist doesn't already exist, creating it");
                            playlist = new BeatSaberPlaylist()
                            {
                                PlaylistID = "CustomSongs",
                                PlaylistName = "Custom Songs"
                            };
                            cfg.Playlists.Add(playlist);
                        }

                       
                        Log.LogMsg($"Attempting to load {customSongsFolders.Count} custom songs...");
                        foreach (var cs in customSongsFolders)
                        {
                            playlist.SongList.Add(new BeatSaberSong()
                            {
                                CustomSongFolder = cs
                            });
                        }
                        Log.LogMsg("Applying new configuration...");
                        qae.UpdateConfig(cfg, fp, "/sdcard/Android/data/com.beatgames.beatsaber/files/assets");
                    }

                }
               

            }
        }

        static List<string> GetCustomSongsFromPath(string path)
        {
            List<string> customSongsFolders = new List<string>();

            if (File.Exists(Path.Combine(path, "Info.dat")))
            {
                //do one
                Log.LogErr("Found Info.dat in customSongsFolder, injecting one custom song.");
                customSongsFolders.Add(path);
            }
            else
            {
                //do many
                List<string> foundSongs = Directory.EnumerateDirectories(path).Where(y => File.Exists(Path.Combine(y, "Info.dat"))).ToList();
                Log.LogMsg($"Found {foundSongs.Count()} custom songs to inject");
                customSongsFolders.AddRange(foundSongs);
            }
            return customSongsFolders;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

   
    }
}