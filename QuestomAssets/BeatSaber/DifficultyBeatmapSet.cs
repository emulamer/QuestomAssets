using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.BeatSaber
{
    public class DifficultyBeatmapSet
    {

        //unity asset format only
        [JsonIgnore]
        public ISmartPtr<AssetsObject> BeatmapCharacteristic { get; set; }

        //json format only
        [JsonProperty("_beatmapCharacteristicName")]
        public Characteristic BeatmapCharacteristicName { get; set; }

        [JsonProperty("_difficultyBeatmaps")]
        public List<DifficultyBeatmap> DifficultyBeatmaps { get; private set; } = new List<DifficultyBeatmap>();

        public DifficultyBeatmapSet()
        { }

        public DifficultyBeatmapSet(AssetsFile assetsFile, AssetsReader reader)
        {
            Parse(assetsFile, reader);
        }

        private void Parse(AssetsFile assetsFile, AssetsReader reader)
        {
            BeatmapCharacteristic = SmartPtr<AssetsObject>.Read(assetsFile, reader);
            DifficultyBeatmaps = reader.ReadArrayOf(x => new DifficultyBeatmap(assetsFile, x));
        }

        public void Write(AssetsWriter writer)
        {
            BeatmapCharacteristic.Write(writer);

            writer.Write(DifficultyBeatmaps.Count);
            foreach (var db in DifficultyBeatmaps)
            {
                db.Write(writer);
            }           
        }

    }


}