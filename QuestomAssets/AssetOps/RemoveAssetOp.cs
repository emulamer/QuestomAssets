using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Mods.Assets;

namespace QuestomAssets.AssetOps
{
    public class RemoveAssetOp : AssetOp
    {
        public override bool IsWriteOp => true;

        private string _filename;
        private AssetLocator _locator;
        public RemoveAssetOp(string filename, AssetLocator locator)
        {
            _filename = filename;
            _locator = locator;
        }

        internal override void PerformOp(OpContext context)
        {
            var ao = _locator.Locate(context.Manager);
            context.Manager.GetAssetsFile(_filename).DeleteObject(ao);
        }
    }
}
