using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using Newtonsoft.Json;

namespace QuestomAssets.Models
{
    public class BeatSaberSong
    {
        public BeatSaberSong()
        { }

        [JsonIgnore]
        internal BeatmapLevelDataObject LevelData { get; set; }

        public string SongID { get; set; }

        public string SongName { get; internal set; }

        public string CoverArtFilename { get; internal set; }

        public string SongSubName { get; internal set; }
                
        public string SongAuthorName { get; internal set; }
                
        public string LevelAuthorName { get; internal set; }

        /// <summary>
        /// The path on the device to the folder where the custom song lives, e.g. setting this value to "TheBestSong" would map to something like "/sdcard/BeatOnData/CustomSongs/TheBestSong/"
        /// </summary>
        public string CustomSongPath { get; set; }

        public byte[] TryGetCoverPngBytes()
        {
            if (LevelData == null)
                return null;
            try
            {
                return QuestomAssets.Utils.ImageUtils.Instance.TextureToPngBytes(LevelData.CoverImageTexture2D?.Object);
            }
            catch (Exception ex)
            {
                Log.LogErr("Unable to get cover PNG bytes", ex);
                return null;
            }
        }

    }
}
