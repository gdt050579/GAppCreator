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
    public partial class AnimationObjectZOrderEditor : Form
    {
        private AnimO.AnimationObject animObj;
        public string ZOrder = "";
        public AnimationObjectZOrderEditor(string _ZOrder, AnimO.AnimationObject _animObj)
        {
            animObj = _animObj;
            InitializeComponent();
            Populate(_ZOrder, animObj);
        }

        private void Populate(string ZOrder, AnimO.AnimationObject animObj)
        {
            List<string> l = Project.StringListToList(ZOrder);
            Dictionary<string, AnimO.GenericElement> d = new Dictionary<string, AnimO.GenericElement>();
            foreach (var e in animObj.Elements)
            {
                d[e.Name] = e;
            }
            // sanity check
            List<string> toDelete = new List<string>();
            foreach (string s in l)
                if (d.ContainsKey(s) == false)
                    toDelete.Add(s);
            foreach (string s in toDelete)
                l.Remove(s);
            
            Dictionary<string, bool> exists = new Dictionary<string, bool>();
            foreach (string s in l)
            {
                exists[s] = true;
            }
            // adaug ce e in plus la urma
            foreach (string k in d.Keys)
            {
                if (exists.ContainsKey(k) == false)
                    l.Add(k);
            }
            lstItems.Items.Clear();
            foreach (string s in l)
            {
                ListViewItem lvi = new ListViewItem(s);
                lvi.SubItems.Add(d[s].propDescription);
                lstItems.Items.Add(lvi);
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            List<string> l = new List<string>();
            foreach (ListViewItem lvi in lstItems.Items)
                l.Add(lvi.Text);
            ZOrder = Project.ListToStringList(l);
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void MoveSelectedItemTo(int add)
        {
            if ((lstItems.SelectedIndices == null) || (lstItems.SelectedIndices.Count == 0))
                return;
            int c_index = lstItems.SelectedIndices[0];
            if (c_index < 0)
                return;
            ListViewItem lvi = lstItems.Items[c_index];
            if (lvi == null)
                return;
            int new_position = c_index + add;
            if (new_position < 0)
                new_position = 0;
            if (new_position >= lstItems.Items.Count - 1)
                new_position = lstItems.Items.Count - 1;
            if (new_position == c_index)
                return; // nu am de ce sa mut
            if (new_position < 0)
                return;
            lstItems.Items.RemoveAt(c_index);
            lstItems.Items.Insert(new_position, lvi);
            lvi.Selected = true;
            lvi.EnsureVisible();
        }

        private void OnMoveUp(object sender, EventArgs e)
        {
            MoveSelectedItemTo(-1);
        }

        private void OnMoveDown(object sender, EventArgs e)
        {
            MoveSelectedItemTo(1);
        }

        private void OnMoveTop(object sender, EventArgs e)
        {
            MoveSelectedItemTo(-100000);
        }

        private void OnMoveBottom(object sender, EventArgs e)
        {
            MoveSelectedItemTo(100000);
        }

        private void OnResetZOrder(object sender, EventArgs e)
        {
            lstItems.Items.Clear();
            foreach (var elem in animObj.Elements)
            {
                ListViewItem lvi = new ListViewItem(elem.Name);
                lvi.SubItems.Add(elem.propDescription);
                lstItems.Items.Add(lvi);
            }
        }
    }
}
