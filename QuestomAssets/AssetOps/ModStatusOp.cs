using QuestomAssets.Models;
using QuestomAssets.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
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
            context.Engine.ModManager.SetModStatus(Definition, TargetModStatus);
        }
    }


}
