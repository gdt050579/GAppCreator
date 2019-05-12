using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GAppCreator
{
    public partial class PreviewSound : PreviewControl
    {
        WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
        public PreviewSound()
        {
            InitializeComponent();
            comboVolume.Items.Add("10%");
            comboVolume.Items.Add("20%");
            comboVolume.Items.Add("30%");
            comboVolume.Items.Add("40%");
            comboVolume.Items.Add("50%");
            comboVolume.Items.Add("60%");
            comboVolume.Items.Add("70%");
            comboVolume.Items.Add("80%");
            comboVolume.Items.Add("90%");
            comboVolume.Items.Add("100%");
            comboVolume.SelectedIndex = 9;
        }
        public override void OnNewPreviewObject()
        {
            lstInfo.Items.Clear();
            lstInfo.Items.Add("Name"); lstInfo.Items[0].SubItems.Add("");
            lstInfo.Items.Add("Duration"); lstInfo.Items[1].SubItems.Add("");
            //lstInfo.Items.Add("Current"); lstInfo.Items[2].SubItems.Add("");

            String path = (String)SelectedObject;
            if (path == null)
                return;
            lstInfo.Items[0].SubItems[1].Text = Path.GetFileName(path);            
            wplayer.URL = path;
            wplayer.settings.volume = (comboVolume.SelectedIndex + 1) * 10;
            wplayer.PlayStateChange += wplayer_PlayStateChange;
            if (btnAutoStart.Checked)
            {
                wplayer.settings.autoStart = true;
                OnPlayMusic(null, null);
            }
            else
                OnStopMusic(null, null);
            this.Invalidate();
        }

        void wplayer_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                OnStopMusic(null, null);
            }
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                lstInfo.Items[1].SubItems[1].Text = wplayer.currentMedia.durationString;
            }
            //lstInfo.Items.Add(NewState.ToString());
        }

        private void OnPlayMusic(object sender, EventArgs e)
        {
            btnStop.Enabled = true;
            btnPlay.Enabled = false;
            wplayer.controls.play();
        }

        private void OnStopMusic(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            btnPlay.Enabled = true;
            wplayer.controls.stop();
        }

        private void comboVolume_SelectedIndexChanged(object sender, EventArgs e)
        {
            wplayer.settings.volume = (comboVolume.SelectedIndex + 1) * 10;
        }

        private void PreviewSound_VisibleChanged(object sender, EventArgs e)
        {
            OnStopMusic(null, null);
        }
    }
}
