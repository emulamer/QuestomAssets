using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsObject
    {
        public virtual byte[] Data { get; set; }

        public AssetsObjectInfo ObjectInfo { get; set; }

        public AssetsObject(AssetsObjectInfo objectInfo)
        {
            ObjectInfo = objectInfo;
        }

        public AssetsObject(AssetsObjectInfo objectInfo, AssetsReader reader)
        {
            ObjectInfo = objectInfo;
            Parse(reader);
        }

        protected virtual void Parse(AssetsReader reader)
        {
            reader.SeekObjectData(ObjectInfo.DataOffset);
            Data = reader.ReadBytes(ObjectInfo.DataSize);
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
