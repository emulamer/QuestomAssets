using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class GameObject : AssetsObject, IHaveName
    {
        public GameObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.GameObjectClassID)
        {
            IsActive = true;
        }

        public GameObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
            ParseDetails(reader);
        }

        //protected void UpdateType(AssetsMetadata metadata, Guid scriptHash, PPtr monoscriptTypePtr)
        //{
        //    base.UpdateType(metadata, scriptHash);
        //}

        //public GameObject()
        //{ IsActive = true; }

        protected GameObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        { IsActive = true; }


        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);            
        }

        protected void ParseDetails(AssetsReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Component.Add(new PPtr(reader));
            Layer = reader.ReadUInt32();
            Name = reader.ReadString();
            Tag = reader.ReadUInt16();
            IsActive = reader.ReadBoolean();            
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Component.Count);
            foreach (var c in Component)
                c.Write(writer);
            writer.Write(Layer);
            writer.Write(Name);
            writer.Write(Tag);
            writer.Write(IsActive);
        }

        public override void Write(AssetsWriter writer)
        {
            WriteBase(writer);
        } 

        public List<PPtr> Component { get; set; }

        public UInt32 Layer { get; set; }

        public string Name { get; set; }

        public UInt16 Tag { get; set; }

        public bool IsActive { get; set; }
    }
}
