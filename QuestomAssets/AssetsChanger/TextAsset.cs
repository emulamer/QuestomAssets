using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public sealed class TextAsset : AssetsObject, IHaveName
    {
        public string Name { get; set; }
        public string Script { get; set; }

        public TextAsset(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.TextAssetClassID)
        {
        }

        public TextAsset(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            Name = reader.ReadString();
            Script = reader.ReadString();
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.Write(Script);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
        }

        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
        public override byte[] Data { get => throw new InvalidOperationException("Data cannot be accessed from this class!"); set => throw new InvalidOperationException("Data cannot be accessed from this class!"); }
    }
}
