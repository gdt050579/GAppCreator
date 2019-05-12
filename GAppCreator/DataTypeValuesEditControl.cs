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
    public partial class DataTypeValuesEditControl : UserControl
    { 
        Project prj;
        ProjectContext Context;
        Structure Struct = null;
        Dictionary<string, GenericResource> baseResources = null;
        Dictionary<string, StringValues> stringResources = null;
        StringFormat firstRawFormat = new StringFormat();
        List<int> columnIndexTemp = new List<int>();


        public DataTypeValuesEditControl()
        {
            InitializeComponent();
            firstRawFormat.Alignment = StringAlignment.Far;
            dg.RowHeadersWidth = 70;
            dg.RowPostPaint += dg_RowPostPaint;
            dg.CellContentClick += dg_CellContentClick;
        }

        void dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 0))
                return;
            if (dg.Columns[e.ColumnIndex].Tag == null)
                return;                    
            ColumnInfo cInfo = (ColumnInfo)dg.Columns[e.ColumnIndex].Tag;
            StructureValue sv = (StructureValue)dg.Rows[e.RowIndex].Tag;
            if (sv.IsNull)
                return;
            if (cInfo.ColumnType == ColumnEditType.List)
            {
                DataTypeListValuesEditDialog dlg = new DataTypeListValuesEditDialog(Context, cInfo, sv.FieldValues[e.ColumnIndex - 3]);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    sv.FieldValues[e.ColumnIndex - 3] = dlg.ResultList;
                    UpdateDataGridRow(e.RowIndex);
                }
            }
        }
        public void SetStructure(ProjectContext pContext, Structure s)
        {
            this.Enabled = true;
            Struct = null;
            this.txFilter.Text = s.Filter;
            Context = pContext;
            prj = pContext.Prj;
            baseResources = prj.GetBaseResources();
            stringResources = prj.GetStringResources();
            EnumSelectValueControl.InitControl(prj);
            ResourceSelectControl.InitControl(Context, true, true);
            DataTypeSelectControl.InitControl(Context);
            Struct = s;
            Struct.Values.Sort();
            InitDataGrid();
            UpdateDataGridRows();
        }
        public void ClearStructureData()
        {
            Struct = null;
            dg.Rows.Clear();
            dg.Columns.Clear();            
            this.Enabled = false;
            lbVisibleRecords.Text = "";
        }
        private void InitDataGrid()
        {
            dg.Rows.Clear();
            dg.Columns.Clear();
            columnIndexTemp.Clear();

            // foarte important ca ColumnInfo sa fie NULL la urmatoarele 3
            AddColumn("Var Name", "Name of the variable that will be available in code.\nIf you don't enter a name this field will be invisible for GAC code", ColumnEditType.VariableName, null,null,0);            
            AddColumn("Array", "Array of the variable",  ColumnEditType.VariableArray, null,null,0);
            AddColumn("Null", "If this field is NULL or not", ColumnEditType.NullField, null, null,0);
            // adaug field-urile    
            for (int tr=0;tr<Struct.Fields.Count;tr++)
            {
                if ((Struct.Fields[tr].DisplayIndex<0) || (Struct.Fields[tr].DisplayIndex>9998))
                    columnIndexTemp.Add(9999*10000+tr);
                else
                    columnIndexTemp.Add(Struct.Fields[tr].DisplayIndex * 10000 + tr);
                
                ColumnInfo cInfo = new ColumnInfo(Struct.Fields[tr], prj);
                string toolTip = "Mode: " + cInfo.Mode.ToString() + "\nType : " + cInfo.Type+"\nDescription:"+cInfo.Field.Description;
                if (cInfo.Field.List)
                {
                    AddColumn(cInfo.Field.Name, toolTip, ColumnEditType.List, "", cInfo, Struct.Fields[tr].ColumnWidth);
                }
                else
                {
                    switch (cInfo.Mode)
                    {
                        case ConstantModeType.BasicTypes:
                            if (cInfo.BasicType == BasicTypesConstantType.Boolean)
                                AddColumn(cInfo.Field.Name, toolTip, ColumnEditType.Boolean, false, cInfo, Struct.Fields[tr].ColumnWidth);
                            else
                                AddColumn(cInfo.Field.Name, toolTip, ColumnEditType.Text, "", cInfo, Struct.Fields[tr].ColumnWidth);
                            break;
                        case ConstantModeType.Enumerations:
                            if (cInfo.E == null)
                                AddColumn(cInfo.Field.Name, toolTip + "\nUnknown column type - expecting a valid enumeration", ColumnEditType.UnknownType, null, cInfo, Struct.Fields[tr].ColumnWidth);
                            else
                            {
                                if (cInfo.E.IsBitSet)
                                    AddColumn(cInfo.Field.Name, toolTip, ColumnEditType.BitSets, null, cInfo, Struct.Fields[tr].ColumnWidth);
                                else
                                    AddColumn(cInfo.Field.Name, toolTip, ColumnEditType.Enums, null, cInfo, Struct.Fields[tr].ColumnWidth);
                            }
                            break;
                        case ConstantModeType.DataTypes:
                            AddColumn(cInfo.Field.Name, toolTip, ColumnEditType.DataType, false, cInfo, Struct.Fields[tr].ColumnWidth);
                            break;
                        case ConstantModeType.Resources:
                            AddColumn(cInfo.Field.Name, toolTip, ColumnEditType.Resources, false, cInfo, Struct.Fields[tr].ColumnWidth);
                            break;
                    }
                }
            }
            // primele 3 sunt frozen
            dg.Columns[0].Frozen = true;
            dg.Columns[1].Frozen = true;
            dg.Columns[2].Frozen = true;
            // pun si indexii
            columnIndexTemp.Sort();
            for (int tr=0;tr<columnIndexTemp.Count;tr++)
            {
                dg.Columns[(columnIndexTemp[tr] % 10000) + 3].DisplayIndex = tr + 3;
            }
        
        }
        void AddColumn(string name, string toolTip,  ColumnEditType cType, object defaultValue,ColumnInfo cInfo,int columnSize)
        {
            System.Windows.Forms.DataGridViewCellStyle cellStyle;

            switch (cType)
            {
                case ColumnEditType.Boolean:
                    DataGridViewCheckBoxColumn boolColumn = new DataGridViewCheckBoxColumn();
                    boolColumn.Width = 50;
                    dg.Columns.Add(boolColumn);
                    break;
                case ColumnEditType.NullField:
                    DataGridViewCheckBoxColumn nullColumn = new DataGridViewCheckBoxColumn();
                    nullColumn.Width = 35;  
                    cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
                    cellStyle.BackColor = Color.FromArgb(255, 245, 245);
                    cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    nullColumn.DefaultCellStyle = cellStyle;              
                    dg.Columns.Add(nullColumn);
                    break;
                case ColumnEditType.Text:
                    DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
                    textColumn.Width = 100;
                    dg.Columns.Add(textColumn);
                    break;
                case ColumnEditType.UnknownType:
                    DataGridViewTextBoxColumn unkColumn = new DataGridViewTextBoxColumn();
                    unkColumn.Width = 100;
                    cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
                    cellStyle.BackColor = Color.FromArgb(255, 192, 192);
                    unkColumn.DefaultCellStyle = cellStyle;
                    unkColumn.ReadOnly = true;
                    dg.Columns.Add(unkColumn);
                    break;
                case ColumnEditType.VariableName:
                    DataGridViewTextBoxColumn varNameColumn = new DataGridViewTextBoxColumn();
                    varNameColumn.Width = 150;
                    cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
                    cellStyle.BackColor = Color.FromArgb(240, 255, 255);
                    varNameColumn.DefaultCellStyle = cellStyle;
                    dg.Columns.Add(varNameColumn);
                    break;
                case ColumnEditType.VariableArray:
                    DataGridViewTextBoxColumn varArrayColumn = new DataGridViewTextBoxColumn();
                    varArrayColumn.Width = 50;
                    cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
                    cellStyle.BackColor = Color.FromArgb(230, 255, 245);
                    varArrayColumn.DefaultCellStyle = cellStyle;
                    dg.Columns.Add(varArrayColumn);
                    break;
                case ColumnEditType.Enums:
                case ColumnEditType.BitSets:
                    CustomColumn<EnumSelectValueControl> enumColumn = new CustomColumn<EnumSelectValueControl>(defaultValue);
                    enumColumn.Width = 100;
                    if (cType == ColumnEditType.BitSets)
                        enumColumn.Width = 200;
                    dg.Columns.Add(enumColumn);
                    break;
                case ColumnEditType.Resources:
                    CustomColumn<ResourceSelectControl> resColumn = new CustomColumn<ResourceSelectControl>(defaultValue);
                    resColumn.Width = 150;
                    dg.Columns.Add(resColumn);
                    break;
                case ColumnEditType.DataType:
                    CustomColumn<DataTypeSelectControl> dtColumn = new CustomColumn<DataTypeSelectControl>(defaultValue);
                    dtColumn.Width = 100;
                    dg.Columns.Add(dtColumn);
                    break;
                case ColumnEditType.List:
                    DataGridViewButtonColumn lstColumn = new DataGridViewButtonColumn();
                    lstColumn.Width = 100;
                    lstColumn.CellTemplate = new ListButtonCell();
                    dg.Columns.Add(lstColumn);
                    break;
            }
            if (cInfo!=null)
                cInfo.ColumnType = cType;
            dg.Columns[dg.Columns.Count - 1].HeaderText = name;
            dg.Columns[dg.Columns.Count - 1].ToolTipText = toolTip;
            dg.Columns[dg.Columns.Count - 1].Tag = cInfo;
            dg.Columns[dg.Columns.Count - 1].SortMode = DataGridViewColumnSortMode.NotSortable;
            if (columnSize > 0)
                dg.Columns[dg.Columns.Count - 1].Width = columnSize;
            
            if (cType == ColumnEditType.UnknownType)
            {

            }         
        }
        object GetDataGridCellFormatedValues(int columnID,string value)
        {
            ColumnInfo cInfo = (ColumnInfo)dg.Columns[columnID].Tag;
            if (cInfo.Field.List)
            {
                return value;
            }
            switch (cInfo.ColumnType)
            {
                case ColumnEditType.Boolean: return ((value[0] | 0x20) == 't');
                case ColumnEditType.Text: return value;
                case ColumnEditType.Enums:
                case ColumnEditType.BitSets: return new KeyValuePair<string, string>(cInfo.E.Name, value);
                case ColumnEditType.Resources: return new KeyValuePair<ResourcesConstantType, string>(cInfo.ResType, value);
                case ColumnEditType.DataType: return new KeyValuePair<string, string>(cInfo.DataType.Name, value);
                default: return value;
            }
        }
        void UpdateDataGridRow(int id)
        {
            DataGridViewRow r = dg.Rows[id];
            StructureValue sv = Struct.Values[id];
            r.Tag = sv;
            r.Cells[0].Value = sv.Name;
            r.Cells[1].Value = Project.ArrayToString(sv.Array1, sv.Array2);
            r.Cells[2].Value = sv.IsNull;
            int colCount = dg.Columns.Count;
            int fieldValuePoz = 0;
            int poz = 3;
            while (poz<colCount)
            {
                if (sv.IsNull)
                    r.Cells[poz].Value = null;
                else
                    r.Cells[poz].Value = GetDataGridCellFormatedValues(poz, sv.FieldValues[fieldValuePoz]);
                r.Cells[poz].ReadOnly = sv.IsNull;
                poz++;
                fieldValuePoz++;
            }        
        }
        void UpdateDataGridRows()
        {
            dg.Rows.Clear();
            if (Struct == null) return;
            Struct.Values.Sort();
            if (Struct.Values.Count > 0)
            {
                dg.Rows.Add(Struct.Values.Count);
                for (int tr = 0; tr < Struct.Values.Count; tr++)
                {
                    UpdateDataGridRow(tr);
                }
            }
            FilterRows();
        }

        void dg_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (cbShowUniqueIDs.Checked == false)
                return;
            DataGridView dg = (DataGridView)sender;
            if (e.RowIndex < 0)
                return;
            if (dg.Rows[e.RowIndex].IsNewRow)
                return;            
            string rowNumber = "";
            StructureValue sv = dg.Rows[e.RowIndex].Tag as StructureValue;
            if (sv != null)
                rowNumber = sv.LinkID.ToString();
            SizeF size = e.Graphics.MeasureString(rowNumber, this.Font);
            if (dg.RowHeadersWidth < (int)(size.Width + 20)) dg.RowHeadersWidth = (int)(size.Width + 20);
            e.Graphics.DrawString(rowNumber, dg.Font, SystemBrushes.ControlText, dg.RowHeadersWidth - 5, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2), firstRawFormat);
        }

        private void OnAdd(object sender, EventArgs e)
        {
            DataTypeValuesAddDialog dlg = new DataTypeValuesAddDialog(prj);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int linkID = Struct.AddValues(dlg.ResultName, dlg.Array1, dlg.Array2,dlg.FieldIsNull);
                UpdateDataGridRows();
            }
        }
        private void OnResizeArray(object sender, EventArgs e)
        {
            string cName = "";
            foreach (DataGridViewCell cell in dg.SelectedCells)
            {
                if ((cell.OwningRow != null) && (cell.OwningRow.IsNewRow == false))
                {
                    StructureValue sv = (StructureValue)cell.OwningRow.Tag;
                    if (sv.Name.Length>0)
                    {
                        cName = sv.Name;
                        break;
                    }
                }
            }
            DataTypeValuesResizeArrayDialog dlg = new DataTypeValuesResizeArrayDialog(Struct, cName);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Struct.ResizeValuesVariable(dlg.VariableName, dlg.oldArray1, dlg.oldArray2, dlg.newArray1, dlg.newArray2, dlg.NewFieldsAreNull);
                UpdateDataGridRows();
            }
        }
        private void OnDeleteRows(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select at least one cell/row !");
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
                    Struct.Values.Remove((StructureValue)o.Tag);
                UpdateDataGridRows();
            }
        }
        private void FilterRows()
        {
            string s = txFilter.Text;
            bool found = false;
            bool hideInvisible = cbHideInvisible.Checked;
            string name;
            int count_visible = 0;
            if (s.Length == 0)
            {
                for (int tr = 0; tr < dg.Rows.Count; tr++)
                {
                    if ((hideInvisible) && (dg.Rows[tr].Cells[0].Value.ToString().Length == 0))
                    {
                        dg.Rows[tr].Visible = false;
                    }
                    else
                    {
                        dg.Rows[tr].Visible = true;
                        count_visible++;
                    }
                }
            }
            else
            {
                for (int tr = 0; tr < dg.Rows.Count; tr++)
                {
                    if (dg.Rows[tr].IsNewRow)
                        continue;
                    name = dg.Rows[tr].Cells[0].Value.ToString();
                    if ((hideInvisible) && (name.Length == 0))
                        found = false;
                    else
                        found = (name.IndexOf(s,StringComparison.InvariantCultureIgnoreCase)>=0);
                    dg.Rows[tr].Visible = found;
                    if (found)
                        count_visible++;
                }
            }
            lbVisibleRecords.Text = count_visible.ToString() + " / " + dg.RowCount.ToString();
            if ((count_visible == 0) && (dg.RowCount > 0))
                lbVisibleRecords.ForeColor = Color.Red;
            else
                lbVisibleRecords.ForeColor = Color.Black;
        }
        private void OnChangeTextFilter(object sender, EventArgs e)
        {
            if (Struct!=null)
                Struct.Filter = txFilter.Text;
            FilterRows();
        }

        private void OnValidateCell(object sender, DataGridViewCellValidatingEventArgs e)
        {
            StructureValue sv = (StructureValue)dg.Rows[e.RowIndex].Tag;
            // numele
            if (e.ColumnIndex == 0)
            {
                string newValue = e.FormattedValue.ToString();
                if (newValue.Equals(sv.Name)==false)
                    e.Cancel = !ValidateNewName(e.RowIndex,sv,newValue);               
            }
            // Array
            if (e.ColumnIndex == 1)
            {
                e.Cancel = !ValidateNewArray(sv, e.FormattedValue.ToString());
            }
            // null
            if (e.ColumnIndex == 2)
            {
                bool val = (bool)e.FormattedValue;
                if (val!=sv.IsNull)
                {
                    sv.IsNull = val;
                    UpdateDataGridRow(e.RowIndex);
                    dg.InvalidateRow(e.RowIndex);                    
                }
            }
            // restul coloanelor
            if (sv.IsNull)
                return;
            if (e.ColumnIndex >= 3)
            {
                e.Cancel = !ValidateNewFieldValue(e.ColumnIndex, e.ColumnIndex - 3, sv, e.FormattedValue);
            }
        }
        private bool ValidateNewName(int rowIndex,StructureValue sv,string newName)
        {    
            if (newName.Length == 0)
            {
                if ((sv.Array1 < 0) && (sv.Array2 < 0))
                {
                }
                else
                {
                    if (MessageBox.Show("Constant '" + sv.Name + "' is part of an array. Would you like to delete the entire array ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        for (int tr = 0; tr < dg.Rows.Count; tr++)
                        {
                            StructureValue rowSV = (StructureValue)dg.Rows[tr].Tag;
                            if (rowSV == sv)
                                continue;
                            if (rowSV.Name.Equals(sv.Name))
                            {
                                rowSV.Name = "";
                                rowSV.Array1 = rowSV.Array2 = -1;
                                UpdateDataGridRow(tr);
                            }
                        }
                    }
                }
                sv.Name = "";
                sv.Array1 = sv.Array2 = -1;
                UpdateDataGridRow(rowIndex);
                return true;
            }
            if (Project.ValidateVariableNameCorectness(newName, false) == false)
            {
                MessageBox.Show("Invalid constant name - it must contains letters, numbers and symbol '_' !");
                return false;
            }
            Structure s = prj.GetStructureForVariableName(newName);
            if ((s != null) && (s!=Struct))
            {
                MessageBox.Show("Constant '" + newName + "' already exists in data type '" + s.Name + "' !");
                return false;
            }
            // decid daca trebuie sa redenumesc pe toate sau doar unul
            if ((sv.Array1 < 0) && (sv.Array2 < 0))
            {
                sv.Name = newName;
            }
            else
            {
                if (MessageBox.Show("Constant '"+sv.Name+"' is part of an array. Would you like to rename all of the array variables to the new name : '"+newName+"' ?","Rename", MessageBoxButtons.YesNo)== DialogResult.Yes)
                {
                    for (int tr=0;tr<dg.Rows.Count;tr++)
                    {
                        StructureValue rowSV = (StructureValue)dg.Rows[tr].Tag;
                        if (rowSV == sv)
                            continue;
                        if (rowSV.Name.Equals(sv.Name))
                        {
                            rowSV.Name = newName;
                            UpdateDataGridRow(tr);
                        }
                    }
                }
                sv.Name = newName;
            }
            return true;
        }
        private bool ValidateNewArray(StructureValue sv, string newArray)
        {
            int a1 = -1, a2 = -1;
            if ((newArray.Equals("-")) && (sv.Array1 == -1) && (sv.Array2 == -1))
                return true;
            bool res = Project.StringToArray(newArray, ref a1, ref a2);
            if (res==false)
            {
                MessageBox.Show("Invalid array identifier. \nUse '-' for an empty string for no array, a number for a vector index (Ex: 5), or two numbers separated by a comma for matrix indexes (Ex: 3,4) !");
                return false;
            }
            if ((a1!=sv.Array1) || (a2!=sv.Array2))
            {
                if (sv.Name.Length==0)
                {
                    MessageBox.Show("Only a named field can be part of an array !");
                    return false;
                }
                if (Struct.FindStructureValue(sv.Name,a1,a2)!=null)
                {
                    MessageBox.Show("A field with the same array indexes already exists !");
                    return false;
                }
                sv.Array1 = a1;
                sv.Array2 = a2;
                return true;
            }
            return true;
        }
        private bool ValidateNewFieldValue(int columnIndex,int FieldIndex,StructureValue sv,object newValue)
        {
            ColumnInfo cInfo = (ColumnInfo)dg.Columns[columnIndex].Tag;
            string result = "";
            if (cInfo.Field.List)          
                    return true;
            if (prj.ValidateValueForType(cInfo, sv.FieldValues[FieldIndex], newValue, baseResources, stringResources, out result) == false)
            {
                MessageBox.Show(result);
                return false;
            }
            if (result!=null)
                sv.FieldValues[FieldIndex] = result;
            return true;
        }

        private void OnShowUniqueIDs(object sender, EventArgs e)
        {
            dg.Invalidate();
        }

        private void OnHideInvisibleFields(object sender, EventArgs e)
        {
            FilterRows();
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            Struct.Values.Sort();
            UpdateDataGridRows();
        }

        public void RefreshStructure()
        {
            if (Struct!=null)
            {
                InitDataGrid();
                OnRefresh(null, null);
            }
        }

        private void OnColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Tag == null)
                return;
            ColumnInfo cInfo = (ColumnInfo)e.Column.Tag;
            if (cInfo.Field != null)
                cInfo.Field.ColumnWidth = e.Column.Width;
        }

        private void OnChangeDisplayIndex(object sender, DataGridViewColumnEventArgs e)
        {
            for (int tr = 3; tr < dg.Columns.Count;tr++)
            {
                if (dg.Columns[tr].Tag == null)
                    return;
                ColumnInfo cInfo = (ColumnInfo)dg.Columns[tr].Tag;
                if (cInfo.Field != null)
                    cInfo.Field.DisplayIndex = dg.Columns[tr].DisplayIndex; 
            }
        }

        private void OnDuplicateValue(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count==0)
            {
                MessageBox.Show("Please select multiple cells of the same type !");
                return;
            }
            string s = "";
            ColumnInfo cInfo = null;
            for (int tr=0;tr<dg.SelectedCells.Count;tr++)
            {
                int columnIndex = dg.SelectedCells[tr].ColumnIndex;                
                if (dg.Columns[columnIndex].Tag == null)
                    continue;
                ColumnInfo ci = (ColumnInfo)dg.Columns[columnIndex].Tag;
                string forma = ci.Field.CanBeNull.ToString()+"::"+ci.Field.Type+"::"+ci.Field.List.ToString();
                if (cInfo==null)
                {
                    s = forma;
                    cInfo = ci;
                }
                if (s!=forma)
                {
                    MessageBox.Show("All selected cells must be of the same type and have the same 'list' and 'can be null' property !");
                    return;
                }
            }
            if (cInfo==null)
            {
                MessageBox.Show("Please select a cell that has a structure value and not Name, array of NULL property !");
                return;
            }
            // stiu tipul - calculez valoarea pe care sa o pun
            string newValue = null;
            if (cInfo.Field.List)
            {
                DataTypeListValuesEditDialog lstDlg = new DataTypeListValuesEditDialog(Context, cInfo, "");
                if (lstDlg.ShowDialog() == DialogResult.OK)
                    newValue = lstDlg.ResultList;
            }
            else
            {
                switch (cInfo.Mode)
                {
                    case ConstantModeType.BasicTypes:
                        BasicTypeConstantEditDialog bdlg = new BasicTypeConstantEditDialog(prj, false, "", cInfo.BasicType, "", "");
                        if (bdlg.ShowDialog() == DialogResult.OK)
                            newValue = bdlg.FieldValue;
                        break;
                    case ConstantModeType.Enumerations:
                        EnumSelectValueDialog edlg = new EnumSelectValueDialog(prj, cInfo.E.Name, "");
                        if (edlg.ShowDialog() == DialogResult.OK)
                            newValue = edlg.EnumValueResult;
                        break;
                    case ConstantModeType.Resources:
                        ResourceSelectDialog rdlg = new ResourceSelectDialog(Context, cInfo.ResType, true, cInfo.Field.CanBeNull);
                        if (rdlg.ShowDialog() == DialogResult.OK)
                            newValue = rdlg.SelectedResource;
                        break;
                }
            }
            if (newValue == null)
            {
                MessageBox.Show("Setting multiple values for selected type is not possible !");
                return;   
            }
            Dictionary<int, bool> d = new Dictionary<int, bool>();
            for (int tr = 0; tr < dg.SelectedCells.Count; tr++)
            {
                int columnIndex = dg.SelectedCells[tr].ColumnIndex;
                if (dg.Columns[columnIndex].Tag == null)
                    continue;
                if (columnIndex<3)
                    continue;
                StructureValue sv = (StructureValue)dg.Rows[dg.SelectedCells[tr].RowIndex].Tag;
                d[dg.SelectedCells[tr].RowIndex] = true;
                sv.FieldValues[columnIndex - 3] = newValue;
            }
            foreach (int rowIndex in d.Keys)
                UpdateDataGridRow(rowIndex);
        }


    }
}
