using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System;
using System.Collections.Generic;
//using System.Windows.Forms;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using PluginInterface;
using System.Drawing.Imaging;

namespace GAppCreator
{
    namespace AnimO
    {
        public enum BoardViewMode
        {
            Design,
            Play
        };
        public interface IRefreshDesign
        {
            void Refresh();
        }
        public enum Coordinates
        {
            Pixels,
            Percentage,
        }
        public class RuntimeContext
        {
            // toate informatiile despre un element care vor fi necesare in timpul transformarii
            public float ScaleWidth = 1.0f;
            public float ScaleHeight = 1.0f;
            public float X_Percentage = 0;
            public float Y_Percentage = 0;
            public float WidthInPixels = 0;
            public float HeightInPixels = 0;
            public Alignament Align = Alignament.TopLeft;
            public UInt32 ColorBlending = 0xFFFFFFFF;
            public Bitmap Image = null;
            public bool Visible = true;
            public RectangleF boundRect_ = new RectangleF();
            public RectangleF ScreenRect = new RectangleF();
            public static uint BlendModeToColor(BlendingMode mode, int color, float alpha)
            {
                switch (mode)
                {
                    case BlendingMode.ColorBlending: return (uint)color;
                    case BlendingMode.AlphaBlending: return ((uint)(alpha * 255)) << 24 | (((uint)0xFFFFFF) & ((uint)color));
                    default: return 0xFFFFFFFF;
                }
            }
        }

        public class Canvas
        {
            private Pen tempPen = new Pen(System.Drawing.Color.Black, 1);
            private SolidBrush tempBrush = new SolidBrush(System.Drawing.Color.Black);

            int left, top, width, height;
            float scale;
            ColorMatrix cm = new ColorMatrix();
            ImageAttributes imageAttributes = new ImageAttributes();
            Rectangle imageRect = new Rectangle();
            Graphics internalGraphics = null;
            SolidBrush brush = new SolidBrush(Color.White);
            RectangleF tempRectF = new RectangleF();


            public float ConvertXAxisToPercentage(int x)
            {
                return ((float)x) / ((float)width);
            }
            public float ConvertYAxisToPercentage(int y)
            {
                return ((float)y) / ((float)height);
            }
            public float ConvertXAxisToPercentage(float x)
            {
                return x / ((float)width);
            }
            public float ConvertYAxisToPercentage(float y)
            {
                return y / ((float)height);
            }
            private void ComputeRectOnScreen(float x, float y, float objWidthInPixels, float objHeightInPixels, Alignament align, ref RectangleF resultedRectInPixels, bool fixedSize = false, float scaleWidth = 1.0f, float scaleHeight = 1.0f)
            {
                float xPoz = x * width + left;
                float yPoz = y * height + top;
                float w, h;
                if (fixedSize)
                {
                    w = objWidthInPixels;
                    h = objHeightInPixels;
                }
                else
                {
                    w = objWidthInPixels * scale * scaleWidth;
                    h = objHeightInPixels * scale * scaleHeight;
                }
                switch (align)
                {
                    case Alignament.Center: xPoz -= w / 2; yPoz -= h / 2; break;
                    case Alignament.TopLeft: break;
                    case Alignament.TopCenter: xPoz -= w / 2; break;
                    case Alignament.TopRight: xPoz -= w; break;
                    case Alignament.RightCenter: xPoz -= w; yPoz -= h / 2; break;
                    case Alignament.BottomRight: xPoz -= w; yPoz -= h; break;
                    case Alignament.BottomCenter: xPoz -= w / 2; yPoz -= h; break;
                    case Alignament.BottomLeft: yPoz -= h; break;
                    case Alignament.LeftCenter: yPoz -= h / 2; break;
                }
                resultedRectInPixels.X = xPoz;
                resultedRectInPixels.Y = yPoz;
                resultedRectInPixels.Width = w;
                resultedRectInPixels.Height = h;
            }
            private void TranslateScreenRectToScaledScreenRect(RectangleF screenRect, ref RectangleF result, bool fixedSize = false)
            {
                if (fixedSize)
                {
                    result.Width = screenRect.Width;
                    result.Height = screenRect.Height;
                }
                else
                {
                    result.Width = screenRect.Width * scale;
                    result.Height = screenRect.Height * scale;
                }
                result.X = screenRect.X * scale + left;
                result.Y = screenRect.Y * scale + top;
            }
            private void TranslateScreenRectToScaledScreenRect(RuntimeContext rContext, ref RectangleF result, bool fixedSize = false)
            {
                TranslateScreenRectToScaledScreenRect(rContext.ScreenRect, ref result, fixedSize);
            }
            private void ComputeRectOnScreen(RuntimeContext rContext, bool useImageMemberForWidth, ref RectangleF resultedRectInPixels)
            {
                if (useImageMemberForWidth)
                    ComputeRectOnScreen(rContext.X_Percentage, rContext.Y_Percentage, rContext.Image.Width, rContext.Image.Height, rContext.Align, ref resultedRectInPixels, false, rContext.ScaleWidth, rContext.ScaleHeight);
                else
                    ComputeRectOnScreen(rContext.X_Percentage, rContext.Y_Percentage, rContext.WidthInPixels, rContext.HeightInPixels, rContext.Align, ref resultedRectInPixels, false, rContext.ScaleWidth, rContext.ScaleHeight);
            }
            private void ComputeRectInPercentages(float x, float y, float objWidthInPixels, float objHeightInPixels, Alignament align, ref RectangleF resultedRectInPixels, bool fixedSize = false, float scaleWidth = 1.0f, float scaleHeight = 1.0f)
            {
                float xPoz = x;
                float yPoz = y;
                float w, h;
                if (fixedSize)
                {
                    w = objWidthInPixels;
                    h = objHeightInPixels;
                }
                else
                {
                    // calculez pozitia in procente - nu ma intereseaza scale-ul in acest punct
                    w = objWidthInPixels * scaleWidth;
                    h = objHeightInPixels * scaleHeight;
                }
                w = w / ((float)width / scale);
                h = h / ((float)height / scale);
                switch (align)
                {
                    case Alignament.Center: xPoz -= w / 2; yPoz -= h / 2; break;
                    case Alignament.TopLeft: break;
                    case Alignament.TopCenter: xPoz -= w / 2; break;
                    case Alignament.TopRight: xPoz -= w; break;
                    case Alignament.RightCenter: xPoz -= w; yPoz -= h / 2; break;
                    case Alignament.BottomRight: xPoz -= w; yPoz -= h; break;
                    case Alignament.BottomCenter: xPoz -= w / 2; yPoz -= h; break;
                    case Alignament.BottomLeft: yPoz -= h; break;
                    case Alignament.LeftCenter: yPoz -= h / 2; break;
                }
                resultedRectInPixels.X = xPoz;
                resultedRectInPixels.Y = yPoz;
                resultedRectInPixels.Width = w;
                resultedRectInPixels.Height = h;
            }
            public void ComputeRectInPercentages(RuntimeContext rContext, bool useImageMemberForWidth, ref RectangleF resultedRectInPercentages)
            {
                if (useImageMemberForWidth)
                {
                    float w = 0;
                    float h = 0;
                    if (rContext.Image != null)
                    {
                        w = rContext.Image.Width;
                        h = rContext.Image.Height;
                    }
                    ComputeRectInPercentages(rContext.X_Percentage, rContext.Y_Percentage, w, h, rContext.Align, ref resultedRectInPercentages, false, rContext.ScaleWidth, rContext.ScaleHeight);
                }
                else
                    ComputeRectInPercentages(rContext.X_Percentage, rContext.Y_Percentage, rContext.WidthInPixels, rContext.HeightInPixels, rContext.Align, ref resultedRectInPercentages, false, rContext.ScaleWidth, rContext.ScaleHeight);
            }



            #region General Settings
            public void SetGraphics(Graphics g)
            {
                internalGraphics = g;
            }
            public void SetScreen(int _left, int _top, int _width, int _height, float _scale)
            {
                left = _left;
                top = _top;
                width = _width;
                height = _height;
                scale = _scale;
            }
            public int GetWidth() { return width; }
            public int GetHeight() { return height; }
            public float GetScale() { return scale; }
            public float GetLeft() { return left; }
            public float GetTop() { return top; }
            #endregion

            #region Image
            //public void GetLastImageRect(ref Rectangle resultedImageRect)
            //{
            //    resultedImageRect.X = (int)(lastXPoz);
            //    resultedImageRect.Y = (int)(lastYPoz);
            //    resultedImageRect.Width = (int)(imageRect.Width / scale);
            //    resultedImageRect.Height = (int)(imageRect.Height / scale);
            //}
            //public void DrawImage_(Image img, float x, float y, Alignament align, uint ColorBlending = 0xFFFFFFFF, float scaleWidth = 1.0f, float scaleHeight = 1.0f)
            //{
            //    if (internalGraphics == null)
            //        return;
            //    if (img == null)
            //        return;
            //    ComputeRectOnScreen(x, y, img.Width, img.Height, align, ref tempRectF, false, scaleWidth, scaleHeight);
            //    cm.Matrix00 = ((float)((ColorBlending >> 16) & 0xFF)) / 255.0f; // RED
            //    cm.Matrix11 = ((float)((ColorBlending >> 8) & 0xFF)) / 255.0f; // GREEN
            //    cm.Matrix22 = ((float)((ColorBlending >> 0) & 0xFF)) / 255.0f; // BLUE 
            //    cm.Matrix33 = ((float)((ColorBlending >> 24) & 0xFF)) / 255.0f; // ALPHA                
            //    imageAttributes.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //    imageRect.X = (int)tempRectF.X;
            //    imageRect.Y = (int)tempRectF.Y;
            //    imageRect.Width = (int)tempRectF.Width;
            //    imageRect.Height = (int)tempRectF.Height;
            //    internalGraphics.DrawImage(img, imageRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
            //}
            public void DrawImage(Image img, RectangleF screenPozition, uint ColorBlending = 0xFFFFFFFF)
            {
                if (internalGraphics == null)
                    return;
                if (img == null)
                    return;
                TranslateScreenRectToScaledScreenRect(screenPozition, ref tempRectF, false);
                imageRect.X = (int)tempRectF.X;
                imageRect.Y = (int)tempRectF.Y;
                imageRect.Width = (int)tempRectF.Width;
                imageRect.Height = (int)tempRectF.Height;
                cm.Matrix00 = ((float)((ColorBlending >> 16) & 0xFF)) / 255.0f; // RED
                cm.Matrix11 = ((float)((ColorBlending >> 8) & 0xFF)) / 255.0f; // GREEN
                cm.Matrix22 = ((float)((ColorBlending >> 0) & 0xFF)) / 255.0f; // BLUE 
                cm.Matrix33 = ((float)((ColorBlending >> 24) & 0xFF)) / 255.0f; // ALPHA                
                imageAttributes.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                internalGraphics.DrawImage(img, imageRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            public void DrawImage(RuntimeContext rContext)
            {
                DrawImage(rContext.Image, rContext.ScreenRect, rContext.ColorBlending);
                //DrawImage(rContext.Image, rContext.X_Percentage, rContext.Y_Percentage, rContext.Align, rContext.ColorBlending, rContext.ScaleWidth, rContext.ScaleHeight);
            }
            public void FillScreen(Image img)
            {
                ClearScreen((int)0x7FFF8040);
            }
            #endregion


            #region Clipping
            public void EnableClipping(RuntimeContext rContext)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(rContext, false, ref tempRectF);
                internalGraphics.SetClip(tempRectF);
            }
            public void ClearClipping(Graphics g)
            {
                g.ResetClip();
            }
            public void ClearClipping()
            {
                ClearClipping(internalGraphics);
            }
            #endregion

            #region Rectangle Functions
            public void ClearScreen(int aRGB)
            {
                DrawRect(0, 0, 1, 1, Alignament.TopLeft, 0, aRGB, 0);
            }


            public void DrawRect(RectangleF screenPozition, int borderColor, int fillColor, float penWidth, bool fixedSize = false)
            {
                if (internalGraphics == null)
                    return;
                TranslateScreenRectToScaledScreenRect(screenPozition, ref tempRectF, fixedSize);

                if (fillColor != 0) // color.Transparent
                {
                    tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                    internalGraphics.FillRectangle(tempBrush, tempRectF);
                }
                if ((borderColor != 0) && (penWidth > 0))
                {
                    tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                    tempPen.Width = penWidth;
                    internalGraphics.DrawRectangle(tempPen, tempRectF.X, tempRectF.Y, tempRectF.Width, tempRectF.Height);
                }
            }
            public void DrawRect(RuntimeContext rContext, int borderColor, int fillColor, float penWidth, bool fixedSize = false)
            {
                DrawRect(rContext.ScreenRect, borderColor, fillColor, penWidth, fixedSize);
            }

            public void DrawRect(float x, float y, float rectWithInPixels, float rectHeightInPixels, Alignament align, int borderColor, int fillColor, float penWidth, float scaleWidth = 1, float scaleHeight = 1, bool fixedSize = false)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(x, y, rectWithInPixels, rectHeightInPixels, align, ref tempRectF, fixedSize, scaleWidth, scaleHeight);

                if (fillColor != 0) // color.Transparent
                {
                    tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                    internalGraphics.FillRectangle(tempBrush, tempRectF);
                }
                if ((borderColor != 0) && (penWidth > 0))
                {
                    tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                    tempPen.Width = penWidth;
                    internalGraphics.DrawRectangle(tempPen, tempRectF.X, tempRectF.Y, tempRectF.Width, tempRectF.Height);
                }
            }
            public void DrawRectWithPixelsCoordonates(int left, int top, int right, int bottom, int borderColor, int fillColor, int penWidth)
            {
                DrawRect(ConvertXAxisToPercentage(left), ConvertYAxisToPercentage(top), right - left, bottom - top, Alignament.TopLeft, borderColor, fillColor, penWidth);
            }
            public void DrawObjectRect(RuntimeContext rContext, bool useImageForWidth, int borderColor)
            {
                if ((useImageForWidth) && (rContext.Image != null))
                    this.DrawRect(rContext.X_Percentage, rContext.Y_Percentage, rContext.Image.Width, rContext.Image.Height, rContext.Align, borderColor, 0, 1, rContext.ScaleWidth, rContext.ScaleHeight);
                else
                    this.DrawRect(rContext.X_Percentage, rContext.Y_Percentage, rContext.WidthInPixels, rContext.HeightInPixels, rContext.Align, borderColor, 0, 1, rContext.ScaleWidth, rContext.ScaleHeight);
            }

            public void FillRect(RuntimeContext rContext)
            {
                DrawRect(rContext, 0, (int)rContext.ColorBlending, 0);
            }

            public void FillExclusionRect(RuntimeContext rContext)
            {
                if (internalGraphics == null)
                    return;
                //ComputeRectOnScreen(rContext, false, ref tempRectF);
                TranslateScreenRectToScaledScreenRect(rContext, ref tempRectF, false);

                brush.Color = Color.FromArgb((int)rContext.ColorBlending);
                int l = (int)tempRectF.Left;
                int t = (int)tempRectF.Top;
                int r = (int)tempRectF.Right;
                int b = (int)tempRectF.Bottom;

                if ((l > left + this.width) || (r < left) || (t > top + this.height) || (b < top))
                {
                    // fac un fill la ecran si gata
                    internalGraphics.FillRectangle(brush, left, top, width, height);
                    return;
                }
                // altfel am o zona comuna
                if (t < top)
                    t = top;
                if (l < left)
                    l = left;
                if (b > top + height)
                    b = top + height;
                if (r > left + width)
                    r = left + width;

                internalGraphics.FillRectangle(brush, left, top, width, t - top);
                internalGraphics.FillRectangle(brush, left, b, width, top + height - b);
                if (l > left)
                {
                    internalGraphics.FillRectangle(brush, left, t, l - left, b - t);
                }
                if (r < left + width)
                {
                    internalGraphics.FillRectangle(brush, r, t, left + width - r, b - t);
                }

            }
            #endregion

            #region Line function
            private void DrawLine(float x1, float y1, float x2, float y2, int lineColor, int lineWidth)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(x1, y1, 0, 0, Alignament.TopLeft, ref tempRectF, true);
                float _x1 = tempRectF.X;
                float _y1 = tempRectF.Y;
                ComputeRectOnScreen(x2, y2, 0, 0, Alignament.TopLeft, ref tempRectF, true);
                float _x2 = tempRectF.X;
                float _y2 = tempRectF.Y;
                tempPen.Color = System.Drawing.Color.FromArgb(lineColor);
                tempPen.Width = lineWidth;
                internalGraphics.DrawLine(tempPen, _x1, _y1, _x2, _y2);
            }
            public void DrawLineWithPixelsCoordonates(int x1, int y1, int x2, int y2, int lineColor, int lineWidth)
            {
                DrawLine(ConvertXAxisToPercentage(x1), ConvertYAxisToPercentage(y1), ConvertXAxisToPercentage(x2), ConvertYAxisToPercentage(y2), lineColor, lineWidth);
            }
            #endregion

            #region Ellipse Functions
            public void DrawEllipse(float x, float y, float rectWidth, float rectHeight, Alignament align, int borderColor, int fillColor, float penWidth, float scaleWidth = 1, float scaleHeight = 1, bool fixedSize = false)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(x, y, rectWidth, rectHeight, align, ref tempRectF, fixedSize, scaleWidth, scaleHeight);

                if (fillColor != 0) // color.Transparent
                {
                    tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                    internalGraphics.FillEllipse(tempBrush, tempRectF);
                }
                if ((borderColor != 0) && (penWidth > 0))
                {
                    tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                    tempPen.Width = penWidth;
                    internalGraphics.DrawEllipse(tempPen, tempRectF.X, tempRectF.Y, tempRectF.Width, tempRectF.Height);
                }
            }

            //public void DrawEllipse(int left, int top, int right, int bottom, int borderColor)
            //{
            //    g_DrawEllipse(left, top, right - left, bottom - top, borderColor, 0, 1);
            //}
            //public void DrawEllipseWH(int left, int top, int width, int height, int borderColor)
            //{
            //    g_DrawEllipse(left, top, width, height, borderColor, 0, 1);
            //}
            //public void DrawEllipse(GRect rect, int borderColor)
            //{
            //    g_DrawEllipse(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, 0, 1);
            //}
            //public void DrawEllipse(int left, int top, int right, int bottom, int borderColor, int fillColor, int penWidth)
            //{
            //    g_DrawEllipse(left, top, right - left, bottom - top, borderColor, fillColor, penWidth);
            //}
            //public void DrawEllipseWH(int left, int top, int width, int height, int borderColor, int fillColor, int penWidth)
            //{
            //    g_DrawEllipse(left, top, width, height, borderColor, fillColor, penWidth);
            //}
            //public void DrawEllipse(GRect rect, int borderColor, int fillColor, int penWidth)
            //{
            //    g_DrawEllipse(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, fillColor, penWidth);
            //}

            #endregion

            #region Circle Functions
            public void DrawCircle(float x, float y, float ray, int borderColor, bool fixedSize = true)
            {
                DrawEllipse(x, y, ray * 2, ray * 2, Alignament.Center, borderColor, 0, 1, 1.0f, 1.0f, fixedSize);
            }
            public void DrawCircle(int x, int y, int ray, int borderColor, int fillColor, int penWidth, bool fixedSize = true)
            {
                DrawEllipse(x, y, ray * 2, ray * 2, Alignament.Center, borderColor, fillColor, penWidth, 1.0f, 1.0f, fixedSize);
            }
            #endregion

        }


        #region Transformations

        public interface ITransformationEvent
        {
            void OnEvent(GenericTransformation trans);
        }

        [XmlInclude(typeof(TouchBoundaryTransformation))]
        [XmlInclude(typeof(TouchStatusTransformation))]
        [XmlInclude(typeof(CountDown))]
        [XmlInclude(typeof(NumberIncreaseTransformation))]
        [XmlInclude(typeof(NumericFormatterTransformation))]
        [XmlInclude(typeof(TextCharacterVisibilityTransformation))]
        [XmlInclude(typeof(AlphaBlendingStateTransformation))]
        [XmlInclude(typeof(FontSizeTransformation))]
        [XmlInclude(typeof(SetNewRelativePositionTransformation))]
        [XmlInclude(typeof(SetNewAbsolutePositionTransformation))]
        [XmlInclude(typeof(Stopper))]
        [XmlInclude(typeof(BranchBlock))]
        [XmlInclude(typeof(IfElseBlock))]
        [XmlInclude(typeof(TextCenterFlowTransformation))]
        [XmlInclude(typeof(TextFlowTransformation))]
        [XmlInclude(typeof(SetNewImageTransformation))]
        [XmlInclude(typeof(SetImageIndexTransformation))]
        [XmlInclude(typeof(ImageIndexLinearTransformation))]
        [XmlInclude(typeof(SetNewTextTransformation))]
        [XmlInclude(typeof(ColorBlendStateTransformation))]
        [XmlInclude(typeof(ZOrderTransformation))]
        [XmlInclude(typeof(VisibleStateTransformation))]
        [XmlInclude(typeof(ButtonEnableTransformation))]
        [XmlInclude(typeof(QuadraticBezierTransformation))]
        [XmlInclude(typeof(MoveAbsoluteLinearTransformation))]
        [XmlInclude(typeof(MoveRelativeLinearTransformation))]
        [XmlInclude(typeof(ContinousBlock))]
        [XmlInclude(typeof(ColorBlendForwardAndBackTransformation))]
        [XmlInclude(typeof(ColorBlendLinearTransformation))]
        [XmlInclude(typeof(ScaleHeightForwardAndBackTransformation))]
        [XmlInclude(typeof(ScaleHeightLinearTransformation))]
        [XmlInclude(typeof(ScaleWidthForwardAndBackTransformation))]
        [XmlInclude(typeof(ScaleWidthLinearTransformation))]
        [XmlInclude(typeof(ScaleForwardAndBackTransformation))]
        [XmlInclude(typeof(ScaleLinearTransformation))]
        [XmlInclude(typeof(AlphaBlendingForwardAndBackTransformation))]
        [XmlInclude(typeof(AlphaBlendingLinearTransformation))]
        [XmlInclude(typeof(TransformationBlock))]
        [XmlInclude(typeof(RepeatBlock))]
        [XmlInclude(typeof(RepeatUntil))]
        [XmlInclude(typeof(DoOnceUntil))]
        [XmlInclude(typeof(PopupLoop))]
        [XmlInclude(typeof(DoOncePopupLoop))]
        [XmlInclude(typeof(WaitUntil))]
        [XmlInclude(typeof(SoundTransformation))]
        [XmlInclude(typeof(EventTransformation))]
        [XmlInclude(typeof(TimerTransformation))]
        [XmlInclude(typeof(GenericElementTransformation))]
        [XmlType("Transformation"), XmlRoot("Transformation")]
        public class GenericTransformation
        {
            [XmlAttribute()]
            public string BranchName = "";

            [XmlIgnore()]
            public string CppName = "";

            [XmlAttribute()]
            public bool Collapsed = false;

            [XmlIgnore()]
            public static ITransformationEvent TransEvent = null;
            [XmlIgnore()]
            public static Project Prj = null;

            #region Atribute
            [XmlIgnore(), Description("Name of the branch (only required if the transformation is part of a branch)"), Category("Branch"), DisplayName("Branch Name")]
            public string _BranchName
            {
                get { return BranchName; }
                set
                {
                    if ((Project.ValidateVariableNameCorectness(value, false) == false) && (value.Length > 0))
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    BranchName = value;
                }
            }
            #endregion

            #region Virtual functions
            public virtual List<GenericTransformation> GetBlockTransformations()
            {
                return null;
            }
            public virtual string GetName()
            {
                return Factory.GetName(GetType());
            }
            public virtual string GetIconKey()
            {
                return Factory.GetIconKey(GetType());
            }
            #endregion

            #region Dynamic Execution
            protected GenericElement ElementObject = null;
            protected bool Started = false;
            public void SetElement(GenericElement element) { ElementObject = element; }
            public void Init()
            {
                OnInit();
                Started = true;
            }
            public bool Update()
            {
                if (Started == false)
                    return false;
                if (OnUpdate()) // continuu
                    return true;
                Started = false;
                return false;
            }
            protected virtual bool OnUpdate() { return false; }
            protected virtual void OnInit() { }
            protected string GetLocationValue(float value)
            {
                return Project.ProcentToString(value);
            }
            protected float SetLocationValue(string strRepresentation, float currentValue)
            {
                return Project.StringToProcent(strRepresentation, -1000, 1000, currentValue);
            }
            protected string GetSizeInPixels(float value)
            {
                return value.ToString() + " px";
            }
            protected float SetSizeInPixels(string strRepresentation, float currentValue)
            {
                float result = 0.0f;
                if (float.TryParse(strRepresentation.ToLower().Replace("px", "").Trim(), out result))
                {
                    if (result >= 0)
                        return result;
                }
                return currentValue;
            }
            protected string GetOnStartFieldInit(string parameter, string localName)
            {
                if (parameter.Length > 0)
                    return "\n\t" + CppName + "." + localName + " = param_" + parameter + ";";
                return "";
            }

            public virtual string GetCPPClassName() { return "?"; }
            public virtual string CreateInitializationCPPCode() { return "\n???? - missing initialization code for: " + this.GetType().ToString() + "\n"; }
            public virtual string CreateOnStartCPPCode()
            {
                return "";
            }
            public virtual void PopulateParameters(AnimationParameters p)
            {
            }
            public virtual void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements) { }
            public virtual void AddAnimationFunction(GACParser.Module m) { }
            public virtual string GetAnimationFunctionCPPImplementation(string className) { return ""; }
            public virtual string GetAnimationFunctionCPPHeaderDefinition() { return ""; }
            public virtual void CreateGACEnums(Dictionary<string, List<string>> enums) { }
            #endregion

            public string ToXMLString()
            {
                try
                {
                    var stringwriter = new System.IO.StringWriter();
                    var serializer = new XmlSerializer(typeof(GenericTransformation));
                    serializer.Serialize(stringwriter, this);
                    return stringwriter.ToString();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to convert frame to XML: \n" + e.ToString());
                    return "";
                }
            }
            public static GenericTransformation FromXMLString(string xmlText)
            {
                try
                {
                    var stringReader = new System.IO.StringReader(xmlText);
                    var serializer = new XmlSerializer(typeof(GenericTransformation));
                    GenericTransformation af = serializer.Deserialize(stringReader) as GenericTransformation;
                    if (af == null)
                    {
                        MessageBox.Show("Unable to create transformation from XML: \n" + xmlText);
                    }
                    return af;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to create transformation from XML: \n" + e.ToString());
                    return null;
                }
            }
        }

        [XmlType("Timer"), XmlRoot("Timer")]
        public class TimerTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public int Times = 1;
            [XmlAttribute()]
            public string UserValue_Times = "";

            #region Atribute
            [XmlIgnore(), Description("Number of times to wait until the next transformation starts"), Category("Timer"), DisplayName("Times")]
            public int _Times
            {
                get { return Times; }
                set { if (value > 0) Times = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Times")]
            public string _UserValue_Times
            {
                get { return UserValue_Times; }
                set { UserValue_Times = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Timer (Waits for " + Times.ToString() + " frames)";
            }
            public override string GetIconKey()
            {
                return "timer";
            }
            #endregion

            #region Dynamic Execution
            private int index;
            protected override void OnInit()
            {
                index = Times;
            }
            protected override bool OnUpdate()
            {
                index--;
                return index > 0;
            }
            public override string GetCPPClassName() { return "Timer"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Timer transformation\n\t" + this.CppName + ".NumberOfTimes = " + Times.ToString() + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Times.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Times, "int"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Times, "NumberOfTimes") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("ZOrder"), XmlRoot("ZOrder")]
        public class ZOrderTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public string ZOrder = "";
            [XmlIgnore()]
            public int ZOrderID = -1;

            #region Atribute
            [XmlIgnore(), Description("Sets a new ZOrder among existing element"), Category("Z-Order"), DisplayName("Z-Order"), Editor(typeof(AnimationZOrderEditor), typeof(UITypeEditor))]
            public string _ZOrder
            {
                get { return ZOrder; }
                set { ZOrder = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "ZOrder (" + ZOrder + ")";
            }
            public override string GetIconKey()
            {
                return "z_order";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (TransEvent != null)
                    TransEvent.OnEvent(this);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "ZOrder"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//ZOrder transformation\n\t" + this.CppName + ".ZOrderID = " + ZOrderID.ToString() + ";\n";
            }
            #endregion
        }

        [XmlType("TriggerEvent"), XmlRoot("TriggerEvent")]
        public class EventTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public string EventID = "";
            [XmlAttribute()]
            public string UserValue_Event = "";

            [XmlIgnore()]
            public int EventIDValue = -1;

            #region Atribute
            [XmlIgnore(), Description("Event that will be triggered"), Category("Event"), DisplayName("Event"), Editor(typeof(EventIDSelectorEditor), typeof(UITypeEditor))]
            public string _EventID
            {
                get { return EventID; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("Invalid name for event - you can only use letters and numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    EventID = value;
                }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Event")]
            public string _UserValue_Event
            {
                get { return UserValue_Event; }
                set { UserValue_Event = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Trigger Event (" + EventID + ")";
            }
            public override string GetIconKey()
            {
                return "event";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                EventIDValue = -1;
                if (EventID.Length == 0)
                {
                    prj.EC.AddError("Animation - '" + animationName + "' : You have to add an event ID to an Event Transformation !");
                    return;
                }
                foreach (var e in prj.ObjectEventsIDs)
                    if (e.Name.Equals(EventID))
                    {
                        EventIDValue = e.ID;
                        return;
                    }
                prj.EC.AddError("Animation - '" + animationName + "' : Event '" + EventID + "' is not defined !");
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "Event"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Event transformation\n\t" + this.CppName + ".EventID = " + EventIDValue.ToString() + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Event.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Event, "int"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Event, "EventID") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("TouchStatus"), XmlRoot("TouchStatus")]
        public class TouchStatusTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public bool Enable = true;

            #region Atribute
            [XmlIgnore(), Description("Weather or not touch is enabled (application wise)"), Category("Touch"), DisplayName("Touch enabled")]
            public bool _Enable
            {
                get { return Enable; }
                set { Enable = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                if (Enable)
                    return "Application touch: enabled";
                else
                    return "Application touch: disabled";
            }
            public override string GetIconKey()
            {
                return "touch_status";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "TouchStatus"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Touch status transformation\n\t" + this.CppName + ".TouchEnabled = " + Enable.ToString().ToLower() + ";\n";
            }
            #endregion
        }

        [XmlType("TouchBoundary"), XmlRoot("TouchBoundary")]
        public class TouchBoundaryTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public Alignament Align = Alignament.Center;
            [XmlAttribute()]
            public float X = 0.5f;
            [XmlAttribute()]
            public float Y = 0.5f;
            [XmlAttribute()]
            public float Width = 50;
            [XmlAttribute()]
            public float Height = 50;
            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "", UserValue_ScaleWidth = "", UserValue_ScaleHeight = "", UserValue_Align = "", UserValue_ColorBlending = "", UserValue_Width = "", UserValue_Height = "";


            #region Atribute
            [XmlIgnore(), Description("Rectangle Aliganment"), Category("Layout"), DisplayName("Alignament")]
            public Alignament _Align
            {
                get { return Align; }
                set { Align = value; }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value, X); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value, Y); }
            }
            [XmlIgnore(), Description("Scale width"), Category("Layout"), DisplayName("Width")]
            public string _Width
            {
                get { return GetSizeInPixels(Width); }
                set { Width = SetSizeInPixels(value, Width); }
            }
            [XmlIgnore(), Description("Scale height"), Category("Layout"), DisplayName("Height")]
            public string _Height
            {
                get { return GetSizeInPixels(Height); }
                set { Height = SetSizeInPixels(value, Height); }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Alignament")]
            public string _UserValue_Align
            {
                get { return UserValue_Align; }
                set { UserValue_Align = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Width")]
            public string _UserValue_Width
            {
                get { return UserValue_Width; }
                set { UserValue_Width = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Height")]
            public string _UserValue_Height
            {
                get { return UserValue_Height; }
                set { UserValue_Height = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Touch boundary: " + Align.ToString() + "(" + _X + "," + _Y + ") Size = [" + _Width + "px x " + _Height + "px]";
            }
            public override string GetIconKey()
            {
                return "touch_boundary";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "TouchBoundary"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//Touch boundary transformation\n\t";
                s += "\t" + this.CppName + ".X = " + X.ToString() + ";\n";
                s += "\t" + this.CppName + ".Y = " + Y.ToString() + ";\n";
                s += "\t" + this.CppName + ".Width = " + Width.ToString() + ";\n";
                s += "\t" + this.CppName + ".Height = " + Height.ToString() + ";\n";
                s += "\t" + this.CppName + ".Alignament = GAC_ALIGNAMENT_" + this.Align.ToString().ToUpper() + ";\n";
                return s;
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                if (UserValue_Width.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Width, "float"));
                if (UserValue_Height.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Height, "float"));
                if (UserValue_Align.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Align, "unsigned int", "Alignament"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_X, "X") + GetOnStartFieldInit(UserValue_Y, "Y") + GetOnStartFieldInit(UserValue_Align, "Alignament") + GetOnStartFieldInit(UserValue_Width, "Width") + GetOnStartFieldInit(UserValue_Height, "Height") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("PlaySound"), XmlRoot("PlaySound")]
        public class SoundTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public string SoundName = "";
            [XmlAttribute()]
            public string UserValue_Sound = "";


            #region Atribute
            [XmlIgnore(), Description("Sound that will be played"), Category("Sound"), DisplayName("Sound"), Editor(typeof(SoundSelectorEditor), typeof(UITypeEditor))]
            public string _SoundName
            {
                get { return SoundName; }
                set { SoundName = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Sound")]
            public string _UserValue_Sound
            {
                get { return UserValue_Sound; }
                set { UserValue_Sound = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Play sound (" + SoundName + ")";
            }
            public override string GetIconKey()
            {
                return "sound";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (SoundName.Length == 0)
                {
                    prj.EC.AddError("Animation - '" + animationName + "' : You have to add a sound to be played !");
                    return;
                }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "SoundPlay"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Sound transformation\n\t" + this.CppName + ".sound = Res.Sounds." + SoundName + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Sound.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Sound, "GApp::Resources::Sound*", "Sound"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Sound, "sound") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("ElementTransformation"), XmlRoot("EelementTransformation")]
        public class GenericElementTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public string Element = ""; // numele elementului pentru care se aplica transformarea AnimationElementEditor

            [XmlIgnore(), Description("Element to wich this transformation applies for."), Category("General"), DisplayName("Element"), Editor(typeof(AnimationElementEditor), typeof(UITypeEditor))]
            public string _Element
            {
                get { return Element; }
                set { Element = value; }
            }

            public override string GetName()
            {
                string s = GetTransformationDescription();
                if (s.Length == 0)
                    return Factory.GetName(GetType()) + " (" + Element + ")";
                else
                    return Factory.GetName(GetType()) + " (" + Element + " => " + s + ")";
            }
            protected virtual string GetTransformationDescription()
            {
                return "";
            }
            public override string GetCPPClassName()
            {
                string transformationName = this.GetType().ToString();
                int idx = transformationName.LastIndexOf('.');
                if (idx >= 0)
                    transformationName = transformationName.Substring(idx + 1);
                transformationName = transformationName.Replace("Transformation", "");
                return transformationName;
            }
        }

        [XmlType("Repeat"), XmlRoot("Repeat")]
        public class RepeatBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block
            [XmlAttribute()]
            public int Times = 1;
            [XmlAttribute()]
            public string UserValue_Times = "";

            #region Atribute
            [XmlIgnore(), Description("Number of times the content of this block will be repeated"), Category("Block"), DisplayName("Times")]
            public int _Times
            {
                get { return Times; }
                set { if (value > 0) Times = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Times")]
            public string _UserValue_Times
            {
                get { return UserValue_Times; }
                set { UserValue_Times = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Repeat block";
            }
            public override string GetIconKey()
            {
                return "repeat_block";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : Repeat blocks within an animation object must only have one child !");
            }
            #endregion

            #region Dynamic Execution
            private int index;
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                index = Times;
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    index--;
                    if (index <= 0)
                        return false;
                    // altfel o iau de la capat
                    transToRepeat.Init();
                }
                return true;
            }
            public override string GetCPPClassName() { return "Repeat"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Repeat transformation\n\t" + this.CppName + ".NumberOfTimes = " + Times.ToString() + ";\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Times.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Times, "int"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Times, "NumberOfTimes") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("Continous"), XmlRoot("Continous")]
        public class ContinousBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Continous block";
            }
            public override string GetIconKey()
            {
                return "infinite_loop_block";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : Continous/Infinitee loops within an animation object must only have one child !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    transToRepeat.Init();
                }
                return true;
            }
            public override string GetCPPClassName() { return "Continous"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Continous transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n";
            }
            #endregion
        }

        [XmlType("Stopper"), XmlRoot("Stopper")]
        public class Stopper : GenericTransformation
        {

            #region Virtual functions
            public override string GetName()
            {
                return "Stopper";
            }
            public override string GetIconKey()
            {
                return "stop";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return true;
            }
            public override string GetCPPClassName() { return "Stopper"; }
            public override string CreateInitializationCPPCode()
            {
                return "";
            }
            #endregion
        }

        [XmlType("RepeatUntil"), XmlRoot("RepeatUntil")]
        public class RepeatUntil : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string LoopExitTriggerName = "";
            [XmlAttribute()]
            public bool ForceExitOnTrigger = false;

            #region Atribute
            [XmlIgnore(), Description("Name of the trigger that if set the loop ends"), Category("Exit"), DisplayName("Exit trigger")]
            public string _LoopExitTriggerName
            {
                get { return LoopExitTriggerName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    LoopExitTriggerName = value;
                }
            }
            [XmlIgnore(), Description("If set to true, the Repeat...Until loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Repeat ... Until block (Trigger: " + LoopExitTriggerName + ")";
            }
            public override string GetIconKey()
            {
                return "repeat_until";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : RepeatUntil loops within an animation object must only have one child !");
                if (Project.ValidateVariableNameCorectness(LoopExitTriggerName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : RepeatUntil loop with invalid trigger name - should contain letter, numbers and character '_' and the first character must be a letter !");

            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "RepeatUntil"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//RepeatUntil transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ExitCondition = false;\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                string nm = "Trigger" + LoopExitTriggerName;
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                return "\n\tvoid Trigger" + LoopExitTriggerName + " ();\n";
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                return "\nvoid " + className + "::Trigger" + LoopExitTriggerName + "() { this->" + this.CppName + ".ExitCondition = true; } \n";
            }
            public override string CreateOnStartCPPCode()
            {
                return "\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            #endregion
        }

        [XmlType("DoOnceUntil"), XmlRoot("DoOnceUntil")]
        public class DoOnceUntil : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string LoopExitTriggerName = "";
            [XmlAttribute()]
            public bool ForceExitOnTrigger = true;

            #region Atribute
            [XmlIgnore(), Description("Name of the trigger that if set the loop ends"), Category("Exit"), DisplayName("Exit trigger")]
            public string _LoopExitTriggerName
            {
                get { return LoopExitTriggerName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("This field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    LoopExitTriggerName = value;
                }
            }
            [XmlIgnore(), Description("If set to true, the Do...Until loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Do Once... Until (Trigger: " + LoopExitTriggerName + ")";
            }
            public override string GetIconKey()
            {
                return "do_until";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : DoOnceUntil within an animation object must only have one child !");
                if (Project.ValidateVariableNameCorectness(LoopExitTriggerName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : DoOnceUntil with invalid trigger name - should contain letter, numbers and character '_' and the first character must be a letter !");

            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "DoOnceUntil"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//DoOnceUntil transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ExitCondition = false;\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                string nm = "Trigger" + LoopExitTriggerName;
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                return "\n\tvoid Trigger" + LoopExitTriggerName + " ();\n";
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                return "\nvoid " + className + "::Trigger" + LoopExitTriggerName + "() { this->" + this.CppName + ".ExitCondition = true; } \n";
            }
            public override string CreateOnStartCPPCode()
            {
                return "\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            #endregion
        }

        [XmlType("PopupLoop"), XmlRoot("PopupLoop")]
        public class PopupLoop : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public bool ForceExitOnTrigger = true;

            #region Atribute
            [XmlIgnore(), Description("If set to true, the Popup loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Popup Loop";
            }
            public override string GetIconKey()
            {
                return "popup_loop";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : PopupLoop within an animation object must only have one child !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "PopupLoop"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//PopupLoop transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            #endregion
        }

        [XmlType("DoOncePopupLoop"), XmlRoot("DoOncePopupLoop")]
        public class DoOncePopupLoop : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public bool ForceExitOnTrigger = true;

            #region Atribute
            [XmlIgnore(), Description("If set to true, the Popup loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Do Once Popup Loop";
            }
            public override string GetIconKey()
            {
                return "do_once_popup_loop";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : Do once PopupLoop within an animation object must only have one child !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "DoOncePopupLoop"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//DoOncePopupLoop transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            #endregion
        }

        [XmlType("WaitUntil"), XmlRoot("WaitUntil")]
        public class WaitUntil : GenericTransformation
        {
            [XmlAttribute()]
            public string LoopExitTriggerName = "";

            #region Atribute
            [XmlIgnore(), Description("Name of the trigger that if set wait ends"), Category("Exit trigger"), DisplayName("Exit trigger")]
            public string _LoopExitTriggerName
            {
                get { return LoopExitTriggerName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("This field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    LoopExitTriggerName = value;
                }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Wait Until (Trigger: " + LoopExitTriggerName + ")";
            }
            public override string GetIconKey()
            {
                return "wait_until";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Project.ValidateVariableNameCorectness(LoopExitTriggerName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : WaitUntil with invalid trigger name - should contain letter, numbers and character '_' and the first character must be a letter !");

            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false; // nu continue
            }
            public override string GetCPPClassName() { return "WaitUntil"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//WaitUntil transformation\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                string nm = "Trigger" + LoopExitTriggerName;
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                return "\n\tvoid Trigger" + LoopExitTriggerName + " ();\n";
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                return "\nvoid " + className + "::Trigger" + LoopExitTriggerName + "() { this->" + this.CppName + ".ExitCondition = true; } \n";
            }
            public override string CreateOnStartCPPCode()
            {
                return "\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            #endregion
        }

        [XmlType("IfElse"), XmlRoot("IfElse")]
        public class IfElseBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string ConditionParameterName = "";

            [XmlAttribute()]
            public bool EditModeConditionValue = true;

            #region Atribute
            [XmlIgnore(), Description("Name of the condition parameter"), Category("Branch"), DisplayName("Condition parameter name")]
            public string _ConditionParameterName
            {
                get { return ConditionParameterName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    ConditionParameterName = value;
                }
            }
            [XmlIgnore(), Description("Specify if in the edit mode condition should be considered 'True' or 'False'"), Category("Branch"), DisplayName("Edit mode condition value")]
            public bool _EditModeConditionValue
            {
                get { return EditModeConditionValue; }
                set { EditModeConditionValue = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "If..Else (Condition: " + EditModeConditionValue.ToString() + ")";
            }
            public override string GetIconKey()
            {
                return "if_else";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count == 0)
                    prj.EC.AddError("Animation - '" + animationName + "' : If..Else blocks  must contain at least one child !");
                if (Transformations.Count > 2)
                    prj.EC.AddError("Animation - '" + animationName + "' : If..Else blocks  cannot have more than two children ('then' branch and 'else' branch)");
                if ((Transformations.Count > 0) && (Transformations[0].BranchName != "Then"))
                    prj.EC.AddError("Animation - '" + animationName + "' : First brach from the If..Else block must be named 'Then' !");
                if ((Transformations.Count > 1) && (Transformations[1].BranchName != "Else"))
                    prj.EC.AddError("Animation - '" + animationName + "' : Second brach from the If..Else block must be named 'Else' !");
                if (Project.ValidateVariableNameCorectness(ConditionParameterName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : Invalid condition parameter name '" + ConditionParameterName + "'. This field must contain only letter, numbers and character '_'. The first character must be a letter !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                transToRepeat = null;
                if ((EditModeConditionValue) && (Transformations.Count > 0))
                    transToRepeat = Transformations[0];
                if ((!EditModeConditionValue) && (Transformations.Count > 1))
                    transToRepeat = Transformations[1];
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                    return false; // in simulator fac o singura tura
                return true;
            }
            public override string GetCPPClassName() { return "IfElseBlock"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//If..Else transformation";
                if (Transformations.Count > 0)
                    s += "\n\t" + this.CppName + ".then_branch = &" + Transformations[0].CppName + ";";
                else
                    s += "\n\t" + this.CppName + ".then_branch = NULL;";
                if (Transformations.Count > 1)
                    s += "\n\t" + this.CppName + ".else_branch = &" + Transformations[1].CppName + ";";
                else
                    s += "\n\t" + this.CppName + ".else_branch = NULL;";

                s += "\n\t" + this.CppName + ".IfElseCondition = true;\n";
                return s;
            }

            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(ConditionParameterName, "IfElseCondition") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                p.ParametersList.Add(new ParameterInformation(ConditionParameterName, "bool"));
                base.PopulateParameters(p);
            }

            #endregion
        }

        [XmlType("Branch"), XmlRoot("Branch")]
        public class BranchBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string BranchConditionsGroup = "";

            [XmlAttribute()]
            public string EditModeBranchExecution = "";

            #region Atribute
            [XmlIgnore(), Description("Name of branch group name"), Category("Branch"), DisplayName("Branch conditions group")]
            public string _BranchConditionsGroup
            {
                get { return BranchConditionsGroup; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    BranchConditionsGroup = value;
                }
            }
            [XmlIgnore(), Description("Name of branch to follow on edit mode"), Category("Branch"), DisplayName("Edit mode branch to execute")]
            public string _EditModeBranchExecution
            {
                get { return EditModeBranchExecution; }
                set { EditModeBranchExecution = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Branch (Class: " + BranchConditionsGroup + ") - Execute: " + EditModeBranchExecution;
            }
            public override string GetIconKey()
            {
                return "branch_block";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count == 0)
                    prj.EC.AddError("Animation - '" + animationName + "' : Branch blocks  must contain at least one child !");
                bool found_one = false;
                foreach (var t in Transformations)
                {
                    if (Project.ValidateVariableNameCorectness(t.BranchName, false) == false)
                        prj.EC.AddError("Animation - '" + animationName + "' : Branch block with a branch with an invalid name: '" + t.BranchName + "' - should contain letter, numbers and character '_' and the first character must be a letter !");
                    if (t.BranchName == EditModeBranchExecution)
                        found_one = true;
                }
                if (found_one == false)
                {
                    string res = "";
                    foreach (var t in Transformations)
                        res += t.BranchName + ",";
                    prj.EC.AddError("Animation - '" + animationName + "' : Branch block with invalid 'Edit modee branch to execute' field - should be one of the following: '" + res + "' !");
                }
                if (Project.ValidateVariableNameCorectness(BranchConditionsGroup, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : Branch block with invalid 'Branch conditions group' name - should contain letter, numbers and character '_' and the first character must be a letter !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                transToRepeat = null;
                foreach (var t in Transformations)
                    if (t.BranchName == EditModeBranchExecution)
                    {
                        transToRepeat = t;
                        break;
                    }
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                    return false; // in simulator fac o singura tura
                return true;
            }
            public override string GetCPPClassName() { return "BranchBlock"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//Branch transformation\n\t" + this.CppName + ".list = new GApp::Animations::Transformations::Transformation* [" + Transformations.Count.ToString() + "];";
                s += "\n\t" + this.CppName + ".NumberOfTransformations = " + Transformations.Count.ToString() + ";";
                int index = 0;
                foreach (var t in Transformations)
                {
                    s += "\n\t" + this.CppName + ".list[" + index.ToString() + "] = &" + t.CppName + ";";
                    index++;
                }
                s += "\n\t" + this.CppName + ".BranchIndex = -1;\n\t" + this.CppName + ".trans = NULL;\n";
                return s;
            }

            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(BranchConditionsGroup, "BranchIndex") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                p.ParametersList.Add(new ParameterInformation(BranchConditionsGroup, "int", BranchConditionsGroup + "Enum"));
                base.PopulateParameters(p);
            }
            public override void CreateGACEnums(Dictionary<string, List<string>> enums)
            {
                List<string> l = new List<string>();
                foreach (var t in Transformations)
                    l.Add(t.BranchName);
                enums[this.BranchConditionsGroup] = l;
            }

            #endregion
        }

        public enum TransformationBlockType
        {
            Sequance,
            Parallel,
        };

        [XmlType("Block"), XmlRoot("Block")]
        public class TransformationBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block
            [XmlAttribute()]
            public TransformationBlockType BlockType = TransformationBlockType.Sequance;

            #region Atribute
            [XmlIgnore(), Description("Describes how the elements within the block will be executed"), Category("Block"), DisplayName("Block type")]
            public TransformationBlockType _BlockType
            {
                get { return BlockType; }
                set { BlockType = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                if (BlockType == TransformationBlockType.Parallel)
                    return "Parallel block";
                else
                    return "Sequance block";
            }
            public override string GetIconKey()
            {
                if (BlockType == TransformationBlockType.Parallel)
                    return "parralel_block";
                else
                    return "sequance_block";
            }
            #endregion

            #region Dynamic Execution
            private int transformation_index, finished;
            private bool[] useInParalelMode = new bool[128];
            protected override void OnInit()
            {
                if (BlockType == TransformationBlockType.Parallel)
                {
                    finished = Transformations.Count;
                    for (int tr = 0; tr < finished; tr++)
                    {
                        useInParalelMode[tr] = true;
                        Transformations[tr].Init();
                    }
                }
                else
                {
                    transformation_index = 0;
                    Transformations[transformation_index].Init();
                }
            }
            protected override bool OnUpdate()
            {
                if (BlockType == TransformationBlockType.Parallel)
                {
                    if (finished > 0)
                    {
                        for (int tr = 0; tr < Transformations.Count; tr++)
                        {
                            if ((useInParalelMode[tr]) && (Transformations[tr].Update() == false))
                            {
                                useInParalelMode[tr] = false;
                                finished--;
                            }
                        }
                    }
                    if (finished <= 0)
                        return false;
                }
                else
                {
                    if (Transformations[transformation_index].Update() == false)
                    {
                        transformation_index++;
                        if (transformation_index >= Transformations.Count)
                            return false;
                        Transformations[transformation_index].Init();
                    }
                }
                return true;
            }
            public override string GetCPPClassName() { if (BlockType == TransformationBlockType.Parallel) return "Parallel"; else return "Sequance"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + BlockType.ToString() + " transformation\n\t" + this.CppName + ".list = new GApp::Animations::Transformations::Transformation* [" + Transformations.Count.ToString() + "];";
                s += "\n\t" + this.CppName + ".NumberOfTransformations = " + Transformations.Count.ToString() + ";";
                int index = 0;
                foreach (var t in Transformations)
                {
                    s += "\n\t" + this.CppName + ".list[" + index.ToString() + "] = &" + t.CppName + ";";
                    index++;
                }
                return s + "\n";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count == 0)
                    prj.EC.AddError("Animation - '" + animationName + "' : Parralel or Sequence blocks have to have at least one child !");
            }
            #endregion
        }

        [XmlType("LinearTransformation"), XmlRoot("LinearTransformation")]
        public class LinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float Start = 0.0f;
            [XmlAttribute()]
            public float End = 1.0f;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_Start = "", UserValue_End = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Start")]
            public string _Start
            {
                get { return Project.ProcentToString(Start); }
                set { Start = Project.StringToProcent(value, 0, 100, Start, true); }
            }
            [XmlIgnore(), Description("End value (percentage)"), Category("Transformation"), DisplayName("End")]
            public string _End
            {
                get { return Project.ProcentToString(End); }
                set { End = Project.StringToProcent(value, 0, 100, End, true); }
            }
            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start")]
            public string _UserValue_Start
            {
                get { return UserValue_Start; }
                set { UserValue_Start = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End")]
            public string _UserValue_End
            {
                get { return UserValue_End; }
                set { UserValue_End = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected float step_value, current_value;
            protected bool BackAndForward = false;
            protected override void OnInit()
            {
                steps_count = Steps;
                if (BackAndForward)
                    steps_count *= 2;
                step_value = (End - Start) / Steps;
                current_value = Start;
                OnUpdateElement(ElementObject);
            }
            protected override bool OnUpdate()
            {
                if (BackAndForward)
                {
                    if (steps_count > Steps)
                        current_value += step_value;
                    else
                        current_value -= step_value;
                }
                else
                {
                    current_value += step_value;
                }
                steps_count--;
                OnUpdateElement(ElementObject);
                return steps_count > 0;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "From " + Project.ProcentToString(Start) + " to " + Project.ProcentToString(End) + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Start = " + this.Start.ToString() + ";";
                s += "\n\t" + this.CppName + ".End = " + this.End.ToString() + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Start, "Start") + GetOnStartFieldInit(UserValue_End, "End") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Start.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Start, "float"));
                if (UserValue_End.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_End, "float"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("AlphaBlendingLinear"), XmlRoot("AlphaBlendingLinear")]
        public class AlphaBlendingLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.AlphaBlending, (int)ElementObject.ExecutionContext.ColorBlending, current_value);
            }
        }

        [XmlType("AlphaBlendingForwardAndBack"), XmlRoot("AlphaBlendingForwardAndBack")]
        public class AlphaBlendingForwardAndBackTransformation : LinearTransformation
        {
            public AlphaBlendingForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.AlphaBlending, (int)ElementObject.ExecutionContext.ColorBlending, current_value);
            }
        }

        [XmlType("ScaleLinear"), XmlRoot("ScaleLinear")]
        public class ScaleLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
                ElementObject.ExecutionContext.ScaleHeight = current_value;
            }
        }

        [XmlType("ScaleForwardAndBack"), XmlRoot("ScaleForwardAndBack")]
        public class ScaleForwardAndBackTransformation : LinearTransformation
        {
            public ScaleForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
                ElementObject.ExecutionContext.ScaleHeight = current_value;
            }
        }

        [XmlType("ScaleWidthLinear"), XmlRoot("ScaleWidthLinear")]
        public class ScaleWidthLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
            }
        }

        [XmlType("ScaleWidthForwardAndBack"), XmlRoot("ScaleWidthForwardAndBack")]
        public class ScaleWidthForwardAndBackTransformation : LinearTransformation
        {
            public ScaleWidthForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
            }
        }

        [XmlType("ScaleHeightLinear"), XmlRoot("ScaleHeightLinear")]
        public class ScaleHeightLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleHeight = current_value;
            }
        }

        [XmlType("ScaleHeightForwardAndBack"), XmlRoot("ScaleHeightForwardAndBack")]
        public class ScaleHeightForwardAndBackTransformation : LinearTransformation
        {
            public ScaleHeightForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleHeight = current_value;
            }
        }

        [XmlType("ColorBlendingGenericLinearTransformation"), XmlRoot("ColorBlendingGenericLinearTransformation")]
        public class ColorBlendingGenericLinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int StartColor = -1;
            [XmlAttribute()]
            public int EndColor = 0;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_StartColor = "", UserValue_EndColor = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start color"), Category("Transformation"), DisplayName("Start color")]
            public Color _StartColor
            {
                get { return System.Drawing.Color.FromArgb(StartColor); }
                set { StartColor = value.ToArgb(); }
            }
            [XmlIgnore(), Description("End color"), Category("Transformation"), DisplayName("End color")]
            public Color _EndColor
            {
                get { return System.Drawing.Color.FromArgb(EndColor); }
                set { EndColor = value.ToArgb(); }
            }
            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start color")]
            public string _UserValue_StartColor
            {
                get { return UserValue_StartColor; }
                set { UserValue_StartColor = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End color")]
            public string _UserValue_EndColor
            {
                get { return UserValue_EndColor; }
                set { UserValue_EndColor = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected Color c_work;
            protected bool BackAndForward = false;
            private int GetChannel(float start, float end, int index)
            {
                return (int)(((end - start) / Steps) * index + start);
            }
            private Color ComputeColor(int index)
            {
                Color s = Color.FromArgb(StartColor);
                Color e = Color.FromArgb(EndColor);
                return Color.FromArgb(GetChannel(s.A, e.A, index), GetChannel(s.R, e.R, index), GetChannel(s.G, e.G, index), GetChannel(s.B, e.B, index));
            }
            protected override void OnInit()
            {
                steps_count = Steps;
                if (BackAndForward)
                    steps_count *= 2;
                ElementObject.ExecutionContext.ColorBlending = (uint)StartColor;
            }
            protected override bool OnUpdate()
            {
                if (BackAndForward)
                {
                    if (steps_count > Steps)
                        c_work = ComputeColor(Steps * 2 - steps_count);
                    else
                        c_work = ComputeColor(steps_count);
                }
                else
                {
                    c_work = ComputeColor(Steps - steps_count);
                }
                steps_count--;
                ElementObject.ExecutionContext.ColorBlending = (uint)c_work.ToArgb();

                return steps_count > 0;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "From " + StartColor.ToString() + " to " + EndColor.ToString() + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Start = 0x" + this.StartColor.ToString("X8") + ";";
                s += "\n\t" + this.CppName + ".End = 0x" + this.EndColor.ToString("X8") + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_StartColor, "Start") + GetOnStartFieldInit(UserValue_EndColor, "End") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_StartColor.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartColor, "unsigned int", "Color"));
                if (UserValue_EndColor.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndColor, "unsigned int", "Color"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("ColorBlendLinear"), XmlRoot("ColorBlendLinear")]
        public class ColorBlendLinearTransformation : ColorBlendingGenericLinearTransformation
        {
        }

        [XmlType("ColorBlendForwardAndBack"), XmlRoot("ColorBlendForwardAndBack")]
        public class ColorBlendForwardAndBackTransformation : ColorBlendingGenericLinearTransformation
        {
            public ColorBlendForwardAndBackTransformation() { BackAndForward = true; }
        }

        [XmlType("MoveRelativeLinear"), XmlRoot("MoveRelativeLinear")]
        public class MoveRelativeLinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float OffsetX = 0.0f;
            [XmlAttribute()]
            public float OffsetY = 1.0f;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_OffsetX = "", UserValue_OffsetY = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Offset X")]
            public string _OffsetX
            {
                get { return GetLocationValue(OffsetX); }
                set { OffsetX = SetLocationValue(value, OffsetX); }
            }
            [XmlIgnore(), Description("End value (percentage)"), Category("Transformation"), DisplayName("Offset Y")]
            public string _OffsetY
            {
                get { return GetLocationValue(OffsetY); }
                set { OffsetY = SetLocationValue(value, OffsetY); }
            }
            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset X")]
            public string _UserValue_OffsetX
            {
                get { return UserValue_OffsetX; }
                set { UserValue_OffsetX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset Y")]
            public string _UserValue_OffsetY
            {
                get { return UserValue_OffsetY; }
                set { UserValue_OffsetY = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected override void OnInit()
            {
                steps_count = Steps;
            }
            protected override bool OnUpdate()
            {
                steps_count--;
                ElementObject.ExecutionContext.X_Percentage += (OffsetX / (float)Steps);
                ElementObject.ExecutionContext.Y_Percentage += (OffsetY / (float)Steps);
                return steps_count > 0;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Move to (" + _OffsetX + "," + _OffsetY + ") " + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".OffsetX = " + this.OffsetX.ToString() + ";";
                s += "\n\t" + this.CppName + ".OffsetY = " + this.OffsetY.ToString() + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_OffsetX, "OffsetX") + GetOnStartFieldInit(UserValue_OffsetY, "OffsetY") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_OffsetX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetX, "float"));
                if (UserValue_OffsetY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetY, "float"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("MoveAbsoluteLinear"), XmlRoot("MoveAbsoluteLinear")]
        public class MoveAbsoluteLinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float X = 0.5f;
            [XmlAttribute()]
            public float Y = 0.5f;
            [XmlAttribute()]
            public int StepSize = 1;
            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "", UserValue_StepSize = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value, X); }
            }
            [XmlIgnore(), Description("End value (percentage)"), Category("Transformation"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value, Y); }
            }
            [XmlIgnore(), Description("Set size (units)"), Category("Transformation"), DisplayName("Step size")]
            public string _StepSize
            {
                get { return StepSize.ToString() + " units"; }
                set
                {
                    string ss = "";
                    for (int tr = 0; tr < value.Length; tr++)
                    {
                        if ((value[tr] < '0') || (value[tr] > '9'))
                            break;
                        ss += value[tr];
                    }
                    int res = 0;
                    if (int.TryParse(ss, out res) == false)
                    {
                        MessageBox.Show("Unable to translate '" + value + "' to a valid number !");
                        return;
                    }
                    if (res < 1)
                    {
                        MessageBox.Show("Step size must be at least one unit !");
                        return;
                    }
                    StepSize = res;
                }
            }



            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Step size")]
            public string _UserValue_StepSize
            {
                get { return UserValue_StepSize; }
                set { UserValue_StepSize = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected float addX, addY;
            protected override void OnInit()
            {
                float dist, f_steps_count;
                //float dist = (float)Math.Sqrt((X - ElementObject.ExecutionContext.X) * (X - ElementObject.ExecutionContext.X) + (Y - ElementObject.ExecutionContext.Y) * (Y - ElementObject.ExecutionContext.Y));

                int design_resolution_width = 1;
                int design_resolution_height = 1;

                Project.SizeToValues(GenericTransformation.Prj.DesignResolution, ref design_resolution_width, ref design_resolution_height);
                dist = (float)Math.Sqrt((X - ElementObject.ExecutionContext.X_Percentage) * (float)design_resolution_width * (X - ElementObject.ExecutionContext.X_Percentage) * (float)design_resolution_width + (Y - ElementObject.ExecutionContext.Y_Percentage) * (float)design_resolution_height * (Y - ElementObject.ExecutionContext.Y_Percentage) * (float)design_resolution_height);

                f_steps_count = dist / (float)StepSize;
                addX = (X - ElementObject.ExecutionContext.X_Percentage) / f_steps_count;
                addY = (Y - ElementObject.ExecutionContext.Y_Percentage) / f_steps_count;
                steps_count = (int)f_steps_count;

            }
            protected override bool OnUpdate()
            {
                steps_count--;
                if (steps_count <= 0)
                {
                    ElementObject.ExecutionContext.X_Percentage = X;
                    ElementObject.ExecutionContext.Y_Percentage = Y;
                }
                else
                {
                    ElementObject.ExecutionContext.X_Percentage += addX;
                    ElementObject.ExecutionContext.Y_Percentage += addY;
                }
                return steps_count > 0;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Move to (" + _X + "," + _Y + ") " + " StepSize = " + _StepSize;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".X = " + this.X.ToString() + ";";
                s += "\n\t" + this.CppName + ".Y = " + this.Y.ToString() + ";";
                s += "\n\t" + this.CppName + ".StepSize = " + this.StepSize.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_X, "X") + GetOnStartFieldInit(UserValue_Y, "Y") + GetOnStartFieldInit(UserValue_StepSize, "StepSize") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                if (UserValue_StepSize.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StepSize, "int"));

                base.PopulateParameters(p);
            }
        }

        [XmlType("QuadraticBezier"), XmlRoot("QuadraticBezier")]
        public class QuadraticBezierTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float StartPointX = 0.0f;
            [XmlAttribute()]
            public float StartPointY = 1.0f;
            [XmlAttribute()]
            public float EndPointX = 1.0f;
            [XmlAttribute()]
            public float EndPointY = 1.0f;
            [XmlAttribute()]
            public float ControlPointX = 0.5f;
            [XmlAttribute()]
            public float ControlPointY = 0.5f;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_StartPointX = "", UserValue_StartPointY = "", UserValue_EndPointX = "", UserValue_EndPointY = "", UserValue_ControlPointX = "", UserValue_ControlPointY = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start value"), Category("Transformation"), DisplayName("Start Point X")]
            public string _StartPointX
            {
                get { return GetLocationValue(StartPointX); }
                set { StartPointX = SetLocationValue(value, StartPointX); }
            }
            [XmlIgnore(), Description("Start value"), Category("Transformation"), DisplayName("Start Point Y")]
            public string _StartPointY
            {
                get { return GetLocationValue(StartPointY); }
                set { StartPointY = SetLocationValue(value, StartPointY); }
            }
            [XmlIgnore(), Description("Control value"), Category("Transformation"), DisplayName("Control Point X")]
            public string _ControlPointX
            {
                get { return GetLocationValue(ControlPointX); }
                set { ControlPointX = SetLocationValue(value, ControlPointX); }
            }
            [XmlIgnore(), Description("Control value"), Category("Transformation"), DisplayName("Control Point Y")]
            public string _ControlPointY
            {
                get { return GetLocationValue(ControlPointY); }
                set { ControlPointY = SetLocationValue(value, ControlPointY); }
            }
            [XmlIgnore(), Description("End value"), Category("Transformation"), DisplayName("End Point X")]
            public string _EndPointX
            {
                get { return GetLocationValue(EndPointX); }
                set { EndPointX = SetLocationValue(value, EndPointX); }
            }
            [XmlIgnore(), Description("End value"), Category("Transformation"), DisplayName("End Point Y")]
            public string _EndlPointY
            {
                get { return GetLocationValue(EndPointY); }
                set { EndPointY = SetLocationValue(value, EndPointY); }
            }

            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start Point X")]
            public string _UserValue_StartPointX
            {
                get { return UserValue_StartPointX; }
                set { UserValue_StartPointX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start Point Y")]
            public string _UserValue_StartPointY
            {
                get { return UserValue_StartPointY; }
                set { UserValue_StartPointY = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Control Point X")]
            public string _UserValue_ControlPointX
            {
                get { return UserValue_ControlPointX; }
                set { UserValue_ControlPointX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Control Point Y")]
            public string _UserValue_ControlPointY
            {
                get { return UserValue_ControlPointY; }
                set { UserValue_ControlPointY = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End Point X")]
            public string _UserValue_EndPointX
            {
                get { return UserValue_EndPointX; }
                set { UserValue_EndPointX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End Point Y")]
            public string _UserValue_EndPointY
            {
                get { return UserValue_EndPointY; }
                set { UserValue_EndPointY = value; }
            }


            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected override void OnInit()
            {
                steps_count = 0;
            }
            protected override bool OnUpdate()
            {
                steps_count++;
                //x = (1 - t) * (1 - t) * p[0].x + 2 * (1 - t) * t * p[1].x + t * t * p[2].x;
                //y = (1 - t) * (1 - t) * p[0].y + 2 * (1 - t) * t * p[1].y + t * t * p[2].y;
                float t = ((float)steps_count) / ((float)Steps);
                ElementObject.ExecutionContext.X_Percentage = (1 - t) * (1 - t) * StartPointX + 2 * (1 - t) * t * ControlPointX + t * t * EndPointX;
                ElementObject.ExecutionContext.Y_Percentage = (1 - t) * (1 - t) * StartPointY + 2 * (1 - t) * t * ControlPointY + t * t * EndPointY;

                return steps_count < Steps;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Bezier Move from (" + _StartPointX + "," + _StartPointY + ") to (" + _EndPointX + "," + _EndlPointY + ") in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".StartPointX = " + this.StartPointX.ToString() + ";";
                s += "\n\t" + this.CppName + ".StartPointY = " + this.StartPointY.ToString() + ";";
                s += "\n\t" + this.CppName + ".ControlPointX = " + this.ControlPointX.ToString() + ";";
                s += "\n\t" + this.CppName + ".ControlPointY = " + this.ControlPointY.ToString() + ";";
                s += "\n\t" + this.CppName + ".EndPointX = " + this.EndPointX.ToString() + ";";
                s += "\n\t" + this.CppName + ".EndPointY = " + this.EndPointY.ToString() + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_StartPointX, "StartPointX") + GetOnStartFieldInit(UserValue_StartPointY, "StartPointY") +
                       GetOnStartFieldInit(UserValue_ControlPointX, "ControlPointX") + GetOnStartFieldInit(UserValue_ControlPointY, "ControlPointY") +
                       GetOnStartFieldInit(UserValue_EndPointX, "StartEndX") + GetOnStartFieldInit(UserValue_EndPointY, "EndPointY") +
                       GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_StartPointX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartPointX, "float"));
                if (UserValue_StartPointY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartPointY, "float"));
                if (UserValue_ControlPointX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ControlPointX, "float"));
                if (UserValue_ControlPointY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ControlPointY, "float"));
                if (UserValue_EndPointX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndPointX, "float"));
                if (UserValue_EndPointY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndPointY, "float"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("VisibleState"), XmlRoot("VisibleState")]
        public class VisibleStateTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public bool Visible = true;
            [XmlAttribute()]
            public string UserValue_Visible = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Visible")]
            public bool _Visible
            {
                get { return Visible; }
                set { Visible = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Visible")]
            public string _UserValue_Visible
            {
                get { return UserValue_Visible; }
                set { UserValue_Visible = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected float step_value, current_value;
            protected bool BackAndForward = false;
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.Visible = Visible;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set visible state: " + Visible.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Visible = " + this.Visible.ToString().ToLower() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Visible, "Visible") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Visible.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Visible, "bool"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("ButtonEnableState"), XmlRoot("ButtonEnableState")]
        public class ButtonEnableTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public bool Enabled = true;
            [XmlAttribute()]
            public string UserValue_Enabled = "";

            #region Atribute
            [XmlIgnore(), Description("Indicates if the button state should be set to enable or not"), Category("Button"), DisplayName("Enabled")]
            public bool _Visible
            {
                get { return Enabled; }
                set { Enabled = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Enabled")]
            public string _UserValue_Visible
            {
                get { return UserValue_Enabled; }
                set { UserValue_Enabled = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                SimpleButtonElement sbe = ElementObject as SimpleButtonElement;
                if (sbe != null)
                {
                    sbe.Enabled = Enabled;
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set button enable state: " + Enabled.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Enabled = " + this.Enabled.ToString().ToLower() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Enabled, "Enabled") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Enabled.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Enabled, "bool"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("SetNewAbsolutePosition"), XmlRoot("SetNewAbsolutePosition")]
        public class SetNewAbsolutePositionTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float X = 0;
            [XmlAttribute()]
            public float Y = 0;

            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "";

            #region Atribute
            [XmlIgnore(), Description("X-axes"), Category("Transformation"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value, X); }
            }
            [XmlIgnore(), Description("Y-axes"), Category("Transformation"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value, Y); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.X_Percentage = X;
                ElementObject.ExecutionContext.Y_Percentage = Y;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set new position (" + _X + "," + _Y + ")";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".X = " + this.X.ToString() + ";";
                s += "\n\t" + this.CppName + ".Y = " + this.Y.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_X, "X") + GetOnStartFieldInit(UserValue_Y, "Y") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("SetNewRelativePosition"), XmlRoot("SetNewRelativePosition")]
        public class SetNewRelativePositionTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float OffsetX = 0;
            [XmlAttribute()]
            public float OffsetY = 0;

            [XmlAttribute()]
            public string UserValue_OffsetX = "", UserValue_OffsetY = "";

            #region Atribute
            [XmlIgnore(), Description("X-axes"), Category("Transformation"), DisplayName("Offset X")]
            public string _X
            {
                get { return GetLocationValue(OffsetX); }
                set { OffsetX = SetLocationValue(value, OffsetX); }
            }
            [XmlIgnore(), Description("Y-axes"), Category("Transformation"), DisplayName("Offset Y")]
            public string _Y
            {
                get { return GetLocationValue(OffsetY); }
                set { OffsetY = SetLocationValue(value, OffsetY); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset X")]
            public string _UserValue_X
            {
                get { return UserValue_OffsetX; }
                set { UserValue_OffsetX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset Y")]
            public string _UserValue_Y
            {
                get { return UserValue_OffsetY; }
                set { UserValue_OffsetY = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.X_Percentage += OffsetX;
                ElementObject.ExecutionContext.Y_Percentage += OffsetY;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Move to offset (" + _X + "," + _Y + ")";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".OffsetX = " + this.OffsetX.ToString() + ";";
                s += "\n\t" + this.CppName + ".OffsetY = " + this.OffsetY.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_OffsetX, "OffsetX") + GetOnStartFieldInit(UserValue_OffsetY, "OffsetY") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_OffsetX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetX, "float"));
                if (UserValue_OffsetY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetY, "float"));
                base.PopulateParameters(p);
            }
        }


        [XmlType("ColorBlendState"), XmlRoot("ColorBlendState")]
        public class ColorBlendStateTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int NewColor = -1;
            [XmlAttribute()]
            public string UserValue_Color = "";

            #region Atribute
            [XmlIgnore(), Description("Color)"), Category("Transformation"), DisplayName("Color")]
            public Color _Color
            {
                get { return System.Drawing.Color.FromArgb(NewColor); }
                set { NewColor = value.ToArgb(); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_Visible
            {
                get { return UserValue_Color; }
                set { UserValue_Color = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.ColorBlending = (uint)NewColor;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set color: " + Color.FromArgb(NewColor).ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Color = " + this.NewColor.ToString().ToLower() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Color, "Color") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Color.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Color, "unsigned int", "Color"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("AlphaBlendingState"), XmlRoot("AlphaBlendingState")]
        public class AlphaBlendingStateTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float NewAlpha = 1.0f;
            [XmlAttribute()]
            public string UserValue_Alpha = "";

            #region Atribute
            [XmlIgnore(), Description("Alpha"), Category("Transformation"), DisplayName("Alpha")]
            public string _Alpha
            {
                get { return Project.ProcentToString(NewAlpha); }
                set { NewAlpha = Project.StringToProcent(value, 0, 100, NewAlpha, true); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Alpha")]
            public string _UserValue_Alpha
            {
                get { return UserValue_Alpha; }
                set { UserValue_Alpha = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.AlphaBlending, (int)ElementObject.ExecutionContext.ColorBlending, NewAlpha);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set alpha: " + _Alpha;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Alpha = " + this.NewAlpha.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Alpha, "Alpha") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Alpha.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Alpha, "float"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("SetNewText"), XmlRoot("SetNewText")]
        public class SetNewTextTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Text = "";
            [XmlAttribute()]
            public string StringResource = "";
            [XmlAttribute()]
            public bool UseStringResource = false;
            [XmlAttribute()]
            public string UserValue_Text = "";

            #region Atribute
            [XmlIgnore(), Category("Text"), DisplayName("Text")]
            public string _Text
            {
                get { return Text; }
                set { Text = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Resource"), Editor(typeof(StringSelectorEditor), typeof(UITypeEditor))]
            public string _StringResource
            {
                get { return StringResource; }
                set { StringResource = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Use string resources")]
            public bool _UseStringResource
            {
                get { return UseStringResource; }
                set { UseStringResource = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Text")]
            public string _UserValue_Text
            {
                get { return UserValue_Text; }
                set { UserValue_Text = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    if (UseStringResource)
                        ((AnimO.TextElement)ElementObject).tp.SetText(StringResource, true, GenericElement.CurrentAppResources);
                    else
                        ((AnimO.TextElement)ElementObject).tp.SetText(Text, false, GenericElement.CurrentAppResources);
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                if (UseStringResource)
                    return "From resources: " + StringResource;
                else
                    return Text;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                if (UserValue_Text.Length > 0)
                {
                    s += "\n\t" + CppName + ".Text.Set(param_" + UserValue_Text + ");";
                }
                else
                {
                    if (this.UseStringResource)
                        s += "\n\t" + CppName + ".Text.Set(Res.Strings." + this.StringResource + ");";
                    else
                        s += "\n\t" + CppName + ".Text.Set(\"" + this.Text + "\");";
                }
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Text, "Text") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Text.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Text, "GApp::Utils::String", "string"));
                base.PopulateParameters(p);
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (this.UseStringResource)
                {
                    if (resources.Strings.ContainsKey(this.StringResource) == false)
                        prj.EC.AddError("Animation '" + animationName + "' has a SetText transformation with an invalid string: '" + StringResource + "' !");
                }
            }
        }

        [XmlType("SetNewImage"), XmlRoot("SetNewImage")]
        public class SetNewImageTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Image = "";
            [XmlAttribute()]
            public string UserValue_Image = "";

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Image"), Editor(typeof(ImageSelectorEditor), typeof(UITypeEditor))]
            public string _Image
            {
                get { return Image; }
                set { Image = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Image")]
            public string _UserValue_Image
            {
                get { return UserValue_Image; }
                set { UserValue_Image = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (TransEvent != null)
                    TransEvent.OnEvent(this);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return Image;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Image = Res.Images." + this.Image + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Image, "Image") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Image.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Image, "GApp::Resources::Bitmap *", "Bitmap"));
                base.PopulateParameters(p);
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (resources.Images.ContainsKey(this.Image) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an unknwon image: '" + Image + "' !");
            }
            public void SetImage(Bitmap bmp)
            {
                if (ElementObject.GetType() == typeof(AnimO.ImageElement))
                {
                    ((AnimO.ImageElement)ElementObject).ExecutionContext.Image = bmp;
                }
            }
        }

        [XmlType("ImageIndexGenericTransformation"), XmlRoot("ImageIndexGenericTransformation")]
        public class ImageIndexGenericTransformation : GenericElementTransformation
        {
            protected bool SetImageIndex(int idx)
            {
                if (ElementObject.GetType() != typeof(AnimO.ImageElement))
                    return false;

                AnimO.ImageElement img = ((AnimO.ImageElement)ElementObject);
                List<string> images = Project.StringListToList(img.Images, ';');
                if ((idx >= 0) && (idx < images.Count))
                {
                    string imgName = images[idx];
                    if (AnimO.GenericElement.CurrentAppResources.Images.ContainsKey(imgName))
                    {
                        img.ExecutionContext.Image = AnimO.GenericElement.CurrentAppResources.Images[imgName].Picture;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (elements.ContainsKey(this.Element))
                {
                    GenericElement el = elements[this.Element];
                    if (el.GetType() != typeof(AnimO.ImageElement))
                    {
                        prj.EC.AddError("Animation '" + animationName + "' has an 'SetImageIndex' transformation for a non Image element: '" + this.Element + "' !");
                        return;
                    }
                    if (((ImageElement)el).HasMultipleImages() == false)
                    {
                        prj.EC.AddError("Set image index requires a multi image elements : '" + this.Element + "' - in animation: " + animationName + " !");
                        return;
                    }
                }
            }
        }

        [XmlType("SetImageIndex"), XmlRoot("SetImageIndex")]
        public class SetImageIndexTransformation : ImageIndexGenericTransformation
        {
            [XmlAttribute()]
            public int ImageIndex = 0;
            [XmlAttribute()]
            public string UserValue_ImageIndex = "";

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Image Index")]
            public int _ImageIndex
            {
                get { return ImageIndex; }
                set { if (value >= 0) ImageIndex = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Image Index")]
            public string _UserValue_ImageIndex
            {
                get { return UserValue_ImageIndex; }
                set { UserValue_ImageIndex = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                SetImageIndex(ImageIndex);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return ImageIndex.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".ImageIndex = " + this.ImageIndex.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_ImageIndex, "ImageIndex") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_ImageIndex.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ImageIndex, "int", "int"));
                base.PopulateParameters(p);
            }

        }


        [XmlType("ImageIndexGenericLinear"), XmlRoot("ImageIndexGenericLinear")]
        public class ImageIndexLinearGenericTransformation : ImageIndexGenericTransformation
        {
            [XmlAttribute()]
            public int StartIndex = 0;
            [XmlAttribute()]
            public int EndIndex = 0;
            [XmlAttribute()]
            public int Step = 1;
            [XmlAttribute()]
            public int FramesInterval = 1;

            [XmlAttribute()]
            public string UserValue_StartIndex = "", UserValue_EndIndex = "", UserValue_Step = "", UserValue_FramesInterval = "";

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Start Index"), Description("Start index (image index)")]
            public int _StartIndex
            {
                get { return StartIndex; }
                set { if (value >= 0) StartIndex = value; }
            }
            [XmlIgnore(), Category("Image"), DisplayName("End Index"), Description("End index (image index)")]
            public int _EndIndex
            {
                get { return EndIndex; }
                set { if (value >= 0) EndIndex = value; }
            }
            [XmlIgnore(), Category("Image"), DisplayName("Frames Interval"), Description("Number of frames that need to pas before changing an image index")]
            public int _FramesInterval
            {
                get { return FramesInterval; }
                set { if (value >= 1) FramesInterval = value; }
            }

            [XmlIgnore(), Category("Image"), DisplayName("Step"), Description("Value to increase/decrease image index (from start to end)")]
            public int _Step
            {
                get { return Step; }
                set { if (value >= 1) Step = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start Index")]
            public string _UserValue_StartIndex
            {
                get { return UserValue_StartIndex; }
                set { UserValue_StartIndex = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End Index")]
            public string _UserValue_EndIndex
            {
                get { return UserValue_EndIndex; }
                set { UserValue_EndIndex = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Step")]
            public string _UserValue_Step
            {
                get { return UserValue_Step; }
                set { UserValue_Step = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Frames Interval")]
            public string _UserValue_FramesInterval
            {
                get { return UserValue_FramesInterval; }
                set { UserValue_FramesInterval = value; }
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "From " + StartIndex.ToString() + " to " + EndIndex.ToString() + ", Step:" + Step.ToString() + ", FramesInterval:" + FramesInterval.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".StartIndex = " + this.StartIndex.ToString() + ";";
                s += "\n\t" + this.CppName + ".EndIndex = " + this.EndIndex.ToString() + ";";
                s += "\n\t" + this.CppName + ".Step = " + this.Step.ToString() + ";";
                s += "\n\t" + this.CppName + ".FramesInterval = " + this.FramesInterval.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_StartIndex, "StartIndex") +
                        GetOnStartFieldInit(UserValue_EndIndex, "EndIndex") +
                        GetOnStartFieldInit(UserValue_Step, "Step") +
                        GetOnStartFieldInit(UserValue_FramesInterval, "FramesInterval") +
                        base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_StartIndex.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartIndex, "int", "int"));
                if (UserValue_EndIndex.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndIndex, "int", "int"));
                if (UserValue_Step.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Step, "int", "int"));
                if (UserValue_FramesInterval.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_FramesInterval, "int", "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("ImageIndexLinear"), XmlRoot("ImageIndexLinear")]
        public class ImageIndexLinearTransformation : ImageIndexLinearGenericTransformation
        {
            int currentIndex, timer;
            #region Dynamic Execution
            protected override void OnInit()
            {
                SetImageIndex(StartIndex);
                currentIndex = StartIndex;
                timer = FramesInterval;
            }
            protected override bool OnUpdate()
            {
                timer--;
                if (timer > 0)
                    return true;
                timer = FramesInterval;
                if (currentIndex == EndIndex)
                    return false;
                if (currentIndex < EndIndex)
                {
                    currentIndex += Step;
                    if (currentIndex > EndIndex)
                        currentIndex = EndIndex;
                }
                else
                {
                    currentIndex -= Step;
                    if (currentIndex < EndIndex)
                        currentIndex = EndIndex;
                }
                SetImageIndex(currentIndex);
                return true;
            }
            #endregion
        }

        [XmlType("TextFlow"), XmlRoot("TextFlow")]
        public class TextFlowTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int FramesUpdate = 1;
            [XmlAttribute()]
            public string UserValue_FramesUpdate = "";

            #region Atribute
            [XmlIgnore(), Category("Flow"), DisplayName("Update time"), Description("Number of frames that need to pass to display another character from the text (minimal value is 1)")]
            public int _FramesUpdate
            {
                get { return FramesUpdate; }
                set { if (value >= 1) FramesUpdate = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Update time")]
            public string _UserValue_FramesUpdate
            {
                get { return UserValue_FramesUpdate; }
                set { UserValue_FramesUpdate = value; }
            }
            #endregion

            #region Dynamic Execution
            private int time, poz;
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    ((AnimO.TextElement)ElementObject).tp.SetCharactesVisibility(0, 0x7fffffff, false);
                    time = FramesUpdate;
                    poz = 0;
                }
            }
            protected override bool OnUpdate()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    time--;
                    if (time == 0)
                    {
                        time = FramesUpdate;
                        TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                        poz++;
                        tp.SetCharactesVisibility(0, poz, true);
                        if (poz >= tp.GetTextLength())
                            return false;
                        return true;
                    }
                    return true;
                }
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Every " + FramesUpdate.ToString() + " frame(s)";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".FramesUpdate = " + FramesUpdate.ToString() + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_FramesUpdate, "FramesUpdate") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_FramesUpdate.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_FramesUpdate, "int"));
                base.PopulateParameters(p);
            }

        }

        [XmlType("TextCharacterVisibility"), XmlRoot("TextCharacterVisibility")]
        public class TextCharacterVisibilityTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int Start = 0;
            [XmlAttribute()]
            public int End = -1;
            [XmlAttribute()]
            public int Step = 1;
            [XmlAttribute()]
            public bool Visibility = true;

            [XmlAttribute()]
            public string UserValue_Start = "", UserValue_End = "", UserValue_Step = "", UserValue_Visibility = "";

            #region Atribute
            [XmlIgnore(), Category("Characters"), DisplayName("Start character index"), Description("Index of the first character that will be changed ! It must be bigger or equal to 0.")]
            public int _Start
            {
                get { return Start; }
                set { if (value >= 0) Start = value; }
            }
            [XmlIgnore(), Category("Characters"), DisplayName("End character index"), Description("Index of the last character that will be changed ! If negative it is consider to be an index from the right side of the string (-1 means last character)")]
            public int _End
            {
                get { return End; }
                set { End = value; }
            }
            [XmlIgnore(), Category("Characters"), DisplayName("Step"), Description("Step to jump to the next character staring from 'Start' until 'End'. Must be bigger than 0.")]
            public int _Step
            {
                get { return Step; }
                set { if (value > 0) Step = value; }
            }
            [XmlIgnore(), Category("Characters"), DisplayName("Visibility"), Description("If characters that matches the rule should be made visible or not.")]
            public bool _Visibility
            {
                get { return Visibility; }
                set { Visibility = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start character index")]
            public string _UserValue_Start
            {
                get { return UserValue_Start; }
                set { UserValue_Start = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End character index")]
            public string _UserValue_End
            {
                get { return UserValue_End; }
                set { UserValue_End = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Step")]
            public string _UserValue_Step
            {
                get { return UserValue_Step; }
                set { UserValue_Step = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Visibility")]
            public string _UserValue_Visibility
            {
                get { return UserValue_Visibility; }
                set { UserValue_Visibility = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    int sz = tp.GetTextLength();
                    int _end = End;
                    if (_end < 0)
                        _end = sz + End;
                    for (int tr = Start; tr <= _end; tr += Step)
                        tp.SetCharactesVisibility(tr, tr + 1, Visibility);
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }

            #endregion

            protected override string GetTransformationDescription()
            {
                return "Character visibility (Start:" + Start.ToString() + " End:" + End.ToString() + " Step:" + Step.ToString() + " Visible:" + Visibility.ToString() + ")";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".Start = " + Start.ToString() + ";";
                s += "\n\t" + this.CppName + ".End = " + End.ToString() + ";";
                s += "\n\t" + this.CppName + ".Step = " + Step.ToString() + ";";
                s += "\n\t" + this.CppName + ".Visibility = " + Visibility.ToString().ToLower() + ";";

                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Start, "Start") + GetOnStartFieldInit(UserValue_End, "End") + GetOnStartFieldInit(UserValue_Step, "Step") + GetOnStartFieldInit(UserValue_Visibility, "Visibility") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Start.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Start, "int"));
                if (UserValue_End.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_End, "int"));
                if (UserValue_Step.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Step, "int"));
                if (UserValue_Visibility.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Visibility, "bool"));
                base.PopulateParameters(p);
            }

        }

        [XmlType("TextCenterFlow"), XmlRoot("TextCenterFlow")]
        public class TextCenterFlowTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int FramesUpdate = 1;
            [XmlAttribute()]
            public string UserValue_FramesUpdate = "";

            #region Atribute
            [XmlIgnore(), Category("Flow"), DisplayName("Update time"), Description("Number of frames that need to pass to display another character from the text (minimal value is 1)")]
            public int _FramesUpdate
            {
                get { return FramesUpdate; }
                set { if (value >= 1) FramesUpdate = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Update time")]
            public string _UserValue_FramesUpdate
            {
                get { return UserValue_FramesUpdate; }
                set { UserValue_FramesUpdate = value; }
            }
            #endregion

            #region Dynamic Execution
            private int time, poz;
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    ((AnimO.TextElement)ElementObject).tp.SetCharactesVisibility(0, 0x7fffffff, false);
                    time = FramesUpdate;
                    poz = ((AnimO.TextElement)ElementObject).tp.GetTextLength() / 2;
                }
            }
            protected override bool OnUpdate()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    time--;
                    if (time == 0)
                    {
                        time = FramesUpdate;
                        TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                        poz--;
                        tp.SetCharactesVisibility(poz, (tp.GetTextLength() - poz), true);
                        if (poz < 0)
                        {
                            tp.SetCharactesVisibility(0, 0x7fffffff, true);
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Every " + FramesUpdate.ToString() + " frame(s)";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".FramesUpdate = " + FramesUpdate.ToString() + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_FramesUpdate, "FramesUpdate") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_FramesUpdate.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_FramesUpdate, "int"));
                base.PopulateParameters(p);
            }

        }

        [XmlType("FontSize"), XmlRoot("FontSize")]
        public class FontSizeTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float Size = 1.0f;
            [XmlAttribute()]
            public string UserValue_Size = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Font size")]
            public string _Size
            {
                get { return Project.ProcentToString(Size); }
                set { Size = Project.StringToProcent(value, 0, 100, Size, true); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Font size")]
            public string _UserValue_Size
            {
                get { return UserValue_Size; }
                set { UserValue_Size = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    ((AnimO.TextElement)ElementObject).ExecutionContext.ScaleWidth = Size;
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Font size : " + _Size;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Size = " + this.Size.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Size, "Size") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Size.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Size, "float"));
                base.PopulateParameters(p);
            }
        }

        public enum NumberFormatMethod : int
        {
            Integer = 0,
            IntegerWithDigitGrouping = 1,
            FloatWith2DecimalPlaces = 2,
            FloatWith3DecimalPlaces = 3,
            Percentage = 4,
            Percentage2DecimalPlaces = 5,
        };

        public class NumericFormatValidator
        {
            public int intValue = 0;
            public float floatValue = 0;
            public string cppType = "";
            public string cppFormatRepresentation = "";
            public string csFormatedValue = "";
            public string strRepresentation = "";
            public string error = "";
            public bool FloatNumber;

            private bool UpdateCPPFormat(NumberFormatMethod m)
            {
                switch (m)
                {
                    case NumberFormatMethod.Integer: cppType = "int"; return true;
                    case NumberFormatMethod.IntegerWithDigitGrouping: cppType = "int"; return true;
                    case NumberFormatMethod.FloatWith2DecimalPlaces: cppType = "float"; return true;
                    case NumberFormatMethod.FloatWith3DecimalPlaces: cppType = "float"; return true;
                    case NumberFormatMethod.Percentage: cppType = "float"; return true;
                    case NumberFormatMethod.Percentage2DecimalPlaces: cppType = "float"; return true;
                }
                error = "Unable to convert values of type '" + m.ToString() + "' to a C++ format !";
                return false;
            }

            private bool UpdateCSFormatedValue(NumberFormatMethod m)
            {
                switch (m)
                {
                    case NumberFormatMethod.Integer: csFormatedValue = intValue.ToString(); return true;
                    case NumberFormatMethod.IntegerWithDigitGrouping: csFormatedValue = intValue.ToString("N0", CultureInfo.CreateSpecificCulture("en-US")); return true;
                    case NumberFormatMethod.FloatWith2DecimalPlaces: csFormatedValue = floatValue.ToString("F2"); return true;
                    case NumberFormatMethod.FloatWith3DecimalPlaces: csFormatedValue = floatValue.ToString("F3"); return true;
                    case NumberFormatMethod.Percentage: csFormatedValue = ((int)(floatValue * 100.0f)).ToString() + "%"; return true;
                    case NumberFormatMethod.Percentage2DecimalPlaces: csFormatedValue = floatValue.ToString("P2", CultureInfo.InvariantCulture).Replace(" ", ""); return true;
                }
                error = "Unable to convert values of type '" + m.ToString() + "' to a C-Sharp format !";
                return false;
            }

            public bool ValidateData(string value, NumberFormatMethod m)
            {
                int iValue = 0;
                float fValue = 0;
                error = "";
                strRepresentation = "";
                cppFormatRepresentation = "";
                cppType = "";
                csFormatedValue = "";
                FloatNumber = true;
                if (UpdateCPPFormat(m) == false)
                    return false;

                switch (m)
                {
                    case NumberFormatMethod.Integer:
                    case NumberFormatMethod.IntegerWithDigitGrouping:
                        if (int.TryParse(value, out iValue) == false)
                        {
                            error = "Value '" + value + "' is not valid for method: '" + m.ToString() + "' !";
                            return false;
                        }
                        intValue = iValue;
                        strRepresentation = iValue.ToString();
                        cppFormatRepresentation = strRepresentation;
                        FloatNumber = false;
                        break;
                    case NumberFormatMethod.FloatWith2DecimalPlaces:
                    case NumberFormatMethod.FloatWith3DecimalPlaces:
                        if (float.TryParse(value, out fValue) == false)
                        {
                            error = "Value '" + value + "' is not valid for method: '" + m.ToString() + "' !";
                            return false;
                        }
                        floatValue = fValue;
                        strRepresentation = fValue.ToString();
                        cppFormatRepresentation = strRepresentation;
                        break;
                    case NumberFormatMethod.Percentage:
                    case NumberFormatMethod.Percentage2DecimalPlaces:
                        if (Project.StringToProcent(value, ref fValue) == false)
                        {
                            error = "Value '" + value + "' is not valid for method: '" + m.ToString() + "' !";
                            return false;
                        }
                        floatValue = fValue;
                        strRepresentation = Project.ProcentToString(floatValue);
                        cppFormatRepresentation = floatValue.ToString();
                        break;
                }
                if (strRepresentation.Length > 0)
                {
                    return UpdateCSFormatedValue(m);
                }
                error = "No translation available for value '" + value + "' as method: '" + m.ToString() + "' !";
                return false;
            }
        }

        [XmlType("NumericFormatter"), XmlRoot("NumericFormatter")]
        public class NumericFormatterTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Value = "12345";
            [XmlAttribute()]
            public NumberFormatMethod Method = NumberFormatMethod.Integer;

            [XmlAttribute()]
            public string UserValue_Value = "";

            #region Atribute
            [XmlIgnore(), Category("Format"), DisplayName("Value"), Description("Number to be formated")]
            public string _FramesUpdate
            {
                get { return Value; }
                set { if (nv.ValidateData(value, Method)) Value = nv.strRepresentation; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("Method"), Description("Format method")]
            public NumberFormatMethod _Method
            {
                get { return Method; }
                set { Method = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Value")]
            public string _UserValue_Value
            {
                get { return UserValue_Value; }
                set { UserValue_Value = value; }
            }
            #endregion

            #region Dynamic Execution
            private NumericFormatValidator nv = new NumericFormatValidator();

            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    if (nv.ValidateData(Value, Method))
                        tp.SetText(nv.csFormatedValue, false, null);
                    else
                        tp.SetText("???", false, null);
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            #endregion



            protected override string GetTransformationDescription()
            {
                return "Number format " + Value + " as " + Method.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + "." + nv.cppType + "Value = " + nv.cppFormatRepresentation + ";";
                s += "\n\t" + this.CppName + ".FormatType = " + ((int)Method).ToString() + " ;";
                return s + "\n";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (nv.ValidateData(Value, Method) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an invalid NumericFormat transformation: => " + nv.error);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Value, nv.cppType + "Value") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (nv.ValidateData(Value, Method) == false)
                    nv.cppType = "???";
                if (UserValue_Value.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Value, nv.cppType));
                base.PopulateParameters(p);
            }

        }

        [XmlType("NumberIncrease"), XmlRoot("NumberIncrease")]
        public class NumberIncreaseTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Start = "0";
            [XmlAttribute()]
            public string End = "0";
            [XmlAttribute()]
            public int Steps = 20;
            [XmlAttribute()]
            public NumberFormatMethod Method = NumberFormatMethod.Integer;

            [XmlAttribute()]
            public string UserValue_Start = "", UserValue_End = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Category("Format"), DisplayName("Start value"), Description("Number to be formated")]
            public string _Start
            {
                get { return Start; }
                set { if (nvStart.ValidateData(value, Method)) Start = nvStart.strRepresentation; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("End value"), Description("Number to be formated")]
            public string _End
            {
                get { return End; }
                set { if (nvStart.ValidateData(value, Method)) End = nvStart.strRepresentation; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("Steps"), Description("Number steps to go from 'Start' to 'End'")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value > 0) Steps = value; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("Method"), Description("Format method")]
            public NumberFormatMethod _Method
            {
                get { return Method; }
                set { Method = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start value")]
            public string _UserValue_Start
            {
                get { return UserValue_Start; }
                set { UserValue_Start = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End value")]
            public string _UserValue_End
            {
                get { return UserValue_End; }
                set { UserValue_End = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            private NumericFormatValidator nvStart = new NumericFormatValidator();
            private NumericFormatValidator nvEnd = new NumericFormatValidator();
            private NumericFormatValidator nvCurrent = new NumericFormatValidator();
            private int step;

            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    if (nvStart.ValidateData(Start, Method))
                        tp.SetText(nvStart.csFormatedValue, false, null);
                    else
                        tp.SetText("???", false, null);
                    nvEnd.ValidateData(End, Method);
                    step = 0;
                }
            }
            protected override bool OnUpdate()
            {
                step++;
                bool result_ok = false;
                if (nvStart.FloatNumber)
                {
                    float dif = nvEnd.floatValue - nvStart.floatValue;
                    float cValue = nvStart.floatValue + (dif * step) / Steps;
                    if (cValue > nvEnd.floatValue)
                        cValue = nvEnd.floatValue;
                    if ((Method == NumberFormatMethod.Percentage) || (Method == NumberFormatMethod.Percentage2DecimalPlaces))
                        result_ok = nvCurrent.ValidateData(Project.ProcentToString(cValue), Method);
                    else
                        result_ok = nvCurrent.ValidateData(cValue.ToString(), Method);
                }
                else
                {
                    int dif = nvEnd.intValue - nvStart.intValue;
                    int cValue = nvStart.intValue + (dif * step) / Steps;
                    if (cValue > nvEnd.intValue)
                        cValue = nvEnd.intValue;
                    result_ok = nvCurrent.ValidateData(cValue.ToString(), Method);
                }
                TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                if (result_ok)
                    tp.SetText(nvCurrent.csFormatedValue, false, null);

                return step < Steps;
            }
            #endregion



            protected override string GetTransformationDescription()
            {
                return "Number increase from " + _Start + " to " + _End + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + "." + nvStart.cppType + "Start = " + nvStart.cppFormatRepresentation + ";";
                s += "\n\t" + this.CppName + "." + nvEnd.cppType + "End = " + nvEnd.cppFormatRepresentation + ";";
                s += "\n\t" + this.CppName + ".Steps = " + Steps.ToString() + " ;";
                s += "\n\t" + this.CppName + ".FormatType = " + ((int)Method).ToString() + " ;";
                return s + "\n";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (nvStart.ValidateData(Start, Method) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an invalid NumericFormat transformation: => " + nvStart.error);
                if (nvEnd.ValidateData(End, Method) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an invalid NumericFormat transformation: => " + nvEnd.error);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Start, nvStart.cppType + "Start") + GetOnStartFieldInit(UserValue_End, nvEnd.cppType + "End") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (nvStart.ValidateData(Start, Method) == false)
                    nvStart.cppType = "???";
                if (UserValue_Start.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Start, nvStart.cppType));
                if (nvEnd.ValidateData(End, Method) == false)
                    nvEnd.cppType = "???";
                if (UserValue_End.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_End, nvEnd.cppType));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }

        }

        public enum CountDownFormat : int
        {
            Auto = 0,
            Seconds = 1,
            MinutesAndSeconds = 2,
            HoursMinutesSeconds = 3,
            DaysHourMinutesSeconds = 4,
        };

        [XmlType("CountDown"), XmlRoot("CountDown")]
        public class CountDown : GenericElementTransformation
        {
            [XmlAttribute()]
            public int SecondsLeft = 30;
            [XmlAttribute()]
            public CountDownFormat Method = CountDownFormat.Auto;

            [XmlAttribute()]
            public string UserValue_SecondsLeft = "";

            #region Atribute
            [XmlIgnore(), Category("Format"), DisplayName("Seconds left"), Description("Number of seconds left from the countdown")]
            public int _SecondsLeft
            {
                get { return SecondsLeft; }
                set { if (value >= 0) SecondsLeft = value; }
            }

            [XmlIgnore(), Category("Format"), DisplayName("Method"), Description("Format method")]
            public CountDownFormat _Method
            {
                get { return Method; }
                set { Method = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Seconds left")]
            public string _UserValue_SecondsLeft
            {
                get { return UserValue_SecondsLeft; }
                set { UserValue_SecondsLeft = value; }
            }
            #endregion

            #region Dynamic Execution
            private int counter, cValue;

            private void UpdateSeconds()
            {
                int d, h, r;
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    string res = "";
                    CountDownFormat cd = Method;
                    if (cd == CountDownFormat.Auto)
                    {
                        if (cValue < 60)
                            cd = CountDownFormat.Seconds;
                        else if (cValue < (60 * 60))
                            cd = CountDownFormat.MinutesAndSeconds;
                        else if (cValue < (60 * 60 * 24))
                            cd = CountDownFormat.HoursMinutesSeconds;
                        else
                            cd = CountDownFormat.DaysHourMinutesSeconds;
                    }
                    switch (cd)
                    {
                        case CountDownFormat.Seconds:
                            res = String.Format(":{0:00}", cValue);
                            break;
                        case CountDownFormat.MinutesAndSeconds:
                            res = String.Format("{0:00}:{1:00}", cValue / 60, cValue % 60);
                            break;
                        case CountDownFormat.HoursMinutesSeconds:
                            h = cValue / 3600; r = cValue % 3600;
                            res = String.Format("{0:00}:{1:00}:{2:00}", h, r / 60, r % 60);
                            break;
                        case CountDownFormat.DaysHourMinutesSeconds:
                            d = cValue / (3600 * 24); r = cValue % (3600 * 24);
                            h = r / 3600; r = r % 3600;
                            res = String.Format("{0}d {1:00}:{2:00}:{3:00}", d, h, r / 60, r % 60);
                            break;
                    }
                    tp.SetText(res, false, null);
                }
            }

            protected override void OnInit()
            {
                cValue = SecondsLeft;
                counter = 0;
                UpdateSeconds();
            }
            protected override bool OnUpdate()
            {
                if (cValue <= 0)
                    return false;
                counter++;
                if (counter >= 60)
                {
                    counter = 0;
                    cValue--;
                    UpdateSeconds();
                }
                return true;
            }
            #endregion



            protected override string GetTransformationDescription()
            {
                return "Count down from " + SecondsLeft.ToString() + " seconds";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".SecondsLeft = " + SecondsLeft.ToString() + ";";
                s += "\n\t" + this.CppName + ".FormatType = " + ((int)Method).ToString() + " ;";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_SecondsLeft, "SecondsLeft") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_SecondsLeft.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SecondsLeft, "int"));
                base.PopulateParameters(p);
            }

        }


        public class TransformationDescriptor
        {
            public Type type;
            public String Name, Icon, Description;
            public HashSet<Type> CompatibilityElements;
            public TransformationDescriptor(Type _type, string name, string icon, string compatibility, string description)
            {
                type = _type;
                Name = name;
                Icon = icon;
                Description = description;
                CompatibilityElements = new HashSet<Type>();
                foreach (char ch in compatibility)
                {
                    switch (ch)
                    {
                        case 'I': CompatibilityElements.Add(typeof(AnimO.ImageElement)); break;
                        case 'T': CompatibilityElements.Add(typeof(AnimO.TextElement)); break;
                        case 'S': CompatibilityElements.Add(typeof(AnimO.EntireSurfaceElement)); break;
                        case 'R': CompatibilityElements.Add(typeof(AnimO.RectangleElement)); break;
                        case 'E': CompatibilityElements.Add(typeof(AnimO.ExclusionRectangleElement)); break;
                        case 'C': CompatibilityElements.Add(typeof(AnimO.ClipRectangleElement)); break;
                        case 'B': CompatibilityElements.Add(typeof(AnimO.SimpleButtonElement)); break;
                    }
                }
            }

            #region List View Props
            [XmlIgnore(), Browsable(false)]
            public string propName { get { return Name; } }
            [XmlIgnore(), Browsable(false)]
            public string propDescription { get { return Description; } }
            [XmlIgnore(), Browsable(false)]
            public string propIcon { get { return Icon; } }
            #endregion
        };
        public class Factory
        {
            private static List<TransformationDescriptor> transformation_list = new List<TransformationDescriptor>()
            {
                // alpha
                new TransformationDescriptor(typeof(AlphaBlendingLinearTransformation),"Alpha blending linear transform","alpha_blending_linear","ITSRE","Increses/Decreses alpha value within a specific interval"),
                new TransformationDescriptor(typeof(AlphaBlendingForwardAndBackTransformation),"Back and Forward aplha blending","alpha_blending_forward_and_back","ITSRE","Increses/Decreses alpha value within a specific interval from start to and and then back to start"),
                // scale
                new TransformationDescriptor(typeof(ScaleLinearTransformation),"Scale linear transform","scale_linear","ITRECB","Increses/Decreses image or text scale within a specific interval"),
                new TransformationDescriptor(typeof(ScaleForwardAndBackTransformation),"Back and Forward scale transformation","scale_forrward_and_back","ITRECB","Scale an image/text to a specific value and then go back to the original one"),
                new TransformationDescriptor(typeof(ScaleWidthLinearTransformation),"Width Scale linear transform","scale_width_linear","IRECB","Increses/Decreses image or text width scale within a specific interval"),
                new TransformationDescriptor(typeof(ScaleWidthForwardAndBackTransformation),"Back and Forward Width scale transformation","scale_width_forward_and_back","IRECB","Scale the width for an image/text to a specific value and then go back to the original one"),
                new TransformationDescriptor(typeof(ScaleHeightLinearTransformation),"Height Scale linear transform","scale_height_linear","IRECB","Increses/Decreses image or text height scale within a specific interval"),
                new TransformationDescriptor(typeof(ScaleHeightForwardAndBackTransformation),"Back and Forward Height scale transformation","scale_height_forward_and_back","IRECB","Scale the height for an image/text to a specific value and then go back to the original one"),
                // color
                new TransformationDescriptor(typeof(ColorBlendLinearTransformation),"Color blending linear transformation","color_linear","ITSRE","Change the blending color from one color to another !"),
                new TransformationDescriptor(typeof(ColorBlendForwardAndBackTransformation),"Color blending back and forward transformation","color_forward_and_back","ITSRE","Change the blending color from one color to another and back !"),
                new TransformationDescriptor(typeof(ColorBlendStateTransformation),"Blend color","color","ITSRE","Sets the blending color for an element"),
                new TransformationDescriptor(typeof(AlphaBlendingStateTransformation),"Alpha blending","alpha_blending","ITSRE","Sets the alpha channel for an element"),
                // move
                new TransformationDescriptor(typeof(MoveRelativeLinearTransformation),"Move element relative to its position","move_relative_linear","ITRECB","Move element relative to its position with an X offset and an Y offset "),
                new TransformationDescriptor(typeof(MoveAbsoluteLinearTransformation),"Move element to a position","move_absolute_linear","ITRECB","Move element to an absolute position."),
                new TransformationDescriptor(typeof(QuadraticBezierTransformation),"Quadratic Bezier move","move_bezier","ITRECB","Move element quadratic bezier curves"),
                new TransformationDescriptor(typeof(SetNewAbsolutePositionTransformation),"Set position","absolute_position","ITRECB","Move element to an absolute position"),
                new TransformationDescriptor(typeof(SetNewRelativePositionTransformation),"Move to position","relative_position","ITRECB","Move element relative to its current position"),
                // state
                new TransformationDescriptor(typeof(VisibleStateTransformation),"Visibility state","visible","ITSRECB","Changes the visibility state of an element"),
                // specific texte
                new TransformationDescriptor(typeof(SetNewTextTransformation),"Set text","set_text","T","Set a new text for a text element"),
                new TransformationDescriptor(typeof(TextFlowTransformation),"Text flow","text_flow","T","Display a text character by character"),
                new TransformationDescriptor(typeof(TextCenterFlowTransformation),"Text Center flow","text_center_flow","T","Display a text character by character starting from the center and moving to margins"),
                new TransformationDescriptor(typeof(FontSizeTransformation),"Font size","font_size","T","Sets the size of the font used by a text element."),
                new TransformationDescriptor(typeof(TextCharacterVisibilityTransformation),"Character visibility","char_visibility","T","Sets the visibility state of characters from a text !"),
                new TransformationDescriptor(typeof(NumericFormatterTransformation),"Numeric formatter","numeric_formatter","T","Formats a number"),
                new TransformationDescriptor(typeof(NumberIncreaseTransformation),"Number increase","numeric_formatter_linear","T","Increases/Decreases a number from a start value to an end value."),
                new TransformationDescriptor(typeof(CountDown),"Count down","countdown","T","Creates a count down for a specific number of seconds/ticks !"),

                // specific imagini
                new TransformationDescriptor(typeof(SetNewImageTransformation),"Set image","set_image","I","Set a new image to an image element"),
                new TransformationDescriptor(typeof(SetImageIndexTransformation),"Set image index","set_image_index","I","Set the index of the image from the multi image element"),
                new TransformationDescriptor(typeof(ImageIndexLinearTransformation),"Image index linear transformation","image_index_linear","I","Changes the index of the image from the multi image element within a specific interval"),                

                // specific butoane
                new TransformationDescriptor(typeof(ButtonEnableTransformation),"Set button enable state","set_button_enable_state","B","Set the state of a button - enabled or not"),
            };
            private static Dictionary<Type, TransformationDescriptor> transformations_dict = null;
            private static void CreateTransformationDictionary()
            {
                if (transformations_dict != null)
                    return;
                transformations_dict = new Dictionary<Type, TransformationDescriptor>();
                foreach (TransformationDescriptor td in transformation_list)
                    transformations_dict[td.type] = td;
            }

            public static string GetName(Type transformationType)
            {
                CreateTransformationDictionary();
                if (transformations_dict.ContainsKey(transformationType))
                    return transformations_dict[transformationType].Name;
                return "??? (" + transformationType.ToString() + ")";
            }
            public static string GetIconKey(Type transformationType)
            {
                CreateTransformationDictionary();
                if (transformations_dict.ContainsKey(transformationType))
                    return transformations_dict[transformationType].Icon;
                return "";
            }
            public static bool IsTransformationCompatible(Type transformationType, AnimO.GenericElement elem)
            {
                return IsTransformationCompatible(transformationType, elem.GetType());
            }
            public static bool IsTransformationCompatible(Type transformationType, Type elementType)
            {
                CreateTransformationDictionary();
                if (transformations_dict.ContainsKey(transformationType))
                    return transformations_dict[transformationType].CompatibilityElements.Contains(elementType);
                return false;
            }
            public static List<TransformationDescriptor> GetTransformationList()
            {
                return transformation_list;
            }
            public static GenericTransformation CreateTransformation(Type transformationType, GenericElement element)
            {
                GenericElementTransformation trans = (GenericElementTransformation)Activator.CreateInstance(transformationType);
                if (trans != null)
                {
                    trans.Element = element.Name;
                }
                return trans;
            }
        }

        #endregion

        #region Elements
        public enum YesNo
        {
            No,
            Yes,
        };
        public enum SimpleButtonSetter
        {
            No,
            OneForEachState,
            OneForAll,
        };
        public enum PositionGetterType
        {
            No,
            Percentages,
            Pixels
        };

        public class ElementRectangle
        {
            public float X_Percentage;
            public float Y_Percentage;
            public float WidthInPixels;
            public float HeightInPixels;
            public Alignament Align;
            public bool FullScreen;
        };

        [XmlInclude(typeof(EntireSurfaceElement))]
        [XmlInclude(typeof(TextElement))]
        [XmlInclude(typeof(ImageElement))]
        [XmlInclude(typeof(RectangleElement))]
        [XmlInclude(typeof(ExclusionRectangleElement))]
        [XmlInclude(typeof(ClipRectangleElement))]
        [XmlInclude(typeof(DisableClippingElement))]
        [XmlInclude(typeof(SimpleButtonElement))]
        [XmlInclude(typeof(GenericElementWithPosition))]
        [XmlInclude(typeof(GenericElementWithPositionAndSize))]
        [XmlType("Element"), XmlRoot("Element")]
        public class GenericElement
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public bool Visible = true;
            [XmlAttribute()]
            public string Parent = "<None>";
            [XmlAttribute()]
            public string UserValue_Visible = "";
            [XmlIgnore()]
            public RuntimeContext ExecutionContext = new RuntimeContext();

            [XmlIgnore()]
            public static AppResources CurrentAppResources = null;
            [XmlIgnore()]
            public static IRefreshDesign RefreshDesignCallback = null;

            [XmlIgnore()]
            public bool _ShowInBoardAnimation_ = true;
            [XmlIgnore()]
            public bool _FoundInZOrder_ = false;
            [XmlIgnore()]
            public GenericElement ParentElement = null;


            #region Atribute
            [XmlIgnore(), Description("Name"), Category("General"), DisplayName("Name")]
            public string _Name
            {
                get { return Name; }
                set { Name = value; }
            }
            [XmlIgnore(), Description("Initial Visibility"), Category("General"), DisplayName("Visible")]
            public bool _Visible
            {
                get { return Visible; }
                set { Visible = value; }
            }
            [XmlIgnore(), Description("Parent element (to compute the relative position)"), Category("General"), DisplayName("Parent"), Editor(typeof(AnimationElementRelativePositionEditor), typeof(UITypeEditor))]
            public string _RelativeToElement
            {
                get { return Parent; }
                set
                {
                    if ((value.Equals("")) || (value.Equals("<None>", StringComparison.InvariantCultureIgnoreCase)))
                        Parent = "<None>";
                    else
                        Parent = value;
                }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Visible")]
            public string _UserValue_Visible
            {
                get { return UserValue_Visible; }
                set { UserValue_Visible = value; }
            }

            #endregion

            #region List View Props
            [XmlIgnore(), Browsable(false)]
            public string propName { get { return Name; } }
            [XmlIgnore(), Browsable(false)]
            public string propDescription { get { return GetDescription(); } }
            [XmlIgnore(), Browsable(false)]
            public string propIcon { get { return GetIconKey(); } }
            [XmlIgnore(), Browsable(false)]
            public bool propVisible
            {
                set { _ShowInBoardAnimation_ = value; if (RefreshDesignCallback != null) RefreshDesignCallback.Refresh(); }
                get { return _ShowInBoardAnimation_; }
            }
            #endregion

            #region Internal functions
            protected string GetPercentageValue(float value)
            {
                return Project.ProcentToString(value);
            }
            protected float SetPercentageValue(string strRepresentation, float currentValue)
            {
                return Project.StringToProcent(strRepresentation, 0, 1000, currentValue);
            }
            protected string GetLocationValue(float value)
            {
                return Project.ProcentToString(value);
            }
            protected float SetLocationValue(string strRepresentation, float currentValue)
            {
                return Project.StringToProcent(strRepresentation, -1000, 1000, currentValue);
            }
            public static string GetSizeInPixels(float value)
            {
                return value.ToString() + " px";
            }
            public static float SetSizeInPixels(string strRepresentation, float currentValue)
            {
                float result = 0.0f;
                if (float.TryParse(strRepresentation.ToLower().Replace("px", "").Trim(), out result))
                {
                    if (result >= 0)
                        return result;
                }
                return currentValue;
            }
            protected string GetParamOrDefaultValue(string defaultValue, string paramName)
            {
                if (paramName.Length > 0)
                    return "param_" + paramName;
                return defaultValue;
            }
            protected void CreateGetterForPosition(GACParser.Module m)
            {
                string nm = "Get" + Name + "X";
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "float", "", null);
                nm = "Get" + Name + "Y";
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "float", "", null);
            }
            protected string CreateGetPositionCPPHeaderDefinition()
            {
                return "\n\tfloat Get" + Name + "X ();\n\tfloat Get" + Name + "Y ();";
            }
            protected string CreateGetPositionCPPImplementation(string className, PositionGetterType pType)
            {
                return "\nfloat " + className + "::Get" + Name + "X () { return " + Name + ".GetX(this," + (pType == PositionGetterType.Pixels).ToString().ToLower() + "); }\nfloat " + className + "::Get" + Name + "Y (){ return " + Name + ".GetY(this," + (pType == PositionGetterType.Pixels).ToString().ToLower() + "); }";
            }
            protected void CreateSetterForColor(GACParser.Module m, string setterName)
            {
                string nm = "Set" + Name + setterName;
                GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "color", "color", "Color", "", null));
                m.Members[nm] = mb;
            }
            protected string CreateSetColorCPPImplementation(string className, string setterName)
            {
                return "\nvoid " + className + "::Set" + Name + setterName + " (unsigned int __color__) { " + Name + ".ColorTransform = __color__; if (" + Name + ".CallVirtualFunction) " + Name + ".OnUpdateBlendColor(this);  };";
            }
            protected string CreateSetColorCPPHeaderDefinition(string setterName)
            {
                return "\n\tvoid Set" + Name + setterName + " (unsigned int __color__);";
            }

            #endregion


            #region Virtual Functions
            protected virtual float GetWidthInPixels() { return 0; }
            protected virtual float GetHeightInPixels() { return 0; }
            protected virtual void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {

            }
            public virtual bool IsFullScreen() { return false; }
            public static void ComputeScreenRect(float percentage_X, float percentage_y, float widthInPixels, float heightInPixels, float scaleWidth, float scaleHeight, Alignament align, float parentLeftInPixels, float parentTopInPixels, float parentWidth, float parentHeight, ref RectangleF result)
            {
                float x = percentage_X * parentWidth + parentLeftInPixels;
                float y = percentage_y * parentHeight + parentTopInPixels;
                widthInPixels *= scaleWidth;
                heightInPixels *= scaleHeight;
                switch (align)
                {
                    case Alignament.Center: x -= widthInPixels / 2.0f; y -= heightInPixels / 2.0f; break;
                    case Alignament.TopLeft: break;
                    case Alignament.TopCenter: x -= widthInPixels / 2.0f; break;
                    case Alignament.TopRight: x -= widthInPixels; break;
                    case Alignament.RightCenter: x -= widthInPixels; y -= heightInPixels / 2.0f; break;
                    case Alignament.BottomRight: x -= widthInPixels; y -= heightInPixels; break;
                    case Alignament.BottomCenter: x -= widthInPixels / 2.0f; y -= heightInPixels; break;
                    case Alignament.BottomLeft: y -= heightInPixels; break;
                    case Alignament.LeftCenter: y -= heightInPixels / 2.0f; break;
                }
                result.X = x;
                result.Y = y;
                result.Width = widthInPixels;
                result.Height = heightInPixels;
            }
            public static void ComputeScreenRect(float percentage_X, float percentage_y, float widthInPixels, float heightInPixels, float scaleWidth, float scaleHeight, Alignament align, RectangleF parent, ref RectangleF result)
            {
                ComputeScreenRect(percentage_X, percentage_y, widthInPixels, heightInPixels, scaleWidth, scaleHeight, align, parent.Left, parent.Top, parent.Width, parent.Height, ref result);
            }
            public static void ComputeScreenRect(RuntimeContext rContext, bool useImage, RectangleF parent)
            {
                float w, h;
                if (useImage)
                {
                    if (rContext.Image != null)
                    {
                        w = rContext.Image.Width;
                        h = rContext.Image.Height;
                    }
                    else
                    {
                        w = 0;
                        h = 0;
                    }
                }
                else
                {
                    w = rContext.WidthInPixels;
                    h = rContext.HeightInPixels;
                }
                ComputeScreenRect(rContext.X_Percentage, rContext.Y_Percentage, w, h, rContext.ScaleWidth, rContext.ScaleHeight, rContext.Align, parent.Left, parent.Top, parent.Width, parent.Height, ref rContext.ScreenRect);
            }

            public void ComputeScreenRect(float screenWidthInPixels, float screenHeightInPixels)
            {
                float w, h, s_w, s_h;
                if (IsFullScreen())
                {
                    w = screenWidthInPixels;
                    h = screenHeightInPixels;
                    s_w = s_h = 1.0f;
                }
                else
                {
                    w = GetWidthInPixels(); s_w = this.ExecutionContext.ScaleWidth;
                    h = GetHeightInPixels(); s_h = this.ExecutionContext.ScaleHeight;
                }
                if (this.ParentElement != null)
                    ComputeScreenRect(ExecutionContext.X_Percentage, ExecutionContext.Y_Percentage, w, h, s_w, s_h, ExecutionContext.Align, this.ParentElement.ExecutionContext.ScreenRect, ref ExecutionContext.ScreenRect);
                else
                    ComputeScreenRect(ExecutionContext.X_Percentage, ExecutionContext.Y_Percentage, w, h, s_w, s_h, ExecutionContext.Align, 0, 0, screenWidthInPixels, screenHeightInPixels, ref ExecutionContext.ScreenRect);
            }
            public void UpdateFromScreenRect(float leftInPixels, float topInPixels, float widthInPixels, float HeightInPixels, float screenWidthInPixels, float screenHeightInPixels)
            {
                float x_p = 0, y_p = 0; // x_p = leftInPixels + widthInPixels/ 2.0f;

                //
                //

                switch (this.ExecutionContext.Align)
                {
                    case Alignament.Center:
                    case Alignament.RightCenter:
                    case Alignament.LeftCenter: y_p = topInPixels + HeightInPixels / 2.0f; break;

                    case Alignament.TopLeft:
                    case Alignament.TopCenter:
                    case Alignament.TopRight: y_p = topInPixels; break;

                    case Alignament.BottomRight:
                    case Alignament.BottomCenter:
                    case Alignament.BottomLeft: y_p = topInPixels + HeightInPixels; break;
                }
                switch (this.ExecutionContext.Align)
                {
                    case Alignament.TopLeft:
                    case Alignament.BottomLeft:
                    case Alignament.LeftCenter: x_p = leftInPixels; break;

                    case Alignament.BottomRight:
                    case Alignament.RightCenter:
                    case Alignament.TopRight: x_p = leftInPixels + widthInPixels; break;

                    case Alignament.Center:
                    case Alignament.TopCenter:
                    case Alignament.BottomCenter: x_p = leftInPixels + widthInPixels / 2.0f; break;
                }
                if (this.ParentElement == null)
                {
                    x_p = x_p / screenWidthInPixels;
                    y_p = y_p / screenHeightInPixels;
                }
                else
                {
                    x_p = (x_p - this.ParentElement.ExecutionContext.ScreenRect.Left) / this.ParentElement.ExecutionContext.ScreenRect.Width;
                    y_p = (y_p - this.ParentElement.ExecutionContext.ScreenRect.Top) / this.ParentElement.ExecutionContext.ScreenRect.Height;
                }
                // am recalculat pozitiile
                // calculez si sclaraile
                float scale_w = 0;
                float scale_h = 0;
                float w_pixels = 0;
                float h_pixels = 0;
                // pot sa am 2 cazuri
                // a) modific scale-ul
                // b) modific size-ul

                // a) modific scale-ul
                scale_w = widthInPixels / this.GetWidthInPixels();
                scale_h = HeightInPixels / this.GetHeightInPixels();
                w_pixels = widthInPixels / this.ExecutionContext.ScaleWidth;
                h_pixels = HeightInPixels / this.ExecutionContext.ScaleHeight;
                SetPositionAndSize(x_p, y_p, w_pixels, h_pixels, scale_w, scale_h);
            }

            protected virtual string GetDescription()
            {
                return "<None>";
            }
            public virtual string GetTypeName()
            {
                return "";
            }
            public virtual string GetIconKey()
            {
                return "";
            }
            public virtual void InitRuntimeContext()
            {
                ExecutionContext.Visible = Visible;
            }
            public virtual void OnPaint(Canvas c, float deviceWidth, float deviceHeight, AnimO.BoardViewMode viewMode) { }
            public void Paint(Canvas c, float deviceWidth, float deviceHeight, AnimO.BoardViewMode viewMode)
            {
                switch (viewMode)
                {
                    case BoardViewMode.Design:
                        if (_ShowInBoardAnimation_ == false)
                            return;
                        OnPaint(c, deviceWidth, deviceHeight, viewMode);
                        break;
                    case BoardViewMode.Play:
                        if (ExecutionContext.Visible)
                            OnPaint(c, deviceWidth, deviceHeight, viewMode);
                        break;
                }
            }
            public virtual string Validate(Project prj, AppResources resources)
            {
                return "Missing implementation for this element type: " + GetType().ToString();
            }
            public virtual string GetCPPClassName() { return "?"; }
            public virtual string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = "";
                s += "\n\t" + Name + ".Visible = " + GetParamOrDefaultValue(this.Visible.ToString().ToLower(), UserValue_Visible) + ";";
                if (this.Parent.Equals("<None>"))
                    s += "\n\t" + Name + ".Parent = NULL;";
                else
                    s += "\n\t" + Name + ".Parent = &" + Parent + ";";
                return s;
            }
            public virtual void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Visible.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Visible, "bool"));
            }
            public virtual void AddAnimationFunction(GACParser.Module m) { }
            public virtual string GetAnimationFunctionCPPHeaderDefinition() { return ""; }
            public virtual string GetAnimationFunctionCPPImplementation(string className) { return ""; }
            #endregion

            public string ToXMLString()
            {
                try
                {
                    var stringwriter = new System.IO.StringWriter();
                    var serializer = new XmlSerializer(typeof(GenericElement));
                    serializer.Serialize(stringwriter, this);
                    return stringwriter.ToString();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to convert element to XML: \n" + e.ToString());
                    return "";
                }
            }

            public static GenericElement FromXMLString(string xmlText)
            {
                try
                {
                    var stringReader = new System.IO.StringReader(xmlText);
                    var serializer = new XmlSerializer(typeof(GenericElement));
                    GenericElement af = serializer.Deserialize(stringReader) as GenericElement;
                    if (af == null)
                    {
                        MessageBox.Show("Unable to create element from XML: \n" + xmlText);
                    }
                    return af;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to create element from XML: \n" + e.ToString());
                    return null;
                }
            }

            public GenericElement MakeCopy()
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GenericElement));
                    StringWriter textWriter = new StringWriter();
                    serializer.Serialize(textWriter, this);
                    StringReader textReader = new StringReader(textWriter.ToString());
                    return (GenericElement)serializer.Deserialize(textReader);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        public class GenericElementWithPosition : GenericElement
        {
            [XmlAttribute()]
            public Alignament Align = Alignament.Center;
            [XmlAttribute()]
            public float ScaleWidth = 1.0f;
            [XmlAttribute()]
            public float ScaleHeight = 1.0f;
            [XmlAttribute()]
            public float X = 0.5f;
            [XmlAttribute()]
            public float Y = 0.5f;
            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "", UserValue_ScaleWidth = "", UserValue_ScaleHeight = "", UserValue_Align = "";
            [XmlAttribute()]
            public PositionGetterType Getter_Position = PositionGetterType.No;


            #region Atribute
            [XmlIgnore(), Description("Rectangle Aliganment"), Category("Layout"), DisplayName("Alignament")]
            public Alignament _Align
            {
                get { return Align; }
                set { Align = value; }
            }
            [XmlIgnore(), Description("Scale width"), Category("Zoom"), DisplayName("Scale Width")]
            public string _ScaleWidth
            {
                get { return GetPercentageValue(ScaleWidth); }
                set { ScaleWidth = SetPercentageValue(value, ScaleWidth); }
            }
            [XmlIgnore(), Description("Scale height"), Category("Zoom"), DisplayName("Scale Height")]
            public string _ScaleHeight
            {
                get { return GetPercentageValue(ScaleHeight); }
                set { ScaleHeight = SetPercentageValue(value, ScaleHeight); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value, X); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value, Y); }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Alignament")]
            public string _UserValue_Align
            {
                get { return UserValue_Align; }
                set { UserValue_Align = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Scale Width")]
            public string _UserValue_ScaleWidth
            {
                get { return UserValue_ScaleWidth; }
                set { UserValue_ScaleWidth = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Scale Height")]
            public string _UserValue_ScaleHeight
            {
                get { return UserValue_ScaleHeight; }
                set { UserValue_ScaleHeight = value; }
            }
            #endregion

            #region Getters
            [XmlIgnore(), Description("Specifies if a getter for element position is to be created."), Category("Getters"), DisplayName("Position")]
            public PositionGetterType _Getter_Position
            {
                get { return Getter_Position; }
                set { Getter_Position = value; }
            }
            #endregion

            #region Virtual functions

            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                if (UserValue_ScaleWidth.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ScaleWidth, "float"));
                if (UserValue_ScaleHeight.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ScaleHeight, "float"));
                if (UserValue_Align.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Align, "unsigned int", "Alignament"));
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);

                s += "\n\t" + Name + ".X = " + GetParamOrDefaultValue(this.X.ToString(), UserValue_X) + ";";
                s += "\n\t" + Name + ".Y = " + GetParamOrDefaultValue(this.Y.ToString(), UserValue_Y) + ";";
                s += "\n\t" + Name + ".ScaleWidth = " + GetParamOrDefaultValue(this.ScaleWidth.ToString(), UserValue_ScaleWidth) + ";";
                s += "\n\t" + Name + ".ScaleHeight = " + GetParamOrDefaultValue(this.ScaleHeight.ToString(), UserValue_ScaleHeight) + ";";
                s += "\n\t" + Name + ".Alignament = " + GetParamOrDefaultValue("GAC_ALIGNAMENT_" + this.Align.ToString().ToUpper(), UserValue_Align) + ";";
                return s;
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.ScaleWidth = ScaleWidth;
                ExecutionContext.ScaleHeight = ScaleHeight;
                ExecutionContext.X_Percentage = X;
                ExecutionContext.Y_Percentage = Y;
                ExecutionContext.Align = Align;
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Getter_Position != PositionGetterType.No)
                    CreateGetterForPosition(m);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPHeaderDefinition();
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPImplementation(className, Getter_Position);
                return s;
            }
            public override string Validate(Project prj, AppResources resources)
            {

                return null;
            }
            #endregion
        }
        public class GenericElementWithPositionAndSize : GenericElementWithPosition
        {
            [XmlAttribute()]
            public float Width = 100;
            [XmlAttribute()]
            public float Height = 100;
            [XmlAttribute()]
            public string UserValue_Width = "", UserValue_Height = "";

            #region Atribute
            [XmlIgnore(), Description("Width of the object"), Category("Layout"), DisplayName("Width")]
            public string _Width
            {
                get { return GetSizeInPixels(Width); }
                set { Width = SetSizeInPixels(value, Width); }
            }
            [XmlIgnore(), Description("Height of the object"), Category("Layout"), DisplayName("Height")]
            public string _Height
            {
                get { return GetSizeInPixels(Height); }
                set { Height = SetSizeInPixels(value, Height); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Width")]
            public string _UserValue_Width
            {
                get { return UserValue_Width; }
                set { UserValue_Width = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Height")]
            public string _UserValue_Height
            {
                get { return UserValue_Height; }
                set { UserValue_Height = value; }
            }
            #endregion

            #region Virtual functions
            protected override float GetWidthInPixels() { return this.Width; }
            protected override float GetHeightInPixels() { return this.Height; }
            protected override string GetDescription()
            {
                return Align.ToString() + "(" + GetLocationValue(X) + "," + GetLocationValue(Y) + ") Size = [" + GetLocationValue(Width) + " x " + GetLocationValue(Height) + " pixels]";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_Width.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Width, "float"));
                if (UserValue_Height.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Height, "float"));
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".WidthInPixels = " + GetParamOrDefaultValue(this.Width.ToString() + "* (Core.ResolutionAspectRatio)", UserValue_Width) + ";";
                s += "\n\t" + Name + ".HeightInPixels = " + GetParamOrDefaultValue(this.Height.ToString() + "* (Core.ResolutionAspectRatio)", UserValue_Height) + ";";
                return s;
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.WidthInPixels = Width;
                ExecutionContext.HeightInPixels = Height;
            }
            #endregion
        }
        public class ImageElement : GenericElementWithPosition
        {
            [XmlAttribute()]
            public string UserValue_ColorBlending = "", UserValue_Images = "";
            [XmlAttribute()]
            public int ColorBlending = -1;
            [XmlAttribute()]
            public string Images = "";
            [XmlAttribute()]
            public int ImageIndex = 0;
            [XmlAttribute()]
            public YesNo Getter_Image = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Image = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Images"), Description("List of images used"), Editor(typeof(ImageListSelectorEditor), typeof(UITypeEditor))]
            public string _Images
            {
                get { return Images; }
                set { Images = value; }
            }
            [XmlIgnore(), Category("Image"), DisplayName("Image Index"), Description("Image index from the list of indexes")]
            public int _ImageIndex
            {
                get { return ImageIndex; }
                set
                {
                    if (value < 0)
                        ImageIndex = 0;
                    else
                    {
                        int count = Project.StringListToList(Images, ';').Count;
                        if (value >= count)
                            ImageIndex = count - 1;
                        else
                            ImageIndex = value;
                        if (ImageIndex < 0)
                            ImageIndex = 0;
                    }
                }
            }
            [XmlIgnore(), Category("Image"), DisplayName("Blend Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }
            #endregion

            #region Initialization parameters (user dependent)
            public string _UserValue_ColorBlending
            {
                get { return UserValue_ColorBlending; }
                set { UserValue_ColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Image/Images")]
            public string _UserValue_Images
            {
                get { return UserValue_Images; }
                set { UserValue_Images = value; }
            }
            #endregion

            #region Getters and Setters
            [XmlIgnore(), Description("Specifies if a getter for element image is to be created."), Category("Getters"), DisplayName("Image")]
            public YesNo _Getter_Image
            {
                get { return Getter_Image; }
                set { Getter_Image = value; }
            }
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Image")]
            public YesNo _Setter_Image
            {
                get { return Setter_Image; }
                set { Setter_Image = value; }
            }
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Blend Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            private List<Bitmap> internalImages = new List<Bitmap>();

            protected override float GetWidthInPixels()
            {
                if (ImageIndex < internalImages.Count)
                    return internalImages[ImageIndex].Width;
                return 0;
            }
            protected override float GetHeightInPixels()
            {
                if (ImageIndex < internalImages.Count)
                    return internalImages[ImageIndex].Height;
                return 0;
            }
            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                this.ScaleWidth = scaledWidth;
                this.ScaleHeight = scaleHeight;
            }
            protected override string GetDescription()
            {
                int count = Project.StringListToList(Images, ';').Count;
                if (count <= 1)
                {
                    if (Images.Trim().Length == 0)
                        return ("<no image/images selected>");
                    return Images;
                }
                else
                {
                    return count.ToString() + " images (" + Images + ")";
                }
            }
            public override string GetIconKey()
            {
                List<string> imgLst = Project.StringListToList(Images, ';');
                if ((imgLst != null) && (imgLst.Count > ImageIndex) && (ImageIndex >= 0))
                    return GenericResource.GetResourceVariableKey(typeof(ImageResource), imgLst[ImageIndex]);

                return "";
            }
            public override string GetTypeName()
            {
                return "Image";
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
                ExecutionContext.Image = internalImages[ImageIndex];
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.DrawImage(this.ExecutionContext);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                List<String> lst = Project.StringListToList(Images, ';');
                if ((lst == null) || (lst.Count == 0))
                    return "Expecting at least one image !";
                internalImages.Clear();
                foreach (string s in lst)
                {
                    if (resources.Images.ContainsKey(s) == false)
                        return "Image resource with name '" + s + "' does not exists !";
                    internalImages.Add(resources.Images[s].Picture);
                }
                if (ImageIndex < 0)
                    return "Expecting a valid image index - between 0 and " + (internalImages.Count - 1).ToString();
                if (ImageIndex >= internalImages.Count)
                    return "Expecting a valid image index - between 0 and " + (internalImages.Count - 1).ToString();
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".ColorTransform = " + GetParamOrDefaultValue("0x" + this.ColorBlending.ToString("X8"), UserValue_ColorBlending) + ";";
                if (internalImages.Count == 1)
                {
                    List<String> lst = Project.StringListToList(Images, ';');
                    s += "\n\t" + Name + ".Image = " + GetParamOrDefaultValue("Res.Images." + lst[0], UserValue_Images) + ";";
                    return s;
                }
                else
                {
                    List<String> lst = Project.StringListToList(Images, ';');
                    s += "\n\t" + Name + ".ImageList = new GApp::Resources::Bitmap *[" + lst.Count.ToString() + "];";
                    s += "\n\t" + Name + ".ImageIndex = " + this.ImageIndex.ToString() + ";";
                    s += "\n\t" + Name + ".Count = " + lst.Count.ToString() + ";";
                    for (int tr = 0; tr < lst.Count; tr++)
                        s += "\n\t" + Name + ".ImageList[" + tr.ToString() + "] = Res.Images." + lst[tr] + ";";
                    s += "\n\t" + Name + ".Image = " + GetParamOrDefaultValue(Name + ".ImageList[" + Name + ".ImageIndex]", UserValue_Images) + ";";
                    return s;
                }
            }
            public override string GetCPPClassName()
            {
                if (internalImages.Count == 1)
                    return "SingleImageElement";
                return "MultipleImageElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_ColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ColorBlending, "unsigned int", "Color"));
                if (UserValue_Images.Length > 0)
                {
                    if (HasMultipleImages() == false)
                        p.ParametersList.Add(new ParameterInformation(UserValue_Images, "GApp::Resources::Bitmap *", "Bitmap"));
                }
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Setter_Image != YesNo.No)
                {
                    string nm = "Set" + Name + "Image";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "image", "image", "Bitmap", "", null));
                    m.Members[nm] = mb;
                }
                if (Getter_Image != YesNo.No)
                {
                    string nm = "Get" + Name + "Image";
                    m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "Bitmap", "", null);
                }
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "BlendColor");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Setter_Image != YesNo.No)
                    s += "\n\tvoid Set" + Name + "Image (GApp::Resources::Bitmap* __image__);";
                if (Getter_Image != YesNo.No)
                    s += "\nGApp::Resources::Bitmap* Get" + Name + "Image ();";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("BlendColor");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Setter_Image != YesNo.No)
                    s += "\nvoid " + className + "::Set" + Name + "Image(GApp::Resources::Bitmap* __image__) { ((GApp::Animations::Elements::SingleImageElement*)(&" + Name + "))->Image = __image__; };";
                if (Getter_Image != YesNo.No)
                    s += "\nGApp::Resources::Bitmap* " + className + "::Get" + Name + "Image() { return (((GApp::Animations::Elements::SingleImageElement*)(&" + Name + "))->Image); };";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "BlendColor");
                return s;
            }
            #endregion

            public bool HasMultipleImages()
            {
                return internalImages.Count > 1;
            }
        }
        public class TextElement : GenericElementWithPositionAndSize
        {
            [XmlAttribute()]
            public string Text = "";
            [XmlAttribute()]
            public string StringResource = "";
            [XmlAttribute()]
            public bool WordWrap = false, Justify = false, UseStringResource = false;
            [XmlAttribute()]
            public string Font = "";
            [XmlAttribute()]
            public Alignament TextAlignament = Alignament.Center;
            [XmlAttribute()]
            public float LineSpace = 0.0f, Size = 1f;
            [XmlAttribute()]
            public int ColorBlending = -1;
            [XmlAttribute()]
            public string UserValue_Font = "", UserValue_Size = "", UserValue_TextAlign = "", UserValue_Text = "", UserValue_Color = "";
            [XmlAttribute()]
            public YesNo Getter_Text = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Text = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            [XmlIgnore()]
            public TextPainter tp = new TextPainter();

            #region Atribute
            [XmlIgnore(), Category("Text"), DisplayName("Text")]
            public string _Text
            {
                get { return Text; }
                set { Text = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Resource"), Editor(typeof(StringSelectorEditor), typeof(UITypeEditor))]
            public string _StringResource
            {
                get { return StringResource; }
                set { StringResource = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Use string resources")]
            public bool _UseStringResource
            {
                get { return UseStringResource; }
                set { UseStringResource = value; }
            }

            [XmlIgnore(), Category("Font"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Word Wrap")]
            public bool _WordWrap
            {
                get { return WordWrap; }
                set { WordWrap = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Justify")]
            public bool _Jusity
            {
                get { return Justify; }
                set { Justify = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Font"), Description("Use '' string for the default font"), Editor(typeof(FontSelectorEditor), typeof(UITypeEditor))]
            public string _Font
            {
                get { return Font; }
                set { Font = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Text Alignament")]
            public Alignament _TextAlignament
            {
                get { return TextAlignament; }
                set { TextAlignament = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Line spaces")]
            public string _LineSpace
            {
                get { return Project.ProcentToString(LineSpace); }
                set
                {
                    float v = 0;
                    if (Project.StringToProcent(value, ref v) == false)
                        MessageBox.Show("Invalid percentage value");
                    LineSpace = v;
                }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Size")]
            public string _Size
            {
                get { return GetPercentageValue(Size); }
                set { Size = SetPercentageValue(value, Size); }
            }


            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Font")]
            public string _UserValue_Font
            {
                get { return UserValue_Font; }
                set { UserValue_Font = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Size")]
            public string _UserValue_Size
            {
                get { return UserValue_Size; }
                set { UserValue_Size = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Text Alignament")]
            public string _UserValue_TextAlign
            {
                get { return UserValue_TextAlign; }
                set { UserValue_TextAlign = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Text")]
            public string _UserValue_Text
            {
                get { return UserValue_Text; }
                set { UserValue_Text = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_Color
            {
                get { return UserValue_Color; }
                set { UserValue_Color = value; }
            }

            #endregion

            #region Getters
            [XmlIgnore(), Description("Specifies if a getter for element text is to be created."), Category("Getters"), DisplayName("Text")]
            public YesNo _Getter_Text
            {
                get { return Getter_Text; }
                set { Getter_Text = value; }
            }
            [XmlIgnore(), Description("Sets the text for this specific element"), Category("Setters"), DisplayName("Text")]
            public YesNo _Setter_Text
            {
                get { return Setter_Text; }
                set { Setter_Text = value; }
            }
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                this.Width = unscaled_widthInPixels;
                this.Height = unscaled_heightInPixels;
            }
            protected override string GetDescription()
            {
                if (UseStringResource)
                    return "Resource: " + this.StringResource;
                else
                    return "Text:" + this.Text;
            }
            public override string GetIconKey()
            {
                return "__Font__";
            }
            public override string GetTypeName()
            {
                return "Text";
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
                if (UseStringResource)
                    tp.SetText(this.StringResource, true, CurrentAppResources);
                else
                    tp.SetText(Text, false, CurrentAppResources);
                tp.SetSpaceWidth(-1);
                tp.SetCharacterSpacing(-1);
                tp.SetLineSpace(LineSpace);
                tp.SetAlignament(TextAlignament);
                tp.SetFont(Font);
                tp.SetWordWrap(WordWrap);
                tp.SetJustify(Justify);
                tp.SetCharactesVisibility(0, 0xFFFFFF, true);
                tp.ForceRecompute();
            }

            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                tp.SetPosition(ExecutionContext.ScreenRect.Left, ExecutionContext.ScreenRect.Top, ExecutionContext.ScreenRect.Right, ExecutionContext.ScreenRect.Bottom);
                float fontsz = Size;
                if (ExecutionContext.ScaleWidth < ExecutionContext.ScaleHeight)
                    fontsz *= ExecutionContext.ScaleWidth;
                else
                    fontsz *= ExecutionContext.ScaleHeight;
                tp.SetFontSize(TextPainter.FontSizeMethod.Scale, fontsz);
                tp.SetBlending(BlendingMode.ColorBlending, (int)ExecutionContext.ColorBlending, 1.0f);
                tp.Paint(c, CurrentAppResources);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                if (resources.Fonts.ContainsKey(Font) == false)
                    return "Font '" + Font + "' does not exists !";
                if (this.UseStringResource)
                {
                    if (resources.Strings.ContainsKey(StringResource) == false)
                        return "String resource: '" + StringResource + "' does not exists !";
                }
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".CallVirtualFunction = true;";
                s += "\n\t" + Name + ".fontSize = " + GetParamOrDefaultValue(this.Size.ToString(), UserValue_Size) + ";";
                s += "\n\t" + Name + ".TP.SetFont(Res.Fonts." + GetParamOrDefaultValue(this.Font, UserValue_Font) + ");";
                s += "\n\t" + Name + ".TP.SetFontSize(GAC_FONTSIZETYPE_SCALE, " + GetParamOrDefaultValue(this.Size.ToString(), UserValue_Size) + ");";
                s += "\n\t" + Name + ".TP.SetLineSpace(" + this.LineSpace.ToString() + ");";
                s += "\n\t" + Name + ".TP.SetTextJustify(" + this.Justify.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetWordWrap(" + this.WordWrap.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetColorBlending(" + GetParamOrDefaultValue("0x" + this.ColorBlending.ToString("X8"), UserValue_Color) + ");";
                s += "\n\t" + Name + ".ColorTransform = " + GetParamOrDefaultValue("0x" + this.ColorBlending.ToString("X8"), UserValue_Color) + ";";
                s += "\n\t" + Name + ".TP.SetViewRectWH(" + Name + ".X * this->Width," + Name + ".Y * this->Height," + Name + ".Alignament," + Name + ".WidthInPixels * this->Width," + Name + ".HeightInPixels * this->Height);";
                s += "\n\t" + Name + ".TP.SetDockPosition(" + GetParamOrDefaultValue("GAC_ALIGNAMENT_" + this.TextAlignament.ToString().ToUpper(), UserValue_TextAlign) + ");";
                if (UserValue_Text.Length > 0)
                {
                    s += "\n\t" + Name + ".TP.SetText(param_" + UserValue_Text + ");";
                }
                else
                {
                    if (this.UseStringResource)
                        s += "\n\t" + Name + ".TP.SetText(Res.Strings." + this.StringResource + ");";
                    else
                        s += "\n\t" + Name + ".TP.SetText(\"" + this.Text + "\");";
                }
                return s;
            }
            public override string GetCPPClassName()
            {
                return "TextElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_Font.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Font, "GApp::Resources::Font*", "Font"));
                if (UserValue_Size.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Size, "float"));
                if (UserValue_TextAlign.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_TextAlign, "unsigned int", "Alignament"));
                if (UserValue_Color.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Color, "unsigned int", "Color"));
                if (UserValue_Text.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Text, "GApp::Utils::String", "string"));
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Getter_Position != PositionGetterType.No)
                    CreateGetterForPosition(m);
                if (Setter_Text != YesNo.No)
                {
                    string nm = "Set" + Name + "Text";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "text", "text", "string", "", null));
                    m.Members[nm] = mb;
                }
                if (Getter_Text != YesNo.No)
                {
                    string nm = "Get" + Name + "Text";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "bool", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "text", "text", "String", "", null));
                    m.Members[nm] = mb;
                }
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "Color");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = "";
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPHeaderDefinition();
                if (Setter_Text != YesNo.No)
                    s += "\n\tvoid Set" + Name + "Text (const char* __text__);";
                if (Getter_Text != YesNo.No)
                    s += "\n\tbool Get" + Name + "Text (GApp::Utils::String *copy);";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("Color");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = "";
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPImplementation(className, Getter_Position);
                if (Setter_Text != YesNo.No)
                    s += "\nvoid " + className + "::Set" + Name + "Text(const char* __text__) { ((GApp::Animations::Elements::TextElement*)(&" + Name + "))->TP.SetText(__text__); };";
                if (Getter_Text != YesNo.No)
                    s += "\nbool " + className + "::Get" + Name + "Text(GApp::Utils::String *__copy__) { return ((GApp::Animations::Elements::TextElement*)(&" + Name + "))->TP.CopyText(__copy__); };";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "Color");
                return s;
            }
            #endregion
        }
        public class EntireSurfaceElement : GenericElement
        {
            [XmlAttribute()]
            public int ColorBlending = 0x7F000000;
            [XmlAttribute()]
            public string UserValue_Color = "";
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Surface"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_Color
            {
                get { return UserValue_Color; }
                set { UserValue_Color = value; }
            }
            #endregion

            #region Getters
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            private List<Bitmap> internalImages = new List<Bitmap>();
            protected override string GetDescription()
            {
                return "Surface color: " + Color.FromArgb(ColorBlending).ToString();
            }
            public override string GetIconKey()
            {
                return "__EntireSurface__";
            }
            public override string GetTypeName()
            {
                return "Entire Surface";
            }
            public override void InitRuntimeContext()
            {
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
                ExecutionContext.Visible = Visible;
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.DrawRect(0, 0, c.GetWidth(), c.GetHeight(), Alignament.TopLeft, 0, (int)ExecutionContext.ColorBlending, 0);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = "";
                s += "\n\t" + Name + ".ColorTransform = " + GetParamOrDefaultValue("0x" + ColorBlending.ToString("X8"), UserValue_Color) + ";";
                s += "\n\t" + Name + ".Visible = " + GetParamOrDefaultValue(this.Visible.ToString().ToLower(), UserValue_Visible) + ";";
                return s;
            }
            public override string GetCPPClassName()
            {
                return "EntireSurfaceElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Color.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Color, "unsigned int", "Color"));
            }

            public override void AddAnimationFunction(GACParser.Module m)
            {
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "Color");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = "";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("Color");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = "";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "Color");
                return s;
            }
            #endregion
        }
        public class RectangleElement : GenericElementWithPositionAndSize
        {

            [XmlAttribute()]
            public string UserValue_ColorBlending = "";
            [XmlAttribute()]
            public int ColorBlending = -1;
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Color"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_ColorBlending
            {
                get { return UserValue_ColorBlending; }
                set { UserValue_ColorBlending = value; }
            }
            #endregion

            #region Getters
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                this.Width = unscaled_widthInPixels;
                this.Height = unscaled_heightInPixels;
            }
            public override string GetIconKey()
            {
                return "__Rectangle__";
            }
            public override string GetTypeName()
            {
                return "Rectangle";
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.FillRect(this.ExecutionContext);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".ColorTransform = " + GetParamOrDefaultValue("0x" + this.ColorBlending.ToString("X8"), UserValue_ColorBlending) + ";";
                return s;

            }
            public override string GetCPPClassName()
            {
                return "RectangleElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_ColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ColorBlending, "unsigned int", "Color"));
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "Color");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("Color");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "Color");
                return s;
            }
            #endregion

        }
        public class ExclusionRectangleElement : RectangleElement
        {
            #region Virtual Functions
            public override string GetIconKey()
            {
                return "__ExclusionRect__";
            }
            public override string GetTypeName()
            {
                return "Exclusion Rectangle";
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.FillExclusionRect(this.ExecutionContext);
            }
            public override string GetCPPClassName()
            {
                return "ExclusionRectElement";
            }
            #endregion

        }
        public class ClipRectangleElement : GenericElementWithPositionAndSize
        {
            #region Virtual Functions
            public override string GetIconKey()
            {
                return "__ClipRect__";
            }
            public override string GetTypeName()
            {
                return "Clipping";
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.EnableClipping(this.ExecutionContext);
            }
            public override string GetCPPClassName()
            {
                return "ClippingElement";
            }
            #endregion

        }
        public class DisableClippingElement : GenericElement
        {
            #region Virtual Functions
            protected override string GetDescription()
            {
                return "Disable clipping";
            }
            public override string GetIconKey()
            {
                return "__DisableClipping__";
            }
            public override string GetTypeName()
            {
                return "Disable Clipping";
            }
            public override void InitRuntimeContext()
            {
                ExecutionContext.Visible = Visible;
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.ClearClipping();
            }
            public override string Validate(Project prj, AppResources resources)
            {
                return null;
            }
            public override string GetCPPClassName()
            {
                return "DisableClippingElement";
            }
            #endregion
        }
        public enum SimpleButtonBackgroundStyle
        {
            Image,
            Rectangle,
        }
        public class ButtonFaceContainer
        {
            public string BackgroundImage = "";
            public int BackgroundColorBlending = -1;

            public string SymbolImage = "";
            public Alignament SymbolAlign = Alignament.Center;
            public float SymbolScaleWidth = 1.0f;
            public float SymbolScaleHeight = 1.0f;
            public float SymbolX = 0.5f;
            public float SymbolY = 0.5f;
            public int SymbolColorBlending = -1;

            public string Text = "";
            public string TextStringResource = "";
            public bool TextWordWrap = false, TextJustify = false, TextUseStringResource = false;
            public string TextFont = "";
            public Alignament TextAlignament = Alignament.Center;
            public float TextLineSpace = 0.0f, TextSize = 1f;
            public int TextColorBlending = -1;
            public float TextX = 0.5f;
            public float TextY = 0.5f;
            public float TextRectangleWidth = 100;
            public float TextRectangleHeight = 100;
            public Alignament TextRectangleAlignament = Alignament.Center;

            public string UserValue_BackgroundImage = "", UserValue_BackgroundColorBlending = "";
            public string UserValue_SymbolImage = "", UserValue_SymbolColorBlending = "", UserValue_SymbolAlign = "";
            public string UserValue_SymbolScaleWidth = "", UserValue_SymbolScaleHeight = "", UserValue_SymbolX = "", UserValue_SymbolY = "";
            public string UserValue_TextColorBlending = "", UserValue_Text = "";

            public Bitmap Background = null;
            public Bitmap Symbol = null;
            public TextPainter tp = new TextPainter();

            #region Atribute background
            [XmlIgnore(), Category("Background"), DisplayName("Image"), Editor(typeof(ImageSelectorEditor), typeof(UITypeEditor))]
            public string _BackgroundImage
            {
                get { return BackgroundImage; }
                set { BackgroundImage = value; }
            }
            [XmlIgnore(), Category("Background"), DisplayName("Blend Color")]
            public System.Drawing.Color _BackgroundColorBlending
            {
                get { return System.Drawing.Color.FromArgb(BackgroundColorBlending); }
                set { BackgroundColorBlending = value.ToArgb(); }
            }
            #endregion

            #region Atribute Simbol
            [XmlIgnore(), Category("Symbol"), DisplayName("Image"), Editor(typeof(ImageSelectorEditor), typeof(UITypeEditor))]
            public string _SymbolImage
            {
                get { return SymbolImage; }
                set { SymbolImage = value; }
            }
            [XmlIgnore(), Description("Aliganment"), Category("Symbol"), DisplayName("Alignament")]
            public Alignament _SymbolAlign
            {
                get { return SymbolAlign; }
                set { SymbolAlign = value; }
            }
            [XmlIgnore(), Description("Scale width"), Category("Symbol"), DisplayName("Scale Width")]
            public string _SymbolScaleWidth
            {
                get { return Project.ProcentToString(SymbolScaleWidth); }
                set { SymbolScaleWidth = Project.StringToProcent(value, 0, 1000, SymbolScaleWidth); }
            }
            [XmlIgnore(), Description("Scale height"), Category("Symbol"), DisplayName("Scale Height")]
            public string _SymbolScaleHeight
            {
                get { return Project.ProcentToString(SymbolScaleHeight); }
                set { SymbolScaleHeight = Project.StringToProcent(value, 0, 1000, SymbolScaleHeight); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Symbol"), DisplayName("X")]
            public string _SymbolX
            {
                get { return Project.ProcentToString(this.SymbolX); }
                set { SymbolX = Project.StringToProcent(value, -1000, 1000, SymbolX); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Symbol"), DisplayName("Y")]
            public string _SymbolY
            {
                get { return Project.ProcentToString(this.SymbolY); }
                set { SymbolY = Project.StringToProcent(value, -1000, 1000, SymbolY); }
            }
            [XmlIgnore(), Category("Symbol"), DisplayName("Blend Color")]
            public System.Drawing.Color _SymbolColorBlending
            {
                get { return System.Drawing.Color.FromArgb(SymbolColorBlending); }
                set { SymbolColorBlending = value.ToArgb(); }
            }
            #endregion

            #region Atribute Text
            [XmlIgnore(), Category("Text"), DisplayName("Text")]
            public string _Text
            {
                get { return Text; }
                set { Text = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Resource"), Editor(typeof(StringSelectorEditor), typeof(UITypeEditor))]
            public string _StringResource
            {
                get { return TextStringResource; }
                set { TextStringResource = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Use string resources")]
            public bool _UseStringResource
            {
                get { return TextUseStringResource; }
                set { TextUseStringResource = value; }
            }

            [XmlIgnore(), Category("Text"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(TextColorBlending); }
                set { TextColorBlending = value.ToArgb(); }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Word Wrap")]
            public bool _WordWrap
            {
                get { return TextWordWrap; }
                set { TextWordWrap = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Justify")]
            public bool _Jusity
            {
                get { return TextJustify; }
                set { TextJustify = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Font"), Description("Use '' string for the default font"), Editor(typeof(FontSelectorEditor), typeof(UITypeEditor))]
            public string _Font
            {
                get { return TextFont; }
                set { TextFont = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Text Alignament")]
            public Alignament _TextAlignament
            {
                get { return TextAlignament; }
                set { TextAlignament = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Line spaces")]
            public string _LineSpace
            {
                get { return Project.ProcentToString(TextLineSpace); }
                set
                {
                    float v = 0;
                    if (Project.StringToProcent(value, ref v) == false)
                        MessageBox.Show("Invalid percentage value");
                    TextLineSpace = v;
                }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Size")]
            public string _Size
            {
                get { return Project.ProcentToString(TextSize); }
                set { TextSize = Project.StringToProcent(value, 0, 1000, TextSize); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Text Layout"), DisplayName("X")]
            public string _TextX
            {
                get { return Project.ProcentToString(this.TextX); }
                set { TextX = Project.StringToProcent(value, -1000, 1000, TextX); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Text Layout"), DisplayName("Y")]
            public string _TextY
            {
                get { return Project.ProcentToString(this.TextY); }
                set { TextY = Project.StringToProcent(value, -1000, 1000, TextY); }
            }
            [XmlIgnore(), Category("Text Layout"), DisplayName("Text Rectangle Alignament")]
            public Alignament _TextRectangleAlignament
            {
                get { return TextRectangleAlignament; }
                set { TextRectangleAlignament = value; }
            }
            [XmlIgnore(), Description("Text rectangle with"), Category("Text Layout"), DisplayName("Width")]
            public string _TextRectangleWidth
            {
                get { return AnimO.GenericElement.GetSizeInPixels(TextRectangleWidth); }
                set { TextRectangleWidth = AnimO.GenericElement.SetSizeInPixels(value, TextRectangleWidth); }
            }
            [XmlIgnore(), Description("Text rectangle  height"), Category("Text Layout"), DisplayName("Height")]
            public string _TextRectangleHeight
            {
                get { return AnimO.GenericElement.GetSizeInPixels(TextRectangleHeight); }
                set { TextRectangleHeight = AnimO.GenericElement.SetSizeInPixels(value, TextRectangleHeight); }
            }
            #endregion

            #region Initialization parameters (user dependent)
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Background"), DisplayName("Image")]
            public string _UserValue_BackgroundImage
            {
                get { return UserValue_BackgroundImage; }
                set { UserValue_BackgroundImage = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Background"), DisplayName("Blend Color")]
            public string _UserValue_BackgroundColorBlending
            {
                get { return UserValue_BackgroundColorBlending; }
                set { UserValue_BackgroundColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Image")]
            public string _UserValue_SymbolImage
            {
                get { return UserValue_SymbolImage; }
                set { UserValue_SymbolImage = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Blend Color")]
            public string _UserValue_SymbolColorBlending
            {
                get { return UserValue_SymbolColorBlending; }
                set { UserValue_SymbolColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Alignament")]
            public string _UserValue_SymbolAlign
            {
                get { return UserValue_SymbolAlign; }
                set { UserValue_SymbolAlign = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Scale Width")]
            public string _UserValue_SymbolScaleWidth
            {
                get { return UserValue_SymbolScaleWidth; }
                set { UserValue_SymbolScaleWidth = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Scale Height")]
            public string _UserValue_SymbolScaleHeight
            {
                get { return UserValue_SymbolScaleHeight; }
                set { UserValue_SymbolScaleHeight = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("X")]
            public string _UserValue_SymbolX
            {
                get { return UserValue_SymbolX; }
                set { UserValue_SymbolX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Y")]
            public string _UserValue_SymbolY
            {
                get { return UserValue_SymbolY; }
                set { UserValue_SymbolY = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Text"), DisplayName("Blend Color")]
            public string _UserValue_TextColorBlending
            {
                get { return UserValue_TextColorBlending; }
                set { UserValue_TextColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Text"), DisplayName("Text")]
            public string _UserValue_Text
            {
                get { return UserValue_Text; }
                set { UserValue_Text = value; }
            }
            #endregion

            private string GetValue(Dictionary<string, string> d, string key, string defaultValue)
            {
                string val;
                if (d.TryGetValue(key, out val))
                    return val;
                return defaultValue;
            }
            private int GetValue(Dictionary<string, string> d, string key, int defaultValue)
            {
                string val;
                int iVal;
                if (d.TryGetValue(key, out val))
                {
                    if (int.TryParse(val, out iVal))
                        return iVal;
                    return defaultValue;
                }
                return defaultValue;
            }
            private bool GetValue(Dictionary<string, string> d, string key, bool defaultValue)
            {
                string val;
                if (d.TryGetValue(key, out val))
                {
                    return (val.Equals("true", StringComparison.InvariantCultureIgnoreCase));
                }
                return defaultValue;
            }
            private Alignament GetValue(Dictionary<string, string> d, string key, Alignament defaultValue)
            {
                string val;
                Alignament aVal;
                if (d.TryGetValue(key, out val))
                {
                    if (Enum.TryParse<Alignament>(val, out aVal))
                        return aVal;
                    return defaultValue;
                }
                return defaultValue;
            }
            public bool CreateFromString(string format)
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                List<string> lst = Project.StringListToList(format, ';');
                if (lst == null)
                    return false;
                foreach (string item in lst)
                {
                    int idx = item.IndexOf(':');
                    if (idx >= 0)
                    {
                        d[item.Substring(0, idx)] = item.Substring(idx + 1).Trim();
                    }
                }
                // fac asignarile
                BackgroundImage = GetValue(d, "BackgroundImage", BackgroundImage);
                BackgroundColorBlending = GetValue(d, "BackgroundColorBlending", BackgroundColorBlending);

                SymbolImage = GetValue(d, "SymbolImage", SymbolImage);
                SymbolAlign = GetValue(d, "SymbolAlign", SymbolAlign);
                _SymbolScaleWidth = GetValue(d, "SymbolScaleWidth", _SymbolScaleWidth);
                _SymbolScaleHeight = GetValue(d, "SymbolScaleHeight", _SymbolScaleHeight);
                _SymbolX = GetValue(d, "SymbolX", _SymbolX);
                _SymbolY = GetValue(d, "SymbolY", _SymbolY);
                SymbolColorBlending = GetValue(d, "SymbolColorBlending", SymbolColorBlending);

                Text = GetValue(d, "Text", Text);
                TextStringResource = GetValue(d, "TextStringResource", TextStringResource);
                TextWordWrap = GetValue(d, "TextWordWrap", TextWordWrap);
                TextJustify = GetValue(d, "TextJustify", TextJustify);
                TextUseStringResource = GetValue(d, "TextUseStringResource", TextUseStringResource);
                TextFont = GetValue(d, "TextFont", TextFont);
                TextAlignament = GetValue(d, "TextAlignament", TextAlignament);
                _LineSpace = GetValue(d, "TextLineSpace", _LineSpace);
                _Size = GetValue(d, "TextSize", _Size);
                TextColorBlending = GetValue(d, "TextColorBlending", TextColorBlending);
                _TextX = GetValue(d, "TextX", _TextX);
                _TextY = GetValue(d, "TextY", _TextY);
                _TextRectangleWidth = GetValue(d, "TextRectangleWidth", _TextRectangleWidth);
                _TextRectangleHeight = GetValue(d, "TextRectangleHeight", _TextRectangleHeight);
                TextRectangleAlignament = GetValue(d, "TextRectangleAlignament", TextRectangleAlignament);

                UserValue_BackgroundImage = GetValue(d, "UserValue_BackgroundImage", UserValue_BackgroundImage);
                UserValue_BackgroundColorBlending = GetValue(d, "UserValue_BackgroundColorBlending", UserValue_BackgroundColorBlending);

                UserValue_SymbolImage = GetValue(d, "UserValue_SymbolImage", UserValue_SymbolImage);
                UserValue_SymbolColorBlending = GetValue(d, "UserValue_SymbolColorBlending", UserValue_SymbolColorBlending);
                UserValue_SymbolAlign = GetValue(d, "UserValue_SymbolAlign", UserValue_SymbolAlign);
                UserValue_SymbolScaleWidth = GetValue(d, "UserValue_SymbolScaleWidth", UserValue_SymbolScaleWidth);
                UserValue_SymbolScaleHeight = GetValue(d, "UserValue_SymbolScaleHeight", UserValue_SymbolScaleHeight);
                UserValue_SymbolX = GetValue(d, "UserValue_SymbolX", UserValue_SymbolX);
                UserValue_SymbolY = GetValue(d, "UserValue_SymbolY", UserValue_SymbolY);

                UserValue_TextColorBlending = GetValue(d, "UserValue_TextColorBlending", UserValue_TextColorBlending);
                UserValue_Text = GetValue(d, "UserValue_Text", UserValue_Text);
                return true;
            }
            public string CreateString()
            {
                string s = "";
                s += "BackgroundImage:" + BackgroundImage + ";";
                s += "BackgroundColorBlending:" + BackgroundColorBlending.ToString() + ";";

                s += "SymbolImage:" + SymbolImage + ";";
                s += "SymbolAlign:" + SymbolAlign.ToString() + ";";
                s += "SymbolScaleWidth:" + _SymbolScaleWidth + ";";
                s += "SymbolScaleHeight:" + _SymbolScaleHeight + ";";
                s += "SymbolX:" + _SymbolX + ";";
                s += "SymbolY:" + _SymbolY + ";";
                s += "SymbolColorBlending:" + SymbolColorBlending.ToString() + ";";

                s += "Text:" + Text + ";";
                s += "TextStringResource:" + TextStringResource + ";";
                s += "TextWordWrap:" + TextWordWrap.ToString() + ";";
                s += "TextJustify:" + TextJustify.ToString() + ";";
                s += "TextUseStringResource:" + TextUseStringResource.ToString() + ";";
                s += "TextFont:" + TextFont + ";";
                s += "TextAlignament:" + TextAlignament.ToString() + ";";
                s += "TextLineSpace:" + _LineSpace + ";";
                s += "TextSize:" + _Size + ";";
                s += "TextColorBlending:" + TextColorBlending.ToString() + ";";
                s += "TextX:" + _TextX + ";";
                s += "TextY:" + _TextY + ";";
                s += "TextRectangleWidth:" + _TextRectangleWidth + ";";
                s += "TextRectangleHeight:" + _TextRectangleHeight + ";";
                s += "TextRectangleAlignament:" + TextRectangleAlignament.ToString() + ";";


                s += "UserValue_BackgroundImage:" + UserValue_BackgroundImage + ";";
                s += "UserValue_BackgroundColorBlending:" + UserValue_BackgroundColorBlending + ";";

                s += "UserValue_SymbolImage:" + UserValue_SymbolImage + ";";
                s += "UserValue_SymbolColorBlending:" + UserValue_SymbolColorBlending + ";";
                s += "UserValue_SymbolAlign:" + UserValue_SymbolAlign + ";";
                s += "UserValue_SymbolScaleWidth:" + UserValue_SymbolScaleWidth + ";";
                s += "UserValue_SymbolScaleHeight:" + UserValue_SymbolScaleHeight + ";";
                s += "UserValue_SymbolX:" + UserValue_SymbolX + ";";
                s += "UserValue_SymbolY:" + UserValue_SymbolY + ";";

                s += "UserValue_TextColorBlending:" + UserValue_TextColorBlending + ";";
                s += "UserValue_Text:" + UserValue_Text + ";";
                return s;
            }
            public string Validate(AppResources resources, SimpleButtonBackgroundStyle mode)
            {
                Background = null;
                Symbol = null;
                if (mode == SimpleButtonBackgroundStyle.Image)
                {
                    if (BackgroundImage.Length == 0)
                        return "Background image is mandatory for a button !";
                    if (resources.Images.ContainsKey(BackgroundImage) == false)
                        return "Unknwon bakcground image: " + BackgroundImage;
                    Background = resources.Images[BackgroundImage].Picture;
                }
                else
                {
                    if (BackgroundImage.Length > 0)
                    {
                        if (resources.Images.ContainsKey(BackgroundImage) == false)
                            return "Unknwon bakcground image: " + BackgroundImage;
                        Background = resources.Images[BackgroundImage].Picture;
                    }
                }
                if (SymbolImage.Length > 0)
                {
                    if (resources.Images.ContainsKey(SymbolImage) == false)
                        return "Unknwon symbol image: " + SymbolImage;
                    Symbol = resources.Images[SymbolImage].Picture;
                }
                if (TextFont.Length > 0)
                {
                    if (resources.Fonts.ContainsKey(TextFont) == false)
                        return "Unknwon font: " + TextFont;
                    if ((TextUseStringResource) && (resources.Strings.ContainsKey(TextStringResource) == false))
                        return "Unknwon string: " + TextStringResource;
                }
                return null;
            }
            public void InitRuntimeContext(RuntimeContext ExecutionContext, RuntimeContext symbolExecutionContext, RuntimeContext textExecutionContext, SimpleButtonBackgroundStyle BackgroundStyle)
            {
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    if (Background != null)
                    {
                        ExecutionContext.WidthInPixels = Background.Width;
                        ExecutionContext.HeightInPixels = Background.Height;
                    }
                    else
                    {
                        ExecutionContext.WidthInPixels = 0;
                        ExecutionContext.HeightInPixels = 0;
                    }
                }
                else
                {
                    // el vine deja setat cu dimensiunea din base.Init(...) - dar e dimensiunea pentru dreptunghi
                }
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, this.BackgroundColorBlending, 1.0f);
                ExecutionContext.Image = this.Background;

                symbolExecutionContext.Image = this.Symbol;
                symbolExecutionContext.X_Percentage = this.SymbolX;
                symbolExecutionContext.Y_Percentage = this.SymbolY;
                symbolExecutionContext.Align = this.SymbolAlign;
                symbolExecutionContext.ScaleWidth = this.SymbolScaleWidth;
                symbolExecutionContext.ScaleHeight = this.SymbolScaleHeight;
                symbolExecutionContext.Visible = true;
                symbolExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, this.SymbolColorBlending, 1.0f);

                // text
                textExecutionContext.X_Percentage = this.TextX;
                textExecutionContext.Y_Percentage = this.TextY;
                textExecutionContext.Align = this.TextRectangleAlignament;
                textExecutionContext.ScaleWidth = this.TextSize;
                textExecutionContext.ScaleHeight = this.TextSize;
                textExecutionContext.Visible = true;
                textExecutionContext.WidthInPixels = this.TextRectangleWidth;
                textExecutionContext.HeightInPixels = this.TextRectangleHeight;
                textExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, this.TextColorBlending, 1.0f);
                if (TextUseStringResource)
                    tp.SetText(this.TextStringResource, true, GenericElement.CurrentAppResources);
                else
                    tp.SetText(Text, false, GenericElement.CurrentAppResources);
                tp.SetSpaceWidth(-1);
                tp.SetCharacterSpacing(-1);
                tp.SetLineSpace(TextLineSpace);
                tp.SetAlignament(TextAlignament);
                tp.SetFont(TextFont);
                tp.SetWordWrap(TextWordWrap);
                tp.SetJustify(TextJustify);
                tp.SetCharactesVisibility(0, 0xFFFFFF, true);
                tp.ForceRecompute();

            }
            public static void Paint(Canvas c, RectangleF tempRect, RuntimeContext ExecutionContext, RuntimeContext symbolExecutionContext, RuntimeContext textExecutionContext, TextPainter tp, SimpleButtonBackgroundStyle style, int compRectColor = 0)
            {
                if (style == SimpleButtonBackgroundStyle.Image)
                {
                    c.DrawImage(ExecutionContext);
                    c.ComputeRectInPercentages(ExecutionContext, true, ref tempRect);
                    if (compRectColor != 0)
                        c.DrawObjectRect(ExecutionContext, true, compRectColor);
                }
                else
                {
                    c.FillRect(ExecutionContext);
                    c.ComputeRectInPercentages(ExecutionContext, false, ref tempRect);
                    if (compRectColor != 0)
                        c.DrawObjectRect(ExecutionContext, false, compRectColor);
                }


                // simbol
                GenericElement.ComputeScreenRect(symbolExecutionContext, true, ExecutionContext.ScreenRect);
                c.DrawImage(symbolExecutionContext);
                if (compRectColor != 0)
                    c.DrawObjectRect(symbolExecutionContext, true, compRectColor);

                GenericElement.ComputeScreenRect(textExecutionContext, true, ExecutionContext.ScreenRect);
                tp.SetPosition(textExecutionContext.ScreenRect.Left, textExecutionContext.ScreenRect.Top, textExecutionContext.ScreenRect.Right, textExecutionContext.ScreenRect.Bottom);
                tp.SetFontSize(TextPainter.FontSizeMethod.Scale, textExecutionContext.ScaleWidth);
                tp.SetBlending(BlendingMode.ColorBlending, (int)textExecutionContext.ColorBlending, 1.0f);
                tp.Paint(c, GenericElement.CurrentAppResources);
            }
            private string GetParamOrDefaultValue(string defaultValue, string paramName)
            {
                if (paramName.Length > 0)
                    return "param_" + paramName;
                return defaultValue;
            }
            public string CreateOnStartCPPCode(AnimationObject animObj, string Name)
            {
                string s = "";
                s += "\n\t//------ Code for face: " + Name;
                if (BackgroundImage.Length == 0)
                    s += "\n\t" + Name + ".Background = " + GetParamOrDefaultValue("NULL", UserValue_BackgroundImage) + ";";
                else
                    s += "\n\t" + Name + ".Background = " + GetParamOrDefaultValue("Res.Images." + BackgroundImage, UserValue_BackgroundImage) + ";";
                s += "\n\t" + Name + ".BackgroundColorBlending = " + GetParamOrDefaultValue("0x" + this.BackgroundColorBlending.ToString("X8"), UserValue_BackgroundColorBlending) + ";";

                if (SymbolImage.Length == 0)
                    s += "\n\t" + Name + ".Symbol = " + GetParamOrDefaultValue("NULL", UserValue_SymbolImage) + ";";
                else
                    s += "\n\t" + Name + ".Symbol = " + GetParamOrDefaultValue("Res.Images." + SymbolImage, UserValue_SymbolImage) + ";";
                s += "\n\t" + Name + ".SymbolColorBlending = " + GetParamOrDefaultValue("0x" + this.SymbolColorBlending.ToString("X8"), UserValue_SymbolColorBlending) + ";";
                s += "\n\t" + Name + ".SymbolScaleWidth = " + GetParamOrDefaultValue(SymbolScaleWidth.ToString(), UserValue_SymbolScaleWidth) + ";";
                s += "\n\t" + Name + ".SymbolScaleHeight = " + GetParamOrDefaultValue(SymbolScaleHeight.ToString(), UserValue_SymbolScaleHeight) + ";";
                s += "\n\t" + Name + ".SymbolX = " + GetParamOrDefaultValue(SymbolX.ToString(), UserValue_SymbolX) + ";";
                s += "\n\t" + Name + ".SymbolY = " + GetParamOrDefaultValue(SymbolY.ToString(), UserValue_SymbolY) + ";";
                s += "\n\t" + Name + ".SymbolAlign = " + GetParamOrDefaultValue("GAC_ALIGNAMENT_" + this.SymbolAlign.ToString().ToUpper(), UserValue_SymbolAlign) + ";";
                // Text
                if (TextFont.Length == 0)
                    s += "\n\t" + Name + ".TP.SetFont(NULL);";
                else
                    s += "\n\t" + Name + ".TP.SetFont(Res.Fonts." + this.TextFont + ");";
                s += "\n\t" + Name + ".TP.SetFontSize(GAC_FONTSIZETYPE_SCALE, " + this.TextSize.ToString() + ");";
                s += "\n\t" + Name + ".TP.SetLineSpace(" + this.TextLineSpace.ToString() + ");";
                s += "\n\t" + Name + ".TP.SetTextJustify(" + this.TextJustify.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetWordWrap(" + this.TextWordWrap.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetColorBlending(" + GetParamOrDefaultValue("0x" + this.TextColorBlending.ToString("X8"), UserValue_TextColorBlending) + ");";
                s += "\n\t" + Name + ".TextColorBlending = " + GetParamOrDefaultValue("0x" + this.TextColorBlending.ToString("X8"), UserValue_TextColorBlending) + ";";
                s += "\n\t" + Name + ".TP.SetDockPosition(GAC_ALIGNAMENT_" + this.TextAlignament.ToString().ToUpper() + ");";
                //s += "\n\t" + Name + ".TP.SetViewRectWH(" + Name + ".X * this->Width," + Name + ".Y * this->Height," + Name + ".Alignament," + Name + ".WidthInPixels * this->Width," + Name + ".HeightInPixels * this->Height);";
                s += "\n\t" + Name + ".TextRectX = " + TextX.ToString() + ";";
                s += "\n\t" + Name + ".TextRectY = " + TextY.ToString() + ";";
                s += "\n\t" + Name + ".TextRectAlign = GAC_ALIGNAMENT_" + this.TextRectangleAlignament.ToString().ToUpper() + ";";
                s += "\n\t" + Name + ".TextRectWidth = " + TextRectangleWidth.ToString() + " * (Core.ResolutionAspectRatio);";
                s += "\n\t" + Name + ".TextRectHeight = " + TextRectangleHeight.ToString() + " * (Core.ResolutionAspectRatio);";

                if (UserValue_Text.Length > 0)
                {
                    s += "\n\t" + Name + ".TP.SetText(param_" + UserValue_Text + ");";
                }
                else
                {
                    if (this.TextUseStringResource)
                        s += "\n\t" + Name + ".TP.SetText(Res.Strings." + this.TextStringResource + ");";
                    else
                        s += "\n\t" + Name + ".TP.SetText(\"" + this.Text + "\");";
                }
                return s;
            }
            public void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_SymbolX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolX, "float"));
                if (UserValue_SymbolY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolY, "float"));
                if (UserValue_SymbolScaleWidth.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolScaleWidth, "float"));
                if (UserValue_SymbolScaleHeight.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolScaleHeight, "float"));
                if (UserValue_SymbolAlign.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolAlign, "unsigned int", "Alignament"));
                if (UserValue_BackgroundColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_BackgroundColorBlending, "unsigned int", "Color"));
                if (UserValue_SymbolColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolColorBlending, "unsigned int", "Color"));
                if (UserValue_TextColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_TextColorBlending, "unsigned int", "Color"));
                if (UserValue_BackgroundImage.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_BackgroundImage, "GApp::Resources::Bitmap *", "Bitmap"));
                if (UserValue_SymbolImage.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolImage, "GApp::Resources::Bitmap *", "Bitmap"));
                if (UserValue_Text.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Text, "GApp::Utils::String", "string"));
            }
        }
        public class ButtonAction
        {
            public enum ActionType
            {
                None,
                SendEvent,
                OpenRatePage,
                OpenDeveloperPage,
                OpenFacebookPage,
                OpenBrowser,
                GoToScene,
                CloseApplication,
                ChangeSoundSettingsOnOff,
                ChangeSoundSettingsOnSfxOff,
                ShowRewardableAd,
            };
            public string ActionName = null;
            public bool PerformActionAfterAnimationEnds = false;
            public ActionType ActionID = ActionType.None;
            public static ButtonAction CreateFromString(string strFormat)
            {
                return null;
            }
            public static string[] GetActionsList()
            {
                return Enum.GetNames(typeof(ActionType));
            }
        }
        public class SimpleButtonElement : GenericElementWithPositionAndSize
        {
            [XmlAttribute()]
            public string Normal = "";
            [XmlAttribute()]
            public string Pressed = "";
            [XmlAttribute()]
            public string Inactive = "";
            [XmlAttribute()]
            public bool Enabled = true;
            [XmlAttribute()]
            public bool SendEventWhenAnimationEnds = false;
            [XmlAttribute()]
            public string ClickEvent = "";
            [XmlAttribute()]
            public SimpleButtonBackgroundStyle BackgroundStyle = SimpleButtonBackgroundStyle.Image;
            [XmlAttribute()]
            public string SoundName = "";


            [XmlIgnore()]
            public int EventIDValue = -1;



            [XmlAttribute()]
            public string UserValue_Enabled = "", UserValue_Event = "";
            [XmlAttribute()]
            public string UserValue_Sound = "";

            [XmlAttribute()]
            public SimpleButtonSetter Setter_Background_Image = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Symbol_Image = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Background_Color = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Symbol_Color = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Text_Color = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Text = SimpleButtonSetter.No;
            [XmlAttribute()]
            public YesNo Getter_Enabled = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Enabled = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Button"), DisplayName("Normal state"), Description("Aspect for normal button state"), Editor(typeof(ButtonStateSelectorEditor), typeof(UITypeEditor))]
            public string _Normal
            {
                get { return Normal; }
                set { Normal = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Pressed state"), Description("Aspect for pressed button state"), Editor(typeof(ButtonStateSelectorEditor), typeof(UITypeEditor))]
            public string _Pressed
            {
                get { return Pressed; }
                set { Pressed = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Inactive state"), Description("Aspect for inactive button state"), Editor(typeof(ButtonStateSelectorEditor), typeof(UITypeEditor))]
            public string _Inactive
            {
                get { return Inactive; }
                set { Inactive = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Enabled")]
            public bool _Enabled
            {
                get { return Enabled; }
                set { Enabled = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Send click event when animation ends")]
            public bool _SendEventWhenAnimationEnds
            {
                get { return SendEventWhenAnimationEnds; }
                set { SendEventWhenAnimationEnds = value; }
            }
            [XmlIgnore(), Description("Event that will be triggered"), Category("Button"), DisplayName("Click Event"), Editor(typeof(EventIDSelectorEditor), typeof(UITypeEditor))]
            public string _ClickEvent
            {
                get { return ClickEvent; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("Invalid name for event - you can only use letters and numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    ClickEvent = value;
                }
            }
            [XmlIgnore(), Description("Button Aliganment"), Category("Button"), DisplayName("Background Style")]
            public SimpleButtonBackgroundStyle _Mode
            {
                get { return BackgroundStyle; }
                set { BackgroundStyle = value; }
            }
            [XmlIgnore(), Description("Sound that will be played when the button is clicked"), Category("Button"), DisplayName("Sound"), Editor(typeof(SoundSelectorEditor), typeof(UITypeEditor))]
            public string _SoundName
            {
                get { return SoundName; }
                set { SoundName = value; }
            }
            #endregion

            #region Initialization parameters (user dependent)
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Enabled")]
            public string _UserValue_Enabled
            {
                get { return UserValue_Enabled; }
                set { UserValue_Enabled = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Click Event")]
            public string _UserValue_Event
            {
                get { return UserValue_Event; }
                set { UserValue_Event = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Sound")]
            public string _UserValue_Sound
            {
                get { return UserValue_Sound; }
                set { UserValue_Sound = value; }
            }
            #endregion

            #region Getters and Setters
            [XmlIgnore(), Description("Specifies if a getter for element position is to be created."), Category("Getters"), DisplayName("Enabled")]
            public YesNo _Getter_Enabled
            {
                get { return Getter_Enabled; }
                set { Getter_Enabled = value; }
            }

            [XmlIgnore(), Description("Sets the background image for this specific element"), Category("Setters"), DisplayName("Background Image")]
            public SimpleButtonSetter _Setter_Background_Image
            {
                get { return Setter_Background_Image; }
                set { Setter_Background_Image = value; }
            }
            [XmlIgnore(), Description("Sets the symbol image for this specific element"), Category("Setters"), DisplayName("Symbol Image")]
            public SimpleButtonSetter _Setter_Symbol_Image
            {
                get { return Setter_Symbol_Image; }
                set { Setter_Symbol_Image = value; }
            }
            [XmlIgnore(), Description("Sets the background image for this specific element"), Category("Setters"), DisplayName("Background Color")]
            public SimpleButtonSetter _Setter_Background_Color
            {
                get { return Setter_Background_Color; }
                set { Setter_Background_Color = value; }
            }
            [XmlIgnore(), Description("Sets the symbol color for this specific element"), Category("Setters"), DisplayName("Symbol Color")]
            public SimpleButtonSetter _Setter_Symbol_Color
            {
                get { return Setter_Symbol_Color; }
                set { Setter_Symbol_Color = value; }
            }
            [XmlIgnore(), Description("Sets the symbol image for this specific element"), Category("Setters"), DisplayName("Enabled")]
            public YesNo _Setter_Enabled
            {
                get { return Setter_Enabled; }
                set { Setter_Enabled = value; }
            }
            [XmlIgnore(), Description("Sets the text color for this specific element"), Category("Setters"), DisplayName("Text Color")]
            public SimpleButtonSetter _Setter_Text_Color
            {
                get { return Setter_Text_Color; }
                set { Setter_Text_Color = value; }
            }
            [XmlIgnore(), Description("Sets the text color for this specific element"), Category("Setters"), DisplayName("Text")]
            public SimpleButtonSetter _Setter_Text
            {
                get { return Setter_Text; }
                set { Setter_Text = value; }
            }
            #endregion

            #region Virtual Functions
            private ButtonFaceContainer FaceNormal = new ButtonFaceContainer();
            private ButtonFaceContainer FacePressed = new ButtonFaceContainer();
            private ButtonFaceContainer FaceInactive = new ButtonFaceContainer();
            private RuntimeContext symbolExecutionContext = new RuntimeContext();
            private RuntimeContext textExecutionContext = new RuntimeContext();

            private RectangleF tempRect = new RectangleF();

            protected override float GetWidthInPixels()
            {
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    if (FaceNormal.Background != null)
                        return FaceNormal.Background.Width;
                    else
                        return 0;
                }
                else
                    return this.Width;
            }
            protected override float GetHeightInPixels()
            {
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    if (FaceNormal.Background != null)
                        return FaceNormal.Background.Height;
                    else
                        return 0;
                }
                else
                    return this.Height;
            }

            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    this.ScaleWidth = scaledWidth;
                    this.ScaleHeight = scaleHeight;
                }
                else
                {
                    this.Width = unscaled_widthInPixels;
                    this.Height = unscaled_heightInPixels;
                }
            }

            protected override string GetDescription()
            {
                return "Event:" + ClickEvent + " , Enable:" + Enabled.ToString() + " , Visible:" + Visible.ToString();
            }
            public override string GetIconKey()
            {
                return "__SimpleButton__";
            }
            public override string GetTypeName()
            {
                return "SimpleButton";
            }
            public ButtonFaceContainer GetCurrentFace()
            {
                ButtonFaceContainer face = FaceNormal;
                if (Enabled == false)
                    face = FaceInactive;
                return face;
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                GetCurrentFace().InitRuntimeContext(ExecutionContext, symbolExecutionContext, textExecutionContext, BackgroundStyle);
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                ButtonFaceContainer.Paint(c, tempRect, ExecutionContext, symbolExecutionContext, textExecutionContext, GetCurrentFace().tp, BackgroundStyle);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                string result = "";
                string res;

                EventIDValue = -1;
                if (ClickEvent.Length == 0)
                    result += "SimpleButton '" + Name + "' : You have to add a click event ID !\n";
                foreach (var e in prj.ObjectEventsIDs)
                    if (e.Name.Equals(ClickEvent))
                    {
                        EventIDValue = e.ID;
                        break;
                    }
                if (EventIDValue == -1)
                    result += "Unknwown event: '" + ClickEvent + "' in simple button : '" + Name + "' !\n";

                if (FaceNormal.CreateFromString(Normal) == false)
                {
                    result += "Fail to create normal face for button\n";
                }
                else
                {
                    res = FaceNormal.Validate(resources, BackgroundStyle);
                    if (res != null)
                        result += "Unable to validate normal face: " + res + "\n";
                }

                if (FacePressed.CreateFromString(Pressed) == false)
                {
                    result += "Fail to create normal face for button\n";
                }
                else
                {
                    res = FacePressed.Validate(resources, BackgroundStyle);
                    if (res != null)
                        result += "Unable to validate pressed face: " + res + "\n";
                }

                if (FaceInactive.CreateFromString(Inactive) == false)
                {
                    result += "Fail to create normal face for button\n";
                }
                else
                {
                    res = FaceInactive.Validate(resources, BackgroundStyle);
                    if (res != null)
                        result += "Unable to validate inactive face: " + res + "\n";
                }
                if (result.Length == 0)
                    return null;

                return result;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".Enabled = " + GetParamOrDefaultValue(this.Enabled.ToString().ToLower(), UserValue_Enabled) + ";";
                s += "\n\t" + Name + ".SendEventWhenAnimationEnds = " + this.SendEventWhenAnimationEnds.ToString().ToLower() + ";";
                s += "\n\t" + Name + ".ClickEvent = " + GetParamOrDefaultValue(this.EventIDValue.ToString(), UserValue_Event) + ";";
                s += "\n\t" + Name + ".UseBackgoundImage = " + (this.BackgroundStyle == SimpleButtonBackgroundStyle.Image).ToString().ToLower() + ";";
                s += "\n\t" + Name + ".IsPressed = false;";
                s += "\n\t" + Name + ".CanProcessTouchEvents = true;";
                if (SoundName.Length > 0)
                    s += "\n\t" + Name + ".ClickSound = " + GetParamOrDefaultValue("Res.Sounds." + this.SoundName, UserValue_Sound) + ";";
                else
                    s += "\n\t" + Name + ".ClickSound = " + GetParamOrDefaultValue("NULL", UserValue_Sound) + ";";

                s += FaceNormal.CreateOnStartCPPCode(animObj, Name + ".Normal");
                s += FacePressed.CreateOnStartCPPCode(animObj, Name + ".Pressed");
                s += FaceInactive.CreateOnStartCPPCode(animObj, Name + ".Inactive");
                return s;
            }
            public override string GetCPPClassName()
            {
                return "SimpleButtonElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_Enabled.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Enabled, "bool"));
                if (UserValue_Event.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Event, "int"));
                if (UserValue_Sound.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Sound, "GApp::Resources::Sound*", "Sound"));
                FaceNormal.PopulateParameters(p);
                FacePressed.PopulateParameters(p);
                FaceInactive.PopulateParameters(p);
            }
            private void CreateGetterForButtonSetter(GACParser.Module m, string name, SimpleButtonSetter style, string paramName, string paramType)
            {
                GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, name, name, "void", "", null);
                if (style == SimpleButtonSetter.OneForAll)
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName, paramName, paramType, "", null));
                else
                {
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName + "Normal", paramName + "Normal", paramType, "", null));
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName + "Pressed", paramName + "Pressed", paramType, "", null));
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName + "Inactive", paramName + "Inactive", paramType, "", null));
                }
                m.Members[name] = mb;
            }
            private string CreateCPPHeaderSetterForButtonSetter(string name, SimpleButtonSetter style, string paramName, string cppParamType)
            {
                if (style == SimpleButtonSetter.OneForAll)
                    return "\n\tvoid " + name + " (" + cppParamType + " " + paramName + "); ";
                else
                    return "\n\tvoid " + name + " (" + cppParamType + " " + paramName + "Normal, " + cppParamType + " " + paramName + "Pressed, " + cppParamType + " " + paramName + "Inactive); ";
            }
            private string CreateCPPCodeSetterForButtonSetter(string className, string name, string objName, string objParam, SimpleButtonSetter style, string paramName, string cppParamType)
            {
                string s = "";
                if (style == SimpleButtonSetter.OneForAll)
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + ") {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal." + objParam + " = " + paramName + ";";
                    s += "\n\t\tel->Pressed." + objParam + " = " + paramName + ";";
                    s += "\n\t\tel->Inactive." + objParam + " = " + paramName + ";";
                }
                else
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + "Normal, " + cppParamType + " " + paramName + "Pressed, " + cppParamType + " " + paramName + "Inactive) {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal." + objParam + " = " + paramName + "Normal;";
                    s += "\n\t\tel->Pressed." + objParam + " = " + paramName + "Pressed;";
                    s += "\n\t\tel->Inactive." + objParam + " = " + paramName + "Inactive;";
                }
                return s + "\n\t}\n";
            }
            private string CreateCPPCodeSetterForButtonTextSetter(string className, string name, string objName, SimpleButtonSetter style, string paramName, string cppParamType)
            {
                string s = "";
                if (style == SimpleButtonSetter.OneForAll)
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + ") {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal.TP.SetText(" + paramName + ");";
                    s += "\n\t\tel->Pressed.TP.SetText(" + paramName + ");";
                    s += "\n\t\tel->Inactive.TP.SetText(" + paramName + ");";
                }
                else
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + "Normal, " + cppParamType + " " + paramName + "Pressed, " + cppParamType + " " + paramName + "Inactive) {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal.TP.SetText(" + paramName + "Normal);";
                    s += "\n\t\tel->Pressed.TP.SetText(" + paramName + "Pressed);";
                    s += "\n\t\tel->Inactive.TP.SetText(" + paramName + "Inactive);";
                }
                return s + "\n\t}\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Getter_Enabled != YesNo.No)
                {
                    string nm = "Is" + Name + "Enabled";
                    m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "bool", "", null);
                }
                if (Setter_Symbol_Image != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "SymbolImage", Setter_Symbol_Image, "symbolImage", "Bitmap");
                if (Setter_Background_Image != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "BackgroundImage", Setter_Background_Image, "backgroundImage", "Bitmap");

                if (Setter_Symbol_Color != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "SymbolColor", Setter_Symbol_Color, "symbolColor", "Color");
                if (Setter_Background_Color != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "BackgroundColor", Setter_Background_Color, "backgroundColor", "Color");
                if (Setter_Text_Color != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "TextColor", Setter_Text_Color, "textColor", "Color");

                if (Setter_Text != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "Text", Setter_Text, "text", "string");

                if (Setter_Enabled != YesNo.No)
                {
                    string nm = "Set" + Name + "Enable";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "enableState", "enableState", "bool", "", null));
                    m.Members[nm] = mb;
                }
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Setter_Symbol_Image != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "SymbolImage", Setter_Symbol_Image, "symbolImage", "GApp::Resources::Bitmap*");
                if (Setter_Background_Image != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "BackgroundImage", Setter_Background_Image, "backgroundImage", "GApp::Resources::Bitmap*");
                if (Setter_Symbol_Color != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "SymbolColor", Setter_Symbol_Color, "symbolColor", "unsigned int");
                if (Setter_Background_Color != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "BackgroundColor", Setter_Background_Color, "backgroundColor", "unsigned int");
                if (Setter_Text_Color != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "TextColor", Setter_Text_Color, "textColor", "unsigned int");
                if (Setter_Text != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "Text", Setter_Text, "text", "const char *");
                if (Getter_Enabled != YesNo.No)
                    s += "\n\tbool Is" + Name + "Enabled ();";
                if (Setter_Enabled != YesNo.No)
                    s += "\n\tvoid Set" + Name + "Enable (bool enableState);";

                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Setter_Symbol_Image != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "SymbolImage", Name, "Symbol", Setter_Symbol_Image, "symbolImage", "GApp::Resources::Bitmap*");
                if (Setter_Background_Image != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "BackgroundImage", Name, "Background", Setter_Background_Image, "backgroundImage", "GApp::Resources::Bitmap*");
                if (Setter_Symbol_Color != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "SymbolColor", Name, "SymbolColorBlending", Setter_Symbol_Color, "symbolColor", "unsigned int");
                if (Setter_Background_Color != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "BackgroundColor", Name, "BackgroundlColorBlending", Setter_Background_Color, "backgroundColor", "unsigned int");
                if (Setter_Text_Color != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "TextColor", Name, "TextColorBlending", Setter_Text_Color, "textColor", "unsigned int");
                if (Setter_Text != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonTextSetter(className, "Set" + Name + "Text", Name, Setter_Text, "text", "const char *");

                if (Getter_Enabled != YesNo.No)
                {
                    s += "\n\tbool " + className + "::Is" + Name + "Enabled () {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + Name + "));";
                    s += "\n\t\treturn el->Enabled;";
                    s += "\n\t}";
                }
                if (Setter_Enabled != YesNo.No)
                {
                    s += "\n\tvoid " + className + "::Set" + Name + "Enable (bool enableState) {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + Name + "));";
                    s += "\n\t\tel->IsPressed = false;";
                    s += "\n\t\tel->Enabled = enableState;";
                    s += "\n\t}";
                }
                return s;
            }
            #endregion
        }


        #endregion

        #region Animation Object
        public class ParameterInformation
        {
            public string Name = "";
            public string CppType = "";
            public string GACType = "";
            public bool UseAsParameter = true;
            public ParameterInformation(string name, string cppType) { Name = name; CppType = cppType; GACType = cppType; }
            public ParameterInformation(string name, string cppType, string gacType) { Name = name; CppType = cppType; GACType = gacType; }
        }
        public class AnimationParameters
        {
            public Dictionary<string, ParameterInformation> Parameters = new Dictionary<string, ParameterInformation>();
            public List<ParameterInformation> ParametersList = new List<ParameterInformation>();

            public void Clear()
            {
                ParametersList.Clear();
                Parameters.Clear();
            }
        };
        public enum AnimationDesignMode
        {
            Screen,
            Control,
            Button
        }

        [XmlType("AnimationObject"), XmlRoot("AnimationObject")]
        public class AnimationObject
        {
            [XmlAttribute()]
            public string Name;
            [XmlAttribute()]
            public Coordinates Coord = Coordinates.Percentage;
            [XmlAttribute()]
            public AnimationDesignMode DesignMode = AnimationDesignMode.Screen;
            [XmlAttribute()]
            public int ControlWidth = 100;
            [XmlAttribute()]
            public int ControlHeight = 50;
            [XmlAttribute()]
            public int BackgroundColor = Color.Black.ToArgb();
            [XmlAttribute()]
            public string ZOrder = "";
            [XmlAttribute()]
            public bool AutoStart = false;

            [XmlArrayItem(typeof(EntireSurfaceElement))]
            [XmlArrayItem(typeof(ImageElement))]
            [XmlArrayItem(typeof(TextElement))]
            [XmlArrayItem(typeof(RectangleElement))]
            [XmlArrayItem(typeof(ExclusionRectangleElement))]
            [XmlArrayItem(typeof(ClipRectangleElement))]
            [XmlArrayItem(typeof(DisableClippingElement))]
            [XmlArrayItem(typeof(SimpleButtonElement))]
            [XmlArrayItem(typeof(GenericElementWithPosition))]
            [XmlArrayItem(typeof(GenericElementWithPositionAndSize))]
            public List<GenericElement> Elements = new List<GenericElement>();

            #region List View Props
            [XmlIgnore(), Browsable(false)]
            public string propName { get { return Name; } }
            [XmlIgnore(), Browsable(false)]
            public string propDescription { get { return "Elements: " + Elements.Count.ToString() + " (" + ZOrder + ")"; } }
            [XmlIgnore(), Browsable(false)]
            public string propDesignMode { get { return DesignMode.ToString(); } }
            [XmlIgnore(), Browsable(false)]
            public bool propAutoStart { get { return AutoStart; } set { AutoStart = value; } }
            [XmlIgnore(), Browsable(false)]
            public string propCoord { get { return Coord.ToString(); } }
            #endregion

            #region Atribute
            [XmlIgnore(), Description("Name"), Category("General"), DisplayName("Name")]
            public string _Name
            {
                get { return Name; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("Name should only contain letters, numbers and '_' character. The first character of the name must be a letter : 'A' - 'Z' !");
                        return;
                    }
                    Name = value;
                }
            }
            [XmlIgnore(), Description("Elements"), Category("General"), DisplayName("Elements")]
            public int _Elements
            {
                get { return Elements.Count; }
            }
            [XmlIgnore(), Description("Elements"), Category("General"), DisplayName("Z-Order")]
            public string _Z_Order
            {
                get { return ZOrder; }
            }
            [XmlIgnore(), Description("Automatically start animation when object is created"), Category("General"), DisplayName("Auto start")]
            public bool _AutoStart
            {
                get { return AutoStart; }
                set { AutoStart = value; }
            }
            [XmlIgnore(), Description("Coordinates type"), Category("Layout"), DisplayName("Coordinates")]
            public Coordinates _Coord
            {
                get { return Coord; }
                set { Coord = value; }
            }
            [XmlIgnore(), Description("Select the purpose of the animation. It could be for Screen animations or Control animations."), Category("Layout"), DisplayName("Design Mode")]
            public AnimationDesignMode _DesignMode
            {
                get { return DesignMode; }
                set { DesignMode = value; }
            }
            [XmlIgnore(), Description("Size of the control that holds the animation"), Category("Layout"), DisplayName("Control Size")]
            public string _Size
            {
                get { return string.Format("{0} x {1}", ControlWidth, ControlHeight); ; }
                set
                {
                    int w = 0, h = 0;
                    if (Project.SizeToValues(value, ref w, ref h)) { ControlWidth = w; ControlHeight = h; }
                }
            }
            [XmlIgnore(), Category("Layout"), DisplayName("Design background color")]
            public System.Drawing.Color _BackgroundColor
            {
                get { return System.Drawing.Color.FromArgb(BackgroundColor); }
                set { BackgroundColor = value.ToArgb(); }
            }
            #endregion

            public GenericTransformation Main = null;
            private int ZOrderID = -1;
            private AnimationParameters animParams = new AnimationParameters();

            [XmlIgnore()]
            public List<GenericElement> ParentsPositionOrder = new List<GenericElement>();

            private void ComputeAllTransformationsListRecursivelly(AnimO.GenericTransformation root, List<GenericTransformation> lst)
            {
                if (root == null)
                    return;
                root.CppName = "transformation_" + lst.Count.ToString();
                if (root.GetType() == typeof(ZOrderTransformation))
                {
                    ((ZOrderTransformation)root).ZOrderID = ZOrderID;
                    ZOrderID++;
                }
                lst.Add(root);
                var lst_trans = root.GetBlockTransformations();
                if (lst_trans != null)
                {
                    foreach (var t in lst_trans)
                        ComputeAllTransformationsListRecursivelly(t, lst);
                }
            }
            List<GenericTransformation> GetAllTransformations()
            {
                List<GenericTransformation> lst = new List<GenericTransformation>();
                ZOrderID = 1;
                ComputeAllTransformationsListRecursivelly(Main, lst);
                return lst;
            }

            private void ProcessTransformation(Dictionary<string, GenericElement> elements, AnimO.GenericTransformation root, Project prj, AppResources resources)
            {
                if (root == null)
                    return;
                root.Validate(prj, Name, resources, elements);
                // doar pentru ZOrdeer
                if (root.GetType() == typeof(ZOrderTransformation))
                    ValidateZOrder(((ZOrderTransformation)root).ZOrder, elements, prj);
                var lst = root.GetBlockTransformations();
                root.SetElement(null);
                var et = root as GenericElementTransformation;
                if (et != null)
                {
                    if (elements.ContainsKey(et.Element) == false)
                    {
                        prj.EC.AddError("Animation '" + Name + "' has a transformation that refers to an unexisting element : '" + et.Element + "' !");
                    }
                    else
                    {
                        et.SetElement(elements[et.Element]);
                    }
                }
                if (lst != null)
                {
                    foreach (var t in lst)
                        ProcessTransformation(elements, t, prj, resources);
                }
            }
            private void CreateParametersForTransformations(AnimO.GenericTransformation root, AnimationParameters p)
            {
                if (root == null)
                    return;
                root.PopulateParameters(p);
                var lst = root.GetBlockTransformations();
                if (lst != null)
                {
                    foreach (var t in lst)
                    {
                        CreateParametersForTransformations(t, p);
                    }
                }
            }
            private bool CreateParameters(AnimationParameters p, Project prj)
            {
                bool ok = true;
                foreach (var e in Elements)
                {
                    e.PopulateParameters(p);
                }
                CreateParametersForTransformations(Main, p);
                // fac si dictionarul
                foreach (ParameterInformation pi in p.ParametersList)
                {
                    if (p.Parameters.ContainsKey(pi.Name))
                    {
                        ParameterInformation current = p.Parameters[pi.Name];
                        if (pi.GACType != current.GACType)
                        {
                            if (prj != null)
                                prj.EC.AddError("Parameter '" + pi.Name + "' from animation :'" + this.Name + "' has been defined with two different types (" + pi.GACType + " and " + current.GACType + ") !");
                            ok = false;
                        }
                        if (pi.CppType != current.CppType)
                        {
                            if (prj != null)
                                prj.EC.AddError("Parameter '" + pi.Name + "' from animation :'" + this.Name + "' has been defined with two different cpp translations (" + pi.CppType + " and " + current.CppType + ") !");
                            ok = false;
                        }
                        pi.UseAsParameter = false;
                    }
                    else
                    {
                        p.Parameters[pi.Name] = pi;
                        pi.UseAsParameter = true;
                    }
                }
                return ok;
            }

            private string ProcessNode(string name, int depth, Dictionary<string, List<string>> nodes, Dictionary<string, int> nodes_order)
            {
                if (depth > 0)
                {
                    if ((nodes_order[name] & 0x10000) != 0)
                        return "Element '" + name + "' is part of a circular reference !";
                    nodes_order[name] = 0x10000 | depth;
                }
                if (nodes.ContainsKey(name))
                {
                    depth++;
                    foreach (string copil in nodes[name])
                    {
                        string s = ProcessNode(copil, depth, nodes, nodes_order);
                        if (s != null)
                            return s;
                    }
                }
                return null;
            }
            public bool ValidateElementOrder(Project prj, Dictionary<string, GenericElement> elements)
            {
                Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();
                Dictionary<string, int> nodes_order = new Dictionary<string, int>();
                ParentsPositionOrder.Clear();
                foreach (GenericElement el in this.Elements)
                {
                    if (el.Parent.Equals("<None>"))
                        el.ParentElement = null;
                    else
                    {
                        if (elements.TryGetValue(el.Parent, out el.ParentElement) == false)
                        {
                            prj.EC.AddError("Element '" + el.Name + "' has an invalid parent: '" + el.Parent + "' in animation: '" + this.Name + "'");
                            return false;
                        }
                    }
                    nodes_order[el.Name] = 0;
                    if (nodes.ContainsKey(el.Parent) == false)
                        nodes[el.Parent] = new List<string>();
                    nodes[el.Parent].Add(el.Name);
                }
                if (nodes.ContainsKey("<None>") == false)
                {
                    prj.EC.AddError("Animation '" + this.Name + "' does not have any element relative to animation (that have a parent <None>)");
                    return false;
                }
                string res;
                res = ProcessNode("<None>", 0, nodes, nodes_order);
                if (res != null)
                {
                    prj.EC.AddError(res + " in animation '" + this.Name + "'");
                    return false;
                }
                // verific daca toate nodurile au fost procesate
                foreach (string nodeName in nodes_order.Keys)
                    if ((nodes_order[nodeName] & 0x10000) == 0)
                        prj.EC.AddError("Element '" + nodeName + "' from Animation '" + this.Name + "' does not have a valid parent !");
                if (prj.EC.HasErrors())
                    return false;
                // all good - fac si lista cu ordinea de procesare
                List<KeyValuePair<string, int>> all_nodes = new List<KeyValuePair<string, int>>();
                foreach (var item in nodes_order)
                    all_nodes.Add(item);
                all_nodes.Sort((firstValue, nextValue) => { return firstValue.Value.CompareTo(nextValue.Value); });
                // pun datele

                foreach (var item in all_nodes)
                {
                    ParentsPositionOrder.Add(elements[item.Key]);
                }
                return true;
            }
            private void ValidateZOrder(string z_order, Dictionary<string, GenericElement> elements, Project prj)
            {
                List<string> l = Project.StringListToList(z_order);
                foreach (string k in elements.Keys)
                    elements[k]._FoundInZOrder_ = false;
                foreach (string name in l)
                {
                    if (elements.ContainsKey(name) == false)
                    {
                        prj.EC.AddError("Animation '" + Name + "' has an invalid/unknwon element in the Z-Order list: '" + name + "' !");
                    }
                    else
                    {
                        elements[name]._FoundInZOrder_ = true;
                    }
                }
                // verific daca toate sunt
                foreach (string k in elements.Keys)
                    if (elements[k]._FoundInZOrder_ == false)
                    {
                        prj.EC.AddError("Animation '" + Name + "' does not have element '" + k + "' added in the Z-Order list !");
                    }
            }

            public bool ValidateAnimation(Project prj, AppResources resources)
            {
                if ((DesignMode == AnimationDesignMode.Control) || (DesignMode == AnimationDesignMode.Button))
                {
                    if (ControlWidth < 10)
                        prj.EC.AddError("Animation '" + Name + "' has a Control/Button Width too small: " + ControlWidth.ToString() + " !");
                    if (ControlHeight < 10)
                        prj.EC.AddError("Animation '" + Name + "' has a Control/Button Height too small: " + ControlHeight.ToString() + " !");
                }
                Dictionary<string, GenericElement> elements = new Dictionary<string, GenericElement>();
                foreach (var e in Elements)
                {
                    if (elements.ContainsKey(e.Name))
                        prj.EC.AddError("Animation '" + Name + "' has a duplicate element: '" + e.Name + "' !");
                    else
                        elements[e.Name] = e;
                    string s = e.Validate(prj, resources);
                    if (s != null)
                        prj.EC.AddError("Animation '" + Name + "' has an incorecet set up element: '" + e.Name + "' => Error: " + s);
                }
                // validez ZOrder
                ValidateZOrder(ZOrder, elements, prj);
                // validez relatia element - parinte
                ValidateElementOrder(prj, elements);
                // setez si eelementele in transformari
                ProcessTransformation(elements, Main, prj, resources);
                // validez si parametri
                animParams.Clear();
                if (CreateParameters(animParams, prj) == false)
                    prj.EC.AddError("Unable to valide all parameters !");
                // ies
                return !prj.EC.HasErrors();
            }

            public AnimationObject MakeCopy()
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AnimationObject));
                    StringWriter textWriter = new StringWriter();
                    serializer.Serialize(textWriter, this);
                    StringReader textReader = new StringReader(textWriter.ToString());
                    return (AnimationObject)serializer.Deserialize(textReader);
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            private void AddCreateParameters(GACParser.Member fnc, GACParser.Module m)
            {
                foreach (var p in this.animParams.ParametersList)
                {
                    if (p.UseAsParameter)
                        fnc.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, p.Name, p.Name, p.GACType, "", null));
                }
            }
            public void BuildGACParameters(GACParser.Module m, Dictionary<string, List<string>> enums)
            {
                m.Members["Start"] = new GACParser.Member(m, GACParser.MemberType.Function, "Start", "Start", "bool", "", null);
                m.Members["Stop"] = new GACParser.Member(m, GACParser.MemberType.Function, "Stop", "Stop", "bool", "", null);
                m.Members["IsRunning"] = new GACParser.Member(m, GACParser.MemberType.Function, "IsRunning", "IsRunning", "bool", "", null);
                m.Members["Pause"] = new GACParser.Member(m, GACParser.MemberType.Function, "Pause", "Pause", "bool", "", null);
                m.Members["Resume"] = new GACParser.Member(m, GACParser.MemberType.Function, "Resume", "Resume", "bool", "", null);
                m.Members["IsPaused"] = new GACParser.Member(m, GACParser.MemberType.Function, "IsPaused", "IsPaused", "bool", "", null);
                m.Members["Paint"] = new GACParser.Member(m, GACParser.MemberType.Function, "Paint", "Paint", "void", "", null);
                m.Members["ExitPopupLoop"] = new GACParser.Member(m, GACParser.MemberType.Function, "ExitPopupLoop", "ExitPopupLoop", "void", "", null);
                m.Members["ResetMovementOffsets"] = new GACParser.Member(m, GACParser.MemberType.Function, "ResetMovementOffsets", "ResetMovementOffsets", "void", "", null);
                GACParser.Member MoveWithOffset = new GACParser.Member(m, GACParser.MemberType.Function, "MoveWithOffset", "MoveWithOffset", "void", "", null);
                MoveWithOffset.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "offsetX", "offsetX", "float", "", null));
                MoveWithOffset.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "offsetY", "offsetY", "float", "", null));
                MoveWithOffset.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "coord", "coord", "Coordinates", "", null));
                m.Members["MoveWithOffset"] = MoveWithOffset;
                GACParser.Member OnTouchEvent = new GACParser.Member(m, GACParser.MemberType.Function, "OnTouchEvent", "OnTouchEvent", "bool", "", null);
                OnTouchEvent.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "touchEvent", "touchEvent", "TouchEvent", "", null));
                m.Members["OnTouchEvent"] = OnTouchEvent;
                // update parametri
                animParams.Clear();
                CreateParameters(animParams, null);
                // functia de create
                GACParser.Member mCreate = new GACParser.Member(m, GACParser.MemberType.Function, "Create", "Create", "void", "", null);
                AddCreateParameters(mCreate, m);
                m.Members["Create"] = mCreate;
                // constructorii
                GACParser.Member mConstrScene = new GACParser.Member(m, GACParser.MemberType.Constructor, "", "", "", "", null);
                mConstrScene.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "scene", "scene", "Scene", "", null));
                AddCreateParameters(mConstrScene, m);
                m.Members[""] = mConstrScene;

                GACParser.Member mConstrApp = new GACParser.Member(m, GACParser.MemberType.Constructor, "", "", "", "", null);
                mConstrApp.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "app", "app", "Application", "", null));
                AddCreateParameters(mConstrApp, m);
                m.Members[""].Overrides = new List<GACParser.Member>();
                m.Members[""].Overrides.Add(mConstrApp);

                GACParser.Member mConstrObj = new GACParser.Member(m, GACParser.MemberType.Constructor, "", "", "", "", null);
                mConstrObj.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "obj", "__obj__", "Object", "", null));
                AddCreateParameters(mConstrObj, m);
                m.Members[""].Overrides.Add(mConstrObj);

                var l = GetAllTransformations();
                foreach (var t in l)
                {
                    t.AddAnimationFunction(m);
                    t.CreateGACEnums(enums);
                }
                foreach (var e in Elements)
                    e.AddAnimationFunction(m);
            }

            public string GetCPPClassName()
            {
                return "GAC_" + Name;
            }
            private string CreateParameterListPrototype()
            {
                string lst = "";
                foreach (var p in this.animParams.ParametersList)
                {
                    if (p.UseAsParameter)
                        lst += p.CppType + " " + p.Name + ", ";
                }
                if (lst.Length > 0)
                {
                    lst = lst.Substring(0, lst.Length - 2);
                }
                return lst;
            }
            private string CreateParameterListForCall()
            {
                string lst = "";
                foreach (var p in this.animParams.ParametersList)
                {
                    if (p.UseAsParameter)
                        lst += p.Name + ", ";
                }
                if (lst.Length > 0)
                {
                    lst = lst.Substring(0, lst.Length - 2);
                }
                return lst;
            }
            public string CreateCPPWrapperHeaderClass()
            {
                string s = "\n// ====================== Animation wrapper class for " + Name + " ===========================================\n";
                s += "\nclass " + GetCPPClassName() + " : public GApp::Animations::AnimationObject {";
                // elementele
                s += "\n\tGApp::Animations::Elements::Element *ZOrder[" + Elements.Count.ToString() + "];";
                s += "\n\tGApp::Animations::Elements::Element *RelativeOrder[" + Elements.Count.ToString() + "];";

                foreach (GenericElement e in Elements)
                {
                    s += "\n\tGApp::Animations::Elements::" + e.GetCPPClassName() + " " + e.Name + ";";
                }
                // transformarile
                var l = GetAllTransformations();
                foreach (var t in l)
                {
                    s += "\n\tGApp::Animations::Transformations::" + t.GetCPPClassName() + " " + t.CppName + ";";
                }
                // parametri             
                Dictionary<string, bool> p_d = new Dictionary<string, bool>();
                foreach (var p in this.animParams.ParametersList)
                {
                    //if (p_d.ContainsKey(p.Name) == false)
                    //{
                    if (p.UseAsParameter)
                        s += "\n\t" + p.CppType + " param_" + p.Name + ";";
                    //    p_d[p.Name] = true;
                    //}
                }
                string param_list = CreateParameterListPrototype();

                s += "\n\n\tvoid InitializeTransformations();";
                s += "\npublic:";
                if (param_list.Length > 0)
                {
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Scene *scene, " + param_list + ");";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Application *application, " + param_list + ");";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::FrameworkObject *frameworkObject, " + param_list + ");";
                    s += "\n\t void Create (" + param_list + ");";
                }
                else
                {
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Scene *scene);";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Application *a);";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::FrameworkObject *frameworkObject);";
                    s += "\n\t void Create ();";
                }
                foreach (var t in l)
                {
                    s += t.GetAnimationFunctionCPPHeaderDefinition();
                }
                foreach (GenericElement e in Elements)
                {
                    s += e.GetAnimationFunctionCPPHeaderDefinition();
                }
                s += "\n\tvirtual void Paint();";
                s += "\n\tvirtual void ButtonPaint(bool visible);";
                s += "\n\tvirtual bool ControlPaint(bool loop) ;";
                s += "\n\tvirtual bool OnStart();";
                s += "\n\tvirtual void SetZOrder(int index);";
                s += "\n\tvirtual bool OnTouchEvent(GApp::Controls::TouchEvent *te);";
                return s + "\n};\n";
            }

            public string CreateCPPWrapperCodeClass()
            {
                string s = "\n// ====================== Animation wrapper class for " + Name + " ===========================================\n";
                // constructorii
                string param_list = CreateParameterListPrototype();
                string param_list_for_call = CreateParameterListForCall();
                bool hasClipping = false;
                // constructor componeent
                string constr_code = "\n\tCoreContext = __init_object__->CoreContext;";
                if ((DesignMode == AnimationDesignMode.Control) || (DesignMode == AnimationDesignMode.Button))
                {
                    constr_code += "\n\tWidth = " + ControlWidth.ToString() + " * (Core.ResolutionAspectRatio);";
                    constr_code += "\n\tHeight = " + ControlHeight.ToString() + " * (Core.ResolutionAspectRatio);";
                }
                else
                {
                    constr_code += "\n\tWidth = (float)(Core.Width);";
                    constr_code += "\n\tHeight = (float)(Core.Height);";
                }
                //if (this.Coord == Coordinates.Pixels)
                //    constr_code += "\n\tFlags = GAC_COORDINATES_PIXELS << 16;";
                //else
                // COORDONATELE SUNT TOT TIMPUL IN PROCENTE
                constr_code += "\n\tFlags = GAC_COORDINATES_PERCENTAGE << 16;";
                for (int tr = 0; tr < this.ParentsPositionOrder.Count; tr++)
                    constr_code += "\n\tthis->RelativeOrder[" + tr.ToString() + "] = &" + this.ParentsPositionOrder[tr].Name + ";";
                constr_code += "\n\tInitializeTransformations();";
                constr_code += "\n\tCreate(" + param_list_for_call + ");";
                constr_code += "\n}";

                // constructor pentru SCENA =======================================================================================================================================
                if (param_list.Length > 0)
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Scene *__init_object__, " + param_list + ") {";
                else
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Scene *__init_object__) {";
                s += constr_code;

                // constructor pentru Applicaction =======================================================================================================================================
                if (param_list.Length > 0)
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Application *__init_object__, " + param_list + ") {";
                else
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Application *__init_object__) {";
                s += constr_code;

                // constructor pentru FrameworkObject =======================================================================================================================================
                if (param_list.Length > 0)
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::FrameworkObject *__init_object__, " + param_list + ") {";
                else
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::FrameworkObject *__init_object__) {";
                s += constr_code;

                // functia de create
                s += "\nvoid " + GetCPPClassName() + "::Create(" + param_list + ") {";
                s += "\n\tthis->OffsetX = this->OffsetY = 0.0f;";
                if ((AutoStart) && (DesignMode == AnimationDesignMode.Screen))
                    s += "\n\tStop();";
                foreach (var p in this.animParams.ParametersList)
                    if (p.UseAsParameter)
                        s += "\n\tparam_" + p.Name + " = " + p.Name + ";";
                if ((AutoStart) && (DesignMode == AnimationDesignMode.Screen))
                    s += "\n\tStart();";
                s += "\n}";

                s += "\nvoid " + GetCPPClassName() + "::InitializeTransformations() {";
                var transformations_list = GetAllTransformations();
                foreach (var t in transformations_list)
                {
                    s += t.CreateInitializationCPPCode();
                }
                s += "\n}";

                // codul pentru OnStart
                s += "\nbool " + GetCPPClassName() + "::OnStart() {";
                foreach (var elem in this.Elements)
                {
                    s += elem.CreateOnStartCPPCode(this);
                    if (elem.GetType() == typeof(AnimO.ClipRectangleElement))
                        hasClipping = true;
                }
                foreach (var t in transformations_list)
                {
                    s += t.CreateOnStartCPPCode();
                }
                // ZOrder
                s += "\n\tSetZOrder(0);";
                // pornesc prima transformare
                if (Main != null)
                    s += "\n\ttransformation_0.Init(this);";
                s += "\n\treturn true;";
                s += "\n}";

                string paint_code = "";
                paint_code += "\n\t\tGApp::Animations::Elements::Element **z = &RelativeOrder[0];";
                for (int tr = 0; tr < (Elements.Count) - 1; tr++)
                {
                    paint_code += "\n\t\t(*z)->UpdateScreenRect(this);z++;";
                }
                paint_code += "\n\t\t(*z)->UpdateScreenRect(this);";
                paint_code += "\n\t\tz = &ZOrder[0];";
                for (int tr = 0; tr < (Elements.Count) - 1; tr++)
                {
                    paint_code += "\n\t\t(*z)->Paint(this);z++;";
                }
                paint_code += "\n\t\t(*z)->Paint(this);";
                // codul pentru Paint
                switch (this.DesignMode)
                {
                    case AnimationDesignMode.Screen:
                        s += "\nbool " + GetCPPClassName() + "::ControlPaint(bool loop)  { return true; };";
                        s += "\nvoid " + GetCPPClassName() + "::ButtonPaint(bool visible) { };";
                        s += "\nvoid " + GetCPPClassName() + "::Paint() {";
                        s += "\n\tif ((Flags & 3)!=1) return;";
                        if (Main != null)
                            s += "\n\tif (transformation_0.Update(this)==false) { Stop(); return; }";
                        // paint
                        s += paint_code;
                        // sterg clipping-ul (daca am)
                        if (hasClipping)
                            s += "\n\tG.ClearClip();";
                        s += "\n}";
                        break;
                    case AnimationDesignMode.Button:
                        s += "\nvoid " + GetCPPClassName() + "::Paint() { };";
                        s += "\nbool " + GetCPPClassName() + "::ControlPaint(bool loop)  { return true; };";
                        s += "\nvoid " + GetCPPClassName() + "::ButtonPaint(bool visible) {";
                        s += "\n\tif ((Flags & 1)!=1) Start();";
                        if (Main != null)
                            s += "\n\tif (transformation_0.Update(this)==false) { Stop(); Start(); }";
                        // cazul 1 (fara ordine)
                        s += "\n\tif (visible) {";
                        s += paint_code;
                        s += "\n\t};";
                        s += "\n}";
                        break;
                    case AnimationDesignMode.Control:
                        s += "\nvoid " + GetCPPClassName() + "::Paint() { };";
                        s += "\nvoid " + GetCPPClassName() + "::ButtonPaint(bool visible) { };";
                        s += "\nbool " + GetCPPClassName() + "::ControlPaint(bool loop) {";
                        s += "\n\tif ((Flags & 3)!=1) return true;";
                        if (Main != null)
                        {
                            s += "\n\tbool ret_code = true;";
                            s += "\n\tif (transformation_0.Update(this)==false) { if (loop) Start(); else { Stop();ret_code=false;} } ";
                            s += paint_code;
                            s += "\n\treturn ret_code;";
                        }
                        else
                        {
                            s += paint_code;
                            s += "\n\treturn true;";
                        }
                        s += "\n}";
                        break;
                }


                // codul pentru alte funcii
                string class_name = GetCPPClassName();
                foreach (var t in transformations_list)
                {
                    s += t.GetAnimationFunctionCPPImplementation(class_name);
                }
                foreach (var e in Elements)
                {
                    s += e.GetAnimationFunctionCPPImplementation(class_name);
                }
                // ZOrder
                s += "\nvoid " + GetCPPClassName() + "::SetZOrder(int index) {";
                s += "\n\tswitch (index) {";
                s += CreateZOrderSetUpCppCode(ZOrder, 0); // default
                foreach (var t in transformations_list)
                {
                    if (t.GetType() == typeof(ZOrderTransformation))
                        s += CreateZOrderSetUpCppCode(((ZOrderTransformation)t).ZOrder, ((ZOrderTransformation)t).ZOrderID);
                }
                s += "\n\t}";
                s += "\n}";

                // OnTouchEvent
                s += "\nbool " + GetCPPClassName() + "::OnTouchEvent(GApp::Controls::TouchEvent *te) {";
                s += "\n\treturn ProcessTouchEvents(te,ZOrder," + Elements.Count.ToString() + ");";
                s += "\n}";
                return s + "\n";
            }

            private string CreateZOrderSetUpCppCode(string z_order, int index)
            {
                List<string> l = Project.StringListToList(z_order);
                l.Reverse();
                string s = "\n\t\tcase " + index.ToString() + ":";
                for (int tr = 0; tr < l.Count; tr++)
                {
                    s += "\n\t\t\tZOrder[" + tr.ToString() + "] = &" + l[tr] + ";";
                }
                s += "\n\t\t\tbreak;";
                return s;
            }

            public void AddElement(AnimO.GenericElement e)
            {
                Elements.Add(e);
                if (ZOrder.Length == 0)
                    ZOrder = e.Name;
                else
                    ZOrder = e.Name + "," + ZOrder;
            }
            public void RemoveElement(AnimO.GenericElement e)
            {
                Elements.Remove(e);
                List<string> l = Project.StringListToList(ZOrder);
                for (int tr = 0; tr < l.Count; tr++)
                {
                    if (l[tr] == e.Name)
                    {
                        l.RemoveAt(tr);
                        break;
                    }
                }
                ZOrder = Project.ListToStringList(l);
            }


        }


        #endregion 
    }


}
