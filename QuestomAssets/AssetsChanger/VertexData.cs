using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class VertexData
    {
        /*
        public VertexData()
        { }
        public VertexData(AssetsReader reader)
        {
            Parse(reader);
        }

        public void Parse(AssetsReader reader)
        {
            VertexCount = reader.ReadUInt32();
            reader.ReadArrayOf(r => new Vector(r));
            Data = reader.ReadArray();
            reader.AlignTo(4);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(VertexCount);
            writer.WriteArrayOf(Channels, (o, w) => o.Write(w));
            writer.WriteArray(Data);
        }

        public UInt32 VertexCount { get; set; }
        public List<Vector> Channels { get; set; }
        public byte[] Data { get; set; }*/
    }
}
