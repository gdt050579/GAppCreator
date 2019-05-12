namespace GAppCreator
{
    partial class InputBox
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
            this.lbInfo = new System.Windows.Forms.Label();
            this.txValue = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bntOk = new System.Windows.Forms.Button();
            this.comboInfo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lbInfo
            // 
            this.lbInfo.Location = new System.Drawing.Point(13, 9);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(404, 33);
            this.lbInfo.TabIndex = 0;
            this.lbInfo.Text = "label1";
            this.lbInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txValue
            // 
            this.txValue.Location = new System.Drawing.Point(13, 39);
            this.txValue.Name = "txValue";
            this.txValue.Size = new System.Drawing.Size(404, 20);
            this.txValue.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(342, 74);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // bntOk
            // 
            this.bntOk.Location = new System.Drawing.Point(261, 74);
            this.bntOk.Name = "bntOk";
            this.bntOk.Size = new System.Drawing.Size(75, 23);
            this.bntOk.TabIndex = 3;
            this.bntOk.Text = "Ok";
            this.bntOk.UseVisualStyleBackColor = true;
            this.bntOk.Click += new System.EventHandler(this.bntOk_Click);
            // 
            // comboInfo
            // 
            this.comboInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInfo.FormattingEnabled = true;
            this.comboInfo.Location = new System.Drawing.Point(12, 65);
            this.comboInfo.Name = "comboInfo";
            this.comboInfo.Size = new System.Drawing.Size(405, 21);
            this.comboInfo.TabIndex = 4;
            // 
            // InputBox
            // 
            this.AcceptButton = this.bntOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(429, 109);
            this.ControlBox = false;
            this.Controls.Add(this.comboInfo);
            this.Controls.Add(this.bntOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txValue);
            this.Controls.Add(this.lbInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InputBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.TextBox txValue;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button bntOk;
        private System.Windows.Forms.ComboBox comboInfo;
    }
}