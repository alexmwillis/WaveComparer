using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;

using WaveComparerLib.Interfaces;
using WaveComparerLib.Event_Args;

namespace WaveComparerLib
{
    public class AudioFilesLoader : ILoader<List<IAudioFile>>
    {
        private BackgroundWorker _loadAsyncWorker = new BackgroundWorker();
        private TaskManager _taskManager;
        private bool _searchSubdirectories = true; //TODO Properties.Settings.Default.SearchSubdirectories;
        // This list is intentionally not observable so it can be run in a seperate thread
        private List<IAudioFile> _loadedAudioFiles = new List<IAudioFile>();

        public AudioFilesLoader()
        {
            _loadAsyncWorker.DoWork += (o, e) => this.Load((string)e.Argument, true);
            _loadAsyncWorker.RunWorkerCompleted += this.OnLoaded;

            _taskManager = new TaskManager();
            _taskManager.ProgressChanged += ((o, e) =>
            {
                if (LoadProgressChanged != null)
                    LoadProgressChanged(this, e);
            });

            // Save library whenever files are reloaded
            this.Loaded += (o, e) => WaveComparerToolBox.Instance.XmlAudioFileLibrary.Save();
        }

        #region Public Interface

        public List<IAudioFile> LoadedAudioFiles { get { return _loadedAudioFiles; } }

        public event EventHandler<EventArgs> LoadProgressChanged;

        public bool SearchSubdirectories
        {
            get { return _searchSubdirectories; }
            set { _searchSubdirectories = value; }
        }

        public int LoadProgress { get { return _taskManager.ProgressPercent; } }

        public void LoadAsync(string directoryPath)
        {
            _loadAsyncWorker.RunWorkerAsync(directoryPath);
        }

        public void Load(string directoryPath, bool isAsync)
        {
            DirectoryInfo di = new DirectoryInfo(directoryPath);

            var option = new SearchOption();
            if (SearchSubdirectories) { option = SearchOption.AllDirectories; }
            else { option = SearchOption.TopDirectoryOnly; }

            FileInfo[] fiArray = di.GetFiles("*", option);

            // Load audio files in selected directory
            foreach (FileInfo fi in fiArray)
            {
                var name = fi.FullName; // need a local var for each name
                _taskManager.AddBatchTask(() =>
                {
                    // Action that returns true if successful
                    IAudioFile audioFile = null;
                    try
                    {
                        audioFile = new LazyAudioFile(name);
                        //if (PitchShiftAudioFiles && MainAudioFile != null)
                        //{
                        //    audioFile.PitchShiftTo(MainAudioFile);
                        //}
                        _loadedAudioFiles.Add(audioFile);
                        return true;
                    }
                    catch (InvalidDataException)
                    {
                        return false;
                    }
                });
            }
            _taskManager.RunTasks();

            if (!isAsync)
                this.OnLoaded(this, null);
        }

        public void OnLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.Loaded != null)
                this.Loaded(this, new LoadedEventArgs<List<IAudioFile>>(_loadedAudioFiles));
        }

        #endregion //Public Interface

        #region Private members

        //void LoadThread(string directoryPath, ThreadFinishedDel threadFinishedDel)
        //{
        //    Thread thread = new Thread((o) =>
        //    {
        //        Load((string)o);
        //        threadFinishedDel();
        //    });
        //    thread.Start(directoryPath);
        //}

        #endregion // Private members

        public event LoadedEventHandler<List<IAudioFile>> Loaded;
    }
}
