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

        public override void InstallComponent(ModContext context)
        {
            Log.LogMsg($"Installing HookMod component");
            if (InstallAction == null)
                throw new InvalidOperationException("HookMod is being installed, but has no install action!");
            InstallAction.Install(context.ModFilesProvider, context.Config);
        }

        public override void UninstallComponent(ModContext context)
        {
            if (UninstallAction == null)
                throw new InvalidOperationException("HookMod is being uninstalled, but has no uninstall action!");
            UninstallAction.Uninstall(context.Config);
        }
    }
}
