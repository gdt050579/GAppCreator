using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class DebugCommandsDialog : Form
    {
        Project prj;
        public DebugCommandsDialog(Project p)
        {
            InitializeComponent();
            prj = p;
            UpdateDebugCommandsList("");
        }

        private void OnAddDebugCommand(object sender, EventArgs e)
        {
            DebugCommandEdit dlg = new DebugCommandEdit(null);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                prj.SetDebugCommand(dlg.Command);
                UpdateDebugCommandsList(dlg.Command.Name);
            }
        }
        private void UpdateDebugCommandsList(string toSelect)
        {
            lstCommands.Items.Clear();
            // sort
            foreach (DebugCommand cmd in prj.DebugCommands)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = cmd.Name;
                lvi.Text = cmd.Name;
                lvi.SubItems.Add(cmd.Description);
                for (int tr=0;tr<8;tr++)
                {
                    if (tr<cmd.Parameters.Count)
                    {
                        lvi.SubItems.Add(cmd.Parameters[tr].Name + " (" + cmd.Parameters[tr].Type.ToString() + ")");
                    }
                    else
                    {
                        lvi.SubItems.Add("-");
                    }
                }
                lstCommands.Items.Add(lvi);
                if (cmd.Name.ToLower().Equals(toSelect.ToLower()))
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                }
            }
        }

        private void OnDblClicked(object sender, MouseEventArgs e)
        {
            if (lstCommands.SelectedItems.Count!=1)
            {
                MessageBox.Show("You have to select an item first !");
                return;
            }
            int index = prj.DebugCommandToIndex(lstCommands.SelectedItems[0].Text);
            if (index<0)
            {
                MessageBox.Show("Internal error !!! (Unable to find : " + lstCommands.SelectedItems[0].Text+" )");
                return;
            }
            DebugCommandEdit dlg = new DebugCommandEdit(prj.DebugCommands[index]);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                prj.SetDebugCommand(dlg.Command);
                UpdateDebugCommandsList(dlg.Command.Name);
            }
        }

        private void OnEditDebugCommand(object sender, EventArgs e)
        {
            OnDblClicked(null, null);
        }

        private void OnDeleteDebugCommand(object sender, EventArgs e)
        {
            if (lstCommands.SelectedItems.Count != 1)
            {
                MessageBox.Show("You have to select an item first !");
                return;
            }
            if (MessageBox.Show("Delete '"+lstCommands.SelectedItems[0].Text+"' ?","Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                int index = prj.DebugCommandToIndex(lstCommands.SelectedItems[0].Text);
                if (index < 0)
                {
                    MessageBox.Show("Internal error !!! (Unable to find : " + lstCommands.SelectedItems[0].Text + " )");
                    return;
                }
                prj.DebugCommands.RemoveAt(index);
                UpdateDebugCommandsList("");
            }
        }
    }
}
