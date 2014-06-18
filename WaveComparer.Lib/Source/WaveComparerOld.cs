using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration;

using WaveComparerLib.Gen_Utils;
using WaveComparerLib.Analysis;

namespace WaveComparerLib
{
    public enum SortTypeOld
    {
        SingleInterationCluster,
        Cluster,
        MainAudioFile,
        LowMidHigh,
        HighMidLow,
        LowMidLow,
    }

    public class WaveComparerOld
    {
        bool _muteAudioFiles = true; // *** Properties.Settings.Default.MuteAudio;
        
        AudioFile _mainAudioFile;
        ClusterableList<AudioFile> _audioFiles;

        public static readonly List<Type> ToolableEnumTypes = new List<Type>() { typeof(SortTypeOld) };

        // Constructor
        public WaveComparerOld(ClusterableList<AudioFile> audioFileList)
        {
            if (audioFileList == null)
                throw new ArgumentNullException("audioFileList");
            _audioFiles = audioFileList;
        }

        // Toolable Properties
        [Toolable("Mute", "Audio"), ConfigurationProperty("muteAudioFiles", DefaultValue = true)]
        public bool MuteAudioFiles { get { return _muteAudioFiles; } set { _muteAudioFiles = value; } }
        [Toolable("Sort Type", "Sort")]
        public SortTypeOld SortBy { get; set; }
        [Toolable("Pitch Shift Audio Files", "Audio")]
        public bool PitchShiftAudioFiles { get; set; } // *** this should be moved to config
        //[Toolable("Log Frequency", "Frequency Analysis")]
        //public bool LogFrequency { get { return Hertz.LogFrequency; } set { Hertz.LogFrequency = value; } }

        // Properties
        public IAudioFile MainAudioFile
        {
            get { return _mainAudioFile; }
            private set
            {
                _mainAudioFile = value;
                if (!MuteAudioFiles) _mainAudioFile.Play();
            }
        }
        
        public AudioFileList AudioFiles { get { return null; } }

        public void LoadMainAudioFile(string fileName)
        {
            if (MainAudioFile != null)
              MainAudioFile = new LazyAudioFile(fileName);
        }

        [Toolable("Reset Clusters", "Sort")]
        public void ResetClusters()
        {
            this.AudioFiles.ResetClusters();
        }

        [Toolable("Sort", "Sort")]
        public void Sort()
        {
            var count = FrequencyPartitionList.Instance.Count();
            var area = count * (count - 1) / 2;
            int midPoint = count / 2;
            //var bench = new Vector();

            switch (SortBy)
            {
                case SortTypeOld.MainAudioFile:

                    //TheAudioFileList.Sort((a, b) => a.CompareTo(MainAudioFile) - b.CompareTo(MainAudioFile));
                    break;

                case SortTypeOld.HighMidLow:

                    for (int i = 0; i < count; i++)
                    {
                        //bench[i] = i * 100 / area;
                    }
                    //AudioFiles.Sort((a, b) => a.CompareTo(bench) - b.CompareTo(bench));
                    break;

                case SortTypeOld.LowMidHigh:

                    for (int i = 0; i < count; i++)
                    {
                        //bench[i] = (count - i - 1) * 100 / area;
                    }
                    //AudioFiles.Sort((a, b) => a.CompareTo(bench) - b.CompareTo(bench));
                    break;

                case SortTypeOld.LowMidLow:

                    //for (int i = 0; i < midpoint; i++)
                    //{
                    //    bench[i] = i;
                    //}
                    //for (int i = midpoint; i < count; i++)
                    //{
                    //    bench[i] = count - i - 1;
                    //}
                    //var total = bench.sum();
                    //for (int i = 0; i < bench.length; i++)
                    //{
                    //    bench[i] *= (100 / total);
                    //}
                    //AudioFiles.Sort((a, b) => a.CompareTo(bench) - b.CompareTo(bench));
                    break;

                case SortTypeOld.Cluster:

                    AudioFiles.FuzzyC_MeansCluster();
                    break;

                case SortTypeOld.SingleInterationCluster:

                    AudioFiles.SingleInterationCluster();
                    break;

                default:
                    throw new Exception("A vaild sort type was not specified");            
            }
        }
    }
}
