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
    public partial class AndroidChartBoostSDK : SDKSettingsUserControl
    {
        private AndroidBuildConfiguration config = null;
        public AndroidChartBoostSDK()
        {
            InitializeComponent();
            SDKIconName = "chartboost";
            SDKName = "Chartboost";
        }
        public override void Init(GenericBuildConfiguration _config)
        {
            config = (AndroidBuildConfiguration)_config;
            txAppID.Text = config.ChartboostAppID;
            txAppSignature.Text = config.ChartboostAppSignature;
        }
        public override bool UpdateBuildConfiguration()
        {
            config.ChartboostAppID = txAppID.Text;
            config.ChartboostAppSignature = txAppSignature.Text;
            return true;
        }
    }
}
