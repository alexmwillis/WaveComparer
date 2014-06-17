using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using WaveComparerLib.WindowsFileSystem;

namespace WaveComparerLib.XML_Serialisation
{
    public class XmlFolder : XmlFileSystemEntry
    {
        public List<XmlFolder> Folders { get; set; }
        public List<XmlFile> Files { get; set; }

        public XmlFolder()
        {
            this.Folders = new List<XmlFolder>();
            this.Files = new List<XmlFile>();
        }

        public XmlFolder(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            this.FullName = dirInfo.FullName;
            this.Name = dirInfo.Name;

            this.Folders = new List<XmlFolder>();
            this.Files = new List<XmlFile>();
        }

        public void AddFile(XmlFile file)
        {
            var nextFolder = WindowsFolder.GetNextFolder(this.FullName, file.FullName);
            if (nextFolder != null)
            {
                
                var folderFound = false;
                foreach (var folder in Folders)
                {
                    if (folder.Name == nextFolder)
                    {
                        folder.AddFile(file);
                        folderFound = true;
                    }
                }
                if (!folderFound)
                {
                    string path;
                    if (this.FullName != null)
                        path = Path.Combine(this.FullName, nextFolder);
                    else
                        path = nextFolder;
                    var folder = new XmlFolder(path);
                    folder.AddFile(file);
                    Folders.Add(folder);
                }
            }
            else
            {
                // Remove previous version of file
                this.Files.RemoveAll((xmlf) => 
                    {
                        return xmlf.FullName == file.FullName;
                    }
                );                
                this.Files.Add(file);
            }
        }

        public XmlFile GetFile(XmlFile file)
        {
            XmlFile foundFile = null;

            var nextFolder = WindowsFolder.GetNextFolder(this.FullName, file.FullName);
            if (nextFolder != null)
            {
                foreach (var folder in Folders)
                {
                    if (folder.Name == nextFolder)
                    {
                        foundFile = folder.GetFile(file);
                    }
                }
            }
            else
            {
                foundFile = this.Files.FirstOrDefault((f) => f.FullName == file.FullName);
            }
            return foundFile;
        }

        public static XmlFolder RootFolder
        {
            get
            {
                var rootFolder = new XmlFolder();
                Environment.GetLogicalDrives().ToList().ForEach(
                drive => rootFolder.Folders.Add(new XmlFolder() { FullName = drive, Name = drive.TrimEnd('\\') })
                );
                return rootFolder;
            }
        }

    }
}
