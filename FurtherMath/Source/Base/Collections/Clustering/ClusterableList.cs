using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FurtherMath.Base.Collections.Clustering
{
    public class ClusterableList<T> : VectorList<T> 
        where T : class, IVectorConvertable, INotifyChanged
    {   
        public const double ConvergenceLimit = 0.03;
        public const int K = 2; // Number of clusters

        public ObservableList<ObservableCluster<T>> Clusters { get; private set; }
        public double TotalDistance { get; private set; }
        
        private void initialiseClusters()
        {
            Clusters.Clear();            
            // Forgy method initialisation
            //var rnd = new Random();
            //for (int i = 0; i < K; i++)
            //{
            //    var j = rnd.Next(0, this.Count - 1);
            //    Clusters.Add(new FuzzyCluster<T>(this[j].ToVector(), this));
            //}

            // Initialise to random vectors
            //for (int i = 0; i < K; i++)
            //{
            //    Clusters.Add(new FuzzyCluster<T>(Vector.Random(0, this.Count - 1), this));
            //}

            // Forgy method initialisation, with randomisation
            var rnd = new Random();
            for (int i = 0; i < K; i++)
            {
                var j = rnd.Next(0, this.Count - 1);
                var v1 = this[j].ToVector();
                var v2 = Vector.Random(0, 12, v1.Dimension);
                Clusters.Add(new FuzzyCluster<T>(v1 + v2, this));
            }
        }

        public ClusterableList(int dimension)
            : base(dimension)
        {
            Clusters = new ObservableList<ObservableCluster<T>>();
        }

        public void ResetClusters()
        {
            foreach (var c in Clusters)
            {
                c.Reset();
            }
        }

        public void SingleInterationCluster()
        {
            if (Clusters.Count == 0) initialiseClusters();
            SingleInteration();
        }

        private void SingleInteration()
        {
            foreach (var c in Clusters)
            {
                c.Vectors.Clear();
            }

            TotalDistance = 0;
                
            foreach (var t in this)
            {
                var distances = new double[Clusters.Count]; // Distances between each vector and cluster
                var v = t.ToVector();
                ObservableCluster<T> closestCluster = null;
                double closestDistance = double.PositiveInfinity;

                // Calculate distances
                for (int i = 0; i < Clusters.Count; i++)
                {
                    var d = ~(v - Clusters[i].Centroid);
                    distances[i] = d;
                    if (d < closestDistance)
                    {
                        closestCluster = Clusters[i];
                        closestDistance = d;
                    }
                }
                // Assign vector to closest cluster
                closestCluster.Vectors.Add(t);

                var zeros = from zero in distances
                            where zero == 0
                            select zero;

                // Calculate weighting
                double weightTotal = 0;
                for (int i = 0; i < Clusters.Count; i++)
                {   
                    double weightInverse = 0;
                    double weight = 0;

                    if (zeros.Count() > 0)
                    {
                        throw new Exception();
                        //if (distances[i] == 0)
                        //    weight = 1 / zeros.Count();
                        //else
                        //    weight = 0;
                    }
                    else
                    {
                        for (int j = 0; j < distances.Length; j++)
                        {
                            weightInverse += Math.Pow((distances[i] / distances[j]), 2);
                        }
                        weight = 1 / weightInverse;
                    }
                    weightTotal += weight;

                    var fc = (FuzzyCluster<T>)Clusters[i];
                    fc.SetVectorWeight(t, weight);
                }                
                Debug.Assert(Math.Round(weightTotal,4) == 1);
                TotalDistance += closestDistance;
            }

            foreach (var c in Clusters)
            {
                c.SetCentroid();
            }
        }

        public void FuzzyC_MeansCluster()
        {
            initialiseClusters();
            
            double lastTotalDistance = 0;
            TotalDistance = double.PositiveInfinity;

            while (Math.Abs(lastTotalDistance - TotalDistance) > ConvergenceLimit)
            {
                lastTotalDistance = TotalDistance;
                SingleInteration();
            }

            // Reorganise list *** should propably remove this
            this.Clear();
            foreach (var c in Clusters)
            {
                foreach (var v in c.Vectors)
                {
                    this.Add(v);
                }
            }
        }

        public int GetClusterEnum(ObservableCluster<T> cluster)
        {
            return this.Clusters.IndexOf(cluster);
        }
    }
}
