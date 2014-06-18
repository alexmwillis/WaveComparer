using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace WaveComparer.Lib.WindowsFileSystem
{
    public class WindowsFolder : FileSystemEntry
    {
        DirectoryInfo _directoryInfo;
        WindowsFileFactory _fileFactory = new WindowsFileFactory();

        private ObservableCollection<FileSystemEntry> _fileSystemEntries;

        public ObservableCollection<FileSystemEntry> FileSystemEntries
        {
            get
            {
                if (_fileSystemEntries == null)
                {
                    _fileSystemEntries = new ObservableCollection<FileSystemEntry>();
                    foreach (var dir in Directory.GetDirectories(this.FullName))
                    {
                        _fileSystemEntries.Add(new WindowsFolder(new DirectoryInfo(dir)));
                    }
                    // TODO make filter configurable!
                    foreach (var file in Directory.GetFiles(this.FullName, "*.WAV"))
                    {
                        _fileSystemEntries.Add(_fileFactory.GetWindowsFile(new FileInfo(file)));
                    }
                }
                return _fileSystemEntries;
            }
        }

        public WindowsFolder(DirectoryInfo directoryInfo)
            : base(directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public static WindowsFolder GetWindowsFolder(string path)
        {
            return new WindowsFolder(new DirectoryInfo(path));
        }

        public bool HasEntries
        {
            get
            {
                try
                {
                    return Directory.GetFileSystemEntries(this.FullName).Count() > 0;
                }
                catch (IOException)
                {
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
            }
        }

        public static string GetNextFolder(string dir, string subPath)
        {
            if (dir == null)
                // If directory not passed in, then return first folder of sub-path
                return Path.GetDirectoryName(subPath).Split('\\')[0];

            List<string> dirFolders;
            if (Directory.Exists(dir))
            {
                if (Directory.GetDirectoryRoot(dir) == dir)
                    dirFolders = new List<string>() { dir.Trim('\\') };
                else
                    dirFolders = dir.Split('\\').ToList();
            }
            else
                throw new ArgumentException("Invalid directory", "dir");
            var subPathFolders = Path.GetDirectoryName(subPath).Split('\\').ToList();
            // Validate sub-path
            if (subPathFolders.Count() < dirFolders.Count())
                throw new ArgumentException("Invalid sub-path", "subPath");

            for (int i = 0; i < dirFolders.Count - 1; i++)
            {
                if (dirFolders[i] != subPathFolders[i])
                    throw new ArgumentException("Invalid sub-path", "subPath");
            }
            if (subPathFolders.Count() == dirFolders.Count())
                return null; // sub-path is path
            else
                return subPathFolders[dirFolders.Count()];
        }
    }
}

