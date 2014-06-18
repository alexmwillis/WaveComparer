using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace FurtherMath.Base.Collections
{
    public class ObservableList<T> : VirtualList<T>, IObservableList<T>
        where T : INotifyChanged
    {   
        //protected void NotifyReset()
        //{
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //}

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event ItemChangedEventHandler ItemChanged;
        public event ChangedEventHandler Changed;

        public ObservableList()
        {
            //var t = this.GetType();
            //var properties = t.GetProperties();
            //foreach (var p in properties)
            //{
            //    var ca = p.GetCustomAttributes(typeof(CollectionDependentAttribute), true);
            //    var propertyName = p.Name;
            //    if (ca.Length > 0)
            //    {
            //        this.CollectionChanged += (o, e) =>
            //            {
            //                this.OnPropertyChanged(propertyName);
            //            };
            //        this.ItemChanged += (o, e) =>
            //            {
            //                this.OnPropertyChanged(propertyName);
            //            };
            //    }
            //}
        }
        
        // Summary:
        //     Raises the System.Collections.ObjectModel.ObservableCollection<T>.CollectionChanged
        //     event with the provided arguments.
        //
        // Parameters:
        //   e:
        //     Arguments of the event being raised.
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
            this.OnChanged();
        }

        protected virtual void OnItemChanged(object sender, EventArgs e)
        {
            var index = this.IndexOf((T)sender);
            var itemChangedArgs = new ItemChangedEventArgs(sender, index);
            if (ItemChanged != null)
                ItemChanged(this, itemChangedArgs);
            this.OnChanged();
        }

        protected virtual void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        protected virtual void OnItemChanged(T item)
        {
            this.OnItemChanged(item, EventArgs.Empty);
        }

        //
        // Summary:
        //     Raises the System.Collections.ObjectModel.ObservableCollection<T>.PropertyChanged
        //     event with the provided arguments.
        //
        // Parameters:
        //   e:
        //     Arguments of the event being raised.
        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    var e = new PropertyChangedEventArgs(propertyName);
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, e);
        //}
        
        public override void Add(T item)
        {
            base.Add(item);
            item.Changed += this.OnItemChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            //OnPropertyChanged("Count");
        }

        public override void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            foreach (var item in collection)
            {
                item.Changed += this.OnItemChanged;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection));
        }

        public override bool Remove(T item)
        {
            item.Changed -= this.OnItemChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            //OnPropertyChanged("Count");
            return base.Remove(item);
        }
        
        public override void Clear()
        {
            foreach (var item in this)
            {
                item.Changed -= this.OnItemChanged;
            }
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            //OnPropertyChanged("Count");
        }

        public override T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index].Changed -= this.OnItemChanged;
                base[index] = value;
                base[index].Changed += this.OnItemChanged;                
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
            }
        }

        public override void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            // The event isn't quite correct, as all items are moved
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this, 0, 0));
        }
    }
}
