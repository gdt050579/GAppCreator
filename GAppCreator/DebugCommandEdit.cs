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
    public partial class DebugCommandEdit : Form
    {
        public DebugCommand Command = null;
        public DebugCommandEdit(DebugCommand cmd)
        {
            InitializeComponent();
            if (cmd!=null)
            {
                // populez
                txName.Text = cmd.Name;
                txDesc.Text = cmd.Description;
                // parametri
                foreach (DebugCommandParam param in cmd.Parameters)
                {
                    object []o = new Object[4];
                    string Name = param.Name;
                    string Desc = param.Description;
                    string EnumValue = param.EnumValues;
                    o[0] = Name;
                    o[1] = param.Type.ToString();
                    o[2] = Desc;
                    o[3] = EnumValue;
                    dgParams.Rows.Add(o);
                }
            }
        }

        private string GetCell(int x,int y)
        {
            if ((y < 0) || (y >= dgParams.Rows.Count))
                return "";
            if ((x<0) || (x>dgParams.Columns.Count))
                return "";
            if (dgParams.Rows[y].Cells[x].Value == null)
                return "";
            return dgParams.Rows[y].Cells[x].Value.ToString();
        }
        private void OnOK(object sender, EventArgs e)
        {
            if (Project.ValidateVariableNameCorectness(txName.Text)==false)
            {
                MessageBox.Show("Invalid command name. It should start with a capital letter and can only contain letters, numbers and '_' symbol !");
                txName.Focus();
                return;
            }
            if (txDesc.Text.Trim().Length==0)
            {
                MessageBox.Show("Please provide a description for this command !");
                txDesc.Focus();
                return;
            }
            // iau parametri pe rand
            List<DebugCommandParam> lst = new List<DebugCommandParam>();
            for (int tr=0;tr<dgParams.Rows.Count-1;tr++)
            {
                string paramName = GetCell(0,tr);
                if (Project.ValidateVariableNameCorectness(paramName) == false)
                {
                    MessageBox.Show("Parameter name ('"+paramName+"') is invalid. It should start with a capital letter and can only contain letters, numbers and '_' symbol !");
                    dgParams.Focus();
                    return;
                }
                string paramDesc = GetCell(2, tr);
                if (paramDesc.Trim().Length==0)
                {
                    MessageBox.Show("Please provide a description for parameter '"+paramName+"' !");
                    dgParams.Focus();
                    return;
                }
                DebugCommandParamType type = DebugCommandParamType.None;
                if (Enum.TryParse<DebugCommandParamType>(GetCell(1,tr),out type)==false)
                {
                    MessageBox.Show("Please provide a valid type for parameter '"+paramName+"' !");
                    dgParams.Focus();
                    return;
                }
                if (type == DebugCommandParamType.Enum)
                {
                    Dictionary<string, sbyte> d = new Dictionary<string, sbyte>();
                    string err = "";
                    if (Project.StringEnumListToDict(GetCell(3,tr),d,out err)==false)
                    {
                        MessageBox.Show("Please provide valid enum values for parameter '" + paramName + "'\r\n"+err);
                        dgParams.Focus();
                        return;
                    }
                }
                // all is good - avem un parametru
                DebugCommandParam param = new DebugCommandParam();
                param.Name = paramName;
                param.Description = paramDesc;
                param.Type = type;
                if (type == DebugCommandParamType.Enum)
                    param.EnumValues = GetCell(3,tr);
                lst.Add(param);
            }
            // toti parametri sunt ok
            Command = new DebugCommand();
            Command.Name = txName.Text;
            Command.Description = txDesc.Text;
            Command.Parameters = lst;
            if (Command.GetParametersPackedMemorySize()>8)
            {
                MessageBox.Show("Too many parameters. Total size of the parameters must not exceed 8 bytes !");
                Command = null;
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
