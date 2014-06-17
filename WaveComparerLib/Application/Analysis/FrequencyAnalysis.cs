using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

using FurtherMath.Base;
using FurtherMath.SignalProcessing;

using WaveComparerLib.Interfaces;
using WaveComparerLib.Helpers;

namespace WaveComparerLib.Analysis
{
    public class FrequencyAnalysis
    {
        // Consts
        public const int FFTSize = 4096; 
                
        public const int NumberOfSamplesInSTFFT = 20000;
        public static int stfftNumberOfFrames = NumberOfSamplesInSTFFT / SignalFunctions.HopSize;

        public const double FreqPerBin = (double)AudioFile.BaseSampleRate / FFTSize;

        private SampledSignal _signal;

        public FrequencyAnalysis(SampledSignal signal)
        {
            _signal = signal;
            SpectrumAnalysis();
            // Retrigger spectrum analysis when signal is changed
            _signal.Changed += (o, e) => { SpectrumAnalysis(); };
            BandedSignals = new BandedSignalsList(STFrequencySpectrum); 
        }

        // Properties
        public FrequencySpectrum FrequencySpectrum { get; private set; }
        public TimeFrequencyGrid STFrequencySpectrum { get; private set; } // Short Time
        public FrequencySpectrum AveragedSTFrequencySpectrum { get; private set; } // Averaged Short Time
        public int CenterOfSTFrequencySpectrum { get; private set; }

        public BandedSignalsList BandedSignals { get; private set; }
        
        public int ShortTimeCount
        {
            get
            {
                return this.STFrequencySpectrum.yLength;
            }
        }
        public double ShortTimeLength
        {
            get
            {
                return this.STFrequencySpectrum.yInterval * this.STFrequencySpectrum.yLength;
            }
        }

        private void PopulateFrequencySpectrum(SampledSignal signal)
        {
            var data = (double[])signal.NormalisedSignal[0];

            if (data.Length < FFTSize)
            {
                // Zero pad
                Array.Resize(ref data, FFTSize);
            }
            var xRe = data;
            var xIm = new double[xRe.Length];
            double[] yRe, yIm;

            SignalFunctions.fft(FFTSize, xRe, xIm, out yRe, out yIm);
            // Only take half of output, as it is symetric about centre (since input signal is real)
            var halfYRe = new double[yRe.Length / 2];
            var halfYIm = new double[yIm.Length / 2];
            Array.Copy(yRe, halfYRe, halfYRe.Length);
            Array.Copy(yIm, halfYIm, halfYIm.Length);
            FrequencySpectrum = new FrequencySpectrum(FreqPerBin, SignalFunctions.Amplitude(halfYRe, halfYIm));
        }

        private void PopulateSTFrequencySpectrum(SampledSignal signal)
        {
            var data = (double[])signal.NormalisedSignal[0];

            var xRe = data;
            var xIm = new double[xRe.Length];
            double[][] yRe, yIm;

            SignalFunctions.stfft(FFTSize, xRe, xIm, out yRe, out yIm, stfftNumberOfFrames);

            var array = new double[yRe.Length][];
            for (int i = 0; i < array.Length; i++)
            {
                // Only take half of output, as it is symetric about centre (since input signal is real)
                var halfSTYRe = new double[yRe[i].Length / 2];
                var halfSTYIm = new double[yIm[i].Length / 2];
                Array.Copy(yRe[i], halfSTYRe, halfSTYRe.Length);
                Array.Copy(yIm[i], halfSTYIm, halfSTYIm.Length);
                array[i] = SignalFunctions.Amplitude(halfSTYRe, halfSTYIm);
            }
            STFrequencySpectrum = new TimeFrequencyGrid(signal.SamplingInterval * SignalFunctions.HopSize, FreqPerBin, array);
        }

        private void SpectrumAnalysis()
        {
            PopulateFrequencySpectrum(_signal);
            PopulateSTFrequencySpectrum(_signal);
            AveragedSTFrequencySpectrum = new FrequencySpectrum(STFrequencySpectrum.yInterval,
                ArrayOps.AverageArray((double[][])STFrequencySpectrum));
            CenterOfSTFrequencySpectrum =
                ArrayOps.AreaMidPoint<double>((double[])AveragedSTFrequencySpectrum);
        }        
    }
}
