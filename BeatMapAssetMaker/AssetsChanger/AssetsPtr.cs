using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsPtr
    {

        public AssetsPtr(UPtr uptr)
        {
            FileID = uptr.FileID;
            PathID = uptr.PathID;
        }
        public int FileID { get; private set; }
        public Int64 PathID { get; private set; }

        public static bool operator == (AssetsPtr a, AssetsPtr b)
        {
            return AreEqual(a, b);
        }

        public static bool operator != (AssetsPtr a, AssetsPtr b)
        {
            return !AreEqual(a, b);
        }

        private static bool AreEqual(AssetsPtr a, AssetsPtr b)
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;
            return (a.PathID == b.PathID && a.FileID == b.FileID);
        }

        public AssetsPtr() {
            FileID = 0;
            PathID = 0;
        }
        public AssetsPtr(int fileID, Int64 pathID)
        {
            FileID = fileID;
            PathID = pathID;
        }

        public AssetsPtr(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            FileID = reader.ReadInt32();
            PathID = reader.ReadInt64();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(FileID);
            writer.Write(PathID);
        }
    }
}
