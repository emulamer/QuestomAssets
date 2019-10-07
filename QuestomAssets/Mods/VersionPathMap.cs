using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public class VersionPathMap
    {
        public string ModTargetVersion { get; private set; }
        public string BeatSaberVersion { get; private set; }
        public VersionPathMap(string modTargetVersion, string beatsaberVersion)
        {
            ModTargetVersion = modTargetVersion;
            BeatSaberVersion = beatsaberVersion;
        }
    }
}
