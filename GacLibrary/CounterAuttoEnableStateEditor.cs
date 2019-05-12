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
    public partial class CounterAuttoEnableStateEditor : Form
    {
        public string ResultVersion = "";
        public static Project prj = null;

        private CounterAutoEnableStateObject []condObj;

        public CounterAuttoEnableStateEditor(string value)
        {
            InitializeComponent();
            condObj = new CounterAutoEnableStateObject[5];
            for (int tr=0;tr<condObj.Length;tr++)
            {
                condObj[condObj.Length-(tr+1)] = new CounterAutoEnableStateObject();
                condObj[condObj.Length - (tr + 1)].Dock = DockStyle.Top;
                this.Controls.Add(condObj[condObj.Length - (tr + 1)]);
            }
            List<EnableStateCondition> lst = Counter.StringRepresentationToConditionList(value);
            for (int tr = 0; (tr < lst.Count) && (tr < condObj.Length); tr++)
                condObj[tr].Create(lst[tr]);
        }

        private void OnOK(object sender, EventArgs e)
        {
            List<EnableStateCondition> lst = new List<EnableStateCondition>();
            for (int tr=0;tr<condObj.Length;tr++)
            {
                if (condObj[tr].IsEnabled() == false)
                    continue;
                EnableStateCondition esc = condObj[tr].GetCondition();
                if (esc == null)
                {
                    MessageBox.Show("Error: " + condObj[tr].GetError());
                    condObj[tr].Focus();
                    return;
                }
                lst.Add(esc);
            }
            string result = Counter.ConditionListToStringRepresentation(lst);
            if (result == null)
            {
                MessageBox.Show("Failed to convert list object to string format !");
                return;
            }
            ResultVersion = result;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
