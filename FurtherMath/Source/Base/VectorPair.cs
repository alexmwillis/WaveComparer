using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurtherMath.Base
{
    public struct VectorPair
    {
        public readonly Vector FirstVector;
        public readonly Vector SecondVector;

        public VectorPair(Vector firstVector, Vector secondVector)
        {
            FirstVector = firstVector;
            SecondVector = secondVector;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            if (obj is VectorPair)
            {
                var other = (VectorPair)obj;
                return
                    this.FirstVector + this.SecondVector == other.FirstVector + other.SecondVector;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            var combinedVector = this.FirstVector + this.SecondVector;
            var hash = combinedVector.GetHashCode();
            return hash;
        }

        public static bool operator ==(VectorPair a, VectorPair b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(VectorPair a, VectorPair b)
        {
            return !(a == b);
        }
    }
}
