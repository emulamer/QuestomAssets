using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Utils;

namespace QuestomAssets.Mods.Assets
{
    public class AssetsModComponent : ModComponent
    {
        public override ModComponentType Type => ModComponentType.AssetsMod;

        public AssetsModActionGroup InstallAction { get; set; }

        public AssetsModActionGroup UninstallAction { get; set; }

        public override List<AssetOp> GetInstallOps(ModContext context)
        {
            if (InstallAction == null)
                throw new InvalidOperationException("Tried to install AssetsModComponent, but install action is null.");
            if (InstallAction.Actions == null || InstallAction.Actions.Count < 1)
                throw new InvalidOperationException("Install action has no asset actions defined!");

            using (new LogTiming("preloading asset files for assets mod"))
            {
                if (InstallAction.PreloadFiles != null)
                    InstallAction.PreloadFiles.ForEach(x => context.GetEngine().Manager.GetAssetsFile(x));
            }
            List<AssetOp> ops = new List<AssetOp>();
            foreach (var action in InstallAction.Actions.OrderBy(x=> x.StepNumber))
            {
                using (new LogTiming($"getting operations for asset mod action step {action.StepNumber}"))
                {
                    ops.AddRange(action.GetOps(context));
                }
            }
            Log.LogMsg($"Returning {ops.Count} for assets mod component installation...");
            return ops;
        }

        public override List<AssetOp> GetUninstallOps(ModContext context)
        {
            if (UninstallAction == null)
                throw new InvalidOperationException("Tried to install AssetsModComponent, but install action is null.");
            if (UninstallAction.Actions == null || UninstallAction.Actions.Count < 1)
                throw new InvalidOperationException("Install action has no asset actions defined!");
            if (string.IsNullOrEmpty(context.Config.BackupApkFileAbsolutePath))
                throw new InvalidOperationException("Uninstall assets mod can't happen when the backup APK isn't set!");
            if (!File.Exists(context.Config.BackupApkFileAbsolutePath))
                throw new Exception($"Backup APK file does not exist at {context.Config.BackupApkFileAbsolutePath}");

            using (new LogTiming("preloading asset files for uninstall assets mod"))
            {
                if (UninstallAction.PreloadFiles != null)
                    UninstallAction.PreloadFiles.ForEach(x => context.GetEngine().Manager.GetAssetsFile(x));
            }
            Log.LogMsg($"Opening backup APK...");
            List<AssetOp> ops = new List<AssetOp>();
            using (var apk = new ZipFileProvider(context.Config.BackupApkFileAbsolutePath, FileCacheMode.None, true, FileUtils.GetTempDirectory()))
            {
                var backupCfg = new QaeConfig() { AssetsPath = BeatSaber.BSConst.KnownFiles.AssetsRootPath, SongsPath = "", ModsSourcePath = "", PlaylistArtPath = "", RootFileProvider = apk };
                using (var backupQae = new QuestomAssetsEngine(backupCfg))
                {
                    context.BackupEngine = backupQae;
                    foreach (var action in UninstallAction.Actions.OrderBy(x => x.StepNumber))
                    {
                        using (new LogTiming($"getting operations for asset mod uninstall action step {action.StepNumber}"))
                        {
                            ops.AddRange(action.GetOps(context));
                        }
                    }
                }
                context.BackupEngine = null;
            }

            Log.LogMsg($"Returning {ops.Count} for assets mod component uninstall...");
            return ops;
        }
    }
}
