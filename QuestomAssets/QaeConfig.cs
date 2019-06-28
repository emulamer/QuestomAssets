using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public class QaeConfig
    {
        public IAssetsFileProvider RootFileProvider { get; set; }
        public IAssetsFileProvider SongFileProvider { get; set; }

        public string AssetsPath { get; set; }

        public string SongsPath { get; set; }

        public string PlaylistArtPath { get; set; }

        public string ModsSourcePath { get; set; }

        public IAssetsFileProvider ModLibsFileProvider { get; set; } 
    }
}
