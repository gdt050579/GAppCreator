namespace GAppCreator
{
    partial class DebugParamControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugParamControl));
            this.lbName = new System.Windows.Forms.ToolStripLabel();
            this.txValue = new System.Windows.Forms.ToolStripTextBox();
            this.comboEnum = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lbDescription = new System.Windows.Forms.ToolStripLabel();
            this.btnColor = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbName
            // 
            this.lbName.AutoSize = false;
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(120, 41);
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txValue
            // 
            this.txValue.AutoSize = false;
            this.txValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txValue.Name = "txValue";
            this.txValue.Size = new System.Drawing.Size(120, 23);
            // 
            // comboEnum
            // 
            this.comboEnum.AutoSize = false;
            this.comboEnum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEnum.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.comboEnum.Name = "comboEnum";
            this.comboEnum.Size = new System.Drawing.Size(120, 23);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbName,
            this.txValue,
            this.btnColor,
            this.comboEnum,
            this.lbDescription});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(730, 32);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lbDescription
            // 
            this.lbDescription.AutoSize = false;
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(400, 41);
            this.lbDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnColor
            // 
            this.btnColor.AutoSize = false;
            this.btnColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnColor.Image = ((System.Drawing.Image)(resources.GetObject("btnColor.Image")));
            this.btnColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(120, 29);
            this.btnColor.Text = "Color";
            this.btnColor.Click += new System.EventHandler(this.OnChangeColor);
            // 
            // DebugParamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Name = "DebugParamControl";
            this.Size = new System.Drawing.Size(730, 32);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripLabel lbName;
        private System.Windows.Forms.ToolStripTextBox txValue;
        private System.Windows.Forms.ToolStripComboBox comboEnum;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lbDescription;
        private System.Windows.Forms.ToolStripButton btnColor;

    }
}
