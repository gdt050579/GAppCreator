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
    public partial class ConstantEditDialog : Form
    {
        ProjectContext Context;
        Project prj;
        ConstantValue Const;
        bool HasComboForDefaultValue;

        public StructureField Result = null;
        int MatrixColumnsCount = 1;

        public ConstantEditDialog(ProjectContext pContext, ConstantValue _constant)
        {
            InitializeComponent();
            Context = pContext;
            prj = pContext.Prj;
            Const = _constant;

            // populez combo-ul cu mods
            foreach (string value in Enum.GetNames(typeof(ConstantModeType)))
                if (value != ConstantModeType.DataTypes.ToString())
                    comboTypeMode.Items.Add(value);

            if (Const != null)
            {
                txName.Text = Const.Name;
                txDescription.Text = Const.Description;
                SetComboValue(comboTypeMode, ConstantHelper.GetConstantMode(Const.Type).ToString());
                OnTypeModeChanges(null, null);
                string type = ConstantHelper.GetConstantTypeName(Const.Type);
                if (type != null)
                {
                    SetComboValue(comboType, type);
                    OnTypeChanges(null, null);
                    if (HasComboForDefaultValue)
                        SetComboValue(comboDefaultValue, Const.Value);
                    else
                        txDefaultValue.Text = Const.Value;
                    cbList.Checked = Const.MatrixColumnsCount>0;
                    MatrixColumnsCount = Const.MatrixColumnsCount;
                }

            }
            else
            {
                // setez niste valori default
                SetComboValue(comboTypeMode, ConstantModeType.BasicTypes.ToString());
                OnTypeModeChanges(null, null);
                SetComboValue(comboType, BasicTypesConstantType.Int32.ToString());
                OnTypeChanges(null, null);
                txDefaultValue.Text = "0";
            }
        }      

        private void OnOK(object sender, EventArgs e)
        {
            if (Project.ValidateVariableNameCorectness(txName.Text, false) == false)
            {
                MessageBox.Show("Invalid constant name - it must contains letters, numbers and symbol '_' !");
                txName.Focus();
                return;
            }
            if (comboTypeMode.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a mode for field constant !");
                comboTypeMode.Focus();
                return;
            }
            if (comboType.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a type for this constant !");
                comboType.Focus();
                return;
            }

            ConstantValue Result = new ConstantValue();
            Result.Name = txName.Text;
            Result.Description = txDescription.Text.Trim();
            Result.Type = GetComboValue(comboTypeMode) + "::" + GetComboValue(comboType);
            Result.Value = txDefaultValue.Text;
            Result.MatrixColumnsCount = MatrixColumnsCount;
            if (cbList.Checked == false)
                Result.MatrixColumnsCount = 0;
            if (comboDefaultValue.Visible)
            {
                Result.Value = GetComboValue(comboDefaultValue);
                if (Result.Value == null)
                {
                    MessageBox.Show("Please select a default value for this constant !");
                    comboDefaultValue.Focus();
                    return;
                }
            }
            string error = "";
            if (Result.MatrixColumnsCount > 0)
            {
                Result.Value = ConstantHelper.ValidateBasicTypeArrayValues(Result.Value, ConstantHelper.GetBasicTypesType(Result.Type), Result.MatrixColumnsCount, out error);
            }
            else
            {
                Result.Value = prj.ValidateValueForType(Result.Value, Result.Type, out error, null, null, true);
            }
            if (Result.Value == null)
            {
                MessageBox.Show(error);
                return;
            }

            if (Const != null)
            {
                // verific daca nu exista deja
                if ((Result.Name != Const.Name) && (prj.GetConstant(Result.Name) != null))
                {
                    MessageBox.Show("Constant '" + Result.Name + "' already exists !");
                    txName.Focus();
                    return;
                }
                Const.Name = Result.Name;
                Const.Description = Result.Description;
                Const.Value = Result.Value;
                Const.Type = Result.Type;
                Const.MatrixColumnsCount = Result.MatrixColumnsCount;
            }
            else
            {
                prj.Constants.Add(Result);
            }

            
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private string GetComboValue(ComboBox combo,string errorResult = null)
        {
            if (combo.SelectedItem == null)
                return errorResult;
            return combo.SelectedItem.ToString();
        }
        private void SetComboValue(ComboBox combo,string value)
        {
            for (int tr=0;tr<combo.Items.Count;tr++)
                if (value.Equals(combo.Items[tr].ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    combo.SelectedIndex = tr;
                    return;
                }
        }

        private void OnTypeModeChanges(object sender, EventArgs e)
        {
            comboType.Items.Clear();
            if (comboTypeMode.SelectedIndex<0)
            {
                comboType.Enabled = false;
                return;
            }
            comboType.Enabled = true;

            ConstantModeType mode = ConstantHelper.GetConstantMode(GetComboValue(comboTypeMode)+"::");
            switch (mode)
            {
                case ConstantModeType.BasicTypes:
                    foreach (BasicTypesConstantType ct in Enum.GetValues(typeof(BasicTypesConstantType)))
                        if (ct != BasicTypesConstantType.None)
                            comboType.Items.Add(ct.ToString());
                    break;
                case ConstantModeType.Resources:
                    foreach (ResourcesConstantType ct in Enum.GetValues(typeof(ResourcesConstantType)))
                        if (ct != ResourcesConstantType.None)
                            comboType.Items.Add(ct.ToString());
                    break;
                case ConstantModeType.Enumerations:
                    foreach (Enumeration en in prj.Enums)
                        comboType.Items.Add(en.Name);
                    break;
                case ConstantModeType.DataTypes:
                    foreach (Structure en in prj.Structs)
                        comboType.Items.Add(en.Name);
                    break;
            }
            OnTypeChanges(null,null);
        }

        private void OnTypeChanges(object sender, EventArgs e)
        {
            txDefaultValue.Enabled = false;
            txDefaultValue.Visible = true;
            txDefaultValue.ReadOnly = false;
            btnSetDefaultvalue.Enabled = false;
            btnSetDefaultvalue.Visible = true;
            comboDefaultValue.Enabled = false;
            comboDefaultValue.Visible = false;
            cbList.Enabled = false;
            cbList.Checked = false;
            comboDefaultValue.Items.Clear();
            txDefaultValue.Text = "";
            HasComboForDefaultValue = false;
            if ((comboTypeMode.SelectedIndex < 0) || (comboType.SelectedIndex<0))
                return;
            ConstantModeType mode = ConstantHelper.GetConstantMode(GetComboValue(comboTypeMode,"") + "::");
            switch (mode)
            {
                case ConstantModeType.BasicTypes:
                    BasicTypesConstantType bct = ConstantHelper.GetBasicTypesType(GetComboValue(comboType,""));                    
                    switch (bct)
                    {
                        case BasicTypesConstantType.UInt8:
                        case BasicTypesConstantType.UInt16:
                        case BasicTypesConstantType.UInt32:
                        case BasicTypesConstantType.UInt64:
                        case BasicTypesConstantType.Int8:
                        case BasicTypesConstantType.Int16:
                        case BasicTypesConstantType.Int32:
                        case BasicTypesConstantType.Int64:
                        case BasicTypesConstantType.Float32:
                        case BasicTypesConstantType.Float64:
                            txDefaultValue.Text = "0";
                            txDefaultValue.Enabled = btnSetDefaultvalue.Enabled = true;
                            btnSetDefaultvalue.Tag = bct;
                            cbList.Enabled = true;
                            break;
                        case BasicTypesConstantType.Color:
                            txDefaultValue.Text = "0xFF000000";
                            txDefaultValue.Enabled = btnSetDefaultvalue.Enabled = true;
                            btnSetDefaultvalue.Tag = bct;
                            cbList.Enabled = true;
                            break;
                        case BasicTypesConstantType.String:
                            txDefaultValue.Enabled = btnSetDefaultvalue.Enabled = true;
                            btnSetDefaultvalue.Tag = bct;
                            cbList.Enabled = true;
                            break;
                        case BasicTypesConstantType.Boolean:
                            comboDefaultValue.Visible = true;
                            comboDefaultValue.Enabled = true;
                            txDefaultValue.Visible = btnSetDefaultvalue.Visible = false;
                            comboDefaultValue.Items.Add("true");
                            comboDefaultValue.Items.Add("false");
                            HasComboForDefaultValue = true;
                            break;
                    }
                    break;
                case ConstantModeType.Resources:
                    ResourcesConstantType rct = ConstantHelper.GetResourcesType("Resources::"+GetComboValue(comboType,""));
                    if (rct!= ResourcesConstantType.None)
                    {
                        txDefaultValue.Enabled = btnSetDefaultvalue.Enabled = true;
                        txDefaultValue.Visible = btnSetDefaultvalue.Visible = true;                    
                        btnSetDefaultvalue.Tag = rct;
                    }                    
                    break;
                case ConstantModeType.Enumerations:
                    Enumeration en = prj.GetEnumeration(GetComboValue(comboType, ""));
                    if (en!=null)
                    {
                        btnSetDefaultvalue.Tag = en;
                        if (en.IsBitSet)
                        {
                            txDefaultValue.Enabled = btnSetDefaultvalue.Enabled = true;
                            txDefaultValue.Visible = btnSetDefaultvalue.Visible = true;
                            txDefaultValue.ReadOnly = true;
                            txDefaultValue.Text = "";                            
                        }
                        else
                        {
                            comboDefaultValue.Visible = true;
                            HasComboForDefaultValue = true;
                            txDefaultValue.Visible = btnSetDefaultvalue.Visible = false;
                            foreach (EnumValue a in en.Values)
                                comboDefaultValue.Items.Add(a.Name);
                            comboDefaultValue.Enabled = true;
                        }
                    }
                    else
                    {
                        comboDefaultValue.Visible = true;
                        HasComboForDefaultValue = true;
                        txDefaultValue.Visible = btnSetDefaultvalue.Visible = false;
                    }
                    break;
            }
        }

        private void OnSetDefaultValue(object sender, EventArgs e)
        {
            if (btnSetDefaultvalue.Tag == null)
                return;
            if (btnSetDefaultvalue.Tag.GetType() == typeof(BasicTypesConstantType))
            {
                if (cbList.Checked)
                {
                    BasicTypeConstantArrayEditDialog dlg = new BasicTypeConstantArrayEditDialog(prj,((BasicTypesConstantType)btnSetDefaultvalue.Tag), txDefaultValue.Text, MatrixColumnsCount);
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        txDefaultValue.Text = dlg.ResultValue;
                        MatrixColumnsCount = dlg.ColumnsCount;
                    }
                }
                else {
                    BasicTypeConstantEditDialog dlg = new BasicTypeConstantEditDialog(prj, false, "", ((BasicTypesConstantType)btnSetDefaultvalue.Tag), "", txDefaultValue.Text);
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        txDefaultValue.Text = dlg.FieldValue;
                    }
                }
            }
            if (btnSetDefaultvalue.Tag.GetType() == typeof(ResourcesConstantType))
            {
                ResourceSelectDialog dlg = new ResourceSelectDialog(Context, (ResourcesConstantType)btnSetDefaultvalue.Tag, true,false);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txDefaultValue.Text = dlg.SelectedResource;
                }
            }
            if (btnSetDefaultvalue.Tag.GetType() == typeof(Enumeration))
            {
                EnumSelectValueDialog dlg = new EnumSelectValueDialog(prj, ((Enumeration)btnSetDefaultvalue.Tag).Name, txDefaultValue.Text);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txDefaultValue.Text = dlg.EnumValueResult;
                }
            }
        }
    }
}
