using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using FurtherMath.Base;
using FurtherMath.Base.Collections;
using FurtherMath.SignalProcessing;

namespace WaveComparerLib.Analysis
{   
    public class BandedSignal : IComparable<BandedSignal>, INotifyChanged
    {
        public IntervalArray Signal { get; private set; }
        public IntervalArray Envelope { get; private set; }
        public double MaxAmplitude { get; private set; }
        public double Area { get; set; } // *** need to be able to set this, not encapulated
        public int AttackTime { get; private set; }
        public int DecayTime { get; private set; }

        public BandedSignal(TimeFrequencyGrid stFrequencySpectrum)
        {
            Signal = new IntervalArray(stFrequencySpectrum.yInterval, stFrequencySpectrum.AveY());
            SetEnvelope();
            SetStats();
        }

        void SetEnvelope()
        {
            // *** This needs to tidied up
            var signal = (double[])Signal;
            var envelope = new double[signal.Length];

            int attackLength = ArrayOps.GetIndexOfMax(signal);
            int decayLength = signal.Length - attackLength - 1;
            double[] YAttack = new double[attackLength + 1];
            double[] XAttack = new double[attackLength + 1];
            double[] YDecay = new double[decayLength];
            double[] XDecay = new double[decayLength];
            // Calculate Attack *** needs to be encapsulated
            
            if (XAttack.Length == 1)
            {
                envelope[0] = signal[0];
            }
            else
            {
                for (int i = 0; i < XAttack.Length; i++)
                {
                    XAttack[i] = i;
                }
                Array.Copy(signal, YAttack, YAttack.Length);
                double CovOVarAttack = ArrayOps.GetCovariance(XAttack, YAttack) / ArrayOps.GetVariance(XAttack);
                for (int i = 0; i < XAttack.Length; i++)
                {
                    envelope[i] = CovOVarAttack * (i - XAttack.Average()) + YAttack.Average();
                }
            }

            if (XDecay.Length == 1)
            {
                envelope[envelope.Length - 1] = signal[signal.Length - 1];
            }
            else if (XDecay.Length > 1)
            {
                // Calculate Decay *** needs to be encapsulated
                for (int i = 0; i < XDecay.Length; i++)
                {
                    XDecay[i] = i;
                }

                Array.Copy(signal, attackLength, YDecay, 0, YDecay.Length);
                for (int i = 0; i < YDecay.Length; i++)
                {
                    YDecay[i] = Math.Log(YDecay[i]);
                }
                double CovOVarDecay =
                    ArrayOps.GetCovariance(XDecay, YDecay) / ArrayOps.GetVariance(XDecay);

                for (int i = 0; i < XDecay.Length; i++)
                {
                    envelope[i + XAttack.Length] = Math.Exp(CovOVarDecay * (i - XDecay.Average()) + YDecay.Average());
                }
            }

            Envelope = new IntervalArray(Signal.xInterval, envelope);
        }

        void SetStats()
        {
            // *** This needs to be improved - attackTime is not very useful.
            int k = ArrayOps.GetIndexOfMax(Signal.Values);
            this.MaxAmplitude = this.Signal.Values[k];
            this.Area = this.Signal.Values.Sum();
            // *** Envelope needs some work
            //this.Area = this.Envelope.Values.Sum();
            if (Double.IsNaN(this.Area))
                throw new Exception();
            this.AttackTime = k;
            this.DecayTime = 0; // *** need to work this out - maybe this should be the half life
        }

        public int CompareTo(BandedSignal other)
        {
            return (int)Math.Abs((this.ToValue() - other.ToValue()));            
        }

        //public int CompareTo(int value)
        //{
        //    return (int)Math.Abs((this.Area - value));
        //    // return Math.Abs((int)maxAmplitude - (int)(value as BandedEnvelope).maxAmplitude);
        //}

        public double ToValue()
        {
            return this.Area;
        }

        // TODO Implement
        public event ChangedEventHandler Changed;
    }
}
