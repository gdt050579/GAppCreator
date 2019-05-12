namespace GAppCreator
{
    partial class LanguageSelectDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LanguageSelectDialog));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.selectAllLanguagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllLanguagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.lstLanguages = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.revertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbInfo = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripSeparator1,
            this.lbInfo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(252, 38);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllLanguagesToolStripMenuItem,
            this.clearAllLanguagesToolStripMenuItem,
            this.revertSelectionToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(68, 35);
            this.toolStripDropDownButton1.Text = "Selection";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // selectAllLanguagesToolStripMenuItem
            // 
            this.selectAllLanguagesToolStripMenuItem.Name = "selectAllLanguagesToolStripMenuItem";
            this.selectAllLanguagesToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.selectAllLanguagesToolStripMenuItem.Text = "Select all languages";
            this.selectAllLanguagesToolStripMenuItem.Click += new System.EventHandler(this.OnSelectAll);
            // 
            // clearAllLanguagesToolStripMenuItem
            // 
            this.clearAllLanguagesToolStripMenuItem.Name = "clearAllLanguagesToolStripMenuItem";
            this.clearAllLanguagesToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.clearAllLanguagesToolStripMenuItem.Text = "Clear all languages";
            this.clearAllLanguagesToolStripMenuItem.Click += new System.EventHandler(this.OnClearAll);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 239);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(252, 58);
            this.panel1.TabIndex = 7;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(158, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 3;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(77, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 2;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnOK);
            // 
            // lstLanguages
            // 
            this.lstLanguages.CheckBoxes = true;
            this.lstLanguages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstLanguages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLanguages.FullRowSelect = true;
            this.lstLanguages.GridLines = true;
            this.lstLanguages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstLanguages.Location = new System.Drawing.Point(0, 38);
            this.lstLanguages.Name = "lstLanguages";
            this.lstLanguages.Size = new System.Drawing.Size(252, 201);
            this.lstLanguages.TabIndex = 8;
            this.lstLanguages.UseCompatibleStateImageBehavior = false;
            this.lstLanguages.View = System.Windows.Forms.View.Details;
            this.lstLanguages.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstLanguages_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 210;
            // 
            // revertSelectionToolStripMenuItem
            // 
            this.revertSelectionToolStripMenuItem.Name = "revertSelectionToolStripMenuItem";
            this.revertSelectionToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.revertSelectionToolStripMenuItem.Text = "Revert selection";
            this.revertSelectionToolStripMenuItem.Click += new System.EventHandler(this.OnRevertSelection);
            // 
            // lbInfo
            // 
            this.lbInfo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lbInfo.AutoSize = false;
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(120, 35);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // LanguageSelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 297);
            this.Controls.Add(this.lstLanguages);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LanguageSelectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Language Select";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem selectAllLanguagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllLanguagesToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListView lstLanguages;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripMenuItem revertSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lbInfo;
    }
}