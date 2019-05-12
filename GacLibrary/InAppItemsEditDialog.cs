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
    public partial class InAppItemsEditDialog : Form
    {
        public string InAppItemList = "";
        public InAppItemsEditDialog(string inAppList)
        {
            InitializeComponent();
            List<string> l = Project.StringListToList(inAppList);
            l.Sort();
            foreach (string s in l)
            {
                string k = s, v = "";
                if (s.Contains(":"))
                {
                    k = s.Split(':')[0];
                    v = s.Split(':')[1];
                }
                else if (s.Contains("="))
                {
                    k = s.Split('=')[0];
                    v = s.Split('=')[1];
                }
                dg.Rows.Add(new object[2] { k.Trim(),v.Trim() });
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            InAppItemList = "";
            for (int tr = 0; tr < dg.Rows.Count; tr++)
            {
                if (GetCell(tr, 0).Length == 0)
                    continue;
                float value = -1;
                if ((float.TryParse(GetCell(tr, 1), out value) == false) || (value < 0))
                {
                    MessageBox.Show("Invalid number: '" + GetCell(tr, 1) + "' at row: " + (tr + 1).ToString());
                    return;
                }
                InAppItemList += GetCell(tr, 0) + ":" + GetCell(tr, 1) + " , ";
            }
            if (InAppItemList.EndsWith(", "))
                InAppItemList = InAppItemList.Substring(0, InAppItemList.Length - 2);
            InAppItemList = InAppItemList.Trim();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void OnDelete(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell !");
                return;
            }
            Dictionary<DataGridViewRow, bool> d = new Dictionary<DataGridViewRow, bool>();
            foreach (DataGridViewCell cell in dg.SelectedCells)
            {
                if ((cell.OwningRow != null) && (cell.OwningRow.IsNewRow == false))
                    d[cell.OwningRow] = true;
            }
            if (d.Keys.Count == 0)
            {
                MessageBox.Show("Please select a valid cell !");
                return;
            }
            if (MessageBox.Show("Delete " + d.Keys.Count.ToString() + " entries ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewRow o in d.Keys)
                    dg.Rows.Remove(o);
            }
        }
        private string GetCell(int row, int column)
        {
            if ((row < 0) || (column < 0))
                return "";
            object o = dg.Rows[row].Cells[column].Value;
            string sValue = "";
            if (o != null)
                sValue = o.ToString();
            return sValue;
        }
        private void OnChangeCellValue(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0))
                return;
            string sValue = GetCell(e.RowIndex, e.ColumnIndex);

            if (e.ColumnIndex == 0)
            {
                if ((sValue.Length > 0) && (GetCell(e.RowIndex, 1).Length == 0))
                    dg.Rows[e.RowIndex].Cells[1].Value = "1";
                if (sValue.Length == 0)
                    dg.Rows.RemoveAt(e.RowIndex);
            }
        }
        private bool CheckIfNameExists(string s, int rowToIgnore)
        {
            s = s.ToLower();
            for (int tr = 0; tr < dg.Rows.Count; tr++)
            {
                if (tr == rowToIgnore)
                    continue;
                if (s == GetCell(tr, 0).ToLower())
                    return true;
            }
            return false;
        }
        private void OnValidateCell(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if ((string.IsNullOrEmpty(e.FormattedValue.ToString())) && (e.ColumnIndex == 0))
                return;
            if (e.ColumnIndex > 0)
            {
                if ((GetCell(e.RowIndex, 0).Length == 0) && (string.IsNullOrEmpty(e.FormattedValue.ToString()) == false))
                {
                    MessageBox.Show("You have to set a 'Name' first !");
                    e.Cancel = true;
                    return;
                }
                if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
                    return;
            }
            // nume
            if (e.ColumnIndex == 0)
            {
                if (Project.ValidateVariableNameCorectness(e.FormattedValue.ToString(), false) == false)
                {
                    MessageBox.Show("Invalid name - should contains letters (A-Z,a-z), numbers of '_'  character !");
                    e.Cancel = true;
                    return;
                }
                if (CheckIfNameExists(e.FormattedValue.ToString(), e.RowIndex))
                {
                    MessageBox.Show("Name '" + e.FormattedValue.ToString() + "' is already used !");
                    e.Cancel = true;
                    return;
                }
            }
            // valoare
            if (e.ColumnIndex == 1)
            {
                float value = -1;
                if ((float.TryParse(e.FormattedValue.ToString(), out value) == false) || (value < 0))
                {
                    MessageBox.Show("Invalid number - '" + e.FormattedValue.ToString() + "' !");
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
