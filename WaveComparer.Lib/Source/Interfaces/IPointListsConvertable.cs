using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using FurtherMath.Base;

namespace WaveComparer.Lib.Interfaces
{
    public interface IPointListsConvertable
    {
        List<List<Point>> ToPointLists();
    }
}
