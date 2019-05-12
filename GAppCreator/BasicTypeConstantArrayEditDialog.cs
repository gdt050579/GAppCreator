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
    public partial class BasicTypeConstantArrayEditDialog : Form
    {

        public class ColorPickerColumn : DataGridViewColumn
        {
            public ColorPickerColumn()
                : base(new ColorPickerCell())
            {

            }

            public override DataGridViewCell CellTemplate
            {
                get
                {
                    return base.CellTemplate;
                }
                set
                {
                    // Ensure that the cell used for the template is a CalendarCell.
                    if (value != null &&
                        !value.GetType().IsAssignableFrom(typeof(ColorPickerCell)))
                    {
                        throw new InvalidCastException("Must be a ColorPicker");
                    }
                    base.CellTemplate = value;
                }
            }
        }

        public class ColorPickerCell : DataGridViewButtonCell
        {
            SolidBrush brush = new SolidBrush(Color.Black);

            public override object DefaultNewRowValue
            {
                get
                {
                    return "0xFFFFFFFF";
                }
            }

            protected override void Paint(Graphics graphics,
                                            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                            DataGridViewElementStates elementState, object value,
                                            object formattedValue, string errorText,
                                            DataGridViewCellStyle cellStyle,
                                            DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                            DataGridViewPaintParts paintParts)
            {
                if (this.Value == null)
                {
                    this.Value = "0xFFFFFFFF";
                    value = this.Value;
                }
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, null, errorText, cellStyle, advancedBorderStyle, paintParts);




                if (value != null)
                {
                    Rectangle r = new Rectangle(cellBounds.X + 10, cellBounds.Y + 5, 20, cellBounds.Height - 10);
                    int result = 0;
                    string s = value.ToString();
                    if (s.StartsWith("0x")) {
                        result = int.Parse(s.Substring(2),System.Globalization.NumberStyles.HexNumber);
                        brush.Color = Color.FromArgb(result);
                        graphics.FillRectangle(brush, r);
                    }
                    graphics.DrawRectangle(Pens.Black, r);
                    r.X = cellBounds.X + 40;
                    r.Y = cellBounds.Y+5;
                    r.Width = cellBounds.Width - 45;
                    r.Height = cellBounds.Height - 10;

                    graphics.DrawString(s, cellStyle.Font, System.Drawing.Brushes.Black, r);
                }
            }
        }


        Project prj;
        StringFormat firstRawFormat = new StringFormat();
        BasicTypesConstantType BasicType = BasicTypesConstantType.None;
        string originalValue = "";
        public string ResultValue = "";
        public int ColumnsCount;
        public BasicTypeConstantArrayEditDialog(Project _prj,BasicTypesConstantType bct,string value,int columnsCount)
        {
            InitializeComponent();
            prj = _prj;
            BasicType = bct;
            originalValue = value;
            firstRawFormat.Alignment = StringAlignment.Far;
            dg.RowHeadersWidth = 70;
            dg.RowPostPaint += dg_RowPostPaint;
            if (bct == BasicTypesConstantType.Color)
                dg.CellContentClick += dg_CellContentClick;
            if (columnsCount < 1)
                columnsCount = 1;
            InsertColumns(columnsCount);
            PopulateInitialValues(value, columnsCount);
        }

        void dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            object value = dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (value!=null)
            {
                int result = 0;
                string s = value.ToString();
                if (s.StartsWith("0x"))
                    result = int.Parse(s.Substring(2),System.Globalization.NumberStyles.HexNumber);
                dlg.Color = Color.FromArgb(result);
            }
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0x"+dlg.Color.ToArgb().ToString("X8");
        }
        void PopulateInitialValues(string value,int columnsCount)
        {
            List<string> v = null;
            if (BasicType == BasicTypesConstantType.String)
                v = Project.StringListToList(value, ';', false);
            else
                v = Project.StringListToList(value, ',');
            if (v == null)
                return;
            int rows = v.Count / columnsCount;
            if ((v.Count % columnsCount) != 0)
                rows++;
            if (rows <= 0)
                rows = 1;
            InsertRows(rows);
            int y = 0;
            int x = 0;
            for (int tr=0;tr<v.Count;tr++)
            {
                dg.Rows[y].Cells[x].Value = v[tr];
                x++;
                if (x == columnsCount)
                {
                    x = 0;
                    y++;
                }
            }
        }
        void InsertColumns(int count = 1,int index = -1)
        {
            for (int tr = 0; tr < count; tr++)
            {
                DataGridViewColumn col = null;
                switch (BasicType)
                {
                    case BasicTypesConstantType.Boolean:
                        col = new DataGridViewCheckBoxColumn();
                        col.Width = 40;
                        break;
                    case BasicTypesConstantType.Int8:
                    case BasicTypesConstantType.Int16:
                    case BasicTypesConstantType.Int32:
                    case BasicTypesConstantType.Int64:
                    case BasicTypesConstantType.UInt8:
                    case BasicTypesConstantType.UInt16:
                    case BasicTypesConstantType.UInt32:
                    case BasicTypesConstantType.UInt64:
                    case BasicTypesConstantType.Float32:
                    case BasicTypesConstantType.Float64:
                        col = new DataGridViewTextBoxColumn();
                        col.Width = 40;
                        break;
                    case BasicTypesConstantType.String:
                        col = new DataGridViewTextBoxColumn();
                        col.Width = 100;
                        break;
                    case BasicTypesConstantType.Color:
                        //col = new DataGridViewButtonColumn();
                        col = new ColorPickerColumn();
                        col.Width = 130;
                        break;
                }
                if (col != null)
                {
                    if (index < 0)
                        dg.Columns.Add(col);
                    else
                        dg.Columns.Insert(index, col);
                }
            }

            // numerotez
            for (int tr = 0; tr < dg.Columns.Count; tr++)
                dg.Columns[tr].HeaderText = tr.ToString();
        }
        void InsertRows(int count = 1,int index = -1)
        {
            if (index < 0)
                dg.Rows.Add(count);
            else
                dg.Rows.Insert(index, count);
        }
        void dg_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dg = (DataGridView)sender;
            if (e.RowIndex < 0)
                return;
            if (dg.Rows[e.RowIndex].IsNewRow)
                return;
            string rowNumber = e.RowIndex.ToString();
            SizeF size = e.Graphics.MeasureString(rowNumber, this.Font);
            if (dg.RowHeadersWidth < (int)(size.Width + 20)) dg.RowHeadersWidth = (int)(size.Width + 20);
            e.Graphics.DrawString(rowNumber, dg.Font, SystemBrushes.ControlText, dg.RowHeadersWidth - 5, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2), firstRawFormat);
        }
        private void OnOK(object sender, EventArgs e)
        {
            if (dg.Columns.Count==0)
            {
                MessageBox.Show("You have to have at least one column !");
                return;
            }
            if (dg.Rows.Count == 0)
            {
                MessageBox.Show("You have to have at least one row !");
                return;
            }
            ResultValue = "";
            for (int y = 0; y < dg.Rows.Count;y++ )
            {
                for (int x=0;x<dg.Columns.Count;x++)
                {
                    string value = "";
                    if (dg.Rows[y].Cells[x].Value!=null)
                        value = dg.Rows[y].Cells[x].Value.ToString();
                    string result = ConstantHelper.ValidateBasicTypeValue(value, BasicType);
                    if (result == null)
                    {
                        MessageBox.Show("Value for Row:" + y.ToString() + " and Column:" + x.ToString() + " [" + value + "] is not of type " + BasicType.ToString());
                        return;
                    }
                    ResultValue += result;
                    if (BasicType == BasicTypesConstantType.String)
                    {
                        ResultValue += ";";
                        if (result.Contains(';'))
                        {
                            MessageBox.Show("String from Row:" + y.ToString() + " and Column:" + x.ToString() + " [" + result + "] contains a semicolumn (';') that is an invalid character for array lists !");
                            return;
                        }
                    }
                    else
                        ResultValue += ",";
                }
            }
            // Sterg ultimul caracter
            ResultValue = ResultValue.Substring(0, ResultValue.Length - 1);
            ColumnsCount = dg.Columns.Count;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnAddColumn(object sender, EventArgs e)
        {
            InsertColumns();
        }

        private void OnInsertColumn(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count!=1)
            {
                MessageBox.Show("Please select only one cell !");
                return;
            }
            InsertColumns(1, dg.SelectedCells[0].ColumnIndex);
        }

        private void OnAddRow(object sender, EventArgs e)
        {
            InsertRows();
        }

        private void OnInsertRow(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count != 1)
            {
                MessageBox.Show("Please select only one cell !");
                return;
            }
            InsertRows(1, dg.SelectedCells[0].RowIndex);
        }

        private void OnDeleteColumn(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell !");
                return;
            }
            Dictionary<DataGridViewColumn, bool> d = new Dictionary<DataGridViewColumn, bool>();
            foreach (DataGridViewCell cell in dg.SelectedCells)
            {
                if ((cell.OwningColumn != null))
                    d[cell.OwningColumn] = true;
            }
            if (d.Keys.Count == 0)
            {
                MessageBox.Show("Please select a valid cell !");
                return;
            }
            if (MessageBox.Show("Delete " + d.Keys.Count.ToString() + " columns ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewColumn o in d.Keys)
                    dg.Columns.Remove(o);
            }
        }

        private void OnDeleteRow(object sender, EventArgs e)
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
            if (MessageBox.Show("Delete " + d.Keys.Count.ToString() + " rows ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (DataGridViewRow o in d.Keys)
                    dg.Rows.Remove(o);
            }
        }

        private void OnSetColumnsWidth(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Columns width", 50, 20, 1000);
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int w = (int)ib.FloatResult;
                if (w < 20) w = 20;
                for (int tr = 0; tr < dg.Columns.Count; tr++)
                    dg.Columns[tr].Width = w;                    
            }
        }

        private void OnDuplicateValue(object sender, EventArgs e)
        {
            if (dg.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select at least one cell !");
                return;
            }
            BasicTypeConstantEditDialog dlg = new BasicTypeConstantEditDialog(prj, false, "", BasicType, "", "");
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int tr = 0; tr < dg.SelectedCells.Count; tr++)
                    dg.SelectedCells[tr].Value = dlg.FieldValue;
            }
        }
    }
}
