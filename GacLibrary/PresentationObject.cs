using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Reflection;
using System.Drawing;
using System.Drawing.Design;

namespace GAppCreator
{
    #region Helper classes
    public enum ImageResizeMode
    {
        Dock,
        Fill,
        Fit,
        Shrink,
    };

    public enum Alignament: int
    {
        TopLeft = 0,
        TopCenter = 1,
        TopRight = 2,
        RightCenter = 3,
        BottomRight = 4,
        BottomCenter = 5,
        BottomLeft = 6,
        LeftCenter = 7,
        Center = 8
    };

    public enum AlignComponent
    {
        Left,
        Top,
        Right,
        Bottom,
        CenterX,
        CenterY,
        Width,
        Height,
    };
    public enum BinaryAnimObjectType : byte
    {
        LineColor = 0,
        LineColorWidth = 1,
        LineFull = 2,

        Clipping = 5,
        ClearClipping = 6,

        RectFull = 10,
        RectFill = 11,
        RectStroke = 12,
        RectFillAndStroke = 13,

        ImageSimple = 20,        
        ImageResized = 21,
        ImageSimpleWithShader = 22,
        ImageResizedWithShader = 23,
        ImageDocked = 24,
        ImageDockedWithShader = 25,

        TextFromResources = 30,
        TextFromString = 31,
        TextFromResourcesWidthShader = 32,
        TextFromStringWidthShader = 33,
        TextFromResourcesWidthColorBlending = 34,
        TextFromStringWidthColorBlending = 35,
        TextFromResourcesWidthAlphaBlending = 36,
        TextFromStringWidthAlphaBlending = 37,

        TextFromUserValue = 38,
        TextFromUserValueWidthShader = 39,
        TextFromUserValueWithColorBlending = 40,
        TextFromUserValueWithAlphaBlending = 41,

        ImageSimpleWithColorBlending = 60,
        ImageSimpleWithAlphaBlending = 61,
        ImageResizedWithColorBlending = 62,
        ImageResizedWithAlphaBlending = 63,
        ImageDockedWithColorBlending = 64,
        ImageDockedWithAlphaBlending = 65,



        ImageFull = 255,



    };
    public enum BlendingMode: byte
    {
        None = 0,
        Shader = 1,
        ColorBlending = 2,
        AlphaBlending = 3,
    };
    public enum TextModeType
    {
        None,
        UseTextField,
        FromResources,
        UserValue,
    };

    public class ByteBuffer
    {
        private List<byte> Buffer = new List<byte>();
        private ErrorsContainer ec;
        public void AddByte(byte value)
        {
            Buffer.Add(value);
        }
        public void AddByte(int value)
        {
            if ((value<0) || (value>255))
            {
                ec.AddError(String.Format("Value to big or too small to be added as a Byte: {0}", value));
                return;
            }
            Buffer.Add((byte)value);
        }
        public void AddWord(UInt16 value)
        {
            Buffer.Add((byte)(value % 256));
            Buffer.Add((byte)(value / 256));            
        }
        public void AddShort(int value)
        {
            Int16 i16 = (Int16)value;
            if (((int)i16)!=value)            
            {
                ec.AddError(String.Format("Invalid value for Int16 conversion: {0}",value));
                return;
            }
            AddWord((UInt16)i16);
        }
        public void AddWord(int value)
        {
            if ((value<0) || (value>0xFFFF))
            {
                ec.AddError(String.Format("Invalid value for UInt16 conversion: {0}", value));
                return;
            }
            AddWord((UInt16)value);
        }
        public void AddInt(int value)
        {
            uint y = (uint)value;
            AddByte((byte)((y & 0x000000FF)));
            AddByte((byte)((y & 0x0000FF00) >> 8));
            AddByte((byte)((y & 0x00FF0000) >> 16));
            AddByte((byte)((y & 0xFF000000) >> 24));            
        }
        public void AddUInt(uint value)
        {
            uint y = value;
            AddByte((byte)((y & 0x000000FF)));
            AddByte((byte)((y & 0x0000FF00) >> 8));
            AddByte((byte)((y & 0x00FF0000) >> 16));
            AddByte((byte)((y & 0xFF000000) >> 24));
        }
        public void AddType(BinaryAnimObjectType btype)
        {
            AddByte((byte)btype);
        }
        public void Add24BitUint(int value)
        {
            if (value < 0)
            {
                ec.AddError(String.Format("Value can not be negative for Add24BitUint function: {0}", value));
                return;
            }
            if (value >= 256*256*256)
            {
                ec.AddError(String.Format("Value is to big for Add24BitUint function: {0} - max allowed is 16,777,216", value));
                return;
            }
            AddByte((byte)(value & 0xFF));
            AddByte((byte)((value & 0xFF00)>>8));
            AddByte((byte)((value & 0xFF0000) >> 16));
            //AddByte((byte)(value / (256 * 256)));
            //value = value % (256 * 256);
            //AddByte((byte)(value / 256));
            //AddByte((byte)(value % 256));
        }
        public void AddCompactValue(int value)
        {
            int v1,v2,v3;
            if (value<0)
            {
                ec.AddError(String.Format("Value can not be negative for AddCompactValue function: {0}",value));
                return;
            }
            if (value < 128)
                AddByte((byte)value);
            else if (value < 16383)
            {
                v1 = (value / 128) | 0x80;
                v2 = (value % 128);
                AddByte((byte)v1);
                AddByte((byte)v2);
            } else if (value<(128*128*128-1))
            {
                v1 = (value / (128*128)) | 0x80;
                value = value % (128*128);    
                v2 = (value / 128) | 0x80;
                v3 = (value % 128);   
                AddByte((byte)v1);
                AddByte((byte)v2);         
                AddByte((byte)v3);
            } else {
                ec.AddError(String.Format("Value can not be biger than 2,097,151 for AddCompactValue function: {0}", value));
                return;
            }
        }
        public void AddString(string s,bool addSize)
        {
            if (s.Length > 127)
            {
                ec.AddError(String.Format("String to long (127 characters maximum): {0}",s));
                return;
            }
            if (addSize)
                Buffer.Add((byte)(s.Length));           
            foreach (char ch in s)
            {
                if ((ch < 0) || (ch > 127))
                {
                    ec.AddError(String.Format("Invalid character {0} in string {1}", ch,s));
                    return;
                }
                Buffer.Add((byte)ch);
            }
        }
        public byte[] GetBuffer()
        {
            return Buffer.ToArray();
        }
        public void Clear()
        {
            Buffer.Clear();
        }
        public int Count()
        {
            return Buffer.Count;
        }
        public string ComputeMD5()
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(Buffer.ToArray());
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public ByteBuffer(ErrorsContainer eContainer)
        {
            ec = eContainer;
        }
        public ErrorsContainer GetErrorsCountainer()
        {
            return ec;
        }
    };
    public class ResourceAnimationItem
    {
        public string Name = null;
        public int Index = 0;
        public GenericResource Resource = null;
        public ResourceAnimationItem() { }
        public ResourceAnimationItem(int index) { Index = index;  }
    }
    [XmlType("ResourceKeyName"), XmlRoot("ResourceKeyName")]
    public class ResourceKeyName
    {
        [XmlAttribute()]
        public string ResourceName = "", KeyName = "";
    }
    public class ResourcesIndexes
    {
        public Dictionary<string, ResourceAnimationItem> ImageIndex = new Dictionary<string, ResourceAnimationItem>();
        public Dictionary<string, ResourceAnimationItem> StringIndex = new Dictionary<string, ResourceAnimationItem>();
        public Dictionary<string, int> StringValueIndex = new Dictionary<string, int>();
        public Dictionary<string, ResourceAnimationItem> SoundsIndex = new Dictionary<string, ResourceAnimationItem>();
        public Dictionary<string, ResourceAnimationItem> FontsIndex = new Dictionary<string, ResourceAnimationItem>();
        public Dictionary<string, ResourceAnimationItem> ShaderIndex = new Dictionary<string, ResourceAnimationItem>();
    }
    #endregion

    [XmlInclude(typeof(RectanglePresentationObject))]
    [XmlInclude(typeof(ImagePresentationObject))]
    [XmlInclude(typeof(LinePresentationObject))]
    [XmlInclude(typeof(TextPresentationObject))]
    [XmlInclude(typeof(ClipPresentationObject))]
    public class PresentationObject
    {
        [XmlIgnore()]
        public int SelectionIndex = -1;

        [XmlAttribute()]
        public string ID = "";

        #region Atribute
        [XmlIgnore(), Category("ID"), DisplayName("ID")]
        public string _ID
        {
            get { return ID; }
            set { ID = value; }
        }
        #endregion

        #region Metode Virtuale
        public virtual void Paint(AnimO.Canvas canvas, GRect drawingRect, AppResources Resources) {}
        public virtual string GetName() { return "PresentationObject"; }
        public virtual void ComputeRect(GRect rect) {}
        public virtual void SetRectangle(GRect rect) { }
        public virtual int GetValue(AlignComponent ac, GRect tempRect) { return 0; }
        public virtual void SetValue(AlignComponent ac, int newValue, GRect tempRect) { }
        public virtual void AddBinaryBuffer(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources) { }
        public virtual string GetIconName() { return ""; }
        #endregion

        #region Metode Normale

        public void SetRectangle(PresentationSelection sel)
        {
            SetRectangle(sel.rect);
        }
        public string ToXMLString()
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stringwriter, this);
                return stringwriter.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to convert to XML: " + GetName() + "\n" + e.ToString());
                return "";
            }
        }
        public static PresentationObject FromXMLString(string xmlText, Type objectType)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(objectType);
                PresentationObject ao = (PresentationObject)serializer.Deserialize(stringReader);
                if (ao == null)
                {
                    MessageBox.Show("Unable to create AnimationObject from XML:\n" + xmlText);
                }
                return ao;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to create AnimationObject from XML:\n" + e.ToString());
                return null;
            }
        }
        protected int AlignamentToInt(Alignament a)
        {
            switch (a)
            {
                case Alignament.TopLeft: return 0;
                case Alignament.TopCenter: return 1;
                case Alignament.TopRight: return 2;
                case Alignament.RightCenter: return 3;
                case Alignament.BottomRight: return 4;
                case Alignament.BottomCenter: return 5;
                case Alignament.BottomLeft: return 6;
                case Alignament.LeftCenter: return 7;
                case Alignament.Center: return 8;
            }
            return 9;
        }
        protected void EnableAttr(string name, bool value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[name];
            ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
            FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldToChange.SetValue(attribute, !value);
        }
        #endregion
    }
    public class BasicPaintObject : PresentationObject
    {
        [XmlAttribute()]
        public int X, Y, Width, Height;
        [XmlAttribute()]
        public Alignament Align = Alignament.TopLeft;
        [XmlAttribute()]
        public float ScaleWidth = 1.0f, ScaleHeight = 1.0f;
        [XmlAttribute()]
        public float Angle;
        [XmlAttribute()]
        public Alignament RotateCenterAlign = Alignament.Center;
        [XmlAttribute()]
        public int RotateCenterX, RotateCenterY;
        [XmlAttribute()]
        public bool UseRotateAlign = true;

        #region Atribute
        [XmlIgnore(), Category("Layout"), DisplayName("X")]
        public int _X
        {
            get { return X; }
            set { X = value; }
        }
        [XmlIgnore(), Category("Layout"), DisplayName("Y")]
        public int _Y
        {
            get { return Y; }
            set { Y = value; }
        }
        [XmlIgnore(), Category("Layout"), DisplayName("Width")]
        public int _Width
        {
            get { return Width; }
            set { Width = value; }
        }
        [XmlIgnore(), Category("Layout"), DisplayName("Height")]
        public int _Height
        {
            get { return Height; }
            set { Height = value; }
        }
        [XmlIgnore(), Category("Layout"), DisplayName("Alignament")]
        public Alignament _Align
        {
            get { return Align; }
            set { Align = value; }
        }
        [XmlIgnore(), Category("Scale and Skew"), DisplayName("Scale Width")]
        public float _ScaleWidth
        {
            get { return ScaleWidth; }
            set { ScaleWidth = value; }
        }
        [XmlIgnore(), Category("Scale and Skew"), DisplayName("Scale Height")]
        public float _ScaleHeight
        {
            get { return ScaleHeight; }
            set { ScaleHeight = value; }
        }

        [XmlIgnore(), Category("Rotation"), DisplayName("Angle")]
        public float _Angle
        {
            get { return Angle; }
            set { Angle = value; }
        }
        [XmlIgnore(), Category("Rotation"), DisplayName("Center Alignament")]
        public Alignament _CenterAngle
        {
            get { return RotateCenterAlign; }
            set { RotateCenterAlign = value; }
        }
        [XmlIgnore(), Category("Rotation"), DisplayName("Center X")]
        public int _CenteX
        {
            get { return RotateCenterX; }
            set { RotateCenterX = value; }
        }
        [XmlIgnore(), Category("Rotation"), DisplayName("Center Y")]
        public int _CenteY
        {
            get { return RotateCenterY; }
            set { RotateCenterY = value; }
        }
        [XmlIgnore(), Category("Rotation"), DisplayName("Use Alignament")]
        public bool _UseRotateAlign
        {
            get { return UseRotateAlign; }
            set { UseRotateAlign = value; }
        }

        #endregion

        #region Virtual functions
        public override void ComputeRect(GRect rect)
        {
            rect.Set(X, Y, Align, (int)(Width * ScaleWidth), (int)(Height * ScaleHeight));
        }
        public override void SetRectangle(GRect rect)
        {
            X = rect.GetAnchorX(Align);
            Y = rect.GetAnchorY(Align);
            Width = (int)(rect.GetWidth() / ScaleWidth);
            Height = (int)(rect.GetHeight() / ScaleHeight);
        }
        public override int  GetValue(AlignComponent ac, GRect tempRect)
        {
            ComputeRect(tempRect);
            switch (ac)
            {
                case AlignComponent.Left:
                    return tempRect.Left;
                case AlignComponent.Top:
                    return tempRect.Top;
                case AlignComponent.Right:
                    return tempRect.Right;
                case AlignComponent.Bottom:
                    return tempRect.Bottom;
                case AlignComponent.Width:
                    return tempRect.GetWidth();
                case AlignComponent.Height:
                    return tempRect.GetHeight();
                case AlignComponent.CenterX:
                    return tempRect.CenterX();
                case AlignComponent.CenterY:
                    return tempRect.CenterY();
            };
            return 0;
        }
        public override void SetValue(AlignComponent ac, int newValue, GRect tempRect)
        {
            ComputeRect(tempRect);
            switch (ac)
            {
                case AlignComponent.Left:
                    tempRect.Add(newValue - tempRect.Left, 0);
                    break;
                case AlignComponent.Top:
                    tempRect.Add(0, newValue - tempRect.Top);
                    break;
                case AlignComponent.Right:
                    tempRect.Add(newValue - tempRect.Right, 0);
                    break;
                case AlignComponent.Bottom:
                    tempRect.Add(0, newValue - tempRect.Bottom);
                    break;
                case AlignComponent.Width:
                    tempRect.SetWidthFromLeft(newValue);
                    break;
                case AlignComponent.Height:
                    tempRect.SetHeightFromTop(newValue);
                    break;
                case AlignComponent.CenterX:
                    tempRect.Add(newValue - tempRect.Left - tempRect.GetWidth() / 2, 0);
                    break;
                case AlignComponent.CenterY:
                    tempRect.Add(0, newValue - tempRect.Top - tempRect.GetHeight() / 2);
                    break;
            };
            // updatez pozitiile
            X = tempRect.GetAnchorX(Align);
            Y = tempRect.GetAnchorY(Align);
            if (ScaleWidth > 0)
                Width = (int)(tempRect.GetWidth() / ScaleWidth);
            if (ScaleHeight > 0)
                Height = (int)(tempRect.GetHeight() / ScaleHeight);
        }
        #endregion

    }
    public class BasicBlenderObject: BasicPaintObject
    {
        [XmlAttribute()]
        public string ShaderName = "";
        [XmlAttribute()]
        public int ColorBlending = -1;
        [XmlAttribute()]
        public float AlphaBlending = 1.0f;
        [XmlAttribute()]
        public BlendingMode BlendMode = BlendingMode.None;

        #region Atribute
        [XmlIgnore(), Category("Blending"), DisplayName("Shader"), Description("Selects the shader to be used for this object. Empty name means the default shader."), Editor(typeof(ShaderSelectorEditor), typeof(UITypeEditor))]
        public string _ShaderName
        {
            get { return ShaderName; }
            set { ShaderName = value; }
        }
        [XmlIgnore(), Category("Blending"), DisplayName("ColorBlending")]
        public System.Drawing.Color _ColorBlending
        {
            get { return System.Drawing.Color.FromArgb(ColorBlending); }
            set { ColorBlending = value.ToArgb(); }
        }
        [XmlIgnore(), Category("Blending"), DisplayName("AlphaBlending")]
        public string _AlphaBlending
        {
            get { return Project.ProcentToString(AlphaBlending); }
            set
            {
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                if ((v >= 0) && (v<=100))
                    AlphaBlending = v;
            }
        }
        [XmlIgnore(), Category("Blending"), DisplayName("Mode"), Description("Selects the way the blending is done")]
        public BlendingMode _BlendMode
        {
            get { return BlendMode; }
            set { BlendMode = value; }
        }
        #endregion


        protected bool HasShader()
        {
            if ((ShaderName == null) || (ShaderName.Length == 0))
                return false;
            return true;
        }
        public bool CheckShaderValidity(AppResources Resources)
        {
            if (HasShader() == false)
                return true;
            return Resources.Shaders.ContainsKey(ShaderName);
        }
        protected void AddShaderBinaryInfo_old(string objType, string objName, ByteBuffer bb, ResourcesIndexes ri)
        {
            if (!HasShader())
                return;
            if (ri.ShaderIndex.ContainsKey(ShaderName) == false)
            {
                bb.GetErrorsCountainer().AddError(String.Format("Shader {2} in {0} '{1}' is not defined as a variable/resource !", objType, objName, ShaderName));
                return;
            }
            bb.AddByte(ri.ShaderIndex[ShaderName].Index & 0xFF);
            bb.AddByte(0); // fara extra variabile
        }
        protected void AddBlendingInfo(String objType, ByteBuffer bb, ResourcesIndexes ri)
        {
            switch (BlendMode)
            {
                case BlendingMode.None: break; // nu am nimic de adaugat
                case BlendingMode.AlphaBlending: bb.AddByte((byte)(AlphaBlending * 100.0)); break;
                case BlendingMode.ColorBlending: bb.AddInt(ColorBlending); break;
                case BlendingMode.Shader:
                    if (!HasShader())
                    {
                        bb.GetErrorsCountainer().AddError(String.Format("Expecting a shader in object '{0}; for BlendingMode.Shader !", objType));
                    }
                    if (ri.ShaderIndex.ContainsKey(ShaderName) == false)
                    {
                        bb.GetErrorsCountainer().AddError(String.Format("Shader {1} in a '{0}' object is not defined as a variable/resource !", objType, ShaderName));
                        return;
                    }
                    bb.AddByte(ri.ShaderIndex[ShaderName].Index & 0xFF);
                    bb.AddByte(0); // fara extra variabile
                    break;
                default: bb.GetErrorsCountainer().AddError(String.Format("Unknwon blend method '{0}'", BlendMode)); return;
            }
        }
    }


    [XmlType("Rectangle"), XmlRoot("Rectangle")]
    public class RectanglePresentationObject : BasicBlenderObject
    {
        [XmlAttribute()]
        public int BackgroundColor = 0, MarginColor = Color.White.ToArgb(), MarginWidth = 1;

        #region Atribute
        [XmlIgnore(), Category("Appearance"), DisplayName("BackgroundColor")]
        public System.Drawing.Color _BackgroundColor
        {
            get { return System.Drawing.Color.FromArgb(BackgroundColor); }
            set { BackgroundColor = value.ToArgb(); }
        }
        [XmlIgnore(), Category("Appearance"), DisplayName("MarginColor")]
        public System.Drawing.Color _MarginColor
        {
            get { return System.Drawing.Color.FromArgb(MarginColor); }
            set { MarginColor = value.ToArgb(); }
        }
        [XmlIgnore(), Category("Appearance"), DisplayName("MarginWidth")]
        public int _MarginWidth
        {
            get { return MarginWidth; }
            set { MarginWidth = value; if (MarginWidth > 255) MarginWidth = 255; if (MarginWidth < 0) MarginWidth = 0; }
        }
        #endregion

        #region Metode Virtuale
        public override string GetName()
        {
            return String.Format("Rectangle({0},{1},W:{2},H:{3}) - Back:{4}, Margin:{5}, Width: {6}", X, Y, Width, Height, BackgroundColor, MarginColor, MarginWidth);
        }
        public override string GetIconName()
        {
            return "rect";
        }
        public override void Paint(AnimO.Canvas canvas, GRect r, AppResources Resources)
        {
            canvas.DrawRectWithPixelsCoordonates(r.Left,r.Top,r.Right,r.Bottom, MarginColor, BackgroundColor, MarginWidth);
        }
        public override void AddBinaryBuffer(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            bool hasStroke = ((MarginWidth > 0) && (Color.FromArgb(MarginColor).A != 0));
            bool hasFill = (Color.FromArgb(BackgroundColor).A != 0);
            

            if (Angle>0)
            {
                bb.AddType(BinaryAnimObjectType.RectFull);
                bb.AddShort(r.Left);
                bb.AddShort(r.Top);
                bb.AddShort(r.Right);
                bb.AddShort(r.Bottom);
                bb.AddInt(BackgroundColor);
                bb.AddInt(MarginColor);
                bb.AddByte(MarginWidth);
                if (UseRotateAlign)
                {
                    bb.AddShort(r.GetAnchorX(RotateCenterAlign));
                    bb.AddShort(r.GetAnchorY(RotateCenterAlign));
                }
                else
                {
                    bb.AddShort(RotateCenterX);
                    bb.AddShort(RotateCenterY);
                }
                bb.AddShort((int)(Angle * 100));
            } else {
                if ((hasFill) && (hasStroke))
                {
                    bb.AddType(BinaryAnimObjectType.RectFillAndStroke);
                    bb.AddShort(r.Left);
                    bb.AddShort(r.Top);
                    bb.AddShort(r.Right);
                    bb.AddShort(r.Bottom);
                    bb.AddInt(BackgroundColor);
                    bb.AddInt(MarginColor);
                    bb.AddByte(MarginWidth);
                }
                if ((hasFill) && (!hasStroke))
                {
                    bb.AddType(BinaryAnimObjectType.RectFill);
                    bb.AddShort(r.Left);
                    bb.AddShort(r.Top);
                    bb.AddShort(r.Right);
                    bb.AddShort(r.Bottom);
                    bb.AddInt(BackgroundColor);
                }
                if ((!hasFill) && (hasStroke))
                {
                    bb.AddType(BinaryAnimObjectType.RectStroke);
                    bb.AddShort(r.Left);
                    bb.AddShort(r.Top);
                    bb.AddShort(r.Right);
                    bb.AddShort(r.Bottom);
                    bb.AddInt(MarginColor);
                    bb.AddByte(MarginWidth);
                }
            }
        }
        #endregion
    }
    [XmlType("Line"), XmlRoot("Line")]
    public class LinePresentationObject : PresentationObject
    {
        [XmlAttribute()]
        public int Color = System.Drawing.Color.White.ToArgb();
        
        [XmlAttribute()]
        public int Width = 1,X1,Y1,X2,Y2;



        #region Atribute
        [XmlIgnore(), Category("Appearance"), DisplayName("LineColor")]
        public System.Drawing.Color _Color
        {
            get { return System.Drawing.Color.FromArgb(Color); }
            set { Color = value.ToArgb(); }
        }
        [XmlIgnore(), Category("Appearance"), DisplayName("LineWidth")]
        public int _Width
        {
            get { return Width; }
            set { Width = value; if (Width > 255) Width = 255; if (Width < 0) Width = 0; }
        }
        [XmlIgnore(), Category("Coordonates"), DisplayName("X1")]
        public int _X1
        {
            get { return X1; }
            set { X1 = value; }
        }
        [XmlIgnore(), Category("Coordonates"), DisplayName("Y1")]
        public int _Y1
        {
            get { return Y1; }
            set { Y1 = value; }
        }
        [XmlIgnore(), Category("Coordonates"), DisplayName("X2")]
        public int _X2
        {
            get { return X2; }
            set { X2 = value; }
        }
        [XmlIgnore(), Category("Coordonates"), DisplayName("Y2")]
        public int _Y2
        {
            get { return Y2; }
            set { Y2 = value; }
        }
        #endregion

        #region Metode Virtuale
        public override string GetName()
        {
            return String.Format("Line({0},{1}-{2},{3})", X1, Y1, X2, Y2);
        }
        public override string GetIconName()
        {
            return "line";
        }
        public override void Paint(AnimO.Canvas canvas, GRect r, AppResources Resources)
        {
            if ((Width > 0) && (Color != 0))
                canvas.DrawLineWithPixelsCoordonates(X1, Y1, X2, Y2, Color, Width);
        }
        public override void ComputeRect(GRect rect)
        {
            if (X1 < X2)
            {
                rect.Left = X1;
                rect.Right = X2;
            }
            else
            {
                rect.Right = X1;
                rect.Left = X2;
            }
            if (Y1 < Y2)
            {
                rect.Top = Y1;
                rect.Bottom = Y2;
            }
            else
            {
                rect.Bottom = Y1;
                rect.Top = Y2;
            }
        }
        public override void SetRectangle(GRect rect)
        {
            if (X1 < X2)
            {
                X1 = rect.Left;
                X2 = rect.Right;
            }
            else
            {
                X1 = rect.Right;
                X2 = rect.Left;
            }
            if (Y1 < Y2)
            {
                Y1 = rect.Top;
                Y2 = rect.Bottom;
            }
            else
            {
                Y1 = rect.Bottom;
                Y2 = rect.Top;
            }
        }
        public override int GetValue(AlignComponent ac, GRect tempRect)
        {
            ComputeRect(tempRect);
            switch (ac)
            {
                case AlignComponent.Left:
                    return tempRect.Left;
                case AlignComponent.Top:
                    return tempRect.Top;
                case AlignComponent.Right:
                    return tempRect.Right;
                case AlignComponent.Bottom:
                    return tempRect.Bottom;
                case AlignComponent.Width:
                    return tempRect.GetWidth();
                case AlignComponent.Height:
                    return tempRect.GetHeight();
                case AlignComponent.CenterX:
                    return tempRect.CenterX();
                case AlignComponent.CenterY:
                    return tempRect.CenterY();
            };
            return 0;
        }
        public override void SetValue(AlignComponent ac, int newValue, GRect tempRect)
        {
            ComputeRect(tempRect);
            switch (ac)
            {
                case AlignComponent.Left:
                    tempRect.Add(newValue - tempRect.Left, 0);
                    break;
                case AlignComponent.Top:
                    tempRect.Add(0, newValue - tempRect.Top);
                    break;
                case AlignComponent.Right:
                    tempRect.Add(newValue - tempRect.Right, 0);
                    break;
                case AlignComponent.Bottom:
                    tempRect.Add(0, newValue - tempRect.Bottom);
                    break;
                case AlignComponent.Width:
                    tempRect.SetWidthFromLeft(newValue);
                    break;
                case AlignComponent.Height:
                    tempRect.SetHeightFromTop(newValue);
                    break;
                case AlignComponent.CenterX:
                    tempRect.Add(newValue - tempRect.Left - tempRect.GetWidth() / 2, 0);
                    break;
                case AlignComponent.CenterY:
                    tempRect.Add(0, newValue - tempRect.Top - tempRect.GetHeight() / 2);
                    break;
            };
            // updatez pozitiile
            SetRectangle(tempRect);
        }
        public override void AddBinaryBuffer(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            //if (Angle != 0)
            //{                
            //    bb.AddType(BinaryAnimObjectType.LineFull);
            //    bb.AddShort(X1);
            //    bb.AddShort(Y1);
            //    bb.AddShort(X2);
            //    bb.AddShort(Y2);
            //    bb.AddByte(Width);
            //    bb.AddInt(Color);
            //    if (UseRotateAlign)
            //    {
            //        bb.AddShort(r.GetAnchorX(RotateCenterAlign));
            //        bb.AddShort(r.GetAnchorY(RotateCenterAlign));
            //    }
            //    else
            //    {
            //        bb.AddShort(RotateCenterX);
            //        bb.AddShort(RotateCenterY);
            //    }
            //    bb.AddShort((int)(Angle * 100));
            //}
            //else
            {
                if (Width == 1)
                {
                    bb.AddType(BinaryAnimObjectType.LineColor);
                    bb.AddShort(X1);
                    bb.AddShort(Y1);
                    bb.AddShort(X2);
                    bb.AddShort(Y2);
                    bb.AddInt(Color);
                }
                else
                {
                    bb.AddType(BinaryAnimObjectType.LineColorWidth);
                    bb.AddShort(X1);
                    bb.AddShort(Y1);
                    bb.AddShort(X2);
                    bb.AddShort(Y2);
                    bb.AddByte(Width);
                    bb.AddInt(Color);
                }
            }
        }
        #endregion

        #region Metode proprii
        public void SetLine(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
        }
        #endregion
    }
    [XmlType("Clip"), XmlRoot("Clip")]
    public class ClipPresentationObject : PresentationObject
    {
        [XmlAttribute()]
        public int Width = 1, X1, Y1, X2, Y2;

        #region Atribute
        [XmlIgnore(), Category("Coordonates"), DisplayName("X1")]
        public int _X1
        {
            get { return X1; }
            set { X1 = value; }
        }
        [XmlIgnore(), Category("Coordonates"), DisplayName("Y1")]
        public int _Y1
        {
            get { return Y1; }
            set { Y1 = value; }
        }
        [XmlIgnore(), Category("Coordonates"), DisplayName("X2")]
        public int _X2
        {
            get { return X2; }
            set { X2 = value; }
        }
        [XmlIgnore(), Category("Coordonates"), DisplayName("Y2")]
        public int _Y2
        {
            get { return Y2; }
            set { Y2 = value; }
        }
        #endregion

        #region Metode Virtuale
        public override string GetName()
        {
            return String.Format("Clip({0},{1}-{2},{3})", X1, Y1, X2, Y2);
        }
        public override string GetIconName()
        {
            return "clip";
        }
        public override void Paint(AnimO.Canvas canvas, GRect r, AppResources Resources)
        {
            int col = (int)0x7FFFFFFF;
            canvas.DrawRectWithPixelsCoordonates(0, 0, X1, Y1, 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(X1, 0, X2, Y1, 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(X2, 0, canvas.GetWidth(), Y1, 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(X2, Y1, canvas.GetWidth(), Y2, 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(X2, Y2, canvas.GetWidth(), canvas.GetHeight(), 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(X1, Y2, X2, canvas.GetHeight(), 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(0, Y2, X1, canvas.GetHeight(), 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(0, Y1, X1, Y2, 0, col, 0);


            //g.DrawRect(0, 0, X1, g.Height, 0, col, 0);
            //g.DrawRect(X2, 0, g.Width, g.Height, 0, col, 0);
            //g.DrawRect(X1, 0, X2, Y1, 0, col, 0);
            //g.DrawRect(X1, Y2, X2, g.Height, 0, col, 0);
            canvas.DrawRectWithPixelsCoordonates(X1, Y1, X2, Y2, -1, 0, 2);
        }

        public override void AddBinaryBuffer(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            bb.AddType(BinaryAnimObjectType.Clipping);
            bb.AddShort(X1);
            bb.AddShort(Y1);
            bb.AddShort(X2);
            bb.AddShort(Y2);
        }
        public override void ComputeRect(GRect rect)
        {
            if (X1 <= X2)
            {
                rect.Left = X1;
                rect.Right = X2;
            }
            else
            {
                rect.Right = X1;
                rect.Left = X2;
            }
            if (Y1 <= Y2)
            {
                rect.Top = Y1;
                rect.Bottom = Y2;
            }
            else
            {
                rect.Bottom = Y1;
                rect.Top = Y2;
            }
        }
        public override void SetRectangle(GRect rect)
        {
            if (X1 <= X2)
            {
                X1 = rect.Left;
                X2 = rect.Right;
            }
            else
            {
                X1 = rect.Right;
                X2 = rect.Left;
            }
            if (Y1 <= Y2)
            {
                Y1 = rect.Top;
                Y2 = rect.Bottom;
            }
            else
            {
                Y1 = rect.Bottom;
                Y2 = rect.Top;
            }
        }
        public override int GetValue(AlignComponent ac, GRect tempRect)
        {
            ComputeRect(tempRect);
            switch (ac)
            {
                case AlignComponent.Left:
                    return tempRect.Left;
                case AlignComponent.Top:
                    return tempRect.Top;
                case AlignComponent.Right:
                    return tempRect.Right;
                case AlignComponent.Bottom:
                    return tempRect.Bottom;
                case AlignComponent.Width:
                    return tempRect.GetWidth();
                case AlignComponent.Height:
                    return tempRect.GetHeight();
                case AlignComponent.CenterX:
                    return tempRect.CenterX();
                case AlignComponent.CenterY:
                    return tempRect.CenterY();
            };
            return 0;
        }
        public override void SetValue(AlignComponent ac, int newValue, GRect tempRect)
        {
            ComputeRect(tempRect);
            switch (ac)
            {
                case AlignComponent.Left:
                    tempRect.Add(newValue - tempRect.Left, 0);
                    break;
                case AlignComponent.Top:
                    tempRect.Add(0, newValue - tempRect.Top);
                    break;
                case AlignComponent.Right:
                    tempRect.Add(newValue - tempRect.Right, 0);
                    break;
                case AlignComponent.Bottom:
                    tempRect.Add(0, newValue - tempRect.Bottom);
                    break;
                case AlignComponent.Width:
                    tempRect.SetWidthFromLeft(newValue);
                    break;
                case AlignComponent.Height:
                    tempRect.SetHeightFromTop(newValue);
                    break;
                case AlignComponent.CenterX:
                    tempRect.Add(newValue - tempRect.Left - tempRect.GetWidth() / 2, 0);
                    break;
                case AlignComponent.CenterY:
                    tempRect.Add(0, newValue - tempRect.Top - tempRect.GetHeight() / 2);
                    break;
            };
            // updatez pozitiile
            SetRectangle(tempRect);
        }
        #endregion
    }
    [XmlType("ClearClip"), XmlRoot("ClearClip")]
    public class ClearClipAnimationObject : PresentationObject
    {

        #region Metode Virtuale
        public override void AddBinaryBuffer(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            bb.AddType(BinaryAnimObjectType.ClearClipping);
        }
        #endregion
    }
    [XmlType("Image"), XmlRoot("Image")]
    public class ImagePresentationObject : BasicBlenderObject
    {
        [XmlAttribute()]
        public string ImageName = "";
        [XmlAttribute()]
        public ImageResizeMode ResizeMode = ImageResizeMode.Fill;
        [XmlAttribute()]
        public Alignament ImageAlign = Alignament.Center;

        [XmlIgnore()]
        GBitmap tempBmp = new GBitmap();
        [XmlIgnore()]
        AnimO.RuntimeContext rc = new AnimO.RuntimeContext();

        #region Atribute
        [XmlIgnore(), Category("Image"), DisplayName("Image"), Editor(typeof(ImageSelectorEditor), typeof(UITypeEditor))]
        public string _ImageName
        {
            get { return ImageName; }
            set { ImageName = value; }
        }
        [XmlIgnore(), Category("Image"), DisplayName("Resize Mode")]
        public ImageResizeMode _ResizeMode
        {
            get { return ResizeMode; }
            set { ResizeMode = value; }
        }
        [XmlIgnore(), Category("Image"), DisplayName("Alignament")]
        public Alignament _ImageAlign
        {
            get { return ImageAlign; }
            set { ImageAlign = value; }
        }
        #endregion

        #region Metode Virtuale
        public override string GetName()
        {
            return String.Format("Image({0},{1},{2})", ImageName, X, Y);
        }
        public override string GetIconName()
        {
            return "image";
        }
        public override void Paint(AnimO.Canvas canvas, GRect r, AppResources Resources)
        {
            //*
            if ((ImageName==null) || (ImageName.Length==0) || (Resources.Images.ContainsKey(ImageName) == false))
            {
                canvas.DrawRectWithPixelsCoordonates(r.Left, r.Top, r.Right, r.Bottom, Color.White.ToArgb(), Color.Black.ToArgb(), 1);
                canvas.DrawLineWithPixelsCoordonates(r.Left, r.Top, r.Right, r.Bottom, Color.Black.ToArgb(), 1);
                canvas.DrawLineWithPixelsCoordonates(r.Left, r.Bottom, r.Right, r.Top, Color.Black.ToArgb(), 1);
                //g.DrawString(null, "Missing: " + ImageName, r.CenterX(), r.CenterY(), Alignament.Center, Color.Red.ToArgb(), 8);
            }
            else            
            {             
                ImageResource ir = (ImageResource)Resources.Images[ImageName];
                rc.Image = ir.Picture;                
                rc.ColorBlending = AnimO.RuntimeContext.BlendModeToColor(BlendMode,ColorBlending,AlphaBlending);
                float imgWidth = ir.Picture.Width;
                float imgHeight = ir.Picture.Height;
                float rw, rh;


                switch (this.ResizeMode)
                {
                    //case ImageResizeMode.None:
                    //    DrawImage(bmp, x, y, align, bmp.Width, bmp.Height);
                    //    break;
                    case ImageResizeMode.Fill:
                        rc.X_Percentage = canvas.ConvertXAxisToPercentage(r.Left);
                        rc.Y_Percentage = canvas.ConvertYAxisToPercentage(r.Top);
                        rc.ScaleWidth = (float)r.GetWidth() / imgWidth;
                        rc.ScaleHeight = (float)r.GetHeight() / imgHeight;
                        rc.Align = Alignament.TopLeft;
                        canvas.DrawImage(rc);
                        break;
                    case ImageResizeMode.Fit:
                        rw = (float)r.GetWidth() / imgWidth;
                        rh = (float)r.GetHeight() / imgHeight;
                        if (rw > rh)
                            rw = rh;
                        rc.ScaleWidth = rw;
                        rc.ScaleHeight = rw;
                        rc.X_Percentage = canvas.ConvertXAxisToPercentage(r.GetDockX(this.ImageAlign, (int)(imgWidth * rc.ScaleWidth)));
                        rc.Y_Percentage = canvas.ConvertYAxisToPercentage(r.GetDockY(this.ImageAlign, (int)(imgHeight * rc.ScaleHeight)));
                        rc.Align = Alignament.TopLeft;
                        canvas.DrawImage(rc);
                        break;
                    case ImageResizeMode.Shrink:
                        rw = (float)r.GetWidth() / imgWidth;
                        rh = (float)r.GetHeight() / imgHeight;
                        if ((rw >= 1) && (rh >= 1))
                        {
                            rc.ScaleWidth = 1.0f;
                            rc.ScaleHeight = 1.0f;
                            rc.X_Percentage = canvas.ConvertXAxisToPercentage(r.GetDockX(this.ImageAlign, (int)imgWidth));
                            rc.Y_Percentage = canvas.ConvertYAxisToPercentage(r.GetDockY(this.ImageAlign, (int)imgHeight));
                            canvas.DrawImage(rc);
                        }
                        else
                        {
                            if (rw > rh)
                                rw = rh;
                            rc.ScaleWidth = rw;
                            rc.ScaleHeight = rw;
                            rc.X_Percentage = canvas.ConvertXAxisToPercentage(r.GetDockX(this.ImageAlign, (int)(imgWidth * rc.ScaleWidth)));
                            rc.Y_Percentage = canvas.ConvertYAxisToPercentage(r.GetDockY(this.ImageAlign, (int)(imgHeight * rc.ScaleHeight)));
                            canvas.DrawImage(rc);
                        }
                        break;
                    case ImageResizeMode.Dock:
                        rc.ScaleWidth = this.ScaleWidth;
                        rc.ScaleHeight = this.ScaleHeight;
                        rc.X_Percentage = canvas.ConvertXAxisToPercentage(r.GetAnchorX(this.ImageAlign));
                        rc.Y_Percentage = canvas.ConvertYAxisToPercentage(r.GetAnchorY(this.ImageAlign));
                        rc.Align = this.ImageAlign;
                        canvas.DrawImage(rc);
                        break;
                }
            }
            // */
        }
        public override void AddBinaryBuffer(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            int w = -1, h = -1;
            //*
            if (Resources.Images.ContainsKey(ImageName))
            {
                Image i = Resources.Images[ImageName].Picture;
                if (i != null)
                {
                    w = i.Width;
                    h = i.Height;
                }
            }
            if ((w <= 0) || (h <= 0))
            {
                bb.GetErrorsCountainer().AddError(String.Format("Image '{0}' is not defined as a variable or is not build !", ImageName));
                return;
            }
            if (Angle == 0)
            {            
                switch (ResizeMode)
                {
                    case ImageResizeMode.Fill:
                        if ((w == r.GetWidth()) && (h == r.GetHeight()))
                        {
                            switch (BlendMode)
                            {
                                case BlendingMode.None: bb.AddType(BinaryAnimObjectType.ImageSimple); break;
                                case BlendingMode.ColorBlending: bb.AddType(BinaryAnimObjectType.ImageSimpleWithColorBlending); break;
                                case BlendingMode.AlphaBlending: bb.AddType(BinaryAnimObjectType.ImageSimpleWithAlphaBlending); break;
                                case BlendingMode.Shader: bb.AddType(BinaryAnimObjectType.ImageSimpleWithShader); break;
                                default: bb.GetErrorsCountainer().AddError(String.Format("Unknwon blend method '{0}' in Image '{1}'", BlendMode, ImageName)); return;
                            } 

                            bb.AddWord(ri.ImageIndex[ImageName].Index);
                            bb.AddShort(r.Left);
                            bb.AddShort(r.Top);
                            //AddShaderBinaryInfo("Image", ImageName, bb, ri);
                            AddBlendingInfo("Image:" + ImageName, bb, ri);      
                        }
                        else
                        {
                            switch (BlendMode)
                            {
                                case BlendingMode.None: bb.AddType(BinaryAnimObjectType.ImageResized); break;
                                case BlendingMode.ColorBlending: bb.AddType(BinaryAnimObjectType.ImageResizedWithColorBlending); break;
                                case BlendingMode.AlphaBlending: bb.AddType(BinaryAnimObjectType.ImageResizedWithAlphaBlending); break;
                                case BlendingMode.Shader: bb.AddType(BinaryAnimObjectType.ImageResizedWithShader); break;
                                default: bb.GetErrorsCountainer().AddError(String.Format("Unknwon blend method '{0}' in Image '{1}'", BlendMode, ImageName)); return;
                            } 
                            bb.AddWord(ri.ImageIndex[ImageName].Index);
                            bb.AddShort(r.Left);
                            bb.AddShort(r.Top);
                            bb.AddShort(r.GetWidth());
                            bb.AddShort(r.GetHeight());
                            //AddShaderBinaryInfo("Image", ImageName, bb, ri);
                            AddBlendingInfo("Image:" + ImageName, bb, ri);      
                        }
                        break;
                    case ImageResizeMode.Dock:
                            switch (BlendMode)
                            {
                                case BlendingMode.None: bb.AddType(BinaryAnimObjectType.ImageDocked); break;
                                case BlendingMode.ColorBlending: bb.AddType(BinaryAnimObjectType.ImageDockedWithColorBlending); break;
                                case BlendingMode.AlphaBlending: bb.AddType(BinaryAnimObjectType.ImageDockedWithAlphaBlending); break;
                                case BlendingMode.Shader: bb.AddType(BinaryAnimObjectType.ImageDockedWithShader); break;
                                default: bb.GetErrorsCountainer().AddError(String.Format("Unknwon blend method '{0}' in Image '{1}'", BlendMode, ImageName)); return;
                            }
                            bb.AddWord(ri.ImageIndex[ImageName].Index);
                            bb.AddShort(r.GetAnchorX(ImageAlign));
                            bb.AddShort(r.GetAnchorY(ImageAlign));
                            bb.AddByte(AlignamentToInt(ImageAlign));
                            AddBlendingInfo("Image:" + ImageName, bb, ri);      
                            //AddShaderBinaryInfo("Image", ImageName, bb, ri);
                        break;
                    default:
                        bb.GetErrorsCountainer().AddError(String.Format("Image '{0}' is resized using {1}. This mode is not suported yet !", ImageName,ResizeMode.ToString()));
                        break;

                }

            }
            else
            {
                bb.GetErrorsCountainer().AddError(String.Format("Image '{0}' uses rotation. Rotation is not supported yet !", ImageName));
                bb.AddType(BinaryAnimObjectType.ImageFull);
                bb.AddWord(ri.ImageIndex[ImageName].Index);
                bb.AddShort(r.Left);
                bb.AddShort(r.Top);
                bb.AddShort(r.GetWidth());
                bb.AddShort(r.GetHeight());
                if (UseRotateAlign)
                {
                    bb.AddShort(r.GetAnchorX(RotateCenterAlign));
                    bb.AddShort(r.GetAnchorY(RotateCenterAlign));
                }
                else
                {
                    bb.AddShort(RotateCenterX);
                    bb.AddShort(RotateCenterY);
                }
                bb.AddShort((int)(Angle * 100));
            }
            // */ 
        }
        #endregion
    }
    [XmlType("Text"), XmlRoot("Text")]
    public class TextPresentationObject : BasicBlenderObject
    {
        [XmlAttribute()]
        public string Text = "";
        [XmlAttribute()]
        public string StringResource = "";
        [XmlAttribute()]
        public string ValueName = "";
        [XmlAttribute()]
        public bool WordWrap = false, Justify = false;
        [XmlAttribute()]
        public string Font = "";
        [XmlAttribute()]
        public Alignament TextAlignament = Alignament.Center;
        [XmlAttribute()]
        public float LineSpace = 0.0f,Size = 1f,SpaceWidth=-1,CharacterSpace=-1;
        [XmlAttribute()]
        public TextPainter.FontSizeMethod FontSizeType = TextPainter.FontSizeMethod.Scale;
        [XmlAttribute()]
        public TextModeType TextMode = TextModeType.None;

        [XmlIgnore()]
        TextPainter tp = new TextPainter();

        #region Atribute
        [XmlIgnore(), Category("Text"), DisplayName("Mode"), Description("How the text is to be used (Constant means a constant text that cannot be change)")]
        public TextModeType _TextMode
        {
            get { return TextMode; }
            set { TextMode = value; }
        }
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
        [XmlIgnore(), Category("Text"), DisplayName("Value name"), Description("Represents the name of the value that will be used to store a dynamic text !")]
        public string _ValueName
        {
            get { return ValueName; }
            set { ValueName = value; }
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
        [XmlIgnore(), Category("Font"), DisplayName("Alignament")]
        public Alignament _TextAlignament
        {
            get { return TextAlignament; }
            set { TextAlignament = value; }
        }
        [XmlIgnore(), Category("Font"), DisplayName("Line spaces")]
        public string _LineSpace
        {
            get { return Project.ProcentToString(LineSpace); }
            set { 
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                LineSpace = v;
            }
        }
        [XmlIgnore(), Category("Font"), DisplayName("Character spaces")]
        public float _CharacterSpace
        {
            get { return CharacterSpace; }
            set { CharacterSpace = value; }
        }
        [XmlIgnore(), Category("Font"), DisplayName("Space width")]
        public float _SpaceWidth
        {
            get { return SpaceWidth; }
            set { SpaceWidth = value; }
        }
        [XmlIgnore(), Category("Font"), DisplayName("Size")]
        public float _Size
        {
            get { return Size; }
            set { Size = value; }
        }
        [XmlIgnore(), Category("Font"), DisplayName("FontSizeMethod")]
        public TextPainter.FontSizeMethod _FontSizeMethod
        {
            get { return FontSizeType; }
            set { FontSizeType = value; }
        }
        #endregion

        public void Refresh()
        {
            tp.ForceRecompute();
        }
        private int FontSizeMethodToInt(TextPainter.FontSizeMethod value)
        {
            switch (value)
            {
                case TextPainter.FontSizeMethod.Scale: return 0;
                case TextPainter.FontSizeMethod.Pixels: return 1;
                case TextPainter.FontSizeMethod.PercentageOfHeight: return 2;
                case TextPainter.FontSizeMethod.ShrinkToFitWidth: return 3;
                case TextPainter.FontSizeMethod.ZoomToFitWidth: return 4;
            }
            return 0;
        }
        

        #region Metode Virtuale
        public override string GetName()
        {
            string s = "";
            switch (TextMode)
            {
                case TextModeType.None: s = "Text(invalid!!!)"; break;
                case TextModeType.FromResources: s = "Text(Resource=" + this.StringResource + ")"; break;
                case TextModeType.UserValue: s = "Text(name=" + this.ValueName + ") [" + Text + "]"; break;
                case TextModeType.UseTextField: s = "Text(\"" + Text + "\")"; break;
                default: s = "Text(???)"; break;
            }
            return String.Format("{0} X={1},Y={2},W={3},H={4},A={5},BlendMode={6}", s, X, Y, this.Width, this.Height, this.TextAlignament, this.BlendMode);
        }
        public override string GetIconName()
        {
            return "text";
        }
        public override void Paint(AnimO.Canvas canvas, GRect r, AppResources Resources)
        {
            tp.SetPosition(r.Left, r.Top, r.Right, r.Bottom);
            tp.SetAlignament(TextAlignament);
            tp.SetFont(Font);
            tp.SetWordWrap(WordWrap);
            tp.SetJustify(Justify);
            if (TextMode == TextModeType.FromResources)
                tp.SetText(this.StringResource, true, Resources);
            else
                tp.SetText(Text, false, Resources);
            tp.SetFontSize(TextPainter.FontSizeMethod.Scale, Size);
            tp.SetSpaceWidth(SpaceWidth);
            tp.SetCharacterSpacing(CharacterSpace);
            tp.SetLineSpace(LineSpace);
            tp.SetBlending(BlendMode, ColorBlending, AlphaBlending);
            tp.Paint(canvas, Resources);
        }
        private void AddStandardFontProperties(ByteBuffer bb,ResourcesIndexes ri, GRect r)
        {
            byte tmp;
           
            bb.AddShort(r.Left);
            bb.AddShort(r.Top);
            bb.AddShort(r.Right);
            bb.AddShort(r.Bottom);
            tmp = (byte)(AlignamentToInt(TextAlignament));
            if (WordWrap)
                tmp |= 64;
            if (Justify)
                tmp |= 32;
            bb.AddByte(tmp);

            // FONT ID
            bb.AddByte((byte)ri.FontsIndex[Font].Index);
            bb.AddByte(FontSizeMethodToInt(FontSizeType));
            bb.AddWord((int)(Size * 100));
            bb.AddShort((int)(LineSpace * 100));
        }
        private void AddBinaryBuffer_FromResources(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            if (Resources.Strings.ContainsKey(this.StringResource) == false)
            {
                bb.GetErrorsCountainer().AddError(String.Format("String resource '{0}' is not does not exists !", this.StringResource));
                return;
            }
            switch (BlendMode)
            {
                case BlendingMode.None: bb.AddType(BinaryAnimObjectType.TextFromResources); break;
                case BlendingMode.ColorBlending: bb.AddType(BinaryAnimObjectType.TextFromResourcesWidthColorBlending); break;
                case BlendingMode.AlphaBlending: bb.AddType(BinaryAnimObjectType.TextFromResourcesWidthAlphaBlending); break;
                case BlendingMode.Shader: bb.AddType(BinaryAnimObjectType.TextFromResourcesWidthShader); break;
                default: bb.GetErrorsCountainer().AddError(String.Format("Unknwon blend method '{0}' in Font '{1}'", BlendMode, Font)); return;
            }
            bb.AddWord(ri.StringIndex[this.StringResource].Index);
            AddStandardFontProperties(bb, ri, r);
            AddBlendingInfo("String resource:" + this.StringResource, bb, ri);
        }
        private void AddBinaryBuffer_FromUserValue(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            if (ValueName.Length==0)
            {
                bb.GetErrorsCountainer().AddError(String.Format("Expecting a valid (non empty) value name !"));
                return;
            }
            switch (BlendMode)
            {
                case BlendingMode.None: bb.AddType(BinaryAnimObjectType.TextFromUserValue); break;
                case BlendingMode.ColorBlending: bb.AddType(BinaryAnimObjectType.TextFromUserValueWithColorBlending); break;
                case BlendingMode.AlphaBlending: bb.AddType(BinaryAnimObjectType.TextFromUserValueWithAlphaBlending); break;
                case BlendingMode.Shader: bb.AddType(BinaryAnimObjectType.TextFromUserValueWidthShader); break;
                default: bb.GetErrorsCountainer().AddError(String.Format("Unknwon blend method '{0}' in Font '{1}'", BlendMode, Font)); return;
            }
            bb.AddWord(ri.StringValueIndex[this.ValueName]);
            AddStandardFontProperties(bb, ri, r);
            AddBlendingInfo("String user value:" + this.ValueName, bb, ri);
        }
        private void AddBinaryBuffer_UseTextField(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            switch (BlendMode)
            {
                case BlendingMode.None: bb.AddType(BinaryAnimObjectType.TextFromString); break;
                case BlendingMode.ColorBlending: bb.AddType(BinaryAnimObjectType.TextFromStringWidthColorBlending); break;
                case BlendingMode.AlphaBlending: bb.AddType(BinaryAnimObjectType.TextFromStringWidthAlphaBlending); break;
                case BlendingMode.Shader: bb.AddType(BinaryAnimObjectType.TextFromStringWidthShader); break;
            }
            AddStandardFontProperties(bb, ri, r);
            bb.AddString(Text, true);
            AddBlendingInfo("String:" + Text, bb, ri);
        }
        public override void AddBinaryBuffer(ByteBuffer bb, ResourcesIndexes ri, GRect r, AppResources Resources)
        {
            if (TextMode == TextModeType.None)
            {
                bb.GetErrorsCountainer().AddError(String.Format("Field 'TextMode' was not set (for string '{0}') !", Text));
                return;
            }
            if (Resources.Fonts.ContainsKey(Font) == false)
            {
                bb.GetErrorsCountainer().AddError(String.Format("Font '{0}' is not defined as a variable or is not build !", Font));
                return;
            }            
            switch (TextMode)
            {
                case TextModeType.FromResources:
                    AddBinaryBuffer_FromResources(bb, ri, r, Resources);
                    break;
                case TextModeType.UseTextField:
                    AddBinaryBuffer_UseTextField(bb, ri, r, Resources);
                    break;
                case TextModeType.UserValue:
                    AddBinaryBuffer_FromUserValue(bb, ri, r, Resources);
                    break;
                default:
                    bb.GetErrorsCountainer().AddError(String.Format("Unknwon text mode: '{0}'", TextMode));
                    break;
            }
        }
        #endregion
    }
}
