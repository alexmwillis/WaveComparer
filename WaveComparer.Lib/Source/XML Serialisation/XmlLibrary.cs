using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparer.Lib.XML_Serialisation
{
    public class XmlLibrary<T> where T : XmlFile
    {
        static private XmlLibrary<T> _library;

        private string _libraryLocation;
        
        public XmlFolder Root { get; set; }

        private XmlLibrary()
        {
            this.Root = XmlFolder.RootFolder;
        }

        public static XmlLibrary<T> GetXMLAudiofileLibrary(string libraryLocation)
        {
            if (_library == null)
            {
                Type[] extraTypes = { typeof(T) };
                var d = new Deserialiser();
                _library = d.Deserialise<XmlLibrary<T>>(libraryLocation, extraTypes);
                if (_library == null)
                {
                    _library = new XmlLibrary<T>();
                }
                _library._libraryLocation = libraryLocation;
            }
            return _library;
        }        
        
        ~XmlLibrary()
        {
            this.Save();
        }

        public void Save()
        {
            Type[] extraTypes = { typeof(T) };
            var s = new Serialiser();
            s.Serialise(this, _libraryLocation, extraTypes);
        }

        public void AddFile(XmlFile file)
        {
            // TODO make this thread safe
            this.Root.AddFile(file);            
        }

        public T GetFile(XmlFile file)
        {
            // TODO make this thread safe
            return (T)this.Root.GetFile(file);
        }
    }
}
