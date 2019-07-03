using Newtonsoft.Json;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace QuestomAssets
{
    /// <summary>
    /// Keeps track of the status of mods relative to committed changes to the asset files
    /// </summary>
    internal class ModConfig
    {
        public List<string> InstalledModIDs { get; set; } = new List<string>();

        public static ModConfig Load(QaeConfig config)
        {
            try
            {
                if (config.ModsStatusFile != null && config.RootFileProvider.FileExists(config.ModsStatusFile))
                {
                    string modCfgTxt = System.Text.Encoding.UTF8.GetString(config.RootFileProvider.Read(config.ModsStatusFile));
                    return JsonConvert.DeserializeObject<ModConfig>(modCfgTxt);
                }
                else
                {
                    return new ModConfig();
                }
            }
            catch (Exception ex)
            {
                Log.LogErr($"Unable to read/parse mod status file from {config.ModsStatusFile}!  Mod statuses will be lost!", ex);
                return new ModConfig();
            }
        }

        public void Save(QaeConfig config)
        {
            try
            {
                byte[] cfgBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
                config.RootFileProvider.Write(config.ModsStatusFile, cfgBytes, true);
            }
            catch (Exception ex)
            {
                Log.LogErr($"Failed to write mod status file to {config.ModsStatusFile}!  Mod statuses will be lost!", ex);
                throw;
            }
        }

        public ModConfig Clone()
        {
            return new ModConfig() { InstalledModIDs = InstalledModIDs.ToList() };
        }

        public bool Matches( ModConfig y)
        {
            if (this == y)
                return true;
            if (this == null)
                return false;
            if (this.InstalledModIDs.Count != y.InstalledModIDs.Count)
                return false;
            if (this.InstalledModIDs.Any(a => !y.InstalledModIDs.Exists(b => b == a)))
                return false;

            return true;
        }

    }
}
