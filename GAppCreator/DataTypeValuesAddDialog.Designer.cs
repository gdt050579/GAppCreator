namespace GAppCreator
{
    partial class DataTypeValuesAddDialog
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbArray = new System.Windows.Forms.Label();
            this.comboArray = new System.Windows.Forms.ComboBox();
            this.nmArray1 = new System.Windows.Forms.NumericUpDown();
            this.nmArray2 = new System.Windows.Forms.NumericUpDown();
            this.lbX = new System.Windows.Forms.Label();
            this.nmQuantity = new System.Windows.Forms.NumericUpDown();
            this.cbNull = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmArray1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmArray2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmQuantity)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 116);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(422, 58);
            this.panel1.TabIndex = 12;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(328, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(247, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // txName
            // 
            this.txName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txName.Location = new System.Drawing.Point(72, 17);
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(330, 20);
            this.txName.TabIndex = 0;
            this.txName.TextChanged += new System.EventHandler(this.txName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Name";
            // 
            // lbArray
            // 
            this.lbArray.AutoSize = true;
            this.lbArray.Location = new System.Drawing.Point(12, 52);
            this.lbArray.Name = "lbArray";
            this.lbArray.Size = new System.Drawing.Size(31, 13);
            this.lbArray.TabIndex = 13;
            this.lbArray.Text = "Array";
            // 
            // comboArray
            // 
            this.comboArray.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboArray.FormattingEnabled = true;
            this.comboArray.Items.AddRange(new object[] {
            "None",
            "Vector",
            "Matrix"});
            this.comboArray.Location = new System.Drawing.Point(72, 49);
            this.comboArray.Name = "comboArray";
            this.comboArray.Size = new System.Drawing.Size(121, 21);
            this.comboArray.TabIndex = 1;
            this.comboArray.SelectedIndexChanged += new System.EventHandler(this.OnArrayModeChanged);
            // 
            // nmArray1
            // 
            this.nmArray1.Location = new System.Drawing.Point(216, 49);
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
            this.nmArray1.TabIndex = 2;
            this.nmArray1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nmArray2
            // 
            this.nmArray2.Location = new System.Drawing.Point(320, 49);
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
            this.nmArray2.TabIndex = 3;
            this.nmArray2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(304, 52);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(12, 13);
            this.lbX.TabIndex = 17;
            this.lbX.Text = "x";
            // 
            // nmQuantity
            // 
            this.nmQuantity.Location = new System.Drawing.Point(72, 49);
            this.nmQuantity.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmQuantity.Name = "nmQuantity";
            this.nmQuantity.Size = new System.Drawing.Size(330, 20);
            this.nmQuantity.TabIndex = 4;
            this.nmQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbNull
            // 
            this.cbNull.AutoSize = true;
            this.cbNull.Location = new System.Drawing.Point(72, 82);
            this.cbNull.Name = "cbNull";
            this.cbNull.Size = new System.Drawing.Size(89, 17);
            this.cbNull.TabIndex = 5;
            this.cbNull.Text = "Field is NULL";
            this.cbNull.UseVisualStyleBackColor = true;
            // 
            // DataTypeValuesAddDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(422, 174);
            this.Controls.Add(this.cbNull);
            this.Controls.Add(this.lbX);
            this.Controls.Add(this.nmArray2);
            this.Controls.Add(this.nmArray1);
            this.Controls.Add(this.comboArray);
            this.Controls.Add(this.lbArray);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nmQuantity);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DataTypeValuesAddDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add values";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nmArray1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmArray2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbArray;
        private System.Windows.Forms.ComboBox comboArray;
        private System.Windows.Forms.NumericUpDown nmArray1;
        private System.Windows.Forms.NumericUpDown nmArray2;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.NumericUpDown nmQuantity;
        private System.Windows.Forms.CheckBox cbNull;
    }
}