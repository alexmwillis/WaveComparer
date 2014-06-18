using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using FurtherMath.Base.Collections;

using WaveComparer.Lib.Event_Args;

namespace WaveComparer.Lib.Interfaces
{
    public delegate void ItemChangedEventHandler(object sender, ItemChangedEventArgs e);

    public interface IObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyChanged
    {
        event ItemChangedEventHandler ItemChanged;        
    }
}
