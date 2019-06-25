using BeatmapAssetMaker;
using NUnit.Framework;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Models;
using QuestomAssets.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace QuestomAssets.Tests
{
    public class FolderAssetsTestNotCombined : FolderAssetsTestCombined
    {
        
        protected override IAssetsFileProvider GetProvider()
        {
            return new FolderFileProvider($".\\TestAssets{TestRandomNum}\\", false, true);
        }


    }
}
