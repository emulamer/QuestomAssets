using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public class QaeConfig
    {
        public IFileProvider RootFileProvider { get; set; }
        public IFileProvider SongFileProvider { get; set; }

        public string AssetsPath { get; set; }

        public string SongsPath { get; set; }

        public string PlaylistArtPath { get; set; }

        public string ModsSourcePath { get; set; }

        public IFileProvider ModLibsFileProvider { get; set; } 
    }
}
