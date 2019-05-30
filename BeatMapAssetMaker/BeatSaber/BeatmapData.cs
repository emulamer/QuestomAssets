using BeatmapAssetMaker.AssetsChanger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BeatmapAssetMaker.BeatSaber
{
    public class BeatmapData: AssetsMonoBehaviourObject
    {
        public BeatmapData(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public BeatmapData(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public BeatmapData(AssetsMetadata metadata) : base(metadata, AssetsConstants.ScriptHash.BeatmapDataHash, AssetsConstants.ScriptPtr.BeatmapDataScriptPtr)
        { }

        public void UpdateTypes(AssetsMetadata metadata)
        {
            base.UpdateType(metadata, AssetsConstants.ScriptHash.BeatmapDataHash, AssetsConstants.ScriptPtr.BeatmapDataScriptPtr);
        }
        public BeatmapData()
        {  }

        public BeatmapData(AssetsReader reader)
        {
            Parse(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            JsonData = reader.ReadString();
            reader.AlignToObjectData(4);
            SignatureBytes = reader.ReadArray();
            reader.AlignToObjectData(4);
            ProjectedData = reader.ReadArray();
            reader.AlignToObjectData(4);
        }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(JsonData);
            writer.AlignTo(4);
            writer.WriteArray(SignatureBytes);
            writer.AlignTo(4);
            writer.WriteArray(ProjectedData);
            writer.AlignTo(4);
        }
        
        public void TransformToProjectedData()
        {
            if (string.IsNullOrWhiteSpace(JsonData))
                throw new InvalidOperationException("JsonData must be set before transforming to ProjectedData.");
            
            var saveData = BeatmapSaveData.DeserializeFromJSONString(JsonData);

            ProjectedData = saveData.SerializeToBinary();
        }

        public void TransformToJsonData()
        {
            if (ProjectedData == null || ProjectedData.Length < 1)
                throw new InvalidOperationException("ProjectedData must be set before transforming to JsonData.");

            var saveData = BeatmapSaveData.DeserializeFromFromBinary(ProjectedData);
            JsonData = saveData.SerializeToJSONString();
        }

        [JsonProperty("_jsonData")]
        public string JsonData { get; set; }

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

