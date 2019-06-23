using Newtonsoft.Json;
using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using QuestomAssets.AssetsChanger;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace QuestomAssets.Models
{
    public class BeatSaberPlaylist : INotifyPropertyChanged
    {
        public BeatSaberPlaylist()
        {
            SongList = new ObservableCollection<BeatSaberSong>();
            SongList.CollectionChanged += (e, a) =>
             {
                 PropChanged("SongList");
                 if (a.OldItems != null)
                 {
                     foreach (var oi in a.OldItems)
                     {
                         var s = oi as BeatSaberSong;
                         s.PropertyChanged -= SubSong_PropertyChanged;
                     }
                 }
                 if (a.NewItems != null)
                 {
                     foreach (var ni in a.NewItems)
                     {
                         var s = ni as BeatSaberSong;
                         s.PropertyChanged += SubSong_PropertyChanged;
                     }
                 }
             };
        }

        private void SubSong_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropChanged(nameof(SongList));
        }

        [JsonIgnore]
        internal BeatmapLevelPackObject LevelPackObject { get; set; }

        [JsonIgnore]
        internal SpriteObject CoverArtSprite { get; set; }

        //private string _coverArtFilename;
        //public string CoverArtFilename
        //{
        //    get => _coverArtFilename;
        //    set
        //    {
        //        if (_coverArtFilename != value)
        //            PropChanged(nameof(CoverArtFilename));
        //        _coverArtFilename = value;
        //    }
        //}

        private string _playlistID;
        public string PlaylistID
        {
            get => _playlistID;
            set
            {
                if (_playlistID != value)
                    PropChanged(nameof(PlaylistID));
                _playlistID = value;
            }
        }

        private string _playlistName;
        public string PlaylistName { get => _playlistName;
            set
            {
                if (_playlistName != value)
                    PropChanged(nameof(PlaylistName));
                _playlistName = value;
            }
        }
        
        public ObservableCollection<BeatSaberSong> SongList { get; private set; }

        public byte[] CoverImageBytes { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
