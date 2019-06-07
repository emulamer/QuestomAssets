using Emulamer.Utils;
using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsManager : IDisposable
    {
        private IApkFileIO _apkReader;
        public Dictionary<string, Type> ClassNameToTypes { get; private set; } = new Dictionary<string, Type>();
        public bool UseTempFiles {get; private set;}
        //private Apkifier _apkReader;
        //TODO: to make it useful for anything else, shouldn't be an APK path, should be something that implements an interface
        public AssetsManager(IApkFileIO apkReader, Dictionary<string, Type> classNameToTypes, bool lazyLoad = false, bool useTempFiles = false)
        {
            _apkReader = apkReader;
            LazyLoad = lazyLoad;
            ClassNameToTypes = classNameToTypes;
            UseTempFiles = useTempFiles;
        }
        public void SetReader(IApkFileIO reader)
        {
            _apkReader = reader;
        }
        private Dictionary<string, AssetsFile> _openAssetsFiles = new Dictionary<string, AssetsFile>();
        public bool LazyLoad { get; private set; }
        public List<AssetsFile> OpenFiles
        {
            get
            {
                return _openAssetsFiles.Values.ToList();
            }
        }

        private Dictionary<string, string> _tempFileMap = new Dictionary<string, string>();
        public AssetsFile GetAssetsFile(string assetsFilename)
        {
            if (_openAssetsFiles.ContainsKey(assetsFilename))
                return _openAssetsFiles[assetsFilename];
            Stream stream = _apkReader.ReadCombinedAssets(BSConst.KnownFiles.AssetsRootPath + assetsFilename);
            if (UseTempFiles)
            {
                var tempFile = Path.GetTempFileName();
                var fileStream = File.Open(tempFile, FileMode.Create, FileAccess.ReadWrite);
                using (stream)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
                _tempFileMap.Add(assetsFilename, tempFile);
                stream = fileStream;
                stream.Seek(0, SeekOrigin.Begin);
            }            
            AssetsFile assetsFile = new AssetsFile(this, assetsFilename, stream, BSConst.GetAssetTypeMap());
            _openAssetsFiles.Add(assetsFilename, assetsFile);
            return assetsFile;
        }

        public void WriteAllOpenAssets(IApkFileIO apkWriter)
        {
            foreach (var assetsFileName in _openAssetsFiles.Keys.ToList())
            {
                var assetsFile = _openAssetsFiles[assetsFileName];
                if (assetsFile.HasChanges)
                {
                    Log.LogMsg($"File {assetsFileName} has changed, writing new contents.");
                    try
                    {
                        apkWriter.WriteCombinedAssets(assetsFile, BSConst.KnownFiles.AssetsRootPath + assetsFileName);
                    }
                    catch (Exception ex)
                    {
                        Log.LogErr($"Exception writing assets file {assetsFileName}", ex);
                        throw;
                    }
                }
                //_openAssetsFiles.Remove(assetsFileName);
            }
        }

        private Dictionary<string, MonoScriptObject> _classCache = new Dictionary<string, MonoScriptObject>();
        public MonoScriptObject GetScriptObject(string className)
        {
            if (_classCache.ContainsKey(className))
                return _classCache[className];
            var ggm = GetAssetsFile("globalgamemanagers.assets");
            var classObj = ggm.FindAsset<MonoScriptObject>(x => x.Object.Name == className);
            if (classObj == null)
                throw new Exception($"Unable to find a script with type name {className}!");
            _classCache.Add(className, classObj.Object);
            return classObj.Object;
        }

        public IObjectInfo<T> MassFirstAsset<T>(Func<IObjectInfo<T>, bool> filter, bool deepSearch = true) where T : AssetsObject
        {
            return MassFindAssets(filter, deepSearch).First();
        }

        public IObjectInfo<T> MassFirstOrDefaultAsset<T>(Func<IObjectInfo<T>, bool> filter, bool deepSearch = true) where T : AssetsObject
        {
            return MassFindAssets(filter, deepSearch).FirstOrDefault();
        }

        private IEnumerable<IObjectInfo<T>> MassFindAssets<T>(AssetsFile file, Func<IObjectInfo<T>, bool> filter, bool deepSearch, List<AssetsFile> searched, List<AssetsFile> deepSearched) where T : AssetsObject
        {
            if (!searched.Contains(file))
            {
                searched.Add(file);
                foreach (var res in file.FindAssets(filter))
                    yield return res;
            }
            if (deepSearch)
            {
                foreach (var extFile in file.Metadata.ExternalFiles)
                {
                    var ext = GetAssetsFile(extFile.FileName);
                    if (!deepSearched.Contains(ext))
                    {
                        deepSearched.Add(ext);
                        foreach (var res in MassFindAssets(ext, filter, deepSearch, searched, deepSearched))
                            yield return res;
                    }

                }
            }
            yield break;
        }
        public IEnumerable<IObjectInfo<T>> MassFindAssets<T>(Func<IObjectInfo<T>, bool> filter, bool deepSearch = true) where T : AssetsObject
        {
            List<AssetsFile> searched = new List<AssetsFile>();
            List<AssetsFile> deepSearched = new List<AssetsFile>();
            //do a quick pass on the open assts files so that if we find one and stop at that, we don't iterate them all
            foreach (var file in _openAssetsFiles.Values.ToList())
                foreach (var res in file.FindAssets(filter))
                    yield return res;

            if (deepSearch)
            {
                //now do a deep search
                foreach (var file in _openAssetsFiles.Values.ToList())
                    foreach (var res in MassFindAssets(file, filter, true, searched, deepSearched))
                        yield return res;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                    // TODO: dispose managed state (managed objects).
                    foreach (var assetsFileName in _openAssetsFiles.Keys.ToList())
                    {
                        try
                        {
                            if (UseTempFiles)
                            {
                                var assetFile = _openAssetsFiles[assetsFileName];
                                assetFile.BaseStream.Close();
                                assetFile.BaseStream.Dispose();
                                try
                                {
                                    File.Delete(_tempFileMap[assetsFileName]);
                                }
                                catch (Exception ex)
                                { Log.LogErr("Failed to delete a temp file.", ex); }
                            }
                            _openAssetsFiles.Remove(assetsFileName);
                        }
                        catch
                        { }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AssetsManager()
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
