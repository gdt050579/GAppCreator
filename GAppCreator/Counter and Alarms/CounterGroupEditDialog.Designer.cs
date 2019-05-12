namespace GAppCreator
{
    partial class CounterGroupEditDialog
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboMethod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txName = new System.Windows.Forms.TextBox();
            this.lbMinimalTimeLimit = new System.Windows.Forms.Label();
            this.nmMinimTimeLimit = new System.Windows.Forms.NumericUpDown();
            this.lbMinimalTimeLimitSeconds = new System.Windows.Forms.Label();
            this.cbEnableTimer = new System.Windows.Forms.CheckBox();
            this.lbAfterUpdateBeehavior = new System.Windows.Forms.Label();
            this.comboAfterUpdateBehavior = new System.Windows.Forms.ComboBox();
            this.comboStartTimerMethod = new System.Windows.Forms.ComboBox();
            this.lbStartTimerMethod = new System.Windows.Forms.Label();
            this.comboScenes = new System.Windows.Forms.ComboBox();
            this.cbUseEnableConditionProperty = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmMinimTimeLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(490, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(409, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 233);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 58);
            this.panel1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Name";
            // 
            // comboMethod
            // 
            this.comboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMethod.FormattingEnabled = true;
            this.comboMethod.Location = new System.Drawing.Point(99, 55);
            this.comboMethod.Name = "comboMethod";
            this.comboMethod.Size = new System.Drawing.Size(466, 21);
            this.comboMethod.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Update method";
            // 
            // txName
            // 
            this.txName.Location = new System.Drawing.Point(99, 20);
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(466, 20);
            this.txName.TabIndex = 11;
            // 
            // lbMinimalTimeLimit
            // 
            this.lbMinimalTimeLimit.AutoSize = true;
            this.lbMinimalTimeLimit.Location = new System.Drawing.Point(35, 141);
            this.lbMinimalTimeLimit.Name = "lbMinimalTimeLimit";
            this.lbMinimalTimeLimit.Size = new System.Drawing.Size(84, 13);
            this.lbMinimalTimeLimit.TabIndex = 12;
            this.lbMinimalTimeLimit.Text = "Minimal time limit";
            this.lbMinimalTimeLimit.Click += new System.EventHandler(this.label3_Click);
            // 
            // nmMinimTimeLimit
            // 
            this.nmMinimTimeLimit.Location = new System.Drawing.Point(145, 139);
            this.nmMinimTimeLimit.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmMinimTimeLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmMinimTimeLimit.Name = "nmMinimTimeLimit";
            this.nmMinimTimeLimit.Size = new System.Drawing.Size(367, 20);
            this.nmMinimTimeLimit.TabIndex = 13;
            this.nmMinimTimeLimit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbMinimalTimeLimitSeconds
            // 
            this.lbMinimalTimeLimitSeconds.AutoSize = true;
            this.lbMinimalTimeLimitSeconds.Location = new System.Drawing.Point(518, 141);
            this.lbMinimalTimeLimitSeconds.Name = "lbMinimalTimeLimitSeconds";
            this.lbMinimalTimeLimitSeconds.Size = new System.Drawing.Size(47, 13);
            this.lbMinimalTimeLimitSeconds.TabIndex = 14;
            this.lbMinimalTimeLimitSeconds.Text = "seconds";
            this.lbMinimalTimeLimitSeconds.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cbEnableTimer
            // 
            this.cbEnableTimer.AutoSize = true;
            this.cbEnableTimer.Location = new System.Drawing.Point(16, 116);
            this.cbEnableTimer.Name = "cbEnableTimer";
            this.cbEnableTimer.Size = new System.Drawing.Size(259, 17);
            this.cbEnableTimer.TabIndex = 15;
            this.cbEnableTimer.Text = "Enable minimal time limit before trigering a counter";
            this.cbEnableTimer.UseVisualStyleBackColor = true;
            this.cbEnableTimer.CheckedChanged += new System.EventHandler(this.cbEnableTimer_CheckedChanged);
            // 
            // lbAfterUpdateBeehavior
            // 
            this.lbAfterUpdateBeehavior.AutoSize = true;
            this.lbAfterUpdateBeehavior.Location = new System.Drawing.Point(35, 167);
            this.lbAfterUpdateBeehavior.Name = "lbAfterUpdateBeehavior";
            this.lbAfterUpdateBeehavior.Size = new System.Drawing.Size(109, 13);
            this.lbAfterUpdateBeehavior.TabIndex = 17;
            this.lbAfterUpdateBeehavior.Text = "After update behavior";
            // 
            // comboAfterUpdateBehavior
            // 
            this.comboAfterUpdateBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAfterUpdateBehavior.FormattingEnabled = true;
            this.comboAfterUpdateBehavior.Location = new System.Drawing.Point(145, 164);
            this.comboAfterUpdateBehavior.Name = "comboAfterUpdateBehavior";
            this.comboAfterUpdateBehavior.Size = new System.Drawing.Size(420, 21);
            this.comboAfterUpdateBehavior.TabIndex = 18;
            // 
            // comboStartTimerMethod
            // 
            this.comboStartTimerMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStartTimerMethod.FormattingEnabled = true;
            this.comboStartTimerMethod.Items.AddRange(new object[] {
            "Programatically (manually)",
            "When application start",
            "Before every scene",
            "On enter on a specific scene"});
            this.comboStartTimerMethod.Location = new System.Drawing.Point(145, 195);
            this.comboStartTimerMethod.Name = "comboStartTimerMethod";
            this.comboStartTimerMethod.Size = new System.Drawing.Size(284, 21);
            this.comboStartTimerMethod.TabIndex = 20;
            this.comboStartTimerMethod.SelectedIndexChanged += new System.EventHandler(this.comboStartTimerMethod_SelectedIndexChanged);
            // 
            // lbStartTimerMethod
            // 
            this.lbStartTimerMethod.AutoSize = true;
            this.lbStartTimerMethod.Location = new System.Drawing.Point(35, 198);
            this.lbStartTimerMethod.Name = "lbStartTimerMethod";
            this.lbStartTimerMethod.Size = new System.Drawing.Size(54, 13);
            this.lbStartTimerMethod.TabIndex = 19;
            this.lbStartTimerMethod.Text = "Start timer";
            // 
            // comboScenes
            // 
            this.comboScenes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboScenes.FormattingEnabled = true;
            this.comboScenes.Location = new System.Drawing.Point(435, 195);
            this.comboScenes.Name = "comboScenes";
            this.comboScenes.Size = new System.Drawing.Size(130, 21);
            this.comboScenes.TabIndex = 21;
            // 
            // cbUseEnableConditionProperty
            // 
            this.cbUseEnableConditionProperty.AutoSize = true;
            this.cbUseEnableConditionProperty.Location = new System.Drawing.Point(16, 93);
            this.cbUseEnableConditionProperty.Name = "cbUseEnableConditionProperty";
            this.cbUseEnableConditionProperty.Size = new System.Drawing.Size(457, 17);
            this.cbUseEnableConditionProperty.TabIndex = 22;
            this.cbUseEnableConditionProperty.Text = "Automatically enable conters based on their \'Enable Condition\' property before ev" +
    "ery update";
            this.cbUseEnableConditionProperty.UseVisualStyleBackColor = true;
            // 
            // CounterGroupEditDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 291);
            this.Controls.Add(this.cbUseEnableConditionProperty);
            this.Controls.Add(this.comboScenes);
            this.Controls.Add(this.comboStartTimerMethod);
            this.Controls.Add(this.lbStartTimerMethod);
            this.Controls.Add(this.comboAfterUpdateBehavior);
            this.Controls.Add(this.lbAfterUpdateBeehavior);
            this.Controls.Add(this.cbEnableTimer);
            this.Controls.Add(this.lbMinimalTimeLimitSeconds);
            this.Controls.Add(this.nmMinimTimeLimit);
            this.Controls.Add(this.lbMinimalTimeLimit);
            this.Controls.Add(this.txName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboMethod);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CounterGroupEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Counter Group";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nmMinimTimeLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboMethod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txName;
        private System.Windows.Forms.Label lbMinimalTimeLimit;
        private System.Windows.Forms.NumericUpDown nmMinimTimeLimit;
        private System.Windows.Forms.Label lbMinimalTimeLimitSeconds;
        private System.Windows.Forms.CheckBox cbEnableTimer;
        private System.Windows.Forms.Label lbAfterUpdateBeehavior;
        private System.Windows.Forms.ComboBox comboAfterUpdateBehavior;
        private System.Windows.Forms.ComboBox comboStartTimerMethod;
        private System.Windows.Forms.Label lbStartTimerMethod;
        private System.Windows.Forms.ComboBox comboScenes;
        private System.Windows.Forms.CheckBox cbUseEnableConditionProperty;
    }
}