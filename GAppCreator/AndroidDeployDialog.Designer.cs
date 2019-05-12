namespace GAppCreator
{
    partial class AndroidDeployDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AndroidDeployDialog));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lbDevices = new System.Windows.Forms.ToolStripLabel();
            this.comboDevices = new System.Windows.Forms.ToolStripComboBox();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbPackages = new System.Windows.Forms.ToolStripLabel();
            this.comboPackages = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbFromAllProcesses = new System.Windows.Forms.ToolStripMenuItem();
            this.cbOnlyMyApp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cbClearUserData = new System.Windows.Forms.ToolStripMenuItem();
            this.cbClearLogCatCahe = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cbInstallAndRun = new System.Windows.Forms.ToolStripMenuItem();
            this.cbJustInstall = new System.Windows.Forms.ToolStripMenuItem();
            this.cbJustRun = new System.Windows.Forms.ToolStripMenuItem();
            this.cbDoNothing = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAutoScroll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lstMessages = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.smallIcons = new System.Windows.Forms.ImageList(this.components);
            this.Worker = new System.ComponentModel.BackgroundWorker();
            this.btnSaveLog = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbDevices,
            this.comboDevices,
            this.btnRefresh,
            this.toolStripSeparator1,
            this.lbPackages,
            this.comboPackages,
            this.toolStripSeparator2,
            this.btnSettings,
            this.btnStart,
            this.btnStop,
            this.toolStripSeparator3,
            this.btnAutoScroll,
            this.toolStripButton1,
            this.btnSaveLog});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1262, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lbDevices
            // 
            this.lbDevices.Name = "lbDevices";
            this.lbDevices.Size = new System.Drawing.Size(47, 35);
            this.lbDevices.Text = "Devices";
            // 
            // comboDevices
            // 
            this.comboDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDevices.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboDevices.Name = "comboDevices";
            this.comboDevices.Size = new System.Drawing.Size(160, 38);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 35);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRefresh.Click += new System.EventHandler(this.OnRefreshDeviceList);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // lbPackages
            // 
            this.lbPackages.Name = "lbPackages";
            this.lbPackages.Size = new System.Drawing.Size(56, 35);
            this.lbPackages.Text = "Packages";
            // 
            // comboPackages
            // 
            this.comboPackages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPackages.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboPackages.Name = "comboPackages";
            this.comboPackages.Size = new System.Drawing.Size(300, 38);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // btnSettings
            // 
            this.btnSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbFromAllProcesses,
            this.cbOnlyMyApp,
            this.toolStripMenuItem1,
            this.cbClearUserData,
            this.cbClearLogCatCahe,
            this.toolStripMenuItem2,
            this.cbInstallAndRun,
            this.cbJustInstall,
            this.cbJustRun,
            this.cbDoNothing});
            this.btnSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.Image")));
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(62, 35);
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbFromAllProcesses
            // 
            this.cbFromAllProcesses.Name = "cbFromAllProcesses";
            this.cbFromAllProcesses.Size = new System.Drawing.Size(309, 22);
            this.cbFromAllProcesses.Text = "Show logs from all running processes";
            this.cbFromAllProcesses.Click += new System.EventHandler(this.OnShowFullLog);
            // 
            // cbOnlyMyApp
            // 
            this.cbOnlyMyApp.Checked = true;
            this.cbOnlyMyApp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOnlyMyApp.Name = "cbOnlyMyApp";
            this.cbOnlyMyApp.Size = new System.Drawing.Size(309, 22);
            this.cbOnlyMyApp.Text = "Show logs from my application";
            this.cbOnlyMyApp.Click += new System.EventHandler(this.OnShowLogFromMyApp);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(306, 6);
            // 
            // cbClearUserData
            // 
            this.cbClearUserData.CheckOnClick = true;
            this.cbClearUserData.Name = "cbClearUserData";
            this.cbClearUserData.Size = new System.Drawing.Size(309, 22);
            this.cbClearUserData.Text = "Clear user data before install";
            // 
            // cbClearLogCatCahe
            // 
            this.cbClearLogCatCahe.Checked = true;
            this.cbClearLogCatCahe.CheckOnClick = true;
            this.cbClearLogCatCahe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbClearLogCatCahe.Name = "cbClearLogCatCahe";
            this.cbClearLogCatCahe.Size = new System.Drawing.Size(309, 22);
            this.cbClearLogCatCahe.Text = "Clear logcat cache before start";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(306, 6);
            // 
            // cbInstallAndRun
            // 
            this.cbInstallAndRun.Checked = true;
            this.cbInstallAndRun.CheckOnClick = true;
            this.cbInstallAndRun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbInstallAndRun.Name = "cbInstallAndRun";
            this.cbInstallAndRun.Size = new System.Drawing.Size(309, 22);
            this.cbInstallAndRun.Text = "Install and Run application";
            this.cbInstallAndRun.Click += new System.EventHandler(this.OnSetInstallAndRun);
            // 
            // cbJustInstall
            // 
            this.cbJustInstall.CheckOnClick = true;
            this.cbJustInstall.Name = "cbJustInstall";
            this.cbJustInstall.Size = new System.Drawing.Size(309, 22);
            this.cbJustInstall.Text = "Install application";
            this.cbJustInstall.Click += new System.EventHandler(this.OnSetJustInstall);
            // 
            // cbJustRun
            // 
            this.cbJustRun.CheckOnClick = true;
            this.cbJustRun.Name = "cbJustRun";
            this.cbJustRun.Size = new System.Drawing.Size(309, 22);
            this.cbJustRun.Text = "Run application (it must be already installed)";
            this.cbJustRun.Click += new System.EventHandler(this.OnSetJustRun);
            // 
            // cbDoNothing
            // 
            this.cbDoNothing.CheckOnClick = true;
            this.cbDoNothing.Name = "cbDoNothing";
            this.cbDoNothing.Size = new System.Drawing.Size(309, 22);
            this.cbDoNothing.Text = "Do nothing (start logging)";
            this.cbDoNothing.Click += new System.EventHandler(this.OnJustLog);
            // 
            // btnStart
            // 
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(35, 35);
            this.btnStart.Text = "Start";
            this.btnStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStart.Click += new System.EventHandler(this.OnStart);
            // 
            // btnStop
            // 
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(35, 35);
            this.btnStop.Text = "Stop";
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStop.Click += new System.EventHandler(this.OnStop);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // btnAutoScroll
            // 
            this.btnAutoScroll.Checked = true;
            this.btnAutoScroll.CheckOnClick = true;
            this.btnAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnAutoScroll.Image = ((System.Drawing.Image)(resources.GetObject("btnAutoScroll.Image")));
            this.btnAutoScroll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAutoScroll.Name = "btnAutoScroll";
            this.btnAutoScroll.Size = new System.Drawing.Size(66, 35);
            this.btnAutoScroll.Text = "AutoScroll";
            this.btnAutoScroll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAutoScroll.Click += new System.EventHandler(this.OnSetAutoScroll);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(38, 35);
            this.toolStripButton1.Text = "Clear";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.OnClearList);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 445);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1262, 54);
            this.panel1.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1175, 14);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.OnTerminateLogSession);
            // 
            // lstMessages
            // 
            this.lstMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3});
            this.lstMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMessages.FullRowSelect = true;
            this.lstMessages.GridLines = true;
            this.lstMessages.Location = new System.Drawing.Point(0, 38);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(1262, 407);
            this.lstMessages.SmallImageList = this.smallIcons;
            this.lstMessages.TabIndex = 3;
            this.lstMessages.UseCompatibleStateImageBehavior = false;
            this.lstMessages.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Module/TAG";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Message";
            this.columnHeader3.Width = 1200;
            // 
            // smallIcons
            // 
            this.smallIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallIcons.ImageStream")));
            this.smallIcons.TransparentColor = System.Drawing.Color.Magenta;
            this.smallIcons.Images.SetKeyName(0, "cpperror");
            this.smallIcons.Images.SetKeyName(1, "cppevent");
            this.smallIcons.Images.SetKeyName(2, "cppinfo");
            this.smallIcons.Images.SetKeyName(3, "javaerror");
            this.smallIcons.Images.SetKeyName(4, "javaevent");
            this.smallIcons.Images.SetKeyName(5, "javainfo");
            this.smallIcons.Images.SetKeyName(6, "framework");
            this.smallIcons.Images.SetKeyName(7, "debug");
            this.smallIcons.Images.SetKeyName(8, "error");
            this.smallIcons.Images.SetKeyName(9, "info");
            this.smallIcons.Images.SetKeyName(10, "warning");
            // 
            // Worker
            // 
            this.Worker.WorkerReportsProgress = true;
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OnStartBackgroundWork);
            this.Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.OnProgressChanged);
            this.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnBackgroundWorkCompleted);
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveLog.Image")));
            this.btnSaveLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(58, 35);
            this.btnSaveLog.Text = "Save Log";
            this.btnSaveLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSaveLog.Click += new System.EventHandler(this.OnSaveLog);
            // 
            // AndroidDeployDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 499);
            this.ControlBox = false;
            this.Controls.Add(this.lstMessages);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "AndroidDeployDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Android Deploy Management Console";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ListView lstMessages;
        private System.Windows.Forms.ToolStripLabel lbDevices;
        private System.Windows.Forms.ToolStripComboBox comboDevices;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lbPackages;
        private System.Windows.Forms.ToolStripComboBox comboPackages;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton btnSettings;
        private System.Windows.Forms.ToolStripMenuItem cbFromAllProcesses;
        private System.Windows.Forms.ToolStripMenuItem cbOnlyMyApp;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cbClearUserData;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cbClearLogCatCahe;
        private System.ComponentModel.BackgroundWorker Worker;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem cbInstallAndRun;
        private System.Windows.Forms.ToolStripMenuItem cbJustInstall;
        private System.Windows.Forms.ToolStripMenuItem cbJustRun;
        private System.Windows.Forms.ToolStripMenuItem cbDoNothing;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ImageList smallIcons;
        private System.Windows.Forms.ToolStripButton btnAutoScroll;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton btnSaveLog;
    }
}