using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class DataTypeValuesResizeArrayDialog : Form
    {
        ArrayCounter ac = new ArrayCounter();
        public string VariableName = "";
        public int oldArray1, oldArray2;
        public int newArray1, newArray2;
        public bool NewFieldsAreNull = false;

        public DataTypeValuesResizeArrayDialog(Structure s,string currentName)
        {
            InitializeComponent();

            comboArray.SelectedIndex = 0;
            OnChangeArrayType(null, null);

            foreach (StructureValue sv in s.Values)
                if (sv.Name.Length>0)
                    ac.Add(sv.Name, sv.Array1, sv.Array2);
            
            foreach (string varName in ac.Variables)
            {
                comboVariables.Items.Add(varName);
                if (varName.Equals(currentName))
                    comboVariables.SelectedIndex = comboVariables.Items.Count - 1;
            }
            OnChangeVariableName(null, null);
        }

        private void OnOK(object sender, EventArgs e)
        {
            VariableName = GetVariableName();
            if (VariableName.Length==0)
            {
                MessageBox.Show("Please select a name from the list !");
                comboVariables.Focus();
                return;
            }
            oldArray1 = ac.GetArray1(VariableName);
            oldArray2 = ac.GetArray2(VariableName);
            switch (comboArray.SelectedIndex)
            {
                case 0: newArray1 = newArray2 = -1; break;
                case 1: newArray1 = (int)nmArray1.Value; newArray2 = -1; break;
                case 2: newArray1 = (int)nmArray1.Value; newArray2 = (int)nmArray2.Value; break;
            }
            if ((oldArray1==newArray1) && (oldArray2==newArray2))
            {
                MessageBox.Show("You have not changed the array type or the array size of the current variable. There is nothing to resize !");
                comboArray.Focus();
                return;
            }
            NewFieldsAreNull = cbNull.Checked;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnChangeArrayType(object sender, EventArgs e)
        {
            nmArray1.Enabled = lbX.Enabled = nmArray2.Enabled = false;
            switch (comboArray.SelectedIndex)
            {
                case 1: nmArray1.Enabled = true; break;
                case 2: nmArray1.Enabled = lbX.Enabled = nmArray2.Enabled = true; break;
            }
        }

        private string GetVariableName()
        {
            if (comboVariables.SelectedIndex < 0)
                return "";
            return comboVariables.Items[comboVariables.SelectedIndex].ToString();            
        }

        private void OnChangeVariableName(object sender, EventArgs e)
        {
            string name = GetVariableName();
            if (name.Length==0)
            {
                comboArray.Enabled = false;
                cbNull.Enabled = false;
                return;
            }
            comboArray.Enabled = true;
            cbNull.Enabled = true;
            int a1 = ac.GetArray1(name);
            int a2 = ac.GetArray2(name);
            if ((a1>0) && (a2>0))
            {
                nmArray1.Value = a1;
                nmArray2.Value = a2;
                comboArray.SelectedIndex = 2;
            } else
            if ((a1>0) && (a2<=0))
            {
                nmArray1.Value = a1;
                nmArray2.Value = 1;
                comboArray.SelectedIndex = 1;
            }
            else
            {
                nmArray1.Value = 1;
                nmArray2.Value = 1;
                comboArray.SelectedIndex = 0;
            }
        }
    }
}
