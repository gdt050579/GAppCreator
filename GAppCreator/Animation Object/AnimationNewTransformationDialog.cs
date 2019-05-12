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
    public partial class AnimationNewTransformationDialog : Form
    {
        AnimO.AnimationObject animObject;
        ProjectContext pContext;
        GListView lstTransformations, lstElements;
        public AnimO.GenericElement SelectedElement = null;
        public Type SelectedTransformationnType = null;

        private static string filterText = "";
        private static bool showNonAplicatble = true;

        public AnimationNewTransformationDialog(ProjectContext _pContext, AnimO.AnimationObject _obj, ImageList imgTransformations, object currentElement)
        {
            animObject = _obj;
            pContext = _pContext;
            InitializeComponent();            

            lstElements = new GListView();
            lstElements.AddColumn("Name", "propName", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstElements.SetColumnIconPropertyName(0, "propIcon");
            lstElements.AddColumn("Description", "propDescription", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstElements.AllColumns[0].GroupKeyGetter = delegate(object x) { return ((AnimO.GenericElement)x).GetTypeName(); };
            lstElements.Dock = DockStyle.Fill;
            lstElements.View = View.Tile;
            lstElements.SmallImageList = pContext.SmallIcons;
            lstElements.LargeImageList = pContext.LargeIcons;
            lstElements.UseColumnForTileView(0);
            lstElements.UseColumnForTileView(1);
            lstElements.TileSize = new System.Drawing.Size(200, 70);
            lstElements.HideSelection = false;
            lstElements.MultiSelect = false;
            lstElements.SetObjects(animObject.Elements);
            lstElements.SelectObjectFromList(currentElement);
            lstElements.OnObjectsSelected += lstElements_OnObjectsSelected;

            this.splitContainer1.Panel2.Controls.Add(lstElements);

            lstTransformations = new GListView();
            lstTransformations.AddColumn("Name", "propName", 600, GListView.RenderType.ItemRenderer, true, HorizontalAlignment.Left);
            lstTransformations.SetColumnIconPropertyName(0, "propIcon");

            lstTransformations.Dock = DockStyle.Fill;
            lstTransformations.View = View.Details;
            lstTransformations.SmallImageList = imgTransformations;
            lstTransformations.LargeImageList = imgTransformations;
            lstTransformations.ShowGroups = false;
            lstTransformations.TileSize = new System.Drawing.Size(400, 32);
            lstTransformations.HideSelection = false;
            lstTransformations.MultiSelect = false;
            lstTransformations.SetObjects(AnimO.Factory.GetTransformationList());
            lstTransformations.DisabledItemStyle = new BrightIdeasSoftware.SimpleItemStyle();
            lstTransformations.DisabledItemStyle.ForeColor = Color.Gray;
            this.splitContainer1.Panel1.Controls.Add(lstTransformations);

            lstTransformations.OnFilterObject += lstTransformations_OnFilterObject;
            lstTransformations.OnObjectsSelected += lstTransformations_OnObjectsSelected;

            txFilter.Text = filterText;
            cbShowNonAplicable.Checked = showNonAplicatble;
            OnRefilter(null, null);
        }

        void lstTransformations_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            if (selected)
            {
                lbTransformation.Text = ((AnimO.TransformationDescriptor)lstTransformations.GetCurrentSelectedObject()).Name;
            }
            else
            {
                lbTransformation.Text = "";
            }
        }

        void lstElements_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            if (selected)
            {
                SelectedElement = (AnimO.GenericElement)lstElements.GetCurrentSelectedObject();
                lbElement.Text = SelectedElement.Name + " (" + SelectedElement.propDescription + ")";
            }
            else
            {
                SelectedElement = null;
                lbElement.Text = "";
            }
            
            lstTransformations.Refilter();
        }

        bool lstTransformations_OnFilterObject(object obj)
        {
            AnimO.TransformationDescriptor td = (AnimO.TransformationDescriptor)obj;
            if (SelectedElement == null)
            {
                lstTransformations.DisableObject(obj);
                if (cbShowNonAplicable.Checked == false)
                    return false;
            }
            else
            {
                if (AnimO.Factory.IsTransformationCompatible(td.type, SelectedElement))
                    lstTransformations.EnableObject(obj);
                else
                {
                    lstTransformations.DisableObject(obj);
                    if (cbShowNonAplicable.Checked == false)
                        return false;
                }
            }
            if (txFilter.TextLength == 0)
                return true;
            if (td.Name.IndexOf(txFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;
            if (td.Description.IndexOf(txFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;            
            return false;
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (SelectedElement == null)
            {
                MessageBox.Show("Please select an element to apply a transformation on !");
                return;
            }
            AnimO.TransformationDescriptor tr = (AnimO.TransformationDescriptor)lstTransformations.GetCurrentSelectedObject();
            if (tr==null)
            {
                MessageBox.Show("Please select an element to apply a transformation on !");
                return;
            }
            if (AnimO.Factory.IsTransformationCompatible(tr.type,SelectedElement)==false)
            {
                MessageBox.Show("Transformation '" + tr.Name + "' can not be applied on object '" + SelectedElement.Name + "' !");
                return;
            }
            SelectedTransformationnType = tr.type;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            filterText = txFilter.Text;
            showNonAplicatble = cbShowNonAplicable.Checked;
            this.Close();
        }

        private void OnRefilter(object sender, EventArgs e)
        {
            lstTransformations.Refilter();
        }

        private void OnShowNonAplicable(object sender, EventArgs e)
        {
            lstTransformations.Refilter();
        }
    }
}
