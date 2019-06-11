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

        public override event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs eventArgs)
        {
            PropertyChanged?.Invoke(this, eventArgs);
        }

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

        public override void Write(AssetsWriter writer)
        {
            WriteBase(writer);
        }

        public System.Collections.ObjectModel.ObservableCollection<ISmartPtr<AssetsObject>> Components { get; set; } = new System.Collections.ObjectModel.ObservableCollection<ISmartPtr<AssetsObject>>();

        public UInt32 Layer { get; set; }

        public string Name { get; set; }

        public UInt16 Tag { get; set; }

        public bool IsActive { get; set; }
    }
}
