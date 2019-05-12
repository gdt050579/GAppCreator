namespace GAppCreator
{
    partial class SettingsSnapshotControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsSnapshotControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cbListView = new System.Windows.Forms.ToolStripButton();
            this.cbTreeView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txFilter = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.pnlHost = new System.Windows.Forms.Panel();
            this.treeSnapshots = new System.Windows.Forms.TreeView();
            this.lstSnapshots = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1.SuspendLayout();
            this.pnlHost.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbListView,
            this.cbTreeView,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.txFilter,
            this.toolStripSeparator2,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripSeparator3,
            this.toolStripButton5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(751, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // cbListView
            // 
            this.cbListView.Image = ((System.Drawing.Image)(resources.GetObject("cbListView.Image")));
            this.cbListView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbListView.Name = "cbListView";
            this.cbListView.Size = new System.Drawing.Size(57, 35);
            this.cbListView.Text = "List View";
            this.cbListView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbListView.Click += new System.EventHandler(this.OnChangeViewMode);
            // 
            // cbTreeView
            // 
            this.cbTreeView.Image = ((System.Drawing.Image)(resources.GetObject("cbTreeView.Image")));
            this.cbTreeView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbTreeView.Name = "cbTreeView";
            this.cbTreeView.Size = new System.Drawing.Size(62, 35);
            this.cbTreeView.Text = "Tree View";
            this.cbTreeView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbTreeView.Click += new System.EventHandler(this.OnChangeViewMode);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(33, 35);
            this.toolStripLabel1.Text = "Filter";
            // 
            // txFilter
            // 
            this.txFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFilter.Name = "txFilter";
            this.txFilter.Size = new System.Drawing.Size(150, 38);
            this.txFilter.TextChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(44, 35);
            this.toolStripButton1.Text = "Delete";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.OnDeleteSnapshot);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(54, 35);
            this.toolStripButton2.Text = "Rename";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.Click += new System.EventHandler(this.OnRename);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(47, 35);
            this.toolStripButton3.Text = "Import";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton3.Click += new System.EventHandler(this.OnImportSnapshot);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(89, 35);
            this.toolStripButton4.Text = "Change parent";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton4.Click += new System.EventHandler(this.OnChangeParent);
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
            this.toolStripButton5.Size = new System.Drawing.Size(44, 35);
            this.toolStripButton5.Text = "Check";
            this.toolStripButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton5.Click += new System.EventHandler(this.OnValidate);
            // 
            // pnlHost
            // 
            this.pnlHost.Controls.Add(this.treeSnapshots);
            this.pnlHost.Controls.Add(this.lstSnapshots);
            this.pnlHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHost.Location = new System.Drawing.Point(0, 38);
            this.pnlHost.Name = "pnlHost";
            this.pnlHost.Size = new System.Drawing.Size(751, 318);
            this.pnlHost.TabIndex = 1;
            // 
            // treeSnapshots
            // 
            this.treeSnapshots.FullRowSelect = true;
            this.treeSnapshots.HideSelection = false;
            this.treeSnapshots.ItemHeight = 20;
            this.treeSnapshots.Location = new System.Drawing.Point(204, 33);
            this.treeSnapshots.Name = "treeSnapshots";
            this.treeSnapshots.ShowNodeToolTips = true;
            this.treeSnapshots.Size = new System.Drawing.Size(191, 217);
            this.treeSnapshots.TabIndex = 1;
            this.treeSnapshots.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeSnapshots_BeforeCollapse);
            this.treeSnapshots.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeSnapshots_BeforeExpand);
            this.treeSnapshots.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClickedOnTree);
            this.treeSnapshots.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeSnapshots_MouseDown);
            // 
            // lstSnapshots
            // 
            this.lstSnapshots.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstSnapshots.FullRowSelect = true;
            this.lstSnapshots.GridLines = true;
            this.lstSnapshots.HideSelection = false;
            this.lstSnapshots.Location = new System.Drawing.Point(21, 33);
            this.lstSnapshots.Name = "lstSnapshots";
            this.lstSnapshots.Size = new System.Drawing.Size(157, 217);
            this.lstSnapshots.TabIndex = 0;
            this.lstSnapshots.UseCompatibleStateImageBehavior = false;
            this.lstSnapshots.View = System.Windows.Forms.View.Details;
            this.lstSnapshots.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClickedOnList);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ID";
            this.columnHeader4.Width = 40;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Snapshot";
            this.columnHeader1.Width = 400;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Added";
            this.columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Parent snapshot";
            this.columnHeader3.Width = 100;
            // 
            // SettingsSnapshotControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlHost);
            this.Controls.Add(this.toolStrip1);
            this.Name = "SettingsSnapshotControl";
            this.Size = new System.Drawing.Size(751, 356);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlHost.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cbListView;
        private System.Windows.Forms.Panel pnlHost;
        private System.Windows.Forms.ToolStripButton cbTreeView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txFilter;
        private System.Windows.Forms.ListView lstSnapshots;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TreeView treeSnapshots;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}
