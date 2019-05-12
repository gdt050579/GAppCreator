using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginInterface
{
    public interface ResourcePlugin
    {
        string GetResourceTypeDescription();
        string GetResourceTypeExtension();
        object Load(string SourceFileName,out string error);
        bool New(string SourceFileName,out string error);
        bool Edit(string SourceFileName,out string error);
        bool Build(object Resource, string OutputFileName,out string error);
        Image GetIcon(object Resource, int width, int height,out string error);
        string GetResourceInformation(object Resource);
        UserControl GetPreviewControl();
    }
}
