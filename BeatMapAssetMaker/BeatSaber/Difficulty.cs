using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.BeatSaber
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Difficulty : int
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Expert = 3,
        ExpertPlus = 4
    }
}

