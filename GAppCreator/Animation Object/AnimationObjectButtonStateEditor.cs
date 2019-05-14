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
    public partial class AnimationObjectButtonStateEditor : Form
    {
        private static SolidBrush backFill = new SolidBrush(Color.Black);
        private static string zoom = "100";
        public string Result;
        private AnimO.ButtonFaceContainer btn = new AnimO.ButtonFaceContainer();
        private AppResources resources;
        private AnimO.Canvas canvas = new AnimO.Canvas();
        private AnimO.RuntimeContext executionContext = new AnimO.RuntimeContext();
        private AnimO.RuntimeContext symbolExecutionContext = new AnimO.RuntimeContext();
        private AnimO.RuntimeContext textExecutionContext = new AnimO.RuntimeContext();        
        private AnimO.SimpleButtonBackgroundStyle backStyle = AnimO.SimpleButtonBackgroundStyle.Image;
        private float rectWidth, rectHeight,cScale;
        RectangleF tempRect = new RectangleF();
        private int marginColor = 0;

        public AnimationObjectButtonStateEditor(string format,AppResources _resources, float rectWidthInPixels,float rectHeightInPixels,AnimO.SimpleButtonBackgroundStyle _mode)
        {
            InitializeComponent();
            resources = _resources;
            backStyle = _mode;
            rectWidth = rectWidthInPixels;
            rectHeight = rectHeightInPixels;
            executionContext.X_Percentage = 0.5f;
            executionContext.Y_Percentage = 0.5f;
            executionContext.Align = Alignament.Center;
            btn.CreateFromString(format);
            lbMode.Text = "Background Style\n" + backStyle.ToString();
            propButton.SelectedObject = btn;
            ChangeZoom(zoom);
            propButton_PropertyValueChanged(null, null);
        }

        private void OnOK(object sender, EventArgs e)
        {
            string res = btn.Validate(resources,backStyle);
            if (res!=null)
            {
                MessageBox.Show(res, "Error");
                return;
            }
            Result = btn.CreateString();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnPaintPreview(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(backFill, 0, 0, previewPanel.Width, previewPanel.Height);
            canvas.SetGraphics(g);
            canvas.SetScreen(0, 0, previewPanel.Width, previewPanel.Height, 1.0f);
            executionContext.WidthInPixels = rectWidth;
            executionContext.HeightInPixels = rectHeight;
            executionContext.ScreenRect.X = previewPanel.Width / 2 - rectWidth / 2;
            executionContext.ScreenRect.Y = previewPanel.Height / 2 - rectHeight / 2;
            executionContext.ScreenRect.Width = rectWidth;
            executionContext.ScreenRect.Height = rectHeight;
            btn.InitRuntimeContext(executionContext, symbolExecutionContext,textExecutionContext, backStyle);
            AnimO.ButtonFaceContainer.Paint(canvas, tempRect, executionContext, symbolExecutionContext,textExecutionContext, btn.tp, backStyle, marginColor);
            //btn.Paint(canvas, executionContext,tempRect,cScale,rectWidth/previewPanel.Width,rectHeight/previewPanel.Height,backStyle);
        }

        private void OnSetBackgroundColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = backFill.Color;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                backFill.Color = dlg.Color;
                previewPanel.Invalidate();
            }
        }
        private void ChangeMarginColor(string colorName)
        {
            foreach (var a in mnuMarginColor.DropDownItems)
            {
                var si = a as ToolStripMenuItem;
                if (si != null)
                {
                    if (si.Text.Equals(colorName))
                        si.Checked = true;
                    else
                        si.Checked = false;
                }
            }
            if (colorName.Equals("No color"))
                marginColor = 0;
            if (colorName.Equals("Black"))
                marginColor = Color.FromArgb(255,0,0,0).ToArgb();
            if (colorName.Equals("White"))
                marginColor = Color.FromArgb(255, 255, 255, 255).ToArgb();
            if (colorName.Equals("Red"))
                marginColor = Color.FromArgb(255, 255, 0, 0).ToArgb();
            if (colorName.Equals("Yellow"))
                marginColor = Color.FromArgb(255, 255, 255, 0).ToArgb();
            if (colorName.Equals("Green"))
                marginColor = Color.FromArgb(255, 0, 255, 0).ToArgb();
            if (colorName.Equals("Gray"))
                marginColor = Color.FromArgb(255, 128, 128, 128).ToArgb();

            previewPanel.Invalidate();
        }
        private void ChangeZoom(string value)
        {
            int val = 0;
            if (int.TryParse(value,out val))
            {
                zoom = value;
                cScale = ((float)val) / 100.0f;
                previewPanel.Invalidate();
                foreach (var a in mnuZoom.DropDownItems)
                {
                    var si = a as ToolStripMenuItem;
                    if (si != null)
                    {
                        if (si.Text == zoom.ToString() + "%")
                            si.Checked = true;
                        else
                            si.Checked = false;
                    }
                }
            }
        }

        private void OnChangeMarginColor(object sender, EventArgs e)
        {
            var itm = sender as ToolStripMenuItem;
            if (itm != null)
            {
                ChangeMarginColor(itm.Text);
            }
        }

        private void OnChangeZoom(object sender, EventArgs e)
        {
            var itm = sender as ToolStripMenuItem;
            if (itm!=null)
            {
                ChangeZoom(itm.Text.Replace("%", "").Trim());
            }
        }

        private void propButton_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string res = btn.Validate(resources,backStyle);
            if (res == null)
                lbError.Text = "All ok";
            else
                lbError.Text = res;
            previewPanel.Invalidate();
        }
    }
}
