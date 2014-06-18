using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurtherMath.Base
{
    public delegate Vector Transform(Vector vector);

    public interface IVectorConvertable
    { 
        Vector ToVector();
    }
}
