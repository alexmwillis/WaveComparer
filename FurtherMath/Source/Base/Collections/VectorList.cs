using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

using FurtherMath.Measures;

namespace FurtherMath.Base.Collections
{
    public class VectorList<T> : ObservableList<T> 
        where T : class, IVectorConvertable, INotifyChanged
    {
        // Constrain vector in list to be the same dimension
        public readonly int VectorDimension;

        private IMeasure<T> _measure;
        private T _sortBy;

        public Vector Variance { get { return GetVariance(); } }

        public VectorList(int vectorDimension)
            : this(vectorDimension, new EuclideanMeasure<T>())
        { }

        public VectorList(int vectorDimension, IMeasure<T> measure)
        {
            _measure = measure;
            this.VectorDimension = vectorDimension;            
        }

        public T SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; }
        } 
 
        public void Sort()
        { 
            this.Sort(
                (a, b) =>
                {
                    var aDist = _measure.Distance(a, _sortBy);
                    var bDist = _measure.Distance(b, _sortBy);
                    if (aDist > bDist)
                        return 1;
                    else if (aDist < bDist)
                        return -1;
                    else
                        return 0;                    
                }); 
        }

        public override void Add(T item)
        {
            base.Add(item);
        }

        public override void AddRange(IEnumerable<T> collection)
        {           
            base.AddRange(collection);
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Returns the variance of the data for each dimension
        /// </summary>
        /// <returns></returns>
        public Vector GetVariance()
        {
            // Case of large dimensions *** ugly
            if (this.VectorDimension > 10)
            {
                return Vector.EmptyVector;
            }
            else
            {
                if (this.Count == 0)
                {
                    return new Vector(this.VectorDimension);
                }
                else
                {
                    var columns = this.Count;
                    var rows = this.VectorDimension;

                    var matrix = new double[rows][];
                    for (int i = 0; i < rows; i++)
                    {
                        matrix[i] = new double[columns];
                    }

                    for (int i = 0; i < columns; i++)
                    {
                        var v = this[i].ToVector();
                        for (int j = 0; j < v.Dimension; j++)
                        {
                            matrix[j][i] = v[j];
                        }
                    }
                    var vector = new Vector(this.VectorDimension);
                    for (int i = 0; i < rows; i++)
                    {
                        vector[i] = ArrayOps.GetVariance(matrix[i]);
                    }
                    return vector;
                }
            }
        }        

        // TODO define in interface
        public double[,] ToDoubleArray()
        {
            var n = this.Count;
            var m = this.VectorDimension;

            var doubleArray = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                var v = this[i].ToVector();
                for (int j = 0; j < v.Dimension; j++)
                {
                    doubleArray[i, j] = v[j];
                }
            }
            return doubleArray;
        }
    }
}
