using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ModComponentType
    {
        HookMod,
        AssetsMod,
        FileCopyMod
    }
}
