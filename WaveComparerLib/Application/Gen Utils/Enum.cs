using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Gen_Utils
{
    /// <summary>
    /// Enum extension for less ugly parse code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Enum<T>
    {
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }    
}
