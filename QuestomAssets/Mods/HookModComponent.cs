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

        
    }
}
