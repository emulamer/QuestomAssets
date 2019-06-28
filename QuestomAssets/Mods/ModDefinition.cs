using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.Mods
{
    public class ModDefinition
    {
        /// <summary>
        /// Unique identifier of this mod
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The (display) name of the mod
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The author of the mod
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// A link to more information about the mod
        /// </summary>
        public string InfoUrl { get; set; }

        /// <summary>
        /// The description of the mod
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The category this mod falls into for display and organizational purposes
        /// </summary>
        public ModCategory Category { get; set; }

        /// <summary>
        /// The version of Beat Saber that this mod was designed for
        /// </summary>
        public string TargetBeatSaberVersion { get; set; }

        /// <summary>
        /// Whether or not the mod can be uninstalled cleanly without resetting assets, etc.
        /// </summary>
        public bool CanUninstall { get; set; }

        /// <summary>
        /// The list of individual components of this mod
        /// </summary>
        public List<ModComponent> Components { get; set; } = new List<ModComponent>();


        public void Install(ModContext context)
        {
            using (new LogTiming($"Installing mod ID {ID}"))
            {

                if (Components == null || Components.Count < 1)
                {
                    Log.LogErr($"The mod with ID {ID} has no components to install.");
                    throw new Exception($"The mod with ID {ID} has no components to install.");
                }
                try
                {
                    foreach (var comp in Components)
                    {
                        comp.InstallComponent(context);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Mod ID {ID} threw an exception while installing.", ex);
                    throw new Exception($"Mod ID {ID} failed to install.", ex);
                }
            }
        }

        public void Uninstall(ModContext context)
        {
            if (!CanUninstall)
                throw new InvalidOperationException($"Mod ID {ID} does not support uninstalling (CanUninstall is false).");

            using (new LogTiming($"Uninstalling mod ID {ID}"))
            {
                if (Components == null || Components.Count < 1)
                {
                    Log.LogErr($"The mod with ID {ID} has no components to uninstall.");
                    throw new Exception($"The mod with ID {ID} has no components to uninstall.");
                }
                try
                {
                    foreach (var comp in Components)
                    {
                        comp.UninstallComponent(context);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Mod ID {ID} threw an exception while uninstalling.", ex);
                    throw new Exception($"Mod ID {ID} failed to uninstall.", ex);
                }
            }
        }

        public const string MOD_FILE_NAME = "beatonmod.json";

        public static ModDefinition LoadFromZipFile(Ionic.Zip.ZipFile zip)
        {
            try
            {
                var apk = new ApkAssetsFileProvider(zip, FileCacheMode.None, true);
                return LoadModDef(apk);
            }
            catch (Exception ex)
            {
                Log.LogErr($"ModDefinition failed to load from zip file.", ex);
                throw new Exception($"ModDefinition failed to load from zip file.", ex);
            }
        }

        //public static ModDefinition InstallFromZipFile(Ionic.Zip.ZipFile zip, QaeConfig config, QuestomAssetsEngine engine)
        //{
        //    try
        //    {
        //        var apk = new ApkAssetsFileProvider(zip, FileCacheMode.None, true);
        //        return InstallModFile(apk, config, engine);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.LogErr($"ModDefinition failed to load from zip file.", ex);
        //        throw new Exception($"ModDefinition failed to load from zip file.", ex);
        //    }
        //}
        private static ModDefinition LoadModDef(ApkAssetsFileProvider apk)
        {
          
            ModDefinition def = null;
            if (!apk.FileExists(MOD_FILE_NAME))
            {
                throw new Exception($"ModDefinition can't load zip file becase it does not contain {MOD_FILE_NAME}");
            }
            using (JsonTextReader jr = new JsonTextReader(new StreamReader(apk.GetReadStream(MOD_FILE_NAME))))
            {
                def = new JsonSerializer().Deserialize<ModDefinition>(jr);
            }
            if (def == null)
                throw new Exception("ModDefinition failed to deserialize.");
            return def;
        }

        private static ModDefinition InstallModFile(string modFileName, QaeConfig config, QuestomAssetsEngine engine)
        {
            ModDefinition def;
            using (var rs = config.RootFileProvider.GetReadStream(config.ModsSourcePath + "/" + modFileName))
            {
                using (ApkAssetsFileProvider apk = new ApkAssetsFileProvider(rs, FileCacheMode.None, true))
                {
                    def = LoadModDef(apk);
                    Log.LogMsg($"Installing mod ID {def.ID}");
                    var context = new ModContext(apk, config, engine);
                    def.Install(context);
                    return def;
                }
            }
        }

        public static ModDefinition InstallFromZip(string zipFileName, QaeConfig config, QuestomAssetsEngine engine)
        {
            //need to get these paths using config out of here, it's a side effect of a poor job in the download class
            try
            {
                if (!config.RootFileProvider.FileExists(config.ModsSourcePath+"/"+ zipFileName))
                    throw new Exception($"Mod zip file {zipFileName} does not exist!");
                
                var apk = new ApkAssetsFileProvider(zipFileName, FileCacheMode.None, true);
                return InstallModFile(zipFileName, config, engine);
            }
            catch (Exception ex)
            {
                Log.LogErr($"ModDefinition failed to load from zip file {zipFileName}", ex);
                throw new Exception($"ModDefinition failed to load from zip file {zipFileName}", ex);
            }
        }
    }
}
