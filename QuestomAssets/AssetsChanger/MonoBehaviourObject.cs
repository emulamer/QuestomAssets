using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MonoBehaviourObject : Component, IHaveName
    {
        public MonoBehaviourObject(AssetsFile assetsFile, MonoScriptObject scriptObject) : base(assetsFile, scriptObject.PropertiesHash)
        {
            Enabled = 1;
            MonoscriptTypePtr = new SmartPtr<MonoScriptObject>(this, (IObjectInfo<MonoScriptObject>)scriptObject.ObjectInfo);
        }

        public MonoBehaviourObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
            ParseDetails(reader);
        }

        //protected void UpdateType(AssetsMetadata metadata, Guid scriptHash, PPtr monoscriptTypePtr)
        //{
        //    base.UpdateType(metadata, scriptHash);
        //    MonoscriptTypePtr = monoscriptTypePtr;           
        //}

        //public MonoBehaviourObject()
        //{ Enabled = 1; }

        protected MonoBehaviourObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        { Enabled = 1; }

        //public AssetsMonoBehaviourObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        //{ }

        private byte[] _scriptParametersData;

        
        [JsonIgnore]
        public int Enabled { get; set; } = 1;

        [JsonIgnore]
        public ISmartPtr<MonoScriptObject> MonoscriptTypePtr { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual byte[] ScriptParametersData
        {
            get
            {
                return _scriptParametersData;
            }
            set
            {
                _scriptParametersData = value;
            }
        }

        [JsonIgnore]
        public override byte[] Data
        {
            get
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
            }
            set
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
            }
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            Enabled = reader.ReadInt32();
            MonoscriptTypePtr = SmartPtr<MonoScriptObject>.Read(ObjectInfo.ParentFile, this, reader);
            Name = reader.ReadString();
        }

        private void ParseDetails(AssetsReader reader)
        {
            var readLength = ObjectInfo.DataSize - (reader.Position - ObjectInfo.DataOffset);  
            ScriptParametersData = reader.ReadBytes(readLength);
            reader.AlignTo(4);
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Enabled);
            MonoscriptTypePtr.Write(writer);
            writer.Write(Name);      
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
            writer.Write(ScriptParametersData);
            writer.AlignTo(4);
        } 
    }
}
