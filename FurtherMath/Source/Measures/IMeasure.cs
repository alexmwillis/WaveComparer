using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;

namespace FurtherMath.Measures
{
    public interface IMeasure<T>
        where T: IVectorConvertable
    {
        /// <summary>
        /// Returns distance of x from y
        /// </summary>
        /// <param name="x">First object to measure</param>
        /// <param name="y">Second object to measure</param>
        /// <returns>Distance</returns>
        double Distance(T x, T y);
    }
}
