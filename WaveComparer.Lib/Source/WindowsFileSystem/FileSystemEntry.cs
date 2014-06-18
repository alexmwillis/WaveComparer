using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveComparer.Lib.WindowsFileSystem
{
    public class FileSystemEntry
    {
        private FileSystemInfo _fileSystemInfo;

        public string FullName
        {
            get { return _fileSystemInfo.FullName; }
        }

        public string Name
        {
            get { return _fileSystemInfo.Name; }
        }

        public FileSystemEntry(FileSystemInfo fileSystemInfo)
        {
            _fileSystemInfo = fileSystemInfo;            
        }
    }
}