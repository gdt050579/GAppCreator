using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace GAppCreator
{
    public partial class PresentationPlayerDialog : Form
    {
        Presentation Anim;
        int curentFrame = 0, RemainintTicks;
        System.Drawing.Bitmap screenBitmap;
        System.Drawing.Graphics screenGraphcs;
        AnimO.Canvas canvas;
        AppResources Resources;

        public PresentationPlayerDialog(Presentation anim, AppResources r)
        {
            InitializeComponent();
            EnablePlayControls(true);
            Anim = anim;
            Resources = r;
            screenBitmap = new System.Drawing.Bitmap(Anim.Width, Anim.Height);
            screenGraphcs = System.Drawing.Graphics.FromImage(screenBitmap);
            screenGraphcs.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            screenGraphcs.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            canvas = new AnimO.Canvas();
            canvas.SetGraphics(screenGraphcs);
                        
            //this.Width = Anim.Width;
            //this.Height = Anim.Height;
            this.SetClientSizeCore(Anim.Width, Anim.Height);
            lbTotalFrames.Text = "/" + Anim.Frames.Count.ToString();
            SetFrame(0);

            // creez si combo-ul
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                comboLanguage.Items.Add(l);
            }
            comboLanguage.SelectedIndex = 0;
            ComputeTotalTime();
        }

        private void AnimationPlayerDialog_Paint(object sender, PaintEventArgs e)
        {
            if ((Anim != null) && (curentFrame >= 0) && (curentFrame < Anim.Frames.Count))
            {
                PresentationFrame frame = Anim.Frames[curentFrame];
                canvas.SetScreen(0, 0, Width, Height, 1.0f);
                canvas.ClearScreen(Anim.BackgroundColor);
                if (Anim.BackgroundImage != null)
                    canvas.FillScreen(Anim.BackgroundImage.__internal__image);
                frame.Paint(canvas,Resources);
                e.Graphics.DrawImage(screenBitmap, 0, 0);
            }            
        }
        private void EnablePlayControls(bool value)
        {
            btnPlay.Enabled = value;
            btnStop.Enabled = !value;
            btnNextFrame.Enabled = value;
            btnPreviousFrame.Enabled = value;
            txCurrentFrame.Enabled = value;
            txInterval.Enabled = value;
            lbInterval.Enabled = value;
            lbTotalFrames.Enabled = value;
            comboLanguage.Enabled = value;
            animTimer.Enabled = !value;
            lbTotalTime.Enabled = !value;
            cbLoop.Enabled = value;
            
            if (value)
                panelCommands.Visible = true;
        }
        private int StringToInt(string text, int defaultValue)
        {
            int result = defaultValue;
            if (int.TryParse(text, out result) == false)
                return defaultValue;
            return result;
        }
        private void SetFrame(int index)
        {
            if ((Anim != null) && (Anim.Frames.Count>0))
            {
                if (index < 0) 
                    index = 0;
                if (index >= Anim.Frames.Count)
                    index = Anim.Frames.Count - 1;
                curentFrame = index;
                txCurrentFrame.Text = (index + 1).ToString();
                RemainintTicks = Anim.Frames[index]._Time;
                Invalidate();
            }
        }
        private void OnPlay(object sender, EventArgs e)
        {
            EnablePlayControls(false);
        }

        private void OnStop(object sender, EventArgs e)
        {
            EnablePlayControls(true);
        }

        private void OnTimer(object sender, EventArgs e)
        {
            RemainintTicks--;
            if (RemainintTicks <= 0)
            {
                if ((curentFrame + 1) >= Anim.Frames.Count)
                {
                    if (cbLoop.Checked)
                        SetFrame(0);
                    else
                        OnStop(null, null);
                }
                else
                    SetFrame(curentFrame + 1);
            }
        }

        private void txInterval_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Return)
            {
                int time = StringToInt(txInterval.Text,-1);
                if ((time < 30) || (time > 5000))
                {
                    MessageBox.Show("Time interval should be a value between 30 and 5000");
                }
                else
                {
                    animTimer.Interval = time;
                }
                e.Handled = true;
                ComputeTotalTime();
            }
            
        }

        private void txCurrentFrame_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Return) && (Anim!=null))
            {
                int id = StringToInt(txCurrentFrame.Text, -1);
                if ((id < 1) || (id > Anim.Frames.Count))
                {
                    MessageBox.Show("Frame index should be between 1 and "+Anim.Frames.Count.ToString());
                }
                else
                {
                    curentFrame = (id - 1);
                    this.Invalidate();
                }
                e.Handled = true;
            }
        }

        private void OnPreviousFrame(object sender, EventArgs e)
        {
            SetFrame(curentFrame - 1);
        }

        private void OnNextFrame(object sender, EventArgs e)
        {
            SetFrame(curentFrame + 1);
        }

        private void AnimationPlayerDialog_MouseMove(object sender, MouseEventArgs e)
        {
            if (animTimer.Enabled)
            {
                panelCommands.Visible = (e.Y < 30);                    
            }
        }

        private void OnChangeLanguage(object sender, EventArgs e)
        {
            /*
            if ((gb!=null) && (comboLanguage.SelectedItem!=null))
            {
                gb.UpdateResourcesContextsLanguageForStrings((Language)comboLanguage.SelectedItem);
                this.Invalidate();
            }
             */
        }
        private void ComputeTotalTime()
        {
            int totalCount = 0;
            if ((Anim != null) && (Anim.Frames.Count > 0))
            {
                for (int tr = 0; tr < Anim.Frames.Count; tr++)
                    totalCount += Anim.Frames[tr]._Time;
            }
            totalCount *= animTimer.Interval;
            int ms = totalCount % 1000;
            totalCount /= 1000;
            int h = totalCount / 3600;
            totalCount %= 3600;
            int m = totalCount / 60;
            int s = totalCount % 60;
            lbTotalTime.Text = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:d3}", h, m, s, ms);
        }
    }
}
