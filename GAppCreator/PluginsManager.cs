using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class PluginsManager : Form
    {
        Project prj;
        PluginList Plugins;
        string originalList = "";
        public PluginsManager(Project p,PluginList pl)
        {
            prj = p;
            originalList = p.PluginList;
            Plugins = pl;
            InitializeComponent();
            UpdatePluginStatus();

        }
        private void UpdatePluginStatus()
        {
            lstAvailable.Items.Clear();
            Plugins.RefreshAvailablePlugins();
            foreach (string s in Plugins.AvailableModules)
                lstAvailable.Items.Add(s);
            lstPlugins.Items.Clear();
            foreach (string moduleName in Plugins.ModuleToExtension.Keys)
            {
                ListViewItem lvi = new ListViewItem(Plugins.ModuleToExtension[moduleName]);
                lvi.SubItems.Add(moduleName);
                ResourcePluginData rpd = Plugins.Plugins[Plugins.ModuleToExtension[moduleName]];
                lvi.SubItems.Add(rpd.Interface.GetResourceTypeDescription());
                lvi.Tag = rpd;
                lstPlugins.Items.Add(lvi);
            }
        }

        private void OnCreateNewPlugin(object sender, EventArgs e)
        {
            NewPluginDialog dlg = new NewPluginDialog();
            if (dlg.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                string root = Path.Combine(prj.GetProjectPluginsCodeFolder(), dlg.PluginName);
                if (Directory.Exists(root))
                {
                    MessageBox.Show("Plugin '" + dlg.PluginName + "' defined in folder '" + root + "' already exists !");
                    Process.Start(root);
                    return;
                }
                while (true)
                {
                    if (Disk.CreateFolder(root, prj.EC) == false)
                        break;
                    if (Disk.CreateFolder(Path.Combine(root, "Bin"),prj.EC) == false)
                        break;
                    if (Disk.CreateFolder(Path.Combine(root, "Properties"), prj.EC) == false)
                        break;
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    d["$$PLUGIN.NAME$$"] = dlg.PluginName;
                    d["$$EXTENSION$$"] = dlg.PluginExtension;
                    d["$$DESCRIPTION$$"] = dlg.PluginDescription;
                    d["$$PROJECT.GUID$$"] = Guid.NewGuid().ToString();
                    d["$$PLUGIN.INTERFACE$$"] = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "PluginInterface.dll");
                    d["$$SOURCE.DLL$$"] = Path.Combine(root, "bin", "Output", dlg.PluginName + ".Plugin.dll");
                    d["$$DEST.DLL$$"] = Path.Combine(prj.GetProjectResourcePluginsFolder(), dlg.PluginName + ".Plugin.dll");
                    d["$$ASSEMBLY.GUID$$"] = Guid.NewGuid().ToString();
                    d["$$TESTER.GUID$$"] = Guid.NewGuid().ToString();
                    d["$$TESTERASSEMBLY.GUID$$"] = Guid.NewGuid().ToString();

                    Dictionary<string, string> files = new Dictionary<string, string>();
                    files["Project.csproj"] = dlg.PluginName + "Project.csproj";
                    files["Interface.cs"] = dlg.PluginName + "Interface.cs";
                    files["Preview.cs"] = dlg.PluginName + "Preview.cs";
                    files["Preview.Designer.cs"] = dlg.PluginName + "Preview.Designer.cs";
                    files["Editor.cs"] = dlg.PluginName + "Editor.cs";
                    files["Editor.Designer.cs"] = dlg.PluginName + "Editor.Designer.cs";
                    files["ByteBuffer.cs"] = "ByteBuffer.cs";
                    files["Resource.cs"] = dlg.PluginName + ".cs";
                    files["AssemblyInfo.cs"] = Path.Combine("Properties", "AssemblyInfo.cs");
                    files["Solution.sln"] = dlg.PluginName + "Solution.sln";
                    files["Tester.cs"] = Path.Combine("Tester", "Tester.cs");
                    files["TesterAssemblyInfo.cs"] = Path.Combine("Tester", "Properties", "AssemblyInfo.cs");
                    files["Tester.csproj"] = Path.Combine("Tester", "Tester.csproj");

                    foreach (string key in files.Keys)
                    {
                        Project.CreateResource("ResourcePlugin", key, d, Path.Combine(root, files[key]), prj.EC);
                    }
                    if (prj.EC.HasErrors())
                        break;
                    // totul e ok
                    MessageBox.Show("Plugin '" + dlg.PluginName + "' created !");
                    Process.Start(root);
                    return;
                }
                prj.ShowErrors();
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private void OnCancel(object sender, EventArgs e)
        {
            prj.PluginList = originalList;
            OnReloadPlugins(null, null);
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void OnAddPlugin(object sender, EventArgs e)
        {
            if (lstAvailable.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select a item from the available plugins list first !");
                return;
            }
            Plugins.AddPlugin(lstAvailable.SelectedItems[0].Text);
            OnReloadPlugins(null, null);
        }

        private void OnReloadPlugins(object sender, EventArgs e)
        {
            if (Plugins.ReloadPlugins() == false)
                prj.ShowErrors();
            UpdatePluginStatus();
        }

        private void OnDeletePlugin(object sender, EventArgs e)
        {
            if (lstPlugins.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a item from the plugins list first !");
                return;
            }
            Plugins.DeletePlugin(lstPlugins.SelectedItems[0].SubItems[1].Text);
            OnReloadPlugins(null, null);
        }

        private void OnOpenPluginFolder(object sender, EventArgs e)
        {
            if (lstPlugins.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a item from the plugins list first !");
                return;
            }
            ResourcePluginData rpd = (ResourcePluginData)lstPlugins.SelectedItems[0].Tag;
            string root = Path.Combine(prj.GetProjectPluginsCodeFolder(), rpd.ModuleName.ToLower().Replace(".plugin.dll", ""));
            Process.Start(root);
        }


    }
}
