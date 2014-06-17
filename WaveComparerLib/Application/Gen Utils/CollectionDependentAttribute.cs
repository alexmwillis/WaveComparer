using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Gen_Utils
{
    /// <summary>
    /// Indicates that the property is dependent on the items in the collection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CollectionDependentAttribute : Attribute
    {
    }
}
