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
    public partial class NewSceneDialog : Form
    {
        private class TemplateList
        {
            public string TemplateName = "";
            public string TemplateFile = "";
            public TemplateList(string Name, string File) { TemplateName = Name; TemplateFile = File; }
        };
        Project prj;
        public string SceneName = "";
        public string SceneFileName = "";
        TemplateCodeGenerator code = new TemplateCodeGenerator();
        List<TemplateList> Templates = new List<TemplateList>();

        public NewSceneDialog(Project _prj)
        {
            InitializeComponent();
            prj = _prj;
            code.Init(Project.GetResource("GAC", "Scene.gac", null));

            foreach (string itemName in code.GetItems())
                AddFunction(itemName, code.Items[itemName].Use, code.Items[itemName].Description);

            comboOnBack.SelectedIndex = 1;
            comboOnPaint.SelectedIndex = 1;

            foreach (string s in GACParser.StandardScenes.Keys)
                comboStandardScenes.Items.Add(s);
            comboStandardScenes.Sorted = true;

            Templates.Add(new TemplateList("Custom scene", ""));
            Templates.Add(new TemplateList("AdMob Interstitial scene", "Scene_admob_interstitial.gac"));
            Templates.Add(new TemplateList("Exit AdMob Interstitial scene", "Scene_exit_admob_interstitial.gac"));
            Templates.Add(new TemplateList("Comercial scene with two interstitial ads", "Scene_two_insterstitials.gac"));

            foreach (TemplateList tl in Templates)
                comboTemplate.Items.Add(tl.TemplateName);

            rbStandardScenes.Checked = true;
            OnEnableDisableScenes(null, null);

            comboTemplate.SelectedIndex = 0;
            OnSelectTemplate(null, null);

        }

        private void AddFunction(string name,bool isChecked,string description)
        {
            ListViewItem lvi = new ListViewItem(name);
            lvi.SubItems.Add(description);
            lvi.Checked = isChecked;
            lstFunctions.Items.Add(lvi);
        }

        private void OnOK(object sender, EventArgs e)
        {
            string scnName = "";
            if (comboStandardScenes.Enabled)
            {
                if (comboStandardScenes.SelectedIndex >= 0)
                    scnName = comboStandardScenes.SelectedItem.ToString();
            }
            else
            {
                scnName = txCustomScene.Text;
            }
            if (Project.ValidateVariableNameCorectness(scnName)==false)
            {
                MessageBox.Show("Invalid scene name. Must start with a capital letter and contain letters and numbers !");
                return;
            }
            if (comboTemplate.SelectedIndex == 0)
            {
                // pregatesc codul
                foreach (ListViewItem lvi in lstFunctions.Items)
                    code.Items[lvi.Text].Use = lvi.Checked;
                code.Translates["$$SCENE.NAME$$"] = scnName;
                code.Translates["$$SCENE.PAINT$$"] = "";
                if (comboOnPaint.SelectedIndex == 1)
                    code.Translates["$$SCENE.PAINT$$"] = "PaintControls();";
                code.Translates["$$SCENE.ONBACKKEY$$"] = "";
                if (comboOnBack.SelectedIndex == 1)
                    code.Translates["$$SCENE.ONBACKKEY$$"] = "if (keyEvent.IsBackPressed()) Application.ChangeScene(Scenes.Main);";
                if (comboOnBack.SelectedIndex == 2)
                    code.Translates["$$SCENE.ONBACKKEY$$"] = "if (keyEvent.IsBackPressed()) Application.GoToNextScene();";
                if (comboOnBack.SelectedIndex == 3)
                    code.Translates["$$SCENE.ONBACKKEY$$"] = "if (keyEvent.IsBackPressed()) Application.Close();";
            }
            else
            {
                code.Init(Project.GetResource("GAC", Templates[comboTemplate.SelectedIndex].TemplateFile, null));
                code.Translates["$$SCENE.NAME$$"] = scnName;
            }
            SceneFileName = scnName;
            if (SceneFileName.ToLower().EndsWith("scene") == false)
                SceneFileName += "Scene";
            SceneFileName += ".gac";
            string gacFile = Path.Combine(prj.ProjectPath, "Sources", SceneFileName);
            if (File.Exists(gacFile))
            {
                MessageBox.Show("File " + gacFile + " already exists !");
                return;
            }
            if (Disk.SaveFile(gacFile, code.CreateCode(), null) == false)
            {
                MessageBox.Show("Unable to create file: " + gacFile);
                return;
            }
            // totul e ok - returnez
            SceneName = scnName;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void OnEnableDisableScenes(object sender, EventArgs e)
        {
            comboStandardScenes.Enabled = rbStandardScenes.Checked;
            txCustomScene.Enabled = rbCustomScenes.Checked;
        }

        private void OnSelectTemplate(object sender, EventArgs e)
        {
            bool enabled = comboTemplate.SelectedIndex==0;
            lstFunctions.Enabled = enabled;
            lbOnBackKey.Enabled = enabled;
            lbOnPaint.Enabled = enabled;
            comboOnBack.Enabled = enabled;
            comboOnPaint.Enabled = enabled;
        }
    }
}
