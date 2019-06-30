using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static QuestomAssets.MusicConfigCache;

namespace QuestomAssets.AssetOps
{
    public class SortPlaylistOp : AssetOp
    {
        public override bool IsWriteOp => true;

        public SortPlaylistOp(string playlistID, PlaylistSortMode sortMode, bool reverse)
        {
            PlaylistID = playlistID;
            SortMode = sortMode;
            Reverse = reverse;
        }

        public string PlaylistID { get; private set; }
        public PlaylistSortMode SortMode { get; private set; }
        public bool Reverse { get; private set; }
        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(PlaylistID))
                throw new InvalidOperationException("Playlist ID must be provided.");

            if (!context.Cache.PlaylistCache.ContainsKey(PlaylistID))
                throw new InvalidOperationException("Playlist ID does not exist.");

            var plCache = context.Cache.PlaylistCache[PlaylistID];

            var songList = plCache.Songs.Values.ToArray();
            Comparer<OrderedSong> comparer;
            switch (SortMode)
            {
                case PlaylistSortMode.Name:
                    comparer = Comparer<OrderedSong>.Create((s1, s2) =>
                    {
                        try
                        {
                            return s1.Song.SongName.ToUpper().CompareTo(s2.Song.Name.ToUpper());
                        }
                        catch
                        {
                            return -1;
                        }
                    });
                    break;
                case PlaylistSortMode.MaxDifficulty:
                    comparer = Comparer<OrderedSong>.Create((s1, s2) =>
                    {
                        try
                        {
                            return s1.Song.DifficultyBeatmapSets.SelectMany(x => x.DifficultyBeatmaps).Max(x => x.Difficulty)
                                .CompareTo(s2.Song.DifficultyBeatmapSets.SelectMany(x => x.DifficultyBeatmaps).Max(x => x.Difficulty));
                        }
                        catch
                        {
                            return -1;
                        }
                    });
                    break;
                case PlaylistSortMode.LevelAuthor:
                    comparer = Comparer<OrderedSong>.Create((s1, s2) =>
                    {
                        try
                        {
                            return s1.Song.LevelAuthorName.ToUpper().CompareTo(s2.Song.LevelAuthorName.ToUpper());
                        }
                        catch
                        {
                            return -1;
                        }
                    });
                    break;
                default:
                    throw new NotImplementedException("Unhandled playlist sort mode.");
            }
            Array.Sort(songList, comparer);

            if (Reverse)
                Array.Reverse(songList);

            plCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.ToList().ForEach(x =>
            {
                plCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(x);
                x.Dispose();
            });
            for (int i = 0; i < songList.Count(); i++)
            {
                var song = songList[i];
                song.Order = i;
                plCache.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Add(song.Song.PtrFrom(plCache.Playlist.BeatmapLevelCollection.Object));
            }
        }
    }
}
