using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparerLib.Event_Args;

namespace WaveComparerLib.Interfaces
{
    public delegate void LoadedEventHandler<T>(object sender, LoadedEventArgs<T> e);

    interface ILoader<T>
    {
        event LoadedEventHandler<T> Loaded;
    }
}
