using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsReader : BinaryReader
    {
        public int MetadataOffset { get; set; }

        public int ObjectDataOffset { get; set; }

        public AssetsReader(Stream s) : base(s, UTF8Encoding.UTF8, true)
        { }

        public void AlignToMetadata(int bytes)
        {
            int count = bytes - ((Position - MetadataOffset) % bytes);
            if (count > 0 && count < bytes)
                ReadBytes(count);
        }
        public void AlignToObjectData(int bytes)
        {
            int count = bytes - ((Position - ObjectDataOffset) % bytes);
            if (count > 0 && count < bytes)
                ReadBytes(count);
        }
        public void AlignTo(int bytes)
        {
            int count = bytes - (Position % bytes);
            if (count > 0 && count < bytes)
                ReadBytes(count);
        }
        public bool IsDataAligned(int bytes)
        {
            return ((Position - ObjectDataOffset) % bytes) == 0;
        }

        public string ReadCStr()
        {
            List<byte> bytes = new List<byte>();
            byte b = ReadByte();
            while(b != 0)
            {
                bytes.Add(b);
                b = ReadByte();
            }
            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }

        public Guid ReadGuid()
        {
            var bytes = ReadBytes(16);
            return new Guid(bytes);
        }

        public Int32 ReadBEInt32()
        {
            var bytes = ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public void SeekObjectData(int offset)
        {
            BaseStream.Seek(ObjectDataOffset + offset, SeekOrigin.Begin);
        }

        public int ObjectDataPosition
        {
            get { return Position - ObjectDataOffset; }
        }

        public int Position
        {
            get { return (int)BaseStream.Position; }
        }

        public new string ReadString()
        {
            int length = ReadInt32();
            byte[] stringBytes = ReadBytes(length);
            return System.Text.Encoding.UTF8.GetString(stringBytes);
        }

        public byte[] ReadArray()
        {
            int count = ReadInt32();
            return ReadBytes(count);
        }

        public List<T> ReadArrayOf<T>(Func<AssetsReader,T> objectCreator)
        {
            List<T> list = new List<T>();
            int count = ReadInt32();
            for (int i = 0; i < count; i++)
            {
                list.Add(objectCreator(this));
            }
            return list;
        }
    }
}
