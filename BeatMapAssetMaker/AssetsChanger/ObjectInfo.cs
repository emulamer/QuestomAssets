using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class ObjectInfo
    {
        public Int64 ObjectID { get; set; }
        public Int32 DataOffset { get; set; }
        public Int32 DataSize { get; set; }
        public Int32 TypeIndex { get; set; }

        public ObjectInfo()
        { }

        public ObjectInfo(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            ObjectID = reader.ReadInt64();
            DataOffset = reader.ReadInt32();
            DataSize = reader.ReadInt32();
            TypeIndex = reader.ReadInt32();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(DataOffset);
            writer.Write(DataSize);
            writer.Write(TypeIndex);
        }

        public PPtr LocalPtrTo
        {
            get
            {
                return new PPtr(0, ObjectID);
            }
        }
    }
}
