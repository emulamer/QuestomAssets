using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class StreamingInfo
    {
        public StreamingInfo()
        { }

        public StreamingInfo(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            offset = reader.ReadUInt32();
            size = reader.ReadUInt32();
            path = reader.ReadString();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(offset);
            writer.Write(size);
            writer.Write(path);
        }
        public UInt32 offset { get; set; }
        public UInt32 size { get; set; }
        public string path { get; set; }
    }
}
