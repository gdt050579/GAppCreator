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
    public partial class ProjectTabMemoryProfiles : BaseProjectContainer
    {
        Dictionary<string, GenericResource> currentResources = null; // folosit in profile
        Dictionary<string, List<GenericResource>> currentResourcesGroupedByVariableName = null;
        Dictionary<string, List<GenericResource>> varGroups = new Dictionary<string, List<GenericResource>>();
        public ProjectTabMemoryProfiles()
        {
            InitializeComponent();

            // grupuri pentru profile
            lstAvailableResourcesForProfile.Groups.Add(typeof(PresentationResource).ToString(), "Presentations");
            lstAvailableResourcesForProfile.Groups.Add(typeof(ImageResource).ToString(), "Images");
            lstAvailableResourcesForProfile.Groups.Add(typeof(ShaderResource).ToString(), "Shaders");
            lstAvailableResourcesForProfile.Groups.Add(typeof(SoundResource).ToString(), "Sounds");
            lstAvailableResourcesForProfile.Groups.Add(typeof(RawResource).ToString(), "Raw");
            lstAvailableResourcesForProfile.Groups.Add(typeof(FontResource).ToString(), "Fonts");

            lstProfileResources.Groups.Add("unknown", "Missing resources");
            lstProfileResources.Groups.Add(typeof(PresentationResource).ToString(), "Presentations");
            lstProfileResources.Groups.Add(typeof(ImageResource).ToString(), "Images");
            lstProfileResources.Groups.Add(typeof(ShaderResource).ToString(), "Shaders");
            lstProfileResources.Groups.Add(typeof(SoundResource).ToString(), "Sounds");
            lstProfileResources.Groups.Add(typeof(RawResource).ToString(), "Raw");
            lstProfileResources.Groups.Add(typeof(FontResource).ToString(), "Fonts");

            currentResourcesGroupedByVariableName = new Dictionary<string, List<GenericResource>>();
        }
        public override void OnActivate()
        {
            currentResources = Context.Prj.GetBaseResources();
            PopulateProfileList();
            lstAvailableResourcesForProfile.Items.Clear();
            lstAvailableResourcesForProfile.Enabled = false;
            lstProfileResources.Items.Clear();
            lstProfileResources.Enabled = false;            
        }
        private void PopulateAvailableResourcesForProfile()
        {
            lstAvailableResourcesForProfile.Items.Clear();
            lstAvailableResourcesForProfile.Enabled = false;
            if (lstProfiles.SelectedItems.Count != 1)
                return;
            Profile p = (Profile)lstProfiles.SelectedItems[0].Tag;
            Dictionary<string, string> d = p.GetAllResources();
            if (cbAnyProfile.Checked)
            {
                foreach (Profile pp in Context.Prj.Profiles)
                {
                    Dictionary<string, string> dd = pp.GetAllResources();
                    foreach (string key in dd.Keys)
                        d[key] = dd[key];
                }
            }

            if (cbGroupByVariableName.Checked)
                varGroups.Clear();
            string nk;

            lstAvailableResourcesForProfile.SmallImageList = Context.SmallIcons;
            lstAvailableResourcesForProfile.LargeImageList = Context.LargeIcons;
            string filter = txProfileResourcesFilter.Text.ToLower();
            foreach (GenericResource res in Context.Prj.Resources)
            {
                if (Profile.IsAnAccepedResource(res) == false)
                    continue;
                if (d.ContainsKey(res.GetResourceVariableKey()))
                    continue;
                // daca e o imagine derivata ii fac skip
                if (res.IsBaseResource() == false)
                    continue;
                nk = res.GetResourceVariableKey();
                if ((filter.Length > 0) && (nk.ToLower().Contains(filter) == false))
                    continue;
                if (cbGroupByVariableName.Checked)
                {
                    
                    int idx = nk.IndexOf('[');
                    if (idx > 0)
                        nk = nk.Substring(0, idx);
                    if (varGroups.ContainsKey(nk))
                    {
                        varGroups[nk].Add(res);
                    }
                    else
                    {
                        varGroups[nk] = new List<GenericResource>();
                        varGroups[nk].Add(res);
                        ListViewItem lvi = new ListViewItem();
                        lvi.Tag = varGroups[nk];
                        lvi.Text = res.Name;
                        lvi.SubItems.Add(res._Type);
                        lvi.SubItems.Add("");
                        lvi.ImageKey = res.GetIconImageListKey();
                        lvi.Group = lstAvailableResourcesForProfile.Groups[res.GetType().ToString()];
                        lstAvailableResourcesForProfile.Items.Add(lvi);
                    }
                }
                else
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Tag = res;
                    lvi.Text = res.GetResourceVariableName();
                    lvi.SubItems.Add(res._Type);
                    lvi.SubItems.Add(res.GetResourceInformation());
                    lvi.ImageKey = res.GetIconImageListKey();
                    lvi.Group = lstAvailableResourcesForProfile.Groups[res.GetType().ToString()];
                    lstAvailableResourcesForProfile.Items.Add(lvi);
                }
            }
            // ajustez numerele
            if (cbGroupByVariableName.Checked)
            {
                foreach (ListViewItem lvi in lstAvailableResourcesForProfile.Items)
                {
                    if (lvi.Tag.GetType() == typeof(List<GenericResource>))
                    {
                        List<GenericResource> l = (List<GenericResource>)lvi.Tag;
                        if ((l.Count > 1) && (lvi.SubItems[2].Text.Length == 0))
                            lvi.SubItems[2].Text = l.Count.ToString() + " elements";
                    }
                }
            }
            lstAvailableResourcesForProfile.Enabled = true;
        }
        private void PopulateResourcesForProfile()
        {
            lstProfileResources.Items.Clear();
            lstProfileResources.Enabled = false;
            if (lstProfiles.SelectedItems.Count != 1)
                return;
            currentResourcesGroupedByVariableName.Clear();
            Profile p = (Profile)lstProfiles.SelectedItems[0].Tag;
            lstProfileResources.SmallImageList = Context.SmallIcons;
            lstProfileResources.LargeImageList = Context.LargeIcons;
            Dictionary<string, string> d = p.GetAllResources();
            string nk;
            string filter = txProfileResourcesFilter.Text.ToLower();
            foreach (string k in d.Keys)
            {
                if ((filter.Length > 0) && (d[k].ToLower().Contains(filter) == false))
                    continue;
                if (currentResources.ContainsKey(k))
                {
                    GenericResource res = currentResources[k];
                    if (cbGroupByVariableName.Checked)
                    {
                        int idx = k.IndexOf('[');
                        if (idx > 0)
                            nk = k.Substring(0, idx);
                        else
                            nk = k;
                        if (currentResourcesGroupedByVariableName.ContainsKey(nk))
                        {
                            currentResourcesGroupedByVariableName[nk].Add(res);
                        }
                        else
                        {
                            currentResourcesGroupedByVariableName[nk] = new List<GenericResource>();
                            currentResourcesGroupedByVariableName[nk].Add(res);
                            ListViewItem lvi = new ListViewItem();
                            lvi.Tag = currentResourcesGroupedByVariableName[nk];
                            lvi.Text = res.Name;
                            lvi.SubItems.Add(res._Type);
                            lvi.SubItems.Add("");
                            lvi.ImageKey = res.GetIconImageListKey();
                            lvi.Group = lstProfileResources.Groups[res.GetType().ToString()];
                            if ((res.GetType() == typeof(RawResource)) && (((RawResource)res).LocalCopy == false))
                            {
                                lvi.ForeColor = Color.Red;
                                lvi.SubItems[2].Text = "Raw resources that does not store data localy should not be added in a memory management profile !";
                            }
                            lstProfileResources.Items.Add(lvi);
                        }
                      
                    }
                    else
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Tag = res;
                        lvi.Text = res.GetResourceVariableName();
                        lvi.SubItems.Add(res._Type);
                        lvi.SubItems.Add(res.GetResourceInformation());
                        lvi.ImageKey = res.GetIconImageListKey();
                        lvi.Group = lstProfileResources.Groups[res.GetType().ToString()];
                        if ((res.GetType() == typeof(RawResource)) && (((RawResource)res).LocalCopy == false))
                        {
                            lvi.ForeColor = Color.Red;
                            lvi.SubItems[2].Text = "Raw resources that does not store data localy should not be added in a memory management profile !";
                        }
                        lstProfileResources.Items.Add(lvi);
                    }
                }
                else
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = d[k];
                    lvi.Tag = k;
                    lvi.SubItems.Add("Unknwon");
                    lvi.SubItems.Add("Current resource does not exist !");
                    lvi.ForeColor = Color.Red;
                    lvi.Group = lstProfileResources.Groups["unknown"];
                    lstProfileResources.Items.Add(lvi);
                }
            }
            // ajustez numerele
            if (cbGroupByVariableName.Checked)
            {
                foreach (ListViewItem lvi in lstProfileResources.Items)
                {
                    if (lvi.Tag.GetType() == typeof(List<GenericResource>))
                    {
                        List<GenericResource> l = (List<GenericResource>)lvi.Tag;
                        if ((l.Count > 1) && (lvi.SubItems[2].Text.Length == 0))
                            lvi.SubItems[2].Text = l.Count.ToString() + " elements";
                    }
                }
            }
            lstProfileResources.Enabled = true;
        }
        private void UpdateProfileItem(ListViewItem lvi)
        {
            Profile p = (Profile)(lvi.Tag);
            lvi.Text = Project.GetVariableName(p.Name, p.Array1, p.Array2);
            lvi.SubItems[1].Text = p.Type.ToString();
            int total = p.Images.Count + p.Sounds.Count + p.Shaders.Count + p.Presentations.Count + p.Raw.Count + p.Fonts.Count;
            if (total > 0)
                lvi.SubItems[2].Text = total.ToString();
            else
                lvi.SubItems[2].Text = "-";
            lvi.SubItems[2].BackColor = Color.LightBlue;
            lvi.UseItemStyleForSubItems = false;


            // imagini
            if (p.Images.Count > 0)
                lvi.SubItems[3].Text = p.Images.Count.ToString();
            else
                lvi.SubItems[3].Text = "-";

            // sunete
            if (p.Sounds.Count > 0)
                lvi.SubItems[4].Text = p.Sounds.Count.ToString();
            else
                lvi.SubItems[4].Text = "-";

            // shadere
            if (p.Shaders.Count > 0)
                lvi.SubItems[5].Text = p.Shaders.Count.ToString();
            else
                lvi.SubItems[5].Text = "-";

            // animati
            if (p.Presentations.Count > 0)
                lvi.SubItems[6].Text = p.Presentations.Count.ToString();
            else
                lvi.SubItems[6].Text = "-";


            // animati
            if (p.Raw.Count > 0)
                lvi.SubItems[7].Text = p.Raw.Count.ToString();
            else
                lvi.SubItems[7].Text = "-";

            // Fonts
            if (p.Fonts.Count > 0)
                lvi.SubItems[8].Text = p.Fonts.Count.ToString();
            else
                lvi.SubItems[8].Text = "-";

            if (total == 0)
                lvi.ForeColor = Color.Red;
            else
                lvi.ForeColor = Color.Black;


        }
        private void PopulateProfileList()
        {
            lstProfiles.Items.Clear();
            propProfile.SelectedObject = null;
            propProfile.SelectedObjects = null;
            foreach (Profile p in Context.Prj.Profiles)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = p;
                for (int tr = 0; tr < lstProfiles.Columns.Count; tr++)
                    lvi.SubItems.Add("");

                lstProfiles.Items.Add(lvi);
                UpdateProfileItem(lvi);
            }
        }
        private void OnAddNewProfile(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter profile name", "NewProfile");
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Profile p = new Profile();
                p.Name = ib.StringResult;
                Context.Prj.Profiles.Add(p);
                PopulateProfileList();
                PopulateAvailableResourcesForProfile();
                PopulateResourcesForProfile();
            }
        }
        private void OnAddNewProfileArray(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter profile name", "NewProfile");
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InputBox ib2 = new InputBox("Select profile array size: simple (10) or matrix (10x20)", "10");
                if (ib2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int a1 = -1;
                    int a2 = -1;
                    if (ib2.StringResult.ToLower().Contains('x'))
                    {
                        if (Project.SizeToValues(ib2.StringResult, ref a1, ref a2) == false)
                        {
                            MessageBox.Show("Invalid numeric value (expecting number x number) found " + ib2.StringResult);
                            return;
                        }
                    }
                    else
                    {
                        if (int.TryParse(ib2.StringResult, out a1) == false)
                        {
                            MessageBox.Show("Invalid numeric value: " + ib2.StringResult);
                            return;
                        }
                    }
                    if (a1 > 1000)
                    {
                        MessageBox.Show("First index " + a1.ToString() + " is too large - should be smaller than 1000");
                        return;
                    }
                    if (a2 > 1000)
                    {
                        MessageBox.Show("Second index " + a2.ToString() + " is too large - should be smaller than 1000");
                        return;
                    }
                    if (a1 <= 0)
                    {
                        MessageBox.Show("First Index should be bigger than 0 to have an array ");
                        return;
                    }
                    if (a2 > 0)
                    {
                        for (int tr = 0; tr < a1; tr++)
                        {
                            for (int gr = 0; gr < a2; gr++)
                            {
                                Profile p = new Profile();
                                p.Name = ib.StringResult;
                                p.Array1 = tr;
                                p.Array2 = gr;
                                Context.Prj.Profiles.Add(p);
                            }
                        }
                    }
                    else
                    {
                        for (int tr = 0; tr < a1; tr++)
                        {
                            Profile p = new Profile();
                            p.Name = ib.StringResult;
                            p.Array1 = tr;
                            Context.Prj.Profiles.Add(p);
                        }
                    }
                    PopulateProfileList();
                    PopulateAvailableResourcesForProfile();
                    PopulateResourcesForProfile();
                }
            }
        }
        private void OnDeleteProfiles(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("You have to have at least one profile selected to be able to delete it");
                return;
            }
            if (MessageBox.Show("Delete " + lstProfiles.SelectedItems.Count.ToString() + " profile(s) ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (ListViewItem lvi in lstProfiles.SelectedItems)
                    Context.Prj.Profiles.Remove((Profile)lvi.Tag);
                PopulateProfileList();
                PopulateAvailableResourcesForProfile();
                PopulateResourcesForProfile();
            }
        }
        private void lstProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            propProfile.SelectedObject = null;
            propProfile.SelectedObjects = null;

            lstAvailableResourcesForProfile.Items.Clear();
            lstAvailableResourcesForProfile.Enabled = false;
            lstProfileResources.Items.Clear();
            lstProfileResources.Enabled = false;

            if (lstProfiles.SelectedItems.Count == 1)
            {
                propProfile.SelectedObject = (Profile)lstProfiles.SelectedItems[0].Tag;
                PopulateAvailableResourcesForProfile();
                PopulateResourcesForProfile();
            }
            if (lstProfiles.SelectedItems.Count > 1)
            {
                Profile[] p = new Profile[lstProfiles.SelectedItems.Count];
                for (int tr = 0; tr < lstProfiles.SelectedItems.Count; tr++)
                    p[tr] = (Profile)lstProfiles.SelectedItems[tr].Tag;
                propProfile.SelectedObjects = p;
            }
        }
        private void propProfile_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            foreach (ListViewItem lvi in lstProfiles.SelectedItems)
                UpdateProfileItem(lvi);
        }
        private void OnAddResourcesToProfile(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a profile first !");
                return;
            }
            if (lstAvailableResourcesForProfile.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a resource to be added !");
                return;
            }
            Profile p = (Profile)lstProfiles.SelectedItems[0].Tag;
            foreach (ListViewItem lvi in lstAvailableResourcesForProfile.SelectedItems)
            {
                if (lvi.Tag.GetType() == typeof(List<GenericResource>))
                {
                    List<GenericResource> l = (List<GenericResource>)lvi.Tag;
                    foreach (GenericResource res in l)
                        p.AddResource(res);
                } else {
                    p.AddResource((GenericResource)lvi.Tag);
                }
            }                
            PopulateAvailableResourcesForProfile();
            PopulateResourcesForProfile();
            UpdateProfileItem(lstProfiles.SelectedItems[0]);
        }
        private void OnRemoveResourceFromProfile(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a profile first !");
                return;
            }
            if (lstProfiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a resource to be remove !");
                return;
            }
            Profile p = (Profile)lstProfiles.SelectedItems[0].Tag;
            foreach (ListViewItem lvi in lstProfileResources.SelectedItems)
            {
                if (lvi.Tag.GetType() == typeof(string))
                {
                    p.RemoveMissedResource(lvi.Tag.ToString(), lvi.Text);
                    continue;
                }
                if (lvi.Tag.GetType() == typeof(List<GenericResource>))
                {
                    List<GenericResource> l = (List<GenericResource>)lvi.Tag;
                    foreach (GenericResource res in l)
                        p.RemoveResource(res.GetType(), res.GetResourceVariableName());
                    continue;
                }
                p.RemoveResource(lvi.Tag.GetType(), lvi.Text);
                    
            }
            PopulateAvailableResourcesForProfile();
            PopulateResourcesForProfile();
            UpdateProfileItem(lstProfiles.SelectedItems[0]);
        }
        private void OnShowProfileWithLargeIcons(object sender, EventArgs e)
        {
            if (cbProfileWithLargeIcons.Checked)
            {
                lstProfileResources.View = View.Tile;
                lstAvailableResourcesForProfile.View = View.Tile;
            }
            else
            {
                lstProfileResources.View = View.Details;
                lstAvailableResourcesForProfile.View = View.Details;
            }
        }
        private void OnChangeProfileFilter(object sender, EventArgs e)
        {
            PopulateAvailableResourcesForProfile();
            PopulateResourcesForProfile();
        }
        private void OnShowTextureAtlasses(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a profile first !");
                return;
            }
            /*
            Profile p = (Profile)lstProfiles.SelectedItems[0].Tag;
            Dictionary<string, Bitmap> d = Context.prj.GetAvailableImages();
            List<Bitmap> lst = new List<Bitmap>();
            foreach (string k in p.Images)
            {
                if (d.ContainsKey(k) == false)
                {
                    MessageBox.Show("Image " + k + " is not loaded !");
                    return;
                }
                lst.Add(d[k]);
            }
            TextureAtlasesDialog dlg = new TextureAtlasesDialog(p,Context.prj,lst);
            dlg.ShowDialog();
             */
        }

        private void OnChangeAvailableFilterType(object sender, EventArgs e)
        {
            cbAnyProfile.Checked = (sender == cbAnyProfile);
            cbCurrentProfile.Checked = (sender == cbCurrentProfile);
            PopulateAvailableResourcesForProfile();
        }

        private void OnChangeGroupByVariableName(object sender, EventArgs e)
        {
            PopulateAvailableResourcesForProfile();
            PopulateResourcesForProfile();
        }
    }
}
