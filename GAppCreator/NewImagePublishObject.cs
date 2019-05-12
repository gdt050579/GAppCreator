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
    public partial class NewImagePublishObject : Form
    {
        Image LocalImage = null;
        Project prj;
        int i_width, i_height;
        static int scopeIndex = -1, languageIndex = -1;

        public ImagePublish Result=null;

        public NewImagePublishObject(Image img,Project p)
        {
            InitializeComponent();
            prj = p;
            comboExportType.SelectedIndex = 1;
            LocalImage = img;
            if (LocalImage!=null)
            {
                comboSource.Items.Add("Local snapshot system");
                comboSource.SelectedIndex = 0;
                this.TopMost = true;
            }
            else
            {
                comboSource.Items.Add("Import from a file (*.png, *.svg)");
                comboSource.Items.Add("Create from clipboard");
                if (Clipboard.ContainsImage())
                {
                    comboSource.SelectedIndex = 1;
                }
                else
                {
                    comboSource.SelectedIndex = 0;
                }
                // icon-urile
                comboSource.Items.Add("Project icon");
                // icon-urile specifice
                for (int tr=1;tr<prj.BuildConfigurations.Count;tr++)
                {
                    if (prj.BuildConfigurations[tr].Icon.Length>0)
                    {
                        comboSource.Items.Add("Project specific icon (" + prj.BuildConfigurations[tr].Name+")");
                    }
                }
            }

            foreach (string s in Enum.GetNames(typeof(ImagePublishScope)))
                comboScope.Items.Add(s);
            comboScope.SelectedIndex = 0;
            foreach (string s in Enum.GetNames(typeof(ExtendedLanguage)))
            {
                comboLanguage.Items.Add(s);
                if (s == ExtendedLanguage.All.ToString())
                    comboLanguage.SelectedIndex = comboLanguage.Items.Count - 1;
            }
            if (scopeIndex >= 0)
                comboScope.SelectedIndex = scopeIndex;
            if (languageIndex >= 0)
                comboLanguage.SelectedIndex = languageIndex;
        }

        private void comboSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboExportType.Enabled = true;
            if (LocalImage!=null)
            {
                txPath.Enabled = false;
                btnBrowse.Enabled = false;
                txPath.Text = "Snapshot image: " + LocalImage.Width.ToString() + " x " + LocalImage.Height.ToString();
            }
            else
            {
                switch (comboSource.SelectedIndex)
                {
                    case 0: // import 
                        txPath.Enabled = true;
                        btnBrowse.Enabled = true;
                        break;
                    case 1: // clipboard
                        if (Clipboard.ContainsImage())
                        {
                            Image i = Clipboard.GetImage();
                            txPath.Text = "Image: " + i.Width.ToString() + " x " + i.Height.ToString();
                        }
                        else
                        {
                            txPath.Text = "There is no image in the clipboard !";
                        }
                        txPath.Enabled = false;
                        btnBrowse.Enabled = false;
                        break;
                    case 2: // icon
                        txPath.Enabled = false;
                        btnBrowse.Enabled = false;
                        comboExportType.SelectedIndex = 1;
                        comboExportType.Enabled = false;
                        txPath.Text = Path.Combine(prj.GetProjectResourceSourceFolder(), prj.Icon);
                        break;
                    default: // alta iconita
                        string ss = comboSource.Items[comboSource.SelectedIndex].ToString();
                        if (ss.StartsWith("Project specific icon ("))
                        {
                            string bldName = ss.Substring(23, ss.Length - 24);
                            txPath.Enabled = false;
                            btnBrowse.Enabled = false;
                            comboExportType.SelectedIndex = 1;
                            comboExportType.Enabled = false;
                            txPath.Text = Path.Combine(prj.GetProjectResourceSourceFolder(), prj.GetBuild(bldName).Icon);
                        }
                        break;

                }

            }
        }

        private void OnBrowseForFile(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Images files|*.png;*.svg;|All Files|*.*";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txPath.Text = dlg.FileName;
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (txName.Text.Trim().Length==0)
            {
                MessageBox.Show("Please enter a name for this image !");
                txName.Focus();
                return;
            }
            if (comboSource.SelectedIndex == 1) // Iau din clipboard
            {
                if (Clipboard.ContainsImage()==false)
                {
                    MessageBox.Show("Clipboard do not contain an image !");
                    comboSource.Focus();
                    return;
                }
            }
            if ((comboSource.SelectedIndex != 1) && (LocalImage==null))
            {
                if (File.Exists(txPath.Text.ToLower().Trim())==false)
                {
                    MessageBox.Show("File :" + txPath.Text.ToLower().Trim() + " does not exists !");
                    txPath.Focus();
                    return;
                }
            }
            // all is good - creez obiectul
            Result = new ImagePublish();
            if (Result.Create(prj)==false)
            {
                prj.ShowErrors();
                return;
            }
            Result.Name = txName.Text.Trim();
            Result.Scope = (ImagePublishScope)Enum.Parse(typeof(ImagePublishScope), comboScope.SelectedItem.ToString());
            Result.Lang = (ExtendedLanguage)Enum.Parse(typeof(ExtendedLanguage), comboLanguage.SelectedItem.ToString());
            i_width = i_height = -1;
            if (CreateImage(Result.GetObjectFolder(),comboExportType.SelectedIndex==1)==false)
            {
                prj.ShowErrors();
                return;
            }
            if ((i_width>0) && (i_height>0))
            {
                Result.Size = string.Format("{0} x {1}", i_width, i_height);
            }
            string ss = comboSource.Items[comboSource.SelectedIndex].ToString();
            if (ss.StartsWith("Project specific icon ("))
            {
                Result.Builds = ss.Substring(23, ss.Length - 24);
            }

            scopeIndex = comboScope.SelectedIndex;
            languageIndex = comboLanguage.SelectedIndex;
            prj.PublishData.Add(Result);
            prj.PublishData.Sort();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close(); 
        }

        private bool CreateSVGFromPNG(string pngPath,string svgPath)
        {
            string template = "<svg xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"$$WIDTH$$\" height=\"$$HEIGHT$$\"><image x=\"0\" y=\"0\" width=\"$$WIDTH$$\" height=\"$$HEIGHT$$\" xlink:href=\"data:image/png;base64,$$BASE64$$\"/></svg>";
            Image i = Project.LoadImage(pngPath, prj.EC);
            if (i == null)
                return false;
            template = template.Replace("$$WIDTH$$", i.Width.ToString());
            template = template.Replace("$$HEIGHT$$", i.Height.ToString());
            i_width = i.Width;
            i_height = i.Height;
            i = null;
            byte[] data = Disk.ReadFile(pngPath, prj.EC);
            if (data == null)
                return false;
            template = template.Replace("$$BASE64$$", Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks));
            if (Disk.SaveFile(svgPath, template, prj.EC) == false)
                return false;
            return true;
        }
        private bool CreateImage(string path,bool exportAsSVG)
        {
            if (LocalImage!=null)
            {
                if (Disk.SaveImage(Path.Combine(path, "image.png"), LocalImage, prj.EC) == false)
                    return false;
                if (exportAsSVG)
                {
                    if (CreateSVGFromPNG(Path.Combine(path, "image.png"), Path.Combine(path, "source.svg")) == false)
                        return false;
                }
                return true;
            }
            if (comboSource.SelectedIndex==1) // din clipboard
            {
                Image i = Clipboard.GetImage();
                if (i==null)
                {
                    prj.EC.AddError("Unable to retrieve image from clipboard !");
                    return false;
                }
                if (Disk.SaveImage(Path.Combine(path, "image.png"), i, prj.EC) == false)
                    return false;
                if (exportAsSVG)
                {
                    if (CreateSVGFromPNG(Path.Combine(path, "image.png"), Path.Combine(path, "source.svg")) == false)
                        return false;
                }
                return true;
            }
            if (comboSource.SelectedIndex!=1) // dintr-un fisier sau iconita
            {
                string s_path = txPath.Text.ToLower().Trim();
                if (File.Exists(s_path)==false)
                {
                    prj.EC.AddError("File is missing: " + s_path);
                    return false;
                }
                if (s_path.EndsWith(".png"))
                {
                    if (Disk.Copy(s_path, Path.Combine(path, "image.png"), prj.EC) == false)
                        return false;
                    if (exportAsSVG)
                    {
                        if (CreateSVGFromPNG(Path.Combine(path, "image.png"), Path.Combine(path, "source.svg")) == false)
                            return false;
                    }
                    return true;
                }
                if (s_path.EndsWith(".svg"))
                {
                    if (Disk.Copy(s_path, Path.Combine(path, "source.svg"), prj.EC) == false)
                        return false;
                    return true;
                }
                prj.EC.AddError("Unknown file type: "+s_path+" - Expecting SVG or PNG !");
                return false;
            }
            prj.EC.AddError("Internbal error - unknown source !");
            return false;
        }
    }
}
