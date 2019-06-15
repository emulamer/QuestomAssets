using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using Newtonsoft.Json;

namespace QuestomAssets
{
    public class BeatSaberSong
    {
        public BeatSaberSong()
        { }

        [JsonIgnore]
        internal BeatmapLevelDataObject LevelData { get; set; }

        [JsonIgnore]
        internal string SourceOgg { get; set; }

        [JsonIgnore]
        public byte[] CoverArtBytes { get; set; }

        public string SongID { get; set; }

        public string SongName { get; internal set; }

        public string CoverArtBase64PNG { get; internal set; }

        public string SongSubName { get; internal set; }
                
        public string SongAuthorName { get; internal set; }
                
        public string LevelAuthorName { get; internal set; }

        public string CustomSongFolder { get;  set; }
    }
}
