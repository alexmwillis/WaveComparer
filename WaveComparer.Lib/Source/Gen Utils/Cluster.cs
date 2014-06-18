using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using WaveComparerLib.Interfaces;

namespace WaveComparerLib.Gen_Utils
{
    public class ObservableCluster<T> : INotifyChanged where T : IVectorConvertable, INotifyChanged
        
    {
        private Vector initialCenter;

        public event ChangedEventHandler Changed;

        public Vector Centroid { get; protected set; }
        public Vector Mean
        {
            get
            {
                if (Vectors.Count > 0) return VectorOps.Average(Vectors.ConvertAll<Vector>((t) => t.ToVector()));
                else return Centroid;
            }
        }
        public ObservableList<T> Vectors { get; private set; }

        public ObservableCluster(Vector initialCenter)
        {
            this.initialCenter = initialCenter;
            this.Centroid = initialCenter;
            this.Vectors = new ObservableList<T>();
        }

        /// <summary>
        /// Sets the Marker to the current center of the cluster and returns the distance between them 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns>The distance between the current center and the marker</returns>
        public virtual void SetCentroid()
        {
            Centroid = Mean;
            this.OnChanged();
        }

        public virtual void Reset()
        {
            this.Centroid = initialCenter;
            this.OnChanged();
        }

        public void OnChanged()
        {
            if (this.Changed != null)
                this.Changed(this, EventArgs.Empty);
        }
    }

    public class FuzzyCluster<T> : ObservableCluster<T> where T : IVectorConvertable, INotifyChanged
    {
        List<Tuple<double, T>> WeightVectorTuples = new List<Tuple<double, T>>();

        public double TotalWeight
        {
            get
            {
                double total = 0;
                foreach (var tuple in WeightVectorTuples)
                {
                    total += tuple.Item1;
                }
                return total;
            }
        }

        public Vector WeightedMean
        {
            get
            {
                if (Vectors.Count > 0)
                {
                    var weightedVectors = new List<Vector>();
                    for (int i = 0; i < WeightVectorTuples.Count; i++)
                    {
                        var weightedVector = WeightVectorTuples[i].Item1 * WeightVectorTuples[i].Item2.ToVector();
                        weightedVectors.Add(weightedVector);
                    }
                    return VectorOps.Average(weightedVectors);
                }
                else return Centroid;
            }
        }

        public FuzzyCluster(Vector initialCenter, List<T> vectors)
            : base(initialCenter)
        {
            for (int i = 0; i < vectors.Count; i++)
            {
                WeightVectorTuples.Add(new Tuple<double, T>(0, vectors[i]));
            }
        }

        public void SetVectorWeight(T vector, double weight)
        {
            var tuple = WeightVectorTuples.Find((t) => (object)t.Item2 == (object)vector);
            WeightVectorTuples.Remove(tuple);
            WeightVectorTuples.Add(new Tuple<double, T>(weight, tuple.Item2));

            // *** this needs to be raised by another event, e.g. this.OnChanged()?
            // OnPropertyChanged(new PropertyChangedEventArgs("TotalWeight")); // *** this could be costly
        }

        public override void SetCentroid()
        {
            Centroid = WeightedMean;
            this.OnChanged();
        }
    }
}
