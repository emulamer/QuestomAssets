using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class DeleteSongOp : AssetOp
    {
        public DeleteSongOp(string songID)
        {
            SongID = songID;
        }

        public string SongID { get; private set; }
        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(SongID))
                throw new InvalidOperationException("SongID must be provided.");
            if (!context.Cache.SongCache.ContainsKey(SongID))
                throw new InvalidOperationException("SongID does not exist.");
            OpCommon.DeleteSong(context, SongID);
        }
    }
}
