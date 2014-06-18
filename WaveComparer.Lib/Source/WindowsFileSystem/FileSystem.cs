using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WaveComparer.Lib.WindowsFileSystem
{
    public class FileSystem
    {
        private ObservableCollection<WindowsFolder> _drives = new ObservableCollection<WindowsFolder>();

        public FileSystem()
        { 
            Environment.GetLogicalDrives().ToList().ForEach(
            drive => _drives.Add(WindowsFolder.GetWindowsFolder(drive))
            );
        }

        public ObservableCollection<WindowsFolder> Drives
        {
            get { return _drives; }
        }
    }
}
