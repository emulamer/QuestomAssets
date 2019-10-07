using QuestomAssets.AssetOps;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public class FileCopyModComponent : ModComponent
    {
        public override ModComponentType Type => ModComponentType.FileCopyMod;

        public string SourceFileName { get; set; }

        public string TargetRootedPathAndFileName { get; set; }

        public override List<AssetOp> GetInstallOps(ModContext context)
        {
            Log.LogMsg($"Installing FileCopy component");
            if (string.IsNullOrEmpty(SourceFileName))
            {
                throw new InvalidOperationException("FileCopy is being installed, but has no filename!");
            }
            if (string.IsNullOrEmpty(TargetRootedPathAndFileName))
            {
                throw new InvalidOperationException("FileCopy is being installed, but has no root filesystem target path!");
            }

            if (!context.Config.RootFileProvider.FileExists(context.ModPath.CombineFwdSlash(SourceFileName)))
            {
                throw new Exception($"FileCopy installation failed because the file {SourceFileName} could not be found.");
            }
            if (System.IO.File.Exists(TargetRootedPathAndFileName))
            {
                Log.LogMsg($"FileCopy target file {TargetRootedPathAndFileName} already exists, it will be overwritten.");
            }
            try
            {
                var bytes = context.Config.RootFileProvider.Read(context.ModPath.CombineFwdSlash(SourceFileName));
                var op = new QueuedFileOp() { Type = QueuedFileOperationType.WriteFile, TargetPath = TargetRootedPathAndFileName, SourceData = bytes, ProviderType = QueuedFileOperationProviderType.FileSystemRoot };
                return new List<AssetOp>() { op };
            }
            catch (Exception ex)
            {
                Log.LogErr($"FileCopy install failed to queue install mod to {TargetRootedPathAndFileName}.", ex);
                throw;
            }

        }

        public override List<AssetOp> GetUninstallOps(ModContext context)
        {            
            if (string.IsNullOrWhiteSpace(TargetRootedPathAndFileName))
            {
                return new List<AssetOp>();
            }

            try
            {
                var op = new QueuedFileOp() { ProviderType = QueuedFileOperationProviderType.FileSystemRoot, TargetPath = TargetRootedPathAndFileName, Type = QueuedFileOperationType.DeleteFile };
                return new List<AssetOp>() { op };
            }
            catch (Exception ex)
            {
                Log.LogErr($"Uninstall HookMod failed to queue delete file {TargetRootedPathAndFileName}!", ex);
                throw;
            }

        }

    }
}
