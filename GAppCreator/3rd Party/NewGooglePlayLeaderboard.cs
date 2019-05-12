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
    public partial class NewGooglePlayLeaderboard : Form
    {
        public string VarName = "";
        public string ID = "";
        public string Type = "";
        public NewGooglePlayLeaderboard(string leaderboard)
        {
            InitializeComponent();
            if (leaderboard != null)
            {
                List<string> w = Project.StringListToList(leaderboard, '|');
                if (w.Count == 3)
                {
                    txName.Text = w[0];
                    txID.Text = w[1];
                    if (w[2] == "A")
                        comboType.SelectedIndex = 0;
                    if (w[2] == "W")
                        comboType.SelectedIndex = 1;
                    if (w[2] == "T")
                        comboType.SelectedIndex = 2;
                }
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            VarName = txName.Text.Trim();
            if (Project.ValidateVariableNameCorectness(VarName,false)==false)
            {
                MessageBox.Show("Invalid constant name (use a-z, A-Z, 0-9 and _) !");
                txName.Focus();
                return;
            }
            ID = txID.Text.Trim();
            if (ID.Length==0)
            {
                MessageBox.Show("Please select an ID !");
                txID.Focus();
                return;
            }
            if (comboType.SelectedIndex<0)
            {
                MessageBox.Show("Please select a type !");
                comboType.Focus();
                return;
            }
            switch (comboType.SelectedIndex)
            {
                case 0: Type = "A"; break;
                case 1: Type = "W"; break;
                case 2: Type = "T"; break;
                default: MessageBox.Show("Unknown type !"); return;
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
