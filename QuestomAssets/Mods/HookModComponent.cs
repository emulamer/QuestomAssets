using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public class HookModComponent : ModComponent
    {
        public override ModComponentType Type => ModComponentType.HookMod;

        /// <summary>
        /// The action to be performed when the mod is installed
        /// </summary>
        public HookModInstallAction InstallAction { get; set; }

        /// <summary>
        /// Optionally, the action to be performed when the mod is uninstalled if the ModDefinition's CanUninstall == true
        /// </summary>
        public HookModUninstallAction UninstallAction { get; set; }

        public override List<AssetOp> GetInstallOps(ModContext context)
        {
            Log.LogMsg($"Installing HookMod component");
            if (InstallAction == null)
                throw new InvalidOperationException("HookMod is being installed, but has no install action!");
            return InstallAction.GetInstallOps(context);
        }

        public override List<AssetOp> GetUninstallOps(ModContext context)
        {
            if (UninstallAction == null)
                throw new InvalidOperationException("HookMod is being uninstalled, but has no uninstall action!");
            return UninstallAction.GetUninstallOps(context);
        }
    }
}
