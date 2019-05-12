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
    public partial class ProjectTabCountersAndAlerts : BaseProjectContainer
    {
        GListView lstCounters;
        GListView lstCountersGroups;
        GListView lstAlerts;

        public ProjectTabCountersAndAlerts()
        {
            InitializeComponent();

            lstCounters = new GListView();            
            lstCounters.AddColumn("Name", "propName", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstCounters.AddColumn("Group", "propGroup", 150, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstCounters.AddColumn("Builds", "propBuilds", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstCounters.AddColumn("Interval", "propInterval", 60, GListView.RenderType.Default, true, HorizontalAlignment.Right);
            lstCounters.AddColumn("Max times", "propMaxTimes", 60, GListView.RenderType.Default, true, HorizontalAlignment.Right);
            lstCounters.AddColumn("Priority", "propPriority", 60, GListView.RenderType.Default, true, HorizontalAlignment.Center);
            lstCounters.AddColumn("Enable", "propEnabled", 60, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstCounters.AddColumn("Persistent", "propPersistent", 60, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstCounters.AddColumn("Scene", "propScene", 100, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstCounters.AllColumns[0].GroupKeyGetter = delegate(object x) { return ((Counter)x).propGroup; };
            lstCounters.OnObjectsSelected += lstCounters_OnObjectsSelected;
            lstCounters.Dock = DockStyle.Fill;

            lstCountersGroups = new GListView();
            lstCountersGroups.AddColumn("Group", "propGroup", 500, GListView.RenderType.ItemRenderer, false, HorizontalAlignment.Left);
            lstCountersGroups.Dock = DockStyle.Fill;
            lstCountersGroups.OnObjectDoubleClicked += lstCountersGroups_OnObjectDoubleClicked;

            this.splitContainer3.Panel1.Controls.Add(this.lstCounters);
            this.splitContainer3.Panel2.Controls.Add(this.lstCountersGroups);

            // alerts
            lstAlerts = new GListView();
            lstAlerts.AddColumn("Name", "propName", 600, GListView.RenderType.ItemRenderer, false, HorizontalAlignment.Left);
            lstAlerts.AddColumn("Builds", "propBuilds", 250, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstAlerts.AddColumn("Enable", "propEnabled", 60, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstAlerts.AddColumn("OneTime only", "propOneTimeOnly", 80, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstAlerts.AddColumn("Duration", "propDuration", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstAlerts.AddColumn("Push Notification", "propPushNotification", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstAlerts.OnObjectDoubleClicked += lstAlerts_OnObjectDoubleClicked;

            lstAlerts.Dock = DockStyle.Fill;

            this.panel1.Controls.Add(this.lstAlerts);
        }

        #region Counters
        void lstCountersGroups_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            string original_name = ((CounterGroup)SelectedObject).Name;
            CounterGroupEditDialog dlg = new CounterGroupEditDialog(Context.Prj, (CounterGroup)SelectedObject);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (((CounterGroup)SelectedObject).Name != original_name)
                {
                    foreach (Counter gc in Context.Prj.Counters)
                    {
                        if (gc.Group.Equals(original_name))
                            gc.Group = ((CounterGroup)SelectedObject).Name;
                    }
                }
                RefreshCounters();
            }
        }
        void lstCounters_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            if (selected)
                propertyCounter.SelectedObjects = lstCounters.GetSelectedObjectsArray();
            else
            {
                propertyCounter.SelectedObjects = null;
                propertyCounter.SelectedObject = null;
            }
        }
        void RefreshCounters()
        {
            if ((Context != null) && (Context.Prj != null))
            {
                lstCounters.SetObjects(Context.Prj.Counters);
                lstCountersGroups.SetObjects(Context.Prj.CountersGroups);
            }
        }
        private void OnAddNewCounter(object sender, EventArgs e)
        {
            if ((Context == null) || (Context.Prj == null))
                return;
            Counter gc = new Counter();
            gc.Name = "Counter_" + (Context.Prj.Counters.Count + 1).ToString();
            Random r = new Random();
            bool found;
            do
            {
                gc.Hash = r.Next(1,10000000).ToString("X");
                found = false;
                foreach (Counter existing_gc in Context.Prj.Counters)
                    if (existing_gc.Equals(gc.Hash))
                        found = true;
            } while (found == true);
            Context.Prj.Counters.Add(gc);
            RefreshCounters();
            lstCounters.SelectObjectFromList(gc);
        }
        private void OnDeleteCounter(object sender, EventArgs e)
        {
            if ((Context == null) || (Context.Prj == null))
                return;
            object[] o = lstCounters.GetSelectedObjectsArray();
            if ((o==null) || (o.Length==0))
            {
                MessageBox.Show("Please select at least one counter for deletion !");
                return;
            }
            if (MessageBox.Show("Delete "+o.Length.ToString()+" counters ?","Delete",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (object obj in o)
                {
                    Context.Prj.Counters.Remove((Counter)obj);
                }
                RefreshCounters();
                propertyCounter.SelectedObjects = null;
                propertyCounter.SelectedObject = null;
            }
        }
        private void OnValidateCounters(object sender, EventArgs e)
        {
            if ((Context == null) || (Context.Prj == null))
                return;
            if (Context.Prj.CheckCountersIntegrity() == false)
                Context.Prj.ShowErrors();
            else
                MessageBox.Show("All counters are set cosrectly !");
        }
        private void propertyCounter_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            lstCounters.RefreshSelectedObjects();
        }
        private void OnAddGroup(object sender, EventArgs e)
        {
            CounterGroupEditDialog dlg = new CounterGroupEditDialog(Context.Prj, null);
            if (dlg.ShowDialog() == DialogResult.OK)
                RefreshCounters();
        }

        private void OnDeleteGroup(object sender, EventArgs e)
        {
            if (lstCountersGroups.GetCurrentSelectedObjectsListCount() < 1)
            {
                MessageBox.Show("Please select at least one group for deletion !");
                return;
            }
            if (MessageBox.Show("Delete " + lstCountersGroups.GetCurrentSelectedObjectsListCount().ToString() + " groups ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (object obj in lstCountersGroups.GetCurrentSelectedObjectsList())
                    Context.Prj.CountersGroups.Remove((CounterGroup)obj);
                // refac numele de grupuri
                Dictionary<string, bool> d = new Dictionary<string, bool>();
                foreach (CounterGroup gcg in Context.Prj.CountersGroups)
                    d[gcg.Name.Trim().ToLower()] = true;
                foreach (Counter gc in Context.Prj.Counters)
                {
                    string group = gc.Group.Trim().ToLower();
                    if (d.ContainsKey(group) == false)
                        gc.Group = "";
                }
                RefreshCounters();
            }
        }

        private void OnEditGroup(object sender, EventArgs e)
        {
            object o = lstCountersGroups.GetCurrentSelectedObject();
            if (o==null)
            {
                MessageBox.Show("Please select one object to edit from the group counters list !");
                return;
            }
            lstCountersGroups_OnObjectDoubleClicked(null, o);
        }
        private void OnShowSimulator(object sender, EventArgs e)
        {
            if ((Context == null) || (Context.Prj == null))
                return;
            if (Context.Prj.CheckCountersIntegrity() == false)
            {
                MessageBox.Show("There are some problems in the definitions of the counters/groups. Simulator can not run in this case. Click on the 'Validate' button to get more information !");
                return;
            }
            CounterGroupSimulator dlg = new CounterGroupSimulator(Context.Prj);
            dlg.ShowDialog();
        }

        #endregion

        #region Functii virtuale
        public override void OnActivate()
        {
            CounterGroupSelectorEditor.prj = Context.Prj;
            CounterAuttoEnableStateEditor.prj = Context.Prj;
            RefreshCounters();
            RefreshAlerts();
        }     
        #endregion

        #region Alerts

        private void OnAddAlert(object sender, EventArgs e)
        {
            if ((Context == null) || (Context.Prj == null))
                return;
            AlarmEditDialog dlg = new AlarmEditDialog(null,Context);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                RefreshAlerts();
                // calculez ultimul ID
                int lastID = 0;
                foreach (var al in Context.Prj.Alarms)
                    if (al.UniqueID > lastID)
                        lastID = al.UniqueID;
                Alarm a = dlg.GetCurrentAlarm();
                a.UniqueID = lastID + 1;
                lstAlerts.SelectObjectFromList(a);
            }
        }
        private void OnDeleteAlert(object sender, EventArgs e)
        {
            if (lstAlerts.GetCurrentSelectedObjectsListCount() < 1)
            {
                MessageBox.Show("Please select at least one alert for deletion !");
                return;
            }
            if (MessageBox.Show("Delete " + lstAlerts.GetCurrentSelectedObjectsListCount().ToString() + " alerts ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (object obj in lstAlerts.GetCurrentSelectedObjectsList())
                    Context.Prj.Alarms.Remove((Alarm)obj);
                RefreshAlerts();
            }
        }

        private void OnEeditAlert(object sender, EventArgs e)
        {
            object o = lstAlerts.GetCurrentSelectedObject();
            if (o == null)
            {
                MessageBox.Show("Please select one alert to edit !");
                return;
            }
            lstAlerts_OnObjectDoubleClicked(null, o);
        }
        void RefreshAlerts()
        {
            if ((Context != null) && (Context.Prj != null))
            {
                lstAlerts.SetObjects(Context.Prj.Alarms);
            }
        }
        private void lstAlerts_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            AlarmEditDialog dlg = new AlarmEditDialog((Alarm)SelectedObject, Context);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                RefreshAlerts();
                lstAlerts.SelectObjectFromList(dlg.GetCurrentAlarm());
            }
        }
        private void OnValidateAlarms(object sender, EventArgs e)
        {
            if ((Context==null) || (Context.Prj==null))
                return;
            if (Context.Prj.CheckAlarmsIntegrity() == false)
                Context.Prj.ShowErrors();
            else
                MessageBox.Show("All alarms are set corectly !");
        }
        #endregion





    }
}
