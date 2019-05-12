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
    public partial class SettingsSnapshotEditDialog : Form
    {
        public string Result = "";
        public SettingsSnapshotEditDialog(string value)
        {
            InitializeComponent();
            ctrlSnapshots.SetCurrentSnapshotID(value);
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (ctrlSnapshots.HasSnapshotSelecteed()==false)
            {
                MessageBox.Show("Please select a snapshot first !");
                return;
            }
            Result = ctrlSnapshots.GetSelectedSnapshotStringRepresentation();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void ctrlSnapshots_OnSelectSnapshot(object sender)
        {
            OnOK(null, null);
        }
    }
}
