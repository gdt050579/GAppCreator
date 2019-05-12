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
    public partial class PackageCreateDialog : Form
    {
        Project prj;
        public PackageCreateDialog(Project p)
        {
            prj = p;
            InitializeComponent();
            AddContentItem("Project information (include strings, links to sources, resources, plugins, builds, ...)", ZipContentType.ProjectXML);
            AddContentItem("GAC Sources (GAC language sources)", ZipContentType.GACSources);
            AddContentItem("Strings (Language specific translations)", ZipContentType.Strings);
            AddContentItem("Resources (SVG Images, MP3, Font SVG Images, Shaders, etc)", ZipContentType.SourceResources);
            AddContentItem("Build resources (raw images, sound, binary raw data, etc)", ZipContentType.OutputResources);
            AddContentItem("Icons and Splash screen", ZipContentType.IconsAndSplashScreen);
            AddContentItem("Font templates (localy generated templates used for font generation)", ZipContentType.FontTemplates);
            AddContentItem("Plugins (generated plugins (.dll) for raw data edit)", ZipContentType.Plugins);
            AddContentItem("Plugin Sources (C# sources for each plugin)", ZipContentType.PluginsSources);
            AddContentItem("Binaries (executables packages)", ZipContentType.Binaries);
            AddContentItem("Publish materials (images, texts, video, etc)", ZipContentType.PublishMaterials);
            AddContentItem("Advertisments (ads setups)", ZipContentType.Advertisments);
            AddContentItem("Snapshots for application settings", ZipContentType.SystemSettingSnapshots);
            comboContent.SelectedIndex = 2;
            comboMethod.SelectedIndex = 0;
        }
        private void AddContentItem(string text, ZipContentType id)
        {
            ListViewItem lvi = new ListViewItem(text);
            lvi.Tag = id;
            lstContentItems.Items.Add(lvi);
        }

        private void OnSelectArchiveName(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Zip files|*.zip|All files|*.*";
            dlg.AddExtension = true;
            dlg.DefaultExt = "zip";
            DateTime dt = DateTime.Now;
            dlg.FileName = prj.GetProjectName() + "_" + string.Format("{0}-{1:D2}-{2:D2}---{3:D2}-{4:D2}-{5:D2}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second) + ".zip";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txArchiveName.Text = dlg.FileName;
            }
        }
        private void Check(ZipContentType ids)
        {
            foreach (ListViewItem lvi in lstContentItems.Items)
            {
                ZipContentType ct = (ZipContentType)lvi.Tag;
                lvi.Checked = ((ids & ct) == ct);
            }
        }
        private void OnChangeContentType(object sender, EventArgs e)
        {
            /*
             * 0 Only project data
             * 1 Project and Resources (no code)
             * 2 Backup (Project, Resources, Templates, Plugins and Code)
             * 3 Extende Backup (Project, Resources, Templates, Plugins, Code and Publisher materials)
             * 4 Marketing - Andra
             * 5 Full
             * 6 Custom
             */
            cbGacProjectSettings.Checked = false;
            switch (comboContent.SelectedIndex)
            {
                case 0: Check(ZipContentType.ProjectXML | ZipContentType.Advertisments | ZipContentType.Strings); break;
                case 1: Check(ZipContentType.ProjectXML | ZipContentType.SourceResources | ZipContentType.Advertisments | ZipContentType.Strings); break;
                case 2: Check(ZipContentType.ProjectXML | ZipContentType.SourceResources | ZipContentType.Advertisments | ZipContentType.Strings | ZipContentType.FontTemplates | ZipContentType.Plugins | ZipContentType.GACSources); break;
                case 3: Check(ZipContentType.ProjectXML | ZipContentType.SourceResources | ZipContentType.Advertisments | ZipContentType.Strings | ZipContentType.FontTemplates | ZipContentType.Plugins | ZipContentType.GACSources | ZipContentType.PublishMaterials); break;
                case 4: Check(ZipContentType.ProjectXML | ZipContentType.Binaries | ZipContentType.IconsAndSplashScreen | ZipContentType.PublishMaterials | ZipContentType.Advertisments | ZipContentType.Strings); cbGacProjectSettings.Checked = true; break;
                case 5: Check(ZipContentType.ProjectXML | ZipContentType.Binaries | ZipContentType.IconsAndSplashScreen); cbGacProjectSettings.Checked = true; break;
                case 6: Check(ZipContentType.ProjectXML | ZipContentType.SourceResources | ZipContentType.FontTemplates | ZipContentType.Plugins | ZipContentType.GACSources | ZipContentType.Binaries | ZipContentType.OutputResources | ZipContentType.PluginsSources | ZipContentType.IconsAndSplashScreen | ZipContentType.PublishMaterials | ZipContentType.Advertisments | ZipContentType.Strings | ZipContentType.SystemSettingSnapshots); break;
                case 7: Check(ZipContentType.None); break;
            }
            lstContentItems.Enabled = (comboContent.SelectedIndex == 7);
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (txArchiveName.Text.Length==0)
            {
                MessageBox.Show("Select a name for the archive !");
                txArchiveName.Focus();
                return;
            }
            if ((txPassword.Enabled) && (txPassword.Text.Length==0))
            {
                MessageBox.Show("Please enter a valid password !");
                txPassword.Focus();
                return;            
            }
            ZipContentType ct = ZipContentType.None;
            foreach (ListViewItem lvi in lstContentItems.Items)
                if (lvi.Checked)
                    ct |= (ZipContentType)lvi.Tag;
            if (ct == ZipContentType.None)
            {
                MessageBox.Show("You have to select at least one item to be added to the archive !");
                lstContentItems.Focus();
                return;
            }
            if (cbGacProjectSettings.Checked)
            {
                ProjectContext.IDEProjectSettings cfg = new ProjectContext.IDEProjectSettings();
                cfg.EnableResourceTab = (ct & ZipContentType.SourceResources) != 0;
                cfg.EnableCodeTab = (ct & ZipContentType.GACSources) != 0;
                cfg.EnableMemoryProfileTab = cfg.EnableResourceTab; // nu are sens sa fie enablat daca nu am codul
                cfg.EnablePublisherTab = (ct & ZipContentType.PublishMaterials) != 0;
                cfg.EnableStringsTab = (ct & ZipContentType.Strings) != 0;
                cfg.EnableAdvertismentTab = (ct & ZipContentType.Advertisments) != 0;
                if (cfg.Save(Path.Combine(prj.ProjectPath, "project.settings"), prj.EC) == false)
                {
                    prj.ShowErrors();
                    return;
                }
            }
            if (prj.ZipProject(txArchiveName.Text, ct, cbGacProjectSettings.Checked) == false)
            {
                Disk.DeleteFile(Path.Combine(prj.ProjectPath, "project.settings"),prj.EC);
                prj.ShowErrors();
            }
            else
            {
                // daca e cazul - fac si 7zip-ul
                string arc7z = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "7z.exe");
                if (txPassword.Enabled)
                {
                    string resultArchive = txArchiveName.Text + ".7z";
                    string param = "a -p\"" + txPassword.Text + "\" -mhe \"" + resultArchive + "\" \"" + txArchiveName.Text + "\"";
                    //MessageBox.Show(param);
                    prj.RunCommand(arc7z, param, "", true, true);
                    if (File.Exists(resultArchive))
                    {
                        if (Disk.DeleteFile(txArchiveName.Text, prj.EC) == false)
                            prj.ShowErrors();
                        MessageBox.Show("7Zip archive created ok !");
                    }
                    else
                    {
                        MessageBox.Show("Unablle to create 7Zip archive. The zip archive was created !");
                    }
                } else
                    MessageBox.Show("ZIP Archive created ok !");
                if (Disk.DeleteFile(Path.Combine(prj.ProjectPath, "project.settings"),prj.EC)==false)
                    prj.ShowErrors();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void OnChangeCompressMethodType(object sender, EventArgs e)
        {
            txPassword.Enabled = (comboMethod.SelectedIndex == 1);
        }
    }
}
