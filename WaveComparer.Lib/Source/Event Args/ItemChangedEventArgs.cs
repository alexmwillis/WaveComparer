using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Event_Args
{
    public class ItemChangedEventArgs : EventArgs
    {
        public readonly object ChangedItem;
        public readonly int ItemIndex;

        public ItemChangedEventArgs(object changedItem, int itemIndex)
        {
            ChangedItem = changedItem;
            ItemIndex = itemIndex;
        }
    }
}
