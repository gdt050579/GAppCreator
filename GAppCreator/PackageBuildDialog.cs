using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace GAppCreator
{
    public partial class PackageBuildDialog : BackgroundWorkerForm
    {
        private string BuildName;
        private string resultOutputFolder = "";
        private bool QuickBuild;
        private Button btnOpenFolder;
        private Button btnDeploy;

        public bool GoToDeployDialog = false;

        public PackageBuildDialog(Project proj,string buildName,bool quick): base(proj)
        {
            this.Text = "Build '" + buildName + "' package";
            BuildName = buildName;
            QuickBuild = quick;
            InitializeComponent();
            btnOpenFolder = AddNewButton(1, "Open folder");
            btnOpenFolder.Enabled = false;
            btnOpenFolder.Click += new EventHandler(OnOpenFolder);
            btnExit.Text = "Stop build";
            btnDeploy = AddNewButton(2, "Deploy");
            btnDeploy.Enabled = false;
            btnDeploy.Click += new EventHandler(OnDeployBuild);
        }

        void OnDeployBuild(object sender, EventArgs e)
        {
            GoToDeployDialog = true;
            btnDeploy.Enabled = false;
            OnCloseForm();
        }

        protected override void OnCloseForm()
        {
            if (btnOpenFolder.Enabled == false)
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            else
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        protected override void OnStartBackgroundWork()
        {            
            UpdateInfo(BuildTask.AddOperation,"Connecting to server '"+prj.Settings.BuildServerAddress+"'");
            Dictionary<string, string> p = new Dictionary<string, string>();
            p["project"] = prj.GetProjectName();
            if (QuickBuild)
                p["quick"] = "1";
            else
                p["quick"] = "0";
            p["buildname"] = BuildName;
            
            if (client.Query("queryprojectstatus", p) == false)
                return;
            GZipPackage zp = prj.CreateProjectPackage(QuickBuild,BuildName,true);
            if (client.GetStringResult() == "[STATUS_CODE_3]")
            {
                UpdateInfo(BuildTask.AddOperation, "Computing diffs ...");
                if (client.Query("queryprojecthashes", p) == false)
                    return;
                GZipPackage zp2 = new GZipPackage();
                if (zp2.Uncompress(prj.Settings.Password, client.GetResult(), Path.Combine(prj.ProjectPath, "temp"), prj.EC) == false)
                    return;
                if (zp.FilterFileList(Path.Combine(prj.ProjectPath, "temp", prj.Settings.UserName + "_" + prj.GetProjectName() + ".hashlist"), prj.EC) == false)
                    return;
                if (zp.Compress(Path.Combine(prj.ProjectPath, "temp", "tosend.packed"), prj.Settings.Password, prj.EC) == false)
                    return;
            }
            else
            {
                if (zp.Compress(Path.Combine(prj.ProjectPath, "temp", "tosend.packed"), prj.Settings.Password, prj.EC) == false)
                    return;
            }
            UpdateInfo(BuildTask.AddOperation, "Sending package to build ("+zp.GetFilesCount().ToString()+" files)");
            if (client.Upload("uploaddiffandbuild",Path.Combine(prj.ProjectPath, "temp", "tosend.packed"), p) == false)
                return;
            UpdateInfo(BuildTask.OperationComplete, "");
            Dictionary<string, byte> added = new Dictionary<string, byte>();
            bool done = false;
            bool allOK = false;
            string lastTask = "";
            while ((!done) && (!allOK))
            {
                System.Threading.Thread.Sleep(1000);
                if (client.Query("getbuildstatus", p ) == false)
                    return;
                string s = client.GetStringResult();
                string[] w = s.Split('\n');
                string proc = "";
                if (w != null)
                {
                    bool isLastTask = false;
                    for(int tr=0;tr<w.Length;tr++)
                    {
                        string line = w[tr].Trim();
                        if (line.StartsWith("P:") == false)
                        {
                            if ((isLastTask) && (proc.Length > 0))
                                UpdateInfo(BuildTask.UpdateProgress, proc);
                            proc = "";
                        }
                        if (line.StartsWith("T:"))
                        {
                            line = line.Substring(2);
                            //Console.WriteLine("Task:" + line + " - " + added.ContainsKey(line).ToString() + " - last:" + lastTask);
                            if (added.ContainsKey(line)==false)
                            {
                                if (added.ContainsKey(lastTask))
                                    added[lastTask] = 2;
                                UpdateInfo(BuildTask.AddOperation, line);
                                lastTask = line;
                                isLastTask = true;
                                added[line] = 1;
                            }                            
                            isLastTask = (line == lastTask);
                            continue;
                        }
                        if (line.StartsWith("P:"))
                        {
                            proc = line.Substring(2) + "%";
                            continue;
                        }
                        if (line.StartsWith("E:"))
                        {
                            //  iau totul pana la capat si consider eroare
                            string ss = "";
                            for (int gr = tr; gr < w.Length; gr++)
                            {
                                string ln = w[gr];
                                if ((ln.StartsWith("P:")) || (ln.StartsWith("O:")) || (ln.StartsWith("X:")) || (ln.StartsWith("T:")))
                                    ln = ln.Substring(2);
                                if (ln.Trim().Length == 0)
                                    continue;
                                ss += ln + "\n";
                            }
                            prj.EC.AddError(ss.Replace("\\n","\r\n"));
                            return;
                        }
                        if (line.StartsWith("O:"))
                            allOK = true;
                        if (line.StartsWith("X:"))
                            done = true;
                    }
                }

            }
            if (allOK)
            {
                string path = Path.Combine(prj.ProjectPath, "bin", BuildName);
                if (Disk.CreateFolder(path,prj.EC)==false)
                    return;
                GenericBuildConfiguration gbc = null;
                for (int tr = 1; tr < prj.BuildConfigurations.Count; tr++)
                    if (prj.BuildConfigurations[tr].Name == BuildName)
                        gbc = prj.BuildConfigurations[tr];
                if (gbc == null)
                {
                    prj.EC.AddError("Unknwon build configuration: " + BuildName);
                    return;
                }
                DateTime dt = DateTime.Now;                
                path = Path.Combine(path, string.Format("{0}-{1:D2}-{2:D2}---{3:D2}-{4:D2}-{5:D2}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second)+"___"+gbc.GetOutputApplicationFileName(prj));
                if (client.Query("getbinresult", p) == false)
                    return;
                if (Disk.SaveFile(path, client.GetResult(), prj.EC) == false)
                    return;
                // daca am ajuns pana aici - totul e ok
                UpdateInfo(BuildTask.AddOperation, "Build complete => " + Path.GetFileName(path));
                UpdateInfo(BuildTask.OperationComplete, "");
                UpdateInfo(BuildTask.BuildOk, "");
                resultOutputFolder = Path.Combine(prj.ProjectPath, "bin", BuildName);
            }            
        }

        protected override void OnNewAction(BuildTask actionType, string value)
        {
            //Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + ":OnNewAction(" + ri.Action.ToString()+","+ri.Value + ")");
            switch (actionType)
            {
                case BuildTask.BuildOk:
                    btnExit.Text = "Close";
                    btnOpenFolder.Enabled = true;
                    btnDeploy.Enabled = true;
                    break;
                default:
                    base.OnNewAction(actionType, value);
                    break;
            }

        }

        protected override void OnBackgroundWorkCompleted()
        {
            if (btnOpenFolder.Enabled == false)
                this.Close();
        }

        private void OnOpenFolder(object sender, EventArgs e)
        {
            Process.Start(resultOutputFolder);
        }

    }
}
