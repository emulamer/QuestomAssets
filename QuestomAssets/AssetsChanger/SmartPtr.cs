using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public interface ISmartPtr<out T> where T : AssetsObject
    {
        AssetsObject Owner { get; }
        IObjectInfo<T> Target { get; }
        void Dispose();
        void WritePtr(AssetsWriter writer);
        bool IsNew { get; set; }
        T Object { get; }
        int FileID { get; }
        long PathID { get; }
    }

    public class SmartPtr<T> : ISmartPtr<T>, IDisposable where T : AssetsObject
    {
        public SmartPtr(AssetsObject owner, T target)
        {
            Init(owner, (IObjectInfo<T>)target.ObjectInfo);
        }

        public SmartPtr(AssetsObject owner, IObjectInfo<T> target)
        {
            Init(owner, target);
        }

        private void Init(AssetsObject owner, IObjectInfo<T> target)
        {
            Target = target ?? throw new NullReferenceException("Target cannot be null");
            Owner = owner ?? throw new NullReferenceException("Owner cannot be null");
            //TODO: not sure this is only ever called by new objects
            IsNew = true;
            Target.ParentFile.AddPtrRef(this);
            Owner.ObjectInfo.ParentFile.AddPtrRef(this);
        }

        public T Object { get
            {
                return Target.Object;
            }
        }

        public static SmartPtr<T> Read(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            if (owner == null)
            {
                Log.LogErr("WARNING: SmartPtr created without an owner!");
            }
            int fileID = reader.ReadInt32();
            reader.AlignTo(4);
            Int64 pathID = reader.ReadInt64();
            if (fileID == 0 && pathID == 0)
                return null;

            var objInfo = assetsFile.GetObjectInfo<T>(fileID, pathID);

            if (objInfo == null)
            {
                Log.LogErr($"WARNING: Could not find objectinfo for creating SmartPtr of type {typeof(T).Name} on owner type {owner?.GetType()?.Name ?? "(null owner)"}!  Returned a null pointer instead.");
                return null;
            }

            SmartPtr<T> ptr = new SmartPtr<T>(owner, assetsFile.GetObjectInfo<T>(fileID, pathID));
            //TODO: not sure this is only ever called by existing objects
            ptr.IsNew = false;

            return ptr;
        }

        public bool IsNew { get; set; }
        public AssetsObject Owner { get; private set; }
        //{
        //    get
        //    {
        //        return _owner;
        //    }
        //    set
        //    {
        //        if (_owner != value)
        //        {
        //            if (_owner != null)
        //            {
        //                _owner.ParentFile.RemovePtrRef(this);
        //            }
        //            if (value != null)
        //            {
        //                value.ParentFile.AddPtrRef(this);
        //                value.ParentFile.GetOrAddExternalFileIDRef(Target.ParentFile);
        //            }
        //        }
        //        _owner = value;
        //    }
        //}

        public IObjectInfo<T> Target { get; private set; }


        //private int _fileID = 0;

            public long PathID
        {
            get
            {
                if (Target == null)
                    throw new Exception("Target is null!");
                return Target.ObjectID;
            }
        }

        public int FileID
        {
            get
            {
                if (Owner == null)
                    throw new NotSupportedException("SmartPtr doesn't have an owner, getting FileID isn't supported!");
                if (Owner.ObjectInfo.ParentFile == Target.ParentFile)
                    return 0;

                return Owner.ObjectInfo.ParentFile.GetFileIDForFile(Target.ParentFile);
            }
        }

        //public ulong PathID
        //{
        //    get; set;
        //}

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
                        Target.ParentFile.RemovePtrRef(this);
                    }
                    Owner = null;
                    Target = null;
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

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

        public void WritePtr(AssetsWriter writer)
        {
            writer.Write(FileID);
            writer.AlignTo(4);
            writer.Write(Target.ObjectID);
        }



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
