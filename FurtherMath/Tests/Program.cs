using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FurtherMathTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Tests();
            tests.Setup();
            tests.TestVectorPair();
            tests.TestTrainingSetMeasure();            
        }        
    }
}
