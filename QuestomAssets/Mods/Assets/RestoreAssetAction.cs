using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class RestoreAssetAction : AssetAction
    {
        public override AssetActionType Type => AssetActionType.ReplaceAsset;

        public AssetLocator Locator { get; set; }

        public override IEnumerable<AssetOp> GetOps(ModContext context)
        {
            if (Locator == null)
                throw new Exception("Locator is null for RestoreAssetsAction!");
            if (context.BackupEngine == null)
                throw new Exception("BackupEngine is not set in the context!");
            byte[] assetData = null;
            try
            {
                var bqae = context.BackupEngine;
                var fromAsset = Locator.Locate(bqae.Manager);
                if (fromAsset == null)
                    throw new Exception($"Unable to locate asset in backup apk for mod uninstallation on step {StepNumber}!");                
                using (var ms = new MemoryStream())
                {
                    using (var writer = new AssetsWriter(ms))
                    {
                        fromAsset.Write(writer);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    assetData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception while restoring asset!", ex);
                throw;
            }
            yield return new ReplaceAssetOp(Locator, assetData);

        }
    }
}
