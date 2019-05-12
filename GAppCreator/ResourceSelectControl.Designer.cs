namespace GAppCreator
{
    partial class ResourceSelectControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceSelectControl));
            this.lstResource = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnFilterType = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txFilter = new System.Windows.Forms.ToolStripTextBox();
            this.cbLargeIcons = new System.Windows.Forms.ToolStripButton();
            this.btnBlack = new System.Windows.Forms.ToolStripButton();
            this.btnNone = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstResource
            // 
            this.lstResource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstResource.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstResource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstResource.FullRowSelect = true;
            this.lstResource.GridLines = true;
            this.lstResource.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstResource.HideSelection = false;
            this.lstResource.Location = new System.Drawing.Point(0, 38);
            this.lstResource.MultiSelect = false;
            this.lstResource.Name = "lstResource";
            this.lstResource.Size = new System.Drawing.Size(279, 268);
            this.lstResource.TabIndex = 3;
            this.lstResource.UseCompatibleStateImageBehavior = false;
            this.lstResource.View = System.Windows.Forms.View.Details;
            this.lstResource.SelectedIndexChanged += new System.EventHandler(this.OnSelectItem);
            this.lstResource.DoubleClick += new System.EventHandler(this.OnDblClickOnResource);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 196;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Properties";
            this.columnHeader2.Width = 310;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFilterType,
            this.toolStripLabel1,
            this.txFilter,
            this.cbLargeIcons,
            this.btnBlack,
            this.btnNone});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(279, 38);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnFilterType
            // 
            this.btnFilterType.Image = ((System.Drawing.Image)(resources.GetObject("btnFilterType.Image")));
            this.btnFilterType.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilterType.Name = "btnFilterType";
            this.btnFilterType.Size = new System.Drawing.Size(46, 35);
            this.btnFilterType.Text = "Type";
            this.btnFilterType.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
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
            this.txFilter.Size = new System.Drawing.Size(60, 38);
            this.txFilter.TextChanged += new System.EventHandler(this.OnTextFilterChanged);
            // 
            // cbLargeIcons
            // 
            this.cbLargeIcons.CheckOnClick = true;
            this.cbLargeIcons.Image = ((System.Drawing.Image)(resources.GetObject("cbLargeIcons.Image")));
            this.cbLargeIcons.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbLargeIcons.Name = "cbLargeIcons";
            this.cbLargeIcons.Size = new System.Drawing.Size(71, 35);
            this.cbLargeIcons.Text = "Large Icons";
            this.cbLargeIcons.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbLargeIcons.Click += new System.EventHandler(this.OnChangeViewMode);
            // 
            // btnBlack
            // 
            this.btnBlack.CheckOnClick = true;
            this.btnBlack.Image = ((System.Drawing.Image)(resources.GetObject("btnBlack.Image")));
            this.btnBlack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBlack.Name = "btnBlack";
            this.btnBlack.Size = new System.Drawing.Size(39, 35);
            this.btnBlack.Text = "Black";
            this.btnBlack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnBlack.Click += new System.EventHandler(this.OnSetBackgoundBlack);
            // 
            // btnNone
            // 
            this.btnNone.Image = ((System.Drawing.Image)(resources.GetObject("btnNone.Image")));
            this.btnNone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(40, 35);
            this.btnNone.Text = "None";
            this.btnNone.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNone.Click += new System.EventHandler(this.OnNone);
            // 
            // ResourceSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstResource);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ResourceSelectControl";
            this.Size = new System.Drawing.Size(279, 306);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstResource;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton btnFilterType;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txFilter;
        private System.Windows.Forms.ToolStripButton cbLargeIcons;
        private System.Windows.Forms.ToolStripButton btnBlack;
        private System.Windows.Forms.ToolStripButton btnNone;
    }
}
