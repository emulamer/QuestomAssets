using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;

namespace BeatmapAssetMaker.BeatSaber
{
    public class AssetsBeatmapLevelCollectionObject : AssetsMonoBehaviourObject
    {
        public AssetsBeatmapLevelCollectionObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        {
        }

        public AssetsBeatmapLevelCollectionObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        {
        }


    }
}
