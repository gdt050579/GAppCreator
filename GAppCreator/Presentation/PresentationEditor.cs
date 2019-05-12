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
    public partial class PresentationEditor : Form
    {
        PresentationFrameViewer FrameViewer;
        Project prj;
        PresentationResource res;
        ImageList resImagesSmall, resImagesLarge;
        AppResources Resources;

        public PresentationEditor(ProjectContext pContext, PresentationResource ar)
        {
            InitializeComponent();

            ImageSelectorEditor.EditForm = new ResourceSelectDialog(pContext, ResourcesConstantType.Image, true, false);
            FontSelectorEditor.EditForm = new ResourceSelectDialog(pContext, ResourcesConstantType.Font, true, false);
            ShaderSelectorEditor.EditForm = new ResourceSelectDialog(pContext, ResourcesConstantType.Shader, true, false);
            StringSelectorEditor.EditForm = new ResourceSelectDialog(pContext, ResourcesConstantType.String, true, false);

            res = ar;
            prj = pContext.Prj;

            resImagesLarge = pContext.LargeIcons;
            resImagesSmall = pContext.SmallIcons;
            Resources = res.anim.Resources;

            comboRelativeTo.Items.Add(PresentationFrameViewer.RelativeTo.First);
            comboRelativeTo.Items.Add(PresentationFrameViewer.RelativeTo.Last);
            comboRelativeTo.Items.Add(PresentationFrameViewer.RelativeTo.Median);
            comboRelativeTo.Items.Add(PresentationFrameViewer.RelativeTo.Maxim);
            comboRelativeTo.Items.Add(PresentationFrameViewer.RelativeTo.Minim);
            comboRelativeTo.SelectedIndex = 0;

            Resources.Lang = prj.DefaultLanguage;
            Dictionary<Language, bool> d = new Dictionary<Language, bool>();
            foreach (StringValues s in prj.Strings)
            {
                foreach (StringValue sv in s.Values)
                {
                    if ((sv.Value!=null) && (sv.Value.Length>0))
                        d[sv.Language] = true;
                }
            }
            d[prj.DefaultLanguage] = true;
            foreach (Language l in d.Keys)
            {
                comboLanguage.Items.Add(l.ToString());
                if (l == Resources.Lang)
                    comboLanguage.SelectedIndex = comboLanguage.Items.Count - 1;
            }



            FrameViewer = new PresentationFrameViewer(pContext, Resources);
            FrameViewer.ObjectsList = FrameObjectsList;
            FrameViewer.comboRelativeTo = comboRelativeTo;
            FrameViewer.ObjectsProperties = objProp;
            FrameViewer.AnimationProperties = propAnimation;
            FrameViewer.FrameProperties = frameProp;
            FrameViewer.FramesList = lstFrames;
            FrameViewer.LabelInfo = lbInfos;

            panelFrame.Controls.Add(FrameViewer);
            FrameViewer.Left = 0;
            FrameViewer.Top = 0;

            FrameViewer.SetAnimation(res.anim);
        }
        /*
        public void SetAnimation(Animation animObject)
        {
            FrameViewer.SetAnimation(animObject,rContext,gb);
        }
        */
        private void OnSetSelectionMode(object sender, EventArgs e)
        {
            cbModeImage.Checked = (sender == cbModeImage);
            cbModeLine.Checked = (sender == cbModeLine);

            cbModeRectangle.Checked = (sender == cbModeRectangle);
            cbModeSelect.Checked = (sender == cbModeSelect);
            cbModeText.Checked = (sender == cbModeText);
            cbModeClip.Checked = (sender == cbModeClip);

            if (cbModeImage.Checked)
                FrameViewer.mode = PresentationFrameViewer.SelectionMode.Image;
            if (cbModeLine.Checked)
                FrameViewer.mode = PresentationFrameViewer.SelectionMode.Line;
            if (cbModeRectangle.Checked)
                FrameViewer.mode = PresentationFrameViewer.SelectionMode.Rectangle;
            if (cbModeSelect.Checked)
                FrameViewer.mode = PresentationFrameViewer.SelectionMode.Select;
            if (cbModeText.Checked)
                FrameViewer.mode = PresentationFrameViewer.SelectionMode.Text;
            if (cbModeClip.Checked)
                FrameViewer.mode = PresentationFrameViewer.SelectionMode.Clipping;
        }

        private void FrameObjectsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FrameViewer.UpdateSelectionFromObjectList();
        }

        private void objProp_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FrameViewer.UpdateSelectionFromObjectList();
        }

        private void btnMoveZOrderUp_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveObjectsInZOrder(1);
        }

        private void btnMoveZOrderTop_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveObjectsInZOrder(1000000);
        }

        private void btnMoveZOrderDown_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveObjectsInZOrder(-1);
        }

        private void btnMoveZOrderBottom_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveObjectsInZOrder(-1000000);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.DeleteSelectedFrames();
            else
                FrameViewer.DeleteSelected();
        }

        private void btnAlignToBottom_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.Bottom, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void btnAlignToTop_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.Top, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void btnAlignToLeft_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.Left, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void btnAlignToRight_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.Right, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void btnAlignToVerticalCenter_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.CenterX, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void btnAlignToHorizontalCenter_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.CenterY, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void btnMakeTheSameWith_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.Width, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void btnMakeTheSameHeight_Click(object sender, EventArgs e)
        {
            FrameViewer.AlignObjects(AlignComponent.Height, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem);
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void OnAddNewFrame(object sender, EventArgs e)
        {
            FrameViewer.AddNewFrame();
        }

        private void OnFrameSelected(object sender, EventArgs e)
        {
            FrameViewer.OnNewFrameSelected();
        }

        private void AnimationEditor_KeyUp(object sender, KeyEventArgs e)
        {
            //this.Text = e.KeyCode.ToString();
            if ((FrameViewer.Visible) && (FrameViewer.Enabled) && (FrameViewer.Focused))
            {
                if (FrameViewer.sel.IsVisible())
                {
                    switch (e.KeyCode)
                    {
                        case Keys.PageUp: FrameViewer.MoveObjectsInZOrder(1); break;
                        case Keys.PageDown: FrameViewer.MoveObjectsInZOrder(-1); break;
                        case Keys.Home: FrameViewer.MoveObjectsInZOrder(1000000); break;
                        case Keys.End: FrameViewer.MoveObjectsInZOrder(-1000000); break;
                        case Keys.L: FrameViewer.AlignObjects(AlignComponent.Left, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                        case Keys.R: FrameViewer.AlignObjects(AlignComponent.Right, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                        case Keys.T: FrameViewer.AlignObjects(AlignComponent.Top, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                        case Keys.B: FrameViewer.AlignObjects(AlignComponent.Bottom, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                        case Keys.W: FrameViewer.AlignObjects(AlignComponent.Width, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                        case Keys.H: FrameViewer.AlignObjects(AlignComponent.Height, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                        case Keys.V: FrameViewer.AlignObjects(AlignComponent.CenterY, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                    }
                    if (e.Control)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.H: FrameViewer.AlignObjects(AlignComponent.CenterX, (PresentationFrameViewer.RelativeTo)comboRelativeTo.SelectedItem); break;
                        }
                    }
                }
            }
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.N: FrameViewer.AddNewFrame(); break;
                    case Keys.D: FrameViewer.DuplicateCurentFrame(); break;
                    case Keys.S: FrameViewer.SaveToXML(); break;
                    case Keys.C: OnCopyToClipboard(null, null); break;
                    case Keys.X: OnCutToClipboard(null, null); break;
                    case Keys.V: OnPaste(null, null); break;
                    case Keys.G: OnGoToFrame(null, null); break;
                    case Keys.D1: OnCopyToClipboard1(null, null); break;
                    case Keys.D2: OnCopyToClipboard2(null, null); break;
                    case Keys.D3: OnCopyToClipboard3(null, null); break;
                    case Keys.D4: OnCopyToClipboard4(null, null); break;
                    case Keys.D5: OnCopyToClipboard5(null, null); break;
                    case Keys.D6: OnCopyToClipboard6(null, null); break;
                    case Keys.D7: OnCopyToClipboard7(null, null); break;
                    case Keys.D8: OnCopyToClipboard8(null, null); break;
                    case Keys.D9: OnCopyToClipboard9(null, null); break;
                }
            }
            if (e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1: OnPasteFromSlot1(null, null); break;
                    case Keys.D2: OnPasteFromSlot2(null, null); break;
                    case Keys.D3: OnPasteFromSlot3(null, null); break;
                    case Keys.D4: OnPasteFromSlot4(null, null); break;
                    case Keys.D5: OnPasteFromSlot5(null, null); break;
                    case Keys.D6: OnPasteFromSlot6(null, null); break;
                    case Keys.D7: OnPasteFromSlot7(null, null); break;
                    case Keys.D8: OnPasteFromSlot8(null, null); break;
                    case Keys.D9: OnPasteFromSlot9(null, null); break;
                }
            }
            switch (e.KeyCode)
            {
                case Keys.Delete: btnDelete_Click(null, null); break;
                case Keys.F5: OnPlay(null, null); break;
            }
        }

        private void OnDuplicateCurrentFrame(object sender, EventArgs e)
        {
            FrameViewer.DuplicateCurentFrame();
        }

        private void frameProp_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FrameViewer.UpdateSelectedFramesIcon();
        }

        private void btnMoveFramesZOrderUp_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveFramesInZOrder(-1);
        }

        private void btnMoveFrameZOrderTop_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveFramesInZOrder(-1000000);
        }

        private void btnMoveFrameZOrderDown_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveFramesInZOrder(1);
        }

        private void btnMoveFrameZOrderBottom_Click(object sender, EventArgs e)
        {
            FrameViewer.MoveFramesInZOrder(1000000);
        }

        private void OnCopyToClipboard(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(-1);
            else
                FrameViewer.CopyObjectsToClipboard(-1);
        }

        private void OnCopyToClipboard1(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(1);
            else
                FrameViewer.CopyObjectsToClipboard(1);
        }

        private void OnCopyToClipboard2(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(2);
            else
                FrameViewer.CopyObjectsToClipboard(2);
        }

        private void OnCopyToClipboard3(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(3);
            else
                FrameViewer.CopyObjectsToClipboard(3);
        }

        private void OnCopyToClipboard4(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(4);
            else
                FrameViewer.CopyObjectsToClipboard(4);
        }

        private void OnCopyToClipboard5(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(5);
            else
                FrameViewer.CopyObjectsToClipboard(5);
        }

        private void OnCopyToClipboard6(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(6);
            else
                FrameViewer.CopyObjectsToClipboard(6);
        }

        private void OnCopyToClipboard7(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(7);
            else
                FrameViewer.CopyObjectsToClipboard(7);
        }

        private void OnCopyToClipboard8(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(8);
            else
                FrameViewer.CopyObjectsToClipboard(8);
        }

        private void OnCopyToClipboard9(object sender, EventArgs e)
        {
            if (lstFrames.Focused)
                FrameViewer.CopyFramesToClipboard(9);
            else
                FrameViewer.CopyObjectsToClipboard(9);
        }

        private void OnCutToClipboard(object sender, EventArgs e)
        {
            OnCopyToClipboard(sender, e);
            if ((lstFrames.Focused))
                FrameViewer.DeleteSelectedFrames();
            else
                FrameViewer.DeleteSelected();
        }

        private void OnSaveToXML(object sender, EventArgs e)
        {
            FrameViewer.SaveToXML();
        }

        private void OnPaste(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(-1);
        }

        private void OnPasteFromSlot1(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(1);
        }

        private void OnPasteFromSlot2(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(2);
        }

        private void OnPasteFromSlot3(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(3);
        }

        private void OnPasteFromSlot4(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(4);
        }

        private void OnPasteFromSlot5(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(5);
        }

        private void OnPasteFromSlot6(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(6);
        }

        private void OnPasteFromSlot7(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(7);
        }

        private void OnPasteFromSlot8(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(8);
        }

        private void OnPasteFromSlot9(object sender, EventArgs e)
        {
            FrameViewer.PasteFromClipboard(9);
        }

        private void OnSetImage(object sender, EventArgs e)
        {
            FrameViewer.SetImageForObjects();
        }

        private void OnSetOriginalSize(object sender, EventArgs e)
        {
            FrameViewer.ResizeSelectedImagesToOriginalSize();
        }

        private void OnSetString(object sender, EventArgs e)
        {
            FrameViewer.SetStringForObjects();
        }

        private void OnSetBackgroundColor(object sender, EventArgs e)
        {
            if (colorDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FrameViewer.SetBackgroundColor(colorDlg.Color.ToArgb());
            }
        }

        private void OnSetBackgroundImage(object sender, EventArgs e)
        {
            /*
            ResourceSelectDialog dlg = new ResourceSelectDialog(FrameViewer.GetResources(), ResourceSelectDialog.ResourceType.Images);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FrameViewer.SetBackgroundImage(FrameViewer.GetResources().Images[dlg.SelectedResourceName].Value);                
            }
             */
        }

        private void OnClearBackgroundImage(object sender, EventArgs e)
        {
            FrameViewer.SetBackgroundImage(null);
        }

        private void OnSetResourcesLanguage(object sender, EventArgs e)
        {
            /*
            InputBox ib = new InputBox(Language.English);
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FrameViewer.SetLanguage(ib.ResultLanguage);
            }
             */
        }

        private void OnPlay(object sender, EventArgs e)
        {
            FrameViewer.PlayAnimation();
        }

        private void OnSetFont(object sender, EventArgs e)
        {
            FrameViewer.SetFontForObjects();
        }
        private void OnSetShader(object sender, EventArgs e)
        {
            FrameViewer.SetShaderForObjects();
        }

        private void OnChangeLanguage(object sender, EventArgs e)
        {
            Resources.Lang = (Language)Enum.Parse(typeof(Language), comboLanguage.SelectedItem.ToString());
            if (FrameViewer != null)
            {
                FrameViewer.OnChangeLanguage();
                FrameViewer.Refresh();
            }
        }
        private void GoToFrame(int index)
        {
            if ((index < 1) || (index > lstFrames.Items.Count))
            {
                MessageBox.Show("Invalid frame number (should be beetwin 1 and " + lstFrames.Items.Count.ToString() + ")");
                return;
            }
            lstFrames.SelectedItems.Clear();
            lstFrames.Items[index - 1].Selected = true;
            lstFrames.Items[index - 1].EnsureVisible();
        }
        private void OnGoToFrame(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter frame number [1.." + lstFrames.Items.Count.ToString() + "] ", 1, 1, lstFrames.Items.Count);
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GoToFrame((int)ib.FloatResult);
            }
        }

        private void GoToFrameError(int start,int dir)
        {
            prj.EC.Reset();
            ByteBuffer bb = new ByteBuffer(prj.EC);
            GRect tempRect = new GRect();

            ResourcesIndexes ri = res.anim.CreateResourcesIndexes();

            for (int tr = start+dir; (tr < res.anim.Frames.Count) && (tr>=0); tr+=dir)
            {
                foreach (PresentationObject obj in res.anim.Frames[tr].Objects)
                {
                    bb.Clear();
                    obj.ComputeRect(tempRect);
                    obj.AddBinaryBuffer(bb, ri, tempRect, Resources);
                    if (bb.Count() == 0)
                    {
                        prj.EC.Reset();
                        GoToFrame(tr + 1);
                        return;
                    }
                }
            }
            prj.EC.Reset();
            MessageBox.Show("No more objects without reference in this animation found !");
        }

        private void OnGoToNextError(object sender, EventArgs e)
        {
            if (lstFrames.SelectedItems.Count > 0)
                GoToFrameError(lstFrames.SelectedIndices[0], 1);
            else
                GoToFrameError(-1, 1);
        }

        private void OnGoToPreviousError(object sender, EventArgs e)
        {
            if (lstFrames.SelectedItems.Count > 0)
                GoToFrameError(lstFrames.SelectedIndices[0], -1);
            else
                GoToFrameError(lstFrames.Items.Count, 1);

        }

        private void OnCheckForErrors(object sender, EventArgs e)
        {
            res.anim.ExportToBinaryFormat(null, prj.EC);
            if (prj.EC.HasErrors())
            {
                prj.ShowErrors();
                return;
            }
            MessageBox.Show("No errors found !");
        }

        private void OnGoToBookmark(object sender, EventArgs e)
        {
            List<int> l = new List<int>();
            for (int tr = 0; (tr < res.anim.Frames.Count); tr ++)
            {
                if (res.anim.Frames[tr]._Bookmark)
                    l.Add(tr + 1);
            }
            if (l.Count==0)
            {
                MessageBox.Show("No bookmarks defined !");
                return;
            }
            InputBox ib = new InputBox("Enter bookmark number [1.." + l.Count.ToString() + "] ", 1, 1, l.Count);
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GoToFrame(l[(int)ib.FloatResult]);
            }

        }

        private void OnSetTemplateNames(object sender, EventArgs e)
        {
            PresentationResourceKeyEditorDialog dlg = new PresentationResourceKeyEditorDialog(prj, this.res.anim,resImagesSmall,resImagesLarge);
            dlg.ShowDialog();
            //this.res.anim.CreateResourcesIndexes();
        }


    }
}
