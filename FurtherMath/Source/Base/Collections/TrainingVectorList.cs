using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Measures;

namespace FurtherMath.Base.Collections
{
    public class TrainingVectorList<T> : IObservableList<T>
        where T : class, IVectorConvertable, INotifyChanged
    {
        private VectorList<T> _list;
        private TrainingSetMeasure<T> _measure;

        public TrainingVectorList(int vectorDimension, string temp)
        {
            _measure = new TrainingSetMeasure<T>();
            _list = new VectorList<T>(vectorDimension, _measure);            
        }

        /// <summary>
        /// Scales the distance between the item passed in and the item that the list is sorted by
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factor"></param>
        public void ScaleDistance(T item, double factor)
        {
            if (_list.SortBy != null && item != _list.SortBy)
                _measure.ScaleDistance(item, _list.SortBy, factor);
        }

        public T SortBy
        {
            get { return _list.SortBy; }
            set { _list.SortBy = value; }
        }

        public void Sort()
        {
            _list.Sort();
        }     

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _list.AddRange(collection);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _list.CollectionChanged += value;
            }
            remove
            {
                _list.CollectionChanged -= value;
            }
        }

        public event ChangedEventHandler Changed
        {
            add
            {
                _list.Changed += value;
            }
            remove
            {
                _list.Changed -= value;
            }
        }

        public event ItemChangedEventHandler ItemChanged
        {
            add
            {
                _list.ItemChanged += value;
            }
            remove
            {
                _list.ItemChanged -= value;
            }
        }
    }
}
