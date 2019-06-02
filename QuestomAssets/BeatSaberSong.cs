using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets
{
    public class BeatSaberSong
    {
        internal BeatSaberSong()
        { }

        public BeatSaberSong(string customSongFolder)
        {
            CustomSongFolder = customSongFolder;
        }
        internal long LevelPathID { get; set; }

        public string LevelID { get; internal set; }

        public string SongName { get; internal set; }

        public Bitmap CoverArt { get; internal set; }

        internal PPtr CoverArtPtr { get; set; }

        public string CoverArtFile { get; internal set; }
        
        public string SongSubName { get; internal set; }
                
        public string SongAuthorName { get; internal set; }
                
        public string LevelAuthorName { get; internal set; }

        public string CustomSongFolder { get; internal set; }

        private void LoadCustomSong()
        {

        }
    }
}
