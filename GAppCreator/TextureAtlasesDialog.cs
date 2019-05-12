using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class TextureAtlasesDialog : Form
    {
        class ImagePosition
        {
            public int Left, Top;
            public Bitmap bmp;
            public int TextureID;
            public bool Added;
            public void Reset()
            {
                Left = Top = 0;
                TextureID = 0;
                Added = false;
            }
            public ImagePosition()
            {
                Reset();
            }
        };        
        ImagePosition[] images;        
        
        public TextureAtlasesDialog(Profile profile,Project p, List<Bitmap> _images)
        {            
            InitializeComponent();

            _images.Sort(delegate(Bitmap b1, Bitmap b2) { return (b1.Width * b1.Height).CompareTo(b2.Width * b2.Height); });
            images = new ImagePosition[_images.Count];
            for (int tr=0;tr<images.Length;tr++)
            {
                images[tr] = new ImagePosition();
                images[tr].bmp = _images[tr];
            }
            comboTextureSize.SelectedIndex = 0;
            
        }
        int Power2Biggest(int value)
        {
	        for (int tr=0;tr<30;tr++)
		        if ((((int)1)<<tr)>=value)
			        return 1<<tr;
	        return -1;
        }
        void ComputeMaxWidthAndMaxHeight(ref int maxWidth,ref int maxHeight)
        {
            maxWidth = maxHeight = 0;
            for (int tr = 0; tr < images.Length; tr++)
            {
                if (images[tr].Added)
                    continue;
                if (images[tr].bmp.Width > maxWidth)
                    maxWidth = images[tr].bmp.Width;
                if (images[tr].bmp.Height > maxHeight)
                    maxHeight = images[tr].bmp.Height;
            }
        }
    }
}
