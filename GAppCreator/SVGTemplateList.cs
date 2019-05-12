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
    public partial class SVGTemplateList : UserControl
    {
        public class TemplateInfo
        {
            public string Name="";
            public string Path="";
            public Image Picture=null;
            public string PicturePath = "";
            public string Group="";
        };
        public event EventHandler OnDoubleClicked = null;
        public event EventHandler OnTemplateSelected = null;
        
        public SVGTemplateList()
        {
            InitializeComponent();
        }
        public void Clear()
        {
            ListTemplates.Items.Clear();
            ListTemplates.Groups.Clear();
            TemplateImages.Images.Clear();
        }
        public bool IsTemplateSelected()
        {
            return (ListTemplates.SelectedItems.Count == 1);
        }
        public string GetSelectedPath()
        {
            if (ListTemplates.SelectedItems.Count == 1)
                return (string)ListTemplates.SelectedItems[0].Tag;
            else
                return "";
        }
        public TemplateInfo GetSelectedTemplate()
        {
            if (ListTemplates.SelectedItems.Count == 1)
                return GetTemplateByName(ListTemplates.SelectedItems[0].Text);
            else
                return null;
        }
        public TemplateInfo GetTemplateByName(string name)
        { 
            name = name.ToLower();
            foreach (ListViewItem lvi in ListTemplates.Items)
                if (lvi.Text.ToLower().Equals(name))
                {
                    TemplateInfo ti = new TemplateInfo();
                    ti.Name = lvi.Text;
                    ti.Path = (string)lvi.Tag;
                    if (ti.Path.Length > 4)
                        ti.PicturePath = ti.Path.Substring(0, ti.Path.Length - 4) + ".png";
                    if (lvi.Group != null)
                        ti.Group = lvi.Group.Header;
                    if (TemplateImages.Images.ContainsKey(ti.Name))
                        ti.Picture = TemplateImages.Images[ti.Name];
                    return ti;
                }
            return null;
        }
        public void RemoveFromList(string name)
        {
            name = name.ToLower();
            ListViewItem toDelete = null;
            foreach (ListViewItem lvi in ListTemplates.Items)
                if (lvi.Text.ToLower().Equals(name))
                {
                    toDelete = lvi;
                    break;
                }
            if (toDelete!=null)
                ListTemplates.Items.Remove(toDelete);
        }
        public string GetSelectedTemaplateName()
        {
            if (ListTemplates.SelectedItems.Count == 1)
                return (string)ListTemplates.SelectedItems[0].Text;
            else
                return "";
        }
        public void AddFromFolder(string folder)
        {
            AddFromFolder(folder, null);
        }
        public void AddFromFolder(string folder,string group)
        {
            if ((group!=null) && (group.Length>0))
            {
                if (ListTemplates.Groups[group]==null)
                {
                    ListTemplates.Groups.Add(group, group);
                }
            }
            Dictionary<string, int> d = new Dictionary<string, int>();
            List<string> files = new List<string>();
            try
            {
                string[] fls = Directory.GetFiles(folder);
                foreach (string s in fls)
                {
                    if ((s.ToLower().EndsWith(".png")) || (s.ToLower().EndsWith(".svg")))
                    {
                        string name = s.Substring(0,s.LastIndexOf('.'));
                        if (d.ContainsKey(name)==false)
                            d[name]=0;
                        if (s.ToLower().EndsWith(".png"))
                            d[name]|=1;
                        if (s.ToLower().EndsWith(".svg"))
                            d[name]|=2;
                    }
                }
            }
            catch (Exception)
            {

            }
            // caut png-uri pentru care exista svg-uri si le adaug
            foreach (string fname in d.Keys)
            {
                if (d[fname]==3)
                    files.Add(fname);
            }
            files.Sort();
            // adaug
            foreach (string fname in files)
            {
                Image img = Project.LoadImage(fname + ".png");
                if (img == null)
                    continue;
                TemplateImages.Images.Add(Path.GetFileName(fname),img);
                ListViewItem lvi = new ListViewItem(Path.GetFileName(fname));
                lvi.ImageKey = Path.GetFileName(fname);
                lvi.Tag = fname + ".svg";
                if ((group != null) && (group.Length > 0))
                    lvi.Group = ListTemplates.Groups[group];
                ListTemplates.Items.Add(lvi);
            }
        }

        public ListViewItem SelectTemplate(string template)
        {
            if (template == null)
                return null;
            foreach (ListViewItem lvi in ListTemplates.Items)
                if (lvi.Text.ToLower()==template.ToLower())
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                    return lvi;
                }
            return null;
        }

        private void OnDblClicked(object sender, EventArgs e)
        {
            if ((OnDoubleClicked != null) && (ListTemplates.SelectedItems.Count == 1))
            {            
                OnDoubleClicked(this, null);
            }
        }

        private void OnSelectedItemChanged(object sender, EventArgs e)
        {
            if (OnTemplateSelected!=null)
            {
                OnTemplateSelected(this, null);
            }
        }
    }
}
