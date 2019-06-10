using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QuestomAssets.Utils;

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

        public static RawPtr ToRawPtr<T>(this ISmartPtr<T> ptr) where T : AssetsObject
        {
            return new RawPtr(ptr.FileID, ptr.PathID);
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
        public static ExclusionMode GetExclusionMode(this IEnumerable<CloneExclusion> exclus, object targetObject, PropertyInfo propInfo)
        {
            if (exclus == null)
                return ExclusionMode.None;

            ExclusionMode mode = ExclusionMode.None;
            foreach (var e in exclus)
            {
                if (e.Matches(targetObject, propInfo))
                {
                    if (e.Mode > mode)
                        mode = e.Mode;
                }
            }
            return mode;
        }

        public static Array RemoveAt(this Array source, int index)
        {
            char[] b = new char[50];
            var dest = Array.CreateInstance(source.GetType().GetElementType(), source.Length - 1);
            
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
    }
}
