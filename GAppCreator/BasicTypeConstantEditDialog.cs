using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class BasicTypeConstantEditDialog : Form
    {
        public string FieldName;
        public string FieldValue;
        public string FieldDescription;

        Project prj;
        SolidBrush br = new SolidBrush(Color.Black);
        BasicTypesConstantType basicType = BasicTypesConstantType.None;
        public BasicTypeConstantEditDialog(Project p, bool _editFieldName,string fieldName, BasicTypesConstantType type,string description,string value)
        {
            InitializeComponent();

            prj = p;

            // default-uri
            rbFalse.Checked = true;
            SetColor(0xFF808080);


            pnlBool.Dock = DockStyle.Fill;
            pnlFloat.Dock = DockStyle.Fill;
            pnlNumeric.Dock = DockStyle.Fill;
            pnlStringValue.Dock = DockStyle.Fill;
            pnlColor.Dock = DockStyle.Fill;
            txFieldName.Text = fieldName;
            txFieldName.Enabled = _editFieldName;
            txFieldType.Text = type.ToString();
            basicType = type;
            txDescription.Text = description;
            
            // setez tipul


            this.Width = 600;
            if ((value!=null) && (type!= BasicTypesConstantType.None))
            {
                if (ConstantHelper.IsNumber(basicType))
                {
                    pnlNumeric.Visible = true;
                    value = value.Trim().ToUpper();
                    if (value.StartsWith("0X"))
                        txNum16.Text = value.Substring(2);
                    else
                        txNum10.Text = value;
                }
                if (ConstantHelper.IsFloat(basicType))
                {
                    pnlFloat.Visible = true;
                    txFloat.Text = value;
                }
                switch (basicType)
                {
                    case BasicTypesConstantType.String:
                        //if ((value.StartsWith("\"")) && (value.EndsWith("\"")))
                        //    value = value.Substring(1,value.Length-2);
                        //value = value.Replace("\\n", "\r\n").Replace("\\t", "\t").Replace("\\\"", "\"").Replace("\\\\", "\\");
                        txStringValue.Text = value;
                        pnlStringValue.Visible = true;
                        break;
                    case BasicTypesConstantType.Color:
                        value = value.Trim().ToUpper();
                        if (value.StartsWith("0X"))
                            txColorHexa.Text = value.Substring(2);
                        else
                            txColorDecimal.Text = value;
                        pnlColor.Visible = true;
                        break;
                    case BasicTypesConstantType.Boolean:
                        rbTrue.Checked = value.Trim().ToLower().StartsWith("true");
                        rbFalse.Checked = !rbTrue.Checked;
                        pnlBool.Visible = true;
                        break;
                }
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (txFieldName.Enabled)
            {
                if (Project.ValidateVariableNameCorectness(txFieldName.Text, false) == false)
                {
                    MessageBox.Show("Invalid field name - it must contains letters, numbers and symbol '_' !");
                    txFieldName.Focus();
                    return;
                }
            }
            FieldName = txFieldName.Text;
;
            FieldValue = null;
            string cValue = null;
            if (ConstantHelper.IsNumber(basicType))
                cValue = txNum10.Text;
            if (ConstantHelper.IsFloat(basicType))
                cValue = txFloat.Text;
            switch (basicType)
            {
                case BasicTypesConstantType.String: cValue = txStringValue.Text; break;
                case BasicTypesConstantType.Color: cValue = txColorDecimal.Text; break;
                case BasicTypesConstantType.Boolean: cValue = rbTrue.Checked.ToString(); break;
            }
            if (cValue == null)
            {
                MessageBox.Show("Internal error - don't know how to process type: " + basicType.ToString());
                return;
            }

            string error = "";
            FieldValue = prj.ValidateValueForType(cValue, basicType.ToString(),out error,null,null,false);
            // artrebui daca e null sa verific si pentru enum-uri
            if (FieldValue == null)
            {
                MessageBox.Show(error);
                return;
            }
            FieldDescription = txDescription.Text.Trim();
            // all is ok
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }



        #region Color Type
        static int[,] colIndexes = new int[19,3] {
                {2,0,0},{2,0,1},{2,0,2},{2,1,2},{2,1,0},{2,1,1}, 
                {2,2,0},{2,2,1},{1,2,1},{1,2,0},{0,2,0},{0,2,1}, 
                {1,2,2},{0,2,2},{1,1,2},{0,1,2},{1,0,2},{0,0,2},
                {2,2,2}
        };
        private void OnPaintPallete(object sender, PaintEventArgs e)
        {
            int cellWidth = 11;
            int cellHeight = 11;
            int red,green,blue,proc;

            for (int x = 0; x < 19; x++)
            {
                red = colIndexes[x,0];
                green = colIndexes[x,1];
                blue = colIndexes[x,2];
                for (int y=0;y<16;y++)
                {
                    proc = y * 255 / 15;
                    br.Color = Color.FromArgb(red * proc / 2,green * proc / 2, blue * proc / 2);
                    e.Graphics.FillRectangle(br, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                }
            }

        }
        private void OnSelectColorFromPallete(object sender, MouseEventArgs e)
        {
            int cellWidth = 11;
            int cellHeight = 11;
            int x = e.X / cellWidth;
            int y = e.Y / cellHeight;
            if (x > 18)
                x = 18;
            if (y > 15)
                y = 15;
            uint red = (uint)colIndexes[x, 0];
            uint green = (uint)colIndexes[x, 1];
            uint blue = (uint)colIndexes[x, 2];
            uint proc = (uint)(y * 255 / 15);
            SetColor((uint)(((uint)0xFF << 24) | ((red * proc / 2) << 16) | ((green * proc / 2) << 8) | (blue * proc / 2)));
        }
        private void OnPaintColor(object sender, PaintEventArgs e)
        {
            int w = pnlPreviewColor.Width;
            int h = pnlPreviewColor.Height;
            int sz = 10;
            for (int x = 0; x <= (w / sz); x++)
            {
                for (int y = 0; y <= (h / sz); y++)
                {
                    if (((x + y) % 2) == 0)
                        e.Graphics.FillRectangle(Brushes.White, x * sz, y * sz, sz, sz);
                    else
                        e.Graphics.FillRectangle(Brushes.Black, x * sz, y * sz, sz, sz);
                }
            }
            br.Color = Color.FromArgb((int)nmAlpha.Value, (int)nmRed.Value, (int)nmGreen.Value, (int)nmBlue.Value);
            e.Graphics.FillRectangle(br, sz * 3 / 2, sz * 3 / 2, w - sz * 3, h - sz * 3);
        }

        private void OnChangeColorDecimal(object sender, EventArgs e)
        {
            uint res = 0;
            if (UInt32.TryParse(txColorDecimal.Text,out res) == false)
                res = 0;
            SetColor(res);
        }

        private void OnChangeColorHexa(object sender, EventArgs e)
        {
            uint res = 0;
            if (UInt32.TryParse(txColorHexa.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res) == false)
                res = 0;
            SetColor(res);
        }
        private void SetColor(uint color)
        {
            txColorDecimal.Text = color.ToString();
            txColorDecimal.Select(txColorDecimal.Text.Length, 0);
            txColorHexa.Text = color.ToString("X");
            txColorHexa.Select(txColorHexa.Text.Length, 0);
            nmAlpha.Value = color >> 24;
            nmRed.Value = (color >> 16) & 0xFF;
            nmGreen.Value = (color >> 8) & 0xFF;
            nmBlue.Value = color & 0xFF;
            pnlPreviewColor.Invalidate();
        }
        private void OnColorChannelChanged(object sender, EventArgs e)
        {
            SetColor((((uint)nmAlpha.Value) << 24) | (((uint)nmRed.Value) << 16) | (((uint)nmGreen.Value) << 8) | (((uint)nmBlue.Value) << 0));
        }        
        #endregion

        #region Float Values

        private void OnChangeValueFloat(object sender, EventArgs e)
        {
            float f = 0;
            double d = 0;
            if (basicType == BasicTypesConstantType.Float32)
            {
                if (float.TryParse(txFloat.Text, out f) == false)
                    txFloat.Text = "0.0";
            }
            if (basicType == BasicTypesConstantType.Float64)
            {
                if (double.TryParse(txFloat.Text, out d) == false)
                    txFloat.Text = "0.0";
            }
        }

        #endregion

        #region Decimal Type

        private Int64 AlignToIntervalI64(Int64 value,Int64 min,Int64 max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        private UInt64 AlignToIntervalUI64(UInt64 value, UInt64 min, UInt64 max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        private void SetValueI64(Int64 i64)
        {
            switch (basicType)
            {
                case BasicTypesConstantType.Int8: i64 = AlignToIntervalI64(i64, -128, 127); break;
                case BasicTypesConstantType.Int16: i64 = AlignToIntervalI64(i64, Int16.MinValue, Int16.MaxValue); break;
                case BasicTypesConstantType.Int32: i64 = AlignToIntervalI64(i64, Int32.MinValue, Int32.MaxValue); break;
                case BasicTypesConstantType.Int64: break;
                default: i64 = 0; break;
            }
            txNum10.Text = i64.ToString();
            txNum10.Select(txNum10.Text.Length, 0);
            //txNum16.Text = s;
            //txNum16.Select(txNum16.Text.Length, 0);
            txNum16.Enabled = false;
            txNum16.Text = "";
        }
        private void SetValueUI64(UInt64 u64)
        {
            switch (basicType)
            {
                case BasicTypesConstantType.Int8: u64 = AlignToIntervalUI64(u64, 0, 127); break;
                case BasicTypesConstantType.UInt8: u64 = AlignToIntervalUI64(u64, 0, 255); break;
                case BasicTypesConstantType.Int16: u64 = AlignToIntervalUI64(u64, 0, (UInt64)Int16.MaxValue); break;
                case BasicTypesConstantType.UInt16: u64 = AlignToIntervalUI64(u64, 0, UInt16.MaxValue); break;
                case BasicTypesConstantType.Int32: u64 = AlignToIntervalUI64(u64, 0, (UInt64)Int32.MaxValue); break;
                case BasicTypesConstantType.UInt32: u64 = AlignToIntervalUI64(u64, 0, UInt32.MaxValue); break;
                case BasicTypesConstantType.Int64: u64 = AlignToIntervalUI64(u64, 0, (UInt64)Int64.MaxValue); break;
                case BasicTypesConstantType.UInt64: u64 = AlignToIntervalUI64(u64, 0, UInt64.MaxValue); break;
                default: u64 = 0; break;
            }
            txNum10.Text = u64.ToString();
            txNum10.Select(txNum10.Text.Length, 0);
            txNum16.Text = u64.ToString("X");
            txNum16.Select(txNum16.Text.Length, 0);
            txNum16.Enabled = true;
        }
        private void OnChangeNumericDecimal(object sender, EventArgs e)
        {
            UInt64 u64;
            Int64 i64;

            if (txNum10.Text == "-")
                return;
            if (txNum10.TextLength == 0)
            {
                txNum16.Text = "";
                return;
            }
            if (txNum10.Text.Trim().StartsWith("-"))
            {
                if (Int64.TryParse(txNum10.Text, out i64) == false)
                    i64 = 0;
                SetValueI64(i64);                
            }
            else
            {
                if (UInt64.TryParse(txNum10.Text, out u64) == false)
                    u64 = 0;
                SetValueUI64(u64);
            }

        }
        private void OnChangeNumericalHexa(object sender, EventArgs e)
        {
            UInt64 u64;

            if (txNum16.TextLength == 0)
            {
                txNum10.Text = "";
                return;
            }
            if (UInt64.TryParse(txNum16.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out u64) == false)
                u64 = 0;
            SetValueUI64(u64);
        }
        #endregion







    }
}
