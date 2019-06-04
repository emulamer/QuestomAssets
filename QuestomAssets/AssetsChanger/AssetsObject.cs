using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    [SmartPtrAware]
    public class AssetsObject
    {
        [JsonIgnore]
        public virtual byte[] Data { get; set; }

        [JsonIgnore]
        public ObjectInfo ObjectInfo { get; set; }

        public AssetsObject()
        { }

        public AssetsObject(AssetsMetadata metadata, int classID)
        {
            ObjectInfo = new ObjectInfo()
            {
                TypeIndex = metadata.Types.IndexOf(metadata.Types.First(x => x.ClassID == classID))
            };
        }


        public AssetsObject(AssetsMetadata metadata, Guid scriptHash)
        {
            ObjectInfo = new ObjectInfo()
            {
                TypeIndex = metadata.GetTypeIndexFromScriptHash(scriptHash)
            };
        }

        public AssetsObject(ObjectInfo objectInfo)
        {
            ObjectInfo = objectInfo;
        }

        public AssetsObject(ObjectInfo objectInfo, AssetsReader reader)
        {
            ObjectInfo = objectInfo;
            Parse(reader);
            ParseDetails(reader);
        }

        protected virtual void Parse(AssetsReader reader)
        {
            reader.Seek(ObjectInfo.DataOffset);
        }

        private void ParseDetails(AssetsReader reader)
        {
            Data = reader.ReadBytes(ObjectInfo.DataSize);
        }
        
        protected virtual void WriteBase(AssetsWriter writer)
        {

        }

        public virtual void Write(AssetsWriter writer)
        {
            writer.Write(Data);
        }

        public int GetSize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsWriter writer = new AssetsWriter(ms))
                {
                    Write(writer);
                }
                return (int)ms.Length;
            }
        }
    }
}
