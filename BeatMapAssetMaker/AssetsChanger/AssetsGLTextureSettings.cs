using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsGLTextureSettings
    {
        //public Int32 serializedVersion: 2
        public Int32 FilterMode { get; set; }
        public Int32 Aniso { get; set; }
        public Single MipBias { get; set; }
        public Int32 WrapU { get; set; }
        public Int32 WrapV { get; set; }
        public Int32 WrapW { get; set; }
    }
}
