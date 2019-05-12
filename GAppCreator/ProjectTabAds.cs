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
    public partial class ProjectTabAds : BaseProjectContainer
    {
        GListView lstAds;
        public ProjectTabAds()
        {
            InitializeComponent();

            lstAds = new GListView();
            lstAds.AddColumn("Name", "propName", 400, GListView.RenderType.ItemRenderer, true, HorizontalAlignment.Left);
            lstAds.AddColumn("Provider", "propProvider", 100, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstAds.AddColumn("Builds", "propBuilds", 200, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstAds.AddColumn("Type", "propType", 150, GListView.RenderType.Default, true, HorizontalAlignment.Left);
            lstAds.AddColumn("Load On Startup", "propLoadOnStartup", 100, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstAds.AddColumn("Reload after open", "propReLoadAfterOpen", 100, GListView.RenderType.BooleanCheckBox, true, HorizontalAlignment.Center);
            lstAds.RowHeight = 34 + 14;
            lstAds.OnObjectsSelected += lstAds_OnObjectsSelected;

            pnlAds.Panel2.Controls.Add(lstAds);
            lstAds.Dock = DockStyle.Fill;
        }

        void lstAds_OnObjectsSelected(object source, bool selected, System.Collections.IList SelectedObjects)
        {
            propAds.SelectedObjects = lstAds.GetSelectedObjectsArray();
        }

        private void propAds_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            lstAds.RefreshSelectedObjects();
        }


        public override void OnActivate()
        {
            lstAds.SetObjects(Context.Prj.Ads);
        }

        private void OnDeleteAd(object sender, EventArgs e)
        {
            if (lstAds.GetCurrentSelectedObjectsListCount() != 1)
            {
                MessageBox.Show("You have to select one Ad before deletion !");
                return;
            }
            GenericAd ad = (GenericAd)lstAds.GetCurrentSelectedObject();
            if (MessageBox.Show("Are you sure do you want to delete " + ad.Name + " ?", "Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            Context.Prj.Ads.Remove(ad);
            lstAds.SetObjects(Context.Prj.Ads);

        }
        private void AddNewAd(GenericAd ad)
        {
            ad.Name = "Ad_" + (Context.Prj.Ads.Count + 1).ToString();
            Context.Prj.Ads.Add(ad);
            lstAds.SetObjects(Context.Prj.Ads);
        }
        private void OnAddGoogleAdMobBanner(object sender, EventArgs e)
        {
            AddNewAd(new GoogleAdMobBanner());
        }
        private void OnAddGoogleAdMobInterstitial(object sender, EventArgs e)
        {
            AddNewAd(new GoogleAdMobInterstitial());
        }

        private void OnAddGoogleAdMobRewardable(object sender, EventArgs e)
        {
            AddNewAd(new GoogleAdMobRewardable());
        }
        private void OnAddGoogleAdMobNativeExpress(object sender, EventArgs e)
        {
            AddNewAd(new GoogleAdMobNativeExpress());
        }
        private void OnAddChartboostInterstitial(object sender, EventArgs e)
        {
            AddNewAd(new ChartboostInterstitial());
        }

        private void OnAddChartboostRewardable(object sender, EventArgs e)
        {
            AddNewAd(new ChartboostRewardable());
        }
        private void OnAddChartboostInPlay(object sender, EventArgs e)
        {
            AddNewAd(new ChartboostInPlay());
        }

        private void OnDuplicateAd(object sender, EventArgs e)
        {
            if (lstAds.GetCurrentSelectedObjectsListCount() != 1)
            {
                MessageBox.Show("Please select only one Ad to be duplicated !");
                lstAds.Focus();
                return;
            }
            GenericAd ad = (GenericAd)lstAds.GetCurrentSelectedObject();
            GenericAd newAd = ad.Duplicate();
            if (newAd == null)
            {
                MessageBox.Show("Internal error - ad of type '" + ad.GetType().ToString() + "' does not support duplication !");
                return;
            }
            Context.Prj.Ads.Add(newAd);
            lstAds.SetObjects(Context.Prj.Ads);
        }





    }
}
