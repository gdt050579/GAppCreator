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
    public partial class EnumSelectValueDialog : Form, ITerminateEdit
    {
        private EnumSelectValueControl esc;
        public string EnumValueResult;
        public EnumSelectValueDialog(Project prj, string enumName,string enumValue)
        {
            InitializeComponent();
            EnumSelectValueControl.InitControl(prj);
            esc = new EnumSelectValueControl();
            esc.Dock = DockStyle.Fill;
            esc.Init(new KeyValuePair<string,string>(enumName,enumValue), this);
            pnl.Controls.Add(esc);
        }

        private void OnOK(object sender, EventArgs e)
        {
            object o = esc.GetResultedValue();
            if (o!=null)
                EnumValueResult = ((KeyValuePair<string, string>)o).Value;
            else
                EnumValueResult = null;
            if (EnumValueResult==null)
            {
                MessageBox.Show("Please select at least a value for current enumeration !");
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        public void FinishEdit()
        {
            OnOK(null, null);
        }
    }
}
