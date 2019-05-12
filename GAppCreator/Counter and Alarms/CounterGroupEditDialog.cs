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
    public partial class CounterGroupEditDialog : Form
    {
        Project prj;
        CounterGroup group;
        public CounterGroupEditDialog(Project _prj, CounterGroup _group)
        {
            InitializeComponent();
            for (int tr = 0; tr < CounterGroup.GetUpdateMethodsCount(); tr++)
                comboMethod.Items.Add(CounterGroup.GetUpdateMethod(tr));
            for (int tr = 0; tr < CounterGroup.GetAfterUpdateTimerStatusCount(); tr++)
                comboAfterUpdateBehavior.Items.Add(CounterGroup.GetAfterUpdateTimerStatus(tr));
            // populez si combo-ul de scene
            GACParser.Module moduleScenes = GACParser.Compiler.GetModule("Scenes");
            if (moduleScenes != null)
            {
                foreach (string sceneName in moduleScenes.Members.Keys)
                {
                    comboScenes.Items.Add(sceneName);
                }
            }
            prj = _prj;
            group = _group;
            if (group != null)
            {
                txName.Text = group.Name;
                comboMethod.SelectedIndex = group.UpdateMethod;
                cbUseEnableConditionProperty.Checked = group.UseEnableConditionProperty;
                if (group.MinimTimeInterval<1)
                {
                    cbEnableTimer.Checked = false;
                }
                else
                {
                    cbEnableTimer.Checked = true;
                    nmMinimTimeLimit.Value = group.MinimTimeInterval;
                    comboStartTimerMethod.SelectedIndex = group.StartTimerMethod;
                    SetValueToComboBox(comboScenes, group.StartTimerScene);
                    comboAfterUpdateBehavior.SelectedIndex = group.AfterUpdateBehavior;
                }                
            }
            cbEnableTimer_CheckedChanged(null, null);
            txName.Focus();
        }

        private void OnOK(object sender, EventArgs e)
        {
            string vname = txName.Text.Trim();
            if (vname.Length==0)
            {
                MessageBox.Show("Expecting a name for the group !");
                txName.Focus();
                return;
            }
            if (Project.ValidateVariableNameCorectness(vname, false) == false)
            {
                MessageBox.Show("Invalid name: '" + vname + "' - it should contains letter, numbers and '_' character !");
                txName.Focus();
                return;
            }
            if (comboMethod.SelectedIndex<0)
            {
                MessageBox.Show("Please select an update method !");
                comboMethod.Focus();
                return;
            }
            if ((comboAfterUpdateBehavior.SelectedIndex < 0) && (comboAfterUpdateBehavior.Enabled))
            {
                MessageBox.Show("Please select an after update behavior for timer !");
                comboAfterUpdateBehavior.Focus();
                return;
            }
            if ((comboStartTimerMethod.SelectedIndex < 0) && (comboStartTimerMethod.Enabled))
            {
                MessageBox.Show("Please select how timer should be started !");
                comboStartTimerMethod.Focus();
                return;
            }
            if ((comboScenes.SelectedIndex < 0) && (comboScenes.Enabled))
            {
                MessageBox.Show("Please select a scene !");
                comboScenes.Focus();
                return;
            }       
            foreach (CounterGroup g in prj.CountersGroups)
            {
                if ((vname.ToLower().Equals(g.Name.ToLower())) && (g != group))
                {
                    MessageBox.Show("Group '" + vname + "' already exists !");
                    txName.Focus();
                    return;
                }
            }
            if (group==null)
            {
                group = new CounterGroup();
                prj.CountersGroups.Add(group);
            }
            group.Name = vname;
            group.UpdateMethod = comboMethod.SelectedIndex;
            group.UseEnableConditionProperty = cbUseEnableConditionProperty.Checked;
            group.MinimTimeInterval = (int)(nmMinimTimeLimit.Value);
            group.StartTimerMethod = comboStartTimerMethod.SelectedIndex;
            group.StartTimerScene = comboScenes.Items[comboScenes.SelectedIndex].ToString();
            group.AfterUpdateBehavior = comboAfterUpdateBehavior.SelectedIndex;

            if (cbEnableTimer.Checked == false)
            {
                group.MinimTimeInterval = 0;
                group.StartTimerMethod = 0;
                group.StartTimerScene = "";
                group.AfterUpdateBehavior = 0;
            }
            prj.CountersGroups.Sort();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void cbEnableTimer_CheckedChanged(object sender, EventArgs e)
        {
            lbAfterUpdateBeehavior.Enabled = cbEnableTimer.Checked;
            lbMinimalTimeLimit.Enabled = cbEnableTimer.Checked;
            lbMinimalTimeLimitSeconds.Enabled = cbEnableTimer.Checked;
            comboAfterUpdateBehavior.Enabled = cbEnableTimer.Checked;
            comboScenes.Enabled = cbEnableTimer.Checked;
            comboStartTimerMethod.Enabled = cbEnableTimer.Checked;
            nmMinimTimeLimit.Enabled = cbEnableTimer.Checked;
            lbStartTimerMethod.Enabled = cbEnableTimer.Checked;
            comboStartTimerMethod_SelectedIndexChanged(null, null);
        }
        private void SetValueToComboBox(ComboBox combo,string value)
        {
            for (int tr=0;tr<combo.Items.Count;tr++)
                if (combo.Items[tr].ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    combo.SelectedIndex = tr;
                    return;
                }
        }

        private void comboStartTimerMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboScenes.Enabled = (comboStartTimerMethod.SelectedIndex == 3) && (comboStartTimerMethod.Enabled);
        }
    }
}
