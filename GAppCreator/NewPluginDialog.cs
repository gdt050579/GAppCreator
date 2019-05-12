using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class NewPluginDialog : Form
    {
        public string PluginName;
        public string PluginDescription;
        public string PluginExtension;

        public NewPluginDialog()
        {
            InitializeComponent();
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (Project.ValidateVariableNameCorectness(txPluginName.Text.Trim())==false)
            {
                MessageBox.Show("Invalid plugin name - use only the following characters: [A-Z, a-z, 0-9 and '_']. The first letter must be a capital letter !");
                txPluginName.Focus();
                return;
            }
            if (txDescription.Text.Trim().Length==0)
            {
                MessageBox.Show("Enter a description for the plugin");
                txDescription.Focus();
                return;
            }
            bool invalidExtension = (txExtension.Text.Trim().Length==0);
            foreach (char ch in txExtension.Text)
            {
                if ((ch >= 'a') && (ch <= 'z'))
                    continue;
                invalidExtension = true;
            }
            if (txExtension.Text.Trim().Length == 0)
                invalidExtension = true;
            if (invalidExtension)
            {
                MessageBox.Show("Extension should be form only out of small letters !");
                txExtension.Focus();
                return;
            }
            PluginName = txPluginName.Text.Trim();
            PluginDescription = txDescription.Text.Trim();
            PluginExtension = txExtension.Text.Trim();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
