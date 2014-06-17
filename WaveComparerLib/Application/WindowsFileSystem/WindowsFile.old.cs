using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparerLib.Gen_Utils;

namespace WaveComparerLib.Framework
{
    public class WindowsFileOld
    {
        protected string _longFileName;
        protected string _shortFileName;
        protected string[] _fileLocation;

        public string LongFileName { get { return _longFileName; } }
        public string ShortFileName { get { return _shortFileName; } }
        public string[] FileLocation { get { return _fileLocation; } }

        public WindowsFileOld(string longFileName)
        {
            var split = longFileName.Split(new Char [] {'\\'});
            _longFileName = longFileName;
            _shortFileName = split[split.Length - 1];
            _fileLocation = new string[split.Length - 1];
            Array.Copy(split, _fileLocation, split.Length - 1);
        }
    }
}
