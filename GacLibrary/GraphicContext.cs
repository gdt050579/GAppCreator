using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GAppCreator
{
    public class GraphicContext
    {
        protected internal Graphics g;
        
        private System.Drawing.Drawing2D.GraphicsState []ContextStateStack = new System.Drawing.Drawing2D.GraphicsState[32];
        private int ContextStateStackIndex = 0;
        private Rectangle rTemp = new Rectangle();
        //private RectangleF rfTemp = new RectangleF();
        private GRect grTemp = new GRect();
        private Pen gPen = new Pen(System.Drawing.Color.Black, 1);
        private Pen tempPen = new Pen(System.Drawing.Color.Black, 1);
        private SolidBrush gBrush = new SolidBrush(System.Drawing.Color.Black);
        private SolidBrush tempBrush = new SolidBrush(System.Drawing.Color.Black);
        private System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
        private String tempString = new String(' ', 2048);
        public int Width, Height;
        private GRect drawingBounds = new GRect();
        //private TextPainter tempTextMeasures = new TextPainter();// new TextPainter(null,"",0,0,Alignament.Center,1,1,Color.Black);

        


        #region Develop Only
        public void SetGraphics(Graphics gg)
        {
            g = gg;
        }
        #endregion

        #region Bitmap Functions
        private void g_DrawImage(GBitmap bmp, int x, int y, int w, int h)
        {
            g.DrawImage(bmp.__internal__image, x, y, w, h);

        }
        
        public void FillScreen(GBitmap bmp)
        {
            g_DrawImage(bmp, 0, 0, Width, Height);
        }
        public void FillScreen(GBitmap bmp, Alignament align,ImageResizeMode mode)
        {
            grTemp.Set(0, 0, Width, Height);
            DrawImage__(bmp, grTemp, align, mode);
        }
        public void DrawImage(GBitmap bmp, int x, int y)
        {
            g_DrawImage(bmp, x, y, bmp.Width, bmp.Height);
        }
        public void DrawImageCentered(GBitmap bmp, int x, int y)
        {
            g_DrawImage(bmp, x-bmp.Width/2, y-bmp.Height/2, bmp.Width, bmp.Height);
        }
        public void DrawImage(GBitmap bmp, int x, int y, int newWidth,int newHeight)
        {
            g_DrawImage(bmp, x, y, newWidth, newHeight);
        }
        public void DrawImage(GBitmap bmp, GRect destination)
        {
            g_DrawImage(bmp, destination.Left, destination.Top, destination.GetWidth(), destination.GetHeight());
        }
        public void DrawImage(GBitmap bmp, int x, int y, Alignament align)
        {
            switch (align)
            {
                case Alignament.BottomCenter:
                    g_DrawImage(bmp, x - bmp.Width / 2, y - bmp.Height,bmp.Width,bmp.Height);
                    break;
                case Alignament.BottomLeft:
                    g_DrawImage(bmp, x, y - bmp.Height, bmp.Width, bmp.Height);
                    break;
                case Alignament.BottomRight:
                    g_DrawImage(bmp, x - bmp.Width, y - bmp.Height,bmp.Width,bmp.Height);
                    break;
                case Alignament.Center:
                    g_DrawImage(bmp, x - bmp.Width / 2, y - bmp.Height/2,bmp.Width,bmp.Height);
                    break;
                case Alignament.LeftCenter:
                    g_DrawImage(bmp, x, y - bmp.Height/2,bmp.Width,bmp.Height);
                    break;
                case Alignament.RightCenter:
                    g_DrawImage(bmp, x - bmp.Width, y - bmp.Height/2,bmp.Width,bmp.Height);
                    break;
                case Alignament.TopCenter:
                    g_DrawImage(bmp, x - bmp.Width / 2, y,bmp.Width,bmp.Height);
                    break;
                case Alignament.TopLeft:
                    g_DrawImage(bmp, x, y,bmp.Width,bmp.Height);
                    break;
                case Alignament.TopRight:
                    g_DrawImage(bmp, x - bmp.Width, y,bmp.Width,bmp.Height);
                    break;
            }
        }
        public void DrawImage(GBitmap bmp, int x, int y, Alignament align, int width, int height)
        {
            switch (align)
            {
                case Alignament.BottomCenter:
                    g_DrawImage(bmp, x - width / 2, y - height, width, height);
                    break;
                case Alignament.BottomLeft:
                    g_DrawImage(bmp, x, y - height, width, height);
                    break;
                case Alignament.BottomRight:
                    g_DrawImage(bmp, x - width, y - height, width, height);
                    break;
                case Alignament.Center:
                    g_DrawImage(bmp, x - width / 2, y - height / 2, width, height);
                    break;
                case Alignament.LeftCenter:
                    g_DrawImage(bmp, x, y - height / 2, width, height);
                    break;
                case Alignament.RightCenter:
                    g_DrawImage(bmp, x - width, y - height / 2, width, height);
                    break;
                case Alignament.TopCenter:
                    g_DrawImage(bmp, x - width / 2, y, width, height);
                    break;
                case Alignament.TopLeft:
                    g_DrawImage(bmp, x, y, width, height);
                    break;
                case Alignament.TopRight:
                    g_DrawImage(bmp, x - width, y, width, height);
                    break;
            }
        }
        public void DrawImage(GBitmap bmp, int x, int y, Alignament align,float scale)
        {
            DrawImage(bmp, x, y, align, (int)(bmp.Width * scale), (int)(bmp.Height * scale));
        }
        public void DrawImage__(GBitmap bmp, GRect destination, Alignament align, ImageResizeMode mode)
        {
            int x = destination.GetAnchorX(align);
            int y = destination.GetAnchorY(align);
            float rw, rh;
            

            switch (mode)
            {
                //case ImageResizeMode.None:
                //    DrawImage(bmp, x, y, align, bmp.Width, bmp.Height);
                //    break;
                case ImageResizeMode.Fill:
                    DrawImage(bmp, x, y, align, destination.GetWidth(), destination.GetHeight());
                    break;
                case ImageResizeMode.Fit:
                    rw = ((float)destination.GetWidth()) / bmp.Width;
                    rh = ((float)destination.GetHeight()) / bmp.Height;
                    if (rw > rh)
                        rw = rh;
                    DrawImage(bmp, x, y, align, rw);
                    break;
                case ImageResizeMode.Shrink:
                    rw = ((float)destination.GetWidth()) / bmp.Width;
                    rh = ((float)destination.GetHeight()) / bmp.Height;
                    if ((rw >= 1) && (rh >= 1))
                    {
                        DrawImage(bmp, x, y, align, bmp.Width, bmp.Height);
                    } 
                    else
                    {
                        if (rw > rh)
                            rw = rh;
                        DrawImage(bmp, x, y, align, rw);
                    }
                    break;
                case ImageResizeMode.Dock:
                    int startX = 0, startY = 0;
                    int szX = bmp.Width;
                    int szY = bmp.Height;
                    switch (align)
                    {
                        case Alignament.TopLeft:
                            startX = destination.Left;
                            startY = destination.Top;
                            break;
                        case Alignament.TopCenter:
                            startX = destination.CenterX() - szX / 2;
                            startY = destination.Top;
                            break;
                        case Alignament.TopRight:
                            startX = destination.Right-szX;
                            startY = destination.Top;
                            break;
                        case Alignament.RightCenter:
                            startX = destination.Right-szX;
                            startY = destination.CenterY()-szY/2;
                            break;
                        case Alignament.BottomRight:
                            startX = destination.Right-szX;
                            startY = destination.Bottom-szY;
                            break;
                        case Alignament.BottomCenter:
                            startX = destination.CenterX() - szX / 2;
                            startY = destination.Bottom-szY;
                            break;
                        case Alignament.BottomLeft:
                            startX = destination.Left;
                            startY = destination.Bottom-szY;
                            break;
                        case Alignament.LeftCenter:
                            startX = destination.Left;
                            startY = destination.CenterY()-szY/2;
                            break;
                        case Alignament.Center:
                            startX = destination.CenterX()-szX/2;
                            startY = destination.CenterY()-szY/2;
                            break;
                    }
                    DrawImage(bmp, startX, startY);
                    break;
            }

        }
        public void DrawImage(GBitmap bmp, int left, int top, int right, int bottom, Alignament align, ImageResizeMode mode)
        {
            grTemp.Set(left, top, right, bottom);
            DrawImage__(bmp, grTemp, align, mode);
        }
        /*
        public void DrawImage(GBitmap bmp, float x, float y, Alignament align,CoordinateType coordType)
        {
            switch (coordType)
            {
                case CoordinateType.Percentage: DrawImage(bmp, (int)(x * Width), (int)(y * Height), align); break;
                case CoordinateType.DrawingBounds: DrawImage(bmp, (int)(x * drawingBounds.GetWidth() + drawingBounds.Left), (int)(y * drawingBounds.GetHeight() + drawingBounds.Top), align); break;
                default: DrawImage(bmp, (int)x, (int)y, align); break;
            }
        }
        public void DrawImage(GBitmap bmp, float x, float y, Alignament align, float scale, CoordinateType coordType)
        {
            switch (coordType)
            {
                case CoordinateType.Percentage: DrawImage(bmp, (int)(x * Width), (int)(y * Height), align, scale); break;
                case CoordinateType.DrawingBounds: DrawImage(bmp, (int)(x * drawingBounds.GetWidth() + drawingBounds.Left), (int)(y * drawingBounds.GetHeight() + drawingBounds.Top), align,scale); break;
                default: DrawImage(bmp, (int)x, (int)y, align,scale); break;
            }
        }
        */
        public void DrawImage(GBitmap bmp, int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int destLeft, int destTop, int destWidth, int destHeight)
        {
            rTemp.X = destLeft;
            rTemp.Y = destTop;
            rTemp.Width = destWidth;
            rTemp.Height = destHeight;
            g.DrawImage(bmp.__internal__image, rTemp, sourceLeft, sourceTop, sourceWidth, sourceHeight, GraphicsUnit.Pixel);
        }
        /*
        public void DrawImage(ImagePainter img)
        {
            if (img.Image != null)
            {
                img.Recompute();
                if (img.ImgResizeMode == ImageResizeMode.Tiles)
                {
                    DrawImage(img.Image, img.DrawingRect, img.layout.Align, ImageResizeMode.Tiles);
                }
                else
                {
                    if (img.RotateAngle != 0)
                    {
                        SaveState();
                        Rotate(img.RotateAngle, img.ImageRect.CenterX(),img.ImageRect.CenterY());
                        g_DrawImage(img.Image, img.ImageRect.Left, img.ImageRect.Top, img.ImageRect.GetWidth(), img.ImageRect.GetHeight());
                        RestoreState();
                    }
                    else
                    {
                        g_DrawImage(img.Image, img.ImageRect.Left, img.ImageRect.Top, img.ImageRect.GetWidth(), img.ImageRect.GetHeight());
                    }
                }
            }
        }
        public void DrawImageAndText(TextImagePainter tximg)
        {
            bool hasText;

            // dreptunghiul
            DrawRect(tximg.DrawRect, tximg.ColorMargin, tximg.ColorBack, tximg.MarginWidth);
            // background-ul
            if (tximg.Background != null)
                DrawImage(tximg.Background);
            // imaginea si textul
            hasText = (tximg.Text != null) && (tximg.Text.Text != null) && (tximg.Text.Text.Length > 0);

            // daca am si text si face
            if ((tximg.Face != null) && (tximg.Face.Image!=null) && (hasText) && (tximg.ImgTextRel != ImageTextRelations.None))
            {
                tximg.Text.Recompute(this);
                tximg.Face.Recompute();

                int sx = tximg.InternalDrawRect.CenterX() - (tximg.Text.rect.GetWidth() + tximg.ImageTextSpace + tximg.Face.ImageRect.GetWidth()) / 2;
                int sy = tximg.InternalDrawRect.CenterY() - (tximg.Text.rect.GetHeight() + tximg.ImageTextSpace + tximg.Face.ImageRect.GetHeight()) / 2;

                switch (tximg.ImgTextRel)
                {
                    case ImageTextRelations.ImageAboveText:
                        tximg.Face.SetPosition(tximg.InternalDrawRect.CenterX(), sy, Alignament.TopCenter, LayoutCoordinateType.Absolute);
                        DrawImage(tximg.Face);
                        sy += tximg.Face.ImageRect.GetHeight() + tximg.ImageTextSpace;
                        tximg.Text.SetLocation(tximg.InternalDrawRect.CenterX(), sy, Alignament.TopCenter);
                        DrawString(tximg.Text);
                        break;
                    case ImageTextRelations.ImageBeforeText:
                        tximg.Text.SetLocation(tximg.InternalDrawRect.CenterX(), sy, Alignament.TopCenter);
                        DrawString(tximg.Text);
                        sy += tximg.Text.rect.GetHeight() + tximg.ImageTextSpace;
                        tximg.Face.SetPosition(tximg.InternalDrawRect.CenterX(), sy, Alignament.TopCenter, LayoutCoordinateType.Absolute);
                        DrawImage(tximg.Face);
                        break;
                    case ImageTextRelations.ImageOnLeftOfTheText:
                        tximg.Face.SetPosition(sx, tximg.InternalDrawRect.CenterY(), Alignament.LeftCenter, LayoutCoordinateType.Absolute);
                        DrawImage(tximg.Face);
                        sx += tximg.Face.ImageRect.GetWidth() + tximg.ImageTextSpace;
                        tximg.Text.SetLocation(sx, tximg.InternalDrawRect.CenterY(), Alignament.LeftCenter);
                        DrawString(tximg.Text);
                        break;
                    case ImageTextRelations.ImageOnLeftOfTheTextWithWrap:
                        tximg.Face.SetPosition(0, tximg.InternalDrawRect.CenterY(), Alignament.LeftCenter, LayoutCoordinateType.Absolute);
                        DrawImage(tximg.Face);
                        tximg.Text.SetDrawingRect(tximg.Face.ImageRect.Right + tximg.ImageTextSpace,tximg.InternalDrawRect.Top,tximg.InternalDrawRect.Right,tximg.InternalDrawRect.Bottom);
                        tximg.Text.SetLocation(0, tximg.InternalDrawRect.CenterY(), Alignament.LeftCenter);
                        DrawString(tximg.Text);
                        break;
                    case ImageTextRelations.ImageOnRightOfTheText:
                        tximg.Text.SetLocation(sx, tximg.InternalDrawRect.CenterY(), Alignament.LeftCenter);
                        DrawString(tximg.Text);
                        sx += tximg.Text.rect.GetWidth() + tximg.ImageTextSpace;                        
                        tximg.Face.SetPosition(sx, tximg.InternalDrawRect.CenterY(), Alignament.LeftCenter, LayoutCoordinateType.Absolute);
                        DrawImage(tximg.Face);
                        break;
                    case ImageTextRelations.ImageOnRightOfTheTextWithWrap:
                        tximg.Face.SetPosition(tximg.InternalDrawRect.GetWidth(), tximg.InternalDrawRect.CenterY(), Alignament.RightCenter, LayoutCoordinateType.Absolute);
                        DrawImage(tximg.Face);
                        tximg.Text.SetDrawingRect(tximg.InternalDrawRect.Left, tximg.InternalDrawRect.Top, tximg.Face.ImageRect.Left - tximg.ImageTextSpace, tximg.InternalDrawRect.Bottom);
                        tximg.Text.SetLocation(0, tximg.InternalDrawRect.CenterY(), Alignament.LeftCenter);
                        DrawString(tximg.Text);
                        break;
                }
            }
            else
            {
                if (tximg.Face != null)
                    DrawImage(tximg.Face);
                if (hasText)
                {
                    int x = tximg.InternalDrawRect.GetAnchorX(tximg.Text.layout.Align);
                    int y = tximg.InternalDrawRect.GetAnchorY(tximg.Text.layout.Align);
                    tximg.Text.SetLocation(x, y);
                    DrawString(tximg.Text);
                }
            }
            //DrawRect(tximg.InternalDrawRect, Color.Red, 3);
        }
         */ 
        #endregion

        #region Rectangle Functions
        public void ClearScreen(int aRGB)
        {
            tempBrush.Color = System.Drawing.Color.FromArgb(aRGB);
            g.FillRectangle(tempBrush,0, 0, Width, Height);
        }

        private void g_DrawRectangle(int x, int y, int w, int h, int borderColor, int fillColor, int penWidth)
        {
            if (fillColor != 0) // color.Transparent
            {
                tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                g.FillRectangle(tempBrush, x, y, w, h);
            }
            if ((borderColor != 0) && (penWidth > 0))
            {
                tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                tempPen.Width = penWidth;
                g.DrawRectangle(tempPen, x, y, w, h);
            }
        }
        public void DrawRect(int left, int top, int right, int bottom, int borderColor)
        {
            g_DrawRectangle(left, top, right - left, bottom - top,borderColor,0,1);
        }
        public void DrawRectWH(int left, int top, int width, int height, int borderColor)
        {
            g_DrawRectangle(left, top, width, height, borderColor, 0, 1);
        }
        public void DrawRect(GRect rect, int borderColor)
        {
            g_DrawRectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, 0, 1);
        }
        public void DrawRect(int left, int top, int right, int bottom, int borderColor,int fillColor,int penWidth)
        {
            g_DrawRectangle(left, top, right - left, bottom - top, borderColor, fillColor, penWidth);
        }
        public void DrawRectWH(int left, int top, int width, int height, int borderColor, int fillColor, int penWidth)
        {
            g_DrawRectangle(left, top, width, height, borderColor, fillColor, penWidth);
        }
        public void DrawRect(GRect rect, int borderColor, int fillColor, int penWidth)
        {
            g_DrawRectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, fillColor, penWidth);
        }
        #endregion

        #region Round Rectangle Functions
        public void DrawRoundRect(int left, int top, int right, int bottom, int ArcWidth, int ArcHeight, int borderColor, int fillColor, int penWidth)
        {
            gp.Reset();
            ArcHeight *= 2;
            ArcWidth *= 2;
            gp.AddArc(left, top, ArcWidth, ArcHeight, 180, 90);
            gp.AddArc(right - ArcWidth, top, ArcWidth, ArcHeight, 270, 90);
            gp.AddArc(right - ArcWidth, bottom-ArcHeight, ArcWidth, ArcHeight, 0, 90);
            gp.AddArc(left, bottom - ArcHeight, ArcWidth, ArcHeight, 90, 90);
            gp.CloseFigure();


            if (fillColor != 0)
            {
                tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                g.FillPath(tempBrush, gp);
            }
            if ((borderColor != 0) && (penWidth > 0))
            {
                tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                tempPen.Width = penWidth;
                g.DrawPath(tempPen, gp);
            }
        }
        #endregion

        #region Ellipse Functions

        private void g_DrawEllipse(int x, int y, int w, int h, int borderColor, int fillColor, int penWidth)
        {
            if (fillColor != 0) // color.Transparent
            {
                tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                g.FillEllipse(tempBrush, x, y, w, h);
            }
            if ((borderColor != 0) && (penWidth > 0))
            {
                tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                tempPen.Width = penWidth;
                g.DrawEllipse(tempPen, x, y, w, h);
            }
        }
        public void DrawEllipse(int left, int top, int right, int bottom, int borderColor)
        {
            g_DrawEllipse(left, top, right - left, bottom - top, borderColor, 0, 1);
        }
        public void DrawEllipseWH(int left, int top, int width, int height, int borderColor)
        {
            g_DrawEllipse(left, top, width, height, borderColor, 0, 1);
        }
        public void DrawEllipse(GRect rect, int borderColor)
        {
            g_DrawEllipse(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, 0, 1);
        }
        public void DrawEllipse(int left, int top, int right, int bottom, int borderColor, int fillColor, int penWidth)
        {
            g_DrawEllipse(left, top, right - left, bottom - top, borderColor, fillColor, penWidth);
        }
        public void DrawEllipseWH(int left, int top, int width, int height, int borderColor, int fillColor, int penWidth)
        {
            g_DrawEllipse(left, top, width, height, borderColor, fillColor, penWidth);
        }
        public void DrawEllipse(GRect rect, int borderColor, int fillColor, int penWidth)
        {
            g_DrawEllipse(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, fillColor, penWidth);
        }

        #endregion

        #region Circle Functions
        public void DrawCircle(int x, int y, int ray, int borderColor)
        {
            g_DrawEllipse(x - ray, y - ray, ray * 2, ray * 2, borderColor, 0, 1);
        }
        public void DrawCircle(int x, int y, int ray, int borderColor, int fillColor, int penWidth)
        {
            g_DrawEllipse(x - ray, y - ray, ray * 2, ray * 2, borderColor, fillColor, penWidth);
        }
        #endregion

        #region ARC Functions
        public void DrawArc(int left, int top, int right, int bottom, int startAngle, int endAngle,int aRGB, int penWidth)
        {
            tempPen.Color = System.Drawing.Color.FromArgb(aRGB);
            tempPen.Width = penWidth;
            g.DrawArc(tempPen, left, top, right - left, bottom - top, startAngle, endAngle - startAngle);
        }
        #endregion

        #region Line function
        private void g_DrawLine(int x1, int y1, int x2, int y2, int lineColor, int lineWidth)
        {
            tempPen.Color = System.Drawing.Color.FromArgb(lineColor);
            tempPen.Width = lineWidth;
            g.DrawLine(tempPen, x1, y1, x2, y2);
        }
        public void DrawLine(int x1, int y1, int x2, int y2,int aRGB)
        {
            g_DrawLine(x1, y1, x2, y2, aRGB,1);
        }
        public void DrawLine(int x1, int y1, int x2, int y2, int aRGB,int penWidth)
        {
            g_DrawLine(x1, y1, x2, y2, aRGB, penWidth);
        }
        #endregion

        #region Pixel Functions
        public void SetPixel(int x, int y, int aRGB)
        {
            tempBrush.Color = System.Drawing.Color.FromArgb(aRGB);
            g.FillRectangle(tempBrush, x,y,1,1);
        }
        #endregion

        #region String function
        /*
        private void DrawGlyphString(GlyphsFont glyph, string text, float scale,float x, float y, int charSpace)
        {
            if (scale == 1)
            {
                foreach (char ch in text)
                {
                    if (ch == ' ')
                    {
                        x += glyph.SpaceWidth+charSpace;
                        continue;
                    }
                    int code = ch;
                    GlyphsFont.GlyphData gd = null;
                    if ((code >= glyph.MinDirectGlyphsCode) && (code <= glyph.MaxDirectGlyphsCode))
                    {
                        gd = glyph.DirectGlyphs[code - glyph.MinDirectGlyphsCode];
                    }
                    else
                    {
                        if (glyph.OtherGlyphs != null)
                            if (glyph.OtherGlyphs.TryGetValue(code, out gd) == false)
                                gd = null;
                    }
                    if (gd != null)
                    {
                        g.DrawImage(gd.Image.__internal__image, x, y - (gd.Image.Height - gd.Image.Height*gd.BaseLinePercentage), gd.Image.Width, gd.Image.Height);
                        x += gd.Image.Width + charSpace;
                    }
                }
            }
            else
            {
                foreach (char ch in text)
                {
                    if (ch == ' ')
                    {
                        x += (int)(glyph.SpaceWidth*scale);
                        continue;
                    }
                    int code = ch;
                    GlyphsFont.GlyphData gd = null;
                    if ((code >= glyph.MinDirectGlyphsCode) && (code <= glyph.MaxDirectGlyphsCode))
                    {
                        gd = glyph.DirectGlyphs[code - glyph.MinDirectGlyphsCode];
                    }
                    else
                    {
                        if (glyph.OtherGlyphs != null)
                            if (glyph.OtherGlyphs.TryGetValue(code, out gd) == false)
                                gd = null;
                    }
                    if (gd != null)
                    {
                        g.DrawImage(gd.Image.__internal__image, x, y - (int)((gd.Image.Height - -gd.Image.Height * gd.BaseLinePercentage) * scale), (int)(gd.Image.Width * scale), (int)(gd.Image.Height * scale));
                        x += (int)(gd.Image.Width*scale + charSpace);
                    }
                }
            }
        }
        private void DrawStringForGlyph(TextPainter tm)
        {
            if (tm.LinesCount == 0)
            {
                DrawGlyphString(tm.glyphFont, tm.Text, tm.fontSize, tm.textX, tm.textY, tm.glyphCharSpace);
            }
            else
            {
                for (int tr = 0; tr < tm.LinesCount; tr++)
                {
                    DrawGlyphString(tm.glyphFont, tm.Lines[tr], tm.fontSize, tm.lineX[tr], tm.lineY[tr], tm.glyphCharSpace);
                }
            }
            //DrawRect(tm.rect, Color.Red);
        }
        private void DrawStringForFont(TextPainter tm)
        {
            // necesar doar pt. develop - unde e un singur obiect de tipul font
            tm.font._internal_SetFontSize(tm.fontSize);
            if (animRecorder != null)
            {
                animRecorder.DrawString(tm,this);
            }

            if (tm.LinesCount == 0)
            {
                switch (tm.Effects)
                {
                    case TextEffects.Simple:
                        g.DrawString(tm.Text, tm.font.font, tm.tempBrush1, tm.textX, tm.textY);
                        break;
                    case TextEffects.DoubleText:
                        g.DrawString(tm.Text, tm.font.font, tm.tempBrush2, tm.textX + tm.textOffsetX, tm.textY + tm.textOffsetY);
                        g.DrawString(tm.Text, tm.font.font, tm.tempBrush1, tm.textX, tm.textY);
                        break;
                }

            }
            else
            {
                for (int tr = 0; tr < tm.LinesCount; tr++)
                {
                    switch (tm.Effects)
                    {
                        case TextEffects.Simple:
                            g.DrawString(tm.Lines[tr], tm.font.font, tm.tempBrush1, tm.lineX[tr], tm.lineY[tr]);
                            break;
                        case TextEffects.DoubleText:
                            g.DrawString(tm.Lines[tr], tm.font.font, tm.tempBrush2, tm.lineX[tr] + tm.textOffsetX, tm.lineY[tr] + tm.textOffsetY);
                            g.DrawString(tm.Lines[tr], tm.font.font, tm.tempBrush1, tm.lineX[tr], tm.lineY[tr]);
                            break;
                    }
                }
            }
            //DrawRect(tm.rect, Color.Red);
        }
        public void DrawString(TextPainter tm)
        {
            if (tm.NeedRecompute!=0)
                tm.RecomputeMeasures(this);
            if (tm.glyphFont != null)
                DrawStringForGlyph(tm);
            else
                DrawStringForFont(tm);
            //DrawRect(tm.DrawingRect, Color.Green);
        }
        public void DrawString(TextPainter tm, int aRGB)
        {
            tm.SetColor(aRGB);
            DrawString(tm);
        }
        public void DrawString(GFont font, string text, int x, int y, Alignament align, int aRGB, float size)
        {
            tempTextMeasures.SetFont(font);
            tempTextMeasures.SetText(text);
            tempTextMeasures.SetLocation(x, y, align);
            tempTextMeasures.SetColor(aRGB);
            tempTextMeasures.SetFontSize(size);
            DrawString(tempTextMeasures);
        }
        */
        #endregion

        #region Colors

        public void SetFillColor(int aRGB)
        {            
            gBrush.Color = System.Drawing.Color.FromArgb(aRGB);
        }
        public void SetFillColor(int a, int r, int g, int b)
        {
            gBrush.Color = System.Drawing.Color.FromArgb(a, r, g, b);
        }
        public void SetFillColor(int r, int g, int b)
        {
            gBrush.Color = System.Drawing.Color.FromArgb(255, r, g, b);
        }
        public void SetPenColor(int aRGB)
        {
            gPen.Color = System.Drawing.Color.FromArgb(aRGB);
        }
        public void SetPenColor(int a, int r, int g, int b)
        {
            gPen.Color = System.Drawing.Color.FromArgb(a, r, g, b);
        }
        public void SetPenColor(int r, int g, int b)
        {
            gPen.Color = System.Drawing.Color.FromArgb(255, r, g, b);
        }
        public void SetPenWidth(int newWidth)
        {
            gPen.Width = newWidth;
        }
        #endregion

        #region State Function
        public void SaveState()
        {
            ContextStateStack[ContextStateStackIndex] = g.Save();
            ContextStateStackIndex++;
            if (ContextStateStackIndex > 31)
                ContextStateStackIndex = 31;
        }
        public void RestoreState()
        {
            ContextStateStackIndex--;
            if (ContextStateStackIndex < 0)
                ContextStateStackIndex = 0;
            g.Restore(ContextStateStack[ContextStateStackIndex]);
        }
        public void Rotate(float degrees, int pointX, int pointY)
        {
            g.TranslateTransform(pointX, pointY);
            g.RotateTransform(degrees);
            g.TranslateTransform(-pointX, -pointY);
        }
        public void TranslateOriginAbsolute(int pointX, int pointY)
        {            
            g.TranslateTransform(pointX-g.Transform.OffsetX, pointY-g.Transform.OffsetY);
        }
        public void TranslateOriginRelative(int pointX, int pointY)
        {
            g.TranslateTransform(pointX, pointY);
        }
        public void ClipRect(int left, int top, int right, int bottom)
        {
            rTemp.X = left;
            rTemp.Y = top;
            rTemp.Width = (right-left);
            rTemp.Height = (bottom-top);
            g.IntersectClip(rTemp);
            
        }
        public void ClipRect(GRect rect)
        {
            rTemp.X = rect.Left;
            rTemp.Y = rect.Top;
            rTemp.Width = (rect.Right - rect.Left);
            rTemp.Height = (rect.Bottom - rect.Top);
            g.IntersectClip(rTemp);
        }
        public void ClipRectWH(int left, int top, int width, int height)
        {
            rTemp.X = left;
            rTemp.Y = top;
            rTemp.Width = width;
            rTemp.Height = height;
            g.IntersectClip(rTemp);
        }
        public void Reset()
        {
            g.ResetClip();
            g.ResetTransform();
            ContextStateStackIndex = 0;
        }
        public void EnableInterpolationMode(bool value)
        {
            if (value)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
            }
            else
            {
                g.InterpolationMode = InterpolationMode.Default;
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }
        }
        public void SetDrawingBounds(int left, int top, int right, int bottom)
        {
            drawingBounds.Set(left,top,right,bottom);
        }
        public void SetDrawingBoundsWH(int x, int y, int w, int h)
        {
            drawingBounds.SetWH(x, y, w, h);
        }
        public void SetDrawingBounds(GRect rect)
        {
            drawingBounds.Set(rect);
        }
        public float ConvertXFromDrawingBounds(float x)
        {
            return drawingBounds.Left + x * drawingBounds.GetWidth();
        }
        public float ConvertYFromDrawingBounds(float y)
        {
            return drawingBounds.Top + y * drawingBounds.GetHeight();
        }
        #endregion
    }
}
