namespace GAppCreator
{
    partial class TaskDialog
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
            this.label2 = new System.Windows.Forms.Label();
            this.txTask = new System.Windows.Forms.TextBox();
            this.txAddedOn = new System.Windows.Forms.TextBox();
            this.txCompletedOn = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnCompleteReopen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Task";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 158);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Added on";
            // 
            // txTask
            // 
            this.txTask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txTask.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txTask.Location = new System.Drawing.Point(90, 39);
            this.txTask.Multiline = true;
            this.txTask.Name = "txTask";
            this.txTask.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txTask.Size = new System.Drawing.Size(413, 108);
            this.txTask.TabIndex = 2;
            this.txTask.TextChanged += new System.EventHandler(this.txTask_TextChanged);
            // 
            // txAddedOn
            // 
            this.txAddedOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txAddedOn.Location = new System.Drawing.Point(90, 155);
            this.txAddedOn.Name = "txAddedOn";
            this.txAddedOn.ReadOnly = true;
            this.txAddedOn.Size = new System.Drawing.Size(413, 20);
            this.txAddedOn.TabIndex = 3;
            // 
            // txCompletedOn
            // 
            this.txCompletedOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txCompletedOn.Location = new System.Drawing.Point(90, 183);
            this.txCompletedOn.Name = "txCompletedOn";
            this.txCompletedOn.ReadOnly = true;
            this.txCompletedOn.Size = new System.Drawing.Size(413, 20);
            this.txCompletedOn.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Completed on";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Task Type";
            // 
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(90, 10);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(413, 21);
            this.comboType.TabIndex = 8;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(428, 227);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 31);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(347, 227);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 31);
            this.button2.TabIndex = 10;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnCompleteReopen
            // 
            this.btnCompleteReopen.Location = new System.Drawing.Point(15, 227);
            this.btnCompleteReopen.Name = "btnCompleteReopen";
            this.btnCompleteReopen.Size = new System.Drawing.Size(75, 31);
            this.btnCompleteReopen.TabIndex = 11;
            this.btnCompleteReopen.Text = "Complete";
            this.btnCompleteReopen.UseVisualStyleBackColor = true;
            this.btnCompleteReopen.Click += new System.EventHandler(this.OnCompleteOrReopen);
            // 
            // TaskDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 270);
            this.ControlBox = false;
            this.Controls.Add(this.btnCompleteReopen);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txCompletedOn);
            this.Controls.Add(this.txAddedOn);
            this.Controls.Add(this.txTask);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TaskDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Task";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txTask;
        private System.Windows.Forms.TextBox txAddedOn;
        private System.Windows.Forms.TextBox txCompletedOn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnCompleteReopen;
    }
}