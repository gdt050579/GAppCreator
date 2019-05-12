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
    public partial class EnumEditDialog : Form
    {
        Enumeration enumObject;
        Project prj;

        public EnumEditDialog(Project _prj,Enumeration _enumObject)
        {
            InitializeComponent();

            prj = _prj;
            enumObject = _enumObject;

            foreach (string value in Enum.GetNames(typeof(BasicTypesConstantType)))
                comboType.Items.Add(value);
            comboType.SelectedIndex = 0; // NONE
            
            if (enumObject != null)
            {
                txName.Text = enumObject.Name;
                txDescription.Text = enumObject.Description;
                cbIsBitSet.Checked = enumObject.IsBitSet;
                for (int tr = 0; tr < comboType.Items.Count; tr++)
                    if (comboType.Items[tr].ToString().ToLower().Equals(enumObject.Type.ToString().ToLower()))
                    {
                        comboType.SelectedIndex = tr;
                        break;
                    }
            }
        }
        private void OnOK(object sender, EventArgs e)
        {
            if (Project.ValidateVariableNameCorectness(txName.Text,false)==false)
            {
                MessageBox.Show("Invalid enumeration name - it should contains only letters (a-z or A-Z) or numbers (0-9) !");
                txName.Focus();
                return;
            }
            if (comboType.SelectedIndex<0)
            {
                MessageBox.Show("Please select a type for the enumerations !");
                comboType.Focus();
                return;
            }

            BasicTypesConstantType newType = BasicTypesConstantType.Int32;
            if (Enum.TryParse<BasicTypesConstantType>(comboType.SelectedItem.ToString(),out newType)==false)
            {
                MessageBox.Show("Unknown type !!!");
                comboType.Focus();
                return;
            }
            if (newType == BasicTypesConstantType.None)
            {
                MessageBox.Show("Enumeration type can not be None !");
                comboType.Focus();
                return;
            }
            // verific daca poate sa fie bitset
            if ((cbIsBitSet.Checked) && (ConstantHelper.IsInteger(newType)==false))
            {
                MessageBox.Show("Only integer types can be bit sets (Int8,Int16,Int32,Int64,UInt8,UInt16,UInt32,UInt64) !");
                cbIsBitSet.Focus();
                return;
            }
            // verific si daca nu exista deja

            foreach (Enumeration enm in prj.Enums)
                if ((enm != enumObject) && (enm.Name.ToLower().Equals(txName.Text.ToLower())))
                {
                    MessageBox.Show("Enum '" + txName.Text + "' already exists !");
                    txName.Focus();
                    return;
                }
            if (enumObject!=null)
            {
                if ((ConstantHelper.CanConvertBasicTypeTo(enumObject.Type, newType) == false) && (enumObject.Values.Count > 0))
                {
                    if (MessageBox.Show("Converting from '"+enumObject.Type.ToString()+"' to '"+newType.ToString()+"' will lose data. Changing this enum from one to another will delete all of the current enum value.\nAre you sure ?", "Delete values", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        comboType.Focus();
                        return;
                    }
                    enumObject.Values.Clear();
                }
            }
            else
            {
                enumObject = new Enumeration();
                prj.Enums.Add(enumObject);
            }
            // setez valorile
            enumObject.Name = txName.Text;
            enumObject.Description = txDescription.Text;
            enumObject.Type = newType;
            enumObject.IsBitSet = cbIsBitSet.Checked;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
