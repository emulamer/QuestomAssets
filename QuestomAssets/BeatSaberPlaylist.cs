using Newtonsoft.Json;
using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace QuestomAssets
{
    public class BeatSaberPlaylist
    {


        [JsonIgnore]
        internal BeatmapLevelPackObject LevelPackObject { get; set; }

        [JsonIgnore]
        internal BeatmapLevelCollectionObject LevelCollection { get; set; }

        public string PlaylistID { get; set; }

        public string PlaylistName { get; set; }

        public Bitmap CoverArt { get; set; }

        public string CoverArtFile { get; set; }

        public List<BeatSaberSong> SongList { get; } = new List<BeatSaberSong>();

    }
}
