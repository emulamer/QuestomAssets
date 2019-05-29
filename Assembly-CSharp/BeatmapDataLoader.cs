using System;
using System.Collections.Generic;


public class BeatmapDataLoader
{


    public static BeatmapSaveData GetBeatmapSaveDataFromJson(string json)
    {
        return BeatmapSaveData.DeserializeFromJSONString(json);
        
    }
}
