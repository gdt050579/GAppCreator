using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class DebugParamControl : UserControl
    {
        DebugCommandParam param;
        Dictionary<string, sbyte> enums = new Dictionary<string, sbyte>();

        public DebugParamControl()
        {
            InitializeComponent();
            param = null;
            Clear();
        }
        public void Clear()
        {
            lbDescription.Visible = false;
            lbName.Visible = false;
            txValue.Visible = false;
            comboEnum.Visible = false;
            btnColor.Visible = false;
        }
        public void SetColor(Color c)
        {
            btnColor.BackColor = c;
            Color fore = Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
            btnColor.ForeColor = fore;
            btnColor.Text = "Col:"+c.A.ToString() + "," + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString();
        }
        private bool ShowParamConvertError()
        {
            MessageBox.Show("Value '" + txValue.Text + "' is not a valid " + param.Type.ToString() + " value. Check if the number is within type required limits !");
            return false;
        }
        public bool AddValue(List<byte> result)
        {
            // daca nu sunt activ - nu adaug nimik
            if (lbName.Visible == false)
                return true; 
            switch (param.Type)
            {
                case DebugCommandParamType.Boolean:
                    result.Add((byte)comboEnum.SelectedIndex);
                    break;
                case DebugCommandParamType.Int8:
                    sbyte int8Value = 0;
                    if (sbyte.TryParse(txValue.Text, out int8Value)==false)
                        return ShowParamConvertError();
                    result.Add((byte)int8Value);
                    break;
                case DebugCommandParamType.Int16:
                    short int16Value = 0;
                    if (short.TryParse(txValue.Text, out int16Value) == false)
                        return ShowParamConvertError();
                    result.AddRange(BitConverter.GetBytes(int16Value));
                    break;
                case DebugCommandParamType.Int32:
                    int int32Value = 0;
                    if (int.TryParse(txValue.Text, out int32Value) == false)
                        return ShowParamConvertError();
                    result.AddRange(BitConverter.GetBytes(int32Value));
                    break;
                case DebugCommandParamType.UInt8:
                    byte uint8Value = 0;
                    if (byte.TryParse(txValue.Text, out uint8Value) == false)
                        return ShowParamConvertError();
                    result.Add((byte)uint8Value);
                    break;
                case DebugCommandParamType.UInt16:
                    ushort uint16Value = 0;
                    if (ushort.TryParse(txValue.Text, out uint16Value) == false)
                        return ShowParamConvertError();
                    result.AddRange(BitConverter.GetBytes(uint16Value));
                    break;
                case DebugCommandParamType.UInt32:
                    uint uint32Value = 0;
                    if (uint.TryParse(txValue.Text, out uint32Value) == false)
                        return ShowParamConvertError();
                    result.AddRange(BitConverter.GetBytes(uint32Value));
                    break;
                case DebugCommandParamType.Float32:
                    float floatValue = 0;
                    if (float.TryParse(txValue.Text, out floatValue) == false)
                        return ShowParamConvertError();
                    result.AddRange(BitConverter.GetBytes(floatValue));
                    break;
                case DebugCommandParamType.Color:                    
                    result.Add(btnColor.BackColor.B);
                    result.Add(btnColor.BackColor.G);
                    result.Add(btnColor.BackColor.R);
                    result.Add(btnColor.BackColor.A);
                    break;
                case DebugCommandParamType.Enum:
                    result.Add((byte)enums[comboEnum.Items[comboEnum.SelectedIndex].ToString()]);
                    break;
                default:
                    MessageBox.Show("Unkwno type: " + param.Type.ToString());
                    return false;
            }
            return true;
        }
        public void SetParam(DebugCommandParam parameter)
        {
            param = parameter;
            Clear();
            if (param!=null)
            {
                lbDescription.Visible = true;
                lbDescription.Text = param.Description;
                lbName.Visible = true;
                lbName.Text = param.Name;
                switch (param.Type)
                {
                    case DebugCommandParamType.Int8:
                    case DebugCommandParamType.UInt8:
                    case DebugCommandParamType.Int16:
                    case DebugCommandParamType.UInt16:
                    case DebugCommandParamType.Int32:
                    case DebugCommandParamType.UInt32:
                    case DebugCommandParamType.Float32:
                        txValue.Visible = true;
                        txValue.Text = "0";
                        break;
                    case DebugCommandParamType.Color:
                        btnColor.Visible = true;
                        SetColor(Color.White);
                        break;

                    case DebugCommandParamType.Boolean:
                        comboEnum.Items.Clear();
                        comboEnum.Sorted = false;
                        comboEnum.Items.Add("False");
                        comboEnum.Items.Add("True");
                        comboEnum.SelectedIndex = 0;
                        comboEnum.Visible = true;
                        break;
                    case DebugCommandParamType.Enum:
                        string err = "";
                        enums.Clear();
                        if (Project.StringEnumListToDict(param.EnumValues, enums, out err)==false)
                        {
                            MessageBox.Show("Invalid enum values for: " + param.Name + "\r\n" + err);
                            Clear();
                            return;
                        }
                        comboEnum.Items.Clear();
                        comboEnum.Sorted = true;
                        foreach (string k in enums.Keys)
                            comboEnum.Items.Add(k);
                        comboEnum.SelectedIndex = 0;
                        comboEnum.Visible = true;
                        break;
                    default:
                        Clear();
                        break;
                }
            }
        }

        private void OnChangeColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SetColor(dlg.Color);
            }
        }
    }
}
