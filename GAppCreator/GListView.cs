using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;
using System.Windows.Forms;
using System.Drawing;

namespace GAppCreator
{
    public class GListView: ObjectListView
    {
        public enum RenderType
        {
            Default,
            ItemRenderer,
            BooleanCheckBox,
        }
        public delegate void OnObjectDoubleClickedDelegate(object source, object SelectedObject);
        public delegate void OnObjectsSelectedDelegate(object source, bool selected, System.Collections.IList SelectedObjects);
        public delegate ContextMenuStrip OnShowContextualMenuDelegate(object source, bool selected, System.Collections.IList SelectedObjects);
        public delegate bool OnFilterObjectDelegate(object obj);

        
        public event OnObjectDoubleClickedDelegate OnObjectDoubleClicked = null;
        public event OnObjectsSelectedDelegate OnObjectsSelected = null;
        public event OnShowContextualMenuDelegate OnShowContextualMenu = null;
        public event OnFilterObjectDelegate OnFilterObject = null;
        private ModelFilter mFilter;
        public GListView()
        {
            View = System.Windows.Forms.View.Details;
            FullRowSelect = true;
            GridLines = true;
            HideSelection = false;
            this.ShowItemCountOnGroups = true;
            this.UseFiltering = true;
            this.ShowSortIndicators = true;

            //this.TintSortColumn = true;

            this.SelectionChanged += new System.EventHandler(this._OnSelectedEvent);
            this.DoubleClick += new System.EventHandler(this._OnDoubleClickEvent);
            this.MouseUp += GListView_MouseUp;
            this.ColumnClick += GListView_ColumnClick;


            mFilter = new ModelFilter(delegate(object x)
            {
                if (OnFilterObject != null) return OnFilterObject(x);
                return true;
            });
            this.ModelFilter = mFilter;

            this.CustomSorter = delegate(OLVColumn column, SortOrder order)
            {
                this.ListViewItemSorter = new ColumnComparer(column, order);
            };
            
            this.KeyDown += GListView_KeyDown;
            this.KeyUp += GListView_KeyUp;
        }

        void GListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;               
            }
        }


        void GListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _OnDoubleClickEvent(null, null);
                e.Handled = true;
                e.SuppressKeyPress = true;
                
            }
        }

        void GListView_BeforeSorting(object sender, BeforeSortingEventArgs e)
        {
        }

        void GListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column>=0)
            {
                OLVColumn col = AllColumns[e.Column];
                ShowGroups = ((bool)col.Tag);
            }
        }

        void GListView_MouseUp(object sender, MouseEventArgs e)
        {
            if ((OnShowContextualMenu == null) || (e.Button != System.Windows.Forms.MouseButtons.Right))
                return;
            ContextMenuStrip menu = null;
            if ((this.SelectedObjects == null) || (this.SelectedObjects.Count == 0))
            {
                menu = OnShowContextualMenu(this, false, null);
            }
            if ((this.SelectedObjects != null) && (this.SelectedObjects.Count > 0))
            {
                menu = OnShowContextualMenu(this, true, this.SelectedObjects);
            }
            if (menu != null)
                menu.Show(this, e.Location);
        }

        private void _OnDoubleClickEvent(object sender, EventArgs e)
        {
            if ((OnObjectDoubleClicked != null) && (this.SelectedObjects!=null))
                OnObjectDoubleClicked(this, this.SelectedObject);
        }

        private void _OnSelectedEvent(object sender, EventArgs e)
        {
            if ((this.SelectedObjects == null) || (this.SelectedObjects.Count==0))
            {
                if (OnObjectsSelected != null)
                    OnObjectsSelected(this,false,null);
            }
            if ((this.SelectedObjects != null) && (this.SelectedObjects.Count>0))
            {
                if (OnObjectsSelected != null)
                    OnObjectsSelected(this,true,this.SelectedObjects);
            }
        }
        private void _OnFormatGroupt(OLVGroup group, GroupingParameters parms)
        {
            //group.BottomDescription = group.Items.Count.ToString() + " Items";
            //group.HeaderAlignment = HorizontalAlignment.Center;
            //group.Header = group.Key.ToString();
        }

        public void SetColumnPropertyName(int index,string prop)
        {
            this.AllColumns[index].AspectName = prop;
        }

        public void AddColumn(string Name, string prop, int Width, RenderType rendererType, bool canBeGroup, HorizontalAlignment align)
        {
            OLVColumn col = new OLVColumn(Name, prop);
            col.Width = Width;
            col.Tag = canBeGroup;
            col.TextAlign = align;
            col.GroupFormatter = delegate(OLVGroup group, GroupingParameters parms) { _OnFormatGroupt(group, parms); };
                        
            if (rendererType == RenderType.ItemRenderer)
            {
                ItemRenderer ir = new GAppCreator.ItemRenderer();
                ir.DescriptionAspectName = "propDescription";
                ir.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
                ir.UseGdiTextRendering = true;
                //ir.ColorAspectName
                //ir.DescriptionFont = new System.Drawing.Font("Arial", 9);
                col.Renderer = ir;
                this.RowHeight = 34;
            }
            if (rendererType == RenderType.BooleanCheckBox)
            {
                col.CheckBoxes = true;
            }
            if (Columns.Count == 0)
                ShowGroups = canBeGroup;
            this.AllColumns.Add(col);
            this.Columns.Add(col);
        }
        public void SetColumnIconPropertyName(int index,string propName)
        {
            if ((index>=0) && (index<AllColumns.Count))
            {
                this.AllColumns[index].ImageAspectName = propName;
            }
        }
        public void UseColumnForTileView(int index)
        {
            if ((index >= 0) && (index < AllColumns.Count))
            {
                this.AllColumns[index].IsTileViewColumn = true;
            }
        }
        public int GetColumnsCount()
        {
            return AllColumns.Count;
        }
        public void SetColumnVisible(int index, bool visible)
        {
            if ((index >= 0) && (index < AllColumns.Count))
            {
                this.AllColumns[index].IsVisible = visible;
            }
        }
        public object GetCurrentSelectedObject()
        {
            if ((SelectedObjects != null) && (SelectedObjects.Count == 1))
                return SelectedObjects[0];
            return null;
        }
        public System.Collections.IList GetCurrentSelectedObjectsList()
        {
            if ((SelectedObjects == null) || (SelectedObjects.Count == 0))
                return null;
            return SelectedObjects;

        }
        public int GetCurrentSelectedObjectsListCount()
        {
            if ((SelectedObjects == null) || (SelectedObjects.Count == 0))
                return 0;
            return SelectedObjects.Count;

        }
        public void Refilter()
        {
            this.ModelFilter = mFilter;
        }
        public object[] GetSelectedObjectsArray()
        {
            if ((SelectedObjects == null) || (SelectedObjects.Count == 0))
                return null;
            object[] o = new object[SelectedObjects.Count];
            SelectedObjects.CopyTo(o, 0);
            return o;
        }

        public void SelectObjectFromList(Object obj)
        {
            this.SelectObject(obj,true);
        }
    }
}
