using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GAppCreator
{
    public partial class ProjectTabResources : BaseProjectContainer
    {
        private class UpdateResourceImageObject
        {
            public Bitmap bmp64, bmp16;
            public string ImageKey;
            public GenericResource R;
            public bool DefaultImage;

            public void Create(GenericResource r)
            {
                R = r;
                ImageKey = r.GetIconImageListKey();
                if (ImageKey.StartsWith("__") == false)
                {
                    bmp64 = R.CreateIcon(64, 64);
                    bmp16 = R.CreateIcon(16, 16);
                    DefaultImage = false;
                }
                else
                {
                    DefaultImage = true;
                }

            }
        };

        PreviewData pData = new PreviewData();
        Dictionary<string, PreviewControl> PreviewControls = new Dictionary<string, PreviewControl>();
        List<GenericResource> ResourcesToProcess = new List<GenericResource>();
        GListView lstResources;
        GenericResource lastSelectedResource = null;

        public ProjectTabResources()
        {
            InitializeComponent();

            // preview stuff
            PreviewControls["Image"] = new PreviewImage();
            PreviewControls["Font"] = new PreviewFont();
            PreviewControls["Shader"] = new PreviewShader();
            PreviewControls["Sound"] = new PreviewSound();
            foreach (string key in PreviewControls.Keys)
            {
                panelResourcePreview.Controls.Add(PreviewControls[key]);
                PreviewControls[key].Dock = DockStyle.Fill;
                PreviewControls[key].Visible = false;
            }

            InitResourceList();
        }
        private void InitResourceList()
        {
            lstResources = new GListView();
            lstResources.AddColumn("Name", "propMe", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstResources.SetColumnIconPropertyName(0, "propIcon");
            lstResources.AddColumn("Type", "propType", 100, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstResources.AddColumn("Builds", "propBuilds", 150, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstResources.AddColumn("Language", "propLanguage", 100, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstResources.AddColumn("Design Resolution", "propDesignResolution", 150, GListView.RenderType.Default, true, HorizontalAlignment.Center);
            lstResources.AddColumn("Properties", "propDescription", 150, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstResources.AddColumn("Source", "propSource", 300, GListView.RenderType.Default, true, HorizontalAlignment.Left);

            lstResources.AllColumns[0].GroupKeyGetter = delegate(object x) { return ((GenericResource)x).propType; };
            lstResources.AllColumns[0].AspectToStringConverter = delegate(object x) 
            {
                if (x.GetType() == typeof(string))
                    return (string)x;
                return ((GenericResource)x).propName; 
            };

 
            lstResources.UseColumnForTileView(2);
            lstResources.UseColumnForTileView(3);
            lstResources.UseColumnForTileView(5);

            lstResources.OnFilterObject += lstResources_OnFilterObject;
            lstResources.OnObjectsSelected += lstResources_OnObjectsSelected;
            lstResources.OnObjectDoubleClicked += lstResources_OnObjectDoubleClicked;
        
            lstResources.TileSize = new System.Drawing.Size(300, 64);

            lstResources.Dock = DockStyle.Fill;
            this.splitContainer2.Panel1.Controls.Add(this.lstResources);
        }

        void lstResources_OnObjectDoubleClicked(object source, object SelectedObject)
        {
            if (lstResources.GetCurrentSelectedObject()!=null)
            {
                GenericResource r = (GenericResource)lstResources.GetCurrentSelectedObject();
                if (r.GetType() == typeof(FontResource))
                {
                    FontEditor fontED = new FontEditor(Context.Prj, (FontResource)r);//, Context.SmallIcons);
                    fontED.ShowDialog();
                }
                if (r.GetType() == typeof(ShaderResource))
                {
                    ShaderEditor sed = new ShaderEditor(Context.Prj, (ShaderResource)r, Context.SmallIcons);
                    sed.ShowDialog();
                }
                if (r.GetType() == typeof(PresentationResource))
                {
                    // trebuei sa ii fac un load inainte ca sa isi ia ultimele poze/stringuri/etc
                    ((PresentationResource)r).anim.SetProject(Context.Prj, Context.Resources);
                    PresentationEditor aed = new PresentationEditor(Context, (PresentationResource)r);
                    aed.ShowDialog();
                }
                if (r.GetType() == typeof(ImageResource))
                {
                    switch (r.GetSourceType())
                    {
                        case "svg": Context.Prj.RunCommand(Context.Settings.InskapePath, r.GetSourceFullPath(), "Inskape", false, false); Context.Prj.ShowErrors(); break;
                        default: Context.Prj.RunCommand(Context.Settings.ImageEditorPath, r.GetSourceFullPath(), "Image Editor", false, false); Context.Prj.ShowErrors(); break;
                    }
                }
                if (r.GetType() == typeof(RawResource))
                {
                    ResourcePluginData plg = ((RawResource)r).GetPlugin();
                    if (plg != null)
                    {
                        string error = "";
                        if (plg.Interface.Edit(r.GetSourceFullPath(), out error) == false)
                        {
                            MessageBox.Show("Failed to edit current object !\nError:" + error);
                        }
                        else
                        {
                            // totul e ok - trebuie sa o reincarc
                            if (r.Load() == false)
                                Context.Prj.ShowErrors();
                        }

                    }
                }
                lstResources.RefreshSelectedObjects();
            }           
        }
        private void RefreshResourceList()
        {
            lstResources.SetObjects(Context.Prj.Resources);
        }
        void lstResources_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            lastSelectedResource = null;
            if (selected)
            {
                if ((SelectedObjects==null) || (SelectedObjects.Count==0))
                {
                    resProperties.SelectedObjects = null;
                    if (Context.Prj != null)
                        Context.LabelInfo.Text = "No Selection";
                    ShowPreview(null);
                    return;
                }
                if (Context.Prj != null)
                    Context.LabelGeneralInfo.Text = SelectedObjects.Count.ToString() + " selected / " + Context.Prj.Resources.Count.ToString();
                resProperties.SelectedObjects = lstResources.GetSelectedObjectsArray();
                if (SelectedObjects.Count == 1)
                    ShowPreview((GenericResource)lstResources.GetCurrentSelectedObject());
                else
                    ShowPreview(null);
            }
            else
            {
                resProperties.SelectedObjects = null;
                    if (Context.Prj != null)
                        Context.LabelInfo.Text = "No Selection";
                ShowPreview(null);
            }
        }

        bool lstResources_OnFilterObject(object obj)
        {
            GenericResource gr = (GenericResource)obj;
            if ((cbShowDerivedResources.Checked == false) && (gr.IsBaseResource()==false))
                return false;
            if ((gr.GetType() == typeof(ImageResource)) && (cbShowImages.Checked == false))
                return false;
            if ((gr.GetType() == typeof(FontResource)) && (cbShowFonts.Checked == false))
                return false;
            if ((gr.GetType() == typeof(RawResource)) && (cbShowRawResources.Checked == false))
                return false;
            if ((gr.GetType() == typeof(SoundResource)) && (cbShowMusicResources.Checked == false))
                return false;
            if ((gr.GetType() == typeof(ShaderResource)) && (cbShowShaders.Checked == false))
                return false;
            if ((gr.GetType() == typeof(PresentationResource)) && (cbShowPresentations.Checked == false))
                return false;
            if (txResourceFilter.Text.Length == 0)
                return true;
            if (btnContaining.Visible)
                return gr.IsFilteredBy(txResourceFilter.Text);
            else
                return !gr.IsFilteredBy(txResourceFilter.Text);
        }
        public override void OnSetContext()
        {
            lstResources.SmallImageList = Context.SmallIcons;
            lstResources.LargeImageList = Context.LargeIcons;  
        }
        public override void OnActivate()
        {
            lstResources.SetObjects(null);
            lstResources.SetObjects(Context.Prj.Resources);
            if (lastSelectedResource != null)
                lstResources.SelectObjectFromList(lastSelectedResource);
        }

        public void Task_BuildResources()
        {
            if (Context.IDESettings.EnableResourceTab)
            {
                Context.Task.SetMinMax(0, ResourcesToProcess.Count);
                Context.Task.CreateSubTask("Building resources ...");
                Context.Task.SendCommand(Command.DisableResourceList);
                UpdateResourceImageObject urio = new UpdateResourceImageObject();
                try
                {
                    // intai cele de baza
                    foreach (GenericResource r in ResourcesToProcess)
                    {
                        if (r.IsDeriveFromOtherResources() == true)
                            continue;
                        if (r.Build())
                        {
                            if (r.Load())
                            {
                                urio.Create(r);
                                Context.Task.SendCommand(Command.UpdateResourceIcon, urio);
                                Context.Task.SendCommand(Command.UpdatePreview, r);
                            }
                        }
                        Context.Task.IncrementProgress();
                    }
                    // apoi derivatele
                    foreach (GenericResource r in ResourcesToProcess)
                    {
                        if (r.IsDeriveFromOtherResources() == false)
                            continue;
                        if (r.Build())
                        {
                            if (r.Load())
                            {
                                urio.Create(r);
                                Context.Task.SendCommand(Command.UpdateResourceIcon, urio);
                                Context.Task.SendCommand(Command.UpdatePreview, r);
                            }
                        }
                        Context.Task.IncrementProgress();
                    }
                }
                catch (Exception e)
                {
                    Context.Prj.EC.AddException("Unable to create resource!", e);
                }
                Context.Task.UpdateSuccessErrorState(!Context.Prj.EC.HasErrors());
                Context.Task.SendCommand(Command.UpdateResourceList);
                Context.Task.SendCommand(Command.EnableResourceList);
            }
        }
        public void Task_LoadResources()
        {
            if (Context.IDESettings.EnableResourceTab)
            {
                Context.Task.SetMinMax(0, ResourcesToProcess.Count);
                Context.Task.CreateSubTask("Loading resources ...");
                Context.Task.SendCommand(Command.DisableResourceList);
                UpdateResourceImageObject urio = new UpdateResourceImageObject();
                foreach (GenericResource r in ResourcesToProcess)
                {
                    if (r.Load())
                    {
                        urio.Create(r);
                        Context.Task.SendCommand(Command.UpdateResourceIcon, urio);
                    }
                    Context.Task.IncrementProgress();
                }
                Context.Task.UpdateSuccessErrorState(!Context.Prj.EC.HasErrors());
                Context.Task.SendCommand(Command.UpdateResourceList);
                Context.Task.SendCommand(Command.EnableResourceList);
            }
        }
        private void LoadOneResource(GenericResource r)
        {
            if (Context.IDESettings.EnableResourceTab)
            {
                ResourcesToProcess.Clear();
                ResourcesToProcess.Add(r);
                Context.Task.Start(Task_LoadResources, "Load resources");
            }
        }
        public void InitDefaultResourceIcons()
        {
            Context.SmallIcons.Images.Clear();
            Context.LargeIcons.Images.Clear();
            foreach (string key in standardIconsSmall.Images.Keys)
                Context.SmallIcons.Images.Add(key, standardIconsSmall.Images[key]);
            foreach (string key in standardIconsLarge.Images.Keys)
                Context.LargeIcons.Images.Add(key, standardIconsLarge.Images[key]);
        }
        private void ShowPreview(GenericResource r)
        {
            lastSelectedResource = r;
            foreach (string key in PreviewControls.Keys)
            {
                PreviewControls[key].Visible = false;
            }
            if (r == null)
                return;
            r.GetPreviewData(pData);
            if (PreviewControls.ContainsKey(pData.PreviewDataType))
            {
                PreviewControls[pData.PreviewDataType].SetPreviewObject(Context.Prj, Context.SmallIcons, pData.Data);
                PreviewControls[pData.PreviewDataType].Visible = true;
            }
        }
        public void UpdatePluginPreview()
        {
            // sterg toate pluginurile existente
            List<string> toDelete = null;
            foreach (string key in PreviewControls.Keys)
            {
                // daca e de la un plugin
                if (key.StartsWith("_"))
                {
                    if (toDelete == null)
                        toDelete = new List<string>();
                    toDelete.Add(key);
                }
            }
            if (toDelete!=null)
            {
                foreach (string key in toDelete)
                {
                    panelResourcePreview.Controls.Remove(PreviewControls[key]);
                    PreviewControls.Remove(key);
                }
            }
            // adaug pluginurile noi
            foreach (string key in Context.Plugins.Plugins.Keys)
            {
                UserControl uc = Context.Plugins.Plugins[key].Interface.GetPreviewControl();
                if (uc!=null)
                {
                    PreviewPlugin plg = new PreviewPlugin(uc);
                    PreviewControls["_" + key + "_"] = plg;
                    panelResourcePreview.Controls.Add(plg);
                    plg.Visible = false;
                    plg.Dock = DockStyle.Fill;
                }
            }
        }


        private void OnCheckResourceIntegrity(object sender, EventArgs e)
        {
            if (Context.Prj != null)
            {
                if (Context.Prj.CheckResourcesIntegrity() == false)
                    Context.Prj.ShowErrors();
                else
                    MessageBox.Show("All resources are set coorectly !");
            }
        }
        private void OnChangeViewResourceTypes(object sender, EventArgs e)
        {
            lstResources.Refilter();
        }
        private void OnShowDerivedResources(object sender, EventArgs e)
        {
            lstResources.Refilter();
        }

        private void AddExternalResource(ResourceType rt)
        {
            if (Context.Prj == null)
                return;
            ResourceLoadDialog rld = new ResourceLoadDialog(Context.Prj.GetProjectResourceSourceFolder(), Context.Prj);
            rld.EnableResourceType(rt);
            if (rld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Context.Prj.AddResources(rld.SelectedFiles, ResourcesToProcess);
                RefreshResourceList();
                Context.Task.Start(Task_BuildResources, "Load resources");
            }
        }
        private void OnAddNewRasterImageResource(object sender, EventArgs e)
        {
            AddExternalResource(ResourceType.RasterImage);
        }

        private void OnAddNewVectorImageResource(object sender, EventArgs e)
        {
            AddExternalResource(ResourceType.VectorImage);
        }

        private void OnAddNewMusicResource(object sender, EventArgs e)
        {
            AddExternalResource(ResourceType.Music);
        }

        private void OnAddNewRawResource(object sender, EventArgs e)
        {
            AddExternalResource(ResourceType.Raw);
        }

        private void OnChangeViewMode(object sender, EventArgs e)
        {
            cbTilesViewMode.Checked = sender == cbTilesViewMode;
            cbListViewMode.Checked = sender == cbListViewMode;
            cbTreeViewMode.Checked = sender == cbTreeViewMode;

            if (sender == cbListViewMode)
            {
                lstResources.View = View.Details;
            }
            if (sender == cbTilesViewMode)
            {
                lstResources.View = View.Tile;
            }
        }



        private void OnChangeResourceProperties(object s, PropertyValueChangedEventArgs e)
        {
            lstResources.RefreshSelectedObjects();
        }


        private void OnResourceTextFilterChanged(object sender, EventArgs e)
        {
            lstResources.Refilter();
        }


        private void OnAddNewShader(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter shader name", "NewShader");
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ShaderResource s = new ShaderResource();
                s.Create();
                s.Name = ib.StringResult.Replace(" ", "");
                Context.Prj.AddResource(s);
                RefreshResourceList();
                LoadOneResource(s);
            }
        }

        private void OnAddAnimation(object sender, EventArgs e)
        {
            AddExternalResource(ResourceType.Presentation);
        }

        private void OnCreateNewGlyphFont(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter font name", "NewFont");
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FontResource t = new FontResource();
                t.Create(Context.Prj);
                t.Name = ib.StringResult.Replace(" ", "");
                Context.Prj.AddResource(t);
                RefreshResourceList();
                LoadOneResource(t);
            }
        }

        private void OnDeleteResources(object sender, EventArgs e)
        {
            if (lstResources.GetCurrentSelectedObjectsListCount()>0)
            {
                if (MessageBox.Show("Delete " + lstResources.GetCurrentSelectedObjectsListCount().ToString() + " items ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    object[] toDelete = lstResources.GetSelectedObjectsArray();
                    foreach (object r in toDelete)
                    {
                        Context.Prj.Resources.Remove((GenericResource)r);
                    }
                    // caut imagini in profile si le sterg
                    foreach (Profile p in Context.Prj.Profiles)
                    {
                        foreach (object gr in toDelete)
                        {
                            GenericResource r = ((GenericResource)gr);
                            p.RemoveResource(r.GetType(), r.GetResourceVariableName());
                        }
                    }
                    RefreshResourceList();

                }
            }
        }
        private void OnReloadAll(object sender, EventArgs e)
        {
            ResourcesToProcess.Clear();
            foreach (GenericResource r in Context.Prj.Resources)
                ResourcesToProcess.Add(r);
            Context.Task.Start(Task_LoadResources, "Load resources");
        }

        private void OnReloadSelected(object sender, EventArgs e)
        {
            ResourcesToProcess.Clear();
            if (lstResources.GetCurrentSelectedObjectsListCount() == 0)
                return;
            foreach (object o in lstResources.GetCurrentSelectedObjectsList())
                ResourcesToProcess.Add((GenericResource)o);
            Context.Task.Start(Task_LoadResources, "Load resources");
        }
        private void OnBuildAll(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure ?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            ResourcesToProcess.Clear();
            foreach (GenericResource r in Context.Prj.Resources)
                ResourcesToProcess.Add(r);
            Context.Task.Start(Task_BuildResources, "Build resources");
        }

        private void OnBuildSelected(object sender, EventArgs e)
        {
            ResourcesToProcess.Clear();
            if (lstResources.GetCurrentSelectedObjectsListCount() == 0)
                return;
            foreach (object o in lstResources.GetCurrentSelectedObjectsList())
                ResourcesToProcess.Add((GenericResource)o);
            Context.Task.Start(Task_BuildResources, "Build resources");
        }

        private void OnBuildResourcesThatWereNotBuild(object sender, EventArgs e)
        {
            ResourcesToProcess.Clear();
            foreach (GenericResource r in Context.Prj.Resources)
                if (r.IsLoaded()==false)
                    ResourcesToProcess.Add(r);
            if (ResourcesToProcess.Count == 0)
                MessageBox.Show("All resources have been load ! There is no need to build any of them !");
            else
                Context.Task.Start(Task_BuildResources, "Build resources");
        }
        private void OnGenerateImagesForDifferentResolutions(object sender, EventArgs e)
        {
            List<ImageResource> i = new List<ImageResource>();
            if (lstResources.GetCurrentSelectedObjectsListCount() == 0)
                return;
            foreach (object o in lstResources.GetCurrentSelectedObjectsList())
            {
                GenericResource r = (GenericResource)o;
                if (r.IsBaseResource() == false)
                    continue;
                if (r.GetType() == typeof(ImageResource))
                    i.Add((ImageResource)r);
            }
            CreateImagesResourceForDifferentResolutionsDialog dlg = new CreateImagesResourceForDifferentResolutionsDialog(i, Context.Prj, Context.SmallIcons);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ResourcesToProcess.Clear();
                foreach (ImageResource img in dlg.NewGeneratedImages)
                {
                    ResourcesToProcess.Add(img);
                }
                Context.Prj.AddResources(ResourcesToProcess);
                RefreshResourceList();
                Context.Task.Start(Task_BuildResources, "Load resources");
            }
        }

        public override void OnOpenNewProject(bool newProject)
        {
            RefreshResourceList();
            ResourcesToProcess.Clear();
            foreach (GenericResource r in Context.Prj.Resources)
                ResourcesToProcess.Add(r);
            InitDefaultResourceIcons();
        }
        public override void OnCommand(Command cmd, object parameters = null)
        {

            switch (cmd)
            {
                    case Command.DisableResourceList: lstResources.Enabled = false; break;
                    case Command.EnableResourceList: lstResources.Enabled = true; break;
                    case Command.UpdatePreview:
                        if (lstResources.GetCurrentSelectedObject()!=null)
                        {
                            if ((GenericResource)lstResources.GetCurrentSelectedObject() == (GenericResource)((BackgroundTaskAction)parameters).CommandParam)
                            {
                                lstResources_OnObjectsSelected(null, true, lstResources.GetCurrentSelectedObjectsList());
                            }
                        }
                        break;
                    case Command.UpdateResourceIcon:
                        UpdateResourceImageObject urio = (UpdateResourceImageObject)((BackgroundTaskAction)parameters).CommandParam;
                        if (urio.DefaultImage == false)
                        {
                            if (Context.LargeIcons.Images.ContainsKey(urio.ImageKey))
                                Context.LargeIcons.Images.RemoveByKey(urio.ImageKey);
                            if (Context.SmallIcons.Images.ContainsKey(urio.ImageKey))
                                Context.SmallIcons.Images.RemoveByKey(urio.ImageKey);

                            if (urio.bmp64 != null)
                            {
                                Context.LargeIcons.Images.Add(urio.ImageKey, urio.bmp64);
                            }
                            if (urio.bmp16 != null)
                            {
                                Context.SmallIcons.Images.Add(urio.ImageKey, urio.bmp16);
                            }
                        }
                        break;
                    case Command.UpdateResourceList:
                        //RefreshResourceList();
                        break;
                    case Command.BuildSelectedResources:
                        OnBuildSelected(null, null);
                        break;
                    case Command.Delete:
                        OnDeleteResources(null, null);
                        break;
            }
        }

        private void OnAddNewPluginObject(object sender, EventArgs e)
        {
            if (Context.Plugins.Plugins.Count==0)
            {
                MessageBox.Show("No plugins defined for this project !");
                return;
            }
            NewPluginObjectDialog dlg = new NewPluginObjectDialog(Context.Prj,Context.Plugins);
            if (dlg.ShowDialog()== DialogResult.OK)
            {
                ResourcesToProcess.Clear();
                foreach (string resName in dlg.PluginSourceNames)
                {
                    string sourcePath = Context.Prj.GetProjectResourceSourceFullPath(resName);
                    if (File.Exists(sourcePath))
                    {
                        if (MessageBox.Show("Resource " + resName + " already exists !. Override ?", "Override", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            ResourcesToProcess.Clear();
                            return;
                        }
                        if (Disk.DeleteFile(sourcePath, Context.Prj.EC) == false)
                        {
                            Context.Prj.ShowErrors();
                            ResourcesToProcess.Clear();
                            return;
                        }
                    }
                    string error = "";
                    if (dlg.Plugin.Interface.New(sourcePath, out error) == false)
                    {
                        MessageBox.Show("Unable to create resource file: " + resName + "\nError:" + error);
                        ResourcesToProcess.Clear();
                        return;
                    }
                    // all is good - adaug si resursa
                    RawResource r = new RawResource();
                    r.Create(resName);
                    Context.Prj.AddResource(r);
                    ResourcesToProcess.Add(r);
                }
                RefreshResourceList();
                Context.Task.Start(Task_LoadResources, "Load resources");
            }
        }

        private void OnFilterContainingString(object sender, EventArgs e)
        {
            btnNOTContaining.Visible = true;
            btnContaining.Visible = false;
            OnResourceTextFilterChanged(null, null);
        }

        private void OnFilterNOTContainingString(object sender, EventArgs e)
        {
            btnNOTContaining.Visible = false;
            btnContaining.Visible = true;
            OnResourceTextFilterChanged(null, null);
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }





    }
}
