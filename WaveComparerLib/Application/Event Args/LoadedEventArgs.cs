﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Event_Args
{
    public class LoadedEventArgs<T> : EventArgs
    {
        public readonly T LoadedObject;

        public LoadedEventArgs(T obj)
        {
            LoadedObject = obj;
        }

    }
}
