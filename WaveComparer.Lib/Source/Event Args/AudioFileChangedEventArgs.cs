using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparer.Lib
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
