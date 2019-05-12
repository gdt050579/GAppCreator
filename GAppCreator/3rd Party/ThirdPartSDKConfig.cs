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
    public partial class ThirdPartSDKConfig : Form
    {
        private List<SDKSettingsUserControl> settingsList = new List<SDKSettingsUserControl>();
        public ThirdPartSDKConfig(GenericBuildConfiguration build)
        {
            InitializeComponent();

            // pregatesc listele de SDK-uri ----- ANDROID -----
            if (build.GetType() == typeof(AndroidBuildConfiguration))
            {
                settingsList.Add(new AndroidAdMobSDK());
                settingsList.Add(new AndroidChartBoostSDK());
                settingsList.Add(new AndroidGooglePlayServices());
                settingsList.Add(new AndroidFireBaseSDK());
                settingsList.Add(new AndroidGoogleAnalyticsSDK());
            }

            // populez datele
            foreach (var sdk in settingsList)
            {
                ListViewItem lvi = new ListViewItem(sdk.SDKName, sdk.SDKIconName);
                lvi.Tag = sdk;
                sdk.Init(build);
                lstSDK.Items.Add(lvi);
                pnlHost.Controls.Add(sdk);
                sdk.Location = new Point(0, 0);
                sdk.Width = pnlHost.Width-8;
                sdk.Visible = false;
            } 
        }
        private void SelectSDK(SDKSettingsUserControl sdkToSelect)
        {
            foreach (var sdk in settingsList)
            {
                sdk.Visible = (sdk == sdkToSelect);
            }
        }
        private void OnSelectSDK(object sender, EventArgs e)
        {
            if (lstSDK.SelectedItems.Count == 1)
                SelectSDK((SDKSettingsUserControl)lstSDK.SelectedItems[0].Tag);
            else
                SelectSDK(null);
        }
        private void OnOK(object sender, EventArgs e)
        {
            foreach (var sdk in settingsList)
            {
                if (sdk.UpdateBuildConfiguration() == false)
                    return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
