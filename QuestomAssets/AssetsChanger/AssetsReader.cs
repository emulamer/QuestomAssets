using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsReader : BinaryReader
    {
        private int _startPosition = 0;
        private bool _align = true;
        public AssetsReader(Stream s, bool align = true) : base(s, UTF8Encoding.ASCII, true)
        {
            _startPosition = (int)s.Position;
            _align = align;
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

        public void AlignTo(int bytes = 0)
        {
            int count = bytes - (Position % bytes);
            if (count > 0 && count < bytes)
                ReadBytes(count);
        }
        public Guid ReadGuid()
        {
            if (_align)
                AlignTo(4);
            var bytes = ReadBytes(16);
            return new Guid(bytes);
        }

        public Int32 ReadBEInt32()
        {
            var bytes = ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public UInt32 ReadBEUInt32()
        {
            var bytes = ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public Int64 ReadBEInt64()
        {
            var bytes = ReadBytes(8);
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public Int16 ReadBEInt16()
        {
            var bytes = ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public UInt16 ReadBEUInt16()
        {
            var bytes = ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        //public void SeekObjectData(int offset)
        //{
        //    BaseStream.Seek(ObjectDataOffset + offset, SeekOrigin.Begin);
        //}
        /// <summary>
        /// Seeks to the position relative to the AssetReader
        /// </summary>
        public void Seek(int offset)
        {
            BaseStream.Seek(_startPosition + offset, SeekOrigin.Begin);
        }

        //public int ObjectDataPosition
        //{
        //    get { return Position - ObjectDataOffset; }
        //}

        /// <summary>
        /// Gets the position relative to the AssetReader
        /// </summary>
        public int Position
        {
            get { return (int)BaseStream.Position - _startPosition; }
        }

        public override string ReadString()
        {
            int length = ReadInt32();
            byte[] stringBytes = ReadBytes(length);
            if (_align)
                AlignTo(4);
            return System.Text.Encoding.ASCII.GetString(stringBytes);            
        }


        public override int ReadInt32()
        {
            return base.ReadInt32();
        }

        public override ulong ReadUInt64()
        {
            if (_align)
                AlignTo(4);
            return base.ReadUInt64();
        }

        public override float ReadSingle()
        {
            return base.ReadSingle();
        }

        public override uint ReadUInt32()
        {
            return base.ReadUInt32();
        }

        public override long ReadInt64()
        {
            if (_align)
                AlignTo(4);
            return base.ReadInt64();
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
