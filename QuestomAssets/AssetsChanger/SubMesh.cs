using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class Submesh
    {
        public Submesh()
        { }

        public Submesh(AssetsReader reader)
        {
            Parse(reader);
        }

        public void Parse(AssetsReader reader)
        {
            FirstByte = reader.ReadUInt32();
            IndexCount = reader.ReadUInt32();
            Topology = reader.ReadInt32();
            BaseVertex = reader.ReadUInt32();
            FirstVertex = reader.ReadUInt32();
            VertexCount = reader.ReadUInt32();
            LocalAABB = new AABB(reader);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(FirstByte);
            writer.Write(IndexCount);
            writer.Write(Topology);
            writer.Write(BaseVertex);
            writer.Write(FirstVertex);
            writer.Write(VertexCount);
            LocalAABB.Write(writer);
        }

        public UInt32 FirstByte { get; set; }
        public UInt32 IndexCount { get; set; }
        public int Topology { get; set; }
        public UInt32 BaseVertex { get; set; }
        public UInt32 FirstVertex { get; set; }
        public UInt32 VertexCount { get; set; }
        public AABB LocalAABB { get; set; }
    }
}
