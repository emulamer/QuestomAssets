using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public abstract class AssetAction
    {
        public int StepNumber { get; set; }
        public abstract AssetActionType Type { get; }
    }
}
