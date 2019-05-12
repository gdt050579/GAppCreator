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
using System.Threading;

namespace GAppCreator
{
    public partial class AndroidDeployDialog : Form
    {
        private class DeviceInfo
        {
            public string Id;
            public string Name;
        };
        private class AndroidLogSettings
        {
            public bool ClearLogCat;
            public bool ClearUserData;
            public bool PerformInstall;
            public bool RunApplication;
            public bool OnlyMyLogs;            
            public string APKPath;
            public string DeviceID;
        };
        Project prj;
        List<DeviceInfo> Devices = new List<DeviceInfo>();
        List<string> msgList = new List<string>();
        AndroidBuildConfiguration AndroidConfig;
        AndroidLogSettings LogSettings = new AndroidLogSettings();
        bool sync = false;
        bool closeLogSesion = false;
        Process adbProcess = null;
        public AndroidDeployDialog(Project p,AndroidBuildConfiguration androidConfig)
        {
            prj = p;
            AndroidConfig = androidConfig;
            InitializeComponent();
            RefreshPackageList();
            OnRefreshDeviceList(null, null);
            EnableButtons(true);
        }

        private void EnableButtons(bool forConfig)
        {
            lbDevices.Enabled = forConfig;
            lbPackages.Enabled = forConfig;
            comboDevices.Enabled = forConfig;
            comboPackages.Enabled = forConfig;
            btnRefresh.Enabled = forConfig;
            btnStart.Enabled = forConfig;
            btnSettings.Enabled = forConfig;
            btnSaveLog.Enabled = forConfig;

            // grupul 2
            btnStop.Enabled = !forConfig;
        }

        private void RefreshPackageList()
        {
            string path = Path.Combine(prj.ProjectPath, "bin", AndroidConfig.Name);
            try
            {
                string[] names = Directory.GetFiles(path);
                List<string> nm = new List<string>();
                foreach (string name in names)
                {
                    if (name.ToLower().EndsWith(".apk"))
                    {
                        nm.Add(Path.GetFileName(name));
                    }
                }
                nm.Sort();
                nm.Reverse();
                foreach (string name in nm)
                    comboPackages.Items.Add(name);
            }
            catch (Exception)
            {
            }
            if (comboPackages.Items.Count == 0)
            {
                MessageBox.Show("You have no Android build made for " + AndroidConfig.Name);
                return;
            }
            comboPackages.SelectedIndex = 0; // latest
        }
        private void OnRefreshDeviceList(object sender, EventArgs e)
        {
            comboDevices.Items.Clear();
            Devices.Clear();
            if (File.Exists(prj.Settings.AndroidADB)==false)
            {
                MessageBox.Show("Path to Android ADB was not set. Go to settings dialog and configure Android ADB.");
                return;
            }
            string s = prj.ShellExecute(prj.Settings.AndroidADB, "devices -l", "");
            if (s != null)
            {
                s = s.Replace("\t"," ") + "\n";
                string[] lines = s.Split('\n');
                foreach (string ln in lines)
                {
                    string line = ln.Trim();
                    if (line.Length < 17)
                        continue;
                    if (line.StartsWith("List of "))
                        continue;
                    bool ok = true;
                    int size = 0;
                    for (size = 0; (size < 16) && (line[size] != ' '); size++)
                    {
                        ok &= (((line[size] >= '0') && (line[size] <= '9')) || ((line[size] >= 'a') && (line[size] <= 'z')) || ((line[size] >= 'A') && (line[size] <= 'Z')));
                    }
                    if (line.StartsWith("emulator"))
                    {
                        size = line.IndexOf(" ");
                        if (size < 0)
                            size = line.Length;
                        ok = true;
                    }
                    if (!ok)
                        continue;
                    // am un ID - caut si numele
                    string name = "?";
                    int i1 = line.IndexOf(" model:");
                    if (i1 > 0)
                    {
                        i1 += 7;
                        int i2 = line.IndexOf(" ", i1);
                        if (i2 < 0)
                            i2 = line.Length;
                        if (i2 >= i1)
                            name = line.Substring(i1, i2 - i1).Trim();
                    }
                    DeviceInfo di = new DeviceInfo();
                    di.Id = line.Substring(0, size);
                    di.Name = name;
                    Devices.Add(di);
                }
            }
            // populez combobox-ul
            foreach (DeviceInfo di in Devices)
                comboDevices.Items.Add(di.Name + " (" + di.Id + ")");
            if (Devices.Count == 0)
            {
                MessageBox.Show("There are no Android devices connected to this computer.");
            }
            else
            {
                comboDevices.SelectedIndex = 0;
            }
        }

        private void OnShowFullLog(object sender, EventArgs e)
        {
            cbFromAllProcesses.Checked = true;
            cbOnlyMyApp.Checked = false;
        }

        private void OnShowLogFromMyApp(object sender, EventArgs e)
        {
            cbFromAllProcesses.Checked = false;
            cbOnlyMyApp.Checked = true;
        }

        private void OnStart(object sender, EventArgs e)
        {
            if (comboDevices.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a device !");
                return;
            }
            if (File.Exists(prj.Settings.AndroidADB) == false)
            {
                MessageBox.Show("Android ADB is not set. Please set it up from the Settings dialog !");
                return;
            }
            if (comboPackages.SelectedIndex < 0)
                LogSettings.APKPath = "";
            else
                LogSettings.APKPath = (string)comboPackages.Items[comboPackages.SelectedIndex];
            LogSettings.PerformInstall = ((cbInstallAndRun.Checked) || (cbJustInstall.Checked));
            LogSettings.RunApplication = ((cbInstallAndRun.Checked) || (cbJustRun.Checked));
            LogSettings.ClearLogCat = cbClearLogCatCahe.Checked;
            LogSettings.ClearUserData = cbClearUserData.Checked;
            LogSettings.DeviceID = Devices[comboDevices.SelectedIndex].Id;
            LogSettings.OnlyMyLogs = cbOnlyMyApp.Checked;            
            if ((LogSettings.PerformInstall) && (LogSettings.APKPath.Length == 0))
            {
                MessageBox.Show("Please select an package to be installed or chose 'Do nothing...' option if you want to install the package manually.");
                return;
            }
            closeLogSesion = false;
            EnableButtons(false);
            Worker.RunWorkerAsync();
        }

        private void OnStop(object sender, EventArgs e)
        {
            closeLogSesion = true;
        }

        private void OnStartBackgroundWork(object senderObject, DoWorkEventArgs earg)
        {            
            string device_id = "-s "+LogSettings.DeviceID+" ";
            string param = "";
            string result = "";
            
            if (LogSettings.PerformInstall)
            {
                if (LogSettings.ClearUserData)
                {
                    // e nevoie de un fresh install - facem mai intai uninstall
                    param = device_id+" uninstall "+AndroidConfig.Package;
                    AddFrameworkMessage("Uninstalling: " + AndroidConfig.Package);
                    result = prj.ShellExecute(prj.Settings.AndroidADB, param, "");
                }
                // instalam aplicatia
                param = device_id + " install -r \"" + Path.Combine(prj.ProjectPath, "bin", AndroidConfig.Name,LogSettings.APKPath) + "\"";
                AddFrameworkMessage("Installing: " + LogSettings.APKPath);
                result = prj.ShellExecute(prj.Settings.AndroidADB, param, "");
                AddFrameworkMessage(result);
            }
            // curat cache-ul existent
            if (LogSettings.ClearLogCat)
            {
                AddFrameworkMessage("Cleaning up logcat cache...");
                param = device_id + " logcat -c ";
                prj.ShellExecute(prj.Settings.AndroidADB, param, "",5);
                if (prj.EC.HasErrors())
                {
                    AddFrameworkErrors();
                    return;
                }
            }
            // rulez aplicatia
            if (LogSettings.RunApplication)
            {
                AddFrameworkMessage("Starting application ...");
                //adb shell am start -a android.intent.action.MAIN -n com.SimpleTest/.MainActivity
                param = device_id + " shell am start -a android.intent.action.MAIN -n " + AndroidConfig.Package + "/.MainActivity";
                result = prj.ShellExecute(prj.Settings.AndroidADB, param, "");
                AddFrameworkMessage(result);
            }
            // pornesc logcat-ul
            param = device_id+" logcat ";
            if (LogSettings.OnlyMyLogs) 
                param+=" -s GAPP ";
            try
            {
                adbProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = prj.Settings.AndroidADB,
                        Arguments = param,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };                           
                AutoResetEvent outputWaitHandle = new AutoResetEvent(false);
                adbProcess.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        msgList.Add(e.Data);
                    }
                };
                adbProcess.Start();
                adbProcess.BeginOutputReadLine();
                while ((adbProcess.HasExited==false) && (closeLogSesion==false))
                {
                    outputWaitHandle.WaitOne(1000);
                    if (msgList.Count>0)
                        UpdateMessages();
                }
                closeLogSesion = true;
                if (adbProcess.HasExited == false)
                    adbProcess.Kill();
                AddFrameworkMessage("Terminating log session ...");
            }
            catch (Exception exc)
            {
                prj.EC.AddException("Unable to start loggin from ADB", exc);
                AddFrameworkErrors();
                return;
            }
        }
        private void AddFrameworkErrors()
        {
            for (int tr = 0; tr < prj.EC.GetCount(); tr++)
            {
                ErrorsContainer.ErrorInfo ei = prj.EC.Get(tr);
                string s = "##FRMERR##"+ei.Error;
                if ((ei.Exception!=null) && (ei.Exception.Length>0))
                    s+=" => "+ei.Exception;
                msgList.Add(s);
            }
            UpdateMessages();
        }
        private void AddFrameworkMessage(string ss)
        {
            if (ss.Contains("\n"))
            {
                string[] l = ss.Split('\n');
                foreach (string line in l)
                {
                    string s2 = line.Trim();
                    if (s2.Length>0)
                        msgList.Add("##FRMINFO##" + s2);
                }
            }
            else
            {
                msgList.Add("##FRMINFO##" + ss);
            }
            UpdateMessages();
        }
        private void UpdateMessages()
        {
            sync = true;
            Worker.ReportProgress(0);
            while (sync) { }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ListViewItem lvi;
            string s;
            int index,i2;
            char ch;
            int count = msgList.Count;
            for (int tr=0;tr<count;tr++)
            {
                s = msgList[tr];
                if (s.Length > 8)
                {
                    if (s.StartsWith("##FRMINFO##"))
                    {
                        lvi = new ListViewItem("Framework");
                        lvi.SubItems.Add(s.Substring(11).Trim());
                        lvi.ImageKey = "framework";
                        lvi.ForeColor = Color.Gray;
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    if ((index=s.IndexOf("[C++  INFO] "))>0)
                    {
                        lvi = new ListViewItem("Debug Info");
                        lvi.SubItems.Add(s.Substring(index + 12).Trim());
                        lvi.ImageKey = "cppinfo";
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    if ((index = s.IndexOf("[C++ EVENT] ")) > 0)
                    {
                        lvi = new ListViewItem("Event");
                        lvi.SubItems.Add(s.Substring(index + 12).Trim());
                        lvi.ImageKey = "cppevent";
                        lvi.ForeColor = Color.DarkGreen;
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    if ((index = s.IndexOf("[JavaEVENT] THID:")) > 0)
                    {
                        lvi = new ListViewItem("Thread="+s.Substring(index+17,4).Trim());
                        lvi.SubItems.Add(s.Substring(index + 25).Trim());
                        lvi.ImageKey = "javaevent";
                        lvi.ForeColor = Color.DarkGreen;
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    if ((index = s.IndexOf("[Java INFO] THID:")) > 0)
                    {
                        lvi = new ListViewItem("Thread=" + s.Substring(index + 17, 4).Trim());
                        lvi.SubItems.Add(s.Substring(index + 25).Trim());
                        lvi.ImageKey = "javainfo";
                        if (s.Contains("----- APP STARTED -----"))
                        {
                            lvi.ForeColor = Color.Yellow;
                            lvi.BackColor = Color.DarkGreen;
                        }
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    if ((index = s.IndexOf("[C++ ERROR] ")) > 0)
                    {
                        index += 12;
                        i2 = s.IndexOf(":", index);
                        if (i2 < 0)
                            continue;
                        i2 = s.IndexOf(") ->", i2);
                        if (i2 < 0)
                            continue;
                        if (i2 + 4 >= s.Length)
                            continue;
                        lvi = new ListViewItem(s.Substring(index,i2-index+1));
                        lvi.SubItems.Add(s.Substring(i2+4).Trim());
                        lvi.ImageKey = "cpperror";
                        lvi.ForeColor = Color.Red;
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    if ((index = s.IndexOf("[JavaERROR] THID:")) > 0)
                    {
                        lvi = new ListViewItem("Thread=" + s.Substring(index + 17, 4).Trim());
                        lvi.SubItems.Add(s.Substring(index + 25).Trim());
                        lvi.ImageKey = "javaerror";
                        lvi.ForeColor = Color.Red;
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    ch = s[0];
                    if ((s[1] == '/') && ((ch == 'D') || (ch == 'E') || (ch == 'I') || (ch == 'W') || (ch == 'V')))
                    {
                        index = s.IndexOf(":");
                        if (index < 4)
                            continue;
                        lvi = new ListViewItem(s.Substring(2, index - 2).Trim());
                        lvi.SubItems.Add(s.Substring(index + 1).Trim());
                        switch (ch)
                        {
                            case 'E':
                                lvi.ImageKey = "error";
                                lvi.ForeColor = Color.DarkRed;
                                break;
                            case 'W':
                                lvi.ImageKey = "warning";
                                lvi.ForeColor = Color.Orange;
                                break;
                            case 'D':
                                lvi.ImageKey = "debug";
                                lvi.ForeColor = Color.Blue;
                                break;
                            default:
                                lvi.ImageKey = "info";
                                lvi.ForeColor = Color.Gray;
                                break;
                        }
                        lstMessages.Items.Add(lvi);
                        continue;
                    }
                    lvi = new ListViewItem("Android");
                    lvi.SubItems.Add(s.Trim());
                    lvi.ImageKey = "framework";
                    lvi.ForeColor = Color.LightGray;
                    lstMessages.Items.Add(lvi);
                    
                }
            }
            msgList.RemoveRange(0, count);
            lstMessages.Columns[1].Text = String.Format("Messages ({0})", lstMessages.Items.Count);
            if (btnAutoScroll.Checked)
            {
                count = lstMessages.Items.Count;
                if (count > 0)
                    lstMessages.Items[count - 1].EnsureVisible();
            }
            sync = false;
        }

        private void OnBackgroundWorkCompleted(object sender, RunWorkerCompletedEventArgs ex)
        {
            // opresc ADB-ul daca e cazul
            try
            {
                if ((adbProcess != null) && (adbProcess.HasExited == false))
                    adbProcess.Kill();
            }
            catch (Exception e)
            {
                prj.EC.AddException("Unable to terminate adb process !", e);
                prj.ShowErrors();
            }
            EnableButtons(true);
            if (btnClose.Enabled == false)
                this.Close();
        }

        private void OnSetInstallAndRun(object sender, EventArgs e)
        {
            cbInstallAndRun.Checked = true;
            cbJustInstall.Checked = false;
            cbJustRun.Checked = false;
            cbDoNothing.Checked = false;
        }

        private void OnSetJustInstall(object sender, EventArgs e)
        {
            cbInstallAndRun.Checked = false;
            cbJustInstall.Checked = true;
            cbJustRun.Checked = false;
            cbDoNothing.Checked = false;
        }

        private void OnSetJustRun(object sender, EventArgs e)
        {
            cbInstallAndRun.Checked = false;
            cbJustInstall.Checked = false;
            cbJustRun.Checked = true;
            cbDoNothing.Checked = false;
        }

        private void OnJustLog(object sender, EventArgs e)
        {
            cbInstallAndRun.Checked = false;
            cbJustInstall.Checked = false;
            cbJustRun.Checked = false;
            cbDoNothing.Checked = true;
        }

        private void OnTerminateLogSession(object sender, EventArgs e)
        {
            if (Worker.IsBusy)
            {
                btnClose.Enabled = false;
                closeLogSesion = true;
            }
            else
            {
                this.Close();
            }
        }

        private void OnSetAutoScroll(object sender, EventArgs e)
        {
            int count = lstMessages.Items.Count;
            if (count > 0)
                lstMessages.Items[count - 1].EnsureVisible();
        }

        private void OnClearList(object sender, EventArgs e)
        {            
            lstMessages.Items.Clear();
            lstMessages.Columns[1].Text = String.Format("Messages ({0})", lstMessages.Items.Count);
        }

        private void OnSaveLog(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Text files|*.txt|All Files|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string s = "";
                foreach (ListViewItem lvi in lstMessages.Items)
                {
                    string h = lvi.Text + "                                                                                                                     ";
                    h = h.Substring(0, 32) + " | ";
                    s += h + lvi.SubItems[1].Text.TrimStart() + "\n";
                }
                if (Disk.SaveFile(dlg.FileName, s, prj.EC) == false)
                    prj.ShowErrors();
            }
        }
    }
}
