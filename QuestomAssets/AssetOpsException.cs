using QuestomAssets.AssetOps;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public class AssetOpsException : Exception
    {
        public List<AssetOp> FailedOps { get; private set; }
        public AssetOpsException(string message, List<AssetOp> failedOps) : base(message)
        {
            FailedOps = failedOps;
        }
    }
}
