using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BeatmapDataSO
{
    
    public System.String _jsonData; // 0x0000000C

    public System.Byte[] _signatureBytes; // 0x00000010
    public System.Byte[] _projectedData; // 0x00000014
    //public BeatmapData _beatmapData; // 0x00000018
    public System.Single _beatsPerMinute; // 0x0000001C
    public System.Single _shuffle; // 0x00000020
    public System.Single _shufflePeriod; // 0x00000024
    public System.Boolean _hasRequiredDataForLoad; // 0x00000028
    
    

}

