using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class RawPtr
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is RawPtr))
                return false;
            return this == (RawPtr)obj;
        }

        public override int GetHashCode()
        {
            return FileID.GetHashCode() ^ PathID.GetHashCode();
        }

        public int FileID { get; private set; }
        public Int64 PathID { get; private set; }

        public static bool operator ==(RawPtr a, RawPtr b)
        {
            return AreEqual(a, b);
        }

        public static bool operator !=(RawPtr a, RawPtr b)
        {
            return !AreEqual(a, b);
        }

        private static bool AreEqual(RawPtr a, RawPtr b)
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;
            return (a.PathID == b.PathID && a.FileID == b.FileID);
        }

        public RawPtr()
        {
            FileID = 0;
            PathID = 0;
        }
        public RawPtr(int fileID, Int64 pathID)
        {
            FileID = fileID;
            PathID = pathID;
        }

        public RawPtr(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            FileID = reader.ReadInt32();
            reader.AlignTo(4);
            PathID = reader.ReadInt64();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(FileID);
            writer.AlignTo(4);
            writer.Write(PathID);
        }
    }
}
