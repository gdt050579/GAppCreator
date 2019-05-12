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
    public partial class GitDialog : Form
    {
        Project prj = null;
        ProjectContext context;
        Git g = new Git();
        public GitDialog(ProjectContext _context)
        {
            InitializeComponent();
            prj = _context.Prj;
            context = _context;
        }

        private bool CheckForGit ()
        {
            if (g.SetGitFolder(prj.ProjectPath)==false)
            {
                if (MessageBox.Show("Current project is not part of any git repository. Would you like to create a repository for it ?", "Git", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return false;
                InputBox ib = new InputBox("Git repo url (or empty string for no remote repository)", "https://github.com/......");
                if (ib.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;
                if (g.CreateRepo(prj.ProjectPath)==false)
                {
                    MessageBox.Show(g.GetLastError(),"Error");
                    return false;
                }
                if (ib.StringResult.Trim().Length>0)
                {
                    if (g.SetRemoteAddress(ib.StringResult.Trim())==false)
                        MessageBox.Show("Repository was created but only localy. The remote address provided: "+ib.StringResult+" is not valid !\n"+g.GetLastError(), "Error");
                }
                // adaug projeect si fac un prim commit
                if (g.AddFile(Path.Combine(prj.ProjectPath,"project.gappcreator"))==false)
                {
                    MessageBox.Show("Fail to add project file to repo !\n" + g.GetLastError(), "Error");
                    if (Disk.DeleteDirectory(Path.Combine(prj.ProjectPath, ".git"), prj.EC) == false)
                        prj.ShowErrors();
                    return false;
                }
                if (g.Commit("First commit on "+DateTime.Now.ToString())==false)
                {
                    MessageBox.Show("Fail to commmit project file to repo !\n" + g.GetLastError(), "Error");
                    if (Disk.DeleteDirectory(Path.Combine(prj.ProjectPath, ".git"), prj.EC) == false)
                        prj.ShowErrors();
                    return false;
                }
                // creez si gitgnore
                // creez si .gitignore
                string ignore = "";
                ignore += "[Bb]in/\n";
                ignore += "[Tt]emp/\n";
                ignore += "[Bb]uilds/\n";
                ignore += "[Cc]pp[Pp]roject/\n";
                ignore += "[Rr]esources/[Oo]utput/\n";
                ignore += "[Rr]esources/[Pp]lugins/\n";
                if (Disk.SaveFile(Path.Combine(prj.ProjectPath, ".gitignore"), ignore, prj.EC) == false)
                    prj.ShowErrors();
            }
            return true;
        }
        private void UpdateFileList()
        {
            lstFiles.Items.Clear();
            Dictionary<string,Git.RepoFileStatus> d =  g.GetStatus(true);
            if (d == null)
            {
                MessageBox.Show(g.GetLastError(), "Error");
                return;
            }
            Dictionary<string, string> l = prj.GetProjectFilesThatShouldBeInGitRepo();            
            lstFiles.BeginUpdate();
            int count = 0;
            foreach (string fname in l.Keys)
            {
                ListViewItem lvi = new ListViewItem(fname);
                lvi.Tag = false;
                bool skip = false;
                if (d.ContainsKey(fname))
                {
                    switch (d[fname])
                    {
                        case Git.RepoFileStatus.Added:
                        case Git.RepoFileStatus.Unversioned:
                            lvi.Group = lstFiles.Groups["n"];
                            lvi.ForeColor = Color.Green;
                            lvi.Tag = true;
                            count++;
                            break;
                        case Git.RepoFileStatus.Deleted:
                            lvi.Group = lstFiles.Groups["d"];
                            lvi.ForeColor = Color.DarkRed;
                            lvi.Tag = true;
                            count++;
                            break;
                        case Git.RepoFileStatus.Modified:
                            lvi.Group = lstFiles.Groups["m"];
                            lvi.ForeColor = Color.Blue;
                            lvi.Tag = true;
                            count++;
                            break;
                        case Git.RepoFileStatus.UpToDate:
                            lvi.Group = lstFiles.Groups["u"];
                            skip = true;
                            break;
                        case Git.RepoFileStatus.Ignored:
                            lvi.Group = lstFiles.Groups["i"];
                            lvi.ForeColor = Color.Brown;
                            skip = true;
                            break;
                        default:
                            lvi.Group = lstFiles.Groups["?"];
                            lvi.ForeColor = Color.Gray;
                            skip = true;
                            break;
                    }
                }
                else
                {
                    lvi.Group = lstFiles.Groups["n"];
                    lvi.ForeColor = Color.Green;
                    lvi.Tag = true;
                    count++;
                }
                if ((skip) && (btnOnlyChanges.Checked))
                    continue;
                lvi.Checked = (bool)lvi.Tag;
                lstFiles.Items.Add(lvi);
            }
            lstFiles.EndUpdate();
            if (count>0)
            {
                //btnCommit.Text = "Commit (" + count.ToString() + " files)";
                btnCommit.Enabled = true;
            }
            else
            {
                //btnCommit.Text = "Commit";
                btnCommit.Enabled = false;
            }
        }
        private void GitDialog_Load(object sender, EventArgs e)
        {
            if (CheckForGit() == false)
            {
                this.Close();
                return;
            }
            UpdateFileList();
        }

        private void OnCommit(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Commit message", "");
            if (ib.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                // vad daca am si new files si le adaug
                Dictionary<string, string> l = prj.GetProjectFilesThatShouldBeInGitRepo();
                List<string> newFiles = new List<string>();
                foreach (ListViewItem lvi in lstFiles.Items)
                    if ((lvi.Checked) && (l.ContainsKey(lvi.Text)))
                        newFiles.Add(l[lvi.Text]);
                if (newFiles.Count>0)
                {
                    if (g.AddFiles(newFiles.ToArray())==false)
                    {
                        MessageBox.Show(g.GetLastError(), "Error");
                        return;
                    }
                }
                if (g.Commit(ib.StringResult)==false)
                    MessageBox.Show(g.GetLastError(), "Error");
                UpdateFileList();
            }
        }

        private void OnChangeOnlyChanges(object sender, EventArgs e)
        {
            UpdateFileList();
        }

        private void OnEditRepoURL(object sender, EventArgs e)
        {
            string s = g.GetRemoteAddress();
            if (s==null)
            {
                MessageBox.Show(g.GetLastError(), "Error");
                return;
            }
            InputBox dlg = new InputBox("Set remote repository URL:", s);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (g.SetRemoteAddress(dlg.StringResult)==false)
                    MessageBox.Show(g.GetLastError(), "Error");
            }
        }

        private void OnPush(object sender, EventArgs e)
        {
            string userName, password;

            // user name
            if (context.Settings.GitUserName.Length==0)
            {
                InputBox dlg = new InputBox("Git user name:", "");
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                userName = dlg.StringResult;
            }
            else
            {
                userName = context.Settings.GitUserName;
            }

            // password
            if (context.Settings.GitPassword.Length == 0)
            {
                InputBox dlg = new InputBox("Git password:", "");
                if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                password = dlg.StringResult;
            }
            else
            {
                password = context.Settings.GitPassword;
            }

            // incerc sa fac comitul
            if (g.Push(userName,password,true)==false)
            {
                MessageBox.Show("Failed to push changes to remote repository !\n" + g.GetLastError());
            }
        }

        private void OnValidateItemCheck(object sender, ItemCheckEventArgs e)
        {
            if ((bool)(lstFiles.Items[e.Index].Tag)==false)
            {
                e.NewValue = CheckState.Unchecked;
            }
        }
    }
}
