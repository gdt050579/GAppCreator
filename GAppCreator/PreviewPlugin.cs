using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    class PreviewPlugin: PreviewControl
    {
        UserControl preview;
        public PreviewPlugin(UserControl pluginPreviewControl): base()
        {
            this.Controls.Add(pluginPreviewControl);
            preview = pluginPreviewControl;
            pluginPreviewControl.Dock = DockStyle.Fill;
        }
        public override void OnNewPreviewObject()
        {
            preview.Tag = SelectedObject;
            preview.Invalidate();
        }
    }
}
