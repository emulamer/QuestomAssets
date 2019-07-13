using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class PackedSingleVector
    {
        public PackedSingleVector()
        { }

        public PackedSingleVector(AssetsReader reader)
        {
            Parse(reader);
        }

        public void Parse(AssetsReader reader)
        {
            NumItems = reader.ReadUInt32();
            Range = reader.ReadSingle();
            Start = reader.ReadSingle();
            Data = reader.ReadArrayOf(r =>r.ReadByte()).ToArray();
            BitSize = reader.ReadByte();
            reader.AlignTo(4);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(NumItems);
            writer.Write(Range);
            writer.Write(Start);
            writer.WriteArray(Data);
            writer.Write(BitSize);
            writer.AlignTo(4);
        }

        public UInt32 NumItems { get; set; }
        public Single Range { get; set; }
        public Single Start { get; set; }
        public byte[] Data { get; set; }
        public byte BitSize { get; set; }
    }
}
