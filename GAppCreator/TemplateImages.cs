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
    public partial class TemplateImages : Form
    {
        public string SVGName;
        public string TemplatePath;
        public TemplateImages(bool onlySelect)
        {
            InitializeComponent();
            string[] dirs = Directory.GetDirectories(Project.GetResourceFullPath("Images", ""));
            foreach (string dir in dirs)
            {
                comboCategories.Items.Add(Path.GetFileName(dir));
            }
            comboCategories.Sorted = true;
            if (comboCategories.Items.Count>0)
                comboCategories.SelectedIndex = 0;
            if (onlySelect)
            {
                txImageName.Visible = false;
                lbImageName.Visible = false;
            }
        }

        private void OnSelectImage(object sender, EventArgs e)
        {
            if ((txImageName.Text.Length==0) && (txImageName.Visible))
            {
                MessageBox.Show("Please select a name for the new SVG file !");
                txImageName.Focus();
                return;
            }
            if (Templates.GetSelectedPath().Length==0)
            {
                MessageBox.Show("Please select an image from the available templates !");
                return;
            }
            TemplatePath = Templates.GetSelectedPath();
            SVGName = txImageName.Text + ".svg";
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnChangeCategory(object sender, EventArgs e)
        {
            Templates.Clear();
            if (comboCategories.SelectedIndex>=0)
                Templates.AddFromFolder(Project.GetResourceFullPath("Images",comboCategories.SelectedItem.ToString()));
        }

        private void OnRefreshImages(object sender, EventArgs e)
        {
            OnChangeCategory(null, null);
        }
    }
}
