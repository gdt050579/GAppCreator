namespace GAppCreator
{
    partial class PresentationFrameViewer
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
            this.SuspendLayout();
            // 
            // AnimationFrameViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "PresentationFrameViewer";
            this.Size = new System.Drawing.Size(84, 34);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PresentationFrameViewer_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AnimationFrameViewer_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PresentationFrameViewer_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PresentationFrameViewer_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PresentationFrameViewer_MouseUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.AnimationFrameViewer_PreviewKeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
