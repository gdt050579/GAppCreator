using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace GAppCreator
{
    public partial class ProjectTabConstants : BaseProjectContainer
    {
        GListView lstEnums, lstEnumValues,lstDataTypes,lstDataTypeFields,lstConstants;
        DataTypeValuesEditControl db;
        public ProjectTabConstants()
        {
            InitializeComponent();
            InitEnumComponents();
            InitDataTypesComponents();
            InitConstantsComponent();
            OnChangeView(cbConstants, null);
        }


        private void OnChangeView(object sender, EventArgs e)
        {
            bool value;

            // enumerari
            value = sender == cbEnums;
            cbEnums.Checked = value;
            btnAddEnum.Visible = value;
            btnAddEnumValue.Visible = value;
            btnDeleteEnum.Visible = value;
            btnDeleteEnumValue.Visible = value;
            btnEditEnum.Visible = value;
            btnEditEnumValue.Visible = value;
            txEnumValueFilter.Visible = value;
            lbFilterEnumValues.Visible = value;
            sepEnum1.Visible = value;
            sepEnum2.Visible = value;
            pnlEnum.Visible = value;

            // DataTypes
            value = sender == cbDataTypes;
            cbDataTypes.Checked = value;
            pnlDataTypes.Visible = value;
            btnAddDataType.Visible = value;
            btnDeleteDataType.Visible = value;
            btnDeleteDataTypeField.Visible = value;
            btnAddDataTypeField.Visible = value;
            btnEditDataTypeField.Visible = value;
            btnEditDataType.Visible = value;
            sepDataType1.Visible = value;

            // Constants
            value = sender == cbConstants;
            cbConstants.Checked = value;
            btnAddConstant.Visible = value;
            btnEditConstantValue.Visible = value;
            pnlConstants.Visible = value;
            sepConstants1.Visible = value;
            lbFilterConstant.Visible = value;
            txConstantsFilter.Visible = value;
            btnDeleteConstant.Visible = value;
        }

        private void OnCheckIntegrity(object sender, EventArgs e)
        {
            if (Context.Prj != null)
            {
                if (Context.Prj.CheckDataTypesIntegrity()== false)
                    Context.Prj.ShowErrors();
                else
                    MessageBox.Show("All constants/enumerations and data types are set coorectly !");
            }
        }

        public override void OnActivate()
        {
            UpdateEnums();
            UpdateDataTypes();
            UpdateConstants();
        }
        public override void OnOpenNewProject(bool newProject)
        {
            base.OnOpenNewProject(newProject);
        }

        #region Enums
        private void InitEnumComponents()
        {
            lstEnums = new GListView();
            lstEnums.AddColumn("Name", "propName", 260, GListView.RenderType.ItemRenderer, false, HorizontalAlignment.Left);
            lstEnums.AddColumn("Type", "propType", 70, GListView.RenderType.Default, true,  HorizontalAlignment.Left);
            lstEnums.AddColumn("Values", "propValues", 50, GListView.RenderType.Default, false,  HorizontalAlignment.Right);
            lstEnums.AddColumn("BitSet", "propIsBitSet", 50, GListView.RenderType.BooleanCheckBox, false, HorizontalAlignment.Center);            
            lstEnums.MultiSelect = false;
            lstEnums.OnObjectDoubleClicked += lstEnums_OnDoubleClickedEvent;
            lstEnums.OnObjectsSelected += lstEnums_OnObjectsSelected;
            lstEnums.OnShowContextualMenu += lstEnums_OnShowContextualMenu;
            lstEnums.Dock = DockStyle.Fill;


            lstEnumValues = new GListView();
            lstEnumValues.AddColumn("Name", "propName", 400, GListView.RenderType.ItemRenderer, false,  HorizontalAlignment.Left);
            lstEnumValues.AddColumn("Value", "propValue", 600, GListView.RenderType.Default, false,  HorizontalAlignment.Left);
            lstEnumValues.AddColumn("BitSet", "propValueBitSet8", 100, GListView.RenderType.Default, false, HorizontalAlignment.Center);
            lstEnumValues.AddColumn("BitSet", "propValueBitSet16", 120, GListView.RenderType.Default, false, HorizontalAlignment.Center);
            lstEnumValues.AddColumn("BitSet", "propValueBitSet32", 160, GListView.RenderType.Default, false, HorizontalAlignment.Center);
            lstEnumValues.AddColumn("BitSet", "propValueBitSet64", 320, GListView.RenderType.Default, false, HorizontalAlignment.Center);
            lstEnumValues.AddColumn("Hex", "propValueHex", 150, GListView.RenderType.Default, false, HorizontalAlignment.Right);
            lstEnumValues.AddColumn("Value", "propValueNormal", 150, GListView.RenderType.Default, false, HorizontalAlignment.Right);
            lstEnumValues.AddColumn("Value", "propBool", 100, GListView.RenderType.BooleanCheckBox, false, HorizontalAlignment.Center);

            lstEnumValues.OnObjectDoubleClicked += lstEnumValues_OnObjectDoubleClicked;
            lstEnumValues.OnObjectsSelected += lstEnumValues_OnObjectsSelected;
            lstEnumValues.OnFilterObject += lstEnumValues_OnFilterObject;
            lstEnumValues.Dock = DockStyle.Fill;

            HideEnumValueColumns();

            pnlEnum.Panel1.Controls.Add(lstEnums);
            pnlEnum.Panel2.Controls.Add(lstEnumValues);
            pnlEnum.Dock = DockStyle.Fill;
        }

        private void UpdateEnumControlStatus()
        {
            bool value = lstEnums.GetCurrentSelectedObject() != null;
            btnDeleteEnum.Enabled = value;
            btnEditEnum.Enabled = value;
            btnAddEnumValue.Enabled = value;

            value = value & (lstEnumValues.GetCurrentSelectedObjectsListCount() > 0);
            btnDeleteEnumValue.Enabled = value;
            btnEditEnumValue.Enabled = value & (lstEnumValues.GetCurrentSelectedObjectsListCount() == 1);
        }
        private void UpdateEnums()
        {
            lstEnumValues.SetObjects(null);
            lstEnums.SetObjects(Context.Prj.Enums);
            UpdateEnumControlStatus();
        }
        private void OnAddEnum(object sender, EventArgs e)
        {
            EnumEditDialog dlg = new EnumEditDialog(Context.Prj, null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Context.Prj.Enums.Sort();
                UpdateEnums();
            }
        }
        void lstEnums_OnDoubleClickedEvent(object obj, object SelectedObject)
        {
            if (SelectedObject == null)
                return;
            EnumEditDialog dlg = new EnumEditDialog(Context.Prj, (Enumeration)SelectedObject);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Context.Prj.Enums.Sort();
                UpdateEnums();
            }            
        }
        void HideEnumValueColumns()
        {
            for (int tr = 1; tr < lstEnumValues.GetColumnsCount(); tr++)
                lstEnumValues.SetColumnVisible(tr, false);
            lstEnumValues.RebuildColumns();
        }
        void lstEnums_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            HideEnumValueColumns();
            if (!selected)
            {
                lstEnumValues.SetObjects(null);
                UpdateEnumControlStatus();
                return;
            }
            Enumeration e = (Enumeration)SelectedObjects[0];
            if (e.IsBitSet)
            {
                switch (e.Type)
                {
                    case BasicTypesConstantType.Int8:
                    case BasicTypesConstantType.UInt8:
                        lstEnumValues.SetColumnVisible(2, true);
                        break;
                    case BasicTypesConstantType.Int16:
                    case BasicTypesConstantType.UInt16:
                        lstEnumValues.SetColumnVisible(3, true);
                        break;
                    case BasicTypesConstantType.Int32:
                    case BasicTypesConstantType.UInt32:
                        lstEnumValues.SetColumnVisible(4, true);
                        break;
                    case BasicTypesConstantType.Int64:
                    case BasicTypesConstantType.UInt64:
                        lstEnumValues.SetColumnVisible(5, true);
                        break;
                }
            }
            if (ConstantHelper.IsInteger(e.Type))
            {
                lstEnumValues.SetColumnVisible(6, true); // hex
                lstEnumValues.SetColumnVisible(7, true); // hex
            }
            else if (e.Type == BasicTypesConstantType.Boolean)
            {
                lstEnumValues.SetColumnVisible(8, true);
            }
            else
            {
                lstEnumValues.SetColumnVisible(1, true);
            }

            lstEnumValues.SetObjects(e.Values);
            UpdateEnumControlStatus();
            lstEnumValues.RebuildColumns();
        }
        private void OnDeleteEnum(object sender, EventArgs e)
        {
            Enumeration o = lstEnums.GetCurrentSelectedObject() as Enumeration;
            if (o == null)
            {
                MessageBox.Show("Please select an object first !");
                return;
            }
            if (MessageBox.Show("Remove enumeration '"+o.Name+"' ?","Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Context.Prj.Enums.Remove(o);
                UpdateEnums();
            }
        }
        private void OnEditEnumeration(object sender, EventArgs e)
        {
            lstEnums_OnDoubleClickedEvent(null, lstEnums.GetCurrentSelectedObject());
        }
        ContextMenuStrip lstEnums_OnShowContextualMenu(object source, bool selected, System.Collections.IList SelectedObjects)
        {         
            menuEnum.Items[1].Enabled = selected;
            menuEnum.Items[2].Enabled = selected;
            menuEnum.Items[4].Enabled = selected;
            if (selected)
            {
                Enumeration item = SelectedObjects[0] as Enumeration;
                menuEnum.Items[1].Text = "Delete '" + item.Name+"'";
                menuEnum.Items[2].Text = "Edit '" + item.Name+"'";
            }
            else
            {
                menuEnum.Items[1].Text = "Delete";
                menuEnum.Items[2].Text = "Edit";
            }
            return menuEnum;
        }

        private void OnAddEnumValue(object sender, EventArgs e)
        {
            Enumeration o = lstEnums.GetCurrentSelectedObject() as Enumeration;
            string value = "";
            if (ConstantHelper.IsSignedNumber(o.Type))
            {
                Int64 cValue = 0;
                Int64 max = Int64.MinValue;
                foreach (EnumValue ev in o.Values)
                {
                    if (Int64.TryParse(ev.Value, out cValue))
                    {
                        if (max == Int64.MinValue)
                            max = cValue;
                        if (cValue > max)
                            max = cValue;
                    }
                }
                if ((o.Values.Count>0) && (cValue<Int64.MaxValue))
                {
                    value = (cValue + 1).ToString();
                }
            }
            if (ConstantHelper.IsUnsignedNumber(o.Type))
            {
                UInt64 cValue = 0;
                UInt64 max = UInt64.MinValue;
                foreach (EnumValue ev in o.Values)
                {
                    if (UInt64.TryParse(ev.Value, out cValue))
                    {
                        if (max == UInt64.MinValue)
                            max = cValue;
                        if (cValue > max)
                            max = cValue;
                    }
                }
                if ((o.Values.Count > 0) && (cValue < UInt64.MaxValue))
                {
                    value = (cValue + 1).ToString();
                }
            }
            BasicTypeConstantEditDialog dlg = new BasicTypeConstantEditDialog(Context.Prj, true, "", o.Type, "", value);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                EnumValue ev = new EnumValue();
                ev.Name = dlg.FieldName;
                ev.Value = dlg.FieldValue;
                ev.Description = dlg.FieldDescription;
                o.Values.Add(ev);
                lstEnumValues.SetObjects(o.Values);
                UpdateEnumControlStatus();
                lstEnums.RefreshObject(o);
            }
        }
        private void OnDeleteEnumValues(object sender, EventArgs e)
        {
            if (lstEnumValues.GetCurrentSelectedObjectsListCount()<0)
            {
                MessageBox.Show("Please select at least one value for deletion !");
                return;
            }
            if (MessageBox.Show("Delete "+lstEnumValues.GetCurrentSelectedObjectsListCount().ToString()+" values ?","Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Enumeration o = lstEnums.GetCurrentSelectedObject() as Enumeration;
                foreach (object obj in lstEnumValues.GetCurrentSelectedObjectsList())
                    o.Values.Remove((EnumValue)obj);
                lstEnumValues.SetObjects(o.Values);
                UpdateEnumControlStatus();
                lstEnums.RefreshObject(o);
            }
        }
        void lstEnumValues_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            UpdateEnumControlStatus();
        }
        void lstEnumValues_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            if (SelectedObject == null)
            {
                MessageBox.Show("Please select a value for edit !");
                return;
            }
            EnumValue ev = SelectedObject as EnumValue;
            Enumeration e = lstEnums.GetCurrentSelectedObject() as Enumeration;
            BasicTypeConstantEditDialog dlg = new BasicTypeConstantEditDialog(Context.Prj, true, ev.Name, e.Type, ev.Description, ev.Value);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                EnumValue newEV = e.FindValue(dlg.FieldName);
                if ((newEV!=null) && (ev!=newEV))
                {
                    MessageBox.Show("There is already a value with the name '" + dlg.FieldName + "'. Delete that value first and then you can rename the current one !");
                    return;
                }
                ev.Name = dlg.FieldName;
                ev.Value = dlg.FieldValue;
                ev.Description = dlg.FieldDescription;
                ev.Value = dlg.FieldValue;
                lstEnumValues.RefreshObject(ev);
                UpdateEnumControlStatus();
            }          
        }
        private void OnEditEnumValue(object sender, EventArgs e)
        {
            lstEnumValues_OnObjectDoubleClicked(null, lstEnumValues.GetCurrentSelectedObject());
        }
        bool lstEnumValues_OnFilterObject(object obj)
        {
            EnumValue ev = obj as EnumValue;
            if ((ev == null) || (txEnumValueFilter.Text.Length==0))
                return true;
            if (ev.Name.IndexOf(txEnumValueFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;
            if (ev.Description.IndexOf(txEnumValueFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;
            return false;
        }
        private void OnChangeEnumValueFilter(object sender, EventArgs e)
        {
            lstEnumValues.Refilter();
        }
        #endregion

        #region DataTypes
        private void InitDataTypesComponents()
        {
            lstDataTypes = new GListView();
            lstDataTypes.AddColumn("Name", "propName", 310, GListView.RenderType.ItemRenderer, false, HorizontalAlignment.Left);
            lstDataTypes.AddColumn("Fields", "propFields", 70, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstDataTypes.AddColumn("Values", "propValues", 70, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstDataTypes.MultiSelect = false;
            lstDataTypes.OnObjectDoubleClicked += lstDataTypes_OnObjectDoubleClicked;
            lstDataTypes.OnShowContextualMenu += lstDataTypes_OnShowContextualMenu;
            lstDataTypes.OnObjectsSelected += lstDataTypes_OnObjectsSelected;
            lstDataTypes.Dock = DockStyle.Fill;


            lstDataTypeFields = new GListView();
            lstDataTypeFields.AddColumn("Name", "propName", 500, GListView.RenderType.ItemRenderer, false, HorizontalAlignment.Left);
            lstDataTypeFields.AddColumn("Type", "propType", 150, GListView.RenderType.Default, false, HorizontalAlignment.Left);
            lstDataTypeFields.AddColumn("List", "propIsList", 60, GListView.RenderType.BooleanCheckBox, false, HorizontalAlignment.Center);
            lstDataTypeFields.AddColumn("CanBeNull", "propCanBeNull", 100, GListView.RenderType.BooleanCheckBox, false, HorizontalAlignment.Center);
            lstDataTypeFields.AddColumn("Default Value", "propDefaultValue", 300, GListView.RenderType.Default, false, HorizontalAlignment.Left);

            lstDataTypeFields.OnObjectsSelected += lstDataTypeFields_OnObjectsSelected;
            lstDataTypeFields.OnObjectDoubleClicked += lstDataTypeFields_OnObjectDoubleClicked;
            lstDataTypeFields.Dock = DockStyle.Fill;

            pnlDataTypes.Panel1.Controls.Add(lstDataTypes);
            pnlSplitDataTypeDatas.Panel1.Controls.Add(lstDataTypeFields);

            db = new DataTypeValuesEditControl();
            db.Dock = DockStyle.Fill;
            pnlSplitDataTypeDatas.Panel2.Controls.Add(db);


            pnlDataTypes.Dock = DockStyle.Fill;
        }

        void lstDataTypeFields_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            if (SelectedObject == null)
            {
                MessageBox.Show("Please select a field for edit !");
                return;
            }
            StructureField sfield = SelectedObject as StructureField;
            Structure cStruct = lstDataTypes.GetCurrentSelectedObject() as Structure;
            StructureFieldEditDialog dlg = new StructureFieldEditDialog(Context,cStruct,sfield); 
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (sfield.List != dlg.Result.List)
                {
                    if (MessageBox.Show("Field '" + sfield.Name + "' has a different List property. This will reset all of the existing values for this field to their default value.\nAre you sure ?", "Reset", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    cStruct.ClearField(sfield.Name, dlg.Result.DefaultValue);
                }
                else
                {
                    if (sfield.Type != dlg.Result.Type)
                    {
                        DataTypeFieldChangeValueTypeRefactoringDialog dlgRefactoring = new DataTypeFieldChangeValueTypeRefactoringDialog(Context, cStruct, sfield, dlg.Result);
                        if (dlgRefactoring.ShowDialog() != DialogResult.OK)
                            return;
                    }
                }
                sfield.Name = dlg.Result.Name;
                sfield.Description = dlg.Result.Description;
                sfield.DefaultValue = dlg.Result.DefaultValue;
                sfield.CanBeNull = dlg.Result.CanBeNull;
                sfield.List = dlg.Result.List;
                sfield.Type = dlg.Result.Type;

                lstDataTypeFields.SetObjects(cStruct.Fields);
                UpdateDataTypesControlStatus();
                lstDataTypes.RefreshObject(cStruct);
                db.RefreshStructure();
            }           
        }

        void lstDataTypeFields_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            UpdateDataTypesControlStatus();
        }

        void lstDataTypes_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            if (!selected)
            {
                lstDataTypeFields.SetObjects(null);
                UpdateDataTypesControlStatus();
                db.ClearStructureData();
                return;
            }
            lstDataTypeFields.SetObjects(((Structure)SelectedObjects[0]).Fields);
            db.SetStructure(Context, (Structure)SelectedObjects[0]);
            UpdateDataTypesControlStatus();           
        }

        ContextMenuStrip lstDataTypes_OnShowContextualMenu(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            menuDataType.Items[1].Enabled = selected;
            menuDataType.Items[2].Enabled = selected;
            menuDataType.Items[4].Enabled = selected;
            if (selected)
            {
                Structure item = SelectedObjects[0] as Structure;
                menuDataType.Items[1].Text = "Delete '" + item.Name + "'";
                menuDataType.Items[2].Text = "Edit '" + item.Name + "'";
            }
            else
            {
                menuDataType.Items[1].Text = "Delete";
                menuDataType.Items[2].Text = "Edit";
            }
            return menuDataType;            
        }

        void lstDataTypes_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            if (SelectedObject == null)
                return;
            DataStructureEditDialog dlg = new DataStructureEditDialog(Context.Prj, (Structure)SelectedObject);
            if (dlg.ShowDialog() == DialogResult.OK)
                UpdateDataTypes();
        }
        private void UpdateDataTypesControlStatus()
        {
            bool value = lstDataTypes.GetCurrentSelectedObject() != null;
            btnDeleteDataType.Enabled = value;            
            btnEditDataType.Enabled = value;
            btnAddDataTypeField.Enabled = value;

            value = value & (lstDataTypeFields.GetCurrentSelectedObjectsListCount() > 0);
            btnDeleteDataTypeField.Enabled = value;
            btnEditDataTypeField.Enabled = value & (lstDataTypeFields.GetCurrentSelectedObjectsListCount() == 1);
        }
        private void UpdateDataTypes()
        {
            lstDataTypeFields.SetObjects(null);
            lstDataTypes.SetObjects(Context.Prj.Structs);
            UpdateDataTypesControlStatus();
            db.ClearStructureData();
        }
        private void OnAddNewDataType(object sender, EventArgs e)
        {
            DataStructureEditDialog dlg = new DataStructureEditDialog(Context.Prj,null);
            if (dlg.ShowDialog() == DialogResult.OK)
                UpdateDataTypes();  
        }
        private void OnDeleteDataType(object sender, EventArgs e)
        {
            Structure o = lstDataTypes.GetCurrentSelectedObject() as Structure;
            if (o == null)
            {
                MessageBox.Show("Please select an data type first !");
                return;
            }
            if (MessageBox.Show("Remove data type '" + o.Name + "' ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Context.Prj.Structs.Remove(o);
                UpdateDataTypes();
            }
        }
        private void OnEditDataType(object sender, EventArgs e)
        {
            lstDataTypes_OnObjectDoubleClicked(null, lstDataTypes.GetCurrentSelectedObject());
        }
        private void OnAddDataTypeField(object sender, EventArgs e)
        {
            Structure o = lstDataTypes.GetCurrentSelectedObject() as Structure;
            StructureFieldEditDialog dlg = new StructureFieldEditDialog(Context,o,null); //Context.Prj, o, null,null,Context);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                o.AddField(dlg.Result);                
                lstDataTypeFields.SetObjects(o.Fields);
                UpdateDataTypesControlStatus();
                lstDataTypes.RefreshObject(o);
                db.RefreshStructure();
            }
        }
        private void OnDeleteDataTypeFields(object sender, EventArgs e)
        {
            if (lstDataTypeFields.GetCurrentSelectedObjectsListCount() < 0)
            {
                MessageBox.Show("Please select at least one field for deletion !");
                return;
            }
            if (MessageBox.Show("Delete " + lstDataTypeFields.GetCurrentSelectedObjectsListCount().ToString() + " fields ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Structure o = lstDataTypes.GetCurrentSelectedObject() as Structure;
                foreach (object obj in lstDataTypeFields.GetCurrentSelectedObjectsList())
                    o.RemoveField((StructureField)obj);                
                lstDataTypeFields.SetObjects(o.Fields);
                UpdateDataTypesControlStatus();
                lstDataTypes.RefreshObject(o);
                db.RefreshStructure();
            }
        }
        private void OnEditDataTypeField(object sender, EventArgs e)
        {
            lstDataTypeFields_OnObjectDoubleClicked(null, lstDataTypeFields.GetCurrentSelectedObject());
        }
        #endregion

        #region Constants
        private void InitConstantsComponent()
        {
            lstConstants = new GListView();
            lstConstants.AddColumn("Name", "propName", 500, GListView.RenderType.ItemRenderer, false, HorizontalAlignment.Left);
            lstConstants.AddColumn("Mode", "propMode", 100, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstConstants.AddColumn("Type", "propType", 150, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstConstants.AddColumn("Value", "propValue", 400, GListView.RenderType.Default, false, HorizontalAlignment.Left);
            lstConstants.OnObjectDoubleClicked += lstConstants_OnObjectDoubleClicked;
            lstConstants.OnFilterObject += lstConstants_OnFilterObject;
            lstConstants.OnShowContextualMenu += lstConstants_OnShowContextualMenu;
            lstConstants.Dock = DockStyle.Fill;

            pnlConstants.Controls.Add(lstConstants);
            pnlConstants.Dock = DockStyle.Fill;
            lstConstants.Dock = DockStyle.Fill;
        }

        ContextMenuStrip lstConstants_OnShowContextualMenu(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            menuConstans.Items[1].Enabled = selected;
            menuConstans.Items[2].Enabled = selected;
            if (selected)
            {
                ConstantValue item = SelectedObjects[0] as ConstantValue;
                menuConstans.Items[1].Text = "Delete " + SelectedObjects.Count.ToString() + " constant(s)";
                if (SelectedObjects.Count==1)
                    menuConstans.Items[2].Text = "Edit '" + item.Name + "'";
                else
                {
                    menuConstans.Items[2].Text = "Edit";
                    menuConstans.Items[2].Enabled = false;
                }
            }
            else
            {
                menuConstans.Items[1].Text = "Delete";
                menuConstans.Items[2].Text = "Edit";
            }
            return menuConstans;           
        }

        bool lstConstants_OnFilterObject(object obj)
        {
            ConstantValue cnst = obj as ConstantValue;
            if ((cnst == null) || (txConstantsFilter.Text.Length == 0))
                return true;
            if (cnst.Name.IndexOf(txConstantsFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;
            if (cnst.Description.IndexOf(txConstantsFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;
            return false;
        }

        void lstConstants_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            OnEditConstantValue(null, null);
        }
        void UpdateConstants()
        {
            lstConstants.SetObjects(null);
            lstConstants.SetObjects(Context.Prj.Constants);
        }
        private void OnAddConstant(object sender, EventArgs e)
        {
            ConstantEditDialog dlg = new ConstantEditDialog(Context, null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Context.Prj.Constants.Sort();
                UpdateConstants();
            }
        }
        private void OnEditConstantValue(object sender, EventArgs e)
        {
            if (lstConstants.GetCurrentSelectedObject()==null)
            {
                MessageBox.Show("Please select a constant first !");
                return;
            }
            ConstantValue c = (ConstantValue)lstConstants.GetCurrentSelectedObject();
            ConstantEditDialog dlg = new ConstantEditDialog(Context, c);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                lstConstants.RefreshSelectedObjects();
            }
        }
        private void OnChangeConstantFilter(object sender, EventArgs e)
        {
            lstConstants.Refilter();
        }
        private void OnDeleteConstant(object sender, EventArgs e)
        {
            if (lstConstants.GetCurrentSelectedObjectsListCount() < 0)
            {
                MessageBox.Show("Please select at least one constant for deletion !");
                return;
            }
            if (MessageBox.Show("Delete " + lstConstants.GetCurrentSelectedObjectsListCount().ToString() + " constants ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (object obj in lstConstants.GetCurrentSelectedObjectsList())
                    Context.Prj.Constants.Remove((ConstantValue)obj);
                UpdateConstants();
            }
        }
        #endregion



    }
}
