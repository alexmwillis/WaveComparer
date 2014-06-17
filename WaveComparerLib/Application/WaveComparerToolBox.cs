using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparerLib.Gen_Utils;
using WaveComparerLib.XML_Serialisation;
using WaveComparerLib.WindowsFileSystem;

namespace WaveComparerLib
{
    public class WaveComparerToolBox
    {
        private static WaveComparerToolBox _instance;

        public static WaveComparerToolBox Instance
        {
            get { return _instance; }
        }

        public static void Initialise()
        {
            if (_instance == null)
                _instance = new WaveComparerToolBox();
            else
                throw new InvalidOperationException("ToolBox already initialised");
        }

        public List<Action<object>> AvailableActions { get; private set; }
        public XmlLibrary<XmlAudioFile> XmlAudioFileLibrary { get; private set; }

        protected WaveComparerToolBox()
        {
            this.InternalInitialise();
        }

        protected void InternalInitialise()
        {
            this.XmlAudioFileLibrary = XmlLibrary<XmlAudioFile>.GetXMLAudiofileLibrary(
                Properties.Settings.Default.LibraryLocation);
            this.AvailableActions = new List<Action<object>>();            
        }

        public void AddAvailableAction(Action<object> action)
        {            
            this.AvailableActions.Add(action);
        }
    }
}

