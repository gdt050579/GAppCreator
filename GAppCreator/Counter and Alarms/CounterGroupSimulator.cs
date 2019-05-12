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
    public partial class CounterGroupSimulator : Form
    {
        private class InternalCounter
        {
            public int Value;
            public int Times;
            public int Interval;
            public int Priority;
            public int MaxTimes;
            public bool Enabled ;
            public Counter CounterObject;
            

            public InternalCounter(ListViewItem lvi)
            {
                Value = 0;
                Times = 0;
                Enabled = lvi.Checked;
                CounterObject = (Counter)lvi.Tag;
                Interval = CounterObject.Interval;
                Priority = CounterObject.Priority;
                MaxTimes = CounterObject.MaxTimes;                
            }
            public bool Update()
            {
                if (this.Enabled == false)
                    return false;
                this.Value++;
                if (this.Value >= this.Interval)
                {
                    this.Value = 0;
                    this.Times++;
                    if ((this.Times >= this.MaxTimes) && (this.MaxTimes != 0))
                        this.Enabled = false;

                    return true;
                }
                return false;
            }
        }
        Project prj;
        Dictionary<string, List<Counter>> groups = new Dictionary<string, List<Counter>>();
        CounterGroup currentGroup;
        Dictionary<string, int> Stats = new Dictionary<string, int>();
        public CounterGroupSimulator(Project _prj)
        {
            prj = _prj;
            InitializeComponent();
            foreach (GenericBuildConfiguration gbc in prj.BuildConfigurations)
                comboBuilds.Items.Add(gbc.Name);
            comboBuilds.SelectedIndex = 0;
            UpdateGroupLists();            
        }
        private void UpdateGroupLists()
        {
            Dictionary<string,Counter> d = new Dictionary<string,Counter>();
            groups.Clear();
            ClearAll();
            prj.CheckCountersIntegrity(prj.BuildConfigurations[comboBuilds.SelectedIndex], d, groups);
            comboGroups.Items.Clear();
            foreach (string s in groups.Keys)
                comboGroups.Items.Add(s);
        }
        private void ClearAll()
        {
            currentGroup = null;
            lstCounters.Items.Clear();
            lbGroupType.Text = "";
        }
        private void OnSelectGroup(object sender, EventArgs e)
        {
            if (comboGroups.SelectedIndex>=0)
            {
                currentGroup = null;
                foreach (CounterGroup cg in prj.CountersGroups)
                    if (cg.Name == comboGroups.Items[comboGroups.SelectedIndex].ToString())
                        currentGroup = cg;
                lstCounters.Items.Clear();
                if (currentGroup == null)
                {
                    ClearAll();
                    return;
                }
                lbGroupType.Text = currentGroup.propDescription;
                List<Counter> lst = groups[currentGroup.Name];
                foreach (Counter c in lst)
                {
                    ListViewItem lvi = new ListViewItem(c.Name);
                    lvi.SubItems.Add(c.Interval.ToString());
                    lvi.SubItems.Add(c.propMaxTimes);
                    lvi.SubItems.Add(c.Priority.ToString());
                    lvi.Tag = c;
                    lvi.Checked = c.Enabled;
                    lstCounters.Items.Add(lvi);
                }
            }
            else
            {
                ClearAll();
            }
        }
        private void AddSimIteration(InternalCounter cnt,int iteration)
        {
            if (Stats.ContainsKey("__all__") == false)
                Stats["__all__"] = 0;
            Stats["__all__"]++;
            ListViewItem lvi = new ListViewItem((iteration + 1).ToString());
            if (cnt != null)
            {
                lvi.SubItems.Add(cnt.CounterObject.Name);
                lvi.SubItems.Add(string.Format("Enabled: {0}, Times: {1}/{2}", cnt.Enabled, cnt.Times, cnt.MaxTimes));
                if (Stats.ContainsKey(cnt.CounterObject.Name) == false)
                    Stats[cnt.CounterObject.Name] = 0;
                Stats[cnt.CounterObject.Name]++;
            }
            else
            {
                if (Stats.ContainsKey("__empty__") == false)
                    Stats["__empty__"] = 0;
                Stats["__empty__"]++;
                if (cbHideEmptyIterations.Checked)
                    return;
                lvi.SubItems.Add("-");
                lvi.SubItems.Add("");
            }
            lstSim.Items.Add(lvi);
        }
        private int GetValue(string key)
        {
            if (Stats.ContainsKey(key))
                return Stats[key];
            return 0;
        }
        private void AddStatValue(string name,int value,Color c)
        {
            ListViewItem lvi = new ListViewItem(name);
            lvi.SubItems.Add(value.ToString());
            lvi.SubItems.Add(string.Format("{0:P2}",(double)value/(double)GetValue("__all__")));
            lvi.ForeColor = c;
            lstStatistics.Items.Add(lvi);
        }
        private void OnRunSimulation(object sender, EventArgs e)
        {
            Stats.Clear();
            lstStatistics.Items.Clear();
            if (currentGroup == null)
            {
                MessageBox.Show("Please select a group first !");
                comboGroups.Focus();
                return;
            }
            lstSim.Items.Clear();
            if (lstCounters.Items.Count==0)
            {
                MessageBox.Show("No items in this group !");
                return;
            }
            List<InternalCounter> l = new List<InternalCounter>();
            foreach (ListViewItem lvi in lstCounters.Items)
                l.Add(new InternalCounter(lvi));
            int nrIterations = 10;
            if (cbIt25.Checked) nrIterations = 25;
            if (cbIt50.Checked) nrIterations = 50;
            if (cbIt100.Checked) nrIterations = 100;
            if (cbIt200.Checked) nrIterations = 200;
            if (cbIt500.Checked) nrIterations = 500;
            if (cbIt1000.Checked) nrIterations = 1000;

            lstSim.BeginUpdate();

            for (int tr = 0; tr < nrIterations; tr++)
            {
                switch (currentGroup.UpdateMethod)
                {
                    case 0: 
                        RunSimulation_UpdateAllCounters(l, tr); 
                        break;
                    case 1:
                        RunSimulation_UpdateBestCounter(l, tr);
                        break;
                    default: 
                        MessageBox.Show("Implementation for simulation of method #" + currentGroup.UpdateMethod.ToString() + " is not available !"); 
                        tr = nrIterations; // ca sa fortez iesirea din for
                        break;
                }
                    
            }
            lstSim.EndUpdate();
            if (GetValue("__all__") == 0)
                return;
            AddStatValue("Not triggered", GetValue("__empty__"), Color.DarkRed);
            AddStatValue("Triggered", GetValue("__all__") - GetValue("__empty__"), Color.DarkGreen);
            foreach (string key in Stats.Keys)
            {
                if ((key == "__empty__") || (key == "__all__"))
                    continue;
                AddStatValue("  " + key, Stats[key], Color.Black);
            }
            
            
        }
        private void RunSimulation_UpdateAllCounters(List<InternalCounter> counters,int iterationID)
        {
            InternalCounter ret = null;
	        for (int tr = 0; tr < counters.Count; tr++)
	        {
		        InternalCounter gc = counters[tr];
		        if (gc.Enabled == false)
			        continue;
		        if (gc.Value < gc.Interval)
			        gc.Value++;
		        if (gc.Value >= gc.Interval)
		        {
			        if (ret == null)
				        ret = gc;
			        else {
				        if (gc.Priority < ret.Priority)
					        ret = gc;
			        }
		        }
	        }
	        if (ret != null)
	        {
		        ret.Value = 0;
		        ret.Times++;
		        ret.Priority += 100;
		        if ((ret.Times >= ret.MaxTimes) && (ret.MaxTimes != 0))
			        ret.Enabled = false;
	        }
            AddSimIteration(ret, iterationID);
        }
        private void RunSimulation_UpdateBestCounter(List<InternalCounter> counters, int iterationID)
        {
            InternalCounter ret = null;
            for (int tr = 0; tr < counters.Count; tr++)
            {
                InternalCounter gc = counters[tr];
                if (gc.Enabled == false)
                    continue;
                if (ret == null)
                    ret = gc;
                else
                {
                    if (gc.Priority < ret.Priority)
                        ret = gc;
                }
            }
            if (ret != null)
            {
                if (ret.Update())
                {
                    ret.Priority += 100;
                }
                else
                {
                    ret = null;
                }
            }
            AddSimIteration(ret, iterationID);
        }

        private void OnSelectIterations(object sender, EventArgs e)
        {
            cbIt25.Checked = (sender == cbIt25);
            cbIt50.Checked = (sender == cbIt50);
            cbIt100.Checked = (sender == cbIt100);
            cbIt200.Checked = (sender == cbIt200);
            cbIt500.Checked = (sender == cbIt500);
            cbIt1000.Checked = (sender == cbIt1000);
        }

        private void OnSelectBuild(object sender, EventArgs e)
        {
            UpdateGroupLists();
        }
    }
}
