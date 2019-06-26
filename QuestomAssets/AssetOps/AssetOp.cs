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
        public DateTime? FinishedAt { get; private set; }

        public Exception Exception { get; private set; }

        internal void SetStatus(OpStatus status, Exception ex = null)
        {
            Status = status;
            if (status == OpStatus.Complete || status == OpStatus.Failed)
            {
                FinishedAt = DateTime.Now;
                IsFinished = true;
                FinishedEvent.Set();
                OpFinished?.Invoke(this, this);
                Exception = ex;
            }
        }
        internal abstract void PerformOp(OpContext context);
    }
}
