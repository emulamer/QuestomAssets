using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public interface ISmartPtrList<T> : IList<ISmartPtr<T>> where T : AssetsObject
    {

    }

     public class SmartPtrList<T> : ISmartPtrList<T> where T: AssetsObject
    {
        public SmartPtrList(IObjectInfo<AssetsObject> owner)
        {
            Owner = owner;
        }

        public IObjectInfo<AssetsObject> Owner { get; private set; }

        private List<ISmartPtr<T>> _list = new List<ISmartPtr<T>>();

        public ISmartPtr<T> this[int index] { get => _list[index];
            set
            {
                UnsetThisAsOwner(_list[index]);
                SetThisOwner(value);
                _list[index] = value;
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(ISmartPtr<T> item)
        {
            SetThisOwner(item);
            _list.Add(item);            
        }

        private void UnsetThisAsOwner(ISmartPtr<T> item)
        {
            item.Owner = null;
            item.UnsetOwnerProperty = null;
        }

        private void SetThisOwner(ISmartPtr<T> item)
        {
            if (item.Owner != null)
            {
                item.UnsetOwnerProperty();
            }
            item.Owner = Owner;
            item.UnsetOwnerProperty = () => this.Remove(item) ;
        }

        public void Clear()
        {
            _list.ForEach(x => Remove(x));
            _list.Clear();
        }

        public bool Contains(ISmartPtr<T> item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(ISmartPtr<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ISmartPtr<T>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(ISmartPtr<T> item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, ISmartPtr<T> item)
        {
            SetThisOwner(item);
            _list.Insert(index, item);
        }

        public bool Remove(ISmartPtr<T> item)
        {
            UnsetThisAsOwner(item);
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            UnsetThisAsOwner(_list[index]);
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
