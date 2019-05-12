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
    public partial class DataTypeSelectControl : UserControl, IDataGridUserControl
    {
        private static ProjectContext Context;
        private static Project prj;
        private ITerminateEdit editControl = null;
        private Structure Struct = null;
        public string SelectedLinkID = "";

        public static void InitControl(ProjectContext pContext)
        {
            Context = pContext;
            prj = Context.Prj;
        }

        public DataTypeSelectControl()
        {
            InitializeComponent();
        }

        object IDataGridUserControl.GetResultedValue()
        {
            return new KeyValuePair<string, string>(Struct.Name, SelectedLinkID);
        }

        string IDataGridUserControl.GetStringRepresentation(object o)
        {
            if (o == null)
                return "";
            if (o.GetType() == typeof(KeyValuePair<string, string>))
            {
                KeyValuePair<string, string> pair = (KeyValuePair<string, string>)o;
                if ((pair.Value == null) || (pair.Value.Length == 0))
                    return "Null object";
                string DataTypeName = pair.Key;
                if (ConstantHelper.GetConstantMode(DataTypeName) == ConstantModeType.DataTypes)
                    DataTypeName = ConstantHelper.GetEnumerationType(DataTypeName);
                Structure s = prj.GetStructure(DataTypeName);
                if (s==null)
                {
                    return "Unkown: " + DataTypeName;
                }
                int LinkID = -1;
                if (int.TryParse(pair.Value,out LinkID)==false)
                {
                    return "Invalid ID:" + pair.Value;
                }
                foreach (StructureValue sv in s.Values)
                {
                    if (sv.LinkID == LinkID)
                    {
                        return pair.Value + " (" + Project.GetVariableName(sv.Name, sv.Array1, sv.Array2) + ")";
                    }
                }
                return "Missing ID:" + pair.Value;
            }
            return "?";
        }

        bool IDataGridUserControl.HasCustomPaint()
        {
            return false;
        }

        void IDataGridUserControl.DrawCell(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, Font fnt, StringFormat sfmt)
        {
            throw new NotImplementedException();
        }

        void IDataGridUserControl.Init(object o, ITerminateEdit edit)
        {
            if ((o != null) && (prj != null))
            {
                KeyValuePair<string, string> pair = (KeyValuePair<string, string>)o;
                string DataTypeName = pair.Key;
                if (ConstantHelper.GetConstantMode(DataTypeName) == ConstantModeType.DataTypes)
                    DataTypeName = ConstantHelper.GetEnumerationType(DataTypeName);
                Struct = prj.GetStructure(DataTypeName);
                if (Struct == null)
                {
                    MessageBox.Show("Unknown data type: " + DataTypeName);
                }
                else
                {
                    SelectedLinkID = pair.Value;
                    // populez coloanele
                    lst.Columns.Clear();
                    lst.Columns.Add("LinkID", 60);
                    lst.Columns.Add("Variable Name", 100);
                    lst.Columns.Add("Array", 60);
                    for (int tr = 0; tr < Struct.Fields.Count; tr++)
                        lst.Columns.Add(Struct.Fields[tr].Name, 80);
                    // populez valoarile
                    lst.Items.Clear();
                    foreach (StructureValue sv in Struct.Values)
                    {
                        ListViewItem lvi = new ListViewItem(sv.LinkID.ToString());
                        lvi.SubItems.Add(sv.Name);
                        lvi.SubItems.Add(Project.ArrayToString(sv.Array1, sv.Array2));
                        foreach (string s in sv.FieldValues)
                            lvi.SubItems.Add(s);
                        lvi.Tag = sv.LinkID;
                        lst.Items.Add(lvi);
                        if (lvi.Text == pair.Value)
                        {
                            lvi.Selected = true;
                            lvi.EnsureVisible();
                        }
                    }
                }
            }
            editControl = edit;
        }

        private void OnNullObject(object sender, EventArgs e)
        {
            lst.SelectedItems.Clear();
            if (editControl != null)
                editControl.FinishEdit();
        }

        private void OnDoubleClick(object sender, MouseEventArgs e)
        {
            if (editControl != null)
                editControl.FinishEdit();
        }

        private void OnSelectItem(object sender, EventArgs e)
        {
            if (lst.SelectedItems.Count > 0)
            {
                SelectedLinkID = lst.SelectedItems[0].Text;
            }
            else
            {
                SelectedLinkID = "";
            }
        }
    }
}
