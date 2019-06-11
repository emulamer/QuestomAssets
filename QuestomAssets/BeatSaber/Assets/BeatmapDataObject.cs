using QuestomAssets.AssetsChanger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json.Linq;

namespace QuestomAssets.BeatSaber
{
    public class BeatmapDataObject: MonoBehaviourObject
    {
        public BeatmapDataObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        //public BeatmapDataObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        //{ }

        public BeatmapDataObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("BeatmapDataSO"))
        { }

        
        
        //public BeatmapDataObject(AssetsReader reader)
        //{
        //    Parse(reader);
        //}

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            JsonData = reader.ReadString();
            reader.AlignTo(4);
        }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(JsonData);
            writer.WriteArray(new byte[128]);
            writer.Write((Int32)0);
            writer.AlignTo(4);
        }
        private string _jsonData;
        [JsonProperty("_jsonData")]
        public string JsonData
        {
            get
            {
                return _jsonData;
            }
            set
            {
                var json = value;
                if (json != null)
                {
                    var jo = JObject.Parse(json);
                    if (jo.ContainsKey("_BPMChanges"))
                    {
                        jo.Remove("_BPMChanges");
                        json = jo.ToString(Formatting.None);
                    }
                }
                _jsonData = value;
            }
        }

        [JsonProperty("_signatureBytes")]
        public byte[] SignatureBytes { get; set; } = new byte[128];

        [JsonProperty("_projectedData")]
        public byte[] ProjectedData { get; set; }

        [JsonProperty("_beatsPerMinute")]
        public Single BeatsPerMinute { get; set; }

        [JsonProperty("_shuffle")]
        public Single Shuffle { get; set; }

        [JsonProperty("_shufflePeriod")]
        public Single ShufflePeriod { get; set; }

        [JsonProperty("_hasRequiredDataForLoad")]
        public bool HasRequiredDataForLoad { get; set; }



    }
}

