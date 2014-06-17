using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using FurtherMath.Base;
using FurtherMath.Base.Collections;
using FurtherMath.SignalProcessing;

using WaveComparerLib.AudioFileIO;
using WaveComparerLib.Gen_Utils;

namespace WaveComparerLib
{
    public class SampledSignal : INotifyChanged
    {
        double _normalisedValue;
        
        // Events
        public event ChangedEventHandler Changed;
        
        // Properties
        public ushort BitDepth { get; private set; }
        public uint SampleRate
        {
            get { return (uint)(1 / NormalisedSignal[0].xInterval); }
            set { SignalProcessing.Resample(this, value); }
        }
        public uint NumberOfChannels { get { return (uint)NormalisedSignal.Length; } }
        public int NumberOfSamples { get { return NormalisedSignal[0].Count(); } }
        public IntervalArray[] NormalisedSignal { get; private set; }
        public double SamplingInterval { get { return NormalisedSignal[0].xInterval; } }
        public double SampleLength
        {
            get
            {
                return NormalisedSignal[0].Length;
            }
        }
            
        public Point this[int channel, int sample]
        {
            get
            {
                return NormalisedSignal[channel][sample];
            }
        }
        public IntervalArray this[int channel]
        {
            get
            {
                return NormalisedSignal[channel];
            }
            set
            {
                NormalisedSignal[channel] = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the SampledSignal class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="numberOfChannels"></param>
        /// <param name="bitDepth"></param>
        /// <param name="sampleRate"></param>
        public SampledSignal(double[] data, uint numberOfChannels, ushort bitDepth, uint sampleRate)
        {
            this.BitDepth = bitDepth;
            
            this.NormalisedSignal = new IntervalArray[numberOfChannels];
            // Process data into seperate channels
            var array = new double[numberOfChannels][];
            for (int i = 0; i < numberOfChannels; i++)
            {
                array[i] = new double[data.Length / numberOfChannels];
            }
            for (int i = 0; i < array[0].Length; i++)
            {
                for (int j = 0; j < numberOfChannels; j++)
                {
                    array[j][i] = data[(numberOfChannels * i) + j];
                    if (Math.Abs(array[j][i]) > _normalisedValue)
                    {
                        _normalisedValue = Math.Abs(array[j][i]);
                    }
                }
            }
            // Normalise signal
            var invNormalisedValue = 1 / _normalisedValue;
            for (int i = 0; i < NormalisedSignal.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    array[i][j] *= invNormalisedValue;
                }
            }
            int k = 0;
            foreach (var a in array)
            {
                NormalisedSignal[k] = new IntervalArray((double)1 / sampleRate, a);
                k++;
            }
            ProcessData();
        }

        /// <summary>
        /// Here we process the audio data, such as trim silence
        /// </summary>
        void ProcessData()
        {
            // threshold of 0.03 is an arbitrary value *** this should be changed to configuration
            SignalProcessing.TrimStart(NormalisedSignal, 0.03f);
            // Remove any audio after a silence of longer than the STFTT window length
            SignalProcessing.TrimAfterSilence(NormalisedSignal, SignalFunctions.WindowLength);
            //
            SignalProcessing.TrimEnd(NormalisedSignal, 0.03f);
        }

        void OnChangedEvent(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public WaveFileFormat GetWaveFileFormat()
        {
            var firstChannel = (double[])NormalisedSignal[0];
            var samples = new double[firstChannel.Length];
            for (uint i = 0; i < firstChannel.Length - 1; i++)
            {
                samples[i] = firstChannel[i] * _normalisedValue;
            }
            return new WaveFileFormat(samples, this.SampleRate, this.BitDepth);
        }

        public void ChangePitch(double ratio)
        {
            // Resample, but don't change sample rate *** should probably rename Resample
            SignalProcessing.Resample(this, (uint)(this.SampleRate * ratio));
            OnChangedEvent(EventArgs.Empty);
        }
    }
}
