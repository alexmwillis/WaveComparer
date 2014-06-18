using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparer.Lib.Event_Args;

namespace WaveComparer.Lib.Interfaces
{
    public delegate void LoadedEventHandler<T>(object sender, LoadedEventArgs<T> e);

    interface ILoader<T>
    {
        event LoadedEventHandler<T> Loaded;
    }
}
