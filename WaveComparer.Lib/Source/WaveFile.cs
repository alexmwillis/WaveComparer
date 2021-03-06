﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparer.Lib.AudioFileIO;
using WaveComparer.Lib.XML_Serialisation;

namespace WaveComparer.Lib
{
    public class WaveFile : AudioFile
    {
        public WaveFile(string fileName) : base(fileName) { }

        protected override SampledSignal ReadFile()
        {
            WaveFileReader reader = new WaveFileReader(FileName);
            var waveFile = reader.ReadData();
            var doubleArray = waveFile.dataChunk.doubleArray;
            var numberOfChannels = waveFile.formatChunk.wChannels;
            var bitDepth = waveFile.formatChunk.dwBitsPerSample;
            var sampleRate = waveFile.formatChunk.dwSamplesPerSec;
            var signal = new SampledSignal(doubleArray, numberOfChannels, bitDepth, sampleRate);
            signal.SampleRate = BaseSampleRate; // Resamples signal
            return signal;
        }
    }
}
