using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.BeatSaber;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuestomAssets.Models
{
    //NOTE: stuff will probably break if you use the same name for a song and a playlist, or duplicate any built in ones
    /// <summary>
    /// Primary configuration model for QustomAssets
    /// </summary>
    public class BeatSaberQuestomConfig : INotifyPropertyChanged
    {
        public BeatSaberQuestomConfig()
        {
            Playlists = new ObservableCollection<BeatSaberPlaylist>();
            Playlists.CollectionChanged += (e, a) =>
            {
                PropChanged("Playlists");
                if (a.OldItems != null)
                {
                    foreach (var oi in a.OldItems)
                    {
                        var p = oi as BeatSaberPlaylist;
                        p.PropertyChanged -= Playlists_PropertyChanged;

                    }
                }
                if (a.NewItems != null)
                {
                    foreach (var ni in a.NewItems)
                    {
                        var p = ni as BeatSaberPlaylist;
                        p.PropertyChanged += Playlists_PropertyChanged;
                    }
                }
            };
        }

        private void Playlists_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropChanged(nameof(Playlists));
        }

        public bool Matches(BeatSaberQuestomConfig config)
        {
            if (this.Playlists.Count != config.Playlists.Count)
                return false;

            for (int p = 0; p < Playlists.Count; p++)
            {
                var thisOne = Playlists[p];
                var thatOne = config.Playlists[p];
                if (thisOne.PlaylistID != thatOne.PlaylistID)
                    return false;
                if (thisOne.PlaylistName != thatOne.PlaylistName)
                    return false;
                if (thisOne.SongList.Count != thatOne.SongList.Count)
                    return false;

                for (int s = 0; s < thisOne.SongList.Count; s++)
                {
                    var thisSong = thisOne.SongList[s];
                    var thatSong = thatOne.SongList[s];
                    if (thisSong.SongID != thatSong.SongID)
                        return false;
                }
            }

            if (this.Saber?.SaberID != config.Saber?.SaberID)
                return false;

            //lazy copy/paste.  Make a proper comparer
            if (this.LeftColor?.A != config?.LeftColor?.A || this.LeftColor?.R != config?.LeftColor?.R || this.LeftColor?.G != config?.LeftColor?.G || this.LeftColor?.B != config?.LeftColor?.B)
                return false;
            if (this.RightColor?.A != config?.RightColor?.A || this.RightColor?.R != config?.RightColor?.R || this.RightColor?.G != config?.RightColor?.G || this.RightColor?.B != config?.RightColor?.B)
                return false;
            if (this.TextChanges == null && config.TextChanges != null || this.TextChanges != null && config.TextChanges == null)
                return false;
            if (this.TextChanges != null)
            {
                if (this.TextChanges.Count != config.TextChanges.Count)
                {
                    //I think this is right...
                    if (TextChanges.Any(x => !config.TextChanges.Any(y => y.Item1 == x.Item1 && y.Item2 == x.Item2)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The list of playlists (i.e.. level packs) that will show up in Beat Saber
        /// </summary>
        public ObservableCollection<BeatSaberPlaylist> Playlists { get; private set; }

        private SaberModel _saberModel;
        /// <summary>
        /// Currently no way to set back to original saber :(
        /// </summary>
        public SaberModel Saber
        {
            get => _saberModel;
            set
            {
                if (_saberModel != value)
                {
                    PropChanged(nameof(SaberModel));
                    if (_saberModel != null)
                    {
                        _saberModel.PropertyChanged -= SaberModel_PropertyChanged;
                    }
                    if (value != null)
                    {
                        value.PropertyChanged += SaberModel_PropertyChanged;
                    }
                }
                _saberModel = value;
            }
        }

        private void SaberModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //nameof won't work for this property because of the type....
            PropChanged(nameof(SaberModel));
        }

        private BeatSaberColor _leftColor;
        /// <summary>
        /// The color to use for the left saber/blocks.  Set to null to revert to default
        /// </summary>
        public BeatSaberColor LeftColor
        {
            get => _leftColor;
            set
            {
                if (_leftColor != value)
                {
                    PropChanged(nameof(LeftColor));
                    if (_leftColor != null)
                    {
                        _leftColor.PropertyChanged -= LeftColor_PropertyChanged;
                    }
                    if (value != null)
                    {
                        value.PropertyChanged += LeftColor_PropertyChanged;
                    }
                }
                _leftColor = value;
            }
        }

        private void LeftColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropChanged(nameof(LeftColor));
        }

        private BeatSaberColor _rightColor;
        /// <summary>
        /// The color to use for the right saber/blocks.  Set to null to revert to default
        /// </summary>
        public BeatSaberColor RightColor
        {
            get => _rightColor;
            set
            {
                if (_rightColor != value)
                {
                    PropChanged(nameof(RightColor));
                    if (_rightColor != null)
                    {
                        _rightColor.PropertyChanged -= RightColor_PropertyChanged;
                    }
                    if (value != null)
                    {
                        value.PropertyChanged += RightColor_PropertyChanged;
                    }
                }
                _rightColor = value;
            }
        }

        private void RightColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropChanged(nameof(RightColor));
        }

        /// <summary>
        /// The texts to change. Provide a key and a value for each text to change.
        /// Keys can be found here: https://github.com/sc2ad/QuestModdingTools/blob/master/BeatSaberLocale.txt
        /// </summary>
        public List<(string, string)> TextChanges { get; set; } = new List<(string, string)>();
        //TODO: hook up TextChanges to prop changed


        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        /*
Input structure should look like this:          
{
Playlists: [
{
PlaylistID: string,     //a-z, A-Z and 0-9 only, no special characters or spaces

PlaylistName: string, 
CoverArtFile: string,   //if the PlaylistID already exists and CoverArtFile is null or empty it will not be changed
//if it doesn't exist and CoverArtFile is null or empty, a default will be used

SongList: [
SongID,            //LevelID should be passed for songs that already exist
//OR
CustomSongFolder    //CustomSongFolder should be passed for new songs that are being loaded
//this is the base path where info.dat and other files for the custom song exists
]                        
}
],
//NULL if you don't want to change it
Saber: {
SaberID: string,            //SaberID should be passed for sabers that already exist
//"Basic" is the default
//OR
CustomSaberFolder string,   //CustomSaberFolder should be passed for new sabers that are being loaded
//this is the base path where saberinfo.dat and other files for the custom saber exist


},
//NULL FOR BOTH ITEMS if you don't want to change it (or just ignore this field)
Colors: [
{
"_color": {
"R": float,
"G": float,
"B": float,
"A": float
}
},
{
"_color": {
"R": float,
"G": float,
"B": float,
"A": float
}
}
],
// Ignore this field if you don't want to change any text
TextChanges: [
{
"Item1": string,
"Item2": string
}
]
}

Example saberinfo.dat file:             
{
"Format": "dat",    //currently always "dat"
"ID": "Katana",     //unique id of the saber, a-z A-Z 0-9 only
"DatFiles":{
"SaberBlade": "SaberBlade.dat",                 //dat files (as would be exported raw from UABE) 
"SaberGlowingEdges": "SaberGlowingEdges.dat",
"SaberHandle": "SaberHandle.dat"
}
}


Exported data format:
{
Playlists: [
{
PlaylistID: string, 
PlaylistName: string, 
CoverArtPngBase64: string,
SongList: [
SongID,
SongName,
CoverArtPngBase64,
SongSubName,
SongAuthorName,
LevelAuthorName
]
}
],

Saber: {
ID: string                  
}             
}



verification:
check that ID+[AssetTypeName] exists.  if it does, more data isn't required

*/
    }
}
