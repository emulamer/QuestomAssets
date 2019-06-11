using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestomAssets.AssetsChanger;
using System.ComponentModel;

namespace QuestomAssets.BeatSaber
{
    public class DifficultyBeatmapSet : INotifyPropertyChanged
    {

        //unity asset format only
        [JsonIgnore]
        public ISmartPtr<AssetsObject> BeatmapCharacteristic { get; set; }

        //json format only
        [JsonProperty("_beatmapCharacteristicName")]
        public Characteristic BeatmapCharacteristicName { get; set; }

        [JsonProperty("_difficultyBeatmaps")]
        public System.Collections.ObjectModel.ObservableCollection<DifficultyBeatmap> DifficultyBeatmaps { get; private set; } = new System.Collections.ObjectModel.ObservableCollection<DifficultyBeatmap>();

        public DifficultyBeatmapSet()
        { }

        public DifficultyBeatmapSet(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            Parse(assetsFile, owner, reader);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Parse(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            BeatmapCharacteristic = SmartPtr<AssetsObject>.Read(assetsFile, owner, reader);
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