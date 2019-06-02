using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    /// <summary>
    /// Primary configuration model for QustomAssets
    /// </summary>
    public class BeatSaberQustomConfig
    {
        /// <summary>
        /// The list of playlists (i.e.. level packs) that will show up in Beat Saber
        /// </summary>
        public List<BeatSaberPlaylist> Playlists { get; } = new List<BeatSaberPlaylist>();

        /// <summary>
        /// Currently no way to set back to original saber :(
        /// </summary>
        public SaberModel Saber { get; set; }

        /*
         * Input structure shold look like this:
         * 
         * {
         *      Playlists: [
         *  
         *  
         *  
         *      ],
         *      //NULL if you don't want to change it
         *      Saber: {
         *          ID: string,     //Identifier for saber.  Shouldn't contain spaces or special characters.  alpha and number only. "Basic" is the default
         *          
         *          //If ID is passed in and found to already exist in the assets the following are not needed.  If ID
         *          SaberBladeDatFile: string,          //the dat file for the SaberBlade object (as would be exported "raw" from UABE)
         *
         *          SaberGlowingEdgesDatFile: string,   //the dat file for the SaberGlowingEdges object (as would be exported "raw" from UABE)
         *
         *          SaberHandleDatFile { get; set; }    //the dat file for the SaberHandleDatFile object (as would be exported "raw" from UABE)
         *      
         *      }
         * 
         * 
         * }
         * 
         * 
         * 
         * verification:
         *  check that ID+[AssetTypeName] exists.  if it does, more data isn't required
         * 
         */
    }
}
