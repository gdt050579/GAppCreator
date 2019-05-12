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
    public partial class TabSelector : Form
    {
        public TabSelector()
        {
            InitializeComponent();
        }
        public void UpdateItems(List<TabPage> pages)
        {
            lstPages.Items.Clear();
            foreach (TabPage tp in pages)
            {
                ProjectFile pf = (ProjectFile)tp.Tag;
                GACEditor ed = (GACEditor)pf.Editor;
                ListViewItem lvi = new ListViewItem(pf.Name);
                lvi.SubItems.Add(ed.TextLength.ToString() + " bytes");
                if (pf.Name.ToLower().EndsWith(".gac"))
                {
                    lvi.ImageKey = "gacfile";
                    if (ed.HasTextBeenModified)
                        lvi.ImageKey = "gacfilenotsaved";
                }
                else if (pf.Name.ToLower().EndsWith(".cpp"))
                {
                    lvi.ImageKey = "cppfile";
                }
                lstPages.Items.Add(lvi);
            }
            lstPages.Items[0].Selected = true;
            lstPages.Items[0].EnsureVisible();
        }
        public void SelectNext(int direction)
        {
            if (lstPages.SelectedIndices.Count==0)
                return;
            if (lstPages.Items.Count == 0)
                return;
            int index = lstPages.SelectedIndices[0] + direction;
            if (index < 0)
                index = lstPages.Items.Count - 1;
            if (index >= lstPages.Items.Count)
                index = 0;
            lstPages.Items[index].Selected = true;
            lstPages.Items[index].EnsureVisible();
        }
        public int GetSelectedTabIndex()
        {
            if (lstPages.SelectedIndices.Count == 0)
                return -1;
            return lstPages.SelectedIndices[0];
        }
    }
}
