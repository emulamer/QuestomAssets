using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public class ModContext
    {
        public ModContext(string modPath, QaeConfig config, Func<QuestomAssetsEngine> getEngine)
        {
            ModPath = modPath;
            Config = config;
            GetEngine = getEngine;
        }
        public string ModPath { get; private set; }
        public QaeConfig Config { get; private set; }
        public Func<QuestomAssetsEngine> GetEngine { get; private set; }
    }
}
