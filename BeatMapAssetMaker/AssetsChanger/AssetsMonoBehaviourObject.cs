using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsMonoBehaviourObject : AssetsObject
    {
        public AssetsMonoBehaviourObject(AssetsMetadata metadata, Guid scriptHash, AssetsPtr monoscriptTypePtr) : base(metadata, scriptHash)
        {
            Enabled = 1;
            MonoscriptTypePtr = monoscriptTypePtr;
        }

        public AssetsMonoBehaviourObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
            ParseDetails(reader);
        }

        protected void UpdateType(AssetsMetadata metadata, Guid scriptHash, AssetsPtr monoscriptTypePtr)
        {
            base.UpdateType(metadata, scriptHash);
            MonoscriptTypePtr = monoscriptTypePtr;           
        }

        public AssetsMonoBehaviourObject()
        { Enabled = 1; }

        public AssetsMonoBehaviourObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { Enabled = 1; }

        //public AssetsMonoBehaviourObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        //{ }

        private byte[] _scriptParametersData;

        [JsonIgnore]
        public AssetsPtr GameObjectPtr { get; set; } = new AssetsPtr();

        [JsonIgnore]
        public int Enabled { get; set; } = 1;

        [JsonIgnore]
        public AssetsPtr MonoscriptTypePtr { get; set; }

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
            GameObjectPtr = new AssetsPtr(reader);
            Enabled = reader.ReadInt32();
            MonoscriptTypePtr = new AssetsPtr(reader);
            Name = reader.ReadString();
            reader.AlignToObjectData(4);
        }

        private void ParseDetails(AssetsReader reader)
        {
            var readLength = ObjectInfo.DataSize - (reader.Position - (reader.ObjectDataOffset + ObjectInfo.DataOffset));  
            ScriptParametersData = reader.ReadBytes(readLength);
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            GameObjectPtr.Write(writer);
            writer.Write(Enabled);
            MonoscriptTypePtr.Write(writer);
            writer.Write(Name);
            writer.AlignTo(4);            
        }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(ScriptParametersData);
        } 
    }
}
