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
    public partial class PresentationResourceKeyEditorDialog : Form
    {
        Presentation anim;
        Project prj;
        ResourcesIndexes ri;

        public PresentationResourceKeyEditorDialog(Project _prj, Presentation _anim,ImageList smallImageList,ImageList largeImageList)
        {
            InitializeComponent();

            anim = _anim;
            prj = _prj;

            lstResources.Groups.Add(typeof(ImageResource).ToString(), "Images");
            lstResources.Groups.Add("String", "Strings");
            lstResources.Groups.Add(typeof(FontResource).ToString(), "Fonts");
            lstResources.Groups.Add(typeof(ShaderResource).ToString(), "Shaders");
            lstResources.Groups.Add(typeof(SoundResource).ToString(), "Sounds");

            lstResources.SmallImageList = smallImageList;
            lstResources.LargeImageList = largeImageList;

            lstResources.TileSize = new System.Drawing.Size(180, 100);

            ri = anim.CreateResourcesIndexes();
            foreach (GenericResource gr in prj.Resources)
            {
                if (ri.ImageIndex.ContainsKey(gr.GetResourceVariableName()))
                {
                    if (ri.ImageIndex[gr.GetResourceVariableName()].Resource==null)
                        ri.ImageIndex[gr.GetResourceVariableName()].Resource = gr;
                }                    
                if (ri.FontsIndex.ContainsKey(gr.GetResourceVariableName()))
                {
                    if (ri.FontsIndex[gr.GetResourceVariableName()].Resource == null)
                        ri.FontsIndex[gr.GetResourceVariableName()].Resource = gr;
                }
                if (ri.ShaderIndex.ContainsKey(gr.GetResourceVariableName()))
                {
                    if (ri.ShaderIndex[gr.GetResourceVariableName()].Resource == null)
                        ri.ShaderIndex[gr.GetResourceVariableName()].Resource = gr;
                }
                if (ri.SoundsIndex.ContainsKey(gr.GetResourceVariableName()))
                {
                    if (ri.SoundsIndex[gr.GetResourceVariableName()].Resource == null)
                        ri.SoundsIndex[gr.GetResourceVariableName()].Resource = gr;
                }
            }

            PopulateList();
        }

        private void AddItem(string resourceName,ResourceAnimationItem rai,Type t)
        {
            if (txFilter.Text.Length > 0)
            {
                if (resourceName.IndexOf(txFilter.Text, StringComparison.InvariantCultureIgnoreCase) < 0)
                    return;
            }
            ListViewItem lvi = new ListViewItem(resourceName);
            lvi.SubItems.Add(rai.Name);
            lvi.Group = lstResources.Groups[t.ToString()];
            lvi.Tag = rai;
            if (rai.Resource != null)
                lvi.ImageKey = rai.Resource.GetIconImageListKey();
            lstResources.Items.Add(lvi);
        }
        private void AddStringItem(string resourceName, ResourceAnimationItem rai)
        {
            if (txFilter.Text.Length>0)
            {
                if (resourceName.IndexOf(txFilter.Text, StringComparison.InvariantCultureIgnoreCase)<0)
                    return;
            }
            ListViewItem lvi = new ListViewItem(resourceName);
            lvi.SubItems.Add(rai.Name);
            lvi.Tag = rai;
            lvi.Group = lstResources.Groups["String"];
            lstResources.Items.Add(lvi);
        }
        private void PopulateList()
        {
            lstResources.Items.Clear();
            // imagini
            foreach (string key in ri.ImageIndex.Keys)
                AddItem(key, ri.ImageIndex[key], typeof(ImageResource));
            // stringuri
            foreach (string key in ri.StringIndex.Keys)
                AddStringItem(key, ri.StringIndex[key]);
            // fonturi
            foreach (string key in ri.FontsIndex.Keys)
                AddItem(key, ri.FontsIndex[key], typeof(FontResource));
            // shadere
            foreach (string key in ri.ShaderIndex.Keys)
                AddItem(key, ri.ShaderIndex[key], typeof(ShaderResource));
            // sunete
            foreach (string key in ri.SoundsIndex.Keys)
                AddItem(key, ri.SoundsIndex[key], typeof(SoundResource));


            UpdateColorsForItems();
        }

        private void UpdateColorsForItems()
        {
            foreach (ListViewItem lvi in lstResources.Items)
            {
                if (lvi.SubItems[1].Text.Length == 0)
                    lvi.ForeColor = Color.Gray;
                else
                    lvi.ForeColor = Color.Black;
            }
        }

        private void UpdateTemplateNames(List<ResourceKeyName> lst,string group)
        {
            lst.Clear();
            foreach (ListViewItem lvi in lstResources.Items)
                if ((lvi.SubItems[1].Text.Length>0) && (lvi.Group == lstResources.Groups[group]))
                {
                    ResourceKeyName rkn = new ResourceKeyName();
                    rkn.ResourceName = lvi.Text;
                    rkn.KeyName = lvi.SubItems[1].Text;
                    lst.Add(rkn);
                }
        }
        private void OnOK(object sender, EventArgs e)
        {            
            UpdateTemplateNames(anim.ImageTemplateNames,typeof(ImageResource).ToString());
            UpdateTemplateNames(anim.StringTemplateNames, "String");
            UpdateTemplateNames(anim.FontTemplateNames, typeof(FontResource).ToString());
            UpdateTemplateNames(anim.ShaderTemplateNames, typeof(ShaderResource).ToString());
            UpdateTemplateNames(anim.SoundTemplateNames, typeof(SoundResource).ToString());

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnShowLargeIcons(object sender, EventArgs e)
        {
            if (btnLargeIcons.Checked)
                lstResources.View = View.Tile;
            else
                lstResources.View = View.Details;
        }

        private void OnChangeFilter(object sender, EventArgs e)
        {
            PopulateList();
        }

        private void OnAddEdit(object sender, EventArgs e)
        {
            if (lstResources.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select one item !");
                return;
            }
            InputBox ib = new InputBox("Enter key name", lstResources.SelectedItems[0].SubItems[1].Text);
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string s = ib.StringResult;
                if (Project.ValidateVariableNameCorectness(s,false)==false)
                {
                    MessageBox.Show("The key name must be form from the following characters: [a-z], [A-Z], [0-9] , '_' and must start with a letter !");
                    return;
                }
                lstResources.SelectedItems[0].SubItems[1].Text = s;
                ((ResourceAnimationItem)lstResources.SelectedItems[0].Tag).Name = s;
                UpdateColorsForItems();
            }
        }

        private void OnDoubleClicked(object sender, MouseEventArgs e)
        {
            OnAddEdit(null, null);
        }

        private void OnDeleteKey(object sender, EventArgs e)
        {
            if (lstResources.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please select at least one item !");
                return;
            }
            if (MessageBox.Show("Delete " + lstResources.SelectedItems.Count.ToString() + " keys ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (ListViewItem lvi in lstResources.SelectedItems)
                {
                    lvi.SubItems[1].Text = "";
                    ((ResourceAnimationItem)lvi.Tag).Name = "";
                }
                UpdateColorsForItems();
            }
        }

        private void OnCopyNameToKey(object sender, EventArgs e)
        {
            if (lstResources.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please select at least one item !");
                return;
            }
            if (MessageBox.Show("Copy name to key for " + lstResources.SelectedItems.Count.ToString() + " items ?", "Copy name to key", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (ListViewItem lvi in lstResources.SelectedItems)
                {
                    ResourceAnimationItem rai = (ResourceAnimationItem)lvi.Tag;
                    rai.Name = lvi.Text;
                    lvi.SubItems[1].Text = rai.Name;
                }
                UpdateColorsForItems();
            }
        }
    }
}
