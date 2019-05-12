namespace GAppCreator
{
    partial class StringAddDialog
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
            this.lbArray = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbX = new System.Windows.Forms.Label();
            this.nmArray2 = new System.Windows.Forms.NumericUpDown();
            this.nmArray1 = new System.Windows.Forms.NumericUpDown();
            this.comboArray = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dgValues = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmArray2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmArray1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgValues)).BeginInit();
            this.SuspendLayout();
            // 
            // lbArray
            // 
            this.lbArray.AutoSize = true;
            this.lbArray.Location = new System.Drawing.Point(-47, 43);
            this.lbArray.Name = "lbArray";
            this.lbArray.Size = new System.Drawing.Size(31, 13);
            this.lbArray.TabIndex = 26;
            this.lbArray.Text = "Array";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 380);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(662, 58);
            this.panel1.TabIndex = 25;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(568, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(487, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-47, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Name";
            // 
            // lbX
            // 
            this.lbX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(541, 47);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(12, 13);
            this.lbX.TabIndex = 34;
            this.lbX.Text = "x";
            // 
            // nmArray2
            // 
            this.nmArray2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nmArray2.Location = new System.Drawing.Point(561, 44);
            this.nmArray2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmArray2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmArray2.Name = "nmArray2";
            this.nmArray2.Size = new System.Drawing.Size(82, 20);
            this.nmArray2.TabIndex = 30;
            this.nmArray2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nmArray1
            // 
            this.nmArray1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nmArray1.Location = new System.Drawing.Point(453, 44);
            this.nmArray1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmArray1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmArray1.Name = "nmArray1";
            this.nmArray1.Size = new System.Drawing.Size(82, 20);
            this.nmArray1.TabIndex = 29;
            this.nmArray1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // comboArray
            // 
            this.comboArray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboArray.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboArray.FormattingEnabled = true;
            this.comboArray.Items.AddRange(new object[] {
            "None",
            "Vector",
            "Matrix"});
            this.comboArray.Location = new System.Drawing.Point(71, 44);
            this.comboArray.Name = "comboArray";
            this.comboArray.Size = new System.Drawing.Size(359, 21);
            this.comboArray.TabIndex = 28;
            this.comboArray.SelectedIndexChanged += new System.EventHandler(this.OnArrayModeChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Array";
            // 
            // txName
            // 
            this.txName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txName.Location = new System.Drawing.Point(71, 12);
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(572, 20);
            this.txName.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Name";
            // 
            // dgValues
            // 
            this.dgValues.AllowUserToAddRows = false;
            this.dgValues.AllowUserToDeleteRows = false;
            this.dgValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgValues.Location = new System.Drawing.Point(14, 91);
            this.dgValues.Name = "dgValues";
            this.dgValues.Size = new System.Drawing.Size(629, 267);
            this.dgValues.TabIndex = 35;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Language";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Language";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Value";
            this.Column2.Name = "Column2";
            // 
            // StringAddDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(662, 438);
            this.Controls.Add(this.dgValues);
            this.Controls.Add(this.lbX);
            this.Controls.Add(this.nmArray2);
            this.Controls.Add(this.nmArray1);
            this.Controls.Add(this.comboArray);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbArray);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "StringAddDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add new string";
            this.Shown += new System.EventHandler(this.StringAddDialog_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nmArray2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmArray1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgValues)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbArray;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.NumericUpDown nmArray2;
        private System.Windows.Forms.NumericUpDown nmArray1;
        private System.Windows.Forms.ComboBox comboArray;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgValues;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}