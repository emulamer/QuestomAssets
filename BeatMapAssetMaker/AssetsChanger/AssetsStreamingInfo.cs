using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsStreamingInfo
    {
        public UInt32 offset { get; set; }
        public UInt32 size { get; set; }
        public string path { get; set; }
    }
}
