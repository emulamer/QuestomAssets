using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;
using Newtonsoft.Json;

namespace BeatmapAssetMaker.BeatSaber
{
    public sealed class AssetsBeatmapLevelDataObject : AssetsMonoBehaviourObject
    {
        public AssetsBeatmapLevelDataObject()
        { }

        public AssetsBeatmapLevelCollectionObject(AssetsMetadata metadata) : base(metadata, AssetsConstants.ClassHash, AssetsConstants.ScriptPtr.BeatmapLevelCollectionTypePtr)
        { }

        public AssetsBeatmapLevelDataObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public AssetsBeatmapLevelDataObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public override byte[] ScriptParametersData
        {
            get
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
            set
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
        }
        asdfasdfasdfsadfsadfsdfsadfsfsfsdaf
        //public BeatmapLevelData BeatmapLevelData { get; set; }       

        private void SerializeScriptParameters()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var writer = new AssetsWriter(ms))
                {
                    BeatmapLevelData.Write(writer);
                    ScriptParametersData = ms.ToArray();
                }
            }
        }

        private void DeserializeScriptParameters(byte[] scriptParametersData)
        {
            using (MemoryStream ms = new MemoryStream(ScriptParametersData))
            {
                using (AssetsReader reader = new AssetsReader(ms))
                {
                    BeatmapLevelData = new BeatmapLevelData();

                    BeatmapLevelData.LevelID = reader.ReadString();
                    BeatmapLevelData.SongName = reader.ReadString();
                    BeatmapLevelData.SongAuthorName = reader.ReadString();
                    BeatmapLevelData.LevelAuthorName = reader.ReadString();
                    BeatmapLevelData.AudioClip = new AssetsPtr(reader);
                    BeatmapLevelData.BeatsPerMinute = reader.ReadSingle();
                    BeatmapLevelData.SongTimeOffset = reader.ReadSingle();
                    BeatmapLevelData.Shuffle = reader.ReadSingle();
                    BeatmapLevelData.ShufflePeriod = reader.ReadSingle();
                    BeatmapLevelData.PreviewStartTime = reader.ReadSingle();
                    BeatmapLevelData.PreviewDuration = reader.ReadSingle();
                    BeatmapLevelData.CoverImageTexture2D = new AssetsPtr(reader);
                    BeatmapLevelData.EnvironmentSceneInfo = new AssetsPtr(reader);
                    int numBeatmaps = reader.ReadInt32();
                    for (int i = 0; i < numBeatmaps; i++)
                    {
                        BeatmapLevelData.DifficultyBeatmapSets.Add(DeserializeBeatmapSet(reader));
                    }                    
                }
            }
        }


        private void Parse(AssetsReader reader)
        {
            JsonData = reader.ReadString();
            SignatureBytes = reader.ReadArray();
            ProjectedData = reader.ReadArray();
            reader.AlignTo(4);
        }

        public void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(JsonData);
            writer.WriteArray(SignatureBytes);
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

        private DifficultyBeatmapSet DeserializeBeatmapSet(AssetsReader reader)
        {

            DifficultyBeatmapSet set = new DifficultyBeatmapSet();
            set._beatmapCharacteristic = new AssetsPtr(reader).ToUPtr();
            int numBeatmaps = reader.ReadInt32();
            for (int i = 0; i < numBeatmaps; i++)
            {
                set._difficultyBeatmaps.Add(DeserializeDifficultyBeatmap(reader));
            }
            return set;
        }

        private DifficultyBeatmapSO DeserializeDifficultyBeatmap(AssetsReader reader)
        {
            DifficultyBeatmapSO bm = new DifficultyBeatmapSO();
            bm._difficulty = (Difficulty)reader.ReadInt32();
            bm._difficultyRank = reader.ReadInt32();
            bm._noteJumpMovementSpeed = reader.ReadSingle();
            bm._noteJumpStartBeatOffset = reader.ReadInt32();
            bm._beatmapDataPtr = new AssetsPtr(reader).ToUPtr();
            return bm;
        }

    }
}
