namespace GAppCreator
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Standard Operations", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Recent projects", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Older projects", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "New project",
            "Create a new GAC Project"}, "__new__");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Load existing project",
            "Load an existing project from a local drive"}, "__load__");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.collapseToDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unCommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.goToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToDefinitionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.goToDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchDefinitionInCurrentfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.aPIBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripSeparator();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem21 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.goToNextErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.vacuumProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txMemoryStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbIDETimes = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbGeneralInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbTaskName = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbTaskProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lbInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainTab = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabPublisher = new System.Windows.Forms.TabPage();
            this.tabResources = new System.Windows.Forms.TabPage();
            this.tabProfiles = new System.Windows.Forms.TabPage();
            this.tabStrings = new System.Windows.Forms.TabPage();
            this.tabAds = new System.Windows.Forms.TabPage();
            this.tabConstants = new System.Windows.Forms.TabPage();
            this.tabCountersAndAlarms = new System.Windows.Forms.TabPage();
            this.tabAnimationObjects = new System.Windows.Forms.TabPage();
            this.tabCode = new System.Windows.Forms.TabPage();
            this.resImagesLarge = new System.Windows.Forms.ImageList(this.components);
            this.resImagesSmall = new System.Windows.Forms.ImageList(this.components);
            this.taskEventIcons = new System.Windows.Forms.ImageList(this.components);
            this.memoryStatusTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlMainScreen = new System.Windows.Forms.Panel();
            this.lstMainScreenOperation = new System.Windows.Forms.ListView();
            this.columnHeader33 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader34 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader35 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.iconMainScreen = new System.Windows.Forms.ImageList(this.components);
            this.timerIDETimes = new System.Windows.Forms.Timer(this.components);
            this.tabProjectMain = new GAppCreator.ProjectTabMain();
            this.tabProjectPublishMaterials = new GAppCreator.ProjectTabPublishMaterials();
            this.tabProjectResources = new GAppCreator.ProjectTabResources();
            this.tabProjectMemoryProfiles = new GAppCreator.ProjectTabMemoryProfiles();
            this.tabProjectStrings = new GAppCreator.ProjectTabStrings();
            this.tabProjectAds = new GAppCreator.ProjectTabAds();
            this.tabProjectConstants = new GAppCreator.ProjectTabConstants();
            this.tabProjectCountersAndTimers = new GAppCreator.ProjectTabCountersAndAlerts();
            this.tabProjectAnimationObjects = new GAppCreator.ProjectTabAnimation();
            this.tabProjectEditor = new GAppCreator.ProjectTabEditor();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabPublisher.SuspendLayout();
            this.tabResources.SuspendLayout();
            this.tabProfiles.SuspendLayout();
            this.tabStrings.SuspendLayout();
            this.tabAds.SuspendLayout();
            this.tabConstants.SuspendLayout();
            this.tabCountersAndAlarms.SuspendLayout();
            this.tabAnimationObjects.SuspendLayout();
            this.tabCode.SuspendLayout();
            this.pnlMainScreen.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.projectToolStripMenuItem,
            this.resourcesToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(2564, 35);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem19,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectToolStripMenuItem.Image")));
            this.newProjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(205, 30);
            this.newProjectToolStripMenuItem.Text = "New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.OnCreateNewProject);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(205, 30);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.OnSaveProject);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+O";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(205, 30);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OnLoadProject);
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.Size = new System.Drawing.Size(202, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(205, 30);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foldingToolStripMenuItem,
            this.codeToolStripMenuItem,
            this.toolStripMenuItem20,
            this.findToolStripMenuItem,
            this.findWindowsToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.replaceWindowToolStripMenuItem,
            this.toolStripMenuItem21,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // foldingToolStripMenuItem
            // 
            this.foldingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foldAllToolStripMenuItem,
            this.exToolStripMenuItem,
            this.toolStripMenuItem7,
            this.collapseToDefinitionsToolStripMenuItem});
            this.foldingToolStripMenuItem.Name = "foldingToolStripMenuItem";
            this.foldingToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.foldingToolStripMenuItem.Text = "Folding";
            // 
            // foldAllToolStripMenuItem
            // 
            this.foldAllToolStripMenuItem.Name = "foldAllToolStripMenuItem";
            this.foldAllToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+Minus";
            this.foldAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.OemMinus)));
            this.foldAllToolStripMenuItem.Size = new System.Drawing.Size(331, 30);
            this.foldAllToolStripMenuItem.Tag = "CollapseAll";
            this.foldAllToolStripMenuItem.Text = "Collapse All";
            this.foldAllToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // exToolStripMenuItem
            // 
            this.exToolStripMenuItem.Name = "exToolStripMenuItem";
            this.exToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+Plus";
            this.exToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Oemplus)));
            this.exToolStripMenuItem.Size = new System.Drawing.Size(331, 30);
            this.exToolStripMenuItem.Tag = "ExpandAll";
            this.exToolStripMenuItem.Text = "Expand All";
            this.exToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(328, 6);
            // 
            // collapseToDefinitionsToolStripMenuItem
            // 
            this.collapseToDefinitionsToolStripMenuItem.Name = "collapseToDefinitionsToolStripMenuItem";
            this.collapseToDefinitionsToolStripMenuItem.ShortcutKeyDisplayString = "Alt+0";
            this.collapseToDefinitionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D0)));
            this.collapseToDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(331, 30);
            this.collapseToDefinitionsToolStripMenuItem.Tag = "CollapseToDefinitions";
            this.collapseToDefinitionsToolStripMenuItem.Text = "Collapse to Definitions";
            this.collapseToDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // codeToolStripMenuItem
            // 
            this.codeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commentToolStripMenuItem,
            this.unCommentToolStripMenuItem,
            this.toolStripMenuItem3,
            this.goToLineToolStripMenuItem,
            this.goToDefinitionToolStripMenuItem1,
            this.goToDefinitionToolStripMenuItem,
            this.searchDefinitionInCurrentfileToolStripMenuItem,
            this.toolStripMenuItem4,
            this.aPIBrowserToolStripMenuItem});
            this.codeToolStripMenuItem.Name = "codeToolStripMenuItem";
            this.codeToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.codeToolStripMenuItem.Text = "Code";
            // 
            // commentToolStripMenuItem
            // 
            this.commentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("commentToolStripMenuItem.Image")));
            this.commentToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.commentToolStripMenuItem.Name = "commentToolStripMenuItem";
            this.commentToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+C";
            this.commentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.C)));
            this.commentToolStripMenuItem.Size = new System.Drawing.Size(432, 30);
            this.commentToolStripMenuItem.Tag = "Comment";
            this.commentToolStripMenuItem.Text = "Comment";
            this.commentToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // unCommentToolStripMenuItem
            // 
            this.unCommentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("unCommentToolStripMenuItem.Image")));
            this.unCommentToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.unCommentToolStripMenuItem.Name = "unCommentToolStripMenuItem";
            this.unCommentToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+U";
            this.unCommentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.U)));
            this.unCommentToolStripMenuItem.Size = new System.Drawing.Size(432, 30);
            this.unCommentToolStripMenuItem.Tag = "UnComment";
            this.unCommentToolStripMenuItem.Text = "UnComment";
            this.unCommentToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(429, 6);
            this.toolStripMenuItem3.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("goToLineToolStripMenuItem.Image")));
            this.goToLineToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            this.goToLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.goToLineToolStripMenuItem.Size = new System.Drawing.Size(432, 30);
            this.goToLineToolStripMenuItem.Tag = "GoToLine";
            this.goToLineToolStripMenuItem.Text = "GoTo Line";
            this.goToLineToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // goToDefinitionToolStripMenuItem1
            // 
            this.goToDefinitionToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("goToDefinitionToolStripMenuItem1.Image")));
            this.goToDefinitionToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.goToDefinitionToolStripMenuItem1.Name = "goToDefinitionToolStripMenuItem1";
            this.goToDefinitionToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.goToDefinitionToolStripMenuItem1.Size = new System.Drawing.Size(432, 30);
            this.goToDefinitionToolStripMenuItem1.Tag = "GoToDefinition";
            this.goToDefinitionToolStripMenuItem1.Text = "GoTo Definition / Preview";
            this.goToDefinitionToolStripMenuItem1.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // goToDefinitionToolStripMenuItem
            // 
            this.goToDefinitionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("goToDefinitionToolStripMenuItem.Image")));
            this.goToDefinitionToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goToDefinitionToolStripMenuItem.Name = "goToDefinitionToolStripMenuItem";
            this.goToDefinitionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.goToDefinitionToolStripMenuItem.Size = new System.Drawing.Size(432, 30);
            this.goToDefinitionToolStripMenuItem.Tag = "OpenGoToDefinitionDialog";
            this.goToDefinitionToolStripMenuItem.Text = "Search Definition";
            this.goToDefinitionToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // searchDefinitionInCurrentfileToolStripMenuItem
            // 
            this.searchDefinitionInCurrentfileToolStripMenuItem.Name = "searchDefinitionInCurrentfileToolStripMenuItem";
            this.searchDefinitionInCurrentfileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.K)));
            this.searchDefinitionInCurrentfileToolStripMenuItem.Size = new System.Drawing.Size(432, 30);
            this.searchDefinitionInCurrentfileToolStripMenuItem.Tag = "SearchDefinitionInCurrentFile";
            this.searchDefinitionInCurrentfileToolStripMenuItem.Text = "Search Definition in current file";
            this.searchDefinitionInCurrentfileToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(429, 6);
            // 
            // aPIBrowserToolStripMenuItem
            // 
            this.aPIBrowserToolStripMenuItem.Name = "aPIBrowserToolStripMenuItem";
            this.aPIBrowserToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.aPIBrowserToolStripMenuItem.Size = new System.Drawing.Size(432, 30);
            this.aPIBrowserToolStripMenuItem.Tag = "ApiBrowser";
            this.aPIBrowserToolStripMenuItem.Text = "API browser";
            this.aPIBrowserToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            this.toolStripMenuItem20.Size = new System.Drawing.Size(318, 6);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+F";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.findToolStripMenuItem.Tag = "Find";
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // findWindowsToolStripMenuItem
            // 
            this.findWindowsToolStripMenuItem.Name = "findWindowsToolStripMenuItem";
            this.findWindowsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.F)));
            this.findWindowsToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.findWindowsToolStripMenuItem.Tag = "FindWindow";
            this.findWindowsToolStripMenuItem.Text = "Find window";
            this.findWindowsToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.replaceToolStripMenuItem.Tag = "Replace";
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // replaceWindowToolStripMenuItem
            // 
            this.replaceWindowToolStripMenuItem.Name = "replaceWindowToolStripMenuItem";
            this.replaceWindowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.H)));
            this.replaceWindowToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.replaceWindowToolStripMenuItem.Tag = "ReplaceWindow";
            this.replaceWindowToolStripMenuItem.Text = "Replace window";
            this.replaceWindowToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // toolStripMenuItem21
            // 
            this.toolStripMenuItem21.Name = "toolStripMenuItem21";
            this.toolStripMenuItem21.Size = new System.Drawing.Size(318, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.copyToolStripMenuItem.Tag = "Copy";
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+V";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.pasteToolStripMenuItem.Tag = "Paste";
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+X";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.cutToolStripMenuItem.Tag = "Cut";
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.selectAllToolStripMenuItem.Tag = "SelectAll";
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Z";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.undoToolStripMenuItem.Tag = "Undo";
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Y";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(321, 30);
            this.redoToolStripMenuItem.Tag = "Redo";
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.quickRunToolStripMenuItem,
            this.compileToolStripMenuItem,
            this.toolStripMenuItem1,
            this.goToNextErrorToolStripMenuItem,
            this.toolStripMenuItem2,
            this.vacuumProjectToolStripMenuItem,
            this.packProjectToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(78, 29);
            this.projectToolStripMenuItem.Text = "&Project";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("runToolStripMenuItem.Image")));
            this.runToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runToolStripMenuItem.Size = new System.Drawing.Size(255, 30);
            this.runToolStripMenuItem.Tag = "Run";
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // quickRunToolStripMenuItem
            // 
            this.quickRunToolStripMenuItem.Name = "quickRunToolStripMenuItem";
            this.quickRunToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.quickRunToolStripMenuItem.Size = new System.Drawing.Size(255, 30);
            this.quickRunToolStripMenuItem.Tag = "QuickRun";
            this.quickRunToolStripMenuItem.Text = "Quick Run";
            this.quickRunToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // compileToolStripMenuItem
            // 
            this.compileToolStripMenuItem.Name = "compileToolStripMenuItem";
            this.compileToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.compileToolStripMenuItem.Size = new System.Drawing.Size(255, 30);
            this.compileToolStripMenuItem.Tag = "Compile";
            this.compileToolStripMenuItem.Text = "Compile";
            this.compileToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(252, 6);
            // 
            // goToNextErrorToolStripMenuItem
            // 
            this.goToNextErrorToolStripMenuItem.Name = "goToNextErrorToolStripMenuItem";
            this.goToNextErrorToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.goToNextErrorToolStripMenuItem.Size = new System.Drawing.Size(255, 30);
            this.goToNextErrorToolStripMenuItem.Tag = "GoToNextError";
            this.goToNextErrorToolStripMenuItem.Text = "GoTo Next Error";
            this.goToNextErrorToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(252, 6);
            // 
            // vacuumProjectToolStripMenuItem
            // 
            this.vacuumProjectToolStripMenuItem.Enabled = false;
            this.vacuumProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("vacuumProjectToolStripMenuItem.Image")));
            this.vacuumProjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.vacuumProjectToolStripMenuItem.Name = "vacuumProjectToolStripMenuItem";
            this.vacuumProjectToolStripMenuItem.Size = new System.Drawing.Size(255, 30);
            this.vacuumProjectToolStripMenuItem.Text = "Vacuum project";
            // 
            // packProjectToolStripMenuItem
            // 
            this.packProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("packProjectToolStripMenuItem.Image")));
            this.packProjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.packProjectToolStripMenuItem.Name = "packProjectToolStripMenuItem";
            this.packProjectToolStripMenuItem.Size = new System.Drawing.Size(255, 30);
            this.packProjectToolStripMenuItem.Text = "Pack/Backup project";
            this.packProjectToolStripMenuItem.Click += new System.EventHandler(this.OnPackProject);
            // 
            // resourcesToolStripMenuItem
            // 
            this.resourcesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildSelectedToolStripMenuItem});
            this.resourcesToolStripMenuItem.Name = "resourcesToolStripMenuItem";
            this.resourcesToolStripMenuItem.Size = new System.Drawing.Size(189, 29);
            this.resourcesToolStripMenuItem.Text = "Resource and Strings";
            // 
            // buildSelectedToolStripMenuItem
            // 
            this.buildSelectedToolStripMenuItem.Name = "buildSelectedToolStripMenuItem";
            this.buildSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.buildSelectedToolStripMenuItem.Size = new System.Drawing.Size(313, 30);
            this.buildSelectedToolStripMenuItem.Tag = "BuildSelectedResources";
            this.buildSelectedToolStripMenuItem.Text = "Build selected";
            this.buildSelectedToolStripMenuItem.Click += new System.EventHandler(this.OnSimpleMenuCommand);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem5,
            this.aboutToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(240, 30);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.OnShowSettings);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(237, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(240, 30);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.OnShowAboutInformations);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(240, 30);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for updates";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.OnCheckForUpdates);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txMemoryStatus,
            this.lbIDETimes,
            this.lbGeneralInfo,
            this.lbTaskName,
            this.lbTaskProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 995);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(2564, 40);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txMemoryStatus
            // 
            this.txMemoryStatus.AutoSize = false;
            this.txMemoryStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.txMemoryStatus.Name = "txMemoryStatus";
            this.txMemoryStatus.Size = new System.Drawing.Size(120, 35);
            // 
            // lbIDETimes
            // 
            this.lbIDETimes.AutoSize = false;
            this.lbIDETimes.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lbIDETimes.Name = "lbIDETimes";
            this.lbIDETimes.Size = new System.Drawing.Size(200, 35);
            // 
            // lbGeneralInfo
            // 
            this.lbGeneralInfo.AutoSize = false;
            this.lbGeneralInfo.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lbGeneralInfo.Name = "lbGeneralInfo";
            this.lbGeneralInfo.Size = new System.Drawing.Size(250, 35);
            // 
            // lbTaskName
            // 
            this.lbTaskName.AutoSize = false;
            this.lbTaskName.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lbTaskName.Name = "lbTaskName";
            this.lbTaskName.Size = new System.Drawing.Size(150, 35);
            this.lbTaskName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbTaskName.Visible = false;
            // 
            // lbTaskProgress
            // 
            this.lbTaskProgress.Name = "lbTaskProgress";
            this.lbTaskProgress.Size = new System.Drawing.Size(300, 34);
            this.lbTaskProgress.Visible = false;
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = false;
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(200, 17);
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.tabGeneral);
            this.mainTab.Controls.Add(this.tabPublisher);
            this.mainTab.Controls.Add(this.tabResources);
            this.mainTab.Controls.Add(this.tabProfiles);
            this.mainTab.Controls.Add(this.tabStrings);
            this.mainTab.Controls.Add(this.tabAds);
            this.mainTab.Controls.Add(this.tabConstants);
            this.mainTab.Controls.Add(this.tabCountersAndAlarms);
            this.mainTab.Controls.Add(this.tabAnimationObjects);
            this.mainTab.Controls.Add(this.tabCode);
            this.mainTab.Location = new System.Drawing.Point(0, 37);
            this.mainTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(2271, 955);
            this.mainTab.TabIndex = 2;
            this.mainTab.SelectedIndexChanged += new System.EventHandler(this.OnMainTabChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.tabProjectMain);
            this.tabGeneral.Location = new System.Drawing.Point(4, 29);
            this.tabGeneral.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabGeneral.Size = new System.Drawing.Size(2263, 922);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabPublisher
            // 
            this.tabPublisher.Controls.Add(this.tabProjectPublishMaterials);
            this.tabPublisher.Location = new System.Drawing.Point(4, 29);
            this.tabPublisher.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPublisher.Name = "tabPublisher";
            this.tabPublisher.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPublisher.Size = new System.Drawing.Size(2263, 922);
            this.tabPublisher.TabIndex = 7;
            this.tabPublisher.Text = "Publish Materials";
            this.tabPublisher.UseVisualStyleBackColor = true;
            // 
            // tabResources
            // 
            this.tabResources.Controls.Add(this.tabProjectResources);
            this.tabResources.Location = new System.Drawing.Point(4, 29);
            this.tabResources.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabResources.Name = "tabResources";
            this.tabResources.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabResources.Size = new System.Drawing.Size(2263, 922);
            this.tabResources.TabIndex = 1;
            this.tabResources.Text = "Resources";
            this.tabResources.UseVisualStyleBackColor = true;
            // 
            // tabProfiles
            // 
            this.tabProfiles.Controls.Add(this.tabProjectMemoryProfiles);
            this.tabProfiles.Location = new System.Drawing.Point(4, 29);
            this.tabProfiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabProfiles.Name = "tabProfiles";
            this.tabProfiles.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabProfiles.Size = new System.Drawing.Size(2263, 922);
            this.tabProfiles.TabIndex = 5;
            this.tabProfiles.Text = "Memory Management Profiles";
            this.tabProfiles.UseVisualStyleBackColor = true;
            // 
            // tabStrings
            // 
            this.tabStrings.Controls.Add(this.tabProjectStrings);
            this.tabStrings.Location = new System.Drawing.Point(4, 29);
            this.tabStrings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabStrings.Name = "tabStrings";
            this.tabStrings.Size = new System.Drawing.Size(2263, 922);
            this.tabStrings.TabIndex = 3;
            this.tabStrings.Text = "Strings";
            this.tabStrings.UseVisualStyleBackColor = true;
            // 
            // tabAds
            // 
            this.tabAds.Controls.Add(this.tabProjectAds);
            this.tabAds.Location = new System.Drawing.Point(4, 29);
            this.tabAds.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAds.Name = "tabAds";
            this.tabAds.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAds.Size = new System.Drawing.Size(2263, 922);
            this.tabAds.TabIndex = 6;
            this.tabAds.Text = "Advertisements";
            this.tabAds.UseVisualStyleBackColor = true;
            // 
            // tabConstants
            // 
            this.tabConstants.Controls.Add(this.tabProjectConstants);
            this.tabConstants.Location = new System.Drawing.Point(4, 29);
            this.tabConstants.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabConstants.Name = "tabConstants";
            this.tabConstants.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabConstants.Size = new System.Drawing.Size(2263, 922);
            this.tabConstants.TabIndex = 8;
            this.tabConstants.Text = "Constants";
            this.tabConstants.UseVisualStyleBackColor = true;
            // 
            // tabCountersAndAlarms
            // 
            this.tabCountersAndAlarms.Controls.Add(this.tabProjectCountersAndTimers);
            this.tabCountersAndAlarms.Location = new System.Drawing.Point(4, 29);
            this.tabCountersAndAlarms.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabCountersAndAlarms.Name = "tabCountersAndAlarms";
            this.tabCountersAndAlarms.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabCountersAndAlarms.Size = new System.Drawing.Size(2263, 922);
            this.tabCountersAndAlarms.TabIndex = 9;
            this.tabCountersAndAlarms.Text = "Counters and Alarms";
            this.tabCountersAndAlarms.UseVisualStyleBackColor = true;
            // 
            // tabAnimationObjects
            // 
            this.tabAnimationObjects.Controls.Add(this.tabProjectAnimationObjects);
            this.tabAnimationObjects.Location = new System.Drawing.Point(4, 29);
            this.tabAnimationObjects.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAnimationObjects.Name = "tabAnimationObjects";
            this.tabAnimationObjects.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAnimationObjects.Size = new System.Drawing.Size(2263, 922);
            this.tabAnimationObjects.TabIndex = 10;
            this.tabAnimationObjects.Text = "Animation Objects";
            this.tabAnimationObjects.UseVisualStyleBackColor = true;
            // 
            // tabCode
            // 
            this.tabCode.Controls.Add(this.tabProjectEditor);
            this.tabCode.Location = new System.Drawing.Point(4, 29);
            this.tabCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabCode.Name = "tabCode";
            this.tabCode.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabCode.Size = new System.Drawing.Size(2263, 922);
            this.tabCode.TabIndex = 4;
            this.tabCode.Text = "Code";
            this.tabCode.UseVisualStyleBackColor = true;
            // 
            // resImagesLarge
            // 
            this.resImagesLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.resImagesLarge.ImageSize = new System.Drawing.Size(64, 64);
            this.resImagesLarge.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // resImagesSmall
            // 
            this.resImagesSmall.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.resImagesSmall.ImageSize = new System.Drawing.Size(16, 16);
            this.resImagesSmall.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // taskEventIcons
            // 
            this.taskEventIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("taskEventIcons.ImageStream")));
            this.taskEventIcons.TransparentColor = System.Drawing.Color.Magenta;
            this.taskEventIcons.Images.SetKeyName(0, "Running");
            this.taskEventIcons.Images.SetKeyName(1, "Success");
            this.taskEventIcons.Images.SetKeyName(2, "Info");
            this.taskEventIcons.Images.SetKeyName(3, "Error");
            // 
            // memoryStatusTimer
            // 
            this.memoryStatusTimer.Enabled = true;
            this.memoryStatusTimer.Interval = 10000;
            this.memoryStatusTimer.Tick += new System.EventHandler(this.OnUpdateMemoryStatus);
            // 
            // pnlMainScreen
            // 
            this.pnlMainScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMainScreen.Controls.Add(this.lstMainScreenOperation);
            this.pnlMainScreen.Location = new System.Drawing.Point(2332, 75);
            this.pnlMainScreen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlMainScreen.Name = "pnlMainScreen";
            this.pnlMainScreen.Size = new System.Drawing.Size(299, 476);
            this.pnlMainScreen.TabIndex = 4;
            // 
            // lstMainScreenOperation
            // 
            this.lstMainScreenOperation.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader33,
            this.columnHeader34,
            this.columnHeader35});
            this.lstMainScreenOperation.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewGroup1.Header = "Standard Operations";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "Recent projects";
            listViewGroup2.Name = "listViewGroup2";
            listViewGroup3.Header = "Older projects";
            listViewGroup3.Name = "listViewGroup3";
            this.lstMainScreenOperation.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            listViewItem1.Group = listViewGroup1;
            listViewItem1.Tag = "-2";
            listViewItem2.Group = listViewGroup1;
            listViewItem2.Tag = "-1";
            this.lstMainScreenOperation.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.lstMainScreenOperation.LargeImageList = this.iconMainScreen;
            this.lstMainScreenOperation.Location = new System.Drawing.Point(0, 0);
            this.lstMainScreenOperation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstMainScreenOperation.MultiSelect = false;
            this.lstMainScreenOperation.Name = "lstMainScreenOperation";
            this.lstMainScreenOperation.Size = new System.Drawing.Size(297, 474);
            this.lstMainScreenOperation.TabIndex = 4;
            this.lstMainScreenOperation.TileSize = new System.Drawing.Size(400, 100);
            this.lstMainScreenOperation.UseCompatibleStateImageBehavior = false;
            this.lstMainScreenOperation.View = System.Windows.Forms.View.Tile;
            this.lstMainScreenOperation.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseDblClickedOnMainScreen);
            // 
            // iconMainScreen
            // 
            this.iconMainScreen.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconMainScreen.ImageStream")));
            this.iconMainScreen.TransparentColor = System.Drawing.Color.Transparent;
            this.iconMainScreen.Images.SetKeyName(0, "__load__");
            this.iconMainScreen.Images.SetKeyName(1, "__new__");
            // 
            // timerIDETimes
            // 
            this.timerIDETimes.Enabled = true;
            this.timerIDETimes.Interval = 1000;
            this.timerIDETimes.Tick += new System.EventHandler(this.OnUpdateWorkingTime);
            // 
            // tabProjectMain
            // 
            this.tabProjectMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectMain.Location = new System.Drawing.Point(4, 5);
            this.tabProjectMain.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectMain.Name = "tabProjectMain";
            this.tabProjectMain.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectMain.TabIndex = 0;
            // 
            // tabProjectPublishMaterials
            // 
            this.tabProjectPublishMaterials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectPublishMaterials.Location = new System.Drawing.Point(4, 5);
            this.tabProjectPublishMaterials.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectPublishMaterials.Name = "tabProjectPublishMaterials";
            this.tabProjectPublishMaterials.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectPublishMaterials.TabIndex = 0;
            // 
            // tabProjectResources
            // 
            this.tabProjectResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectResources.Location = new System.Drawing.Point(4, 5);
            this.tabProjectResources.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectResources.Name = "tabProjectResources";
            this.tabProjectResources.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectResources.TabIndex = 0;
            // 
            // tabProjectMemoryProfiles
            // 
            this.tabProjectMemoryProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectMemoryProfiles.Location = new System.Drawing.Point(4, 5);
            this.tabProjectMemoryProfiles.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectMemoryProfiles.Name = "tabProjectMemoryProfiles";
            this.tabProjectMemoryProfiles.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectMemoryProfiles.TabIndex = 0;
            // 
            // tabProjectStrings
            // 
            this.tabProjectStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectStrings.Location = new System.Drawing.Point(0, 0);
            this.tabProjectStrings.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectStrings.Name = "tabProjectStrings";
            this.tabProjectStrings.Size = new System.Drawing.Size(2263, 922);
            this.tabProjectStrings.TabIndex = 0;
            // 
            // tabProjectAds
            // 
            this.tabProjectAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectAds.Location = new System.Drawing.Point(4, 5);
            this.tabProjectAds.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectAds.Name = "tabProjectAds";
            this.tabProjectAds.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectAds.TabIndex = 0;
            // 
            // tabProjectConstants
            // 
            this.tabProjectConstants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectConstants.Location = new System.Drawing.Point(4, 5);
            this.tabProjectConstants.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectConstants.Name = "tabProjectConstants";
            this.tabProjectConstants.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectConstants.TabIndex = 0;
            // 
            // tabProjectCountersAndTimers
            // 
            this.tabProjectCountersAndTimers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectCountersAndTimers.Location = new System.Drawing.Point(4, 5);
            this.tabProjectCountersAndTimers.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectCountersAndTimers.Name = "tabProjectCountersAndTimers";
            this.tabProjectCountersAndTimers.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectCountersAndTimers.TabIndex = 0;
            // 
            // tabProjectAnimationObjects
            // 
            this.tabProjectAnimationObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectAnimationObjects.Location = new System.Drawing.Point(4, 5);
            this.tabProjectAnimationObjects.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectAnimationObjects.Name = "tabProjectAnimationObjects";
            this.tabProjectAnimationObjects.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectAnimationObjects.TabIndex = 0;
            // 
            // tabProjectEditor
            // 
            this.tabProjectEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProjectEditor.Location = new System.Drawing.Point(4, 5);
            this.tabProjectEditor.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.tabProjectEditor.Name = "tabProjectEditor";
            this.tabProjectEditor.Size = new System.Drawing.Size(2255, 912);
            this.tabProjectEditor.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2564, 1035);
            this.Controls.Add(this.pnlMainScreen);
            this.Controls.Add(this.mainTab);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnCloseGAC);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mainTab.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabPublisher.ResumeLayout(false);
            this.tabResources.ResumeLayout(false);
            this.tabProfiles.ResumeLayout(false);
            this.tabStrings.ResumeLayout(false);
            this.tabAds.ResumeLayout(false);
            this.tabConstants.ResumeLayout(false);
            this.tabCountersAndAlarms.ResumeLayout(false);
            this.tabAnimationObjects.ResumeLayout(false);
            this.tabCode.ResumeLayout(false);
            this.pnlMainScreen.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbInfo;
        private System.Windows.Forms.TabControl mainTab;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabResources;
        private System.Windows.Forms.TabPage tabStrings;
        private System.Windows.Forms.ImageList resImagesLarge;
        private System.Windows.Forms.ImageList resImagesSmall;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lbGeneralInfo;
        private System.Windows.Forms.ToolStripStatusLabel lbTaskName;
        private System.Windows.Forms.ToolStripProgressBar lbTaskProgress;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ImageList taskEventIcons;
        private System.Windows.Forms.TabPage tabCode;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem collapseToDefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel txMemoryStatus;
        private System.Windows.Forms.Timer memoryStatusTimer;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.TabPage tabProfiles;
        private System.Windows.Forms.Panel pnlMainScreen;
        private System.Windows.Forms.ListView lstMainScreenOperation;
        private System.Windows.Forms.ImageList iconMainScreen;
        private System.Windows.Forms.ColumnHeader columnHeader33;
        private System.Windows.Forms.ColumnHeader columnHeader34;
        private System.Windows.Forms.ColumnHeader columnHeader35;
        private System.Windows.Forms.TabPage tabAds;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem19;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem20;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem21;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private ProjectTabMain tabProjectMain;
        private ProjectTabResources tabProjectResources;
        private ProjectTabMemoryProfiles tabProjectMemoryProfiles;
        private ProjectTabStrings tabProjectStrings;
        private ProjectTabAds tabProjectAds;
        private ProjectTabEditor tabProjectEditor;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickRunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem goToNextErrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem packProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vacuumProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceWindowToolStripMenuItem;
        private ProjectTabPublishMaterials tabProjectPublishMaterials;
        private System.Windows.Forms.TabPage tabPublisher;
        private System.Windows.Forms.ToolStripStatusLabel lbIDETimes;
        private System.Windows.Forms.Timer timerIDETimes;
        private System.Windows.Forms.TabPage tabConstants;
        private ProjectTabConstants tabProjectConstants;
        private System.Windows.Forms.ToolStripMenuItem codeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unCommentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resourcesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchDefinitionInCurrentfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToDefinitionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem aPIBrowserToolStripMenuItem;
        private System.Windows.Forms.TabPage tabCountersAndAlarms;
        private ProjectTabCountersAndAlerts tabProjectCountersAndTimers;
        private System.Windows.Forms.TabPage tabAnimationObjects;
        private ProjectTabAnimation tabProjectAnimationObjects;
        
    }
}

