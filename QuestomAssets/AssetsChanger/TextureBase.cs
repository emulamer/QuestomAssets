using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class TextureBase : AssetsObject, IHaveName
    {
        public TextureBase(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        {
        }       

        public TextureBase(AssetsFile assetsFile, int classID) : base(assetsFile, classID)
        { }

        protected override void ParseBase(AssetsReader reader)
        {
            base.ParseBase(reader);
            Name = reader.ReadString();
            ForcedFallbackFormat = reader.ReadInt32();
            DownscaleFallback = reader.ReadBoolean();
            reader.AlignTo(4);            
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.Write(ForcedFallbackFormat);
            writer.Write(DownscaleFallback);
            writer.AlignTo(4);
        }


        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
        public override byte[] Data { get => throw new InvalidOperationException("Data cannot be accessed from this class!"); set => throw new InvalidOperationException("Data cannot be accessed from this class!"); }


        public string Name { get; set; }
        public Int32 ForcedFallbackFormat { get; set; }
        public bool DownscaleFallback { get; set; }

    }


}
