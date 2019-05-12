using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GAppCreator
{
    public partial class ProjectTabEditor : BaseProjectContainer
    {
        enum FindLocation: int
        {
            CurrentDocument = 0,
            CurrentProject = 1,
            AllOpenedDocuments = 2,
        }
        CodeTabControl tabSources;
        GACEditor.FindReplaceSettings findSettings = new GACEditor.FindReplaceSettings();
        bool generateResourceHeaderAndCpp = true;
        ProjectFile currentIntelliSenseFile = null;
        string currentIntelliSenseText = "";
        string currentIntelliSenseFileName = "";
        GenericBuildConfiguration buildToUseForCodeSettings = null;

        public ProjectTabEditor()
        {
            InitializeComponent();

            // tab-ul de cod
            tabSources = new CodeTabControl();
            this.splitContainer7.Panel1.Controls.Add(this.tabSources);
            this.tabSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSources.ImageList = this.sourceFileIcons;
            this.tabSources.Location = new System.Drawing.Point(0, 0);
            this.tabSources.Name = "tabSources";
            this.tabSources.SelectedIndex = 0;
            this.tabSources.ShowToolTips = true;
            this.tabSources.Size = new System.Drawing.Size(261, 364);
            this.tabSources.TabIndex = 0;
            this.tabSources.SelectedIndexChanged += new System.EventHandler(this.OnNewSouceFileHasFocus);
            this.tabSources.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnClickOnSouceTab);

            comboFindFormat.SelectedIndex = 0;
            comboFindLocation.SelectedIndex = 0;

            
            GACParser.AutocompleteIcons = GACIcons;
            GACParser.UpdateGlobalAutoComplete(null, null);
            GACParser.CreatePreprocessAutoCompleteList();
        }
        private void OnContextMenuCommand(object sender, EventArgs e)
        {
            ProcessContextMenuCommand(sender);
        }
        public override void OnActivate()
        {
            comboUseCompilerUseBuildSettings.Items.Clear();
            foreach (GenericBuildConfiguration build in Context.Prj.BuildConfigurations)
                comboUseCompilerUseBuildSettings.Items.Add(build.Name);
            comboUseCompilerUseBuildSettings.SelectedIndex = Context.Prj.GetBuildIndex(Context.CurrentBuild.Name);
            buildToUseForCodeSettings = Context.Prj.BuildConfigurations[comboUseCompilerUseBuildSettings.SelectedIndex];
            generateResourceHeaderAndCpp = true;
            GACParser.UpdateDebugCommands(Context.Prj);
            GACParser.UpdateControlIDs(Context.Prj);
            Context.Task.Start(Task_QuickCompile, "Prepare sources");
            UpdateFramewokClasses();
        }
        public override void OnCommand(Command cmd, object parameters = null)
        {
            GACEditor ed;
            switch (cmd)
            {
                case Command.SelectAll:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    ed.Selection.SelectAll();                  
                    break;
                case Command.Delete:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    if (ed.Selection.Length>0)
                        ed.NativeInterface.DeleteBack();
                    break;
                case Command.Copy:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;;
                    ed.Clipboard.Copy();
                    break;
                case Command.Cut:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break; ;
                    ed.Clipboard.Cut();
                    break;
                case Command.Paste:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break; ;
                    ed.Clipboard.Paste();
                    break;
                case Command.Undo:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break; ;
                    ed.UndoRedo.Undo();
                    break;
                case Command.Redo:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break; ;
                    ed.UndoRedo.Redo();
                    break;
                case Command.CollapseAll:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    ed.FoldAll();
                    break;
                case Command.ExpandAll:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    ed.UnfoldAll();
                    break;
                case Command.CollapseToDefinitions:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    ed.CollapseToDefinitions();
                    break;
                case Command.Find:
                    ShowFindReplacePanel(false);
                    break;
                case Command.FindWindow:
                    ShowFindReplaceWindow(false);
                    break;
                case Command.Replace:
                    ShowFindReplacePanel(true);
                    break;
                case Command.ReplaceWindow:
                    ShowFindReplaceWindow(true);
                    break;
                case Command.FindAllInCurrentDocument:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    FindAll(FindLocation.CurrentDocument, ed.GetSelectionOrCurrentWord());
                    break;
                case Command.FindAllInCurrentProject:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    FindAll(FindLocation.CurrentProject, ed.GetSelectionOrCurrentWord());
                    break;
                case Command.ESCPressed:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    ed.Focus();
                    ((KeyPressEventArgs)parameters).Handled = true;
                    break;
                case Command.OpenSourceCodeFile:
                    OpenSourceCodeFile((ProjectFile)(((BackgroundTaskAction)parameters).CommandParam));
                    break;
                case Command.AddCompileError:
                    AddCompileError((String)(((BackgroundTaskAction)parameters).CommandParam), false);
                    break;
                case Command.AddIntelliSenseError:
                    AddCompileError((String)(((BackgroundTaskAction)parameters).CommandParam), true);
                    break;
                case Command.AddCompileOutput:
                    AddOutputString((String)(((BackgroundTaskAction)parameters).CommandParam));
                    break;
                case Command.UpdateGlobalAutoComplete:
                    GACParser.UpdateGlobalAutoComplete(Context.Prj, buildToUseForCodeSettings);
                    break;
                case Command.UpdateLocalTypes:
                    GACEditor.UpdateWithLocalTypes();
                    break;
                case Command.UpdateGacFileList:
                    UpdateSourceCodeList();
                    break;
                case Command.EnableIntelliSenseTimer:
                    intellisenseTimer.Enabled = true;
                    break;
                case Command.RunFile:
                    ExecutionControlDialog dlg = new ExecutionControlDialog((string)(((BackgroundTaskAction)parameters).CommandParam), Context.Prj);
                    dlg.Show();
                    break;
                case Command.GoToLine:
                    if ((ed = GetCurrentEditor(false)) == null)
                        break;
                    ed.GoTo.ShowGoToDialog();
                    break;
                case Command.GoToNextError:
                    GoToNextError();
                    break;
                case Command.Run:
                    OnRun(null, null);
                    break;
                case Command.QuickRun:
                    OnQuickRun(null, null);
                    break;
                case Command.RunInVisualStudio:
                    OnRunInVisualStudio(null, null);
                    break;
                case Command.Compile:
                    OnCompile(null, null);
                    break;
                case Command.Comment:
                    OnComment(null, null);
                    break;
                case Command.UnComment:
                    OnUnComment(null, null);
                    break;
                case Command.OpenGoToDefinitionDialog:
                    SearchDefinition(false,"");
                    break;
                case Command.GoToDefinition:
                    OnGoToDefinitionForCurrentWord(null, null);
                    break;
                case Command.SearchDefinitionInCurrentFile:
                    SearchDefinition(true,"");
                    break;
                case Command.ApiBrowser:
                    OnBrowseForAPI(null, null);
                    break;                       
            }
        }
        public override void OnOpenNewProject(bool newProject)
        {
            tabSources.TabPages.Clear();
            UpdateSourceCodeList();            
        }

        #region Tasks

        public void Task_QuickCompile()
        {
            QuickCompile();
        }
        public void Task_CompileAll()
        {
            //Context.Prj.BuildConfigurations[0].G
            CompileGACtoCpp();
        }
        public void Task_UpdateIntelliSense()
        {
            UpdateIntelliSense();
        }
        public void Task_CompileAllAndRun()
        {
            if (generateResourceHeaderAndCpp)
            {
                ((DevelopBuildConfiguration)Context.Prj.BuildConfigurations[0]).BuildToUseForCodeSettings = buildToUseForCodeSettings;
                Context.Prj.BuildConfigurations[0].Prepare(Context.Prj, Context.Task);
                Context.Prj.BuildConfigurations[0].Build();
                if (Context.Prj.EC.HasErrors())
                {
                    Context.Prj.ShowErrors();
                    return;
                }
                generateResourceHeaderAndCpp = false;
            }
            if (CompileGACtoCpp() == false)
                return;
            Context.Task.UpdateSuccessErrorState(BuildCppToExe());
        }
        public void Task_CompileAllAndRunVisualStudio()
        {
            if (generateResourceHeaderAndCpp)
            {
                ((DevelopBuildConfiguration)Context.Prj.BuildConfigurations[0]).BuildToUseForCodeSettings = buildToUseForCodeSettings;
                Context.Prj.BuildConfigurations[0].Prepare(Context.Prj, Context.Task);
                Context.Prj.BuildConfigurations[0].Build();
                if (Context.Prj.EC.HasErrors())
                {
                    Context.Prj.ShowErrors();
                    return;
                }
                generateResourceHeaderAndCpp = false;
            }
            if (CompileGACtoCpp() == false)
                return;
            Context.Task.UpdateSuccessErrorState(true);
            Context.Prj.RunCommand(Context.Settings.VSDevEnv, Path.Combine(Context.Prj.ProjectPath, "CppProject",Context.Prj.GetProjectName()+".vcxproj"), "", false, false);
        }
        #endregion

        #region Source Code
        private void OnMouseUpOnFileList(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ProjectFile p = null;
                if (lstProjectFiles.SelectedItems.Count>0)
                    p = (ProjectFile)lstProjectFiles.SelectedItems[0].Tag;
                if (p == null)
                {
                    contextFileOpMenu.Items[5].Text = "Delete";
                    contextFileOpMenu.Items[6].Text = "Rename";
                    contextFileOpMenu.Items[5].Enabled = false;
                    contextFileOpMenu.Items[6].Enabled = false;
                }
                else
                {
                    contextFileOpMenu.Items[5].Text = "Delete " + p.Name;
                    contextFileOpMenu.Items[6].Text = "Rename " + p.Name + " to ...";
                    contextFileOpMenu.Items[5].Enabled = true;
                    contextFileOpMenu.Items[6].Enabled = true;
                }
                contextFileOpMenu.Show(lstProjectFiles, e.Location);
            }
        }
        private void OnMouseClickOnFileList(object sender, MouseEventArgs e)
        {

        }
        private void OnRenameGACFile(object sender, EventArgs e)
        {
            if (lstProjectFiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a GAC file to be renamed !");
                return;
            }
            ProjectFile p = (ProjectFile)lstProjectFiles.SelectedItems[0].Tag;
            if (p == null)
            {
                MessageBox.Show("File '" + lstProjectFiles.SelectedItems[0].Text + "' can not be renamed !");
                return;
            }
            if (p.Name.ToLower().EndsWith(".gac")==false)
            {
                MessageBox.Show("File '" + p.Name + "' is not a GAC file. Only '.gac' files can be renamed !");
                return;
            }
            if ((p.Name.ToLower() == "app.gac") || (p.Name.ToLower() == "global.gac"))
            {
                MessageBox.Show("File '" + p.Name + "' can not be renames as it is critical for the GAC project. Only scenes, framework objects and normal classes can be renamed.");
                return;
            }
            InputBox ib = new InputBox("Rename '" + p.Name + "' to ?", p.Name);
            if (ib.ShowDialog() != DialogResult.OK)
                return;
            string newName = ib.StringResult;
            if (newName.Contains('.'))
            {
                if (newName.ToLower().EndsWith(".gac") == false)
                {
                    MessageBox.Show("File '" + newName + "' is not a GAC file. It should have a '.gac' extension !");
                    return;
                }
            }
            else
            {
                newName += ".gac";
            }
            if ((p.Name.ToLower().EndsWith("scene.gac")) && (newName.ToLower().EndsWith("scene.gac")==false))
            {
                newName = newName.Replace(".gac", "Scene.gac");
            }
            // verific daca exista deja la mine in lista
            foreach (ProjectFile pf in Context.Prj.Files)
            {
                if (pf.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show("A file '" + pf.Name + "' already exists in your project !");
                    return;
                }
            }
            string newFilePath = Path.Combine(Context.Prj.ProjectPath, "Sources", newName);
            string currentFilePath = Path.Combine(Context.Prj.ProjectPath, "Sources", p.Name);
            if (File.Exists(newName))
            {
                if (MessageBox.Show("File '" + newName + "' already exists. Override ?", "Override", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                // ster fisieru vechi
                if (Disk.DeleteFile(newName, Context.Prj.EC) == false)
                {
                    Context.Prj.ShowErrors();
                    return;
                }
            }
            // fac redenumirea efectiva
            if (Disk.MoveFile(currentFilePath,newFilePath,Context.Prj.EC)==false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            TabPage tp = GetTabPage(p.Name);
            if (tp != null)
                tp.Text = newName;
            p.Name = newName;
            UpdateSourceCodeList();
        }
        private void OnDeleteGACFile(object sender, EventArgs e)
        {
            if (lstProjectFiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a GAC file to be deleted !");
                return;
            }
            ProjectFile p = (ProjectFile)lstProjectFiles.SelectedItems[0].Tag;
            if (p == null)
            {
                MessageBox.Show("File '" + lstProjectFiles.SelectedItems[0].Text + "' can not be deleted !");
                return;
            }
            if ((p.Name.ToLower() == "app.gac") || (p.Name.ToLower() == "global.gac"))
            {
                MessageBox.Show("File '" + p.Name + "' can not be deleted as it is critical for the GAC project. Only scenes, framework objects and normal classes can be removed.");
                return;
            }
            if (MessageBox.Show("Are you sure do you want to delete " + p.Name, "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (Disk.DeleteFile(Path.Combine(Context.Prj.ProjectPath, "Sources", p.Name), Context.Prj.EC) == false)
                {
                    Context.Prj.ShowErrors();
                    return;
                }
                TabPage tp = GetTabPage(p.Name);
                if (tp != null)
                    CloseSourceTab(tp);
                Context.Prj.Files.Remove(p);

                UpdateSourceCodeList();
            }
        }
        private void OnAddNewGACScene(object sender, EventArgs e)
        {
            NewSceneDialog dlg = new NewSceneDialog(Context.Prj);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ProjectFile pf = new ProjectFile();
                pf.Name = dlg.SceneFileName;
                pf.Opened = false;
                Context.Prj.Files.Add(pf);
                UpdateSourceCodeList();
                OpenSourceCodeFile(pf);
            }

            //CreateNewGACFile("Enter new scene name !", "NewScene", "Scene.gac", "$$SCENE.NAME$$");
        }
        private void OnAddNewGACControl(object sender, EventArgs e)
        {
            CreateNewGACFile("Enter new control name !", "NewControl", "Control.gac", "$$CONTROL.NAME$$");
        }
        private void OnAddNewGACClass(object sender, EventArgs e)
        {
            CreateNewGACFile("Enter new class name !", "NewClass", "Class.gac", "$$CLASS.NAME$$");
        }
        private void OnAddNewGACFrameworkObject(object sender, EventArgs e)
        {
            CreateNewGACFile("Enter new framework name !", "NewObject", "Object.gac", "$$CLASS.NAME$$");
        }
        private void CreateNewGACFile(string label, string defaultValue, string fileTemplate, string name_marker)
        {
            InputBox ib = new InputBox(label, defaultValue);
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string gacFile = Path.Combine(Context.Prj.ProjectPath, "Sources", ib.StringResult + ".gac");
                if (File.Exists(gacFile))
                {
                    MessageBox.Show("File " + gacFile + " already exists !");
                    return;
                }
                Dictionary<string, string> d = new Dictionary<string, string>();
                d[name_marker] = ib.StringResult;
                string h_c = Project.GetResource("GAC", fileTemplate, d, null);
                if (h_c.Length == 0)
                    return;
                if (Disk.SaveFile(gacFile, h_c, null) == false)
                {
                    MessageBox.Show("Unable to create file: " + gacFile);
                    return;
                }
                ProjectFile pf = new ProjectFile();
                pf.Name = ib.StringResult + ".gac";
                pf.Opened = false;
                Context.Prj.Files.Add(pf);
                UpdateSourceCodeList();
                OpenSourceCodeFile(pf);
            }
        }
        static int CompareProjectFile(ProjectFile a, ProjectFile b)
        {
            return a.Name.CompareTo(b.Name);
        }
        private void UpdateSourceCodeList()
        {
            lstProjectFiles.BeginUpdate();
            lstProjectFiles.Items.Clear();
            Context.Prj.Files.Sort(new Comparison<ProjectFile>(CompareProjectFile));
            foreach (ProjectFile p in Context.Prj.Files)
            {
                ListViewItem lvi = new ListViewItem(p.Name);
                lvi.SubItems.Add("-");
                if (p.ErrorsCount > 0)
                {
                    lvi.SubItems[1].Text = p.ErrorsCount.ToString();
                    lvi.ForeColor = Color.Red;
                }
                lvi.Tag = p;
                GACParser.GACFile gf = GACParser.Compiler.GetGACFile(p.Name);
                lvi.ImageKey = gf.FileType.ToString();
                lvi.Checked = p.Opened;
                if ((gf.FileType == GACParser.GACFileType.Application) || (gf.FileType == GACParser.GACFileType.Global))
                    lvi.Group = lstProjectFiles.Groups["Application"];
                else
                    lvi.Group = lstProjectFiles.Groups[lvi.ImageKey];
                lstProjectFiles.Items.Add(lvi);
            }
            foreach (TabPage tp in tabSources.TabPages)
            {
                string name = ((ProjectFile)tp.Tag).Name.ToLower();
                if (name.EndsWith(".gac"))
                    continue;
                if ((name.EndsWith(".h")) || (name.EndsWith(".cpp")))
                {
                    ListViewItem lvi = new ListViewItem(name);
                    lvi.SubItems.Add("-");
                    lvi.Tag = tp.Tag;
                    lvi.Checked = true;
                    lvi.Group = lstProjectFiles.Groups[Path.GetExtension(name).Replace(".", "")];
                    lstProjectFiles.Items.Add(lvi);
                }
            }
            lstProjectFiles.EndUpdate();
        }
        private TabPage GetTabPage(string FileName)
        {
            foreach (TabPage tp in tabSources.TabPages)
            {
                if (((ProjectFile)tp.Tag).Name.ToLower().Equals(FileName.ToLower()))
                    return tp;
            }
            return null;
        }
        private ProjectFile GetProjectFile(TabPage tp)
        {
            if ((tp==null) || (tp.Tag==null))
                return null;
            var pf = tp.Tag as ProjectFile;
            return pf;
        }
        private GACEditor GetGACEditor(ProjectFile pf,bool loadIfNotLoaded)
        {
            if (pf == null)
                return null;
            if (pf.Editor == null)
                return null;
            if ((pf.Editor==null) && (loadIfNotLoaded))
            {
                return new GACEditor(Context.Prj, pf.Name, new GACParser(), null, editorContextMenu);
            }
            return (GACEditor)pf.Editor;
        }
        private GACEditor GetGACEditor(string FileName,bool loadIfNotLoaded)
        {
            TabPage tp = GetTabPage(FileName);
            ProjectFile pf = GetProjectFile(tp);
            return GetGACEditor(pf,loadIfNotLoaded);
        }
        private ProjectFile OpenSourceCodeFile(ProjectFile p)
        {
            // vad daca e un tab deja deschis
            TabPage tpa = GetTabPage(p.Name);
            if (tpa != null)
            {
                tabSources.SelectedTab = tpa;
                return (ProjectFile)tpa.Tag;
            }
            // altfel il adaug - mai intai verific daca nu exista deja inlista mea
            foreach (ProjectFile pf in Context.Prj.Files)
                if (pf.Name.ToLower().Equals(p.Name.ToLower()))
                {
                    p = pf;
                    break;
                }
            tpa = new TabPage(p.Name);
            tpa.Tag = p;
            p.Opened = true;
            p.Editor = new GACEditor(Context.Prj, p.Name, p.Parser, tpa, editorContextMenu);
            tpa.Controls.Add((GACEditor)p.Editor);
            ((GACEditor)p.Editor).Dock = DockStyle.Fill;
            tabSources.TabPages.Add(tpa);
            tabSources.SelectedTab = tpa;
            UpdateSourceCodeList();
            return p;
        }
        private void OnDblClickOnProjectFiles(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstProjectFiles.SelectedItems)
                OpenSourceCodeFile((ProjectFile)lvi.Tag);
        }
        private void OnClickOnSouceTab(object sender, MouseEventArgs e)
        {
            Point p = this.tabSources.PointToClient(Cursor.Position);
            for (int i = 0; i < this.tabSources.TabCount; i++)
            {
                Rectangle r = this.tabSources.GetTabRect(i);
                if (r.Contains(p))
                {
                    this.tabSources.SelectedIndex = i; // i is the index of tab under cursor
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                        sourceTabContextMenu.Show(this.tabSources, e.Location);
                }
            }
        }

        private void CloseSourceTab(TabPage tab)
        {
            ProjectFile pf = (ProjectFile)tab.Tag;
            GACEditor ed = (GACEditor)pf.Editor;
            if (ed.HasTextBeenModified)
            {
                DialogResult res = MessageBox.Show("File " + Path.GetFileName(ed.fullFilePath) + " has been modified! Save it ?", "Save", MessageBoxButtons.YesNoCancel);
                if (res == System.Windows.Forms.DialogResult.Cancel)
                    return;
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    if (ed.SaveFile() == false)
                    {
                        Context.Prj.ShowErrors();
                        return;
                    }
                }
            }
            pf.Opened = false;
            tabSources.TabPages.Remove(tab);
        }
        private void OnCloseCurrentSourceTab(object sender, EventArgs e)
        {
            CloseSourceTab(tabSources.SelectedTab);
        }
        private void OnCloseAllSourceTabExceptCurrent(object sender, EventArgs e)
        {
            List<TabPage> tabs = new List<TabPage>();
            foreach (TabPage tp in tabSources.TabPages)
            {
                if (tp != tabSources.SelectedTab)
                    tabs.Add(tp);
            }
            foreach (TabPage tp in tabs)
                CloseSourceTab(tp);
            tabs = null;

        }
        private void OnReloadCurrentSourceTab(object sender, EventArgs e)
        {
            ProjectFile pf = (ProjectFile)tabSources.SelectedTab.Tag;
            GACEditor ed = (GACEditor)pf.Editor;
            if (ed.HasTextBeenModified)
            {
                if (MessageBox.Show("File " + Path.GetFileName(ed.fullFilePath) + " has been modified! Do you want to reload it ?", "Reload", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
            }
            ed.Reload();

        }
        ProjectFile GetCurrentEditedFile(bool fileIsGACFile)
        {
            if (tabSources.TabPages.Count == 0)
                return null; ;
            ProjectFile pf = (ProjectFile)(tabSources.SelectedTab.Tag);
            if (fileIsGACFile)
            {
                if (pf.Name.ToLower().EndsWith(".gac") == false)
                    return null;
            }
            return pf;
        }
        GACEditor GetCurrentEditor(bool fileIsGACFile)
        {
            ProjectFile pf = GetCurrentEditedFile(fileIsGACFile);
            if (pf == null)
                return null;
            if (pf.Editor == null)
                return null;
            return (GACEditor)pf.Editor;
        }
        private void OnSaveCurentTab(object sender, EventArgs e)
        {
            ProjectFile pf = (ProjectFile)tabSources.SelectedTab.Tag;
            GACEditor ed = (GACEditor)pf.Editor;
            if (ed.HasTextBeenModified)
            {
                if (ed.SaveFile() == false)
                {
                    Context.Prj.ShowErrors();
                    return;
                }
            }
        }
        private void GotoLine(string fileName, int line)
        {
            ProjectFile pf = new ProjectFile();
            pf.Name = Path.GetFileName(fileName);
            pf.Opened = true;
            pf = OpenSourceCodeFile(pf);
            GACEditor ed = (GACEditor)pf.Editor;
            ed.GoToLine(line);
            //ed.GoTo.Line(line);
            ed.Focus();
            UpdateSourceCodeList();
        }
        private void OnErrorDoubleClick(object sender, EventArgs e)
        {
            if (lstCompileErrors.SelectedItems.Count == 1)
            {
                LineErrorAnalyzer.Analyze((String)lstCompileErrors.SelectedItems[0].Tag);
                if (LineErrorAnalyzer.Line >= 0)
                    GotoLine(LineErrorAnalyzer.FileName, LineErrorAnalyzer.Line - 1);
            }
        }
        private void OnIntellisenseDoubleClick(object sender, EventArgs e)
        {
            if (lstIntelliSense.SelectedItems.Count == 1)
            {
                LineErrorAnalyzer.Analyze((String)lstIntelliSense.SelectedItems[0].Tag);
                if (LineErrorAnalyzer.Line >= 0)
                    GotoLine(LineErrorAnalyzer.FileName, LineErrorAnalyzer.Line - 1);
            }
        }
        public bool SaveAllSourceFiles()
        {
            if (Context.Prj == null)
                return false;
            foreach (ProjectFile pf in Context.Prj.Files)
            {
                if ((pf.Opened) && (pf.Editor != null))
                    ((GACEditor)pf.Editor).SaveFile();
            }
            return !Context.Prj.EC.HasErrors();
        }
        private void OnSaveAllSources(object sender, EventArgs e)
        {
            if (SaveAllSourceFiles() == false)
                Context.Prj.ShowErrors();
        }
        private void AddCompileError(string error, bool addToIntellisenseTab)
        {
            string[] lst = error.Replace("\r", "\n").Replace("\n\n", "\n").Split('\n');
            ListView lv = lstCompileErrors;
            if (addToIntellisenseTab)
                lv = lstIntelliSense;
            foreach (string s in lst)
            {
                if (s.Trim().Length == 0)
                    continue;
                LineErrorAnalyzer.Analyze(s);
                ListViewItem lvi = new ListViewItem(LineErrorAnalyzer.Line.ToString());
                lvi.SubItems.Add(LineErrorAnalyzer.Content.Trim());
                if (addToIntellisenseTab == false)
                {
                    ListViewGroup lg = lv.Groups[LineErrorAnalyzer.FileName];
                    if (lg == null)
                    {
                        lg = lv.Groups.Add(LineErrorAnalyzer.FileName, LineErrorAnalyzer.FileName);
                    }
                    lvi.Group = lg;
                }
                lvi.Tag = s;
                lvi.ToolTipText = LineErrorAnalyzer.Content.Trim();
                bool add = false;
                if (s.ToLower().Contains("error"))
                {
                    lvi.ForeColor = Color.Red;
                    add = true;
                }
                if (s.ToLower().Contains("warning"))
                {
                    lvi.ForeColor = Color.Brown;
                    add = true;
                }
                if (add)
                    lv.Items.Add(lvi);

                if (LineErrorAnalyzer.FileName.Length > 0)
                {
                    GACEditor ed = GetGACEditor(LineErrorAnalyzer.FileName, false);
                    if (ed != null)
                        ed.AddSyntaxError(s);
                }
            }
            if (addToIntellisenseTab == false)
            {
                if (lstCompileErrors.Items.Count > 0)
                    tabCompileErrors.Text = "Errors (" + lstCompileErrors.Items.Count.ToString() + ")";
                else
                    tabCompileErrors.Text = "No errors";
            }
            else
            {
                if (lstIntelliSense.Items.Count > 0)
                    tabIntellisense.Text = "Intellisense (" + lstIntelliSense.Items.Count.ToString() + ")";
                else
                    tabIntellisense.Text = "Intellisense";
            }
            // updatam si listele de fisiere
            if (addToIntellisenseTab == false)
            {
                Dictionary<string, ProjectFile> lf = new Dictionary<string, ProjectFile>();
                foreach (ProjectFile pf in Context.Prj.Files)
                {
                    lf[pf.Name] = pf;
                    pf.ErrorsCount = 0;
                }
                foreach (ListViewItem lvi in lstCompileErrors.Items)
                {
                    if (lf.ContainsKey(lvi.Group.Header))
                    {
                        lf[lvi.Group.Header].ErrorsCount++;
                    }
                }
                UpdateSourceCodeList();
            }
        }
        private void AddOutputString(string ss)
        {
            txtOutput.Text += ss + "\r\n"; ;
            txtOutput.ScrollToCaret();
        }
        private void OnNewSouceFileHasFocus(object sender, EventArgs e)
        {
            ProjectFile pf = GetCurrentEditedFile(true);
            if (pf != null)
            {
                if (pf.Editor != null)
                    ((GACEditor)pf.Editor).HasTextBeenModifiedAfterIntelliSense = true;
                // caut si panelul de erori
                foreach (ListViewItem lvi in lstCompileErrors.Items)
                {
                    if ((lvi.Group != null) && (lvi.Group.Name.Equals(pf.Name)))
                    {
                        lvi.EnsureVisible();
                        //lstCompileErrors.TopItem = lvi;
                        break;
                    }
                }
            }
        }
        private void ClearCompilerStatusList(bool fullCompile)
        {
            lstCompileErrors.Items.Clear();
            lstCompileErrors.Groups.Clear();
            tabCompileErrors.Text = "No errors";
            foreach (ProjectFile pf in Context.Prj.Files)
                pf.ErrorsCount = 0;
            txtOutput.Text = "";

            if (fullCompile)
            {
                List<TabPage> remove = null;
                foreach (TabPage tp in tabSources.TabPages)
                {
                    ((GACEditor)(((ProjectFile)(tp.Tag)).Editor)).ClearSyntaxError();
                    if (((ProjectFile)(tp.Tag)).Name.ToLower().EndsWith(".gac") == false)
                    {
                        if (remove == null)
                            remove = new List<TabPage>();
                        remove.Add(tp);
                    }
                }
                if (remove != null)
                {
                    foreach (TabPage tp in remove)
                        tabSources.TabPages.Remove(tp);
                }

            }


            UpdateSourceCodeList();
        }

        private void OnRun(object sender, EventArgs e)
        {
            ClearCompilerStatusList(true);

            if (SaveAllSourceFiles() == false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            if ((Context.Settings.CL == null) || (Context.Settings.CL.Length == 0))
            {
                MessageBox.Show("Please set the VS cl compiler from Context.Settings path !");
                return;
            }
            if ((Context.Settings.VSVarsBat == null) || (Context.Settings.VSVarsBat.Length == 0))
            {
                MessageBox.Show("Please set the 'vsvars32.bat' batch file from settings !");
                return;
            }
            if (GACParser.Compiler.CheckDefinitions(Context.Prj) == false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            Context.Task.Start(Task_CompileAllAndRun, "Run");
        }
        private void OnQuickRun(object sender, EventArgs e)
        {
            string outputFile = Path.Combine(Context.Prj.ProjectPath, "Bin", Context.Prj.GetProjectName() + ".exe");
            if (File.Exists(outputFile))
            {
                //Context.Prj.RunCommand(outputFile, "", "", false, false);
                ExecutionControlDialog dlg = new ExecutionControlDialog(outputFile, Context.Prj);
                dlg.Show();
            }
            else
            {
                OnRun(null, null);
            }
        }
        private void OnRunInVisualStudio(object sender, EventArgs e)
        {
            ClearCompilerStatusList(true);
            if (SaveAllSourceFiles() == false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            if ((Context.Settings.VSDevEnv == null) || (Context.Settings.VSDevEnv.Length == 0))
            {
                MessageBox.Show("Please set the VS devenv.exe path from Context.Settings path !");
                return;
            }
            Context.Task.Start(Task_CompileAllAndRunVisualStudio, "Visual Studio");
        }
        private void OnCompile(object sender, EventArgs e)
        {
            ClearCompilerStatusList(true);
            if (SaveAllSourceFiles() == false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            Context.Task.Start(Task_CompileAll, "Compile");
        }
        private void OnReloadDefinitions(object sender, EventArgs e)
        {
            GACParser.LoadGacDefinitions(Application.ExecutablePath + ".gacxml");
            GACParser.UpdateGlobalAutoComplete(Context.Prj, buildToUseForCodeSettings);
            GACEditor.UpdateWithLocalTypes();
            UpdateFramewokClasses();
        }
        private void OnReloadCppErrors(object sender, EventArgs e)
        {
            string compileLog = Path.Combine(Context.Prj.ProjectPath, "CppProject", "compile.log");
            string content = Disk.ReadFileAsString(compileLog, Context.Prj.EC);
            if (Context.Prj.EC.HasErrors())
            {
                Context.Prj.ShowErrors();
                return;
            }
            ClearCompilerStatusList(false);
            AddCompileError(content, false);
        }

        private void GoToNextError()
        {
            ListView l = null;
            if (lstCompileErrors.Items.Count > 0)
                l = lstCompileErrors;
            else if (lstIntelliSense.Items.Count > 0)
                l = lstIntelliSense;
            if (l==null)
                return;
            int index = -1;
            if (l.SelectedItems.Count == 1)
                index = l.SelectedIndices[0];
            index++;
            if (index >= l.Items.Count)
                index = 0;
            l.Items[index].Selected = true;
            l.Items[index].EnsureVisible();
            if (l == lstIntelliSense)
                OnIntellisenseDoubleClick(null,null);
            if (l== lstCompileErrors)
                OnErrorDoubleClick(null,null);
        }

        private void OnComment(object sender, EventArgs e)
        {
            GACEditor ed;
            if ((ed = GetCurrentEditor(false)) == null)
                return;
            ed.CommenOrUnCommentSelection(true);
        }

        private void OnUnComment(object sender, EventArgs e)
        {
            GACEditor ed;
            if ((ed = GetCurrentEditor(false)) == null)
                return;
            ed.CommenOrUnCommentSelection(false);
        }
        #endregion

        #region Compile GAC to C++

        private void OnChangeBuildToUseForCodeSettings(object sender, EventArgs e)
        {
            if (comboUseCompilerUseBuildSettings.SelectedIndex >= 0)
                buildToUseForCodeSettings = Context.Prj.BuildConfigurations[comboUseCompilerUseBuildSettings.SelectedIndex];
            else
                buildToUseForCodeSettings = null;
            generateResourceHeaderAndCpp = true;
            GACParser.UpdateDebugCommands(Context.Prj);
            GACParser.UpdateControlIDs(Context.Prj);
            Context.Task.Start(Task_QuickCompile, "Prepare sources");
            UpdateFramewokClasses();
        }
        private bool CompileGACtoCpp()
        {
            bool UpdateEditorLocalTypes = false;
            bool res = GAC2CPPConvertor.Convert(Context.Prj, Context.Task, true, ref UpdateEditorLocalTypes, buildToUseForCodeSettings,false,true);
            if (UpdateEditorLocalTypes)
                GACEditor.UpdateWithLocalTypes();
            return res;
        }
        private bool BuildCppToExe()
        {
            Context.Task.CreateSubTask("Linking ...");
            Context.Task.SendCommand(Command.AddCompileOutput, "Linking code ...");
            string GACtoCPPFolder = Path.Combine(Context.Prj.ProjectPath, "CppProject");

            string s = "@cd " + GACtoCPPFolder + "\n";
            s += GACtoCPPFolder.Substring(0, 2) + "\n";
            s += "@del /Q *.obj\n";
            s += "@call \"" + Context.Settings.VSVarsBat + "\"\n";
            s += "@\"" + Context.Settings.CL + "\" ";

            string lib_folder = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Libs");

            s += " -I\"" + lib_folder + "\"";
            s += " -DPLATFORM_DEVELOP";
            if (Context.Prj.BuildConfigurations[0].EnableErrorLogging)
                s += " -DENABLE_ERROR_LOGGING";
            if (Context.Prj.BuildConfigurations[0].EnableEventLogging)
                s += " -DENABLE_EVENT_LOGGING";
            if (Context.Prj.BuildConfigurations[0].EnableInfoLogging)
                s += " -DENABLE_INFO_LOGGING";

            s += " main.cpp resources.cpp AnimationTemplateWrapper.cpp ";
            foreach (ProjectFile pf in Context.Prj.Files)
                s += " " + pf.Parser.GetFileName("cpp");

            s += " /link kernel32.lib \"" + Path.Combine(lib_folder, "GAppFramework.lib") + "\"";

            string outputFile = Path.Combine(Context.Prj.ProjectPath, "Bin", Context.Prj.GetProjectName() + ".exe");
            if (Disk.DeleteFile(outputFile, Context.Prj.EC) == false)
                return false;
            s += " /OUT:\"" + outputFile + "\"";

            string compileLog = Path.Combine(GACtoCPPFolder, "compile.log");
            if (Disk.DeleteFile(compileLog, Context.Prj.EC) == false)
                return false;

            s += " >\"" + compileLog + "\"";
            if (Disk.SaveFile(Path.Combine(GACtoCPPFolder, "build.bat"), s, Context.Prj.EC) == false)
                return false;
            if (Context.Prj.RunCommand("C:\\Windows\\System32\\cmd.exe", "/C " + Path.Combine(GACtoCPPFolder, "build.bat"), "Compiler...", true, true) == false)
                return false;
            string content = Disk.ReadFileAsString(compileLog, Context.Prj.EC);
            if (Context.Prj.EC.HasErrors())
                return false;

            Context.Task.SendCommand(Command.AddCompileError, content);
            Context.Task.SendCommand(Command.AddCompileOutput, content);

            // copii si DLL-ul
            if (Disk.Copy(Path.Combine(lib_folder, "GAppFramework.dll"), Path.Combine(Context.Prj.ProjectPath, "Bin", "GAppFramework.dll"), Context.Prj.EC) == false)
                return false;
            if (Disk.Copy(Path.Combine(lib_folder, "bass.dll"), Path.Combine(Context.Prj.ProjectPath, "Bin", "bass.dll"), Context.Prj.EC) == false)
                return false;

            if (File.Exists(outputFile))
            {
                Context.Task.SendCommand(Command.AddCompileOutput, "Running : " + outputFile);
                Context.Task.SendCommand(Command.RunFile, outputFile);
                return true;
            }

            Context.Task.SendCommand(Command.AddCompileOutput, "File " + outputFile + " was not created !");
            return false;
        }
        private void QuickCompile()
        {
            GAC2CPPConvertor.QuickCompile(Context.Prj, Context.Task, buildToUseForCodeSettings);
        }
        private void UpdateIntelliSense()
        {
            string err = "";
            while (true)
            {
                try
                {
                    GACParser.ClearError();
                    GACParser p = currentIntelliSenseFile.Parser;
                    if (p.Parse(currentIntelliSenseText, currentIntelliSenseFileName,false) == false)
                        break;

                    if (p.PreprocessDefinitions() == false)
                        break;
                    if (GACParser.ComputeClassOrder() == false)
                        break;
                    p.UpdateTokensForLocalModules();
                    GACParser.GACFile gf = GACParser.Compiler.GetGACFile(currentIntelliSenseFileName);
                    bool res = true;
                    foreach (GACParser.Module m in gf.Modules.Values)
                    {
                        if (m.Type != GACParser.ModuleType.Class)
                            continue;
                        if (p.AnalizeClassMembers(m) == false)
                        {
                            err += GACParser.GetError() + "\n";
                            res = false;
                        }
                        m.UpdateAutoCompleteList();
                    }
                    if (currentIntelliSenseFileName.ToLower().Equals("global.gac"))
                    {
                        GACParser.Module m = GACParser.Compiler.GetModule("Global");
                        if (m != null)
                        {
                            if (p.AnalizeClassMembers(m) == false)
                            {
                                err += GACParser.GetError() + "\n";
                                res = false;
                            }
                        }
                    }
                    if (res == false)
                        break;
                    p.AnalizeCode();
                    break;
                }
                catch (Exception)
                {
                    break;
                }
            }
            err += GACParser.GetError();
            GACEditor.UpdateWithLocalTypes();
            if (err.Length > 0)
            {
                Context.Task.SendCommand(Command.AddIntelliSenseError, err);
            }
        }
        private void OnUpdateIntelliSense(object sender, EventArgs e)
        {
            if (Context.Task.IsBusy)
                return;
            ProjectFile pf = GetCurrentEditedFile(true);
            if (pf == null)
                return;
            if (pf.Editor == null)
                return;
            if (((GACEditor)pf.Editor).HasTextBeenModifiedAfterIntelliSense == false)
                return;
            ((GACEditor)pf.Editor).HasTextBeenModifiedAfterIntelliSense = false;
            // fac compilarea pe alt fir si updatez 
            currentIntelliSenseFile = pf;
            currentIntelliSenseText = ((GACEditor)pf.Editor).Text;
            currentIntelliSenseFileName = pf.Name;
            lstIntelliSense.Items.Clear();
            ((GACEditor)pf.Editor).ClearSyntaxError();
            tabIntellisense.Text = "IntelliSense";
            Context.Task.Start(Task_UpdateIntelliSense, "IntelliSense");
        }
        public void ActivateIntelliSense(bool enable)
        {
            intellisenseTimer.Enabled = enable;
        }
        #endregion

        #region Find/Replace for GAC Editor
        private void ShowFindReplaceWindow(bool showReplace)
        {
            ProjectFile pf = GetCurrentEditedFile(false);
            GACEditor ed = (GACEditor)pf.Editor;
            if (ed.Focused)
            {
                if (showReplace)
                    ed.FindReplace.ShowReplace();                     
                else
                    ed.FindReplace.ShowFind();
            }
        }
        private void ShowFindReplacePanel(bool replaceIsEnabled)
        {
            ProjectFile pf = GetCurrentEditedFile(false);
            if (((GACEditor)pf.Editor).Focused)
            {
                tabEditorOptions.SelectedTab = tabGacEditorFindReplace;
                tabGacEditorFindReplace.Focus();
                cbGacEditorEnableReplaceWidth.Checked = replaceIsEnabled;
                comboGacTextToFind.Focus();
            }
        }
        private void PrepareFindSettings(GACEditor.FindReplaceSettings settings)
        {
            settings.TextToFind = comboGacTextToFind.Text;
            settings.ReplaceText = comboGacReplaceText.Text;
            if (cbGacEditorEnableReplaceWidth.Checked == false)
                settings.ReplaceText = null;
            settings.Options = ScintillaNET.SearchFlags.Empty;
            if (cbFindMatchCase.Checked)
                settings.Options |= ScintillaNET.SearchFlags.MatchCase;
            if (cbFindWholeWord.Checked)
                settings.Options |= ScintillaNET.SearchFlags.WholeWord;
            if (comboFindFormat.SelectedIndex == 1)
                settings.Options |= ScintillaNET.SearchFlags.Posix;
            if (comboFindFormat.SelectedIndex == 2)
                settings.Options |= ScintillaNET.SearchFlags.RegExp;
        }
        private void OnActivateGacFindDialog(object sender, EventArgs e)
        {
            GACEditor ed = GetCurrentEditor(false);  
            if (ed == null)
                return;
            comboGacTextToFind.Text = ed.GetSelectionOrCurrentWord();
            ed.EnableDisableEditor(false);
            ////ed.GraySyntaxColorForGAC();
            ////comboGacTextToFind.Text = "";
            ////comboGacReplaceText.Text = "";
            //if (ed.Selection.Length > 0)
            //    comboGacTextToFind.Text = ed.Selection.Text;
            comboGacTextToFind.Focus();
            OnTextToFindChanges(null, null);
        }

        private void OnLeaveGacFindDialog(object sender, EventArgs e)
        {
            GACEditor ed = GetCurrentEditor(false);
            if (ed == null)
                return;
            ed.EnableDisableEditor(true);
        }
        private bool AddFindResultsToList(GACEditor ed,string text,ScintillaNET.SearchFlags options)
        {
            List<ScintillaNET.Range> findings = ed.FindReplace.FindAll(text, options);
            if ((findings == null) || (findings.Count==0))
                return false;
            string name = Path.GetFileName(ed.fullFilePath);
            ListViewGroup g = lstFindResults.Groups.Add(name, name+" ("+findings.Count.ToString()+")");
            g.Tag = ed;
            foreach (ScintillaNET.Range r in findings)
            {
                ListViewItem lvi = new ListViewItem("Line "+(r.StartingLine.Number+1).ToString());
                lvi.Tag = r;
                ScintillaNET.Line ln = r.StartingLine;
                lvi.SubItems.Add(ln.Text);
                lvi.Group = g;
                lstFindResults.Items.Add(lvi);
            }
            return true;
        }
        private void FindAll(FindLocation loc, string text, ScintillaNET.SearchFlags options = ScintillaNET.SearchFlags.Empty|ScintillaNET.SearchFlags.MatchCase)
        {
            if ((text == null) || (text.Length == 0))
                return;
            lbSearchedText.Text = "Search for '" + text + "'";
            GACEditor ed = GetCurrentEditor(false);
            lstFindResults.Items.Clear();
            lstFindResults.Groups.Clear();

            switch (loc)
            {
                case FindLocation.CurrentDocument:
                    if (ed == null)
                        return;
                    AddFindResultsToList(ed, text, options);
                    break;
                case FindLocation.CurrentProject:
                    foreach (ProjectFile pf in Context.Prj.Files)
                    {
                        ed = GetGACEditor(pf, true);
                        if (ed!=null)
                            AddFindResultsToList(ed, text, options);
                    }
                    break;
                case FindLocation.AllOpenedDocuments:
                    foreach (TabPage tp in tabSources.TabPages)
                    {
                        ed = GetGACEditor(GetProjectFile(tp), false);
                        if (ed != null)
                            AddFindResultsToList(ed, text, options);
                    }
                    break;
            }
            if (lstFindResults.Items.Count>0)
            {
                tabEditorOptions.SelectedTab = tabFindResults;
                tabFindResults.Focus();
                lstFindResults.Focus();
            }
            else
            {
                MessageBox.Show("Requested string :'" + text + "' was not found !");
            }
        }
        private int FindNextDocumentThatMatchSearch(FindLocation loc,int index,int dir)
        {
            if (loc == FindLocation.AllOpenedDocuments)
            {
                index += dir;
                if ((index >= tabSources.TabPages.Count) || (index < 0))
                    return -1;
                GACEditor ed = GetGACEditor(GetProjectFile(tabSources.TabPages[index]), false);
                if (ed == null)
                    return -1;

            }
            if (loc == FindLocation.CurrentProject)
            {

            }
            return -1;
        }
        private void Find(bool next)
        {
            GACEditor ed;
            PrepareFindSettings(findSettings);
            GACEditor.FindResult res;
            FindLocation loc = (FindLocation)comboFindLocation.SelectedIndex;
            if ((findSettings.TextToFind==null) || (findSettings.TextToFind.Length==0))
            {
                MessageBox.Show("Please select a text to find first !");
                comboGacTextToFind.Focus();
                return;
            }
            switch (loc)
            {
                case FindLocation.CurrentDocument:
                    ed = GetCurrentEditor(false);
                    if (ed == null)
                        return;
                    res = ed.FindText(findSettings, next, true);
                    if (res == GACEditor.FindResult.NotFound)
                        MessageBox.Show("Unable to find the specific text in the document !");
                    else if (res == GACEditor.FindResult.EndOfDocument)
                    {
                        if (MessageBox.Show("You have reached the end of the document. Start find from the beggining ?","Find", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            ed.FindText(findSettings, next, false);
                    }
                    break;
                // current project
                case FindLocation.CurrentProject:
                    break;
                // all opened documents
                case FindLocation.AllOpenedDocuments:
                    ed = GetCurrentEditor(false);
                    if (ed == null)
                        return;
                    res = ed.FindText(findSettings, next, true);
                    if ((res == GACEditor.FindResult.NotFound) || (res==GACEditor.FindResult.EndOfDocument))
                    {
                        // trc la urmatorul document deschis
                    }
                    break;
            }
        }
        private void OnTextToFindChanges(object sender, EventArgs e)
        {
            btnFindNext.Enabled = btnFindPrevious.Enabled =  btnFindReplaceAll.Enabled = comboGacTextToFind.Text.Length > 0;
            btnGacReplace.Enabled = btnFindNext.Enabled & cbGacEditorEnableReplaceWidth.Checked;
        }
        private void OnFindNext(object sender, EventArgs e)
        {
            Find(true);
        }
        private void OnFindPrevious(object sender, EventArgs e)
        {
            Find(false);
        }
        private void OnEnableReplaceWidth(object sender, EventArgs e)
        {
            comboGacReplaceText.Enabled = cbGacEditorEnableReplaceWidth.Checked;
            btnGacReplace.Enabled = btnFindNext.Enabled & cbGacEditorEnableReplaceWidth.Checked;
            if (cbGacEditorEnableReplaceWidth.Checked)
                btnFindReplaceAll.Text = "Replace &All";
            else
                btnFindReplaceAll.Text = "Find &All";
        }
        private void comboGacTextToFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                if (comboGacTextToFind.Items.Contains(comboGacTextToFind.Text) == false)
                    comboGacTextToFind.Items.Insert(0, comboGacTextToFind.Text);
                OnFindNext(null, null);
            }
        }
        private void comboGacReplaceText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                if (comboGacReplaceText.Items.Contains(comboGacReplaceText.Text) == false)
                    comboGacReplaceText.Items.Insert(0, comboGacReplaceText.Text);
                OnFindNext(null, null);
            }
        }
        private void OnClickOnFindResult(object sender, MouseEventArgs e)
        {
            if (lstFindResults.SelectedItems.Count != 1)
                return;
            string fname = lstFindResults.SelectedItems[0].Group.Name;
            int lineNo;
            if (int.TryParse(lstFindResults.SelectedItems[0].SubItems[0].Text.Replace("Line ", "").Trim(), out lineNo) == false)
                return;
            ScintillaNET.Range r = (ScintillaNET.Range)lstFindResults.SelectedItems[0].Tag;
            GotoLine(fname, lineNo-1);
            GACEditor ed = GetGACEditor(fname, false);
            if (ed == null)
                return;
            ed.Selection.Range = r;
            lstFindResults.SelectedItems[0].EnsureVisible();
        }


        private void OnNextStringFound(object sender, EventArgs e)
        {
            if (lstFindResults.Items.Count == 0)
                return;
            int index = -1;
            if (lstFindResults.SelectedItems.Count == 1)
                index = lstFindResults.SelectedIndices[0];
            index++;
            if (index >= lstFindResults.Items.Count)
                index = 0;
            lstFindResults.Items[index].Selected = true;
            OnClickOnFindResult(null, null);
        }

        private void OnPreviousStringFound(object sender, EventArgs e)
        {
            if (lstFindResults.Items.Count == 0)
                return;
            int index = lstFindResults.Items.Count;
            if (lstFindResults.SelectedItems.Count == 1)
                index = lstFindResults.SelectedIndices[0];
            index--;
            if (index < 0)
                index = lstFindResults.Items.Count - 1;
            lstFindResults.Items[index].Selected = true;
            OnClickOnFindResult(null, null);
        }
        private void OnFindReplaceAll(object sender, EventArgs e)
        {
            if (cbGacEditorEnableReplaceWidth.Checked == false)
            {
                PrepareFindSettings(findSettings);
                FindLocation loc = (FindLocation)comboFindLocation.SelectedIndex;
                FindAll(loc, findSettings.TextToFind, findSettings.Options);
            }
        }
        #endregion

        #region Debug Commands , Control IDs and Object events

        private void UpdateGACParserConstants()
        {
            GACParser.UpdateDebugCommands(Context.Prj);
            GACParser.UpdateControlIDs(Context.Prj);
            GACParser.UpdateObjectEventsIDs(Context.Prj);
        }
        private void OnShowDebugCommandsWindow(object sender, EventArgs e)
        {
            DebugCommandsDialog dlg = new DebugCommandsDialog(Context.Prj);
            dlg.ShowDialog();
            UpdateGACParserConstants();
        }
        private void OnShowControlIDs(object sender, EventArgs e)
        {
            IDsEditor dlg = new IDsEditor(Context.Prj,true);
            if (dlg.ShowDialog() == DialogResult.OK)
                UpdateGACParserConstants();
        }

        private void OnShowObjectEvents(object sender, EventArgs e)
        {
            IDsEditor dlg = new IDsEditor(Context.Prj, false);
            if (dlg.ShowDialog() == DialogResult.OK)
                UpdateGACParserConstants();
        }

        #endregion

        #region Code definitions
        private void GoToDefinition(GACParser.LocalDefinition ld)
        {
            if (ld == null)
                return;
            if (ld.File.Length==0)
            {
                MessageBox.Show("No file specified for current definition (" + ld.Name + ")");
                return;
            }
            if (ld.LineNumber < 0)
            {
                MessageBox.Show("No line number specified for current definition (" + ld.Name + ")");
                return;
            }
            if (ld.Start < 0)
            {
                MessageBox.Show("No start position specified for current definition (" + ld.Name + ")");
                return;
            }
            GotoLine(ld.File, ld.LineNumber);
            GACEditor ed = GetGACEditor(ld.File, false);
            if (ed == null)
                return;
            ed.Selection.Range = new ScintillaNET.Range(ld.Start, ld.Start, ed);  
        }
        private void SearchDefinition(bool onlyInCurrentFile,string prototype)
        {
            ProjectFile pf = GetCurrentEditedFile(true);
            string fname = "";
            if (pf != null)
                fname = pf.Name;
            GoToDefinitionDialog dlg = new GoToDefinitionDialog(fname, onlyInCurrentFile,prototype);
            if (dlg.ShowDialog() == DialogResult.OK)
                GoToDefinition(dlg.ResultedItem);
        }
        private void OnGoToDefinition(object sender, EventArgs e)
        {
            SearchDefinition(false,"");
        }
        private void OnGoToDefinitionForCurrentWord(object sender, EventArgs e)
        {
            GACEditor ed = GetCurrentEditor(true);
            if (ed == null)
            {
                MessageBox.Show("This command works only on a .gac file !");
                return;
            }
            GACParser.Location location = ed.GetCurrentLocation();
            GACParser.Intellisense i = new GACParser.Intellisense();
            i.Analize(location);
            if (i.CurrentWordInfo == null)
                return;
            if (i.CurrentWordInfo.prototype.Length == 0)
            {
                MessageBox.Show("'"+i.CurrentWordInfo.token.Text+"' is not a browseable symbol !");
                return;
            }
            switch (i.CurrentWordInfo.Type)
            {
                case GACParser.Intellisense.InfoType.VariableOrParameter:
                case GACParser.Intellisense.InfoType.Scenes:                
                    GoToDefinition(i.CurrentWordInfo.localDef);
                    return;
                case GACParser.Intellisense.InfoType.Global:
                    GotoLine(i.CurrentWordInfo.localDef.File, 0);
                    return;
                case GACParser.Intellisense.InfoType.Images:
                    new ResourcePreviewDialog(Context, i.CurrentWordInfo.token.Text, ResourcesConstantType.Image).ShowDialog();
                    return;
                case GACParser.Intellisense.InfoType.Sounds:
                    new ResourcePreviewDialog(Context, i.CurrentWordInfo.token.Text, ResourcesConstantType.Sound).ShowDialog();
                    return;
                case GACParser.Intellisense.InfoType.Shaders:
                    new ResourcePreviewDialog(Context, i.CurrentWordInfo.token.Text, ResourcesConstantType.Shader).ShowDialog();
                    return;
                case GACParser.Intellisense.InfoType.Fonts:
                    new ResourcePreviewDialog(Context, i.CurrentWordInfo.token.Text, ResourcesConstantType.Font).ShowDialog();
                    return;
                case GACParser.Intellisense.InfoType.Strings:
                    new ResourcePreviewDialog(Context, i.CurrentWordInfo.token.Text, ResourcesConstantType.String).ShowDialog();
                    return;           
            }


            int count = 0;
            List<GACParser.LocalDefinition> def = GACParser.GetLocalDefinitions();
            GACParser.LocalDefinition found = null;
            foreach (GACParser.LocalDefinition ld in def)
            {
                if (ld.Prototype == i.CurrentWordInfo.prototype)
                {
                    found = ld;
                    count++;
                }
            }
            if (count==1)
            {
                GoToDefinition(found);
                return;
            }
            if (count>1)
            {
                SearchDefinition(false, i.CurrentWordInfo.prototype);
                return;
            }
            if (i.CurrentWordInfo.prototype.Length > 0)
            {
                ApiViewDialog dlg = new ApiViewDialog(i.CurrentWordInfo.prototype);
                dlg.ShowDialog();
            }
            else
            {
                MessageBox.Show("Unable to find browseable information about this item !");
            }
        }
        #endregion

        #region Api Browser
        private void UpdateMembers(TreeNode node, GACParser.Module m)
        {
            if (m.Members != null)
            {
                foreach (string mbkey in m.Members.Keys)
                {
                    GACParser.Member mb = m.Members[mbkey];
                    if (m.Type == GACParser.ModuleType.Enum)
                    {
                        TreeNode trn = new TreeNode(mb.Name);
                        trn.ImageKey = "constant";
                        node.Nodes.Add(trn);
                    }
                    else
                    {
                        TreeNode trn = new TreeNode(mb.GetDefinition(""));
                        node.Nodes.Add(trn);
                        switch (mb.Type)
                        {
                            case GACParser.MemberType.Constant: trn.ImageKey = "variable"; break;
                            case GACParser.MemberType.Constructor: trn.ImageKey = "constructor"; break;
                            case GACParser.MemberType.Destructor: trn.ImageKey = "destructor"; break;
                            case GACParser.MemberType.Function: trn.ImageKey = "function"; break;
                            case GACParser.MemberType.Variable: trn.ImageKey = "variable"; break;
                        }
                    }
                }
            }
        }
        private void UpdateModules(TreeNode node, Dictionary<string, GACParser.Module> modules, Dictionary<string, bool> added)
        {
            if (modules == null)
                return;
            foreach (string key in modules.Keys)
            {
                GACParser.Module m = modules[key];
                if (added.ContainsKey(m.Name))
                    continue;
                TreeNode trn = new TreeNode(m.Name);
                if (node == null)
                    treeFrameworkClasses.Nodes.Add(trn);
                else
                    node.Nodes.Add(trn);
                if (m.TypeInformation == GACParser.ModuleTypeInformation.IsApp)
                    trn.ImageKey = "app";
                else if (m.TypeInformation == GACParser.ModuleTypeInformation.IsScene)
                    trn.ImageKey = "scene";
                else if (m.Type == GACParser.ModuleType.Namespace)
                    trn.ImageKey = "namespace";
                else if (m.Type == GACParser.ModuleType.Class)
                {
                    trn.ImageKey = "class";
                    if (m.BasicType == true)
                        trn.ImageKey = "basic_type";
                }
                else if (m.Type == GACParser.ModuleType.StaticClass)
                    trn.ImageKey = "static_class";
                else if (m.Type == GACParser.ModuleType.Enum)
                    trn.ImageKey = "enum";

                added[m.Name] = true;
                UpdateModules(trn, m.Modules,added);
                UpdateMembers(trn, m);                

            }

        }
        private void UpdateFramewokClasses()
        {
            treeFrameworkClasses.Nodes.Clear();
            GACParser.GACFile c = GACParser.Compiler.GetGACFile("");
            if (c == null)
                return;
            Dictionary<string, bool> added = new Dictionary<string, bool>();
            UpdateModules(null,c.Modules,added);
            treeFrameworkClasses.Sort();
        }
        private void OnRefreshFrameworkClasses(object sender, EventArgs e)
        {
            UpdateFramewokClasses();
        }
        private void OnBrowseForAPI(object sender, EventArgs e)
        {
            ApiViewDialog dlg = new ApiViewDialog("");
            dlg.ShowDialog();
        }
        #endregion





    }
}
