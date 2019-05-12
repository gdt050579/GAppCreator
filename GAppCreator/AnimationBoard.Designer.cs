namespace GAppCreator
{
    partial class AnimationBoard
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
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.OnTimer);
            // 
            // AnimationBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "AnimationBoard";
            this.Size = new System.Drawing.Size(478, 394);
            this.ClientSizeChanged += new System.EventHandler(this.AnimationBoard_SizeChanged);
            this.SizeChanged += new System.EventHandler(this.AnimationBoard_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnimationBoard_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AnimationBoard_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AnimationBoard_MouseDown);
            this.MouseEnter += new System.EventHandler(this.AnimationBoard_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.AnimationBoard_MouseLeave);
            this.MouseHover += new System.EventHandler(this.AnimationBoard_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AnimationBoard_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AnimationBoard_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer;
    }
}
