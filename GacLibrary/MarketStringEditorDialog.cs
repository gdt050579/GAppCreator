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
    public partial class MarketStringEditorDialog : Form
    {
        enum MarketStringType
        {
            None,
            GooglePlayAppPage,
            GooglePlayDeveloperPage,
            AmazonWebMarketAppPage,
            AmazonAndroidMarketAppPage,
            AmazonAndroidSearch,
            LGSmartWorldAppPage,
            LGSmartWorldSearch,
            SamsungAndroidAppPage,
            SamsungSellerID,
            SlideMeAppPage,
            SlideMeDeveloperPage,
            OperaSearch,
        };
        public string ResultedURL = "";
        private Dictionary<MarketStringType,string> StringTypes = new Dictionary<MarketStringType,string>();
        private MarketStringType Type = MarketStringType.None;
        public MarketStringEditorDialog(string value)
        {
            InitializeComponent();
            if (value != null)
                txResultedURL.Text = value;
            // setez datele
            StringTypes[MarketStringType.None] = "<None - plan url>";
            StringTypes[MarketStringType.GooglePlayAppPage] = "GooglePlay Application Page (rate)";
            StringTypes[MarketStringType.GooglePlayDeveloperPage] = "GooglePlay Developer Page";
            StringTypes[MarketStringType.AmazonWebMarketAppPage] = "Amazon (Web) Application Page (rate)";
            StringTypes[MarketStringType.AmazonAndroidMarketAppPage] = "Amazon (Android) Application Page (rate)";
            StringTypes[MarketStringType.AmazonAndroidSearch] = "Amazon (Android) Market Search";
            StringTypes[MarketStringType.LGSmartWorldAppPage] = "LG SmartWorld (Android) Application Page (rate)";
            StringTypes[MarketStringType.LGSmartWorldSearch] = "LG SmartWorld (Android) Search)";
            StringTypes[MarketStringType.SamsungAndroidAppPage] = "Samsung market (Android) Application Page (rate)";
            StringTypes[MarketStringType.SamsungSellerID] = "Samsung seller ID";
            StringTypes[MarketStringType.SlideMeAppPage] = "SlideMe (Android) Application Page (rate)";
            StringTypes[MarketStringType.SlideMeDeveloperPage] = "SlideMe (Android) Developer Page";
            StringTypes[MarketStringType.OperaSearch] = "Opera web search";

            // update la combo
            foreach (MarketStringType type in StringTypes.Keys)
                comboType.Items.Add(StringTypes[type]);
            comboType.SelectedIndex = 0;
            OnChangeType(null, null);
        }

        private void OnOK(object sender, EventArgs e)
        {
            ResultedURL = txResultedURL.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnChangeType(object sender, EventArgs e)
        {
            if (comboType.SelectedItem == null)
                return;
            foreach (MarketStringType type in StringTypes.Keys)
                if (StringTypes[type].Equals(comboType.SelectedItem.ToString()))
                {
                    Type = type;
                    UpdateTypeFields();
                }
        }
        
        private void UpdateTypeFields()
        {
            txParam1.Visible = lbParam1.Visible = false;
            txParam1.Text = "";
            switch (Type)
            {
                case MarketStringType.None:
                    break;
                case MarketStringType.GooglePlayAppPage:
                case MarketStringType.AmazonWebMarketAppPage:
                case MarketStringType.AmazonAndroidMarketAppPage:
                case MarketStringType.SamsungAndroidAppPage:
                case MarketStringType.SlideMeAppPage:
                    lbParam1.Text = "Package name";
                    txParam1.Visible = lbParam1.Visible = true;
                    break;
                case MarketStringType.SamsungSellerID:
                    lbParam1.Text = "Seller IS";
                    txParam1.Visible = lbParam1.Visible = true;
                    break;
                case MarketStringType.LGSmartWorldAppPage:
                    lbParam1.Text = "App PID";
                    txParam1.Visible = lbParam1.Visible = true;
                    break;
                case MarketStringType.GooglePlayDeveloperPage:
                case MarketStringType.SlideMeDeveloperPage:
                    lbParam1.Text = "Company name";
                    txParam1.Visible = lbParam1.Visible = true;
                    break;
                case MarketStringType.AmazonAndroidSearch:
                case MarketStringType.LGSmartWorldSearch:
                case MarketStringType.OperaSearch:
                    lbParam1.Text = "Search word";
                    txParam1.Visible = lbParam1.Visible = true;
                    break;
            }
        }

        private void UpdateResultedMarketURL()
        {
            switch (Type)
            {
                case MarketStringType.None:
                    break;
                case MarketStringType.GooglePlayAppPage:
                    txResultedURL.Text = "market://details?id=" + txParam1.Text.Trim().Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.GooglePlayDeveloperPage:
                    txResultedURL.Text = "market://search?q=pub:" + txParam1.Text.Trim().Replace(" ", "+").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.AmazonAndroidSearch:
                    txResultedURL.Text = "amzn://apps/android?s=" + txParam1.Text.Trim().Replace(" ", "%20").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.AmazonWebMarketAppPage:
                    txResultedURL.Text = "http://www.amazon.com/gp/mas/dl/android?p=" + txParam1.Text.Trim().Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.AmazonAndroidMarketAppPage:
                    txResultedURL.Text = "amzn://apps/android?p=" + txParam1.Text.Trim().Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.SamsungAndroidAppPage:
                    txResultedURL.Text = "samsungapps://ProductDetail/" + txParam1.Text.Trim().Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.SamsungSellerID:
                    txResultedURL.Text = "samsungapps://SellerDetail/" + txParam1.Text.Trim();
                    break;
                case MarketStringType.SlideMeAppPage:
                    txResultedURL.Text = "sam://details?id=" + txParam1.Text.Trim().Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.SlideMeDeveloperPage:
                    txResultedURL.Text = "sam://search?q=pub:" + txParam1.Text.Trim().Replace(" ", "+").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
                case MarketStringType.LGSmartWorldAppPage:
                    txResultedURL.Text = "lgsmartworld://p=" + txParam1.Text.Trim();
                    break;
                case MarketStringType.LGSmartWorldSearch:
                    txResultedURL.Text = "lgsmartworld://s=" + txParam1.Text.Trim();
                    break;
                case MarketStringType.OperaSearch:
                    txResultedURL.Text = "http://android.oms.apps.opera.com/en_us/catalog.php?search=" + txParam1.Text.Trim().Replace(" ", "+").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    break;
            }
        }

        private void OnChangeParam1(object sender, EventArgs e)
        {
            UpdateResultedMarketURL();
        }
    }
}
