namespace GAppCreator
{
    partial class ProjectTabAds
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectTabAds));
            this.toolStrip6 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton15 = new System.Windows.Forms.ToolStripDropDownButton();
            this.googleAdMobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bannerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interstitialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rewardableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nativeExpressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartboostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interstitialToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rewardableToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton16 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.pnlAds = new System.Windows.Forms.SplitContainer();
            this.propAds = new System.Windows.Forms.PropertyGrid();
            this.inPlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlAds)).BeginInit();
            this.pnlAds.Panel1.SuspendLayout();
            this.pnlAds.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip6
            // 
            this.toolStrip6.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton15,
            this.toolStripButton16,
            this.toolStripButton1,
            this.toolStripSeparator12});
            this.toolStrip6.Location = new System.Drawing.Point(0, 0);
            this.toolStrip6.Name = "toolStrip6";
            this.toolStrip6.Size = new System.Drawing.Size(1013, 38);
            this.toolStrip6.TabIndex = 1;
            this.toolStrip6.Text = "toolStrip6";
            // 
            // toolStripButton15
            // 
            this.toolStripButton15.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.googleAdMobToolStripMenuItem,
            this.chartboostToolStripMenuItem});
            this.toolStripButton15.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton15.Image")));
            this.toolStripButton15.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton15.Name = "toolStripButton15";
            this.toolStripButton15.Size = new System.Drawing.Size(42, 35);
            this.toolStripButton15.Text = "Add";
            this.toolStripButton15.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // googleAdMobToolStripMenuItem
            // 
            this.googleAdMobToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bannerToolStripMenuItem,
            this.interstitialToolStripMenuItem,
            this.rewardableToolStripMenuItem,
            this.nativeExpressToolStripMenuItem});
            this.googleAdMobToolStripMenuItem.Name = "googleAdMobToolStripMenuItem";
            this.googleAdMobToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.googleAdMobToolStripMenuItem.Text = "Google AdMob";
            // 
            // bannerToolStripMenuItem
            // 
            this.bannerToolStripMenuItem.Name = "bannerToolStripMenuItem";
            this.bannerToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.bannerToolStripMenuItem.Text = "Banner";
            this.bannerToolStripMenuItem.Click += new System.EventHandler(this.OnAddGoogleAdMobBanner);
            // 
            // interstitialToolStripMenuItem
            // 
            this.interstitialToolStripMenuItem.Name = "interstitialToolStripMenuItem";
            this.interstitialToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.interstitialToolStripMenuItem.Text = "Interstitial";
            this.interstitialToolStripMenuItem.Click += new System.EventHandler(this.OnAddGoogleAdMobInterstitial);
            // 
            // rewardableToolStripMenuItem
            // 
            this.rewardableToolStripMenuItem.Name = "rewardableToolStripMenuItem";
            this.rewardableToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.rewardableToolStripMenuItem.Text = "Rewardable";
            this.rewardableToolStripMenuItem.Click += new System.EventHandler(this.OnAddGoogleAdMobRewardable);
            // 
            // nativeExpressToolStripMenuItem
            // 
            this.nativeExpressToolStripMenuItem.Name = "nativeExpressToolStripMenuItem";
            this.nativeExpressToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.nativeExpressToolStripMenuItem.Text = "Native Express";
            this.nativeExpressToolStripMenuItem.Click += new System.EventHandler(this.OnAddGoogleAdMobNativeExpress);
            // 
            // chartboostToolStripMenuItem
            // 
            this.chartboostToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.interstitialToolStripMenuItem1,
            this.rewardableToolStripMenuItem1,
            this.inPlayToolStripMenuItem});
            this.chartboostToolStripMenuItem.Name = "chartboostToolStripMenuItem";
            this.chartboostToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.chartboostToolStripMenuItem.Text = "Chartboost";
            // 
            // interstitialToolStripMenuItem1
            // 
            this.interstitialToolStripMenuItem1.Name = "interstitialToolStripMenuItem1";
            this.interstitialToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.interstitialToolStripMenuItem1.Text = "Interstitial";
            this.interstitialToolStripMenuItem1.Click += new System.EventHandler(this.OnAddChartboostInterstitial);
            // 
            // rewardableToolStripMenuItem1
            // 
            this.rewardableToolStripMenuItem1.Name = "rewardableToolStripMenuItem1";
            this.rewardableToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.rewardableToolStripMenuItem1.Text = "Rewardable";
            this.rewardableToolStripMenuItem1.Click += new System.EventHandler(this.OnAddChartboostRewardable);
            // 
            // toolStripButton16
            // 
            this.toolStripButton16.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton16.Image")));
            this.toolStripButton16.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton16.Name = "toolStripButton16";
            this.toolStripButton16.Size = new System.Drawing.Size(44, 35);
            this.toolStripButton16.Text = "Delete";
            this.toolStripButton16.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton16.Click += new System.EventHandler(this.OnDeleteAd);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(61, 35);
            this.toolStripButton1.Text = "Duplicate";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.OnDuplicateAd);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 38);
            // 
            // pnlAds
            // 
            this.pnlAds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAds.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.pnlAds.Location = new System.Drawing.Point(0, 38);
            this.pnlAds.Name = "pnlAds";
            // 
            // pnlAds.Panel1
            // 
            this.pnlAds.Panel1.Controls.Add(this.propAds);
            this.pnlAds.Size = new System.Drawing.Size(1013, 561);
            this.pnlAds.SplitterDistance = 333;
            this.pnlAds.TabIndex = 2;
            // 
            // propAds
            // 
            this.propAds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propAds.Location = new System.Drawing.Point(0, 0);
            this.propAds.Name = "propAds";
            this.propAds.Size = new System.Drawing.Size(331, 559);
            this.propAds.TabIndex = 1;
            this.propAds.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propAds_PropertyValueChanged);
            // 
            // inPlayToolStripMenuItem
            // 
            this.inPlayToolStripMenuItem.Name = "inPlayToolStripMenuItem";
            this.inPlayToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.inPlayToolStripMenuItem.Text = "InPlay";
            this.inPlayToolStripMenuItem.Click += new System.EventHandler(this.OnAddChartboostInPlay);
            // 
            // ProjectTabAds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlAds);
            this.Controls.Add(this.toolStrip6);
            this.Name = "ProjectTabAds";
            this.Size = new System.Drawing.Size(1013, 599);
            this.toolStrip6.ResumeLayout(false);
            this.toolStrip6.PerformLayout();
            this.pnlAds.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlAds)).EndInit();
            this.pnlAds.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip6;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton15;
        private System.Windows.Forms.ToolStripMenuItem googleAdMobToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton16;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.SplitContainer pnlAds;
        private System.Windows.Forms.PropertyGrid propAds;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem bannerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interstitialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rewardableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nativeExpressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chartboostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interstitialToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem rewardableToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem inPlayToolStripMenuItem;
    }
}
