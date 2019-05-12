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
    public partial class StringAddDialog : Form
    {
        Project prj;
        public string ResultName = "";
        public int Array1, Array2;
        public Dictionary<Language, string> Values = new Dictionary<Language, string>();

        public StringAddDialog(Project _prj)
        {
            InitializeComponent();
            prj = _prj;
            dgValues.Columns[1].Width = 500;
            comboArray.SelectedIndex = 0;

            Dictionary<Language, bool> SelectedLanguages = new Dictionary<Language, bool>();
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                SelectedLanguages[l] = false;
                foreach (StringValues sv in prj.Strings)
                {
                    if (sv.Get(l).Length > 0)
                    {
                        SelectedLanguages[l] = true;
                        break;
                    }
                }
            }
            dgValues.Rows.Add(new string[] { prj.DefaultLanguage.ToString(), "" });
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                if (l == prj.DefaultLanguage)
                    continue;
                if (SelectedLanguages[l] == false)
                    continue;
                dgValues.Rows.Add(new string[] { l.ToString(), "" });
            }
            OnArrayModeChanged(null, null);
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (txName.TextLength == 0)
            {
                MessageBox.Show("Invalid constant name - it must contains letters, numbers and symbol '_' !");
                txName.Focus();
                return;
            }
            
            if (Project.ValidateVariableNameCorectness(txName.Text, false) == false)
            {
                MessageBox.Show("Invalid constant name - it must contains letters, numbers and symbol '_' !");
                txName.Focus();
                return;
            }
            for (int tr = 0; tr < prj.Strings.Count; tr++)
            {
                if (prj.Strings[tr].VariableName.Equals(txName.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show("String '" + txName.Text + "' already exists !");
                    txName.Focus();
                    return;
                }
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
            for (int tr = 0; tr < dgValues.Rows.Count;tr++)
            {
                Language l;
                if (Enum.TryParse(dgValues.Rows[tr].Cells[0].Value.ToString(), out l))
                {
                    Values[l] = dgValues.Rows[tr].Cells[1].Value.ToString();
                }
            }
            
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
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

        private void StringAddDialog_Shown(object sender, EventArgs e)
        {
            txName.Focus();
        }
    }
}
