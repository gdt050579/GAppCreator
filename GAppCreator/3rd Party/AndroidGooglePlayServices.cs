using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class AndroidGooglePlayServices : SDKSettingsUserControl
    {
        AndroidBuildConfiguration config = null;
        List<string> leaderboards = null;
        public AndroidGooglePlayServices()
        {
            InitializeComponent();
            SDKIconName = "google";
            SDKName = "Google";
        }
        public override void Init(GenericBuildConfiguration _config)
        {
            config = (AndroidBuildConfiguration)_config;
            cbEnable.Checked = config.GooglePlayServices;
            leaderboards = Project.StringListToList(config.GooglePlayLeaderboardsList);
            txAppID.Text = config.GooglePlayServicesAppID;
            UpdateLeaderboards();
        }
        public override bool UpdateBuildConfiguration()
        {
            config.GooglePlayServices = cbEnable.Checked;
            config.GooglePlayLeaderboardsList = Project.ListToStringList(leaderboards);
            config.GooglePlayServicesAppID = txAppID.Text;
            return true;
        }

        #region Leaderboards

        private void UpdateLeaderboards()
        {
            lstLeaderboards.Items.Clear();
            foreach (string lb in leaderboards)
            {
                List<string> w = Project.StringListToList(lb, '|');
                if (w.Count != 3)
                    continue;
                ListViewItem lvi = new ListViewItem(lstLeaderboards.Items.Count.ToString());
                lvi.SubItems.Add(w[0]);
                lvi.SubItems.Add(w[1]);
                if (w[2] == "A")
                    lvi.SubItems.Add("All times");
                else if (w[2] == "W")
                    lvi.SubItems.Add("This week");
                else
                    lvi.SubItems.Add("Today");
                lstLeaderboards.Items.Add(lvi);
            }
        }

        private int GetLeaderboardIndex(string name)
        {
            name += "|";
            for (int tr = 0; tr < leaderboards.Count; tr++)
                if (leaderboards[tr].StartsWith(name))
                    return tr;
            return -1;
        }

        private void OnAddLeaderboard(object sender, EventArgs e)
        {
            NewGooglePlayLeaderboard dlg = new NewGooglePlayLeaderboard(null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (GetLeaderboardIndex(dlg.VarName)>=0)
                {
                    MessageBox.Show("Leaderboard '" + dlg.VarName + "' already exists. Plase select another name !");
                    return;
                }
                leaderboards.Add(dlg.VarName + "|" + dlg.ID + "|" + dlg.Type);
                UpdateLeaderboards();
            }
        }

        private void OnDeleteLeaderboard(object sender, EventArgs e)
        {
            if (lstLeaderboards.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select a leaderboard to delete !");
                return;
            }
            if (MessageBox.Show("Are you sure do you want to delete leaderboard '" + lstLeaderboards.SelectedItems[0].SubItems[1].Text, "Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            leaderboards.RemoveAt(lstLeaderboards.SelectedIndices[0]);
            UpdateLeaderboards();
        }

        private void OnEditLeaderboard(object sender, EventArgs e)
        {
            if (lstLeaderboards.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select a leaderboard to edit !");
                return;
            }
            int index = lstLeaderboards.SelectedIndices[0];
            NewGooglePlayLeaderboard dlg = new NewGooglePlayLeaderboard(leaderboards[index]);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int newIndex = GetLeaderboardIndex(dlg.VarName);
                if ((newIndex>=0) && (newIndex!=index))
                {
                    MessageBox.Show("Leaderboard '" + dlg.VarName + "' already exists. Plase select another name !");
                    return;
                }
                leaderboards[index] = dlg.VarName + "|" + dlg.ID + "|" + dlg.Type;
                UpdateLeaderboards();
            }
        }
        private void lstLeaderboards_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstLeaderboards.SelectedIndices.Count == 1)
                OnEditLeaderboard(null, null);
        }
        #endregion


    }
}
