namespace GAppCreator
{
    partial class ResolutionEditorDialog
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
            this.txCustom = new System.Windows.Forms.TextBox();
            this.rbCustom = new System.Windows.Forms.RadioButton();
            this.rbStandard = new System.Windows.Forms.RadioButton();
            this.comboStandard = new System.Windows.Forms.ComboBox();
            this.rbWindow = new System.Windows.Forms.RadioButton();
            this.comboWindow = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 131);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 58);
            this.panel1.TabIndex = 10;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(213, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 3;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(132, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 2;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnOK);
            // 
            // txCustom
            // 
            this.txCustom.Location = new System.Drawing.Point(147, 11);
            this.txCustom.Name = "txCustom";
            this.txCustom.Size = new System.Drawing.Size(141, 20);
            this.txCustom.TabIndex = 12;
            // 
            // rbCustom
            // 
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(12, 12);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(113, 17);
            this.rbCustom.TabIndex = 13;
            this.rbCustom.TabStop = true;
            this.rbCustom.Text = "Custom Resolution";
            this.rbCustom.UseVisualStyleBackColor = true;
            this.rbCustom.CheckedChanged += new System.EventHandler(this.OnSelectMode);
            // 
            // rbStandard
            // 
            this.rbStandard.AutoSize = true;
            this.rbStandard.Location = new System.Drawing.Point(12, 50);
            this.rbStandard.Name = "rbStandard";
            this.rbStandard.Size = new System.Drawing.Size(126, 17);
            this.rbStandard.TabIndex = 14;
            this.rbStandard.TabStop = true;
            this.rbStandard.Text = "Standard Resolutions";
            this.rbStandard.UseVisualStyleBackColor = true;
            this.rbStandard.CheckedChanged += new System.EventHandler(this.OnSelectMode);
            // 
            // comboStandard
            // 
            this.comboStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStandard.FormattingEnabled = true;
            this.comboStandard.Location = new System.Drawing.Point(147, 49);
            this.comboStandard.Name = "comboStandard";
            this.comboStandard.Size = new System.Drawing.Size(141, 21);
            this.comboStandard.TabIndex = 15;
            // 
            // rbWindow
            // 
            this.rbWindow.AutoSize = true;
            this.rbWindow.Location = new System.Drawing.Point(12, 88);
            this.rbWindow.Name = "rbWindow";
            this.rbWindow.Size = new System.Drawing.Size(103, 17);
            this.rbWindow.TabIndex = 16;
            this.rbWindow.TabStop = true;
            this.rbWindow.Text = "Window specific";
            this.rbWindow.UseVisualStyleBackColor = true;
            this.rbWindow.CheckedChanged += new System.EventHandler(this.OnSelectMode);
            // 
            // comboWindow
            // 
            this.comboWindow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboWindow.FormattingEnabled = true;
            this.comboWindow.Location = new System.Drawing.Point(147, 87);
            this.comboWindow.Name = "comboWindow";
            this.comboWindow.Size = new System.Drawing.Size(141, 21);
            this.comboWindow.TabIndex = 17;
            // 
            // ResolutionEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 189);
            this.ControlBox = false;
            this.Controls.Add(this.comboWindow);
            this.Controls.Add(this.rbWindow);
            this.Controls.Add(this.comboStandard);
            this.Controls.Add(this.rbStandard);
            this.Controls.Add(this.rbCustom);
            this.Controls.Add(this.txCustom);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ResolutionEditorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Resolution";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox txCustom;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.RadioButton rbStandard;
        private System.Windows.Forms.ComboBox comboStandard;
        private System.Windows.Forms.RadioButton rbWindow;
        private System.Windows.Forms.ComboBox comboWindow;
    }
}