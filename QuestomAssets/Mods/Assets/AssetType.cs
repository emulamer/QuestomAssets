using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssetType
    {
        Mesh,
        Transform,
        GameObject,
        Texture2D,
        AudioClip,
        Sprite,
        Text,
        MeshFilter,
        Component,
        MonoBehaviour
    }
}
