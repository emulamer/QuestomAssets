using Newtonsoft.Json;
using QuestomAssets.AssetsChanger;
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

        public static ModDefinition LoadDefinitionFromProvider(IFileProvider provider)
        {
            try
            {
                return LoadModDef(provider);
            }
            catch (Exception ex)
            {
                Log.LogErr($"ModDefinition failed to load from zip file.", ex);
                throw new Exception($"ModDefinition failed to load from zip file.", ex);
            }
        }

        private static ModDefinition LoadModDef(IFileProvider provider, string path = "")
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

        /// <summary>
        /// Installs (or queues the ops to install) a mod from the directory RELATIVE TO THE BEATONDATAROOT
        /// </summary>
        public static void InstallModFromDirectory(string modDirectory, QaeConfig config, Func<QuestomAssetsEngine> getEngine)
        {
            var def = LoadModDef(config.RootFileProvider, modDirectory);
            var context = new ModContext(modDirectory, config, getEngine);
            def.Install(context);
        }
    }
}
