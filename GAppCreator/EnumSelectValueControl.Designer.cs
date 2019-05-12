namespace GAppCreator
{
    partial class EnumSelectValueControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnumSelectValueControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbEnumName = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lstValues = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbBitSet = new System.Windows.Forms.ToolStripLabel();
            this.lbSingle = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbSingle,
            this.lbBitSet,
            this.toolStripSeparator1,
            this.lbEnumName,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(301, 38);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // lbEnumName
            // 
            this.lbEnumName.AutoSize = false;
            this.lbEnumName.Name = "lbEnumName";
            this.lbEnumName.Size = new System.Drawing.Size(120, 35);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // lstValues
            // 
            this.lstValues.CheckBoxes = true;
            this.lstValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstValues.FullRowSelect = true;
            this.lstValues.GridLines = true;
            this.lstValues.Location = new System.Drawing.Point(0, 38);
            this.lstValues.Name = "lstValues";
            this.lstValues.Size = new System.Drawing.Size(301, 235);
            this.lstValues.TabIndex = 4;
            this.lstValues.UseCompatibleStateImageBehavior = false;
            this.lstValues.View = System.Windows.Forms.View.Details;
            this.lstValues.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OnCheckItem);
            this.lstValues.SelectedIndexChanged += new System.EventHandler(this.OnChangeItem);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Descrription";
            this.columnHeader3.Width = 400;
            // 
            // lbBitSet
            // 
            this.lbBitSet.AutoSize = false;
            this.lbBitSet.Image = ((System.Drawing.Image)(resources.GetObject("lbBitSet.Image")));
            this.lbBitSet.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.lbBitSet.Name = "lbBitSet";
            this.lbBitSet.Size = new System.Drawing.Size(50, 35);
            this.lbBitSet.Text = "BitSet";
            this.lbBitSet.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.lbBitSet.Visible = false;
            // 
            // lbSingle
            // 
            this.lbSingle.AutoSize = false;
            this.lbSingle.Image = ((System.Drawing.Image)(resources.GetObject("lbSingle.Image")));
            this.lbSingle.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.lbSingle.Name = "lbSingle";
            this.lbSingle.Size = new System.Drawing.Size(50, 35);
            this.lbSingle.Text = "Single";
            this.lbSingle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.lbSingle.Visible = false;
            // 
            // EnumSelectValueControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstValues);
            this.Controls.Add(this.toolStrip1);
            this.Name = "EnumSelectValueControl";
            this.Size = new System.Drawing.Size(301, 273);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ListView lstValues;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripLabel lbBitSet;
        private System.Windows.Forms.ToolStripLabel lbEnumName;
        private System.Windows.Forms.ToolStripLabel lbSingle;
    }
}
