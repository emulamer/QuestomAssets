using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.BeatSaber
{
    public class DifficultyBeatmap
    {
        [JsonProperty("_difficulty")]
        public Difficulty Difficulty { get; set; }

        [JsonProperty("_difficultyRank")]
        public int DifficultyRank { get; set; }

        [JsonProperty("_noteJumpMovementSpeed")]
        public Single NoteJumpMovementSpeed { get; set; }

        /// <summary>
        /// This value is actually an integer, but some custom maps have a float in there and it won't deserialize properly
        /// </summary>
        [JsonProperty("_noteJumpStartBeatOffset")]
        public Single NoteJumpStartBeatOffset { get; set; }

        [JsonProperty("_beatmapData")]
        public BeatmapDataObject BeatmapData { get; set; }

        //unity assets format only
        [JsonIgnore]
        public ISmartPtr<BeatmapDataObject> BeatmapDataPtr { get; set; }

        //[JsonIgnore]
        //public BeatmapSaveData BeatmapSaveData { get; set; }

        public DifficultyBeatmap()
        { }

        public DifficultyBeatmap(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            Parse(assetsFile, owner, reader);
        }

        private void Parse(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            Difficulty = (Difficulty)reader.ReadInt32();
            DifficultyRank = reader.ReadInt32();
            NoteJumpMovementSpeed = reader.ReadSingle();
            NoteJumpStartBeatOffset = reader.ReadInt32();
            BeatmapDataPtr = SmartPtr<BeatmapDataObject>.Read(assetsFile, owner, reader);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write((int)Difficulty);
            writer.Write(DifficultyRank);
            writer.Write(NoteJumpMovementSpeed);
            Int32 jump = (Int32)NoteJumpStartBeatOffset;
            writer.Write(jump);
            BeatmapDataPtr.Write(writer);            
        }
    }

}