using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class RestoreAssetAction : LocatorAssetAction
    {
        public override AssetActionType Type => AssetActionType.RestoreAsset;

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
            AssetLocator locatorOverride = null;
            try
            {
                var res = Locator.Locate(context.GetEngine().Manager, false);
                if (res == null)
                    throw new LocatorException("Unable to find asset.");
            }
            catch (LocatorException ex)
            {
                Log.LogErr($"The locator for restore threw an exception, attempting to locate against backup and identify path.", ex);
                try
                {
                    var res = Locator.Locate(context.BackupEngine.Manager, false);
                    if (res == null)
                        throw new LocatorException("Unable to find asset, locator returned null");
                    locatorOverride = new AssetLocator() { PathIs = new PathLocator() { AssetFilename = res.ObjectInfo.ParentFile.AssetsFilename, PathID = res.ObjectInfo.ObjectID } };
                }
                catch (Exception ex2)
                {
                    Log.LogErr($"Unable to find path in backup for the locator either", ex2);
                    throw ex;
                }
            }
            yield return new ReplaceAssetOp(locatorOverride??Locator, assetData, true);

        }
    }
}
