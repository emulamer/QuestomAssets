using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MonoBehaviourObject : AssetsObject, IHaveName
    {
        public MonoBehaviourObject(AssetsFile assetsFile, Guid scriptHash, ISmartPtr<AssetsObject> monoscriptTypePtr) : base(assetsFile, scriptHash)
        {
            Enabled = 1;
            MonoscriptTypePtr = monoscriptTypePtr;
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
        public PPtr GameObjectPtr { get; set; } = new PPtr();

        [JsonIgnore]
        public int Enabled { get; set; } = 1;

        [JsonIgnore]
        public PPtr MonoscriptTypePtr { get; set; }

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
            GameObjectPtr = new PPtr(reader);
            Enabled = reader.ReadInt32();
            MonoscriptTypePtr = new PPtr(reader);
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
            GameObjectPtr.Write(writer);
            writer.Write(Enabled);
            MonoscriptTypePtr.Write(writer);
            writer.Write(Name);      
        }

        public override void Write(AssetsWriter writer)
        {
            WriteBase(writer);
            writer.Write(ScriptParametersData);
            writer.AlignTo(4);
        } 
    }
}
