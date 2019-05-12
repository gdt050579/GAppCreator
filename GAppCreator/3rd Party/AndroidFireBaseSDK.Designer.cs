namespace GAppCreator
{
    partial class AndroidFireBaseSDK
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
            this.txGoogleServicesJSON = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbCrashAnalytics = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txGoogleServicesJSON
            // 
            this.txGoogleServicesJSON.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txGoogleServicesJSON.Location = new System.Drawing.Point(178, 8);
            this.txGoogleServicesJSON.Multiline = true;
            this.txGoogleServicesJSON.Name = "txGoogleServicesJSON";
            this.txGoogleServicesJSON.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txGoogleServicesJSON.Size = new System.Drawing.Size(849, 278);
            this.txGoogleServicesJSON.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "google-services.json file";
            // 
            // cbCrashAnalytics
            // 
            this.cbCrashAnalytics.AutoSize = true;
            this.cbCrashAnalytics.Location = new System.Drawing.Point(6, 296);
            this.cbCrashAnalytics.Name = "cbCrashAnalytics";
            this.cbCrashAnalytics.Size = new System.Drawing.Size(176, 17);
            this.cbCrashAnalytics.TabIndex = 8;
            this.cbCrashAnalytics.Text = "Enable Crash Analytics services";
            this.cbCrashAnalytics.UseVisualStyleBackColor = true;
            // 
            // AndroidFireBaseSDK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbCrashAnalytics);
            this.Controls.Add(this.txGoogleServicesJSON);
            this.Controls.Add(this.label1);
            this.Name = "AndroidFireBaseSDK";
            this.Size = new System.Drawing.Size(1030, 329);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txGoogleServicesJSON;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbCrashAnalytics;
    }
}
