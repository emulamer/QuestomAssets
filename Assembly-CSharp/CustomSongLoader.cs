using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

public class CustomSongLoader
{
    public static BeatmapLevelDataSO LoadFromPath(string songPath)
    {
        string infoFile = Path.Combine(songPath, "info.dat");

        BeatmapLevelDataSO bml = null;
        using (var sr = new StreamReader(infoFile))
        {
            bml = JsonConvert.DeserializeObject<BeatmapLevelDataSO>(sr.ReadToEnd());
        }

        foreach (var d in bml._difficultyBeatmapSets)
        {
            foreach (var dm in d._difficultyBeatmaps)
            {
                var dataFile = Path.Combine(songPath, $"{dm._difficulty.ToString()}.dat");
                if (!File.Exists(dataFile))
                {
                    //oh no!
                    continue;
                }
                string jsonData;
                using (var sr = new StreamReader(dataFile))
                {
                    jsonData = sr.ReadToEnd();
                }
                dm._beatmapData = new BeatmapDataSO()
                {
                    _beatsPerMinute = bml._beatsPerMinute,
                    _shuffle = bml._shuffle,
                    _shufflePeriod = bml._shufflePeriod,
                    _jsonData = jsonData
                };
                dm._beatmapSaveData = JsonConvert.DeserializeObject<BeatmapSaveData>(jsonData);


            }
        }
        return bml;
    }
}

