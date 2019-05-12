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
    public partial class EnumSelectValueControl : UserControl, IDataGridUserControl
    {
        private static Project prj;
        private ITerminateEdit editControl = null;
        private Enumeration enm = null;
        private bool IgnoreCheckEvent;
        private string EnumName = "";

        

        public EnumSelectValueControl()
        {
            InitializeComponent();
            IgnoreCheckEvent = false;
        }

        public static void InitControl(Project _prj)
        {
            prj = _prj;
        }

        private void OnCheckItem(object sender, ItemCheckedEventArgs e)
        {
            if (enm == null)
                return;
            if (enm.IsBitSet)
                return;
            if (e.Item == null)
                return;
            if (e.Item.Checked == false)
                return;
            if (IgnoreCheckEvent)
                return;
            IgnoreCheckEvent = true;
            if (lstValues.Items != null)
            {
                for (int tr = 0; tr < lstValues.Items.Count; tr++)
                    if (lstValues.Items[tr]!=null)
                        lstValues.Items[tr].Checked = lstValues.Items[tr] == e.Item;
            }
            IgnoreCheckEvent = false;
            if (e.Item.Selected==false)
                e.Item.Selected = true;
        }
        
        private void OnChangeItem(object sender, EventArgs e)
        {
            if (enm == null)
                return;
            if (enm.IsBitSet)
                return;
            if (lstValues.SelectedItems.Count == 1)
            {
                lstValues.SelectedItems[0].Checked = true;
            }
        }

        public object GetResultedValue()
        {
            if (enm == null)
                return null;
            string s = "";
            foreach (ListViewItem lvi in lstValues.Items)
            {
                if (lvi.Checked)
                    s += lvi.Text + ", ";
            }
            // daca nu e bitset trebuie sa fie o valoare selectata
            if ((s.Length == 0) && (enm.IsBitSet == false))
                return null;
            if (s.EndsWith(", "))
                s = s.Substring(0, s.Length - 2);
            return new KeyValuePair<string, string>(EnumName, s);
        }

        public string GetStringRepresentation(object o)
        {
            if (o == null)
                return "";
            if (o.GetType() == typeof(KeyValuePair<string, string>))
                return ((KeyValuePair<string, string>)o).Value;
            return "?";
        }

        public bool HasCustomPaint()
        {
            return false;
        }

        public void DrawCell(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value,Font fnt,StringFormat sfmt)
        {
            throw new NotImplementedException();
        }

        public void Init(object o, ITerminateEdit edit)
        {
            if ((o!=null) && (prj!=null))
            {
                KeyValuePair<string, string> pair = (KeyValuePair<string, string>)o;
                EnumName = pair.Key;
                if (ConstantHelper.GetConstantMode(EnumName) == ConstantModeType.Enumerations)
                    EnumName = ConstantHelper.GetEnumerationType(EnumName);
                enm = prj.GetEnumeration(EnumName);
                if (enm==null)
                {
                    MessageBox.Show("Unknown enumeration: " + EnumName);
                }
                else
                {
                    lbEnumName.Text = enm.Name;
                    lstValues.Items.Clear();
                    foreach (EnumValue ev in enm.Values)
                    {
                        ListViewItem lvi = new ListViewItem(ev.Name);
                        lvi.SubItems.Add(ev.Value);
                        lvi.SubItems.Add(ev.Description);
                        lstValues.Items.Add(lvi);
                    }
                    if (enm.IsBitSet)
                    {
                        lbBitSet.Visible = true;
                        lstValues.MultiSelect = true;
                        Dictionary<string, string> d = Project.StringListToDict(pair.Value);
                        if (d!=null) {
                            for (int tr = 0; tr < lstValues.Items.Count;tr++)
                            {
                                if (d.ContainsKey(lstValues.Items[tr].Text.ToLower()))
                                {
                                    lstValues.Items[tr].Checked = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        lbSingle.Visible = true;
                        lstValues.MultiSelect = false;
                        for (int tr = 0; tr < lstValues.Items.Count;tr++)
                        {
                            if (pair.Value.Equals(lstValues.Items[tr].Text, StringComparison.InvariantCultureIgnoreCase))
                            {
                                IgnoreCheckEvent = true;
                                lstValues.Items[tr].Checked = true;
                                IgnoreCheckEvent = false;
                                break;
                            }
                        }
                    }
                }                
            }
            editControl = edit;
        }


    }
}
