using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;

namespace FurtherMath.Measures
{
    public class TrainingSetMeasure<T> : IDefinableMeasure<T>
        where T : class, IVectorConvertable
    {
        Dictionary<VectorPair, double> _distanceDictionary = new Dictionary<VectorPair,double>();
        IMeasure<T> _baseMeasure = new EuclideanMeasure<T>();

        private void SetDistance(T a, T b, double d)
        {
            if (a == b)
                throw new ArgumentException("a and b can't be equal");

            var key = new VectorPair(a.ToVector(), b.ToVector());
            if (_distanceDictionary.ContainsKey(key))
            {
                _distanceDictionary.Remove(key);
            }
            _distanceDictionary.Add(key, d);
        }

        public double Distance(T a, T b)
        {
            var key = new VectorPair(a.ToVector(), b.ToVector());
            if (_distanceDictionary.ContainsKey(key))
            {
                return _distanceDictionary[key];
            }
            else
            {
                return _baseMeasure.Distance(a, b);
            }
        }
                
        public void ScaleDistance(T a, T b, double factor)
        {
            this.SetDistance(a, b, factor * this.Distance(a, b));
        }
    }
}
