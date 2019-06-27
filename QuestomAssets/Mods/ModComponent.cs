using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public abstract class ModComponent
    {
        /// <summary>
        /// The type of modification this component will perform
        /// </summary>
        public abstract ModComponentType Type { get; }

        ///// <summary>
        ///// The action to be performed when the mod is installed
        ///// </summary>
        //public abstract IModAction InstallAction { get; set; }

        ///// <summary>
        ///// Optionally, the action to be performed when the mod is uninstalled if the ModDefinition's CanUninstall == true
        ///// </summary>
        //public abstract IModAction UninstallAction { get; set; }
    }
}
