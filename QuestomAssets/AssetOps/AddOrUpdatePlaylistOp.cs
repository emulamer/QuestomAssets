using QuestomAssets.BeatSaber;
using QuestomAssets.Models;
using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.AssetsChanger;
using static QuestomAssets.MusicConfigCache;

namespace QuestomAssets.AssetOps
{
    public class AddOrUpdatePlaylistOp : AssetOp
    {
        public override bool IsWriteOp => true;

        public AddOrUpdatePlaylistOp(BeatSaberPlaylist playlist)
        {
            Playlist = playlist;
        }

        public BeatSaberPlaylist Playlist { get; private set; }
        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(Playlist.PlaylistID))
                throw new Exception("PlaylistID must be provided.");

            if (string.IsNullOrEmpty(Playlist.PlaylistName))
                throw new Exception($"PlaylistName must be provided for ID {Playlist.PlaylistName}");

            var songsAssetFile = context.Engine.GetSongsAssetsFile();
            BeatmapLevelPackObject levelPack = null;
            if (context.Cache.PlaylistCache.ContainsKey(Playlist.PlaylistID))
            {
                Log.LogMsg($"Playlist {Playlist.PlaylistID} will be updated");
                levelPack = context.Cache.PlaylistCache[Playlist.PlaylistID].Playlist;
                levelPack.PackName = Playlist.PlaylistName;
                Playlist.LevelPackObject = levelPack;
                OpCommon.UpdateCoverImage(Playlist, context, songsAssetFile);
            }
            else
            {
                levelPack = OpCommon.CreatePlaylist(context, Playlist, songsAssetFile);
            }
            
        }

        

    }
}
