using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class MovePlaylistOp : AssetOp
    {
        public MovePlaylistOp(string playlistID, int index)
        {
            PlaylistID = playlistID;
            Index = index;
        }

        public override bool IsWriteOp => true;

        public string PlaylistID { get; private set; }

        public int Index { get; private set; }

        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(PlaylistID))
                throw new InvalidOperationException("PlaylistID must be provided.");
            if (!context.Cache.PlaylistCache.ContainsKey((PlaylistID)))
                throw new InvalidOperationException("Playlist ID was not found.");
            if (Index < 0 || Index >= context.Cache.PlaylistCache.Count)
                throw new InvalidOperationException("Index is out of range.");

            var mainCol = context.Engine.GetMainLevelPack();
            var ptr = mainCol.BeatmapLevelPacks.FirstOrDefault(x => x.Object.PackID == PlaylistID);
            if (ptr == null)
                throw new Exception("Unable to find this playlist in the main level pack!  Cache is out of sync!");

            var packIndex = mainCol.BeatmapLevelPacks.IndexOf(ptr);
            mainCol.BeatmapLevelPacks.Remove(ptr);
            var newIndex = Index;
            if (packIndex < Index)
                newIndex--;
            mainCol.BeatmapLevelPacks.Insert(newIndex, ptr);

            //just to be on the safe side, operate on the cache separately and reorder it
            var playlist = context.Cache.PlaylistCache[PlaylistID];
            var ordered = context.Cache.PlaylistCache.Values.OrderBy(x => x.Order).ToList();
            var oldIndex = ordered.IndexOf(playlist);
            ordered.Remove(playlist);
            newIndex = Index;
            if (oldIndex < Index)
                newIndex--;

            ordered.Insert(newIndex, playlist);
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Order = i;
            }
        }
    }
}
