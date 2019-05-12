namespace GAppCreator
{
    partial class ApplicationTitleDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationTitleDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbBuildTitle = new System.Windows.Forms.Label();
            this.comboLanguage = new System.Windows.Forms.ComboBox();
            this.txDefault = new System.Windows.Forms.TextBox();
            this.txBuild = new System.Windows.Forms.TextBox();
            this.btnPrjToBld = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnBldToPrj = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 120);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(526, 58);
            this.panel1.TabIndex = 6;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(432, 16);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 30);
            this.button3.TabIndex = 3;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(351, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 2;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnOK);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Language";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Project default Title";
            // 
            // lbBuildTitle
            // 
            this.lbBuildTitle.AutoSize = true;
            this.lbBuildTitle.Location = new System.Drawing.Point(13, 77);
            this.lbBuildTitle.Name = "lbBuildTitle";
            this.lbBuildTitle.Size = new System.Drawing.Size(53, 13);
            this.lbBuildTitle.TabIndex = 9;
            this.lbBuildTitle.Text = "Build Title";
            // 
            // comboLanguage
            // 
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.FormattingEnabled = true;
            this.comboLanguage.Location = new System.Drawing.Point(145, 10);
            this.comboLanguage.Name = "comboLanguage";
            this.comboLanguage.Size = new System.Drawing.Size(362, 21);
            this.comboLanguage.TabIndex = 10;
            this.comboLanguage.SelectedIndexChanged += new System.EventHandler(this.OnChangeLanguage);
            // 
            // txDefault
            // 
            this.txDefault.Location = new System.Drawing.Point(145, 42);
            this.txDefault.Name = "txDefault";
            this.txDefault.Size = new System.Drawing.Size(311, 20);
            this.txDefault.TabIndex = 11;
            // 
            // txBuild
            // 
            this.txBuild.Location = new System.Drawing.Point(145, 74);
            this.txBuild.Name = "txBuild";
            this.txBuild.Size = new System.Drawing.Size(311, 20);
            this.txBuild.TabIndex = 12;
            // 
            // btnPrjToBld
            // 
            this.btnPrjToBld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrjToBld.ImageIndex = 0;
            this.btnPrjToBld.ImageList = this.imageList1;
            this.btnPrjToBld.Location = new System.Drawing.Point(468, 36);
            this.btnPrjToBld.Name = "btnPrjToBld";
            this.btnPrjToBld.Size = new System.Drawing.Size(39, 30);
            this.btnPrjToBld.TabIndex = 4;
            this.btnPrjToBld.UseVisualStyleBackColor = true;
            this.btnPrjToBld.Click += new System.EventHandler(this.OnCopyFromProject);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageList1.Images.SetKeyName(0, "ArrowDown.bmp");
            this.imageList1.Images.SetKeyName(1, "ArrowUp.bmp");
            // 
            // btnBldToPrj
            // 
            this.btnBldToPrj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBldToPrj.ImageIndex = 1;
            this.btnBldToPrj.ImageList = this.imageList1;
            this.btnBldToPrj.Location = new System.Drawing.Point(468, 68);
            this.btnBldToPrj.Name = "btnBldToPrj";
            this.btnBldToPrj.Size = new System.Drawing.Size(39, 30);
            this.btnBldToPrj.TabIndex = 13;
            this.btnBldToPrj.UseVisualStyleBackColor = true;
            this.btnBldToPrj.Click += new System.EventHandler(this.OnCopyFromBuild);
            // 
            // ApplicationTitleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 178);
            this.ControlBox = false;
            this.Controls.Add(this.btnBldToPrj);
            this.Controls.Add(this.btnPrjToBld);
            this.Controls.Add(this.txBuild);
            this.Controls.Add(this.txDefault);
            this.Controls.Add(this.comboLanguage);
            this.Controls.Add(this.lbBuildTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ApplicationTitleDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Application Title";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbBuildTitle;
        private System.Windows.Forms.ComboBox comboLanguage;
        private System.Windows.Forms.TextBox txDefault;
        private System.Windows.Forms.TextBox txBuild;
        private System.Windows.Forms.Button btnPrjToBld;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnBldToPrj;
    }
}