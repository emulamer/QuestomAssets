using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class TextAsset : AssetsObject, IHaveName
    {
        public string Name { get; set; }
        public string Script { get; set; }

        public TextAsset(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.TextAssetClassID)
        {
        }

        public TextAsset(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
            ParseDetails(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);            
        }

        protected void ParseDetails(AssetsReader reader)
        {
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
    }
}
