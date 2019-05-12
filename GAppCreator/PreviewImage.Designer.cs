namespace GAppCreator
{
    partial class PreviewImage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewImage));
            this.pnlPreviewImage = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cb800 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb500 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb400 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb300 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb200 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb100 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb50 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb25 = new System.Windows.Forms.ToolStripMenuItem();
            this.cb10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cbAuto = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbResolution = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cbBlack = new System.Windows.Forms.ToolStripButton();
            this.cbWhite = new System.Windows.Forms.ToolStripButton();
            this.cbGray = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbShowMargin = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cbShowPixels = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowPercentages = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlScroll = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.lbPosition = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.lbSelection = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.pnlScroll.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPreviewImage
            // 
            this.pnlPreviewImage.AutoScroll = true;
            this.pnlPreviewImage.Location = new System.Drawing.Point(38, 65);
            this.pnlPreviewImage.Name = "pnlPreviewImage";
            this.pnlPreviewImage.Size = new System.Drawing.Size(122, 157);
            this.pnlPreviewImage.TabIndex = 1;
            this.pnlPreviewImage.Paint += new System.Windows.Forms.PaintEventHandler(this.OnDrawImage);
            this.pnlPreviewImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.pnlPreviewImage.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.pnlPreviewImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.pnlPreviewImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.lbResolution,
            this.toolStripSeparator2,
            this.cbBlack,
            this.cbWhite,
            this.cbGray,
            this.toolStripButton2,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(467, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cb800,
            this.cb500,
            this.cb400,
            this.cb300,
            this.cb200,
            this.cb100,
            this.cb50,
            this.cb25,
            this.cb10,
            this.toolStripMenuItem1,
            this.cbAuto});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(52, 35);
            this.toolStripButton1.Text = "Zoom";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cb800
            // 
            this.cb800.Name = "cb800";
            this.cb800.Size = new System.Drawing.Size(116, 22);
            this.cb800.Text = "800 %";
            this.cb800.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb500
            // 
            this.cb500.Name = "cb500";
            this.cb500.Size = new System.Drawing.Size(116, 22);
            this.cb500.Text = "500 %";
            this.cb500.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb400
            // 
            this.cb400.Name = "cb400";
            this.cb400.Size = new System.Drawing.Size(116, 22);
            this.cb400.Text = "400 %";
            this.cb400.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb300
            // 
            this.cb300.Name = "cb300";
            this.cb300.Size = new System.Drawing.Size(116, 22);
            this.cb300.Text = "300 %";
            this.cb300.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb200
            // 
            this.cb200.Name = "cb200";
            this.cb200.Size = new System.Drawing.Size(116, 22);
            this.cb200.Text = "200 %";
            this.cb200.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb100
            // 
            this.cb100.Name = "cb100";
            this.cb100.Size = new System.Drawing.Size(116, 22);
            this.cb100.Text = "100 %";
            this.cb100.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb50
            // 
            this.cb50.Name = "cb50";
            this.cb50.Size = new System.Drawing.Size(116, 22);
            this.cb50.Text = "50 %";
            this.cb50.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb25
            // 
            this.cb25.Name = "cb25";
            this.cb25.Size = new System.Drawing.Size(116, 22);
            this.cb25.Text = "25 %";
            this.cb25.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cb10
            // 
            this.cb10.Name = "cb10";
            this.cb10.Size = new System.Drawing.Size(116, 22);
            this.cb10.Text = "10 %";
            this.cb10.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(113, 6);
            this.toolStripMenuItem1.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // cbAuto
            // 
            this.cbAuto.Checked = true;
            this.cbAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAuto.Name = "cbAuto";
            this.cbAuto.Size = new System.Drawing.Size(116, 22);
            this.cbAuto.Text = "Auto Fit";
            this.cbAuto.Click += new System.EventHandler(this.OnChangeScale);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // lbResolution
            // 
            this.lbResolution.AutoSize = false;
            this.lbResolution.Name = "lbResolution";
            this.lbResolution.Size = new System.Drawing.Size(70, 30);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // cbBlack
            // 
            this.cbBlack.Image = ((System.Drawing.Image)(resources.GetObject("cbBlack.Image")));
            this.cbBlack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbBlack.Name = "cbBlack";
            this.cbBlack.Size = new System.Drawing.Size(39, 35);
            this.cbBlack.Text = "Black";
            this.cbBlack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbBlack.Click += new System.EventHandler(this.OnChangeBackColor);
            // 
            // cbWhite
            // 
            this.cbWhite.Image = ((System.Drawing.Image)(resources.GetObject("cbWhite.Image")));
            this.cbWhite.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbWhite.Name = "cbWhite";
            this.cbWhite.Size = new System.Drawing.Size(42, 35);
            this.cbWhite.Text = "White";
            this.cbWhite.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbWhite.Visible = false;
            this.cbWhite.Click += new System.EventHandler(this.OnChangeBackColor);
            // 
            // cbGray
            // 
            this.cbGray.Image = ((System.Drawing.Image)(resources.GetObject("cbGray.Image")));
            this.cbGray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbGray.Name = "cbGray";
            this.cbGray.Size = new System.Drawing.Size(35, 35);
            this.cbGray.Text = "Gray";
            this.cbGray.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbGray.Visible = false;
            this.cbGray.Click += new System.EventHandler(this.OnChangeBackColor);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(39, 35);
            this.toolStripButton2.Text = "Copy";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.Click += new System.EventHandler(this.OnCopyImage);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbShowMargin,
            this.toolStripMenuItem2,
            this.cbShowPixels,
            this.cbShowPercentages});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(49, 35);
            this.toolStripDropDownButton1.Text = "Tools";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbShowMargin
            // 
            this.cbShowMargin.CheckOnClick = true;
            this.cbShowMargin.Name = "cbShowMargin";
            this.cbShowMargin.Size = new System.Drawing.Size(229, 22);
            this.cbShowMargin.Text = "Show margin";
            this.cbShowMargin.Click += new System.EventHandler(this.OnChangeMarginVizibility);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(226, 6);
            // 
            // cbShowPixels
            // 
            this.cbShowPixels.Name = "cbShowPixels";
            this.cbShowPixels.Size = new System.Drawing.Size(229, 22);
            this.cbShowPixels.Text = "Show position in pixels";
            this.cbShowPixels.Click += new System.EventHandler(this.OnChangeMetrics);
            // 
            // cbShowPercentages
            // 
            this.cbShowPercentages.Checked = true;
            this.cbShowPercentages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPercentages.Name = "cbShowPercentages";
            this.cbShowPercentages.Size = new System.Drawing.Size(229, 22);
            this.cbShowPercentages.Text = "Show position in percentages";
            this.cbShowPercentages.Click += new System.EventHandler(this.OnChangeMetrics);
            // 
            // pnlScroll
            // 
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlScroll.Controls.Add(this.pnlPreviewImage);
            this.pnlScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScroll.Location = new System.Drawing.Point(0, 38);
            this.pnlScroll.Name = "pnlScroll";
            this.pnlScroll.Size = new System.Drawing.Size(467, 278);
            this.pnlScroll.TabIndex = 2;
            this.pnlScroll.SizeChanged += new System.EventHandler(this.OnChangePreviewImageSize);
            this.pnlScroll.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintRestOfBackground);
            // 
            // toolStrip2
            // 
            this.toolStrip2.AutoSize = false;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbPosition,
            this.toolStripSeparator3,
            this.lbSelection,
            this.toolStripSeparator4});
            this.toolStrip2.Location = new System.Drawing.Point(0, 316);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(467, 36);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // lbPosition
            // 
            this.lbPosition.AutoSize = false;
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(80, 33);
            this.lbPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 36);
            // 
            // lbSelection
            // 
            this.lbSelection.AutoSize = false;
            this.lbSelection.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbSelection.Name = "lbSelection";
            this.lbSelection.Size = new System.Drawing.Size(180, 33);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 36);
            // 
            // PreviewImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlScroll);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Name = "PreviewImage";
            this.Size = new System.Drawing.Size(467, 352);
            this.Enter += new System.EventHandler(this.PreviewImage_Enter);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlScroll.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlPreviewImage;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel pnlScroll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lbResolution;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem cb800;
        private System.Windows.Forms.ToolStripMenuItem cb500;
        private System.Windows.Forms.ToolStripMenuItem cb400;
        private System.Windows.Forms.ToolStripMenuItem cb300;
        private System.Windows.Forms.ToolStripMenuItem cb200;
        private System.Windows.Forms.ToolStripMenuItem cb100;
        private System.Windows.Forms.ToolStripMenuItem cb50;
        private System.Windows.Forms.ToolStripMenuItem cb25;
        private System.Windows.Forms.ToolStripMenuItem cb10;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cbAuto;
        private System.Windows.Forms.ToolStripButton cbBlack;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem cbShowMargin;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel lbPosition;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cbShowPixels;
        private System.Windows.Forms.ToolStripMenuItem cbShowPercentages;
        private System.Windows.Forms.ToolStripLabel lbSelection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton cbWhite;
        private System.Windows.Forms.ToolStripButton cbGray;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}
