using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib
{
    class ListEntryChangedEventArgs : EventArgs 
    {
        public readonly object ChangedItem;
        public readonly int ListIndex;

        public ListEntryChangedEventArgs(object changedItem, int listIndex)
        {
            ChangedItem = changedItem;
            ListIndex = listIndex;
        }
    }
}
