using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using FurtherMath.Base;

using WaveComparerLib.Interfaces;  

namespace WaveComparerLib
{
    public struct Point<T> where T : IConvertible
    {
        public readonly T X;
        public readonly T Y;

        public Point(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public static explicit operator Point(Point<T> point)
        {
            var dbleX = Convert.ToDouble(point.X);
            var dbleY = Convert.ToDouble(point.Y);
            return new Point((int)(dbleX * 100), (int)(dbleY * 100));
        }
    }

    /// <summary>
    /// Reresents an array of points where each point is evenly spaced along the x-axis by the given interval.
    /// </summary>
    public class IntervalArray : IPointListConvertable
    {
        public readonly double xInterval;

        public double[] Values { get; set; }
        public virtual double Length { get { return this.Count() * xInterval; } } 

        public IntervalArray(double xInterval, double[] yValues)
        {
            this.Values = yValues;
            this.xInterval = xInterval;
        }

        public virtual Point this[int index]
        {
            get
            {
                return new Point(xInterval * index, Values[index]);
            }
        }

        public static explicit operator double[](IntervalArray array)
        {
            return array.Values;
        }

        //public static explicit operator List<Point<double>>(IntervalArray array)
        //{
        //    var lp = new List<Point<double>>();
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        lp.Add(array[i]);
        //    }
        //    return lp;
        //}

        public IntervalArray GetSubset(double xFloor, double xCeiling)
        {
            var indexFloor = (int)Math.Ceiling(xFloor / xInterval);
            var indexCeiling = (int)Math.Floor(xCeiling / xInterval);
            double[] array = new double[indexCeiling - indexFloor];
            Array.Copy(Values, indexFloor, array, 0, indexCeiling - indexFloor);
            return new IntervalArray(xInterval, array);
        }

        public int Count()
        {
            return Values.Length;
        }

        public virtual List<Point> ToPointList()
        {
            var pl = new List<Point>();
            for (int i = 0; i < this.Count(); i++)
            {
                pl.Add((Point)this[i]);
            }
            return pl;
        }
    }
}
