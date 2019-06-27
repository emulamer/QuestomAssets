using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public class ModDefinition
    {
        /// <summary>
        /// Unique identifier of this mod
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The (display) name of the mod
        /// </summary>
       public string Name { get; set; }

        /// <summary>
        /// The description of the mod
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The category this mod falls into for display and organizational purposes
        /// </summary>
        public ModCategory Category { get; set; }

        /// <summary>
        /// The version of Beat Saber that this mod was designed for
        /// </summary>
        public string TargetBeatSaberVersion { get; set; }

        /// <summary>
        /// Whether or not the mod can be uninstalled cleanly without resetting assets, etc.
        /// </summary>
        public bool CanUninstall { get; set; }

        /// <summary>
        /// The list of individual components of this mod
        /// </summary>
        public List<ModComponent> Components { get; set; } = new List<ModComponent>();
    }
}
