using BeatmapAssetMaker.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatmapAssetMaker.BeatSaber
{
    class MiscUtils
    {

        public static Dictionary<Guid, Type> GetKnownAssetTypes()
        {
            Dictionary<Guid, Type> scriptHashToTypes = new Dictionary<Guid, Type>();
            scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelPackScriptHash, typeof(BeatSaber.BeatmapLevelPackObject));
            scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelCollectionScriptHash, typeof(BeatSaber.BeatmapLevelCollectionObject));
            scriptHashToTypes.Add(AssetsConstants.ScriptHash.MainLevelsCollectionHash, typeof(BeatSaber.MainLevelPackCollectionObject));
            scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapDataHash, typeof(BeatmapDataObject));
            scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelDataHash, typeof(BeatmapLevelDataObject));
            return scriptHashToTypes;
        }

        //string playlist = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\Playlists\SongBrowserPluginFavorites.json";
        //string customSongsFolder2 = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\CustomSongs";
        //string copyToFolder = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\ToConvert";
        //dynamic favs = JObject.Parse(File.ReadAllText(playlist));

        //JArray songs = favs["songs"] as JArray;
        //var songsToCopy = (from x in songs select x["key"].Value<string>()).ToList();
        //foreach (var key in songsToCopy)
        //{
        //    if (string.IsNullOrWhiteSpace(key))
        //        continue;
        //    string songPath = Path.Combine(customSongsFolder2, key) +"\\";
        //    if (!Directory.Exists(songPath))
        //    {
        //        Console.WriteLine($"Missing custom song: {songPath}");
        //        continue;
        //    }
        //    try
        //    {
        //        new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory(songPath, copyToFolder);
        //    } catch
        //    {
        //        Console.WriteLine($"Error copying {songPath}");
        //    }
        //}
        //return;
    }
}
