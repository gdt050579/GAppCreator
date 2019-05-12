namespace GAppCreator
{
    partial class PresentationPlayerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PresentationPlayerDialog));
            this.panelCommands = new System.Windows.Forms.ToolStrip();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.cbLoop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPreviousFrame = new System.Windows.Forms.ToolStripButton();
            this.txCurrentFrame = new System.Windows.Forms.ToolStripTextBox();
            this.lbTotalFrames = new System.Windows.Forms.ToolStripLabel();
            this.btnNextFrame = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lbInterval = new System.Windows.Forms.ToolStripLabel();
            this.txInterval = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.comboLanguage = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.lbTotalTime = new System.Windows.Forms.ToolStripLabel();
            this.animTimer = new System.Windows.Forms.Timer(this.components);
            this.panelCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCommands
            // 
            this.panelCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPlay,
            this.btnStop,
            this.cbLoop,
            this.toolStripSeparator1,
            this.btnPreviousFrame,
            this.txCurrentFrame,
            this.lbTotalFrames,
            this.btnNextFrame,
            this.toolStripSeparator2,
            this.lbInterval,
            this.txInterval,
            this.toolStripSeparator3,
            this.comboLanguage,
            this.toolStripSeparator4,
            this.lbTotalTime});
            this.panelCommands.Location = new System.Drawing.Point(0, 0);
            this.panelCommands.Name = "panelCommands";
            this.panelCommands.Size = new System.Drawing.Size(577, 25);
            this.panelCommands.TabIndex = 0;
            this.panelCommands.Text = "toolStrip1";
            // 
            // btnPlay
            // 
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(23, 22);
            this.btnPlay.ToolTipText = "Play";
            this.btnPlay.Click += new System.EventHandler(this.OnPlay);
            // 
            // btnStop
            // 
            this.btnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(23, 22);
            this.btnStop.Text = "toolStripButton2";
            this.btnStop.ToolTipText = "Stop";
            this.btnStop.Click += new System.EventHandler(this.OnStop);
            // 
            // cbLoop
            // 
            this.cbLoop.CheckOnClick = true;
            this.cbLoop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cbLoop.Image = ((System.Drawing.Image)(resources.GetObject("cbLoop.Image")));
            this.cbLoop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbLoop.Name = "cbLoop";
            this.cbLoop.Size = new System.Drawing.Size(23, 22);
            this.cbLoop.Text = "toolStripButton1";
            this.cbLoop.ToolTipText = "Loop this animation";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnPreviousFrame
            // 
            this.btnPreviousFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPreviousFrame.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousFrame.Image")));
            this.btnPreviousFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousFrame.Name = "btnPreviousFrame";
            this.btnPreviousFrame.Size = new System.Drawing.Size(23, 22);
            this.btnPreviousFrame.Text = "toolStripButton3";
            this.btnPreviousFrame.ToolTipText = "Previous Frame";
            this.btnPreviousFrame.Click += new System.EventHandler(this.OnPreviousFrame);
            // 
            // txCurrentFrame
            // 
            this.txCurrentFrame.AutoSize = false;
            this.txCurrentFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txCurrentFrame.Name = "txCurrentFrame";
            this.txCurrentFrame.Size = new System.Drawing.Size(40, 23);
            this.txCurrentFrame.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txCurrentFrame_KeyUp);
            // 
            // lbTotalFrames
            // 
            this.lbTotalFrames.Name = "lbTotalFrames";
            this.lbTotalFrames.Size = new System.Drawing.Size(30, 22);
            this.lbTotalFrames.Text = "/400";
            // 
            // btnNextFrame
            // 
            this.btnNextFrame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNextFrame.Image = ((System.Drawing.Image)(resources.GetObject("btnNextFrame.Image")));
            this.btnNextFrame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(23, 22);
            this.btnNextFrame.Text = "toolStripButton4";
            this.btnNextFrame.ToolTipText = "Next Frame";
            this.btnNextFrame.Click += new System.EventHandler(this.OnNextFrame);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // lbInterval
            // 
            this.lbInterval.Name = "lbInterval";
            this.lbInterval.Size = new System.Drawing.Size(73, 22);
            this.lbInterval.Text = "Interval (ms)";
            // 
            // txInterval
            // 
            this.txInterval.AutoSize = false;
            this.txInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txInterval.Name = "txInterval";
            this.txInterval.Size = new System.Drawing.Size(30, 23);
            this.txInterval.Text = "30";
            this.txInterval.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txInterval_KeyUp);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // comboLanguage
            // 
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.Name = "comboLanguage";
            this.comboLanguage.Size = new System.Drawing.Size(80, 25);
            this.comboLanguage.SelectedIndexChanged += new System.EventHandler(this.OnChangeLanguage);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // lbTotalTime
            // 
            this.lbTotalTime.AutoSize = false;
            this.lbTotalTime.Name = "lbTotalTime";
            this.lbTotalTime.Size = new System.Drawing.Size(80, 22);
            this.lbTotalTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // animTimer
            // 
            this.animTimer.Interval = 30;
            this.animTimer.Tick += new System.EventHandler(this.OnTimer);
            // 
            // PresentationPlayerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 509);
            this.Controls.Add(this.panelCommands);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PresentationPlayerDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Presentation Player Dialog";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AnimationPlayerDialog_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AnimationPlayerDialog_MouseMove);
            this.panelCommands.ResumeLayout(false);
            this.panelCommands.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip panelCommands;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnPreviousFrame;
        private System.Windows.Forms.ToolStripTextBox txCurrentFrame;
        private System.Windows.Forms.ToolStripLabel lbTotalFrames;
        private System.Windows.Forms.ToolStripButton btnNextFrame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel lbInterval;
        private System.Windows.Forms.ToolStripTextBox txInterval;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Timer animTimer;
        private System.Windows.Forms.ToolStripComboBox comboLanguage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel lbTotalTime;
        private System.Windows.Forms.ToolStripButton cbLoop;
    }
}