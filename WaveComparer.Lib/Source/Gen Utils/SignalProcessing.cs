using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparer.Lib.Gen_Utils
{
    public static class SignalProcessing
    {
        public static void Resample(SampledSignal signal, uint destinationSampleRate)
        {
            // Basic resample
            if (signal.SampleRate != destinationSampleRate)
            {
                var sourceSamples = signal.NumberOfSamples;
                var destinationSamples = destinationSampleRate * sourceSamples / signal.SampleRate;
                var conversionFactor = (double)sourceSamples / destinationSamples;

                for (int i = 0; i < signal.NumberOfChannels; i++)
                {
                    var resampleArray = new double[destinationSamples];
                    resampleArray[0] = signal[i,0].Y;

                    for (int j = 1; j < destinationSamples - 1; j++)
                    {
                        int lowerBound = (int)Math.Floor(j * conversionFactor);
                        int upperBound = (int)Math.Ceiling(j * conversionFactor);
                        double weight = lowerBound / (j * conversionFactor);
                        resampleArray[j] = weight * signal[i,lowerBound].Y + (1 - weight) * signal[i,upperBound].Y;
                    }
                    // *** thus prevents out of bounds error, need futher analysis as to why required
                    resampleArray[destinationSamples - 1] = signal[i, sourceSamples - 1].Y;
                    signal[i] = new IntervalArray((double)1 / destinationSampleRate, resampleArray);
                }
            }
        }

        /// <summary>
        /// If silence greater than silenceLength, remove remainder of audio
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="silenceLength"></param>
        public static void TrimAfterSilence(IntervalArray[] signal, int silenceLength)
        {
            var consecutiveZeroCount = 0;
            var firstChannel = (double[])signal[0];
            // Currently only checks channel 1 ***
            for (int i = 0; i < firstChannel.Length - 3; i++)
            {
                if (firstChannel[i] == 0)
                {
                    consecutiveZeroCount += 1;
                }
                else
                {
                    consecutiveZeroCount = 0;
                }
                if (consecutiveZeroCount > silenceLength)
                {
                    // for each channel trim data after index i
                    for (int j = 0; j < signal.Length; j++)
                    {
                        var array = new double[i];
                        Array.Copy((double[])signal[j], 0, array, 0, array.Length);
                        signal[j].Values = array;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Trim audio from beginning and end of signal that falls under threshold
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="threshold"></param>
        public static void TrimStart(IntervalArray[] signal, float threshold)
        {
            var firstChannel = (double[])signal[0];
            for (int i = 0; i < firstChannel.Length; i++)
            {
                if (Math.Abs(firstChannel[i]) > threshold)
                {
                    // for each channel trim data before index i
                    for (int j = 0; j < signal.Length; j++)
                    {
                        var array = new double[signal[j].Count() - i];
                        Array.Copy((double[])signal[j], i, array, 0, array.Length);
                        signal[j].Values = array;
                    }
                    break;
                }
            }
        }

        public static void TrimEnd(IntervalArray[] signal, float threshold)
        {
            var firstChannel = (double[])signal[0];
            for (int i = 0; i < firstChannel.Length; i++)
            {
                if (Math.Abs(firstChannel[firstChannel.Length - i - 1]) > threshold)
                {
                    // for each channel trim data after index i
                    for (int j = 0; j < signal.Length; j++)
                    {
                        var array = new double[signal[j].Count() - i];
                        Array.Copy((double[])signal[j], 0, array, 0, array.Length);
                        signal[j].Values = array;
                    }
                    break;
                }
            }
        }
    }
}
