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
    public partial class StructureFieldEditDialog : Form
    {
        ProjectContext Context;
        Project prj;
        StructureField field;
        Structure structure;
        bool HasComboForDefaultValue;

        public StructureField Result = null;


        public StructureFieldEditDialog(ProjectContext pContext, Structure _structure, StructureField _field)
        {
            InitializeComponent();
            Context = pContext;
            prj = pContext.Prj;
            field = _field;
            structure = _structure;

            // populez combo-ul cu mods
            foreach (string value in Enum.GetNames(typeof(ConstantModeType)))
                comboTypeMode.Items.Add(value);

            if (field!=null)
            {
                txName.Text = field.Name;
                txDescription.Text = field.Description;
                SetComboValue(comboTypeMode, ConstantHelper.GetConstantMode(field.Type).ToString());
                OnTypeModeChanges(null, null);
                string type = ConstantHelper.GetConstantTypeName(field.Type);
                if (type != null)
                {
                    SetComboValue(comboType, type);
                    OnTypeChanges(null, null);
                    if (HasComboForDefaultValue)
                        SetComboValue(comboDefaultValue, field.DefaultValue);
                    else
                        txDefaultValue.Text = field.DefaultValue;
                }
                cbCanBeNull.Checked = field.CanBeNull;
                cbIsList.Checked = field.List;
            }
            else
            {
                OnTypeModeChanges(null, null);
                OnTypeChanges(null, null);
            }
        }      

        private void OnOK(object sender, EventArgs e)
        {
            if (Project.ValidateVariableNameCorectness(txName.Text, false) == false)
            {
                MessageBox.Show("Invalid field name - it must contains letters, numbers and symbol '_' !");
                txName.Focus();
                return;
            }
            if (comboTypeMode.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a mode for field type !");
                comboTypeMode.Focus();
                return;
            }
            if (comboType.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a type for this field !");
                comboType.Focus();
                return;
            }

            Result = new StructureField();
            Result.Name = txName.Text.Trim();
            Result.Description = txDescription.Text.Trim();
            Result.Type = GetComboValue(comboTypeMode) + "::" + GetComboValue(comboType);
            Result.DefaultValue = txDefaultValue.Text;
            if (comboDefaultValue.Visible)
            {
                Result.DefaultValue = GetComboValue(comboDefaultValue);
                if (Result.DefaultValue == null)
                {
                    MessageBox.Show("Please select a default value for this field !");
                    comboDefaultValue.Focus();
                    return;
                }
            }
            if (cbCanBeNull.Enabled)
                Result.CanBeNull = cbCanBeNull.Checked;
            Result.List = cbIsList.Checked;

            string error = "";
            Result.DefaultValue = prj.ValidateValueForType(Result.DefaultValue, Result.Type, out error, null, null, Result.CanBeNull);
            if (Result.DefaultValue == null)
            {
                MessageBox.Show(error);
                return;
            }


            if ((field != null) && (structure != null))
            {
                // verific daca nu exista deja
                StructureField sf = structure.FindField(Result.Name);
                if ((Result.Name != field.Name) && (sf != null) && (sf!=field))
                {
                    MessageBox.Show("Field '" + Result.Name + "' already exists in structure '" + structure.Name + "' !");
                    txName.Focus();
                    return;
                }
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
            cbCanBeNull.Enabled = false;
            cbIsList.Enabled = true;
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
                    cbCanBeNull.Checked = false;
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
                            break;
                        case BasicTypesConstantType.Color:
                            txDefaultValue.Text = "0xFF000000";
                            txDefaultValue.Enabled = btnSetDefaultvalue.Enabled = true;
                            btnSetDefaultvalue.Tag = bct;
                            break;
                        case BasicTypesConstantType.String:
                            txDefaultValue.Enabled = btnSetDefaultvalue.Enabled = true;
                            btnSetDefaultvalue.Tag = bct;
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
                    cbCanBeNull.Enabled = true;
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
                    cbCanBeNull.Checked = false;
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
                case ConstantModeType.DataTypes:
                    cbCanBeNull.Enabled = true;
                    break;
            }
        }

        private void OnSetDefaultValue(object sender, EventArgs e)
        {
            if (btnSetDefaultvalue.Tag == null)
                return;
            if (btnSetDefaultvalue.Tag.GetType() == typeof(BasicTypesConstantType))
            {
                BasicTypeConstantEditDialog dlg = new BasicTypeConstantEditDialog(prj, false, "", ((BasicTypesConstantType)btnSetDefaultvalue.Tag), "", txDefaultValue.Text);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txDefaultValue.Text = dlg.FieldValue;
                    //if (((BasicTypesConstantType)btnSetDefaultvalue.Tag) == BasicTypesConstantType.String)
                    //    txDefaultValue.Text = dlg.FieldValue.Substring(1, dlg.FieldValue.Length - 2);
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
