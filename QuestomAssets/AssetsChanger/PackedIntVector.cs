using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class PackedIntVector
    {
        public UInt32 NumItems { get; set; }
        public byte[] Data { get; set; }
        public byte BitSize { get; set; }

        public PackedIntVector()
        { }
        public PackedIntVector(AssetsReader reader)
        {
            Parse(reader);
        }

        public void Parse(AssetsReader reader)
        {
            NumItems = reader.ReadUInt32();
            Data = reader.ReadArray();
            reader.AlignTo(4);
            BitSize = reader.ReadByte();
            reader.AlignTo(4);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(NumItems);
            writer.WriteArray(Data);
            writer.AlignTo(4);
            writer.Write(BitSize);
            writer.AlignTo(4);
        }
            
    }
}
