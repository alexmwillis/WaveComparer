using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using WaveComparerLib.Interfaces;

namespace WaveComparerLib
{
    public class Vector : ICollection
    {
        double[] _vector;

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
                throw new ArgumentException("Vectors must have the same dimension");

            var dim = a.Dimension;
            var c = new Vector(dim);
            for (int i = 0; i < dim; i++)
            {
                c[i] = a[i] - b[i];
            }
            return c;
        }
        
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
                throw new ArgumentException("Vectors must have the same dimension");

            var dim = a.Dimension;
            var c = new Vector(dim);
            for (int i = 0; i < dim; i++)
            {
                c[i] = a[i] + b[i];
            }
            return c;
        }

        public static Vector operator *(double scalar, Vector a)
        {
            if (scalar == double.PositiveInfinity || scalar == double.NegativeInfinity)
            {
                throw new ArgumentException("You tried to multiply by infinity");
            }
            var dim = a.Dimension;
            var b = new Vector(dim);
            for (int i = 0; i < dim; i++)
            {
                b[i] = scalar * a[i];
            }
            return b;
        }

        /// <summary>
        /// This is the Magnitude operator
        /// </summary>
        /// <param name="a">a is a vector</param>
        /// <returns></returns>
        public static double operator ~(Vector a)
        {
            double c = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                c += a[i] * a[i];
            }
            return Math.Sqrt(c);
        }

        public static explicit operator double[](Vector v)
        {
            return v._vector;
        }

        public static Vector Random(int lowerBound, int upperBound, int dimension)
        {
            var rnd = new Random();
            var v = new Vector(dimension);
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = rnd.Next(lowerBound, upperBound);
            }
            return v;
        }

        public static Vector Fill(int value, int dimension)
        {
            var v = new Vector(dimension);
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = value;
            } 
            return v;
        }

        /// <summary>
        /// Parameterless constructor for XML serialiser
        /// </summary>
        private Vector() { }

        public Vector(int dimension)
        {
            _vector = new double[dimension];
        }

        public Vector(double[] values)
        {
            _vector = values;
        }
        
        public double Magnitude { get { return ~this; } }

        public double this[int i] { 
            get 
            { 
                return _vector[i]; 
            } 
            set
            {
                // *** This hides error, however binding tries to set this to NaN, which shouldn't raise an exception
                if (!Double.IsNaN(value))
                {
                    _vector[i] = value;
                }
            } 
        }

        public Vector Copy()
        {
            var v = new Vector(this.Dimension);
            for (int i = 0; i < this.Length; i++)
            {
                v[i] = this[i];    
            }
            return v;
        }

        public int Dimension
        {
            get
            {
                return _vector.Length;
            }
        }

        public int Length { get { return _vector.Length; } }

        public void CopyTo(Array array, int index)
        {
            _vector.CopyTo(array, index);
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { return _vector.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _vector.SyncRoot; }
        }

        public IEnumerator GetEnumerator()
        {
            return _vector.GetEnumerator();
        }

        public static Vector EmptyVector
        {
            get
            {
                return new Vector(0);
            }
        }

        public override string ToString()
        {
            string stringRepresentation = "";
            for (int i = 0; i < this.Dimension; i++)
            {
                stringRepresentation += this[i].ToString("N1");
                if (i < this.Dimension - 1)
                    stringRepresentation += "\t ";
            }
            return stringRepresentation;
        }

        public string AsString
        {
            get
            {
                return this.ToString();
            }
        }
    }
}
