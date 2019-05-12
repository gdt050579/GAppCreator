using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace GAppCreator
{
    public delegate void AsynchroneActionEventHandler();
    public delegate void StartBackgroundTaskEventHandler(string name);
    public enum BackgroundTaskType
    {
        SetMinMax,
        CreateSubTask,
        Info,
        UpdateProgress,
        Command,
        UpdateTaskState,
    };
    public enum TaskEventState
    {
        Running,
        Error,
        Success,
        Info
    };
    public class BackgroundTaskAction
    {
        public int Min, Max, Value;
        public BackgroundTaskType Type;
        public string Content;
        public TaskEventState Tes;
        public Command Command;
        public Object CommandParam;
    };
    public class BackgroundTask: BackgroundWorker,ITaskNotifier
    {
        private AsynchroneActionEventHandler AsynchronActionFunction = null;
        private StartBackgroundTaskEventHandler StartBackgroundTaskFunction = null;
        private BackgroundTaskAction act = new BackgroundTaskAction();
        //private EventWaitHandle sync = new EventWaitHandle(false, EventResetMode.ManualReset);
        private bool sync;
      
        public static void PerformTask(object sender, DoWorkEventArgs e)
        {
            if ((e != null) && (e.Argument != null))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = Project.defaultLocale;
                ((BackgroundTask)e.Argument).AsynchronActionFunction();
            }
        }
        private void Send()
        {
            //sync.Set();
            //optimizarile trebuie sa fie oprite - altfel secvebta asta de code e convertita doar in ReportProgress si nu mai merge sincul
            sync = false;
            ReportProgress(0, act);
            while (!sync) { }
            //sync.WaitOne();
        }
        public void Done()
        {
            sync = true;
        }
        public BackgroundTask(ProgressChangedEventHandler OnChangeProgress, RunWorkerCompletedEventHandler OnComplete, StartBackgroundTaskEventHandler OnStart)
        {
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
            this.DoWork += PerformTask;
            this.ProgressChanged += OnChangeProgress;
            this.RunWorkerCompleted += OnComplete;
            StartBackgroundTaskFunction = OnStart;
        }
        public void Start(AsynchroneActionEventHandler functionToRun, string name)
        {
            if (IsBusy == false)
            {
                StartBackgroundTaskFunction(name);
                AsynchronActionFunction = functionToRun;
                act.Min = act.Max = act.Value = 0;
                act.Content = "";
                RunWorkerAsync(this);
            }
        }
        public void SetMinMax(int min, int max)
        {
            if (min < max)
            {
                act.Min = min;
                act.Max = max;
                act.Value = 0;
                act.Type = BackgroundTaskType.SetMinMax;
                Send();
            }
        }
        public void CreateSubTask(string text)
        {
            act.Tes = TaskEventState.Running;
            act.Content = text;
            act.Type = BackgroundTaskType.CreateSubTask;
            Send();
        }
        public void Info(string text)
        {
            act.Tes = TaskEventState.Info;
            act.Content = text;
            act.Type = BackgroundTaskType.Info; ;
            Send();
        }

        public void UpdateProgress(int value)
        {
            if ((value >= act.Min) && (value < act.Max))
            {
                act.Value = value;
                act.Type = BackgroundTaskType.UpdateProgress;
                Send();
            }
        }
        public void UpdateTaskEventState(TaskEventState tes)
        {
            act.Type = BackgroundTaskType.UpdateTaskState;
            act.Tes = tes;
            Send();
        }
        public bool UpdateSuccessErrorState(bool isSuccesiful)
        {
            if (isSuccesiful)
                UpdateTaskEventState(TaskEventState.Success);
            else
                UpdateTaskEventState(TaskEventState.Error);
            return isSuccesiful;
        }
        public void IncrementProgress()
        {
            UpdateProgress(act.Value + 1);
        }

        public void SendCommand(Command cmd)
        {
            act.Type = BackgroundTaskType.Command;
            act.CommandParam = null;
            act.Command = cmd;
            Send();
        }
        public void SendCommand(Command cmd, Object param)
        {
            act.Type = BackgroundTaskType.Command;
            act.CommandParam = param;
            act.Command = cmd;
            Send();
        }
    }
}
