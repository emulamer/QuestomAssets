using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsObject
    {
        [JsonIgnore]
        public virtual byte[] Data { get; set; }

        [JsonIgnore]
        public IObjectInfo<AssetsObject> ObjectInfo { get; set; }

        public AssetsObject()
        { }

        public AssetsObject(AssetsFile assetsFile, int classID)
        {

            ObjectInfo = ObjectInfo<AssetsObject>.FromTypeIndex(assetsFile, assetsFile.Metadata.Types.IndexOf(assetsFile.Metadata.Types.First(x => x.ClassID == classID)));
            
        }


        public AssetsObject(AssetsFile assetsFile, Guid scriptHash)
        {
            ObjectInfo = ObjectInfo<AssetsObject>.FromTypeIndex(assetsFile, assetsFile.Metadata.GetTypeIndexFromScriptHash(scriptHash));
        }

        protected AssetsObject(IObjectInfo<AssetsObject> objectInfo)
        {
            ObjectInfo = objectInfo;
        }

        public AssetsObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader)
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
