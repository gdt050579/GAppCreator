using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class SettingsDialog : Form
    {
        SystemSettings Settings;
        public SettingsDialog(SystemSettings settings)
        {
            InitializeComponent();
            Settings = settings;

            txInkscapePath.Text = Settings.InskapePath;
            txImageEditor.Text = Settings.ImageEditorPath;
            txBuildServerAddress.Text = Settings.BuildServerAddress;
            txUserName.Text = Settings.UserName;
            txPassword.Text = Settings.Password;
            txVSVars32.Text = Settings.VSVarsBat;
            txCl.Text = Settings.CL;
            txAndroidADB.Text = Settings.AndroidADB;
            txGitExe.Text = Settings.GitPath;
            txGitUser.Text = Settings.GitUserName;
            txGitPassword.Text = Settings.GitPassword;
            txDevEnv.Text = Settings.VSDevEnv;
            for (int tr=0;tr<comboVSVersion.Items.Count;tr++)
                if (comboVSVersion.Items[tr].ToString().Contains(Settings.VSToolSet))
                {
                    comboVSVersion.SelectedIndex = tr;
                }
        }

        private void BrowseForFile(string filename, string filter, TextBox txb)
        {
            openDLG.Filter = filter;
            openDLG.FileName = filename;
            if (openDLG.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txb.Text = openDLG.FileName;            
        }
        private void BrowseForFolder(TextBox txb)
        {
            if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txb.Text = openFolder.SelectedPath;
        }
        private void OnBrowseForInkscape(object sender, EventArgs e)
        {
            BrowseForFile("inkscape.exe", "", txInkscapePath);
        }
        private void OnBrowseForRasterImageEditor(object sender, EventArgs e)
        {
            BrowseForFile("", "Executable files|*.exe", txImageEditor);
        }
        private void OnBrowseForVSVars32(object sender, EventArgs e)
        {
            BrowseForFile("", "VSVars32 bat file|vsvars32.bat|All Files|*.*", txVSVars32);
        }
        private void OnBrowseForClExe(object sender, EventArgs e)
        {
            BrowseForFile("", "MS Compiler|cl.exe|All Files|*.*", txCl);
        }
        private void OnBrowseForADT(object sender, EventArgs e)
        {
            BrowseForFile("", "Android ADB|adb.exe|All Files|*.*", txAndroidADB);
        }
        private void OnUpdateSettings(object sender, EventArgs e)
        {
            Settings.InskapePath = txInkscapePath.Text;
            Settings.ImageEditorPath = txImageEditor.Text;
            Settings.VSVarsBat = txVSVars32.Text;
            Settings.CL = txCl.Text;
            Settings.AndroidADB = txAndroidADB.Text;
            Settings.BuildServerAddress = txBuildServerAddress.Text;
            Settings.UserName = txUserName.Text;
            Settings.Password = txPassword.Text;
            Settings.GitUserName = txGitUser.Text;
            Settings.GitPassword = txGitPassword.Text;
            Settings.GitPath = txGitExe.Text;
            Settings.VSDevEnv = txDevEnv.Text;

            string vstoolset = "";
            if (comboVSVersion.SelectedItem != null)
                vstoolset = comboVSVersion.SelectedItem.ToString();
            if (vstoolset.Length>0)
            {
                int i1 = vstoolset.IndexOf('(');
                int i2 = vstoolset.LastIndexOf(')');
                if ((i1>=0) && (i2>=0))
                {
                    vstoolset = vstoolset.Substring(i1 + 1, i2 - i1 - 1);
                    Settings.VSToolSet = vstoolset.Trim();
                }
                else
                {
                    MessageBox.Show("Unknwon VSTool Set - " + vstoolset);
                }
            }

            Settings.Save();
            Close();
        }

        private void OnGeneratePassword(object sender, EventArgs e)
        {
            Random r = new Random();
            string letters = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string pass = "";
            for (int tr = 0; tr < 16; tr++)
            {
                int index = r.Next(letters.Length);
                pass += letters[index];
                letters.Remove(index, 1);
            }
            txPassword.Text = pass;
        }

        private void OnBrowseForrGit(object sender, EventArgs e)
        {
            BrowseForFile("git.exe", "", txGitExe);
        }

        private void OnBrowseForDevEnv(object sender, EventArgs e)
        {
            BrowseForFile("devenv.exe", "", txDevEnv);
        }




    }
}
