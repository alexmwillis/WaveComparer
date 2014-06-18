using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace WaveComparer.Lib.XML_Serialisation
{
    public class Serialiser
    {
        public void Serialise(object a, string fileName, Type[] extraTypes)
        {
            XmlSerializer x;
            // TODO workout why this sometimes doesn't work
            try
            {
                x = new XmlSerializer(a.GetType(), extraTypes);
            }
            catch (Exception e)
            {
                throw e;
            }
            TextWriter t = new StreamWriter(fileName);            
            x.Serialize(t, a);
            t.Close();
        }
    }
}
