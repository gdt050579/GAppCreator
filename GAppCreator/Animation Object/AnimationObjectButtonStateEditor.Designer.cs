namespace GAppCreator
{
    partial class AnimationObjectButtonStateEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationObjectButtonStateEditor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.mnuZoom = new System.Windows.Forms.ToolStripDropDownButton();
            this.zoom10 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom25 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom50 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom75 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom100 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom150 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom200 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom400 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.lbError = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbMode = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.panel2 = new System.Windows.Forms.Panel();
            this.previewPanel = new System.Windows.Forms.Panel();
            this.propButton = new System.Windows.Forms.PropertyGrid();
            this.mnuMarginColor = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 737);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1260, 89);
            this.panel1.TabIndex = 8;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(1119, 25);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 46);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(998, 25);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 46);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton5,
            this.mnuMarginColor,
            this.mnuZoom,
            this.toolStripSeparator3,
            this.lbError,
            this.toolStripSeparator1,
            this.lbMode,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1260, 56);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(159, 53);
            this.toolStripButton5.Text = "Background Color";
            this.toolStripButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton5.Click += new System.EventHandler(this.OnSetBackgroundColor);
            // 
            // mnuZoom
            // 
            this.mnuZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoom10,
            this.zoom25,
            this.zoom50,
            this.zoom75,
            this.zoom100,
            this.zoom150,
            this.zoom200,
            this.zoom400});
            this.mnuZoom.Image = ((System.Drawing.Image)(resources.GetObject("mnuZoom.Image")));
            this.mnuZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuZoom.Name = "mnuZoom";
            this.mnuZoom.Size = new System.Drawing.Size(78, 53);
            this.mnuZoom.Text = "Zoom";
            this.mnuZoom.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // zoom10
            // 
            this.zoom10.Name = "zoom10";
            this.zoom10.Size = new System.Drawing.Size(252, 30);
            this.zoom10.Text = "10%";
            this.zoom10.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // zoom25
            // 
            this.zoom25.Name = "zoom25";
            this.zoom25.Size = new System.Drawing.Size(252, 30);
            this.zoom25.Text = "25%";
            this.zoom25.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // zoom50
            // 
            this.zoom50.Name = "zoom50";
            this.zoom50.Size = new System.Drawing.Size(252, 30);
            this.zoom50.Text = "50%";
            this.zoom50.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // zoom75
            // 
            this.zoom75.Name = "zoom75";
            this.zoom75.Size = new System.Drawing.Size(252, 30);
            this.zoom75.Text = "75%";
            this.zoom75.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // zoom100
            // 
            this.zoom100.Name = "zoom100";
            this.zoom100.Size = new System.Drawing.Size(252, 30);
            this.zoom100.Text = "100%";
            this.zoom100.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // zoom150
            // 
            this.zoom150.Name = "zoom150";
            this.zoom150.Size = new System.Drawing.Size(252, 30);
            this.zoom150.Text = "150%";
            this.zoom150.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // zoom200
            // 
            this.zoom200.Name = "zoom200";
            this.zoom200.Size = new System.Drawing.Size(252, 30);
            this.zoom200.Text = "200%";
            this.zoom200.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // zoom400
            // 
            this.zoom400.Name = "zoom400";
            this.zoom400.Size = new System.Drawing.Size(252, 30);
            this.zoom400.Text = "400%";
            this.zoom400.Click += new System.EventHandler(this.OnChangeZoom);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 56);
            // 
            // lbError
            // 
            this.lbError.AutoSize = false;
            this.lbError.Name = "lbError";
            this.lbError.Size = new System.Drawing.Size(400, 53);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 56);
            // 
            // lbMode
            // 
            this.lbMode.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lbMode.AutoSize = false;
            this.lbMode.Name = "lbMode";
            this.lbMode.Size = new System.Drawing.Size(100, 53);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 56);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.previewPanel);
            this.panel2.Controls.Add(this.propButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1260, 681);
            this.panel2.TabIndex = 10;
            // 
            // previewPanel
            // 
            this.previewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewPanel.Location = new System.Drawing.Point(622, 0);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(636, 679);
            this.previewPanel.TabIndex = 1;
            this.previewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintPreview);
            // 
            // propButton
            // 
            this.propButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.propButton.Location = new System.Drawing.Point(0, 0);
            this.propButton.Name = "propButton";
            this.propButton.Size = new System.Drawing.Size(622, 679);
            this.propButton.TabIndex = 0;
            this.propButton.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propButton_PropertyValueChanged);
            // 
            // mnuMarginColor
            // 
            this.mnuMarginColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.mnuMarginColor.Image = ((System.Drawing.Image)(resources.GetObject("mnuMarginColor.Image")));
            this.mnuMarginColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuMarginColor.Name = "mnuMarginColor";
            this.mnuMarginColor.Size = new System.Drawing.Size(143, 53);
            this.mnuMarginColor.Text = "Show Margins";
            this.mnuMarginColor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Checked = true;
            this.toolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(252, 30);
            this.toolStripMenuItem1.Text = "No color";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.OnChangeMarginColor);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(252, 30);
            this.toolStripMenuItem2.Text = "Black";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.OnChangeMarginColor);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(252, 30);
            this.toolStripMenuItem3.Text = "White";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.OnChangeMarginColor);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(252, 30);
            this.toolStripMenuItem4.Text = "Red";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.OnChangeMarginColor);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(252, 30);
            this.toolStripMenuItem5.Text = "Yellow";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.OnChangeMarginColor);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(252, 30);
            this.toolStripMenuItem6.Text = "Green";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.OnChangeMarginColor);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(252, 30);
            this.toolStripMenuItem7.Text = "Gray";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.OnChangeMarginColor);
            // 
            // AnimationObjectButtonStateEditor
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1260, 826);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AnimationObjectButtonStateEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Button state editor";
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel previewPanel;
        private System.Windows.Forms.PropertyGrid propButton;
        private System.Windows.Forms.ToolStripLabel lbError;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lbMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton mnuZoom;
        private System.Windows.Forms.ToolStripMenuItem zoom10;
        private System.Windows.Forms.ToolStripMenuItem zoom25;
        private System.Windows.Forms.ToolStripMenuItem zoom50;
        private System.Windows.Forms.ToolStripMenuItem zoom75;
        private System.Windows.Forms.ToolStripMenuItem zoom100;
        private System.Windows.Forms.ToolStripMenuItem zoom150;
        private System.Windows.Forms.ToolStripMenuItem zoom200;
        private System.Windows.Forms.ToolStripMenuItem zoom400;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton mnuMarginColor;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
    }
}