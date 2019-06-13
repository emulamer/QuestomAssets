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
                    QuestomAssets.Utils.ImageUtils.Instance = new ImageUtilsDroid();
                    var file = info.ActivityInfo.ApplicationInfo.PublicSourceDir;
                    using (var apkFileProvider = new ApkAssetsFileProvider(file, ApkAssetsFileProvider.FileCacheMode.Memory, false, System.IO.Path.GetTempPath()))
                    {
                        QuestomAssets.QuestomAssetsEngine qae = new QuestomAssets.QuestomAssetsEngine();
                        var cfg = qae.GetCurrentConfig(apkFileProvider);
                        qae.SignAPK(apkFileProvider);
                    }
                    
                }
               

            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

   
    }
}