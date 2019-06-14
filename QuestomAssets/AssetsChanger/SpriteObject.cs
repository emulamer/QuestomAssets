using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{

    /// <summary>
    /// !!!!!!!!!!!!THIS CLASS IS HORRIBLY INCOMPLETE AND JUST HACKED TOGETHER TO GET TO THE TEXTURE POINTER!!!!!!!!!!
    /// </summary>
    public class SpriteObject : AssetsObject, IHaveName
    {
        public SpriteObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.SpriteClassID)
        {
        }

        public SpriteObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        //public SpriteObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        //{ }
        
        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            int startPosition = reader.Position;
            Name = reader.ReadString();
            UnparsedData1 = reader.ReadBytes(100);
            Texture = SmartPtr<Texture2DObject>.Read(ObjectInfo.ParentFile, this, reader);
            int readLen = ObjectInfo.DataSize - (reader.Position - startPosition);
            UnparsedData2 = reader.ReadBytes(readLen);
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.Write(UnparsedData1);
            Texture.Write(writer);
            writer.Write(UnparsedData2);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
        }

        public string Name { get; set; }

        public byte[] UnparsedData1 { get; set; }

        public ISmartPtr<Texture2DObject> Texture { get; set; }

        public byte[] UnparsedData2 { get; set; }

    }
}
