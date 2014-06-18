using System;
using System.Diagnostics;
using System.Collections.Generic;

using FurtherMath.Base.Collections;

using WaveComparer.Lib.Interfaces;

namespace WaveComparer.Lib
{
    public class AudioFileList : TrainingVectorList<IAudioFile>
    {
        public AudioFileList()
            : base(AudioFile.GetVectorLength(), WaveComparer.Lib.Properties.Settings.Default.MeasureLocation)
        { }
    }
}
