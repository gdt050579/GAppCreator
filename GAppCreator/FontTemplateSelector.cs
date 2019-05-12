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
    public partial class FontTemplateSelector : Form
    {
        public SVGTemplateList.TemplateInfo ResultedTemplate = null;
        public FontTemplateSelector(Project prj)
        {
            InitializeComponent();
            Templates.AddFromFolder(prj.GetProjectFontTemplatesFolder(), "Custom");
            Templates.AddFromFolder(Project.GetResourceFullPath("Fonts", ""), "Default");
            
        }

        private void OnOK(object sender, EventArgs e)
        {
            ResultedTemplate = Templates.GetSelectedTemplate();
            if (ResultedTemplate==null)
            {
                MessageBox.Show("Please select a template !");
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Templates_OnDoubleClicked(object sender, EventArgs e)
        {
            if (Templates.IsTemplateSelected())
                OnOK(null, null);
        }

        private void Templates_OnTemplateSelected(object sender, EventArgs e)
        {
            btnOK.Enabled = Templates.IsTemplateSelected();
        }
        public SVGTemplateList.TemplateInfo GetTemplateInfo(string name)
        {
            return Templates.GetTemplateByName(name);
        }
    }
}
