using Newtonsoft.Json;
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

        public abstract void InstallComponent(ModContext context);

        public abstract void UninstallComponent(ModContext context);
    }
}
