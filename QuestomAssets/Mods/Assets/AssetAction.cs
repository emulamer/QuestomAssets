using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    [JsonConverter(typeof(AssetActionTypeConverter))]
    public abstract class AssetAction
    {
        public int StepNumber { get; set; }
        public abstract AssetActionType Type { get; }

        public abstract IEnumerable<AssetOps.AssetOp> GetOps(ModContext context);
    }
}
