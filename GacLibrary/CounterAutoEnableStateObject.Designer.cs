namespace GAppCreator
{
    partial class CounterAutoEnableStateObject
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
            this.comboMethod = new System.Windows.Forms.ComboBox();
            this.btnAndOr = new System.Windows.Forms.Button();
            this.nmValue = new System.Windows.Forms.NumericUpDown();
            this.comboValue = new System.Windows.Forms.ComboBox();
            this.btnAddReemove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nmValue)).BeginInit();
            this.SuspendLayout();
            // 
            // comboMethod
            // 
            this.comboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMethod.FormattingEnabled = true;
            this.comboMethod.Location = new System.Drawing.Point(50, 4);
            this.comboMethod.Name = "comboMethod";
            this.comboMethod.Size = new System.Drawing.Size(415, 21);
            this.comboMethod.TabIndex = 0;
            this.comboMethod.SelectedIndexChanged += new System.EventHandler(this.comboMethod_SelectedIndexChanged);
            // 
            // btnAndOr
            // 
            this.btnAndOr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAndOr.Location = new System.Drawing.Point(277, 26);
            this.btnAndOr.Name = "btnAndOr";
            this.btnAndOr.Size = new System.Drawing.Size(64, 23);
            this.btnAndOr.TabIndex = 1;
            this.btnAndOr.Text = "AND";
            this.btnAndOr.UseVisualStyleBackColor = true;
            this.btnAndOr.Click += new System.EventHandler(this.OnClickBtnAndOr);
            // 
            // nmValue
            // 
            this.nmValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nmValue.Location = new System.Drawing.Point(471, 4);
            this.nmValue.Name = "nmValue";
            this.nmValue.Size = new System.Drawing.Size(334, 20);
            this.nmValue.TabIndex = 2;
            // 
            // comboValue
            // 
            this.comboValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboValue.FormattingEnabled = true;
            this.comboValue.Location = new System.Drawing.Point(471, 4);
            this.comboValue.Name = "comboValue";
            this.comboValue.Size = new System.Drawing.Size(334, 21);
            this.comboValue.TabIndex = 3;
            // 
            // btnAddReemove
            // 
            this.btnAddReemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddReemove.Location = new System.Drawing.Point(4, 4);
            this.btnAddReemove.Name = "btnAddReemove";
            this.btnAddReemove.Size = new System.Drawing.Size(40, 45);
            this.btnAddReemove.TabIndex = 4;
            this.btnAddReemove.Text = "+";
            this.btnAddReemove.UseVisualStyleBackColor = true;
            this.btnAddReemove.Click += new System.EventHandler(this.OnAddRemove);
            // 
            // CounterAutoEnableStateObject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnAddReemove);
            this.Controls.Add(this.comboValue);
            this.Controls.Add(this.nmValue);
            this.Controls.Add(this.btnAndOr);
            this.Controls.Add(this.comboMethod);
            this.Name = "CounterAutoEnableStateObject";
            this.Size = new System.Drawing.Size(808, 52);
            this.Resize += new System.EventHandler(this.CounterAutoEnableStateObject_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.nmValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboMethod;
        private System.Windows.Forms.Button btnAndOr;
        private System.Windows.Forms.NumericUpDown nmValue;
        private System.Windows.Forms.ComboBox comboValue;
        private System.Windows.Forms.Button btnAddReemove;
    }
}
