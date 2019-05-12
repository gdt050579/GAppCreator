namespace GAppCreator
{
    partial class CounterGroupSimulator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CounterGroupSimulator));
            this.pnlCloseButton = new System.Windows.Forms.Panel();
            this.btnCloseDialog = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.comboBuilds = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.comboGroups = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbGroupType = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbIt25 = new System.Windows.Forms.ToolStripMenuItem();
            this.cbIt50 = new System.Windows.Forms.ToolStripMenuItem();
            this.cbIt100 = new System.Windows.Forms.ToolStripMenuItem();
            this.cbIt200 = new System.Windows.Forms.ToolStripMenuItem();
            this.cbIt500 = new System.Windows.Forms.ToolStripMenuItem();
            this.cbIt1000 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cbHideEmptyIterations = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstCounters = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstSim = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lstStatistics = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlCloseButton.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCloseButton
            // 
            this.pnlCloseButton.Controls.Add(this.btnCloseDialog);
            this.pnlCloseButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCloseButton.Location = new System.Drawing.Point(0, 445);
            this.pnlCloseButton.Name = "pnlCloseButton";
            this.pnlCloseButton.Size = new System.Drawing.Size(902, 50);
            this.pnlCloseButton.TabIndex = 3;
            // 
            // btnCloseDialog
            // 
            this.btnCloseDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseDialog.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCloseDialog.Location = new System.Drawing.Point(800, 8);
            this.btnCloseDialog.Name = "btnCloseDialog";
            this.btnCloseDialog.Size = new System.Drawing.Size(90, 34);
            this.btnCloseDialog.TabIndex = 0;
            this.btnCloseDialog.Text = "Close";
            this.btnCloseDialog.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.comboBuilds,
            this.toolStripSeparator3,
            this.toolStripLabel1,
            this.comboGroups,
            this.toolStripSeparator1,
            this.lbGroupType,
            this.toolStripSeparator2,
            this.toolStripDropDownButton1,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(902, 38);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(34, 35);
            this.toolStripLabel2.Text = "Build";
            // 
            // comboBuilds
            // 
            this.comboBuilds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBuilds.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.comboBuilds.Name = "comboBuilds";
            this.comboBuilds.Size = new System.Drawing.Size(121, 38);
            this.comboBuilds.SelectedIndexChanged += new System.EventHandler(this.OnSelectBuild);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(40, 35);
            this.toolStripLabel1.Text = "Group";
            // 
            // comboGroups
            // 
            this.comboGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGroups.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.comboGroups.Name = "comboGroups";
            this.comboGroups.Size = new System.Drawing.Size(121, 38);
            this.comboGroups.SelectedIndexChanged += new System.EventHandler(this.OnSelectGroup);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // lbGroupType
            // 
            this.lbGroupType.AutoSize = false;
            this.lbGroupType.Name = "lbGroupType";
            this.lbGroupType.Size = new System.Drawing.Size(360, 22);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbIt25,
            this.cbIt50,
            this.cbIt100,
            this.cbIt200,
            this.cbIt500,
            this.cbIt1000,
            this.toolStripMenuItem1,
            this.cbHideEmptyIterations});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(62, 35);
            this.toolStripDropDownButton1.Text = "Settings";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbIt25
            // 
            this.cbIt25.Name = "cbIt25";
            this.cbIt25.Size = new System.Drawing.Size(288, 22);
            this.cbIt25.Text = "25 iterations";
            this.cbIt25.Click += new System.EventHandler(this.OnSelectIterations);
            // 
            // cbIt50
            // 
            this.cbIt50.Name = "cbIt50";
            this.cbIt50.Size = new System.Drawing.Size(288, 22);
            this.cbIt50.Text = "50 iterations";
            this.cbIt50.Click += new System.EventHandler(this.OnSelectIterations);
            // 
            // cbIt100
            // 
            this.cbIt100.Checked = true;
            this.cbIt100.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIt100.Name = "cbIt100";
            this.cbIt100.Size = new System.Drawing.Size(288, 22);
            this.cbIt100.Text = "100 iterations";
            this.cbIt100.Click += new System.EventHandler(this.OnSelectIterations);
            // 
            // cbIt200
            // 
            this.cbIt200.Name = "cbIt200";
            this.cbIt200.Size = new System.Drawing.Size(288, 22);
            this.cbIt200.Text = "200 iterations";
            this.cbIt200.Click += new System.EventHandler(this.OnSelectIterations);
            // 
            // cbIt500
            // 
            this.cbIt500.Name = "cbIt500";
            this.cbIt500.Size = new System.Drawing.Size(288, 22);
            this.cbIt500.Text = "500 iterations";
            this.cbIt500.Click += new System.EventHandler(this.OnSelectIterations);
            // 
            // cbIt1000
            // 
            this.cbIt1000.Name = "cbIt1000";
            this.cbIt1000.Size = new System.Drawing.Size(288, 22);
            this.cbIt1000.Text = "1000 iterations";
            this.cbIt1000.Click += new System.EventHandler(this.OnSelectIterations);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(285, 6);
            // 
            // cbHideEmptyIterations
            // 
            this.cbHideEmptyIterations.CheckOnClick = true;
            this.cbHideEmptyIterations.Name = "cbHideEmptyIterations";
            this.cbHideEmptyIterations.Size = new System.Drawing.Size(288, 22);
            this.cbHideEmptyIterations.Text = "Hide iteration without a resulted counter";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(92, 35);
            this.toolStripButton1.Text = "Run Simulation";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.OnRunSimulation);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 38);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstSim);
            this.splitContainer1.Size = new System.Drawing.Size(902, 407);
            this.splitContainer1.SplitterDistance = 354;
            this.splitContainer1.TabIndex = 5;
            // 
            // lstCounters
            // 
            this.lstCounters.CheckBoxes = true;
            this.lstCounters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lstCounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCounters.FullRowSelect = true;
            this.lstCounters.GridLines = true;
            this.lstCounters.Location = new System.Drawing.Point(0, 0);
            this.lstCounters.Name = "lstCounters";
            this.lstCounters.Size = new System.Drawing.Size(352, 225);
            this.lstCounters.TabIndex = 0;
            this.lstCounters.UseCompatibleStateImageBehavior = false;
            this.lstCounters.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Counter";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Interval";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Max Times";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Priority";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lstSim
            // 
            this.lstSim.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lstSim.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSim.FullRowSelect = true;
            this.lstSim.GridLines = true;
            this.lstSim.Location = new System.Drawing.Point(0, 0);
            this.lstSim.Name = "lstSim";
            this.lstSim.Size = new System.Drawing.Size(542, 405);
            this.lstSim.TabIndex = 1;
            this.lstSim.UseCompatibleStateImageBehavior = false;
            this.lstSim.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Iteration";
            this.columnHeader5.Width = 50;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Resulted counter";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Extra information";
            this.columnHeader7.Width = 600;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lstCounters);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lstStatistics);
            this.splitContainer2.Size = new System.Drawing.Size(354, 407);
            this.splitContainer2.SplitterDistance = 227;
            this.splitContainer2.TabIndex = 1;
            // 
            // lstStatistics
            // 
            this.lstStatistics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            this.lstStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStatistics.FullRowSelect = true;
            this.lstStatistics.GridLines = true;
            this.lstStatistics.Location = new System.Drawing.Point(0, 0);
            this.lstStatistics.Name = "lstStatistics";
            this.lstStatistics.Size = new System.Drawing.Size(352, 174);
            this.lstStatistics.TabIndex = 0;
            this.lstStatistics.UseCompatibleStateImageBehavior = false;
            this.lstStatistics.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Item";
            this.columnHeader8.Width = 200;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Count";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "%";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // CounterGroupSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCloseDialog;
            this.ClientSize = new System.Drawing.Size(902, 495);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pnlCloseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CounterGroupSimulator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CounterGroupSimulator";
            this.pnlCloseButton.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlCloseButton;
        private System.Windows.Forms.Button btnCloseDialog;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox comboGroups;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lbGroupType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem cbIt25;
        private System.Windows.Forms.ToolStripMenuItem cbIt50;
        private System.Windows.Forms.ToolStripMenuItem cbIt100;
        private System.Windows.Forms.ToolStripMenuItem cbIt200;
        private System.Windows.Forms.ToolStripMenuItem cbIt500;
        private System.Windows.Forms.ToolStripMenuItem cbIt1000;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lstCounters;
        private System.Windows.Forms.ListView lstSim;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cbHideEmptyIterations;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox comboBuilds;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView lstStatistics;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
    }
}