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
        
        private Apkifier _apk;
        //TODO: to make it useful for anything else, shouldn't be an APK path, should be something that implements an interface
        public AssetsManager(Apkifier apk, bool lazyLoad = false)
        {
            _apk = apk;
            LazyLoad = lazyLoad;
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
    }
}
