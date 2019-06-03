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
        public AssetsFileHeader Header { get; set; }

        public AssetsMetadata Metadata { get; set; }

        public List<AssetsObject> Objects { get; set; }

        private Dictionary<Guid, Type> _scriptHashToTypes = new Dictionary<Guid, Type>();

        public AssetsFile(Stream assetsFileStream, Dictionary<Guid, Type> scriptHashToTypes)
        {
            _scriptHashToTypes = scriptHashToTypes;
            Objects = new List<AssetsObject>();

            assetsFileStream.Seek(0, SeekOrigin.Begin);

            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Header = new AssetsFileHeader(reader);
            }
            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Metadata = new AssetsMetadata(reader);
            }
            assetsFileStream.Seek(Header.ObjectDataOffset, SeekOrigin.Begin);
            using (AssetsReader reader = new AssetsReader(assetsFileStream))
            {
                foreach (var objectInfo in Metadata.ObjectInfos)
                {
                    AssetsObject assetsObject = ReadObject(reader, objectInfo);
                    Objects.Add(assetsObject);
                }
            }
        }

        private AssetsObject ReadObject(AssetsReader reader, ObjectInfo objectInfo)
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
                default:
                    assetsObject = new AssetsObject(objectInfo, reader);
                    break;
            }
            // is this needed?
            //reader.AlignToObjectData(8);
            return assetsObject;
        }

        public void Write(Stream outputStream)
        {
            MemoryStream objectsMS = new MemoryStream();
            MemoryStream metaMS = new MemoryStream();
            using (AssetsWriter writer = new AssetsWriter(objectsMS))
            {
                int ctr = 0;
                foreach (var obj in Objects)
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
            Objects.Add(assetsObject);
        }

        public string GetFilenameForFileID(int fileID)
        {
            return Metadata.ExternalFiles[fileID-1].FileName;
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
            return Objects.FirstOrDefault(x => x as T != null && (name == null || ((x as IHaveName)?.Name == name))) as T;
        }

        public List<T> FindAssets<T>(Func<T, bool> filter) where T: AssetsObject
        {
            return Objects.Where(x => x as T != null && filter(x as T)).Cast<T>().ToList();
        }

        public T GetAssetByID<T>(long objectID) where T : AssetsObject
        {
            return (T)Objects.FirstOrDefault(x => x.ObjectInfo.ObjectID == objectID);               
        }

        public void DeleteObject(AssetsObject assetsObject)
        {
            Objects.Remove(assetsObject);
            Metadata.ObjectInfos.Remove(assetsObject.ObjectInfo);
            //TODO: IDs need to be shored up at all?  reflection loop through all objects looking for refs?
        }

        /// <summary>
        /// This is REALLY unsafe, as it can break lots of things if it guesses they aren't in use but they are referenced from another file
        /// </summary>
        public void DeleteObjectRecursive(AssetsObject assetsObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This is unsafe and will only validate against objects in this file that have known/parsed types
        /// </summary>
        public bool ObjectInUseInFile(PPtr objectPtr)
        {
            //this is probably going to be slow
            foreach (AssetsObject obj in Objects)
            {
                foreach (var prop in obj.GetType().GetProperties().Where(x=>x.PropertyType == typeof(PPtr)))
                {
                    var ptr = prop.GetValue(obj, null) as PPtr;
                    if (ptr == null)
                        continue;
                    //if the pointer doesn't point to this file, skip it
                    if (ptr.FileID != 0)
                        continue;
                    if (ptr.PathID == objectPtr.PathID)
                        return true;
                }
            }
            return false;
        }

        
    }
}
