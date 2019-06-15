using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MonoScriptObject : AssetsObject, IHaveName
    {
        public MonoScriptObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MonoScriptType)
        {
        }

        public MonoScriptObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        //public MonoScriptObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        //{ }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            Name = reader.ReadString();
            ExecutionOrder = reader.ReadInt32();
            PropertiesHash = reader.ReadGuid();
            ClassName = reader.ReadString();
            Namespace = reader.ReadString();
            AssemblyName = reader.ReadString();
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.Write(ExecutionOrder);
            writer.Write(PropertiesHash);
            writer.Write(ClassName);
            writer.Write(Namespace);
            writer.Write(AssemblyName);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
        }

        public string Name { get; set; }
        public Int32 ExecutionOrder { get; set; }
        public Guid PropertiesHash { get; set; }
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string AssemblyName { get; set; }

    }
}
