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
    public partial class NewBuildDialog : Form
    {
        Project prj;
        public string BuildName = "";
        public string DupFrom = "";
        public bool KeepResources = false;
        private string OSName = "";
        
        public NewBuildDialog(Project p)
        {
            prj = p;
            InitializeComponent();
            foreach (string name in Enum.GetNames(typeof(OSType)))
                comboOS.Items.Add(name);
            comboOS.SelectedIndex = 0;
        }
        public OSType GetOS()
        {
            try
            {
                OSType ot = (OSType)Enum.Parse(typeof(OSType), OSName.ToString());
                return ot;
            }
            catch (Exception)
            {
                return OSType.None;
            }
        }
        private void OnCreateNewBuild(object sender, EventArgs e)
        {
            if (Project.ValidateVariableNameCorectness(txName.Text,false)==false)
            {
                MessageBox.Show("Invalid name (should contain only the characters A-Z, a-z, 0-9 and '_'. !");
                txName.Focus();
                return;
            }
            foreach (GenericBuildConfiguration bld in prj.BuildConfigurations)
                if (bld.Name == txName.Text)
                {
                    MessageBox.Show("A build with the name '" + txName.Text + "' already exists !");
                    txName.Focus();
                    return;
                }
            BuildName = txName.Text;
            OSName = comboOS.SelectedItem.ToString();
            OSType ot = GetOS();
            if (ot == OSType.None)
            {
                MessageBox.Show("Please specify a platform !");
                comboOS.Focus();
                return;
            }
            DupFrom = "";
            if (comboDup.SelectedIndex > 0)
                DupFrom = comboDup.SelectedItem.ToString();
            KeepResources = cbKeepResources.Checked;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnChangePlatform(object sender, EventArgs e)
        {
            OSType ot =(OSType)Enum.Parse(typeof(OSType), comboOS.SelectedItem.ToString());
            comboDup.Items.Clear();
            comboDup.Items.Add("<None>");
            foreach (GenericBuildConfiguration bld in prj.BuildConfigurations)
            {
                if (bld.GetOS() == ot)
                    comboDup.Items.Add(bld.Name);
            }
            comboDup.SelectedIndex = 0;
        }

        private void OnChangeDuplicate(object sender, EventArgs e)
        {
            cbKeepResources.Enabled = (comboDup.SelectedIndex > 0);
        }
    }
}
