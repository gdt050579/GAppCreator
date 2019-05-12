using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GAppCreator
{
    public class PreviewFont: PreviewControl
    {
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private ImageList glyphIcons;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ListView lstCharacters;

        public PreviewFont()
        {
            InitializeComponent();
        }


        public override void OnNewPreviewObject()
        {
            FontResource tempFont = (FontResource)SelectedObject;
            glyphIcons.Images.Clear();
            lstCharacters.Items.Clear();
            foreach (Glyph g in tempFont.Glyphs)
            {
                if (g.Picture != null)
                {
                    glyphIcons.Images.Add(g.Code.ToString(), Project.ImageToIcon(prj.EC, (System.Drawing.Bitmap)g.Picture, 16, 16));
                }
            }
            foreach (Glyph g in tempFont.Glyphs)
            {

                ListViewItem lvi = new ListViewItem("");

                lvi.SubItems.Add("" + (char)g.Code);
                lvi.SubItems.Add(g.Code.ToString());
                lvi.SubItems.Add("0x" + g.Code.ToString("X4"));
                lvi.ImageKey = g.Code.ToString();
                lvi.SubItems.Add(Project.ProcentToString(g.BaseLine));
                lstCharacters.Items.Add(lvi);
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lstCharacters = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.glyphIcons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // lstCharacters
            // 
            this.lstCharacters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lstCharacters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCharacters.FullRowSelect = true;
            this.lstCharacters.GridLines = true;
            this.lstCharacters.Location = new System.Drawing.Point(0, 0);
            this.lstCharacters.Name = "lstCharacters";
            this.lstCharacters.Size = new System.Drawing.Size(356, 296);
            this.lstCharacters.SmallImageList = this.glyphIcons;
            this.lstCharacters.TabIndex = 0;
            this.lstCharacters.UseCompatibleStateImageBehavior = false;
            this.lstCharacters.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "-";
            this.columnHeader1.Width = 40;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Char";
            this.columnHeader4.Width = 40;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Dec";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 40;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Hex";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "BaseLine";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // glyphIcons
            // 
            this.glyphIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.glyphIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.glyphIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // PreviewFont
            // 
            this.Controls.Add(this.lstCharacters);
            this.Name = "PreviewFont";
            this.Size = new System.Drawing.Size(356, 296);
            this.ResumeLayout(false);

        }
    }
}
