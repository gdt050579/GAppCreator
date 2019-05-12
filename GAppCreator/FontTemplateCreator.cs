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
    public partial class FontTemplateCreator : Form
    {
        Project prj;
        string temp_svg_name = "";
        public FontTemplateCreator(Project p)
        {
            prj = p;
            InitializeComponent();
            temp_svg_name = Path.Combine(prj.ProjectPath, "Temp", "temp_font_svg_" + Environment.TickCount.ToString() + ".svg");
            Templates.AddFromFolder(prj.GetProjectFontTemplatesFolder(), "Custom");
            Templates.AddFromFolder(Project.GetResourceFullPath("Fonts", ""), "Default");
            
            // setez pe default
           SVGTemplateList.TemplateInfo ti = Templates.GetTemplateByName("Normal");
            if (ti!=null)
                SetTemplate(ti.Path);


        }


        private void OnEditCurrentTemplate(object sender, EventArgs e)
        {
            if (CheckExistsAndCreate() == false)
                return;
            if (prj.RunCommand(prj.Settings.InskapePath, temp_svg_name, "Inkscape", true, false) == false)
                prj.ShowErrors();
            else
                PreviewTemplate('A');

        }

        private void OnCopyExistingTemplate(object sender, EventArgs e)
        {
            if (Templates.IsTemplateSelected())
                SetTemplate(Templates.GetSelectedPath());
            else
                MessageBox.Show("Please select a template from the list below first !");
        }

        private void OnPreviewCharacterChanged(object sender, EventArgs e)
        {
            btnPreview.Enabled = (txChar.Text.Length > 0);
            if (txChar.Text.Length>=2)
            {
                txChar.Text = txChar.Text.Substring(txChar.Text.Length-1, 1);
                txChar.SelectAll();
            }
        }
        private bool CheckExistsAndCreate()
        {
            if (File.Exists(temp_svg_name) == false)
            {
                if (Disk.SaveFile(temp_svg_name, "<svg height=\"100\" width=\"100\"><text x=\"0\" y=\"100\">A</text></svg>", prj.EC) == false)
                {
                    prj.ShowErrors();
                    return false;
                }
            }
            return true;
        }
        void SetTemplate(string path)
        {
            if (Disk.Copy(path, temp_svg_name, prj.EC) == false)
            {
                prj.ShowErrors();
                if (Disk.SaveFile(temp_svg_name, "<svg height=\"100\" width=\"100\"><text x=\"0\" y=\"100\">A</text></svg>", prj.EC) == false)
                    prj.ShowErrors();
            }
            else
            {
                pvi.SetPreviewObject(prj, null, (Bitmap)Project.LoadImage(path.Substring(0, path.Length - 4) + ".png"));
                pvi.Refresh();
            }
        }
        private void PreviewTemplate(char ch)
        {
            pvi.Tag = null;
            pvi.Refresh();
            if (CheckExistsAndCreate() == false)
                return;
            string text = Disk.ReadFileAsString(temp_svg_name, prj.EC);
            if (prj.EC.HasErrors())
            {
                prj.ShowErrors();
                return;
            }
            if (text.Contains(">A<") == false)
            {
                MessageBox.Show("Template file should be create for letter 'A'");
                return;
            }
            int index = text.LastIndexOf(">A<");

            text = text.Substring(0, index) + ">&#" + ((int)ch).ToString() + ";<" + text.Substring(index + 3);
            if (Disk.SaveFile(temp_svg_name + ".preview.svg", text, prj.EC) == false)
            {
                prj.ShowErrors();
                return;
            }
            // fac resize la canvas
            if (prj.ResizeSVGToDrawing(temp_svg_name + ".preview.svg", true) == false)
            {
                prj.ShowErrors();
                return;
            }
            // fac imaginea
            if (prj.SVGtoPNG(temp_svg_name + ".preview.svg", temp_svg_name + ".preview.png", 90, -1, -1, 1.0f, true) == false)
            {
                prj.ShowErrors();
                return;
            }
            if (File.Exists(temp_svg_name + ".preview.png"))
            {
                pvi.SetPreviewObject(prj, null, (Bitmap)Project.LoadImage(temp_svg_name + ".preview.png"));
                pvi.Refresh();
                Disk.DeleteFile(temp_svg_name + ".preview.png", null);
            }
            else
            {
                MessageBox.Show("Preview image was not generated (internal error) !");
            }
        }
        private void OnShowPreview(object sender, EventArgs e)
        {
            PreviewTemplate(txChar.Text[0]);
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (txName.Text.Length==0)
            {
                MessageBox.Show("Please specify a name for the template !");
                txName.Focus();
                return;
            }
            txName.Text = txName.Text.Substring(0, 1).ToUpper() + txName.Text.Substring(1);
            if (Project.ValidateVariableNameCorectness(txName.Text)==false)
            {
                MessageBox.Show("Template name is incorect (should contain the following characters 'A'-'Z', 'a'-'z', '0'-'9', '_' and should start with a capital letter !");
                txName.Focus();
                return;
            }
            // verific daca exista
            SVGTemplateList.TemplateInfo ti = Templates.GetTemplateByName(txName.Text);
            if (ti!=null)
            {
                // daca e ceva default nu il las
                if (ti.Group!="Custom")
                {
                    MessageBox.Show("You can not create a template with the same name as a default template: '"+ti.Name+"' !");
                    txName.Focus();
                    return;
                }
                // daca nu intreb daca vrea sa il suprascriu
                if (MessageBox.Show("Template '" + ti.Name + "' already exists. Override ?", "Override", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                {
                    txName.Focus();
                    return;
                }
            }
            // totul e bine - creez template-ul si poza lui 
            string text = Disk.ReadFileAsString(temp_svg_name, prj.EC);
            if (prj.EC.HasErrors())
            {
                prj.ShowErrors();
                return;
            }
            if (text.Contains(">A<") == false)
            {
                MessageBox.Show("Template file should be create for letter 'A'");
                return;
            }
            int index = text.LastIndexOf(">A<");
            // creez si template-ul

            if (prj.ResizeSVGToDrawing(temp_svg_name, true) == false)
            {
                prj.ShowErrors();
                return;
            }
            if (Disk.Copy(temp_svg_name, Path.Combine(prj.GetProjectFontTemplatesFolder(), txName.Text + ".svg"), prj.EC))
            {
                // fac si imaginea
                if (prj.SVGtoPNG(Path.Combine(prj.GetProjectFontTemplatesFolder(), txName.Text + ".svg"), Path.Combine(prj.GetProjectFontTemplatesFolder(), txName.Text + ".png"), -1, 64, 64, 1.0f, true) == false)
                {
                    prj.ShowErrors();
                    return;
                }
            }
            else
            {
                prj.ShowErrors();
                return;
            }


            //fip.SVG = text.Substring(0, index) + ">$$CHAR$$<" + text.Substring(index + 3);
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void OnTemplateDoubleClicked(object sender, EventArgs e)
        {
            OnCopyExistingTemplate(null, null);
        }

        private void OnTemplateSelected(object sender, EventArgs e)
        {
            btnDelete.Enabled = Templates.IsTemplateSelected();

            btnCopyTemplate.Enabled = Templates.IsTemplateSelected();
        }

        private void OnDeleteTemplate(object sender, EventArgs e)
        {
            SVGTemplateList.TemplateInfo ti = Templates.GetSelectedTemplate();
            if (ti==null)
            {
                MessageBox.Show("Please select a template for delete operation !");
                return;
            }
            if (ti.Group!="Custom")
            {
                MessageBox.Show("Only templates that were generated within the current project can be deleted !\nDefault templates can not be deleted !");
                return;
            }
            if (MessageBox.Show("Delete '"+ti.Name+"' tamplate ?","Delete",MessageBoxButtons.YesNo)== System.Windows.Forms.DialogResult.Yes)
            {
                if (ti.Path.Length>0)
                {
                    if (Disk.DeleteFile(ti.Path, prj.EC) == false)
                        prj.ShowErrors();
                }
                if (ti.PicturePath.Length>0)
                {
                    if (Disk.DeleteFile(ti.PicturePath, prj.EC) == false)
                        prj.ShowErrors();
                }
                Templates.RemoveFromList(ti.Name);
                Templates.Invalidate();
            }
        }
    }
}
