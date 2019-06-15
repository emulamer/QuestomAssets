using Newtonsoft.Json;
using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets
{
    public class BeatSaberPlaylist
    {
        [JsonIgnore]
        internal BeatmapLevelPackObject LevelPackObject { get; set; }

        [JsonIgnore]
        internal SpriteObject CoverArtSprite { get; set; }

        [JsonIgnore]
        public byte[] CoverArtBytes { get; set; }

        public string PlaylistID { get; set; }

        public string PlaylistName { get; set; }

        public string CoverArtBase64PNG { get; internal set; }
        
        //public string CoverArtFile { get; set; }

        public List<BeatSaberSong> SongList { get; } = new List<BeatSaberSong>();

    }
}
