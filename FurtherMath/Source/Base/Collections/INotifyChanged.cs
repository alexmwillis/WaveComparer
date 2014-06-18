﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FurtherMath.Base.Collections
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    public interface INotifyChanged
    {
        event ChangedEventHandler Changed;
    }
}
