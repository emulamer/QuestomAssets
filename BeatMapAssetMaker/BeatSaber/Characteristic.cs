using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.BeatSaber
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Characteristic
    {
        Standard,
        OneSaber,
        NoArrows
    }
}

