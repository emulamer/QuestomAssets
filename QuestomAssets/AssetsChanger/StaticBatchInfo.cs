using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class StaticBatchInfo
    {
        public StaticBatchInfo(AssetsReader reader)
        {
            Parse(reader);
        }

        public StaticBatchInfo()
        {
        }

        public void Parse(AssetsReader reader)
        {
            FirstSubMesh = reader.ReadUInt16();
            SubMeshCount = reader.ReadUInt16();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(FirstSubMesh);
            writer.Write(SubMeshCount);
        }
        public UInt16 FirstSubMesh { get; set; }
        public UInt16 SubMeshCount { get; set; }
    }
}
