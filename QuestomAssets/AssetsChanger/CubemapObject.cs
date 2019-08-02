using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public sealed class CubemapObject : Texture2DObject, IHaveName
    {
        public CubemapObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public CubemapObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.CubeMapClassID)
        { }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            SourceTextures = reader.ReadArrayOf(x => (ISmartPtr<Texture2DObject>)SmartPtr<Texture2DObject>.Read(this.ObjectInfo.ParentFile, this, x));
        }
        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.WriteArrayOf(SourceTextures, (o, w) => o.Write(w));
        }


        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
        public override byte[] Data { get => throw new InvalidOperationException("Data cannot be accessed from this class!"); set => throw new InvalidOperationException("Data cannot be accessed from this class!"); }

        //not positive of this type
        public List<ISmartPtr<Texture2DObject>> SourceTextures;
        

    }


}
