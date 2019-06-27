using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public class HookModUninstallAction : IModAction
    {
        /// <summary>
        /// The filename of the .so library file that should be removed
        /// </summary>
        public string RemoveLibraryFile { get; set; }
    }
}
