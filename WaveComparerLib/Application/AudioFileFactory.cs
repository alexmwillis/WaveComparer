using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Threading;

using WaveComparerLib.XML_Serialisation;

namespace WaveComparerLib
{
    public class AudioFileFactory
    {   
        public static FileType GetFileType(string FileName)
        {
            // TODO probably reuse this functionality in WindowsFileSystem
            if (FileName.ToUpper().EndsWith(".WAV"))
            {
                return FileType.WaveFile;
            }
            else
            {
                return FileType.UnknownFile;
            }
        }

        public AudioFile GetAudioFile(string fileName)
        {
            AudioFile audioFile = null;
            
            if (fileName == "")
            {
                throw new ArgumentNullException("fileName");
            }

            switch (GetFileType(fileName))
            {
                case FileType.WaveFile:

                    audioFile = new WaveFile(fileName);
                    break;

                case FileType.UnknownFile:

                    throw new InvalidDataException("Unreadable Audio File");

            }

            return audioFile;
        }
    }
}
