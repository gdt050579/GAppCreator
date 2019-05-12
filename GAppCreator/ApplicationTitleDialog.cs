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
    public partial class ApplicationTitleDialog : Form
    {
        Project prj;
        GenericBuildConfiguration bld;
        public ApplicationTitleDialog(Project p, GenericBuildConfiguration b, Language l)
        {
            prj = p;
            bld = b;
            InitializeComponent();
            foreach (Language e in Enum.GetValues(typeof(Language)))
            {
                comboLanguage.Items.Add(e.ToString());
                if (e == l)
                    comboLanguage.SelectedIndex = comboLanguage.Items.Count-1;
            }
            lbBuildTitle.Text += " (" + bld.Name + ")";
            if (b.GetType() == typeof(DevelopBuildConfiguration))
            {
                txBuild.ReadOnly = true;
                txBuild.Text = "Default mode always uses project settings !";
                btnBldToPrj.Enabled = false;
                btnPrjToBld.Enabled = false;
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (txDefault.Text.Trim().Length==0)
            {
                MessageBox.Show("Invalid title for the project !");
                txDefault.Focus();
                return;
            }
            Language l = (Language)Enum.Parse(typeof(Language), comboLanguage.SelectedItem.ToString());
            prj.ApplicationName.Set(l, txDefault.Text.Trim());
            if (txBuild.ReadOnly == false)
            {
                if (txBuild.Text.Trim().Length == 0)
                    bld.ApplicationName.Delete(l);
                else
                    bld.ApplicationName.Set(l, txBuild.Text.Trim());
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnChangeLanguage(object sender, EventArgs e)
        {
            txDefault.Text = prj.ApplicationName.Get((Language)Enum.Parse(typeof(Language), comboLanguage.SelectedItem.ToString()));
            txBuild.Text = bld.ApplicationName.Get((Language)Enum.Parse(typeof(Language), comboLanguage.SelectedItem.ToString()));
        }

        private void OnCopyFromProject(object sender, EventArgs e)
        {
            txBuild.Text = txDefault.Text;
        }

        private void OnCopyFromBuild(object sender, EventArgs e)
        {
            txDefault.Text = txBuild.Text;
        }
    }
}
