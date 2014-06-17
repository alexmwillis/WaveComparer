using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveComparerLib.Gen_Utils
{
    public static class Utils
    {
        public static string GetShortFileName(string longFileName)
        {
            string [] split = longFileName.Split(new Char [] {'\\'});
            return split[split.Length - 1];
        }

        public static bool ValidDirectory(string directory)
        {
            return Directory.Exists(directory);
        }
    }
}
