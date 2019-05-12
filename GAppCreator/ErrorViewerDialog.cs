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
    public partial class ErrorViewerDialog : Form
    {
        public ErrorViewerDialog(ErrorsContainer ec)
        {
            InitializeComponent();

            for (int tr = 0; tr < ec.GetCount(); tr++)
            {
                ErrorsContainer.ErrorInfo ei = ec.Get(tr);
                ListViewItem lvi = new ListViewItem(ei.Module);
                lvi.SubItems.Add(ei.Error);
                lvi.ToolTipText = ei.Error + "\n" + ei.Exception;
                lvi.Tag = ei;
                lstErrors.Items.Add(lvi);
            }

            this.Text = "Errors - " + ec.GetCount().ToString();
        }

        private void lstErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstErrors.SelectedItems.Count == 1)
            {
                txtExtraInfo.Text = lstErrors.SelectedItems[0].ToolTipText;
            }
            else
            {
                txtExtraInfo.Text = "";
            }
        }

        private void OnCloseErrorDialog(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
