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
    public partial class GoToDefinitionDialog : Form
    {
        public GACParser.LocalDefinition ResultedItem = null;
        List<GACParser.LocalDefinition> def;
        List<GACParser.LocalDefinition> sorted;
        Font boldF;
        string FileName;
        public GoToDefinitionDialog(string fileName,bool searchInCurrentFileOnly,string prototype)
        {
            InitializeComponent();
            def = GACParser.GetLocalDefinitions();
            sorted = new List<GACParser.LocalDefinition>(def.Count + 16);
            boldF = new Font(lstItems.Font, FontStyle.Bold);
            cbSearchInCurrentFileOnly.Checked = searchInCurrentFileOnly;
            if (prototype.Length > 0)
            {
                txSearch.Text = prototype;
                cbSearchInPrototype.Checked = true;
                cbShowPerfectMatches.Checked = true;
            }
            UpdateItems();
            txSearch.Focus();
            FileName = fileName.ToLower();
            if (FileName.Length == 0)
                Text = "Search Definition";
            else
                Text = "Search Definition (current file is '" + FileName + "')";
        }
        private static int CompareLocalDefinitions(GACParser.LocalDefinition x, GACParser.LocalDefinition y)
        {
            if (x.SearchScore > y.SearchScore)
                return -1;
            if (x.SearchScore < y.SearchScore)
                return 1;
            int res = string.Compare(x.Name, y.Name, true);
            if (res < 0)
                return 1;
            if (res > 0)
                return -1;
            return 0;
        }
        private void UpdateItems()
        {
            lstItems.BeginUpdate();
            lstItems.Items.Clear();
            string txt = txSearch.Text.ToLower().Trim();
            int txtLen = txt.Length;
            bool searchInDefinition = cbSearchInDefinition.Checked;
            if (txt.Contains("."))
                searchInDefinition = true;
            bool searchInPrototype = cbSearchInPrototype.Checked;
            bool searchInCurrentFile = cbSearchInCurrentFileOnly.Checked;
            bool showPerfectMatches = cbShowPerfectMatches.Checked;
            
            if (txt.Length!=0)
            {
                sorted.Clear();
                int foundIndex = -1;
                int ln;
                foreach (GACParser.LocalDefinition ld in def)
                {
                    if (((ld.Type == GACParser.LocalDefinitionType.Application) || (ld.Type == GACParser.LocalDefinitionType.Scene)) && (cbScenes.Checked == false))
                        continue;
                    if ((ld.Type == GACParser.LocalDefinitionType.Class) && (cbClasses.Checked == false))
                        continue;
                    if ((ld.Type == GACParser.LocalDefinitionType.Constructor) && (cbConstructors.Checked == false))
                        continue;
                    if ((ld.Type == GACParser.LocalDefinitionType.Destructor) && (cbDestructor.Checked == false))
                        continue;
                    if ((ld.Type == GACParser.LocalDefinitionType.Member) && (cbMembers.Checked == false))
                        continue;
                    if ((ld.Type == GACParser.LocalDefinitionType.Method) && (cbMethods.Checked == false))
                        continue;
                    if ((ld.Type == GACParser.LocalDefinitionType.LocalVariable) && (cbLocalVariables.Checked == false))
                        continue;
                    if ((ld.Type == GACParser.LocalDefinitionType.Parameter) && (cbParameters.Checked == false))
                        continue;
                    if ((searchInCurrentFile) && (ld.File.Equals(FileName, StringComparison.InvariantCultureIgnoreCase) == false))
                        continue;

                    if (searchInPrototype)
                    {
                        foundIndex = ld.Prototype.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase);
                        ln = ld.Prototype.Length;
                    } else
                    if (searchInDefinition) {
                        foundIndex = ld.Definition.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase);
                        ln = ld.Definition.Length;
                    } else {
                        foundIndex = ld.Name.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase);
                        ln = ld.Name.Length;
                    }
                    if (foundIndex < 0)
                        continue;
                    if (foundIndex == 0)
                    {
                        if (txtLen == ln)
                            ld.SearchScore = 100; // match exact
                        else
                            ld.SearchScore = 50; // starts with
                    }
                    else if (foundIndex + txtLen == ln)
                    {
                        ld.SearchScore = 40; // endswith
                    }
                    else ld.SearchScore = 0;
                    if ((showPerfectMatches) && (ld.SearchScore < 100))
                        continue;
                    // in functie de ce este adaug la scor
                    switch (ld.Type)
                    {
                        case GACParser.LocalDefinitionType.Scene: ld.SearchScore += 9; break;
                        case GACParser.LocalDefinitionType.Application: ld.SearchScore += 8; break;
                        case GACParser.LocalDefinitionType.Class: ld.SearchScore += 7; break;
                        case GACParser.LocalDefinitionType.Constructor: ld.SearchScore += 6; break;
                        case GACParser.LocalDefinitionType.Destructor: ld.SearchScore += 6; break;
                        case GACParser.LocalDefinitionType.Method: ld.SearchScore += 5; break;
                        case GACParser.LocalDefinitionType.Member: ld.SearchScore += 4; break;
                        case GACParser.LocalDefinitionType.Parameter: ld.SearchScore += 3; break;
                        case GACParser.LocalDefinitionType.LocalVariable: ld.SearchScore += 2; break;
                    }
                    // fisier - au prioritate cele din fisierul curent
                    if (ld.File.Equals(FileName, StringComparison.InvariantCultureIgnoreCase))
                        ld.SearchScore += 10;
                    sorted.Add(ld);
                }
                sorted.Sort(CompareLocalDefinitions);
                foreach (GACParser.LocalDefinition ld in sorted)
                {
                    ListViewItem lvi = new ListViewItem(ld.Name);
                    lvi.SubItems.Add(ld.Type.ToString());
                    lvi.SubItems.Add(ld.Definition);
                    lvi.SubItems.Add(ld.Prototype);
                    lvi.Tag = ld;
                    if (ld.SearchScore >= 100)
                        lvi.Font = boldF;
                    lstItems.Items.Add(lvi);
                }
                if (lstItems.Items.Count > 0)
                    lstItems.Items[0].Selected = true;
                if (searchInDefinition)
                    lbInfo.Text = "Search in definitions (found " + lstItems.Items.Count.ToString() + " / " + def.Count.ToString() + ")";
                else
                    lbInfo.Text = "Search for item (found " + lstItems.Items.Count.ToString() + " / " + def.Count.ToString() + ")";
            }
            else
            {
                lbInfo.Text = "Nothing selected from " + def.Count.ToString() + " items";
            }
            lstItems.EndUpdate();
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (lstItems.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select a item first !");
                lstItems.Focus();
            }
            ResultedItem = (GACParser.LocalDefinition)lstItems.SelectedItems[0].Tag;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            UpdateItems();
        }

        private void txSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))
                lstItems.Focus();
        }

        private void GoToDefinition_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Alt) && (e.KeyCode == Keys.S))
                txSearch.Focus();
            if (e.KeyCode == Keys.Back)
                txSearch.Focus();
        }

        private void OnChangeFilter(object sender, EventArgs e)
        {
            UpdateItems();
        }

        private void lstItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnOK(null, null);
        }

        private void lstItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                txSearch.Focus();
        }
    }
}
