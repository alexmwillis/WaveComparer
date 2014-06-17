using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using FurtherMath.Base;

namespace WaveComparerLib.XML_Serialisation
{
    public class XmlVector : List<double>
    {
        /// <summary>
        /// Parameterless constructor for XML deserialiser
        /// </summary>
        private XmlVector()
        {
        }

        public XmlVector(double[] array)
        {
            foreach (var value in array)
            {
                this.Add(value);
            }
        }

        public static explicit operator Vector(XmlVector v)
        {
            return new Vector(v.ToArray());
        }

        public static explicit operator XmlVector(Vector v)
        {
            return new XmlVector((double[])v);
        }
    }
}
