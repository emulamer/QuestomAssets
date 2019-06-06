using Emulamer.Utils;
using QuestomAssets.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsManager
    {

        public Dictionary<string, Type> ClassNameToTypes { get; private set; } = new Dictionary<string, Type>();

        private Apkifier _apk;
        //TODO: to make it useful for anything else, shouldn't be an APK path, should be something that implements an interface
        public AssetsManager(Apkifier apk, Dictionary<string, Type> classNameToTypes, bool lazyLoad = false)
        {
            _apk = apk;
            LazyLoad = lazyLoad;
            ClassNameToTypes = classNameToTypes;
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

        public AssetsFile GetAssetsFile(string assetsFilename)
        {
            if (_openAssetsFiles.ContainsKey(assetsFilename))
                return _openAssetsFiles[assetsFilename];
            AssetsFile assetsFile = new AssetsFile(this, assetsFilename, _apk.ReadCombinedAssets(BSConst.KnownFiles.AssetsRootPath + assetsFilename), BSConst.GetAssetTypeMap());
            _openAssetsFiles.Add(assetsFilename, assetsFile);
            return assetsFile;
        }

        public void WriteAllOpenAssets()
        {
            foreach (var assetsFileName in _openAssetsFiles.Keys.ToList())
            {
                var assetsFile = _openAssetsFiles[assetsFileName];
                try
                {
                    _apk.WriteCombinedAssets(assetsFile, BSConst.KnownFiles.AssetsRootPath + assetsFileName);
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Exception writing assets file {assetsFileName}", ex);
                    throw;
                }
                _openAssetsFiles.Remove(assetsFileName);
            }
        }

        private Dictionary<string, MonoScriptObject> _classCache = new Dictionary<string, MonoScriptObject>();
        public MonoScriptObject GetScriptObject(string className)
        {
            if (_classCache.ContainsKey(className))
                return _classCache[className];
            var classObj = MassFirstOrDefaultAsset<MonoScriptObject>(x => x.Object.Name == className);
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
            yield return null;
        }
        public IEnumerable<IObjectInfo<T>> MassFindAssets<T>(Func<IObjectInfo<T>, bool> filter, bool deepSearch = true) where T : AssetsObject
        {
            List<AssetsFile> searched = new List<AssetsFile>();
            List<AssetsFile> deepSearched = new List<AssetsFile>();
            //do a quick pass on the open assts files so that if we find one and stop at that, we don't iterate them all
            foreach (var file in _openAssetsFiles.Values)
                foreach (var res in file.FindAssets(filter))
                    yield return res;

            if (deepSearch)
            {
                //now do a deep search
                foreach (var file in _openAssetsFiles.Values)
                    foreach (var res in MassFindAssets(file, filter, true, searched, deepSearched))
                        yield return res;
            }
        }
    }
}
