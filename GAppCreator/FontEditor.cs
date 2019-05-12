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
    public partial class FontEditor : Form
    {
        CharacterSet cSet = new CharacterSet();
        FontResource tempFont = new FontResource();
        FontResource originalFont;
        Project prj;
        bool populateList = false;

        Dictionary<int, bool> FilteredCodes = new Dictionary<int, bool>();
        bool KeepCharactersFromFilteredCodes = true;

        SolidBrush baseLineBrush = new SolidBrush(Color.White);
        Pen baseLinePen = new Pen(Color.Black);
        Glyph baseLineGlyph = null;

        SolidBrush previewTextLineBrush = new SolidBrush(Color.White);
        Pen previewTextLinePen = new Pen(Color.Black);
        Font previewFont = new Font("Arial", 12);
        StringFormat centerTextSF = new StringFormat();

        public FontEditor(Project p,FontResource fnt)
        {
            prj = p;
            originalFont = fnt;
            tempFont.DuplicateFrom(fnt);
            InitializeComponent();
            propFont.SelectedObject = tempFont;

            // Languages
            comboLanguage.Items.Add("All");
            foreach (string name in Enum.GetNames(typeof(Language)))
            {
                comboLanguage.Items.Add(name);
            }
            comboLanguage.SelectedIndex = 0;

            // Builds
            foreach (GenericBuildConfiguration gb in prj.BuildConfigurations)
                comboBuilds.Items.Add(gb.Name);
            comboBuilds.SelectedIndex = 0;

            centerTextSF.Alignment = StringAlignment.Center;
            centerTextSF.LineAlignment = StringAlignment.Center;

            // initializari pentru design rezolution
            FontVersionEditor.prj = prj;            
            if (fnt.CheckFontFolders()==false)
                fnt.prj.ShowErrors();

            RefreshGlyphIcons();
            PopulateList();
            SetCurrentGlyph(null);
        }
        public void RefreshGlyphIcons()
        {
            glyphIcons.Images.Clear();
            foreach (Glyph g in tempFont.Glyphs)
            {
                if (g.Picture != null)
                {
                    glyphIcons.Images.Add(g.Code.ToString(), Project.ImageToIcon(prj.EC,(Bitmap)g.Picture,16,16));
                }
            }
        }
        private void RefreshPreviewImages()
        {
            pnlPreviewText.Invalidate();
            pnlEditBaseline.Invalidate();
        }
        private void UpdateListViewItem(ListViewItem lvi)
        {
            bool originalPopulateList = populateList;
            populateList = true;
            // setez baseline-ul
            Glyph g = (Glyph)lvi.Tag;
            lvi.SubItems[5].Text = g.Template;
            lvi.SubItems[6].Text = Project.ProcentToString(g.BaseLine);
            Glyph.GlyphVersionInfo gvi = g.GetVersion();
            string bld = (string)comboBuilds.SelectedItem;
            if ((bld!=null) && (gvi.Builds.ContainsKey(bld.ToLower())))
            {
                string s = "";
                bld = bld.ToLower();
                foreach (string ss in gvi.Builds[bld])
                    s += ss + " , ";
                if (s.EndsWith(" , "))
                    s = s.Substring(0, s.Length - 3);
                lvi.SubItems[7].Text = s;
                lvi.ForeColor = Color.Black;
            }
            else
            {
                lvi.SubItems[7].Text = "Not available !";
                lvi.ForeColor = Color.Red;
            }
            populateList = originalPopulateList;
        }
        public void PopulateList()
        {
            populateList = true;
            lstGlyphs.BeginUpdate();
            lstGlyphs.Items.Clear();
            propGlyph.SelectedObject = null;
            propGlyph.SelectedObjects = null;
            baseLineGlyph = null;
            RefreshPreviewImages();
            previewGlyph.SetPreviewObject(prj, null, null);

            Language filter_lng = Language.English;
            if (comboLanguage.SelectedIndex > 0)
                filter_lng = (Language)Enum.Parse(typeof(Language), (string)comboLanguage.SelectedItem);
            int count = 0;
            foreach (Glyph g in tempFont.Glyphs)
            {
                if (cbOnlyCharsFromAppStrings.Checked)
                {
                    if (KeepCharactersFromFilteredCodes)
                    {
                        if (FilteredCodes.ContainsKey(g.Code) == false)
                            continue;
                    }
                    else
                    {
                        if (FilteredCodes.ContainsKey(g.Code) == true)
                            continue;
                    }
                }
                CharacterSet.Info cInfo = cSet.Get(g.Code);
                string languages = "";
                bool skip = false;
                if (cInfo != null)
                {
                    bool found = false;
                    foreach (Language lng in cInfo.Languages)
                    {
                        found |= (lng == filter_lng);
                        languages += lng.ToString() + ", ";
                    }
                    if ((comboLanguage.SelectedIndex > 0) && (!found))
                        continue;
                    if (languages.EndsWith(", "))
                        languages = languages.Substring(0, languages.Length - 2);
                    if (cInfo.DefaultForEveryLanguage)
                    {
                        if (cbShowDefault.Checked == false)
                            continue;
                    }
                    else
                    {
                        if (cbShowNonDefault.Checked == false)
                            continue;
                    }
                    switch (cInfo.Type)
                    {
                        case CharacterSet.CharacterType.Digit: skip |= (!cbShowDigits.Checked); break;
                        case CharacterSet.CharacterType.LowerCase: skip |= (!cbShowLower.Checked); break;
                        case CharacterSet.CharacterType.UpperCase: skip |= (!cbShowUpper.Checked); break;
                        case CharacterSet.CharacterType.Punctuation: skip |= (!cbShowPunctiation.Checked); break;
                        default: skip |= (!cbShowOtherTypes.Checked); break;
                    };
                }
                else
                {
                    if (cbShowCustomChars.Checked == false)
                        continue;
                }
                if (skip == true)
                    continue;

                ListViewItem lvi = new ListViewItem("" + (char)g.Code);
                
                lvi.SubItems.Add(g.Code.ToString());
                lvi.SubItems.Add("0x" + g.Code.ToString("X4"));
                lvi.ImageKey = g.Code.ToString();
                if (cInfo!=null)
                {
                    if (cInfo.DefaultForEveryLanguage)
                        lvi.SubItems.Add("YES");
                    else
                        lvi.SubItems.Add("-");
                    lvi.SubItems.Add(cInfo.Type.ToString());
                    lvi.SubItems.Add(g.Template);
                    lvi.SubItems.Add(""); // baseline
                }
                else
                {
                    lvi.SubItems.Add("-");
                    lvi.SubItems.Add("?");
                    lvi.SubItems.Add(g.Template);
                    lvi.SubItems.Add(""); // baseline
                }
                lvi.Tag = g;
                lvi.SubItems.Add(""); // rezolutii
                lvi.SubItems.Add(languages);
                UpdateListViewItem(lvi);
                lstGlyphs.Items.Add(lvi);
                count++;
            }
            lstGlyphs.EndUpdate();
            lbInfo.Text = count.ToString() + " filtered from " + tempFont.Glyphs.Count.ToString();
            populateList = false;
        }

        private void OnAddCharacters(object sender, EventArgs e)
        {
            FontGlyphCreator dlg = new FontGlyphCreator(prj, tempFont);
            if (dlg.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                ProgressDialog pdlg = new ProgressDialog(prj);
                pdlg.SetFontGlyphCreator(dlg.CodeList, dlg.TemplateSVG, tempFont);
                pdlg.Start();
                // adaug pe cele care chiar au fost create
                foreach (int code in dlg.CodeList)
                    tempFont.AddGlyph(code,dlg.TemplateName);
                tempFont.SortGlyphs();
                // fac build la cele chiar au fost adaugate
                pdlg.SetFontGlyphBuild(dlg.CodeList, tempFont);
                pdlg.Start();
                // incarc si imaginile
                tempFont.LoadGlyphs(dlg.CodeList);
                if (prj.EC.HasErrors())
                    prj.ShowErrors();
                RefreshGlyphIcons();
                PopulateList();
                RefreshPreviewImages();
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (tempFont.ValidateFont(false) == false)
            {
                prj.ShowErrors();
                return;
            }
            // validare mai intai
            originalFont.DuplicateFrom(tempFont);
            originalFont.UpdateFontBuilds();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
        
        private void OnChangeSelectedGlyph(object sender, EventArgs e)
        {
            SetCurrentGlyph(null);
            if (lstGlyphs.SelectedItems.Count>0)
            {
                if (lstGlyphs.SelectedItems.Count==1)
                {
                    SetCurrentGlyph((Glyph)lstGlyphs.SelectedItems[0].Tag);
                }
                else
                {
                    Glyph[] l = new Glyph[lstGlyphs.SelectedItems.Count];
                    int index = 0;
                    foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
                        l[index++] = (Glyph)lvi.Tag;
                    propGlyph.SelectedObject = null;
                    propGlyph.SelectedObjects = l;
                }
            }
            else
            {
                propGlyph.SelectedObject = null;
                propGlyph.SelectedObjects = null;
            }
            RefreshPreviewImages();
        }

        private void OnChangeFilterLanguage(object sender, EventArgs e)
        {
            PopulateList();
        }

        private void OnChangeFilterOption(object sender, EventArgs e)
        {
            PopulateList();
        }

        private void OnChangeFilterBuild(object sender, EventArgs e)
        {
            PopulateList();
        }

        private void OnPaintBaseline(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(baseLineBrush, 0, 0, pnlEditBaseline.Width, pnlEditBaseline.Height);
            if ((baseLineGlyph != null) && (baseLineGlyph.Picture!=null))
            {
                float zoom = 1;
                if (cbBaseLineZoom10.Checked)
                    zoom = 0.1f;
                if (cbBaseLineZoom25.Checked)
                    zoom = 0.25f;
                if (cbBaseLineZoom50.Checked)
                    zoom = 0.5f;
                if (cbBaseLineZoom100.Checked)
                    zoom = 1.0f;
                if (cbBaseLineZoom200.Checked)
                    zoom = 2.0f;
                if (cbBaseLineZoom300.Checked)
                    zoom = 3.0f;
                if (cbBaseLineZoomFit.Checked)
                {
                    zoom = (pnlEditBaseline.Width * 0.95f) / baseLineGlyph.Picture.Width;
                    float z2 = (pnlEditBaseline.Height * 0.95f) / baseLineGlyph.Picture.Height;
                    if (z2 < zoom)
                        zoom = z2;
                }
                int w = (int)(baseLineGlyph.Picture.Width * zoom);
                int h = (int)(baseLineGlyph.Picture.Height * zoom);
                int px = (pnlEditBaseline.Width - w) / 2;
                int py = (pnlEditBaseline.Height - h) / 2;
                e.Graphics.DrawImage(baseLineGlyph.Picture, px, py, w, h);
                // desenez si baseline-ul
                int pl = (int)((py + h) - h * baseLineGlyph.BaseLine);
                e.Graphics.DrawLine(baseLinePen, 5, pl, pnlEditBaseline.Width - 5, pl);
                e.Graphics.DrawEllipse(baseLinePen, 2, pl - 3, 6, 6);
                e.Graphics.DrawEllipse(baseLinePen, pnlEditBaseline.Width - 8, pl - 3, 6, 6);

            }
        }

        private void OnChangeBaseLineZoom(object sender, EventArgs e)
        {
            cbBaseLineZoom10.Checked = (sender == cbBaseLineZoom10);
            cbBaseLineZoom25.Checked = (sender == cbBaseLineZoom25);
            cbBaseLineZoom50.Checked = (sender == cbBaseLineZoom50);
            cbBaseLineZoom100.Checked = (sender == cbBaseLineZoom100);
            cbBaseLineZoom200.Checked = (sender == cbBaseLineZoom200);
            cbBaseLineZoom300.Checked = (sender == cbBaseLineZoom300);            
            cbBaseLineZoomFit.Checked = (sender == cbBaseLineZoomFit);
            RefreshPreviewImages();
        }

        private void OnChangeBaseLineEditBackground(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = baseLineBrush.Color;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                baseLineBrush.Color = dlg.Color;
                baseLinePen.Color = Color.FromArgb(255 - dlg.Color.R, 255 - dlg.Color.G, 255 - dlg.Color.B);
                RefreshPreviewImages();
            }
        }

        private void SetCurrentGlyph(Glyph g)
        {
            baseLineGlyph = g;
            if (g==null)
            {
                previewGlyph.SetPreviewObject(prj, null, null);
                lbCharacterInfo.Text = "<None>";
                btnCharacterResolutions.Enabled = false;
                btnCharacterResolutions.Text = "Resolutions";
            } else {

                lbCharacterInfo.Text = g.GetCharString();
                btnCharacterResolutions.DropDownItems.Clear();
                btnCharacterResolutions.Enabled = true;
                if (g.PicturesForResolutions.Count == 0)
                {
                    btnCharacterResolutions.Enabled = false;
                    btnCharacterResolutions.Text = "Resolutions";
                }
                else
                {
                    // updatez si meniul
                    ToolStripMenuItem defaultImage = null;
                    foreach (string rez in g.PicturesForResolutions.Keys)
                    {
                        ToolStripMenuItem itm = new ToolStripMenuItem(rez.Replace("x", " x "));
                        itm.Checked = false;
                        itm.Click += OnChangePreviewResolution;
                        itm.Enabled = baseLineGlyph.PicturesForResolutions.ContainsKey(rez);
                        btnCharacterResolutions.DropDownItems.Add(itm);
                        Size sz = Project.SizeToValues(rez);
                        if ((sz.Width == prj.DesignResolutionSize.Width) && (sz.Height == prj.DesignResolutionSize.Height))
                            defaultImage = itm;
                    }
                    btnCharacterResolutions.Text = "Resolutions (" + btnCharacterResolutions.DropDownItems.Count.ToString()+")";
                    if (defaultImage != null)
                        OnChangePreviewResolution(defaultImage, null);
                }

            }
            propGlyph.SelectedObjects = null;
            propGlyph.SelectedObject = g;
            RefreshPreviewImages();
        }

        void OnChangePreviewResolution(object sender, EventArgs e)
        {
            if ((baseLineGlyph==null) || (sender==null))
                return;
            foreach (ToolStripItem itm in btnCharacterResolutions.DropDownItems)
            {
                ToolStripMenuItem tipm = (ToolStripMenuItem)itm;
                tipm.Checked = (tipm == sender);
            }
            string rez = ((ToolStripMenuItem)sender).Text.Replace(" ","");
            if (baseLineGlyph.PicturesForResolutions.ContainsKey(rez))
            {
                if ((baseLineGlyph.PicturesForResolutions[rez]!=null) && (baseLineGlyph.PicturesForResolutions[rez].Picture!=null))
                    previewGlyph.SetPreviewObject(prj, null, baseLineGlyph.PicturesForResolutions[rez].Picture);
                else
                    previewGlyph.SetPreviewObject(prj, null, null);
            }
            else
            {
                previewGlyph.SetPreviewObject(prj, null, null);
            }
        }

        private void UpdateBaseLine(float direction)
        {
            if (baseLineGlyph == null)
                return;
            float step = 0.0125f;
            if ((baseLineGlyph.Picture!=null) && (baseLineGlyph.Picture.Width>0))
                step = 1.0f / baseLineGlyph.Picture.Width;
            baseLineGlyph.BaseLine += (step * direction);

            RefreshPreviewImages();
            propGlyph.SelectedObject = baseLineGlyph;
            OnChangeGlyphProperty(null, null);
        }
        private void OnIncreaseBaseLine(object sender, EventArgs e)
        {
            UpdateBaseLine(1.0f);
        }
        private void OnDecreaseBaseLine(object sender, EventArgs e)
        {
            UpdateBaseLine(-1.0f);
        }

        private void OnChangeGlyphProperty(object s, PropertyValueChangedEventArgs e)
        {
            if (lstGlyphs.SelectedItems.Count == 0)
            {
                baseLineGlyph = null;
                previewGlyph.SetPreviewObject(prj, null, null);
            }
            else
            {
                foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
                    UpdateListViewItem(lvi);
            }
            RefreshPreviewImages();
        }
        private List<int> GetSelectedCodes()
        {
            List<int> l = new List<int>();
            foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
                l.Add(((Glyph)lvi.Tag).Code);
            return l;
        }
        private void OnDoubleClickedGlyph(object sender, EventArgs e)
        {
            if (lstGlyphs.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select one glyph to be edited !");
                return;
            }
            Glyph g = (Glyph)lstGlyphs.SelectedItems[0].Tag;
            string s = tempFont.GetGlyphSourcePath(g.Code);
            if (prj.RunCommand(prj.Settings.InskapePath, s, "Inkscape", true, false) == false)
                prj.ShowErrors();
            else
            {
                // tre sa ii fac build si apoi sa il incarc
                ProgressDialog pdlg = new ProgressDialog(prj);
                pdlg.SetFontGlyphBuild(GetSelectedCodes(), tempFont);
                pdlg.Start();
                tempFont.LoadGlyph(g);
                if (prj.EC.HasErrors())
                    prj.ShowErrors();
                RefreshGlyphIcons();
                UpdateListViewItem(lstGlyphs.SelectedItems[0]);
                previewGlyph.SetPreviewObject(prj, null, g.Picture);
                RefreshPreviewImages();
            }
                
        }

        private void OnEditGlyphTemplates(object sender, EventArgs e)
        {
            FontTemplateCreator dlg = new FontTemplateCreator(prj);
            dlg.ShowDialog();
        }

        private void OnDeleteGlyphs(object sender, EventArgs e)
        {
            if (lstGlyphs.SelectedItems.Count==0)
            {
                MessageBox.Show("Please select at least one glyph for deletion !");
                return;
            }
            if (MessageBox.Show("Delete " + lstGlyphs.SelectedItems.Count.ToString() + " glyph(s) ?", "Delete", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;
            foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
                tempFont.Glyphs.Remove((Glyph)lvi.Tag);
            PopulateList();
            baseLineGlyph = null;
            previewGlyph.SetPreviewObject(prj, null, null);
            RefreshPreviewImages();
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            if (tempFont.Load() == false)
                prj.ShowErrors();
            RefreshGlyphIcons();
            PopulateList();
            baseLineGlyph = null;
            previewGlyph.SetPreviewObject(prj, null, null);
            RefreshPreviewImages();
        }

        private void OnRebuildFromSVG(object sender, EventArgs e)
        {
            List<int> l = GetSelectedCodes();
            if (l.Count==0)
            {
                MessageBox.Show("Please select at least one glyph for building !");
            }
            ProgressDialog pdlg = new ProgressDialog(prj);
            pdlg.SetFontGlyphBuild(GetSelectedCodes(), tempFont);
            pdlg.Start();
            tempFont.LoadGlyphs(l);
            if (prj.EC.HasErrors())
                prj.ShowErrors();
            RefreshGlyphIcons();
            PopulateList();
            baseLineGlyph = null;
            previewGlyph.SetPreviewObject(prj, null, null);
            RefreshPreviewImages();
        }

        private void OnRebuildFromTemplate(object sender, EventArgs e)
        {
            FontTemplateSelector dlg = new FontTemplateSelector(prj);
            Dictionary<int, string> codes = new Dictionary<int, string>();
            foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
            {
                Glyph g = (Glyph)lvi.Tag;
                if ((g.Template == null) || (g.Template.Trim().Length == 0))
                    continue;
                SVGTemplateList.TemplateInfo ti = dlg.GetTemplateInfo(g.Template);
                if (ti==null)
                {
                    prj.EC.AddError("Unable to find template: '" + g.Template + "' for code: " + g.Code.ToString());
                    continue;
                }
                string text = Disk.ReadFileAsString(ti.Path, prj.EC);
                if (text == null)
                    continue;
                if (text.Contains(">A<"))
                {
                    int index = text.LastIndexOf(">A<");
                    text = text.Substring(0, index) + ">$$CHAR$$<" + text.Substring(index + 3);
                }
                codes[g.Code] = text;
            }
            if (prj.EC.HasErrors())
            {
                prj.ShowErrors();
                return;
            }
            if (codes.Count==0)
            {
                MessageBox.Show("No glyph selected or selected glyph use no templates !");
                return;
            }
            // all good - le compilam
            ProgressDialog pdlg = new ProgressDialog(prj);
            pdlg.SetFontGlyphCreatorFromSVGsTemplates(codes,tempFont);
            pdlg.Start();
            List<int> l = new List<int>();
            foreach (int code in codes.Keys)
                if (codes[code]!=null)
                    l.Add(code);
            // fac build la cele chiar au fost adaugate
            pdlg.SetFontGlyphBuild(l, tempFont);
            pdlg.Start();
            // incarc si imaginile
            tempFont.LoadGlyphs(l);
            if (prj.EC.HasErrors())
                prj.ShowErrors();
            RefreshGlyphIcons();
            PopulateList();
            RefreshPreviewImages();
        }

        private void OnPaintPreviewText(object sender, PaintEventArgs e)
        {
            int W = pnlPreviewText.Width;
            int H = pnlPreviewText.Height;
            
            float zoom = 1.0f;
            if (cbPreviewZoom10.Checked) zoom = 0.1f;
            if (cbPreviewZoom25.Checked) zoom = 0.25f;
            if (cbPreviewZoom50.Checked) zoom = 0.5f;
            if (cbPreviewZoom100.Checked) zoom = 1.0f;
            if (cbPreviewZoom200.Checked) zoom = 2.0f;
            if (cbPreviewZoom300.Checked) zoom = 3.0f;
            if (cbPreviewZoom400.Checked) zoom = 4.0f;
            
            Glyph gWidth = null;
            Glyph gHeight = null;
            foreach (Glyph g in tempFont.Glyphs)
            {
                if (g.Code == tempFont.WidthCharacter)               
                    gWidth = g;
                if (g.Code == tempFont.HeightCharacter)
                    gHeight = g;
            }
            if ((gWidth==null) || (gHeight==null))
            {
                e.Graphics.FillRectangle(Brushes.Black, 0, 0, W, H);
                e.Graphics.DrawString("The character reference for Height or Width was not set !", previewFont, Brushes.Red, W / 2, H / 2, centerTextSF);
                return;
            }
            if (gHeight.Picture==null)
            {
                e.Graphics.FillRectangle(Brushes.Black, 0, 0, W, H);
                e.Graphics.DrawString("The image for height reference character is not loaded !", previewFont, Brushes.Red, W / 2, H / 2, centerTextSF);
                return;
            }
            if (gWidth.Picture == null)
            {
                e.Graphics.FillRectangle(Brushes.Black, 0, 0, W, H);
                e.Graphics.DrawString("The image for width reference character is not loaded !", previewFont, Brushes.Red, W / 2, H / 2, centerTextSF);
                return;
            }
            e.Graphics.FillRectangle(previewTextLineBrush, 0, 0, W, H);

            int baseLine = (int)(H * 0.75);
            float xx = 5;
            float char_space = gWidth.Picture.Width * zoom * tempFont.SpaceBetweenChars;
            int hh = (int)(gHeight.Picture.Height * (1.0f - gHeight.BaseLine) * zoom);

            if (txPreview.Text.Length>0)
            {
                foreach (char ch in txPreview.Text)
                {
                    if (ch==' ')
                    {
                        xx += gWidth.Picture.Width * zoom * tempFont.SpaceWidth;
                        xx += char_space;
                        if (xx > W)
                            break;
                        continue;
                    }
                    int index = tempFont.CodeToIndex(ch);
                    if (index < 0)
                    {
                        e.Graphics.FillRectangle(Brushes.Red,xx,baseLine-hh,gWidth.Picture.Width*zoom,hh);
                        e.Graphics.DrawRectangle(Pens.Black,xx,baseLine-hh,gWidth.Picture.Width*zoom,hh);
                        xx += gWidth.Picture.Width * zoom;
                        xx += char_space;
                        if (xx > W)
                            break;
                        continue;
                    }
                    Glyph g = tempFont.Glyphs[index];
                    if (g.Picture != null)
                    {
                        e.Graphics.DrawImage(g.Picture, xx, baseLine - g.Picture.Height * (1 - g.BaseLine) * zoom, g.Picture.Width * zoom, g.Picture.Height * zoom);
                        xx += g.Picture.Width * zoom;
                        xx += char_space;
                        if (xx > W)
                            break;
                    }
                }
            }
            else
            {
                foreach (Glyph g in tempFont.Glyphs)
                {
                    if (g.Picture!=null)
                    {
                        e.Graphics.DrawImage(g.Picture, xx, baseLine - g.Picture.Height * (1 - g.BaseLine) * zoom, g.Picture.Width * zoom, g.Picture.Height * zoom);
                        xx += g.Picture.Width * zoom;
                        xx += char_space;
                        if (xx > W)
                            break;
                    }
                }
            }



            
            e.Graphics.DrawLine(previewTextLinePen, 0, baseLine, W, baseLine);
            e.Graphics.DrawLine(previewTextLinePen, 0, baseLine-hh, W, baseLine-hh);


        }



        private void OnChangePreviewTextBackground(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = previewTextLineBrush.Color;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                previewTextLineBrush.Color = dlg.Color;
                previewTextLinePen.Color = Color.FromArgb(255 - dlg.Color.R, 255 - dlg.Color.G, 255 - dlg.Color.B);
                RefreshPreviewImages();
            }
        }

        private void SetCurrentCharAsWidthReference(object sender, EventArgs e)
        {
            if (lstGlyphs.SelectedItems.Count!=1)
            {
                MessageBox.Show("You have to select one character first !");
                return;
            }
            tempFont.WidthCharacter = ((Glyph)lstGlyphs.SelectedItems[0].Tag).Code;
            propFont.SelectedObject = tempFont;
            RefreshPreviewImages();
        }

        private void SetCurrentCharAsHeightReference(object sender, EventArgs e)
        {
            if (lstGlyphs.SelectedItems.Count != 1)
            {
                MessageBox.Show("You have to select one character first !");
                return;
            }
            tempFont.HeightCharacter = ((Glyph)lstGlyphs.SelectedItems[0].Tag).Code;
            propFont.SelectedObject = tempFont;
            RefreshPreviewImages();
        }

        private void SetAutoWidthAndHeightReferences(object sender, EventArgs e)
        {
            int wMin = -1;
            int hMin = -1;
            int charW = 0;
            int charH = 0;
            bool noImage = false;
            foreach (Glyph g in tempFont.Glyphs)
            {
                if (g.Picture!=null)
                {
                    if (g.Picture.Width>wMin)
                    {
                        wMin = g.Picture.Width;
                        charW = g.Code;
                    }
                    if (g.Picture.Height*(1-g.BaseLine)>hMin)
                    {
                        hMin = (int)(g.Picture.Height * (1 - g.BaseLine));
                        charH = g.Code;
                    }
                }
                else
                {
                    noImage = true;
                }
            }
            if (tempFont.Glyphs.Count == 0)
            {
                MessageBox.Show("You need to have at least one character added into this font for this option to work !");
                return;
            }
            if (charW==0)
            {
                MessageBox.Show("Measures can not be computed for any of the glyphs because images were not loaded for this font.");
                return;
            }
            if (noImage == true)
            {
                MessageBox.Show("Some images were not loaded - computed measures will not be correct !");
                return;
            }
            tempFont.WidthCharacter = charW;
            tempFont.HeightCharacter = charH;
            propFont.SelectedObject = tempFont;
            RefreshPreviewImages();
        }

        private void OnValidateFont(object sender, EventArgs e)
        {
            if (tempFont.ValidateFont(true)==false)
            {
                prj.ShowErrors();
            }
            else
            {
                MessageBox.Show("Font is ok !");
            }
        }

        private void OnChangePreviewText(object sender, EventArgs e)
        {
            RefreshPreviewImages();
        }

        private void OnChangePreviewZoom(object sender, EventArgs e)
        {
            cbPreviewZoom10.Checked = (sender == cbPreviewZoom10);
            cbPreviewZoom25.Checked = (sender == cbPreviewZoom25);
            cbPreviewZoom50.Checked = (sender == cbPreviewZoom50);
            cbPreviewZoom100.Checked = (sender == cbPreviewZoom100);
            cbPreviewZoom200.Checked = (sender == cbPreviewZoom200);
            cbPreviewZoom300.Checked = (sender == cbPreviewZoom300);
            cbPreviewZoom400.Checked = (sender == cbPreviewZoom400);
            RefreshPreviewImages();
        }

        private void OnChangeFontProperty(object s, PropertyValueChangedEventArgs e)
        {
            RefreshPreviewImages();
        }

        private void OnCopySelectedCharacters(object sender, EventArgs e)
        {
            if (lstGlyphs.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one character first !");
                return;
            }
            string ss = "";
            foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
            {
                Glyph g = (Glyph)lvi.Tag;
                ss += (char)g.Code;
            }
            txPreview.Text += ss;
        }

        private void OnSelectCharactersFromStrings(object sender, EventArgs e)
        {
            if (cbOnlyCharsFromAppStrings.Checked)
            {
                FontCharacterSelectorFromStrings dlg = new FontCharacterSelectorFromStrings(prj,false);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FilteredCodes = dlg.SelectedCodes;
                    KeepCharactersFromFilteredCodes = dlg.FilterSelectedCharacters;
                    PopulateList();
                }
                else
                {
                    cbOnlyCharsFromAppStrings.Checked = false;
                    PopulateList();
                }
            }
            else
            {
                PopulateList();
            }
        }

        private void OnAutoAlignCharacters(object sender, EventArgs e)
        {
            if (lstGlyphs.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one glyph to be autoaligned !");
                return;
            }
            foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
            {
                Glyph g = (Glyph)lvi.Tag;
                if (g == null)
                    continue;
                CharacterSet.Info gInfo = cSet.Get(g.Code);
                if ((gInfo == null) || (gInfo.AlignRelation == CharacterSet.AlignamentRelation.None))
                    continue;
                CharacterSet.Info gRef = cSet.Get(gInfo.AlignCharCode);
                if (gRef == null)
                {
                    MessageBox.Show(String.Format("Internal error - character ({0}) has its size referd by character ({1}) that does not exist !",(char)g.Code,(char)gInfo.AlignCharCode));
                    continue;
                }
                AlignCharToChar(g.Code, gInfo.AlignCharCode, gInfo.AlignRelation);
            }
            RefreshPreviewImages();
            if ((lstGlyphs.SelectedItems.Count==1) && (baseLineGlyph!=null))
                propGlyph.SelectedObject = baseLineGlyph;
            OnChangeGlyphProperty(null, null);
        }

        private bool AlignCharToChar(int code,int charToAlignTo, CharacterSet.AlignamentRelation align)
        {
            int i1 = tempFont.CodeToIndex(code);
            int iTo = tempFont.CodeToIndex(charToAlignTo);
            if (i1<0)
            {
                MessageBox.Show(String.Format("Character '{0}' does not exists in the current list or characters !",(char)i1));
                return false;
            }
            if (iTo<0)
            {
                MessageBox.Show(String.Format("Character ({0}) requires character ({1}) to exist for alignament !", (char)i1, (char)iTo));
                return false;
            }
            Glyph g = tempFont.Glyphs[i1];
            Glyph gr = tempFont.Glyphs[iTo];
            float dif;
            switch (align)
            {
                case CharacterSet.AlignamentRelation.Top:
                    dif = ((float)gr.Picture.Height * (1.0f - gr.BaseLine)) - ((float)g.Picture.Height);
                    g.BaseLine = -(dif / (float)g.Picture.Height);
                    break;
                case CharacterSet.AlignamentRelation.Center:
                    dif = (float)gr.Picture.Height / 2 - (float)gr.Picture.Height * gr.BaseLine;
                    dif = dif - ((float)g.Picture.Height) / 2;
                    g.BaseLine = -(dif / (float)g.Picture.Height);
                    break;
                case CharacterSet.AlignamentRelation.Bottom:
                    dif = gr.Picture.Height * gr.BaseLine;
                    g.BaseLine = (dif / (float)g.Picture.Height);
                    break;
            }
            return true;
        }

        private void OnAlignCharacters(CharacterSet.AlignamentRelation align)
        {
            if (txCharToAlignTo.Text.Length != 1)
            {
                MessageBox.Show("Please select a character to align current glyph to !");
                txCharToAlignTo.Focus();
                return;
            }
            if (lstGlyphs.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please select at least one glyph for alignament !");
                return;
            }
            foreach (ListViewItem lvi in lstGlyphs.SelectedItems)
            {
                Glyph g = (Glyph)lvi.Tag;
                if (g == null)
                    continue;
                AlignCharToChar(g.Code, txCharToAlignTo.Text[0], align);
            }
            RefreshPreviewImages();
            if ((lstGlyphs.SelectedItems.Count == 1) && (baseLineGlyph != null))
                propGlyph.SelectedObject = baseLineGlyph;
            OnChangeGlyphProperty(null, null);
        }
        private void OnAlignTop(object sender, EventArgs e)
        {
            OnAlignCharacters(CharacterSet.AlignamentRelation.Top);         
        }

        private void OnAlignCenter(object sender, EventArgs e)
        {
            OnAlignCharacters(CharacterSet.AlignamentRelation.Center); 
        }

        private void OnAlignBottom(object sender, EventArgs e)
        {
            OnAlignCharacters(CharacterSet.AlignamentRelation.Bottom);
        }


    }
}
 