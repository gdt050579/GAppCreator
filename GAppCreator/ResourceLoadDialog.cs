using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace GAppCreator
{
    public partial class ResourceLoadDialog : Form
    {
        class FileInformation: IComparable
        {
            public string Name;
            public long Size;
            public DateTime LastModified;
            public bool Used;

            public static int sortColumn = 2;

            public int CompareTo(object obj)
            {
                // Name
                FileInformation fi = (FileInformation)obj;
                if (sortColumn == 0)
                    return string.Compare(Name, fi.Name, true);
                if (sortColumn == 1)
                    return fi.Size.CompareTo(Size);
                if (sortColumn == 2)
                    return fi.LastModified.CompareTo(LastModified);
                return 0;
            }
        };
        string path = "";
        List<FileInformation> files = new List<FileInformation>();
        public string []SelectedFiles;
        Dictionary<string, bool> usedSources = new Dictionary<string, bool>();
        PreviewImage pvi;
        Project prj;
        

        public void SetSingleSelection()
        {
            lstFiles.MultiSelect = false;
        }
        public void EnableResourceType(ResourceType rt)
        {
            switch (rt)
            {
                case ResourceType.VectorImage: cbVectorImage.Checked = true; break;
                case ResourceType.RasterImage: cbRasterImage.Checked = true; break;
                case ResourceType.Music: cbMusic.Checked = true; break;
                case ResourceType.Raw: cbRaw.Checked = true; break;
                case ResourceType.Presentation: cbPresentation.Checked = true; break;
            }
            PopulateList();
        }
        public ResourceLoadDialog(string readPath, Project p)
        {
            InitializeComponent();

            pvi = new PreviewImage();
            pnlPreview.Controls.Add(pvi);
            pvi.Dock = DockStyle.Fill;
            pvi.Visible = false;

            prj = p;
            foreach (GenericResource r in prj.Resources)
            {
                if ((r.Source != null) && (r.Source.Length > 0))
                    usedSources[r.Source.ToLower()] = true;
            }

            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            path = readPath;
            RefreshFileList();
            PopulateList();
        }
        private string SizeToString(long sz)
        {
            string s = "";
            int count = 0;
            while (sz > 0)
            {
                s = (char)((sz % 10) + '0') + s;
                sz = sz / 10;
                count++;
                if ((count == 3) && (sz > 0))
                {
                    s = "." + s;
                    count = 0;
                }
            }
            if (s.Length == 0)
                return "0";
            else
                return s;
        }
        private void PopulateList()
        {
            lstFiles.Items.Clear();
            string txF = txtNameFilter.Text.ToLower();
            bool used;
            foreach (FileInformation fi in files)
            {
                if ((txF.Length > 0) && (fi.Name.ToLower().Contains(txF) == false))
                    continue;
                ResourceType rt = Project.FileNameToResourceType(fi.Name);
                if ((rt == ResourceType.VectorImage) && (cbVectorImage.Checked == false))
                    continue;
                if ((rt == ResourceType.RasterImage) && (cbRasterImage.Checked == false))
                    continue;
                if ((rt == ResourceType.Music) && (cbMusic.Checked == false))
                    continue;
                if ((rt == ResourceType.Presentation) && (cbPresentation.Checked == false))
                    continue;
                if ((rt == ResourceType.Raw) && (cbRaw.Checked == false))
                    continue;
                used = usedSources.ContainsKey(fi.Name.ToLower());
                if ((cbOnlyUnused.Checked) && (used))
                    continue;
                
                ListViewItem lvi = new ListViewItem(fi.Name);
                lvi.SubItems.Add(SizeToString(fi.Size));
                lvi.SubItems.Add(fi.LastModified.ToString("yyyy-MM-dd  HH:mm:ss", CultureInfo.InvariantCulture));
                if (used)
                    lvi.ForeColor = Color.Gray;
                lstFiles.Items.Add(lvi);
            }
        }
        private void RefreshFileList()
        {
            try
            {
                string[] fnames = Directory.GetFiles(path);
                files.Clear();
                foreach (string fname in fnames)
                {
                    FileInfo f = new FileInfo(fname);
                    FileInformation finf = new FileInformation();
                    finf.Name = Path.GetFileName(fname);
                    finf.Size = f.Length;
                    finf.LastModified = f.LastWriteTime;
                    finf.Used = false;
                    files.Add(finf);
                }
                files.Sort();
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to read files from " + path + "\n" + e.ToString());
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            RefreshFileList();
            PopulateList();
        }

        private void OnTextFilter(object sender, EventArgs e)
        {
            PopulateList();
        }

        private void OnCancelDialog(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnAddToSelectedList(object sender, EventArgs e)
        {
            if ((lstFiles.SelectedItems != null) && (lstFiles.SelectedItems.Count > 0))
            {
                SelectedFiles = new string[lstFiles.SelectedItems.Count];
                for (int tr = 0; tr < lstFiles.SelectedItems.Count; tr++)
                    SelectedFiles[tr] = lstFiles.SelectedItems[tr].Text;
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            this.Close();
        }

        private void OnChangeFileTypeFilter(object sender, EventArgs e)
        {
            PopulateList();
        }
        private void OnClickOnlyUnused(object sender, EventArgs e)
        {
            PopulateList();
        }
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count == 1)
            {
                ResourceType rt = Project.FileNameToResourceType(lstFiles.SelectedItems[0].Text);
                if (rt == ResourceType.RasterImage)
                {
                    Bitmap bmp = (Bitmap)Project.LoadImage(Path.Combine(path, lstFiles.SelectedItems[0].Text));
                    if (bmp!=null)
                    {
                        pvi.Tag = bmp;
                        pvi.Visible = true;
                        pvi.Invalidate();
                        pvi.Refresh();
                        return;
                    }
                }
            }
            pvi.Visible = false;
        }

        private void OnImportFiles(object sender, EventArgs e)
        {
            if (prj == null)
                return;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Resource files|*.png;*.jpg;*.bmp;*.svg;*.mp3;*.wav;*.ogg;*.animxml|All Files|*.*";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProgressDialog ip = new ProgressDialog(prj);
                ip.SetImportFiles(dlg.FileNames);
                ip.Start();
                OnRefresh(null, null);
            }
        }

        private void OnCreateNewSVG(object sender, EventArgs e)
        {
            if (prj == null)
                return;
            InputBox ib = new InputBox("Enter the name of the new svg file", "new_svg_" + DateTime.Now.Ticks.ToString() + ".svg");
            if (ib.ShowDialog() == DialogResult.OK)
            {
                string fname = prj.GetProjectResourceSourceFullPath(ib.StringResult);
                if (fname.ToLower().EndsWith(".svg") == false)
                    fname += ".svg";
                if (File.Exists(fname))
                {
                    if (MessageBox.Show("File " + fname + " already exists!\nDo you want to override it ?", "SVG creator", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    if (Disk.DeleteFile(fname, prj.EC) == false)
                    {
                        prj.ShowErrors();
                        return;
                    }
                }
                string body = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"{0}\" height=\"{1}\" >\n</svg>";
                if (Disk.SaveFile(fname, String.Format(body, 400, 400), prj.EC) == false)
                {
                    prj.ShowErrors();
                }
                else
                {
                    MessageBox.Show("File " + fname + " was created !");
                    if (prj.Settings.InskapePath.Length > 0)
                        prj.RunCommand(prj.Settings.InskapePath, fname, "Inkscape editor", false, false);
                    OnRefresh(null, null);
                }
            }
        }



        private void OnCreateNewAnimation(object sender, EventArgs e)
        {
            if (prj == null)
                return;
            InputBox ib = new InputBox("Enter the name of the new animation file", "new_animation_" + DateTime.Now.Ticks.ToString() + ".animxml");
            if (ib.ShowDialog() == DialogResult.OK)
            {
                string fname = prj.GetProjectResourceSourceFullPath(ib.StringResult);
                if (fname.ToLower().EndsWith(".animxml") == false)
                    fname += ".animxml";
                if (File.Exists(fname))
                {
                    if (MessageBox.Show("File " + fname + " already exists!\nDo you want to override it ?", "Presentation creator", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    if (Disk.DeleteFile(fname, prj.EC) == false)
                    {
                        prj.ShowErrors();
                        return;
                    }
                }
                string body = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Presentation xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" DesignWidth=\"480\" DesignHeight=\"800\" Width=\"480\" Height=\"800\" />";
                if (Disk.SaveFile(fname, body, prj.EC) == false)
                {
                    prj.ShowErrors();
                }
                else
                {
                    MessageBox.Show("File " + fname + " was created !");
                    // editare
                    OnRefresh(null, null);
                }
            }
        }

        private void OnShowResourceFolder(object sender, EventArgs e)
        {
            Process.Start(prj.GetProjectResourceSourceFolder());
        }

        private void OnDeleteResources(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count==0)
            {
                MessageBox.Show("No files selected !");
                return;
            }
            if (MessageBox.Show("Delete "+lstFiles.SelectedItems.Count.ToString()+" resource(s) ?","Delete",MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (ListViewItem lvi in lstFiles.SelectedItems)
                {
                    if (Disk.DeleteFile(prj.GetProjectResourceSourceFullPath(lvi.Text), null) == false)
                        MessageBox.Show("Failed to delete: " + lvi.Text);
                }
            }
            OnRefresh(null, null);
        }

        private void OnEditResource(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select one resource to be edited !");
                return;
            }
            prj.EditResourceWithExternalApp(lstFiles.SelectedItems[0].Text);
            OnRefresh(null, null);
        }

        private void OnDblClickOnFiles(object sender, MouseEventArgs e)
        {
            OnEditResource(null, null);
        }

        private void sVGResourceFromTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateImages dlg = new TemplateImages(false);
            if (dlg.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                string fname = prj.GetProjectResourceSourceFullPath(dlg.SVGName);
                if (fname.ToLower().EndsWith(".svg") == false)
                    fname += ".svg";
                if (File.Exists(fname))
                {
                    if (MessageBox.Show("File " + fname + " already exists!\nDo you want to override it ?", "SVG creator", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    if (Disk.DeleteFile(fname, prj.EC) == false)
                    {
                        prj.ShowErrors();
                        return;
                    }
                }
                if (Disk.Copy(dlg.TemplatePath,fname, prj.EC) == false)
                {
                    prj.ShowErrors();
                }
                else
                {
                    MessageBox.Show("File " + fname + " was created !");
                    if (prj.Settings.InskapePath.Length > 0)
                        prj.RunCommand(prj.Settings.InskapePath, fname, "Inkscape editor", false, false);
                    OnRefresh(null, null);
                }
            }
        }

        private void lstFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            FileInformation.sortColumn = e.Column;
            files.Sort();
            PopulateList();
        }


    }
}
