using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using QuestomAssets.BeatSaber;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsFile : IDisposable
    {
        public AssetsManager Manager { get; private set; }
        public string AssetsFilename { get; private set; }
        public string AssetsRootPath { get; private set; }
        public IAssetsFileProvider FileProvider { get; private set; }
        public bool FileWasSplit { get; private set; }
        public Stream BaseStream { get; private set; }

        public AssetsFile(AssetsManager manager, IAssetsFileProvider fileProvider, string assetsRootPath, string assetsFileName, bool loadData = true)
        {
            Manager = manager;
            FileProvider = fileProvider;
            AssetsRootPath = assetsRootPath;
            AssetsFilename = assetsFileName;

            OpenBaseStream();
            
            BaseStream.Seek(0, SeekOrigin.Begin);
            using (AssetsReader reader = new AssetsReader(BaseStream, false))
            {
                Header = new AssetsFileHeader(reader);
            }

            if (Header.MetadataSize > Header.FileSize || Header.ObjectDataOffset < Header.MetadataSize || Header.Version != 17)
                throw new NotSupportedException($"{AssetsFilename} doesn't appear to be a valid assets file, or {Header.Version} is unsupported!");

            if (loadData)
                LoadData();
        }

        private void OpenBaseStream()
        {
            bool wasCombined;
            var assetsFileStream = FileProvider.ReadCombinedAssets(AssetsRootPath.CombineFwdSlash(AssetsFilename), out wasCombined);
            if (!assetsFileStream.CanSeek)
                throw new NotSupportedException("Stream must support seeking!");
            FileWasSplit = wasCombined;
            BaseStream = assetsFileStream;
        }

        private void CloseBaseStream()
        {
            BaseStream.Close();
            BaseStream.Dispose();
            BaseStream = null;
        }

        public void LoadData()
        {
            BaseStream.Seek(Header.HeaderSize, SeekOrigin.Begin);
            using (AssetsReader reader = new AssetsReader(BaseStream, false))
            {
                Metadata = new AssetsMetadata(this);
                Metadata.Parse(reader);
            }
            BaseStream.Seek(Header.ObjectDataOffset, SeekOrigin.Begin);

            if (Manager.ForceLoadAllFiles)
            {
                foreach (var ext in Metadata.ExternalFiles)
                {
                    Manager.GetAssetsFile(ext.FileName);
                }
            }
            if (!Manager.LazyLoad)
            {
                foreach (var oi in Metadata.ObjectInfos)
                {
                    var o = oi.Object;
                }
                BaseStream.Close();
                BaseStream.Dispose();
                BaseStream = null;
            }
        }

        public int GetOrAddExternalFileIDRef(AssetsFile targetFile)
        {
            ExternalFile file = Metadata.ExternalFiles.FirstOrDefault(x => x.FileName == targetFile.AssetsFilename);
            if (file == null)
            {
                file = new ExternalFile()
                {
                    AssetName = targetFile.AssetsFilename
                };
                Metadata.ExternalFiles.Add(file);
            }

            return Metadata.ExternalFiles.IndexOf(file) + 1;
        }

        private List<ISmartPtr<AssetsObject>> _knownPointers = new List<ISmartPtr<AssetsObject>>();

        public void AddPtrRef(ISmartPtr<AssetsObject> ptr)
        {
            if (!_knownPointers.Contains(ptr))
                _knownPointers.Add(ptr);
        }

        public void RemovePtrRef(ISmartPtr<AssetsObject> ptr)
        {
            _knownPointers.Remove(ptr);
        }

        public AssetsFileHeader Header { get; set; }

        public AssetsMetadata Metadata { get; set; }

        public AssetsReader GetReaderAtDataOffset()
        {
            BaseStream.Seek(Header.ObjectDataOffset, SeekOrigin.Begin);
            return new AssetsReader(BaseStream);
        }
        private bool _hasChanges = false;
        public bool HasChanges
        {
            get
            {
                if (_hasChanges)
                    return true;
                var newPtrs = _knownPointers.Where(x => x.Owner.ObjectInfo.ParentFile == this && x.IsNew).ToList();
                if (newPtrs.Any())
                {
                    return true;
                }
                var newObjInfos = Metadata.ObjectInfos.Where(x => x.IsNew).ToList();
                if (newObjInfos.Any())
                {
                    return true;
                }
                return false;
            }
            set
            {
                _hasChanges = true;
            }
        }

        public int GetOrCreateMatchingTypeIndex(AssetsType type)
        {
            var toFileType = Metadata.Types.Where(x => x.ClassID == type.ClassID 
            //only require hashes to match if it's a monobehaviour
            && (x.ClassID != AssetsConstants.ClassID.MonoBehaviourScriptType || (x.TypeHash == type.TypeHash && x.ScriptHash == type.ScriptHash)))
                //order by class ID and typehash matching first (prefer type hash)
                .OrderBy(x => ((x.ScriptHash == type.ScriptHash)?0:1) + ((x.TypeHash == type.TypeHash)?0:2))
                .FirstOrDefault();
            if (toFileType == null)
            {
                throw new NotSupportedException($"Target file does not seem to have a type that matches the source file's type.  Adding in other type references isn't supported yet.");
            }
            return Metadata.Types.IndexOf(toFileType);
        }

        //not enough ram on the quest... have to do it to the filesystem.  this should probably be an option to use memory
        //      assuming this lib keeps working on pc
        private bool UseFileCache = true;
        private List<string> tempFiles = new List<string>();

        private Stream GetTempStream()
        {
            if (UseFileCache)
            {
                string tempFile = Path.GetTempFileName();
                tempFiles.Add(tempFile);
                return File.Open(tempFile, FileMode.Create, FileAccess.ReadWrite);
            }
            else
            {
                return new MemoryStream();
            }
        }
        private void CleanupTempFiles()
        {
            foreach (string temp in tempFiles.ToList())
            {
                try
                {
                    File.Delete(temp);
                    tempFiles.Remove(temp);
                }
                catch (Exception ex)
                { Log.LogErr("Unable to delete temp file created during asset save!", ex); }
            }
        }

        public void Write()
        {
            try
            {
                using (Stream objectsMS = GetTempStream())
                {
                    using (Stream metaMS = GetTempStream())
                    {
                        using (AssetsWriter writer = new AssetsWriter(objectsMS))
                        {
                            int ctr = 0;
                            foreach (var obj in Metadata.ObjectInfos)
                            {
                                ctr++;
                                var offset = (int)objectsMS.Position;
                                obj.GetObjectForWrite().Write(writer);
                                writer.Flush();
                                obj.DataOffset = offset;
                                var origSize = obj.DataSize;
                                obj.DataSize = (int)(objectsMS.Position - obj.DataOffset);
                                writer.AlignTo(8);
                            }
                        }
                        using (AssetsWriter writer = new AssetsWriter(metaMS))
                        {
                            Metadata.Write(writer);
                        }

                        Header.FileSize = Header.HeaderSize + (int)objectsMS.Length + (int)metaMS.Length;
                        Header.ObjectDataOffset = Header.HeaderSize + (int)metaMS.Length;

                        int diff;
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
                        lock (this)
                        {
                            try
                            {
                                CloseBaseStream();


                                FileProvider.DeleteFiles(AssetsRootPath.CombineFwdSlash(AssetsFilename + ".split*"));

                                using (Stream outputStream = GetTempStream())
                                {
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

                                    outputStream.Seek(0, SeekOrigin.Begin);
                                    if (FileWasSplit)
                                    {
                                        int splitCtr = 0;
                                        byte[] buffer = new byte[1024 * 1024];
                                        do
                                        {
                                            Stream outFile = FileProvider.GetWriteStream($"{AssetsRootPath.CombineFwdSlash(AssetsFilename)}.split{splitCtr}");
                                            var readLen = (int)(outputStream.Length - outputStream.Position);
                                            if (readLen < buffer.Length)
                                            {
                                                outputStream.Read(buffer, 0, readLen);
                                                outFile.Write(buffer, 0, readLen);
                                                break;
                                            }
                                            outputStream.Read(buffer, 0, buffer.Length);
                                            outFile.Write(buffer, 0, buffer.Length);
                                            splitCtr++;
                                        } while (true);

                                    }
                                    else
                                    {
                                        Stream writeStream = FileProvider.GetWriteStream(AssetsRootPath.CombineFwdSlash(AssetsFilename));
                                        outputStream.CopyTo(writeStream);
                                    }
                                }

                                _hasChanges = false;
                                FileProvider.Save();
                                OpenBaseStream();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("CRITICAL: writing and reopening the file failed, the file is possibly destroyed and this object is in an unknown state.", ex);
                            }
                        }
                    }
                }
            }
            finally
            {
                CleanupTempFiles();
            }
        }

        public long GetNextObjectID()
        {
            long nextID = Metadata.ObjectInfos.Max(x => x.ObjectID);
            //if they're all negative, make a MORE negative one
            if (nextID < 0)
                return Metadata.ObjectInfos.Min(x => x.ObjectID) - 1;

            return nextID + 1;
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
        }

        public string GetFilenameForFileID(int fileID)
        {
            return Metadata.ExternalFiles[fileID - 1].FileName;
        }

        public int GetFileIDForFile(AssetsFile file)
        {
            return GetFileIDForFilename(file.AssetsFilename);
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
                {
                    Log.LogErr($"Object info could not be found for path id {pathID} in file {AssetsFilename}!!!!");
                    return null;
                    //throw new Exception($"Object info could not be found for path id {pathID} in file {AssetsFileName}");
                }
                var objTypedInfo = objInfo as IObjectInfo<T>;
                if (objTypedInfo == null)
                    throw new Exception($"Object was the wrong type!  Pointer expected {typeof(T).Name}, target was actually {(objInfo.GetType().GenericTypeArguments[0]?.Name)}");
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (BaseStream != null)
                    {
                        BaseStream.Close();
                        BaseStream.Dispose();
                        BaseStream = null;
                    }
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AssetsFile()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion





    }
}
