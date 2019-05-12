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
    public partial class ApiViewDialog : Form
    {
        List<GACParser.ApiDefinition> def = new List<GACParser.ApiDefinition>();
        List<GACParser.ApiDefinition> sorted;
        Dictionary<string, string> apiTranslations = new Dictionary<string, string>();
        Font boldF;

        public ApiViewDialog(string searchFor)
        {
            InitializeComponent();
            GACParser.GetApiDefinitions(def,apiTranslations);
            sorted = new List<GACParser.ApiDefinition>(def.Count + 16);
            boldF = new Font(lstItems.Font, FontStyle.Bold);
            txSearch.Focus();
            if (searchFor.Length>0)
            {
                txSearch.Text = searchFor;
                cbShowPerfectMatches.Checked = true;
                UpdateItems();
            }
        }

        private static int CompareApiDefinitions(GACParser.ApiDefinition x, GACParser.ApiDefinition y)
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
        private static int CompareMembers(GACParser.Member x, GACParser.Member y)
        {
            return x.Name.CompareTo(y.Name);
        }
        private static int CompareModules(GACParser.Module x, GACParser.Module y)
        {
            return x.Name.CompareTo(y.Name);
        }
        private string ConvertNameSpaceName(string name)
        {
            int index;
            index = name.LastIndexOf('.');
            if (index<0)
                return name;
            if (apiTranslations.ContainsKey(name.Substring(0,index)))
            {
                return apiTranslations[name.Substring(0, index)] + name.Substring(index);
            }
            return name;
        }
        private void UpdateItems()
        {
            txDescription.Text = "";
            lstClassView.Items.Clear();
            lstItems.BeginUpdate();
            lstItems.Items.Clear();
            string txt = ConvertNameSpaceName(txSearch.Text.ToLower().Trim());
            int txtLen = txt.Length;
            bool searchInDefinition = cbSearchInDefinition.Checked;
            if (txt.Contains("."))
                searchInDefinition = true;
            bool showPerfectMatches = cbShowPerfectMatches.Checked;

            if (txt.Length != 0)
            {
                sorted.Clear();
                int foundIndex = -1;
                int ln;
                foreach (GACParser.ApiDefinition ld in def)
                {
                    foundIndex = ld.Name.IndexOf(txt, StringComparison.InvariantCultureIgnoreCase);
                    ln = ld.Name.Length;
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
                    if ((ld.module != null) && (ld.member == null))
                        ld.SearchScore += 2;
                    if ((ld.module != null) && (ld.member != null))
                        ld.SearchScore += 1;

                    sorted.Add(ld);
                }
                sorted.Sort(CompareApiDefinitions);
                foreach (GACParser.ApiDefinition ld in sorted)
                {
                    ListViewItem lvi = new ListViewItem(ld.Name);
                    if (ld.member!=null)
                        lvi.SubItems.Add(ld.member.Type.ToString());
                    else
                        lvi.SubItems.Add(ld.module.Type.ToString());
                    lvi.SubItems.Add(ld.Definition);
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

        private void UpdateModuleInfo(GACParser.Module module = null, GACParser.Member member = null)
        {
            txDescription.Text = "";
            lstClassView.BeginUpdate();
            lstClassView.Items.Clear();
            if (module != null)
            {
                ListViewItem lvi;
                // obiectul
                lstClassView.Items.Add(new ListViewItem(new string[] { "Name", module.Name}, lstClassView.Groups["GeneralInfo"] ));
                lstClassView.Items.Add(new ListViewItem(new string[] { "Type", module.Type.ToString() }, lstClassView.Groups["GeneralInfo"]));
                
                // constructori
                List<GACParser.Member> lst = new List<GACParser.Member>();
                foreach (string s in module.Members.Keys)
                {
                    lst.Add(module.Members[s]);
                }
                lst.Sort(CompareMembers);
                foreach (GACParser.Member m in lst)
                {
                    lvi = new ListViewItem();
                    lvi.Tag = m;

                    switch (m.Type)
                    {
                        case GACParser.MemberType.Constant:
                            lvi.Text = m.DataType;
                            lvi.SubItems.Add(m.Name);
                            lvi.Group = lstClassView.Groups["Values"];
                            lstClassView.Items.Add(lvi);
                            break;
                        case GACParser.MemberType.Variable:
                            lvi.Text = m.DataType;                            
                            if (m.ArrayCount>0)
                                lvi.SubItems.Add(m.Name+" "+m.ArrayGacCode);
                            else
                                lvi.SubItems.Add(m.Name);
                            lvi.Group = lstClassView.Groups["Members"];
                            lstClassView.Items.Add(lvi);
                            break;
                        case GACParser.MemberType.Function:
                            lvi.Text = m.DataType;
                            lvi.SubItems.Add(m.Name + " " + m.GetFunctionParameters());
                            if (m.Virtual)
                                lvi.Group = lstClassView.Groups["VirtualMethods"];
                            else
                                lvi.Group = lstClassView.Groups["Methods"];
                            lstClassView.Items.Add(lvi);
                            if (m.Overrides != null) { 
                                foreach (GACParser.Member mo in m.Overrides)
                                {
                                    ListViewItem lvi2 = new ListViewItem("");
                                    lvi2.SubItems.Add(m.Name + " " + mo.GetFunctionParameters());
                                    if (m.Virtual)
                                        lvi2.Group = lstClassView.Groups["VirtualMethods"];
                                    else
                                        lvi2.Group = lstClassView.Groups["Methods"];
                                    lvi2.ForeColor = Color.Gray;
                                    lvi2.Tag = mo;
                                    lstClassView.Items.Add(lvi2);
                                }
                            }
                            break;
                        case GACParser.MemberType.Constructor:
                            lvi.Text = "<constructor>";                            
                            lvi.SubItems.Add(m.GetFunctionParameters());
                            lvi.Group = lstClassView.Groups["Constructor"];
                            lstClassView.Items.Add(lvi);
                            if (m.Overrides != null) { 
                                foreach (GACParser.Member mo in m.Overrides)
                                {
                                    ListViewItem lvi2 = new ListViewItem("");
                                    lvi2.SubItems.Add("<constructor> " + mo.GetFunctionParameters());
                                    lvi2.Group = lstClassView.Groups["Constructor"];
                                    lvi2.ForeColor = Color.Gray;
                                    lvi2.Tag = mo;
                                    lstClassView.Items.Add(lvi2);
                                }
                            }
                            break;
                        case GACParser.MemberType.Destructor:
                            lvi.Text = "<destructor> ";
                            lvi.SubItems.Add("()");
                            lvi.Group = lstClassView.Groups["Constructor"];
                            lstClassView.Items.Add(lvi);
                            lvi.ForeColor = Color.Red;
                            break;
                    }
                    if ((member!=null) && (m.Name.Equals(member.Name)))
                    {
                        lvi.Selected = true;                                                
                    }
                }
                // module
                if (module.Modules != null)
                {
                    List<GACParser.Module> lstM = new List<GACParser.Module>();
                    foreach (string s in module.Modules.Keys)
                    {
                        lstM.Add(module.Modules[s]);
                    }
                    lstM.Sort(CompareModules);
                    foreach (GACParser.Module mod in lstM)
                    {
                        lstClassView.Items.Add(new ListViewItem(new string[] { mod.Type.ToString(), mod.Name }, lstClassView.Groups["Modules"]));
                    }
                }

            }
            lstClassView.EndUpdate();
            if (lstClassView.SelectedItems.Count > 0)
                lstClassView.EnsureVisible(lstClassView.SelectedIndices[0]);
        }

        private void OnOK(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            UpdateItems();
        }

        private void txSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))
                lstItems.Focus();
        }

        private void lstItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                txSearch.Focus();
        }

        private void ApiViewDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Alt) && (e.KeyCode == Keys.S))
                txSearch.Focus();
            if (e.KeyCode == Keys.Back)
                txSearch.Focus();
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstItems.SelectedItems.Count == 0)
                UpdateModuleInfo();
            else
            {
                GACParser.ApiDefinition ld = (GACParser.ApiDefinition)lstItems.SelectedItems[0].Tag;
                UpdateModuleInfo(ld.module, ld.member);
            }
        }

        private void lstClassView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstClassView.SelectedItems.Count == 0)
                txDescription.Text = "";
            else
            {
                GACParser.Member m = lstClassView.SelectedItems[0].Tag as GACParser.Member;
                if (m == null)
                    txDescription.Text = "";
                else
                    txDescription.Text = m.GetToolTip("").Replace("\n","\r\n");
            }
        }
    }
}
