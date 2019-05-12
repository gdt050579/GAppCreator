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
    public partial class InputBox : Form
    {
        enum ResultType
        {
            String,
            Float,
            StringArray
        };
        public String StringResult;
        public String[] StringList;
        public float FloatResult;


        ResultType rType;
        float minFloatValue, maxFloatValue;

        public InputBox(String label,String defaultValue)
        {
            InitializeComponent();
            lbInfo.Text = label;
            txValue.Text = defaultValue;
            comboInfo.Visible = false;
            StringResult = "";
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            rType = ResultType.String;
        }
        public InputBox(String label, String[] list,String defaultValue)
        {
            InitializeComponent();
            lbInfo.Text = label;
            txValue.Visible = false;
            comboInfo.Visible = true;
            comboInfo.Left = txValue.Left;
            comboInfo.Top = txValue.Top;
            int index = -1;
            for (int tr=0;tr<list.Length;tr++)
            {
                comboInfo.Items.Add(list[tr]);
                if (list[tr].Equals(defaultValue))
                    index = tr;                    
            }
            if (index >= 0)
                comboInfo.SelectedIndex = index;
            StringResult = "";
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            rType = ResultType.StringArray;
        }
        public InputBox(string label, float defaultValue,float minValue,float maxValue)
        {
            InitializeComponent();
            lbInfo.Text = label;
            txValue.Text = defaultValue.ToString();
            comboInfo.Visible = false;
            FloatResult = defaultValue;
            minFloatValue = minValue;
            maxFloatValue = maxValue;
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            rType = ResultType.Float;
        }
        /*
        public InputBox(Language defaultLang)
        {
            InitializeComponent();
            comboInfo.Left = txValue.Left;
            comboInfo.Top = txValue.Top;
            txValue.Visible = false;
            lbInfo.Text = "Select language :";
            foreach (GavApps.Language l in Enum.GetValues(typeof(GavApps.Language)))
            {
                comboInfo.Items.Add(l);
            }
            ResultLanguage = defaultLang;
        }
         */
        private void bntOk_Click(object sender, EventArgs e)
        {
            switch (rType)
            {
                case ResultType.String:
                    StringResult = txValue.Text;
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    break;
                case ResultType.StringArray:
                    StringResult = comboInfo.SelectedItem.ToString();
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    break;
                case ResultType.Float:
                    if (float.TryParse(txValue.Text, out FloatResult) == false)
                    {
                        MessageBox.Show("Invalid float format: '" + txValue.Text+"'");
                        return;
                    }
                    if ((FloatResult < minFloatValue) || (FloatResult > maxFloatValue))
                    {
                        MessageBox.Show("Value should be between " + minFloatValue.ToString() + " to " + maxFloatValue.ToString());
                        return;
                    }
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
