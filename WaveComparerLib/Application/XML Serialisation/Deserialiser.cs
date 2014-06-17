using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace WaveComparerLib.XML_Serialisation
{
    public class Deserialiser
    {
        public T Deserialise<T>(string fileName, Type[] extraTypes)
        {
            if (File.Exists(fileName))
            {
                FileStream s = new FileStream(fileName, FileMode.Open);
                XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T), extraTypes);
                var t = (T)x.Deserialize(s);
                s.Close();
                return t;
            }
            else
                return default(T);            
        }
    }
}
