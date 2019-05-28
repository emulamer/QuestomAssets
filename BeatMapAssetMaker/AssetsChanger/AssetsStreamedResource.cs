using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsStreamedResource
    {
        public AssetsStreamedResource(string source, UInt64 offset, UInt64 size)
        {
            Source = source;
            Offset = offset;
            Size = size;
        }

        public AssetsStreamedResource(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            Source = reader.ReadString();
            reader.AlignToObjectData(4);
            Offset = reader.ReadUInt64();
            Size = reader.ReadUInt64();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(Source);
            writer.AlignTo(4);
            writer.Write(Offset);
            writer.Write(Size);
        }

        public string Source { get; private set; }
        public UInt64 Offset { get; private set; }
        public UInt64 Size { get; private set; }
    }
}
