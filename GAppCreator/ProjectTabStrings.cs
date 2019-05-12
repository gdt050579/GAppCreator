using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GAppCreator
{
    public partial class ProjectTabStrings : BaseProjectContainer
    {
        private class ImagePreview
        {
            public int Left, Right, Top, Bottom;
            public Bitmap bmp = null;
            public string BitmapName = "";

            [XmlIgnore(), Description(""), Category("Location"), DisplayName("Top")]
            public int _Top
            {
                get { return Top; }
                set { Top = value; }
            }
            [XmlIgnore(), Description(""), Category("Location"), DisplayName("Bottom")]
            public int _Bottom
            {
                get { return Bottom; }
                set { Bottom = value; }
            }
            [XmlIgnore(), Description(""), Category("Location"), DisplayName("Left")]
            public int _Left
            {
                get { return Left; }
                set { Left = value; }
            }
            [XmlIgnore(), Description(""), Category("Location"), DisplayName("Right")]
            public int _Right
            {
                get { return Right; }
                set { Right = value; }
            }
            [XmlIgnore(), Description(""), Category("Size"), DisplayName("Width")]
            public int _Width
            {
                get { return Right-Left; }
                set { Right = Left+value; }
            }
            [XmlIgnore(), Description(""), Category("Size"), DisplayName("Height")]
            public int _Height
            {
                get { return Bottom - Top; }
                set { Bottom = Top + value; }
            }
            [XmlIgnore(), Description(""), Category("Image"), DisplayName("Size")]
            public string _Size
            {
                get { if (bmp != null) return bmp.Width.ToString() + " x " + bmp.Height.ToString(); else return ""; }
            }
            [XmlIgnore(), Description(""), Category("Image"), DisplayName("Name")]
            public string _Name
            {
                get { return BitmapName; }
            }
            [XmlIgnore(), Description(""), Category("Scale"), DisplayName("Horizontal")]
            public string _Horizontal
            {
                get 
                {
                    if (bmp == null)
                        return "0%";
                    return Project.ProcentToString(((float)(Right-Left)) / (float)bmp.Width); 
                }
                set
                {
                    float v = 0;
                    if (Project.StringToProcent(value, ref v) == false)
                        MessageBox.Show("Invalid percentage value");
                    if ((v > 0) && (bmp != null))
                        Right = (int)(Left + v * bmp.Width);
                }
            }
            [XmlIgnore(), Description(""), Category("Scale"), DisplayName("Vertical")]
            public string _Vertical
            {
                get
                {
                    if (bmp == null)
                        return "0%";
                    return Project.ProcentToString(((float)(Bottom - Top)) / (float)bmp.Height);
                }
                set
                {
                    float v = 0;
                    if (Project.StringToProcent(value, ref v) == false)
                        MessageBox.Show("Invalid percentage value");
                    if ((v > 0) && (bmp != null))
                        Bottom = (int)(Top + v * bmp.Height);
                }
            }

            public bool SetRect(ViewRectangle r,bool force)
            {
                if ((r.Left!=Left) || (r.Right!=Right) || (r.Top!=Top) || (r.Bottom!=Bottom) || (force))
                {
                    Left = r.Left;
                    Top = r.Top;
                    Right = r.Right;
                    Bottom = r.Bottom;
                    return true;
                }
                return false;
            }
            public void SetOriginalSize()
            {
                if (bmp!=null)
                {
                    Right = Left + bmp.Width;
                    Bottom = Top + bmp.Height;
                }
            }
        }
        Dictionary<Language, bool> SelectedLanguages = new Dictionary<Language, bool>();
        GAppCreator.GraphicContext gc = new GraphicContext();
        TextPainter tp = new TextPainter();
        bool forceNewCoordonated = false;
        Color marginColor;
        ViewRectangle rectText = new ViewRectangle();
        ViewRectangle rectImage = new ViewRectangle();
        ViewRectangle currectRect = null;
        ImagePreview img = new ImagePreview();

        const int NumberOfFixedColumns = 4;
        bool firstEnter = true;

        public ProjectTabStrings()
        {
            InitializeComponent();
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                SelectedLanguages[l] = true;
                comboTranslateFrom.Items.Add(l.ToString());
                if (l == Language.English)
                    comboTranslateFrom.SelectedIndex = comboTranslateFrom.Items.Count - 1;
            }
            currectRect = rectText;
            marginColor = Color.White;
            propPreview.SelectedObject = tp;

            dgStrings.CellContentClick += dgStrings_CellContentClick;

            CreateDataGridColumns();
            UpdateLanguageVisibility();
        }

        #region DataGrid
        private void CreateDataGridColumns()
        {
            DataGridViewTextBoxColumn dgc;
            System.Windows.Forms.DataGridViewCellStyle cellStyle;

            dgStrings.Rows.Clear();
            dgStrings.Columns.Clear();

            dgc = new DataGridViewTextBoxColumn();
            dgc.HeaderText = "Variable Name";
            dgc.Width = 200;
            dgc.Frozen = true;
            cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            cellStyle.BackColor = Color.FromArgb(240, 255, 255);
            dgc.DefaultCellStyle = cellStyle;
            dgc.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgStrings.Columns.Add(dgc);

            dgc = new DataGridViewTextBoxColumn();
            dgc.HeaderText = "Array";
            dgc.Width = 50;
            dgc.Frozen = true;
            cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            cellStyle.BackColor = Color.FromArgb(230, 255, 255);
            dgc.DefaultCellStyle = cellStyle;
            dgc.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgStrings.Columns.Add(dgc);

            DataGridViewButtonColumn dgcc = new DataGridViewButtonColumn();
            dgcc.HeaderText = "Template";
            dgcc.Width = 120;
            dgcc.Frozen = true;
            dgcc.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgStrings.Columns.Add(dgcc);

            DataGridViewCheckBoxColumn dgcb = new DataGridViewCheckBoxColumn();
            dgcb.HeaderText = "Non-Drawable";
            dgcb.Width = 80;
            dgcb.Frozen = true;
            dgcb.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgStrings.Columns.Add(dgcb);

            // strings
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                dgc = new DataGridViewTextBoxColumn();
                dgc.HeaderText = l.ToString();
                dgc.Tag = l;
                dgc.SortMode = DataGridViewColumnSortMode.NotSortable;
                cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
                cellStyle.BackColor = Color.FromArgb(255, 255, 255);
                dgc.DefaultCellStyle = cellStyle;
                dgStrings.Columns.Add(dgc);
            }
        }
        private void UpdateDataGridRow(int index)
        {
            StringValues sv = Context.Prj.Strings[index];
            dgStrings.Rows[index].Tag = sv;
            dgStrings.Rows[index].Cells[0].Value = sv.VariableName;
            dgStrings.Rows[index].Cells[1].Value = Project.ArrayToString(sv.Array1, sv.Array2);
            dgStrings.Rows[index].Cells[2].Value = sv.PreviewTemplate;
            dgStrings.Rows[index].Cells[3].Value = sv.NonDrawable;

            int count = NumberOfFixedColumns;
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                dgStrings.Rows[index].Cells[count].Value = sv.Get(l);
                dgStrings.Rows[index].Cells[count].Tag = l;
                count++;
            }
        }
        private void UpdateLanguageVisibility()
        {
            DataGridViewTextBoxColumn dgc;
            Language l;
            for (int tr = NumberOfFixedColumns; tr < dgStrings.Columns.Count;tr++)
            {
                dgc = (DataGridViewTextBoxColumn)dgStrings.Columns[tr];
                l = (Language)dgc.Tag;
                dgc.Visible = SelectedLanguages[l];
            }
        }
        private void UpdateDataGrid()
        {
            dgStrings.Rows.Clear();
            if ((Context.Prj != null)  && (Context.Prj.Strings != null) && (Context.Prj.Strings.Count>0))
            {
                Context.Prj.Strings.Sort(delegate(StringValues i1, StringValues i2) { return i1.GetSortName().CompareTo(i2.GetSortName()); });
                dgStrings.Rows.Add(Context.Prj.Strings.Count);
                for (int tr = 0; tr < Context.Prj.Strings.Count; tr++)
                {
                    UpdateDataGridRow(tr);
                }              
            }
            FilterRows();
        }
        private void OnValidateCell(object sender, DataGridViewCellValidatingEventArgs e)
        {
            StringValues sv = (StringValues)dgStrings.Rows[e.RowIndex].Tag;
            // numele
            if (e.ColumnIndex == 0)
            {
                string newValue = e.FormattedValue.ToString();
                if (newValue.Equals(sv.VariableName) == false)
                    e.Cancel = !ValidateNewName(e.RowIndex, sv, newValue);
            }
            // Array
            if (e.ColumnIndex == 1)
            {
                e.Cancel = !ValidateNewArray(sv, e.FormattedValue.ToString());
            }
            // Template
            if (e.ColumnIndex == 2)
            {
                //bool val = (bool)e.FormattedValue;
                //if (val != sv.IsNull)
                //{
                //    sv.IsNull = val;
                //    UpdateDataGridRow(e.RowIndex);
                //    dg.InvalidateRow(e.RowIndex);
                //}
            }
            // non-drawable
            if (e.ColumnIndex == 3)
            {
                sv.NonDrawable = (bool)e.FormattedValue;
            }
            // restul coloanelor
            if (e.ColumnIndex >= NumberOfFixedColumns)
            {
                string newValue = e.FormattedValue.ToString();
                object o = dgStrings.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                string cValue = "";
                if (o!=null) cValue = o.ToString();
                if (newValue.Equals(cValue)==false)
                {
                    Language l = (Language)dgStrings.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                    sv.Set(l,newValue);
                }
            }
        }
        private bool ValidateNewName(int rowIndex, StringValues sv, string newName)
        {
            // delete daca numele e gol
            if (newName.Length == 0)
            {
                MessageBox.Show("A string can not have an empty name !");
                return false;
            }
            if (Project.ValidateVariableNameCorectness(newName, false) == false)
            {
                MessageBox.Show("Invalid string name - it must contains letters, numbers and symbol '_' !");
                return false;
            }
            // verific daca mai e o variabila cu acelasi nume
            for (int tr=0;tr<Context.Prj.Strings.Count;tr++)
            {
                if ((Context.Prj.Strings[tr].VariableName.Equals(newName, StringComparison.InvariantCultureIgnoreCase)) && (Context.Prj.Strings[tr].Array1<0))
                {
                    MessageBox.Show("String '" + newName + "' already exists !");
                    return false;
                }
            }

            // decid daca trebuie sa redenumesc pe toate sau doar unul
            if ((sv.Array1 < 0) && (sv.Array2 < 0))
            {
                sv.VariableName = newName;
            }
            else
            {
                if (MessageBox.Show("String '" + sv.VariableName + "' is part of an array. Would you like to rename all of the array variables to the new name : '" + newName + "' ?", "Rename", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    for (int tr = 0; tr < dgStrings.Rows.Count; tr++)
                    {
                        StringValues rowSV = (StringValues)dgStrings.Rows[tr].Tag;
                        if (rowSV == sv)
                            continue;
                        if (rowSV.VariableName.Equals(sv.VariableName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            rowSV.VariableName = newName;
                            UpdateDataGridRow(tr);
                        }
                    }
                }
                sv.VariableName = newName;
            }
            return true;
        }
        private bool ValidateNewArray(StringValues sv, string newArray)
        {
            int a1 = -1, a2 = -1;
            if ((newArray.Equals("-")) && (sv.Array1 == -1) && (sv.Array2 == -1))
                return true;
            bool res = Project.StringToArray(newArray, ref a1, ref a2);
            if (res == false)
            {
                MessageBox.Show("Invalid array identifier. \nUse '-' or an empty string for no array, a number for a vector index (Ex: 5), or two numbers separated by a comma for matrix indexes (Ex: 3,4) !");
                return false;
            }
            if ((a1 != sv.Array1) || (a2 != sv.Array2))
            {
                for (int tr = 0; tr < Context.Prj.Strings.Count; tr++)
                {
                    if ((Context.Prj.Strings[tr].VariableName.Equals(sv.VariableName, StringComparison.InvariantCultureIgnoreCase)) && (Context.Prj.Strings[tr].Array1==a1) && (Context.Prj.Strings[tr].Array2==a2))
                    {
                        MessageBox.Show("A field with the same array indexes already exists !");
                        return false;
                    }
                }
                sv.Array1 = a1;
                sv.Array2 = a2;
                return true;
            }
            return true;
        }
        private void ValidateTemplateValues()
        {
            for (int tr=0;tr<dgStrings.Rows.Count;tr++)
            {
                StringValues sv = (StringValues)dgStrings.Rows[tr].Tag;
                if ((sv.PreviewTemplate==null) || (sv.PreviewTemplate.Length==0) || (sv.PreviewTemplate == "-"))
                    continue;
                bool found = false;
                for (int gr=0;gr<Context.Prj.StringTemplates.Count;gr++)
                    if (sv.PreviewTemplate.Equals(Context.Prj.StringTemplates[gr].Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                if (!found)
                {
                    sv.PreviewTemplate = "";
                    UpdateDataGridRow(tr);
                }
            }
        }
        private DataGridViewCell GetSelectedCell()
        {
            if (dgStrings.SelectedCells.Count != 1)
                return null;
            if (dgStrings.SelectedCells[0].ColumnIndex < NumberOfFixedColumns)
                return null;
            return dgStrings.SelectedCells[0];
        }
        private void UpdateSelectedCellsGridRows()
        {
            Dictionary<int, bool> d = new Dictionary<int, bool>();
            for (int tr=0;tr<dgStrings.SelectedCells.Count;tr++)
            {
                d[dgStrings.SelectedCells[tr].RowIndex] = true;
            }
            foreach (int ii in d.Keys)
                UpdateDataGridRow(ii);
        }
        private void SetCellCorespondingStringValue(DataGridViewCell cell,string newValue)
        {
            if (cell.ColumnIndex >= NumberOfFixedColumns)
            {
                StringValues sv = (StringValues)dgStrings.Rows[cell.RowIndex].Tag;
                Language l = (Language)cell.Tag;
                sv.Set(l, newValue);
            }           
        }
        private void OnRefreshDataGrid(object sender, EventArgs e)
        {
            UpdateDataGrid();
            ValidateTemplateValues();
        }
        private void OnAddNewString(object sender, EventArgs e)
        {
            StringAddDialog dlg = new StringAddDialog(Context.Prj);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;
            if ((dlg.Array1>0) && (dlg.Array2>0))
            {
                for (int tr=0;tr<dlg.Array1;tr++)
                {
                    for (int gr=0;gr<dlg.Array2;gr++)
                    {
                        StringValues sv = new StringValues();
                        sv.VariableName = dlg.ResultName;
                        sv.Array1 = tr;
                        sv.Array2 = gr;
                        sv.Set(dlg.Values);
                        Context.Prj.Strings.Add(sv);
                    }
                }
            } else 
            if ((dlg.Array1>0) && (dlg.Array2<=0))
            {
                for (int tr = 0; tr < dlg.Array1; tr++)
                {
                    StringValues sv = new StringValues();
                    sv.VariableName = dlg.ResultName;
                    sv.Array1 = tr;
                    sv.Array2 = -1;
                    sv.Set(dlg.Values);
                    Context.Prj.Strings.Add(sv);
                }
            }
            else
            {
                StringValues sv = new StringValues();
                sv.VariableName = dlg.ResultName;
                sv.Array1 = -1;
                sv.Array2 = -1;
                sv.Set(dlg.Values);
                Context.Prj.Strings.Add(sv);
            }
            UpdateDataGrid();
            ValidateTemplateValues();
            dgStrings.ClearSelection();
            for (int tr=0;tr<this.dgStrings.Rows.Count;tr++)
            {
                object o = dgStrings.Rows[tr].Cells[0].Value;
                if (o == null)
                    continue;
                if (o.ToString() == dlg.ResultName)
                {
                    dgStrings.Rows[tr].Selected = true;
                    dgStrings.FirstDisplayedScrollingRowIndex = dgStrings.Rows[tr].Index;
                    return;
                }
            }
        }
        private void OnResizeArray(object sender, EventArgs e)
        {
            string cName = "";
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
            {
                if ((cell.OwningRow != null) && (cell.OwningRow.IsNewRow == false))
                {
                    StringValues sv = (StringValues)cell.OwningRow.Tag;
                    cName = sv.VariableName;
                }
            }
            StringResizeArray dlg = new StringResizeArray(Context.Prj, cName);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ResizeStringArray(dlg.VariableName, dlg.oldArray1, dlg.oldArray2, dlg.newArray1, dlg.newArray2);
                UpdateDataGrid();
            }
        }
        private void AddNewString(string name, int array1, int array2, string template)
        {
            StringValues sv = new StringValues();
            sv.VariableName = name;
            sv.Array1 = array1;
            sv.Array2 = array2;
            sv.PreviewTemplate = template;
            Context.Prj.Strings.Add(sv);
        }
        private StringValues FindStringValue(string name, int array1, int array2)
        {
            foreach (StringValues sv in Context.Prj.Strings)
            {
                if ((sv.Array1 != array1) && (array1 >= 0))
                    continue;
                if ((sv.Array2 != array2) && (array2 >= 0))
                    continue;
                if (sv.VariableName.Equals(name))
                    return sv;
            }
            return null;
        }
        public void ResizeStringArray(string name, int oldArray1, int oldArray2, int newArray1, int newArray2)
        {
            char newCase = 'n';
            char oldCase = 'n';
            if ((oldArray1 > 0) && (oldArray2 > 0))
                oldCase = 'm';
            if ((oldArray1 > 0) && (oldArray2 <= 0))
                oldCase = 'v';
            if ((newArray1 > 0) && (newArray2 > 0))
                newCase = 'm';
            if ((newArray1 > 0) && (newArray2 <= 0))
                newCase = 'v';

            List<StringValues> toDelete = new List<StringValues>();

            // caz 1: (none -> vector, none-> array)
            if (oldCase == 'n')
            {
                StringValues sv = FindStringValue(name, -1, -1);
                if (sv != null)
                {
                    sv.Array1 = 0;
                    if (newCase == 'm')
                    {
                        sv.Array2 = 0;
                        for (int tr = 0; tr < newArray1; tr++)
                            for (int gr = 0; gr < newArray2; gr++)
                                if ((tr == 0) && (gr == 0))
                                {

                                }
                                else
                                {
                                    AddNewString(name, tr, gr, sv.PreviewTemplate);
                                }                                    
                    }
                    else
                    {
                        for (int tr = 1; tr < newArray1; tr++)
                            AddNewString(name, tr, -1, sv.PreviewTemplate);
                    }
                }
                return;
            }
            // caz 2: vector->none sau matrix->none
            if (newCase == 'n')
            {
                // cel mai mic devine none - restul sunt sterse
                StringValues found = null;
                if (oldCase == 'm')
                    found = FindStringValue(name, 0, 0);
                else
                    found = FindStringValue(name, 0, -1);

                foreach (StringValues sv in Context.Prj.Strings)
                {
                    if (sv.VariableName.Equals(name) == false)
                        continue;
                    if (sv != found)
                        toDelete.Add(sv);
                }
                found.Array1 = found.Array2 = -1;
                foreach (StringValues sv in toDelete)
                    Context.Prj.Strings.Remove(sv);
                if (found == null)
                    AddNewString(name, -1, -1, "");
                return;
            }
            Dictionary<string, StringValues> d = new Dictionary<string, StringValues>();
            // caz 3: vector -> vector
            if ((oldCase == 'v') && (newCase == 'v'))
            {
                string template = null;
                foreach (StringValues sv in Context.Prj.Strings)
                {
                    if (sv.VariableName.Equals(name) == false)
                        continue;
                    d[Project.GetVariableName(name, sv.Array1, -1)] = sv;
                    if (template == null)
                        template = sv.PreviewTemplate;
                }
                if (template == null)
                    template = "";
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    string key = Project.GetVariableName(name, tr, -1);
                    bool exists = d.ContainsKey(key);
                    if (tr < newArray1)
                    {
                        if (!exists)
                            AddNewString(name, tr, -1, template);
                    }
                    else
                    {
                        if (exists)
                            Context.Prj.Strings.Remove(d[key]);
                    }
                }
                return;
            }
            // caz 4: matrix -> matrix
            if ((oldCase == 'm') && (newCase == 'm'))
            {
                string template = null;
                foreach (StringValues sv in Context.Prj.Strings)
                {
                    if (sv.VariableName.Equals(name) == false)
                        continue;
                    d[Project.GetVariableName(name, sv.Array1, sv.Array2)] = sv;
                    if (template == null)
                        template = sv.PreviewTemplate;
                }
                if (template == null)
                    template = "";
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                int maxIndex2 = Math.Max(newArray2, oldArray2);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    for (int gr = 0; gr < maxIndex2; gr++)
                    {
                        string key = Project.GetVariableName(name, tr, gr);
                        bool exists = d.ContainsKey(key);
                        if ((tr < newArray1) && (gr < newArray2))
                        {
                            if (!exists)
                                AddNewString(name, tr, gr, template);
                        }
                        else
                        {
                            if (exists)
                                Context.Prj.Strings.Remove(d[key]);
                        }
                    }
                }
                return;
            }
            // caz 5: vector->matrix
            if ((oldCase == 'v') && (newCase == 'm'))
            {
                string template = null;
                foreach (StringValues sv in Context.Prj.Strings)
                {
                    if (sv.VariableName.Equals(name) == false)
                        continue;
                    sv.Array2 = 0;
                    d[Project.GetVariableName(name, sv.Array1, 0)] = sv;
                    if (template == null)
                        template = sv.PreviewTemplate;
                }
                if (template == null)
                    template = "";
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                int maxIndex2 = Math.Max(newArray2, oldArray2);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    for (int gr = 0; gr < maxIndex2; gr++)
                    {
                        string key = Project.GetVariableName(name, tr, gr);
                        bool exists = d.ContainsKey(key);
                        if ((tr < newArray1) && (gr < newArray2))
                        {
                            if (!exists)
                                AddNewString(name, tr, gr, template);
                        }
                        else
                        {
                            if (exists)
                                Context.Prj.Strings.Remove(d[key]);
                        }
                    }
                }
                return;
            }
            // caz 6: matrix->vector
            if ((oldCase == 'm') && (newCase == 'v'))
            {
                string template = null;
                foreach (StringValues sv in Context.Prj.Strings)
                {
                    if (sv.VariableName.Equals(name) == false)
                        continue;
                    d[Project.GetVariableName(name, sv.Array1, sv.Array2)] = sv;
                    if (template == null)
                        template = sv.PreviewTemplate;
                }
                if (template == null)
                    template = "";
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                int maxIndex2 = Math.Max(newArray2, oldArray2);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    for (int gr = 0; gr < maxIndex2; gr++)
                    {
                        string key = Project.GetVariableName(name, tr, gr);
                        bool exists = d.ContainsKey(key);
                        if ((tr < newArray1) && (gr < 1))
                        {
                            if (!exists)
                                AddNewString(name, tr, -1, template);
                            else
                                d[key].Array2 = -1;
                        }
                        else
                        {
                            if (exists)
                                Context.Prj.Strings.Remove(d[key]);
                        }
                    }
                }
                return;
            }
        }
        void dgStrings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0 ) || (e.ColumnIndex != 2))
                return;
            StringValues sv = (StringValues)dgStrings.Rows[e.RowIndex].Tag;
            StringTemplateSelector dlg = new StringTemplateSelector(Context.Prj,sv.PreviewTemplate);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                sv.PreviewTemplate = dlg.SelectedTemplate;
                UpdateDataGridRow(e.RowIndex);
                OnStringTableSelectionChanged(null, null);
            }
        }
        #endregion


        #region Filter
        private void OnFilterLanguages(object sender, EventArgs e)
        {
            LanguageSelectDialog dlg = new LanguageSelectDialog(SelectedLanguages);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                UpdateLanguageVisibility();
        }

        private void OnFilterUsedLanguages(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                SelectedLanguages[l] = false;
                foreach (StringValues sv in Context.Prj.Strings)
                {
                    if (sv.Get(l).Length > 0)
                    {
                        SelectedLanguages[l] = true;
                        break;
                    }
                }
            }
            SelectedLanguages[Context.Prj.DefaultLanguage] = true;
            UpdateLanguageVisibility();
        }

        private void OnFilterAllLanguages(object sender, EventArgs e)
        {
            foreach (Language l in Enum.GetValues(typeof(Language)))
                SelectedLanguages[l] = true;
            UpdateLanguageVisibility();
        }

        private void OnFilterMostCommonLanguagesUsed(object sender, EventArgs e)
        {
            foreach (Language l in Enum.GetValues(typeof(Language)))
                SelectedLanguages[l] = false;
            SelectedLanguages[Language.English] = true;
            SelectedLanguages[Language.German] = true;
            SelectedLanguages[Language.French] = true;
            SelectedLanguages[Language.Spanish] = true;
            SelectedLanguages[Language.Portuguese] = true;
            SelectedLanguages[Language.Italian] = true;
            SelectedLanguages[Language.Romanian] = true;
            UpdateLanguageVisibility();
        }

        private void OnRefilter(object sender, EventArgs e)
        {
            FilterRows();
        }

        private void FilterRows()
        {
            string s = txFilter.Text;
            bool found = false;
            int count_visible = 0;
            int columnIndex = 1;
            if (cbAllFields.Checked)
                columnIndex = dgStrings.Columns.Count;
            if (s.Length == 0)
            {
                for (int tr = 0; tr < dgStrings.Rows.Count; tr++)
                {
                    dgStrings.Rows[tr].Visible = true;
                    count_visible++;
                }
            }
            else
            {
                for (int tr = 0; tr < dgStrings.Rows.Count; tr++)
                {
                    if (dgStrings.Rows[tr].IsNewRow)
                        continue;
                    found = false;
                    for (int gr = 0; gr < columnIndex;gr++)
                    {
                        if (dgStrings.Columns[gr].Visible == false)
                            continue;
                        object o = dgStrings.Rows[tr].Cells[gr].Value;
                        if (o == null)
                            continue;
                        if (o.ToString().IndexOf(s, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            found = true;
                            break;
                        }
                    }
                    dgStrings.Rows[tr].Visible = found;
                    if (found)
                        count_visible++;
                }
            }
            lbVisibleRecords.Text = count_visible.ToString() + " / " + dgStrings.RowCount.ToString();
            if ((count_visible == 0) && (dgStrings.RowCount > 0))
                lbVisibleRecords.ForeColor = Color.Red;
            else
                lbVisibleRecords.ForeColor = Color.Black;
        }

        private void OnFilterByVariableNames(object sender, EventArgs e)
        {
            cbOnlyVariableNames.Checked = true;
            cbAllFields.Checked = false;
            FilterRows();
        }

        private void OnFilterByAllFields(object sender, EventArgs e)
        {
            cbOnlyVariableNames.Checked = false;
            cbAllFields.Checked = true;
            FilterRows();
        }
        #endregion


        #region String manipulation functions
        private void OnSaveToCSV(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Comma separated values|*.csv|All Files|*.*";
            dlg.AddExtension = true;
            dlg.DefaultExt = "csv";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Context.Prj.EC.Reset();
                string res = "";
                foreach (DataGridViewTextBoxColumn dgc in dgStrings.Columns)
                    if (dgc.Visible)
                        res += dgc.HeaderText + ",";
                res += "\n";
                for (int tr = 0; tr < dgStrings.Rows.Count; tr++)
                {
                    for (int gr = 0; gr < dgStrings.Columns.Count; gr++)
                    {
                        if (dgStrings.Columns[gr].Visible == false)
                            continue;
                        String cellStr;
                        Object cellValue = dgStrings.Rows[tr].Cells[gr].Value;
                        if (cellValue == null)
                            cellStr = "";
                        else
                            cellStr = cellValue.ToString().Replace("\"", "\"\"");
                        res += "\"" + cellStr + "\",";
                    }
                    res += "\n";
                }
                if (Disk.SaveFile(dlg.FileName, res, Context.Prj.EC) == false)
                    Context.Prj.ShowErrors();
            }
        }

        private void OnConvertStringsToUpperCase(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
                SetCellCorespondingStringValue(cell,cell.Value.ToString().Replace("\\n", "\n").ToUpperInvariant().Replace("\n", "\\n"));
            UpdateSelectedCellsGridRows();
            OnStringTableSelectionChanged(null, null);
        }
        private void OnConvertStringsToNonDiacritical(object sender, EventArgs e)
        {
            CharacterSet cSet = new CharacterSet();
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
            {
                string s1 = cell.Value.ToString().Replace("\\n", "\n");
                string s2 = "";
                foreach (char ch in s1)
                {
                    s2 += cSet.GetNonDiacriticalChar(ch);
                }
                SetCellCorespondingStringValue(cell, s2.Replace("\n", "\\n"));
            }
            UpdateSelectedCellsGridRows();
            OnStringTableSelectionChanged(null, null);
        }
        private void OnConvertStringsToLowerCase(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
                SetCellCorespondingStringValue(cell, cell.Value.ToString().Replace("\\n", "\n").ToLowerInvariant().Replace("\n", "\\n"));
            UpdateSelectedCellsGridRows();
            OnStringTableSelectionChanged(null, null);
        }

        private void OnTranslateStrings(object sender, EventArgs e)
        {
            if ((dgStrings.SelectedCells == null) || (dgStrings.SelectedCells.Count == 0))
            {
                MessageBox.Show("Please select some cells to be translated !");
                return;
            }
            Language l = (Language)Enum.Parse(typeof(Language), comboTranslateFrom.SelectedItem.ToString());
            // verific sa nu am selectata si celula cu aceeasi limba
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
            {
                if (dgStrings.Columns[cell.ColumnIndex].HeaderText == comboTranslateFrom.SelectedItem.ToString())
                {
                    MessageBox.Show("You can not translate from " + comboTranslateFrom.SelectedItem.ToString() + " to " + comboTranslateFrom.SelectedItem.ToString() + ". Deselect cells from the " + comboTranslateFrom.SelectedItem.ToString() + " columns !");
                    return;
                }
                if (cell.ColumnIndex < 2)
                {
                    MessageBox.Show("Please select cells that corespond to a language column (not a name or array column) !");
                    return;
                }
            }
            TranslateDialog td = new TranslateDialog(Context.Prj, l, dgStrings);
            td.ShowDialog();
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
                SetCellCorespondingStringValue(cell, cell.Value.ToString());
            UpdateSelectedCellsGridRows();
            OnStringTableSelectionChanged(null, null);
        }

        private void OnCheckStringAvailability(object sender, EventArgs e)
        {
            TextPainter tempTP = new TextPainter();
            Context.Prj.CheckStringsFontAvailability();
            foreach (StringValues sv in Context.Prj.Strings)
            {
                if ((sv.PreviewTemplate == null) || (sv.PreviewTemplate.Length == 0) || (sv.NonDrawable))
                    continue;
                int idx = Context.Prj.GetStringTemplatePreview(sv.PreviewTemplate);
                if (idx<0)
                {
                    Context.Prj.EC.AddError(String.Format("String template '{0}' defined in string '{1}' does not exists !",sv.PreviewTemplate,Project.GetVariableName(sv.VariableName,sv.Array1,sv.Array2)));
                    continue;
                }
                // setez text painterul
                tempTP.Create(Context.Prj.StringTemplates[idx]);
                foreach (StringValue sval in sv.Values)
                {
                    if ((sval.Value == null) || (sval.Value.Length == 0))
                        continue;
                    tempTP.SetText(sval.Value.Replace("\\n", "\r\n"),false, null);
                    tempTP.Check(Context.Resources, Context.Prj, Project.GetVariableName(sv.VariableName, sv.Array1, sv.Array2), sval.Language.ToString());
                }
            }
            if (Context.Prj.EC.HasErrors())
                Context.Prj.ShowErrors();
            else
                MessageBox.Show("No errors found !");
        }

        private void OnChangeGlyphFont(object sender, EventArgs e)
        {
            tp.SetFont(comboGlyphFont.SelectedItem.ToString());
            previewPanel.Panel2.Invalidate();
        }

        private void txtStrings_TextChanged(object sender, EventArgs e)
        {
            if (txtStrings.Focused)
            {
                DataGridViewCell cell = GetSelectedCell();
                if (cell != null)
                {
                    Language l = (Language)cell.Tag;
                    StringValues sv = (StringValues)dgStrings.Rows[cell.RowIndex].Tag;
                    sv.Set(l, txtStrings.Text.Replace("\r\n", "\\n"));
                    tp.SetText(txtStrings.Text,false, null);
                    previewPanel.Panel2.Invalidate();
                    UpdateDataGridRow(cell.RowIndex);
                }
            }
        }

        private void OnStringTableSelectionChanged(object sender, EventArgs e)
        {
            DataGridViewCell cell = GetSelectedCell();
            if (cell == null)
            {
                txtStrings.Enabled = false;
                txtStrings.Text = "";
                txtStrings.BackColor = Color.Gray;
            }
            else
            {
                txtStrings.Enabled = true;
                if (cell.Value == null)
                    txtStrings.Text = "";
                else
                    txtStrings.Text = cell.Value.ToString().Replace("\\n", "\r\n");
                txtStrings.BackColor = Color.White;
                if (cbAutoSelectTemplate.Checked)
                {
                    object template = dgStrings.Rows[dgStrings.SelectedCells[0].RowIndex].Cells[2].Value;
                    int idx = -1;
                    if (template != null)
                        idx = Context.Prj.GetStringTemplatePreview(template.ToString());
                    if (idx >= 0)
                        UsePreviewTemplate(Context.Prj.StringTemplates[idx],false);
                    else
                        ResetPreview();
                }
            }
            tp.SetText(txtStrings.Text,false,null);
            previewPanel.Panel2.Invalidate();

        }

        private void OnDeleteLanguages(object sender, EventArgs e)
        {
            LanguageSelectDialog dlg = new LanguageSelectDialog(null);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<Language> lst = dlg.GetSelectedLanguages();
                if (lst.Count == 0)
                    return;
                if (MessageBox.Show("Delete " + lst.Count.ToString() + " language(s) ?", "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                foreach (StringValues sv in Context.Prj.Strings)
                {
                    foreach (Language l in lst)
                        sv.Set(l, "");
                }
                for (int tr = 0; tr < dgStrings.Rows.Count; tr++)
                    UpdateDataGridRow(tr);
            }
        }

        private void OnDeleteCells(object sender, EventArgs e)
        {
            if (dgStrings.SelectedCells.Count < 1)
            {
                MessageBox.Show("Please select at least one value for deletion !");
                return;
            }

            Dictionary<StringValues, bool> d = new Dictionary<StringValues, bool>();
            Dictionary<StringValues, bool> d_arr = new Dictionary<StringValues, bool>();
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
            {
                StringValues sv = (StringValues)dgStrings.Rows[cell.RowIndex].Tag;
                d[sv] = true;
                for (int tr=0;tr<Context.Prj.Strings.Count;tr++)
                {
                    if (sv == Context.Prj.Strings[tr])
                        continue;
                    if (sv.VariableName.Equals(Context.Prj.Strings[tr].VariableName, StringComparison.InvariantCultureIgnoreCase))
                        d_arr[Context.Prj.Strings[tr]] = true;
                }
            }

            if (MessageBox.Show("Delete " + d.Keys.Count.ToString() + " string values ?", "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            if (d_arr.Keys.Count>0)
            {
                DialogResult res = MessageBox.Show("Some of the selected strings are part of an array. Delete the array as well ?", "Delete", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Cancel)
                    return;
                if (res == DialogResult.Yes)
                {
                    foreach (StringValues sv in d_arr.Keys)
                        d[sv] = true;
                }
            }

            foreach (StringValues sv in d.Keys)
            {
                Context.Prj.Strings.Remove(sv);
            }
            UpdateDataGrid();
            ValidateTemplateValues();
            OnStringTableSelectionChanged(null, null);
        }
        
        #endregion

        public override void OnActivate()
        {
            comboGlyphFont.Items.Clear();
            comboGlyphFont.Items.Add("<None>");
            comboGlyphFont.SelectedIndex = 0;
            if (Context.IDESettings.EnableResourceTab)
            {
                foreach (string s in Context.Resources.Fonts.Keys)
                {
                    comboGlyphFont.Items.Add(s);
                }
            }
            comboImages.Items.Clear();
            comboImages.Items.Add("<None>");
            comboImages.SelectedIndex = 0;
            if (Context.IDESettings.EnableResourceTab)
            {
                foreach (string s in Context.Resources.Images.Keys)
                {
                    comboImages.Items.Add(s);
                }
            }
            img.BitmapName = "";
            img.bmp = null;
            UpdateStringPreviewTemplates();
            // colorez si coloanele
            Dictionary<Language, bool> dB = Context.Prj.GetBuildAvailableLanguages(Context.CurrentBuild, true);
            Dictionary<Language, bool> dp = Context.Prj.GetProjectAvailableLanguages();
            DataGridViewTextBoxColumn dgc;
            for (int tr = NumberOfFixedColumns; tr < dgStrings.Columns.Count; tr++)
            {
                dgc = (DataGridViewTextBoxColumn)dgStrings.Columns[tr];
                Language l = (Language)dgc.Tag;
                if ((dB.ContainsKey(l)) || (dp.ContainsKey(l)==false))
                {
                    dgc.DefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    dgc.DefaultCellStyle.BackColor = Color.FromArgb(255,232,232);
                }
            }
            if (firstEnter)
            {
                OnFilterUsedLanguages(null, null);
                firstEnter = false;
            }
        }
        
        public override void OnOpenNewProject(bool newProject)
        {
            UpdateDataGrid();
            ValidateTemplateValues();
            UpdateStringPreviewTemplates();
        }

        #region Preview

        private void OnDrawText(object sender, PaintEventArgs e)
        {
            if (rectText.IsInitialize() == false)
                rectText.Create(10, 10, previewPanel.Panel2.Width - 10, previewPanel.Panel2.Height - 10);
            if (rectImage.IsInitialize() == false)
                rectImage.Create(10, 10, previewPanel.Panel2.Width - 10, previewPanel.Panel2.Height - 10);

            if (img.SetRect(rectImage, forceNewCoordonated))
                propPreview.Refresh();

            if (img.bmp != null)
                e.Graphics.DrawImage(img.bmp, img.Left, img.Top, img.Right - img.Left, img.Bottom - img.Top);

            gc.SetGraphics(e.Graphics);
            if (forceNewCoordonated)
            {
                forceNewCoordonated = false;
                tp._Left = rectText.Left - 1; // a.i. cand dau SetPosition sa fortez recalcularea
            }
            tp.SetPosition(rectText.Left, rectText.Top, rectText.Right, rectText.Bottom);
            if ((Context != null) && (Context.Resources != null))
                tp.Paint_Old(gc, Context.Resources);
            if ((currectRect != null) && (cbShowSelection.Checked))
                currectRect.Paint(e.Graphics, marginColor);
            propPreview.Refresh();

        }

        private void OnChangeTextPainterProperty(object s, PropertyValueChangedEventArgs e)
        {
            if (currectRect == rectText)
            {
                rectText.Left = tp._Left;
                rectText.Top = tp._Top;
                rectText.Right = tp._Right;
                rectText.Bottom = tp._Bottom;
                forceNewCoordonated = true;
            }
            if (currectRect == rectImage)
            {
                rectImage.Left = img.Left;
                rectImage.Top = img.Top;
                rectImage.Right = img.Right;
                rectImage.Bottom = img.Bottom;
                forceNewCoordonated = true;
            }
            previewPanel.Panel2.Invalidate();

            //previewPanel.Panel2.Refresh();

        }

        private void OnPreviewTextMouseDown(object sender, MouseEventArgs e)
        {
            if ((currectRect!=null) && (cbShowSelection.Checked))
            {
                if (currectRect.OnMouseDown(e.X, e.Y))
                    previewPanel.Panel2.Invalidate();
            }
        }

        private void OnPreviewTextMouseMove(object sender, MouseEventArgs e)
        {
            if ((currectRect != null)&& (cbShowSelection.Checked))
            {
                if (currectRect.OnMouseMove(e.X, e.Y))
                    previewPanel.Panel2.Invalidate();
                else
                    previewPanel.Panel2.Cursor = currectRect.GetCursorForPoz(e.X, e.Y);
            }
        }

        private void OnPreviewTextMouseUp(object sender, MouseEventArgs e)
        {
            if ((currectRect != null)&& (cbShowSelection.Checked))
            {
                if (currectRect.OnMouseUp(e.X, e.Y))
                    previewPanel.Panel2.Invalidate();
            }
        }

        private void OnChangePreviewBackColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                previewPanel.Panel2.BackColor = dlg.Color;
                marginColor = Color.FromArgb(255 - dlg.Color.R, 255 - dlg.Color.G, 255 - dlg.Color.B);
            }
        }

        private void OnMaximizePreviewRect(object sender, EventArgs e)
        {
            if (currectRect != null)
            {
                currectRect.Create(10, 10, previewPanel.Panel2.Width - 10, previewPanel.Panel2.Height - 10);
                previewPanel.Panel2.Invalidate();
            }
        }

        private void OnShowHideSelection(object sender, EventArgs e)
        {
            previewPanel.Panel2.Invalidate();
        }

        private void OnChangeImage(object sender, EventArgs e)
        {
            img.bmp = null;
            img.BitmapName = "";
            if (comboImages.SelectedIndex>0)
            {
                string s = comboImages.Items[comboImages.SelectedIndex].ToString();
                if (Context.Resources.Images.ContainsKey(s))
                {
                    img.bmp = Context.Resources.Images[s].Picture;
                    img.BitmapName = s;                   
                }
            }
            propPreview.Refresh();
            previewPanel.Panel2.Invalidate();
        }

        private void OnViewTextProperties(object sender, EventArgs e)
        {
            cbViewTextProperties.Checked = true;
            cbViewImageProperties.Checked = false;
            propPreview.SelectedObject = tp;
            currectRect = rectText;
            previewPanel.Panel2.Invalidate();
        }

        private void OnViewImageProperties(object sender, EventArgs e)
        {
            cbViewTextProperties.Checked = false;
            cbViewImageProperties.Checked = true;
            propPreview.SelectedObject = img;
            currectRect = rectImage;
            previewPanel.Panel2.Invalidate();
        }

        private void OnSetOriginalSize(object sender, EventArgs e)
        {
            img.SetOriginalSize();
            rectImage.Left = img.Left;
            rectImage.Top = img.Top;
            rectImage.Right = img.Right;
            rectImage.Bottom = img.Bottom;
            previewPanel.Panel2.Invalidate();
            propPreview.Refresh();
        }

        private void OnCenterCurrentRect(object sender, EventArgs e)
        {
            if (currectRect!=null)
            {
                int w = currectRect.Right - currectRect.Left;
                int h = currectRect.Bottom - currectRect.Top;
                currectRect.Left = previewPanel.Panel2.Width / 2 - w / 2;
                currectRect.Top = previewPanel.Panel2.Height / 2 - h / 2;
                currectRect.Right = currectRect.Left + w;
                currectRect.Bottom = currectRect.Top + h;
                previewPanel.Panel2.Invalidate();
                propPreview.Refresh();
            }
        }

        private void OnSelectImage(object sender, EventArgs e)
        {
            ResourceSelectDialog dlg = new ResourceSelectDialog(Context, ResourcesConstantType.Image,true,false);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                for (int tr = 1; tr < comboImages.Items.Count; tr++)
                {
                    if (comboImages.Items[tr].ToString().Equals(dlg.SelectedResource))
                    {
                        comboImages.SelectedIndex = tr;
                        return;
                    }
                }
                comboImages.SelectedIndex = 0;
            }
        }

        public void UpdateStringPreviewTemplates()
        {
            lstStringPreviewTemplates.Items.Clear();
            Context.Prj.StringTemplates.Sort();
            foreach (StringTemplatePreview stp in Context.Prj.StringTemplates)
            {
                ListViewItem lvi = new ListViewItem(stp.Name);
                lvi.Tag = stp;
                lvi.SubItems.Add(stp.FontName);
                lvi.SubItems.Add(stp.BitmapName);
                lvi.SubItems.Add(Color.FromArgb(stp.BackgroundColor).ToString());
                if (Context.Resources != null)
                {
                    if (Context.Resources.Fonts.ContainsKey(stp.FontName) == false)
                    {
                        lvi.ForeColor = Color.Red;
                        lvi.SubItems[1].Text = "Missing:" + stp.FontName;
                    }
                    if ((stp.BitmapName != null) && (stp.BitmapName.Length > 0) && (Context.Resources.Images.ContainsKey(stp.BitmapName) == false))
                    {
                        lvi.ForeColor = Color.Red;
                        lvi.SubItems[2].Text = "Missing:" + stp.BitmapName;
                    }
                }
                lstStringPreviewTemplates.Items.Add(lvi);
            }
        }

        private void OnAddTextPreviewTemplates(object sender, EventArgs e)
        {
            if (comboGlyphFont.Text=="<None>")
            {
                MessageBox.Show("Please select a font first !");
                return;
            }
            InputBox dlg = new InputBox("Enter template name", "");
            if ((dlg.ShowDialog() == DialogResult.OK) && (dlg.StringResult.Trim().Length>0))
            {
                string name = dlg.StringResult.Trim();
                int index = Context.Prj.GetStringTemplatePreview(name);
                if (index>=0)
                {
                    if (MessageBox.Show("Template '" + name + "' already exists. Replace ?", "Replace", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }
                StringTemplatePreview stp = new StringTemplatePreview();
                stp.FontName = comboGlyphFont.Text;
                stp.Align = tp._textAlign;
                stp.BackgroundColor = previewPanel.Panel2.BackColor.ToArgb();
                stp.BitmapName = img.BitmapName;
                stp.FontSizeValue = tp._FontSizeValue;
                stp.imageBottom = img.Bottom;
                stp.imageLeft = img.Left;
                stp.imageRight = img.Right;
                stp.imageTop = img.Top;
                stp.Justify = tp._Justify;
                stp.LineSpace = tp._LineSpaces;
                stp.MissingCharColor = tp._MissingCharacterColor.ToArgb();
                stp.Name = name;
                stp.SizeMethod = tp._fontSizeMethod;
                stp.textBottom = tp._Bottom;
                stp.textLeft = tp._Left;
                stp.textTop = tp._Top;
                stp.textRight = tp._Right;
                stp.WordWrap = tp._WordWrap;
                if (index >= 0)
                    Context.Prj.StringTemplates.RemoveAt(index);
                Context.Prj.StringTemplates.Add(stp);
                Context.Prj.StringTemplates.Sort();
                UpdateStringPreviewTemplates();
            }
        }

        private void UsePreviewTemplate(StringTemplatePreview stp,bool showErrorMessage)
        {
            // incerc sa setez fontul
            bool found = false;
            for (int tr = 0; tr < comboGlyphFont.Items.Count; tr++)
                if (comboGlyphFont.Items[tr].ToString().ToLower().Equals(stp.FontName.ToLower()))
                {
                    comboGlyphFont.SelectedIndex = tr;
                    found = true;
                    break;
                }

            if (!found)
            {
                if (showErrorMessage)
                    MessageBox.Show("Font '" + stp.FontName + "' does not exits !");
                return;
            }

            found = false;
            for (int tr = 0; tr < comboImages.Items.Count; tr++)
                if (comboImages.Items[tr].ToString().ToLower().Equals(stp.BitmapName.ToLower()))
                {
                    comboImages.SelectedIndex = tr;
                    found = true;
                    break;
                }
            if (!found)
                comboImages.SelectedIndex = 0;


            previewPanel.Panel2.BackColor = Color.FromArgb(stp.BackgroundColor);

            rectImage.Bottom = stp.imageBottom;
            rectImage.Left = stp.imageLeft;
            rectImage.Right = stp.imageRight;
            rectImage.Top = stp.imageTop;
            rectText.Bottom = stp.textBottom;
            rectText.Left = stp.textLeft;
            rectText.Top = stp.textTop;
            rectText.Right = stp.textRight;
            tp.Create(stp);
            
            
            forceNewCoordonated = true;
            previewPanel.Panel2.Invalidate();
        }

        private void ResetPreview()
        {
            comboGlyphFont.SelectedIndex = 0;
            comboImages.SelectedIndex = 0;
            tp._textAlign = Alignament.Center; ;
            previewPanel.Panel2.BackColor = Color.Black;
            rectText.Create(10, 10, previewPanel.Panel2.Width - 10, previewPanel.Panel2.Height - 10);
            rectImage.Create(10, 10, previewPanel.Panel2.Width - 10, previewPanel.Panel2.Height - 10);
            tp._Justify = false;
            tp._MissingCharacterColor = Color.Red;
            tp._fontSizeMethod = TextPainter.FontSizeMethod.Scale;
            tp._FontSizeValue = 1;
            tp._LineSpaces = 0;
            tp._WordWrap = false;
            forceNewCoordonated = true;
            previewPanel.Panel2.Invalidate();
        }

        private void OnUseProfile(object sender, EventArgs e)
        {
            if (lstStringPreviewTemplates.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select only one template to use !");
                return;
            }
            StringTemplatePreview stp = (StringTemplatePreview)lstStringPreviewTemplates.SelectedItems[0].Tag;
            UsePreviewTemplate(stp,true);
        }

        private void OnDeleteTemplates(object sender, EventArgs e)
        {
            if (lstStringPreviewTemplates.SelectedItems.Count==0)
            {
                MessageBox.Show("Please select at least one templete for deletion !");
                return;
            }
            if (MessageBox.Show("Delete " + lstStringPreviewTemplates.SelectedItems.Count.ToString() + " templates ?", "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach (ListViewItem lvi in lstStringPreviewTemplates.SelectedItems)
            {
                Context.Prj.StringTemplates.Remove((StringTemplatePreview)lvi.Tag);
            }
            ValidateTemplateValues();
            UpdateStringPreviewTemplates();
        }

        private void OnAutoSelectTemplate(object sender, EventArgs e)
        {
            if (cbAutoSelectTemplate.Checked)
                OnStringTableSelectionChanged(null, null);
        }

        #endregion



    }
}
