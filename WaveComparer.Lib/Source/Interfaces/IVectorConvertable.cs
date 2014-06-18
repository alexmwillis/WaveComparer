using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Interfaces
{
    public delegate Vector Transform(Vector vector);

    public interface IVectorConvertable
    { 
        Vector ToVector();
    }
}
