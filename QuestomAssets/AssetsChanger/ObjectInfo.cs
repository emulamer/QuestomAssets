using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QuestomAssets.AssetsChanger
{

    public interface IObjectInfo<out T>
    {
        Int64 ObjectID { get; set; }
        AssetsFile ParentFile { get; }
        T Object { get; }
        Int32 DataOffset { get; set; }
        Int32 DataSize { get; set; }
        Int32 TypeIndex { get; }
        void Write(AssetsWriter writer);
        AssetsType Type { get; }
        bool IsNew { get; }
        IObjectInfo<T> Clone();
    }
    public class ObjectInfo<T> : IObjectInfo<T> where T: AssetsObject
    {
        public Int64 ObjectID { get; set; } = -1;
        public Int32 DataOffset { get; set; } = -1;
        public Int32 DataSize { get; set; } = -1;
        public Int32 TypeIndex { get; private set; } = -1;
        public bool IsNew
        {
            get
            {
                return (DataOffset < 0 || DataSize < 0);
            }
        }
        public AssetsFile ParentFile { get; set; }

        public AssetsType Type
        {
            get
            {
                return ParentFile.Metadata.Types[TypeIndex];
            }
        }

        public IObjectInfo<T> Clone()
        {
            T newObj = null;
            using (var ms = new MemoryStream())
            {
                ObjectInfo<T> newInfo = (ObjectInfo<T>)ObjectInfo<T>.FromTypeIndex(ParentFile, TypeIndex, null);
                newInfo.DataOffset = 0;
                newInfo.ObjectID = 0;
                using (var writer = new AssetsWriter(ms))
                {
                    Object.Write(writer);
                }
                newInfo.DataSize = (int)ms.Length;
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new AssetsReader(ms))
                {
                    newObj = (T)Activator.CreateInstance(typeof(T), newInfo, reader);
                }
                newInfo.DataOffset = -1;
                newInfo.DataSize = -1;
                newInfo._object = newObj;
            }
            return (IObjectInfo<T>)newObj.ObjectInfo;
        }

        private ObjectInfo()
        { }

        private ObjectInfo(Int64 objectID, Int32 dataOffset, Int32 dataSize, Int32 typeIndex, AssetsFile parentFile, T assetsObject)
        {
            ObjectID = ObjectID;
            DataOffset = dataOffset;
            DataSize = dataSize;
            TypeIndex = typeIndex;
            ParentFile = parentFile;
            _object = assetsObject;
        }

        internal static IObjectInfo<AssetsObject> Parse(AssetsFile file, AssetsReader reader)
        {
            var objectID = reader.ReadInt64();
            var dataOffset = reader.ReadInt32();
            var dataSize = reader.ReadInt32();
            var typeIndex = reader.ReadInt32();
            var obji = FromTypeIndex(file, typeIndex, null);
            obji.ObjectID = objectID;
            obji.DataOffset = dataOffset;
            obji.DataSize = dataSize;
            return obji;
        }

        public static IObjectInfo<AssetsObject> FromClassID(AssetsFile assetsFile, int classID, AssetsObject assetsObject)
        {
            var typeIndex = assetsFile.Metadata.Types.IndexOf(assetsFile.Metadata.Types.First(x => x.ClassID == classID));
            return FromTypeIndex(assetsFile, typeIndex, assetsObject);
        }
        public static IObjectInfo<AssetsObject> FromTypeHash(AssetsFile assetsFile, Guid typeHash, AssetsObject assetsObject)
        {
            var typeIndex = assetsFile.Metadata.Types.IndexOf(assetsFile.Metadata.Types.First(x => x.TypeHash == typeHash));
            return FromTypeIndex(assetsFile, typeIndex, assetsObject);
        }

        public static IObjectInfo<AssetsObject> FromTypeIndex(AssetsFile assetsFile, int typeIndex, AssetsObject assetsObject)
        {
            var type = GetObjectType(assetsFile, typeIndex);
            var genericInfoType = typeof(ObjectInfo<>).MakeGenericType(type);
            var constructor = genericInfoType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Int64), typeof(int), typeof(int), typeof(int), typeof(AssetsFile), type }, null);

            var genericOI = (IObjectInfo<AssetsObject>)constructor.Invoke(new object[] { (Int64)(-1), (int)-1, (int)-1, typeIndex, assetsFile, assetsObject});
            
            return genericOI;
        }

        private static Type GetObjectType(AssetsFile assetsFile, int typeIndex)
        {
            Type type = null;
            var objectType = assetsFile.Metadata.Types[typeIndex];
            switch (objectType.ClassID)
            {
                case AssetsConstants.ClassID.MonoBehaviourScriptType:
                    var found = assetsFile.Manager.MassFirstOrDefaultAsset<MonoScriptObject>(x => x.Object.PropertiesHash == objectType.TypeHash);
                    
                    if (found != null && assetsFile.Manager.ClassNameToTypes.ContainsKey(found.Object.ClassName))
                    {
                        Type assetObjectType = assetsFile.Manager.ClassNameToTypes[found.Object.ClassName];
                        if (!assetObjectType.IsSubclassOf(typeof(MonoBehaviourObject)))
                        {
                            throw new ArgumentException("Types provided in scriptHashToTypes must be a subclass of AssetsMonoBehaviourObject.");
                        }
                        type = assetObjectType;
                    }
                    else
                    {
                        type = typeof(MonoBehaviourObject);
                    }
                    break;
                case AssetsConstants.ClassID.AudioClipClassID:
                    type = typeof(AudioClipObject);
                    break;
                case AssetsConstants.ClassID.Texture2DClassID:
                    type = typeof(Texture2DObject);
                    break;
                case AssetsConstants.ClassID.GameObjectClassID:
                    type = typeof(GameObject);
                    break;
                case AssetsConstants.ClassID.TextAssetClassID:
                    type = typeof(TextAsset);
                    break;
                case AssetsConstants.ClassID.SpriteClassID:
                    type = typeof(SpriteObject);
                    break;
                case AssetsConstants.ClassID.MonoScriptType:
                    type = typeof(MonoScriptObject);
                    break;
                default:
                    type = typeof(AssetsObject);
                    break;
            }
            return type;
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(DataOffset);
            writer.Write(DataSize);
            writer.Write(TypeIndex);
        }

        //public PPtr LocalPtrTo
        //{
        //    get
        //    {
        //        return new PPtr(0, ObjectID);
        //    }
        //}

        private void LoadObject()
        {
            using (var reader = ParentFile.GetReaderAtDataOffset())
            {
                _object = (T) Activator.CreateInstance(typeof(T), this, reader);
            }
        }

        private T _object;
        public T Object
        {
            get
            {
                if (_object == null)
                {
                    if (DataOffset < 0 || DataSize < 0)
                    {
                        throw new Exception("Object is not set and DataOffset or DataSize is not set!");
                    }
                    else
                    {
                        LoadObject();
                    }
                }
                    
                return _object;
            }
            set
            {
                
                //don't think this should be settable
                throw new Exception("see if this ever gets hit");
                _object = value;
            }
        }


    }
}
