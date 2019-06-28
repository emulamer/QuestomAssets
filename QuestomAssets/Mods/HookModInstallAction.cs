using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QuestomAssets.Mods
{
    public class HookModInstallAction : IModAction
    {
        /// <summary>
        /// The filename of the .so library file of the hook that should be installed
        /// </summary>
        public string InstallLibraryFile { get; set; }


        public void Install(IAssetsFileProvider modFilesProvider, QaeConfig config)
        {
            if (!modFilesProvider.FileExists(InstallLibraryFile))
            {
                throw new Exception($"HookMod installation failed because the library file {InstallLibraryFile} could not be found.");
            }
            if (config.ModLibsFileProvider.FileExists(InstallLibraryFile))
            {
                Log.LogMsg($"HookMod library file {InstallLibraryFile} already exists, it will be overwritten.");
            }
            try
            {
                using (var rs = modFilesProvider.GetReadStream(InstallLibraryFile))
                {
                    rs.CopyTo(config.ModLibsFileProvider.GetWriteStream(InstallLibraryFile));
                }
            }
            catch (Exception ex)
            {
                Log.LogErr($"HookMod install failed to install mod to {InstallLibraryFile}.", ex);
                throw;
            }

        }
    }
}
