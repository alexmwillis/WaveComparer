using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparerLib.Helpers;
using WaveComparerLib.Interfaces;

using Accord.Statistics.Analysis;

namespace FurtherMath.Base.Collections
{
    public class PCAVectorList<T> : VectorList<T> where T : IVectorConvertable, INotifyChanged
    {
        PrincipalComponentAnalysis _PCA;
        int _numberOfComponents;

        public PCAVectorList(int vectorDimension)
            : base(vectorDimension)
        {
            //this.CollectionChanged += (o, e) =>
            //{
            //    if (this.Count > 0)
            //        this.DoPCA();
            //};
        }

        public void DoPCA()
        {
            _PCA = new PrincipalComponentAnalysis(this.ToDoubleArray(), PrincipalComponentAnalysis.AnalysisMethod.Covariance);
            _PCA.Compute();
            _numberOfComponents = _PCA.GetNumberOfComponents(0.8f);
            // Transform all vectors
            for (int i = 0; i < _vectorTypeTuples.Count; i++)
            {
                var vtTuple = _vectorTypeTuples[i];
                var array = (double[])vtTuple.Item1;
                var d1matrix = new double[1, array.Length];
                for (int j = 0; j < array.Length; j++)
                {
                    d1matrix[0, j] = array[j];
                }
                var transMatrix = _PCA.Transform(d1matrix, _numberOfComponents);
                var transVector = new Vector(ArrayHelper.ToSingleDimension<double>(transMatrix));
                vtTuple = new Tuple<Vector, T>(transVector, vtTuple.Item2);
            }
        }

        public Vector PCAComponentProportions { get { return GetPCAComponentProportions(); } }
               
        public Vector GetPCAComponentProportions()
        {
            // TODO null on startup, fix this when next looking at PCA
            //var v = new Vector(_numberOfComponents);
            //var cp = _PCA.ComponentProportions;
            //for (int i = 0; i < _numberOfComponents; i++)
            //{
            //    v[i] = cp[i];
            //}
            //return v;
            return Vector.EmptyVector;
        }
    }
}
