using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class DeletePlaylistOp : AssetOp
    {
        public DeletePlaylistOp(string playlistID, bool deleteSongsOnPlaylist = true)
        {
            PlaylistID = playlistID;
            DeleteSongsOnPlaylist = deleteSongsOnPlaylist;
        }

        public string PlaylistID { get; private set; }
        public bool DeleteSongsOnPlaylist { get; private set; }
        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(PlaylistID))
                throw new InvalidOperationException("Playlist ID must be provided.");

            if (!context.Cache.PlaylistCache.ContainsKey(PlaylistID))
                throw new InvalidOperationException("Playlist ID does not exist.");

            var playlist = context.Cache.PlaylistCache[PlaylistID];

            if (DeleteSongsOnPlaylist)
            {
                try
                {
                    foreach (var song in playlist.Songs.ToList())
                    {
                        OpCommon.DeleteSong(context, song.Key);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while deleting songs from playlist id {playlist.Playlist.PackID}", ex);
                }
                playlist.Songs.Clear();
            }
            //this should be done in song delete already
            //playlist.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.ForEach(x => { x.Target.ParentFile.DeleteObject(x.Object); x.Dispose(); });
            var mlp = context.Engine.GetMainLevelPack();
            var aop = context.Engine.GetAlwaysOwnedModel();
            var mlpptr = mlp.BeatmapLevelPacks.FirstOrDefault(x => x.Object.PackID == playlist.Playlist.PackID);
            var aopptr = aop.AlwaysOwnedPacks.FirstOrDefault(x => x.Object.PackID == playlist.Playlist.PackID);
            if (mlpptr != null)
            {
                mlp.BeatmapLevelPacks.Remove(mlpptr);
                mlpptr.Dispose();
            }
            else
            {
                Log.LogErr($"The playlist id {playlist.Playlist.PackID} didn't exist in the main level packs");
            }
            if (aopptr != null)
            {
                aop.AlwaysOwnedPacks.Remove(aopptr);
                aopptr.Dispose();
            }
            else
            {
                Log.LogErr($"The playlist id {playlist.Playlist.PackID} didn't exist in the always owned level packs");
            }
            var plParent = playlist.Playlist.ObjectInfo.ParentFile;
            plParent.DeleteObject(playlist.Playlist.CoverImage.Object.Texture.Object);
            playlist.Playlist.CoverImage.Object.Texture.Dispose();
            plParent.DeleteObject(playlist.Playlist.CoverImage.Object);
            plParent.DeleteObject(playlist.Playlist);
            playlist.Playlist.CoverImage.Dispose();
            context.Cache.PlaylistCache.Remove(PlaylistID);
            

        }
    }
}
