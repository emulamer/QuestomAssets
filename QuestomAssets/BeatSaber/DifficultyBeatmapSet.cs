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
        public ISmartPtr<BeatmapCharacteristicObject> BeatmapCharacteristic { get; set; }

        //json format only
        [JsonProperty("_beatmapCharacteristicName")]
        public Characteristic BeatmapCharacteristicName { get; set; }

        [JsonProperty("_difficultyBeatmaps")]
        public List<DifficultyBeatmap> DifficultyBeatmaps { get; private set; } = new List<DifficultyBeatmap>();

        public DifficultyBeatmapSet()
        { }

        public DifficultyBeatmapSet(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            Parse(assetsFile, owner, reader);
        }

        private void Parse(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            BeatmapCharacteristic = SmartPtr<BeatmapCharacteristicObject>.Read(assetsFile, owner, reader);
            DifficultyBeatmaps = reader.ReadArrayOf(x => new DifficultyBeatmap(assetsFile, owner, x));
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