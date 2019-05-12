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
    public partial class SDKSettingsUserControl : UserControl
    {
        public string SDKIconName = "";
        public string SDKName = "";

        public SDKSettingsUserControl()
        {
            InitializeComponent();
        }
        public virtual void Init(GenericBuildConfiguration config) { }
        public virtual bool UpdateBuildConfiguration() { return false; }
    }
}
