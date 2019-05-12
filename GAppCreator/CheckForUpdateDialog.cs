using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace GAppCreator
{
    public partial class CheckForUpdateDialog : BackgroundWorkerForm
    {
        public enum CheckForUpdateState
        {
            Error,
            NoUpdatesAvailable,
            Update,
        };
        public CheckForUpdateState State = CheckForUpdateState.Error;
        public CheckForUpdateDialog(Project p): base(p)
        {
            this.Text = "Check for new updates ...";
            InitializeComponent();
        }

        protected override void OnStartBackgroundWork()
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            // request pentru versiune            
            UpdateInfo(BuildTask.AddOperation, "Querying server for new updates ...");
            if (client.Query("querylatestversion", p) == false)
                return;
            string info = client.GetStringResult();
            info = info.Replace("\r", "\n").Replace("\n\n","\n");
            if (info.Contains("\n") == false)
            {
                prj.EC.AddError("version file from the server is incomplete (does not contains version informations)");
                return;
            }
            string newVersion = info.Substring(0, info.IndexOf("\n")).Trim();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            if (newVersion == fvi.FileVersion.Trim())
            {
                UpdateInfo(BuildTask.AddOperation, "You are up to date !");
                UpdateInfo(BuildTask.OperationComplete, "");
                State = CheckForUpdateState.NoUpdatesAvailable;
                return;
            }
            // altfel - avem o versiune noua - o downladam
            string app_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            UpdateInfo(BuildTask.AddOperation, "Uploading version: " + newVersion);
            if (client.Query("uploadlastversion",  p) == false)
                return;
            // totul e ok - am uploadad versionea nou
            UpdateInfo(BuildTask.OperationComplete, "");
            if (MessageBox.Show("Update to latest version ?\nVersion: " + info, "Update", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;     
            if (Disk.SaveFile(Path.Combine(app_path,"Update", "update.dat"), client.GetResult(), prj.EC) == false)
                return;
            if (Disk.Copy(Path.Combine(app_path, "updater.exe"), Path.Combine(app_path, "Update", "updater.exe"), prj.EC) == false)
                return;
            if (Disk.Copy(Path.Combine(app_path, "GacLibrary.dll"), Path.Combine(app_path, "Update", "GacLibrary.dll"), prj.EC) == false)
                return; 
            State = CheckForUpdateState.Update;
        }

        protected override void OnNewAction(BuildTask actionType, string value)
        {
            base.OnNewAction(actionType, value);
        }

        protected override void OnBackgroundWorkCompleted()
        {
            if ((prj.EC.HasErrors()) || (State == CheckForUpdateState.Update))
                this.Close();
        }
    }
}
