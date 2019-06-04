using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{

    public interface ISmartPtr<out T> where T : AssetsObject
    {
        AssetsObject Owner { get; set; }
        T Target { get; }
    }

    public class SmartPtr<T> : ISmartPtr<T>, IDisposable where T : AssetsObject
    {
        public SmartPtr(T target)
        {
            Target = target ?? throw new NullReferenceException("Target cannot be null");
            Target.ObjectInfo.ParentFile.AddPtrRef(this);
        }

        private AssetsObject _owner;
        public AssetsObject Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                if (_owner != value)
                {
                    if (_owner != null)
                    {
                        _owner.ObjectInfo.ParentFile.RemovePtrRef(this);
                    }
                    if (value != null)
                    {
                        value.ObjectInfo.ParentFile.AddPtrRef(this);
                        value.ObjectInfo.ParentFile.GetOrAddExternalFileIDRef(Target.ObjectInfo.ParentFile);
                    }
                }
                _owner = value;
            }
        }

        public T Target { get; private set; }

        private int _fileID = 0;
        public int FileID
        {
            get
            {
                if (Owner == null)
                    throw new NotSupportedException("SmartPtr doesn't have an owner, getting FileID isn't supported!");
                if (Owner.ObjectInfo.ParentFile == Target.ObjectInfo.ParentFile)
                    return 0;

                return Owner.ObjectInfo.ParentFile.GetFileIDForFile(Target.ObjectInfo.ParentFile);                
            }
        }

        public ulong PathID
        {
            get; set;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Owner != null)
                    {
                        Owner.ObjectInfo.ParentFile.RemovePtrRef(this);
                    }
                    if (Target != null)
                    {
                        Owner.ObjectInfo.ParentFile.RemovePtrRef(this);
                    }
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SmartPtr()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion




        //public ExternalFile File
        //{
        //    get
        //    {
        //        _object.ObjectInfo.Owner.Metadata.ExternalFiles
        //    }
        //}

        //public bool IsExternal
        //{
        //    get
        //    {

        //    }
        //}

        //private 

        //private T _object;


        //public T Object {
        //    get
        //    {
        //        return null;
        //    }
        //    set
        //    {

        //    }

        //}


    }

    public class UnreferencedExternalFile
    {
        public AssetsFile TargetFile { get; set; }
        public List<ISmartPtr<AssetsObject>> NeededBy { get; } = new List<ISmartPtr<AssetsObject>>();
    }
    public class SmartPtrException : Exception
    {
        public SmartPtrException(string message, Exception innerException = null) : base(message, innerException)
        { }
    }


}
