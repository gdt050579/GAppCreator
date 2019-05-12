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
    public partial class ResolutionEditorDialog : Form
    {
        bool Landscape = false;
        public static Project prj = null;
        public string SelectedResolution = "";

        public ResolutionEditorDialog(string res)
        {
            InitializeComponent();
            if (prj == null)
                return;
            Size size1 = Project.SizeToValues(prj.DesignResolution);
            Size cSize = Project.SizeToValues(res);
            Landscape = size1.Width >= size1.Height;
            // pun rezolutiile
            comboStandard.Items.Clear();
            foreach (Size sz in Project.Resolutions)
            {
                if (!Landscape)
                    comboStandard.Items.Add(String.Format("{0} x {1}", sz.Height, sz.Width));
                else
                    comboStandard.Items.Add(String.Format("{0} x {1}", sz.Width, sz.Height));
            }
            // pun si chestiile pentru Windows Mode
            comboWindow.Items.Clear();
            comboWindow.Items.Add("Maximized Window");
            comboWindow.Items.Add("Full Screen");
            comboWindow.Items.Add("Best fit");
            // setez elementele
            if (cSize.Width == Project.ScreenResolutionMaximizedWindow)
            {
                comboWindow.SelectedIndex = 0;
                rbWindow.Checked = true;
                return;
            }
            if (cSize.Width == Project.ScreenResolutionFullScreen)
            {
                comboWindow.SelectedIndex = 1;
                rbWindow.Checked = true;
                return;
            }
            if (cSize.Width == Project.ScreenResolutionBestFit)
            {
                comboWindow.SelectedIndex = 2;
                rbWindow.Checked = true;
                return;
            }
            string s = String.Format("{0} x {1}", cSize.Width, cSize.Height);
            for (int tr=0;tr<comboStandard.Items.Count;tr++)
            {
                if (s.Equals(comboStandard.Items[tr].ToString()))
                {
                    comboStandard.SelectedIndex = tr;
                    rbStandard.Checked = true;
                    return;
                }
            }
            // altfel e ceva custom
            txCustom.Text = s;
            rbCustom.Checked = true;
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (rbCustom.Checked)
            {
                Size sz = Project.SizeToValues(txCustom.Text);
                if (sz.Width<1)
                {
                    MessageBox.Show("Invalid size - it should be in the format of 'width x height' !");
                    txCustom.Focus();
                    return;
                }
                SelectedResolution = String.Format("{0} x {1}", sz.Width, sz.Height);
            }
            if (rbStandard.Checked)
            {
                if (comboStandard.SelectedIndex<0)
                {
                    MessageBox.Show("Please select a standard resolution from the list !");
                    comboStandard.Focus();
                    return;
                }
                SelectedResolution = comboStandard.Items[comboStandard.SelectedIndex].ToString();
            }
            if (rbWindow.Checked)
            {
                switch (comboWindow.SelectedIndex)
                {
                    case 0: SelectedResolution = Project.ResolutionToString(Project.ScreenResolutionMaximizedWindow, Project.ScreenResolutionMaximizedWindow); break;
                    case 1: SelectedResolution = Project.ResolutionToString(Project.ScreenResolutionFullScreen, Project.ScreenResolutionFullScreen); ; break;
                    case 2: SelectedResolution = Project.ResolutionToString(Project.ScreenResolutionBestFit, Project.ScreenResolutionBestFit); ; break;
                    default:
                        MessageBox.Show("Please select a window size from the list !");
                        comboWindow.Focus();
                        return;
                }
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnSelectMode(object sender, EventArgs e)
        {
            txCustom.Enabled = (rbCustom.Checked);
            comboStandard.Enabled = (rbStandard.Checked);
            comboWindow.Enabled = (rbWindow.Checked);
        }
    }
}
