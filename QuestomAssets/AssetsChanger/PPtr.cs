using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class PPtr
    {

        public PPtr(UPtr uptr)
        {
            FileID = uptr.FileID;
            PathID = uptr.PathID;
        }
        public int FileID { get; private set; }
        public Int64 PathID { get; private set; }

        public static bool operator == (PPtr a, PPtr b)
        {
            return AreEqual(a, b);
        }

        public static bool operator != (PPtr a, PPtr b)
        {
            return !AreEqual(a, b);
        }

        private static bool AreEqual(PPtr a, PPtr b)
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;
            return (a.PathID == b.PathID && a.FileID == b.FileID);
        }

        public PPtr() {
            FileID = 0;
            PathID = 0;
        }
        public PPtr(int fileID, Int64 pathID)
        {
            FileID = fileID;
            PathID = pathID;
        }

        public PPtr(AssetsReader reader)
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
