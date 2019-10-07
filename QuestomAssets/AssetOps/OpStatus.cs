using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetOps
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OpStatus
    {
        Queued,
        Started,
        Complete,
        Failed
    }
}
