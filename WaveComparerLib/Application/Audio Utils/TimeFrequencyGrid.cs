using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparerLib.Analysis;
using WaveComparerLib.Helpers;

namespace WaveComparerLib
{
    public class TimeFrequencyGrid : GridArray
    {
        public TimeFrequencyGrid(double xInterval, double yInterval, double[][] values) : base(xInterval, yInterval, values) { }
        
        public override IntervalArray this[int xIndex]
        {
            get
            {
                return new FrequencySpectrum(xInterval, Values[xIndex]);
            }
        }

        public override GridArray SubsetY(double yFloor, double yCeiling)
        {
            return new TimeFrequencyGrid(xInterval, yInterval, SubsetYValues(yFloor, yCeiling));
        }
    }
}
