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
    public partial class PreviewShader : PreviewControl
    {
        public PreviewShader()
        {
            InitializeComponent();
        }

        public override void OnNewPreviewObject()
        {
            ShaderResource shd = (ShaderResource)SelectedObject;
            if (shd == null)
                return;
            txVertex.Text = shd.GetVertexShader().Replace("\r\n","\r").Replace("\n","\r").Replace("\r","\r\n");
            txFragment.Text = shd.GetFragmentShader().Replace("\r\n", "\r").Replace("\n", "\r").Replace("\r", "\r\n");
            lstVariable.Items.Clear();
            foreach (ShaderVariable sv in shd.Uniforms)
            {
                ListViewItem lvi = new ListViewItem(sv.Name);
                lvi.SubItems.Add(sv.Type.ToString());
                lvi.SubItems.Add(sv.ClearAfterUsage.ToString());
                lstVariable.Items.Add(lvi);
            }
        }
    }
}
