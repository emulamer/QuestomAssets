using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QuestomAssets.AssetOps
{
    public abstract class AssetOp
    {
        private static int _nextID = 0;
        public int ID { get; } = Interlocked.Increment(ref _nextID);
        public OpStatus Status { get; private set; }
        public bool IsFinished { get; private set; }
        public ManualResetEvent FinishedEvent = new ManualResetEvent(false);

        public event EventHandler<AssetOp> OpFinished;

        internal void SetStatus(OpStatus status)
        {
            Status = status;
            if (status == OpStatus.Complete || status == OpStatus.Failed)
            {
                IsFinished = true;
                FinishedEvent.Set();
                OpFinished?.Invoke(this, this);
            }
        }
        internal abstract void PerformOp(OpContext context);
    }
}
