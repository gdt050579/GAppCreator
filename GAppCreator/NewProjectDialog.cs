using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GAppCreator
{
    public partial class NewProjectDialog : Form
    {
        private static Size[] sizes = Project.Resolutions;
        public string resXmlFile;
        public NewProjectDialog(List<string> mru)
        {
            InitializeComponent();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            UpdateResolutions(true);
            comboDesignResolution.SelectedIndex = 3; // 800 x 480
            comboOrientation.SelectedIndex = 0;
            // fac fill up la MRU
            
            if (mru!=null)
            {
                Dictionary<string, bool> d = new Dictionary<string, bool>();
                foreach (string prj_path in mru)
                    d[Path.GetDirectoryName(prj_path).ToLower()] = true;
                foreach (string k in d.Keys)
                {
                    txFolder.Items.Add(k);
                }
            }
            // fill up si la language
            foreach (string l in Enum.GetNames(typeof(Language)))
            {
                comboLanguage.Items.Add(l);
                if (l == Language.English.ToString())
                    comboLanguage.SelectedIndex = comboLanguage.Items.Count - 1;
            }
        }
        private void UpdateResolutions(bool landscape)
        {
            comboDesignResolution.Items.Clear();
            foreach (Size s in sizes)
            {
                if (landscape)
                    comboDesignResolution.Items.Add(String.Format("{0} x {1}", s.Width, s.Height));
                else
                    comboDesignResolution.Items.Add(String.Format("{0} x {1}", s.Height, s.Width));
            }
        }
        private void OnSetProjectFolder(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txFolder.Text = dlg.SelectedPath;
            }
        }
        private bool CreateFolder(string name)
        {
            if (Disk.CreateFolder(name, null) == false)
            {
                MessageBox.Show("Unable to create folder: '"+name+"'");
                return false;
            }
            return true;
        }
        private bool CreateFile(string resName, string outputName, Dictionary<string, string> replacements)
        {
            string c = Project.GetResource("GAC",resName, replacements,null);
            if (c.Length == 0)
                return false;
            if (Disk.SaveFile(outputName, c, null) == false)
            {
                MessageBox.Show("Unable to create file: " + outputName);
                return false;
            }
            return true;
        }
        private void OnCreateNewProject(object sender, EventArgs e)
        {
            if (txFolder.Text.Length == 0)
            {
                MessageBox.Show("Please select a folder for the project !");
                txFolder.Focus();
                return;
            }
            if (txProjectName.Text.Length == 0)
            {
                MessageBox.Show("Please select a project name");
                txProjectName.Focus();
                return;                
            }
            string proot = Path.Combine(txFolder.Text,txProjectName.Text);
            string lib_path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Libs");
            if (Directory.Exists(proot))
            {
                MessageBox.Show("Path '"+proot+"' already exists !");
                return;  
            }
            if (CreateFolder(proot) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "Bin")) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "Temp")) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "Sources")) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "CppProject")) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "Resources")) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "Resources","Sources")) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "Resources", "Output")) == false)
                return;
            if (CreateFolder(Path.Combine(proot, "Builds")) == false)
                return;
            // fisiere
            Dictionary<string, string> ds = new Dictionary<string, string>()
            {
                { "$$APLICATION.NAME$$"             , txProjectName.Text },
                { "$$SCENE.NAME$$"                  , "MainScene" }, 
            };
            if (CreateFile("MainScene.gac", Path.Combine(proot, "Sources", "MainScene.gac"), ds) == false)
                return;
            if (CreateFile("MainApplication.gac", Path.Combine(proot, "Sources", "App.gac"), ds) == false)
                return;
            if (Disk.SaveFile(Path.Combine(proot, "Sources", "Global.gac"), "global {\n\t//Add global variables here\n}\n", null) == false)
            {
                MessageBox.Show("Unable to create file 'Global.gac'");
                return;
            }
            // copii si logo si icon-ul

            if (Disk.Copy(Project.GetResourceFullPath("Images","logo.svg"), Project.GetProjectResourceSourceFullPath(proot, "Logo.svg"), null) == false)
            {
                MessageBox.Show("Unable to create application logo image !");
                return;
            }
            if (Disk.Copy(Project.GetResourceFullPath("Images","icon.svg"), Project.GetProjectResourceSourceFullPath(proot, "ApplicationIcon.svg"), null) == false)
            {
                MessageBox.Show("Unable to create application main icon !");
                return;
            }
            /* copii si DLL-urile */            
            Disk.Copy(Path.Combine(lib_path, "GAppFramework.dll"), Path.Combine(proot, "Bin", "GAppFramework.dll"), null);

            Project p = new Project();
            int cIndex = comboDesignResolution.SelectedIndex;
            if (comboOrientation.SelectedIndex==1)
                p.DesignResolution = String.Format("{0} x {1}", sizes[cIndex].Width, sizes[cIndex].Height);
            else
                p.DesignResolution = String.Format("{0} x {1}", sizes[cIndex].Height, sizes[cIndex].Width);
            // limba
            Language lng = (Language)Enum.Parse(typeof(Language), comboLanguage.SelectedItem.ToString());
            p.DefaultLanguage = lng;
            // pun si numele
            p.ApplicationName.Set(lng, txProjectName.Text);
            // creez un profil global
            Profile prf = new Profile();
            prf.Name = "GlobalResources";
            prf.Type = ProfileType.Global;
            p.Profiles.Add(prf);
            // adaug si logo-ul si iconita
            p.Icon = "ApplicationIcon.svg";
            if (p.SplashScreen.Create("Logo.svg", 1.0f, "") == false)
            {
                MessageBox.Show("Fail to load application logo");
                return;
            }
            // adaug si un eveniment care e gata in totdo list
            ProjectTask pt = new ProjectTask();
            pt.Text = "Application '"+txProjectName.Text+"' was created by '"+Environment.UserName+"' !";
            pt.Type = TaskType.ToDo;
            pt.SetNow(true);
            pt.SetNow(false);
            p.Tasks.Add(pt);
            ProjectFile pf;
            p.ProjectPath = proot;

            // pun si un default build
            DevelopBuildConfiguration bld = new DevelopBuildConfiguration();
            bld.Name = "Develop";
            bld.AppResolution = p.DesignResolution;
            p.BuildConfigurations.Add(bld);

            pf = new ProjectFile();
            pf.Name = "App.gac";
            pf.Opened = false;
            p.Files.Add(pf);

            pf = new ProjectFile();
            pf.Name = "MainScene.gac";
            pf.Opened = true;
            p.Files.Add(pf);

            pf = new ProjectFile();
            pf.Name = "Global.gac";
            pf.Opened = false;
            p.Files.Add(pf);            
      
            p.Save();
            resXmlFile = Path.Combine(proot, "project.gappcreator");

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnCancel(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnChangeOrientation(object sender, EventArgs e)
        {
            int cIndex = comboDesignResolution.SelectedIndex;
            UpdateResolutions(comboOrientation.SelectedIndex == 1);
            comboDesignResolution.SelectedIndex = cIndex;
        }
    }
}
