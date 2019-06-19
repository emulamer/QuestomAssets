using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Models
{
    public class BeatSaberColor
    {
        [JsonProperty("R")]
        public float R { get; set; }
        [JsonProperty("G")]
        public float G { get; set; }
        [JsonProperty("B")]
        public float B { get; set; }
        [JsonProperty("A")]
        public float A { get; set; }
    }
}
