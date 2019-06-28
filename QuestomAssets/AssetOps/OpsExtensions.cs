using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace QuestomAssets.AssetOps
{
    public static class OpsExtensions
    {

        public static void WaitForFinish(this IEnumerable<AssetOp> ops)
        {
            var opsCopy = ops.ToList();
            using (new LogTiming($"waiting for {opsCopy.Count} ops to complete"))
            {
                while (opsCopy.Count > 0)
                {
                    var waitOps = opsCopy.Take(64);
                    WaitHandle.WaitAll(waitOps.Select(x => x.FinishedEvent).ToArray());
                    opsCopy.RemoveAll(x => waitOps.Contains(x));
                }
            }
        }

    }
}
