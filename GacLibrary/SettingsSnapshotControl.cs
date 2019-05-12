using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GAppCreator
{    
    public partial class SettingsSnapshotControl : UserControl
    {
        static public Project CurrentProject = null;
        private int doubleClickedTicked = 0;
        public delegate void OnSelectSnapshotHandler(object sender);
        public event OnSelectSnapshotHandler OnSelectSnapshot = null;
        public SettingsSnapshotControl()
        {
            InitializeComponent();
            lstSnapshots.Dock = DockStyle.Fill;
            lstSnapshots.Groups.Add(new ListViewGroup("Default settings"));
            lstSnapshots.Groups.Add(new ListViewGroup("Saved snapshots"));
            lstSnapshots.ShowGroups = true;
            treeSnapshots.Dock = DockStyle.Fill;
            txFilter.Text = "";
            OnChangeViewMode(cbListView, null);
            
        }
        private void FilterNode(TreeNode parent,bool checkForFilter, string txt)
        {
            foreach (TreeNode tn in parent.Nodes)
            {
                if ((checkForFilter) && (tn.Text.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase)>=0))
                {
                    tn.BackColor = Color.LightGreen;
                }
                else
                {
                    tn.BackColor = treeSnapshots.BackColor;
                }
                FilterNode(tn, checkForFilter, txt);
            }
        }
        private void ColorizeTreeByFilter()
        {
            string txt = txFilter.Text;
            bool checkFilter = txt.Length > 0;
            foreach (TreeNode tn in treeSnapshots.Nodes)
                FilterNode(tn, checkFilter, txt);

        }
        private void UpdateNodeChildern(TreeNode parent,Dictionary<int, SettingsSnapshotChildrenList> d, int ID)
        {
            TreeNode tn = new TreeNode();
            SettingsSnapshotChildrenList inf = d[ID];
            if (inf.Snapshot==null)
            {
                tn.Text = "!! Missing !! (Snapshot with ID: " + ID.ToString() + ") - Last state will be used";
                tn.Tag = 0;
                tn.ForeColor = Color.Red;
            }
            else
            {
                tn.Text = inf.Snapshot.Info;
                tn.ToolTipText = "Added on: " + inf.Snapshot.Added.ToString();
                tn.Tag = inf.Snapshot.ID;
            }
            parent.Nodes.Add(tn);
            //MessageBox.Show("Add [" + tn.Text + "] to [" + parent.Text + "]");
            foreach (SettingsSnapshot ds in inf.Children)
                UpdateNodeChildern(tn, d, ds.ID);
        }
        private void UpdateSnapshotTree()
        {
            treeSnapshots.Nodes.Clear();
            TreeNode tn1 = new TreeNode("Default settings                             ");
            tn1.NodeFont = new Font("Arial",10,FontStyle.Bold);
            tn1.Tag = 0;
            TreeNode tn = new TreeNode("None (no settings will be loaded at startup) !");
            tn.Tag = -1;
            tn.ForeColor = Color.Blue;
            tn1.Nodes.Add(tn);
            tn = new TreeNode("Last state (application will start from its last state) !");
            tn.Tag = 0;
            tn.ForeColor = Color.Blue;
            tn1.Nodes.Add(tn);
            treeSnapshots.Nodes.Add(tn1);
            

            if (CurrentProject == null)
                return;
            treeSnapshots.BeginUpdate();
            Dictionary<int, SettingsSnapshotChildrenList> d = CurrentProject.SettingsSnapshots.GetRelations();
            TreeNode tn2 = new TreeNode("Saved snapshots                        ");
            tn2.NodeFont = new Font("Arial",10,FontStyle.Bold);
            tn2.Tag = 0;
            treeSnapshots.Nodes.Add(tn2);
            // caut pe toti care au parent cu 0 si adaug
            foreach (SettingsSnapshot ds in CurrentProject.SettingsSnapshots.Snapshots)
            {
                if (ds.ParentID == 0)
                    UpdateNodeChildern(tn2, d, ds.ID);
            }
            doubleClickedTicked = 0;
            treeSnapshots.ExpandAll();
            treeSnapshots.EndUpdate();            
            ColorizeTreeByFilter();
            
        }
        private void UpdateSnapshotList()
        {
            lstSnapshots.Items.Clear();
            ListViewItem itm = new ListViewItem("-");itm.SubItems.Add("None (no settings will be loaded at startup) !");
            itm.Tag = -1;
            itm.Group = lstSnapshots.Groups[0];
            itm.SubItems.Add("");itm.SubItems.Add("");
            itm.ForeColor = Color.Blue;
            lstSnapshots.Items.Add(itm);
            itm = new ListViewItem("-");itm.SubItems.Add("Last state (application will start from its last state) !");
            itm.Tag = 0;
            itm.Group = lstSnapshots.Groups[0];
            itm.SubItems.Add("");itm.SubItems.Add("");
            itm.ForeColor = Color.Blue;
            lstSnapshots.Items.Add(itm);

            if (CurrentProject == null)
                return;
            lstSnapshots.BeginUpdate();
            string textToFind = txFilter.Text;
            bool checkFilter = textToFind.Length>0;
            foreach (SettingsSnapshot ds in CurrentProject.SettingsSnapshots.Snapshots)
            {
                string dateAdded = ds.Added.ToString();
                if (checkFilter)
                {
                    if ((ds.Info.IndexOf(textToFind, StringComparison.InvariantCultureIgnoreCase) < 0) && (dateAdded.IndexOf(textToFind, StringComparison.InvariantCultureIgnoreCase) < 0))
                        continue;
                }
                ListViewItem lvi = new ListViewItem(ds.ID.ToString());
                lvi.SubItems.Add(ds.Info);
                lvi.Tag = ds.ID;
                lvi.SubItems.Add(dateAdded);
                if (ds.ParentID==0)
                    lvi.SubItems.Add("None");
                else
                    lvi.SubItems.Add(ds.ParentID.ToString());
                lvi.Group = lstSnapshots.Groups[1];
                lstSnapshots.Items.Add(lvi);
            }
            lstSnapshots.EndUpdate();            
        }
        public bool HasSnapshotSelecteed()
        {
            if (lstSnapshots.Visible)
                return (lstSnapshots.SelectedItems.Count == 1);
            else
                return (treeSnapshots.SelectedNode != null);
        }
        public int GetSelectedSnapshot()
        {
            if (lstSnapshots.Visible)
            {
                if (lstSnapshots.SelectedItems.Count == 1)
                    return (int)lstSnapshots.SelectedItems[0].Tag;
                return 0;
            }
            else
            {
                if (treeSnapshots.SelectedNode != null)
                    return (int)treeSnapshots.SelectedNode.Tag;
                return 0;
            }
        }
        public string GetSelectedSnapshotStringRepresentation()
        {
            if (CurrentProject == null)
                return "";
            return CurrentProject.SettingsSnapshots.GetStringRepresentation(GetSelectedSnapshot());
        }
        private void OnFilterChanged(object sender, EventArgs e)
        {
            if (lstSnapshots.Visible)
                UpdateSnapshotList();
            else
                ColorizeTreeByFilter();
        }
        public void SetCurrentSnapshotID(int snapshotID)
        {
            for (int tr=0;tr<lstSnapshots.Items.Count;tr++)
            {
                if ((int)(lstSnapshots.Items[tr].Tag) == snapshotID)
                {
                    lstSnapshots.Items[tr].Selected = true;
                    lstSnapshots.Items[tr].EnsureVisible();
                    return;
                }
            }
        }
        public void SetCurrentSnapshotID(string stringRepresentationForSnapshotID)
        {
            if (CurrentProject != null)
                SetCurrentSnapshotID(CurrentProject.SettingsSnapshots.GetSnapshotID(stringRepresentationForSnapshotID));
        }

        private void OnDoubleClickedOnList(object sender, MouseEventArgs e)
        {
            if ((HasSnapshotSelecteed()) && (OnSelectSnapshot!=null))
            {
                OnSelectSnapshot(this);
            }
        }

        private void OnChangeViewMode(object sender, EventArgs e)
        {
            bool res;
            
            // ListView
            res = sender == cbListView;
            if ((cbListView.Checked == false) && (res == true))
                UpdateSnapshotList();
            cbListView.Checked = res;
            lstSnapshots.Visible = res;


            // TreeView
            res = sender == cbTreeView;
            if ((cbTreeView.Checked == false) && (res == true))
                UpdateSnapshotTree();
            cbTreeView.Checked = res;
            treeSnapshots.Visible = res;
        }

        public void RefreshSnapshotLists()
        {
            if (lstSnapshots.Visible)
                UpdateSnapshotList();
            else
                UpdateSnapshotTree();
        }

        private void OnDoubleClickedOnTree(object sender, MouseEventArgs e)
        {
            if (HasSnapshotSelecteed())
            {
                if (OnSelectSnapshot != null)
                    OnSelectSnapshot(this);
            }
        }

        private void OnRename(object sender, EventArgs e)
        {
            if (HasSnapshotSelecteed()==false)
            {
                MessageBox.Show("Please select one item before renaming !");
                return;
            }
            int id = GetSelectedSnapshot();
            if (id<1)
            {
                MessageBox.Show("Items that refer to no state or last state can not be renamed !");
                return;
            }
            // iterez prin lista
            SettingsSnapshot ds = CurrentProject.SettingsSnapshots.GetSnapshotFromID(id);
            if (ds==null)
            {
                MessageBox.Show("Internal error - unable to find snapshot with ID: " + id.ToString());
                return;
            }
            InputBox ib = new InputBox("Enter new snapshot information !", ds.Info);
            ib.TopMost = true;
            if (ib.ShowDialog() == DialogResult.OK)
            {
                ds.Info = ib.StringResult;
                if (lstSnapshots.Visible)
                    lstSnapshots.SelectedItems[0].SubItems[1].Text = ds.Info;
                else
                    treeSnapshots.SelectedNode.Text = ds.Info;

            }
        }

        private void PopulateChildrenList(List<SettingsSnapshot> l, Dictionary<int, SettingsSnapshotChildrenList> d, int id)
        {
            foreach (SettingsSnapshot ss in d[id].Children)
            {
                l.Add(ss);
                PopulateChildrenList(l, d, ss.ID);
            }
        }
        private List<SettingsSnapshot> CreateAllChildrenList(Dictionary<int, SettingsSnapshotChildrenList> d, int id)
        {
            List<SettingsSnapshot> l = new List<SettingsSnapshot>();
            PopulateChildrenList(l, d, id);
            return l;
        }
        private string SnapshotIDToFile(int ID)
        {
            return Path.Combine(CurrentProject.ProjectPath, "Bin", ID.ToString("X8") + ".snapshot");
        }
        private void DeleteSnapshot(SettingsSnapshot ss)
        {
            // sterg si fisierul corespunzator
            if (Disk.DeleteFile(SnapshotIDToFile(ss.ID), CurrentProject.EC) == false)
                CurrentProject.ShowErrors();
            CurrentProject.SettingsSnapshots.Snapshots.Remove(ss);
        }
        private void OnDeleteSnapshot(object sender, EventArgs e)
        {
            if (HasSnapshotSelecteed() == false)
            {
                MessageBox.Show("Please select one item before remove it !");
                return;
            }
            int id = GetSelectedSnapshot();
            if (id < 1)
            {
                MessageBox.Show("Items that refer to no state or last state can not be deleted !");
                return;
            }
            SettingsSnapshot ds = CurrentProject.SettingsSnapshots.GetSnapshotFromID(id);
            if (ds == null)
            {
                MessageBox.Show("Internal error - unable to find snapshot with ID: " + id.ToString());
                return;
            }
            if (MessageBox.Show("Delete '" + ds.Info + "' ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            // verific daca are cumva copii
            Dictionary<int, SettingsSnapshotChildrenList> d = CurrentProject.SettingsSnapshots.GetRelations();
            DialogResult res = DialogResult.No;
            List<SettingsSnapshot> allChildren = CreateAllChildrenList(d, id);
            if (allChildren.Count > 0)
            {
                res = MessageBox.Show("Delete all " + allChildren.Count.ToString() + " snapshots created from '" + ds.Info + "' ?", "Delete", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Cancel)
                    return;
            }
            // ma apuc sa sterg
            if (res== DialogResult.Yes)
            {
                // sterg toti copii
                // nu am ce sa mai linkez - doar sterg item-urile
                foreach (SettingsSnapshot ss in allChildren)
                {
                    DeleteSnapshot(ss);
                }
                DeleteSnapshot(ds);
            }
            else
            {
                // ma sterg doar pe mine
                // linkez copii mei la parintele meu
                foreach (SettingsSnapshot ss in d[id].Children)
                    ss.ParentID = ds.ParentID;
                DeleteSnapshot(ds);
            }
            RefreshSnapshotLists();
        }

        private void OnImportSnapshot(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "Import snapshot file";
            fd.Filter = "Snapshot files|*.snapshot|All files|*.*";
            fd.DefaultExt = ".snapshot";
            fd.Multiselect = false;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                InputBox ib = new InputBox("Snapshot info", "");
                ib.TopMost = true;
                if (ib.ShowDialog() == DialogResult.OK)
                {
                    int newID = CurrentProject.SettingsSnapshots.GetNewID();
                    if (Disk.Copy(fd.FileName, SnapshotIDToFile(newID), null)==false)
                    {
                        MessageBox.Show("Failed to import: " + fd.FileName);
                        return;
                    }
                    CurrentProject.SettingsSnapshots.AddNewSnapshot(newID, 0, ib.StringResult);
                    RefreshSnapshotLists();
                }
            }
        }

        private void OnChangeParent(object sender, EventArgs e)
        {
            if (HasSnapshotSelecteed() == false)
            {
                MessageBox.Show("Please select one item before setting its parent !");
                return;
            }
            int id = GetSelectedSnapshot();
            if (id < 1)
            {
                MessageBox.Show("Items that refer to no state or last state have no parents !");
                return;
            }
            SettingsSnapshot ds = CurrentProject.SettingsSnapshots.GetSnapshotFromID(id);
            if (ds == null)
            {
                MessageBox.Show("Internal error - unable to find snapshot with ID: " + id.ToString());
                return;
            }
            List<string> lst = new List<string>();
            lst.Add("<No parent>");
            Dictionary<int, SettingsSnapshotChildrenList> d = CurrentProject.SettingsSnapshots.GetRelations();
            List<SettingsSnapshot> allChildren = CreateAllChildrenList(d, id);
            Dictionary<int, bool> allChildrenDict = new Dictionary<int, bool>();
            foreach (SettingsSnapshot ss in allChildren)
                allChildrenDict[ss.ID] = true;
            // ma adaug si pe mine
            allChildrenDict[ds.ID] = true;
            // adaug in lista
            foreach (SettingsSnapshot ss in CurrentProject.SettingsSnapshots.Snapshots)
            {
                if (allChildrenDict.ContainsKey(ss.ID))
                    continue;
                lst.Add(CurrentProject.SettingsSnapshots.GetStringRepresentation(ss));
            }
            InputBox ib = new InputBox("Select parent",lst.ToArray(),lst[0]);
            ib.TopMost = true;
            if (ib.ShowDialog() == DialogResult.OK)
            {
                int parentID = CurrentProject.SettingsSnapshots.GetSnapshotID(ib.StringResult);
                if (parentID<0)
                    parentID = 0;
                if (parentID != ds.ParentID)
                {
                    ds.ParentID = parentID;
                    RefreshSnapshotLists();
                }
            }
        }

        private void OnValidate(object sender, EventArgs e)
        {
            foreach (SettingsSnapshot ss in CurrentProject.SettingsSnapshots.Snapshots)
            {
                if (File.Exists(SnapshotIDToFile(ss.ID))==false)
                {
                    MessageBox.Show("Snapshot with ID: " + ss.ID.ToString() + " => '" + ss.Info + "' does not have a file on the disk. It can not be used !. You should delete this snapshot !");
                    return;
                }
            }
            MessageBox.Show("No problems found !");
        }


        private void treeSnapshots_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            //int dif = System.Environment.TickCount - doubleClickedTicked;
            //e.Cancel = dif < 2000;
        }

        private void treeSnapshots_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            //int dif = System.Environment.TickCount - doubleClickedTicked;
            //e.Cancel = dif < 2000;
        }

        private void treeSnapshots_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 1)
                doubleClickedTicked = System.Environment.TickCount;
            else
                doubleClickedTicked = 0;
        }
    }
}
