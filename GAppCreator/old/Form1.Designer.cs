namespace ResourceMaker
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.resProperties = new System.Windows.Forms.PropertyGrid();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.resList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.resImagesLarge = new System.Windows.Forms.ImageList(this.components);
            this.resImagesSmall = new System.Windows.Forms.ImageList(this.components);
            this.panelResourcePreview = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.allImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlyImagesThatWereNotLoadedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txResourceFilter = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbListViewMode = new System.Windows.Forms.ToolStripMenuItem();
            this.cbTilesViewMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.rasterImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vectorImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.musicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.textureAtlasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.glyphFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowImages = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowTextureAtlases = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowGlyphFonts = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(972, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem3,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.OnSaveProject);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(100, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 474);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(972, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = false;
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(200, 17);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(972, 450);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.toolStrip2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(964, 424);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Controls.Add(this.toolStrip1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(964, 424);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Resources";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 41);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.resProperties);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(958, 380);
            this.splitContainer1.SplitterDistance = 278;
            this.splitContainer1.TabIndex = 1;
            // 
            // resProperties
            // 
            this.resProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resProperties.Location = new System.Drawing.Point(0, 0);
            this.resProperties.Name = "resProperties";
            this.resProperties.Size = new System.Drawing.Size(276, 378);
            this.resProperties.TabIndex = 0;
            this.resProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.OnChangeResourceProperties);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.resList);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelResourcePreview);
            this.splitContainer2.Size = new System.Drawing.Size(676, 380);
            this.splitContainer2.SplitterDistance = 508;
            this.splitContainer2.TabIndex = 0;
            // 
            // resList
            // 
            this.resList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.resList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resList.FullRowSelect = true;
            this.resList.GridLines = true;
            this.resList.LargeImageList = this.resImagesLarge;
            this.resList.Location = new System.Drawing.Point(0, 0);
            this.resList.Name = "resList";
            this.resList.Size = new System.Drawing.Size(506, 378);
            this.resList.SmallImageList = this.resImagesSmall;
            this.resList.TabIndex = 0;
            this.resList.UseCompatibleStateImageBehavior = false;
            this.resList.View = System.Windows.Forms.View.Details;
            this.resList.SelectedIndexChanged += new System.EventHandler(this.OnResourceListSelectionChange);
            this.resList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnEditResource);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Properties";
            this.columnHeader3.Width = 400;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Source";
            this.columnHeader4.Width = 200;
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
            // panelResourcePreview
            // 
            this.panelResourcePreview.BackColor = System.Drawing.Color.Silver;
            this.panelResourcePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelResourcePreview.Location = new System.Drawing.Point(0, 0);
            this.panelResourcePreview.Name = "panelResourcePreview";
            this.panelResourcePreview.Size = new System.Drawing.Size(162, 378);
            this.panelResourcePreview.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.txResourceFilter,
            this.toolStripSeparator2,
            this.toolStripSplitButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(958, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(44, 35);
            this.toolStripButton1.Text = "Delete";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.OnDeleteResources);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allImagesToolStripMenuItem,
            this.onlyImagesThatWereNotLoadedToolStripMenuItem,
            this.toolStripMenuItem2,
            this.reloadIconsToolStripMenuItem});
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(56, 35);
            this.toolStripButton2.Text = "Reload";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // allImagesToolStripMenuItem
            // 
            this.allImagesToolStripMenuItem.Name = "allImagesToolStripMenuItem";
            this.allImagesToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.allImagesToolStripMenuItem.Text = "All images";
            // 
            // onlyImagesThatWereNotLoadedToolStripMenuItem
            // 
            this.onlyImagesThatWereNotLoadedToolStripMenuItem.Name = "onlyImagesThatWereNotLoadedToolStripMenuItem";
            this.onlyImagesThatWereNotLoadedToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.onlyImagesThatWereNotLoadedToolStripMenuItem.Text = "Only images that were not loaded";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(249, 6);
            // 
            // reloadIconsToolStripMenuItem
            // 
            this.reloadIconsToolStripMenuItem.Name = "reloadIconsToolStripMenuItem";
            this.reloadIconsToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.reloadIconsToolStripMenuItem.Text = "Reload icons";
            this.reloadIconsToolStripMenuItem.Click += new System.EventHandler(this.OnReloadResourcesIcons);
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
            // txResourceFilter
            // 
            this.txResourceFilter.AutoSize = false;
            this.txResourceFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txResourceFilter.Name = "txResourceFilter";
            this.txResourceFilter.Size = new System.Drawing.Size(100, 23);
            this.txResourceFilter.TextChanged += new System.EventHandler(this.OnResourceTextFilterChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbListViewMode,
            this.cbTilesViewMode,
            this.toolStripMenuItem1,
            this.cbShowImages,
            this.cbShowTextureAtlases,
            this.cbShowGlyphFonts});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(45, 35);
            this.toolStripSplitButton1.Text = "View";
            this.toolStripSplitButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbListViewMode
            // 
            this.cbListViewMode.Checked = true;
            this.cbListViewMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbListViewMode.Name = "cbListViewMode";
            this.cbListViewMode.Size = new System.Drawing.Size(153, 22);
            this.cbListViewMode.Text = "List";
            this.cbListViewMode.Click += new System.EventHandler(this.OnViewListMode);
            // 
            // cbTilesViewMode
            // 
            this.cbTilesViewMode.Name = "cbTilesViewMode";
            this.cbTilesViewMode.Size = new System.Drawing.Size(153, 22);
            this.cbTilesViewMode.Text = "Tiles";
            this.cbTilesViewMode.Click += new System.EventHandler(this.OnViewTilesMode);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(150, 6);
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(964, 424);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Strings";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rasterImageToolStripMenuItem,
            this.vectorImageToolStripMenuItem,
            this.musicToolStripMenuItem,
            this.rawToolStripMenuItem,
            this.toolStripMenuItem4,
            this.textureAtlasToolStripMenuItem,
            this.glyphFontToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(44, 35);
            this.toolStripDropDownButton1.Text = "New";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // rasterImageToolStripMenuItem
            // 
            this.rasterImageToolStripMenuItem.Name = "rasterImageToolStripMenuItem";
            this.rasterImageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rasterImageToolStripMenuItem.Text = "Raster Image ";
            this.rasterImageToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewResource);
            // 
            // vectorImageToolStripMenuItem
            // 
            this.vectorImageToolStripMenuItem.Name = "vectorImageToolStripMenuItem";
            this.vectorImageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.vectorImageToolStripMenuItem.Text = "Vector Image";
            this.vectorImageToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewResource);
            // 
            // musicToolStripMenuItem
            // 
            this.musicToolStripMenuItem.Name = "musicToolStripMenuItem";
            this.musicToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.musicToolStripMenuItem.Text = "Music";
            this.musicToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewResource);
            // 
            // rawToolStripMenuItem
            // 
            this.rawToolStripMenuItem.Name = "rawToolStripMenuItem";
            this.rawToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rawToolStripMenuItem.Text = "Raw";
            this.rawToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewResource);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(149, 6);
            // 
            // textureAtlasToolStripMenuItem
            // 
            this.textureAtlasToolStripMenuItem.Name = "textureAtlasToolStripMenuItem";
            this.textureAtlasToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.textureAtlasToolStripMenuItem.Text = "Texture Atlas";
            this.textureAtlasToolStripMenuItem.Click += new System.EventHandler(this.OnAddNewTextureAtlas);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(958, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildToolStripMenuItem});
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(29, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // buildToolStripMenuItem
            // 
            this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            this.buildToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.buildToolStripMenuItem.Text = "Create developer files";
            this.buildToolStripMenuItem.Click += new System.EventHandler(this.OnCreateDeveloperFiles);
            // 
            // glyphFontToolStripMenuItem
            // 
            this.glyphFontToolStripMenuItem.Name = "glyphFontToolStripMenuItem";
            this.glyphFontToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.glyphFontToolStripMenuItem.Text = "Glyph Font";
            this.glyphFontToolStripMenuItem.Click += new System.EventHandler(this.OnCreateNewGlyphFont);
            // 
            // cbShowImages
            // 
            this.cbShowImages.Checked = true;
            this.cbShowImages.CheckOnClick = true;
            this.cbShowImages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowImages.Name = "cbShowImages";
            this.cbShowImages.Size = new System.Drawing.Size(153, 22);
            this.cbShowImages.Text = "Images";
            this.cbShowImages.Click += new System.EventHandler(this.OnChangeViewResourceTypes);
            // 
            // cbShowTextureAtlases
            // 
            this.cbShowTextureAtlases.Checked = true;
            this.cbShowTextureAtlases.CheckOnClick = true;
            this.cbShowTextureAtlases.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowTextureAtlases.Name = "cbShowTextureAtlases";
            this.cbShowTextureAtlases.Size = new System.Drawing.Size(153, 22);
            this.cbShowTextureAtlases.Text = "Texture Atlases";
            this.cbShowTextureAtlases.Click += new System.EventHandler(this.OnChangeViewResourceTypes);
            // 
            // cbShowGlyphFonts
            // 
            this.cbShowGlyphFonts.Checked = true;
            this.cbShowGlyphFonts.CheckOnClick = true;
            this.cbShowGlyphFonts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowGlyphFonts.Name = "cbShowGlyphFonts";
            this.cbShowGlyphFonts.Size = new System.Drawing.Size(153, 22);
            this.cbShowGlyphFonts.Text = "Glyph Fonts";
            this.cbShowGlyphFonts.Click += new System.EventHandler(this.OnChangeViewResourceTypes);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 496);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbInfo;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid resProperties;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView resList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txResourceFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ImageList resImagesLarge;
        private System.Windows.Forms.ImageList resImagesSmall;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton2;
        private System.Windows.Forms.ToolStripMenuItem allImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onlyImagesThatWereNotLoadedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem reloadIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem cbListViewMode;
        private System.Windows.Forms.ToolStripMenuItem cbTilesViewMode;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Panel panelResourcePreview;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem rasterImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vectorImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem musicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem textureAtlasToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem glyphFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cbShowImages;
        private System.Windows.Forms.ToolStripMenuItem cbShowTextureAtlases;
        private System.Windows.Forms.ToolStripMenuItem cbShowGlyphFonts;
    }
}

