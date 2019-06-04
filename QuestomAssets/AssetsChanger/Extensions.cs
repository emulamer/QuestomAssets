using Emulamer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestomAssets.AssetsChanger
{
    public static class Extensions
    {
        public static UPtr ToUPtr(this AssetsChanger.PPtr ptr)
        {
            return new UPtr()
            {
                FileID = ptr.FileID,
                PathID = ptr.PathID
            };
        }

        public static AssetsChanger.PPtr ToAssetsPtr(this UPtr ptr)
        {
            return new AssetsChanger.PPtr(ptr.FileID, ptr.PathID);
        }

        public static Stream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return bytes;
        }

        
    }
}
