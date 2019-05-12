namespace GAppCreator
{
    partial class NewPluginObjectDialog
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
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboPlugins = new System.Windows.Forms.ComboBox();
            this.txVariableName = new System.Windows.Forms.TextBox();
            this.comboArrayType = new System.Windows.Forms.ComboBox();
            this.array1 = new System.Windows.Forms.NumericUpDown();
            this.array2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.comboLanguage = new System.Windows.Forms.ComboBox();
            this.lbArray1From = new System.Windows.Forms.Label();
            this.lbArray2From = new System.Windows.Forms.Label();
            this.array1End = new System.Windows.Forms.NumericUpDown();
            this.array2End = new System.Windows.Forms.NumericUpDown();
            this.lbArray1To = new System.Windows.Forms.Label();
            this.lbArray2To = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.array1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.array2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.array1End)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.array2End)).BeginInit();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(390, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 3;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(309, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 2;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnOK);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 199);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(484, 58);
            this.panel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Plugin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Array";
            // 
            // comboPlugins
            // 
            this.comboPlugins.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPlugins.FormattingEnabled = true;
            this.comboPlugins.Location = new System.Drawing.Point(81, 25);
            this.comboPlugins.Name = "comboPlugins";
            this.comboPlugins.Size = new System.Drawing.Size(384, 21);
            this.comboPlugins.TabIndex = 8;
            // 
            // txVariableName
            // 
            this.txVariableName.Location = new System.Drawing.Point(81, 52);
            this.txVariableName.Name = "txVariableName";
            this.txVariableName.Size = new System.Drawing.Size(384, 20);
            this.txVariableName.TabIndex = 9;
            // 
            // comboArrayType
            // 
            this.comboArrayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboArrayType.FormattingEnabled = true;
            this.comboArrayType.Items.AddRange(new object[] {
            "None",
            "Vector (one dimension)",
            "Matrix (two dimensions)"});
            this.comboArrayType.Location = new System.Drawing.Point(81, 78);
            this.comboArrayType.Name = "comboArrayType";
            this.comboArrayType.Size = new System.Drawing.Size(384, 21);
            this.comboArrayType.TabIndex = 10;
            this.comboArrayType.SelectedIndexChanged += new System.EventHandler(this.comboArrayType_SelectedIndexChanged);
            // 
            // array1
            // 
            this.array1.Location = new System.Drawing.Point(201, 105);
            this.array1.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.array1.Name = "array1";
            this.array1.Size = new System.Drawing.Size(112, 20);
            this.array1.TabIndex = 11;
            this.array1.ValueChanged += new System.EventHandler(this.array1_ValueChanged);
            // 
            // array2
            // 
            this.array2.Location = new System.Drawing.Point(201, 129);
            this.array2.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.array2.Name = "array2";
            this.array2.Size = new System.Drawing.Size(112, 20);
            this.array2.TabIndex = 12;
            this.array2.ValueChanged += new System.EventHandler(this.array2_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Language";
            // 
            // comboLanguage
            // 
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.FormattingEnabled = true;
            this.comboLanguage.Location = new System.Drawing.Point(81, 160);
            this.comboLanguage.Name = "comboLanguage";
            this.comboLanguage.Size = new System.Drawing.Size(384, 21);
            this.comboLanguage.TabIndex = 14;
            // 
            // lbArray1From
            // 
            this.lbArray1From.AutoSize = true;
            this.lbArray1From.Location = new System.Drawing.Point(78, 108);
            this.lbArray1From.Name = "lbArray1From";
            this.lbArray1From.Size = new System.Drawing.Size(99, 13);
            this.lbArray1From.TabIndex = 15;
            this.lbArray1From.Text = "First dimension from";
            // 
            // lbArray2From
            // 
            this.lbArray2From.AutoSize = true;
            this.lbArray2From.Location = new System.Drawing.Point(78, 131);
            this.lbArray2From.Name = "lbArray2From";
            this.lbArray2From.Size = new System.Drawing.Size(117, 13);
            this.lbArray2From.TabIndex = 16;
            this.lbArray2From.Text = "Second dimension from";
            // 
            // array1End
            // 
            this.array1End.Location = new System.Drawing.Point(353, 105);
            this.array1End.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.array1End.Name = "array1End";
            this.array1End.Size = new System.Drawing.Size(112, 20);
            this.array1End.TabIndex = 17;
            // 
            // array2End
            // 
            this.array2End.Location = new System.Drawing.Point(353, 129);
            this.array2End.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.array2End.Name = "array2End";
            this.array2End.Size = new System.Drawing.Size(112, 20);
            this.array2End.TabIndex = 18;
            // 
            // lbArray1To
            // 
            this.lbArray1To.AutoSize = true;
            this.lbArray1To.Location = new System.Drawing.Point(325, 108);
            this.lbArray1To.Name = "lbArray1To";
            this.lbArray1To.Size = new System.Drawing.Size(16, 13);
            this.lbArray1To.TabIndex = 19;
            this.lbArray1To.Text = "to";
            // 
            // lbArray2To
            // 
            this.lbArray2To.AutoSize = true;
            this.lbArray2To.Location = new System.Drawing.Point(325, 131);
            this.lbArray2To.Name = "lbArray2To";
            this.lbArray2To.Size = new System.Drawing.Size(16, 13);
            this.lbArray2To.TabIndex = 20;
            this.lbArray2To.Text = "to";
            // 
            // NewPluginObjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 257);
            this.ControlBox = false;
            this.Controls.Add(this.lbArray2To);
            this.Controls.Add(this.lbArray1To);
            this.Controls.Add(this.array2End);
            this.Controls.Add(this.array1End);
            this.Controls.Add(this.lbArray2From);
            this.Controls.Add(this.lbArray1From);
            this.Controls.Add(this.comboLanguage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.array2);
            this.Controls.Add(this.array1);
            this.Controls.Add(this.comboArrayType);
            this.Controls.Add(this.txVariableName);
            this.Controls.Add(this.comboPlugins);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewPluginObjectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Plugin Object(s)";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.array1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.array2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.array1End)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.array2End)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboPlugins;
        private System.Windows.Forms.TextBox txVariableName;
        private System.Windows.Forms.ComboBox comboArrayType;
        private System.Windows.Forms.NumericUpDown array1;
        private System.Windows.Forms.NumericUpDown array2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboLanguage;
        private System.Windows.Forms.Label lbArray1From;
        private System.Windows.Forms.Label lbArray2From;
        private System.Windows.Forms.NumericUpDown array1End;
        private System.Windows.Forms.NumericUpDown array2End;
        private System.Windows.Forms.Label lbArray1To;
        private System.Windows.Forms.Label lbArray2To;
    }
}