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
        public static ISmartPtr<T> PtrFrom<T>(this T assetObject, AssetsObject owner) where T : AssetsObject
        {
            return new SmartPtr<T>(owner, assetObject);
        }

        public static ISmartPtr<T> PtrFrom<T>(this IObjectInfo<T> objectInfo, AssetsObject owner) where T : AssetsObject
        {
            return new SmartPtr<T>(owner, objectInfo);
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

        public static void Write<T>(this ISmartPtr<T> ptr, AssetsWriter writer) where T: AssetsObject
        {
            if (ptr == null)
            {
                writer.Write((Int32)0);
                writer.Write((Int64)0);
            }
            else
            {
                ptr.WritePtr(writer);
            }
        }
        
    }
}
