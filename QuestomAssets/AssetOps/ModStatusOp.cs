using QuestomAssets.Models;
using QuestomAssets.Mods;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class ModStatusOp : AssetOp
    {
        public ModStatusOp(ModDefinition definition, ModStatusType status)
        {
            TargetModStatus = status;
            Definition = definition;
        }

        public ModDefinition Definition { get; private set; }
        public ModStatusType TargetModStatus { get; private set; }
        public override bool IsWriteOp => true;

        internal override void PerformOp(OpContext context)
        {
            switch (TargetModStatus)
            {
                case ModStatusType.Installed:
                    if (context.Engine.ModManager.ModConfig.InstalledModIDs.Contains(Definition.ID))
                        Log.LogErr($"ModStatusOp was supposed to install mod ID {Definition.ID} but it is already listed as installed.");
                    else
                        context.Engine.ModManager.ModConfig.InstalledModIDs.Add(Definition.ID);
                    Definition.Status = ModStatusType.Installed;
                    break;
                case ModStatusType.NotInstalled:
                    if (!context.Engine.ModManager.ModConfig.InstalledModIDs.Contains(Definition.ID))
                        Log.LogErr($"ModStatusOp was supposed to uninstall mod ID {Definition.ID} but it doesn't appear to be installed.");
                    else
                        context.Engine.ModManager.ModConfig.InstalledModIDs.Remove(Definition.ID);
                    Definition.Status = ModStatusType.NotInstalled;
                    break;
            }
        }
    }


}
