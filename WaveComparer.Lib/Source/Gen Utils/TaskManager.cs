using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WaveComparer.Lib 
{
    public delegate void WorkFinishedEventHandler(object sender, EventArgs e);
    //public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);

    public delegate bool SuccessfulAction();

    public class TaskManager //: INotifyPropertyChanged
    {
        const bool runParallel = false;

        int completedCount, unCompletedCount, progressPercent; 
        object progressCountLocker, unCompletedCountLocker;
        List<SuccessfulAction> taskList;

        public event ProgressChangedEventHandler ProgressChanged;
 
        public TaskManager()
        {
            progressCountLocker = new object();
            unCompletedCountLocker = new object();
            taskList = new List<SuccessfulAction>();
        }

        // Properties
        public int ProgressPercent
        {
            get { return progressPercent; }
            private set
            {
                progressPercent = value;
                OnProgressChanged(new ProgressChangedEventArgs(value, null));
            }
        }
        public int TaskCount { get { return taskList.Count; } }
        public int CompletedCount
        {
            get { return completedCount; }
            private set
            {
                completedCount = value;
                ProgressPercent = (CompletedCount + UnCompletedCount) * 100 / TaskCount;
                
            }
        }
        public int UnCompletedCount
        {
            get { return unCompletedCount; }
            private set
            {
                unCompletedCount = value;
                ProgressPercent = (CompletedCount + UnCompletedCount) * 100 / TaskCount;
            }
        }

        // Methods
        public void AddBatchTask(SuccessfulAction action)
        {
            taskList.Add(action);
        }

        public void RunTasks()
        {
            completedCount = 0;
            unCompletedCount = 0;
            if (runParallel)
            {
                this.RunTasksInParallel();
            }
            else
            {
                this.RunTasksConcurrently();
            }
        }

        public void RunTasksConcurrently()
        {
            foreach (var task in taskList)
            {
                if (task())
                { 
                    CompletedCount += 1; 
                }
                else
                { 
                    UnCompletedCount += 1; 
                }
            }
            taskList.Clear();
        }

        public void RunTasksInParallel()
        {
            CountdownEvent cde = new CountdownEvent(TaskCount);
            Parallel.ForEach<SuccessfulAction>(taskList, task =>
            {
                try
                {
                    if (task()) { lock (progressCountLocker) { CompletedCount += 1; } }
                    else { lock (unCompletedCountLocker) { UnCompletedCount += 1; } }
                }
                finally
                {
                    cde.Signal();
                }
            });
            cde.Wait();
            taskList.Clear();
        }

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }
    }
}
