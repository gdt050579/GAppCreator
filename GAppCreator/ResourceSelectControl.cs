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
    public partial class ResourceSelectControl : UserControl, IDataGridUserControl
    {
        private static bool disableFilterButton, enableNullResourceButton;
        private static Project prj;
        private static ProjectContext Context;      
        private ResourcesConstantType resourceType = ResourcesConstantType.None;

        public string SelectedResource = "";
        private ITerminateEdit editControl = null;

        public static void InitControl(ProjectContext _context, bool _disableFilterButton, bool _enableNullResourceButton)
        {
            prj = _context.Prj;
            Context = _context;

            disableFilterButton = _disableFilterButton;
            enableNullResourceButton = _enableNullResourceButton;
        }

        public ResourceSelectControl()
        {
            InitializeComponent();

            foreach (ResourcesConstantType rct in Enum.GetValues(typeof(ResourcesConstantType)))
            {
                if (rct == ResourcesConstantType.None)
                    continue;
                ToolStripMenuItem tp = new ToolStripMenuItem(rct.ToString());
                tp.Tag = rct;
                tp.Click += tp_Click;
                if (rct == resourceType)
                    tp.Checked = true;
                btnFilterType.DropDownItems.Add(tp);
            }
            if (disableFilterButton)
                btnFilterType.Visible = false;
            btnNone.Visible = enableNullResourceButton;
            lstResource.SmallImageList = Context.SmallIcons;
            lstResource.LargeImageList = Context.LargeIcons;
            UpdateResourceList();
        }

        void tp_Click(object sender, EventArgs e)
        {
            UpdateResourceType((ResourcesConstantType)(((ToolStripMenuItem)sender).Tag));
            UpdateResourceList();
        }
        private void UpdateResourceType(ResourcesConstantType resType)
        {
            foreach (ToolStripMenuItem tp in btnFilterType.DropDownItems)
                tp.Checked = ((ResourcesConstantType)tp.Tag) == resType;
            resourceType = resType;
        }
        public void UpdateResourceList()
        {
            lstResource.Items.Clear();
            string filter = txFilter.Text.ToLower();
            if ((resourceType!= ResourcesConstantType.None) && (resourceType!= ResourcesConstantType.String))
            {
                Type t = ConstantHelper.ConvertResourcesConstantTypeToResourceType(resourceType);
                if (t==null)
                    return;
                foreach (GenericResource r in prj.Resources)
                {
                    if (r.GetType() != t)
                        continue;
                    // daca e o versiune pentru alte rezolutii - nu o afisez
                    if (r.IsBaseResource() == false)
                        continue;
                    if (filter.Length > 0)
                    {
                        if (r.GetResourceVariableName().ToLower().Contains(filter) == false)
                            continue;
                    }
                    ListViewItem lvi = new ListViewItem(r.GetResourceVariableName());
                    lvi.SubItems.Add(r.GetResourceInformation());
                    lvi.ImageKey = r.GetIconImageListKey();
                    lstResource.Items.Add(lvi);
                }
            }
            // stringuri
            if (resourceType == ResourcesConstantType.String)
            {
                foreach (StringValues sv in prj.Strings)
                {
                    if (sv.GetVariableNameWithArray().ToLower().Contains(filter) == false)
                        continue;
                    ListViewItem lvi = new ListViewItem(sv.GetVariableNameWithArray());
                    lvi.SubItems.Add(sv.Get(prj.DefaultLanguage));                    
                    lstResource.Items.Add(lvi);                    
                }
            }            
        }

        private void OnTextFilterChanged(object sender, EventArgs e)
        {
            UpdateResourceList();
        }

        private void OnDblClickOnResource(object sender, EventArgs e)
        {
            if (editControl != null)
                editControl.FinishEdit();
        }

        private void OnChangeViewMode(object sender, EventArgs e)
        {
            if (cbLargeIcons.Checked)
                lstResource.View = View.Tile;
            else
                lstResource.View = View.Details;

        }

        private void OnSetBackgoundBlack(object sender, EventArgs e)
        {
            if (btnBlack.Checked)
                lstResource.BackColor = Color.Black;
            else
                lstResource.BackColor = Color.White;
        }


        public object GetResultedValue()
        {
            return new KeyValuePair<ResourcesConstantType, string>(resourceType, SelectedResource);
        }

        public string GetStringRepresentation(object o)
        {
            if (o == null)
                return "";
            if (o.GetType() == typeof(KeyValuePair<ResourcesConstantType, string>))
                return ((KeyValuePair<ResourcesConstantType, string>)o).Value;
            return "?";
        }

        public bool HasCustomPaint()
        {
            return true;
        }

        public void DrawCell(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value,Font fnt,StringFormat sFormat)
        {
            string txt = "";
            if (value == null)
            {
                txt = "";
                g.DrawString(txt, fnt, System.Drawing.Brushes.Black, cellBounds, sFormat);
            }
            else
            {                
                KeyValuePair<ResourcesConstantType, string> d = (KeyValuePair<ResourcesConstantType, string>)value;
                if (d.Key == ResourcesConstantType.String)
                {
                    Brush brs = System.Drawing.Brushes.Black;
                    if (Context.Resources.Strings.ContainsKey(d.Value))
                    {
                        txt = d.Value + " => " + Context.Resources.Strings[d.Value].Get(Context.Prj.DefaultLanguage);
                    }
                    else
                    {
                        if ((d.Value == null) || (d.Value.Length == 0))
                        {
                            txt = "<Null>";
                            brs = System.Drawing.Brushes.Gray;
                        }
                        else
                            txt = "Unknwon (" + d.Value + ")";
                    }
                    g.DrawString(txt, fnt, brs, cellBounds, sFormat);
                }
                else
                {
                    txt = GenericResource.GetResourceVariableKey(ConstantHelper.ConvertResourcesConstantTypeToResourceType(d.Key), d.Value);
                    if (Context.SmallIcons.Images.ContainsKey(txt))
                    {
                        g.DrawImage(Context.SmallIcons.Images[txt], cellBounds.Left+4,(cellBounds.Top+cellBounds.Bottom)/2 - Context.SmallIcons.ImageSize.Height/2);
                        int offs = 8 + Context.SmallIcons.ImageSize.Width;
                        cellBounds.X += offs;
                        cellBounds.Width -= offs;
                        g.DrawString(d.Value, fnt, System.Drawing.Brushes.Black, cellBounds, sFormat);
                    }
                    else
                    {
                        g.DrawString(d.Value, fnt, System.Drawing.Brushes.Black, cellBounds, sFormat);
                    }
                }
            }          
            
        }

        public void Init(object o, ITerminateEdit edit)
        {
            editControl = edit;
            if (o!=null)
            {
                KeyValuePair<ResourcesConstantType, string> d = (KeyValuePair<ResourcesConstantType, string>)o;
                UpdateResourceType(d.Key);
                UpdateResourceList();                
                foreach (ListViewItem lvi in lstResource.Items)
                    if (d.Value.Equals(lvi.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        lvi.Selected = true;
                        break;
                    }
                SelectedResource = d.Value;
            }
            
        }

        private void OnNone(object sender, EventArgs e)
        {
            lstResource.SelectedItems.Clear();
            if (editControl != null)
                editControl.FinishEdit();
        }

        private void OnSelectItem(object sender, EventArgs e)
        {
            if (lstResource.SelectedItems.Count > 0)
            {
                SelectedResource = lstResource.SelectedItems[0].Text;
            }
            else
            {
                SelectedResource = "";
            }
        }
    }
}
