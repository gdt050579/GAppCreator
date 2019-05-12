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
    public partial class DataTypeFieldChangeValueTypeRefactoringDialog : Form
    {
        ProjectContext Context;
        Project prj;
        Structure Struct;
        StructureField Field,NewField;
        ColumnInfo cInfo,cOrigInfo;

        public DataTypeFieldChangeValueTypeRefactoringDialog(ProjectContext pContext,Structure s,StructureField _field,StructureField newField)
        {
            InitializeComponent();
            prj = pContext.Prj;
            Field = _field;
            NewField = newField;
            Context = pContext;
            Struct = s;
            lstValues.Columns[2].Text = "Original:"+Field.Type;
            lstValues.Columns[3].Text = "New:" + NewField.Type;
        
            // datele despre coloana
            cInfo = new ColumnInfo(newField, prj);
            cOrigInfo = new ColumnInfo(Field, prj);

            // populez lista:
            int index = Struct.GetFieldIndex(Field.Name);
            if (index>=0)
            {
                foreach (StructureValue sv in Struct.Values)
                {
                    ListViewItem lvi = new ListViewItem(sv.LinkID.ToString());
                    lvi.SubItems.Add(Project.GetVariableName(sv.Name, sv.Array1, sv.Array2));
                    if (index < sv.FieldValues.Count)
                        lvi.SubItems.Add(sv.FieldValues[index]);
                    else
                        lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.Tag = sv;
                    lstValues.Items.Add(lvi);
                }
            }

            // incerc auto translate
            OnTranslate(null, null);
        }

        

        private void OnOK(object sender, EventArgs e)
        {
            int error_count = 0;
            foreach (ListViewItem lvi in lstValues.Items)
            {
                if (lvi.ForeColor != Color.Black)
                    error_count++;
            }
            if (error_count>0)
            {
                MessageBox.Show("Some items were not corectly converted !");
                lstValues.Focus();
                return;
            }
            // totul e ok - copii valorile
            int index = Struct.GetFieldIndex(Field.Name);
            if (index >= 0)
            {
                for (int tr = 0; tr < Struct.Values.Count;tr++)
                {
                    if (index < Struct.Values[tr].FieldValues.Count)
                    {
                        Struct.Values[tr].FieldValues[index] = lstValues.Items[tr].SubItems[3].Text;
                    }
                }
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnKeepOriginalValues(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstValues.Items)
            {
                lvi.SubItems[3].Text = lvi.SubItems[2].Text;
                lvi.ForeColor = Color.Black;
            }
            lbStatus.Text = "All ok";
            lbStatus.ForeColor = Color.Black;
        }

        string TranslateBetweenBasicTypes(string origValue)
        {
            if (cInfo.BasicType == BasicTypesConstantType.String)
                return origValue;
            if (ConstantHelper.CanConvertBasicTypeTo(cOrigInfo.BasicType, cInfo.BasicType))
                return origValue;
            if ((cOrigInfo.BasicType == BasicTypesConstantType.Boolean) && (ConstantHelper.IsNumber(cInfo.BasicType)))
            {
                if (origValue.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    return "1";
                return "0";
            }
            if ((cInfo.BasicType == BasicTypesConstantType.Boolean) && (ConstantHelper.IsNumber(cOrigInfo.BasicType)))
            {
                if (origValue.Equals("0"))
                    return "false";
                return "true";
            }
            string res = ConstantHelper.ValidateBasicTypeValue(origValue, cInfo.BasicType);
            if ((res != null) && (res.Equals(origValue)))
                return origValue;
            return null;
        }
        string Translate(string origValue)
        {
            if ((cInfo.Mode == ConstantModeType.BasicTypes) && (cOrigInfo.Mode == ConstantModeType.BasicTypes))
                return TranslateBetweenBasicTypes(origValue);
            return null;
        }

        private void OnTranslate(object sender, EventArgs e)
        {
            int error_count = 0;
            foreach (ListViewItem lvi in lstValues.Items)
            {
                string res = Translate(lvi.SubItems[2].Text);
                if (res == null)
                {
                    lvi.SubItems[3].Text = "Unable to translate value to "+cInfo.Type;
                    lvi.ForeColor = Color.Red;
                    error_count++;
                }
                else
                {
                    lvi.SubItems[3].Text = res;
                    lvi.ForeColor = Color.Black;
                }
            }
            if (error_count == 0)
            {
                lbStatus.Text = "All ok";
                lbStatus.ForeColor = Color.Black;
            }
            else
            {
                lbStatus.Text = error_count.ToString() + " values could not be translated !";
                lbStatus.ForeColor = Color.Red;
            }
        }

        private void OnUseDefaultValues(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstValues.Items)
            {
                lvi.SubItems[3].Text = NewField.DefaultValue;
                lvi.ForeColor = Color.Black;
            }
            lbStatus.Text = "All ok";
            lbStatus.ForeColor = Color.Black;
        }

        private void OnDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstValues.SelectedItems.Count!=1)
            {
                MessageBox.Show("Select a value first !");
                return;
            }
            ListViewItem lvi = lstValues.SelectedItems[0];
            string result = null;

            switch (cInfo.Mode)
            {
                case ConstantModeType.BasicTypes:
                    BasicTypeConstantEditDialog dlgBasicType = new BasicTypeConstantEditDialog(prj, false, "", cInfo.BasicType, "", NewField.DefaultValue);
                    if (dlgBasicType.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dlgBasicType.FieldValue;
                        //if (cInfo.BasicType == BasicTypesConstantType.String)
                        //    result = dlgBasicType.FieldValue.Substring(1, dlgBasicType.FieldValue.Length - 2);
                    }
                    break;
                case ConstantModeType.Enumerations:
                    EnumSelectValueDialog dlgEnum = new EnumSelectValueDialog(prj, cInfo.E.Name, NewField.DefaultValue);
                    if (dlgEnum.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dlgEnum.EnumValueResult;
                    }
                    break;
                case ConstantModeType.Resources:
                    ResourceSelectDialog dlg = new ResourceSelectDialog(Context, cInfo.ResType, true, false);
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dlg.SelectedResource;
                    }
                    break;
            }
            if (result == null)
            {
                MessageBox.Show("Don't know how to edit values of type: " + NewField.Type);
            }
            else
            {
                lvi.SubItems[3].Text = result;
                lvi.ForeColor = Color.Black;
            }
        }

        private void OnEditValue(object sender, EventArgs e)
        {
            OnDoubleClick(null, null);
        }
    }
}
