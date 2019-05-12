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
using System.Reflection;
using System.Management;
using System.Runtime.InteropServices;

namespace GAppCreator
{
    public partial class Form1 : Form,ProjectContext.CommonFunctions
    {

        ProjectContext Context = new ProjectContext();
        Project prj = null;
       

        public Form1()
        {
            InitializeComponent();

            // initalizez Contextul
            Context.LabelGeneralInfo = lbGeneralInfo;
            Context.LabelInfo = lbInfo;
            Context.BackgroundTaskList = null;
            Context.SmallIcons = new ImageList();
            Context.SmallIcons.ImageSize = new System.Drawing.Size(16, 16);
            Context.SmallIcons.ColorDepth = ColorDepth.Depth32Bit;
            Context.LargeIcons = new ImageList();
            Context.LargeIcons.ImageSize = new System.Drawing.Size(64, 64);
            Context.LargeIcons.ColorDepth = ColorDepth.Depth32Bit;
            Context.Functions = this;

            // initializez controalele pentru tab-urile principale
            tabProjectMain.SetContext(Context);
            tabProjectStrings.SetContext(Context);
            tabProjectConstants.SetContext(Context);
            tabProjectAds.SetContext(Context);
            tabProjectMemoryProfiles.SetContext(Context);
            tabProjectResources.SetContext(Context);
            tabProjectEditor.SetContext(Context);
            tabProjectPublishMaterials.SetContext(Context);
            tabProjectCountersAndTimers.SetContext(Context);
            tabProjectAnimationObjects.SetContext(Context);

            CheckSecID();

            mainTab.Dock = DockStyle.Fill;
            mainTab.Visible = false;

            InitMainScreen();


            Project.defaultLocale = new System.Globalization.CultureInfo("en-us");
            System.Threading.Thread.CurrentThread.CurrentCulture = Project.defaultLocale;
            Context.Settings = SystemSettings.Load();
            Git.SetGitExecutable(Context.Settings.GitPath);



            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Context.Task = new BackgroundTask(OnTaskEvent, OnTaskEnds, OnTaskStarts);

            this.Text = fvi.ProductName + " [" + fvi.FileVersion + "]";


            tabProjectResources.InitDefaultResourceIcons();
            //tabProjectResources.UpdateResourceInformations();

            mainTab.Enabled = false;

            // verific daca am minimul de chestii setate
            string sets = "";
            if ((Context.Settings.BuildServerAddress.Length == 0) || (Context.Settings.UserName.Length == 0) || (Context.Settings.Password.Length == 0))
                sets += "* Build server credentials (address, username, password)\n";
            if ((Context.Settings.CL.Length == 0) || (Context.Settings.VSVarsBat.Length == 0))
                sets += "* Visual Studio Framework (cl Compiler and vsvars.bat)\n";
            if (Context.Settings.InskapePath.Length == 0)
                sets += "* Inkscape image editor\n";
            if (sets.Length > 0)
            {
                MessageBox.Show("One of the following settings is missing. Without it GACCreator can not function properly !\n" + sets, "Settings");
                OnShowSettings(null, null);
            }

            GACParser.LoadGacDefinitions(Application.ExecutablePath + ".gacxml");

            string[] args = Environment.GetCommandLineArgs();
            /*
            string s = "";
            foreach (string ss in args)
                s = s + ss + "\r\n";
            MessageBox.Show(s);
            */
            if (args.Length==2)
            {
                if (args[1].ToLower().EndsWith("project.gappcreator")==false)
                {
                    MessageBox.Show("Expecting a 'project.gappcreator' file !");
                }
                else
                {
                    LoadProject(args[1],false);
                }
            }

            //ConstantEditValueDialog dlg = new ConstantEditValueDialog(true, true, "", "");
            //dlg.ShowDialog();

            //ControlIDEditor dlg = new ControlIDEditor(new Project());
            //dlg.ShowDialog();

            //ConstantEditorDialog dlg = new ConstantEditorDialog();
            //dlg.ShowDialog();

            /*
            AnimationObjectEditor dlg = new AnimationObjectEditor();
            dlg.ShowDialog();
            //this.Close();
            //*/
            //this.Close();

        }

        #region Main Screen
        private List<string> GetMostRecentProjects()
        {
            List<string> ls = new List<string>();
            string s = Disk.ReadFileAsString(Application.ExecutablePath + ".mru", null);
            if (s != null)
            {
                string[] l = s.Split('\n');
                if (l != null)
                {
                    foreach (string n in l)
                    {
                        if (n.Trim().Length > 0)
                            ls.Add(n.Trim());
                    }
                }
            }
            return ls;
        }
        private Image GetMRUProjectIcon(string path)
        {
            return Project.LoadImage(Project.GetProjectResourceOutputFullPath(path, "main_icon_96x96.png"));
        }
        private void UpdateMRUList(string path)
        {
            List<string> l = GetMostRecentProjects();
            int found = -1;
            int index = 0;
            foreach (string s in l)
            {
                if (s.Trim().ToLower().Equals(path.Trim().ToLower()))
                    found = index;
                index += 1;
            }
            if (found != -1)
                l.RemoveAt(found);

            l.Insert(0, path.Trim());
            string res_mru = "";
            foreach (string p in l)
                res_mru += p + "\n";
            Disk.SaveFile(Application.ExecutablePath + ".mru", res_mru, null);
        }
        private string GetMRUProjectName(string path)
        {
            path = path.Trim();
            if (path.EndsWith("\\"))
                return Path.GetFileName(path.Substring(0, path.Length - 1));
            else
                return Path.GetFileName(path);
        }
        void InitMainScreen()
        {
            pnlMainScreen.Dock = DockStyle.Fill;
            pnlMainScreen.Visible = true;
            lstMainScreenOperation.Items[0].Tag = -2;
            lstMainScreenOperation.Items[1].Tag = -1;
            List<string> mru = GetMostRecentProjects();
            int index = 0;
            foreach (string path in mru)
            {
                Image icon = GetMRUProjectIcon(path);
                string name = GetMRUProjectName(path);
                if ((icon != null) && (name.Length > 0))
                {
                    iconMainScreen.Images.Add(index.ToString(), icon);
                    ListViewItem lvi = new ListViewItem(name);
                    lvi.SubItems.Add(path);
                    lvi.ImageKey = index.ToString();
                    lvi.Tag = 0;
                    lstMainScreenOperation.Items.Add(lvi);
                    if (index<3)
                        lvi.Group = lstMainScreenOperation.Groups[1];
                    else
                        lvi.Group = lstMainScreenOperation.Groups[2];
                }
                index++;
            }
        }
        private void OnMouseDblClickedOnMainScreen(object sender, MouseEventArgs e)
        {
            if (lstMainScreenOperation.SelectedItems.Count == 1)
            {
                int index = (int)lstMainScreenOperation.SelectedItems[0].Tag;
                if (index == -2)
                    OnCreateNewProject(null, null);
                if (index == -1)
                    OnLoadProject(null, null);
                if (index == 0)
                    LoadProject(Path.Combine(lstMainScreenOperation.SelectedItems[0].SubItems[1].Text, "project.gappcreator"), false);
            }
        }
        #endregion

        #region Load and resource load and building asyncrone functiones



        public void Task_NewProject()
        {
            tabProjectMain.Task_BuildAllIcons();
            tabProjectMain.Task_BuildSplashScreen();
            Task_LoadProject();
        }
        public void Task_LoadProject()
        {           
            tabProjectResources.Task_LoadResources();
            if (Context.IDESettings.EnableCodeTab)
            {
                Context.Task.SetMinMax(0, prj.Files.Count);
                Context.Task.CreateSubTask("Loading files ...");
                foreach (ProjectFile pf in prj.Files)
                {
                    if (pf.Opened)
                        Context.Task.SendCommand(Command.OpenSourceCodeFile, pf);
                    Context.Task.IncrementProgress();
                }
                Context.Task.UpdateSuccessErrorState(!prj.EC.HasErrors());
            }
            
        }


        #endregion

        #region Task functions

        void AddEventToTaskList(string text, TaskEventState tes)
        {
            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString());
            lvi.ImageKey = tes.ToString();
            lvi.SubItems.Add(text);
            lvi.ToolTipText = text;
            lvi.Tag = tes;
            if (tes == TaskEventState.Error)
                lvi.ForeColor = Color.Red;
            UpdateTaskListState(TaskEventState.Success);
            Context.BackgroundTaskList.Items.Add(lvi);
            lvi.EnsureVisible();
        }
        void UpdateTaskListState(TaskEventState newtes)
        {
            // caut ultimul item care este Running si il setez
            for (int tr = Context.BackgroundTaskList.Items.Count - 1; tr >= 0; tr--)
            {
                TaskEventState tes = (TaskEventState)Context.BackgroundTaskList.Items[tr].Tag;
                if ((tes == TaskEventState.Success) || (tes == TaskEventState.Error))
                    break;
                if (tes == TaskEventState.Running)
                {
                    Context.BackgroundTaskList.Items[tr].Tag = newtes;
                    Context.BackgroundTaskList.Items[tr].ImageKey = newtes.ToString();
                    if (newtes == TaskEventState.Error)
                        Context.BackgroundTaskList.Items[tr].ForeColor = Color.Red;
                    else
                        Context.BackgroundTaskList.Items[tr].ForeColor = Color.Black;
                    return;
                }
            }
        }
        void OnTaskStarts(string name)
        {
            lbTaskName.Text = name;
            lbTaskName.Visible = true;
            lbTaskProgress.Visible = true;
            lbTaskProgress.Minimum = 0;
            lbTaskProgress.Maximum = 0;
            lbTaskProgress.Value = 0;
            Context.BackgroundTaskList.Items.Clear();
        }
        void OnTaskEvent(object sender, ProgressChangedEventArgs e)
        {
            BackgroundTaskAction bta = (BackgroundTaskAction)e.UserState;
            switch (bta.Type)
            {
                case BackgroundTaskType.SetMinMax:
                    lbTaskProgress.Minimum = bta.Min;
                    lbTaskProgress.Maximum = bta.Max;
                    lbTaskProgress.Value = bta.Min;
                    break;
                case BackgroundTaskType.UpdateProgress:
                    if ((bta.Value >= lbTaskProgress.Minimum) && (bta.Value < lbTaskProgress.Maximum))
                        lbTaskProgress.Value = bta.Value;
                    break;
                case BackgroundTaskType.Info:
                case BackgroundTaskType.CreateSubTask:
                    AddEventToTaskList(bta.Content, bta.Tes);
                    break;
                case BackgroundTaskType.UpdateTaskState:
                    UpdateTaskListState(bta.Tes);
                    break;
                case BackgroundTaskType.Command:
                    switch (bta.Command)
                    {
                        case Command.UpdateApplicationIcon:
                        case Command.UpdateApplicationSplashScreen:
                            tabProjectMain.OnCommand(bta.Command, bta);
                            break;
                        case Command.DisableResourceList:
                        case Command.EnableResourceList:
                        case Command.UpdatePreview:
                        case Command.UpdateResourceIcon:
                        case Command.UpdateResourceList:
                            tabProjectResources.OnCommand(bta.Command, bta);
                            break;
                        case Command.OpenSourceCodeFile:
                        case Command.AddCompileError:
                        case Command.AddIntelliSenseError:
                        case Command.AddCompileOutput:
                        case Command.UpdateGlobalAutoComplete:
                        case Command.UpdateGacFileList:
                        case Command.UpdateLocalTypes:
                        case Command.EnableIntelliSenseTimer:
                        case Command.RunFile:
                            tabProjectEditor.OnCommand(bta.Command, bta);
                            break;
                    };
                    break;
            }
            Context.Task.Done();
        }
        void OnTaskEnds(object sender, RunWorkerCompletedEventArgs e)
        {
            lbTaskName.Visible = false;
            lbTaskProgress.Visible = false;
            mainTab.Enabled = true;
            prj.ShowErrors();
        }
        #endregion


        #region Project
        public static void ShowError(ErrorsContainer EC)
        {
            ErrorViewerDialog dlg = new ErrorViewerDialog(EC);
            dlg.ShowDialog();
            EC.Reset();
        }

        private void CreateNewProject()
        {
            NewProjectDialog npd = new NewProjectDialog(GetMostRecentProjects());
            if (npd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OnSaveProject(null, null);
                LoadProject(npd.resXmlFile, true);
            }
        }
        private void OnCreateNewProject(object sender, EventArgs e)
        {
            CreateNewProject();
        }
        private void OnLoadProject(object sender, EventArgs e)
        {
            OnSaveProject(null, null);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Project files|project.gappcreator|All files|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                LoadProject(dlg.FileName, false);
        }
        private void OnSaveProject(object sender, EventArgs e)
        {
            Context.Functions.SaveProject();
        }
        #endregion

        
        private void OnShowSettings(object sender, EventArgs e)
        {
            SettingsDialog dlg = new SettingsDialog(Context.Settings);
            dlg.ShowDialog();
            Git.SetGitExecutable(Context.Settings.GitPath);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //LoadProject(@"E:\a\rm\Untangle\resources.resxml");
            //LoadProject(@"E:\a\rm\test\project.gappcreator");
        }


        #region About
        private void OnShowAboutInformations(object sender, EventArgs e)
        {
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string s = string.Format("Product: {0}\nCompany: {1}\nVersion: {2}\nCopyright: {3}", fvi.ProductName, fvi.CompanyName, fvi.FileVersion, fvi.LegalCopyright);
            MessageBox.Show(s);
        }
        #endregion


        #region SecurityCheck
        private byte[] ComputeSysInfo()
        {
            string s = "";

            s += String.Format("{0}\n", System.Environment.Is64BitOperatingSystem);
            try
            {
                s += String.Format("{0}\n", System.Environment.MachineName);
            }
            catch (Exception)
            {
                s += "\n";
            }
            s += String.Format("{0}\n", System.Environment.ProcessorCount);
            s += String.Format("{0}\n", System.Environment.SystemPageSize);
            s += String.Format("{0}\n", System.Environment.UserName);


            try
            {
                Microsoft.Win32.RegistryKey processor_name = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree);
                if (processor_name != null)
                {
                    object o = processor_name.GetValue("ProcessorNameString");
                    if (o != null)
                        s += o.ToString() + "\n";
                }
            }
            catch (Exception)
            {
                s += "\n";
            }
            string[] drv = null;
            try
            {
                drv = System.Environment.GetLogicalDrives();
            }
            catch (Exception)
            {
                s += "\n";
            }
            if (drv != null)
            {
                foreach (string d in drv)
                {
                    System.IO.DriveInfo di = new System.IO.DriveInfo(d);
                    if (di.DriveType == DriveType.Fixed)
                    {
                        try
                        {
                            s += String.Format("{0}-{1}", di.DriveType, di.Name);
                            s += String.Format("{0},", di.VolumeLabel);
                            s += String.Format("{0},", di.TotalSize);
                            s += String.Format("{0},", di.DriveFormat);
                        }
                        catch (Exception) { }
                    }
                    s += "\n";
                }
            }

            try
            {
                System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher("SELECT Product, SerialNumber FROM Win32_BaseBoard");
                System.Management.ManagementObjectCollection information = searcher.Get();
                foreach (System.Management.ManagementObject obj in information)
                {
                    foreach (System.Management.PropertyData data in obj.Properties)
                        s += String.Format("{0}-{1}\n", data.Name, data.Value);
                }
                searcher.Dispose();
            }
            catch (Exception)
            {
                s += "\n";
            }
            //MessageBox.Show(s);
            System.Security.Cryptography.MD5 m = System.Security.Cryptography.MD5.Create();
            byte[] hash = m.ComputeHash(System.Text.Encoding.ASCII.GetBytes(s));
            return hash;
        }
        private bool hashEQ(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                return false;
            for (int tr = 0; tr < b1.Length; tr++)
                if (b1[tr] != b2[tr])
                    return false;
            return true;
        }
        private void CheckSecID()
        {
            return;
            byte[] b = ComputeSysInfo();
            //GDT
            if (hashEQ(b, new byte[] { 0xFA, 0xCA, 0x2F, 0x74, 0x90, 0xC3, 0x10, 0xD1, 0x63, 0xD1, 0xB5, 0x48, 0x48, 0x4C, 0x0C, 0xA4 }) == true)
                return;
            if (hashEQ(b, new byte[] { 0x99, 0x2F, 0x8C, 0xA0, 0x8D, 0xA7, 0xA4, 0xF1, 0x95, 0x06, 0xB0, 0x8E, 0x2F, 0x71, 0x05, 0xFF }) == true)
                return;
            //GAM
            if (hashEQ(b, new byte[] { 0x52, 0xf5, 0xe6, 0xef, 0x08, 0x4d, 0x6c, 0xa3, 0x13, 0x98, 0x01, 0x16, 0x11, 0xb3, 0x72, 0x3b }) == true)
                return;
            //GAM
            if (hashEQ(b, new byte[] { 0xCB, 0xA4, 0x6C, 0x52, 0x8B, 0x23, 0xA0, 0xE7, 0xA4, 0x9B, 0xD0, 0x35, 0x9D, 0xAB, 0x00, 0x3D }) == true)
                return;
            //NRB
            if (hashEQ(b, new byte[] { 0xB4, 0x2D, 0x07, 0x8F, 0xFE, 0x7E, 0x91, 0x5F, 0x84, 0x2C, 0xCE, 0xFE, 0x0B, 0x12, 0x19, 0xF8 }) == true)
                return;
            //ANDRA 91DD65617444A2183FC7A4CA25D89282 
            if (hashEQ(b, new byte[] { 0x91, 0xDD, 0x65, 0x61, 0x74, 0x44, 0xA2, 0x18, 0x3F, 0xC7, 0xA4, 0xCA, 0x25, 0xD8, 0x92, 0x82 }) == true)
                return;
            //DIANA 91DD65617444A2183FC7A4CA25D89282 
            if (hashEQ(b, new byte[] { 0xEE, 0x90, 0x72, 0x97, 0x2C, 0x7A, 0xCB, 0x3D, 0x45, 0x75, 0xD4, 0x8A, 0x4D, 0x6A, 0x19, 0xFE }) == true)
                return;
            //Andrei FE A7 3B 98 BF 0B DD 16 38 E6 E4 AE F6 76 46 15 
            //Andrei DE 0D 91 1E 6E EB 38 6A 0D DC F4 24 DF 4F 3E 95
            if (hashEQ(b, new byte[] { 0xFE, 0xA7, 0x3B, 0x98, 0xBF, 0x0B, 0xDD, 0x16, 0x38, 0xE6, 0xE4, 0xAE, 0xF6, 0x76, 0x46, 0x15 }) == true)
                return;
            if (hashEQ(b, new byte[] { 0xDE, 0x0D, 0x91, 0x1E, 0x6E, 0xEB, 0x38, 0x6A, 0x0D, 0xDC, 0xF4, 0x24, 0xDF, 0x4F, 0x3E, 0x95 }) == true)
                return;
            //DIANA
            if (hashEQ(b, new byte[] { 0xAC, 0x24, 0x47, 0xCF, 0xF2, 0x3E, 0x9F, 0xDC, 0xD9, 0x2A, 0xB3, 0xBB, 0x5E, 0x9E, 0xC8, 0xD2 }) == true)
                return;
            // ANASTASIA
            if (hashEQ(b, new byte[] { 0xCB, 0x14, 0xE9, 0x49, 0x4D, 0x88, 0xF7, 0x74, 0x27, 0xE6, 0x89, 0xAB, 0x3D, 0xA3, 0xC5, 0xA6 }) == true)
                return;
            // TEODORA
            if (hashEQ(b, new byte[] { 0x50, 0x8D, 0x50, 0xC4, 0xD0, 0x6A, 0x33, 0x39, 0x72, 0x0F, 0x22, 0xC3, 0x1E, 0x9F, 0x86, 0x5B }) == true)
                return;

            if (hashEQ(b, new byte[] { 0xD6, 0x38, 0x71, 0x7D, 0x7F, 0xA6, 0x38, 0xCD, 0xEE, 0x2F, 0xFF, 0xA3, 0x71, 0xEE, 0x13, 0xB8 }) == true)
                return;

            if (hashEQ(b, new byte[] { 0x0E, 0xEE, 0xEE, 0xBF, 0xC3, 0xD1, 0x4D, 0x43, 0xB7, 0x08, 0xC0, 0xBB, 0xFC, 0x17, 0xB5, 0x9D }) == true)
                return;

            if (hashEQ(b, new byte[] { 0xD4, 0x16, 0x01, 0xFE, 0x7B, 0xDE, 0x0C, 0x15, 0x51, 0x27, 0x63, 0x13, 0x21, 0xDE, 0x56, 0x8B }) == true)
                return;

            if (hashEQ(b, new byte[] { 0x2B, 0x0B, 0x1F, 0xDE, 0x3F, 0x23, 0x0B, 0x1C, 0xE6, 0xAB, 0xF3, 0x44, 0xFE, 0x74, 0xDF, 0xFF }) == true)
                return;
            
            //Hash-uri noi de la Sergiu
            if (hashEQ(b, new byte[] { 0x77, 0x2D, 0x33, 0x90, 0x30, 0xC7, 0xB8, 0xFB, 0xD6, 0x29, 0x19, 0xB0, 0xCA, 0xD6, 0x7E, 0x5C }) == true)
                return;
            if (hashEQ(b, new byte[] { 0x18, 0x90, 0x89, 0xC7, 0x35, 0xB3, 0x2F, 0x25, 0x1B, 0x7F, 0x81, 0x17, 0xA8, 0x47, 0x43, 0x83 }) == true)
                return;

            string s = "";
            foreach (byte bb in b)
                s += bb.ToString("X2");

            MessageBox.Show("Unregister copy - exiting !\nSerial ID:" + s);
            this.Close();
        }
        #endregion


        #region Meniu Commands
        private BaseProjectContainer GetCurrentTabContainer()
        {
            return mainTab.SelectedTab.Controls[0] as BaseProjectContainer;
        }
        private void OnPackProject(object sender, EventArgs e)
        {
            if (prj != null)
            {
                PackageCreateDialog createDLG = new PackageCreateDialog(prj);
                createDLG.ShowDialog();
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) // ESC
            {
                GetCurrentTabContainer().OnCommand(Command.ESCPressed,e);
            }
        }
        private void OnSimpleMenuCommand(object sender, EventArgs e)
        {
            var snd = sender as ToolStripMenuItem;
            if (snd==null)
            {
                MessageBox.Show("Current object is not a menu !!!\n" + e.ToString());
                return;
            }
            BaseProjectContainer tab = GetCurrentTabContainer();
            if (tab==null)
            {
                MessageBox.Show("No selected tab !!!");
                return;
            }
            if (snd.Tag==null)
            {
                MessageBox.Show("'Tag' parameter was not set for current menu item !");
                return;
            }
            Command cmd;
            if ((Enum.TryParse<Command>(snd.Tag.ToString(), out cmd) == false))
            {
                MessageBox.Show("Unkwnon commnad for current menu item: " + snd.Tag.ToString());
                return;
            }
            tab.OnCommand(cmd);
        }


        #endregion

        
        private void OnMainTabChanged(object sender, EventArgs e)
        {
            if (prj != null)
                prj.UpdateBuildsListAttribute();
            if (Context.IDESettings.EnableResourceTab)
                Context.Resources = prj.GetAppResources();
            if (Context.IDESettings.EnableCodeTab)
                tabProjectEditor.ActivateIntelliSense(false);
            if (Context.IDESettings.EnablePublisherTab)
                tabProjectPublishMaterials.SaveCurrentContent(false);
            if (mainTab.SelectedTab.Controls.Count==1)
            {
                var b = mainTab.SelectedTab.Controls[0] as BaseProjectContainer;
                b.OnActivate();
            }

        }



        private void OnUpdateMemoryStatus(object sender, EventArgs e)
        {
            long sz = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            long ws = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            if (sz >= 1048576)
            {
                double v = (sz / 1048576);
                double w = (ws / 1048576);
                txMemoryStatus.Text = String.Format("{0:0.00} / {1:0.00}Mb", w, v);
                return;
            }
            else
            {
                double v = (sz / 1024);
                double w = (ws / 1024);
                txMemoryStatus.Text = String.Format("{0:0.00} / {1:0.00}Kb", w, v);
                return;
            }

        }



        private void OnCheckForUpdates(object sender, EventArgs earg)
        {
            Project pp = prj;
            if (pp == null)
            {
                // fac un proiect dummy
                pp = new Project();
                pp.Settings = this.Context.Settings;
                pp.SetErrorFunction(ShowError);
            }
            CheckForUpdateDialog dlg = new CheckForUpdateDialog(pp);
            dlg.ShowDialog();
            if (dlg.State == CheckForUpdateDialog.CheckForUpdateState.Error)
            {
                pp.ShowErrors();
                return;
            }
            if (dlg.State == CheckForUpdateDialog.CheckForUpdateState.Update)
            {
                Context.Functions.SaveProject();
                try
                {
                    Process[] p = Process.GetProcesses();
                    int count = 0;
                    foreach (Process proc in p)
                    {
                        string pname;
                        try { pname = proc.ProcessName.ToLower(); }
                        catch (Exception) { pname = ""; }
                        if (pname == "gappcreator")
                            count++;
                    }
                    if (count > 1)
                    {
                        MessageBox.Show("Please close the other instances of GACCreator before updating ...");
                    }
                    string app_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Update", "updater.exe");
                    //MessageBox.Show("Running: " + app_path);
                    if (pp.RunCommand(app_path, "\"" + Context.Settings.Password + "\"", "Updater", false, true) == false)
                    {
                        pp.ShowErrors();
                        return;
                    }
                    this.Close();
                }
                catch (Exception e)
                {
                    pp.EC.AddException("Unable to list running instances", e);
                    pp.ShowErrors();
                    return;
                }
                // totul e ok - incep
            }
        }


        private void OnCloseGAC(object sender, FormClosingEventArgs e)
        {
            if (Context.Prj!=null)
            {
                DialogResult res = MessageBox.Show("Save current project: " + Context.Prj.GetProjectName() + " ?", "Close", MessageBoxButtons.YesNoCancel);
                if (res == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveProject();
                }
            }
        }


        #region Interface CommonFunctions
        public bool SaveProject()
        {
            if (prj != null)
            {
                tabProjectPublishMaterials.SaveCurrentContent(false);
                tabProjectEditor.SaveAllSourceFiles();
                //long at = prj.ActiveTime;
                //long rt = prj.RunTime;
                //prj.ActiveTime += Context.ActiveTime;
                //prj.RunTime += Context.RunTime;
                prj.Save();
                //prj.ActiveTime = at;
                //prj.RunTime = rt;
                if (prj.EC.HasErrors())
                {
                    prj.ShowErrors();
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool LoadProject(string fname, bool newProject)
        {
            tabProjectEditor.ActivateIntelliSense(false);
            ErrorsContainer tempEC = new ErrorsContainer();
            prj = Project.Load(fname, Context.Settings, tempEC);
            if (prj == null)
            {
                AddEventToTaskList("Loading process " + fname + " failed !", TaskEventState.Error);
                ErrorViewerDialog evd = new ErrorViewerDialog(tempEC);
                evd.ShowDialog();
                return false;
            }
            Context.Prj = prj;
            Context.Plugins.prj = prj;
            prj.Plugins = Context.Plugins;
            prj.SetErrorFunction(ShowError);
            this.Text = prj.GetProjectName();
            ResolutionEditorDialog.prj = prj;
            SettingsSnapshotControl.CurrentProject = prj;

            // incarc gac project settings
            if (File.Exists(Path.Combine(prj.ProjectPath, "project.settings")))
            {
                Context.IDESettings = ProjectContext.IDEProjectSettings.Load(Path.Combine(prj.ProjectPath, "project.settings"),prj.EC);
                if (Context.IDESettings == null)
                {
                    prj.ShowErrors();
                    Context.IDESettings = new ProjectContext.IDEProjectSettings();
                }
            }

            // sterg anumite tab-uri
            if (Context.IDESettings.EnableCodeTab == false)
            {
                mainTab.TabPages.Remove(tabCode);
                mainTab.TabPages.Remove(tabConstants);
            }
            if (Context.IDESettings.EnableResourceTab==false)
                mainTab.TabPages.Remove(tabResources);
            if (Context.IDESettings.EnableMemoryProfileTab == false)
                mainTab.TabPages.Remove(tabProfiles);
            if (Context.IDESettings.EnableStringsTab == false)
                mainTab.TabPages.Remove(tabStrings);
            if (Context.IDESettings.EnableAdvertismentTab == false)
                mainTab.TabPages.Remove(tabAds);
            if (Context.IDESettings.EnablePublisherTab == false)
                mainTab.TabPages.Remove(tabPublisher);   

            // incarc si pluginurile - daca exista
            if (Context.Plugins.ReloadPlugins() == false)
            {
                prj.ShowErrors();
            }
            else
            {
                if (Context.IDESettings.EnableResourceTab)
                {
                    tabProjectResources.UpdatePluginPreview();
                }
            }
            
            // trecem pe primul tab
            pnlMainScreen.Visible = false;
            mainTab.Visible = true;
            mainTab.SelectedIndex = 0;
            mainTab.Enabled = false;
            // fix daca nu am develop build
            if (prj.BuildConfigurations.Count == 0)
            {
                prj.BuildConfigurations.Add(new DevelopBuildConfiguration());
                prj.BuildConfigurations[0].Name = "Develop";
            }
            // activam fiecare tab in parte
            foreach (TabPage tb in mainTab.TabPages)
            {
                if (tb.Controls.Count==1)
                {
                    var b = tb.Controls[0] as BaseProjectContainer;
                    if (b != null)
                        b.OnOpenNewProject(newProject);
                }
            }
            // activez si main tab
            OnMainTabChanged(null, null);
            // salvez in MRU
            UpdateMRUList(Path.GetDirectoryName(fname));
            // pornesc
            if (newProject)
                Context.Task.Start(Task_NewProject, "New project");
            else
                Context.Task.Start(Task_LoadProject, "Load project");
            return true;
        }
        #endregion


        #region Working Times

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private void OnUpdateWorkingTime(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            Context.RunTime++;
            if (ApplicationIsActivated())
                Context.ActiveTime++;
            long h1,m1,s1;
            long h2, m2, s2;
            long v;
            v = Context.RunTime;
            h1 = v/3600;v=v%3600;
            m1 = v/60;s1 = v%60;
            v = Context.ActiveTime;
            h2 = v / 3600; v = v % 3600;
            m2 = v / 60; s2 = v % 60;
            lbIDETimes.Text = string.Format("Run {0:D2}:{1:D2}:{2:D2}    Active {3:D2}:{4:D2}:{5:D2}", h1, m1, s1,h2,m2,s2);
        }
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }
        #endregion



    }
}
