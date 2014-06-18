using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using WaveComparer.Lib.XML_Serialisation;
using WaveComparer.Lib.Analysis;

namespace WaveComparer.Lib.XML_Serialisation
{
    public class XmlAudioFile : XmlFile
    {
        private XmlVector _analysisVector;
        private static string _libraryLocation = Properties.Settings.Default.LibraryLocation;

        [XmlIgnore]
        public bool isLoaded
        {
            get
            {
                return _analysisVector != null;
            }
        }

        public XmlVector AnalysisVector { get { return _analysisVector; } set { _analysisVector = value; } }

        /// <summary>
        /// Parameterless Contructor for XML serialiser
        /// </summary>
        protected XmlAudioFile() : base() { }

        public XmlAudioFile(string longFileName) : base(longFileName) 
        {
        }
    }
}
