﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparer.Lib.WindowsFileSystem;
using WaveComparer.Lib.Interfaces;
using WaveComparer.Lib.Analysis;

namespace WaveComparer.Lib
{
    public class ComparerManager
    {
        Comparer _waveComparer = new Comparer();
        AudioFileLoader _audioFileLoader;
        AudioFilesLoader _audioFilesLoader;

        public ComparerManager()
        {
            var asStrings = Properties.Settings.Default.FrequencyPartitions.Split(',');
            float[] asFloats = Array.ConvertAll<string,float>(asStrings, x => float.Parse(x));

            FrequencyPartitionList.Instance.AddPartitions(asFloats);

            _audioFilesLoader = new AudioFilesLoader();
            _audioFilesLoader.Loaded += (o, e) => _waveComparer.SetAudioFiles(e.LoadedObject);
            _audioFileLoader = new AudioFileLoader();
            _audioFileLoader.Loaded += (o, e) => SetComparerAudioFile(e.LoadedObject);
                        
            WaveComparerToolBox.Initialise();

            var playAudioFile = new Action<object>(
                (o) =>
                {
                    if (o is WindowsAudioFile)
                        (o as WindowsAudioFile).Play();
                });
            var loadAudioFile = new Action<object>(
                (o) =>
                {
                    if (o is WindowsAudioFile)
                        _audioFileLoader.Load((o as WindowsAudioFile).FullName);
                });            

            WaveComparerToolBox.Instance.AddAvailableAction(playAudioFile);
            WaveComparerToolBox.Instance.AddAvailableAction(loadAudioFile);

        }      

        public Comparer Comparer { get { return _waveComparer; } }
        public AudioFileLoader FileLoader { get { return _audioFileLoader; } }
        public AudioFilesLoader FilesLoader { get { return _audioFilesLoader; } }

        public void SetComparerAudioFile(IAudioFile audioFile)
        {
            _waveComparer.AudioFile = audioFile;
        }
    }
}
