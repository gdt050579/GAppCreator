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
using System.Runtime.InteropServices;

namespace GAppCreator
{
    public partial class ExecutionControlDialog : Form
    {
        public enum VariableDataType : int
        {
            Unknown = -1,
            UInt8 = 0,
            UInt16 = 1,
            UInt32 = 2,
            UInt64 = 3,
            Int8 = 4,
            Int16 = 5,
            Int32 = 6,
            Int64 = 7,
            Float32 = 8,
            Float64 = 9,
            Bool = 10,
            String = 11,
        }
        public enum VariableDataStorage
        {
            Normal,
            Serializable,
            Persistent
        };
        public class VariableData : IComparable
        {
            public string Name = "";
            public VariableDataType Type = VariableDataType.Unknown;
            public VariableDataStorage Storage = VariableDataStorage.Normal;
            public byte[] Data = null;

            public void SetTypeAndStorage(byte flags)
            {
                byte dtype = (byte)(flags & (byte)0x0f);
                if (dtype < 12)
                    Type = (VariableDataType)((int)dtype);
                else
                    Type = VariableDataType.Unknown;
                if ((dtype & 128) != 0)
                    Storage = VariableDataStorage.Persistent;
                else if ((dtype & 64) != 0)
                    Storage = VariableDataStorage.Serializable;
                else
                    Storage = VariableDataStorage.Normal;
            }
            private static int GetTypeSize(VariableDataType Type)
            {
                switch (Type)
                {
                    case VariableDataType.UInt8:
                    case VariableDataType.Int8:
                    case VariableDataType.Bool:
                        return 1;
                    case VariableDataType.UInt16:
                    case VariableDataType.Int16:
                        return 2;
                    case VariableDataType.UInt32:
                    case VariableDataType.Int32:
                    case VariableDataType.Float32:
                        return 4;
                    case VariableDataType.UInt64:
                    case VariableDataType.Int64:
                    case VariableDataType.Float64:
                        return 8;
                    default:
                        return 0;
                }
            }
            public string GetValue(byte[] b, VariableDataType Type, int index)
            {
                try
                {
                    switch (Type)
                    {
                        case VariableDataType.UInt8:
                            return string.Format("{0}", b[index]);
                        case VariableDataType.UInt16:
                            return string.Format("{0}", BitConverter.ToUInt16(b, index));
                        case VariableDataType.UInt32:
                            return string.Format("{0}", BitConverter.ToUInt32(b, index));
                        case VariableDataType.UInt64:
                            return string.Format("{0}", BitConverter.ToUInt64(b, index));
                        case VariableDataType.Int8:
                            return string.Format("{0}", (sbyte)b[index]);
                        case VariableDataType.Int16:
                            return string.Format("{0}", BitConverter.ToInt16(b, index));
                        case VariableDataType.Int32:
                            return string.Format("{0}", BitConverter.ToInt32(b, index));
                        case VariableDataType.Int64:
                            return string.Format("{0}", BitConverter.ToInt64(b, index));
                        case VariableDataType.Float32:
                            return string.Format("{0}", BitConverter.ToSingle(b, index));
                        case VariableDataType.Float64:
                            return string.Format("{0}", BitConverter.ToDouble(b, index));
                        case VariableDataType.Bool:
                            if (b[index] == 0)
                                return "False";
                            else
                                return "True";
                        case VariableDataType.String:
                            string tmp = "";
                            for (int tr = 0; tr < b.Length; tr++)
                                if ((b[tr] >= 32) && (b[tr] < 128))
                                    tmp += (char)b[tr];
                                else
                                    tmp += "?";
                            return tmp;
                    }
                    return "?";
                }
                catch (Exception)
                {
                    return "?";
                }
            }
            public void UpdateListViewItem(ListViewItem lvi)
            {
                lvi.Text = Name;
                lvi.Tag = this;
                while (lvi.SubItems.Count < 5)
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem());
                lvi.SubItems[1].Text = Type.ToString();
                if (Storage != VariableDataStorage.Normal)
                    lvi.SubItems[1].Text += " (" + Storage.ToString() + ")";
                int dtSz = GetTypeSize(Type);
                if ((dtSz == 0) || (Data == null))
                    lvi.SubItems[2].Text = "-";
                else
                {
                    lvi.SubItems[2].Text = (Data.Length / dtSz).ToString();
                }
                string tmp = "";
                if (dtSz > 0)
                {
                    for (int tr = 0; tr < Data.Length; tr += dtSz)
                    {
                        tmp += GetValue(Data, Type, tr);
                        tmp += " , ";
                    }
                }
                else
                {
                    if (Data != null)
                        tmp = GetValue(Data, Type, 0);
                }
                if (tmp.EndsWith(" , "))
                    tmp = tmp.Substring(0, tmp.Length - 3);
                lvi.SubItems[3].Text = tmp;
                tmp = "";
                for (int tr = 0; (tr < 32) && (tr < Data.Length); tr++)
                    tmp += string.Format("{0:X2} ", Data[tr]);
                if (Data.Length > 32)
                    tmp += " ... ";
                lvi.SubItems[4].Text = tmp;
                if (Storage == VariableDataStorage.Persistent)
                    lvi.BackColor = Color.LightGreen;
                else if (Storage == VariableDataStorage.Serializable)
                    lvi.BackColor = Color.LightYellow;
                else
                    lvi.BackColor = Color.White;
                if (Name.StartsWith("__"))
                    lvi.ForeColor = Color.Gray;
            }
            public int CompareTo(Object o)
            {
                return Name.CompareTo(((VariableData)o).Name);
            }
        };
        private enum MessageType
        {
            DebugInterfaceInfo,
            DebugInterfaceError,
            FrameworkError,
            FrameworkInfo,
            FrameworkEvent,
            AppInfo,
            AppError,
        };
        private class Message
        {
            public string Text = "";
            public string Line = null;
            public string Function = null;
            public string Condition = null;
            public DateTime Date;
            public MessageType Type;
            public Message(MessageType type, string text)
            {
                Type = type;
                Text = text;
                Date = DateTime.Now;
            }
            public Message(MessageType type, string text, string line, string function, string condition)
            {
                Type = type;
                Text = text;
                Date = DateTime.Now;
                Line = line;
                Function = function;
                Condition = condition;
            }
            public string GetLine()
            {
                string s = String.Format("{0,14}|{1,10}|", Type.ToString(), Date.ToLongTimeString());
                if (Line != null)
                    s += "Line:" + Line + ",";
                if (Function != null)
                    s += "Function:" + Function + ",";
                if (Condition != null)
                    s += "Condition:" + Condition + ",";
                s += Text;
                return s;
            }
        };
        public class AnalyticsInfo
        {
            public Int64 Count = 0;
            public Int64 intSum = 0;
            public double dblSum = 0;
            public ListViewItem itm = null;
        }
        private enum ProcessCommands : int
        {
            Terminate = 0,
            Pause = 1,
            Resume = 2,
            Scale = 3,
            TakePicture = 4,
            GetSettings = 5,
            Suspend = 6,
            ChangeScene = 7,
            GetTexture = 8,
            ReloadExecutionSettings = 9,
            CreateNewSnapshot = 10,
            EnableCounter = 11,
            DisableCounter = 12,
            RecheckAlarms = 13,
            UpdateTimeDelta = 14,
        };
        private enum MemoryStatusInfo
        {
            Wait,
            Update,
            Completed
        }

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        string lastSettingsSnapshotMessage = "";
        int lastSettingsSnapshotID = 0;
        int currentSettingsSnapshotParent = 0;


        Bitmap screenShotBitmap = null;
        Bitmap textureBitmap = null;
        private bool sync;
        List<string> msgList = new List<string>();
        List<Message> Messages = new List<Message>();

        string[] tmpString = new string[4];
        Process executionProcess = null;
        bool closeLogSesion = false;
        bool debugInterfaceWillClose = false;
        bool autoScroll;
        bool restart;
        string FileToRun;
        Project prj;
        ExecutionSettings RunSettings = new ExecutionSettings();
        Dictionary<string, ListViewItem> AppStatusItems = new Dictionary<string, ListViewItem>();
        DebugParamControl[] dbgParams;
        SortedDictionary<uint, MemoryEventInfo> Mem = new SortedDictionary<uint, MemoryEventInfo>();
        List<MemoryEventInfo> MemEvents = new List<MemoryEventInfo>();
        MemoryStatusInfo RefreshMemoryAllocation = MemoryStatusInfo.Wait;
        Stack<DeleteMetaData> DeleteMetaDataStack = new Stack<DeleteMetaData>();
        Dictionary<string, AnalyticsInfo> analytics = new Dictionary<string, AnalyticsInfo>();
        uint MaxMemoryPeek = 0;
        uint CurrentMemoryPeek = 0;


        public ExecutionControlDialog(string run, Project p)
        {
            FileToRun = run;
            prj = p;
            ((DevelopBuildConfiguration)(prj.BuildConfigurations[0])).UpdateRunSettings(RunSettings);
            InitializeComponent();
            lstOutput.Dock = DockStyle.Fill;
            pnlScreenShots.Dock = DockStyle.Fill;
            pnlAppSettings.Dock = DockStyle.Fill;
            pnlAppStatus.Dock = DockStyle.Fill;
            pnlDebugCommands.Dock = DockStyle.Fill;
            pnlMemory.Dock = DockStyle.Fill;
            pnlAnalyticsEmulator.Dock = DockStyle.Fill;
            pnlGlobalCountersAndTimers.Dock = DockStyle.Fill;

            propRunSettings.SelectedObject = RunSettings;
            restart = false;
            UpdateMessageCountInfo();
            this.Text = "Running " + run;

            OnShowOutput(null, null);
            //OnMaximizeMinimize(null, null);
            OnAutoScroll(null, null);


            dbgParams = new DebugParamControl[8];
            for (int tr = 0; tr < 8; tr++)
            {
                dbgParams[tr] = new DebugParamControl();
                dbgParams[tr].TabIndex = tr + 1;
                pnlDebugCommands.Controls.Add(dbgParams[tr]);
                dbgParams[tr].Dock = DockStyle.Bottom;

            }
            UpdateDebugCommandList();

            btnPin.Checked = RunSettings.ConsoleAlwaysOnTop;
            this.TopMost = RunSettings.ConsoleAlwaysOnTop;

        }
        private void ExecutionControlDialog_Load(object sender, EventArgs e)
        {
            this.Left = 0;
            this.Top = 0;
            //this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            cbMaximize.Checked = RunSettings.ConsoleIsMaximized;
            OnMaximizeMinimize(null, null);
        }


        private void SendMessage(ProcessCommands cmd, int value)
        {
            if (executionProcess != null)
                PostMessage(executionProcess.MainWindowHandle, 0xB000, (int)cmd, value);
        }
        private void SendDebugCommand(uint id, int v1, int v2)
        {
            if (executionProcess != null)
                PostMessage(executionProcess.MainWindowHandle, 0xB001 + id, v1, v2);
        }
        private bool CreateRunSettings()
        {
            int x = -1, y = -1;
            switch (RunSettings.ConsolePosition)
            {
                case ControlConsolePosition.OnLeft:
                    x = this.Left + this.Width;
                    y = this.Top;
                    break;
                case ControlConsolePosition.OnTop:
                    x = this.Left;
                    y = this.Top + this.Height;
                    break;
                case ControlConsolePosition.Overlap:
                    x = this.Left;
                    y = this.Top;
                    break;
            }
            // fac fisierul settings
            if (RunSettings.CreateSettings(prj, x, y) == false)
            {
                prj.ShowErrors();
                return false;
            }
            return true;
        }
        private void Worker_DoWork(object senderObject, DoWorkEventArgs earg)
        {
            try
            {
                if (CreateRunSettings() == false)
                    return;

                executionProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = FileToRun,
                        Arguments = "",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,

                    }
                };
                AutoResetEvent outputWaitHandle = new AutoResetEvent(false);
                executionProcess.OutputDataReceived += (sender, e) =>
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
                executionProcess.Start();
                executionProcess.BeginOutputReadLine();

                while ((executionProcess.HasExited == false) && (closeLogSesion == false))
                {
                    outputWaitHandle.WaitOne(1000);
                    if (msgList.Count > 0)
                        UpdateMessages();

                }
                if (executionProcess.HasExited == false)
                    executionProcess.Kill();
                AddFrameworkMessage("Terminating log session ...");
            }
            catch (Exception exc)
            {
                AddFrameworkErrors("Unable to start logging\n" + exc.ToString());
                return;
            }
        }
        private void AddFrameworkErrors(string ss)
        {
            if (ss.Contains("\n"))
            {
                string[] l = ss.Split('\n');
                foreach (string line in l)
                {
                    string s2 = line.Trim();
                    if (s2.Length > 0)
                        msgList.Add("##FRMERR##" + s2);
                }
            }
            else
            {
                msgList.Add("##FRMERR##" + ss);
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
                    if (s2.Length > 0)
                        msgList.Add("##FRMINFO##" + s2);
                }
            }
            else
            {
                msgList.Add("##FRMINFO##" + ss);
            }
            UpdateMessages();
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int count = msgList.Count;
            for (int tr = 0; tr < count; tr++)
            {
                AnalizeOutputMessage(msgList[tr]);
            }
            msgList.RemoveRange(0, count);
            // autoscroll
            if ((autoScroll) && (lstOutput.Items.Count > 0))
                lstOutput.Items[lstOutput.Items.Count - 1].EnsureVisible();
            sync = false;
        }
        private void OnDebugMessage(string[] param)
        {
            if ((param == null) || (param.Length == 0))
            {
                MessageBox.Show("Internal error - receiving a debug message without command or parameters");
                return;
            }
            switch (param[0])
            {
                case "TakePictureFailed": MessageBox.Show("Take Picture failed !"); return;
                case "TakePictureOk": LoadBitmap(Int32.Parse(param[1]), Int32.Parse(param[2])); return;
                case "GetSettingsOk": UpdateAppSettingsListView(); return;
                case "GetSettingsFailed": MessageBox.Show("Getting the settings failed !. Check the output for more details !"); return;
                case "NewScene": AddAppStatus("Scenes", param[2], "Not loaded", param[1], Color.Gray); return;
                case "InitScene": AddAppStatus("Scenes", param[2], "Loaded", param[1], Color.Black); return;
                case "DeactivateScene": AddAppStatus("Scenes", param[2], "Loaded", param[1], Color.Black); return;
                case "ActivateScene": AddAppStatus("Scenes", param[2], "Current", param[1], Color.DarkGreen); return;
                case "ProfileLoad": AddAppStatus("Profiles", param[1], "Loaded (" + param[3] + " textures, " + param[2] + " resources)", param[1], Color.DarkGreen); return;
                case "ProfileUnLoad": AddAppStatus("Profiles", param[1], "Not loaded", param[1], Color.Gray); return;
                case "ProfileDelayLoad": AddAppStatus("Profiles", param[1], "Delay load", param[1], Color.Brown); return;
                case "TextureLoad": AddAppStatus("Textures", "Texture #" + param[1], param[2] + " x " + param[3] + " (" + param[4] + " images)", param[1], Color.Black); return;
                case "TextureUnload": RemoveAppStatus("Textures", "Texture #" + param[1]); return;
                case "GetTextureFailed": MessageBox.Show("Getting the texture failed !\n" + param[1]); return;
                case "GetTextureOk": LoadTexture(Int32.Parse(param[1]), Int32.Parse(param[2])); return;
                case "SettingsLoadedOK": MessageBox.Show("New settings were imported into the app !"); return;
                case "SnapshotCreatedOK":
                    prj.SettingsSnapshots.AddNewSnapshot(lastSettingsSnapshotID, currentSettingsSnapshotParent, lastSettingsSnapshotMessage);
                    currentSettingsSnapshotParent = lastSettingsSnapshotID;
                    ctrlSnapshots.RefreshSnapshotLists();
                    MessageBox.Show("App snapshot created !");
                    return;
                case "Alloc": OnNewMemoryAllocated(param); return;
                case "DeAllocMetaData": AddDeleteMetaData(param); return;
                case "DeAllocPtr": OnMemoryDeAllocated(param); return;
                case "ANALYTICS": OnAnalytics(param[1], param[2]); return;
                case "GLOBALCOUNTER": OnGlobalCounterUpdate(param); return;
                case "GLOBALCOUNTERGROUP": OnGlobalCounterGroupsUpdate(param); return;
                case "ALARM": OnAlarm(param); return;
                case "ALARMTEST": OnAlarmTest(param); return;
                case "ALARMEVENT": OnAlarmEvent(param); return;
            }

            string p = "";
            for (int tr = 1; (tr < param.Length) && (tr < 6); tr++)
                p += "Param #" + tr.ToString() + " : " + param[tr] + "\n";
            MessageBox.Show("Unknown debug message: " + param[0] + "\n" + p);
        }

        private void UpdateMessageCountInfo()
        {
            lstOutput.Columns[5].Text = "Messages (" + lstOutput.Items.Count.ToString() + " / " + Messages.Count.ToString() + ")";
        }
        private void AddListItemMessage(Message m)
        {
            switch (m.Type)
            {
                case MessageType.AppError:
                    if (cbGacErrors.Checked == false)
                        return;
                    break;
                case MessageType.AppInfo:
                    if (cbGacMessages.Checked == false)
                        return;
                    break;
                case MessageType.DebugInterfaceError:
                case MessageType.FrameworkError:
                    if (cbFrameworkErrors.Checked == false)
                        return;
                    break;
                case MessageType.DebugInterfaceInfo:
                case MessageType.FrameworkInfo:
                    if (cbFrameworkMessages.Checked == false)
                        return;
                    break;
                case MessageType.FrameworkEvent:
                    if (cbFrameworkEvents.Checked == false)
                        return;
                    break;
                default:
                    return;
            }
            string s = txFilterLog.Text.ToLower();
            if (s.Length > 0)
            {
                bool hasFilter = m.Text.ToLower().Contains(s);
                if (m.Line != null)
                    hasFilter |= m.Line.ToLower().Contains(s);
                if (m.Condition != null)
                    hasFilter |= m.Condition.ToLower().Contains(s);
                if (m.Function != null)
                    hasFilter |= m.Function.ToLower().Contains(s);
                if (hasFilter == false)
                    return;
            }
            // totul e ok - adaug item-ul
            ListViewItem lvi = new ListViewItem("");
            switch (m.Type)
            {
                case MessageType.AppError:
                    lvi.Text = "Error";
                    lvi.ForeColor = Color.Red;
                    lvi.ImageKey = "error";
                    break;
                case MessageType.AppInfo:
                    lvi.Text = "Info";
                    lvi.ForeColor = Color.Black;
                    lvi.ImageKey = "info";
                    break;
                case MessageType.DebugInterfaceError:
                case MessageType.FrameworkError:
                    lvi.Text = "FrmError";
                    lvi.ForeColor = Color.DarkRed;
                    lvi.ImageKey = "cpperror";
                    break;
                case MessageType.DebugInterfaceInfo:
                case MessageType.FrameworkInfo:
                    lvi.Text = "FrmInfo";
                    lvi.ForeColor = Color.Gray;
                    lvi.ImageKey = "cppinfo";
                    break;
                case MessageType.FrameworkEvent:
                    lvi.Text = "Event";
                    lvi.ForeColor = Color.DarkGreen;
                    lvi.ImageKey = "cppevent";
                    break;
                default:
                    return;
            }
            lvi.SubItems.Add(m.Date.ToLongTimeString());
            if (m.Function != null)
                lvi.SubItems.Add(m.Function);
            else
                lvi.SubItems.Add("");
            if (m.Line != null)
                lvi.SubItems.Add(m.Line);
            else
                lvi.SubItems.Add("");
            if (m.Condition != null)
                lvi.SubItems.Add(m.Condition);
            else
                lvi.SubItems.Add("");
            lvi.Tag = m;
            lvi.SubItems.Add(m.Text);
            lstOutput.Items.Add(lvi);
        }
        private void AddLastMessageToListView()
        {
            AddListItemMessage(Messages[Messages.Count - 1]);
            UpdateMessageCountInfo();
        }
        private void AnalizeOutputMessage(string msg)
        {
            if (msg == null)
                return;
            if (msg.StartsWith("[LOG-ERROR] "))
            {
                Messages.Add(new Message(MessageType.FrameworkError, msg.Substring(12)));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("[LOG-INFO ] "))
            {
                Messages.Add(new Message(MessageType.FrameworkInfo, msg.Substring(12)));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("[GAC-ERROR] "))
            {
                Messages.Add(new Message(MessageType.AppError, msg.Substring(12)));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("[GAC-INFO ] "))
            {
                Messages.Add(new Message(MessageType.AppInfo, msg.Substring(12)));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("[  EVENT  ] "))
            {
                Messages.Add(new Message(MessageType.FrameworkEvent, msg.Substring(12)));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("##FRMINFO##"))
            {
                Messages.Add(new Message(MessageType.DebugInterfaceInfo, msg.Substring(12)));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("##FRMERR##"))
            {
                Messages.Add(new Message(MessageType.DebugInterfaceError, msg.Substring(12)));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("[  ERROR  ]_|_"))
            {
                tmpString[0] = tmpString[1] = tmpString[2] = tmpString[3] = "";
                int start = 14;
                int index;
                int poz = 0;
                do
                {
                    index = msg.IndexOf("_|_", start);
                    if (index < 0)
                    {
                        tmpString[poz] = msg.Substring(start);
                        break;
                    }
                    tmpString[poz] = msg.Substring(start, index - start);
                    start = index + 3;
                    poz++;
                    if (poz == tmpString.Length)
                        break;
                } while (true);
                Messages.Add(new Message(MessageType.FrameworkError, tmpString[3], tmpString[0], tmpString[1], tmpString[2]));
                AddLastMessageToListView();
                return;
            }
            if (msg.StartsWith("##DEBUG##:"))
            {
                OnDebugMessage(msg.Substring(10).Split('|'));
                return;
            }
            // generic 
            if (msg.Trim().Length == 0)
                return;
            Messages.Add(new Message(MessageType.DebugInterfaceInfo, msg));
            AddLastMessageToListView();
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnPause.Enabled = false;
            btnResume.Enabled = false;
            btnStop.Enabled = false;
            btnSuspend.Enabled = false;
            btnStart.Enabled = true;
            btnRestart.Enabled = false;
            btnCreateSnapshot.Enabled = false;
            executionProcess = null;
            if (closeLogSesion)
            {
                debugInterfaceWillClose = true;
                this.Close();
            }
            if ((restart) && (!closeLogSesion))
                restartTimer.Enabled = true;
            restart = false;
        }
        private void UpdateMessages()
        {
            sync = true;
            Worker.ReportProgress(0);
            while (sync) { }
        }
        private void ShowPanel(ToolStripMenuItem itm)
        {
            bool status;
            // output
            status = (itm == cbViewOutput);
            cbViewOutput.Checked = status;
            lstOutput.Visible = status;
            cbAutoScroll.Visible = status;
            btnClearLog.Visible = status;
            sepFilterLog.Visible = status;
            txFilterLog.Visible = status;
            lbFilterLog.Visible = status;
            btnFilterLogMenu.Visible = status;
            btnSaveLog.Visible = status;

            // screen shoots
            status = (itm == cbScreenShots);
            cbScreenShots.Checked = status;
            btnTakePicture.Visible = status;
            pnlScreenShots.Visible = status;
            btnSavePicture.Visible = status;
            btnPublish.Visible = status;
            btnCopyImage.Visible = status;

            // app settings
            status = (itm == cbApplicationSettings);
            cbApplicationSettings.Checked = status;
            btnDeleteSettings.Visible = status;
            btnCreateSnapshot.Visible = status;
            pnlAppSettings.Visible = status;

            // app status
            status = (itm == cbApplicationStatus);
            cbApplicationStatus.Checked = status;
            pnlAppStatus.Visible = status;
            btnGoToScene.Visible = status;
            btnViewTexture.Visible = status;
            btnSaveTexture.Visible = status;

            // debug commands
            status = (itm == cbDebugCommands);
            cbDebugCommands.Checked = status;
            pnlDebugCommands.Visible = status;
            lbDebugCommandQuickSearch.Visible = status;
            txDebugCommandFilter.Visible = status;
            btnRunDebugCommand.Visible = status;

            // memory
            status = (itm == cbMemory);
            cbMemory.Checked = status;
            pnlMemory.Visible = status;

            // analytics emulator
            status = (itm == cbAnalyticsEmulator);
            cbAnalyticsEmulator.Checked = status;
            pnlAnalyticsEmulator.Visible = status;

            // Timerss and Counters
            status = (itm == cbAlarmsAndCounters);
            cbAlarmsAndCounters.Checked = status;
            pnlGlobalCountersAndTimers.Visible = status;

            // general
            btnRefresh.Visible = (cbApplicationSettings.Checked); // | lstXXX.Visible | lstYYY.Visisble | ...


        }
        private void OnMaximizeMinimize(object sender, EventArgs e)
        {
            if (cbMaximize.Checked)
            {
                this.ClientSize = new Size(900, 700);
                pnlContainer.Visible = true;
                pnlCloseButton.Visible = true;
            }
            else
            {
                pnlContainer.Visible = false;
                pnlCloseButton.Visible = false;
                this.ClientSize = new Size(900, toolStrip.Height);
            }
        }

        private void OnShowOutput(object sender, EventArgs e)
        {
            ShowPanel(cbViewOutput);
        }

        private void OnDialogIsShown(object sender, EventArgs e)
        {
            OnStartApp(null, null);
        }

        private void OnShutDown(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.Terminate, 0);
        }
        private void OnPauseApp(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.Pause, 0);
            btnPause.Enabled = false;
            btnResume.Enabled = true;

        }
        private void OnResumeApp(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.Resume, 0);
            btnPause.Enabled = true;
            btnResume.Enabled = false;
        }
        private void OnSuspendApp(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.Suspend, 0);
        }
        private void OnScale100(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.Scale, 1);
            rbScale100.Checked = true;
            rbScale50.Checked = false;
            rbScale25.Checked = false;
        }

        private void OnScale50(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.Scale, 2);
            rbScale100.Checked = false;
            rbScale50.Checked = true;
            rbScale25.Checked = false;
        }

        private void OnScale25(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.Scale, 4);
            rbScale100.Checked = false;
            rbScale50.Checked = false;
            rbScale25.Checked = true;
        }
        private void OnTakePicture(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.TakePicture, 0);
        }
        private void OnCloseDialog(object sender, EventArgs e)
        {
            if (executionProcess != null)
            {
                OnShutDown(null, null);
                closeLogSesion = true;
            }
            else
            {
                debugInterfaceWillClose = true;
                this.Close();
            }
            btnCloseDialog.Enabled = false;
        }

        private void OnAutoScroll(object sender, EventArgs e)
        {
            autoScroll = cbAutoScroll.Checked;
            if ((autoScroll) && (lstOutput.Items.Count > 0))
                lstOutput.Items[lstOutput.Items.Count - 1].EnsureVisible();
        }

        private void OnFormActivated(object sender, EventArgs e)
        {

        }

        private void OnClearLog(object sender, EventArgs e)
        {
            Messages.Clear();
            OnRefilterMessages(null, null);
        }

        private void OnPaintScreenShot(object sender, PaintEventArgs e)
        {
            if (screenShotBitmap != null)
            {
                e.Graphics.DrawImage(screenShotBitmap, 0, 0, screenShotBitmap.Width, screenShotBitmap.Height);
            }
        }

        private void OnShowScreenShots(object sender, EventArgs e)
        {
            ShowPanel(cbScreenShots);
        }

        private void LoadTexture(int w, int h)
        {
            try
            {
                byte[] b = File.ReadAllBytes(FileToRun + ".data");
                if (b.Length != w * h * 4)
                {
                    MessageBox.Show(String.Format("Invalid texture - size {0}x{1} does not match its size: {2}", w, h, b.Length));
                    return;
                }
                textureBitmap = new Bitmap(w, h);
                int p = 0;
                for (int y = h - 1; y >= 0; y--)
                {
                    for (int x = 0; x < w; x++)
                    {
                        textureBitmap.SetPixel(x, y, Color.FromArgb(b[p + 3], b[p], b[p + 1], b[p + 2]));
                        p += 4;
                    }
                }
                pnlTextureView.Visible = true;
                pnlTextureView.Width = textureBitmap.Width;
                pnlTextureView.Height = textureBitmap.Height;
                btnSaveTexture.Enabled = true;
                pnlTextureView.Invalidate();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void LoadBitmap(int w, int h)
        {
            // incarc poza
            try
            {
                byte[] b = File.ReadAllBytes(FileToRun + ".data");
                if (b.Length != w * h * 3)
                {
                    MessageBox.Show(String.Format("Invalid picture - size {0}x{1} does not match its size: {2}", w, h, b.Length));
                    return;
                }
                screenShotBitmap = new Bitmap(w, h);
                int p = 0;
                for (int y = h - 1; y >= 0; y--)
                {
                    for (int x = 0; x < w; x++)
                    {
                        screenShotBitmap.SetPixel(x, y, Color.FromArgb(b[p], b[p + 1], b[p + 2]));
                        p += 3;
                    }
                }
                pnlFullScreenShotView.Width = screenShotBitmap.Width;
                pnlFullScreenShotView.Height = screenShotBitmap.Height;
                pnlFullScreenShotView.Invalidate();
                pnlSmallImageScreenShot.Invalidate();
                lstScreenShotProperties.Items.Clear();
                lstScreenShotProperties.Items.Add("Width");
                lstScreenShotProperties.Items.Add("Height");
                lstScreenShotProperties.Items[0].SubItems.Add(screenShotBitmap.Width.ToString());
                lstScreenShotProperties.Items[1].SubItems.Add(screenShotBitmap.Height.ToString());
                btnSavePicture.Enabled = true;
                btnPublish.Enabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void OnPaintSmallImageScreenShot(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Azure, 0, 0, pnlSmallImageScreenShot.Width, pnlSmallImageScreenShot.Height);
            if (screenShotBitmap != null)
            {
                int p_w = pnlSmallImageScreenShot.Width;
                int p_h = pnlSmallImageScreenShot.Height;
                float r1 = ((float)screenShotBitmap.Width) / ((float)p_w);
                float r2 = ((float)screenShotBitmap.Height) / ((float)p_h);
                if (r2 > r1)
                    r1 = r2;
                int nw = (int)(screenShotBitmap.Width / r1);
                int nh = (int)(screenShotBitmap.Height / r1);
                e.Graphics.DrawImage(screenShotBitmap, p_w / 2 - nw / 2, p_h / 2 - nh / 2, nw, nh);
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            pnlSmallImageScreenShot.Invalidate();
        }

        private void OnSaveSnapshot(object sender, EventArgs eargs)
        {
            if (screenShotBitmap == null)
            {
                MessageBox.Show("You need to take a snapshot first !");
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "PNG Files|*.png|All Files|*.*";
            dlg.DefaultExt = "png";
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    screenShotBitmap.Save(dlg.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to save image to " + dlg.FileName + "\n" + e.ToString());
                }
            }
        }

        private void OnPublishSnapshot(object sender, EventArgs e)
        {
            NewImagePublishObject dlg = new NewImagePublishObject(screenShotBitmap, prj);
            dlg.ShowDialog();
        }

        private void OnRefilterMessages(object sender, EventArgs e)
        {
            lstOutput.BeginUpdate();
            lstOutput.Items.Clear();
            foreach (Message m in Messages)
                AddListItemMessage(m);
            UpdateMessageCountInfo();
            lstOutput.EndUpdate();
        }

        private void OnFilterTextChanged(object sender, EventArgs e)
        {
            OnRefilterMessages(null, null);
        }

        private void OnSaveCompleteLog(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Log Files|*.log|All Files|*.*";
            dlg.DefaultExt = "log";
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string s = "";
                foreach (Message m in Messages)
                    s += m.GetLine() + "\n";
                if (Disk.SaveFile(dlg.FileName, s, null) == false)
                    MessageBox.Show("Failed to save: " + dlg.FileName);
            }
        }

        private void OnSaveFilteredLog(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Log Files|*.log|All Files|*.*";
            dlg.DefaultExt = "log";
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string s = "";
                foreach (ListViewItem lvi in lstOutput.Items)
                    s += ((Message)(lvi.Tag)).GetLine() + "\n";
                if (Disk.SaveFile(dlg.FileName, s, null) == false)
                    MessageBox.Show("Failed to save: " + dlg.FileName);
            }
        }

        private void ExecutionControlDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!debugInterfaceWillClose)
            {
                OnCloseDialog(null, null);
                e.Cancel = true;
            }
        }

        private void OnShowApplicationSettings(object sender, EventArgs e)
        {
            ShowPanel(cbApplicationSettings);
            if (lstAppSettings.Items.Count == 0)
            {
                SendMessage(ProcessCommands.GetSettings, 0);
            }
            ctrlSnapshots.RefreshSnapshotLists();
        }

        private List<VariableData> GetVariableList()
        {
            List<VariableData> l = new List<VariableData>();
            try
            {
                byte[] b = File.ReadAllBytes(FileToRun + ".data");
                int poz = 0;
                while (poz < b.Length)
                {
                    int string_sz = b[poz];
                    poz++;
                    string name = "";
                    int sz = 0;
                    while ((sz < string_sz) && (poz < b.Length))
                    {
                        if ((b[poz] > 32) && (b[poz] < 128))
                            name += (char)(b[poz]);
                        poz++;
                        sz++;
                    }
                    if ((sz < string_sz) || (poz >= b.Length))
                        throw new Exception("Premature end of the name variable !");
                    if (b[poz] != 0)
                        throw new Exception("Missing \0 character after variable name !");
                    poz++;
                    if (poz >= b.Length)
                        throw new Exception("Premature end of the name variable !");
                    byte dtype = b[poz];
                    poz++;
                    if (poz >= b.Length)
                        throw new Exception("Premature end of the variable data type !");
                    UInt32 szData = BitConverter.ToUInt32(b, poz);
                    poz += 4;
                    if (poz >= b.Length)
                        throw new Exception("Premature end of the variable value size !");
                    if (szData + poz > b.Length)
                        throw new Exception("Size of the vaariable is outside the buffer data !");
                    VariableData v = new VariableData();
                    v.Name = name;
                    v.SetTypeAndStorage(dtype);
                    if (szData > 0)
                    {
                        v.Data = new byte[szData];
                        Buffer.BlockCopy(b, poz, v.Data, 0, (int)szData);
                    }
                    poz += (int)szData;
                    l.Add(v);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                l.Sort();
            }
            return l;
        }

        private void UpdateAppSettingsListView()
        {
            List<VariableData> l = GetVariableList();
            lstAppSettings.Items.Clear();
            foreach (VariableData v in l)
            {
                ListViewItem lvi = new ListViewItem();
                v.UpdateListViewItem(lvi);
                lstAppSettings.Items.Add(lvi);
            }
        }

        private void OnStartApp(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnCreateSnapshot.Enabled = true;
            btnRestart.Enabled = true;
            btnStop.Enabled = true;
            btnSuspend.Enabled = true;
            btnPause.Enabled = true;
            btnResume.Enabled = false;
            lstAppSettings.Items.Clear();
            lstOutput.Items.Clear();
            lstAppStatus.Items.Clear();
            lstAppStatus.Groups.Clear();
            lstAnalytics.Items.Clear();
            lstGlobalCounters.Items.Clear();
            lstGlobalGroupCounters.Items.Clear();
            lstAlarms.Items.Clear();
            lbAlarmTestTime.Text = "-";
            lstAlarmEvents.Items.Clear();
            AppStatusItems.Clear();
            lstMemoryEvents.Items.Clear();
            Mem.Clear();
            MemEvents.Clear();
            DeleteMetaDataStack.Clear();
            MaxMemoryPeek = CurrentMemoryPeek = 0;
            RefreshMemoryAllocation = MemoryStatusInfo.Wait;
            analytics.Clear();
            if (RunSettings.StartSnapshot < 0)
                currentSettingsSnapshotParent = 0;
            else
                currentSettingsSnapshotParent = RunSettings.StartSnapshot;
            Worker.RunWorkerAsync();
        }

        private void OnRestartApp(object sender, EventArgs e)
        {
            OnShutDown(null, null);
            restart = true;
        }

        private void OnRefreshView(object sender, EventArgs e)
        {
            if (cbApplicationSettings.Checked)
            {
                SendMessage(ProcessCommands.GetSettings, 0);
            }
        }

        private void OnShowAppicationStatus(object sender, EventArgs e)
        {
            ShowPanel(cbApplicationStatus);
        }
        private void RemoveAppStatus(string grup, string name)
        {
            string key = grup + ":" + name;
            if (AppStatusItems.ContainsKey(key))
            {
                ListViewItem lvi = AppStatusItems[key];
                lstAppStatus.Items.Remove(lvi);
                AppStatusItems.Remove(key);
            }
        }
        private void AddAppStatus(string grup, string name, string value, string tag, Color col)
        {
            string key = grup + ":" + name;
            if (AppStatusItems.ContainsKey(key) == false)
            {
                ListViewItem lvi = new ListViewItem(name);
                ListViewGroup lvg = lstAppStatus.Groups[grup];
                if (lvg == null)
                {
                    // Scenes sa fie primul
                    lvg = new ListViewGroup(grup, grup);
                    if (grup == "Scenes")
                        lstAppStatus.Groups.Insert(0, lvg);
                    else
                        lstAppStatus.Groups.Add(lvg);
                }
                lvi.Group = lvg;
                lvi.SubItems.Add("");
                lstAppStatus.Items.Add(lvi);
                AppStatusItems[key] = lvi;
            }
            AppStatusItems[key].SubItems[1].Text = value;
            AppStatusItems[key].Tag = tag;
            AppStatusItems[key].ForeColor = col;

        }

        private void OnGoToScene(object sender, EventArgs e)
        {
            if ((lstAppStatus.SelectedItems.Count == 0) || (lstAppStatus.SelectedItems[0].Group.Header != "Scenes"))
            {
                MessageBox.Show("Please select a scene from the Application Status View first !");
                return;
            }
            ListViewItem lvi = lstAppStatus.SelectedItems[0];
            int sceneID = -1;
            if ((int.TryParse((string)(lvi.Tag), out sceneID) == false) || (sceneID < 0) || (sceneID >= 64))
            {
                MessageBox.Show("Internal error - scene ID code is " + (string)(lvi.Tag) + ". It should have been between [0..63]");
                return;
            }
            SendMessage(ProcessCommands.ChangeScene, sceneID);
        }


        private void OnSaveTexture(object sender, EventArgs earg)
        {
            if (textureBitmap == null)
            {
                MessageBox.Show("You need to take a snapshot first !");
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "PNG Files|*.png|All Files|*.*";
            dlg.DefaultExt = "png";
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    textureBitmap.Save(dlg.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to save image to " + dlg.FileName + "\n" + e.ToString());
                }
            }
        }

        private void OnViewTexture(object sender, EventArgs e)
        {
            if ((lstAppStatus.SelectedItems.Count == 0) || (lstAppStatus.SelectedItems[0].Group.Header != "Textures"))
            {
                MessageBox.Show("Please select a texture from the Application Status View first !");
                return;
            }
            ListViewItem lvi = lstAppStatus.SelectedItems[0];
            int textureID = -1;
            if ((int.TryParse((string)(lvi.Tag), out textureID) == false) || (textureID < 0))
            {
                MessageBox.Show("Internal error - Invalid texture ID :" + (string)(lvi.Tag));
                return;
            }
            SendMessage(ProcessCommands.GetTexture, textureID);
        }

        private void OnPaintTexture(object sender, PaintEventArgs e)
        {
            if (textureBitmap != null)
            {
                e.Graphics.DrawImage(textureBitmap, 0, 0, textureBitmap.Width, textureBitmap.Height);
            }
        }

        private void OnDeleteSettings(object sender, EventArgs e)
        {
            if (executionProcess != null)
            {
                MessageBox.Show("Application settings can only be deleted once the execution is stopped !");
                return;
            }
            if (MessageBox.Show("Delete application settings ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                string sfile = Path.Combine(Path.GetDirectoryName(FileToRun), "settings.dat");
                if (Disk.DeleteFile(sfile, null) == false)
                    MessageBox.Show("Failed to delete settings !");
                else
                    MessageBox.Show("Settings deleted !");
            }
        }

        private void OnCopyImage(object sender, EventArgs e)
        {
            if (screenShotBitmap == null)
            {
                MessageBox.Show("You have to take a picture first !");
                return;
            }
            Clipboard.SetImage(screenShotBitmap);
        }


        private void OnSettingsChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (CreateRunSettings() == false)
                return;
            SendMessage(ProcessCommands.ReloadExecutionSettings, 0);
        }

        private void OnTryToRestart(object sender, EventArgs e)
        {
            if ((btnStart.Enabled) && (Worker.IsBusy == false))
            {
                restartTimer.Enabled = false;
                OnStartApp(null, null);
            }
            restart = false;
        }

        private void OnShowDebugCommands(object sender, EventArgs e)
        {
            ShowPanel(cbDebugCommands);
        }

        #region DebugParams
        private void ClearDebugParams()
        {
            foreach (DebugParamControl dpc in dbgParams)
                dpc.Clear();
        }

        private void UpdateDebugCommandList()
        {
            lstDebugCommands.Items.Clear();
            prj.DebugCommands.Sort();
            ClearDebugParams();
            string filter = txDebugCommandFilter.Text.ToLower();
            for (int tr = 0; tr < prj.DebugCommands.Count; tr++)
            {
                // filter
                bool add = true;
                if (filter.Length > 0)
                {
                    add = (prj.DebugCommands[tr].Name.ToLower().Contains(filter)) || (prj.DebugCommands[tr].Description.ToLower().Contains(filter));
                }
                if (!add)
                    continue;
                ListViewItem lvi = new ListViewItem(prj.DebugCommands[tr].Name);
                lvi.SubItems.Add(prj.DebugCommands[tr].Description);
                lvi.Tag = tr;
                lstDebugCommands.Items.Add(lvi);
            }
        }

        private void OnFilterDebugCommandChanged(object sender, EventArgs e)
        {
            UpdateDebugCommandList();
        }
        private void OnSelectDebugCommand(object sender, EventArgs e)
        {
            if (lstDebugCommands.SelectedItems.Count == 0)
            {
                ClearDebugParams();
            }
            else
            {
                DebugCommand cmd = prj.DebugCommands[(int)lstDebugCommands.SelectedItems[0].Tag];
                for (int tr = 0; tr < 8; tr++)
                {
                    if (tr < cmd.Parameters.Count)
                    {
                        dbgParams[tr].SetParam(cmd.Parameters[tr]);
                    }
                    else
                    {
                        dbgParams[tr].SetParam(null);
                    }
                }
            }
        }
        private void OnRunDebugCommand(object sender, EventArgs e)
        {
            if (lstDebugCommands.SelectedItems.Count == 0)
            {
                MessageBox.Show("You have to select a command to execute !");
                return;
            }
            List<byte> data = new List<byte>();
            for (int tr = 0; tr < 8; tr++)
                if (dbgParams[tr].AddValue(data) == false)
                {
                    dbgParams[tr].Focus();
                    return;
                }
            while (data.Count < 8)
                data.Add(0);
            if (data.Count > 8)
            {
                MessageBox.Show("Internal error - more than 8 bytes allocated for params !!!");
                return;
            }
            // all si good - ii pot trimite
            byte[] arr = new byte[8];
            for (int tr = 0; tr < 8; tr++)
                arr[tr] = data[tr];
            Int32 u1 = BitConverter.ToInt32(arr, 0);
            Int32 u2 = BitConverter.ToInt32(arr, 4);
            int commandIndex = (int)lstDebugCommands.SelectedItems[0].Tag;
            SendDebugCommand((uint)commandIndex, u1, u2);
        }
        #endregion

        #region Memory Management
        class MemoryEventInfo
        {
            public bool New;
            public DateTime Date = DateTime.Now;
            public uint Address;
            public uint Size;
            public uint Array;
            public string Type;
            public string Function;
            public uint Line;
            public string Code;
            public static MemoryEventInfo CreateNew(string[] param)
            {
                if (param.Length < 8)
                {
                    MessageBox.Show("Too few params (minim required are 7) for Alloc memory data !");
                    return null;
                }
                uint address = 0;
                if (uint.TryParse(param[1], out address) == false)
                {
                    MessageBox.Show(String.Format("Invalid address ({0}). Expecting a numeric value !", param[1]));
                    return null;
                }
                uint size = 0;
                uint elemSize = 0;
                uint ln = 0;
                if (uint.TryParse(param[2], out size) == false)
                {
                    MessageBox.Show(String.Format("Invalid size ({0}). Expecting a numeric value !", param[2]));
                    return null;
                }
                if (uint.TryParse(param[3], out elemSize) == false)
                {
                    MessageBox.Show(String.Format("Invalid element size ({0}). Expecting a numeric value !", param[3]));
                    return null;
                }
                if (uint.TryParse(param[4], out ln) == false)
                {
                    MessageBox.Show(String.Format("Invalid line ({0}). Expecting a numeric value !", param[4]));
                    return null;
                }
                if (size == 0)
                {
                    MessageBox.Show(String.Format("Invalid size ({0}). Expecting a number bigger than 0", param[2]));
                    return null;
                }
                if (elemSize == 0)
                {
                    MessageBox.Show(String.Format("Invalid element size ({0}). Expecting a number bigger than 0", param[3]));
                    return null;
                }
                if ((elemSize > size) || ((size % elemSize) != 0))
                {
                    MessageBox.Show(String.Format("Invalid size({0}) and element size({1}). Size should be a multiple of element size.\nFunction:{2}\nLine:{3}\nType:{4}\nCode:{5}", size, elemSize, param[6], ln, param[5], param[7]));
                    return null;
                }
                // all is ok
                MemoryEventInfo mi = new MemoryEventInfo();
                mi.Address = address;
                mi.Line = ln;
                mi.Function = param[6];
                mi.Type = param[5];
                mi.Code = param[7];
                if (mi.Type.Contains("[]"))
                    mi.Array = (size / elemSize);
                else
                    mi.Array = 0;
                mi.Size = size;
                if (param.Length > 8)
                {
                    for (int tr = 8; tr < param.Length; tr++)
                    {
                        if (tr + 1 < param.Length)
                            mi.Code += "|";
                        mi.Code += param[tr];
                    }
                }
                mi.New = true;
                return mi;
            }
            public static MemoryEventInfo CreateDelete(string[] param, SortedDictionary<uint, MemoryEventInfo> mem, DeleteMetaData dmd)
            {
                if (param.Length != 2)
                {
                    MessageBox.Show("Delete message only has 2 paramaters");
                    return null;
                }
                uint address = 0;
                if (uint.TryParse(param[1], out address) == false)
                {
                    MessageBox.Show(String.Format("Invalid address ({0}). Expecting a numeric value !", param[1]));
                    return null;
                }
                MemoryEventInfo mi = new MemoryEventInfo();
                mi.New = false;
                mi.Function = dmd.Function;
                mi.Code = dmd.Code;
                mi.Line = dmd.Line;
                mi.Array = 0;
                mi.Size = 0;
                mi.Address = address;
                if (mem.ContainsKey(address))
                {
                    mi.Type = mem[address].Type;
                    mi.Size = mem[address].Size;
                    mi.Array = mem[address].Array;
                }
                return mi;
            }
        }
        private class DeleteMetaData
        {
            public string Function;
            public uint Line;
            public string Code;
            public static DeleteMetaData Create(string[] param)
            {
                if (param.Length < 4)
                {
                    MessageBox.Show("Too few params (minim required are 3) for delete meta data!");
                    return null;
                }
                uint ln = 0;
                if (uint.TryParse(param[2], out ln) == false)
                {
                    MessageBox.Show(String.Format("Invalid line ({0}). Expecting a numeric value !", param[4]));
                    return null;
                }
                DeleteMetaData mi = new DeleteMetaData();
                mi.Line = ln;
                mi.Function = param[1];
                mi.Code = param[3];
                if (param.Length > 4)
                {
                    for (int tr = 4; tr < param.Length; tr++)
                    {
                        if (tr + 1 < param.Length)
                            mi.Code += "|";
                        mi.Code += param[tr];
                    }
                }
                return mi;
            }
        }
        private void OnShowMemory(object sender, EventArgs e)
        {
            ShowPanel(cbMemory);
        }
        private void OnNewMemoryAllocated(string[] param)
        {
            MemoryEventInfo mi = MemoryEventInfo.CreateNew(param);
            if (mi != null)
            {
                Mem[mi.Address] = mi;
                MemEvents.Add(mi);
                CurrentMemoryPeek += mi.Size;
                MaxMemoryPeek = Math.Max(CurrentMemoryPeek, MaxMemoryPeek);
                RefreshMemoryAllocation = MemoryStatusInfo.Wait;
            }
        }
        private void OnMemoryDeAllocated(string[] param)
        {
            MemoryEventInfo mi = MemoryEventInfo.CreateDelete(param, Mem, DeleteMetaDataStack.Pop());
            if (mi != null)
            {
                if (Mem.ContainsKey(mi.Address))
                    Mem.Remove(mi.Address);
                if (mi.Size <= CurrentMemoryPeek)
                    CurrentMemoryPeek -= mi.Size;
                else
                    CurrentMemoryPeek = 0;
                MemEvents.Add(mi);
                RefreshMemoryAllocation = MemoryStatusInfo.Wait;
            }
        }
        private void AddDeleteMetaData(string[] param)
        {
            DeleteMetaData di = DeleteMetaData.Create(param);
            if (di != null)
            {
                DeleteMetaDataStack.Push(di);
            }
        }


        private void RefreshMemory()
        {
            lstMemory.BeginUpdate();
            lstMemory.Items.Clear();

            uint total = 0;
            foreach (uint addr in Mem.Keys)
            {
                ListViewItem lvi = new ListViewItem(addr.ToString("X8"));
                MemoryEventInfo mi = Mem[addr];
                lvi.SubItems.Add(mi.Size.ToString());
                lvi.SubItems.Add(mi.Type.ToString());
                lvi.SubItems.Add(mi.Function);
                lvi.SubItems.Add(mi.Code);
                lstMemory.Items.Add(lvi);
                total += mi.Size;
            }
            lstMemoryStats.Items[0].SubItems[1].Text = total.ToString();
            if (total > MaxMemoryPeek)
                MaxMemoryPeek = total;
            lstMemoryStats.Items[1].SubItems[1].Text = MaxMemoryPeek.ToString();
            lstMemoryStats.Items[2].SubItems[1].Text = Mem.Count.ToString();
            lstMemory.EndUpdate();
        }
        private void RefreshMemoryEventsList()
        {
            lstMemoryEvents.BeginUpdate();
            lstMemoryEvents.Items.Clear();
            string ss = txMemoryFilter.Text.ToLower();

            foreach (MemoryEventInfo mi in MemEvents)
            {
                if ((cbShowMemoryNewEvents.Checked == false) && (mi.New == true))
                    continue;
                if ((cbShowMemoryDeleteEvents.Checked == false) && (mi.New == false))
                    continue;
                if (ss.Length > 0)
                {
                    bool add = false;
                    if ((!add) && (mi.Type.ToLower().Contains(ss)))
                        add = true;
                    if ((!add) && (mi.Function.ToLower().Contains(ss)))
                        add = true;
                    if ((!add) && (mi.Code.ToLower().Contains(ss)))
                        add = true;
                    if (!add)
                        continue;
                }
                ListViewItem lvi = new ListViewItem(mi.Date.ToString());
                lvi.Tag = mi;
                if (mi.New)
                    lvi.SubItems.Add("New");
                else
                    lvi.SubItems.Add("Delete");
                lvi.SubItems.Add("0x" + mi.Address.ToString("X8"));
                lvi.SubItems.Add(mi.Type);
                lvi.SubItems.Add(mi.Size.ToString());
                if (mi.Array > 0)
                    lvi.SubItems.Add(mi.Array.ToString());
                else
                    lvi.SubItems.Add("-");
                lvi.SubItems.Add(mi.Function);
                lvi.SubItems.Add(mi.Line.ToString());
                lvi.SubItems.Add(mi.Code);
                if (mi.Address == 0)
                {
                    lvi.ForeColor = Color.Red;
                    lvi.ToolTipText = "Memory was not allocated !";
                }
                if ((mi.Type == null) || (mi.Type.Length == 0))
                {
                    lvi.ForeColor = Color.Red;
                    lvi.ToolTipText = "Invalid address - it was never allocated !";
                }
                lstMemoryEvents.Items.Add(lvi);
                lvi.EnsureVisible();
            }
            lstMemoryEvents.EndUpdate();
            lbMemoryEventsInfo.Text = lstMemoryEvents.Items.Count.ToString() + " / " + MemEvents.Count.ToString();
        }
        private void OnRefreshMemory(object sender, EventArgs e)
        {
            if (RefreshMemoryAllocation == MemoryStatusInfo.Completed)
                return;
            if (RefreshMemoryAllocation == MemoryStatusInfo.Update)
            {
                RefreshMemory();
                RefreshMemoryEventsList();
                RefreshMemoryAllocation = MemoryStatusInfo.Completed;
            }
            if (RefreshMemoryAllocation == MemoryStatusInfo.Wait)
            {
                RefreshMemoryAllocation = MemoryStatusInfo.Update;
                return;
            }
            // fac upate

        }
        private void OnClearEventsList(object sender, EventArgs e)
        {
            MemEvents.Clear();
            RefreshMemoryEventsList();
        }

        private void OnChangeMemoryEventsFilter(object sender, EventArgs e)
        {
            RefreshMemoryEventsList();
        }
        #endregion

        private void OnPinWindow(object sender, EventArgs e)
        {
            this.TopMost = btnPin.Checked;
        }

        private void OnCreateSnapshot(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Snapshot information", "");
            ib.TopMost = true;
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lastSettingsSnapshotID = prj.SettingsSnapshots.GetNewID();
                lastSettingsSnapshotMessage = ib.StringResult;
                SendMessage(ProcessCommands.CreateNewSnapshot, lastSettingsSnapshotID);
            }
        }

        private void OnStartFfromASnapshot(object sender)
        {
            RunSettings.StartSnapshot = ctrlSnapshots.GetSelectedSnapshot();
            if (btnStart.Enabled)
                OnStartApp(null, null);
            else if (btnRestart.Enabled)
                OnRestartApp(null, null);

        }

        private void OnShowAnalyticsEmulator(object sender, EventArgs e)
        {
            ShowPanel(cbAnalyticsEmulator);
        }

        private void OnAnalytics(string name, string value)
        {
            AnalyticsInfo ai;
            if (analytics.ContainsKey(name) == false)
            {
                ai = new AnalyticsInfo();
                ListViewItem lvi = new ListViewItem(name);
                lstAnalytics.Items.Add(lvi);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lstAnalytics.Sort();
                ai.itm = lvi;
                analytics[name] = ai;
            }
            // update la valoare
            ai = analytics[name];
            ai.Count++;
            ai.itm.SubItems[1].Text = ai.Count.ToString();
            ai.itm.SubItems[4].Text = value;
            if (value.Contains("."))
            {
                double d = 0;
                if (double.TryParse(value, out d))
                {
                    ai.dblSum += d;
                }
                ai.itm.SubItems[2].Text = ai.dblSum.ToString();
                ai.itm.SubItems[3].Text = (ai.dblSum / ai.Count).ToString();
            }
            else
            {
                Int64 d = 0;
                if (Int64.TryParse(value, out d))
                {
                    ai.intSum += d;
                }
                ai.itm.SubItems[2].Text = ai.intSum.ToString();
                ai.itm.SubItems[3].Text = (ai.intSum / ai.Count).ToString();
            }
        }

        #region Global Counters and Timers

        private void OnShowGlobalTimersAndCounters(object sender, EventArgs e)
        {
            ShowPanel(cbAlarmsAndCounters);
        }

        private void OnGlobalCounterUpdate(string[] param)
        {
            if ((param == null) || (param.Length != 11))
            {
                MessageBox.Show("Invalid number of parameters for Global Counter update message !");
                return;
            }
            int index = 0;
            if (int.TryParse(param[1], out index) == false)
            {
                MessageBox.Show("Invalid index '" + param[1] + "' for Global Counter update message !");
                return;
            }

            // daca nu am itemruile - le creez
            ListViewItem lvi;
            while (lstGlobalCounters.Items.Count <= index)
            {
                lvi = new ListViewItem();
                lvi.ForeColor = Color.Gray;
                lvi.Tag = 2; // nu stiu starea de check sau non-check
                for (int tr = 0; tr < lstGlobalCounters.Columns.Count; tr++)
                    lvi.SubItems.Add("");
                lstGlobalCounters.Items.Add(lvi);
            }
            // updatez
            lvi = lstGlobalCounters.Items[index];
            lvi.Text = param[2];
            lvi.SubItems[1].Text = param[3];
            lvi.SubItems[2].Text = param[4] + " / " + param[5];
            if (param[7] == "0")
                lvi.SubItems[3].Text = param[6];
            else
                lvi.SubItems[3].Text = param[6] + " / " + param[7];
            if (param[8] != "0")
            {
                lvi.Tag = 1;
                lvi.Checked = true;
                lvi.ForeColor = Color.Black;
            }
            else
            {
                lvi.Tag = 0;
                lvi.Checked = false;
                lvi.ForeColor = Color.Red;
            }

            if (param[9] == "1")
                lvi.SubItems[4].Text = "Persistent";
            else
                lvi.SubItems[4].Text = "-";

            lvi.SubItems[5].Text = param[10];

        }
        private void OnGlobalCounterGroupsUpdate(string[] param)
        {
            if ((param == null) || (param.Length != 7))
            {
                MessageBox.Show("Invalid number of parameters for Global Counter Group update message !");
                return;
            }
            int index = 0;
            if (int.TryParse(param[1], out index) == false)
            {
                MessageBox.Show("Invalid index '" + param[1] + "' for Global Counter Group update message !");
                return;
            }
            if (index > 32)
            {
                MessageBox.Show("Invalid index '" + index.ToString() + "' for Global Counter update message ! (should be less than 32)");
                return;
            }
            // daca nu am itemruile - le creez
            ListViewItem lvi;
            while (lstGlobalGroupCounters.Items.Count <= index)
            {
                lvi = new ListViewItem();
                for (int tr = 0; tr < lstGlobalGroupCounters.Columns.Count; tr++)
                    lvi.SubItems.Add("");
                lstGlobalGroupCounters.Items.Add(lvi);
            }
            // updatez
            bool disabled = false;
            lvi = lstGlobalGroupCounters.Items[index];
            lvi.Text = param[2];
            // skip count
            if (param[3] == "0")
                lvi.SubItems[1].Text = "-";
            else
            {
                lvi.SubItems[1].Text = param[3];
                disabled = true;
            }
            // timer-ul
            if (param[4] == "0")
            {
                lvi.SubItems[2].Text = "Stopped";
                disabled = true;
            }
            else
            {
                int cs = 0, tt = 0;
                if ((int.TryParse(param[6], out cs) == false) || (int.TryParse(param[5], out tt) == false))
                {
                    lvi.SubItems[2].Text = "?";
                }
                else
                {
                    lvi.SubItems[2].Text = (cs / 1000).ToString() + "/" + (tt / 1000).ToString();
                }
            }
            if (disabled)
                lvi.ForeColor = Color.Red;
            else
                lvi.ForeColor = Color.Black;
        }
        private void lstGlobalCounters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int status = (int)lstGlobalCounters.Items[e.Index].Tag;
            if (status == 2)
                return;
            if ((status == 1) && (e.NewValue == CheckState.Checked))
                return;
            if ((status == 0) && (e.NewValue == CheckState.Unchecked))
                return;
            if (e.NewValue == CheckState.Checked)
                SendMessage(ProcessCommands.EnableCounter, e.Index);
            else
                SendMessage(ProcessCommands.DisableCounter, e.Index);

        }

        #endregion

        #region Alarms
        private void OnAlarm(string[] param)
        {
            if ((param == null) || (param.Length != 4))
            {
                MessageBox.Show("Invalid number of parameters for alarm message !");
                return;
            }
            int index = 0;
            if (int.TryParse(param[1], out index) == false)
            {
                MessageBox.Show("Invalid index '" + param[1] + "' for Alarm message !");
                return;
            }
            int uniqueID = 0;
            if (int.TryParse(param[2], out uniqueID) == false)
            {
                MessageBox.Show("Invalid unique ID '" + param[2] + "' for Alarm message !");
                return;
            }
            Alarm a = null;
            foreach (var al in prj.Alarms)
                if (al.UniqueID == uniqueID)
                {
                    a = al;
                    break;
                }
            if (a == null)
            {
                MessageBox.Show("Unable to find an alarm with " + uniqueID.ToString() + " unique ID. Recompile project !");
                return;
            }
            // daca nu am itemruile - le creez
            ListViewItem lvi;
            while (lstAlarms.Items.Count <= index)
            {
                lvi = new ListViewItem();
                for (int tr = 0; tr < lstAlarms.Columns.Count; tr++)
                    lvi.SubItems.Add("");
                lstAlarms.Items.Add(lvi);
            }
            lvi = lstAlarms.Items[index];
            lvi.Tag = a;
            lvi.Text = a.propName;
            // enabled sau nu
            if (param[3] == "0")
            {
                //lvi.Checked = false;
                lvi.ForeColor = Color.Red;
            }
            else
            {
                //lvi.Checked = true;
                lvi.ForeColor = Color.Black;
            }
            // tipul de alarma
            lvi.SubItems[1].Text = a.propDescription;
            // cat dureaza
            lvi.SubItems[2].Text = a.propDuration;

        }
        private void OnAlarmTest(string[] param)
        {
            if ((param == null) || (param.Length != 2))
            {
                MessageBox.Show("Invalid number of parameters for alarm test message !");
                return;
            }
            lbAlarmTestTime.Text = param[1];
        }
        private void OnAlarmEvent(string[] param)
        {
            if ((param == null) || (param.Length != 5))
            {
                MessageBox.Show("Invalid number of parameters for alarm event message !");
                return;
            }
            int index = 0;
            if (int.TryParse(param[2], out index) == false)
            {
                MessageBox.Show("Invalid index '" + param[1] + "' for Alarm Event message !");
                return;
            }
            int state = 0;
            if (int.TryParse(param[4], out state) == false)
            {
                MessageBox.Show("Invalid state '" + param[4] + "' for Alarm Event message !");
                return;
            }
            if ((index<0) || (index>=lstAlarms.Items.Count))
            {
                MessageBox.Show("Invalid index '" + param[1] + "' for Alarm Event message ! (should be between 0 and "+lstAlarms.Items.Count.ToString()+")");
                return;
            }
            Alarm a = (Alarm)lstAlarms.Items[index].Tag;
            if (a==null)
            {
                MessageBox.Show("Invalid index '" + param[1] + "' for Alarm Event message ! (content is null)");
                return;
            }
            if ((state & 2) != 0)
            {
                ListViewItem lvi = new ListViewItem(param[1]);
                lvi.SubItems.Add("[CLOSED ] " + a.Name);
                lstAlarmEvents.Items.Add(lvi);
                lvi.EnsureVisible();
            }
            if ((state & 1) != 0)
            {
                ListViewItem lvi = new ListViewItem(param[1]);
                lvi.SubItems.Add("[TRIGGER] "+a.Name + " -> " + param[3]);
                lstAlarmEvents.Items.Add(lvi);
                lvi.EnsureVisible();
            }

        }
        private void OnForceRecheckAlarms(object sender, EventArgs e)
        {
            SendMessage(ProcessCommands.RecheckAlarms, 0);
        }
        private void OnSetDateTime(object sender, EventArgs e)
        {
            DateTimeDialog dtd = new DateTimeDialog();
            if (dtd.ShowDialog() == DialogResult.OK)
            {
                TimeSpan result = dtd.CurrentDateTime.Subtract(DateTime.Now);
                SendMessage(ProcessCommands.UpdateTimeDelta, ((int)result.TotalSeconds));
            }
        }

        #endregion

    }
}
