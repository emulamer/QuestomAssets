using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QuestomAssets.BeatSaber
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaylistSortMode
    {
        Name,
        MaxDifficulty,
        LevelAuthor
    }
}