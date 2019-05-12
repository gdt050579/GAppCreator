using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace GAppCreator
{
    public class GBitmap
    {
        public string FileName;
        public string Name;
        private float ResolutionFactor;
        private GraphicContext context;
        private byte[] FileImageBuffer;
        public Bitmap __internal__image;
        public int Width, Height;
        public bool UseForAnimation;

        public GBitmap()
        {
            FileName = "";
            Name = "";
            ResolutionFactor = 1.0f;
            __internal__image = null;
            Width = Height = 0;
            UseForAnimation = false;
            context = null;
        }
        public GBitmap(string fileName, float resolutionFactor,bool useForAnimation,string name)
        {
            __internal__image = null;
            FileName = fileName;
            Name = name;
            ResolutionFactor = resolutionFactor;
            __internal__image = null;
            Width = Height = 0;
            UseForAnimation = useForAnimation;
            context = null;
            try
            {
                FileName = System.IO.Path.GetFullPath(fileName);
            }
            catch (Exception e)
            {
                FileName = "";
                Console.WriteLine("Invalid path: " + fileName);
                Console.WriteLine(e.ToString());
            }
        }
        public GBitmap(byte []fileBuffer, float resolutionFactor, bool useForAnimation, string name)
        {
            __internal__image = null;
            FileName = "";
            Name = name;
            ResolutionFactor = resolutionFactor;
            __internal__image = null;
            Width = Height = 0;
            UseForAnimation = useForAnimation;
            context = null;
            FileImageBuffer = fileBuffer;
        }
        public bool InitFromBitmap(Bitmap bmp)
        {
            // incarc efectiv 
            if (bmp == null)
                return false;
            if (ResolutionFactor == 1.0f)
            {
                __internal__image = bmp;
            }
            else
            {
                int newWidth = (int)(bmp.Width * ResolutionFactor);
                int newHeight = (int)(bmp.Height * ResolutionFactor);
                __internal__image = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
                if (__internal__image == null)
                    return false;
                Graphics g = Graphics.FromImage((Image)__internal__image);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.ScaleTransform((float)ResolutionFactor, (float)ResolutionFactor);
                Rectangle rc = new Rectangle(0, 0, bmp.Size.Width, bmp.Size.Height);
                g.DrawImage(bmp, rc, rc, GraphicsUnit.Pixel);
                g.Dispose();
                bmp.Dispose();
                bmp = null;
            }
            return true;
        }
        public bool Load()
        {
            if (__internal__image != null)
                return true;
            if (((FileName == null) || (FileName.Length == 0)) && (FileImageBuffer==null))
                return false;
            try
            {
                if (FileImageBuffer != null)
                {
                    if (InitFromBitmap((Bitmap)Image.FromStream(new System.IO.MemoryStream(FileImageBuffer))) == false)
                        return false;
                }
                else
                {
                    if (InitFromBitmap((Bitmap)Project.LoadImage(FileName)) == false)
                        return false;
                }
                Width = __internal__image.Width;
                Height = __internal__image.Height;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to load: " + FileName);
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public void Dispose()
        {
            if (__internal__image != null)
            {
                if (context != null)
                {
                    context.g.Dispose();
                    context = null;
                }
                __internal__image.Dispose();
                Width = Height = 0;
                __internal__image = null;
            }
        }
        public bool Create(int width, int height)
        {
            if (__internal__image != null)
                return false;
            __internal__image = new Bitmap(width, height);
            if (__internal__image == null)
                return false;
            Width = __internal__image.Width;
            Height = __internal__image.Height;
            ResolutionFactor = 1.0f;
            Name = FileName = "";
            return true;
        }
        public GraphicContext GetGraphicContext()
        {
            if (__internal__image == null)
                return null;
            if (context != null)
                return context;
            context = new GraphicContext();
            context.g = Graphics.FromImage(__internal__image);
            context.Width = Width;
            context.Height = Height;
            return context;
        }
        public void DisposeGraphicContext()
        {
            if (context != null)
            {
                context.g.Dispose();
                context = null;
            }
        }
        public int GetPixel(int x, int y)
        {
            if ((x < 0) || (x >= Width) || (y < 0) || (y >= Height) || (__internal__image == null))
                return 0;
            return __internal__image.GetPixel(x, y).ToArgb();
        }
        public bool SetPixel(int x, int y, int color)
        {
            if ((x < 0) || (x >= Width) || (y < 0) || (y >= Height) || (__internal__image == null))
                return false;
            __internal__image.SetPixel(x, y, System.Drawing.Color.FromArgb(color));
            return true;
        }
    }
}
