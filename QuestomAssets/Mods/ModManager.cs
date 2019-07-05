using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using QuestomAssets;
using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Models;
using QuestomAssets.Mods;

namespace QuestomAssets.Mods
{
    public class ModManager
    {
        public bool HasChanges
        {
            get
            {
                return !ModConfig.Matches(_originalModConfig);
            }
        }

        public const string MOD_FILE_NAME = "beatonmod.json";
        internal ModConfig ModConfig { get; private set; }
        private ModConfig _originalModConfig;
        private QaeConfig _config;
        private Func<QuestomAssetsEngine> _getEngine;
        public ModManager(QaeConfig config, Func<QuestomAssetsEngine> getEngine)
        {
            _config = config;
            _getEngine = getEngine;
            ModConfig = ModConfig.Load(_config);
            _originalModConfig = ModConfig.Clone();
        }

        public ModDefinition LoadDefinitionFromProvider(IFileProvider provider)
        {
            try
            {
                return LoadModDef(provider);
            }
            catch (Exception ex)
            {
                Log.LogErr($"ModDefinition failed to load from provider.", ex);
                throw new Exception($"ModDefinition failed to load from provider.", ex);
            }
        }

        private ModDefinition LoadModDef(IFileProvider provider, string path = "")
        {
            ModDefinition def = null;
            if (!provider.FileExists(path.CombineFwdSlash(MOD_FILE_NAME)))
            {
                throw new Exception($"ModDefinition can't load zip file becase it does not contain {MOD_FILE_NAME}");
            }
            using (JsonTextReader jr = new JsonTextReader(new StreamReader(provider.GetReadStream(path.CombineFwdSlash(MOD_FILE_NAME)))))
            {
                def = new JsonSerializer().Deserialize<ModDefinition>(jr);
            }
            if (def == null)
                throw new Exception("ModDefinition failed to deserialize.");

            return def;
        }

        public void Save()
        {
            ModConfig.Save(_config);
            _originalModConfig.InstalledModIDs = ModConfig.InstalledModIDs.ToList();
        }

        private List<string> _deletedModIDs = new List<string>();

        private List<ModDefinition> _modCache;

        public void ResetCache()
        {
            _modCache = null;
        }

        public void DeleteMod(ModDefinition def)
        {
            if (ModConfig.InstalledModIDs.Contains(def.ID))
            {
                var ops = GetUninstallModOps(def);
                ops.ForEach(x => _getEngine().OpManager.QueueOp(x));
                ops.WaitForFinish();
                if (ops.Any(x=> x.Status == OpStatus.Failed))
                {
                    string err = "";
                    foreach (var failed in ops.Where(x=> x.Status == OpStatus.Failed))
                    {
                        err += failed.GetType().Name + " failed! " + failed.Exception ?? " \n";
                    }
                    throw new Exception("Uninstall of mod failed, cannot delete it! " + err);
                }
            }
            var defPath = _config.ModsSourcePath.CombineFwdSlash(def.ID);
            if (!_config.RootFileProvider.DirectoryExists(defPath))
            {
                Log.LogErr($"Tried to delete mod ID {def.ID} but it doesn't seem to exist at {defPath}.  Going to count it as already uninstalled.");
                return;
            }

            var qfo = new QueuedFileOp() { Type = QueuedFileOperationType.DeleteFolder, TargetPath = defPath };
            _getEngine().QueuedFileOperations.Add(qfo);
            _deletedModIDs.Add(def.ID);
            ResetCache();
        }

        public List<ModDefinition> Mods
        {
            get
            {
                if ( _modCache == null)
                {
                    _modCache = new List<ModDefinition>();
                    foreach (var file in _config.RootFileProvider.FindFiles($"{_config.ModsSourcePath}/*/{MOD_FILE_NAME}"))
                    {
                        //todo: don't like having the root constant here
                        var fulldir = file.GetDirectoryFwdSlash();
                        if (!_config.RootFileProvider.FileExists(fulldir.CombineFwdSlash(MOD_FILE_NAME)))
                        {
                            Log.LogErr($"Mod filename was found in path {file}, but nested paths arend supported so it will be skipped.");
                            continue;
                        }
                        try
                        {
                            var modDef = LoadModDef(_config.RootFileProvider, fulldir);
                            var dirName = fulldir.Substring(_config.ModsSourcePath.Length).Trim('/');
                            if (modDef.ID != dirName)
                            {
                                Log.LogErr($"Mod path {file} doesn't match the mod ID within the file, {modDef.ID}, skipping it.");
                                continue;
                            }

                            //check to see if the mod is queued to be deleted, don't include it if so
                            if (_deletedModIDs.Contains(modDef.ID))
                                continue;

                            if (ModConfig.InstalledModIDs.Contains(modDef.ID))
                                modDef.Status = ModStatusType.Installed;

                            _modCache.Add(modDef);
                        }
                        catch (Exception ex)
                        {
                            Log.LogErr($"Mod in directory {fulldir} failed to load", ex);
                        }
                    }
                }
                foreach (var modDef in _modCache)
                {
                    if (ModConfig.InstalledModIDs.Contains(modDef.ID))
                        modDef.Status = ModStatusType.Installed;
                    else
                        modDef.Status = ModStatusType.NotInstalled;
                }
                return _modCache;
            }
        }

        public List<AssetOp> GetInstallModOps(ModDefinition modDef)
        {
            List<AssetOp> ops = new List<AssetOp>();
            ModContext mc = new ModContext(_config.ModsSourcePath.CombineFwdSlash(modDef.ID), _config, _getEngine);
            //don't really like to put category specific logic in here, but whatever.
            if ( modDef.Category == ModCategory.Saber)
            {
                var otherSabers = Mods.Where(x => x.Category == ModCategory.Saber && x.ID != modDef.ID && x.Status == ModStatusType.Installed);
                foreach (var otherSaber in otherSabers)
                {
                    ops.AddRange(otherSaber.GetUninstallOps(mc));
                }
            }
            
            ops.AddRange(modDef.GetInstallOps(mc));
            return ops;
        }

        public List<AssetOp> GetUninstallModOps(ModDefinition modDef)
        {
            ModContext mc = new ModContext(_config.ModsSourcePath.CombineFwdSlash(modDef.ID), _config, _getEngine);
            return modDef.GetUninstallOps(mc);            
        }
    }

    
}