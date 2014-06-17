using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;

using WaveComparerLib.Interfaces;

namespace WaveComparerLib
{
    public static class VectorOps
    {
        public static Vector Sum(List<Vector> vectors)
        {
            if (vectors.Count() > 0)
            {
                if (vectors.Count() == 1)
                {
                    return vectors[0].Copy();
                }
                else
                {
                    Vector sum;
                    sum = vectors[0] + vectors[1];
                    for (int i = 0; i < vectors.Count(); i++)
                    {
                        sum += vectors[i];
                    }
                    return sum;
                }
            }
            else throw new ArgumentException("Tried to sum an empty list of vectors");
        }

        public static Vector Average(List<Vector> vectors)
        {
            return ((double)1 / vectors.Count()) * Sum(vectors);
        }
    }
}
