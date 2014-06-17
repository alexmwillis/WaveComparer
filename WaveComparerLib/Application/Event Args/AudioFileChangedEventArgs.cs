using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib
{
    public class AudioFileChangedEventArgs : EventArgs
    {
        public readonly AudioFile ChangedAudioFile;

        public AudioFileChangedEventArgs(AudioFile audioFile)
        {
            ChangedAudioFile = audioFile;
        }
    }
}
