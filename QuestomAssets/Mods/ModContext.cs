using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods
{
    public class ModContext
    {
        public ModContext(IAssetsFileProvider modFilesProvider, QaeConfig config, QuestomAssetsEngine engine)
        {
            ModFilesProvider = modFilesProvider;
            Config = config;
            Engine = engine;
        }
        public IAssetsFileProvider ModFilesProvider { get; private set; }
        public QaeConfig Config { get; private set; }
        public QuestomAssetsEngine Engine { get; private set; }
    }
}
