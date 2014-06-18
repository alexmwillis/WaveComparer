using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WaveComparer.Lib.XML_Serialisation
{
    public class XmlFileSystemEntry
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string FullName { get; set; }
    }
}
