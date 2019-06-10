using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsWriter : BinaryWriter
    {
        private int _startPosition = 0;

        public AssetsWriter(Stream s) : base(s, ASCIIEncoding.ASCII, true)
        {
            _startPosition = (int)s.Position;
        }

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
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
                Write(bytes);
            }
            AlignTo(4);
        }

        public override void Write(Int32 value)
        {
            base.Write(value);
        }

        public override void Write(float value)
        {
            base.Write(value);
        }

        public override void Write(uint value)
        {
            base.Write(value);
        }

        public override void Write(ulong value)
        {
            base.Write(value);
        }

        public override void Write(long value)
        {
            base.Write(value);
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
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
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
        public void WriteArray(byte[] bytes)
        {
            Write(bytes.Length);
            Write(bytes);
        }

        public void WriteArrayOf<T>(IList<T> list, Action<T, AssetsWriter> writerFunc)
        {
            int count = list.Count();
            Write(count);
            foreach (var x in list)
            {
                writerFunc(x, this);
            }
        }
        public void WriteChars(string chars)
        {
            base.Write(System.Text.Encoding.ASCII.GetBytes(chars));
        }

        public int Position
        {
            get
            {
                Flush();
                return (int)BaseStream.Position - _startPosition;
            }
        }

        public override long Seek(int offset, SeekOrigin origin)
        {
            return base.Seek(offset - _startPosition, origin);
        }

    }
}
