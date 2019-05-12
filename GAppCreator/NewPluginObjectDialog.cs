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
    public partial class NewPluginObjectDialog : Form
    {
        Project prj;
        PluginList Plugins;
        public List<string> PluginSourceNames = new List<string>();
        public ResourcePluginData Plugin = null;
        List<string> extensions = new List<string>();
        public NewPluginObjectDialog(Project p,PluginList pl)
        {
            prj = p;
            Plugins = pl;
            InitializeComponent();
            foreach (string ext in Plugins.Plugins.Keys)
            {
                comboPlugins.Items.Add(Plugins.Plugins[ext].Interface.GetResourceTypeDescription() + " (" + ext + ")");
                extensions.Add(ext.ToLower());
            }
            comboArrayType.SelectedIndex = 0;
            comboPlugins.SelectedIndex = 0;
            comboLanguage.Items.Add("<None>");
            foreach (string lang in Enum.GetNames(typeof(Language)))
                comboLanguage.Items.Add(lang);
            comboLanguage.SelectedIndex = 0;
        }

        private void comboArrayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            array1.Enabled = comboArrayType.SelectedIndex > 0;
            array1End.Enabled = comboArrayType.SelectedIndex > 0;
            lbArray1From.Enabled = comboArrayType.SelectedIndex > 0;
            lbArray1To.Enabled = comboArrayType.SelectedIndex > 0;
            array2.Enabled = comboArrayType.SelectedIndex > 1;
            array2End.Enabled = comboArrayType.SelectedIndex > 1;
            lbArray2From.Enabled = comboArrayType.SelectedIndex > 1;
            lbArray2To.Enabled = comboArrayType.SelectedIndex > 1;
        }
        private string BuildName(int ar_1,int ar_2)
        {
            string name = txVariableName.Text;
            if (ar_1>=0)
                name += "_[A_" + ar_1.ToString() + "]";
            if (ar_2>=0)
                name += "_[A2_" + ar_2.ToString() + "]";
            if (comboLanguage.SelectedItem.ToString().StartsWith("<") == false)
                name += "_[Lang_" + comboLanguage.SelectedItem.ToString() + "]";
            name += "." + extensions[comboPlugins.SelectedIndex];
            return name;
        }
        private void OnOK(object sender, EventArgs e)
        {
            if (txVariableName.Text.Length == 0)
            {
                MessageBox.Show("Please select a variable name !");
                txVariableName.Focus();
                return;
            }
            if (Project.ValidateVariableNameCorectness(txVariableName.Text.Trim()) == false)
            {
                MessageBox.Show("Invalid resrouce name - use only the following characters: [A-Z, a-z, 0-9 and _].\nResource name always starts with a capital letter !");
                txVariableName.Focus();
                return;
            }
            PluginSourceNames.Clear();
            if ((array1.Enabled == false) && (array2.Enabled == false))
                PluginSourceNames.Add(BuildName(-1, -1));
            if ((array1.Enabled == true) && (array2.Enabled == false))
            {
                for (int tr=(int)array1.Value;tr<=(int)array1End.Value;tr++)
                    PluginSourceNames.Add(BuildName(tr, -1));
            }
            if ((array1.Enabled == true) && (array2.Enabled == true))
            {
                for (int tr = (int)array1.Value; tr <= (int)array1End.Value; tr++)
                    for (int gr = (int)array2.Value; gr <= (int)array2End.Value; gr++)
                        PluginSourceNames.Add(BuildName(tr, gr));
            }
            Plugin = Plugins.Plugins[extensions[comboPlugins.SelectedIndex]];
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void array1_ValueChanged(object sender, EventArgs e)
        {
            array1End.Minimum = array1.Value;
        }

        private void array2_ValueChanged(object sender, EventArgs e)
        {
            array2End.Minimum = array2.Value;
        }
    }
}
