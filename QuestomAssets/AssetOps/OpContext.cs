using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;
using static QuestomAssets.QuestomAssetsEngine;

namespace QuestomAssets.AssetOps
{
    public class OpContext
    {
        public OpContext(QuestomAssetsEngine qae)
        {
            Engine = qae;
        }

        public QuestomAssetsEngine Engine { get; private set; }
        public QaeConfig Config { get => Engine.Config; }
        public AssetsManager Manager { get => Engine.Manager; }
        public MusicConfigCache Cache { get => Engine.MusicCache; }
    }
}
