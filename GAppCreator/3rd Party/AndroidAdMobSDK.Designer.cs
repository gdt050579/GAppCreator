namespace GAppCreator
{
    partial class AndroidAdMobSDK
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
            this.txAppID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbEnableTestMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txAppID
            // 
            this.txAppID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txAppID.Location = new System.Drawing.Point(178, 3);
            this.txAppID.Name = "txAppID";
            this.txAppID.Size = new System.Drawing.Size(849, 20);
            this.txAppID.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "app ID (ca-app-pub-xxxx~xxx)";
            // 
            // cbEnableTestMode
            // 
            this.cbEnableTestMode.AutoSize = true;
            this.cbEnableTestMode.Location = new System.Drawing.Point(6, 37);
            this.cbEnableTestMode.Name = "cbEnableTestMode";
            this.cbEnableTestMode.Size = new System.Drawing.Size(322, 17);
            this.cbEnableTestMode.TabIndex = 6;
            this.cbEnableTestMode.Text = "Enable test mode (app ID will be replaced with a test mode ID )";
            this.cbEnableTestMode.UseVisualStyleBackColor = true;
            // 
            // AndroidAdMobSDK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbEnableTestMode);
            this.Controls.Add(this.txAppID);
            this.Controls.Add(this.label1);
            this.Name = "AndroidAdMobSDK";
            this.Size = new System.Drawing.Size(1030, 85);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txAppID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbEnableTestMode;
    }
}
