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
    public partial class TaskDialog : Form
    {
        ProjectTask task;
        public TaskDialog(ProjectTask t,bool newTask)
        {
            task = t;
            InitializeComponent();
            txTask.Text = task.Text;
            txAddedOn.Text = task.AddedOn;
            if (task.CompletedOn == null)
                task.CompletedOn = "";
            if (task.CompletedOn.Length > 0)
            {
                txCompletedOn.Text = task.CompletedOn;
                btnCompleteReopen.Text = "ReOpen";
            }
            foreach (TaskType tt in Enum.GetValues(typeof(TaskType)))           
                comboType.Items.Add(tt);
            for (int tr=0;tr<comboType.Items.Count;tr++)
                if (task.Type == (TaskType)comboType.Items[tr])
                    comboType.SelectedIndex = tr;
            txTask_TextChanged(null,null);
            if (newTask)
                btnCompleteReopen.Visible = false;
        }
        private void UpdateTask()
        {
            task.Text = txTask.Text;
            task.AddedOn = txAddedOn.Text;
            task.CompletedOn = txCompletedOn.Text;
            if (comboType.SelectedIndex>=0)
                task.Type = (TaskType)comboType.Items[comboType.SelectedIndex];
        }

        private void OnOK(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            UpdateTask();
            this.Close();
        }

        private void OnCompleteOrReopen(object sender, EventArgs e)
        {
            if (txCompletedOn.Text.Length == 0)
            {
                task.SetNow(false);
                txCompletedOn.Text = task.CompletedOn;
            }
            else
                txCompletedOn.Text = "";
            OnOK(null, null);
        }

        private void txTask_TextChanged(object sender, EventArgs e)
        {
            if (txTask.Text.Trim().Length == 0)
            {
                btnOK.Enabled = false;
                btnCompleteReopen.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
                btnCompleteReopen.Enabled = true;
            }
        }

    }
}
