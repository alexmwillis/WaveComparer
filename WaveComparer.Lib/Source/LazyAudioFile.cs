using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FurtherMath.Base;
using FurtherMath.Base.Collections;

using WaveComparer.Lib.XML_Serialisation;
using WaveComparer.Lib.Interfaces;
using WaveComparer.Lib.Gen_Utils;

namespace WaveComparer.Lib
{
    public class LazyAudioFile : IAudioFile
    {
        private Vector _analysisVector;
        private string _fileName;
        private AudioFileFactory _factory = new AudioFileFactory();
        private XmlLibrary<XmlAudioFile> _library = WaveComparerToolBox.Instance.XmlAudioFileLibrary;

        public Vector AnalysisVector
        {
            get { return _analysisVector; }
        }

        public LazyAudioFile(string fileName)
        {
            _fileName = fileName;

            var newFile = false;

            XmlAudioFile xAudioFile = _library.GetFile(new XmlFile(fileName));
            if (xAudioFile == null)
            {
                // TODO log missing files
                newFile = true;
                // Create a stub file - this will be populated by the AudioFile
                xAudioFile = new XmlAudioFile(fileName);
            }

            Vector vector;
            if (xAudioFile.isLoaded)
            {
                vector = (Vector)xAudioFile.AnalysisVector;
            }
            else
            {
                vector = this.GetAudioFile().AnalysisVector;
                xAudioFile.AnalysisVector = (XmlVector)vector;
            }
            _analysisVector = vector;

            // Add file if Audio File creation succeeds
            if (newFile)
                _library.AddFile(xAudioFile);
        }

        protected AudioFile GetAudioFile()
        {
            // TODO is this the best design, i.e. not keeping a local copy of the audiofile? This save memory, but may be slow 
            // if something is observing it.
            return _factory.GetAudioFile(_fileName);
        }

        public Vector ToVector()
        {
            return _analysisVector;
        }
                
        // TODO Implement
        public event ChangedEventHandler Changed;

        public string ShortFileName
        {
            get { return Utils.GetShortFileName(_fileName); }
        }

        public SampledSignal Signal
        {
            get { return this.GetAudioFile().Signal; }
        }


        public Analysis.FrequencyAnalysis Analysis
        {
            get { return this.GetAudioFile().Analysis; }
        }

        public void Play()
        {
            this.GetAudioFile().Play();
        }


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
