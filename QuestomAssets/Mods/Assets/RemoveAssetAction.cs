using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class RemoveAssetAction : LocatorAssetAction
    {
        public override AssetActionType Type => AssetActionType.RemoveAsset;

        public string FileName { get; set; }

        public override IEnumerable<AssetOp> GetOps(ModContext context)
        {
            if (Locator == null)
                throw new Exception("Locator is null for RestoreAssetsAction!");
            yield return new RemoveAssetOp(FileName, Locator);

        }
    }
}
