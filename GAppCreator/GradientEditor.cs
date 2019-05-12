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
    public partial class GradientEditor : Form
    {
        ColorGradient cg = new ColorGradient();
        Brush bk = new SolidBrush(Color.White);

        public GradientEditor()
        {
            InitializeComponent();
            cbLinear.Tag = GradientType.Linear;
            cbCircular.Tag = GradientType.Circular;
            SetType(cg.Type);
        }
        private void UpdateListViewItem(int index)
        {
            ListViewItem lvi = lstStops.Items[index];
            lvi.UseItemStyleForSubItems = false;
            lvi.SubItems[0].Text = "";
            lvi.SubItems[0].BackColor = Color.FromArgb(cg.Stops[index].Alpha, cg.Stops[index].Red, cg.Stops[index].Green, cg.Stops[index].Blue);
            lvi.SubItems[1].Text = (cg.Stops[index].Stop * 100.0f).ToString("0.00");
            lvi.SubItems[2].Text = cg.Stops[index].Red.ToString();
            lvi.SubItems[3].Text = cg.Stops[index].Green.ToString();
            lvi.SubItems[4].Text = cg.Stops[index].Blue.ToString();
            lvi.SubItems[5].Text = cg.Stops[index].Alpha.ToString();
        }
        private void UpdateStopList()
        {
            lstStops.Items.Clear();
            propStops.SelectedObject = null;
            propStops.SelectedObjects = null;
            foreach (ColorGradientStop cgs in cg.Stops)
            {
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add(""); lvi.SubItems.Add("");
                lvi.SubItems.Add(""); lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lstStops.Items.Add(lvi);
                UpdateListViewItem(lstStops.Items.Count - 1);
            }
            pnlPreview.Invalidate();
        }
        private void AddNewStop(int index)
        {
            ColorGradientStop cgs = new ColorGradientStop();
            cgs.Red = 0;
            cgs.Alpha = 0;
            cgs.Green = 0;
            cgs.Blue = 0;
            if (index <= 0)
            {
                cgs.Stop = 0;
                cg.Stops.Insert(0,cgs);
            }
            else if (index >= cg.Stops.Count)
            {
                cgs.Stop = 1;
                cg.Stops.Add(cgs);
            }
            else
            {
                cgs.Stop = (cg.Stops[index - 1].Stop + cg.Stops[index].Stop) / 2.0f;
                cg.Stops.Insert(index, cgs);
            }
            UpdateStopList();    
        }
        private void OnPreviewGradient(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            e.Graphics.FillRectangle(bk, 0, 0, p.Width, p.Height);
            int cx = p.Width / 2;
            int cy = p.Height / 2;

            // pentru linear
            int cPoz = 0;
            float proc;
            ColorGradientStop c1, c2;
            if ((cg.Stops.Count > 1) && (cg.Type == GradientType.Linear))
            {
                for (int tr = 0; tr < 100; tr++)
                {
                    c1 = cg.Stops[cPoz];
                    c2 = cg.Stops[cPoz + 1];
                    if (c1.Stop >= c2.Stop)
                        return;
                    proc = (float)tr / 100.0f;
                    proc = (proc - c1.Stop) / (c2.Stop - c1.Stop);
                    int r = (int)(c2.Red * proc + c1.Red * (1 - proc));
                    int g = (int)(c2.Green * proc + c1.Green * (1 - proc));
                    int b = (int)(c2.Blue * proc + c1.Blue * (1 - proc));
                    int a = (int)(c2.Alpha * proc + c1.Alpha * (1 - proc));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(a, r, g, b)), cx - 50 + tr, cy - 50, cx - 50 + tr, cy + 50);
                    if ((float)(tr+1) >= (c2.Stop * 100.0f))
                        cPoz++;
                    if (cPoz >= cg.Stops.Count)
                        return;
                }
            }

            if ((cg.Stops.Count > 1) && (cg.Type == GradientType.Circular))
            {
                for (int tr = 0; tr < 50; tr++)
                {
                    c1 = cg.Stops[cPoz];
                    c2 = cg.Stops[cPoz + 1];
                    if (c1.Stop >= c2.Stop)
                        return;
                    proc = (float)tr / 50.0f;
                    // pe procente mici nu e ok - ar trebui ceva facut in felul urmator
                    /*
                    while ((cPoz+1<cg.Stops.Count) && (cg.Stops[cPoz + 1].Stop<proc)) { cPoz++; }
                     * 
                     */
                    proc = (proc - c1.Stop) / (c2.Stop - c1.Stop);
                    int r = (int)(c2.Red * proc + c1.Red * (1 - proc));
                    int g = (int)(c2.Green * proc + c1.Green * (1 - proc));
                    int b = (int)(c2.Blue * proc + c1.Blue * (1 - proc));
                    int a = (int)(c2.Alpha * proc + c1.Alpha * (1 - proc));
                    e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(a, r, g, b)), cx - 50 + tr, cy - 50 + tr, (100-tr*2),(100-tr*2));                        
                    if ((float)(tr + 1) >= (c2.Stop * 50.0f))
                        cPoz++;
                    if (cPoz >= cg.Stops.Count)
                        return;
                }
            }
        }

        private void OnAddLastStop(object sender, EventArgs e)
        {
            AddNewStop(cg.Stops.Count);
        }

        private void OnAddFirstStop(object sender, EventArgs e)
        {
            AddNewStop(0);
        }

        private void OnAddBeforCurrentStop(object sender, EventArgs e)
        {
            if (lstStops.SelectedIndices.Count != 1)
            {
                MessageBox.Show("Please select one 'Stop' to act as a reference for the new one !");
                return;
            }
            AddNewStop(lstStops.SelectedIndices[0]);
        }

        private void OnAddAfterCurrentStop(object sender, EventArgs e)
        {
            if (lstStops.SelectedIndices.Count != 1)
            {
                MessageBox.Show("Please select one 'Stop' to act as a reference for the new one !");
                return;
            }
            AddNewStop(lstStops.SelectedIndices[0]+1);
        }

        private void OnStopSelected(object sender, EventArgs e)
        {
            if (lstStops.SelectedItems.Count>0)
            {
                object[] o = new object[lstStops.SelectedItems.Count];
                for (int tr = 0; tr < lstStops.SelectedIndices.Count; tr++)
                    o[tr] = cg.Stops[lstStops.SelectedIndices[tr]];
                propStops.SelectedObjects = o;
            } 
            else {
                propStops.SelectedObjects = null;
            }
        }

        private void OnStopIsChanged(object s, PropertyValueChangedEventArgs e)
        {
            for (int tr = 0; tr < lstStops.SelectedIndices.Count; tr++)
                UpdateListViewItem(lstStops.SelectedIndices[tr]);
            pnlPreview.Invalidate();
        }

        private void OnDeleteStops(object sender, EventArgs e)
        {
            if (lstStops.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Delete " + lstStops.SelectedItems.Count.ToString() + " gradient items ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    ColorGradientStop[] toDelete = new ColorGradientStop[lstStops.SelectedIndices.Count];
                    for (int tr = 0; tr < lstStops.SelectedIndices.Count; tr++)
                        toDelete[tr] = cg.Stops[lstStops.SelectedIndices[tr]];
                    for (int tr = 0; tr < toDelete.Length; tr++)
                        cg.Stops.Remove(toDelete[tr]);
                    UpdateStopList();
                }
            }
            else
            {
                MessageBox.Show("You have to have at least one gradient item selected to delete it !","Delete");
            }
        }

        private void OnSelectColor(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.AnyColor = true;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int tr = 0; tr < lstStops.SelectedIndices.Count; tr++)
                {
                    cg.Stops[lstStops.SelectedIndices[tr]].Red = cd.Color.R;
                    cg.Stops[lstStops.SelectedIndices[tr]].Green = cd.Color.G;
                    cg.Stops[lstStops.SelectedIndices[tr]].Blue = cd.Color.B;
                    cg.Stops[lstStops.SelectedIndices[tr]].Alpha = cd.Color.A;
                }
                UpdateStopList();
            }
        }

        private void OnDistributeUniformly(object sender, EventArgs e)
        {
            if (cg.Stops.Count == 0)
                return;
            if (cg.Stops.Count == 1)
            {
                cg.Stops[0].Stop = 0;
                return;
            }
            if (cg.Stops.Count == 2)
            {
                cg.Stops[0].Stop = 0;
                cg.Stops[1].Stop = 1;
                return;
            }
            float pas = 1.0f / (float)(cg.Stops.Count - 1);
            cg.Stops[0].Stop = 0;
            for (int tr = 1; tr < cg.Stops.Count; tr++)
                cg.Stops[tr].Stop = cg.Stops[tr - 1].Stop + pas;
            cg.Stops[cg.Stops.Count - 1].Stop = 1;
            UpdateStopList();
        }

        private void OnChangeBackColor(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.AnyColor = true;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bk = new SolidBrush(cd.Color);
                pnlPreview.Invalidate();
            }
        }
        private void SetType(GradientType gt)
        {
            cbLinear.Checked = (((GradientType)cbLinear.Tag) == gt);
            cbCircular.Checked = (((GradientType)cbCircular.Tag) == gt);
            cg.Type = gt;
            pnlPreview.Invalidate();
        }
        private void OnValidate(object sender, EventArgs e)
        {
            if (cg.Stops.Count < 2)
            {
                MessageBox.Show("At least 2 stops need to be added !");
                return;
            }
            for (int tr=0;tr<cg.Stops.Count-1;tr++)
                if (cg.Stops[tr].Stop >= cg.Stops[tr + 1].Stop)
                {
                    MessageBox.Show(String.Format("Stop #{0} should have a 'Stop' property smaller than of the Stop #{1} ", tr, tr + 1));
                    return;
                }
            if (cg.Stops[0].Stop != 0)
            {
                MessageBox.Show("First stop should have 'Stop' property equal to 0");
                return;
            }
            if (cg.Stops[cg.Stops.Count - 1].Stop != 1)
            {
                MessageBox.Show("Last stop should have 'Stop' property equal to 1");
                return;
            }
            if (txName.Text.Trim().Length == 0)
            {
                MessageBox.Show("No name added for this gradient !");
                txName.Focus();
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void OnChangeGradientType(object sender, EventArgs e)
        {
            SetType((GradientType)(((ToolStripMenuItem)sender).Tag));
        }

        private void OnDistributeAlphaAfterStop(object sender, EventArgs e)
        {
            for (int tr = 0; tr < cg.Stops.Count; tr++)
                cg.Stops[tr].Alpha = (byte)(255.0f * cg.Stops[tr].Stop);
            UpdateStopList();
        }

        private void OnDistributeRedAfterStop(object sender, EventArgs e)
        {
            for (int tr = 0; tr < cg.Stops.Count; tr++)
                cg.Stops[tr].Red = (byte)(255.0f * cg.Stops[tr].Stop);
            UpdateStopList();
        }

        private void OnDistributeGreenAfterStop(object sender, EventArgs e)
        {
            for (int tr = 0; tr < cg.Stops.Count; tr++)
                cg.Stops[tr].Green = (byte)(255.0f * cg.Stops[tr].Stop);
            UpdateStopList();
        }

        private void OnDistributeBlueAfterStop(object sender, EventArgs e)
        {
            for (int tr = 0; tr < cg.Stops.Count; tr++)
                cg.Stops[tr].Blue = (byte)(255.0f * cg.Stops[tr].Stop);
            UpdateStopList();
        }
    }
}
