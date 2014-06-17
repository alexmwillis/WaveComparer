using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveComparerLib.WindowsFileSystem
{
    public abstract class WindowsFile : FileSystemEntry
    {
        private FileInfo _fileInfo;

        public List<Action<object>> Actions { get; set; }

        public string Extension
        {
            get { return _fileInfo.Extension; }
        }

        public WindowsFile(FileInfo fileInfo)
            : base(fileInfo)
        {
            this.Actions = new List<Action<object>>();

            _fileInfo = fileInfo;
        }  
    }
}
