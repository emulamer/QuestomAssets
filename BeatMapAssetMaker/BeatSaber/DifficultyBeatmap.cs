using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BeatmapAssetMaker.AssetsChanger;

namespace BeatmapAssetMaker.BeatSaber
{
    public class DifficultyBeatmap
    {
        [JsonProperty("_difficulty")]
        public Difficulty Difficulty { get; set; }

        [JsonProperty("_difficultyRank")]
        public int DifficultyRank { get; set; }

        [JsonProperty("_noteJumpMovementSpeed")]
        public Single NoteJumpMovementSpeed { get; set; }

        [JsonProperty("_noteJumpStartBeatOffset")]
        public int NoteJumpStartBeatOffset { get; set; }

        [JsonProperty("_beatmapData")]
        public BeatmapData BeatmapData { get; set; }

        //unity assets format only
        [JsonIgnore]
        public AssetsPtr BeatmapDataPtr { get; set; }

        //[JsonIgnore]
        //public BeatmapSaveData BeatmapSaveData { get; set; }

        public DifficultyBeatmap()
        { }

        public DifficultyBeatmap(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            Difficulty = (Difficulty)reader.ReadInt32();
            DifficultyRank = reader.ReadInt32();
            NoteJumpMovementSpeed = reader.ReadSingle();
            NoteJumpStartBeatOffset = reader.ReadInt32();
            BeatmapDataPtr = new AssetsPtr(reader);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write((int)Difficulty);
            writer.Write(DifficultyRank);
            writer.Write(NoteJumpMovementSpeed);
            writer.Write(NoteJumpStartBeatOffset);
            BeatmapDataPtr.Write(writer);            
        }
    }

}