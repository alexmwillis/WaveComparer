using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using FurtherMath.Base;
using FurtherMath.Measures;

namespace FurtherMathTests
{
    public class VectorConvertableObject : IVectorConvertable, IEquatable<VectorConvertableObject>
    {
        public Vector vector;

        public Vector ToVector()
        {
            return vector; 
        }

        public bool Equals(VectorConvertableObject other)
        {
            return this.Equals((object)other);
        }
    }

    public class Tests
    {
        Vector v1;
        Vector v2;
        Vector v3;

        VectorPair pair1v1;
        VectorPair pair1v2;
        VectorPair pair2;

        VectorConvertableObject vcObj1;
            VectorConvertableObject vcObj2;
            VectorConvertableObject vcObj3;

        public void Setup()
        {
            v1 = new Vector(new double[] { 0, 1, 2 });
            v2 = new Vector(new double[] { 1, 2, 3 });
            v3 = new Vector(new double[] { 2, 4, 6 });

            pair1v1 = new VectorPair(v1, v2);
            pair1v2 = new VectorPair(v2, v1);
            pair2 = new VectorPair(v2, v3);

            vcObj1 = new VectorConvertableObject() { vector = v1};
            vcObj2 = new VectorConvertableObject() { vector = v2};
            vcObj3 = new VectorConvertableObject() { vector = v3};
        }

        public void TestVectorPair()
        {
            Debug.Assert(pair1v1 == pair1v1);
            Debug.Assert(pair1v1 == pair1v2);
            Debug.Assert(pair1v1 != pair2);
            Debug.Assert(pair1v1 != null);
        }

        public void TestTrainingSetMeasure()
        {
            var measure = new TrainingSetMeasure<VectorConvertableObject>();

            // Test euclidean distance
            Debug.Assert(measure.Distance(vcObj1, vcObj2) == ~(v1 - v2));
            Debug.Assert(measure.Distance(vcObj2, vcObj1) == ~(v1 - v2));

            measure.SetDistance(vcObj1, vcObj2, 5);

            // Test specified distance
            Debug.Assert(measure.Distance(vcObj1, vcObj2) == 5);
            Debug.Assert(measure.Distance(vcObj2, vcObj1) == 5);
        }
    }
}
