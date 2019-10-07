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

       // public string CoverArtFilename { get; internal set; }

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
        private static object _coverLock = new object();
        public byte[] TryGetCoverPngBytes()
        {
            if (LevelData == null)
                return null;
            try
            {
                lock (_coverLock)
                {
                    if (LevelData == null)
                    {
                        Log.LogErr($"LevelData for song id {SongID} isn't set!  Cannot get cover texture from assets.");
                        return null;
                    }
                    bool texLoaded = LevelData.CoverImageTexture2D.Target.IsLoaded;
                    var tex = LevelData.CoverImageTexture2D?.Target?.Object;
                    if (tex == null)
                    {
                        Log.LogErr($"Texture from level data on song id {SongID} returned null trying to pull it from assets, which is a problem.  Cover image won't load.");
                        return null;
                    }
                    
                    var png = QuestomAssets.Utils.ImageUtils.Instance.TextureToPngBytes(tex);
                    if (!texLoaded)
                        LevelData.CoverImageTexture2D.Target.FreeObject();
                    return png;
                }                
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
