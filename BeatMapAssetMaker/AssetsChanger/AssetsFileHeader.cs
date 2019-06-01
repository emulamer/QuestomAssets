using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BeatmapAssetMaker.AssetsChanger
{ 
    public class AssetsFileHeader
    {
        /// <summary>
        /// Return the size of the header which is a constant value
        /// </summary>
        public Int32 HeaderSize {
            get
            {
                return 4 + 4 + 4 + 4 + 1 + 3 ;
            }
        }

        public Int32 MetadataSize { get; set; }

        public Int32 FileSize { get; set; }

        public Int32 Version { get; set; }

        public Int32 ObjectDataOffset { get; set; }

        public bool IsBigEndian { get; set; }


        public AssetsFileHeader(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            MetadataSize = reader.ReadBEInt32();
            FileSize = reader.ReadBEInt32();
            Version = reader.ReadBEInt32();
            ObjectDataOffset = reader.ReadBEInt32();
            IsBigEndian = reader.ReadBoolean();
            //padding apparently
            reader.ReadBytes(3);
        }

        public void Write(AssetsWriter writer)
        {
            writer.WriteBEInt32(MetadataSize);
            writer.WriteBEInt32(FileSize);
            writer.WriteBEInt32(Version);
            writer.WriteBEInt32(ObjectDataOffset);
            writer.Write(IsBigEndian);
            writer.Write(new byte[3]);
        }

    }
}
