using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GAppCreator
{
    public class BackgroundWorkerForm: Form
    {
        protected enum BuildTask
        {
            AddOperation,
            UpdateProgress,
            OperationComplete,
            BuildOk
        };
        private class ResultInfo
        {
            public BuildTask Action;
            public string Value;
        };
        private ResultInfo tempri = new ResultInfo();
        private bool sync;
        private System.ComponentModel.BackgroundWorker Worker;
        protected Project prj;
        private ListView lstOperations;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ImageList lstIcons;
        private System.ComponentModel.IContainer components;
        protected HttpBuildServiceClient client;
        protected Button btnExit;

        public BackgroundWorkerForm(Project p)
        {
            prj = p;
            client = new HttpBuildServiceClient(prj);
            Worker = new System.ComponentModel.BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(Worker_DoWork);
            Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(Worker_ProgressChanged);
            Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            this.Shown += new EventHandler(BackgroundWorkerForm_Shown);
            InitializeComponent();
            this.ControlBox = false;
            
            btnExit = AddNewButton(0, "Close");
            btnExit.Click += new EventHandler(btnExit_Click);
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            
        }

        void btnExit_Click(object sender, EventArgs e)
        {
            OnCloseForm();
        }

        protected Button AddNewButton(int index, string text)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new System.Drawing.Point(this.Width-(15+80)-index*90, this.Height-60);            
            b.Size = new System.Drawing.Size(80, 30);
            b.UseVisualStyleBackColor = true;
            this.Controls.Add(b);
            return b;
        }

        void BackgroundWorkerForm_Shown(object sender, EventArgs e)
        {
            Worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            OnBackgroundWorkCompleted();
        }

        private void Worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.UserState!=null)
                OnNewAction(((ResultInfo)e.UserState).Action,((ResultInfo)e.UserState).Value);
            sync = false;
        }

        private void Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            OnStartBackgroundWork();            
        }
        protected void UpdateInfo(BuildTask action, string Value)
        {
            //Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + ":UpdateInfo(" + action.ToString() + "," + Value + ")");
            tempri.Action = action;
            tempri.Value = Value;
            sync = true;
            Worker.ReportProgress(0, tempri);
            while (sync) { };
        }

        protected virtual void OnStartBackgroundWork()
        {
        }
        protected virtual void OnBackgroundWorkCompleted()
        {
        }
        protected virtual void OnNewAction(BuildTask actionType, string value)
        {
            switch (actionType)
            {
                case BuildTask.AddOperation:
                    AddNewExecutingTask(value, true);
                    break;
                case BuildTask.OperationComplete:
                    FinalizeLastExecutingTask(true);
                    break;
                case BuildTask.UpdateProgress:
                    UpdateProgressForLastExecutingTask(value);
                    break;
            }
        }
        protected virtual void OnCloseForm()
        {            
            this.Close();
        }
        protected void FinalizeLastExecutingTask(bool ok)
        {
            if (lstOperations.Items.Count == 0)
                return;
            ListViewItem lvi = lstOperations.Items[lstOperations.Items.Count - 1];
            if (lvi.ImageKey == "run")
            {
                if (ok)
                {
                    lvi.ImageKey = "ok";
                    lvi.SubItems[2].Text = "100%";
                }
                else
                {
                    lvi.ImageKey = "error";
                }
            }
        }
        protected void AddNewExecutingTask(string name,bool finalizeThePreviousOne)
        {
            if (finalizeThePreviousOne)
                FinalizeLastExecutingTask(true);
            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString());
            lvi.SubItems.Add(""); lvi.SubItems.Add("0%");
            lvi.ImageKey = "run";
            lvi.SubItems[1].Text = name;
            lstOperations.Items.Add(lvi);
            lvi.EnsureVisible();
        }
        protected void UpdateProgressForLastExecutingTask(string value)
        {
            if ((lstOperations.Items.Count > 0) && (lstOperations.Items[lstOperations.Items.Count - 1].ImageKey == "run"))
                lstOperations.Items[lstOperations.Items.Count - 1].SubItems[2].Text = value;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackgroundWorkerForm));
            this.lstOperations = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstIcons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // lstOperations
            // 
            this.lstOperations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstOperations.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstOperations.FullRowSelect = true;
            this.lstOperations.GridLines = true;
            this.lstOperations.Location = new System.Drawing.Point(0, 0);
            this.lstOperations.Name = "lstOperations";
            this.lstOperations.Size = new System.Drawing.Size(694, 331);
            this.lstOperations.SmallImageList = this.lstIcons;
            this.lstOperations.TabIndex = 1;
            this.lstOperations.UseCompatibleStateImageBehavior = false;
            this.lstOperations.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Date";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Operation";
            this.columnHeader2.Width = 445;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lstIcons
            // 
            this.lstIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("lstIcons.ImageStream")));
            this.lstIcons.TransparentColor = System.Drawing.Color.Magenta;
            this.lstIcons.Images.SetKeyName(0, "run");
            this.lstIcons.Images.SetKeyName(1, "ok");
            this.lstIcons.Images.SetKeyName(2, "error");
            // 
            // BackgroundWorkerForm
            // 
            this.ClientSize = new System.Drawing.Size(694, 376);
            this.Controls.Add(this.lstOperations);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BackgroundWorkerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }
    }
}
