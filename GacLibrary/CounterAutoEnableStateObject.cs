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
    public partial class CounterAutoEnableStateObject : UserControl
    {
        private string error = "";
        public CounterAutoEnableStateObject()
        {
            InitializeComponent();
            foreach (string s in Counter.ConditionsNames)
                comboMethod.Items.Add(s);
            UpdateExistingStatus();
        }

        private void SelectComboValue(string value)
        {
            for (int tr=0;tr<comboValue.Items.Count;tr++)
            {
                if (comboValue.Items[tr].ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    comboValue.SelectedIndex = tr;
                    return;
                }
            }
            comboValue.SelectedIndex = -1;
        }

        public void Create(EnableStateCondition esc)
        {
            btnAddReemove.Text = "-";
            comboMethod.SelectedIndex = esc.conditionID;
            UpdateExistingStatus();

            if (esc.useAND)
                btnAndOr.Text = "AND";
            else
                btnAndOr.Text = "OR";
            int number = 0;
            bool isNumber = int.TryParse(esc.strValue, out number);
            
            switch (esc.conditionID)
            {
                case 1:
                case 2: 
                    if (isNumber)
                        nmValue.Value = number;
                    break;
                case 3:
                    comboValue.Visible = true;
                    comboValue.Items.Clear();
                    if (CounterAuttoEnableStateEditor.prj != null)
                    {
                        Dictionary<string, bool> ads = new Dictionary<string, bool>();
                        foreach (GenericAd ad in CounterAuttoEnableStateEditor.prj.Ads)
                            ads[ad.Name] = true;
                        foreach (string name in ads.Keys)
                            comboValue.Items.Add(name);
                        SelectComboValue(esc.strValue);
                    }
                    break;
            }
            
        }

        public EnableStateCondition GetCondition()
        {
            error = "";
            if ((comboMethod.Visible == false) || (comboMethod.Enabled == false))
                return null;
            if (comboMethod.SelectedIndex < 0)
            {
                error = "Please select a condition !";
                return null;
            }
            EnableStateCondition es = new EnableStateCondition(comboMethod.SelectedIndex, "", btnAndOr.Text == "AND");
            if (comboValue.Visible)
            {
                if (comboValue.SelectedIndex<0)
                {
                    error = "Please select a value for the current item !!!";
                    return null;
                }
                else
                {
                    es.strValue = comboValue.Items[comboValue.SelectedIndex].ToString();
                }
            }
            if (nmValue.Visible)
                es.strValue = nmValue.Value.ToString();
            return es;
        }
        public bool IsEnabled()
        {
            return ((comboMethod.Visible == true) && (comboMethod.Enabled == true));
        }
        public string GetError()
        {
            return error;
        }

        private void UpdateExistingStatus()
        {
            bool enabled = (btnAddReemove.Text == "-");
            comboMethod.Visible = enabled;
            nmValue.Visible = enabled;
            comboValue.Visible = enabled;
            btnAndOr.Visible = enabled;
            if (!enabled)
                comboMethod.SelectedIndex = -1;
            else
                comboMethod_SelectedIndexChanged(null, null);
        }

        private void OnClickBtnAndOr(object sender, EventArgs e)
        {
            if (btnAndOr.Text == "AND")
                btnAndOr.Text = "OR";
            else
                btnAndOr.Text = "AND";
        }

        private void CounterAutoEnableStateObject_Resize(object sender, EventArgs e)
        {

        }

        private void comboMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboValue.Visible = false;
            nmValue.Visible = false;
            switch (comboMethod.SelectedIndex)
            {
                case 0: break; // FB
                case 1:
                case 2: nmValue.Visible = true; nmValue.Minimum = 0; nmValue.Maximum = 10000; break;
                case 3: 
                    comboValue.Visible = true;
                    comboValue.Items.Clear();
                    if (CounterAuttoEnableStateEditor.prj != null)
                    {
                        Dictionary<string, bool> ads = new Dictionary<string, bool>();
                        foreach (GenericAd ad in CounterAuttoEnableStateEditor.prj.Ads)
                            ads[ad.Name] = true;
                        foreach (string name in ads.Keys)
                            comboValue.Items.Add(name);
                    }                    
                    break;
            }
        }

        private void OnAddRemove(object sender, EventArgs e)
        {
            if (btnAddReemove.Text == "+")
                btnAddReemove.Text = "-";
            else
                btnAddReemove.Text = "+";
            UpdateExistingStatus();
        }
    }
}
