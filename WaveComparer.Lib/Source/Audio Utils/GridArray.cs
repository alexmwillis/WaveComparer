using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;

using WaveComparer.Lib.Interfaces;
using WaveComparer.Lib.Helpers;

namespace WaveComparer.Lib
{
    public struct GridPoint<T> where T : IConvertible
    {
        public readonly T X;
        public readonly T Y;
        public readonly T Z;

        public GridPoint(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    public class GridArray : IVectorConvertable
    {
        public readonly double xInterval;
        public readonly double yInterval;

        public double[][] Values;

        public int xLength { get { return Values[0].Length; } }
        public int yLength { get { return Values.Length; } }
        
        public GridArray(double xInterval, double yInterval, double[][] values)
        {
            this.Values = values;
            this.xInterval = xInterval;
            this.yInterval = yInterval;
        }

        public GridPoint<double> this[int xIndex, int yIndex]
        {
            get
            {
                return new GridPoint<double>(xIndex * xInterval, yIndex * yInterval, Values[xIndex][yIndex]);
            }
        }

        public virtual IntervalArray this[int xIndex]
        {
            get
            {
                return new IntervalArray(xInterval, Values[xIndex]);
            }
        }

        public GridArray SubsetX(double xFloor, double xCeiling)
        {
            var indexFloor = (int)Math.Ceiling(xFloor / xInterval);
            var indexCeiling = (int)Math.Ceiling(xCeiling / xInterval);
            var array = new double[indexCeiling - indexFloor][];
            for (int i = 0; i < indexCeiling - indexFloor; i++)
            {
                array[i] = Values[i + indexFloor];
            }
            return new GridArray(xInterval, yInterval, array);
        }

        protected double[][] SubsetYValues(double yFloor, double yCeiling)
        {
            var indexFloor = (int)Math.Ceiling(yFloor / yInterval);
            var indexCeiling = (int)Math.Ceiling(yCeiling / yInterval);
            var array = new double[Values.Length][];
            for (int i = 0; i < Values.Length; i++)
            {
                array[i] = new double[indexCeiling - indexFloor];
                for (int j = 0; j < indexCeiling - indexFloor; j++)
                {
                    array[i][j] = Values[i][j + indexFloor];
                }
            }
            return array;
        }

        public virtual GridArray SubsetY(double yFloor, double yCeiling)
        {
            return new GridArray(xInterval, yInterval, SubsetYValues(yFloor, yCeiling));
        }

        public double SumY(int xIndex)
        {
            double sum = 0;
            for (int i = 0; i < Values[xIndex].Length; i++)
            {
                sum += Values[xIndex][i];
            }
            return sum;
        }

        public double[] AveY()
        {
            var ave = new double[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                ave[i] = SumY(i) / Values[i].Length;
            }
            return ave;
        }

        public int Count()
        {
            return Values.Length;
        }

        public virtual Vector ToVector()
        {
            // *** remove resize
            var d1Values = ArrayHelper.ToSingleDimension<double>(
                ArrayHelper.Resize(this.Values, (double)1 / 8, (double)1 / 32)
                );
            return new Vector(d1Values);
        }

        public static explicit operator double[][](GridArray grid)
        {
            return grid.Values;
        }
    }
}
