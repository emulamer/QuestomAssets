using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatmapAssetMaker
{
    public static class Extensions
    {
        public static UPtr ToUPtr(this AssetsChanger.PPtr ptr)
        {
            return new UPtr()
            {
                FileID = ptr.FileID,
                PathID = ptr.PathID
            };
        }

        public static AssetsChanger.PPtr ToAssetsPtr(this UPtr ptr)
        {
            return new AssetsChanger.PPtr(ptr.FileID, ptr.PathID);
        }
    }
}
