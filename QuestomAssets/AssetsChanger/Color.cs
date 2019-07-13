using Newtonsoft.Json;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class Color
    {
        [JsonProperty("R")]
        public float R { get; set; }
        [JsonProperty("G")]
        public float G { get; set; }
        [JsonProperty("B")]
        public float B { get; set; }
        [JsonProperty("A")]
        public float A { get; set; }

        public Color()
        { }

        public Color(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            R = reader.ReadSingle();
            G = reader.ReadSingle();
            B = reader.ReadSingle();
            A = reader.ReadSingle();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(R);
            writer.Write(G);
            writer.Write(B);
            writer.Write(A);
        }
    }
}
