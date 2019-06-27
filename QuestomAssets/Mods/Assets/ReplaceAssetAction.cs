using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class ReplaceAssetAction : AssetAction
    {
        public override AssetActionType Type => AssetActionType.ReplaceAsset;

        public string FromDataFile { get; set; }

        public AssetLocator Locator { get; set; }
    }
}
