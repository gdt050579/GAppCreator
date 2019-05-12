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
    public partial class DataStructureEditDialog : Form
    {
        Structure datatypeObject;
        Project prj;

        public DataStructureEditDialog(Project _prj, Structure _datatypeObject)
        {
            InitializeComponent();
            prj = _prj;
            datatypeObject = _datatypeObject;

            if (datatypeObject!=null)
            {
                txName.Text = datatypeObject.Name;
                txDescription.Text = datatypeObject.Description;
            }

        }

        private void OnOK(object sender, EventArgs e)
        {
            if (Project.ValidateVariableNameCorectness(txName.Text, false) == false)
            {
                MessageBox.Show("Invalid enumeration name - it should contains only letters (a-z or A-Z) or numbers (0-9) !");
                txName.Focus();
                return;
            }
            foreach (Structure dtObj in prj.Structs)
                if ((dtObj != datatypeObject) && (dtObj.Name.ToLower().Equals(txName.Text.ToLower())))
                {
                    MessageBox.Show("Data type '" + txName.Text + "' already exists !");
                    txName.Focus();
                    return;
                }
            if (datatypeObject==null)
            {
                datatypeObject = new Structure();
                prj.Structs.Add(datatypeObject);
            }
            // all is ok
            datatypeObject.Description = txDescription.Text.Trim();
            datatypeObject.Name = txName.Text;
            prj.Structs.Sort();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
