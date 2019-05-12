namespace GAppCreator
{
    partial class ProjectTabEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectTabEditor));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("C++ Header Files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("C++ Source Files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Scenes", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("App files", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Custom Controls", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Classes", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Framework Object", System.Windows.Forms.HorizontalAlignment.Left);
            this.GACFileIcons = new System.Windows.Forms.ImageList(this.components);
            this.sourceTabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllExceptCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveCurentTab = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceFileIcons = new System.Windows.Forms.ImageList(this.components);
            this.intellisenseTimer = new System.Windows.Forms.Timer(this.components);
            this.GACIcons = new System.Windows.Forms.ImageList(this.components);
            this.editorContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.goToDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findInCurrentProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findInCurrentDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.foldingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextFileOpMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewFrameworkObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.lstProjectFiles = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeFrameworkClasses = new System.Windows.Forms.TreeView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton14 = new System.Windows.Forms.ToolStripButton();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.tabEditorOptions = new System.Windows.Forms.TabControl();
            this.tabCompileErrors = new System.Windows.Forms.TabPage();
            this.lstCompileErrors = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabIntellisense = new System.Windows.Forms.TabPage();
            this.lstIntelliSense = new System.Windows.Forms.ListView();
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.tabGacEditorFindReplace = new System.Windows.Forms.TabPage();
            this.cbFindWholeWord = new System.Windows.Forms.CheckBox();
            this.cbFindMatchCase = new System.Windows.Forms.CheckBox();
            this.btnGacReplace = new System.Windows.Forms.Button();
            this.btnFindReplaceAll = new System.Windows.Forms.Button();
            this.btnFindPrevious = new System.Windows.Forms.Button();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.comboFindFormat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboFindLocation = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboGacReplaceText = new System.Windows.Forms.ComboBox();
            this.cbGacEditorEnableReplaceWidth = new System.Windows.Forms.CheckBox();
            this.comboGacTextToFind = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabFindResults = new System.Windows.Forms.TabPage();
            this.lstFindResults = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbSearchedText = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton7 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.newFrameworkObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.addExistingFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton11 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton10 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton9 = new System.Windows.Forms.ToolStripDropDownButton();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadGACDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadCErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.runInVisualStudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.comboUseCompilerUseBuildSettings = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton12 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton13 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton15 = new System.Windows.Forms.ToolStripButton();
            this.sourceTabContextMenu.SuspendLayout();
            this.editorContextMenu.SuspendLayout();
            this.contextFileOpMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            this.tabEditorOptions.SuspendLayout();
            this.tabCompileErrors.SuspendLayout();
            this.tabIntellisense.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabGacEditorFindReplace.SuspendLayout();
            this.tabFindResults.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.SuspendLayout();
            // 
            // GACFileIcons
            // 
            this.GACFileIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("GACFileIcons.ImageStream")));
            this.GACFileIcons.TransparentColor = System.Drawing.Color.Magenta;
            this.GACFileIcons.Images.SetKeyName(0, "Application");
            this.GACFileIcons.Images.SetKeyName(1, "Class");
            this.GACFileIcons.Images.SetKeyName(2, "Control");
            this.GACFileIcons.Images.SetKeyName(3, "Global");
            this.GACFileIcons.Images.SetKeyName(4, "Scene");
            // 
            // sourceTabContextMenu
            // 
            this.sourceTabContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem1,
            this.closeAllExceptCurrentToolStripMenuItem,
            this.toolStripMenuItem13,
            this.reloadToolStripMenuItem,
            this.mnuSaveCurentTab});
            this.sourceTabContextMenu.Name = "sourceTabContextMenu";
            this.sourceTabContextMenu.Size = new System.Drawing.Size(201, 98);
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem1.Image")));
            this.closeToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.closeToolStripMenuItem1.Text = "Close";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.OnCloseCurrentSourceTab);
            // 
            // closeAllExceptCurrentToolStripMenuItem
            // 
            this.closeAllExceptCurrentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeAllExceptCurrentToolStripMenuItem.Image")));
            this.closeAllExceptCurrentToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeAllExceptCurrentToolStripMenuItem.Name = "closeAllExceptCurrentToolStripMenuItem";
            this.closeAllExceptCurrentToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.closeAllExceptCurrentToolStripMenuItem.Text = "Close All Except Current";
            this.closeAllExceptCurrentToolStripMenuItem.Click += new System.EventHandler(this.OnCloseAllSourceTabExceptCurrent);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(197, 6);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reloadToolStripMenuItem.Image")));
            this.reloadToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.OnReloadCurrentSourceTab);
            // 
            // mnuSaveCurentTab
            // 
            this.mnuSaveCurentTab.Image = ((System.Drawing.Image)(resources.GetObject("mnuSaveCurentTab.Image")));
            this.mnuSaveCurentTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuSaveCurentTab.Name = "mnuSaveCurentTab";
            this.mnuSaveCurentTab.Size = new System.Drawing.Size(200, 22);
            this.mnuSaveCurentTab.Text = "Save";
            this.mnuSaveCurentTab.Click += new System.EventHandler(this.OnSaveCurentTab);
            // 
            // sourceFileIcons
            // 
            this.sourceFileIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("sourceFileIcons.ImageStream")));
            this.sourceFileIcons.TransparentColor = System.Drawing.Color.Magenta;
            this.sourceFileIcons.Images.SetKeyName(0, "save");
            // 
            // intellisenseTimer
            // 
            this.intellisenseTimer.Interval = 1000;
            this.intellisenseTimer.Tick += new System.EventHandler(this.OnUpdateIntelliSense);
            // 
            // GACIcons
            // 
            this.GACIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("GACIcons.ImageStream")));
            this.GACIcons.TransparentColor = System.Drawing.Color.Fuchsia;
            this.GACIcons.Images.SetKeyName(0, "class");
            this.GACIcons.Images.SetKeyName(1, "namespace");
            this.GACIcons.Images.SetKeyName(2, "enum");
            this.GACIcons.Images.SetKeyName(3, "variable");
            this.GACIcons.Images.SetKeyName(4, "constant");
            this.GACIcons.Images.SetKeyName(5, "function");
            this.GACIcons.Images.SetKeyName(6, "static_function");
            this.GACIcons.Images.SetKeyName(7, "basic_type");
            this.GACIcons.Images.SetKeyName(8, "static_class");
            this.GACIcons.Images.SetKeyName(9, "static_variable");
            this.GACIcons.Images.SetKeyName(10, "resource");
            this.GACIcons.Images.SetKeyName(11, "settings");
            this.GACIcons.Images.SetKeyName(12, "api");
            this.GACIcons.Images.SetKeyName(13, "preprocess");
            this.GACIcons.Images.SetKeyName(14, "keyword");
            // 
            // editorContextMenu
            // 
            this.editorContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripMenuItem2,
            this.goToDefinitionToolStripMenuItem,
            this.findInCurrentProjectToolStripMenuItem,
            this.findInCurrentDocumentToolStripMenuItem,
            this.toolStripMenuItem3,
            this.foldingToolStripMenuItem});
            this.editorContextMenu.Name = "sourceTabContextMenu";
            this.editorContextMenu.Size = new System.Drawing.Size(250, 264);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.selectAllToolStripMenuItem.Tag = "SelectAll";
            this.selectAllToolStripMenuItem.Text = "SelectAll";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.copyToolStripMenuItem.Tag = "Copy";
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.cutToolStripMenuItem.Tag = "Cut";
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.pasteToolStripMenuItem.Tag = "Paste";
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(246, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.undoToolStripMenuItem.Tag = "Undo";
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.redoToolStripMenuItem.Tag = "Redo";
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(246, 6);
            // 
            // goToDefinitionToolStripMenuItem
            // 
            this.goToDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("goToDefinitionToolStripMenuItem.Image")));
            this.goToDefinitionToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.goToDefinitionToolStripMenuItem.Name = "goToDefinitionToolStripMenuItem";
            this.goToDefinitionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.goToDefinitionToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.goToDefinitionToolStripMenuItem.Text = "GoTo Definition                      ";
            this.goToDefinitionToolStripMenuItem.Click += new System.EventHandler(this.OnGoToDefinitionForCurrentWord);
            // 
            // findInCurrentProjectToolStripMenuItem
            // 
            this.findInCurrentProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("findInCurrentProjectToolStripMenuItem.Image")));
            this.findInCurrentProjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.findInCurrentProjectToolStripMenuItem.Name = "findInCurrentProjectToolStripMenuItem";
            this.findInCurrentProjectToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.findInCurrentProjectToolStripMenuItem.Tag = "FindAllInCurrentProject";
            this.findInCurrentProjectToolStripMenuItem.Text = "Find in current project";
            this.findInCurrentProjectToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // findInCurrentDocumentToolStripMenuItem
            // 
            this.findInCurrentDocumentToolStripMenuItem.Name = "findInCurrentDocumentToolStripMenuItem";
            this.findInCurrentDocumentToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.findInCurrentDocumentToolStripMenuItem.Tag = "FindAllInCurrentDocument";
            this.findInCurrentDocumentToolStripMenuItem.Text = "Find in current document";
            this.findInCurrentDocumentToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(246, 6);
            // 
            // foldingToolStripMenuItem
            // 
            this.foldingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.collapseToDefinitionsToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.expandAllToolStripMenuItem});
            this.foldingToolStripMenuItem.Name = "foldingToolStripMenuItem";
            this.foldingToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.foldingToolStripMenuItem.Text = "Folding";
            // 
            // collapseToDefinitionsToolStripMenuItem
            // 
            this.collapseToDefinitionsToolStripMenuItem.Name = "collapseToDefinitionsToolStripMenuItem";
            this.collapseToDefinitionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D0)));
            this.collapseToDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.collapseToDefinitionsToolStripMenuItem.Tag = "CollapseToDefinitions";
            this.collapseToDefinitionsToolStripMenuItem.Text = "Collapse to Definitions";
            this.collapseToDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+Minus";
            this.collapseAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.OemMinus)));
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.collapseAllToolStripMenuItem.Tag = "CollapseAll";
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+Plus";
            this.expandAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Oemplus)));
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.expandAllToolStripMenuItem.Tag = "ExpandAll";
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuCommand);
            // 
            // contextFileOpMenu
            // 
            this.contextFileOpMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.addNewControlToolStripMenuItem,
            this.addNewFrameworkObjectToolStripMenuItem,
            this.addNewClassToolStripMenuItem,
            this.toolStripMenuItem5,
            this.deleteToolStripMenuItem1,
            this.renameToolStripMenuItem});
            this.contextFileOpMenu.Name = "sourceTabContextMenu";
            this.contextFileOpMenu.Size = new System.Drawing.Size(220, 142);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem4.Image")));
            this.toolStripMenuItem4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem4.Text = "Add new &Scene";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.OnAddNewGACScene);
            // 
            // addNewControlToolStripMenuItem
            // 
            this.addNewControlToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addNewControlToolStripMenuItem.Image")));
            this.addNewControlToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.addNewControlToolStripMenuItem.Name = "addNewControlToolStripMenuItem";
            this.addNewControlToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.addNewControlToolStripMenuItem.Text = "Add new &Control";
            this.addNewControlToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewGACControl);
            // 
            // addNewFrameworkObjectToolStripMenuItem
            // 
            this.addNewFrameworkObjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addNewFrameworkObjectToolStripMenuItem.Image")));
            this.addNewFrameworkObjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.addNewFrameworkObjectToolStripMenuItem.Name = "addNewFrameworkObjectToolStripMenuItem";
            this.addNewFrameworkObjectToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.addNewFrameworkObjectToolStripMenuItem.Text = "Add new &Framework object";
            this.addNewFrameworkObjectToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewGACFrameworkObject);
            // 
            // addNewClassToolStripMenuItem
            // 
            this.addNewClassToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addNewClassToolStripMenuItem.Image")));
            this.addNewClassToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.addNewClassToolStripMenuItem.Name = "addNewClassToolStripMenuItem";
            this.addNewClassToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.addNewClassToolStripMenuItem.Text = "Add new class";
            this.addNewClassToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewGACClass);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(216, 6);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem1.Image")));
            this.deleteToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(219, 22);
            this.deleteToolStripMenuItem1.Text = "&Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.OnDeleteGACFile);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("renameToolStripMenuItem.Image")));
            this.renameToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.renameToolStripMenuItem.Text = "&Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.OnRenameGACFile);
            // 
            // splitContainer5
            // 
            this.splitContainer5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer5.Location = new System.Drawing.Point(0, 38);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.splitContainer7);
            this.splitContainer5.Size = new System.Drawing.Size(1481, 722);
            this.splitContainer5.SplitterDistance = 229;
            this.splitContainer5.TabIndex = 2;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(227, 720);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.lstProjectFiles);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(219, 694);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Source Files";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // lstProjectFiles
            // 
            this.lstProjectFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader14});
            this.lstProjectFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstProjectFiles.FullRowSelect = true;
            this.lstProjectFiles.GridLines = true;
            listViewGroup1.Header = "C++ Header Files";
            listViewGroup1.Name = "h";
            listViewGroup2.Header = "C++ Source Files";
            listViewGroup2.Name = "cpp";
            listViewGroup3.Header = "Scenes";
            listViewGroup3.Name = "Scene";
            listViewGroup4.Header = "App files";
            listViewGroup4.Name = "Application";
            listViewGroup5.Header = "Custom Controls";
            listViewGroup5.Name = "Control";
            listViewGroup6.Header = "Classes";
            listViewGroup6.Name = "Class";
            listViewGroup7.Header = "Framework Object";
            listViewGroup7.Name = "FrameworkObject";
            this.lstProjectFiles.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7});
            this.lstProjectFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstProjectFiles.Location = new System.Drawing.Point(3, 3);
            this.lstProjectFiles.MultiSelect = false;
            this.lstProjectFiles.Name = "lstProjectFiles";
            this.lstProjectFiles.Size = new System.Drawing.Size(213, 688);
            this.lstProjectFiles.SmallImageList = this.GACFileIcons;
            this.lstProjectFiles.TabIndex = 0;
            this.lstProjectFiles.UseCompatibleStateImageBehavior = false;
            this.lstProjectFiles.View = System.Windows.Forms.View.Details;
            this.lstProjectFiles.DoubleClick += new System.EventHandler(this.OnDblClickOnProjectFiles);
            this.lstProjectFiles.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClickOnFileList);
            this.lstProjectFiles.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUpOnFileList);
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "File Name";
            this.columnHeader9.Width = 160;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Errors";
            this.columnHeader14.Width = 40;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeFrameworkClasses);
            this.tabPage1.Controls.Add(this.toolStrip2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(219, 694);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Api Browser";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeFrameworkClasses
            // 
            this.treeFrameworkClasses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFrameworkClasses.FullRowSelect = true;
            this.treeFrameworkClasses.ImageIndex = 12;
            this.treeFrameworkClasses.ImageList = this.GACIcons;
            this.treeFrameworkClasses.Location = new System.Drawing.Point(3, 41);
            this.treeFrameworkClasses.Name = "treeFrameworkClasses";
            this.treeFrameworkClasses.SelectedImageIndex = 12;
            this.treeFrameworkClasses.Size = new System.Drawing.Size(213, 650);
            this.treeFrameworkClasses.TabIndex = 1;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton14,
            this.toolStripButton15});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(213, 38);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.TabStop = true;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton14
            // 
            this.toolStripButton14.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton14.Image")));
            this.toolStripButton14.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton14.Name = "toolStripButton14";
            this.toolStripButton14.Size = new System.Drawing.Size(50, 35);
            this.toolStripButton14.Text = "Refresh";
            this.toolStripButton14.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton14.Click += new System.EventHandler(this.OnRefreshFrameworkClasses);
            // 
            // splitContainer7
            // 
            this.splitContainer7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer7.Location = new System.Drawing.Point(0, 0);
            this.splitContainer7.Name = "splitContainer7";
            this.splitContainer7.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.tabEditorOptions);
            this.splitContainer7.Size = new System.Drawing.Size(1248, 722);
            this.splitContainer7.SplitterDistance = 537;
            this.splitContainer7.TabIndex = 0;
            // 
            // tabEditorOptions
            // 
            this.tabEditorOptions.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabEditorOptions.Controls.Add(this.tabCompileErrors);
            this.tabEditorOptions.Controls.Add(this.tabIntellisense);
            this.tabEditorOptions.Controls.Add(this.tabPage8);
            this.tabEditorOptions.Controls.Add(this.tabGacEditorFindReplace);
            this.tabEditorOptions.Controls.Add(this.tabFindResults);
            this.tabEditorOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabEditorOptions.Location = new System.Drawing.Point(0, 0);
            this.tabEditorOptions.Name = "tabEditorOptions";
            this.tabEditorOptions.SelectedIndex = 0;
            this.tabEditorOptions.Size = new System.Drawing.Size(1246, 179);
            this.tabEditorOptions.TabIndex = 1;
            // 
            // tabCompileErrors
            // 
            this.tabCompileErrors.Controls.Add(this.lstCompileErrors);
            this.tabCompileErrors.Location = new System.Drawing.Point(4, 4);
            this.tabCompileErrors.Name = "tabCompileErrors";
            this.tabCompileErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tabCompileErrors.Size = new System.Drawing.Size(1238, 153);
            this.tabCompileErrors.TabIndex = 0;
            this.tabCompileErrors.Text = "Errors";
            this.tabCompileErrors.UseVisualStyleBackColor = true;
            // 
            // lstCompileErrors
            // 
            this.lstCompileErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11});
            this.lstCompileErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCompileErrors.FullRowSelect = true;
            this.lstCompileErrors.GridLines = true;
            this.lstCompileErrors.HideSelection = false;
            this.lstCompileErrors.Location = new System.Drawing.Point(3, 3);
            this.lstCompileErrors.MultiSelect = false;
            this.lstCompileErrors.Name = "lstCompileErrors";
            this.lstCompileErrors.Size = new System.Drawing.Size(1232, 147);
            this.lstCompileErrors.TabIndex = 0;
            this.lstCompileErrors.UseCompatibleStateImageBehavior = false;
            this.lstCompileErrors.View = System.Windows.Forms.View.Details;
            this.lstCompileErrors.DoubleClick += new System.EventHandler(this.OnErrorDoubleClick);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Line number";
            this.columnHeader10.Width = 100;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Error";
            this.columnHeader11.Width = 1500;
            // 
            // tabIntellisense
            // 
            this.tabIntellisense.Controls.Add(this.lstIntelliSense);
            this.tabIntellisense.Location = new System.Drawing.Point(4, 4);
            this.tabIntellisense.Name = "tabIntellisense";
            this.tabIntellisense.Size = new System.Drawing.Size(1236, 153);
            this.tabIntellisense.TabIndex = 1;
            this.tabIntellisense.Text = "Intellisense";
            this.tabIntellisense.UseVisualStyleBackColor = true;
            // 
            // lstIntelliSense
            // 
            this.lstIntelliSense.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader12,
            this.columnHeader13});
            this.lstIntelliSense.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstIntelliSense.FullRowSelect = true;
            this.lstIntelliSense.GridLines = true;
            this.lstIntelliSense.HideSelection = false;
            this.lstIntelliSense.Location = new System.Drawing.Point(0, 0);
            this.lstIntelliSense.MultiSelect = false;
            this.lstIntelliSense.Name = "lstIntelliSense";
            this.lstIntelliSense.Size = new System.Drawing.Size(1236, 153);
            this.lstIntelliSense.TabIndex = 1;
            this.lstIntelliSense.UseCompatibleStateImageBehavior = false;
            this.lstIntelliSense.View = System.Windows.Forms.View.Details;
            this.lstIntelliSense.DoubleClick += new System.EventHandler(this.OnIntellisenseDoubleClick);
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Line number";
            this.columnHeader12.Width = 100;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Error";
            this.columnHeader13.Width = 1500;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.txtOutput);
            this.tabPage8.Location = new System.Drawing.Point(4, 4);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(1236, 153);
            this.tabPage8.TabIndex = 2;
            this.tabPage8.Text = "Output";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(0, 0);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(1236, 153);
            this.txtOutput.TabIndex = 0;
            // 
            // tabGacEditorFindReplace
            // 
            this.tabGacEditorFindReplace.Controls.Add(this.cbFindWholeWord);
            this.tabGacEditorFindReplace.Controls.Add(this.cbFindMatchCase);
            this.tabGacEditorFindReplace.Controls.Add(this.btnGacReplace);
            this.tabGacEditorFindReplace.Controls.Add(this.btnFindReplaceAll);
            this.tabGacEditorFindReplace.Controls.Add(this.btnFindPrevious);
            this.tabGacEditorFindReplace.Controls.Add(this.btnFindNext);
            this.tabGacEditorFindReplace.Controls.Add(this.comboFindFormat);
            this.tabGacEditorFindReplace.Controls.Add(this.label3);
            this.tabGacEditorFindReplace.Controls.Add(this.comboFindLocation);
            this.tabGacEditorFindReplace.Controls.Add(this.label2);
            this.tabGacEditorFindReplace.Controls.Add(this.comboGacReplaceText);
            this.tabGacEditorFindReplace.Controls.Add(this.cbGacEditorEnableReplaceWidth);
            this.tabGacEditorFindReplace.Controls.Add(this.comboGacTextToFind);
            this.tabGacEditorFindReplace.Controls.Add(this.label1);
            this.tabGacEditorFindReplace.Location = new System.Drawing.Point(4, 4);
            this.tabGacEditorFindReplace.Name = "tabGacEditorFindReplace";
            this.tabGacEditorFindReplace.Padding = new System.Windows.Forms.Padding(3);
            this.tabGacEditorFindReplace.Size = new System.Drawing.Size(1236, 153);
            this.tabGacEditorFindReplace.TabIndex = 3;
            this.tabGacEditorFindReplace.Text = "Find / Replace";
            this.tabGacEditorFindReplace.UseVisualStyleBackColor = true;
            this.tabGacEditorFindReplace.Enter += new System.EventHandler(this.OnActivateGacFindDialog);
            this.tabGacEditorFindReplace.Leave += new System.EventHandler(this.OnLeaveGacFindDialog);
            // 
            // cbFindWholeWord
            // 
            this.cbFindWholeWord.AutoSize = true;
            this.cbFindWholeWord.Location = new System.Drawing.Point(9, 94);
            this.cbFindWholeWord.Name = "cbFindWholeWord";
            this.cbFindWholeWord.Size = new System.Drawing.Size(135, 17);
            this.cbFindWholeWord.TabIndex = 9;
            this.cbFindWholeWord.Text = "Match &whole word only";
            this.cbFindWholeWord.UseVisualStyleBackColor = true;
            // 
            // cbFindMatchCase
            // 
            this.cbFindMatchCase.AutoSize = true;
            this.cbFindMatchCase.Location = new System.Drawing.Point(9, 71);
            this.cbFindMatchCase.Name = "cbFindMatchCase";
            this.cbFindMatchCase.Size = new System.Drawing.Size(82, 17);
            this.cbFindMatchCase.TabIndex = 8;
            this.cbFindMatchCase.Text = "Match &case";
            this.cbFindMatchCase.UseVisualStyleBackColor = true;
            // 
            // btnGacReplace
            // 
            this.btnGacReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGacReplace.Enabled = false;
            this.btnGacReplace.Location = new System.Drawing.Point(955, 114);
            this.btnGacReplace.Name = "btnGacReplace";
            this.btnGacReplace.Size = new System.Drawing.Size(88, 33);
            this.btnGacReplace.TabIndex = 7;
            this.btnGacReplace.Text = "&Replace";
            this.btnGacReplace.UseVisualStyleBackColor = true;
            // 
            // btnFindReplaceAll
            // 
            this.btnFindReplaceAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindReplaceAll.Location = new System.Drawing.Point(861, 114);
            this.btnFindReplaceAll.Name = "btnFindReplaceAll";
            this.btnFindReplaceAll.Size = new System.Drawing.Size(88, 33);
            this.btnFindReplaceAll.TabIndex = 6;
            this.btnFindReplaceAll.Text = "Find &All";
            this.btnFindReplaceAll.UseVisualStyleBackColor = true;
            this.btnFindReplaceAll.Click += new System.EventHandler(this.OnFindReplaceAll);
            // 
            // btnFindPrevious
            // 
            this.btnFindPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindPrevious.Location = new System.Drawing.Point(1049, 114);
            this.btnFindPrevious.Name = "btnFindPrevious";
            this.btnFindPrevious.Size = new System.Drawing.Size(88, 33);
            this.btnFindPrevious.TabIndex = 5;
            this.btnFindPrevious.Text = "Find &Previous";
            this.btnFindPrevious.UseVisualStyleBackColor = true;
            this.btnFindPrevious.Click += new System.EventHandler(this.OnFindPrevious);
            // 
            // btnFindNext
            // 
            this.btnFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindNext.Location = new System.Drawing.Point(1143, 114);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(88, 33);
            this.btnFindNext.TabIndex = 4;
            this.btnFindNext.Text = "&Find Next";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.OnFindNext);
            // 
            // comboFindFormat
            // 
            this.comboFindFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboFindFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFindFormat.FormattingEnabled = true;
            this.comboFindFormat.Items.AddRange(new object[] {
            "Normal search",
            "Extended (\\n, \\r, \\t, ...)",
            "Regular expression"});
            this.comboFindFormat.Location = new System.Drawing.Point(1039, 7);
            this.comboFindFormat.Name = "comboFindFormat";
            this.comboFindFormat.Size = new System.Drawing.Size(192, 21);
            this.comboFindFormat.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(984, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Format";
            // 
            // comboFindLocation
            // 
            this.comboFindLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboFindLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFindLocation.FormattingEnabled = true;
            this.comboFindLocation.Items.AddRange(new object[] {
            "Current document",
            "Current project",
            "All opened documents"});
            this.comboFindLocation.Location = new System.Drawing.Point(1039, 32);
            this.comboFindLocation.Name = "comboFindLocation";
            this.comboFindLocation.Size = new System.Drawing.Size(192, 21);
            this.comboFindLocation.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(984, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Location";
            // 
            // comboGacReplaceText
            // 
            this.comboGacReplaceText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboGacReplaceText.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboGacReplaceText.Enabled = false;
            this.comboGacReplaceText.FormattingEnabled = true;
            this.comboGacReplaceText.Location = new System.Drawing.Point(102, 32);
            this.comboGacReplaceText.Name = "comboGacReplaceText";
            this.comboGacReplaceText.Size = new System.Drawing.Size(857, 21);
            this.comboGacReplaceText.TabIndex = 1;
            this.comboGacReplaceText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboGacReplaceText_KeyPress);
            // 
            // cbGacEditorEnableReplaceWidth
            // 
            this.cbGacEditorEnableReplaceWidth.AutoSize = true;
            this.cbGacEditorEnableReplaceWidth.Location = new System.Drawing.Point(9, 34);
            this.cbGacEditorEnableReplaceWidth.Name = "cbGacEditorEnableReplaceWidth";
            this.cbGacEditorEnableReplaceWidth.Size = new System.Drawing.Size(88, 17);
            this.cbGacEditorEnableReplaceWidth.TabIndex = 2;
            this.cbGacEditorEnableReplaceWidth.Text = "Replace with";
            this.cbGacEditorEnableReplaceWidth.UseVisualStyleBackColor = true;
            this.cbGacEditorEnableReplaceWidth.CheckedChanged += new System.EventHandler(this.OnEnableReplaceWidth);
            // 
            // comboGacTextToFind
            // 
            this.comboGacTextToFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboGacTextToFind.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboGacTextToFind.FormattingEnabled = true;
            this.comboGacTextToFind.Location = new System.Drawing.Point(102, 7);
            this.comboGacTextToFind.Name = "comboGacTextToFind";
            this.comboGacTextToFind.Size = new System.Drawing.Size(857, 21);
            this.comboGacTextToFind.TabIndex = 0;
            this.comboGacTextToFind.TextChanged += new System.EventHandler(this.OnTextToFindChanges);
            this.comboGacTextToFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboGacTextToFind_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text to find";
            // 
            // tabFindResults
            // 
            this.tabFindResults.Controls.Add(this.lstFindResults);
            this.tabFindResults.Controls.Add(this.toolStrip1);
            this.tabFindResults.Location = new System.Drawing.Point(4, 4);
            this.tabFindResults.Name = "tabFindResults";
            this.tabFindResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabFindResults.Size = new System.Drawing.Size(1236, 153);
            this.tabFindResults.TabIndex = 4;
            this.tabFindResults.Text = "Find Results";
            this.tabFindResults.UseVisualStyleBackColor = true;
            // 
            // lstFindResults
            // 
            this.lstFindResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3});
            this.lstFindResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFindResults.FullRowSelect = true;
            this.lstFindResults.GridLines = true;
            this.lstFindResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstFindResults.HideSelection = false;
            this.lstFindResults.Location = new System.Drawing.Point(3, 28);
            this.lstFindResults.Margin = new System.Windows.Forms.Padding(0);
            this.lstFindResults.MultiSelect = false;
            this.lstFindResults.Name = "lstFindResults";
            this.lstFindResults.Size = new System.Drawing.Size(1230, 122);
            this.lstFindResults.TabIndex = 2;
            this.lstFindResults.UseCompatibleStateImageBehavior = false;
            this.lstFindResults.View = System.Windows.Forms.View.Details;
            this.lstFindResults.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnClickOnFindResult);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Line Number";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Search for ";
            this.columnHeader3.Width = 1000;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.lbSearchedText});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1230, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "Next";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Previous";
            this.toolStripButton1.Click += new System.EventHandler(this.OnPreviousStringFound);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Next";
            this.toolStripButton2.Click += new System.EventHandler(this.OnNextStringFound);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // lbSearchedText
            // 
            this.lbSearchedText.Name = "lbSearchedText";
            this.lbSearchedText.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton7,
            this.toolStripButton7,
            this.toolStripButton11,
            this.toolStripSeparator7,
            this.toolStripButton8,
            this.toolStripButton10,
            this.toolStripSeparator6,
            this.toolStripButton9,
            this.toolStripSeparator13,
            this.toolStripLabel7,
            this.comboUseCompilerUseBuildSettings,
            this.toolStripSeparator2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton12,
            this.toolStripSeparator3,
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripButton13});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(1481, 38);
            this.toolStrip4.TabIndex = 1;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // toolStripDropDownButton7
            // 
            this.toolStripDropDownButton7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.newFrameworkObjectToolStripMenuItem,
            this.toolStripMenuItem10,
            this.toolStripMenuItem12,
            this.addExistingFileToolStripMenuItem});
            this.toolStripDropDownButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton7.Image")));
            this.toolStripDropDownButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton7.Name = "toolStripDropDownButton7";
            this.toolStripDropDownButton7.Size = new System.Drawing.Size(63, 35);
            this.toolStripDropDownButton7.Text = "New file";
            this.toolStripDropDownButton7.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem8.Image")));
            this.toolStripMenuItem8.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(196, 22);
            this.toolStripMenuItem8.Text = "New Scene";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.OnAddNewGACScene);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem9.Image")));
            this.toolStripMenuItem9.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(196, 22);
            this.toolStripMenuItem9.Text = "New Control";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.OnAddNewGACControl);
            // 
            // newFrameworkObjectToolStripMenuItem
            // 
            this.newFrameworkObjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newFrameworkObjectToolStripMenuItem.Image")));
            this.newFrameworkObjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.newFrameworkObjectToolStripMenuItem.Name = "newFrameworkObjectToolStripMenuItem";
            this.newFrameworkObjectToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.newFrameworkObjectToolStripMenuItem.Text = "New Framework object";
            this.newFrameworkObjectToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewGACFrameworkObject);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem10.Image")));
            this.toolStripMenuItem10.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(196, 22);
            this.toolStripMenuItem10.Text = "New Class";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.OnAddNewGACClass);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(193, 6);
            // 
            // addExistingFileToolStripMenuItem
            // 
            this.addExistingFileToolStripMenuItem.Enabled = false;
            this.addExistingFileToolStripMenuItem.Name = "addExistingFileToolStripMenuItem";
            this.addExistingFileToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.addExistingFileToolStripMenuItem.Text = "Add existing file";
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(44, 35);
            this.toolStripButton7.Text = "Delete";
            this.toolStripButton7.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton7.Click += new System.EventHandler(this.OnDeleteGACFile);
            // 
            // toolStripButton11
            // 
            this.toolStripButton11.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton11.Image")));
            this.toolStripButton11.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton11.Name = "toolStripButton11";
            this.toolStripButton11.Size = new System.Drawing.Size(54, 35);
            this.toolStripButton11.Text = "Rename";
            this.toolStripButton11.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton11.Click += new System.EventHandler(this.OnRenameGACFile);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(35, 35);
            this.toolStripButton8.Text = "Save";
            this.toolStripButton8.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton8.Click += new System.EventHandler(this.OnSaveCurentTab);
            // 
            // toolStripButton10
            // 
            this.toolStripButton10.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton10.Image")));
            this.toolStripButton10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton10.Name = "toolStripButton10";
            this.toolStripButton10.Size = new System.Drawing.Size(52, 35);
            this.toolStripButton10.Text = "Save All";
            this.toolStripButton10.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton10.Click += new System.EventHandler(this.OnSaveAllSources);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.quickRunToolStripMenuItem,
            this.compileToolStripMenuItem,
            this.toolStripMenuItem11,
            this.reloadGACDefinitionsToolStripMenuItem,
            this.reloadCErrorsToolStripMenuItem,
            this.toolStripMenuItem6,
            this.runInVisualStudioToolStripMenuItem});
            this.toolStripButton9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton9.Image")));
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(41, 35);
            this.toolStripButton9.Text = "Run";
            this.toolStripButton9.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeyDisplayString = "F5";
            this.runToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.OnRun);
            // 
            // quickRunToolStripMenuItem
            // 
            this.quickRunToolStripMenuItem.Name = "quickRunToolStripMenuItem";
            this.quickRunToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.quickRunToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.quickRunToolStripMenuItem.Text = "Quick Run";
            this.quickRunToolStripMenuItem.Click += new System.EventHandler(this.OnQuickRun);
            // 
            // compileToolStripMenuItem
            // 
            this.compileToolStripMenuItem.Name = "compileToolStripMenuItem";
            this.compileToolStripMenuItem.ShortcutKeyDisplayString = "F7";
            this.compileToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.compileToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.compileToolStripMenuItem.Text = "Compile";
            this.compileToolStripMenuItem.Click += new System.EventHandler(this.OnCompile);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(195, 6);
            // 
            // reloadGACDefinitionsToolStripMenuItem
            // 
            this.reloadGACDefinitionsToolStripMenuItem.Name = "reloadGACDefinitionsToolStripMenuItem";
            this.reloadGACDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.reloadGACDefinitionsToolStripMenuItem.Text = "Reload GAC Definitions";
            this.reloadGACDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.OnReloadDefinitions);
            // 
            // reloadCErrorsToolStripMenuItem
            // 
            this.reloadCErrorsToolStripMenuItem.Name = "reloadCErrorsToolStripMenuItem";
            this.reloadCErrorsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.reloadCErrorsToolStripMenuItem.Text = "Reload C++ Errors";
            this.reloadCErrorsToolStripMenuItem.Click += new System.EventHandler(this.OnReloadCppErrors);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(195, 6);
            // 
            // runInVisualStudioToolStripMenuItem
            // 
            this.runInVisualStudioToolStripMenuItem.Name = "runInVisualStudioToolStripMenuItem";
            this.runInVisualStudioToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.runInVisualStudioToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.runInVisualStudioToolStripMenuItem.Text = "Run in Visual Studio";
            this.runInVisualStudioToolStripMenuItem.Click += new System.EventHandler(this.OnRunInVisualStudio);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(118, 35);
            this.toolStripLabel7.Text = "Use Build settings for";
            // 
            // comboUseCompilerUseBuildSettings
            // 
            this.comboUseCompilerUseBuildSettings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUseCompilerUseBuildSettings.DropDownWidth = 150;
            this.comboUseCompilerUseBuildSettings.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.comboUseCompilerUseBuildSettings.Name = "comboUseCompilerUseBuildSettings";
            this.comboUseCompilerUseBuildSettings.Size = new System.Drawing.Size(150, 38);
            this.comboUseCompilerUseBuildSettings.SelectedIndexChanged += new System.EventHandler(this.OnChangeBuildToUseForCodeSettings);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(111, 35);
            this.toolStripButton3.Text = "Debug Commands";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton3.Click += new System.EventHandler(this.OnShowDebugCommandsWindow);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(70, 35);
            this.toolStripButton4.Text = "Control IDs";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton4.Click += new System.EventHandler(this.OnShowControlIDs);
            // 
            // toolStripButton12
            // 
            this.toolStripButton12.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton12.Image")));
            this.toolStripButton12.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton12.Name = "toolStripButton12";
            this.toolStripButton12.Size = new System.Drawing.Size(45, 35);
            this.toolStripButton12.Text = "Events";
            this.toolStripButton12.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton12.Click += new System.EventHandler(this.OnShowObjectEvents);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(65, 35);
            this.toolStripButton5.Text = "Comment";
            this.toolStripButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton5.ToolTipText = "Comment (Ctrl+Alt+C)";
            this.toolStripButton5.Click += new System.EventHandler(this.OnComment);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(80, 35);
            this.toolStripButton6.Text = "UnComment";
            this.toolStripButton6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton6.ToolTipText = "UnComment (Ctrl+Alt+U)";
            this.toolStripButton6.Click += new System.EventHandler(this.OnUnComment);
            // 
            // toolStripButton13
            // 
            this.toolStripButton13.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton13.Image")));
            this.toolStripButton13.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton13.Name = "toolStripButton13";
            this.toolStripButton13.Size = new System.Drawing.Size(101, 35);
            this.toolStripButton13.Text = "Search Definition";
            this.toolStripButton13.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton13.ToolTipText = "Search Definition (Ctrl+K)";
            this.toolStripButton13.Click += new System.EventHandler(this.OnGoToDefinition);
            // 
            // toolStripButton15
            // 
            this.toolStripButton15.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton15.Image")));
            this.toolStripButton15.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton15.Name = "toolStripButton15";
            this.toolStripButton15.Size = new System.Drawing.Size(46, 35);
            this.toolStripButton15.Text = "Search";
            this.toolStripButton15.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton15.Click += new System.EventHandler(this.OnBrowseForAPI);
            // 
            // ProjectTabEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer5);
            this.Controls.Add(this.toolStrip4);
            this.Name = "ProjectTabEditor";
            this.Size = new System.Drawing.Size(1481, 760);
            this.sourceTabContextMenu.ResumeLayout(false);
            this.editorContextMenu.ResumeLayout(false);
            this.contextFileOpMenu.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            this.tabEditorOptions.ResumeLayout(false);
            this.tabCompileErrors.ResumeLayout(false);
            this.tabIntellisense.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.tabGacEditorFindReplace.ResumeLayout(false);
            this.tabGacEditorFindReplace.PerformLayout();
            this.tabFindResults.ResumeLayout(false);
            this.tabFindResults.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem newFrameworkObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem addExistingFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ToolStripButton toolStripButton10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton9;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickRunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem reloadGACDefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadCErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripComboBox comboUseCompilerUseBuildSettings;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ListView lstProjectFiles;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.SplitContainer splitContainer7;
        private System.Windows.Forms.TabControl tabEditorOptions;
        private System.Windows.Forms.TabPage tabCompileErrors;
        private System.Windows.Forms.ListView lstCompileErrors;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.TabPage tabIntellisense;
        private System.Windows.Forms.ListView lstIntelliSense;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.TabPage tabGacEditorFindReplace;
        private System.Windows.Forms.CheckBox cbFindWholeWord;
        private System.Windows.Forms.CheckBox cbFindMatchCase;
        private System.Windows.Forms.Button btnGacReplace;
        private System.Windows.Forms.Button btnFindReplaceAll;
        private System.Windows.Forms.Button btnFindPrevious;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.ComboBox comboFindFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboFindLocation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboGacReplaceText;
        private System.Windows.Forms.CheckBox cbGacEditorEnableReplaceWidth;
        private System.Windows.Forms.ComboBox comboGacTextToFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip sourceTabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeAllExceptCurrentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveCurentTab;
        private System.Windows.Forms.ImageList sourceFileIcons;
        private System.Windows.Forms.Timer intellisenseTimer;
        private System.Windows.Forms.ImageList GACIcons;
        private System.Windows.Forms.ContextMenuStrip editorContextMenu;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem foldingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseToDefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findInCurrentProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findInCurrentDocumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.TabPage tabFindResults;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lbSearchedText;
        private System.Windows.Forms.ListView lstFindResults;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ImageList GACFileIcons;
        private System.Windows.Forms.ToolStripButton toolStripButton11;
        private System.Windows.Forms.ContextMenuStrip contextFileOpMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem addNewControlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewFrameworkObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem runInVisualStudioToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton12;
        private System.Windows.Forms.ToolStripButton toolStripButton13;
        private System.Windows.Forms.ToolStripMenuItem goToDefinitionToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView treeFrameworkClasses;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton14;
        private System.Windows.Forms.ToolStripButton toolStripButton15;
    }
}
