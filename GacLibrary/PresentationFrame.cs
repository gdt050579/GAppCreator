using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace GAppCreator
{
    [XmlRoot("Frame"),XmlType("Frame")]
    public class PresentationFrame
    {
        [XmlArrayItem(typeof(RectanglePresentationObject))]
        [XmlArrayItem(typeof(ImagePresentationObject))]
        [XmlArrayItem(typeof(LinePresentationObject))]
        [XmlArrayItem(typeof(TextPresentationObject))]
        [XmlArrayItem(typeof(ClipPresentationObject))]
        public List<PresentationObject> Objects;

        [XmlAttribute()]
        public bool Bookmark;
        [XmlAttribute()]
        public int Time;

        #region Atribute
        [XmlIgnore(), Category("Misc"), DisplayName("IsBookmark")]
        public bool _Bookmark
        {
            get { return Bookmark; }
            set { Bookmark = value; }
        }
        [XmlIgnore(), Category("Misc"), DisplayName("TimeWait")]
        public int _Time
        {
            get { return Time; }
            set { Time = value; }
        }

        [XmlIgnore(), Category("Informations"), DisplayName("Objects Count")]
        public int _ObjectsCount
        {
            get { return Objects.Count; }
        }
        #endregion

        [XmlIgnore()]
        public GBitmap Icon = null;
        [XmlIgnore()]
        public GRect tempRect = new GRect();
        

        public PresentationFrame()
        {
            Objects = new List<PresentationObject>();
            Icon = new GBitmap();
            Icon.Create(64, 64);
        }
        public void Paint(AnimO.Canvas canvas, AppResources Resources)
        {
            foreach (PresentationObject obj in Objects)
            {
                obj.ComputeRect(tempRect);
                //if (obj.Angle != 0)
                //{
                //    g.SaveState();
                //    if (obj.UseRotateAlign)
                //        g.Rotate(obj.Angle, tempRect.GetAnchorX(obj.RotateCenterAlign), tempRect.GetAnchorY(obj.RotateCenterAlign));
                //    else
                //        g.Rotate(obj.Angle, obj.RotateCenterX, obj.RotateCenterY);
                //}

                obj.Paint(canvas,tempRect,Resources);
                if (obj.SelectionIndex >= 0)
                {                    
                    canvas.DrawRectWithPixelsCoordonates(tempRect.Left, tempRect.Top, tempRect.Left+20, tempRect.Top+10, 0,Color.White.ToArgb(),0);
                    canvas.DrawRectWithPixelsCoordonates(tempRect.Left + 1, tempRect.Top + 1, tempRect.Left + 19, tempRect.Top + 8, Color.Black.ToArgb(), 0, 1);
                    //g.DrawString(null, (obj.SelectionIndex + 1).ToString(), tempRect.Left + 10, tempRect.Top + 8, Alignament.BottomCenter, Color.Black.ToArgb(), 6);
                }
                //if (obj.Angle != 0)
                //    g.RestoreState();
            }
        }
        public void Add(PresentationObject obj)
        {
            if (obj != null)
                Objects.Add(obj);
        }
        public PresentationObject HitTest(int x, int y, bool firstHit)
        {
            PresentationObject result = null;
            foreach (PresentationObject obj in Objects)
            {
                obj.ComputeRect(tempRect);
                if (tempRect.Contains(x,y))                
                {
                    result = obj;
                    if (firstHit)
                        break;
                }
            }
            return result;
        }
        public void MoveZOrder(int index, int steps)
        {
            if ((index < 0) || (index >= Objects.Count))
                return;
            int newIndex = index + steps;
            if (newIndex < 0)
                newIndex = 0;
            if (newIndex > Objects.Count - 1)
                newIndex = Objects.Count - 1;
            if (newIndex == index)
                return;
            // mut datele
            PresentationObject aux = Objects[index];
            if (newIndex < index)
            {
                for (int tr = index - 1; tr >= newIndex; tr--)
                    Objects[tr + 1] = Objects[tr];
            }
            else
            {
                for (int tr = index + 1; tr <= newIndex; tr++)
                    Objects[tr - 1] = Objects[tr];
            }
            Objects[newIndex] = aux;
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
                MessageBox.Show("Unable to convert frame to XML: \n" + e.ToString());
                return "";
            }
        }
        public static PresentationFrame FromXMLString(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(PresentationFrame));
                PresentationFrame af = serializer.Deserialize(stringReader) as PresentationFrame;
                if (af == null)
                {
                    MessageBox.Show("Unable to create frame from XML: \n" + xmlText);
                }
                return af;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to create frame from XML: \n"+e.ToString());
                return null;
            }
        }
    }
}
