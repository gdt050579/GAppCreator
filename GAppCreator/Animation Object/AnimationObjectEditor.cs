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
    public partial class AnimationObjectEditor : Form, UITypeEditorStringWrapper, UITypeEditorAnimationZOrderWrapper, UITypeEditorButtonStateWrapper, AnimO.ITransformationEvent, AnimO.IRefreshDesign
    {
        ProjectContext pContext;
        public AnimO.AnimationObject ResultedAnimationObject;
        AnimO.AnimationObject originalObject;
        GListView lstElements;
        AnimO.BoardViewMode viewMode;
        AnimO.ElementRectangle elemRect = new AnimO.ElementRectangle();
        Size deviceSize;

        List<AnimO.GenericElement> ZOrder = new List<AnimO.GenericElement>();
        
        

        public AnimationObjectEditor(ProjectContext _pContext, AnimO.AnimationObject _obj)
        {
            pContext = _pContext;
            EventIDSelectorEditor.prj = pContext.Prj;
            //AnimO.GenericElement.CurrentAnimationObject = _obj;
            AnimO.GenericTransformation.Prj = pContext.Prj;
            AnimO.GenericElement.RefreshDesignCallback = this;

            ResultedAnimationObject = _obj.MakeCopy();
            originalObject = _obj;
            AnimationElementEditor.currentAnimationObject = ResultedAnimationObject;
            AnimationElementRelativePositionEditor.currentAnimationObject = ResultedAnimationObject;
            InitializeComponent();
            ShaderSelectorEditor.EditForm = new ResourceSelectDialog(pContext, ResourcesConstantType.Shader, true, false);
            ImageSelectorEditor.EditForm = new ResourceSelectDialog(pContext, ResourcesConstantType.Image, true, false);
            FontSelectorEditor.EditForm = new ResourceSelectDialog(pContext, ResourcesConstantType.Font, true, false);
            ImageListSelectorEditor.editor = this;
            AnimationZOrderEditor.editor = this;
            ButtonStateSelectorEditor.editor = this;
            
            AnimO.GenericTransformation.TransEvent = this;

            deviceSize = Project.SizeToValues(pContext.Prj.DesignResolution);
            if ((_obj.DesignMode == AnimO.AnimationDesignMode.Control) || (_obj.DesignMode == AnimO.AnimationDesignMode.Button))
            {
                deviceSize.Width = _obj.ControlWidth;
                deviceSize.Height = _obj.ControlHeight;
                btnEnableClipping.Enabled = false;
                btnDisableClipping.Enabled = false;
            }
            board.SetDeviceSize(deviceSize.Width, deviceSize.Height);
            board.SetBackgroundColor(_obj._BackgroundColor);
            
            board.OnPaintCanvas += board_OnPaintCanvas;
            board.OnSelectionChanged += board_OnSelectionChanged;
            board.OnSelectionBegins += board_OnSelectionBegins;
            board.OnSelectionEnds += board_OnSelectionEnds;
            board.OnClickOutsideSelection += Board_OnClickOutsideSelection;
            SetDisplayMode(AnimO.BoardViewMode.Design);

            lstElements = new GListView();            
            lstElements.AddColumn("Name", "propName", 320, GListView.RenderType.ItemRenderer, true, HorizontalAlignment.Left);
            lstElements.SetColumnIconPropertyName(0, "propIcon");
            lstElements.AddColumn("Visible", "propVisible", 50, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstElements.AllColumns[0].GroupKeyGetter = delegate(object x) { return ((AnimO.GenericElement)x).GetTypeName(); };
            lstElements.Dock = DockStyle.Fill;
            lstElements.View = View.Details;
            lstElements.SmallImageList = pContext.LargeIcons;
            lstElements.LargeImageList = pContext.LargeIcons;
            //lstElements.UseColumnForTileView(0);
            //lstElements.UseColumnForTileView(1);
            //lstElements.TileSize = new System.Drawing.Size(200, 70);s

            pnlHostElementsList.Controls.Add(lstElements);
            lstElements.SetObjects(ResultedAnimationObject.Elements);
            lstElements.OnObjectsSelected += lstElements_OnObjectsSelected;


            UpdateElementsZOrder(ResultedAnimationObject.ZOrder);

            UpdateTransformationsTree();
            RefreshBoard();

        }



        void SetDisplayMode(AnimO.BoardViewMode mode)
        {
            lbDisplayMode.Text = "Disply: " + mode.ToString();
            viewMode = mode;
        }

        void board_OnSelectionEnds(AnimO.Canvas canvas)
        {
            propElement.Refresh();
            propTransformation.Refresh();
        }

        void board_OnSelectionBegins(AnimO.Canvas canvas)
        {            
        }

        private void Board_OnClickOutsideSelection(AnimO.Canvas canvas, float x_pixels, float y_pixels)
        {
            UpdateElementsRelativePosition();
            var selected_object = propElement.SelectedObject as AnimO.GenericElement;
            AnimO.GenericElement toSelect = null;
            foreach (var elem in ZOrder)
            {
                if ((elem.ExecutionContext.ScreenRect.Contains(x_pixels,y_pixels)) && (elem._ShowInBoardAnimation_))
                {
                    toSelect = elem;
                }
            }
            if (toSelect != null)
            {
                lstElements.SelectObjectFromList(toSelect);
                propElement.SelectedObject = toSelect;
                SelectCurrentElementInBoard();
            }
            else
            {
                lstElements.SelectObjectFromList(null);
                propElement.SelectedObject = null;
                propElement.SelectedObjects = null;
                board.ClearSelection();
            }
        }


        void board_OnSelectionChanged(AnimO.Canvas canvas)
        {
            var selected_object = propElement.SelectedObject as AnimO.GenericElement;
            if (selected_object == null)
                return;
            var r = board.GetSelectionRect();
            if (r == null)
                return;
            //Console.WriteLine(r.X.ToString()+","+r.Y.ToString()+" - "+r.Width.ToString()+" x "+r.Height.ToString());
            UpdateElementsRelativePosition();
            selected_object.UpdateFromScreenRect(r.GetLeft(), r.GetTop(), r.Width, r.Height, deviceSize.Width, deviceSize.Height);
            selected_object.InitRuntimeContext();
            board.Invalidate();
        }

        public void SelectCurrentElementInBoard()
        {
            var selected_object = propElement.SelectedObject as AnimO.GenericElement;
            if (selected_object == null)
                board.ClearSelection();
            else
            {
                UpdateElementsRelativePosition();
                if (selected_object.IsFullScreen())
                    board.ClearSelection();
                else
                {
                    RectangleF rf = selected_object.ExecutionContext.ScreenRect;
                    //board.Create_ObjectToMoveAndResizeSelection(rf.Left, rf.Top, rf.Width, rf.Height, Alignament.TopLeft);
                    board.Create_ObjectToMoveAndResizeSelection(rf, selected_object.ExecutionContext.Align);
                }
            }
        }

        public void RefreshBoard()
        {
            if (viewMode == AnimO.BoardViewMode.Design)
            {
                ResultedAnimationObject.ValidateAnimation(pContext.Prj, pContext.Resources);
                foreach (var elem in ResultedAnimationObject.Elements)
                    elem.InitRuntimeContext();
                SelectCurrentElementInBoard();
                board.Invalidate();
            }
        }

        void lstElements_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            if ((SelectedObjects == null) || (SelectedObjects.Count == 0) || (SelectedObjects.Count > 1))
                propElement.SelectedObject = null;
            else
                propElement.SelectedObject = SelectedObjects[0];
            SelectCurrentElementInBoard();
         }

        private void propElement_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //if (propElementIgnore) { propElementIgnore = false; return; }
            lstElements.RefreshSelectedObjects();
            RefreshBoard();
        }

        private void OnOK(object sender, EventArgs e)
        {
            pContext.Prj.EC.Reset();
            if (ResultedAnimationObject.ValidateAnimation(pContext.Prj,pContext.Prj.GetAppResources())==false)
            {
                pContext.Prj.ShowErrors();
                return;
            }
            for (int tr = 0; tr < pContext.Prj.AnimationObjects.Count;tr++)
            {
                if (pContext.Prj.AnimationObjects[tr] == originalObject)
                {
                    pContext.Prj.AnimationObjects[tr] = ResultedAnimationObject;
                    break;
                }
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnShowObjectEvents(object sender, EventArgs e)
        {
            IDsEditor dlg = new IDsEditor(this.pContext.Prj, false);
            dlg.ShowDialog();
        }

        #region Interfaces
        string UITypeEditorAnimationZOrderWrapper.Edit(string currentValue)
        {
            AnimationObjectZOrderEditor dlg = new AnimationObjectZOrderEditor(currentValue, ResultedAnimationObject);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return dlg.ZOrder;
            return currentValue;
        }
        string UITypeEditorButtonStateWrapper.Edit(string currentValue)
        {
            Object obj = propElement.SelectedObject;
            if (obj==null)
            {
                MessageBox.Show("Please select a simple element button");
                return currentValue;
            }
            if (obj.GetType() != typeof(AnimO.SimpleButtonElement))
            {
                MessageBox.Show("Please select a simple element button");
                return currentValue;
            }
            AnimO.SimpleButtonElement sbe = (AnimO.SimpleButtonElement)obj;
            AnimationObjectButtonStateEditor dlg = new AnimationObjectButtonStateEditor(currentValue, AnimO.GenericElement.CurrentAppResources,sbe.Width,sbe.Height, sbe.BackgroundStyle);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return dlg.Result;
            return currentValue;
        }
        string UITypeEditorStringWrapper.Edit(string currentValue)
        {
            ColumnInfo cInfo = new ColumnInfo("Resources::Image", pContext.Prj);
            cInfo.Mode = ConstantModeType.Resources;
            cInfo.ColumnType = ColumnEditType.List;
            cInfo.ResType = ResourcesConstantType.Image;
            DataTypeListValuesEditDialog dlg = new DataTypeListValuesEditDialog(pContext, cInfo, currentValue);
            
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dlg.ResultList;
            }
            return currentValue;
        }
        void AnimO.ITransformationEvent.OnEvent(AnimO.GenericTransformation trans)
        {
            if (trans.GetType() == typeof(AnimO.ZOrderTransformation))
            {
                UpdateElementsZOrder(((AnimO.ZOrderTransformation)trans).ZOrder);
            }
            if (trans.GetType() == typeof(AnimO.SetNewImageTransformation))
            {
                ((AnimO.SetNewImageTransformation)trans).SetImage(AnimO.GenericElement.CurrentAppResources.Images[((AnimO.SetNewImageTransformation)trans).Image].Picture);
            }
        }
        #endregion

        #region Elements

        private int CountTransformationThatUsesElements(Dictionary<string, bool> names, AnimO.GenericTransformation root)
        {
            int sum = 0;
            if (root == null)
                return 0;
            List<AnimO.GenericTransformation> lst = root.GetBlockTransformations();
            if (lst != null)
            {
                foreach (var t in lst)
                    sum += CountTransformationThatUsesElements(names, t);
            }
            var el = root as AnimO.GenericElementTransformation;
            if (el != null)
            {
                if (names.ContainsKey(el.Element))
                    sum += 1;
            }
            return sum;
        }

        private void DeleteTransformationThatUsesElements(Dictionary<string, bool> names, AnimO.GenericTransformation root)
        {
            if (root == null)
                return;
            List<AnimO.GenericTransformation> lst = root.GetBlockTransformations();
            if (lst != null)
            {
                List<AnimO.GenericTransformation> toDelete = new List<AnimO.GenericTransformation>();
                foreach (var t in lst)
                {
                    var el = t as AnimO.GenericElementTransformation;
                    if (el != null)
                    {
                        if (names.ContainsKey(el.Element))
                            toDelete.Add(el);
                    }
                }
                foreach (var e in toDelete)
                    lst.Remove(e);
                // recursiv merg mai departe
                foreach (var t in lst)
                    DeleteTransformationThatUsesElements(names, t);
            }
        }

        private void UpdateElementsRelativePosition()
        {
            foreach (var elem in ResultedAnimationObject.ParentsPositionOrder)
                elem.ComputeScreenRect(deviceSize.Width, deviceSize.Height);
        }

        private void OnDeleteElements(object sender, EventArgs e)
        {
            if (lstElements.GetCurrentSelectedObjectsListCount() == 0)
            {
                MessageBox.Show("Please select at least one element to delete !");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete " + lstElements.GetCurrentSelectedObjectsListCount().ToString() + " elements ?", "Delete", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;

            Dictionary<string, bool> names = new Dictionary<string, bool>();
            foreach (object elemn in lstElements.GetCurrentSelectedObjectsList())
            {
                names[((AnimO.GenericElement)elemn).Name] = true;
                ResultedAnimationObject.RemoveElement((AnimO.GenericElement)elemn);
            }
            lstElements.SetObjects(ResultedAnimationObject.Elements);
            UpdateElementsZOrder(ResultedAnimationObject.ZOrder);
            // numar cate elemente sunt intr-o transformare
            int usage = CountTransformationThatUsesElements(names, ResultedAnimationObject.Main);
            if (usage <= 0)
                return;

            if (MessageBox.Show("Deleted elements are refered by " + usage.ToString() + " transformations ?", "Delete those transformations as well ?", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;

            // trebuie sa sterg si transformarile
            DeleteTransformationThatUsesElements(names, ResultedAnimationObject.Main);
            UpdateTransformationsTree();
            
        }

        private void AddElement(AnimO.GenericElement elem)
        {
            ResultedAnimationObject.AddElement(elem);
            lstElements.SetObjects(ResultedAnimationObject.Elements);
            lstElements.SelectObjectFromList(elem);
            UpdateElementsZOrder(ResultedAnimationObject.ZOrder);
        }

        private void OnAddImage(object sender, EventArgs e)
        {
            ResourceSelectDialog dlg = new ResourceSelectDialog(pContext, ResourcesConstantType.Image, true, false);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimO.ImageElement img = new AnimO.ImageElement();
                img.Name = "Image_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
                img.Images = dlg.SelectedResource;
                AddElement(img);
            }
        }
        private void OnAddText(object sender, EventArgs e)
        {
            ResourceSelectDialog dlg = new ResourceSelectDialog(pContext, ResourcesConstantType.Font, true, false);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimO.TextElement txt = new AnimO.TextElement();
                txt.Name = "Text_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
                txt.Font = dlg.SelectedResource;
                txt.Text = "a text";
                AddElement(txt);
            }
        }
        private void OnAddSurfaceElement(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimO.EntireSurfaceElement ese = new AnimO.EntireSurfaceElement();
                ese.Name = "Surface_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
                ese.ColorBlending = dlg.Color.ToArgb();
                AddElement(ese);
            }
        }
        private void OnAddSimpleButtonElement(object sender, EventArgs e)
        {
            AnimO.SimpleButtonElement btn = new AnimO.SimpleButtonElement();
            btn.Name = "Button_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
            AddElement(btn);
        }
        private void OnAddRectangle(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimO.RectangleElement rect = new AnimO.RectangleElement();
                rect.Name = "Rectangle_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
                rect.ColorBlending = dlg.Color.ToArgb();
                AddElement(rect);
            }
        }

        private void AdExclusionRect(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimO.ExclusionRectangleElement rect = new AnimO.ExclusionRectangleElement();
                rect.Name = "ExclusionRect_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
                rect.ColorBlending = dlg.Color.ToArgb();
                AddElement(rect);
            }
        }

        private void OnAddClipping(object sender, EventArgs e)
        {
            AnimO.ClipRectangleElement rect = new AnimO.ClipRectangleElement();
            rect.Name = "Clipping_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
            AddElement(rect);
        }
        private void OnAddDisableClip(object sender, EventArgs e)
        {
            AnimO.DisableClippingElement rect = new AnimO.DisableClippingElement();
            rect.Name = "DisableClip_" + (ResultedAnimationObject.Elements.Count() + 1).ToString();
            AddElement(rect);
        }

        private void OnDuplicateElement(object sender, EventArgs e)
        {
            if (lstElements.GetCurrentSelectedObjectsListCount() == 0)
            {
                MessageBox.Show("Please select at least one element to duplicate !");
                return;
            }
            AnimO.GenericElement elem = (AnimO.GenericElement)lstElements.GetCurrentSelectedObjectsList()[0];
            InputBox ib = new InputBox("Enter the name for the duplicacted Element", "Duplicate_From_" + elem.Name + "_" + Environment.TickCount.ToString());
            if (ib.ShowDialog() != DialogResult.OK)
                return;
            if (Project.ValidateVariableNameCorectness(ib.StringResult, false)==false)
            {
                MessageBox.Show("Invalid name - use a-zA-Z0-9 and _ as valid characters !");
                return;
            }
            // verific sa nu am unul deja
            foreach (var el in ResultedAnimationObject.Elements)
            {
                if (el.Name.Equals(ib.StringResult))
                {
                    MessageBox.Show("Element '" + ib.StringResult + "' already exists !");
                    return;
                }
            }
            AnimO.GenericElement newElem = elem.MakeCopy();
            newElem.Name = ib.StringResult;
            AddElement(newElem);
        }

        #endregion

        #region ZOrder

        private void OnSetDefaultZOrder(object sender, EventArgs e)
        {
            AnimationObjectZOrderEditor dlg = new AnimationObjectZOrderEditor(ResultedAnimationObject.ZOrder, ResultedAnimationObject);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ResultedAnimationObject.ZOrder = dlg.ZOrder;
                UpdateElementsZOrder(ResultedAnimationObject.ZOrder);
            }
        }
        private void UpdateElementsZOrder(string zorder)
        {
            List<string> l = Project.StringListToList(zorder);
            Dictionary<string, AnimO.GenericElement> d = new Dictionary<string, AnimO.GenericElement>();            
            foreach (var e in ResultedAnimationObject.Elements)
            {
                e._FoundInZOrder_ = false;
                d[e.Name] = e;
            }
            // parcurg si validez cele care sunt
            Dictionary<string,bool> toIgnore = new Dictionary<string,bool>();
            foreach (string name in l)
            {
                if (d.ContainsKey(name) == false)
                    toIgnore[name] = true;
                else
                    d[name]._FoundInZOrder_ = true;
            }
            // adaug cele care nu sunt
            foreach (string k in d.Keys)
                if (d[k]._FoundInZOrder_ == false)
                    l.Add(k);
            // refac ZOrder-ul
            l.Reverse();
            this.ZOrder.Clear();
            foreach (string k in l)
            {
                if (toIgnore.ContainsKey(k))
                    continue;
                this.ZOrder.Add(d[k]);
            }
            RefreshBoard();
        }

        #endregion

        #region Transformations
        private void UpdateTreeNode(TreeNode node)
        {
            if (node == null)
                return;
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)node.Tag;
            if (trans != null)
            {
                if ((trans.BranchName!=null) && (trans.BranchName.Length>0))
                    node.Text = "["+trans.BranchName+"] - "+ trans.GetName();
                else
                    node.Text = trans.GetName();
                node.ImageKey = trans.GetIconKey();
            }
            else
            {
                node.Text = "???";
                node.ImageKey = "";
            }
            node.SelectedImageKey = node.ImageKey;
            if (trans.Collapsed)
                node.Collapse(true);
            else
                node.Expand();
        }
        private void CreateTreeNode(TreeNode parent, AnimO.GenericTransformation trans)
        {
            TreeNode tn = new TreeNode();
            tn.Tag = trans;
            if (parent == null)
                treeTransformations.Nodes.Add(tn);
            else
                parent.Nodes.Add(tn);

            List<AnimO.GenericTransformation> copii = trans.GetBlockTransformations();
            if (copii != null)
            {
                foreach (AnimO.GenericTransformation c_trans in copii)
                    CreateTreeNode(tn, c_trans);
            }

            UpdateTreeNode(tn);
        }
        private void UpdateTransformationsTree()
        {
            treeTransformations.Nodes.Clear();
            propTransformation.SelectedObject = null;
            if (ResultedAnimationObject.Main != null)
            {
                CreateTreeNode(null, ResultedAnimationObject.Main);
                //treeTransformations.ExpandAll();
            }
        }
        private void treeTransformations_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AnimationElementEditor.currentTransformation = null;
            if (treeTransformations.SelectedNode == null)
            {
                propTransformation.SelectedObject = null;
                return;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (propTransformation.SelectedObject != trans)
            {
                propTransformation.SelectedObject = trans;
                AnimO.GenericElementTransformation etrans = trans as AnimO.GenericElementTransformation;
                AnimationElementEditor.currentTransformation = etrans;
            }
        }
        private void treeTransformations_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)e.Node.Tag;
            if (trans != null)
                trans.Collapsed = true;
        }

        private void treeTransformations_AfterExpand(object sender, TreeViewEventArgs e)
        {
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)e.Node.Tag;
            if (trans != null)
                trans.Collapsed = false;
        }
        private void _SelectTransformationFromTree(TreeNode root, AnimO.GenericTransformation trans)
        {
            if (root.Tag == trans)
            {
                treeTransformations.SelectedNode = root;
            }
            else
            {
                foreach (TreeNode tr in root.Nodes)
                    _SelectTransformationFromTree(tr, trans);
            }
        }
        private void SelectTransformationFromTree(AnimO.GenericTransformation trans)
        {
            _SelectTransformationFromTree(treeTransformations.Nodes[0], trans);
        }

        private void OnExpandAll(object sender, EventArgs e)
        {
            treeTransformations.ExpandAll();
        }

        private void OnCollapseAll(object sender, EventArgs e)
        {
            treeTransformations.CollapseAll();
        }

        private bool AddTranformation(AnimO.GenericTransformation t)
        {
            if (ResultedAnimationObject.Main == null)
            {
                ResultedAnimationObject.Main = t;
                UpdateTransformationsTree();
                SelectTransformationFromTree(t);
                return true;
            }
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a parent transformation to add this block in to !");
                return false;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans.GetBlockTransformations() == null)
            {
                if (treeTransformations.SelectedNode.Parent == null)
                {
                    MessageBox.Show("Root transformation is not a block transformation !!!");
                    return false;
                }
                trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Parent.Tag;
            }                
            trans.GetBlockTransformations().Add(t);
            UpdateTransformationsTree();
            SelectTransformationFromTree(t);
            return true;
        }

        private void OnAddNewBlock(object sender, EventArgs e)
        {
            AnimationSimpleTransformationDialog dlg = new AnimationSimpleTransformationDialog(imgTransform, true,false);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                AddTranformation(dlg.SelectedTransformation);
        }

        private void OnAddNewParent(object sender, EventArgs e)
        {
            AnimationSimpleTransformationDialog dlg = new AnimationSimpleTransformationDialog(imgTransform, true,false);
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            AnimO.GenericTransformation t = dlg.SelectedTransformation;
            if (ResultedAnimationObject.Main == null)
            {
                ResultedAnimationObject.Main = t;
                UpdateTransformationsTree();
                SelectTransformationFromTree(t);
                return;
            }
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a parent transformation to add a parent !");
                return;
            }

            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans == ResultedAnimationObject.Main)
            {
                // eu sunt root-ul
                t.GetBlockTransformations().Add(trans);
                ResultedAnimationObject.Main = t;
            }
            else
            {
                // caut parintele meu
                AnimO.GenericTransformation parent = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Parent.Tag;
                // caut pozitia mea in lista parintelui
                int found = -1;
                var copii = parent.GetBlockTransformations();
                for (int tr = 0; tr < copii.Count; tr++)
                    if (copii[tr] == trans)
                        found = tr;
                if (found == -1)
                {
                    MessageBox.Show("Internal error - unable to find child position in parent children list !");
                    return;
                }
                copii[found] = t;
                t.GetBlockTransformations().Add(trans);
            }
            UpdateTransformationsTree();
            SelectTransformationFromTree(t);
        }

        private void OnAddNewGenericTransformationAction(object sender, EventArgs e)
        {
            AnimationSimpleTransformationDialog dlg = new AnimationSimpleTransformationDialog(imgTransform, false, true);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                AddTranformation(dlg.SelectedTransformation);
        }

        private void OnConvertCurrentBlockTo(object sender, EventArgs e)
        {
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select node to convert to");
                return;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans.GetBlockTransformations() == null)
            {
                MessageBox.Show("Please select a bock node (sequace, parallel, repeat, branch, ...). Only block nodes can be converted !");
                return;
            }
            AnimationSimpleTransformationDialog dlg = new AnimationSimpleTransformationDialog(imgTransform, true, false);
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            AnimO.GenericTransformation t = dlg.SelectedTransformation;
            // transfer copii
            foreach (var c in trans.GetBlockTransformations())
                t.GetBlockTransformations().Add(c);
            // transfer si branch name-ul
            t.BranchName = trans.BranchName;
            if (trans == ResultedAnimationObject.Main)
            {
                // eu sunt root-ul
                ResultedAnimationObject.Main = t;
            }
            else
            {
                // caut parintele meu
                AnimO.GenericTransformation parent = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Parent.Tag;
                // caut pozitia mea in lista parintelui
                int found = -1;
                var copii = parent.GetBlockTransformations();
                for (int tr = 0; tr < copii.Count; tr++)
                    if (copii[tr] == trans)
                        found = tr;
                if (found == -1)
                {
                    MessageBox.Show("Internal error - unable to find child position in parent children list !");
                    return;
                }
                copii[found] = t;
            }
            UpdateTransformationsTree();
            SelectTransformationFromTree(t);
        }


        private void OnAddNewTransformation(object sender, EventArgs e)
        {
            AnimationNewTransformationDialog dlg = new AnimationNewTransformationDialog(this.pContext, this.ResultedAnimationObject, this.imgTransform, lstElements.GetCurrentSelectedObject());
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimO.GenericTransformation newTrans = AnimO.Factory.CreateTransformation(dlg.SelectedTransformationnType, dlg.SelectedElement);
                if (newTrans == null)
                {
                    MessageBox.Show("Internal error - unable to create object of type: '" + dlg.SelectedTransformationnType.ToString() + "' in the factory object !");
                    return;
                }
                AddTranformation(newTrans);
            }
        }

        private void propTransformation_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateTreeNode(treeTransformations.SelectedNode);
        }

        private void OnDeleteTransformation(object sender, EventArgs e)
        {
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a transformation to delete !");
                return;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans == null)
            {
                MessageBox.Show("Internal error - NULL transformatioon !");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete transformation: '" + trans.GetName() + "' ?", "Delete", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;
            if (trans == ResultedAnimationObject.Main)
            {
                ResultedAnimationObject.Main = null;
            }
            else
            {
                AnimO.GenericTransformation parentTrans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Parent.Tag;
                parentTrans.GetBlockTransformations().Remove(trans);
            }
            UpdateTransformationsTree();
        }

        private void OnDeleteAndMoveToParent(object sender, EventArgs e)
        {
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a transformation to delete !");
                return;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans == null)
            {
                MessageBox.Show("Internal error - NULL transformatioon !");
                return;
            }
            if ((trans.GetBlockTransformations() == null) || (trans.GetBlockTransformations().Count==0))
            {
                // ori nu are copii, ori e o transformare simpla - apelez prima forma
                OnDeleteTransformation(null, null);
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete transformation: '" + trans.GetName() + "' ?", "Delete", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;
            if (trans == ResultedAnimationObject.Main)
            {
                if (trans.GetBlockTransformations().Count>1)
                {
                    MessageBox.Show("Can not delete current block because it has too many children and only ONE can be the root ! Remove the children util only one remains and try again !");
                    return;
                }
                // am un singur copil
                ResultedAnimationObject.Main = trans.GetBlockTransformations()[0];
            }
            else
            {
                // caut parintele meu
                AnimO.GenericTransformation parent = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Parent.Tag;
                // caut pozitia mea in lista parintelui
                int found = -1;
                var copii = parent.GetBlockTransformations();
                for (int tr = 0; tr < copii.Count; tr++)
                    if (copii[tr] == trans)
                        found = tr;
                if (found == -1)
                {
                    MessageBox.Show("Internal error - unable to find child position in parent children list !");
                    return;
                }
                // inserez copii mei
                copii[found] = trans.GetBlockTransformations()[0];
                found++;
                for (int tr = 1; tr < trans.GetBlockTransformations().Count; tr++, found++)
                    copii.Insert(found, trans.GetBlockTransformations()[tr]);
            }
            UpdateTransformationsTree();
        }

        private void OnMoveBranchTo(object sender, EventArgs e)
        {
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a transformation to move to another branch !");
                return;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans == null)
            {
                MessageBox.Show("Internal error - NULL transformation !");
                return;
            }
            if (trans == ResultedAnimationObject.Main)
            {
                MessageBox.Show("Root(main) transformation can not be move !");
                return;
            }
            MoveToAnotherBranchDialog dlg = new MoveToAnotherBranchDialog(imgTransform,ResultedAnimationObject, trans);
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            // ma sterg de la parintele meu curent
            AnimO.GenericTransformation parent = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Parent.Tag;
            parent.GetBlockTransformations().Remove(trans);
            // ma adaug la parintele nou
            dlg.SelectedNewParent.GetBlockTransformations().Add(trans);
            UpdateTransformationsTree();
            SelectTransformationFromTree(trans);
        }

        private void MoveTransformation(int dir)
        {
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a transformation to move !");
                return;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans == null)
            {
                MessageBox.Show("Internal error - NULL transformation !");
                return;
            }
            if (trans == ResultedAnimationObject.Main)
            {
                MessageBox.Show("Root(main) transformation can not be move !");
                return;
            }
            AnimO.GenericTransformation parentTrans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Parent.Tag;
            List<AnimO.GenericTransformation> lst = parentTrans.GetBlockTransformations();
            int index = lst.IndexOf(trans);
            if (index<0)
            {
                MessageBox.Show("Internal error - Expecting a positive index for transformation !");
                return;
            }
            int newIndex = index + dir;
            if (newIndex < 0)
                newIndex = 0;
            lst.Remove(trans);
            if (newIndex >= lst.Count)
                lst.Add(trans);
            else
                lst.Insert(newIndex, trans);
            UpdateTransformationsTree();
            SelectTransformationFromTree(trans);
        }

        private void OnMoveTransformationUp(object sender, EventArgs e)
        {
            MoveTransformation(-1);
        }

        private void OnMoveTransformationDown(object sender, EventArgs e)
        {
            MoveTransformation(1);
        }
        #endregion

        #region Preview

        private void OnCenterBoard(object sender, EventArgs e)
        {
            board.CenterBoard();
        }

        private void AnimationObjectEditor_Shown(object sender, EventArgs e)
        {
            board.CenterBoard();
        }

        private void OnPlay(object sender, EventArgs e)
        {
            pContext.Prj.EC.Reset();
            if (ResultedAnimationObject.ValidateAnimation(pContext.Prj,pContext.Resources)==false)
            {
                pContext.Prj.ShowErrors();
                return;
            }
            if (this.ResultedAnimationObject.Main == null)
            {
                MessageBox.Show("No transformation available - nothing to do !");
                return;
            }
            SetDisplayMode(AnimO.BoardViewMode.Play);
            foreach (var elem in ResultedAnimationObject.Elements)
                elem.InitRuntimeContext();

            UpdateElementsZOrder(ResultedAnimationObject.ZOrder);

            this.ResultedAnimationObject.Main.Init();

            btnPlay.Enabled = false;
            btnStop.Enabled = true;
            btnStartConfig.Enabled = false;
            animationTimer.Enabled = true;
        }

        private void OnPlayAnimation(object sender, EventArgs e)
        {
            if (this.ResultedAnimationObject.Main.Update() == false)
            {
                OnStop(null,null);
            }
            board.Invalidate();
        }

        void board_OnPaintCanvas(AnimO.Canvas canvas)
        {
            float d_w = deviceSize.Width;
            float d_h = deviceSize.Height;
            canvas.ClearClipping();
            UpdateElementsRelativePosition();
            // GDT: Se parcurg toate, apoi se calculeaza pozitiile pentru fiecare (procentuale) - apoi la desenare se deseneaza pe baza pozitiilor relative
            //float tempX = 0, tempY = 0;
            foreach (var elem in ZOrder)
            {
                //if (elem.ParentElement != null)
                //{
                //    tempX = elem.ExecutionContext.X_Percentage;
                //    tempY = elem.ExecutionContext.Y_Percentage;
                //    RectangleF tmpRect = elem.ParentElement.ExecutionContext.boundRect;
                //    elem.ExecutionContext.X_Percentage = tmpRect.Width * elem.ExecutionContext.X_Percentage + tmpRect.Left;
                //    elem.ExecutionContext.Y_Percentage = tmpRect.Height * elem.ExecutionContext.Y_Percentage + tmpRect.Top;
                //}
                elem.Paint(canvas, d_w, d_h, viewMode);
                //if (elem.ParentElement != null)
                //{
                //    // restabilesc
                //    elem.ExecutionContext.X_Percentage = tempX;
                //    elem.ExecutionContext.Y_Percentage = tempY;
                //}
            }
            canvas.ClearClipping();
        }

        private void OnStop(object sender, EventArgs e)
        {
            animationTimer.Enabled = false;
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
            btnStartConfig.Enabled = true;
        }

        private void OnStartConfig(object sender, EventArgs e)
        {
            SetDisplayMode(AnimO.BoardViewMode.Design);          
            pContext.Prj.EC.Reset();
            if (ResultedAnimationObject.ValidateAnimation(pContext.Prj, pContext.Resources) == false)
            {
                pContext.Prj.ShowErrors();
                return;
            }
            foreach (var elem in ResultedAnimationObject.Elements)
                elem.InitRuntimeContext();

            UpdateElementsZOrder(ResultedAnimationObject.ZOrder);
        }

        private void OnAlignToGrid(object sender, EventArgs e)
        {
            board.AlignToGrid = btnAlignToGrid.Checked;
            board.Invalidate();
        }

        private void OnSetBackgroundColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                board.SetBackgroundColor(dlg.Color);
                ResultedAnimationObject._BackgroundColor = dlg.Color;
                board.Invalidate();
            }
        }

        #endregion

        #region Copy/Paste operation
        private void OnCopyTransformation(object sender, EventArgs e)
        {
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a transformation to move to another branch !");
                return;
            }
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            if (trans == null)
            {
                MessageBox.Show("Internal error - NULL transformation !");
                return;
            }
            Clipboard.SetText("TRANSOBJECT:" + trans.ToXMLString());
        }

        private void OnPasteTransformation(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText()==false)
            {
                MessageBox.Show("No transformation object available in the clipboard !");
                return;
            }
            string res = Clipboard.GetText();
            if (res.StartsWith("TRANSOBJECT:")==false)
            {
                MessageBox.Show("Invalid transformation object: Marker not found !");
                return;
            }
            AnimO.GenericTransformation t = AnimO.GenericTransformation.FromXMLString(res.Substring(12));
            if (t == null)
                return;

            AddTranformation(t);
        }
        private void OnCopyElement(object sender, EventArgs e)
        {
            if (lstElements.GetCurrentSelectedObjectsListCount() != 1)
            {
                MessageBox.Show("Please select only one element to copy !");
                return;
            }

            AnimO.GenericElement elem = (AnimO.GenericElement)lstElements.GetCurrentSelectedObject();

            if (elem == null)
            {
                MessageBox.Show("Internal error - NULL transformation !");
                return;
            }
            Clipboard.SetText("ELEMENT:" + elem.ToXMLString());
        }

        private void OnPasteElement(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText() == false)
            {
                MessageBox.Show("No elementavailable in the clipboard !");
                return;
            }
            string res = Clipboard.GetText();
            if (res.StartsWith("ELEMENT:") == false)
            {
                MessageBox.Show("Invalid transformation object: Marker not found !");
                return;
            }
            AnimO.GenericElement elem = AnimO.GenericElement.FromXMLString(res.Substring(8));
            if (elem == null)
                return;

            AddElement(elem);
        }
        #endregion

        void AnimO.IRefreshDesign.Refresh()
        {
            board.Invalidate();
        }

    }
}
