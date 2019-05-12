namespace GAppCreator
{
    partial class NewSceneDialog
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
            this.rbStandardScenes = new System.Windows.Forms.RadioButton();
            this.rbCustomScenes = new System.Windows.Forms.RadioButton();
            this.txCustomScene = new System.Windows.Forms.TextBox();
            this.comboStandardScenes = new System.Windows.Forms.ComboBox();
            this.lstFunctions = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbOnPaint = new System.Windows.Forms.Label();
            this.comboOnPaint = new System.Windows.Forms.ComboBox();
            this.comboOnBack = new System.Windows.Forms.ComboBox();
            this.lbOnBackKey = new System.Windows.Forms.Label();
            this.lbTemplate = new System.Windows.Forms.Label();
            this.comboTemplate = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 461);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(484, 58);
            this.panel1.TabIndex = 6;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(390, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 1;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(309, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 0;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnOK);
            // 
            // rbStandardScenes
            // 
            this.rbStandardScenes.AutoSize = true;
            this.rbStandardScenes.Location = new System.Drawing.Point(12, 13);
            this.rbStandardScenes.Name = "rbStandardScenes";
            this.rbStandardScenes.Size = new System.Drawing.Size(105, 17);
            this.rbStandardScenes.TabIndex = 7;
            this.rbStandardScenes.TabStop = true;
            this.rbStandardScenes.Text = "Standard scenes";
            this.rbStandardScenes.UseVisualStyleBackColor = true;
            this.rbStandardScenes.Click += new System.EventHandler(this.OnEnableDisableScenes);
            // 
            // rbCustomScenes
            // 
            this.rbCustomScenes.AutoSize = true;
            this.rbCustomScenes.Location = new System.Drawing.Point(12, 40);
            this.rbCustomScenes.Name = "rbCustomScenes";
            this.rbCustomScenes.Size = new System.Drawing.Size(92, 17);
            this.rbCustomScenes.TabIndex = 8;
            this.rbCustomScenes.TabStop = true;
            this.rbCustomScenes.Text = "Custom scene";
            this.rbCustomScenes.UseVisualStyleBackColor = true;
            this.rbCustomScenes.Click += new System.EventHandler(this.OnEnableDisableScenes);
            // 
            // txCustomScene
            // 
            this.txCustomScene.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txCustomScene.Location = new System.Drawing.Point(147, 39);
            this.txCustomScene.Name = "txCustomScene";
            this.txCustomScene.Size = new System.Drawing.Size(318, 20);
            this.txCustomScene.TabIndex = 9;
            this.txCustomScene.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // comboStandardScenes
            // 
            this.comboStandardScenes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboStandardScenes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStandardScenes.FormattingEnabled = true;
            this.comboStandardScenes.Location = new System.Drawing.Point(147, 12);
            this.comboStandardScenes.Name = "comboStandardScenes";
            this.comboStandardScenes.Size = new System.Drawing.Size(318, 21);
            this.comboStandardScenes.TabIndex = 10;
            // 
            // lstFunctions
            // 
            this.lstFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFunctions.CheckBoxes = true;
            this.lstFunctions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstFunctions.FullRowSelect = true;
            this.lstFunctions.GridLines = true;
            this.lstFunctions.Location = new System.Drawing.Point(12, 147);
            this.lstFunctions.Name = "lstFunctions";
            this.lstFunctions.Size = new System.Drawing.Size(453, 308);
            this.lstFunctions.TabIndex = 11;
            this.lstFunctions.UseCompatibleStateImageBehavior = false;
            this.lstFunctions.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Function";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 600;
            // 
            // lbOnPaint
            // 
            this.lbOnPaint.AutoSize = true;
            this.lbOnPaint.Location = new System.Drawing.Point(9, 96);
            this.lbOnPaint.Name = "lbOnPaint";
            this.lbOnPaint.Size = new System.Drawing.Size(72, 13);
            this.lbOnPaint.TabIndex = 12;
            this.lbOnPaint.Text = "OnPaint code";
            // 
            // comboOnPaint
            // 
            this.comboOnPaint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboOnPaint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOnPaint.FormattingEnabled = true;
            this.comboOnPaint.Items.AddRange(new object[] {
            "DoNothing",
            "Paint controls"});
            this.comboOnPaint.Location = new System.Drawing.Point(147, 93);
            this.comboOnPaint.Name = "comboOnPaint";
            this.comboOnPaint.Size = new System.Drawing.Size(318, 21);
            this.comboOnPaint.TabIndex = 13;
            // 
            // comboOnBack
            // 
            this.comboOnBack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboOnBack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOnBack.FormattingEnabled = true;
            this.comboOnBack.Items.AddRange(new object[] {
            "Do Nothing",
            "GoTo Main Scene",
            "GoTo Next Scene",
            "Close Application"});
            this.comboOnBack.Location = new System.Drawing.Point(147, 118);
            this.comboOnBack.Name = "comboOnBack";
            this.comboOnBack.Size = new System.Drawing.Size(318, 21);
            this.comboOnBack.TabIndex = 14;
            // 
            // lbOnBackKey
            // 
            this.lbOnBackKey.AutoSize = true;
            this.lbOnBackKey.Location = new System.Drawing.Point(9, 121);
            this.lbOnBackKey.Name = "lbOnBackKey";
            this.lbOnBackKey.Size = new System.Drawing.Size(111, 13);
            this.lbOnBackKey.TabIndex = 15;
            this.lbOnBackKey.Text = "On Back Key Pressed";
            // 
            // lbTemplate
            // 
            this.lbTemplate.AutoSize = true;
            this.lbTemplate.Location = new System.Drawing.Point(9, 71);
            this.lbTemplate.Name = "lbTemplate";
            this.lbTemplate.Size = new System.Drawing.Size(51, 13);
            this.lbTemplate.TabIndex = 16;
            this.lbTemplate.Text = "Template";
            // 
            // comboTemplate
            // 
            this.comboTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTemplate.FormattingEnabled = true;
            this.comboTemplate.Location = new System.Drawing.Point(147, 68);
            this.comboTemplate.Name = "comboTemplate";
            this.comboTemplate.Size = new System.Drawing.Size(318, 21);
            this.comboTemplate.TabIndex = 17;
            this.comboTemplate.SelectedIndexChanged += new System.EventHandler(this.OnSelectTemplate);
            // 
            // NewSceneDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 519);
            this.Controls.Add(this.comboTemplate);
            this.Controls.Add(this.lbTemplate);
            this.Controls.Add(this.lbOnBackKey);
            this.Controls.Add(this.comboOnBack);
            this.Controls.Add(this.comboOnPaint);
            this.Controls.Add(this.lbOnPaint);
            this.Controls.Add(this.lstFunctions);
            this.Controls.Add(this.comboStandardScenes);
            this.Controls.Add(this.txCustomScene);
            this.Controls.Add(this.rbCustomScenes);
            this.Controls.Add(this.rbStandardScenes);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewSceneDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add new scene";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.RadioButton rbStandardScenes;
        private System.Windows.Forms.RadioButton rbCustomScenes;
        private System.Windows.Forms.TextBox txCustomScene;
        private System.Windows.Forms.ComboBox comboStandardScenes;
        private System.Windows.Forms.ListView lstFunctions;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label lbOnPaint;
        private System.Windows.Forms.ComboBox comboOnPaint;
        private System.Windows.Forms.ComboBox comboOnBack;
        private System.Windows.Forms.Label lbOnBackKey;
        private System.Windows.Forms.Label lbTemplate;
        private System.Windows.Forms.ComboBox comboTemplate;
    }
}