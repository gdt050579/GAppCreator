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
    public partial class AndroidAdMobSDK : SDKSettingsUserControl
    {
        private AndroidBuildConfiguration config = null;
        public AndroidAdMobSDK()
        {
            InitializeComponent();
            SDKIconName = "admob";
            SDKName = "Ad Mob";
        }
        public override void Init(GenericBuildConfiguration _config)
        {
            config = (AndroidBuildConfiguration)_config;
            txAppID.Text = config.AdMobAppID;
            cbEnableTestMode.Checked = config.EnableAdMobTestMode;
        }
        public override bool UpdateBuildConfiguration()
        {
            config.AdMobAppID = txAppID.Text;
            config.EnableAdMobTestMode = cbEnableTestMode.Checked;
            return true;
        }
    }
}
