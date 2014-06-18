using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace FurtherMath.Base.Collections
{
    // A class that works just like ArrayList, but sends event
    // notifications whenever the list changes.
    public class ListWithChangedEvent<T> : ArrayList, INotifyChanged
    {
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event ChangedEventHandler Changed;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
        
        public int Add(T value)
        {
            int i = base.Add(value);
            // Additional event args for changing list entry
            OnChanged(new ListEntryChangedEventArgs((object)value, i));
            return i;
        }

        public override void Clear()
        {
            base.Clear();
            OnChanged(EventArgs.Empty);
        }

        new virtual public T this[int index]
        {
            get
            {
                return (T)base[index];
            }
            set
            {
                base[index] = value;
                // Additional event args for changing list entry
                OnChanged(new ListEntryChangedEventArgs((object)value, index));
            }
        }
    }
}
