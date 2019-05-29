using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatmapAssetMaker
{
    public static class Extensions
    {
        public static UPtr ToUPtr(this AssetsChanger.AssetsPtr ptr)
        {
            return new UPtr()
            {
                FileID = ptr.FileID,
                PathID = ptr.PathID
            };
        }

        public static AssetsChanger.AssetsPtr ToAssetsPtr(this UPtr ptr)
        {
            return new AssetsChanger.AssetsPtr(ptr.FileID, ptr.PathID);
        }
    }
}
