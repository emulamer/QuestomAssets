using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Emulamer.Utils;
using System.Reflection;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsFile
    {
        public AssetsManager Manager { get; private set; }
        public string AssetsFileName { get; private set; }

        public int GetOrAddExternalFileIDRef(AssetsFile targetFile)
        {
            ExternalFile file = Metadata.ExternalFiles.FirstOrDefault(x => x.FileName == targetFile.AssetsFileName);
            if (file == null)
            {
                file = new ExternalFile()
                {
                    AssetName = targetFile.AssetsFileName
                };
                Metadata.ExternalFiles.Add(file);
            }

            return Metadata.ExternalFiles.IndexOf(file) + 1;
        }

        private List<ISmartPtr<AssetsObject>> _knownPointers = new List<ISmartPtr<AssetsObject>>();

        public void AddPtrRef(ISmartPtr<AssetsObject> ptr)
        {
            _knownPointers.Add(ptr);
        }

        public void RemovePtrRef(ISmartPtr<AssetsObject> ptr)
        {
            _knownPointers.Remove(ptr);
        }

        public AssetsFileHeader Header { get; set; }

        public AssetsMetadata Metadata { get; set; }

       // public List<AssetsObject> Objects { get; set; }

            

        private Dictionary<Guid, Type> _scriptHashToTypes = new Dictionary<Guid, Type>();

        public Stream BaseStream { get; private set; }

        public AssetsFile(AssetsManager manager, string assetsFileName, Stream assetsFileStream, Dictionary<Guid, Type> scriptHashToTypes)
        {
            Manager = manager;
            if (!assetsFileStream.CanSeek)
                throw new NotSupportedException("Stream must support seeking!");
            BaseStream = assetsFileStream;
            AssetsFileName = assetsFileName;
            _scriptHashToTypes = scriptHashToTypes;
            //Objects = new List<AssetsObject>();

            assetsFileStream.Seek(0, SeekOrigin.Begin);

            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Header = new AssetsFileHeader(reader);
            }
            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Metadata = new AssetsMetadata(this, reader);
            }
            assetsFileStream.Seek(Header.ObjectDataOffset, SeekOrigin.Begin);
            using (AssetsReader reader = new AssetsReader(assetsFileStream))
            {
                foreach (var objectInfo in Metadata.ObjectInfos)
                {
                    AssetsObject assetsObject = ReadObject(reader, objectInfo);
                }
            }
        }

        private AssetsObject ReadObject(AssetsReader reader, IObjectInfo<AssetsObject> objectInfo)
        {
            AssetsObject assetsObject;
            var objectType = Metadata.Types[objectInfo.TypeIndex];
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
                        assetsObject = (AssetsObject)Activator.CreateInstance(assetObjectType, objectInfo, reader);
                    }
                    else
                    {
                        assetsObject = new MonoBehaviourObject(objectInfo, reader);
                    }
                    break;
                case AssetsConstants.ClassID.AudioClipClassID:
                    assetsObject = new AudioClipObject(objectInfo, reader);
                    break;
                case AssetsConstants.ClassID.Texture2DClassID:
                    assetsObject = new Texture2DObject(objectInfo, reader);
                    break;
                case AssetsConstants.ClassID.GameObjectClassID:
                    assetsObject = new GameObject(objectInfo, reader);
                    break;
                case AssetsConstants.ClassID.SpriteClassID:
                    assetsObject = new SpriteObject(objectInfo, reader);
                    break;
                default:
                    assetsObject = new AssetsObject(objectInfo, reader);
                    break;
            }
            // is this needed?
            //reader.AlignToObjectData(8);
            return assetsObject;
        }

        public T CopyAsset<T>(T source) where T : AssetsObject
        {
            T newObj = null;
            using (var ms = new MemoryStream())
            {
                IObjectInfo<AssetsObject> newInfo = ObjectInfo<AssetsObject>.FromTypeIndex(this, source.ObjectInfo.TypeIndex);
                newInfo.DataSize = source.ObjectInfo.DataSize;
                using (var writer = new AssetsWriter(ms))
                    source.Write(writer);
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new AssetsReader(ms))
                    newObj = (T)Activator.CreateInstance(typeof(T), newInfo, reader);


            }
            return newObj;
        }

        public void Write(Stream outputStream)
        {
            MemoryStream objectsMS = new MemoryStream();
            MemoryStream metaMS = new MemoryStream();
            using (AssetsWriter writer = new AssetsWriter(objectsMS))
            {
                int ctr = 0;
                foreach (var obj in Metadata.ObjectInfos.Select(x=> x.Object))
                {
                    ctr++;
                    obj.ObjectInfo.DataOffset = (int)objectsMS.Position;
                    obj.Write(writer);
                    writer.Flush();
                    var origSize = obj.ObjectInfo.DataSize;
                    obj.ObjectInfo.DataSize = (int)(objectsMS.Position - obj.ObjectInfo.DataOffset);
                    writer.AlignTo(8);
                }
            }
            using (AssetsWriter writer = new AssetsWriter(metaMS))
            {
                Metadata.Write(writer);
            }

            Header.FileSize = Header.HeaderSize + (int)objectsMS.Length + (int)metaMS.Length;
            Header.ObjectDataOffset = Header.HeaderSize + (int)metaMS.Length;

            int diff = 0;
            int alignment = 16; //or 32, I don't know which
            //data has to be at least 4096 inward from the start of the file
            if (Header.ObjectDataOffset < 4096)
            {
                diff = 4096 - Header.ObjectDataOffset;
            }
            else
            {
                diff = alignment - (Header.ObjectDataOffset % alignment);
                if (diff == alignment)
                    diff = 0;
            }


            if (diff > 0)
            {
                Header.ObjectDataOffset += diff;
                Header.FileSize += diff;
            }

            Header.MetadataSize = (int)metaMS.Length;
            objectsMS.Seek(0, SeekOrigin.Begin);
            metaMS.Seek(0, SeekOrigin.Begin);



            using (AssetsWriter writer = new AssetsWriter(outputStream))
            {
                Header.Write(writer);
            }
            metaMS.CopyTo(outputStream);


            if (diff > 0)
            {
                outputStream.Write(new byte[diff], 0, diff);
            }

            objectsMS.CopyTo(outputStream);
        }

        public long GetNextObjectID()
        {
            return Metadata.ObjectInfos.Max(x => x.ObjectID) + 1;
        }

        public void AddObject(AssetsObject assetsObject, bool assignNextObjectID = true)
        {
            if (assetsObject.ObjectInfo == null)
                throw new ArgumentException("ObjectInfo must be set!");

            if (assignNextObjectID)
            {
                assetsObject.ObjectInfo.ObjectID = GetNextObjectID();
            }
            if (assetsObject.ObjectInfo.ObjectID < 1)
                throw new ArgumentException("ObjectInfo.ObjectID must be > 0.");
            if (Metadata.ObjectInfos.Exists(x => x.ObjectID == assetsObject.ObjectInfo.ObjectID))
                throw new ArgumentException("ObjectInfo.ObjectID already exists in this file.");

            Metadata.ObjectInfos.Add(assetsObject.ObjectInfo);
            //Objects.Add(assetsObject);
        }

        public string GetFilenameForFileID(int fileID)
        {
            return Metadata.ExternalFiles[fileID - 1].FileName;
        }

        public int GetFileIDForFile(AssetsFile file)
        {
            return GetFileIDForFilename(file.AssetsFileName);
        }

        public int GetFileIDForFilename(string filename)
        {
            var file = Metadata.ExternalFiles.First(x => x.FileName == filename);
            if (file == null)
                throw new Exception($"Filename {filename} does not exist in the file list!");
            return Metadata.ExternalFiles.IndexOf(file)+1;
        }

        public T FindAsset<T>(string name = null) where T: AssetsObject
        {
            return Metadata.ObjectInfos.Where(x => x.Object is ObjectInfo<T>).Select(x => x as ObjectInfo<T>).FirstOrDefault()?.Object;
            //as T != null && (name == null || ((x as IHaveName)?.Name == name))) as T;
        }

        public List<T> FindAssets<T>(Func<T, bool> filter) where T: AssetsObject
        {
            return Metadata.ObjectInfos.Where(x => x.Object is ObjectInfo<T>).Select(x => (x as ObjectInfo<T>).Object).ToList();
        }

        public T GetAssetByID<T>(long objectID) where T : AssetsObject
        {
            return Metadata.ObjectInfos.Where(x => x is ObjectInfo<T> && x.ObjectID == objectID).Cast<ObjectInfo<T>>().FirstOrDefault()?.Object;
        }

        private void CleanupPtrs(IObjectInfo<AssetsObject> objectInfo)
        {
            foreach (var ptr in _knownPointers.Where(x=> x.Owner == objectInfo.Object || x.Target == objectInfo.Object).ToList())
            {
                if (ptr.Owner == objectInfo.Object)
                {
                    //TODO: is this ok?
                    Log.LogErr($"Pointer is still set on an object being removed, forcibly removing pointer from property {ptr.OwnerPropInfo.Name}");
                    ptr.OwnerPropInfo.GetSetMethod().Invoke(ptr.Owner, null);
                }
                else if (ptr.Target == objectInfo.Object)
                {
                    if (ptr.Owner != null)
                    {
                        Log.LogErr($"Pointer is still set to target an object being removed that is still assigned!  Owner type: {ptr.Owner.GetType().Name}, property: {ptr.OwnerPropInfo.Name}.  Forcibly unsetting it!");
                        ptr.OwnerPropInfo.GetSetMethod().Invoke(ptr.Owner, null);
                    }
                }
                ptr.Dispose();
            }
        }

        public void DeleteObject(AssetsObject assetsObject)
        {
            //TODO: implement dispose on these or something?
            var obj = Metadata.ObjectInfos.FirstOrDefault(x => x.Object == assetsObject);
            if (obj == null)
            {
                Log.LogErr("Tried to delete an object that wasn't part of this file");
                return;
            }
            
            Metadata.ObjectInfos.Remove(assetsObject.ObjectInfo);
            //TODO: IDs need to be shored up at all?  reflection loop through all objects looking for refs?
        }

        
    }
}
