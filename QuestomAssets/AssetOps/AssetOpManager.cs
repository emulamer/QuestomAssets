using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace QuestomAssets.AssetOps
{
    public class AssetOpManager : IDisposable
    {
        private ConcurrentQueue<AssetOp> _opQueue = new ConcurrentQueue<AssetOp>();
        private OpContext _context;
        private object _threadLock = new object();
        private Thread _thread;
        private bool _threadAborted = false;

        public bool IsProcessing { get { return _thread != null && _opQueue.Count > 0; } }

        public event EventHandler<AssetOp> OpStatusChanged;

        public List<AssetOp> CurrentQueue { get { return _opQueue.ToList(); } }

        public AssetOpManager(OpContext context)
        {
            _context = context;
        }

        public void QueueOp(AssetOp op)
        {
            op.SetStatus(OpStatus.Queued);
            _opQueue.Enqueue(op);
            OpStatusChanged?.Invoke(this, op);            
            lock (_threadLock)
            {                
                if (_thread == null)
                {
                    _thread = new Thread(WorkerThreadProc);
                    _thread.Start();
                }
            }
            _queueEvent.Set();
        }

        private AutoResetEvent _queueEvent = new AutoResetEvent(false);

        private void WorkerThreadProc(object o)
        {
            bool entered = false;
            //I *think* this covers all the race conditions where the thread would stop as an item was being enqueued, probably need to re-review
            try
            {
                do
                {
                    if (_threadAborted)
                    {
                        Log.LogMsg("Opmanager worker thread has been aborted");
                        return;
                    }
                    AssetOp op = null;
                    bool dequeued = false;
                    bool threadAborting = false;

                    dequeued = _opQueue.TryDequeue(out op);
                    if (!dequeued)
                    {
                        if (entered)
                        {
                            Log.LogMsg("trying to exit on thread " + Thread.CurrentThread.ManagedThreadId);
                            Monitor.Exit(_context.Manager);
                            entered = false;
                        }
                        _queueEvent.WaitOne(2000);
                        continue;
                    }
                    if (!entered)
                    {
                        entered = Monitor.TryEnter(_context.Manager);
                        if (!entered)
                            continue;
                        Log.LogMsg("entered on thread " + Thread.CurrentThread.ManagedThreadId);
                    }
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        op.SetStatus(OpStatus.Started);
                        OpStatusChanged?.Invoke(this, op);
                        Log.LogMsg($"AssetOpManager starting op of type {op.GetType().Name}");
                        sw.Start();
                        op.PerformOp(_context);
                        sw.Stop();
                        Log.LogMsg($"AssetOpManager completed op of type {op.GetType().Name} in {sw.ElapsedMilliseconds}ms");
                        op.SetStatus(OpStatus.Complete);
                        OpStatusChanged?.Invoke(this, op);                        
                    }
                    catch (ThreadAbortException)
                    {
                        //just die if being aborted
                        _thread = null;
                        threadAborting = true;
                        Log.LogMsg("Op thread aborting");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log.LogErr($"Error handling Op type {op.GetType()}, operation threw an exception.", ex);
                        op.SetStatus(OpStatus.Failed, ex);
                    }
                    finally
                    {
                        
                        if (!threadAborting)
                        {
                            OpStatusChanged?.Invoke(this, op);
                        }
                    }
                } while (true);
            }
            catch (ThreadAbortException)
            {
                //Thread aborting, so let it
                _thread = null;
                return;
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception in op manager's worker thread proc!", ex);
            }
            finally
            {
                if (entered)
                    Monitor.Exit(_context.Manager);
                //just to make sure it gets set null in all cases
                _thread = null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (_thread != null)
                    {
                        _threadAborted = true;
                        try
                        {
                            _thread.Abort();
                        }catch (PlatformNotSupportedException)
                        {
                           
                        }
                        _thread = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
