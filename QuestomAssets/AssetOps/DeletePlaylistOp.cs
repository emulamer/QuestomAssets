using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class DeletePlaylistOp : AssetOp
    {
        public override bool IsWriteOp => true;

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

            if (PlaylistID == "CustomSongs")
                throw new InvalidOperationException("Don't delete CustomSongs playlist, it's needed for things.");

            OpCommon.DeletePlaylist(context, PlaylistID, DeleteSongsOnPlaylist);
        }
    }
}
