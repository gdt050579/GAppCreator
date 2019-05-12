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
    public partial class AndroidGoogleAnalyticsSDK : SDKSettingsUserControl
    {
        private AndroidBuildConfiguration config = null;
        public AndroidGoogleAnalyticsSDK()
        {
            InitializeComponent();
            SDKIconName = "google_analytics";
            SDKName = "Google analytics";
        }
        public override void Init(GenericBuildConfiguration _config)
        {
            config = (AndroidBuildConfiguration)_config;
            txTrackingID.Text = config.GoogleAnalyticsTrackingID;
        }
        public override bool UpdateBuildConfiguration()
        {
            config.GoogleAnalyticsTrackingID = txTrackingID.Text;
            return true;
        }
    }
}
