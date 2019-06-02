using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace QuestomAssets
{
    public class BeatSaberPlaylist
    {
        public string PlaylistID { get; internal set; }
        public string PlaylistName { get; set; }

        internal long LevelPackPathID { get; set; }
        internal long levelCollectionPathID { get; set; } 

        public Bitmap CoverArt { get; set; }

        public string CoverArtFile { get; internal set; }

        public List<BeatSaberSong> SongList { get; } = new List<BeatSaberSong>();

    }
}
