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
    public partial class DataTypeListValuesEditDialog : Form
    {
        class ZOrderData
        {
            public ListViewItem Item;
            public int min, max, current;
            public ZOrderData(ListViewItem lvi, int _min, int _max, int _current)
            {
                Item = lvi;
                min = _min;
                max = _max;
                current = _current;
            }
        }

        Project prj;
        ProjectContext Context;
        ColumnInfo cInfo;
        bool IgnoreCheckEventForEnums;
        StringFormat firstRawFormat = new StringFormat();
        StringFormat lstValuesFormat = new StringFormat();
        SolidBrush back = new SolidBrush(Color.Black);
        SolidBrush fore = new SolidBrush(Color.Black);
        SolidBrush extraInfo = new SolidBrush(Color.Gray);
        SolidBrush tmpCol = new SolidBrush(Color.Black);
        Dictionary<string, StructureValue> DataTypesValues;

        public string ResultList = "";

        public DataTypeListValuesEditDialog(ProjectContext pContext,ColumnInfo _cInfo,string value)
        {
            InitializeComponent();
            prj = pContext.Prj;
            Context = pContext;
            cInfo = _cInfo;
            IgnoreCheckEventForEnums = false;
            List<string> l = Project.StringListToList(value,';',false);
            foreach (string s in l)
                lstValues.Items.Add(s);

            lstEnumValues.Dock = DockStyle.Fill;
            lstResValues.Dock = DockStyle.Fill;
            lstDataTypes.Dock = DockStyle.Fill;

            lstValues.DrawItem += lstValues_DrawItem;
            lstValues.DrawSubItem += lstValues_DrawSubItem;
            lstValues.DrawColumnHeader += lstValues_DrawColumnHeader;
            lstValues.OwnerDraw = true;

            firstRawFormat.Alignment = StringAlignment.Far;
            lstValuesFormat.Alignment = StringAlignment.Near;
            lstValuesFormat.LineAlignment = StringAlignment.Center;

            switch (cInfo.Mode)
            {
                case ConstantModeType.Resources: PopulateResourceValues(); txFilter.Visible = true; lbFilter.Visible = true; break;
                case ConstantModeType.Enumerations: PopulateEnumValues(); break;
                case ConstantModeType.BasicTypes: splitPanel.Panel2Collapsed = true; btnAddToList.Visible = false; break;
                case ConstantModeType.DataTypes: PopulateDataTypeValues(); btnEdit.Enabled = btnNew.Enabled = false; txFilter.Visible = true; lbFilter.Visible = true; break;
            }

            // la urma fac si update-ul pe link-uri
            // e important pentru ca dictionarele se creaza in functiile Populate
            foreach (ListViewItem lvi in lstValues.Items)
                UpdateItemExtraInfo(lvi);
        }

        void lstValues_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle(); 
        }

        void lstValues_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        void lstValues_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex!=0)
            {
                e.DrawDefault = true;
                return;
            }
            Rectangle r = e.Bounds;
            extraInfo.Color = Color.Gray;
            if (e.Item.Selected && e.Item.ListView.Focused)
            {
                e.SubItem.BackColor = SystemColors.Highlight;
                e.SubItem.ForeColor = e.Item.ListView.BackColor;
                extraInfo.Color = e.SubItem.ForeColor;
            }
            else if (e.Item.Selected && !e.Item.ListView.Focused)
            {
                e.SubItem.BackColor = SystemColors.Control;
                e.SubItem.ForeColor = e.Item.ListView.ForeColor;
            }
            else
            {
                e.SubItem.BackColor = e.Item.ListView.BackColor;
                e.SubItem.ForeColor = e.Item.ListView.ForeColor;
            }
            back.Color = e.SubItem.BackColor;
            fore.Color = e.SubItem.ForeColor;

            // Draw the standard header background.
            e.DrawBackground();
            e.Graphics.FillRectangle(Brushes.LightGray, r.X, r.Y, 50, e.Bounds.Height);
            string rowNumber = e.ItemIndex.ToString();
            SizeF size = e.Graphics.MeasureString(rowNumber, lstValues.Font);
            e.Graphics.DrawString(rowNumber, lstValues.Font, SystemBrushes.ControlText, r.X+50 - 5, r.Location.Y + ((r.Height - size.Height) / 2), firstRawFormat);



            // iconita
            int poz = r.X + 55;
            if ((cInfo.Mode == ConstantModeType.Resources) && (cInfo.ResType == ResourcesConstantType.Image))
            {
                if ((e.Item.Tag!=null) && (e.Item.Tag.GetType()==typeof(Bitmap)))
                {
                    Image i = (Image)e.Item.Tag;
                    e.Graphics.DrawImage(i, poz, r.Location.Y + r.Height / 2 - i.Height / 2);
                    poz += i.Width + 5;
                }
            }

            e.Graphics.DrawString(e.Item.Text, lstValues.Font, fore, poz, r.Location.Y + r.Height / 2, lstValuesFormat);

            if ((cInfo.Mode == ConstantModeType.Resources) && (cInfo.ResType == ResourcesConstantType.String))
            {
                if ((e.Item.Tag != null) && (e.Item.Tag.GetType() == typeof(string)))
                {
                    int oldX = r.X;
                    r.X = poz + 150;
                    e.Graphics.FillRectangle(back, r);
                    e.Graphics.DrawLine(Pens.Black, r.X + 5, r.Bottom - 5, r.X + 5, r.Top+5);
                    e.Graphics.DrawString((string)e.Item.Tag, lstValues.Font, extraInfo, r.X + 10, r.Location.Y + r.Height / 2, lstValuesFormat);
                    r.X = oldX;
                }
            }

            if ((cInfo.Mode == ConstantModeType.BasicTypes) && (cInfo.BasicType == BasicTypesConstantType.Color))
            {
                if ((e.Item.Tag != null) && (e.Item.Tag.GetType() == typeof(Color)))
                {
                    int oldX = r.X;
                    r.X = poz + 150;
                    e.Graphics.FillRectangle(back, r);
                    e.Graphics.DrawLine(Pens.Black, r.X + 5, r.Bottom - 5, r.X + 5, r.Top + 5);
                    tmpCol.Color = (Color)e.Item.Tag;
                    r.X += 10;
                    e.Graphics.FillRectangle(tmpCol, r.X + 2, r.Y + 2, r.Height - 5, r.Height - 5);
                    e.Graphics.DrawRectangle(Pens.Black, r.X + 2, r.Y + 2, r.Height - 5, r.Height - 5);
                    e.Graphics.DrawString(tmpCol.Color.ToString(), lstValues.Font, extraInfo, r.X + r.Height+5, r.Location.Y + r.Height / 2, lstValuesFormat);
                    r.X = oldX;
                }
            }

            if ((cInfo.Mode == ConstantModeType.DataTypes) && (e.Item.Tag!=null) && (e.Item.Tag.GetType() == typeof(StructureValue)))
            {
                int oldX = r.X;
                r.X = poz + 50;
                e.Graphics.FillRectangle(back, r);
                e.Graphics.DrawLine(Pens.Black, r.X + 5, r.Bottom - 5, r.X + 5, r.Top + 5);
                r.X += 10;
                StructureValue sv = (StructureValue)e.Item.Tag;
                e.Graphics.DrawString(Project.GetVariableName(sv.Name,sv.Array1,sv.Array2), lstValues.Font, extraInfo, r.X + 5, r.Location.Y + r.Height / 2, lstValuesFormat);
                r.X = oldX;
            }


            e.Graphics.DrawLine(Pens.Black, r.Left, r.Bottom-1, r.Right, r.Bottom-1);
            e.Graphics.DrawLine(Pens.Black, r.X + 50, r.Bottom - 1, r.X + 50, r.Top);
            
        }

        private string EditValue(string DefaultValue)
        {
            string result = null;
            switch (cInfo.Mode)
            {
                case ConstantModeType.BasicTypes:
                    BasicTypeConstantEditDialog dlgBasicType = new BasicTypeConstantEditDialog(prj, false, "", cInfo.BasicType, "", DefaultValue);
                    if (dlgBasicType.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dlgBasicType.FieldValue;
                        //if (cInfo.BasicType == BasicTypesConstantType.String)
                        //    result = dlgBasicType.FieldValue.Substring(1, dlgBasicType.FieldValue.Length - 2);
                    }
                    break;
                case ConstantModeType.Enumerations:
                    EnumSelectValueDialog dlgEnum = new EnumSelectValueDialog(prj, cInfo.E.Name, DefaultValue);
                    if (dlgEnum.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dlgEnum.EnumValueResult;
                    }
                    break;
                case ConstantModeType.Resources:
                    ResourceSelectDialog dlg = new ResourceSelectDialog(Context, cInfo.ResType, true, false);
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dlg.SelectedResource;
                    }
                    break;
            }
            return result;
        }

        private void OnOK(object sender, EventArgs e)
        {
            // verific ca totul e ok
            foreach (ListViewItem lvi in lstValues.Items)
            {
                string text = lvi.Text;
                string error = "";
                if (prj.ValidateValueForType(text,cInfo.FullType,out error,null,null,true)==null)
                {
                    MessageBox.Show("Invalid value: " + text + "\n" + error);
                    return;
                }
            }
            // dupa ce toate validarile sunt ok
            string s = "";
            foreach (ListViewItem lvi in lstValues.Items)
            {
                s += lvi.Text + ";";
            }
            if (s.Length > 0)
                s = s.Substring(0, s.Length - 1);
            ResultList = s;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void UpdateItemExtraInfo(ListViewItem lvi)
        {
            lvi.Tag = null;
            if ((cInfo.Mode == ConstantModeType.Resources) && (cInfo.ResType == ResourcesConstantType.Image))
            {
                string txt = GenericResource.GetResourceVariableKey(ConstantHelper.ConvertResourcesConstantTypeToResourceType(ResourcesConstantType.Image), lvi.Text);
                if (Context.SmallIcons.Images.ContainsKey(txt))
                {

                    lvi.Tag = Context.SmallIcons.Images[txt];
                }
            }
            if ((cInfo.Mode == ConstantModeType.Resources) && (cInfo.ResType == ResourcesConstantType.String))
            {
                if (Context.Resources.Strings.ContainsKey(lvi.Text))
                {
                    lvi.Tag = Context.Resources.Strings[lvi.Text].Get(Context.Prj.DefaultLanguage);
                }
            }
            if ((cInfo.Mode == ConstantModeType.BasicTypes) && (cInfo.BasicType == BasicTypesConstantType.Color))
            {
                if (lvi.Text.StartsWith("0x"))
                {
                    UInt32 col = Convert.ToUInt32(lvi.Text.Substring(2),16);
                    lvi.Tag = Color.FromArgb((int)col);
                }
                else
                {
                    lvi.Tag = Color.Transparent;
                }                
            }
            if (cInfo.Mode == ConstantModeType.DataTypes)
            {
                if (DataTypesValues.ContainsKey(lvi.Text))
                    lvi.Tag = DataTypesValues[lvi.Text];
                else
                    lvi.Tag = null;
            }
        }

        private void SearchAndSelectItem(ListView lv,string text)
        {
            foreach (ListViewItem lvi2 in lv.Items)
                lvi2.Selected = false;
            foreach (ListViewItem lvi in lv.Items)
                if (lvi.Text == text)
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                    return;
                }
        }

        private void OnSelectItemFromList(object sender, EventArgs e)
        {
            if (lstValues.SelectedItems.Count == 1)
            {
                if (cInfo.Mode == ConstantModeType.Resources)
                    SearchAndSelectItem(lstResValues, lstValues.SelectedItems[0].Text);
                if (cInfo.Mode == ConstantModeType.DataTypes)
                    SearchAndSelectItem(lstDataTypes, lstValues.SelectedItems[0].Text);
                return;
            }
        }
        private void OnAddNewValue(object sender, EventArgs e)
        {
            string result = EditValue("");
            if (result!=null)
            {
                ListViewItem lvi = new ListViewItem(result);
                UpdateItemExtraInfo(lvi);
                lstValues.Items.Add(lvi);
                lstValues.Items[lstValues.Items.Count - 1].EnsureVisible();              
            }
        }

        private void OnDoubleClicked(object sender, MouseEventArgs e)
        {
            if (lstValues.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select only one item !");
                return;
            }
            ListViewItem lvi = lstValues.SelectedItems[0];
            string result = EditValue(lvi.Text);
            if (result != null)
            {
                lvi.Text = result;
                UpdateItemExtraInfo(lvi);
                lvi.EnsureVisible();
            }
        }

        private void OnEditListValue(object sender, EventArgs e)
        {
            OnDoubleClicked(null, null);
        }

        private void OnDeleteValues(object sender, EventArgs e)
        {
            if (lstValues.SelectedItems.Count<1)
            {
                MessageBox.Show("Please select at least one item for deletion !");
                return;
            }
            if (MessageBox.Show("Are you sure do you want to delete " + lstValues.SelectedItems.Count.ToString() + " items ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                List<ListViewItem> toDelete = new List<ListViewItem>();
                foreach (ListViewItem ll in lstValues.SelectedItems)
                    toDelete.Add(ll);
                foreach (ListViewItem ll in toDelete)
                    lstValues.Items.Remove(ll);
            }
        }

        #region Enumerari
        private void PopulateEnumValues()
        {
            lstEnumValues.Items.Clear();
            if (cInfo.E != null)
            {
                if (cInfo.E.IsBitSet)
                    lstEnumValues.Columns[0].Text = "BitSet:"+cInfo.E.Name;
                else
                    lstEnumValues.Columns[0].Text = "SingleValue:"+cInfo.E.Name;
                foreach (EnumValue ev in cInfo.E.Values)
                {
                    ListViewItem lvi = new ListViewItem(ev.Name);
                    lvi.SubItems.Add(ev.Value);
                    lvi.SubItems.Add(ev.Description);
                    lstEnumValues.Items.Add(lvi);
                }
            }
            lstEnumValues.Visible = true;
        }

        private void OnEnumItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (cInfo.E == null)
                return;
            if (cInfo.E.IsBitSet)
                return;
            if (e.Item == null)
                return;
            if (e.Item.Checked == false)
                return;
            if (IgnoreCheckEventForEnums)
                return;
            IgnoreCheckEventForEnums = true;
            if (lstEnumValues.Items != null)
            {
                for (int tr = 0; tr < lstEnumValues.Items.Count; tr++)
                    if (lstEnumValues.Items[tr] != null)
                        lstEnumValues.Items[tr].Checked = lstEnumValues.Items[tr] == e.Item;
            }
            IgnoreCheckEventForEnums = false;
            if (e.Item.Selected == false)
                e.Item.Selected = true;
        }

        private void OnEnumItemSelectItem(object sender, EventArgs e)
        {
            if (cInfo.E == null)
                return;
            if (cInfo.E.IsBitSet)
                return;
            if (lstEnumValues.SelectedItems.Count == 1)
            {
                lstEnumValues.SelectedItems[0].Checked = true;
            }
        }
        public string GetEnumSelectedValue()
        {
            if (cInfo.E == null)
                return null;
            string s = "";
            foreach (ListViewItem lvi in lstEnumValues.Items)
            {
                if (lvi.Checked)
                    s += lvi.Text + ", ";
            }
            // daca nu e bitset trebuie sa fie o valoare selectata
            if ((s.Length == 0) && (cInfo.E.IsBitSet == false))
                return null;
            if (s.EndsWith(", "))
                s = s.Substring(0, s.Length - 2);
            return s;
        }
        #endregion

        #region Resources
        private void PopulateResourceValues()
        {
            lstResValues.SmallImageList = Context.SmallIcons;
            lstResValues.LargeImageList = Context.LargeIcons;
            lstResValues.Items.Clear();
            lstResValues.Columns[0].Text = cInfo.ResType.ToString();
            string filter = txFilter.Text.ToLower();
            if ((cInfo.ResType != ResourcesConstantType.None) && (cInfo.ResType != ResourcesConstantType.String))
            {
                Type t = ConstantHelper.ConvertResourcesConstantTypeToResourceType(cInfo.ResType);
                if (t == null)
                    return;
                foreach (GenericResource r in prj.Resources)
                {
                    if (r.GetType() != t)
                        continue;
                    if (filter.Length!=0)
                    {
                        if (r.GetResourceVariableName().ToLower().Contains(filter) == false)
                            continue;
                    }
                    // daca e o versiune pentru alte rezolutii - nu o afisez
                    if (r.IsBaseResource() == false)
                        continue;
                    ListViewItem lvi = new ListViewItem(r.GetResourceVariableName());
                    lvi.SubItems.Add(r.GetResourceInformation());
                    lvi.ImageKey = r.GetIconImageListKey();
                    lstResValues.Items.Add(lvi);
                }
            }
            // stringuri
            if (cInfo.ResType == ResourcesConstantType.String)
            {
                foreach (StringValues sv in prj.Strings)
                {
                    if (filter.Length != 0)
                    {
                        if (sv.GetVariableNameWithArray().ToLower().Contains(filter) == false)
                            continue;
                    }
                    ListViewItem lvi = new ListViewItem(sv.GetVariableNameWithArray());
                    lvi.SubItems.Add(sv.Get(prj.DefaultLanguage));
                    lstResValues.Items.Add(lvi);
                }
            }
            // view mode
            if (cInfo.ResType == ResourcesConstantType.Image)
                lstResValues.View = View.Tile;
            lstResValues.Visible = true;
        }
        public List<string> GetResourcesSelectedValues()
        {
            List<string> l = new List<string>();
            foreach (ListViewItem lvi in lstResValues.SelectedItems)
                l.Add(lvi.Text);
            return l;
        }
        #endregion

        #region Data Types
        private void PopulateDataTypeValues()
        {
            lstDataTypes.Items.Clear();
            bool justCreated = false;
            if (DataTypesValues == null)
            {
                DataTypesValues = new Dictionary<string, StructureValue>();
                justCreated = true;
            }
            string filter = txFilter.Text.ToLower();
            if (cInfo.DataType!=null)
            {
                if (lstDataTypes.Columns.Count == 2)
                {
                    foreach (StructureField sf in cInfo.DataType.Fields)
                    {
                        ColumnHeader ch = new ColumnHeader();
                        ch.Text = sf.Name;
                        lstDataTypes.Columns.Add(ch);
                    }
                }
                // populez datele efectv
                foreach (StructureValue sv in cInfo.DataType.Values)
                {
                    if (filter.Length>0)
                    {
                        if (Project.GetVariableName(sv.Name, sv.Array1, sv.Array2).ToLower().Contains(filter) == false)
                            continue;
                    }
                    ListViewItem lvi = new ListViewItem(sv.LinkID.ToString());
                    lvi.SubItems.Add(Project.GetVariableName(sv.Name, sv.Array1, sv.Array2));
                    foreach (string s in sv.FieldValues)
                        lvi.SubItems.Add(s);
                    lstDataTypes.Items.Add(lvi);
                    if (justCreated)
                        DataTypesValues[lvi.Text] = sv;
                }
            }
            lstDataTypes.Visible = true;
        }
        public List<string> GetDataTypeSelectedValues()
        {
            List<string> l = new List<string>();
            foreach (ListViewItem lvi in lstDataTypes.SelectedItems)
                l.Add(lvi.Text);
            return l;
        }
        #endregion

        private void OnAddFromRightList(object sender, EventArgs e)
        {
            string result = null;
            List<string> results = null;
            switch (cInfo.Mode)
            {
                case ConstantModeType.Enumerations:
                    result = GetEnumSelectedValue();
                    if (result == null)
                    {
                        MessageBox.Show("Missing enumeration selection !");
                        return;
                    }
                    lstValues.Items.Add(result);
                    UpdateItemExtraInfo(lstValues.Items[lstValues.Items.Count - 1]);
                    lstValues.Items[lstValues.Items.Count - 1].EnsureVisible();
                    break;
                case ConstantModeType.Resources:
                    results = GetResourcesSelectedValues();
                    if (results.Count==0)
                    {
                        MessageBox.Show("Please select resources that you need to add first !");
                        return;
                    }
                    foreach (string s in results)
                    {
                        lstValues.Items.Add(s);
                        UpdateItemExtraInfo(lstValues.Items[lstValues.Items.Count - 1]);
                    }
                    lstValues.Items[lstValues.Items.Count - 1].EnsureVisible();
                    break;
                case ConstantModeType.DataTypes:
                    results = GetDataTypeSelectedValues();
                    if (results.Count == 0)
                    {
                        MessageBox.Show("Please select a data type item that you need to add first !");
                        return;
                    }
                    foreach (string s in results)
                    {
                        lstValues.Items.Add(s);
                        UpdateItemExtraInfo(lstValues.Items[lstValues.Items.Count - 1]);
                    }
                    lstValues.Items[lstValues.Items.Count - 1].EnsureVisible();
                    break;
            }
        }
        private void OnTextFilterChanges(object sender, EventArgs e)
        {
            switch (cInfo.Mode)
            {
                case ConstantModeType.DataTypes: PopulateDataTypeValues(); break;
                case ConstantModeType.Resources: PopulateResourceValues(); break;
            }
        }

        #region ZOrder
        private void MoveZOrder(int dir)
        {
            if (lstValues.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select items to move !");
                return;
            }
            int diff = lstValues.Items.Count - lstValues.SelectedItems.Count;
            List<ZOrderData> l = new List<ZOrderData>();
            for (int tr = 0; tr < lstValues.SelectedIndices.Count; tr++)
                l.Add(new ZOrderData(lstValues.SelectedItems[tr], tr, tr + diff, lstValues.SelectedIndices[tr]));
            // sterg item-urile si recalculez pozitiile
            foreach (ZOrderData z in l)
            {
                z.Item.Selected = false;
                lstValues.Items.Remove(z.Item);
                z.current += dir;
                if (z.current < z.min)
                    z.current = z.min;
                if (z.current > z.max)
                    z.current = z.max;
            }
            // le inserez din nou in lista
            foreach (ZOrderData z in l)
            {
                lstValues.Items.Insert(z.current, z.Item);
                z.Item.Selected = true;
                z.Item.EnsureVisible();
            }                       
        }

        private void OnMoveZOrderUp(object sender, EventArgs e)
        {
            MoveZOrder(-1);
        }

        private void OnMoveZOrderDown(object sender, EventArgs e)
        {
            MoveZOrder(1);
        }

        private void OnMoveZOrderTop(object sender, EventArgs e)
        {
            MoveZOrder(-10000000);
        }

        private void OnMoveZOrderBottom(object sender, EventArgs e)
        {
            MoveZOrder(10000000);
        }

        #endregion


    }
}
