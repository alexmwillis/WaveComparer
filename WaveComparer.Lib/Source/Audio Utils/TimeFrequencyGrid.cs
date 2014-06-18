using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparer.Lib.Analysis;
using WaveComparer.Lib.Helpers;

namespace WaveComparer.Lib
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
