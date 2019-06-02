using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    /// <summary>
    /// Primary configuration model for QustomAssets
    /// </summary>
    public class BeatSaberQustomConfig
    {
        /// <summary>
        /// The list of playlists (i.e.. level packs) that will show up in Beat Saber
        /// </summary>
        public List<BeatSaberPlaylist> Playlists { get; } = new List<BeatSaberPlaylist>();

        /// <summary>
        /// Currently no way to set back to original saber :(
        /// </summary>
        public CustomSaber Saber { get; set; }
    }
}
