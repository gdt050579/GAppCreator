namespace GAppCreator
{
    partial class PackageCreateDialog
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
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txArchiveName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txPassword = new System.Windows.Forms.TextBox();
            this.comboContent = new System.Windows.Forms.ComboBox();
            this.lstContentItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbGacProjectSettings = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboMethod = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 394);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(521, 58);
            this.panel1.TabIndex = 5;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(427, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 3;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(346, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 2;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnOK);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Archive Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(287, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Content";
            // 
            // txArchiveName
            // 
            this.txArchiveName.Location = new System.Drawing.Point(93, 19);
            this.txArchiveName.Name = "txArchiveName";
            this.txArchiveName.Size = new System.Drawing.Size(375, 20);
            this.txArchiveName.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(474, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 20);
            this.button1.TabIndex = 10;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnSelectArchiveName);
            // 
            // txPassword
            // 
            this.txPassword.Enabled = false;
            this.txPassword.Location = new System.Drawing.Point(346, 46);
            this.txPassword.Name = "txPassword";
            this.txPassword.Size = new System.Drawing.Size(156, 20);
            this.txPassword.TabIndex = 11;
            // 
            // comboContent
            // 
            this.comboContent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboContent.FormattingEnabled = true;
            this.comboContent.Items.AddRange(new object[] {
            "Only project data",
            "Project and Resources (no code)",
            "Backup (Project, Resources, Templates, Plugins and Code)",
            "Extende Backup (Project, Resources, Templates, Plugins, Code and Publisher data)",
            "Marketing (Strings, Binaries, Publisher data, Advertisments)",
            "Test mode (Binaries)",
            "Full",
            "Custom"});
            this.comboContent.Location = new System.Drawing.Point(93, 73);
            this.comboContent.Name = "comboContent";
            this.comboContent.Size = new System.Drawing.Size(409, 21);
            this.comboContent.TabIndex = 12;
            this.comboContent.SelectedIndexChanged += new System.EventHandler(this.OnChangeContentType);
            // 
            // lstContentItems
            // 
            this.lstContentItems.CheckBoxes = true;
            this.lstContentItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstContentItems.FullRowSelect = true;
            this.lstContentItems.GridLines = true;
            this.lstContentItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstContentItems.Location = new System.Drawing.Point(16, 140);
            this.lstContentItems.Name = "lstContentItems";
            this.lstContentItems.Size = new System.Drawing.Size(486, 242);
            this.lstContentItems.TabIndex = 13;
            this.lstContentItems.UseCompatibleStateImageBehavior = false;
            this.lstContentItems.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 600;
            // 
            // cbGacProjectSettings
            // 
            this.cbGacProjectSettings.AutoSize = true;
            this.cbGacProjectSettings.Location = new System.Drawing.Point(16, 107);
            this.cbGacProjectSettings.Name = "cbGacProjectSettings";
            this.cbGacProjectSettings.Size = new System.Drawing.Size(155, 17);
            this.cbGacProjectSettings.TabIndex = 14;
            this.cbGacProjectSettings.Text = "Enable IDE Project settings";
            this.cbGacProjectSettings.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(472, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 20);
            this.button2.TabIndex = 15;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnSelectArchiveName);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Method";
            // 
            // comboMethod
            // 
            this.comboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMethod.FormattingEnabled = true;
            this.comboMethod.Items.AddRange(new object[] {
            "Simple (ZIP file)",
            "Protected (7Zip with password)"});
            this.comboMethod.Location = new System.Drawing.Point(93, 46);
            this.comboMethod.Name = "comboMethod";
            this.comboMethod.Size = new System.Drawing.Size(165, 21);
            this.comboMethod.TabIndex = 17;
            this.comboMethod.SelectedIndexChanged += new System.EventHandler(this.OnChangeCompressMethodType);
            // 
            // PackageCreateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 452);
            this.ControlBox = false;
            this.Controls.Add(this.comboMethod);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cbGacProjectSettings);
            this.Controls.Add(this.lstContentItems);
            this.Controls.Add(this.comboContent);
            this.Controls.Add(this.txPassword);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txArchiveName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackageCreateDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Package Creator";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txArchiveName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txPassword;
        private System.Windows.Forms.ComboBox comboContent;
        private System.Windows.Forms.ListView lstContentItems;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.CheckBox cbGacProjectSettings;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboMethod;
    }
}