using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Mods.Assets;

namespace QuestomAssets.AssetOps
{
    public class ReplaceAssetOp : AssetOp
    {
        public override bool IsWriteOp => true;

        private AssetLocator _locator;
        private byte[] _replaceDataWith;
        public ReplaceAssetOp(AssetLocator locator, byte[] replaceDataWith)
        {
            _locator = locator;
            _replaceDataWith = replaceDataWith;
        }

        internal override void PerformOp(OpContext context)
        {
            var asset = _locator.Locate(context.Manager, false);
            using (var ms = new MemoryStream(_replaceDataWith))
            {
                using (AssetsReader reader = new AssetsReader(ms))
                {
                    asset.ObjectInfo.DataOffset = 0;
                    asset.ObjectInfo.DataSize = _replaceDataWith.Length;
                    asset.Parse(reader);

                    asset.ObjectInfo.ParentFile.HasChanges = true;
                }
            }
        }
    }
}
