namespace GAppCreator
{
    partial class MarketStringEditorDialog
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
            this.comboType = new System.Windows.Forms.ComboBox();
            this.lbParam1 = new System.Windows.Forms.Label();
            this.txParam1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txResultedURL = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 146);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 58);
            this.panel1.TabIndex = 11;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(476, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 3;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(395, 16);
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
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "URL Type";
            // 
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(123, 10);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(428, 21);
            this.comboType.TabIndex = 13;
            this.comboType.SelectedIndexChanged += new System.EventHandler(this.OnChangeType);
            // 
            // lbParam1
            // 
            this.lbParam1.AutoSize = true;
            this.lbParam1.Location = new System.Drawing.Point(13, 48);
            this.lbParam1.Name = "lbParam1";
            this.lbParam1.Size = new System.Drawing.Size(56, 13);
            this.lbParam1.TabIndex = 14;
            this.lbParam1.Text = "URL Type";
            // 
            // txParam1
            // 
            this.txParam1.Location = new System.Drawing.Point(123, 45);
            this.txParam1.Name = "txParam1";
            this.txParam1.Size = new System.Drawing.Size(428, 20);
            this.txParam1.TabIndex = 15;
            this.txParam1.TextChanged += new System.EventHandler(this.OnChangeParam1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Resulted URL";
            // 
            // txResultedURL
            // 
            this.txResultedURL.Location = new System.Drawing.Point(123, 101);
            this.txResultedURL.Multiline = true;
            this.txResultedURL.Name = "txResultedURL";
            this.txResultedURL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txResultedURL.Size = new System.Drawing.Size(428, 39);
            this.txResultedURL.TabIndex = 17;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(16, 81);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(535, 4);
            this.panel2.TabIndex = 18;
            // 
            // MarketStringEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 204);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.txResultedURL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txParam1);
            this.Controls.Add(this.lbParam1);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MarketStringEditorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Market URL Editor";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label lbParam1;
        private System.Windows.Forms.TextBox txParam1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txResultedURL;
        private System.Windows.Forms.Panel panel2;
    }
}