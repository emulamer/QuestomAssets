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
    }
    public class ObjectInfo<T> : IObjectInfo<T> where T: AssetsObject
    {
        public Int64 ObjectID { get; set; }
        public Int32 DataOffset { get; set; }
        public Int32 DataSize { get; set; }
        public Int32 TypeIndex { get; private set; }

        public AssetsFile ParentFile { get; set; }

        public AssetsType Type
        {
            get
            {
                return ParentFile.Metadata.Types[TypeIndex];
            }
        }
 
        

        private ObjectInfo()
        { }

        public ObjectInfo(Int64 objectID, Int32 dataOffset, Int32 dataSize, Int32 typeIndex, AssetsFile parentFile)
        {
            ObjectID = ObjectID;
            DataOffset = dataOffset;
            DataSize = dataSize;
            TypeIndex = typeIndex;
            ParentFile = parentFile;
        }

        internal static IObjectInfo<AssetsObject> Parse(AssetsFile file, AssetsReader reader)
        {
            var objectID = reader.ReadInt64();
            var dataOffset = reader.ReadInt32();
            var dataSize = reader.ReadInt32();
            var typeIndex = reader.ReadInt32();
            var obji = FromTypeIndex(file, typeIndex);
            obji.ObjectID = objectID;
            obji.DataOffset = dataOffset;
            obji.DataSize = dataSize;
            return obji;
        }

        public static IObjectInfo<AssetsObject> FromClassID(AssetsFile assetsFile, int classID)
        {
            var typeIndex = assetsFile.Metadata.Types.IndexOf(assetsFile.Metadata.Types.First(x => x.ClassID == classID));
            return FromTypeIndex(assetsFile, typeIndex);
        }
        public static IObjectInfo<AssetsObject> FromTypeHash(AssetsFile assetsFile, Guid typeHash)
        {
            var typeIndex = assetsFile.Metadata.Types.IndexOf(assetsFile.Metadata.Types.First(x => x.TypeHash == typeHash));
            return FromTypeIndex(assetsFile, typeIndex);
        }

        public static IObjectInfo<AssetsObject> FromTypeIndex(AssetsFile assetsFile, int typeIndex)
        {
            var type = GetObjectType(assetsFile, typeIndex);
            var genericInfoType = typeof(ObjectInfo<>).MakeGenericType(type);
            var genericOI = (IObjectInfo<AssetsObject>)Activator.CreateInstance(genericInfoType, (Int64)0, (int)0, (int)0, typeIndex, assetsFile);
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
                    LoadObject();
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
