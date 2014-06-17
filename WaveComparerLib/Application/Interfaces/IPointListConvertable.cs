﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WaveComparerLib.Interfaces
{
    public interface IPointListConvertable
    {
        List<Point> ToPointList();
    }
}