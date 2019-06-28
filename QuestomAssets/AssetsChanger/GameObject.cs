using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public sealed class GameObject : AssetsObject, IHaveName
    {
        public GameObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.GameObjectClassID)
        {
            IsActive = true;
        }

        public GameObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected GameObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        { IsActive = true; }


        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Components.Add(SmartPtr<AssetsObject>.Read(ObjectInfo.ParentFile, this, reader));
            Layer = reader.ReadUInt32();
            Name = reader.ReadString();
            Tag = reader.ReadUInt16();
            IsActive = reader.ReadBoolean();
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Components.Count);
            foreach (var c in Components)
                c.Write(writer);
            writer.Write(Layer);
            writer.Write(Name);
            writer.Write(Tag);
            writer.Write(IsActive);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
        }

        public List<ISmartPtr<AssetsObject>> Components { get; set; } = new List<ISmartPtr<AssetsObject>>();

        public UInt32 Layer { get; set; }

        public string Name { get; set; }

        public UInt16 Tag { get; set; }

        public bool IsActive { get; set; }
    }
}
