using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

using WaveComparerLib.Interfaces;

using Accord.Statistics.Analysis;

namespace WaveComparerLib.Gen_Utils
{
    public class VectorList<T> : ObservableList<T> where T : IVectorConvertable, INotifyChanged
    {
        // TEMP
        float shortestDist = float.PositiveInfinity;
        List<T> shortestItems = new List<T>();

        // Constrain vector in list to be the same dimension
        public readonly int VectorDimension;

        protected List<Tuple<Vector, T>> _vectorTypeTuples = new List<Tuple<Vector, T>>();

        public Vector Variance { get { return GetVariance(); } }        

        public VectorList(int vectorDimension)
        {
            this.VectorDimension = vectorDimension;            
        }

        public void Sort(Vector vector)
        {
            this.Sort(
                (a, b) =>
                {
                    var aDist = ~(a.ToVector() - vector);
                    var bDist = ~(b.ToVector() - vector);
                    if (aDist > bDist)
                        return 1;
                    else if (aDist < bDist)
                        return -1;
                    else
                        return 0;                    
                });
        }
 
        public void Sort(T t)
        {
            var tV = t.ToVector(); 
            _vectorTypeTuples.Sort(
                (a, b) =>
                {
                    // TEMP
                    var aDist = ~(a.Item1 - tV);
                    if (aDist < shortestDist)
                    {
                        shortestDist = (float)aDist;
                        shortestItems.Clear();
                        shortestItems.Add(a.Item2);
                    }
                    else if (aDist == shortestDist)
                        shortestItems.Add(a.Item2);

                    var bDist = ~(b.Item1 - tV);
                    if (bDist < shortestDist)
                    {
                        shortestDist = (float)bDist;
                        shortestItems.Clear();
                        shortestItems.Add(b.Item2);
                    }
                    else if (bDist == shortestDist)
                        shortestItems.Add(b.Item2);

                    if (aDist > bDist)
                        return 1;
                    else if (aDist < bDist)
                        return -1;
                    else
                        return 0;                    
                });
            base.Clear();
            foreach (var tuple in _vectorTypeTuples)
            {
                base.Add(tuple.Item2);
            }
            // this.Sort((a, b) => a.CompareTo(t) - b.CompareTo(t));
        }

        public override void Add(T item)
        {
            _vectorTypeTuples.Add(new Tuple<Vector, T>(item.ToVector(), item));
            base.Add(item);
        }

        public override void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                _vectorTypeTuples.Add(new Tuple<Vector, T>(item.ToVector(), item));    
            }            
            base.AddRange(collection);
        }

        public override void Clear()
        {
            _vectorTypeTuples.Clear();
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
                _vectorTypeTuples[index] = new Tuple<Vector, T>(value.ToVector(), value);
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
                        vector[i] = FurtherMath.GetVariance(matrix[i]);
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
