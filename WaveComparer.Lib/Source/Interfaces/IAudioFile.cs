using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;
using FurtherMath.Base.Collections;

using WaveComparer.Lib.Analysis;

namespace WaveComparer.Lib.Interfaces
{
    public interface IAudioFile : IVectorConvertable, INotifyChanged
    {
        string ShortFileName { get; }
        Vector AnalysisVector { get; }

        SampledSignal Signal { get; }
        FrequencyAnalysis Analysis { get; }

        void Play();

        void RateGood();

        void RateBad();
    }
}
