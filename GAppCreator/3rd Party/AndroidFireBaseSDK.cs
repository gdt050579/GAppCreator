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
    public partial class AndroidFireBaseSDK : SDKSettingsUserControl
    {
        private AndroidBuildConfiguration config = null;
        public AndroidFireBaseSDK()
        {
            InitializeComponent();
            SDKIconName = "firebase";
            SDKName = "Firebase";
        }
        public override void Init(GenericBuildConfiguration _config)
        {
            config = (AndroidBuildConfiguration)_config;
            string temp = Project.Base64ToString(config.FirebaseGoogleServicesJSONFile);
            if (temp != null)
                txGoogleServicesJSON.Text = temp;
            cbCrashAnalytics.Checked = config.FirebaseCrashAnalytics;
        }
        public override bool UpdateBuildConfiguration()
        {
            config.FirebaseGoogleServicesJSONFile = Project.StringToBase64(txGoogleServicesJSON.Text);
            config.FirebaseCrashAnalytics = cbCrashAnalytics.Checked;
            return true;
        }
    }
}
