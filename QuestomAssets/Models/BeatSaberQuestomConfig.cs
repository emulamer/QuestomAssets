using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.BeatSaber;

namespace QuestomAssets.Models
{
    //NOTE: stuff will probably break if you use the same name for a song and a playlist, or duplicate any built in ones
    /// <summary>
    /// Primary configuration model for QustomAssets
    /// </summary>
    public class BeatSaberQuestomConfig
    {
        /// <summary>
        /// The list of playlists (i.e.. level packs) that will show up in Beat Saber
        /// </summary>
        public List<BeatSaberPlaylist> Playlists { get; } = new List<BeatSaberPlaylist>();

        /// <summary>
        /// Currently no way to set back to original saber :(
        /// </summary>
        public SaberModel Saber { get; set; }

        /// <summary>
        /// The color to use for the left saber/blocks.  Set to null to revert to default
        /// </summary>
        public BeatSaberColor LeftColor { get; set; }

        /// <summary>
        /// The color to use for the right saber/blocks.  Set to null to revert to default
        /// </summary>
        public BeatSaberColor RightColor { get; set; }

        /// <summary>
        /// The texts to change. Provide a key and a value for each text to change.
        /// Keys can be found here: https://github.com/sc2ad/QuestModdingTools/blob/master/BeatSaberLocale.txt
        /// </summary>
        public List<(string, string)> TextChanges { get; set; } = new List<(string, string)>();
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
