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

        public AssetsReader GetReaderAtDataOffset()
        {
            BaseStream.Seek(Header.ObjectDataOffset, SeekOrigin.Begin);
            return new AssetsReader(BaseStream);
        }

        

        public Stream BaseStream { get; private set; }

        public AssetsFile(AssetsManager manager, string assetsFileName, Stream assetsFileStream, Dictionary<string, Type> classNameToTypes)
        {
            Manager = manager;
            if (!assetsFileStream.CanSeek)
                throw new NotSupportedException("Stream must support seeking!");
            BaseStream = assetsFileStream;
            AssetsFileName = assetsFileName;

            assetsFileStream.Seek(0, SeekOrigin.Begin);

            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Header = new AssetsFileHeader(reader);
            }
            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Metadata = new AssetsMetadata(this);
                Metadata.Parse(reader);
            }
            assetsFileStream.Seek(Header.ObjectDataOffset, SeekOrigin.Begin);

            foreach (var ext in Metadata.ExternalFiles)
            {
                manager.GetAssetsFile(ext.FileName);
            }

            if (!manager.LazyLoad)
            {
                foreach (var oi in Metadata.ObjectInfos)
                {
                    var o = oi.Object;
                }
            }
        }
        public T CopyAsset<T>(T source) where T : AssetsObject
        {
            if (source.ObjectInfo.ParentFile != this)
                throw new NotImplementedException("Haven't implemented cloning objects to another file yet.");

            T newObj = null;
            using (var ms = new MemoryStream())
            {
                IObjectInfo<T> newInfo = (IObjectInfo<T>)ObjectInfo<T>.FromTypeIndex(this, source.ObjectInfo.TypeIndex);
                
                newInfo.DataSize = source.ObjectInfo.DataSize;
                //newInfo.ObjectID
                using (var writer = new AssetsWriter(ms))
                    source.Write(writer);
                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new AssetsReader(ms))
                {
                    newObj = (T)Activator.CreateInstance(typeof(T), newInfo, reader);
                }
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

        public IObjectInfo<T> FindAsset<T>(Func<IObjectInfo<T>, bool> filter) where T: AssetsObject
        {
            return FindAssets(filter).FirstOrDefault();
        }

        public IEnumerable<IObjectInfo<T>> FindAssets<T>(Func<IObjectInfo<T>, bool> filter) where T: AssetsObject
        {
            return Metadata.ObjectInfos.Where(x => typeof(IObjectInfo<T>).IsAssignableFrom(x.GetType()) && filter((IObjectInfo<T>)x)).Select(x => (x as IObjectInfo<T>));
        }

        public T GetAssetByID<T>(long objectID) where T : AssetsObject
        {
            return Metadata.ObjectInfos.Where(x => x is ObjectInfo<T> && x.ObjectID == objectID).Cast<ObjectInfo<T>>().FirstOrDefault()?.Object;
        }

        public IObjectInfo<T> GetObjectInfo<T>(int fileID, Int64 pathID)
        {
            if (fileID == 0)
            {
                var objInfo = Metadata.ObjectInfos.FirstOrDefault(x => x.ObjectID == pathID);
                if (objInfo == null)
                    throw new Exception($"Object info could not be found for path id {pathID} in file {AssetsFileName}");
                var objTypedInfo = objInfo as IObjectInfo<T>;
                if (objTypedInfo == null)
                    throw new Exception($"Object was the wrong type!  Pointer expected {typeof(T).Name}, target was actually {(objInfo.GetType().Name)}");
                return objTypedInfo;
            }
            else
            {
                var file = Metadata.ExternalFiles[fileID-1];
                var externFile = Manager.GetAssetsFile(file.FileName);
                return externFile.GetObjectInfo<T>(0, pathID);
            }
        }

        private void CleanupPtrs(IObjectInfo<AssetsObject> objectInfo)
        {

            //TODO implement for colections, go over this logic and make it right
            foreach (var ptr in _knownPointers.Where(x=> x.Owner == objectInfo.Object || x.Target == objectInfo.Object).ToList())
            {
                if (ptr.Owner == objectInfo.Object)
                {
                    //is this a problem?
                    Log.LogMsg($"Pointer is being removed that is owned by this file, cleaning up the target ref.  Owner type: {ptr.Owner.GetType().Name}");
                    if (ptr.Target != null)
                    {
                        //is this ok?
                        ptr.Target.ParentFile.RemovePtrRef(ptr);
                    }
                    ptr.Dispose();
                }
                else if (ptr.Target == objectInfo.Object)
                {
                    if (ptr.Owner != null)
                    {
                        Log.LogErr($"Pointer is still set to target an object of type {ptr.Target.GetType().Name} being removed that is still alive!  Owner type: {ptr.Owner.GetType().Name}.");
                        throw new Exception("Pointer owner is still using this object!");
                    }
                }
                
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

            CleanupPtrs(assetsObject.ObjectInfo);
        }

        
    }
}
