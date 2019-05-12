namespace GAppCreator
{
    partial class GradientEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GradientEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.lastStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.firstStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beforeCurrentStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.afterCurrentStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbLinear = new System.Windows.Forms.ToolStripMenuItem();
            this.cbCircular = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.distributeUniformlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propStops = new System.Windows.Forms.PropertyGrid();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.lstStops = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.distributeAlphaUniformlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.distributeRedChanelAfterStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distributeGreenChanelAfterStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distributeBlueChanelAfterStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator2,
            this.toolStripButton3,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.txName});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(575, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lastStopToolStripMenuItem,
            this.firstStopToolStripMenuItem,
            this.beforeCurrentStopToolStripMenuItem,
            this.afterCurrentStopToolStripMenuItem});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(42, 35);
            this.toolStripButton1.Text = "Add";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // lastStopToolStripMenuItem
            // 
            this.lastStopToolStripMenuItem.Name = "lastStopToolStripMenuItem";
            this.lastStopToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.lastStopToolStripMenuItem.Text = "Last stop";
            this.lastStopToolStripMenuItem.Click += new System.EventHandler(this.OnAddLastStop);
            // 
            // firstStopToolStripMenuItem
            // 
            this.firstStopToolStripMenuItem.Name = "firstStopToolStripMenuItem";
            this.firstStopToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.firstStopToolStripMenuItem.Text = "First stop";
            this.firstStopToolStripMenuItem.Click += new System.EventHandler(this.OnAddFirstStop);
            // 
            // beforeCurrentStopToolStripMenuItem
            // 
            this.beforeCurrentStopToolStripMenuItem.Name = "beforeCurrentStopToolStripMenuItem";
            this.beforeCurrentStopToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.beforeCurrentStopToolStripMenuItem.Text = "Before current stop";
            this.beforeCurrentStopToolStripMenuItem.Click += new System.EventHandler(this.OnAddBeforCurrentStop);
            // 
            // afterCurrentStopToolStripMenuItem
            // 
            this.afterCurrentStopToolStripMenuItem.Name = "afterCurrentStopToolStripMenuItem";
            this.afterCurrentStopToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.afterCurrentStopToolStripMenuItem.Text = "After current stop";
            this.afterCurrentStopToolStripMenuItem.Click += new System.EventHandler(this.OnAddAfterCurrentStop);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(44, 35);
            this.toolStripButton2.Text = "Delete";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.Click += new System.EventHandler(this.OnDeleteStops);
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
            this.toolStripButton3.Size = new System.Drawing.Size(40, 35);
            this.toolStripButton3.Text = "Color";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton3.Click += new System.EventHandler(this.OnSelectColor);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbLinear,
            this.cbCircular});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(46, 35);
            this.toolStripDropDownButton1.Text = "Type";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbLinear
            // 
            this.cbLinear.Name = "cbLinear";
            this.cbLinear.Size = new System.Drawing.Size(152, 22);
            this.cbLinear.Text = "Linear";
            this.cbLinear.Click += new System.EventHandler(this.OnChangeGradientType);
            // 
            // cbCircular
            // 
            this.cbCircular.Name = "cbCircular";
            this.cbCircular.Size = new System.Drawing.Size(152, 22);
            this.cbCircular.Text = "Circular";
            this.cbCircular.Click += new System.EventHandler(this.OnChangeGradientType);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(39, 35);
            this.toolStripLabel1.Text = "Name";
            // 
            // txName
            // 
            this.txName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(100, 38);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.distributeUniformlyToolStripMenuItem,
            this.backgroundColorToolStripMenuItem,
            this.toolStripMenuItem1,
            this.distributeAlphaUniformlyToolStripMenuItem,
            this.distributeRedChanelAfterStopToolStripMenuItem,
            this.distributeGreenChanelAfterStopToolStripMenuItem,
            this.distributeBlueChanelAfterStopToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(62, 35);
            this.toolStripDropDownButton2.Text = "Options";
            this.toolStripDropDownButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // distributeUniformlyToolStripMenuItem
            // 
            this.distributeUniformlyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("distributeUniformlyToolStripMenuItem.Image")));
            this.distributeUniformlyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.distributeUniformlyToolStripMenuItem.Name = "distributeUniformlyToolStripMenuItem";
            this.distributeUniformlyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.distributeUniformlyToolStripMenuItem.Text = "Distribute uniformly";
            this.distributeUniformlyToolStripMenuItem.Click += new System.EventHandler(this.OnDistributeUniformly);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("backgroundColorToolStripMenuItem.Image")));
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.backgroundColorToolStripMenuItem.Text = "Background color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.OnChangeBackColor);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Location = new System.Drawing.Point(0, 38);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(575, 356);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstStops);
            this.splitContainer1.Size = new System.Drawing.Size(573, 354);
            this.splitContainer1.SplitterDistance = 226;
            this.splitContainer1.TabIndex = 0;
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
            this.splitContainer2.Panel1.Controls.Add(this.propStops);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pnlPreview);
            this.splitContainer2.Size = new System.Drawing.Size(226, 354);
            this.splitContainer2.SplitterDistance = 243;
            this.splitContainer2.TabIndex = 0;
            // 
            // propStops
            // 
            this.propStops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propStops.Location = new System.Drawing.Point(0, 0);
            this.propStops.Name = "propStops";
            this.propStops.Size = new System.Drawing.Size(224, 241);
            this.propStops.TabIndex = 0;
            this.propStops.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnStopIsChanged);
            // 
            // pnlPreview
            // 
            this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPreview.Location = new System.Drawing.Point(0, 0);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(224, 105);
            this.pnlPreview.TabIndex = 0;
            this.pnlPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPreviewGradient);
            // 
            // lstStops
            // 
            this.lstStops.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lstStops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStops.FullRowSelect = true;
            this.lstStops.GridLines = true;
            this.lstStops.HideSelection = false;
            this.lstStops.Location = new System.Drawing.Point(0, 0);
            this.lstStops.Name = "lstStops";
            this.lstStops.Size = new System.Drawing.Size(341, 352);
            this.lstStops.TabIndex = 0;
            this.lstStops.UseCompatibleStateImageBehavior = false;
            this.lstStops.View = System.Windows.Forms.View.Details;
            this.lstStops.SelectedIndexChanged += new System.EventHandler(this.OnStopSelected);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Color";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Stop (%)";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Red";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 45;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Green";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 45;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Blue";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 45;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Alpha";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 45;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(417, 400);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnValidate);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(498, 400);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 31);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // distributeAlphaUniformlyToolStripMenuItem
            // 
            this.distributeAlphaUniformlyToolStripMenuItem.Name = "distributeAlphaUniformlyToolStripMenuItem";
            this.distributeAlphaUniformlyToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.distributeAlphaUniformlyToolStripMenuItem.Text = "Distribute Alpha chanel after Stop";
            this.distributeAlphaUniformlyToolStripMenuItem.Click += new System.EventHandler(this.OnDistributeAlphaAfterStop);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(248, 6);
            // 
            // distributeRedChanelAfterStopToolStripMenuItem
            // 
            this.distributeRedChanelAfterStopToolStripMenuItem.Name = "distributeRedChanelAfterStopToolStripMenuItem";
            this.distributeRedChanelAfterStopToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.distributeRedChanelAfterStopToolStripMenuItem.Text = "Distribute Red chanel after Stop";
            this.distributeRedChanelAfterStopToolStripMenuItem.Click += new System.EventHandler(this.OnDistributeRedAfterStop);
            // 
            // distributeGreenChanelAfterStopToolStripMenuItem
            // 
            this.distributeGreenChanelAfterStopToolStripMenuItem.Name = "distributeGreenChanelAfterStopToolStripMenuItem";
            this.distributeGreenChanelAfterStopToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.distributeGreenChanelAfterStopToolStripMenuItem.Text = "Distribute Green chanel after Stop";
            this.distributeGreenChanelAfterStopToolStripMenuItem.Click += new System.EventHandler(this.OnDistributeGreenAfterStop);
            // 
            // distributeBlueChanelAfterStopToolStripMenuItem
            // 
            this.distributeBlueChanelAfterStopToolStripMenuItem.Name = "distributeBlueChanelAfterStopToolStripMenuItem";
            this.distributeBlueChanelAfterStopToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.distributeBlueChanelAfterStopToolStripMenuItem.Text = "Distribute Blue chanel after Stop";
            this.distributeBlueChanelAfterStopToolStripMenuItem.Click += new System.EventHandler(this.OnDistributeBlueAfterStop);
            // 
            // GradientEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 438);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GradientEditor";
            this.Text = "GradientEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
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

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PropertyGrid propStops;
        private System.Windows.Forms.ListView lstStops;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem lastStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem firstStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beforeCurrentStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem afterCurrentStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem cbLinear;
        private System.Windows.Forms.ToolStripMenuItem cbCircular;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txName;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem distributeUniformlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distributeAlphaUniformlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem distributeRedChanelAfterStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distributeGreenChanelAfterStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distributeBlueChanelAfterStopToolStripMenuItem;
    }
}