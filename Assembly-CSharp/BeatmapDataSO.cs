using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BeatmapDataSO
{
    
    [JsonIgnore]
    public UPtr Ptr;
    public System.String _jsonData;
    public System.Byte[] _signatureBytes;
    public System.Byte[] _projectedData;
    //public BeatmapData _beatmapData; // 0x00000018
    public System.Single _beatsPerMinute;
    public System.Single _shuffle;
    public System.Single _shufflePeriod;
    public System.Boolean _hasRequiredDataForLoad;

    [JsonIgnore]
    public BeatmapSaveData _beatmapSaveData;

}

