using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace GAppCreator
{
    public class ColorGradientStop
    {
        [XmlAttribute()]
        public float Stop = 0.0f;
        [XmlAttribute()]
        public byte Red = 0;
        [XmlAttribute()]
        public byte Green = 0;
        [XmlAttribute()]
        public byte Blue = 0;
        [XmlAttribute()]
        public byte Alpha = 0;

        #region Atribute
        [XmlIgnore(), Category("Color"), DisplayName("Red")]
        public byte _R
        {
            get { return Red; }
            set { Red = value; }
        }
        [XmlIgnore(), Category("Color"), DisplayName("Green")]
        public byte _G
        {
            get { return Green; }
            set { Green = value; }
        }
        [XmlIgnore(), Category("Color"), DisplayName("Blue")]
        public byte _B
        {
            get { return Blue; }
            set { Blue = value; }
        }
        [XmlIgnore(), Category("Color"), DisplayName("Alpha")]
        public byte _A
        {
            get { return Alpha; }
            set { Alpha = value; }
        }
        [XmlIgnore(), Category("Stop"), DisplayName("Stop")]
        public float _S
        {
            get { return Stop*100.0f; }
            set { if ((value >= 0) && (value <= 100.0f)) Stop = value / 100.0f; }
        }
        #endregion
    }
    public enum GradientType
    {
        Linear,
        Circular,
    }
    public class ColorGradient
    {
        [XmlAttribute()]
        public GradientType Type = GradientType.Linear;
        public List<ColorGradientStop> Stops = new List<ColorGradientStop>();

    }
}
