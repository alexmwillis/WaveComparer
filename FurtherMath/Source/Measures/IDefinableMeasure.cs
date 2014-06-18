using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;

namespace FurtherMath.Measures
{
    interface IDefinableMeasure<T> : IMeasure<T>
        where T : IVectorConvertable
    {
        void ScaleDistance(T a, T b, double d);
    }
}
