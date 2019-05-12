namespace GAppCreator
{
    partial class ResourcePreviewDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.nmArr2 = new System.Windows.Forms.NumericUpDown();
            this.lbX = new System.Windows.Forms.Label();
            this.nmArr1 = new System.Windows.Forms.NumericUpDown();
            this.lbArr = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.lstStringList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmArr2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmArr1)).BeginInit();
            this.pnlPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.nmArr2);
            this.panel1.Controls.Add(this.lbX);
            this.panel1.Controls.Add(this.nmArr1);
            this.panel1.Controls.Add(this.lbArr);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 403);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(684, 58);
            this.panel1.TabIndex = 3;
            this.panel1.TabStop = true;
            // 
            // nmArr2
            // 
            this.nmArr2.Location = new System.Drawing.Point(166, 23);
            this.nmArr2.Name = "nmArr2";
            this.nmArr2.Size = new System.Drawing.Size(84, 20);
            this.nmArr2.TabIndex = 6;
            this.nmArr2.Visible = false;
            this.nmArr2.ValueChanged += new System.EventHandler(this.OnChangeResource);
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(148, 25);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(12, 13);
            this.lbX.TabIndex = 5;
            this.lbX.Text = "x";
            this.lbX.Visible = false;
            // 
            // nmArr1
            // 
            this.nmArr1.Location = new System.Drawing.Point(58, 23);
            this.nmArr1.Name = "nmArr1";
            this.nmArr1.Size = new System.Drawing.Size(84, 20);
            this.nmArr1.TabIndex = 4;
            this.nmArr1.Visible = false;
            this.nmArr1.ValueChanged += new System.EventHandler(this.OnChangeResource);
            // 
            // lbArr
            // 
            this.lbArr.AutoSize = true;
            this.lbArr.Location = new System.Drawing.Point(12, 25);
            this.lbArr.Name = "lbArr";
            this.lbArr.Size = new System.Drawing.Size(31, 13);
            this.lbArr.TabIndex = 2;
            this.lbArr.Text = "Array";
            this.lbArr.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(590, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlPreview
            // 
            this.pnlPreview.Controls.Add(this.lstStringList);
            this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPreview.Location = new System.Drawing.Point(0, 0);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(684, 403);
            this.pnlPreview.TabIndex = 4;
            // 
            // lstStringList
            // 
            this.lstStringList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstStringList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStringList.FullRowSelect = true;
            this.lstStringList.GridLines = true;
            this.lstStringList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstStringList.Location = new System.Drawing.Point(0, 0);
            this.lstStringList.Name = "lstStringList";
            this.lstStringList.Size = new System.Drawing.Size(684, 403);
            this.lstStringList.TabIndex = 0;
            this.lstStringList.UseCompatibleStateImageBehavior = false;
            this.lstStringList.View = System.Windows.Forms.View.Details;
            this.lstStringList.Visible = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 550;
            // 
            // ResourcePreviewDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.ControlBox = false;
            this.Controls.Add(this.pnlPreview);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ResourcePreviewDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preview";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmArr2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmArr1)).EndInit();
            this.pnlPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.NumericUpDown nmArr2;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.NumericUpDown nmArr1;
        private System.Windows.Forms.Label lbArr;
        private System.Windows.Forms.ListView lstStringList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}