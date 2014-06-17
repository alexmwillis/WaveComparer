using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace WaveComparerLib.XML_Serialisation
{
    public class XmlFile : XmlFileSystemEntry
    {
        [XmlAttribute]
        public string Extension { get; set; }

        public XmlFile() { }

        public XmlFile(string path)
        {
            this.Name = Path.GetFileNameWithoutExtension(path);
            this.FullName = Path.GetFullPath(path);
            this.Extension = Path.GetExtension(path);
        }        
    }
}
