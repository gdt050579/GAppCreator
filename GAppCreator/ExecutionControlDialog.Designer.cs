namespace GAppCreator
{
    partial class ExecutionControlDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExecutionControlDialog));
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "Total Memory",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "Max Memory",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "Total Blocks",
            ""}, -1);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbViewOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.cbScreenShots = new System.Windows.Forms.ToolStripMenuItem();
            this.cbApplicationSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.cbApplicationStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.cbMemory = new System.Windows.Forms.ToolStripMenuItem();
            this.cbDebugCommands = new System.Windows.Forms.ToolStripMenuItem();
            this.cbAnalyticsEmulator = new System.Windows.Forms.ToolStripMenuItem();
            this.cbAlarmsAndCounters = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cbMaximize = new System.Windows.Forms.ToolStripButton();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnRestart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnSuspend = new System.Windows.Forms.ToolStripButton();
            this.btnPause = new System.Windows.Forms.ToolStripButton();
            this.btnResume = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.rbScale100 = new System.Windows.Forms.ToolStripMenuItem();
            this.rbScale50 = new System.Windows.Forms.ToolStripMenuItem();
            this.rbScale25 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cbAutoScroll = new System.Windows.Forms.ToolStripButton();
            this.btnClearLog = new System.Windows.Forms.ToolStripButton();
            this.btnSaveLog = new System.Windows.Forms.ToolStripDropDownButton();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sepFilterLog = new System.Windows.Forms.ToolStripSeparator();
            this.btnFilterLogMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbGacMessages = new System.Windows.Forms.ToolStripMenuItem();
            this.cbGacErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cbFrameworkErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.cbFrameworkMessages = new System.Windows.Forms.ToolStripMenuItem();
            this.cbFrameworkEvents = new System.Windows.Forms.ToolStripMenuItem();
            this.lbFilterLog = new System.Windows.Forms.ToolStripLabel();
            this.txFilterLog = new System.Windows.Forms.ToolStripTextBox();
            this.btnTakePicture = new System.Windows.Forms.ToolStripButton();
            this.btnSavePicture = new System.Windows.Forms.ToolStripButton();
            this.btnPublish = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnGoToScene = new System.Windows.Forms.ToolStripButton();
            this.btnViewTexture = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteSettings = new System.Windows.Forms.ToolStripButton();
            this.btnCopyImage = new System.Windows.Forms.ToolStripButton();
            this.lbDebugCommandQuickSearch = new System.Windows.Forms.ToolStripLabel();
            this.txDebugCommandFilter = new System.Windows.Forms.ToolStripTextBox();
            this.btnRunDebugCommand = new System.Windows.Forms.ToolStripButton();
            this.btnPin = new System.Windows.Forms.ToolStripButton();
            this.btnCreateSnapshot = new System.Windows.Forms.ToolStripButton();
            this.Worker = new System.ComponentModel.BackgroundWorker();
            this.pnlCloseButton = new System.Windows.Forms.Panel();
            this.btnCloseDialog = new System.Windows.Forms.Button();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.pnlGlobalCountersAndTimers = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lstGlobalCounters = new System.Windows.Forms.ListView();
            this.columnHeader39 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader40 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader41 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader42 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader43 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader44 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstGlobalGroupCounters = new System.Windows.Forms.ListView();
            this.columnHeader45 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader46 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader47 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.lstAlarms = new System.Windows.Forms.ListView();
            this.columnHeader48 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader49 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader50 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstAlarmEvents = new System.Windows.Forms.ListView();
            this.columnHeader51 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader52 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnRecheckAlarmTime = new System.Windows.Forms.ToolStripButton();
            this.lbAlarmTestTime = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDateTime = new System.Windows.Forms.ToolStripButton();
            this.pnlAnalyticsEmulator = new System.Windows.Forms.Panel();
            this.lstAnalytics = new System.Windows.Forms.ListView();
            this.columnHeader34 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader35 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader36 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader37 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader38 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlAppSettings = new System.Windows.Forms.SplitContainer();
            this.lstAppSettings = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctrlSnapshots = new GAppCreator.SettingsSnapshotControl();
            this.pnlMemory = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lstMemory = new System.Windows.Forms.ListView();
            this.columnHeader27 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader28 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader29 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader30 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader31 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstMemoryStats = new System.Windows.Forms.ListView();
            this.columnHeader32 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader33 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstMemoryEvents = new System.Windows.Forms.ListView();
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader25 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader21 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader22 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader23 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader24 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader26 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbShowMemoryNewEvents = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowMemoryDeleteEvents = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearEventListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.txMemoryFilter = new System.Windows.Forms.ToolStripTextBox();
            this.lbMemoryEventsInfo = new System.Windows.Forms.ToolStripLabel();
            this.pnlDebugCommands = new System.Windows.Forms.Panel();
            this.lstDebugCommands = new System.Windows.Forms.ListView();
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlAppStatus = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propRunSettings = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lstAppStatus = new System.Windows.Forms.ListView();
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlTextureView = new System.Windows.Forms.Panel();
            this.pnlScreenShots = new System.Windows.Forms.SplitContainer();
            this.pnlFullScreenShotView = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlSmallImageScreenShot = new System.Windows.Forms.Panel();
            this.lstScreenShotProperties = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstOutput = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.smallIcons = new System.Windows.Forms.ImageList(this.components);
            this.restartTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.memTimer = new System.Windows.Forms.Timer(this.components);
            this.btnSaveTexture = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.pnlCloseButton.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGlobalCountersAndTimers)).BeginInit();
            this.pnlGlobalCountersAndTimers.Panel1.SuspendLayout();
            this.pnlGlobalCountersAndTimers.Panel2.SuspendLayout();
            this.pnlGlobalCountersAndTimers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.pnlAnalyticsEmulator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAppSettings)).BeginInit();
            this.pnlAppSettings.Panel1.SuspendLayout();
            this.pnlAppSettings.Panel2.SuspendLayout();
            this.pnlAppSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMemory)).BeginInit();
            this.pnlMemory.Panel1.SuspendLayout();
            this.pnlMemory.Panel2.SuspendLayout();
            this.pnlMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlDebugCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAppStatus)).BeginInit();
            this.pnlAppStatus.Panel1.SuspendLayout();
            this.pnlAppStatus.Panel2.SuspendLayout();
            this.pnlAppStatus.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlScreenShots)).BeginInit();
            this.pnlScreenShots.Panel1.SuspendLayout();
            this.pnlScreenShots.Panel2.SuspendLayout();
            this.pnlScreenShots.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripSeparator1,
            this.cbMaximize,
            this.btnStart,
            this.btnRestart,
            this.btnStop,
            this.btnSuspend,
            this.btnPause,
            this.btnResume,
            this.toolStripSeparator2,
            this.toolStripDropDownButton3,
            this.toolStripSeparator3,
            this.cbAutoScroll,
            this.btnClearLog,
            this.btnSaveLog,
            this.sepFilterLog,
            this.btnFilterLogMenu,
            this.lbFilterLog,
            this.txFilterLog,
            this.btnTakePicture,
            this.btnSavePicture,
            this.btnPublish,
            this.btnRefresh,
            this.btnGoToScene,
            this.btnViewTexture,
            this.btnSaveTexture,
            this.btnDeleteSettings,
            this.btnCopyImage,
            this.lbDebugCommandQuickSearch,
            this.txDebugCommandFilter,
            this.btnRunDebugCommand,
            this.btnPin,
            this.btnCreateSnapshot});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1916, 38);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbViewOutput,
            this.cbScreenShots,
            this.cbApplicationSettings,
            this.cbApplicationStatus,
            this.cbMemory,
            this.cbDebugCommands,
            this.cbAnalyticsEmulator,
            this.cbAlarmsAndCounters});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(45, 35);
            this.toolStripDropDownButton1.Text = "View";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbViewOutput
            // 
            this.cbViewOutput.Name = "cbViewOutput";
            this.cbViewOutput.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.cbViewOutput.Size = new System.Drawing.Size(225, 22);
            this.cbViewOutput.Text = "Output";
            this.cbViewOutput.Click += new System.EventHandler(this.OnShowOutput);
            // 
            // cbScreenShots
            // 
            this.cbScreenShots.Name = "cbScreenShots";
            this.cbScreenShots.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.cbScreenShots.Size = new System.Drawing.Size(225, 22);
            this.cbScreenShots.Text = "Screen Shots";
            this.cbScreenShots.Click += new System.EventHandler(this.OnShowScreenShots);
            // 
            // cbApplicationSettings
            // 
            this.cbApplicationSettings.Name = "cbApplicationSettings";
            this.cbApplicationSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.cbApplicationSettings.Size = new System.Drawing.Size(225, 22);
            this.cbApplicationSettings.Text = "Application Settings";
            this.cbApplicationSettings.Click += new System.EventHandler(this.OnShowApplicationSettings);
            // 
            // cbApplicationStatus
            // 
            this.cbApplicationStatus.Name = "cbApplicationStatus";
            this.cbApplicationStatus.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
            this.cbApplicationStatus.Size = new System.Drawing.Size(225, 22);
            this.cbApplicationStatus.Text = "Application Status";
            this.cbApplicationStatus.Click += new System.EventHandler(this.OnShowAppicationStatus);
            // 
            // cbMemory
            // 
            this.cbMemory.Name = "cbMemory";
            this.cbMemory.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
            this.cbMemory.Size = new System.Drawing.Size(225, 22);
            this.cbMemory.Text = "Memory";
            this.cbMemory.Click += new System.EventHandler(this.OnShowMemory);
            // 
            // cbDebugCommands
            // 
            this.cbDebugCommands.Name = "cbDebugCommands";
            this.cbDebugCommands.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D6)));
            this.cbDebugCommands.Size = new System.Drawing.Size(225, 22);
            this.cbDebugCommands.Text = "Debug Commands";
            this.cbDebugCommands.Click += new System.EventHandler(this.OnShowDebugCommands);
            // 
            // cbAnalyticsEmulator
            // 
            this.cbAnalyticsEmulator.Name = "cbAnalyticsEmulator";
            this.cbAnalyticsEmulator.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D7)));
            this.cbAnalyticsEmulator.Size = new System.Drawing.Size(225, 22);
            this.cbAnalyticsEmulator.Text = "Analytics emulator";
            this.cbAnalyticsEmulator.Click += new System.EventHandler(this.OnShowAnalyticsEmulator);
            // 
            // cbAlarmsAndCounters
            // 
            this.cbAlarmsAndCounters.Name = "cbAlarmsAndCounters";
            this.cbAlarmsAndCounters.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D8)));
            this.cbAlarmsAndCounters.Size = new System.Drawing.Size(225, 22);
            this.cbAlarmsAndCounters.Text = "Alarms and Counters";
            this.cbAlarmsAndCounters.Click += new System.EventHandler(this.OnShowGlobalTimersAndCounters);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // cbMaximize
            // 
            this.cbMaximize.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.cbMaximize.Checked = true;
            this.cbMaximize.CheckOnClick = true;
            this.cbMaximize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMaximize.Image = ((System.Drawing.Image)(resources.GetObject("cbMaximize.Image")));
            this.cbMaximize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbMaximize.Name = "cbMaximize";
            this.cbMaximize.Size = new System.Drawing.Size(61, 35);
            this.cbMaximize.Text = "Maximize";
            this.cbMaximize.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbMaximize.Click += new System.EventHandler(this.OnMaximizeMinimize);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(35, 35);
            this.btnStart.Text = "Start";
            this.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStart.Click += new System.EventHandler(this.OnStartApp);
            // 
            // btnRestart
            // 
            this.btnRestart.Image = ((System.Drawing.Image)(resources.GetObject("btnRestart.Image")));
            this.btnRestart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(47, 35);
            this.btnRestart.Text = "Restart";
            this.btnRestart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRestart.Click += new System.EventHandler(this.OnRestartApp);
            // 
            // btnStop
            // 
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(35, 35);
            this.btnStop.Text = "Stop";
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStop.Click += new System.EventHandler(this.OnShutDown);
            // 
            // btnSuspend
            // 
            this.btnSuspend.Image = ((System.Drawing.Image)(resources.GetObject("btnSuspend.Image")));
            this.btnSuspend.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSuspend.Name = "btnSuspend";
            this.btnSuspend.Size = new System.Drawing.Size(56, 35);
            this.btnSuspend.Text = "Suspend";
            this.btnSuspend.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSuspend.Click += new System.EventHandler(this.OnSuspendApp);
            // 
            // btnPause
            // 
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(42, 35);
            this.btnPause.Text = "Pause";
            this.btnPause.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPause.Click += new System.EventHandler(this.OnPauseApp);
            // 
            // btnResume
            // 
            this.btnResume.Enabled = false;
            this.btnResume.Image = ((System.Drawing.Image)(resources.GetObject("btnResume.Image")));
            this.btnResume.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(53, 35);
            this.btnResume.Text = "Resume";
            this.btnResume.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnResume.Click += new System.EventHandler(this.OnResumeApp);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rbScale100,
            this.rbScale50,
            this.rbScale25});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(47, 35);
            this.toolStripDropDownButton3.Text = "Scale";
            this.toolStripDropDownButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // rbScale100
            // 
            this.rbScale100.Checked = true;
            this.rbScale100.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rbScale100.Name = "rbScale100";
            this.rbScale100.Size = new System.Drawing.Size(105, 22);
            this.rbScale100.Text = "100 %";
            this.rbScale100.Click += new System.EventHandler(this.OnScale100);
            // 
            // rbScale50
            // 
            this.rbScale50.Name = "rbScale50";
            this.rbScale50.Size = new System.Drawing.Size(105, 22);
            this.rbScale50.Text = "50 %";
            this.rbScale50.Click += new System.EventHandler(this.OnScale50);
            // 
            // rbScale25
            // 
            this.rbScale25.Name = "rbScale25";
            this.rbScale25.Size = new System.Drawing.Size(105, 22);
            this.rbScale25.Text = "25 %";
            this.rbScale25.Click += new System.EventHandler(this.OnScale25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // cbAutoScroll
            // 
            this.cbAutoScroll.AccessibleDescription = "";
            this.cbAutoScroll.Checked = true;
            this.cbAutoScroll.CheckOnClick = true;
            this.cbAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoScroll.Image = ((System.Drawing.Image)(resources.GetObject("cbAutoScroll.Image")));
            this.cbAutoScroll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbAutoScroll.Name = "cbAutoScroll";
            this.cbAutoScroll.Size = new System.Drawing.Size(66, 35);
            this.cbAutoScroll.Text = "AutoScroll";
            this.cbAutoScroll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbAutoScroll.Click += new System.EventHandler(this.OnAutoScroll);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Image = ((System.Drawing.Image)(resources.GetObject("btnClearLog.Image")));
            this.btnClearLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(61, 35);
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnClearLog.Click += new System.EventHandler(this.OnClearLog);
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.visibleToolStripMenuItem});
            this.btnSaveLog.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveLog.Image")));
            this.btnSaveLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(67, 35);
            this.btnSaveLog.Text = "Save Log";
            this.btnSaveLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.allToolStripMenuItem.Text = "Complete log";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.OnSaveCompleteLog);
            // 
            // visibleToolStripMenuItem
            // 
            this.visibleToolStripMenuItem.Name = "visibleToolStripMenuItem";
            this.visibleToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.visibleToolStripMenuItem.Text = "Filtered log";
            this.visibleToolStripMenuItem.Click += new System.EventHandler(this.OnSaveFilteredLog);
            // 
            // sepFilterLog
            // 
            this.sepFilterLog.Name = "sepFilterLog";
            this.sepFilterLog.Size = new System.Drawing.Size(6, 38);
            // 
            // btnFilterLogMenu
            // 
            this.btnFilterLogMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbGacMessages,
            this.cbGacErrors,
            this.toolStripMenuItem1,
            this.cbFrameworkErrors,
            this.cbFrameworkMessages,
            this.cbFrameworkEvents});
            this.btnFilterLogMenu.Image = ((System.Drawing.Image)(resources.GetObject("btnFilterLogMenu.Image")));
            this.btnFilterLogMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilterLogMenu.Name = "btnFilterLogMenu";
            this.btnFilterLogMenu.Size = new System.Drawing.Size(49, 35);
            this.btnFilterLogMenu.Text = "Show";
            this.btnFilterLogMenu.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbGacMessages
            // 
            this.cbGacMessages.Checked = true;
            this.cbGacMessages.CheckOnClick = true;
            this.cbGacMessages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGacMessages.Name = "cbGacMessages";
            this.cbGacMessages.Size = new System.Drawing.Size(187, 22);
            this.cbGacMessages.Text = "GAC Messages";
            this.cbGacMessages.Click += new System.EventHandler(this.OnRefilterMessages);
            // 
            // cbGacErrors
            // 
            this.cbGacErrors.Checked = true;
            this.cbGacErrors.CheckOnClick = true;
            this.cbGacErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGacErrors.Name = "cbGacErrors";
            this.cbGacErrors.Size = new System.Drawing.Size(187, 22);
            this.cbGacErrors.Text = "GAC Errors";
            this.cbGacErrors.Click += new System.EventHandler(this.OnRefilterMessages);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(184, 6);
            // 
            // cbFrameworkErrors
            // 
            this.cbFrameworkErrors.CheckOnClick = true;
            this.cbFrameworkErrors.Name = "cbFrameworkErrors";
            this.cbFrameworkErrors.Size = new System.Drawing.Size(187, 22);
            this.cbFrameworkErrors.Text = "Framework Errors";
            this.cbFrameworkErrors.Click += new System.EventHandler(this.OnRefilterMessages);
            // 
            // cbFrameworkMessages
            // 
            this.cbFrameworkMessages.CheckOnClick = true;
            this.cbFrameworkMessages.Name = "cbFrameworkMessages";
            this.cbFrameworkMessages.Size = new System.Drawing.Size(187, 22);
            this.cbFrameworkMessages.Text = "Framework Messages";
            this.cbFrameworkMessages.Click += new System.EventHandler(this.OnRefilterMessages);
            // 
            // cbFrameworkEvents
            // 
            this.cbFrameworkEvents.CheckOnClick = true;
            this.cbFrameworkEvents.Name = "cbFrameworkEvents";
            this.cbFrameworkEvents.Size = new System.Drawing.Size(187, 22);
            this.cbFrameworkEvents.Text = "Framework Events";
            this.cbFrameworkEvents.Click += new System.EventHandler(this.OnRefilterMessages);
            // 
            // lbFilterLog
            // 
            this.lbFilterLog.Name = "lbFilterLog";
            this.lbFilterLog.Size = new System.Drawing.Size(33, 35);
            this.lbFilterLog.Text = "Filter";
            // 
            // txFilterLog
            // 
            this.txFilterLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFilterLog.Name = "txFilterLog";
            this.txFilterLog.Size = new System.Drawing.Size(100, 38);
            this.txFilterLog.TextChanged += new System.EventHandler(this.OnFilterTextChanged);
            // 
            // btnTakePicture
            // 
            this.btnTakePicture.Image = ((System.Drawing.Image)(resources.GetObject("btnTakePicture.Image")));
            this.btnTakePicture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTakePicture.Name = "btnTakePicture";
            this.btnTakePicture.Size = new System.Drawing.Size(76, 35);
            this.btnTakePicture.Text = "Take Picture";
            this.btnTakePicture.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnTakePicture.Click += new System.EventHandler(this.OnTakePicture);
            // 
            // btnSavePicture
            // 
            this.btnSavePicture.Enabled = false;
            this.btnSavePicture.Image = ((System.Drawing.Image)(resources.GetObject("btnSavePicture.Image")));
            this.btnSavePicture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSavePicture.Name = "btnSavePicture";
            this.btnSavePicture.Size = new System.Drawing.Size(35, 35);
            this.btnSavePicture.Text = "Save";
            this.btnSavePicture.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSavePicture.Click += new System.EventHandler(this.OnSaveSnapshot);
            // 
            // btnPublish
            // 
            this.btnPublish.Enabled = false;
            this.btnPublish.Image = ((System.Drawing.Image)(resources.GetObject("btnPublish.Image")));
            this.btnPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(50, 35);
            this.btnPublish.Text = "Publish";
            this.btnPublish.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPublish.Click += new System.EventHandler(this.OnPublishSnapshot);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 35);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRefresh.Click += new System.EventHandler(this.OnRefreshView);
            // 
            // btnGoToScene
            // 
            this.btnGoToScene.Image = ((System.Drawing.Image)(resources.GetObject("btnGoToScene.Image")));
            this.btnGoToScene.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoToScene.Name = "btnGoToScene";
            this.btnGoToScene.Size = new System.Drawing.Size(74, 35);
            this.btnGoToScene.Text = "GoTo Scene";
            this.btnGoToScene.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnGoToScene.Click += new System.EventHandler(this.OnGoToScene);
            // 
            // btnViewTexture
            // 
            this.btnViewTexture.Image = ((System.Drawing.Image)(resources.GetObject("btnViewTexture.Image")));
            this.btnViewTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewTexture.Name = "btnViewTexture";
            this.btnViewTexture.Size = new System.Drawing.Size(78, 35);
            this.btnViewTexture.Text = "View Texture";
            this.btnViewTexture.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnViewTexture.Click += new System.EventHandler(this.OnViewTexture);
            // 
            // btnDeleteSettings
            // 
            this.btnDeleteSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteSettings.Image")));
            this.btnDeleteSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteSettings.Name = "btnDeleteSettings";
            this.btnDeleteSettings.Size = new System.Drawing.Size(89, 35);
            this.btnDeleteSettings.Text = "Delete Settings";
            this.btnDeleteSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDeleteSettings.Click += new System.EventHandler(this.OnDeleteSettings);
            // 
            // btnCopyImage
            // 
            this.btnCopyImage.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyImage.Image")));
            this.btnCopyImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopyImage.Name = "btnCopyImage";
            this.btnCopyImage.Size = new System.Drawing.Size(39, 35);
            this.btnCopyImage.Text = "Copy";
            this.btnCopyImage.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnCopyImage.Click += new System.EventHandler(this.OnCopyImage);
            // 
            // lbDebugCommandQuickSearch
            // 
            this.lbDebugCommandQuickSearch.Name = "lbDebugCommandQuickSearch";
            this.lbDebugCommandQuickSearch.Size = new System.Drawing.Size(76, 35);
            this.lbDebugCommandQuickSearch.Text = "Quick Search";
            // 
            // txDebugCommandFilter
            // 
            this.txDebugCommandFilter.AutoSize = false;
            this.txDebugCommandFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txDebugCommandFilter.Name = "txDebugCommandFilter";
            this.txDebugCommandFilter.Size = new System.Drawing.Size(200, 23);
            this.txDebugCommandFilter.TextChanged += new System.EventHandler(this.OnFilterDebugCommandChanged);
            // 
            // btnRunDebugCommand
            // 
            this.btnRunDebugCommand.Image = ((System.Drawing.Image)(resources.GetObject("btnRunDebugCommand.Image")));
            this.btnRunDebugCommand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRunDebugCommand.Name = "btnRunDebugCommand";
            this.btnRunDebugCommand.Size = new System.Drawing.Size(92, 35);
            this.btnRunDebugCommand.Text = "Run Command";
            this.btnRunDebugCommand.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRunDebugCommand.Click += new System.EventHandler(this.OnRunDebugCommand);
            // 
            // btnPin
            // 
            this.btnPin.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPin.CheckOnClick = true;
            this.btnPin.Image = ((System.Drawing.Image)(resources.GetObject("btnPin.Image")));
            this.btnPin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPin.Name = "btnPin";
            this.btnPin.Size = new System.Drawing.Size(28, 35);
            this.btnPin.Text = "Pin";
            this.btnPin.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPin.Click += new System.EventHandler(this.OnPinWindow);
            // 
            // btnCreateSnapshot
            // 
            this.btnCreateSnapshot.Enabled = false;
            this.btnCreateSnapshot.Image = ((System.Drawing.Image)(resources.GetObject("btnCreateSnapshot.Image")));
            this.btnCreateSnapshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreateSnapshot.Name = "btnCreateSnapshot";
            this.btnCreateSnapshot.Size = new System.Drawing.Size(86, 35);
            this.btnCreateSnapshot.Text = "Save snapshot";
            this.btnCreateSnapshot.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnCreateSnapshot.Click += new System.EventHandler(this.OnCreateSnapshot);
            // 
            // Worker
            // 
            this.Worker.WorkerReportsProgress = true;
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Worker_DoWork);
            this.Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Worker_ProgressChanged);
            this.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
            // 
            // pnlCloseButton
            // 
            this.pnlCloseButton.Controls.Add(this.btnCloseDialog);
            this.pnlCloseButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCloseButton.Location = new System.Drawing.Point(0, 611);
            this.pnlCloseButton.Name = "pnlCloseButton";
            this.pnlCloseButton.Size = new System.Drawing.Size(1916, 50);
            this.pnlCloseButton.TabIndex = 2;
            // 
            // btnCloseDialog
            // 
            this.btnCloseDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseDialog.Location = new System.Drawing.Point(1814, 8);
            this.btnCloseDialog.Name = "btnCloseDialog";
            this.btnCloseDialog.Size = new System.Drawing.Size(90, 34);
            this.btnCloseDialog.TabIndex = 0;
            this.btnCloseDialog.Text = "Close";
            this.btnCloseDialog.UseVisualStyleBackColor = true;
            this.btnCloseDialog.Click += new System.EventHandler(this.OnCloseDialog);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.pnlGlobalCountersAndTimers);
            this.pnlContainer.Controls.Add(this.pnlAnalyticsEmulator);
            this.pnlContainer.Controls.Add(this.pnlAppSettings);
            this.pnlContainer.Controls.Add(this.pnlMemory);
            this.pnlContainer.Controls.Add(this.pnlDebugCommands);
            this.pnlContainer.Controls.Add(this.pnlAppStatus);
            this.pnlContainer.Controls.Add(this.pnlScreenShots);
            this.pnlContainer.Controls.Add(this.lstOutput);
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(0, 38);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(1916, 573);
            this.pnlContainer.TabIndex = 3;
            // 
            // pnlGlobalCountersAndTimers
            // 
            this.pnlGlobalCountersAndTimers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGlobalCountersAndTimers.Location = new System.Drawing.Point(1564, 17);
            this.pnlGlobalCountersAndTimers.Name = "pnlGlobalCountersAndTimers";
            this.pnlGlobalCountersAndTimers.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // pnlGlobalCountersAndTimers.Panel1
            // 
            this.pnlGlobalCountersAndTimers.Panel1.Controls.Add(this.splitContainer2);
            // 
            // pnlGlobalCountersAndTimers.Panel2
            // 
            this.pnlGlobalCountersAndTimers.Panel2.Controls.Add(this.splitContainer4);
            this.pnlGlobalCountersAndTimers.Panel2.Controls.Add(this.toolStrip2);
            this.pnlGlobalCountersAndTimers.Size = new System.Drawing.Size(340, 404);
            this.pnlGlobalCountersAndTimers.SplitterDistance = 202;
            this.pnlGlobalCountersAndTimers.TabIndex = 10;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lstGlobalCounters);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lstGlobalGroupCounters);
            this.splitContainer2.Size = new System.Drawing.Size(338, 200);
            this.splitContainer2.SplitterDistance = 242;
            this.splitContainer2.TabIndex = 1;
            // 
            // lstGlobalCounters
            // 
            this.lstGlobalCounters.CheckBoxes = true;
            this.lstGlobalCounters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader39,
            this.columnHeader40,
            this.columnHeader41,
            this.columnHeader42,
            this.columnHeader43,
            this.columnHeader44});
            this.lstGlobalCounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstGlobalCounters.FullRowSelect = true;
            this.lstGlobalCounters.GridLines = true;
            this.lstGlobalCounters.Location = new System.Drawing.Point(0, 0);
            this.lstGlobalCounters.Name = "lstGlobalCounters";
            this.lstGlobalCounters.Size = new System.Drawing.Size(242, 200);
            this.lstGlobalCounters.TabIndex = 0;
            this.lstGlobalCounters.UseCompatibleStateImageBehavior = false;
            this.lstGlobalCounters.View = System.Windows.Forms.View.Details;
            this.lstGlobalCounters.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstGlobalCounters_ItemCheck);
            // 
            // columnHeader39
            // 
            this.columnHeader39.Text = "Name";
            this.columnHeader39.Width = 150;
            // 
            // columnHeader40
            // 
            this.columnHeader40.Text = "Group";
            this.columnHeader40.Width = 100;
            // 
            // columnHeader41
            // 
            this.columnHeader41.Text = "Count / Interval";
            this.columnHeader41.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader41.Width = 100;
            // 
            // columnHeader42
            // 
            this.columnHeader42.Text = "Times / MaxTimes";
            this.columnHeader42.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader42.Width = 100;
            // 
            // columnHeader43
            // 
            this.columnHeader43.Text = "Type";
            this.columnHeader43.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader43.Width = 80;
            // 
            // columnHeader44
            // 
            this.columnHeader44.Text = "Actions Count";
            this.columnHeader44.Width = 80;
            // 
            // lstGlobalGroupCounters
            // 
            this.lstGlobalGroupCounters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader45,
            this.columnHeader46,
            this.columnHeader47});
            this.lstGlobalGroupCounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstGlobalGroupCounters.FullRowSelect = true;
            this.lstGlobalGroupCounters.GridLines = true;
            this.lstGlobalGroupCounters.Location = new System.Drawing.Point(0, 0);
            this.lstGlobalGroupCounters.Name = "lstGlobalGroupCounters";
            this.lstGlobalGroupCounters.Size = new System.Drawing.Size(92, 200);
            this.lstGlobalGroupCounters.TabIndex = 0;
            this.lstGlobalGroupCounters.UseCompatibleStateImageBehavior = false;
            this.lstGlobalGroupCounters.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader45
            // 
            this.columnHeader45.Text = "Group name";
            this.columnHeader45.Width = 100;
            // 
            // columnHeader46
            // 
            this.columnHeader46.Text = "Skip times";
            this.columnHeader46.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader47
            // 
            this.columnHeader47.Text = "Timer";
            this.columnHeader47.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // splitContainer4
            // 
            this.splitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 38);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.lstAlarms);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.lstAlarmEvents);
            this.splitContainer4.Size = new System.Drawing.Size(340, 160);
            this.splitContainer4.SplitterDistance = 211;
            this.splitContainer4.TabIndex = 2;
            // 
            // lstAlarms
            // 
            this.lstAlarms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader48,
            this.columnHeader49,
            this.columnHeader50});
            this.lstAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAlarms.FullRowSelect = true;
            this.lstAlarms.GridLines = true;
            this.lstAlarms.Location = new System.Drawing.Point(0, 0);
            this.lstAlarms.MultiSelect = false;
            this.lstAlarms.Name = "lstAlarms";
            this.lstAlarms.Size = new System.Drawing.Size(209, 158);
            this.lstAlarms.TabIndex = 1;
            this.lstAlarms.UseCompatibleStateImageBehavior = false;
            this.lstAlarms.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader48
            // 
            this.columnHeader48.Text = "Name";
            this.columnHeader48.Width = 150;
            // 
            // columnHeader49
            // 
            this.columnHeader49.Text = "Activation";
            this.columnHeader49.Width = 200;
            // 
            // columnHeader50
            // 
            this.columnHeader50.Text = "Duration";
            this.columnHeader50.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader50.Width = 100;
            // 
            // lstAlarmEvents
            // 
            this.lstAlarmEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader51,
            this.columnHeader52});
            this.lstAlarmEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAlarmEvents.FullRowSelect = true;
            this.lstAlarmEvents.GridLines = true;
            this.lstAlarmEvents.Location = new System.Drawing.Point(0, 0);
            this.lstAlarmEvents.Name = "lstAlarmEvents";
            this.lstAlarmEvents.Size = new System.Drawing.Size(123, 158);
            this.lstAlarmEvents.TabIndex = 0;
            this.lstAlarmEvents.UseCompatibleStateImageBehavior = false;
            this.lstAlarmEvents.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader51
            // 
            this.columnHeader51.Text = "Date/Time";
            this.columnHeader51.Width = 120;
            // 
            // columnHeader52
            // 
            this.columnHeader52.Text = "Event";
            this.columnHeader52.Width = 500;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRecheckAlarmTime,
            this.lbAlarmTestTime,
            this.toolStripSeparator5,
            this.btnDateTime});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(340, 38);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnRecheckAlarmTime
            // 
            this.btnRecheckAlarmTime.Image = ((System.Drawing.Image)(resources.GetObject("btnRecheckAlarmTime.Image")));
            this.btnRecheckAlarmTime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRecheckAlarmTime.Name = "btnRecheckAlarmTime";
            this.btnRecheckAlarmTime.Size = new System.Drawing.Size(57, 35);
            this.btnRecheckAlarmTime.Text = "ReCheck";
            this.btnRecheckAlarmTime.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRecheckAlarmTime.Click += new System.EventHandler(this.OnForceRecheckAlarms);
            // 
            // lbAlarmTestTime
            // 
            this.lbAlarmTestTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lbAlarmTestTime.AutoSize = false;
            this.lbAlarmTestTime.Name = "lbAlarmTestTime";
            this.lbAlarmTestTime.Size = new System.Drawing.Size(120, 22);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 38);
            // 
            // btnDateTime
            // 
            this.btnDateTime.Image = ((System.Drawing.Image)(resources.GetObject("btnDateTime.Image")));
            this.btnDateTime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDateTime.Name = "btnDateTime";
            this.btnDateTime.Size = new System.Drawing.Size(86, 35);
            this.btnDateTime.Text = "Set Date/Time";
            this.btnDateTime.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDateTime.Click += new System.EventHandler(this.OnSetDateTime);
            // 
            // pnlAnalyticsEmulator
            // 
            this.pnlAnalyticsEmulator.Controls.Add(this.lstAnalytics);
            this.pnlAnalyticsEmulator.Location = new System.Drawing.Point(784, 367);
            this.pnlAnalyticsEmulator.Name = "pnlAnalyticsEmulator";
            this.pnlAnalyticsEmulator.Size = new System.Drawing.Size(601, 101);
            this.pnlAnalyticsEmulator.TabIndex = 9;
            // 
            // lstAnalytics
            // 
            this.lstAnalytics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader34,
            this.columnHeader35,
            this.columnHeader36,
            this.columnHeader37,
            this.columnHeader38});
            this.lstAnalytics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAnalytics.FullRowSelect = true;
            this.lstAnalytics.GridLines = true;
            this.lstAnalytics.Location = new System.Drawing.Point(0, 0);
            this.lstAnalytics.Name = "lstAnalytics";
            this.lstAnalytics.Size = new System.Drawing.Size(601, 101);
            this.lstAnalytics.TabIndex = 0;
            this.lstAnalytics.UseCompatibleStateImageBehavior = false;
            this.lstAnalytics.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader34
            // 
            this.columnHeader34.Text = "Item";
            this.columnHeader34.Width = 400;
            // 
            // columnHeader35
            // 
            this.columnHeader35.Text = "Count";
            this.columnHeader35.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader36
            // 
            this.columnHeader36.Text = "Total";
            this.columnHeader36.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader37
            // 
            this.columnHeader37.Text = "Average";
            this.columnHeader37.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader38
            // 
            this.columnHeader38.Text = "Last Value";
            this.columnHeader38.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader38.Width = 100;
            // 
            // pnlAppSettings
            // 
            this.pnlAppSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAppSettings.Location = new System.Drawing.Point(1408, 17);
            this.pnlAppSettings.Name = "pnlAppSettings";
            this.pnlAppSettings.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // pnlAppSettings.Panel1
            // 
            this.pnlAppSettings.Panel1.Controls.Add(this.lstAppSettings);
            // 
            // pnlAppSettings.Panel2
            // 
            this.pnlAppSettings.Panel2.Controls.Add(this.ctrlSnapshots);
            this.pnlAppSettings.Size = new System.Drawing.Size(150, 510);
            this.pnlAppSettings.SplitterDistance = 255;
            this.pnlAppSettings.TabIndex = 8;
            // 
            // lstAppSettings
            // 
            this.lstAppSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader13,
            this.columnHeader11,
            this.columnHeader12});
            this.lstAppSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAppSettings.FullRowSelect = true;
            this.lstAppSettings.GridLines = true;
            this.lstAppSettings.Location = new System.Drawing.Point(0, 0);
            this.lstAppSettings.Name = "lstAppSettings";
            this.lstAppSettings.Size = new System.Drawing.Size(148, 253);
            this.lstAppSettings.TabIndex = 2;
            this.lstAppSettings.UseCompatibleStateImageBehavior = false;
            this.lstAppSettings.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Field Name";
            this.columnHeader9.Width = 200;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Type";
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Array";
            this.columnHeader13.Width = 40;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Value(s)";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Binary";
            this.columnHeader12.Width = 600;
            // 
            // ctrlSnapshots
            // 
            this.ctrlSnapshots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlSnapshots.Location = new System.Drawing.Point(0, 0);
            this.ctrlSnapshots.Name = "ctrlSnapshots";
            this.ctrlSnapshots.Size = new System.Drawing.Size(148, 249);
            this.ctrlSnapshots.TabIndex = 0;
            this.ctrlSnapshots.OnSelectSnapshot += new GAppCreator.SettingsSnapshotControl.OnSelectSnapshotHandler(this.OnStartFfromASnapshot);
            // 
            // pnlMemory
            // 
            this.pnlMemory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMemory.Location = new System.Drawing.Point(763, 16);
            this.pnlMemory.Name = "pnlMemory";
            this.pnlMemory.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // pnlMemory.Panel1
            // 
            this.pnlMemory.Panel1.Controls.Add(this.splitContainer3);
            // 
            // pnlMemory.Panel2
            // 
            this.pnlMemory.Panel2.Controls.Add(this.lstMemoryEvents);
            this.pnlMemory.Panel2.Controls.Add(this.toolStrip1);
            this.pnlMemory.Size = new System.Drawing.Size(622, 336);
            this.pnlMemory.SplitterDistance = 168;
            this.pnlMemory.TabIndex = 7;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lstMemory);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.lstMemoryStats);
            this.splitContainer3.Size = new System.Drawing.Size(622, 168);
            this.splitContainer3.SplitterDistance = 412;
            this.splitContainer3.TabIndex = 0;
            // 
            // lstMemory
            // 
            this.lstMemory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader27,
            this.columnHeader28,
            this.columnHeader29,
            this.columnHeader30,
            this.columnHeader31});
            this.lstMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMemory.FullRowSelect = true;
            this.lstMemory.GridLines = true;
            this.lstMemory.Location = new System.Drawing.Point(0, 0);
            this.lstMemory.Name = "lstMemory";
            this.lstMemory.Size = new System.Drawing.Size(410, 166);
            this.lstMemory.TabIndex = 0;
            this.lstMemory.UseCompatibleStateImageBehavior = false;
            this.lstMemory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader27
            // 
            this.columnHeader27.Text = "Address";
            this.columnHeader27.Width = 80;
            // 
            // columnHeader28
            // 
            this.columnHeader28.Text = "Size";
            this.columnHeader28.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader29
            // 
            this.columnHeader29.Text = "Type";
            this.columnHeader29.Width = 150;
            // 
            // columnHeader30
            // 
            this.columnHeader30.Text = "Function";
            this.columnHeader30.Width = 150;
            // 
            // columnHeader31
            // 
            this.columnHeader31.Text = "Code";
            this.columnHeader31.Width = 700;
            // 
            // lstMemoryStats
            // 
            this.lstMemoryStats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader32,
            this.columnHeader33});
            this.lstMemoryStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMemoryStats.FullRowSelect = true;
            this.lstMemoryStats.GridLines = true;
            this.lstMemoryStats.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstMemoryStats.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem7,
            listViewItem8,
            listViewItem9});
            this.lstMemoryStats.Location = new System.Drawing.Point(0, 0);
            this.lstMemoryStats.Name = "lstMemoryStats";
            this.lstMemoryStats.Size = new System.Drawing.Size(204, 166);
            this.lstMemoryStats.TabIndex = 0;
            this.lstMemoryStats.UseCompatibleStateImageBehavior = false;
            this.lstMemoryStats.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader32
            // 
            this.columnHeader32.Width = 120;
            // 
            // columnHeader33
            // 
            this.columnHeader33.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader33.Width = 70;
            // 
            // lstMemoryEvents
            // 
            this.lstMemoryEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader18,
            this.columnHeader19,
            this.columnHeader20,
            this.columnHeader25,
            this.columnHeader21,
            this.columnHeader22,
            this.columnHeader23,
            this.columnHeader24,
            this.columnHeader26});
            this.lstMemoryEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMemoryEvents.FullRowSelect = true;
            this.lstMemoryEvents.GridLines = true;
            this.lstMemoryEvents.Location = new System.Drawing.Point(0, 38);
            this.lstMemoryEvents.MultiSelect = false;
            this.lstMemoryEvents.Name = "lstMemoryEvents";
            this.lstMemoryEvents.ShowItemToolTips = true;
            this.lstMemoryEvents.Size = new System.Drawing.Size(620, 124);
            this.lstMemoryEvents.TabIndex = 1;
            this.lstMemoryEvents.UseCompatibleStateImageBehavior = false;
            this.lstMemoryEvents.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Date";
            this.columnHeader18.Width = 140;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Operation";
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "Address";
            this.columnHeader20.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader20.Width = 80;
            // 
            // columnHeader25
            // 
            this.columnHeader25.Text = "Type";
            this.columnHeader25.Width = 120;
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "Size";
            this.columnHeader21.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "Array";
            this.columnHeader22.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader22.Width = 40;
            // 
            // columnHeader23
            // 
            this.columnHeader23.Text = "Function";
            this.columnHeader23.Width = 150;
            // 
            // columnHeader24
            // 
            this.columnHeader24.Text = "Line";
            this.columnHeader24.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader24.Width = 50;
            // 
            // columnHeader26
            // 
            this.columnHeader26.Text = "Code";
            this.columnHeader26.Width = 700;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator4,
            this.toolStripButton1,
            this.toolStripLabel2,
            this.txMemoryFilter,
            this.lbMemoryEventsInfo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(620, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(244, 35);
            this.toolStripLabel1.Text = "Memory allocation and deallocation events";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbShowMemoryNewEvents,
            this.cbShowMemoryDeleteEvents,
            this.toolStripMenuItem2,
            this.clearEventListToolStripMenuItem});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(62, 35);
            this.toolStripButton1.Text = "Options";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbShowMemoryNewEvents
            // 
            this.cbShowMemoryNewEvents.Checked = true;
            this.cbShowMemoryNewEvents.CheckOnClick = true;
            this.cbShowMemoryNewEvents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowMemoryNewEvents.Name = "cbShowMemoryNewEvents";
            this.cbShowMemoryNewEvents.Size = new System.Drawing.Size(204, 22);
            this.cbShowMemoryNewEvents.Text = "Show \'New\' operations";
            this.cbShowMemoryNewEvents.Click += new System.EventHandler(this.OnChangeMemoryEventsFilter);
            // 
            // cbShowMemoryDeleteEvents
            // 
            this.cbShowMemoryDeleteEvents.Checked = true;
            this.cbShowMemoryDeleteEvents.CheckOnClick = true;
            this.cbShowMemoryDeleteEvents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowMemoryDeleteEvents.Name = "cbShowMemoryDeleteEvents";
            this.cbShowMemoryDeleteEvents.Size = new System.Drawing.Size(204, 22);
            this.cbShowMemoryDeleteEvents.Text = "Show \'Delete\' operations";
            this.cbShowMemoryDeleteEvents.Click += new System.EventHandler(this.OnChangeMemoryEventsFilter);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(201, 6);
            // 
            // clearEventListToolStripMenuItem
            // 
            this.clearEventListToolStripMenuItem.Name = "clearEventListToolStripMenuItem";
            this.clearEventListToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.clearEventListToolStripMenuItem.Text = "Clear event list";
            this.clearEventListToolStripMenuItem.Click += new System.EventHandler(this.OnClearEventsList);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(33, 35);
            this.toolStripLabel2.Text = "Filter";
            // 
            // txMemoryFilter
            // 
            this.txMemoryFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMemoryFilter.Name = "txMemoryFilter";
            this.txMemoryFilter.Size = new System.Drawing.Size(150, 38);
            this.txMemoryFilter.TextChanged += new System.EventHandler(this.OnChangeMemoryEventsFilter);
            // 
            // lbMemoryEventsInfo
            // 
            this.lbMemoryEventsInfo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lbMemoryEventsInfo.AutoSize = false;
            this.lbMemoryEventsInfo.Name = "lbMemoryEventsInfo";
            this.lbMemoryEventsInfo.Size = new System.Drawing.Size(150, 35);
            this.lbMemoryEventsInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlDebugCommands
            // 
            this.pnlDebugCommands.Controls.Add(this.lstDebugCommands);
            this.pnlDebugCommands.Location = new System.Drawing.Point(348, 440);
            this.pnlDebugCommands.Name = "pnlDebugCommands";
            this.pnlDebugCommands.Size = new System.Drawing.Size(409, 116);
            this.pnlDebugCommands.TabIndex = 6;
            // 
            // lstDebugCommands
            // 
            this.lstDebugCommands.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader16,
            this.columnHeader17});
            this.lstDebugCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDebugCommands.FullRowSelect = true;
            this.lstDebugCommands.GridLines = true;
            this.lstDebugCommands.HideSelection = false;
            this.lstDebugCommands.Location = new System.Drawing.Point(0, 0);
            this.lstDebugCommands.MultiSelect = false;
            this.lstDebugCommands.Name = "lstDebugCommands";
            this.lstDebugCommands.Size = new System.Drawing.Size(409, 116);
            this.lstDebugCommands.TabIndex = 0;
            this.lstDebugCommands.UseCompatibleStateImageBehavior = false;
            this.lstDebugCommands.View = System.Windows.Forms.View.Details;
            this.lstDebugCommands.SelectedIndexChanged += new System.EventHandler(this.OnSelectDebugCommand);
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Name";
            this.columnHeader16.Width = 150;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Description";
            this.columnHeader17.Width = 600;
            // 
            // pnlAppStatus
            // 
            this.pnlAppStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAppStatus.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.pnlAppStatus.Location = new System.Drawing.Point(348, 16);
            this.pnlAppStatus.Name = "pnlAppStatus";
            // 
            // pnlAppStatus.Panel1
            // 
            this.pnlAppStatus.Panel1.Controls.Add(this.tabControl1);
            // 
            // pnlAppStatus.Panel2
            // 
            this.pnlAppStatus.Panel2.AutoScroll = true;
            this.pnlAppStatus.Panel2.Controls.Add(this.pnlTextureView);
            this.pnlAppStatus.Size = new System.Drawing.Size(409, 398);
            this.pnlAppStatus.SplitterDistance = 306;
            this.pnlAppStatus.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(304, 396);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propRunSettings);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(296, 370);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propRunSettings
            // 
            this.propRunSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propRunSettings.Location = new System.Drawing.Point(3, 3);
            this.propRunSettings.Name = "propRunSettings";
            this.propRunSettings.Size = new System.Drawing.Size(290, 364);
            this.propRunSettings.TabIndex = 0;
            this.propRunSettings.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnSettingsChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lstAppStatus);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(296, 370);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Status";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lstAppStatus
            // 
            this.lstAppStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader14,
            this.columnHeader15});
            this.lstAppStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAppStatus.FullRowSelect = true;
            this.lstAppStatus.GridLines = true;
            this.lstAppStatus.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstAppStatus.Location = new System.Drawing.Point(3, 3);
            this.lstAppStatus.MultiSelect = false;
            this.lstAppStatus.Name = "lstAppStatus";
            this.lstAppStatus.Size = new System.Drawing.Size(290, 364);
            this.lstAppStatus.TabIndex = 1;
            this.lstAppStatus.UseCompatibleStateImageBehavior = false;
            this.lstAppStatus.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Width = 120;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Width = 160;
            // 
            // pnlTextureView
            // 
            this.pnlTextureView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTextureView.Location = new System.Drawing.Point(0, 0);
            this.pnlTextureView.Name = "pnlTextureView";
            this.pnlTextureView.Size = new System.Drawing.Size(200, 100);
            this.pnlTextureView.TabIndex = 0;
            this.pnlTextureView.Visible = false;
            this.pnlTextureView.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintTexture);
            // 
            // pnlScreenShots
            // 
            this.pnlScreenShots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlScreenShots.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.pnlScreenShots.Location = new System.Drawing.Point(21, 232);
            this.pnlScreenShots.Name = "pnlScreenShots";
            // 
            // pnlScreenShots.Panel1
            // 
            this.pnlScreenShots.Panel1.AutoScroll = true;
            this.pnlScreenShots.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlScreenShots.Panel1.Controls.Add(this.pnlFullScreenShotView);
            // 
            // pnlScreenShots.Panel2
            // 
            this.pnlScreenShots.Panel2.Controls.Add(this.splitContainer1);
            this.pnlScreenShots.Size = new System.Drawing.Size(312, 324);
            this.pnlScreenShots.SplitterDistance = 58;
            this.pnlScreenShots.TabIndex = 1;
            // 
            // pnlFullScreenShotView
            // 
            this.pnlFullScreenShotView.Location = new System.Drawing.Point(0, 0);
            this.pnlFullScreenShotView.Name = "pnlFullScreenShotView";
            this.pnlFullScreenShotView.Size = new System.Drawing.Size(200, 100);
            this.pnlFullScreenShotView.TabIndex = 0;
            this.pnlFullScreenShotView.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintScreenShot);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlSmallImageScreenShot);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstScreenShotProperties);
            this.splitContainer1.Size = new System.Drawing.Size(250, 324);
            this.splitContainer1.SplitterDistance = 262;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // pnlSmallImageScreenShot
            // 
            this.pnlSmallImageScreenShot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSmallImageScreenShot.Location = new System.Drawing.Point(0, 0);
            this.pnlSmallImageScreenShot.Name = "pnlSmallImageScreenShot";
            this.pnlSmallImageScreenShot.Size = new System.Drawing.Size(248, 260);
            this.pnlSmallImageScreenShot.TabIndex = 3;
            this.pnlSmallImageScreenShot.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintSmallImageScreenShot);
            // 
            // lstScreenShotProperties
            // 
            this.lstScreenShotProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8});
            this.lstScreenShotProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstScreenShotProperties.FullRowSelect = true;
            this.lstScreenShotProperties.GridLines = true;
            this.lstScreenShotProperties.Location = new System.Drawing.Point(0, 0);
            this.lstScreenShotProperties.Name = "lstScreenShotProperties";
            this.lstScreenShotProperties.Size = new System.Drawing.Size(248, 56);
            this.lstScreenShotProperties.TabIndex = 0;
            this.lstScreenShotProperties.UseCompatibleStateImageBehavior = false;
            this.lstScreenShotProperties.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Property";
            this.columnHeader7.Width = 120;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Value";
            this.columnHeader8.Width = 120;
            // 
            // lstOutput
            // 
            this.lstOutput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lstOutput.FullRowSelect = true;
            this.lstOutput.GridLines = true;
            this.lstOutput.Location = new System.Drawing.Point(21, 16);
            this.lstOutput.Name = "lstOutput";
            this.lstOutput.Size = new System.Drawing.Size(160, 191);
            this.lstOutput.SmallImageList = this.smallIcons;
            this.lstOutput.TabIndex = 0;
            this.lstOutput.UseCompatibleStateImageBehavior = false;
            this.lstOutput.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Type";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Date";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "File";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Function";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Condition";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Message";
            this.columnHeader5.Width = 800;
            // 
            // smallIcons
            // 
            this.smallIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallIcons.ImageStream")));
            this.smallIcons.TransparentColor = System.Drawing.Color.Magenta;
            this.smallIcons.Images.SetKeyName(0, "cpperror");
            this.smallIcons.Images.SetKeyName(1, "cppevent");
            this.smallIcons.Images.SetKeyName(2, "cppinfo");
            this.smallIcons.Images.SetKeyName(3, "error");
            this.smallIcons.Images.SetKeyName(4, "info");
            // 
            // restartTimer
            // 
            this.restartTimer.Tick += new System.EventHandler(this.OnTryToRestart);
            // 
            // memTimer
            // 
            this.memTimer.Enabled = true;
            this.memTimer.Interval = 1000;
            this.memTimer.Tick += new System.EventHandler(this.OnRefreshMemory);
            // 
            // btnSaveTexture
            // 
            this.btnSaveTexture.Enabled = false;
            this.btnSaveTexture.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveTexture.Image")));
            this.btnSaveTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveTexture.Name = "btnSaveTexture";
            this.btnSaveTexture.Size = new System.Drawing.Size(77, 35);
            this.btnSaveTexture.Text = "Save Texture";
            this.btnSaveTexture.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSaveTexture.Click += new System.EventHandler(this.OnSaveTexture);
            // 
            // ExecutionControlDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1916, 661);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.pnlCloseButton);
            this.Controls.Add(this.toolStrip);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExecutionControlDialog";
            this.Text = "ExecutionControlDialog";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.OnFormActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExecutionControlDialog_FormClosing);
            this.Load += new System.EventHandler(this.ExecutionControlDialog_Load);
            this.Shown += new System.EventHandler(this.OnDialogIsShown);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pnlCloseButton.ResumeLayout(false);
            this.pnlContainer.ResumeLayout(false);
            this.pnlGlobalCountersAndTimers.Panel1.ResumeLayout(false);
            this.pnlGlobalCountersAndTimers.Panel2.ResumeLayout(false);
            this.pnlGlobalCountersAndTimers.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGlobalCountersAndTimers)).EndInit();
            this.pnlGlobalCountersAndTimers.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.pnlAnalyticsEmulator.ResumeLayout(false);
            this.pnlAppSettings.Panel1.ResumeLayout(false);
            this.pnlAppSettings.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlAppSettings)).EndInit();
            this.pnlAppSettings.ResumeLayout(false);
            this.pnlMemory.Panel1.ResumeLayout(false);
            this.pnlMemory.Panel2.ResumeLayout(false);
            this.pnlMemory.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMemory)).EndInit();
            this.pnlMemory.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlDebugCommands.ResumeLayout(false);
            this.pnlAppStatus.Panel1.ResumeLayout(false);
            this.pnlAppStatus.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlAppStatus)).EndInit();
            this.pnlAppStatus.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.pnlScreenShots.Panel1.ResumeLayout(false);
            this.pnlScreenShots.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlScreenShots)).EndInit();
            this.pnlScreenShots.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem cbViewOutput;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.ComponentModel.BackgroundWorker Worker;
        private System.Windows.Forms.Panel pnlCloseButton;
        private System.Windows.Forms.Button btnCloseDialog;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.ListView lstOutput;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStripButton cbMaximize;
        private System.Windows.Forms.ImageList smallIcons;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripButton btnPause;
        private System.Windows.Forms.ToolStripButton btnResume;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton cbAutoScroll;
        private System.Windows.Forms.ToolStripButton btnClearLog;
        private System.Windows.Forms.ToolStripButton btnTakePicture;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem rbScale100;
        private System.Windows.Forms.ToolStripMenuItem rbScale50;
        private System.Windows.Forms.ToolStripMenuItem rbScale25;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.SplitContainer pnlScreenShots;
        private System.Windows.Forms.Panel pnlFullScreenShotView;
        private System.Windows.Forms.ToolStripMenuItem cbScreenShots;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lstScreenShotProperties;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Panel pnlSmallImageScreenShot;
        private System.Windows.Forms.ToolStripButton btnSavePicture;
        private System.Windows.Forms.ToolStripDropDownButton btnFilterLogMenu;
        private System.Windows.Forms.ToolStripMenuItem cbGacMessages;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cbFrameworkErrors;
        private System.Windows.Forms.ToolStripMenuItem cbFrameworkEvents;
        private System.Windows.Forms.ToolStripMenuItem cbFrameworkMessages;
        private System.Windows.Forms.ToolStripLabel lbFilterLog;
        private System.Windows.Forms.ToolStripTextBox txFilterLog;
        private System.Windows.Forms.ToolStripDropDownButton btnSaveLog;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sepFilterLog;
        private System.Windows.Forms.ToolStripMenuItem cbGacErrors;
        private System.Windows.Forms.ToolStripMenuItem cbApplicationStatus;
        private System.Windows.Forms.ToolStripMenuItem cbApplicationSettings;
        private System.Windows.Forms.ListView lstAppSettings;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnSuspend;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.SplitContainer pnlAppStatus;
        private System.Windows.Forms.ListView lstAppStatus;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.PropertyGrid propRunSettings;
        private System.Windows.Forms.ToolStripButton btnGoToScene;
        private System.Windows.Forms.ToolStripButton btnViewTexture;
        private System.Windows.Forms.Panel pnlTextureView;
        private System.Windows.Forms.ToolStripButton btnDeleteSettings;
        private System.Windows.Forms.ToolStripButton btnCopyImage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripButton btnPublish;
        private System.Windows.Forms.ToolStripButton btnRestart;
        private System.Windows.Forms.Timer restartTimer;
        private System.Windows.Forms.ToolStripMenuItem cbDebugCommands;
        private System.Windows.Forms.Panel pnlDebugCommands;
        private System.Windows.Forms.ListView lstDebugCommands;
        private System.Windows.Forms.ToolStripLabel lbDebugCommandQuickSearch;
        private System.Windows.Forms.ToolStripTextBox txDebugCommandFilter;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ToolStripButton btnRunDebugCommand;
        private System.Windows.Forms.SplitContainer pnlMemory;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ListView lstMemoryEvents;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListView lstMemory;
        private System.Windows.Forms.ToolStripMenuItem cbMemory;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.ColumnHeader columnHeader21;
        private System.Windows.Forms.ColumnHeader columnHeader22;
        private System.Windows.Forms.ColumnHeader columnHeader23;
        private System.Windows.Forms.ColumnHeader columnHeader24;
        private System.Windows.Forms.ColumnHeader columnHeader25;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ColumnHeader columnHeader26;
        private System.Windows.Forms.ColumnHeader columnHeader27;
        private System.Windows.Forms.ColumnHeader columnHeader28;
        private System.Windows.Forms.ColumnHeader columnHeader29;
        private System.Windows.Forms.ColumnHeader columnHeader30;
        private System.Windows.Forms.ColumnHeader columnHeader31;
        private System.Windows.Forms.ListView lstMemoryStats;
        private System.Windows.Forms.ColumnHeader columnHeader32;
        private System.Windows.Forms.ColumnHeader columnHeader33;
        private System.Windows.Forms.Timer memTimer;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem cbShowMemoryNewEvents;
        private System.Windows.Forms.ToolStripMenuItem cbShowMemoryDeleteEvents;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem clearEventListToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox txMemoryFilter;
        private System.Windows.Forms.ToolStripLabel lbMemoryEventsInfo;
        private System.Windows.Forms.ToolStripButton btnPin;
        private System.Windows.Forms.ToolStripButton btnCreateSnapshot;
        private System.Windows.Forms.SplitContainer pnlAppSettings;
        private SettingsSnapshotControl ctrlSnapshots;
        private System.Windows.Forms.ToolStripMenuItem cbAnalyticsEmulator;
        private System.Windows.Forms.Panel pnlAnalyticsEmulator;
        private System.Windows.Forms.ListView lstAnalytics;
        private System.Windows.Forms.ColumnHeader columnHeader34;
        private System.Windows.Forms.ColumnHeader columnHeader35;
        private System.Windows.Forms.ColumnHeader columnHeader36;
        private System.Windows.Forms.ColumnHeader columnHeader37;
        private System.Windows.Forms.ColumnHeader columnHeader38;
        private System.Windows.Forms.ToolStripMenuItem cbAlarmsAndCounters;
        private System.Windows.Forms.SplitContainer pnlGlobalCountersAndTimers;
        private System.Windows.Forms.ListView lstGlobalCounters;
        private System.Windows.Forms.ColumnHeader columnHeader39;
        private System.Windows.Forms.ColumnHeader columnHeader40;
        private System.Windows.Forms.ColumnHeader columnHeader41;
        private System.Windows.Forms.ColumnHeader columnHeader42;
        private System.Windows.Forms.ColumnHeader columnHeader43;
        private System.Windows.Forms.ColumnHeader columnHeader44;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView lstGlobalGroupCounters;
        private System.Windows.Forms.ColumnHeader columnHeader45;
        private System.Windows.Forms.ColumnHeader columnHeader46;
        private System.Windows.Forms.ColumnHeader columnHeader47;
        private System.Windows.Forms.ListView lstAlarms;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnRecheckAlarmTime;
        private System.Windows.Forms.ColumnHeader columnHeader48;
        private System.Windows.Forms.ColumnHeader columnHeader49;
        private System.Windows.Forms.ColumnHeader columnHeader50;
        private System.Windows.Forms.ToolStripLabel lbAlarmTestTime;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.ListView lstAlarmEvents;
        private System.Windows.Forms.ColumnHeader columnHeader51;
        private System.Windows.Forms.ColumnHeader columnHeader52;
        private System.Windows.Forms.ToolStripButton btnDateTime;
        private System.Windows.Forms.ToolStripButton btnSaveTexture;
    }
}