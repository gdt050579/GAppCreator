using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GAppCreator;

namespace GAppCreator
{
    public partial class FontVersionEditor : Form
    {
        bool Landscape;
        public static Project prj = null;
        public string ResultVersion;
        public FontVersionEditor(string versions)
        {
            InitializeComponent();

            if (prj == null)
                return;

            for (int tr=0;tr<prj.BuildConfigurations.Count;tr++)
            {
                DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
                col.HeaderText = prj.BuildConfigurations[tr].Name;
                dg.Columns.Add(col);
            }

            Glyph.GlyphVersionInfo vi = Glyph.GetVersion(versions);
            foreach (string rezolution in vi.Rezolutions.Keys)
            {
                int index = AddRezolution(rezolution);
                if (index < 0)
                    continue;
                foreach (string bld in vi.Rezolutions[rezolution])
                {
                    int cindex = GetColumnIndex(bld);
                    if (cindex>0)
                    {
                        DataGridViewCheckBoxCell cb = (DataGridViewCheckBoxCell)dg.Rows[index].Cells[cindex];
                        cb.Value = true;
                    }
                }
            }
            Size size1 = Project.SizeToValues(prj.DesignResolution);
            Landscape = size1.Width >= size1.Height;
            foreach (Size sz in Project.Resolutions)
            {
                if (Landscape)
                    btnAddRezolution.DropDownItems.Add(String.Format("{0} x {1}",sz.Width,sz.Height)).Click += OnAddStandardRezolution;
                else
                    btnAddRezolution.DropDownItems.Add(String.Format("{0} x {1}", sz.Height, sz.Width)).Click += OnAddStandardRezolution;
            }
        }


        private int GetColumnIndex(string name)
        {
            name = name.ToLower();
            for (int tr=1;tr<dg.Columns.Count;tr++)
                if (dg.Columns[tr].HeaderText.ToLower().Equals(name))
                    return tr;
            return -1;
        }
        private int GetResolutionIndex(string resolution)
        {
            Size rez = Project.SizeToValues(resolution);
            if ((rez.Width == 0) || (rez.Height == 0))
                return -1;
            for (int tr = 0; tr < dg.Rows.Count; tr++)
            {
                Size r = Project.SizeToValues(dg.Rows[tr].Cells[0].Value.ToString());
                if ((rez.Width == r.Width) && (rez.Height == r.Height))
                    return tr;
            }
            return -1;
        }
        private int AddRezolution(string rezolution)
        {
            Size rez = Project.SizeToValues(rezolution);
            if ((rez.Width == 0) || (rez.Height == 0))
                return -1;
            int index = GetResolutionIndex(rezolution);
            if (index >= 0)
                return index;
            dg.Rows.Add();
            dg.Rows[dg.Rows.Count - 1].Cells[0].Value = String.Format("{0} x {1}", rez.Width, rez.Height);
            return dg.Rows.Count - 1;
        }

        void OnAddStandardRezolution(object sender, EventArgs e)
        {
            ToolStripItem itm = (ToolStripItem)sender;
            AddRezolution(itm.Text);
        }
        private void AddCustomRezolution(object sender, EventArgs e)
        {
            Size rez = Project.SizeToValues(txCustomResolution.Text);
            if ((rez.Width == 0) || (rez.Height == 0))
            {
                MessageBox.Show("Invalid resolution (format is 'width x height'): " + txCustomResolution.Text);
                txCustomResolution.Focus();
                return;
            }
            AddRezolution(txCustomResolution.Text);
            txCustomResolution.Text = "";
        }

        private void OnOK(object sender, EventArgs e)
        {
            // design resolution trebuie sa fie checked pentru Develop
            int index = GetResolutionIndex(prj.DesignResolution);
            if (index==-1)
            {
                MessageBox.Show("Missing design resolution (" + prj.DesignResolution + ") !");
                return;
            }
            if ((dg.Rows[index].Cells[1].Value==null) || (((bool)(dg.Rows[index].Cells[1].Value)) != true))
            {
                MessageBox.Show("Design resolution should always be available for the develop build !");
                return;
            }
            ResultVersion = "";
            for (int tr = 0; tr < dg.Rows.Count;tr++)
            {
                string s = dg.Rows[tr].Cells[0].Value.ToString() + "[";
                // parcurg build-urile
                for (int gr = 0; gr < prj.BuildConfigurations.Count; gr++)
                    if ((dg.Rows[tr].Cells[gr + 1].Value!=null) && (((bool)(dg.Rows[tr].Cells[gr + 1].Value)) == true))
                        s += prj.BuildConfigurations[gr].Name + ",";
                // daca am macar un build
                if (s.EndsWith(","))
                {
                    s = s.Substring(0, s.Length - 1);
                    ResultVersion += s;
                    ResultVersion += "] , ";
                }
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnDeleteResolution(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count==0)
            {
                MessageBox.Show("You have to select at least one resolution to delete !");
                dg.Focus();
                return;
            }
            int rowDesignRez = GetResolutionIndex(prj.DesignResolution);
            Dictionary<int, string> d = new Dictionary<int, string>();
            String RezList = "";
            bool foundDesignRezolution = false;
            for (int tr = 0; tr < dg.SelectedCells.Count; tr++)
            {
                int row = dg.SelectedCells[tr].RowIndex;
                if (row == rowDesignRez)
                {
                    foundDesignRezolution = true;
                    continue;
                }
                if (d.ContainsKey(row))
                    continue;
                if (dg.Rows[row].Cells[0].Value!=null)
                {
                    Size r = Project.SizeToValues(dg.Rows[row].Cells[0].Value.ToString());
                    if ((r.Width>0) && (r.Height>0))
                    {
                        d[row] = String.Format("{0} x {1}", r.Width, r.Height);
                        if (RezList.Length > 0)
                            RezList += " , ";
                        RezList += d[row];
                    }
                }
            }
            if (d.Count==0)
            {
                if (foundDesignRezolution)
                {
                    MessageBox.Show("Design resolution (" + prj.DesignResolution + ") can not be deleted !");
                }
                else
                {
                    MessageBox.Show("Internal errors - no valid resolutions found !");
                }
                dg.Focus();
                return;
            }
            if (MessageBox.Show("Delete the following resolutions ?\n"+RezList,"Delete ?", MessageBoxButtons.YesNo)== System.Windows.Forms.DialogResult.Yes)
            {
                List<DataGridViewRow> toDelete = new List<DataGridViewRow>();
                foreach (int index in d.Keys)
                    toDelete.Add(dg.Rows[index]);
                foreach (DataGridViewRow row in toDelete)
                    dg.Rows.Remove(row);
            }
            
        }
    }
}
