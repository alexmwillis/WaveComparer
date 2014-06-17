using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using FurtherMath.Base;

namespace WaveComparerLib.Interfaces
{
    public interface IPointListsConvertable
    {
        List<List<Point>> ToPointLists();
    }
}
