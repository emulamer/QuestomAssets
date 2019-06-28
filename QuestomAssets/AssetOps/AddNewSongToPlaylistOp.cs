using QuestomAssets.BeatSaber;
using QuestomAssets.Models;
using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.AssetOps
{
    public class AddNewSongToPlaylistOp : AssetOp
    {
        public override bool IsWriteOp => true;

        public AddNewSongToPlaylistOp(BeatSaberSong song, string playlistID)
        {
            Song = song;
            PlaylistID = playlistID;
        }

        public string PlaylistID { get; private set; }
        public BeatSaberSong Song { get; private set; }
        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(Song.SongID))
                throw new InvalidOperationException("SongID must be set on the song!");
            if (string.IsNullOrEmpty(Song.CustomSongPath))
                throw new InvalidOperationException("CustomSongPath must be set on the song!");
            if (!context.Cache.PlaylistCache.ContainsKey(PlaylistID))
                throw new KeyNotFoundException($"PlaylistID {PlaylistID} not found in the cache!");
            if (context.Cache.SongCache.ContainsKey(Song.SongID))
                throw new InvalidOperationException($"SongID {Song.SongID} already exists!");

            BeatmapLevelDataObject level = null;
            try
            {
                var songsAssetFile = context.Engine.GetSongsAssetsFile();
                CustomLevelLoader loader = new CustomLevelLoader(songsAssetFile, context.Config);
                var deser = loader.DeserializeFromJson(Song.CustomSongPath, Song.SongID);
                level = loader.LoadSongToAsset(deser, Song.CustomSongPath, true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception loading custom song folder '{Song.CustomSongPath}' for SongID {Song.SongID}", ex);
            }

            if (level == null)
            {
                throw new Exception($"Song at folder '{Song.CustomSongPath}' for SongID {Song.SongID} failed to load");
            }

            Song.LevelData = level;
            Song.LevelAuthorName = level.LevelAuthorName;
            Song.SongAuthorName = level.SongAuthorName;
            Song.SongName = level.SongName;
            Song.SongSubName = level.SongSubName;
            var playlist = context.Cache.PlaylistCache[PlaylistID];
            playlist.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Add(Song.LevelData.PtrFrom(playlist.Playlist.BeatmapLevelCollection.Object));
            playlist.Songs.Add(Song.SongID, Song.LevelData);
            context.Cache.SongCache.Add(Song.SongID, new QuestomAssetsEngine.MusicConfigCache.SongAndPlaylist() { Playlist = playlist.Playlist, Song = Song.LevelData });
        }

    }
}
