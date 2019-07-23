using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;

namespace QuestomAssets.Mods.Assets
{
    public class CreateAssetAction : AssetAction
    {
        public override AssetActionType Type => AssetActionType.CreateAsset;

        public string FromDataFile { get; set; }

        public string FileName { get; set; }

        public AssetType AssetType { get; set; }

        public bool AllowOverwriteName { get; set; } = false;

        // Only used when AssetType == AssetType.Unknown || AssetType == AssetType.Component
        public int ClassID { get; set; }

        // Only used when AssetType == AssetType.MonoBehaviour
        public string ScriptName;

        public override IEnumerable<AssetOp> GetOps(ModContext context)
        {
            //TODO: I don't like the root path constant here.  Get rid of it.
            if (!context.Config.RootFileProvider.FileExists(context.ModPath.CombineFwdSlash(FromDataFile)))
                throw new Exception($"CreateAssetAction could not find the asset data file {FromDataFile}");

            byte[] assetData;
            try
            {
                assetData = context.Config.RootFileProvider.Read(context.ModPath.CombineFwdSlash(FromDataFile));
            }
            catch (Exception ex)
            {
                Log.LogMsg($"Exception reading {FromDataFile} for ReplaceAssetAction.", ex);
                throw new Exception($"ReplaceAssetAction could not read data file {FromDataFile}", ex);
            }
            yield return new CreateAssetOp(assetData, AssetType, FileName, ClassID, ScriptName);
        }
    }
}
