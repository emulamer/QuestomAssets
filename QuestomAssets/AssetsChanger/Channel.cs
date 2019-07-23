using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class Channel
    {
        public Channel(AssetsReader reader)
        {
            Parse(reader);
        }

        public byte Stream { get; set; }
        public byte Offset { get; set; }
        public byte Format { get; set; }
        public byte Dimension { get; set; }

        public void Parse(AssetsReader reader)
        {
            Stream = reader.ReadByte();
            Offset = reader.ReadByte();
            Format = reader.ReadByte();
            Dimension = reader.ReadByte();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(Stream);
            writer.Write(Offset);
            writer.Write(Format);
            writer.Write(Dimension);
        }
    }
}
