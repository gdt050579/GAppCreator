namespace GAppCreator
{
    partial class BasicTypeConstantEditDialog
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.txFieldType = new System.Windows.Forms.TextBox();
            this.txDescription = new System.Windows.Forms.TextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.txFieldName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlStringValue = new System.Windows.Forms.Panel();
            this.txStringValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlNumeric = new System.Windows.Forms.Panel();
            this.txNum16 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txNum10 = new System.Windows.Forms.TextBox();
            this.lbDecimal = new System.Windows.Forms.Label();
            this.pnlBool = new System.Windows.Forms.Panel();
            this.rbFalse = new System.Windows.Forms.RadioButton();
            this.rbTrue = new System.Windows.Forms.RadioButton();
            this.pnlFloat = new System.Windows.Forms.Panel();
            this.txFloat = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.pnlPallete = new System.Windows.Forms.Panel();
            this.txColorHexa = new System.Windows.Forms.TextBox();
            this.txColorDecimal = new System.Windows.Forms.TextBox();
            this.nmBlue = new System.Windows.Forms.NumericUpDown();
            this.nmGreen = new System.Windows.Forms.NumericUpDown();
            this.nmRed = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.nmAlpha = new System.Windows.Forms.NumericUpDown();
            this.pnlPreviewColor = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlStringValue.SuspendLayout();
            this.pnlNumeric.SuspendLayout();
            this.pnlBool.SuspendLayout();
            this.pnlFloat.SuspendLayout();
            this.pnlColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 348);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1061, 58);
            this.panel1.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(974, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(893, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txFieldType);
            this.panel2.Controls.Add(this.txDescription);
            this.panel2.Controls.Add(this.labelDescription);
            this.panel2.Controls.Add(this.txFieldName);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1061, 117);
            this.panel2.TabIndex = 0;
            // 
            // txFieldType
            // 
            this.txFieldType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txFieldType.Location = new System.Drawing.Point(425, 10);
            this.txFieldType.Name = "txFieldType";
            this.txFieldType.ReadOnly = true;
            this.txFieldType.Size = new System.Drawing.Size(624, 20);
            this.txFieldType.TabIndex = 11;
            // 
            // txDescription
            // 
            this.txDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txDescription.Location = new System.Drawing.Point(89, 36);
            this.txDescription.Multiline = true;
            this.txDescription.Name = "txDescription";
            this.txDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txDescription.Size = new System.Drawing.Size(960, 78);
            this.txDescription.TabIndex = 2;
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(13, 39);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(60, 13);
            this.labelDescription.TabIndex = 10;
            this.labelDescription.Text = "Description";
            // 
            // txFieldName
            // 
            this.txFieldName.Location = new System.Drawing.Point(89, 10);
            this.txFieldName.Name = "txFieldName";
            this.txFieldName.Size = new System.Drawing.Size(277, 20);
            this.txFieldName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(388, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fiels Name";
            // 
            // pnlStringValue
            // 
            this.pnlStringValue.Controls.Add(this.txStringValue);
            this.pnlStringValue.Controls.Add(this.label3);
            this.pnlStringValue.Location = new System.Drawing.Point(0, 175);
            this.pnlStringValue.Name = "pnlStringValue";
            this.pnlStringValue.Size = new System.Drawing.Size(162, 114);
            this.pnlStringValue.TabIndex = 1;
            this.pnlStringValue.Visible = false;
            // 
            // txStringValue
            // 
            this.txStringValue.AcceptsReturn = true;
            this.txStringValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txStringValue.Location = new System.Drawing.Point(89, 9);
            this.txStringValue.Multiline = true;
            this.txStringValue.Name = "txStringValue";
            this.txStringValue.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txStringValue.Size = new System.Drawing.Size(60, 99);
            this.txStringValue.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Value";
            // 
            // pnlNumeric
            // 
            this.pnlNumeric.Controls.Add(this.txNum16);
            this.pnlNumeric.Controls.Add(this.label4);
            this.pnlNumeric.Controls.Add(this.txNum10);
            this.pnlNumeric.Controls.Add(this.lbDecimal);
            this.pnlNumeric.Location = new System.Drawing.Point(178, 148);
            this.pnlNumeric.Name = "pnlNumeric";
            this.pnlNumeric.Size = new System.Drawing.Size(131, 69);
            this.pnlNumeric.TabIndex = 5;
            this.pnlNumeric.Visible = false;
            // 
            // txNum16
            // 
            this.txNum16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txNum16.Location = new System.Drawing.Point(89, 37);
            this.txNum16.Name = "txNum16";
            this.txNum16.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txNum16.Size = new System.Drawing.Size(29, 20);
            this.txNum16.TabIndex = 6;
            this.txNum16.Text = "0";
            this.txNum16.TextChanged += new System.EventHandler(this.OnChangeNumericalHexa);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Hex";
            // 
            // txNum10
            // 
            this.txNum10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txNum10.Location = new System.Drawing.Point(89, 9);
            this.txNum10.Name = "txNum10";
            this.txNum10.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txNum10.Size = new System.Drawing.Size(29, 20);
            this.txNum10.TabIndex = 1;
            this.txNum10.Text = "0";
            this.txNum10.TextChanged += new System.EventHandler(this.OnChangeNumericDecimal);
            // 
            // lbDecimal
            // 
            this.lbDecimal.AutoSize = true;
            this.lbDecimal.Location = new System.Drawing.Point(9, 12);
            this.lbDecimal.Name = "lbDecimal";
            this.lbDecimal.Size = new System.Drawing.Size(45, 13);
            this.lbDecimal.TabIndex = 0;
            this.lbDecimal.Text = "Decimal";
            // 
            // pnlBool
            // 
            this.pnlBool.Controls.Add(this.rbFalse);
            this.pnlBool.Controls.Add(this.rbTrue);
            this.pnlBool.Location = new System.Drawing.Point(178, 223);
            this.pnlBool.Name = "pnlBool";
            this.pnlBool.Size = new System.Drawing.Size(74, 66);
            this.pnlBool.TabIndex = 4;
            this.pnlBool.Visible = false;
            // 
            // rbFalse
            // 
            this.rbFalse.AutoSize = true;
            this.rbFalse.Location = new System.Drawing.Point(12, 36);
            this.rbFalse.Name = "rbFalse";
            this.rbFalse.Size = new System.Drawing.Size(50, 17);
            this.rbFalse.TabIndex = 4;
            this.rbFalse.TabStop = true;
            this.rbFalse.Text = "False";
            this.rbFalse.UseVisualStyleBackColor = true;
            // 
            // rbTrue
            // 
            this.rbTrue.AutoSize = true;
            this.rbTrue.Location = new System.Drawing.Point(12, 10);
            this.rbTrue.Name = "rbTrue";
            this.rbTrue.Size = new System.Drawing.Size(47, 17);
            this.rbTrue.TabIndex = 3;
            this.rbTrue.TabStop = true;
            this.rbTrue.Text = "True";
            this.rbTrue.UseVisualStyleBackColor = true;
            // 
            // pnlFloat
            // 
            this.pnlFloat.Controls.Add(this.txFloat);
            this.pnlFloat.Controls.Add(this.label8);
            this.pnlFloat.Location = new System.Drawing.Point(0, 295);
            this.pnlFloat.Name = "pnlFloat";
            this.pnlFloat.Size = new System.Drawing.Size(118, 44);
            this.pnlFloat.TabIndex = 2;
            this.pnlFloat.Visible = false;
            // 
            // txFloat
            // 
            this.txFloat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txFloat.Location = new System.Drawing.Point(89, 9);
            this.txFloat.Name = "txFloat";
            this.txFloat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txFloat.Size = new System.Drawing.Size(16, 20);
            this.txFloat.TabIndex = 1;
            this.txFloat.Text = "0.0";
            this.txFloat.TextChanged += new System.EventHandler(this.OnChangeValueFloat);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Value";
            // 
            // pnlColor
            // 
            this.pnlColor.Controls.Add(this.pnlPallete);
            this.pnlColor.Controls.Add(this.txColorHexa);
            this.pnlColor.Controls.Add(this.txColorDecimal);
            this.pnlColor.Controls.Add(this.nmBlue);
            this.pnlColor.Controls.Add(this.nmGreen);
            this.pnlColor.Controls.Add(this.nmRed);
            this.pnlColor.Controls.Add(this.label13);
            this.pnlColor.Controls.Add(this.label12);
            this.pnlColor.Controls.Add(this.label11);
            this.pnlColor.Controls.Add(this.nmAlpha);
            this.pnlColor.Controls.Add(this.pnlPreviewColor);
            this.pnlColor.Controls.Add(this.label7);
            this.pnlColor.Controls.Add(this.label9);
            this.pnlColor.Controls.Add(this.label10);
            this.pnlColor.Location = new System.Drawing.Point(315, 148);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(449, 189);
            this.pnlColor.TabIndex = 6;
            this.pnlColor.Visible = false;
            // 
            // pnlPallete
            // 
            this.pnlPallete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlPallete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPallete.Location = new System.Drawing.Point(128, 12);
            this.pnlPallete.Name = "pnlPallete";
            this.pnlPallete.Size = new System.Drawing.Size(210, 160);
            this.pnlPallete.TabIndex = 9;
            this.pnlPallete.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintPallete);
            this.pnlPallete.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnSelectColorFromPallete);
            // 
            // txColorHexa
            // 
            this.txColorHexa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txColorHexa.Location = new System.Drawing.Point(399, 37);
            this.txColorHexa.Name = "txColorHexa";
            this.txColorHexa.Size = new System.Drawing.Size(37, 20);
            this.txColorHexa.TabIndex = 1;
            this.txColorHexa.TextChanged += new System.EventHandler(this.OnChangeColorHexa);
            // 
            // txColorDecimal
            // 
            this.txColorDecimal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txColorDecimal.Location = new System.Drawing.Point(399, 9);
            this.txColorDecimal.Name = "txColorDecimal";
            this.txColorDecimal.Size = new System.Drawing.Size(37, 20);
            this.txColorDecimal.TabIndex = 0;
            this.txColorDecimal.TextChanged += new System.EventHandler(this.OnChangeColorDecimal);
            // 
            // nmBlue
            // 
            this.nmBlue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nmBlue.Location = new System.Drawing.Point(399, 153);
            this.nmBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmBlue.Name = "nmBlue";
            this.nmBlue.Size = new System.Drawing.Size(37, 20);
            this.nmBlue.TabIndex = 5;
            this.nmBlue.ValueChanged += new System.EventHandler(this.OnColorChannelChanged);
            // 
            // nmGreen
            // 
            this.nmGreen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nmGreen.Location = new System.Drawing.Point(399, 127);
            this.nmGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmGreen.Name = "nmGreen";
            this.nmGreen.Size = new System.Drawing.Size(37, 20);
            this.nmGreen.TabIndex = 4;
            this.nmGreen.ValueChanged += new System.EventHandler(this.OnColorChannelChanged);
            // 
            // nmRed
            // 
            this.nmRed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nmRed.Location = new System.Drawing.Point(399, 101);
            this.nmRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmRed.Name = "nmRed";
            this.nmRed.Size = new System.Drawing.Size(37, 20);
            this.nmRed.TabIndex = 3;
            this.nmRed.ValueChanged += new System.EventHandler(this.OnColorChannelChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(350, 156);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "Blue";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(350, 129);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(36, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Green";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(350, 103);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(27, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Red";
            // 
            // nmAlpha
            // 
            this.nmAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nmAlpha.Location = new System.Drawing.Point(399, 76);
            this.nmAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmAlpha.Name = "nmAlpha";
            this.nmAlpha.Size = new System.Drawing.Size(37, 20);
            this.nmAlpha.TabIndex = 2;
            this.nmAlpha.ValueChanged += new System.EventHandler(this.OnColorChannelChanged);
            // 
            // pnlPreviewColor
            // 
            this.pnlPreviewColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlPreviewColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPreviewColor.Location = new System.Drawing.Point(12, 12);
            this.pnlPreviewColor.Name = "pnlPreviewColor";
            this.pnlPreviewColor.Size = new System.Drawing.Size(110, 160);
            this.pnlPreviewColor.TabIndex = 8;
            this.pnlPreviewColor.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintColor);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(350, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Alpha";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(350, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Hex";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(350, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Decimal";
            // 
            // BasicTypeConstantEditDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1061, 406);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.pnlFloat);
            this.Controls.Add(this.pnlBool);
            this.Controls.Add(this.pnlNumeric);
            this.Controls.Add(this.pnlStringValue);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BasicTypeConstantEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Value";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlStringValue.ResumeLayout(false);
            this.pnlStringValue.PerformLayout();
            this.pnlNumeric.ResumeLayout(false);
            this.pnlNumeric.PerformLayout();
            this.pnlBool.ResumeLayout(false);
            this.pnlBool.PerformLayout();
            this.pnlFloat.ResumeLayout(false);
            this.pnlFloat.PerformLayout();
            this.pnlColor.ResumeLayout(false);
            this.pnlColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmAlpha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txFieldName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlStringValue;
        private System.Windows.Forms.TextBox txStringValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlNumeric;
        private System.Windows.Forms.TextBox txNum16;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txNum10;
        private System.Windows.Forms.Label lbDecimal;
        private System.Windows.Forms.Panel pnlBool;
        private System.Windows.Forms.RadioButton rbFalse;
        private System.Windows.Forms.RadioButton rbTrue;
        private System.Windows.Forms.Panel pnlFloat;
        private System.Windows.Forms.TextBox txFloat;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel pnlColor;
        private System.Windows.Forms.NumericUpDown nmBlue;
        private System.Windows.Forms.NumericUpDown nmGreen;
        private System.Windows.Forms.NumericUpDown nmRed;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nmAlpha;
        private System.Windows.Forms.Panel pnlPreviewColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txColorDecimal;
        private System.Windows.Forms.TextBox txColorHexa;
        private System.Windows.Forms.TextBox txDescription;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Panel pnlPallete;
        private System.Windows.Forms.TextBox txFieldType;
    }
}