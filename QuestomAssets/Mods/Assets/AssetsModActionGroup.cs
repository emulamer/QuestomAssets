using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class AssetsModActionGroup
    {
        /// <summary>
        /// The list of asset files that will be preloaded for this mod to 
        /// </summary>
        public List<string> PreloadFiles { get; set; }

        /// <summary>
        /// The list of actions that will be taked on the asset files for the mod
        /// </summary>
        public List<AssetAction> Actions { get; set; }
    }
}
