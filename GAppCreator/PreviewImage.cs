using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class PreviewImage : PreviewControl
    {
        int X1, Y1, X2, Y2;
        SolidBrush myBrush = new SolidBrush(Color.FromArgb(192, Color.White));
        public PreviewImage()
        {
            InitializeComponent();
            X1 = X2 = Y1 = Y2 = -1;
        }
        private void OnPaintRestOfBackground(object sender, PaintEventArgs e)
        {
            if (cbBlack.Visible)
                e.Graphics.FillRectangle(Brushes.Black, 0, 0, pnlScroll.Width, pnlScroll.Height);
            else if (cbWhite.Visible)
                e.Graphics.FillRectangle(Brushes.White, 0, 0, pnlScroll.Width, pnlScroll.Height);
            else
                e.Graphics.FillRectangle(Brushes.Gray, 0, 0, pnlScroll.Width, pnlScroll.Height);
        }
        private void OnDrawImage(object sender, PaintEventArgs e)
        {
            Bitmap img = null;
            if (SelectedObject != null)
                img = (Bitmap)SelectedObject;
            if (img != null)
            {
                if (cbBlack.Visible)
                    e.Graphics.FillRectangle(Brushes.Black, 0, 0, pnlPreviewImage.Width, pnlPreviewImage.Height);
                else if (cbWhite.Visible)
                    e.Graphics.FillRectangle(Brushes.White, 0, 0, pnlPreviewImage.Width, pnlPreviewImage.Height);
                else
                    e.Graphics.FillRectangle(Brushes.Gray, 0, 0, pnlPreviewImage.Width, pnlPreviewImage.Height);
                e.Graphics.DrawImage(img, 0, 0, pnlPreviewImage.Width, pnlPreviewImage.Height);
                if (X1>=0)
                {
                    int l = X1;
                    int t = Y1;
                    int r = X2;
                    int b = Y2;
                    if (X2 < X1) { l = X2; r = X1; }
                    if (Y2 < Y1) { t = Y2; b = Y1; }
                    e.Graphics.FillRectangle(myBrush, l, t, r-l, b-t);
                    e.Graphics.DrawRectangle(Pens.Black, l, t, r - l, b - t);
                }
            }
        }

        private void zoomValue_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedObject != null)
            {
                float scale = 0.0f;
                if (cb10.Checked) scale = 0.1f;
                if (cb25.Checked) scale = 0.25f;
                if (cb50.Checked) scale = 0.5f;
                if (cb100.Checked) scale = 1;
                if (cb200.Checked) scale = 2;
                if (cb300.Checked) scale = 3;
                if (cb400.Checked) scale = 4;
                if (cb500.Checked) scale = 5;
                if (cb800.Checked) scale = 8;

                // daca sunt auto - fac calculele
                Bitmap img = ((Bitmap)SelectedObject);
                float rap1 = (float)pnlScroll.Width / (float)img.Width;
                float rap2 = (float)pnlScroll.Height / (float)img.Height;

                if (rap1 > rap2)
                    rap1 = rap2;
                if (rap1 > 1.0f)
                    rap1 = 1.0f;
                if (scale == 0)
                    scale = rap1;

                pnlPreviewImage.Width = (int)(img.Width * scale);
                pnlPreviewImage.Height = (int)(img.Height * scale);
                pnlPreviewImage.Visible = true;
                if (pnlPreviewImage.Width >= pnlScroll.Width)
                    pnlPreviewImage.Left = 0;
                else
                    pnlPreviewImage.Left = (pnlScroll.Width - pnlPreviewImage.Width) / 2;
                if (pnlPreviewImage.Height >= pnlScroll.Height)
                    pnlPreviewImage.Top = 0;
                else
                    pnlPreviewImage.Top = (pnlScroll.Height - pnlPreviewImage.Height) / 2;

                lbResolution.Text = string.Format("{0} x {1}\n{2}%", img.Width, img.Height,(int)(scale*100.0));
            }
            else
            {
                pnlPreviewImage.Visible = false;
                lbPosition.Text = "";
            }
            
            Refresh();
        }

        public override void OnNewPreviewObject()
        {
            if (SelectedObject != null)
            {
                lbResolution.Text = string.Format("{0} x {1}", ((Bitmap)SelectedObject).Width, ((Bitmap)SelectedObject).Height);
            }
            else
            {
                lbResolution.Text = "";
            }
            X1 = X2 = Y1 = Y2 = -1;
            UpdateSelectionInfo();
            lbPosition.Text = "";
            zoomValue_ValueChanged(null, null);
            Invalidate();            
        }

        private void PreviewImage_Enter(object sender, EventArgs e)
        {

        }


        private void OnChangeBackColor(object sender, EventArgs e)
        {
            if (sender==cbBlack)
            {
                cbBlack.Visible = cbWhite.Visible = false;
                cbGray.Visible = true;
            }
            if (sender == cbGray)
            {
                cbBlack.Visible = cbGray.Visible = false;
                cbWhite.Visible = true;
            }
            if (sender == cbWhite)
            {
                cbGray.Visible = cbWhite.Visible = false;
                cbBlack.Visible = true;
            }
            zoomValue_ValueChanged(null, null);
        }

        private void OnChangeMarginVizibility(object sender, EventArgs e)
        {
            if (cbShowMargin.Checked)
                pnlPreviewImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            else
                pnlPreviewImage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            zoomValue_ValueChanged(null, null);
        }

        private void OnChangePreviewImageSize(object sender, EventArgs e)
        {
            zoomValue_ValueChanged(null, null);
        }

        private void OnChangeScale(object sender, EventArgs e)
        {
            cb10.Checked = sender == cb10;
            cb25.Checked = sender == cb25;
            cb50.Checked = sender == cb50;
            cb100.Checked = sender == cb100;
            cb200.Checked = sender == cb200;
            cb300.Checked = sender == cb300;
            cb400.Checked = sender == cb400;
            cb500.Checked = sender == cb500;
            cb800.Checked = sender == cb800;
            cbAuto.Checked = sender == cbAuto;
            zoomValue_ValueChanged(null, null);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (cbShowPixels.Checked)
                lbPosition.Text = "X = " + e.X.ToString() + "\nY = " + e.Y.ToString();
            else
                lbPosition.Text = string.Format("X = {0:0.00} %\nY = {1:0.00} %",e.X*100.0f/pnlPreviewImage.Width,e.Y*100.0f/pnlPreviewImage.Height);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                X2 = e.X;
                Y2 = e.Y;
                pnlPreviewImage.Invalidate();
                UpdateSelectionInfo();
            }
        }
        private void UpdateSelectionInfo()
        {
            if (X1<0)
            {
                lbSelection.Text = "";
                return;
            }
            int l = X1;
            int t = Y1;
            int r = X2;
            int b = Y2;
            if (X2 < X1) { l = X2; r = X1; }
            if (Y2 < Y1) { t = Y2; b = Y1; }
            if (cbShowPixels.Checked)
                lbSelection.Text = string.Format("({0},{1}) - ({2},{3})\nSize: {4} x {5}", l,t,r,b,r-l,b-t);
            else
                lbSelection.Text = string.Format("({0:0.00}%,{1:0.00}%) - ({2:0.00}%,{3:0.00}%)\nSize: {4:0.00}%  x  {5:0.00}%", l * 100.0f / pnlPreviewImage.Width, t * 100.0f / pnlPreviewImage.Height, r * 100.0f / pnlPreviewImage.Width, b * 100.0f / pnlPreviewImage.Height, (r - l) * 100.0f / pnlPreviewImage.Width, (b - t) * 100.0f / pnlPreviewImage.Height);
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                X1 = X2 = Y1 = Y2 = -1;
                pnlPreviewImage.Invalidate();
                UpdateSelectionInfo();
            }
            else
            {
                X1 = X2 = e.X;
                Y1 = Y2 = e.Y;
                pnlPreviewImage.Invalidate();
                UpdateSelectionInfo();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {

        }
        private void OnMouseLeave(object sender, EventArgs e)
        {
            lbPosition.Text = "";
        }
        private void OnChangeMetrics(object sender, EventArgs e)
        {
            cbShowPixels.Checked = sender == cbShowPixels;
            cbShowPercentages.Checked = sender == cbShowPercentages;
            UpdateSelectionInfo();
        }

        private void OnCopyImage(object sender, EventArgs e)
        {
            Bitmap img = null;
            if (SelectedObject != null)
                img = (Bitmap)SelectedObject;
            if (img == null)
            {
                MessageBox.Show("No image to copy !");
                return;
            }
            Clipboard.SetImage(img);
        }

    }
}
