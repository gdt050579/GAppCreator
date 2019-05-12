using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ResourceMaker
{
    public partial class Form1 : Form
    {
        Project prj = new Project();
        PreviewData pData = new PreviewData();
        Dictionary<string, UserControl> PreviewControls = new Dictionary<string, UserControl>();

        #region Templates
        static string ResourceHeader = @"
#include ""GApp.h""

struct ResourcesImages
{
$$IMAGE_DEFINITIONS$$
};
struct ResourcesTextures
{
$$TEXTURE_DEFINITIONS$$
};
struct ResourcesFonts
{
$$FONT_DEFINITIONS$$
};
class Resources: public GApp::UI::ResourceData
{
public:
    ResourcesImages     Images;
    ResourcesTextures   Textures;
    ResourcesFonts      Fonts;

    bool	            Init(void *Context);    
};
";

        static string ResourceCpp = @"
#include ""Resources.h""

bool Resources::Init(void *Context)
{
// Image Init code
$$IMAGE_INIT_CODE$$

// Texture init code
$$TEXTURE_INIT_CODE$$

// Font init code
$$FONT_INIT_CODE$$


    return true;
}

";
        #endregion

        public Form1()
        {
            InitializeComponent();
            PreviewControls["Image"] = new PreviewImage();
            foreach (string key in PreviewControls.Keys)
            {
                panelResourcePreview.Controls.Add(PreviewControls[key]);
                PreviewControls[key].Dock = DockStyle.Fill;
                PreviewControls[key].Visible = false;
                PreviewControls[key].Tag = null;
            }


            prj = Project.Load("E:\\a\\capture\\resources.resxml");
            //prj.ProjectPath = @"E:\a\capture";

            UpdateResourceListIcons();
            UpdateResourceList();
            
        }

        #region Resource TAB
        private void ShowPreview(GenericResource r)
        {
            foreach (string key in PreviewControls.Keys)
            {
                PreviewControls[key].Visible = false;
                PreviewControls[key].Tag = null;
            }
            if (r == null)
                return;
            r.GetPreviewData(prj, pData);
            if (PreviewControls.ContainsKey(pData.PreviewDataType))
            {
                PreviewControls[pData.PreviewDataType].Tag = pData.Data;
                PreviewControls[pData.PreviewDataType].Visible = true;
            }
        }
        private void UpdateResourceListIcons(GenericResource r)
        {
            Bitmap bmp;
            bmp = r.CreateIcon(prj, 64, 64);
            if (bmp != null)
                resImagesLarge.Images.Add(r.GetVariableName(), bmp);
            bmp = r.CreateIcon(prj, 16, 16);
            if (bmp != null)
                resImagesSmall.Images.Add(r.GetVariableName(), bmp);
        }
        private void UpdateResourceListIcons()
        {
            resImagesLarge.Images.Clear();
            resImagesSmall.Images.Clear();
            foreach (GenericResource r in prj.Resources)
            {
                UpdateResourceListIcons(r);
            }
            prj.ShowErrors();
        }
        private void UpdateResourceItem(int index)
        {
            ListViewItem lvi = resList.Items[index];
            GenericResource r = prj.Resources[(int)lvi.Tag];
            lvi.Text = r.GetVariableName();
            lvi.SubItems[1].Text = r.Type;
            lvi.SubItems[2].Text = r.GetResourceInformation();
            lvi.SubItems[3].Text = r.Source;
            
            if (lvi.ImageKey != r.GetVariableName())
            {
                UpdateResourceListIcons(r);
                lvi.ImageKey = r.GetVariableName();                
            }

        }
        private void UpdateResourceList()
        {
            resProperties.SelectedObjects = null;
            resProperties.SelectedObject = null;
            resList.Items.Clear();
            string tx = txResourceFilter.Text.ToLower();
            for (int tr=0;tr<prj.Resources.Count;tr++)
            {
                GenericResource r = prj.Resources[tr];

                if ((r.GetType() == typeof(ImageResource)) && (cbShowImages.Checked == false))
                    continue;
                if ((r.GetType() == typeof(GlyphFontResource)) && (cbShowGlyphFonts.Checked == false))
                    continue;
                if ((r.GetType() == typeof(TextureAtlasResource)) && (cbShowTextureAtlases.Checked == false))
                    continue;

                if (r.IsFilteredBy(tx)==false)
                    continue;
                ListViewItem lvi = new ListViewItem();
                for (int gr=0;gr<resList.Columns.Count;gr++)
                    lvi.SubItems.Add("");
                lvi.Tag = (int)tr;
                resList.Items.Add(lvi);
                UpdateResourceItem(resList.Items.Count-1);
            }
        }

        private void OnChangeViewResourceTypes(object sender, EventArgs e)
        {
            UpdateResourceList();
        }

        private void OnAddNewResource(object sender, EventArgs e)
        {
            ResourceLoadDialog rld = new ResourceLoadDialog(Path.Combine(prj.ProjectPath,"Resources","Sources"));
            if (rld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                prj.AddResources(rld.SelectedFiles);
                UpdateResourceListIcons();
                UpdateResourceList();
            }
        }

        private void OnViewListMode(object sender, EventArgs e)
        {
            cbListViewMode.Checked = true;
            cbTilesViewMode.Checked = false;
            resList.View = View.Details;
        }

        private void OnViewTilesMode(object sender, EventArgs e)
        {
            cbListViewMode.Checked = false;
            cbTilesViewMode.Checked = true;
            resList.View = View.Tile;
        }

        private void OnResourceListSelectionChange(object sender, EventArgs e)
        {
            if (resList.SelectedIndices.Count == 0)
            {
                lbInfo.Text = "No Selection";
                ShowPreview(null);
            }
            else
                lbInfo.Text = resList.SelectedIndices.Count.ToString() + " selected vars !";
            if (resList.SelectedItems.Count == 1)
            {
                resProperties.SelectedObject = prj.Resources[(int)resList.SelectedItems[0].Tag];
                ShowPreview(prj.Resources[(int)resList.SelectedItems[0].Tag]);
                //if (extraInfoHandler != null)
                //    extraInfoHandler.SetResourceObject(vars.Get((int)resList.SelectedItems[0].Tag));
                return;
            }
            //if (extraInfoHandler != null)
            //    extraInfoHandler.SetResourceObject(null);
            if (resList.SelectedItems.Count > 1)
            {
                GenericResource[] robj = new GenericResource[resList.SelectedItems.Count];
                for (int tr = 0; tr < resList.SelectedItems.Count; tr++)
                    robj[tr] = prj.Resources[(int)resList.SelectedItems[tr].Tag];
                resProperties.SelectedObjects = robj;
                ShowPreview(null);
                return;
            }
            resProperties.SelectedObjects = null;
            resProperties.SelectedObject = null;
            ShowPreview(null);
        }

        private void OnChangeResourceProperties(object s, PropertyValueChangedEventArgs e)
        {
            // updatez doar selectia
            for (int tr = 0; tr < resList.SelectedIndices.Count; tr++)
                UpdateResourceItem(resList.SelectedIndices[tr]);

        }

        private void OnReloadResourcesIcons(object sender, EventArgs e)
        {
            UpdateResourceListIcons();
        }

        private void OnSaveProject(object sender, EventArgs e)
        {
            prj.Save();
            prj.ShowErrors();
        }

        private void OnResourceTextFilterChanged(object sender, EventArgs e)
        {
            UpdateResourceList();
        }
        

        private void OnAddNewTextureAtlas(object sender, EventArgs e)
        {
            TextureAtlasResource t = new TextureAtlasResource();
            t.Create();
            prj.Resources.Add(t);
            UpdateResourceListIcons();
            UpdateResourceList();
            
        }

        private void OnCreateNewGlyphFont(object sender, EventArgs e)
        {
            GlyphFontResource t = new GlyphFontResource();
            t.Create();
            prj.Resources.Add(t);
            UpdateResourceListIcons();
            UpdateResourceList();
        }

        private void OnEditResource(object sender, MouseEventArgs e)
        {
            if ((resList.SelectedItems != null) && (resList.SelectedItems.Count == 1))
            {
                GenericResource r = prj.Resources[(int)resList.SelectedItems[0].Tag];
                if (r.GetType() == typeof(TextureAtlasResource))
                {
                    TextureEditor ted = new TextureEditor(prj, (TextureAtlasResource)r, resImagesSmall);
                    ted.ShowDialog();
                }
                if (r.GetType() == typeof(GlyphFontResource))
                {
                    GlyphFontEditor ged = new GlyphFontEditor(prj, (GlyphFontResource)r, resImagesSmall);
                    ged.ShowDialog();
                }
                UpdateResourceItem(resList.SelectedIndices[0]);
            }
        }

        private void OnDeleteResources(object sender, EventArgs e)
        {
            if ((resList.SelectedItems != null) && (resList.SelectedItems.Count > 0))
            {
                if (MessageBox.Show("Delete " + resList.SelectedItems.Count.ToString() + " items ?") == System.Windows.Forms.DialogResult.OK)
                {
                    List<GenericResource> toDelete = new List<GenericResource>();
                    foreach (ListViewItem lvi in resList.SelectedItems)
                    {
                        toDelete.Add(prj.Resources[(int)lvi.Tag]);
                    }
                    foreach (GenericResource r in toDelete)
                    {
                        prj.Resources.Remove(r);
                    }
                    UpdateResourceList();

                }
            }
        }

        #endregion

        #region General Tab
        private string GenerateHeaderDefinitions(string varType, Type[] types)
        {
            string s = "";
            ArrayCounter ac = new ArrayCounter();
            foreach (Type t in types)
                ac.Add(prj.Resources, t);            

            foreach (string varName in ac.Variables)
            {
                s += string.Format("\t{0} {1};\n",varType,ac.GetVariableName(varName));
            }
            return s;
        }
        private string GenerateHeaderDefinitions(string varType, Type t)
        {
            return GenerateHeaderDefinitions(varType, new Type[] { t });
        }

        private string GenerateSourceInitCode(Type[] types)
        {
            string s = "";
            foreach (GenericResource r in prj.Resources)
            {
                foreach (Type t in types)
                {
                    if (r.GetType() == t)
                    {
                        s += "\t" + r.CreateInitializationCode(prj) + "\n";
                    }
                }
            }
            return s;
        }
        private string GenerateSourceInitCode(Type t)
        {
            return GenerateSourceInitCode(new Type[] { t });
        }

        private bool GenerateResourceCodeFiles()
        {
            string h = ResourceHeader;
            string cpp = ResourceCpp;

            // Imagini
            h = h.Replace("$$IMAGE_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Graphics::Bitmap",new Type[]{typeof(ImageResource)}));
            cpp = cpp.Replace("$$IMAGE_INIT_CODE$$", GenerateSourceInitCode(new Type[] {typeof(ImageResource)}));

            // Texturi
            h = h.Replace("$$TEXTURE_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Graphics::Texture", typeof(TextureAtlasResource)));
            cpp = cpp.Replace("$$TEXTURE_INIT_CODE$$", GenerateSourceInitCode(typeof(TextureAtlasResource)));

            // Fonturi
            h = h.Replace("$$FONT_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Graphics::Font", typeof(GlyphFontResource)));
            cpp = cpp.Replace("$$FONT_INIT_CODE$$", GenerateSourceInitCode(typeof(GlyphFontResource)));

            Disk.SaveFile(prj.ProjectPath + "\\Resources.h", h, prj.EC);
            Disk.SaveFile(prj.ProjectPath + "\\Resources.cpp", cpp, prj.EC);
            return prj.EC.HasErrors();
        }
        

        private bool GenerateResources()
        {
            prj.EC.Reset();
            long pos = 0;

            try
            {

                FileStream f = File.Open(prj.GetResourceFileName(), FileMode.Create);
                foreach (GenericResource g in prj.Resources)
                {
                    bool hasContent = false;
                    byte[] buf = g.GetContent(prj, ref hasContent);
                    if ((buf == null) && (hasContent))
                    {
                        prj.EC.AddError("GenerateResource", "GetContent returned NULL for " + g.GetVariableName() + " [" + g.Type + "]");
                        continue;
                    }
                    if (buf != null)
                    {
                        f.Write(buf, 0, buf.Length);
                        g.ResourceFilePosition = pos;
                        g.ResourceFileSize = buf.Length;
                        pos += buf.Length;
                    }
                }
                f.Close();
                return !prj.EC.HasErrors();
            }
            catch (Exception e)
            {
                prj.EC.AddException("GenerateResources", e.ToString(), e);
                return false;
            }
        }

        private void OnCreateDeveloperFiles(object sender, EventArgs e)
        {
            //GenerateResourceCodeFiles();
            //*
            while (true)
            {
                if (GenerateResources() == false)
                    break;
                if (GenerateResourceCodeFiles() == false)
                    break;
                return;
            }
             //*/
            prj.ShowErrors();
        }

        #endregion






    }
}
