using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaveComparer.Lib.Interfaces;
using WaveComparer.Lib.Event_Args;
using WaveComparer.Lib.WindowsFileSystem;

namespace WaveComparer.Lib
{
    public class AudioFileLoader : ILoader<IAudioFile>
    {
        public AudioFileLoader()
        { }

        public event LoadedEventHandler<IAudioFile> Loaded;

        public void Load(string fileName)
        {
            try
            {
                var a = new LazyAudioFile(fileName);
                this.OnLoaded(a);
            }
            catch
            {
                // TODO message here
            }
        }

        void OnLoaded(IAudioFile audioFile)
        {
            if (Loaded != null)
                Loaded(this, new LoadedEventArgs<IAudioFile>(audioFile));
        }
    }
}
