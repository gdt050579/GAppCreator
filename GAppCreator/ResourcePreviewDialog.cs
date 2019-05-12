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
    public partial class ResourcePreviewDialog : Form
    {
        ProjectContext Context;
        List<GenericResource> lstResources = new List<GenericResource>();
        //List<StringValues> lstStrings = new List<StringValues>();
        PreviewControl preview = null;
        ResourcesConstantType ResourceType;
        PreviewData pData = new PreviewData();

        private static PreviewImage previewImage = new PreviewImage();
        private static PreviewSound previewSound = new PreviewSound();
        private static PreviewShader previewShader = new PreviewShader();
        private static PreviewFont previewFont = new PreviewFont();

        public ResourcePreviewDialog(ProjectContext context,string varName, ResourcesConstantType resourceType)
        {
            InitializeComponent();
            Context = context;
            ResourceType = resourceType;
            Text = "Preview: " + varName;
            Type t = ConstantHelper.ConvertResourcesConstantTypeToResourceType(resourceType);
            if (t != null)
            {
                ArrayCounter ac = new ArrayCounter();
                foreach (GenericResource r in Context.Prj.Resources)
                {
                    if ((r.Name == varName) && (r.GetType() == t))
                    {
                        lstResources.Add(r);
                        ac.Add(r);
                    }
                }
                switch (resourceType)
                {
                    case ResourcesConstantType.Image: preview = previewImage; break;
                    case ResourcesConstantType.Sound: preview = previewSound; break;
                    case ResourcesConstantType.Shader: preview = previewShader; break;
                    case ResourcesConstantType.Font: preview = previewFont; break;
                }
                pnlPreview.Controls.Add(preview);
                preview.Dock = DockStyle.Fill;
                UpdatePreview(0);
                int d1 = ac.GetArray1(varName);
                int d2 = ac.GetArray2(varName);
                if (d1>0)
                {
                    lbArr.Visible = true;
                    nmArr1.Maximum = d1-1;
                    nmArr1.Visible = true;
                    if (d2>0)
                    {
                        lbX.Visible = true;
                        nmArr2.Maximum = d2-1;
                        nmArr2.Visible = true;
                    }
                }
            }
            if (ResourceType == ResourcesConstantType.String)
            {
                foreach (StringValues sv in Context.Prj.Strings)
                {
                    if (sv.VariableName == varName)
                        AddStringValuess(sv);
                }
                lstStringList.Visible = true;
            }
        }
        private void AddStringValuess(StringValues sv)
        {
            if ((sv.Array1<0) && (sv.Array2<0))
            {
                foreach (StringValue s in sv.Values)
                {
                    if ((s.Value!=null) && (s.Value.Length>0))
                    {
                        ListViewItem lvi = new ListViewItem(s.Language.ToString());
                        lvi.SubItems.Add(s.Value);
                        lstStringList.Items.Add(lvi);
                    }
                }            
            } else {
                if ((sv.Array1 >= 0) && (sv.Array2 < 0))
                    lstStringList.Items.Add(sv.Array1.ToString());
                else
                    lstStringList.Items.Add(sv.Array1.ToString()+" , "+sv.Array2.ToString());
                foreach (StringValue s in sv.Values)
                {
                    if ((s.Value != null) && (s.Value.Length > 0))
                    {
                        ListViewItem lvi = new ListViewItem("     "+s.Language.ToString());
                        lvi.SubItems.Add(s.Value);
                        lstStringList.Items.Add(lvi);
                    }
                } 
            }
        }
        private void UpdatePreview(int index)
        {
            if (ResourceType == ResourcesConstantType.String)
            {
                return;
            }
            // altfel daca e o resursa normala
            lstResources[index].GetPreviewData(pData);
            preview.SetPreviewObject(Context.Prj, Context.SmallIcons, pData.Data);
        }

        private void OnChangeResource(object sender, EventArgs e)
        {
            int v1 = (int)nmArr1.Value;
            int v2 = (int)nmArr2.Value;
            if (nmArr2.Visible == false)
                v2 = -1;
            if (nmArr1.Visible == false)
                return;
            for (int tr = 0;tr<lstResources.Count;tr++)
            {
                if (lstResources[tr].Array1 == v1)
                {
                    if ((v2 >= 0) && (lstResources[tr].Array2 != v2))
                        continue;
                    UpdatePreview(tr);
                    break;
                }
            }
        }
    }
}
