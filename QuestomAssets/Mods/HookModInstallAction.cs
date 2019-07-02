using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using QuestomAssets.AssetOps;

namespace QuestomAssets.Mods
{
    public class HookModInstallAction
    {
        /// <summary>
        /// The filename of the .so library file of the hook that should be installed
        /// </summary>
        public string InstallLibraryFile { get; set; }


        public List<AssetOp> GetInstallOps(ModContext context)
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
                var bytes = context.Config.RootFileProvider.Read(context.ModPath.CombineFwdSlash(InstallLibraryFile));
                var op = new QueuedFileOp() { TargetPath = InstallLibraryFile, SourceData = bytes, ProviderType = QueuedFileOperationProviderType.ModLibs };
                return new List<AssetOp>() { op };
            }
            catch (Exception ex)
            {
                Log.LogErr($"HookMod install failed to queue install mod to {InstallLibraryFile}.", ex);
                throw;
            }

        }
    }
}
