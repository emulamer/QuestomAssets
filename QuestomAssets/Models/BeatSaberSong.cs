using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using Newtonsoft.Json;
using System.ComponentModel;

namespace QuestomAssets.Models
{
    public class BeatSaberSong : INotifyPropertyChanged
    {
        public BeatSaberSong()
        { }

        [JsonIgnore]
        internal BeatmapLevelDataObject LevelData { get; set; }

        private string _songID;
        public string SongID
        {
            get => _songID;
            set
            {
                if (_songID != value)
                    PropChanged(nameof(SongID));
                _songID = value;
            }
        }

        public string SongName { get; internal set; }

        public string CoverArtFilename { get; internal set; }

        public string SongSubName { get; internal set; }

        public string SongAuthorName { get; internal set; }

        public string LevelAuthorName { get; internal set; }

        private string _customSongPath;
        /// <summary>
        /// The path on the device to the folder where the custom song lives, e.g. setting this value to "TheBestSong" would map to something like "/sdcard/BeatOnData/CustomSongs/TheBestSong/"
        /// </summary>
        public string CustomSongPath { get => _customSongPath;
            set
            {
                if (_customSongPath != value)
                    PropChanged(nameof(CustomSongPath));
                _customSongPath = value;
            }
        }

        public byte[] TryGetCoverPngBytes()
        {
            if (LevelData == null)
                return null;
            try
            {
                bool texLoaded = LevelData.CoverImageTexture2D.Target.IsLoaded;
                var png = QuestomAssets.Utils.ImageUtils.Instance.TextureToPngBytes(LevelData.CoverImageTexture2D?.Object);
                if (!texLoaded)
                    LevelData.CoverImageTexture2D.Target.FreeObject();

                return png;
            }
            catch (Exception ex)
            {
                Log.LogErr("Unable to get cover PNG bytes", ex);
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
