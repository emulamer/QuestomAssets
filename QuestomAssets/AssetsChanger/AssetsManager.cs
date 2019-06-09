using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;


namespace QuestomAssets.AssetsChanger
{
    public class AssetsManager
    {
        public Dictionary<string, Type> ClassNameToTypes { get; private set; } = new Dictionary<string, Type>();
        private IAssetsFileProvider _fileProvider;
        

        public AssetsManager(IAssetsFileProvider fileProvider, Dictionary<string, Type> classNameToTypes, bool lazyLoad = false, bool forceLoadAllFiles = false)
        {
            _fileProvider = fileProvider;
            LazyLoad = lazyLoad;
            ClassNameToTypes = classNameToTypes;
            ForceLoadAllFiles = forceLoadAllFiles;
        }
        private Dictionary<string, AssetsFile> _openAssetsFiles = new Dictionary<string, AssetsFile>();
        public bool LazyLoad { get; private set; }
        public bool ForceLoadAllFiles { get; private set; }
        public List<AssetsFile> OpenFiles
        {
            get
            {
                return _openAssetsFiles.Values.ToList();
            }
        }

        public AssetsFile GetAssetsFile(string assetsFilename)
        {
            if (_openAssetsFiles.ContainsKey(assetsFilename))
                return _openAssetsFiles[assetsFilename];
            AssetsFile assetsFile = new AssetsFile(this, assetsFilename, _fileProvider.ReadCombinedAssets(assetsFilename), false);
            _openAssetsFiles.Add(assetsFilename, assetsFile);
            assetsFile.Load();
            return assetsFile;
        }

        public void WriteAllOpenAssets()
        {
            foreach (var assetsFileName in _openAssetsFiles.Keys.ToList())
            {
                var assetsFile = _openAssetsFiles[assetsFileName];
                if (assetsFile.HasChanges)
                {
                    Log.LogMsg($"File {assetsFileName} has changed, writing new contents.");
                    try
                    {
                        _fileProvider.WriteCombinedAssets(assetsFile, BSConst.KnownFiles.AssetsRootPath + assetsFileName);
                    }
                    catch (Exception ex)
                    {
                        Log.LogErr($"Exception writing assets file {assetsFileName}", ex);
                        throw;
                    }
                }
                _openAssetsFiles.Remove(assetsFileName);
            }
        }

        private Dictionary<string, MonoScriptObject> _classCache = new Dictionary<string, MonoScriptObject>();
        private Dictionary<Guid, MonoScriptObject> _hashClassCache = new Dictionary<Guid, MonoScriptObject>();
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
        public MonoScriptObject GetScriptObject(Guid propertiesHash)
        {
            if (_hashClassCache.ContainsKey(propertiesHash))
                return _hashClassCache[propertiesHash];
            AssetsFile ggm = null;
            
            //todo: no BSConst in here
            if (_fileProvider.FileExists(BSConst.KnownFiles.AssetsRootPath + "globalgamemanagers"))
            {
                ggm = GetAssetsFile("globalgamemanagers");
            }
           
            IObjectInfo<MonoScriptObject> classObj = null;

            if (ggm != null)
                classObj = ggm.FindAsset<MonoScriptObject>(x => x.Object.PropertiesHash == propertiesHash);
           
            if (classObj == null)
            {
                //todo: no BSConst in here
                if (_fileProvider.FileExists(BSConst.KnownFiles.AssetsRootPath + "globalgamemanagers.assets"))
                {
                    ggm = GetAssetsFile("globalgamemanagers.assets");
                    classObj = ggm.FindAsset<MonoScriptObject>(x => x.Object.PropertiesHash == propertiesHash);
                }
            }

            if (classObj == null)
            {
                Log.LogErr($"WARNING! Did not find the monoscript object for {propertiesHash}!");
                //throw new Exception($"Unable to find a script with type hash {propertiesHash}!");
                return null;
            }
            
            _hashClassCache.Add(propertiesHash, classObj.Object);
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
    }
}
