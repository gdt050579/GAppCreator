namespace GAppCreator
{
    partial class ErrorViewerDialog
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
            this.lstErrors = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtExtraInfo = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstErrors
            // 
            this.lstErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstErrors.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstErrors.FullRowSelect = true;
            this.lstErrors.GridLines = true;
            this.lstErrors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstErrors.Location = new System.Drawing.Point(0, 0);
            this.lstErrors.MultiSelect = false;
            this.lstErrors.Name = "lstErrors";
            this.lstErrors.Size = new System.Drawing.Size(605, 240);
            this.lstErrors.TabIndex = 0;
            this.lstErrors.UseCompatibleStateImageBehavior = false;
            this.lstErrors.View = System.Windows.Forms.View.Details;
            this.lstErrors.SelectedIndexChanged += new System.EventHandler(this.lstErrors_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Module";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Error";
            this.columnHeader2.Width = 1200;
            // 
            // txtExtraInfo
            // 
            this.txtExtraInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtExtraInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtExtraInfo.Location = new System.Drawing.Point(0, 240);
            this.txtExtraInfo.Multiline = true;
            this.txtExtraInfo.Name = "txtExtraInfo";
            this.txtExtraInfo.ReadOnly = true;
            this.txtExtraInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtExtraInfo.Size = new System.Drawing.Size(605, 161);
            this.txtExtraInfo.TabIndex = 1;
            this.txtExtraInfo.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(513, 407);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 28);
            this.button1.TabIndex = 2;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnCloseErrorDialog);
            // 
            // ErrorViewerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 438);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtExtraInfo);
            this.Controls.Add(this.lstErrors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ErrorViewerDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ErrorViewerDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstErrors;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox txtExtraInfo;
        private System.Windows.Forms.Button button1;
    }
}