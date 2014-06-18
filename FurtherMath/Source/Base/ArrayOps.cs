using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurtherMath.Base
{
    public class ArrayOps
    {
        public static double GetCovariance(double[] X, double[] Y)
        {
            int n = X.Length;
            // Get average
            double E_X = X.Average();
            double E_Y = Y.Average();
            double covariance = 0;
            for (int i = 0; i < n; i++)
            {
                covariance += (X[i] - E_X) * (Y[i] - E_Y) / n;
            }
            return covariance;
        }

        public static double GetVariance(double[] X)
        {
            return GetCovariance(X, X);
        }

        public static int GetIndexOfMax(double[] array)
        {
            double maxValue = 0;
            int maxIndex = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > maxValue)
                {
                    maxValue = array[i];
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        public static double[] AverageArray(double[][] array)
        {
            // Averages on the first dimension
            var result = new double[array[0].Length];
            var n = array.Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    result[j] += array[i][j] / n;
                }
            }
            return result;
        }

        public static int AreaMidPoint<T>(T[] array) where T : IConvertible
        {
            var halfTotal = array.Sum<T>((v) => Convert.ToDouble(v)) / 2;
            var index = 0;
            if (halfTotal == 0) throw new ArgumentException("Array is empty");
            double sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += Convert.ToDouble(array[i]);
                if (sum > halfTotal)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
