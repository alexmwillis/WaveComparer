using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using FurtherMath.Base;

namespace WaveComparer.Lib.Analysis
{
    public delegate Point intervalTransform(Point point);  

    public class FrequencySpectrum : IntervalArray
    {
        private bool _logarithmicScale = Properties.Settings.Default.FrequencyLogarithmicScale;

        intervalTransform logTransformX = (p) => { return new Point(Math.Log(p.X), p.Y); };
        
        public override double Length
        {
            get
            {
                if (_logarithmicScale) { return Math.Log(base.Length); }
                else { return base.Length; }
            }
        }

        public FrequencySpectrum(double xInterval, double[] yValues) : base(xInterval, yValues) { }

        public override Point this[int index]
        {
            get
            {
                if (_logarithmicScale)
                {
                    return logTransformX(base[index]);
                }
                else
                {
                    return base[index];
                }
            }
        }

        public override List<Point> ToPointList()
        {
            var pl = base.ToPointList();
            if (_logarithmicScale)
                pl.RemoveAt(0);
            return pl;
        }
    }
}
