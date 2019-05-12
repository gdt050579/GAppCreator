using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class CreateImagesResourceForDifferentResolutionsDialog : Form
    {
        private Project prj;
        private bool Landscape;
        public List<ImageResource> NewGeneratedImages = new List<ImageResource>();

        public CreateImagesResourceForDifferentResolutionsDialog(List<ImageResource> sources, Project p, ImageList smallImageList)
        {
            prj = p;
            InitializeComponent();
            lstImages.SmallImageList = smallImageList;

            Size size1 = Project.SizeToValues(prj.DesignResolution);
            Landscape = size1.Width >= size1.Height;
            foreach (Size size2 in Project.Resolutions)
            {
                lstSettings.Items.Add(new ListViewItem()
                {
                    Group = lstSettings.Groups["Resolutions"],
                    Text = !Landscape ? string.Format("{1} x {0}", (object)size2.Width, (object)size2.Height) : string.Format("{0} x {1}", (object)size2.Width, (object)size2.Height)
                });
            }
            lstSettings.Items.Add(new ListViewItem("Keep the same builds as the ones from the original images")
            {
                Group = lstSettings.Groups["Builds"],
                Checked = true,          
                Tag = "Keep"
            });

            foreach (GenericBuildConfiguration gb in prj.BuildConfigurations)
            {
                lstSettings.Items.Add(new ListViewItem(gb.Name)
                {
                    Group = lstSettings.Groups["Builds"],
                    Tag = "Build"
                });
            }
            foreach (ImageResource img in sources)
                AddImageResource(img);
            if (sources.Count == 0)
                OnAddAllResources((object) null, (EventArgs) null);
        }

        private void AddImageResource(ImageResource i)
        {
            ListViewItem listViewItem = new ListViewItem(i.GetResourceVariableName());
            Size size = Project.SizeToValues(i.DesignResolution);
            listViewItem.SubItems.Add(string.Format("{0} x {1}", size.Width, size.Height));
            listViewItem.SubItems.Add(i.Lang.ToString());
            listViewItem.Tag = i;
            listViewItem.Checked = true;
            listViewItem.ImageKey = i.GetIconImageListKey();
            lstImages.Items.Add(listViewItem);
        }

        private void AddResolution(int w, int h)
        {
            // verific daca rezolutia exista deja
            string s = string.Format("{0} x {1}", w, h);
            foreach (ListViewItem lvi in lstSettings.Items)
                if (lvi.Text == s)
                    return;
            lstSettings.Items.Add(new ListViewItem()
            {
                Group = lstSettings.Groups["Resolutions"],
                Text = s
            });
        }

        private void resolution_Click(object sender, EventArgs e)
        {
            InputBox inputBox = new InputBox("Enter a custom resolution (Ex: 456x868)", (string) prj.DesignResolution);
            if (inputBox.ShowDialog() != DialogResult.OK)
                return;
            Size size = Project.SizeToValues(inputBox.StringResult);
            if (size.Width <= 1 || size.Height <= 1)
            {
                MessageBox.Show("Invalid resolution (both with and height should be bigger than 1)");
                return;
            }
            else
            {
                if (Landscape && size.Width < size.Height && MessageBox.Show("Your project is design in a landscape mode. The resolution you have typed in seems to be for portrait mode. Continue ?", "Invalid resolution", MessageBoxButtons.YesNo) != DialogResult.Yes || !Landscape && size.Width > size.Height && MessageBox.Show("Your project is design in a portrait mode. The resolution you have typed in seems to be for landscape mode. Continue ?", "Invalid resolution", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                AddResolution(size.Width, size.Height);
            }
        }


        private void OnCreateNewResources(object sender, EventArgs e)
        {
            List<Size> sizes = new List<Size>();
            string builds = "";
            foreach (ListViewItem lvi in lstSettings.Items)
            {
                if (lvi.Checked == false)
                    continue;
                if (lvi.Group == lstSettings.Groups["Resolutions"])
                {                    
                    sizes.Add(Project.SizeToValues(lvi.Text));
                    continue;
                }
                if (lvi.Group == lstSettings.Groups["Builds"])
                {
                    if (lvi.Tag.ToString() == "Keep")
                    {
                        builds = null;
                        continue;
                    }
                    if (builds != null)
                        builds += lvi.Text + " , ";
                }
            }
            if (sizes.Count==0)
            {
                MessageBox.Show("Please select at least one resolution for image generation !");
                return;
            }
            if ((builds != null) && (builds.EndsWith(" , ")))
                builds = builds.Substring(0, builds.Length - 3);
            NewGeneratedImages.Clear();
            Dictionary<string,bool> d = new Dictionary<string,bool>();
            foreach (GenericResource r in prj.Resources)
                d[r.GetResourceUniqueKey()] = true;
            foreach (ListViewItem lvi in lstImages.Items)
            {
                if (lvi.Checked == false)
                    continue;
                foreach (Size sz in sizes)
                {
                    ImageResource newImg = new ImageResource();
                    newImg.DuplicateFrom((ImageResource)lvi.Tag);
                    newImg.DesignResolution = string.Format("{0} x {1}",sz.Width,sz.Height);
                    if (builds!=null)
                        newImg.Builds = builds;
                    newImg.Scale *= Project.GetResolutionScale(prj.DesignResolutionSize.Width,prj.DesignResolutionSize.Height,sz.Width,sz.Height);
                    if (d.ContainsKey(newImg.GetResourceUniqueKey()))
                    {
                        MessageBox.Show(string.Format("Image {0} for resolution {1} and language {2} already exists !",newImg.GetResourceVariableName(),newImg.DesignResolution,newImg.Lang));
                        return;
                    }
                    NewGeneratedImages.Add(newImg);
                }
            }
            if (NewGeneratedImages.Count == 0)
            {
                MessageBox.Show("No images were generate. Please check some images to be used for resolution generation !");
                return;
            }
            // all good
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnCheckAll(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in lstImages.Items)
                listViewItem.Checked = true;
        }

        private void OnCLearAll(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in lstImages.Items)
                listViewItem.Checked = false;
        }

        private void OnAddAllResources(object sender, EventArgs e)
        {
            lstImages.Items.Clear();
            foreach (GenericResource r in prj.Resources)
            {
                if (r.IsBaseResource() == false)
                    continue;
                if (r.GetType() != typeof(ImageResource))
                    continue;
                AddImageResource((ImageResource)r);
            }
        }

        private void lstSettings_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Group != lstSettings.Groups["Builds"])
                return;
            lstSettings.BeginUpdate();

            if (e.Item.Tag.ToString()=="Keep")
            {
                if (e.Item.Checked)
                {
                    foreach (ListViewItem lvi in lstSettings.Items)
                    {
                        if (lvi.Group != lstSettings.Groups["Builds"])
                            continue;
                        if (lvi.Tag.ToString() == "Build")
                        {
                            lvi.Checked = false;
                            lvi.ForeColor = Color.Gray;
                        }
                    }
                }
                else
                {
                    foreach (ListViewItem lvi in lstSettings.Items)
                    {
                        if (lvi.Group != lstSettings.Groups["Builds"])
                            continue;
                        if (lvi.Tag.ToString() == "Build")
                        {
                            lvi.ForeColor = Color.Black;
                        }
                    }
                }
            }
            if ((e.Item.Tag.ToString() == "Build") && (e.Item.Checked))
            {
                foreach (ListViewItem lvi in lstSettings.Items)
                {
                    if (lvi.Group != lstSettings.Groups["Builds"])
                        continue;
                    if (lvi.Tag.ToString() == "Keep")
                        lvi.Checked = false;
                }
            }

            lstSettings.EndUpdate();
        }

    
    }
}
