using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

 
    //will need to do mapping from name to UID
    public class DifficultyBeatmapSetSO
    {
        public DifficultyBeatmapSetSO()
        {
            _difficultyBeatmaps = new List<DifficultyBeatmapSO>();
        }

        [JsonIgnore]
        public UPtr _beatmapCharacteristic;

        public List<DifficultyBeatmapSO> _difficultyBeatmaps { get; private set; }

        public void Write(AlignedStream s)
        {
            _beatmapCharacteristic.Write(s);
            s.Write(_difficultyBeatmaps.Count);
            foreach (var db in _difficultyBeatmaps)
            {
                db.Write(s);
            }
        }


        public Characteristic _beatmapCharacteristicName;


}

