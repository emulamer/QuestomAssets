using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class PathLocator
    {
        /// <summary>
        /// The assets filename to find the asset in (no path, do not append .split0 extensions even if the file has it)
        /// </summary>
        public string AssetFilename { get; set; }

        /// <summary>
        /// The PathID of the asset to locate within the file
        /// </summary>
        public Int64 PathID { get; set; }
    }
}
