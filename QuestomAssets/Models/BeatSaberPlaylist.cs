using Newtonsoft.Json;
using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.Models
{
    public class BeatSaberPlaylist
    {
        [JsonIgnore]
        internal BeatmapLevelPackObject LevelPackObject { get; set; }

        [JsonIgnore]
        internal SpriteObject CoverArtSprite { get; set; }

        public string CoverArtFilename { get; set; }

        public string PlaylistID { get; set; }

        public string PlaylistName { get; set; }
        
        public List<BeatSaberSong> SongList { get; } = new List<BeatSaberSong>();

        public byte[] TryGetCoverPngBytes()
        {
            if (LevelPackObject == null)
                return null;
            try
            {
                return QuestomAssets.Utils.ImageUtils.Instance.TextureToPngBytes(LevelPackObject.CoverImage?.Object?.Texture?.Object);
            }
            catch (Exception ex)
            {
                Log.LogErr("Unable to get cover PNG bytes", ex);
                return null;
            }
        }

    }
}
