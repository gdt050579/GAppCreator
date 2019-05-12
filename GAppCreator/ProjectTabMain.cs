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
    public partial class ProjectTabMain : BaseProjectContainer
    {
        public ProjectTabMain()
        {
            InitializeComponent();

            // grupuri taskuri
            lstProjectTasks.Groups.Add("bug", "Bugs");
            lstProjectTasks.Groups.Add("todo", "ToDo");
            lstProjectTasks.Groups.Add("nextupdate", "Next Update");
            lstProjectTasks.Groups.Add("feature", "Features");
            lstProjectTasks.Groups.Add("idea", "Ideea");
            // mereu ultimul
            lstProjectTasks.Groups.Add("completed", "Completed");

            btnServerBuild.Enabled = (comboBuildNames.SelectedIndex > 0);

            // adaug limbile default in lista de limbi
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                ListViewItem lvi = new ListViewItem(l.ToString());
                lvi.Tag = l;
                lvi.SubItems.Add("");
                lstAppName.Items.Add(lvi);

            }

            lbIconInfo.Dock = DockStyle.Fill;
            iconPicture.Dock = DockStyle.Fill;
            iconPicture.Visible = false;
            lbIconInfo.Visible = true;
        }

        #region Project Tasks
        private void comboHideCompletedTasks_Click(object sender, EventArgs e)
        {
            PopulateProjectTasks();
        }
        private void PopulateProjectTasks()
        {
            if (Context.Prj == null)
                return;
            lstProjectTasks.Items.Clear();
            foreach (ProjectTask t in Context.Prj.Tasks)
            {
                if (t.CompletedOn == null)
                    t.CompletedOn = "";
                if ((comboHideCompletedTasks.Checked) && (t.CompletedOn.Trim().Length > 0))
                    continue;
                ListViewItem lvi = new ListViewItem(t.Type.ToString());
                lvi.Tag = t;
                lvi.SubItems.Add(t.AddedOn);
                lvi.SubItems.Add(t.CompletedOn);
                lvi.SubItems.Add(t.Text);
                lvi.ToolTipText = t.Text;

                if (t.Type == TaskType.Bug)
                    lvi.ForeColor = Color.Red;
                if (t.CompletedOn.Trim().Length > 0)
                {
                    lvi.Group = lstProjectTasks.Groups["completed"];
                    lvi.ForeColor = Color.Gray;
                }
                else
                {
                    switch (t.Type)
                    {
                        case TaskType.Bug:
                            lvi.Group = lstProjectTasks.Groups["bug"];
                            break;
                        case TaskType.ToDo:
                            lvi.Group = lstProjectTasks.Groups["todo"];
                            break;
                        case TaskType.Feature:
                            lvi.Group = lstProjectTasks.Groups["feature"];
                            break;
                        case TaskType.Ideea:
                            lvi.Group = lstProjectTasks.Groups["idea"];
                            break;
                        case TaskType.NextUpdate:
                            lvi.Group = lstProjectTasks.Groups["nextupdate"];
                            break;
                    }
                }
                lstProjectTasks.Items.Add(lvi);
            }
        }
        private void OnAddNewTask(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            ProjectTask t = new ProjectTask();
            t.SetNow(true);
            TaskDialog td = new TaskDialog(t, true);
            if (td.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Context.Prj.Tasks.Insert(0, t);
                PopulateProjectTasks();
            }
        }
        private void OnEditProjectTask(object sender, MouseEventArgs e)
        {
            if (lstProjectTasks.SelectedItems.Count != 1)
                return;
            ProjectTask t = (ProjectTask)lstProjectTasks.SelectedItems[0].Tag;
            TaskDialog td = new TaskDialog(t, false);
            if (td.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PopulateProjectTasks();
        }
        private void OnDeleteSelectedProjectTask(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (lstProjectTasks.SelectedItems.Count != 1)
            {
                MessageBox.Show("You have to select a project tasj for deletion first");
                return;
            }
            ProjectTask t = (ProjectTask)lstProjectTasks.SelectedItems[0].Tag;
            if (MessageBox.Show("Delete Context.Task:\n" + t.Text, "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Context.Prj.Tasks.Remove(t);
                PopulateProjectTasks();
            }

        }
        #endregion

        #region Splash Screen
        private void UpdateSplashScrrenPicture()
        {
            if (Context.Prj != null)
            {
                Context.Prj.SplashScreen.OverrideDefaultOutputName("logo.png");
                Context.Prj.SplashScreen.Load();
                pnlSplashScreen.Invalidate();
            }
        }
        public void Task_BuildSplashScreen()
        {
            Context.Task.CreateSubTask("Building Splash Screen Image");
            Context.Prj.SplashScreen.DPI = 90;
            Context.Prj.SplashScreen.OverrideDefaultOutputName("logo.png");
            Context.Task.UpdateSuccessErrorState(Context.Prj.SplashScreen.Build());
            Context.Task.Info("Done");
            Context.Task.SendCommand(Command.UpdateApplicationSplashScreen);
        }
        private void OnPaintSplashScreen(object sender, PaintEventArgs e)
        {
            if ((Context==null) || (Context.Prj == null))
                return;
            int w_ss = pnlSplashScreen.Width;
            int h_ss = pnlSplashScreen.Height;
            if ((w_ss < 10) || (h_ss < 10))
                return;
            DevelopBuildConfiguration bld = (DevelopBuildConfiguration)Context.Prj.BuildConfigurations[0];
            int scr_w = 0, scr_h = 0;
            if (Project.SizeToValues(bld.AppResolution, ref scr_w, ref scr_h) == false)
                return;
            if ((scr_w < 50) || (scr_h < 50))
                return;
            if ((scr_w > 10000) || (scr_h > 10000))
                return;
            float rap = (w_ss * 0.95f) / scr_w;
            float rap2 = (h_ss * 0.95f) / scr_h;
            if (rap > rap2)
                rap = rap2;
            int w = (int)(scr_w * rap);
            int h = (int)(scr_h * rap);
            int x = (w_ss - w) / 2;
            int y = (h_ss - h) / 2;
            int cx = x + w / 2;
            int cy = y + h / 2;
            e.Graphics.FillRectangle(Brushes.White, 0, 0, w_ss, h_ss);
            e.Graphics.FillRectangle(Brushes.Black, x, y, w, h);
            if (Context.Prj.SplashScreen.Picture == null)
            {
                e.Graphics.DrawLine(Pens.Red, x, y, x + w, y + h);
                e.Graphics.DrawLine(Pens.Red, x, y + h, x + w, y);
                return;
            }
            Image i = Context.Prj.SplashScreen.Picture;
            w = (int)(w * Context.Prj.SplashScreenWidth / 100);
            h = (int)(h * Context.Prj.SplashScreenHeight / 100);
            x = cx - w / 2;
            y = cy - h / 2;
            switch (Context.Prj.SplashScreenResizeMode)
            {
                case SplashScreenImageResizeMode.NoResize:
                    e.Graphics.DrawImage(i, cx - i.Width * rap / 2, cy - i.Height * rap / 2, i.Width * rap, i.Height * rap);
                    break;
                case SplashScreenImageResizeMode.Fill:
                    e.Graphics.DrawImage(i, x, y, w, h);
                    break;
                case SplashScreenImageResizeMode.Fit:
                    float r = ((float)w) / i.Width;
                    float r2 = ((float)h) / i.Height;
                    if (r > r2)
                        r = r2;
                    e.Graphics.DrawImage(i, cx - i.Width * r / 2, cy - i.Height * r / 2, i.Width * r, i.Height * r);
                    break;
            };


        }
        private void OnSelectSplashScreenImage(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            ResourceLoadDialog rld = new ResourceLoadDialog(Context.Prj.GetProjectResourceSourceFolder(), Context.Prj);
            rld.SetSingleSelection();
            rld.EnableResourceType(ResourceType.VectorImage);
            rld.EnableResourceType(ResourceType.RasterImage);

            if (rld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImageResource ir = new ImageResource();
                if (ir.Create(rld.SelectedFiles[0], 1.0f, "") == false)
                {
                    MessageBox.Show("Unable to load slash screen image: " + rld.SelectedFiles[0]);
                    return;
                }
                Context.Prj.SplashScreen = ir;
                ir.prj = Context.Prj;
                Context.Task.Start(Task_BuildSplashScreen, "Building splash screen");
            }
        }
        private void OnEditSplashScreenImage(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if ((Context.Prj.SplashScreen == null) || (Context.Prj.SplashScreen.Source.Length == 0))
            {
                OnSelectSplashScreenImage(sender, e);
                return;
            }
            Context.Prj.EditResourceWithExternalApp(Context.Prj.SplashScreen.Source);
        }
        private void OnRebuildSplashScreenImage(object sender, EventArgs e)
        {
            Context.Task.Start(Task_BuildSplashScreen, "Building splash screen");
        }

        private void OnCreateStandardSplashScreenImage(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            string ss = Project.GetResource("Images", "logo.svg", Context.Prj.EC);
            if ((ss == null) || (ss.Length == 0))
            {
                Context.Prj.ShowErrors();
                return;
            }
            if (Disk.SaveFile(Context.Prj.GetProjectResourceSourceFullPath("logo.svg"), ss, Context.Prj.EC) == false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            ImageResource ir = new ImageResource();
            if (ir.Create("logo.svg", 1.0f, "") == false)
            {
                MessageBox.Show("Unable to load logo.svg");
                return;
            }
            Context.Prj.SplashScreen = ir;
            ir.prj = Context.Prj;
            Context.Task.Start(Task_BuildSplashScreen, "Building splash screen");
        }
        private void OnCreateStandardAnimatedSplashScreenImage(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            string ss = Project.GetResource("Images", "animated_logo.svg", Context.Prj.EC);
            if ((ss == null) || (ss.Length == 0))
            {
                Context.Prj.ShowErrors();
                return;
            }
            if (Disk.SaveFile(Context.Prj.GetProjectResourceSourceFullPath("logo.svg"), ss, Context.Prj.EC) == false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            ImageResource ir = new ImageResource();
            if (ir.Create("logo.svg", 1.0f, "") == false)
            {
                MessageBox.Show("Unable to load logo.svg");
                return;
            }
            Context.Prj.SplashScreen = ir;
            Context.Prj.SplashScreenAnimation = true;
            Context.Prj.SplashScreenAnimationSpriteSize = "128 x 128";
            Context.Prj.SplashScreenAnimationSprites = 64;
            ir.prj = Context.Prj;
            Context.Task.Start(Task_BuildSplashScreen, "Building splash screen");
        }
        #endregion

        #region Application Name
        private void UpdateAppLanguages()
        {
            if (Context.Prj == null)
                return;
            lstAppName.Items.Clear();
            bool projectOK, buildOK;
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                ListViewItem lvi = new ListViewItem(l.ToString());
                lvi.Tag = l;
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lstAppName.Items.Add(lvi);
                //SelectedLanguages[l] = true;
                string s = Context.Prj.ApplicationName.Get((Language)lvi.Tag);
                if ((s == null) || (s.Length == 0))
                {
                    lvi.ForeColor = Color.Gray;
                    lvi.SubItems[1].Text = "-";
                    lvi.Group = lstAppName.Groups["Available"];
                    projectOK = false;
                }
                else
                {
                    lvi.ForeColor = Color.Black;
                    lvi.SubItems[1].Text = s;
                    lvi.Group = lstAppName.Groups["Used"];
                    projectOK = true;
                }
                s = "";
                if (Context.CurrentBuild != null)
                    s = Context.CurrentBuild.ApplicationName.Get((Language)lvi.Tag);
                if ((s == null) || (s.Length == 0))
                {
                    lvi.SubItems[2].Text = "-";
                    buildOK = false;
                }
                else
                {
                    lvi.SubItems[2].Text = s;
                    buildOK = true;
                }
                // daca are macar un text
                if ((Context.CurrentBuild != null) && (Context.CurrentBuild.ApplicationName.Values.Count>0))
                {
                    if ((buildOK) && (!projectOK))
                    {
                        lvi.ForeColor = Color.Red;
                        lvi.SubItems[1].Text = "<Missing translation>";
                    }
                    if ((!buildOK) && (projectOK))
                    {
                        lvi.ForeColor = Color.Red;
                        lvi.SubItems[2].Text = "<Missing translation>";
                    }
                }
            }
        }
        private void OnEditAppNameLanguage(object sender, MouseEventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (lstAppName.SelectedItems.Count == 1)
            {
                ApplicationTitleDialog dlg = new ApplicationTitleDialog(Context.Prj, Context.CurrentBuild, (Language)lstAppName.SelectedItems[0].Tag);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    UpdateAppLanguages();
                }
            }
            else
            {
                MessageBox.Show("Please select a language first !");
            }
        }
        private void OnEditApplicationNameForSelectedLanguage(object sender, EventArgs e)
        {
            OnEditAppNameLanguage(null, null);
        }
        private void OnDeleteTitleForCurrentBuild(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (lstAppName.SelectedItems.Count == 1)
            {
                Language l = (Language)lstAppName.SelectedItems[0].Tag;
                if (Context.CurrentBuild.ApplicationName.Contains(l)==false)
                {
                    MessageBox.Show("Build '" + Context.CurrentBuild.Name + "' does not have a specific title for language '" + l.ToString() + "' !");
                    return;
                }
                if (MessageBox.Show("Delete title for build '" + Context.CurrentBuild.Name + "' for language '" + l.ToString() + "' ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Context.CurrentBuild.ApplicationName.Delete(l);
                    UpdateAppLanguages();
                }
            }
            else
            {
                MessageBox.Show("Please select a language first !");
            }
        }
        private void OnDeleteApplicationTitle(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (lstAppName.SelectedItems.Count == 1)
            {
                Language l = (Language)lstAppName.SelectedItems[0].Tag;
                if (MessageBox.Show("Delete title for for language '" + l.ToString() + "' ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Context.Prj.ApplicationName.Delete(l);
                    foreach (GenericBuildConfiguration bld in Context.Prj.BuildConfigurations)
                        bld.ApplicationName.Delete(l);
                    UpdateAppLanguages();
                }
            }
            else
            {
                MessageBox.Show("Please select a language first !");
            }
        }
        #endregion

        #region Builds

        private void OnStartFullProjectBuild(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            Context.Functions.SaveProject();
            PackageBuildDialog pbd = new PackageBuildDialog(Context.Prj, GetCurrentBuild().Name, false);
            pbd.ShowDialog();
            Context.Prj.ShowErrors();
            if (pbd.GoToDeployDialog)
                OnDeplayBuildPackage(null, null);
        }
        private void OnStartQuickProjectBuild(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            Context.Functions.SaveProject();
            PackageBuildDialog pbd = new PackageBuildDialog(Context.Prj, GetCurrentBuild().Name, true);
            pbd.ShowDialog();
            Context.Prj.ShowErrors();
            if (pbd.GoToDeployDialog)
                OnDeplayBuildPackage(null, null);
        }
        private void OnCleanUpServerSetup(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cleaning up server may cause the current build (if any) to crash. Are you sure you want to do this ?", "Clean up", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                HttpBuildServiceClient client = new HttpBuildServiceClient(Context.Prj);
                if (client.Query("clearbuild", new Dictionary<string, string>() { { "project", Context.Prj.GetProjectName() } }) == false)
                    Context.Prj.ShowErrors();
            }
        }
        private void UpdateBuildList(int index)
        {
            //fac update si la lista de la config
            comboBuildNames.Items.Clear();
            foreach (GenericBuildConfiguration bc in Context.Prj.BuildConfigurations)
            {
                comboBuildNames.Items.Add(bc.Name);
            }
            if ((index >= 0) && (index < comboBuildNames.Items.Count))
                comboBuildNames.SelectedIndex = index;
        }
        private void OnNewBuildSelected(object sender, EventArgs e)
        {
            if (Context.Prj == null)
            {
                propBuild.SelectedObject = null;
                return;
            }
            if (comboBuildNames.SelectedIndex >= 0)
            {
                Context.CurrentBuild = Context.Prj.BuildConfigurations[comboBuildNames.SelectedIndex];
                propBuild.SelectedObject = Context.Prj.BuildConfigurations[comboBuildNames.SelectedIndex];
            }
            else
            {
                Context.CurrentBuild = null;
                propBuild.SelectedObject = null;
            }
            btnServerBuild.Enabled = (comboBuildNames.SelectedIndex > 0);
            btnRun.Enabled = (comboBuildNames.SelectedIndex == 0);
            btnDeploy.Enabled = btnServerBuild.Enabled;
            UpdateIconPicture();
            UpdateAppLanguages();
            UpdateDefinesList();
            UpdateBuildLanguageInformations();
        }

        public GenericBuildConfiguration GetCurrentBuild()
        {
            if (Context.Prj == null)
                return null;
            if (comboBuildNames.SelectedIndex < 0)
                return null;
            return Context.Prj.BuildConfigurations[comboBuildNames.SelectedIndex];
        }
        private void OnAddNewBuild(object sender, EventArgs e)
        {
            NewBuildDialog nbd = new NewBuildDialog(Context.Prj);
            if (nbd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            if (Context.Prj.GetBuild(nbd.BuildName)!=null)
            {
                MessageBox.Show("Build '" + nbd.BuildName + "' already exists !");
                return;
            }
            GenericBuildConfiguration newBuild = null;
            switch (nbd.GetOS())
            {
                case OSType.Android:
                    newBuild = new AndroidBuildConfiguration();
                    break;
                case OSType.IOS:
                    newBuild = new IOSBuildConfiguration();
                    break;
                case OSType.Mac:
                    newBuild = new MacBuildConfiguration();
                    break;
                case OSType.WindowsDesktop:
                    newBuild = new WindowsDesktopBuildConfiguration();
                    break;
                default:
                    MessageBox.Show("Builds for '" + nbd.GetOS().ToString() + "' are not available yet !");
                    return;
            };
            if (newBuild != null)
            {
                newBuild.Name = nbd.BuildName;
                if (nbd.DupFrom.Length > 0)
                {
                    if (nbd.KeepResources)
                    {
                        foreach (GenericResource gr in Context.Prj.Resources)
                            gr.DuplicateBuildFrom(nbd.DupFrom,nbd.BuildName);
                    }
                    newBuild.Duplicate(Context.Prj.GetBuild(nbd.DupFrom));

                }
                Context.Prj.BuildConfigurations.Add(newBuild);
                UpdateBuildList(Context.Prj.BuildConfigurations.Count - 1);
            }
        }
        private void OnDeleteBuild(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (Context.CurrentBuild == null)
                return;
            if (Context.CurrentBuild.GetType() == typeof(DevelopBuildConfiguration))
            {
                MessageBox.Show("Develop build can not be deleted. You can only delete other builds !");
                return;
            }
            if (MessageBox.Show("Delete " + Context.CurrentBuild.Name, "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string bName = Context.CurrentBuild.Name.ToLower();
                Context.Prj.BuildConfigurations.Remove(Context.CurrentBuild);
                UpdateBuildList(0);
                // sterg din toate resursele build-ul sters
                foreach (GenericResource r in Context.Prj.Resources)
                    r.RemoveBuild(bName);
            }
        }
        private void OnBuildPropertiesChange(object s, PropertyValueChangedEventArgs e)
        {
            // update la nume
            int index = comboBuildNames.SelectedIndex;
            if (index >= 0)
                comboBuildNames.Items[index] = Context.Prj.BuildConfigurations[index].Name;
            if (index == 0)
                pnlSplashScreen.Invalidate();
        }
        #endregion

        #region Icons

        public void UpdateIconButtonState()
        {
            if (Context.CurrentBuild==null)
                return;
            bool activate = (Context.CurrentBuild.Icon.Length > 0);
            btnMenuEditBuildSpecificIcon.Enabled = activate;
            btnEditBuildSpecificIcon.Enabled = activate;
            btnMenuDeleteBuildIcon.Enabled = activate;
            btnDeleteBuildIcon.Enabled = activate;
            btnRebuildBuildIcon.Enabled = activate;
            btnMenuRebuildBuildIcon.Enabled = activate;
        }
        public void UpdateIconPicture()
        {
            UpdateIconButtonState();
            iconPicture.Image = null;
            Project.ProjectIcons Icons = Context.Prj.GetBuildIcons(Context.CurrentBuild);
            string iconImage = Icons.BuildIcon;
            if (iconImage.Length == 0)
                iconImage = Icons.ProjectIcon;
            if ((iconImage.Length>0) && (File.Exists(iconImage) == true))
            {
                iconPicture.Image = Project.LoadImage(iconImage);
            }
            if (iconPicture.Image == null)
            {
                iconPicture.Visible = false;
                lbIconInfo.Visible = true;
                if (iconImage.Length == 0)
                    lbIconInfo.Text = "No icon was selected for the project or for the current build !";
                else
                    lbIconInfo.Text = "The following icon '" + iconImage + "' does not exist !\nRebuild all icons !";
            }
            else
            {
                lbIconInfo.Visible = false;
                iconPicture.Visible = true;
            }
        }
        private void Task_BuildIcons(Project.ProjectIcons Icons)
        {
            Context.Task.SetMinMax(0, Icons.OutputFiles.Count);
            Context.Task.Info("Building [" + Icons.OutputFiles.Count.ToString() + " icons]");
            foreach (string path in Icons.OutputFiles.Keys)
            {
                Context.Task.CreateSubTask(String.Format("Creating icon: {0}x{0} => {1}", Icons.OutputFiles[path].Size, Path.GetFileName(Icons.OutputFiles[path].Output)));
                Context.Task.UpdateSuccessErrorState(Context.Prj.SVGtoPNG(Icons.OutputFiles[path].Source, Icons.OutputFiles[path].Output, 0, Icons.OutputFiles[path].Size, Icons.OutputFiles[path].Size, 1.0f, true));
                Context.Task.IncrementProgress();
            }
            if (Context.Prj.EC.HasErrors())
                Context.Task.Info("Done with errors !");
            else
                Context.Task.Info("Done");
            Context.Task.SendCommand(Command.UpdateApplicationIcon);
        }
        public void Task_BuildAllIcons()
        {
            Task_BuildIcons(Context.Prj.GetProjectIcons());
        }
        public void Task_BuildBuildSpecificIcons()
        {
            Task_BuildIcons(Context.Prj.GetBuildIcons(Context.CurrentBuild));
        }
        private void OnSelectProjectIcon(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            ResourceLoadDialog rld = new ResourceLoadDialog(Context.Prj.GetProjectResourceSourceFolder(), Context.Prj);
            rld.SetSingleSelection();
            rld.EnableResourceType(ResourceType.VectorImage);

            if (rld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Context.Prj.Icon = rld.SelectedFiles[0];
                Context.Task.Start(Task_BuildAllIcons, "Building icons");
            }
        }
        private void OnAddBuildSpecificIcon(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (Context.CurrentBuild == null)
                return;
            if (Context.CurrentBuild.GetType() == typeof(DevelopBuildConfiguration))
            {
                MessageBox.Show("Specific build icons are only available for builds other than the Develop build !");
                return;
            }
            ResourceLoadDialog rld = new ResourceLoadDialog(Context.Prj.GetProjectResourceSourceFolder(), Context.Prj);
            rld.SetSingleSelection();
            rld.EnableResourceType(ResourceType.VectorImage);

            if (rld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Context.CurrentBuild.Icon = rld.SelectedFiles[0];
                Context.Task.Start(Task_BuildBuildSpecificIcons, "Building icons");
            }
        }
        private void OnEditProjectIcon(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (Context.Prj.Icon.Length == 0)
            {
                OnSelectProjectIcon(sender, e);
                return;
            }
            Context.Prj.EditResourceWithExternalApp(Context.Prj.Icon);
        }
        private void OnEditBuildIcon(object sender, EventArgs e)
        {
            if (Context.Prj == null)
                return;
            if (Context.CurrentBuild == null)
                return;
            if (Context.CurrentBuild.Icon.Length>0)
                Context.Prj.EditResourceWithExternalApp(Context.CurrentBuild.Icon);
        }
        private void OnRebuildAllIcons(object sender, EventArgs e)
        {
            Context.Task.Start(Task_BuildAllIcons, "Building icons");
        }
        private void OnRebuildBuildSpecificIcons(object sender, EventArgs e)
        {
            Context.Task.Start(Task_BuildBuildSpecificIcons, "Building icons");
        }
        private void OnDeleteBuildIcon(object sender, EventArgs e)
        {
            if (Context.CurrentBuild.Icon.Length>0)
            {
                if (MessageBox.Show("Delete icon for "+Context.CurrentBuild.Name+" ?\nFile: "+Context.CurrentBuild.Icon,"Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Context.CurrentBuild.Icon = "";
                    UpdateIconPicture();
                }
            }
        }

        #endregion

        #region Deploy packages and Run
        private void OnDeplayBuildPackage(object sender, EventArgs e)
        {
            GenericBuildConfiguration gbc = GetCurrentBuild();
            if (gbc == null)
                return;
            if (gbc.GetType() == typeof(AndroidBuildConfiguration))
            {
                AndroidDeployDialog add = new AndroidDeployDialog(Context.Prj, (AndroidBuildConfiguration)gbc);
                add.ShowDialog();
                return;
            }
            MessageBox.Show("Deploy is not available for this configuration/operating system: " + gbc.Name);
        }
        private void OnRunDevelop(object sender, EventArgs e)
        {
            string outputFile = Path.Combine(Context.Prj.ProjectPath, "Bin", Context.Prj.GetProjectName() + ".exe");
            if (File.Exists(outputFile))
            {
                ExecutionControlDialog dlg = new ExecutionControlDialog(outputFile, Context.Prj);
                dlg.Show();
            }
            else
            {
                MessageBox.Show("Missing executable file:\n" + outputFile);
            }
        }
        #endregion

        #region Languages
        private bool updatinigLanguageList = false;

        private void OnProjectPropertyChange(object s, PropertyValueChangedEventArgs e)
        {
            pnlSplashScreen.Invalidate();
        }

        private void OnSetAllAvailableLanguages(object sender, EventArgs e)
        {
            lstBuildLanguages.Enabled = (cbAllAvailableLanguages.Checked == false);
            if ((Context!=null) && (Context.CurrentBuild!=null))
                Context.CurrentBuild.AllAvailableLanguages = cbAllAvailableLanguages.Checked;
        }
        private void UpdateBuildLanguageInformations()
        {
            updatinigLanguageList = true;
            lstBuildLanguages.Items.Clear();
            Dictionary<Language, bool> d = Context.Prj.GetProjectAvailableLanguages();
            Dictionary<Language, bool> dl = Context.Prj.GetBuildAvailableLanguages(Context.CurrentBuild,false);
            foreach (Language l in d.Keys)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = l.ToString();
                lvi.Tag = l;
                if (l == Context.Prj.DefaultLanguage)
                    lvi.Text += " (default language)";
                lvi.Checked = dl.ContainsKey(l);
                lstBuildLanguages.Items.Add(lvi);
            }
            cbAllAvailableLanguages.Checked = Context.CurrentBuild.AllAvailableLanguages;
            OnSetAllAvailableLanguages(null, null);
            updatinigLanguageList = false;
        }

        private void lstBuildLanguages_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (updatinigLanguageList)
                return;
            if (((Language)(e.Item.Tag))==Context.Prj.DefaultLanguage)
            {
                e.Item.Checked = true;
            }
            // update la lista
            string l = "";
            foreach (ListViewItem lvi in lstBuildLanguages.Items)
            {
                Language lng = (Language)lvi.Tag;
                if ((lvi.Checked) || (lng == Context.Prj.DefaultLanguage))
                    l += lng.ToString() + ",";
            }
            Context.CurrentBuild.BuildLanguages = l;
        }
        #endregion

        #region Defines
        bool updatingDefineList = false;
        private void UpdateDefinesList()
        {
            
            if (Context.Prj == null)
                return;
            List<string> all = Project.StringListToList(Context.Prj.Defines);
            Dictionary<string, string> d = Project.StringListToDict(Context.CurrentBuild.Defines);
            all.Sort();
            lstDefines.BeginUpdate();
            updatingDefineList = true;
            lstDefines.Items.Clear();
            foreach (string s in all)
            {
                ListViewItem lvi = new ListViewItem(s);
                lvi.Checked = d.ContainsKey(s.ToLower());
                lstDefines.Items.Add(lvi);
            }
            lstDefines.EndUpdate();
            updatingDefineList = false;
        }
        private void OnEnableDisableDefine(object sender, ItemCheckedEventArgs e)
        {
            if ((Context==null) || (Context.Prj == null) || (updatingDefineList) || (lstDefines==null) || (Context.CurrentBuild==null))
                return;
            List<string> l = new List<string>();
            foreach (ListViewItem lvi in lstDefines.Items)
                if (lvi.Checked)
                    l.Add(lvi.Text);
            Context.CurrentBuild.Defines = Project.ListToStringList(l);
        }
        private void OnAddNewDefine(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter a new define name", "Define_" + System.Environment.TickCount.ToString());
            if (ib.ShowDialog() == DialogResult.OK)
            {
                string dname = ib.StringResult.Trim();
                if (Project.ValidateVariableNameCorectness(dname,false)==false)
                {
                    MessageBox.Show("Invalid name '" + dname + "'.\nIt should start with a letter and contain letters,numbers and character '_' !");
                    return;
                }
                Dictionary<string, string> d = Project.StringListToDict(Context.Prj.Defines);
                if (d.ContainsKey(dname.ToLower()))
                {
                    MessageBox.Show("Define '" + dname + "' already exists !");
                    return;
                }
                List<string> l = Project.StringListToList(Context.Prj.Defines);
                l.Add(dname);
                l.Sort();
                Context.Prj.Defines = Project.ListToStringList(l);
                UpdateDefinesList();
            }
        }
        private void OnDeleteDefine(object sender, EventArgs e)
        {
            if (lstDefines.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select a define first !");
                return;
            }
            if (MessageBox.Show("Delete define '" + lstDefines.SelectedItems[0].Text + "' ?", "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            // sterg define-ul
            Context.Prj.Defines = Project.DeleteFromStringList(Context.Prj.Defines, lstDefines.SelectedItems[0].Text);
            foreach (GenericBuildConfiguration gbc in Context.Prj.BuildConfigurations)
                gbc.Defines = Project.DeleteFromStringList(gbc.Defines, lstDefines.SelectedItems[0].Text);
            UpdateDefinesList();
        }
        #endregion

        public override void OnOpenNewProject(bool newProject)
        {
            lbProjectName.Text = Context.Prj.GetProjectName();
            propProject.SelectedObject = Context.Prj;
            if (newProject == false)
            {
                UpdateIconPicture();
                UpdateSplashScrrenPicture();
            }
            UpdateAppLanguages();
            PopulateProjectTasks();

            UpdateBuildList(0);        
        }
        public override void OnCommand(Command cmd, object parameters = null)
        {
            switch (cmd)
            {
                case Command.UpdateApplicationIcon: UpdateIconPicture(); break;
                case Command.UpdateApplicationSplashScreen: UpdateSplashScrrenPicture(); break;
            }
        }
        public override void OnSetContext()
        {
            Context.BackgroundTaskList = lstTaskInfo;
        }
        public override void OnActivate()
        {
            UpdateBuildLanguageInformations();
            UpdateIconButtonState();
            UpdateDefinesList();
        }

        private void OnShowPlugins(object sender, EventArgs e)
        {
            PluginsManager dlg = new PluginsManager(Context.Prj,Context.Plugins);
            if (dlg.ShowDialog()==DialogResult.OK)
            {
                OnReloadResourcePlugins(null, null);
            }
        }

        private void OnReloadResourcePlugins(object sender, EventArgs e)
        {
            if (Context.IDESettings.EnableResourceTab)
            {
                //tabProjectResources.UpdatePluginPreview();
                // trebuie cautat un workaround
            }
        }

        private void OnEnterGitWindow(object sender, EventArgs e)
        {
            GitDialog dlg = new GitDialog(this.Context);
            dlg.ShowDialog();
        }

        private void OnShow3rdPartySDKs(object sender, EventArgs e)
        {
            ThirdPartSDKConfig dlg = new ThirdPartSDKConfig(this.GetCurrentBuild());
            dlg.ShowDialog();
        }
    }
}
