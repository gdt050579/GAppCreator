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
    public partial class FontGlyphCreator : Form
    {
        public enum SelectAction
        {
            SelectOnlyNew,
            SelectAll,
            None,
        };
        class GlyphState
        {
            public bool Selected = false;
            public bool Exists = false;
        };
        public List<int> CodeList = new List<int>();
        public string TemplateSVG = "";
        public string TemplateName = "";
        
        Project prj;        
        Dictionary<int, GlyphState> charCodes = new Dictionary<int, GlyphState>();
        CharacterSet cSet = new CharacterSet();
        bool populating = false;
        public FontGlyphCreator(Project p,FontResource fnt)
        {
            prj = p;
            InitializeComponent();
            OnUseBlank(null, null);
            comboLanguage.Items.Add("All");
            foreach (string name in Enum.GetNames(typeof(Language)))
            {
                comboLanguage.Items.Add(name);
            }
            // populez dictionarul
            List<CharacterSet.Info> l = cSet.GetCharacterTable();
            foreach (CharacterSet.Info ci in l)
                charCodes[ci.CharCode] = new GlyphState();
            // populez si din font dar cu true acuma
            foreach (Glyph g in fnt.Glyphs)
            {
                if (charCodes.ContainsKey(g.Code)==false)
                    charCodes[g.Code] = new GlyphState();
                charCodes[g.Code].Exists = true;
            }
            comboLanguage.SelectedIndex = 0;
            // Populez lista de charactere
            PopulateCharacterList();
        }
        private void ComputeSelected()
        {
            int count = 0;
            foreach (int charCode in charCodes.Keys)
                if (charCodes[charCode].Selected)
                    count++;
            if (count == 0)
                lbSelected.Text = "No glyph(s) selected";
            else
                lbSelected.Text = count.ToString() + " selected";
        }
        private void PopulateCharacterList()
        {
            populating = true;
            List<int> l = new List<int>(charCodes.Keys.Count);
            foreach (int charCode in charCodes.Keys)
                l.Add(charCode);
            l.Sort();
            lstChars.Items.Clear();
            Language filter_lng = Language.English;
            if (comboLanguage.SelectedIndex > 0)
                filter_lng = (Language)Enum.Parse(typeof(Language), (string)comboLanguage.SelectedItem);
            foreach (int charCode in l)
            {
                CharacterSet.Info ci = cSet.Get(charCode);
                if ((cbHideExisting.Checked) && (charCodes[charCode].Exists))
                    continue;
                ListViewItem lvi = new ListViewItem(""+(char)charCode);
                lvi.Tag = charCode;
                lvi.SubItems.Add(charCode.ToString());
                lvi.SubItems.Add(String.Format("0x{0:X4}", charCode));
                
                bool skip = false;
                if (ci!=null)
                {
                    string s = "";
                    bool found = false;
                    foreach (Language lng in ci.Languages)
                    {
                        found |= (lng == filter_lng);
                        s += lng.ToString() + ", ";
                    }
                    if ((comboLanguage.SelectedIndex > 0) && (!found))
                        skip = true;
                    if (s.EndsWith(", "))
                        s = s.Substring(0,s.Length-2);
                    lvi.SubItems.Add(s);
                    if (ci.DefaultForEveryLanguage)
                    {
                        lvi.SubItems.Add("YES");
                        if (cbShowDefault.Checked==false)
                            skip = true;
                    } else {
                        lvi.SubItems.Add("-");
                        if (cbShowNonDefault.Checked==false)
                            skip = true;
                    }
                    lvi.SubItems.Add(ci.Type.ToString());
                    switch (ci.Type)
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
                    if (cbShowCustomChars.Checked==false)
                    {
                        skip = true;
                    } else {
                        lvi.SubItems.Add("Custom character !");
                        lvi.SubItems.Add("-");
                        lvi.SubItems.Add("?");
                    }
                }
                if (skip == true)
                    continue;
                if (charCodes[charCode].Exists)
                    lvi.ForeColor = Color.LightGray;
                lvi.Checked = charCodes[charCode].Selected;
                lstChars.Items.Add(lvi);
            }
            ComputeSelected();
            populating = false;
        }
        private void OnUseBlank(object sender, EventArgs e)
        {
            lbTemplate.Text = "No template";
            lbTemplate.Image = null;
            TemplateSVG = "<svg width=\"64\" height=\"64\"><rect width=\"64\" height=\"64\" style=\"fill:rgb(255,0,0);stroke-width:5;stroke:rgb(0,0,0)\" /></svg>";
            TemplateName = "";
        }

        private void OnUseExistingTemplate(object sender, EventArgs e)
        {
            FontTemplateSelector dlg = new FontTemplateSelector(prj);
            if (dlg.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                string text = Disk.ReadFileAsString(dlg.ResultedTemplate.Path, prj.EC);
                if (prj.EC.HasErrors())
                {
                    prj.ShowErrors();
                    return;
                }
                if (text.Contains(">A<") == false)
                {
                    MessageBox.Show("Template file should be create for letter 'A'. Selected template '"+dlg.ResultedTemplate.Name+"' is not !");
                    return;
                }
                int index = text.LastIndexOf(">A<");
                TemplateSVG = text.Substring(0, index) + ">$$CHAR$$<" + text.Substring(index + 3);
                TemplateName = dlg.ResultedTemplate.Name;
                lbTemplate.Text = dlg.ResultedTemplate.Name;
                lbTemplate.Image = dlg.ResultedTemplate.Picture;
            }
        }

        private void OnCreateNewTemplate(object sender, EventArgs e)
        {
            FontTemplateCreator dlg = new FontTemplateCreator(prj);
            dlg.ShowDialog();
        }

        private void OnChangeFilterLanguage(object sender, EventArgs e)
        {
            PopulateCharacterList();
        }

        private void OnChangeFilterOption(object sender, EventArgs e)
        {
            PopulateCharacterList();
        }

        private void OnClearAll(object sender, EventArgs e)
        {
            foreach (int charCode in charCodes.Keys)
                charCodes[charCode].Selected = false;
            PopulateCharacterList();
        }

        private void OnCheckItem(object sender, ItemCheckedEventArgs e)
        {
            if (populating)
                return;
            int code = (int)e.Item.Tag;
            charCodes[code].Selected = e.Item.Checked;
            ComputeSelected();
        }

        private void OnAddCharactersByCode(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter a list of codes separated by ','. Hexazecimal values must be preceded by 0x prefix. Invalid numeric values will be ignored. Example: 100,0x20,300", "");
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] words = (ib.StringResult + ",").Split(',');
                string res = "";
                List<int> l = new List<int>();
                foreach (string word in words)
                {
                    string w = word.Trim().ToLower();
                    int charCode = -1;
                    if (w.StartsWith("0x"))
                    {
                        try
                        {
                            charCode = Convert.ToInt32(w.Substring(2),16);
                        }
                        catch (Exception)
                        {
                            charCode = -1;
                        }
                    }
                    else
                    {
                        try
                        {
                            charCode = Convert.ToInt32(w, 10);
                        }
                        catch (Exception)
                        {
                            charCode = -1;
                        }
                    }
                    if (charCode < 32)
                        continue;
                    if (charCodes.ContainsKey(charCode) == false)
                    {
                        res += (char)charCode + ", ";
                        l.Add(charCode);
                    }                   
                }
                if (res.EndsWith(", "))
                    res = res.Substring(0, res.Length - 2);
                if (l.Count == 0)
                {
                    MessageBox.Show("All of the characters you added in the text already exists !");
                    return;
                }

                if (MessageBox.Show("Add the following characters ?\n" + res, "Add characters", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (int code in l)
                        charCodes[code] = new GlyphState();
                    PopulateCharacterList();
                }
            }
        }
        private void AddFromString(string ss,SelectAction act)
        {
            string res = "";
            List<int> l = new List<int>();
            foreach (char ch in ss)
            {
                int charCode = (int)ch;
                if (charCode <= 32)
                    continue;
                if (charCodes.ContainsKey(charCode) == false)
                {
                    res += ch + ", ";
                    l.Add(charCode);
                }
            }
            if (res.EndsWith(", "))
                res = res.Substring(0, res.Length - 2);
            if ((l.Count == 0) && (act == SelectAction.None))
            {
                MessageBox.Show("All of the characters you added in the text already exists !");
                return;
            }
            if (l.Count > 0)
            {
                if (MessageBox.Show("Add the following custom characters ?\n" + res, "Add characters", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (int code in l)
                        charCodes[code] = new GlyphState();
                    PopulateCharacterList();
                }
            }
            // partea de select
            if (act == SelectAction.SelectOnlyNew)
            {
                foreach (char ch in ss)
                {
                    int charCode = (int)ch;
                    if (charCode <= 32)
                        continue;
                    if (charCodes[charCode].Exists)
                        continue;
                    charCodes[charCode].Selected = true;
                }
                PopulateCharacterList();
            }
            if (act == SelectAction.SelectAll)
            {
                foreach (char ch in ss)
                {
                    int charCode = (int)ch;
                    if (charCode <= 32)
                        continue;
                    charCodes[charCode].Selected = true;
                }
                PopulateCharacterList();
            }
        }
        private void OnAddCharactersFromText(object sender, EventArgs e)
        {
            InputBox ib = new InputBox("Enter a text (all the unexisting characters from that text will be imported).\nSpace, Tabs, CR, LF and control characters will be ignored !", "");
            if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                AddFromString(ib.StringResult,SelectAction.None);
        }

        private void OnOK(object sender, EventArgs e)
        {
            CodeList.Clear();
            bool overwrite = false;
            foreach (int code in charCodes.Keys)
            {
                if (charCodes[code].Selected)
                    CodeList.Add(code);
                if ((charCodes[code].Exists) && (charCodes[code].Selected))
                    overwrite = true;
            }
            if (CodeList.Count==0)
            {
                MessageBox.Show("No characters selected !");
                return;
            }
            if (overwrite)
            {
                if (MessageBox.Show("Some of the selected characters were already created.\nOverwrite them ?", "Overwrite", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnAddCharactersUsedInAppStrings(object sender, EventArgs e)
        {
            FontCharacterSelectorFromStrings dlg = new FontCharacterSelectorFromStrings(prj,true);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddFromString(dlg.CharactersToBeAdded,dlg.Action);
            }
        }
    }
}
