using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{

    public interface IObjectInfo<out T>
    {
        int ObjectID { get; }
    }
    public class ObjectInfo<T> where T: AssetsObject
    {
        //TODO: pass this in somehow
        private static Dictionary<Guid, Type> _scriptHashToTypes = BeatSaber.BSConst.GetAssetTypeMap();


        public Int64 ObjectID { get; set; }
        public Int32 DataOffset { get; set; }
        public Int32 DataSize { get; set; }
        public Int32 TypeIndex { get; set; }

        public AssetsFile ParentFile { get; set; }

        

        private ObjectInfo()
        { }

        public ObjectInfo(Int64 objectID, Int32 dataOffset, Int32 dataSize, Int32 typeIndex)
        {
            ObjectID = ObjectID;
            DataOffset = dataOffset;
            DataSize = dataSize;
            TypeIndex = typeIndex;
        }

        public static IObjectInfo<AssetsObject> Parse(AssetsMetadata meta, AssetsReader reader)
        {
            var objectID = reader.ReadInt64();
            var dataOffset = reader.ReadInt32();
            var dataSize = reader.ReadInt32();
            var typeIndex = reader.ReadInt32();
            var type = GetObjectType(meta, typeIndex);
            var genericInfoType = typeof(ObjectInfo<>).MakeGenericType(type);
            var genericOI = (Activator.CreateInstance(genericInfoType, objectID, dataOffset, dataSize, typeIndex);
            return (IObjectInfo<AssetsObject>)genericOI;
        }

        private static Type GetObjectType(AssetsMetadata meta, int typeIndex)
        {
            Type type = null;
            var objectType = meta.Types[typeIndex];
            switch (objectType.ClassID)
            {
                case AssetsConstants.ClassID.MonoBehaviourScriptType:
                    if (_scriptHashToTypes.ContainsKey(objectType.ScriptHash))
                    {
                        Type assetObjectType = _scriptHashToTypes[objectType.ScriptHash];
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
                default:
                    type = typeof(AssetsObject);
                    break;
            }
            return type;
        }

        //private static void ParseData(AssetsReader reader)
        //{
        //    ObjectID = reader.ReadInt64();
        //    DataOffset = reader.ReadInt32();
        //    DataSize = reader.ReadInt32();
        //    TypeIndex = reader.ReadInt32();
        //}

        public void Write(AssetsWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(DataOffset);
            writer.Write(DataSize);
            writer.Write(TypeIndex);
        }

        public PPtr LocalPtrTo
        {
            get
            {
                return new PPtr(0, ObjectID);
            }
        }

        private void LoadObject()
        {
            using (AssetsReader reader = new AssetsReader(ParentFile.BaseStream))
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

            }
            set
            {

            }
        }


    }
}
