using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public interface ITerminateEdit
    {
        void FinishEdit();
    }
    public interface IDataGridUserControl
    {
        object GetResultedValue();
        string GetStringRepresentation(object o);
        bool HasCustomPaint();
        void DrawCell(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, Font fnt, StringFormat sfmt);
        void Init(object o, ITerminateEdit edit);
    }
    public class CustomColumn<T> : DataGridViewColumn where T : UserControl, IDataGridUserControl, new()
    {
        public object DefaultCellValue = null;
        public CustomColumn(object _defaultValue) : base(new CustomCell<T>()) { DefaultCellValue = _defaultValue; }
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if ((value != null) && (!value.GetType().IsAssignableFrom(typeof(CustomCell<T>))))
                {
                    throw new InvalidCastException("Must be a custom cell");
                }
                base.CellTemplate = value;
            }
        }
    }

    public class CustomCell<T> : DataGridViewTextBoxCell where T : UserControl, IDataGridUserControl, new()
    {
        private static T tempObject = new T();
        private static StringFormat sFormat = new StringFormat();
        

        public CustomCell() : base() { sFormat.LineAlignment = StringAlignment.Center;}

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            CustomCellControl<T> ctl = DataGridView.EditingControl as CustomCellControl<T>;
            if (Value == null)
                Value = DefaultNewRowValue;
            ctl.Init(Value);

        }

        public override Type EditType
        {
            get { return typeof(CustomCellControl<T>); }
        }

        public override Type ValueType
        {
            get { return typeof(object); }
        }

        public override object DefaultNewRowValue
        {
            get 
            {
                if (this.OwningColumn == null)
                    return null;
                return ((CustomColumn<T>)this.OwningColumn).DefaultCellValue; 
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
            formattedValue = null;
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
            {
                if (tempObject.HasCustomPaint())
                    tempObject.DrawCell(graphics, clipBounds, cellBounds, rowIndex, elementState, value, cellStyle.Font, sFormat);
                else
                {
                    graphics.DrawString(tempObject.GetStringRepresentation(value), cellStyle.Font, System.Drawing.Brushes.Black, cellBounds, sFormat);
                }
            }

        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter formattedValueTypeConverter, System.ComponentModel.TypeConverter valueTypeConverter)
        {
            return formattedValue;
        }
    }
    class CustomCellDropDownForm<T> : Form where T : UserControl, IDataGridUserControl, new()
    {
        Panel pnl;
        T uc;
        public CustomCellDropDownForm()
        {
            this.pnl = new System.Windows.Forms.Panel();
            this.uc = new T();
            this.SuspendLayout();
            this.Width = uc.Width;
            this.Height = uc.Height;
            this.pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.MinimumSize = new Size(0, 0);
            this.ControlBox = false;
            this.Controls.Add(this.pnl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(0, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.pnl.Controls.Add(uc);
            this.uc.Dock = DockStyle.Fill;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.Deactivate += CustomCellDropDownForm_Deactivate;
        }
        void CustomCellDropDownForm_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }
        public void Init(object o, ITerminateEdit control)
        {
            uc.Init(o, control);
        }
        public object GetResultedValue()
        {
            return uc.GetResultedValue();
        }
        public string GetStringRepresentation()
        {
            return uc.GetStringRepresentation(uc.GetResultedValue());
        }
    }
    class CustomCellControl<T> : UserControl, IDataGridViewEditingControl, ITerminateEdit where T : UserControl, IDataGridUserControl, new()
    {
        private Button DropDownButton;
        private TextBox Txt;
        private CustomCellDropDownForm<T> frm = null;
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public CustomCellControl()
        {
            Txt = new TextBox();
            Txt.Dock = DockStyle.Fill;
            Txt.ReadOnly = true;
            Txt.TabIndex = 2;
            this.Controls.Add(Txt);
            DropDownButton = new Button();
            DropDownButton.Text = "...";
            DropDownButton.Dock = DockStyle.Right;
            DropDownButton.AutoSize = true;
            DropDownButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            DropDownButton.Click += DropDownButton_Click;
            DropDownButton.TabIndex = 1;
            DropDownButton.Focus();
            this.Controls.Add(DropDownButton);
            frm = new CustomCellDropDownForm<T>();
            frm.Deactivate += frm_Deactivate;
        }

        void frm_Deactivate(object sender, EventArgs e)
        {
            Txt.Text = frm.GetStringRepresentation();
        }
        void DropDownButton_Click(object sender, EventArgs e)
        {
            Rectangle r = this.RectangleToScreen(this.DisplayRectangle);
            frm.Left = r.Left;
            frm.Top = r.Bottom;
            if (frm.Bottom > Screen.PrimaryScreen.Bounds.Bottom*9/10)
            {
                frm.Top = r.Top - frm.Height;
            }
            if (frm.Right > Screen.PrimaryScreen.Bounds.Right)
            {
                frm.Left = Screen.PrimaryScreen.Bounds.Right - frm.Width;
            }
            frm.Show();
        }
        public void Init(object o)
        {
            if (frm != null)
            {
                frm.Init(o, this);
                Txt.Text = frm.GetStringRepresentation();
            }
        }
        public object EditingControlFormattedValue
        {
            get
            {
                return this;
            }
            set
            {
                //this.Color = Color.Pink;
            }
        }
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return frm.GetResultedValue();
        }
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }
        public void PrepareEditingControlForEdit(bool selectAll)
        {

        }
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }
        protected virtual void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            if (this.dataGridView != null)
            {
                this.dataGridView.NotifyCurrentCellDirty(true);
            }
        }
        protected override void OnLeave(EventArgs eventargs)
        {
            base.OnLeave(eventargs);
            NotifyDataGridViewOfValueChange();
        }

        void ITerminateEdit.FinishEdit()
        {
            NotifyDataGridViewOfValueChange();
            frm.Hide();
        }
    }

    public class ListButtonCell : DataGridViewButtonCell
    {
        static Rectangle tmpRect = new Rectangle();
        static SolidBrush tmpBrush = new SolidBrush(Color.Black);
        static StringFormat strFormat = new StringFormat();
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            tmpRect.X = cellBounds.X;
            tmpRect.Y = cellBounds.Y;
            tmpRect.Width = cellBounds.Width - 1;
            tmpRect.Height = cellBounds.Height - 1;
            if ((elementState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                tmpBrush.Color = cellStyle.SelectionBackColor;
            else
                tmpBrush.Color = cellStyle.BackColor;
            graphics.FillRectangle(tmpBrush, tmpRect);

            if (value != null)
            {
                // afisez textul
                if ((value.GetType() == typeof(string)))
                {
                    string s = (string)value;
                    int elems = 0;
                    foreach (char ch in s)
                        if (ch == ';')
                            elems++;
                    if ((elems >= 0) && (s.Length > 0))
                        elems++;

                    if ((elementState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                        tmpBrush.Color = cellStyle.SelectionForeColor;
                    else
                    {
                        if (elems == 0)
                            tmpBrush.Color = Color.FromArgb(128, cellStyle.ForeColor);
                        else
                            tmpBrush.Color = cellStyle.ForeColor;
                    }
                    strFormat.Alignment = StringAlignment.Near;
                    strFormat.LineAlignment = StringAlignment.Center;

                    if (elems == 0)
                        graphics.DrawString("<Empty>", cellStyle.Font, tmpBrush, tmpRect.X + 5, tmpRect.Y + tmpRect.Height / 2, strFormat);
                    else
                    {
                        graphics.DrawString(String.Format("{0} elements", elems), cellStyle.Font, tmpBrush, tmpRect.X + 5, tmpRect.Y + tmpRect.Height / 2, strFormat);
                    }
                }

                int oldW = cellBounds.Width;
                cellBounds.X = cellBounds.Right - cellBounds.Height;
                cellBounds.Width = cellBounds.Height;
                cellBounds.Height--;
                ButtonRenderer.DrawButton(graphics, cellBounds, "...", this.DataGridView.Font, true, System.Windows.Forms.VisualStyles.PushButtonState.Normal);
                cellBounds.X = cellBounds.Right - oldW;
                cellBounds.Width = oldW;
                cellBounds.Height++;
            }
        }
    }
}
