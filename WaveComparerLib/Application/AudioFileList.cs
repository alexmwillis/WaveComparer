using System;
using System.Diagnostics;
using System.Collections.Generic;

using FurtherMath.Base.Collections;

using WaveComparerLib.Interfaces;

namespace WaveComparerLib
{
    public class AudioFileList : TrainingVectorList<IAudioFile>
    {
        public AudioFileList()
            : base(AudioFile.GetVectorLength(), WaveComparerLib.Properties.Settings.Default.MeasureLocation)
        { }
    }
}
