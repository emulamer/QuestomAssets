using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public abstract class LocatorAssetAction : AssetAction
    {
        public AssetLocator Locator { get; set; }
    }
}
