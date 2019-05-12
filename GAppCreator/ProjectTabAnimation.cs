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
    public partial class ProjectTabAnimation : BaseProjectContainer
    {
        GListView lstAnimations;
        public ProjectTabAnimation()
        {
            InitializeComponent();

            lstAnimations = new GListView();
            lstAnimations.AddColumn("Name", "propName", 500, GListView.RenderType.ItemRenderer, true, HorizontalAlignment.Left);
            lstAnimations.SetColumnIconPropertyName(0, "propIcon");
            lstAnimations.AddColumn("Design Mode", "propDesignMode", 100, GListView.RenderType.Default, true, HorizontalAlignment.Center);
            lstAnimations.AddColumn("Auto Start", "propAutoStart", 80, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstAnimations.AddColumn("Coordonates", "propCoord", 80, GListView.RenderType.Default, true, HorizontalAlignment.Center);

            lstAnimations.AllColumns[0].GroupKeyGetter = delegate (object x) { return ((AnimO.AnimationObject)x).Name.ToUpperInvariant()[0]; };

            lstAnimations.Dock = DockStyle.Fill;
            lstAnimations.View = View.Details;
            lstAnimations.MultiSelect = false;
            lstAnimations.HideSelection = false;

            lstAnimations.OnFilterObject += LstAnimations_OnFilterObject;
            lstAnimations.OnObjectsSelected += lstAnimations_OnObjectsSelected;
            lstAnimations.OnObjectDoubleClicked += lstAnimations_OnObjectDoubleClicked;

            splitContainer1.Panel2.Controls.Add(lstAnimations);    

        }

        void lstAnimations_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            if (SelectedObject == null)
                return;
            AnimationObjectEditor dlg = new AnimationObjectEditor(this.Context, (AnimO.AnimationObject)SelectedObject);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // am creat un obiect nou / modificat
                propAnimation.SelectedObject = dlg.ResultedAnimationObject;
                lstAnimations.SetObjects(Context.Prj.AnimationObjects);
                lstAnimations.SelectObject(dlg.ResultedAnimationObject);
            }            
        }

        void lstAnimations_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            propAnimation.SelectedObject = lstAnimations.GetCurrentSelectedObject();
        }
        private void propAnimation_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            lstAnimations.RefreshSelectedObjects();
        }
        public override void OnActivate()
        {
            AnimO.GenericElement.CurrentAppResources = this.Context.Prj.GetAppResources();
            StringSelectorEditor.EditForm = new ResourceSelectDialog(this.Context, ResourcesConstantType.String, true, false);
            SoundSelectorEditor.EditForm = new ResourceSelectDialog(this.Context, ResourcesConstantType.Sound, true, false);
            lstAnimations.SetObjects(Context.Prj.AnimationObjects);
            propAnimation.SelectedObject = null;
            propAnimation.SelectedObjects = null;
        }
        private void OnTest(object sender, EventArgs e)
        {
            AnimationObjectEditor dlg = new AnimationObjectEditor(this.Context,new AnimO.AnimationObject());
            dlg.ShowDialog();
        }

        private void OnAddAnimtion(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter the name for a new Animation Object:", "AnimationObject_" + (this.Context.Prj.AnimationObjects.Count + 1).ToString());
            if (ib.ShowDialog() != DialogResult.OK)
                return;
            foreach (AnimO.AnimationObject o in this.Context.Prj.AnimationObjects)
            {
                if (o.Name.Equals(ib.StringResult,StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show("There already is an animation named : " + o.Name);
                    return;
                }
            }
            if (Project.ValidateVariableNameCorectness(ib.StringResult,false)==false)
            {
                MessageBox.Show("Name should only contain letters, numbers and '_' character. The first character of the name must be a letter : 'A' - 'Z' !");
                return;
            }
            AnimO.AnimationObject obj = new AnimO.AnimationObject();
            obj.Name = ib.StringResult;
            Context.Prj.AnimationObjects.Add(obj);
            lstAnimations.SetObjects(Context.Prj.AnimationObjects);
            lstAnimations.SelectObject(obj);
        }

        private void OnDeleteAnimation(object sender, EventArgs e)
        {
            if (lstAnimations.GetCurrentSelectedObjectsListCount() == 0)
            {
                MessageBox.Show("Please select at least one element to delete !");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete " + lstAnimations.GetCurrentSelectedObjectsListCount().ToString() + " animations ?", "Delete", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;

            foreach (object elemn in lstAnimations.GetCurrentSelectedObjectsList())
            {
                Context.Prj.AnimationObjects.Remove((AnimO.AnimationObject)elemn);
            }
            lstAnimations.SetObjects(Context.Prj.AnimationObjects);
            propAnimation.SelectedObject = null;
            propAnimation.SelectedObjects = null;
        }

        private void OnValidateAnimations(object sender, EventArgs e)
        {
            if (Context.Prj.CheckAnimationsIntegrity() == false)
                Context.Prj.ShowErrors();
            else
                MessageBox.Show("All animations are set correctly !");
        }

        private void OnDuplicate(object sender, EventArgs e)
        {
            if (lstAnimations.GetCurrentSelectedObjectsListCount() != 1)
            {
                MessageBox.Show("Please select one element to duplicate !");
                return;
            }
            AnimO.AnimationObject currentAnim = (AnimO.AnimationObject)lstAnimations.GetCurrentSelectedObjectsList()[0];
            InputBox ib = new InputBox("Enter the name for the duplicacted Animation Object:", "Duplicate_From_" + currentAnim.Name+"_"+(this.Context.Prj.AnimationObjects.Count + 1).ToString());
            if (ib.ShowDialog() != DialogResult.OK)
                return;
            foreach (AnimO.AnimationObject o in this.Context.Prj.AnimationObjects)
            {
                if (o.Name.Equals(ib.StringResult, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show("There already is an animation named : " + o.Name);
                    return;
                }
            }
            if (Project.ValidateVariableNameCorectness(ib.StringResult, false) == false)
            {
                MessageBox.Show("Name should only contain letters, numbers and '_' character. The first character of the name must be a letter : 'A' - 'Z' !");
                return;
            }
            // all is good - duplic
            AnimO.AnimationObject newAnim = currentAnim.MakeCopy();
            if (newAnim == null)
                return;
            newAnim.Name = ib.StringResult;
            Context.Prj.AnimationObjects.Add(newAnim);
            lstAnimations.SetObjects(Context.Prj.AnimationObjects);
            lstAnimations.SelectObject(newAnim);
        }

        private bool LstAnimations_OnFilterObject(object obj)
        {
            AnimO.AnimationObject o = (AnimO.AnimationObject)obj;
            if (o == null)
                return false;
            if (txAnimationFilter.Text.Length == 0)
                return true;
            return o.Name.IndexOf(txAnimationFilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
        private void OnAnimationFilterChanged(object sender, EventArgs e)
        {
            lstAnimations.Refilter();
        }
    }
}
