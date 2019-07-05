using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class MoveSongInPlaylistOp : AssetOp
    {
        public MoveSongInPlaylistOp(string songID, int index)
        {
            SongID = songID;
            Index = index;
        }

        public override bool IsWriteOp => true;

        public string SongID { get; private set; }
        public int Index { get; private set; }

        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(SongID))
                throw new InvalidOperationException("SongID must be provided.");
            if (!context.Cache.SongCache.ContainsKey(SongID))
                throw new InvalidOperationException("SongID was not found.");
            var songCache = context.Cache.SongCache[SongID];
            var playlistCache = context.Cache.PlaylistCache[songCache.Playlist.PackID];
            if (Index < 0 || Index >= playlistCache.Songs.Count)
                throw new InvalidOperationException("Index is out of range.");

            var ptr = playlistCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.FirstOrDefault(x => x.Object.LevelID == SongID);
            if (ptr == null)
                throw new Exception("Unable to find song pointer!  Cache is out of sync!");
            var songIndex = playlistCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.IndexOf(ptr);
            playlistCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(ptr);
            var newIndex = Index;
            if (newIndex >= playlistCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Count)
                playlistCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Add(ptr);
            else
                playlistCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Insert(newIndex, ptr);

            //do the cache separately to be safe about it
            var ordered = playlistCache.Songs.Values.OrderBy(x => x.Order).ToList();
            var sng = ordered.First(x => x.Song.LevelID == SongID);
            var oldIndex = ordered.IndexOf(sng);
            ordered.Remove(sng);
            newIndex = Index;
            if (newIndex >= ordered.Count)
                ordered.Add(sng);
            else
                ordered.Insert(newIndex, sng);

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Order = i;
            }
        }
    }
}
