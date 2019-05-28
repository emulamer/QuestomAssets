using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;

namespace BeatmapAssetMaker.BeatSaber
{
    public class AssetsBeatmapLevelPackObject : AssetsMonoBehaviourObject
    {
        public AssetsBeatmapLevelPackObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        { }
        public AssetsBeatmapLevelPackObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

    }
}
