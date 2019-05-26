using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public struct UPtr
{
    //public UPtr()
    //{ }
    //public UPtr(UInt32 fileID, UInt64 pathID)
    //{
    //    FileID = fileID;
    //    PathID = pathID;
    //}
    public int FileID;
    public Int64 PathID;

    public void Write(AlignedStream s)
    {
        s.Write(this);
    }
}

    public class UPtrOf<T>
    {
        public UPtr Ptr;
        public T Item;
    }

