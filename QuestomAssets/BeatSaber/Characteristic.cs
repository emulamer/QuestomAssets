using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Characteristic
    {
        Standard,
        OneSaber,
        NoArrows,
        LightShow,
        Lawless
    }
}

