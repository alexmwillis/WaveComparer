using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurtherMath.Base.Collections
{
    /// <summary>
    /// Virtualises List
    /// </summary>
    public class VirtualList<T> : IList<T>
    {
        protected List<T> list;

        public VirtualList()
            : base()
        {
            list = new List<T>();
        }

        public virtual int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public virtual T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public virtual void Add(T item)
        {
            list.Add(item);
        }

        public virtual void AddRange(IEnumerable<T> collection)
        {
            list.AddRange(collection);
        }

        public virtual void Clear()
        {
            list.Clear();
        }

        public virtual bool Contains(T item)
        {
            return list.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get { return list.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool Remove(T item)
        {
            return list.Remove(item);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public virtual List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return list.ConvertAll<TOutput>(converter);
        }

        public virtual void Sort(Comparison<T> comparison)
        {
            list.Sort(comparison);
        }

        public static implicit operator List<T>(VirtualList<T> virtualList)
        {
            return virtualList.list;
        }
    }
}
