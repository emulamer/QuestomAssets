using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestomAssets.AssetsChanger;
using static QuestomAssets.MusicConfigCache;

namespace QuestomAssets.AssetOps
{
    public class MoveSongToPlaylistOp : AssetOp
    {
        public override bool IsWriteOp => true;

        public string SongID { get; private set; }
        public string ToPlaylistID { get; private set; }

        public MoveSongToPlaylistOp(string songID, string toPlaylistID)
        {
            SongID = songID;
            ToPlaylistID = toPlaylistID;
        }

        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(SongID))
                throw new InvalidOperationException("SongID must be provided.");
            if (string.IsNullOrEmpty(ToPlaylistID))
                throw new InvalidOperationException("ToPlaylistID must be provided.");

            if (!context.Cache.SongCache.ContainsKey(SongID))
                throw new InvalidOperationException("SongID could not be found.");
            if (!context.Cache.PlaylistCache.ContainsKey(ToPlaylistID))
                throw new InvalidOperationException("ToPlaylistID could not be found.");

            var song = context.Cache.SongCache[SongID];
            var toPlaylist = context.Cache.PlaylistCache[ToPlaylistID];

            var songPtr = song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.FirstOrDefault(x => x.Object == song.Song);
            if (songPtr == null)
                throw new Exception($"Unable to find the song pointer for song id {song.Song.LevelID} in the playlist it is moving from ({song.Playlist.PackID}).");
            song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(songPtr);
            songPtr.Dispose();
            toPlaylist.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Add(song.Song.PtrFrom(toPlaylist.Playlist));

            //keep the cache updated
            context.Cache.PlaylistCache[song.Playlist.PackID].Songs.Remove(song.Song.LevelID);
            song.Playlist = toPlaylist.Playlist;
            toPlaylist.Songs.Add(song.Song.LevelID, new OrderedSong() { Song = song.Song, Order = toPlaylist.Songs.Count });
        }
    }
}
