using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections;

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
        T Clone();
        T DeepClone(Dictionary<AssetsObject, AssetsObject> clonedObjects = null);
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

        private void ClonePropsInObj(object curObj, AssetsObject parentObj, Dictionary<AssetsObject, AssetsObject> clonedObjects)
        {
            var updateProps = curObj.GetType().GetProperties().ToList();

            //remove any array properties that are a string or a value type
            updateProps.Where(x => x.PropertyType.IsArray && (x.PropertyType.GetElementType().IsValueType || x.PropertyType.GetElementType() == typeof(string)))
                .ToList().ForEach(x => updateProps.Remove(x));

            //look through any properties that are smart pointers, clone their targets and make a new pointer, then remove them from the list of props to update
            var propsToClone = updateProps.Where(x => typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(x.PropertyType)).ToList();
            foreach (var prop in propsToClone)
            {
                var baseVal = prop.GetValue(curObj, null);
                if (baseVal == null)
                    continue;
                var propObj = (prop.GetValue(curObj, null) as ISmartPtr<AssetsObject>).Target.Object;
                AssetsObject assignObject = null;
                //check if we have already cloned this object somewhere else in the tree

                assignObject = propObj.ObjectInfo.DeepClone(clonedObjects);
          

                prop.SetValue(curObj, MakeTypedPointer(parentObj, assignObject), null);
            }
            propsToClone.ForEach(x => updateProps.Remove(x));

            //look through any properties that lists of smart pointers, this code isn't ideal because it only actually supports things that have a default indexer
            //  I should clean this up to work better
            var listsToClone = updateProps.Where(x => typeof(IEnumerable<ISmartPtr<AssetsObject>>).IsAssignableFrom(x.PropertyType)).ToList();
            foreach (var listProp in listsToClone)
            {
                var listVal = listProp.GetValue(curObj, null) as IEnumerable<ISmartPtr<AssetsObject>>;

                if (listVal == null)
                    continue;
                if (listProp.PropertyType.IsArray)
                {
                    Array listArr = (Array)listVal;
                    for (int i = 0; i < listArr.Length; i++)
                    {
                        var ptrVal = listArr.GetValue(i) as ISmartPtr<AssetsObject>;
                        var clonedObj = ptrVal.Target.DeepClone(clonedObjects);
                        listArr.SetValue(clonedObj, i);
                    }
                }
                else
                {
                    var indexerProp = listVal.GetType().GetProperties().Where(x => x.Name == "Item").FirstOrDefault();

                    if (indexerProp == null)
                    {
                        throw new NotSupportedException($"Couldn't find the default indexer property on {curObj.GetType().Name}.{listProp.Name}!");
                    }
                    for (int i = 0; i < listVal.Count(); i++)
                    {
                        var ptrVal = indexerProp.GetValue(listVal, new object[] { i }) as ISmartPtr<AssetsObject>;
                        var clonedObj = ptrVal.Target.DeepClone(clonedObjects);
                        indexerProp.SetValue(listVal, MakeTypedPointer(parentObj as AssetsObject, clonedObj), new object[] { i });
                    }
                }
            }
            listsToClone.ForEach(x => updateProps.Remove(x));

            //look through any objects that are plain old lists of whatever.  this is to catch lists of "structs" that may have pointers in them
            var plainEnumerableToClone = updateProps.Where(x => !x.PropertyType.IsValueType && !(x.PropertyType == typeof(string)) && typeof(IEnumerable).IsAssignableFrom(x.PropertyType)).ToList();
            foreach (var enumProp in plainEnumerableToClone)
            {
                var listVal = enumProp.GetValue(curObj, null) as IEnumerable;

                if (listVal == null)
                    continue;

                foreach (var plainObj in listVal)
                {
                    //pass in the parent AssetsObject that was passed to us since that object will be the "owner", not the struct object
                    ClonePropsInObj(plainObj, parentObj, clonedObjects);
                }                
            }
            plainEnumerableToClone.ForEach(x => updateProps.Remove(x));
            //look through any "struct" type properties that may have pointers in them
            var plainObjToClone = updateProps.Where(x => !x.PropertyType.IsValueType && !(x.PropertyType == typeof(string)));
            foreach (var plainProp in plainObjToClone)
            {
                var objVal = plainProp.GetValue(curObj, null) as IEnumerable;
                if (objVal == null)
                    continue;

                foreach (var plainObj in objVal)
                {
                    //pass in the parent AssetsObject that was passed to us since that object will be the "owner", not the struct object
                    ClonePropsInObj(plainObj, parentObj, clonedObjects);
                }
            }
        }

        public T DeepClone(Dictionary<AssetsObject, AssetsObject> clonedObjects = null)
        {
            if (clonedObjects == null)
            {
                clonedObjects = new Dictionary<AssetsObject, AssetsObject>();
            }
            else
            {
                if (clonedObjects.ContainsKey(this.Object))
                {
                    return (T)clonedObjects[this.Object];
                }
            }
            var curObj = Clone();
            ParentFile.AddObject(curObj, true);

            clonedObjects.Add(this.Object, curObj);
            ClonePropsInObj(curObj, curObj, clonedObjects);
            return curObj;
        }

        private ISmartPtr<AssetsObject> MakeTypedPointer(AssetsObject owner, AssetsObject target)
        {
            var genericInfoType = typeof(SmartPtr<>).MakeGenericType(target.GetType());
            var constructor = genericInfoType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(AssetsObject), target.GetType() }, null);

            if (constructor == null)
                throw new Exception("Unable to find the proper SmartPtr constructor!");
            return (ISmartPtr<AssetsObject>)constructor.Invoke(new object[] { owner, target });
        }

        public T Clone()
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
            return (T)newObj;
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
                    var found = assetsFile.Manager.GetScriptObject(objectType.TypeHash);
                    
                    if (found != null && assetsFile.Manager.ClassNameToTypes.ContainsKey(found.ClassName))
                    {
                        Type assetObjectType = assetsFile.Manager.ClassNameToTypes[found.ClassName];
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
                case AssetsConstants.ClassID.MeshAssetClassID:
                    type = typeof(MeshObject);
                    break;
                case AssetsConstants.ClassID.MeshFilterClassID:
                    type = typeof(MeshFilterObject);
                    break;
                case AssetsConstants.ClassID.TransformClassID:
                    type = typeof(Transform);
                    break;
                case AssetsConstants.ClassID.RectTransformClassID:
                    type = typeof(RectTransform);
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
