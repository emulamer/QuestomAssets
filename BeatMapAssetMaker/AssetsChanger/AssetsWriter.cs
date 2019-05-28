using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsWriter : BinaryWriter
    {
        //public int MetadataStartOffset { get; set; }
       // public int ObjectDataOffset { get; set; }
        public AssetsWriter(Stream s) : base(s, UTF8Encoding.UTF8, true)
        { }

        public void AlignTo(int bytes)
        {
            int count = bytes - (Position % bytes);
            if (count > 0 && count < bytes)
                Write(new byte[count]);
        }

        public void WriteHash(byte[] value)
        {
            if (value == null)
            { Write(new byte[16]); }
            else
            {                
                if (value.Length != 16)
                    throw new InvalidOperationException("Hash must be 16 bytes!");
                Write(value);
            }
        }
        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Write((int)0);
            }
            else
            {
                Write(value.Length);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
                Write(bytes);
            }
        }
        public void Write(Guid value)
        {
            byte[] bytes = value.ToByteArray();
            Write(bytes);
        }

        public void WriteCString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
                Write(bytes);
            }
            Write((byte)0);
        }

        public void WriteBEInt32(Int32 value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            Write(bytes);
        }

        //public void SeekObjectData(int offset)
        //{
        //    BaseStream.Seek(ObjectDataOffset + offset, SeekOrigin.Begin);
        //}

        public int Position
        {
            get
            {
                Flush();
                return (int)BaseStream.Position;
            }
        }
    }
}
