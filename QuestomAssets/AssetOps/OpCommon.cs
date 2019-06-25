using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetOps
{
    internal static class OpCommon
    {
        public static void DeleteSong(OpContext context, string songID)
        {
            var songsAssetFile = context.Engine.GetSongsAssetsFile();
            var song = context.Cache.SongCache[songID];
            var songPtr = song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.FirstOrDefault(x => x.Object == song.Song);
            if (songPtr == null)
                throw new Exception("Song pointer could not be found in the playlist.");

            var sourceFile = song.Song?.AudioClip?.Object?.Resource?.Source;

            song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(songPtr);
            songPtr.Dispose();
            songsAssetFile.DeleteObject(song.Song.AudioClip.Object);
            songsAssetFile.DeleteObject(song.Song.CoverImageTexture2D.Object);
            song.Song.CoverImageTexture2D.Dispose();
            song.Song.DifficultyBeatmapSets.ForEach(x =>
            {
                x.DifficultyBeatmaps.ForEach(y =>
                {
                    songsAssetFile.DeleteObject(y.BeatmapDataPtr.Object);
                    y.BeatmapDataPtr.Dispose();
                });
                x.BeatmapCharacteristic.Dispose();
            });
            songsAssetFile.DeleteObject(song.Song);
            context.Cache.SongCache.Remove(song.Song.LevelID);
            context.Cache.PlaylistCache[song.Playlist.PackID].Songs.Remove(song.Song.LevelID);

            //deleting the song folders is going to have to be done some other way, like a "clean up" in tools.
            //if (sourceFile == null)
            //{
            //    Log.LogErr($"Unable to find audio clip location while deleting song ID {song.Song.LevelID}");
            //}
            //else
            //{
            //    if (!sourceFile.Contains("/"))
            //    {
            //        Log.LogErr($"Attempting to delete Song ID {song.Song.LevelID} but the audio file '{sourceFile}' does not appear to be in a nested directory.  Not going to delete it.");
            //    }
            //    else
            //    {
            //        if (!context.Config.SongFileProvider.FileExists(sourceFile))
            //        {
            //            Log.LogErr($"Attempting to delete Song ID {song.Song.LevelID} but the audio file '{sourceFile}' doesn't exist where it should.");
            //        }
            //        else
            //        {
            //            var rootSongPath = sourceFile.Substring(0, sourceFile.LastIndexOf("/"));
            //            Log.LogMsg($"Attempting to delete the directory '{rootSongPath}'");
            //            try
            //            {
            //                context.Config.SongFileProvider.RmRfDir(rootSongPath);
            //            }
            //            catch (Exception ex)
            //            {
            //                Log.LogErr($"Failed to delete song directory '{rootSongPath}' for Song ID {song.Song.LevelID}", ex);
            //            }
            //        }
            //    }
            //}
        }
    }
}
