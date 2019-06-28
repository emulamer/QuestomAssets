using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.AssetOps;

namespace QuestomAssets.Mods.Assets
{
    public class ReplaceAssetAction : AssetAction
    {
        public override AssetActionType Type => AssetActionType.ReplaceAsset;

        public string FromDataFile { get; set; }

        public AssetLocator Locator { get; set; }

        public override IEnumerable<AssetOp> GetOps(ModContext context)
        {
            if (!context.ModFilesProvider.FileExists(FromDataFile))
                throw new Exception($"ReplaceAssetsAction could not find the asset data file {FromDataFile}");
            if (Locator == null)
                throw new Exception("Locator is null for ReplaceAssetsAction!");

            byte[] assetData;
            try
            {
                assetData = context.ModFilesProvider.Read(FromDataFile);
            }
            catch (Exception ex)
            {
                Log.LogMsg($"Exception reading {FromDataFile} for ReplaceAssetAction.", ex);
                throw new Exception($"ReplaceAssetAction could not read data file {FromDataFile}", ex);
            }
            
            yield return new ReplaceAssetOp(Locator, assetData);
        }
    }
}
