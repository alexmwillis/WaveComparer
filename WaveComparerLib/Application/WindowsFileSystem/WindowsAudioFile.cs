using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Media;

namespace WaveComparerLib.WindowsFileSystem
{
    public class WindowsAudioFile : WindowsFile
    {
        public WindowsAudioFile(FileInfo fileInfo) : base(fileInfo) { }

        public void Play()
        {
            var player = new SoundPlayer(base.FullName);
            try
            {
                player.Play();
            }
            catch
            {
                // TODO handle this
            }
        }
    }
}
