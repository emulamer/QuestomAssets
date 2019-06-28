using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QuestomAssets.Mods
{
    public class HookModUninstallAction : IModAction
    {
        /// <summary>
        /// The filename of the .so library file that should be removed
        /// </summary>
        public string RemoveLibraryFile { get; set; }

        public void Uninstall(QaeConfig config)
        {
            if (!config.ModLibsFileProvider.FileExists(RemoveLibraryFile))
            {
                Log.LogErr($"Tried uninstalling HookMod library {RemoveLibraryFile} but it did not exist.");
                return;
            }
            try
            {
                config.ModLibsFileProvider.Delete(RemoveLibraryFile);
            }
            catch (Exception ex)
            {
                Log.LogErr($"Uninstall HookMod failed to delete file {RemoveLibraryFile}!", ex);
                throw;
            }
        }
    }
}
