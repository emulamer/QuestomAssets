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


        public void Install(ModContext context)
        {
            if (!context.Config.RootFileProvider.FileExists(context.ModPath.CombineFwdSlash(InstallLibraryFile)))
            {
                throw new Exception($"HookMod installation failed because the library file {InstallLibraryFile} could not be found.");
            }
            if (context.Config.ModLibsFileProvider.FileExists(InstallLibraryFile))
            {
                Log.LogMsg($"HookMod library file {InstallLibraryFile} already exists, it will be overwritten.");
            }
            try
            {
                using (var rs = context.Config.RootFileProvider.GetReadStream(context.ModPath.CombineFwdSlash(InstallLibraryFile)))
                {
                    rs.CopyTo(context.Config.ModLibsFileProvider.GetWriteStream(InstallLibraryFile));
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
