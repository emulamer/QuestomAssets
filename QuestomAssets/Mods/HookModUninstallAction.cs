using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using QuestomAssets.AssetOps;

namespace QuestomAssets.Mods
{
    public class HookModUninstallAction
    {
        /// <summary>
        /// The filename of the .so library file that should be removed
        /// </summary>
        public string RemoveLibraryFile { get; set; }

        public List<AssetOp> GetUninstallOps(ModContext context)
        {
            if (!context.Config.ModLibsFileProvider.FileExists(RemoveLibraryFile))
            {
                Log.LogErr($"Tried uninstalling HookMod library {RemoveLibraryFile} but it did not exist.");
                return new List<AssetOp>();
            }
            try
            {
                var op = new QueuedFileOp() { ProviderType = QueuedFileOperationProviderType.ModLibs, TargetPath = RemoveLibraryFile, Type = QueuedFileOperationType.DeleteFile };
                return new List<AssetOp>() { op };
            }
            catch (Exception ex)
            {
                Log.LogErr($"Uninstall HookMod failed to queue delete file {RemoveLibraryFile}!", ex);
                throw;
            }
        }
    }
}
