using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;

using FurtherMath.Base;
using FurtherMath.Base.Collections;

using WaveComparer.Lib.Gen_Utils;
using WaveComparer.Lib.Interfaces;
using WaveComparer.Lib.Analysis;
using WaveComparer.Lib.Event_Args;

namespace WaveComparer.Lib.Analysis
{
    public class BandedSignalsList 
        : ObservableList<BandedSignal>, IVectorConvertable, IPointListsConvertable
    {
        TimeFrequencyGrid stFrequencySpectrum;

        public BandedSignalsList(TimeFrequencyGrid stFrequencySpectrum)
        {
            this.stFrequencySpectrum = stFrequencySpectrum;
            // This was intended to manage the frequency partition list change and this class implemented 
            // IWeakEventListener. I don't think this functionality is required.
            //ChangedEventManager.AddListener(FrequencyPartitionList.Instance, this);
            
            this.SetBandedSignals();
        }

        public void SetBandedSignals()
        {
            double totalArea = 0;
            this.Clear();
            for (int i = 0; i < FrequencyPartitionList.Instance.Count(); i++)
            {
                var freqPartition = FrequencyPartitionList.Instance.GetBand(i);
                var bandedSignal = new BandedSignal(
                    (TimeFrequencyGrid)stFrequencySpectrum.SubsetY(freqPartition[0].Hertz, freqPartition[1].Hertz)
                    );
                Add(bandedSignal);
                totalArea += bandedSignal.Area;
            }
            // Average Areas
            foreach (var bandedSignal in this)
            {
                bandedSignal.Area = 100 * bandedSignal.Area / totalArea;
                this.OnItemChanged(bandedSignal);
            }            
        }

        public void SetBandedSignals(object sender, EventArgs e)
        {
            this.SetBandedSignals();
        }

        public FurtherMath.Base.Vector ToVector()
        {
            var r = new FurtherMath.Base.Vector(this.Count);
            for (int i = 0; i < r.Dimension; i++)
            {
                r[i] = this[i].ToValue();                
            }
            return r;
        }

        public List<List<Point>> ToPointLists()
        {
            var pll = new List<List<Point>>();
            //pll.Add(this[0].Signal.ToPointList());
            //pll.Add(this[0].Envelope.ToPointList());
            foreach (var bs in this)
            {
                pll.Add(bs.Signal.ToPointList());
            }
            return pll;
        }

        //public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        //{
        //    this.SetBandedSignals();
        //    return true;
        //}
    }
}
