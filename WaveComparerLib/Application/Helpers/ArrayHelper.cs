using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Helpers
{
    public static class ArrayHelper
    {
        public static T[] ToSingleDimension<T>(T[][] jaggedArray)
        {
            var x = jaggedArray.Length;
            var y = jaggedArray[0].Length;

            var d1Array = new T[x * y];
            for (int i = 0; i < x; i++)
            {
                Array.Copy(jaggedArray[i], 0, d1Array, y * i, y);
            }
            return d1Array;
        }

        public static T[] ToSingleDimension<T>(T[,] matrix)
        {
            var x = matrix.GetLength(0);
            var y = matrix.GetLength(1);

            var d1Array = new T[x * y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    d1Array[y * i + j] = matrix[i, j];    
                }
            }
            return d1Array;
        }

        internal static T[] PadArray<T>(T[] array, T value, int toLength)
        {
            var paddedArray = new T[toLength];
            if (array.Length >= toLength)
            {
                Array.Copy(array, paddedArray, paddedArray.Length);
                return paddedArray;
            }
            else
            {
                Array.Copy(array, paddedArray, array.Length);
                for (int i = array.Length; i < paddedArray.Length; i++)
                {
                    paddedArray[i] = value;
                }
                return paddedArray;
            }
        }

        // *** remove this its rubbish
        internal static double[][] Resize(double[][] jaggedArray, double xFactor, double yFactor)
        {
            int x = (int)(xFactor * jaggedArray.Length);
            int y = (int)(yFactor * jaggedArray[0].Length);
            var resizedArray = new double[x][];
            for (int i = 0; i < resizedArray.Length; i++)
            {
                resizedArray[i] = new double[y];
                for (int j = 0; j < resizedArray[i].Length; j++)
                {
                    int I = (int)(i / xFactor);
                    int J = (int)(j / yFactor);

                    resizedArray[i][j] = jaggedArray[I][J];
                }
            }
            return resizedArray;
        }
    }
}
