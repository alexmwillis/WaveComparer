using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;

using WaveComparer.Lib.Interfaces;

namespace WaveComparer.Lib
{
    public enum SortType
    {        
        ByAudioFile,
        ByBenchMark
    }

    public class Comparer
    {
        AudioFileList _audioFiles;
        IAudioFile _audioFile;
        Vector _benchMark;
        SortType _sortType;

        public Comparer()
        {
            _audioFiles = new AudioFileList();
        }

        public static readonly List<SortType> SortByOptions = Enum.GetValues(typeof(SortType)).Cast<SortType>().ToList();

        public AudioFileList AudioFiles
        {
            get
            {
                return _audioFiles;
            }
        }
        public IAudioFile AudioFile
        {
            get { return _audioFile; }
            set
            {
                _audioFile = value;
                _audioFiles.SortBy = value;
            }
        }
        public Vector BenchMark { get { return _benchMark; } set { _benchMark = value; } }
        public SortType SortType { get { return _sortType; } set { _sortType = value; } }

        public void Sort()
        {
            switch (SortType)
            {
                case SortType.ByAudioFile:

                    if (this.AudioFile != null)
                    {                        
                        _audioFiles.Sort();
                    }
                    else
                    {
                        throw new Exception("Select an Audio file to compare against"); 
                    }
                    break;

                case SortType.ByBenchMark:

                    if (this.BenchMark != null)
                    {
                        //_audioFiles.Sort(this.BenchMark);
                    }
                    else
                    {
                        // *** display message here
                    }
                    break;

                default:

                    throw new Exception("A vaild sort type was not specified");
            }
        }

        public bool CanSort()
        {
            switch (SortType)
            {
                case SortType.ByAudioFile:

                    return this.AudioFile != null;

                case SortType.ByBenchMark:

                    return this.BenchMark != null;

                default:

                    return false;
            }
        }

        //public event EventHandler AudioFileChanged;

        //public void LoadAudioFileFromList(int index)
        //{
        //    this.AudioFile = this[index];
        //    this.AudioFileChanged(this, EventArgs.Empty);
        //}

        public void SetAudioFiles(List<IAudioFile> list)
        {
            _audioFiles.Clear();
            _audioFiles.AddRange(list);
        }
    }
}
