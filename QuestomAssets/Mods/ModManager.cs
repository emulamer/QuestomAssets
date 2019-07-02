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
        }

        private List<ModDefinition> _modCache;

        public void ResetCache()
        {
            _modCache = null;
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
                return _modCache;
            }
        }

        public List<AssetOp> GetInstallModOps(ModDefinition modDef)
        {
            ModContext mc = new ModContext(_config.ModsSourcePath.CombineFwdSlash(modDef.ID), _config, _getEngine);
            return modDef.GetInstallOps(mc);
        }

        public List<AssetOp> GetUninstallModOps(ModDefinition modDef)
        {
            ModContext mc = new ModContext(_config.ModsSourcePath.CombineFwdSlash(modDef.ID), _config, _getEngine);
            return modDef.GetUninstallOps(mc);            
        }
        //public void DeleteModFolder(string modID)
        //{
        //    try
        //    {
        //        var path = _config.ModsSourcePath.CombineFwdSlash(modID);
        //        if (!_config.RootFileProvider.DirectoryExists(path))
        //        {
        //            //guess that means its already deleted, or somebody broke the rule of modid = directory name
        //            Log.LogErr($"Tried to delete mod, but folder {path} does not exist!");
        //            return;
        //        }
        //        _config.RootFileProvider.RmRfDir(path);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.LogErr($"Exception trying to delete mod id {modID}", ex);
        //        throw;
        //    }
        //}

        //public void UninstallMod(string modID)
        //{
        //    var mod = GetMods().FirstOrDefault(x => x.ID == modID);
        //    if (mod == null)
        //        throw new Exception($"Mod ID {modID} was not found to uninstall!");
        //    if (!mod.CanUninstall)
        //        throw new Exception($"Mod ID {modID} cannot be uninstalled.  You will need to reset assets to remove it.");
        //    ModContext context = new ModContext(modID, _config, _getEngine);
        //    mod.Uninstall(context);
        //}
    }

    
}