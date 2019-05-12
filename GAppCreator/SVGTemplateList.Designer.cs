namespace GAppCreator
{
    partial class SVGTemplateList
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
            this.ListTemplates = new System.Windows.Forms.ListView();
            this.TemplateImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // ListTemplates
            // 
            this.ListTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListTemplates.FullRowSelect = true;
            this.ListTemplates.HideSelection = false;
            this.ListTemplates.LargeImageList = this.TemplateImages;
            this.ListTemplates.Location = new System.Drawing.Point(0, 0);
            this.ListTemplates.MultiSelect = false;
            this.ListTemplates.Name = "ListTemplates";
            this.ListTemplates.Size = new System.Drawing.Size(373, 291);
            this.ListTemplates.TabIndex = 0;
            this.ListTemplates.TileSize = new System.Drawing.Size(168, 70);
            this.ListTemplates.UseCompatibleStateImageBehavior = false;
            this.ListTemplates.SelectedIndexChanged += new System.EventHandler(this.OnSelectedItemChanged);
            this.ListTemplates.DoubleClick += new System.EventHandler(this.OnDblClicked);
            // 
            // TemplateImages
            // 
            this.TemplateImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.TemplateImages.ImageSize = new System.Drawing.Size(64, 64);
            this.TemplateImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SVGTemplateList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ListTemplates);
            this.Name = "SVGTemplateList";
            this.Size = new System.Drawing.Size(373, 291);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ListTemplates;
        private System.Windows.Forms.ImageList TemplateImages;
    }
}
