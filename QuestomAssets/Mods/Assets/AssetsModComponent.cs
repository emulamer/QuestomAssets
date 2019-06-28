using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.Mods.Assets
{
    public class AssetsModComponent : ModComponent
    {
        public override ModComponentType Type => ModComponentType.AssetsMod;

        public AssetsModActionGroup InstallAction { get; set; }

        public AssetsModActionGroup UninstallAction { get; set; }

        public override void InstallComponent(ModContext context)
        {
            if (InstallAction == null)
                throw new InvalidOperationException("Tried to install AssetsModComponent, but install action is null.");
            if (InstallAction.Actions == null || InstallAction.Actions.Count < 1)
                throw new InvalidOperationException("Install action has no asset actions defined!");

            using (new LogTiming("preloading asset files for assets mod"))
            {
                if (InstallAction.PreloadFiles != null)
                    InstallAction.PreloadFiles.ForEach(x => context.Engine.Manager.GetAssetsFile(x));
            }
            List<AssetOp> ops = new List<AssetOp>();
            foreach (var action in InstallAction.Actions.OrderBy(x=> x.StepNumber))
            {
                using (new LogTiming($"getting operations for asset mod action step {action.StepNumber}"))
                {
                    ops.AddRange(action.GetOps(context));
                }
            }
            Log.LogMsg($"Queueing {ops.Count} for assets mod component and waiting for completion...");
            ops.ForEach(x => context.Engine.OpManager.QueueOp(x));

            //TODO: I'd like to just leave all these queued, but the modcontext has an open reader I don't want to let dangle or worry about cleaning up later
            ops.WaitForFinish();
            if (ops.Any(x => x.Status == OpStatus.Failed))
            {
                throw new Exception("At least one operation failed during mod installation.  Mod was not successfully installed");
            }
        }

        public override void UninstallComponent(ModContext context)
        {
            if (UninstallAction == null)
                throw new InvalidOperationException("Tried to uninstall AssetsModComponent, but uninstall action is null.");
        }
    }
}
