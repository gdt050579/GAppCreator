namespace GAppCreator
{
    partial class ConstantEditDialog
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
            this.comboTypeMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSetDefaultvalue = new System.Windows.Forms.Button();
            this.txDefaultValue = new System.Windows.Forms.TextBox();
            this.lbValue = new System.Windows.Forms.Label();
            this.txDescription = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.txName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboDefaultValue = new System.Windows.Forms.ComboBox();
            this.cbList = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboTypeMode
            // 
            this.comboTypeMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboTypeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTypeMode.FormattingEnabled = true;
            this.comboTypeMode.Location = new System.Drawing.Point(364, 95);
            this.comboTypeMode.Name = "comboTypeMode";
            this.comboTypeMode.Size = new System.Drawing.Size(119, 21);
            this.comboTypeMode.TabIndex = 32;
            this.comboTypeMode.SelectedIndexChanged += new System.EventHandler(this.OnTypeModeChanges);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(324, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Mode";
            // 
            // btnSetDefaultvalue
            // 
            this.btnSetDefaultvalue.Location = new System.Drawing.Point(455, 124);
            this.btnSetDefaultvalue.Name = "btnSetDefaultvalue";
            this.btnSetDefaultvalue.Size = new System.Drawing.Size(28, 20);
            this.btnSetDefaultvalue.TabIndex = 30;
            this.btnSetDefaultvalue.Text = "...";
            this.btnSetDefaultvalue.UseVisualStyleBackColor = true;
            this.btnSetDefaultvalue.Click += new System.EventHandler(this.OnSetDefaultValue);
            // 
            // txDefaultValue
            // 
            this.txDefaultValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txDefaultValue.Location = new System.Drawing.Point(105, 124);
            this.txDefaultValue.Name = "txDefaultValue";
            this.txDefaultValue.Size = new System.Drawing.Size(345, 20);
            this.txDefaultValue.TabIndex = 29;
            // 
            // lbValue
            // 
            this.lbValue.AutoSize = true;
            this.lbValue.Location = new System.Drawing.Point(13, 127);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(34, 13);
            this.lbValue.TabIndex = 28;
            this.lbValue.Text = "Value";
            // 
            // txDescription
            // 
            this.txDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txDescription.Location = new System.Drawing.Point(105, 36);
            this.txDescription.Multiline = true;
            this.txDescription.Name = "txDescription";
            this.txDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txDescription.Size = new System.Drawing.Size(378, 53);
            this.txDescription.TabIndex = 23;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(408, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(327, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Description";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 181);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(502, 58);
            this.panel1.TabIndex = 26;
            // 
            // comboType
            // 
            this.comboType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(105, 95);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(199, 21);
            this.comboType.TabIndex = 25;
            this.comboType.SelectedIndexChanged += new System.EventHandler(this.OnTypeChanges);
            // 
            // txName
            // 
            this.txName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txName.Location = new System.Drawing.Point(105, 10);
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(378, 20);
            this.txName.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Name";
            // 
            // comboDefaultValue
            // 
            this.comboDefaultValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDefaultValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDefaultValue.FormattingEnabled = true;
            this.comboDefaultValue.Location = new System.Drawing.Point(105, 124);
            this.comboDefaultValue.Name = "comboDefaultValue";
            this.comboDefaultValue.Size = new System.Drawing.Size(378, 21);
            this.comboDefaultValue.TabIndex = 33;
            // 
            // cbList
            // 
            this.cbList.AutoSize = true;
            this.cbList.Location = new System.Drawing.Point(105, 152);
            this.cbList.Name = "cbList";
            this.cbList.Size = new System.Drawing.Size(180, 17);
            this.cbList.TabIndex = 34;
            this.cbList.Text = "Array of objects (vector or matrix)";
            this.cbList.UseVisualStyleBackColor = true;
            // 
            // ConstantEditDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(502, 239);
            this.Controls.Add(this.cbList);
            this.Controls.Add(this.comboTypeMode);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSetDefaultvalue);
            this.Controls.Add(this.txDefaultValue);
            this.Controls.Add(this.lbValue);
            this.Controls.Add(this.txDescription);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.txName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboDefaultValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConstantEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Constant Editor";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboTypeMode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSetDefaultvalue;
        private System.Windows.Forms.TextBox txDefaultValue;
        private System.Windows.Forms.Label lbValue;
        private System.Windows.Forms.TextBox txDescription;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.TextBox txName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboDefaultValue;
        private System.Windows.Forms.CheckBox cbList;

    }
}