using Newtonsoft.Json;
using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    [JsonConverter(typeof(ModComponentTypeConverter))]
    public abstract class ModComponent
    {
        /// <summary>
        /// The type of modification this component will perform
        /// </summary>
        public abstract ModComponentType Type { get; }

        public abstract List<AssetOp> GetInstallOps(ModContext context);

        public abstract List<AssetOp> GetUninstallOps(ModContext context);
    }
}
