namespace GAppCreator
{
    partial class AndroidChartBoostSDK
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
            this.label1 = new System.Windows.Forms.Label();
            this.txAppID = new System.Windows.Forms.TextBox();
            this.txAppSignature = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "App ID";
            // 
            // txAppID
            // 
            this.txAppID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txAppID.Location = new System.Drawing.Point(135, 10);
            this.txAppID.Name = "txAppID";
            this.txAppID.Size = new System.Drawing.Size(892, 20);
            this.txAppID.TabIndex = 1;
            // 
            // txAppSignature
            // 
            this.txAppSignature.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txAppSignature.Location = new System.Drawing.Point(135, 36);
            this.txAppSignature.Name = "txAppSignature";
            this.txAppSignature.Size = new System.Drawing.Size(892, 20);
            this.txAppSignature.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "App Signature";
            // 
            // AndroidChartBoostSDK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txAppSignature);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txAppID);
            this.Controls.Add(this.label1);
            this.Name = "AndroidChartBoostSDK";
            this.Size = new System.Drawing.Size(1030, 85);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txAppID;
        private System.Windows.Forms.TextBox txAppSignature;
        private System.Windows.Forms.Label label2;
    }
}
