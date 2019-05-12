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
    public partial class DataTypeValuesAddDialog : Form
    {
        Project prj;
        public string ResultName = "";
        public int Array1, Array2;
        public bool FieldIsNull = false;
        public DataTypeValuesAddDialog(Project _prj)
        {
            InitializeComponent();
            prj = _prj;
            comboArray.SelectedIndex = 0;
            OnArrayModeChanged(null, null);
            txName_TextChanged(null, null);
        }

        private void txName_TextChanged(object sender, EventArgs e)
        {
            if (txName.TextLength==0)
            {
                lbArray.Text = "Quantity";
                comboArray.Visible = nmArray1.Visible = nmArray2.Visible = lbX.Visible = false;
                nmQuantity.Visible = true;
            }
            else
            {
                lbArray.Text = "Array";
                comboArray.Visible = nmArray1.Visible = nmArray2.Visible = lbX.Visible = true;
                nmQuantity.Visible = false;
            }
        }
        private void OnArrayModeChanged(object sender, EventArgs e)
        {
            nmArray1.Enabled = lbX.Enabled = nmArray2.Enabled = false;
            switch (comboArray.SelectedIndex)
            {
                case 1: nmArray1.Enabled = true; break;
                case 2: nmArray1.Enabled = lbX.Enabled = nmArray2.Enabled = true; break;
            }
        }
        private void OnOK(object sender, EventArgs e)
        {
            if (txName.TextLength > 0)
            {
                if (Project.ValidateVariableNameCorectness(txName.Text, false) == false)
                {
                    MessageBox.Show("Invalid constant name - it must contains letters, numbers and symbol '_' !");
                    txName.Focus();
                    return;
                }
                Structure s = prj.GetStructureForVariableName(txName.Text);
                if (s!=null)
                {
                    MessageBox.Show("Constant '"+txName.Text+"' already exists in data type '"+s.Name+"' !");
                    txName.Focus();
                    return;
                }
                // verific si cu proiectul daca nu cumva mai exista campul
                Array1 = -1;
                Array2 = -1;
                ResultName = txName.Text;
                switch (comboArray.SelectedIndex)
                {
                    case 1: Array1 = (int)nmArray1.Value; break;
                    case 2: Array1 = (int)nmArray1.Value; Array2 = (int)nmArray2.Value; break;
                }
            }
            else
            {
                Array1 = (int)nmQuantity.Value;
                ResultName = "";
            }
            FieldIsNull = cbNull.Checked;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }


    }
}
