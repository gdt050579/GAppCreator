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
    public partial class StringTemplateSelector : Form
    {
        Project prj;
        public string SelectedTemplate = "";
        public StringTemplateSelector(Project _prj,string currentValue)
        {
            InitializeComponent();
            prj = _prj;

            for (int tr=0;tr<prj.StringTemplates.Count;tr++)
            {
                ListViewItem lvi = new ListViewItem(prj.StringTemplates[tr].Name);
                lst.Items.Add(lvi);
                if (lvi.Text == currentValue)
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                }
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            SelectedTemplate = "";
            if (lst.SelectedItems.Count > 0)
                SelectedTemplate = lst.SelectedItems[0].Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnDoubleClicked(object sender, EventArgs e)
        {
            OnOK(null, null);
        }

        private void OnNoTemplate(object sender, EventArgs e)
        {
            SelectedTemplate = "";
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
