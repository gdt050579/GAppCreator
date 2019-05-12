namespace GAppCreator
{
    partial class ResourceLoadDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceLoadDialog));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton6 = new System.Windows.Forms.ToolStripDropDownButton();
            this.createNewSVGResouceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sVGResourceFromTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.presentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbRasterImage = new System.Windows.Forms.ToolStripMenuItem();
            this.cbVectorImage = new System.Windows.Forms.ToolStripMenuItem();
            this.cbMusic = new System.Windows.Forms.ToolStripMenuItem();
            this.cbPresentation = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cbRaw = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtNameFilter = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.cbOnlyUnused = new System.Windows.Forms.ToolStripButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3,
            this.toolStripDropDownButton6,
            this.toolStripButton1,
            this.toolStripButton6,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.txtNameFilter,
            this.toolStripSeparator2,
            this.toolStripButton2,
            this.cbOnlyUnused});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(768, 38);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.TabStop = true;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(47, 35);
            this.toolStripButton3.Text = "Import";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton3.Click += new System.EventHandler(this.OnImportFiles);
            // 
            // toolStripDropDownButton6
            // 
            this.toolStripDropDownButton6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewSVGResouceToolStripMenuItem1,
            this.sVGResourceFromTemplateToolStripMenuItem,
            this.presentationToolStripMenuItem});
            this.toolStripDropDownButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton6.Image")));
            this.toolStripDropDownButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton6.Name = "toolStripDropDownButton6";
            this.toolStripDropDownButton6.Size = new System.Drawing.Size(54, 35);
            this.toolStripDropDownButton6.Text = "Create";
            this.toolStripDropDownButton6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // createNewSVGResouceToolStripMenuItem1
            // 
            this.createNewSVGResouceToolStripMenuItem1.Name = "createNewSVGResouceToolStripMenuItem1";
            this.createNewSVGResouceToolStripMenuItem1.Size = new System.Drawing.Size(222, 22);
            this.createNewSVGResouceToolStripMenuItem1.Text = "SVG resouce";
            this.createNewSVGResouceToolStripMenuItem1.Click += new System.EventHandler(this.OnCreateNewSVG);
            // 
            // sVGResourceFromTemplateToolStripMenuItem
            // 
            this.sVGResourceFromTemplateToolStripMenuItem.Name = "sVGResourceFromTemplateToolStripMenuItem";
            this.sVGResourceFromTemplateToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.sVGResourceFromTemplateToolStripMenuItem.Text = "SVG resource from template";
            this.sVGResourceFromTemplateToolStripMenuItem.Click += new System.EventHandler(this.sVGResourceFromTemplateToolStripMenuItem_Click);
            // 
            // animationToolStripMenuItem
            // 
            this.presentationToolStripMenuItem.Name = "presentationToolStripMenuItem";
            this.presentationToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.presentationToolStripMenuItem.Text = "Presentation";
            this.presentationToolStripMenuItem.Click += new System.EventHandler(this.OnCreateNewAnimation);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbRasterImage,
            this.cbVectorImage,
            this.cbMusic,
            this.cbPresentation,
            this.toolStripMenuItem1,
            this.cbRaw});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(46, 35);
            this.toolStripButton1.Text = "Type";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbRasterImage
            // 
            this.cbRasterImage.CheckOnClick = true;
            this.cbRasterImage.Name = "cbRasterImage";
            this.cbRasterImage.Size = new System.Drawing.Size(152, 22);
            this.cbRasterImage.Text = "Raster Image";
            this.cbRasterImage.Click += new System.EventHandler(this.OnChangeFileTypeFilter);
            // 
            // cbVectorImage
            // 
            this.cbVectorImage.CheckOnClick = true;
            this.cbVectorImage.Name = "cbVectorImage";
            this.cbVectorImage.Size = new System.Drawing.Size(152, 22);
            this.cbVectorImage.Text = "Vector Image";
            this.cbVectorImage.Click += new System.EventHandler(this.OnChangeFileTypeFilter);
            // 
            // cbMusic
            // 
            this.cbMusic.CheckOnClick = true;
            this.cbMusic.Name = "cbMusic";
            this.cbMusic.Size = new System.Drawing.Size(152, 22);
            this.cbMusic.Text = "Music";
            this.cbMusic.Click += new System.EventHandler(this.OnChangeFileTypeFilter);
            // 
            // cbAnimation
            // 
            this.cbPresentation.CheckOnClick = true;
            this.cbPresentation.Name = "cbPresentation";
            this.cbPresentation.Size = new System.Drawing.Size(152, 22);
            this.cbPresentation.Text = "Presentations";
            this.cbPresentation.Click += new System.EventHandler(this.OnChangeFileTypeFilter);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // cbRaw
            // 
            this.cbRaw.CheckOnClick = true;
            this.cbRaw.Name = "cbRaw";
            this.cbRaw.Size = new System.Drawing.Size(152, 22);
            this.cbRaw.Text = "Other";
            this.cbRaw.Click += new System.EventHandler(this.OnChangeFileTypeFilter);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(31, 35);
            this.toolStripButton6.Text = "Edit";
            this.toolStripButton6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton6.Click += new System.EventHandler(this.OnEditResource);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(44, 35);
            this.toolStripButton4.Text = "Delete";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton4.Click += new System.EventHandler(this.OnDeleteResources);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(74, 35);
            this.toolStripButton5.Text = "Show folder";
            this.toolStripButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton5.Click += new System.EventHandler(this.OnShowResourceFolder);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(76, 35);
            this.toolStripLabel1.Text = "Quick Search";
            // 
            // txtNameFilter
            // 
            this.txtNameFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNameFilter.Name = "txtNameFilter";
            this.txtNameFilter.Size = new System.Drawing.Size(100, 38);
            this.txtNameFilter.TextChanged += new System.EventHandler(this.OnTextFilter);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(50, 35);
            this.toolStripButton2.Text = "Refresh";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.Click += new System.EventHandler(this.OnRefresh);
            // 
            // cbOnlyUnused
            // 
            this.cbOnlyUnused.Checked = true;
            this.cbOnlyUnused.CheckOnClick = true;
            this.cbOnlyUnused.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOnlyUnused.Image = ((System.Drawing.Image)(resources.GetObject("cbOnlyUnused.Image")));
            this.cbOnlyUnused.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbOnlyUnused.Name = "cbOnlyUnused";
            this.cbOnlyUnused.Size = new System.Drawing.Size(79, 35);
            this.cbOnlyUnused.Text = "Only Unused";
            this.cbOnlyUnused.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbOnlyUnused.Click += new System.EventHandler(this.OnClickOnlyUnused);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(600, 469);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 32);
            this.button1.TabIndex = 2;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnAddToSelectedList);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(681, 469);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 32);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnCancelDialog);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(0, 38);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstFiles);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlPreview);
            this.splitContainer1.Size = new System.Drawing.Size(768, 425);
            this.splitContainer1.SplitterDistance = 548;
            this.splitContainer1.TabIndex = 0;
            // 
            // lstFiles
            // 
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.GridLines = true;
            this.lstFiles.Location = new System.Drawing.Point(0, 0);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(546, 423);
            this.lstFiles.TabIndex = 0;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            this.lstFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstFiles_ColumnClick);
            this.lstFiles.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            this.lstFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDblClickOnFiles);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 300;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Creation date";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 140;
            // 
            // pnlPreview
            // 
            this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPreview.Location = new System.Drawing.Point(0, 0);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(214, 423);
            this.pnlPreview.TabIndex = 0;
            // 
            // ResourceLoadDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 506);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ResourceLoadDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add/Create new resource";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem cbRasterImage;
        private System.Windows.Forms.ToolStripMenuItem cbVectorImage;
        private System.Windows.Forms.ToolStripMenuItem cbMusic;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cbRaw;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtNameFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ListView lstFiles;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.ToolStripButton cbOnlyUnused;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton6;
        private System.Windows.Forms.ToolStripMenuItem createNewSVGResouceToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem presentationToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem cbPresentation;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripMenuItem sVGResourceFromTemplateToolStripMenuItem;
    }
}