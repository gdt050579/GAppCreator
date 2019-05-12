namespace GAppCreator
{
    partial class NewBuildDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.txName = new System.Windows.Forms.TextBox();
            this.comboOS = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboDup = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cbKeepResources = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // txName
            // 
            this.txName.Location = new System.Drawing.Point(103, 25);
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(467, 20);
            this.txName.TabIndex = 1;
            // 
            // comboOS
            // 
            this.comboOS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOS.FormattingEnabled = true;
            this.comboOS.Location = new System.Drawing.Point(103, 55);
            this.comboOS.Name = "comboOS";
            this.comboOS.Size = new System.Drawing.Size(467, 21);
            this.comboOS.TabIndex = 2;
            this.comboOS.SelectedIndexChanged += new System.EventHandler(this.OnChangePlatform);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Platform";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Duplicate From";
            // 
            // comboDup
            // 
            this.comboDup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDup.FormattingEnabled = true;
            this.comboDup.Location = new System.Drawing.Point(103, 88);
            this.comboDup.Name = "comboDup";
            this.comboDup.Size = new System.Drawing.Size(467, 21);
            this.comboDup.TabIndex = 5;
            this.comboDup.SelectedIndexChanged += new System.EventHandler(this.OnChangeDuplicate);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(414, 168);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 6;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnCreateNewBuild);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(495, 168);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 30);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // cbKeepResources
            // 
            this.cbKeepResources.AutoSize = true;
            this.cbKeepResources.Checked = true;
            this.cbKeepResources.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbKeepResources.Enabled = false;
            this.cbKeepResources.Location = new System.Drawing.Point(103, 116);
            this.cbKeepResources.Name = "cbKeepResources";
            this.cbKeepResources.Size = new System.Drawing.Size(383, 17);
            this.cbKeepResources.TabIndex = 8;
            this.cbKeepResources.Text = "Keep the same resources in the new build as the ones from the original build";
            this.cbKeepResources.UseVisualStyleBackColor = true;
            // 
            // NewBuildDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 210);
            this.Controls.Add(this.cbKeepResources);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboDup);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboOS);
            this.Controls.Add(this.txName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewBuildDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Build";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txName;
        private System.Windows.Forms.ComboBox comboOS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboDup;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox cbKeepResources;
    }
}