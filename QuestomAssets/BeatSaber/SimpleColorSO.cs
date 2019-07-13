using Newtonsoft.Json;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public sealed class SimpleColorSO : MonoBehaviourObject, INeedAssetsMetadata
    {
        [JsonProperty("_color")]
        public Color Color { get; set; }

        public SimpleColorSO(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo, reader)
        {
            Parse(reader);
        }

        public SimpleColorSO(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        { }

        public SimpleColorSO(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("SimpleColorSO"))
        { }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            Color = new Color(reader);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
            Color.Write(writer);
        }

        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
        public override byte[] ScriptParametersData
        {
            get
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
            set
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
        }
    }
}
