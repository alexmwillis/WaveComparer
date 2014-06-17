using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.ComponentModel;
using System.Diagnostics;

using FurtherMath.Base;
using FurtherMath.Base.Collections;

using WaveComparerLib.AudioFileIO;
using WaveComparerLib.Interfaces;
using WaveComparerLib.Gen_Utils;
using WaveComparerLib.Analysis;

namespace WaveComparerLib
{

    public enum FileType
    {
        WaveFile,
        UnknownFile
    }

    public enum VectorType
    {
        BandedSignalsVector,
        STFrequencySpectrumVector
    }

    public abstract class AudioFile : IAudioFile
    {
        private SampledSignal _signal;
        private FrequencyAnalysis _analysis;
        
        public const int BaseSampleRate = 22050; // 44100;

        // Properties
        public string FileName { get; private set; }
        public string ShortFileName
        {
            get { return Utils.GetShortFileName(FileName); }
        }
        public SampledSignal Signal
        {
            get { return _signal; }
        }

        public FrequencyAnalysis Analysis
        {
            get { return _analysis; }
        }        

        public static VectorType ToVectorMode
        {
            get
            {
                return Enum<VectorType>.Parse(WaveComparerLib.Properties.Settings.Default.AudioFileToVectorMode);
            }
        }

        public AudioFile(string inFileName)
        {
            this.FileName = inFileName;

            _signal = this.ReadFile();
            _analysis = new FrequencyAnalysis(Signal);

            // TODO this should be moved, onChanged should recreate vector
            //analysis.BandedSignals.Changed += (o, e) => this.OnChanged();    
        }

        public void PlayFile()
        {
            var player = new SoundPlayer(FileName);
            player.Play();
        }

        public void Play()
        {
            var writer = new WaveFileWriter();
            var stream = writer.WriteToStream(this.Signal.GetWaveFileFormat());
            var player = new SoundPlayer(stream);
            player.Play();
            stream.Close();
        }

        public override string ToString()
        {
            return ShortFileName;
        }

        protected abstract SampledSignal ReadFile();
                
        /// <summary>
        /// Shifts the pitch to match other AudioFile
        /// </summary>
        /// <param name="other"></param>
        public void PitchShiftTo(AudioFile other)
        {
            Signal.ChangePitch(GetFrequencyRatio(other));
        }

        double GetFrequencyRatio(AudioFile other)
        {
            return (double)this.Analysis.CenterOfSTFrequencySpectrum / other.Analysis.CenterOfSTFrequencySpectrum;
        }

        protected virtual void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        public Vector ToVector()
        {
            Vector vector;
            var analysis = this.Analysis;

            switch (AudioFile.ToVectorMode)
            {
                case VectorType.BandedSignalsVector:

                    vector = analysis.BandedSignals.ToVector();
                    break;

                case VectorType.STFrequencySpectrumVector:

                    vector = analysis.STFrequencySpectrum.ToVector();
                    break;

                default:

                    vector = Vector.EmptyVector;
                    break;
            }
            return vector;
        }

        public static int GetVectorLength()
        {
            switch (AudioFile.ToVectorMode)
            {
                case VectorType.BandedSignalsVector:

                    // *** remove this hardcoding
                    return 3;

                case VectorType.STFrequencySpectrumVector:

                    // *** remove this hardcoding
                    return 1600;
                //return FrequencyAnalysis.stfftNumberOfFrames * FrequencyAnalysis.FFTSize;

                default:

                    return 0;
            }
        }

        public Vector AnalysisVector
        {
            get { return this.ToVector(); }
        }

        public event ChangedEventHandler Changed;

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~AudioFile()
        {
            string msg = string.Format("{0} : {1} ({2}) ({3}) Finalized", 
                DateTime.Now, this.GetType().Name, this.FileName, this.GetHashCode());
            Debug.WriteLine(msg);
        }
#endif



        public void RateGood()
        {
            throw new NotImplementedException();
        }

        public void RateBad()
        {
            throw new NotImplementedException();
        }
    }
}
