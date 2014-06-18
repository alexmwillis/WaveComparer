using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;

namespace FurtherMath.Measures
{
    public class EuclideanMeasure<T> : IMeasure<T>
        where T: IVectorConvertable
    {
        public double Distance(T x, T y)
        {
            return ~(x.ToVector() - y.ToVector());
        }
    }
}
