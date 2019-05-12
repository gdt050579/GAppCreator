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
    public partial class DateTimeDialog : Form
    {
        public DateTime CurrentDateTime;
        public DateTimeDialog()
        {
            InitializeComponent();
            DateTime n = DateTime.Now;
            CurrentDateTime = new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            dateTimePicker1.Value = CurrentDateTime;
            dateTimePicker2.Value = CurrentDateTime;
        }

        private void OnOK(object sender, EventArgs e)
        {
            CurrentDateTime = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, dateTimePicker1.Value.Hour, dateTimePicker1.Value.Minute, dateTimePicker1.Value.Second);
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
