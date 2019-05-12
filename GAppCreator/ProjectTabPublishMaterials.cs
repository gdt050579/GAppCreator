using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Collections;

namespace GAppCreator
{
    public partial class ProjectTabPublishMaterials : BaseProjectContainer
    {
        class FileInformation
        {
            public string FileName;
            public string Type;
            public string Size;
            public string Date;
        };
        class PairCount : IComparable
        {
            public string Text;
            public int Count;
            public PairCount(string txt, int count) { Text = txt; Count = count; }
            public int CompareTo(object o) { return ((PairCount)o).Count.CompareTo(Count); }
        };
        ScintillaNET.Scintilla TextEditor;
        bool NeedRefresh = false;
        SortedDictionary<string, int> WordCounter = new SortedDictionary<string, int>();
        List<PairCount> SortedWordCountList = new List<PairCount>();
        PublishObject CurrentObject;
        List<FileInformation> WorkGroupFiles = new List<FileInformation>();
        Dictionary<string,string> extensionAssociations = new Dictionary<string,string>()
        {
            { ".jpg", "Image"},{ ".png", "Image"},{ ".jpeg", "Image"},{ ".bmp", "Image"},{ ".gif", "Image"},
            { ".txt", "Text"},
            { ".mp4", "Video"}, { ".avi", "Video"}, { ".mpeg", "Video"}, 
        };
        
        public ProjectTabPublishMaterials()
        {
            InitializeComponent();

            // languages
            foreach (string s in Enum.GetNames(typeof(ExtendedLanguage)))
                comboLanguages.Items.Add(s);
            comboLanguages.SelectedIndex = comboLanguages.Items.Count - 1;

            comboBuild.Items.Add("All");
            comboBuild.SelectedIndex = 0;

            lbGenericInfo.Dock = DockStyle.Fill;
            previewImage.Dock = DockStyle.Fill;
        }

        public void UpdatePublishItem(ListViewItem lvi)
        {
            PublishObject o = (PublishObject)lvi.Tag;
            lvi.Text = o.Name;
            lvi.SubItems[1].Text = o.Lang.ToString();
            lvi.SubItems[2].Text = o.Builds;
            lvi.SubItems[3].Text = o.GetInformation();
            lvi.Group = lstPublishObjects.Groups[o.GetObjectType()];            
        }
        public void UpdatePublishObjectList()
        {
            if (Context == null)
                return;
            string txt = txFilter.Text.ToLower();
            lstPublishObjects.BeginUpdate();
            lstPublishObjects.Items.Clear();
            propPublishObjects.SelectedObject = null;
            propPublishObjects.SelectedObjects = null;
            CurrentObject = null;
            ExtendedLanguage lng = ExtendedLanguage.All;
            if (comboLanguages.SelectedIndex>=0)
                lng = (ExtendedLanguage)Enum.Parse(typeof(ExtendedLanguage), comboLanguages.SelectedItem.ToString());
            string bld = "";
            Dictionary<string,string> d = null;
            if (comboBuild.SelectedIndex > 0)
            {
                bld = Context.Prj.BuildConfigurations[comboBuild.SelectedIndex - 1].Name.ToLower();
                d = new Dictionary<string,string>();
            }
            
            foreach (PublishObject o in Context.Prj.PublishData)
            {
                if (txt.Length>0)
                {
                    bool okToKeep = false;
                    if (o.Name.ToLower().Contains(txt))
                        okToKeep = true;
                    if ((!okToKeep) && (o.GetInformation().ToLower().Contains(txt)))
                        okToKeep = true;

                    // daca nu a dat match - ies
                    if (!okToKeep)
                        continue;
                }
                if ((lng != ExtendedLanguage.All) && (o.Lang != lng) && (o.Lang!=ExtendedLanguage.All))
                    continue;
                if (d!=null)
                {
                    Project.StringListToDict(o.Builds, d);
                    if (d.ContainsKey(bld) == false)
                        continue;
                }

                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add(""); lvi.SubItems.Add(""); lvi.SubItems.Add("");
                lvi.Tag = o;
                UpdatePublishItem(lvi);
                lstPublishObjects.Items.Add(lvi);                
            }
            lstPublishObjects.EndUpdate();
        }
        private void PrepareScintilla(Scintilla edit)
        {

            edit.Margins[0].Width = 30;
            edit.Margins[1].Width = 12;
            edit.Margins[1].Type = MarginType.Symbol;
            //edit.Margins[2].Width = 20;
            //edit.Margins[2].IsFoldMargin = true;
            //edit.Margins[2].IsMarkerMargin = false;
            //edit.Margins[2].IsClickable = true;

            edit.LineWrapping.Mode = LineWrappingMode.Word;


            edit.EndOfLine.Mode = EndOfLineMode.LF;

            //edit.ConfigurationManager.Language = "c#";
            edit.IsBraceMatching = true;

            edit.Indentation.TabWidth = 6;
            edit.Indentation.ShowGuides = true;
            edit.Indentation.SmartIndentType = SmartIndent.CPP;

            edit.Folding.IsEnabled = true;
            edit.Folding.UseCompactFolding = true;

            //edit.Lexing.Lexer = ScintillaNET.Lexer.Cpp;

            edit.Caret.HighlightCurrentLine = true;
            edit.Caret.CurrentLineBackgroundColor = System.Drawing.Color.AliceBlue;

            edit.Indicators[0].Style = IndicatorStyle.RoundBox;
            edit.Indicators[0].Color = System.Drawing.Color.Red;
            edit.Indicators[1].Style = IndicatorStyle.Squiggle;
            edit.Indicators[1].Color = System.Drawing.Color.Red;

            edit.Markers[0].Symbol = MarkerSymbol.Circle;
            edit.Markers[0].BackColor = System.Drawing.Color.Red;

            edit.AutoComplete.IsCaseSensitive = false;
            edit.AutoComplete.DropRestOfWord = false;

            edit.SelectionChanged += new EventHandler(Editor_SelectionChanged);
            edit.TextChanged += Editor_TextChanged;
        }

        void Editor_TextChanged(object sender, EventArgs e)
        {
            NeedRefresh = true;
            btnSave.Enabled = true;
        }
        void Editor_SelectionChanged(object sender, EventArgs e)
        {
            ScintillaNET.Scintilla edit = (ScintillaNET.Scintilla)sender;
            edit.GetRange().ClearIndicator(0);
            if (edit.Selection.Start < edit.Selection.End)
            {
                int count = 0;
                edit.FindReplace.Flags = ScintillaNET.SearchFlags.WholeWord;
                foreach (Range r in edit.FindReplace.FindAll(edit.Selection.Text))
                    if ((r.Start != edit.Selection.Start) || (r.End != edit.Selection.End))
                    {
                        r.SetIndicator(0);
                        count++;
                    }
                lstTextInfo.Items[3].SubItems[1].Text = (count+1).ToString();
                // il caut si in lista
                string txt = edit.Selection.Text.ToLower();
                foreach (ListViewItem lvi in lstWordsCount.Items)
                {
                    if (lvi.Text == txt)
                    {
                        lvi.Selected = true;
                        lvi.EnsureVisible();
                        break;
                    }
                }
            }
            else
            {
                lstTextInfo.Items[3].SubItems[1].Text = "0";
            }
        }

        private void OnDblClickOnWord(object sender, EventArgs e)
        {
            if (lstWordsCount.SelectedItems.Count==1)
            {
                //TextEditor.Selection.Clear();
                TextEditor.GetRange().ClearIndicator(0);
                int count = 0;
                TextEditor.FindReplace.Flags = ScintillaNET.SearchFlags.WholeWord;
                foreach (Range r in TextEditor.FindReplace.FindAll(lstWordsCount.SelectedItems[0].Text))                    
                {
                    r.SetIndicator(0);
                    count++;
                }
                lstTextInfo.Items[3].SubItems[1].Text = (count + 1).ToString();
            }
        }


        private void OnNewObjectAdded(PublishObject obj)
        {
            UpdatePublishObjectList();
            // fac select pe object
            foreach (ListViewItem lvi in lstPublishObjects.Items)
            {
                if (lvi.Tag == obj)
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                }
            }
        }
        private void OnAddText(object sender, EventArgs e)
        {
            TextPublish tp = new TextPublish();
            if (tp.Create(Context.Prj)==false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            // fac si un fisier text
            if (Disk.SaveFile(tp.GetObjectFile(),"",Context.Prj.EC)==false)
            {
                Context.Prj.ShowErrors();
                return;
            }
            // am creat obiectul - il adaug
            tp.Name = "New text object";
            Context.Prj.PublishData.Add(tp);
            Context.Prj.PublishData.Sort();
            OnNewObjectAdded(tp);
        }

        private void OnChangeSelectedPublishObjects(object sender, EventArgs e)
        {
            if ((btnSave.Enabled) && (CurrentObject!=null))
            {
                if (MessageBox.Show("Save current object ?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    OnSaveCurrentObject(null, null);
            }
            btnSave.Enabled = false;
            btnEdit.Enabled = false;
            btnRebuild.Enabled = false;
            btnOpenFolder.Enabled = false;
            btnCopyPath.Enabled = false;


            CurrentObject = null;
            pnlTextEditor.Visible = false;
            previewImage.Visible = false;
            lbGenericInfo.Text = lstPublishObjects.SelectedItems.Count.ToString() + " selected objects from " + lstPublishObjects.Items.Count.ToString();
            lbGenericInfo.Visible = false;

            PreviewTimer.Enabled = false;
            if (lstPublishObjects.SelectedItems.Count == 0)
            {
                lbGenericInfo.Visible = true;
                propPublishObjects.SelectedObject = null;
                propPublishObjects.SelectedObjects = null;
                return;
            }
            if (lstPublishObjects.SelectedItems.Count > 1)
            {
                lbGenericInfo.Visible = true;
                Object[] o = new Object[lstPublishObjects.SelectedItems.Count];
                for (int tr = 0; tr < lstPublishObjects.SelectedItems.Count; tr++)
                    o[tr] = lstPublishObjects.SelectedItems[tr].Tag;
                propPublishObjects.SelectedObject = null;
                propPublishObjects.SelectedObjects = o;
                return;
            }
            PreviewObject((PublishObject)lstPublishObjects.SelectedItems[0].Tag);
        }

        private void PreviewObject(PublishObject p)
        {
            lbGenericInfo.Visible = false;
            propPublishObjects.SelectedObjects = null;
            propPublishObjects.SelectedObject = p;
            CurrentObject = p;
            btnOpenFolder.Enabled = true;
            btnCopyPath.Enabled = true;

            if (p.GetType() == typeof(TextPublish))
            {
                string data = Disk.ReadFileAsString(p.GetObjectFile(), Context.Prj.EC);
                if (Context.Prj.EC.HasErrors())
                {
                    TextEditor.Text = "";
                    Context.Prj.ShowErrors();
                }
                else
                {
                    TextEditor.Text = data;
                    btnSave.Enabled = false;
                }
                NeedRefresh = true;
                pnlTextEditor.Visible = true;
                PreviewTimer.Enabled = true;
            }
            if (p.GetType() == typeof(ImagePublish))
            {
                Image i = Project.LoadImage(p.GetObjectFile());
                if (i!=null)
                {
                    ((ImagePublish)p).Size = string.Format("{0} x {1}",i.Width,i.Height);
                    OnChangeSelectedObjectProperty(null,null);
                }
                previewImage.SetPreviewObject(Context.Prj, null, i);
                previewImage.Visible = true;
                NeedRefresh = false;
                PreviewTimer.Enabled = false;
                btnEdit.Enabled = true;
                btnRebuild.Enabled = true;
            }
            if (p.GetType() == typeof(VideoPublish))
            {
                // activeaza preview-ul
            }
        }
        private void UpdateTextInfoData()
        {
            string txt = TextEditor.Text;
            string word = "";
            bool onWord = false;
            int w_Count = 0;
            WordCounter.Clear();
            foreach (char ch in txt)
            {
                if (((ch>='a') && (ch<='z')) || ((ch>='0') && (ch<='9')) || (ch=='_'))
                {
                    word += ch;
                    onWord = true;
                } else 
                if (((ch>='A') && (ch<='Z')))
                {
                    word += (char)(ch+32);
                    onWord = true;
                }
                else
                {
                    if (onWord)
                    {
                        if (word.Length > 3)
                        {
                            if (!WordCounter.ContainsKey(word))
                                WordCounter[word] = 1;
                            else
                                WordCounter[word]++;
                        }
                        word = "";
                        w_Count++;
                    }
                    onWord = false;
                }
            }
            if (onWord)
            {
                if (word.Length > 3)
                {
                    if (!WordCounter.ContainsKey(word))
                        WordCounter[word] = 1;
                    else
                        WordCounter[word]++;
                }
                w_Count++;
            }
            lstTextInfo.Items[0].SubItems[1].Text = w_Count.ToString();
            lstTextInfo.Items[1].SubItems[1].Text = TextEditor.Lines.Count.ToString();
            lstTextInfo.Items[2].SubItems[1].Text = TextEditor.TextLength.ToString();
            
            // sortez
            SortedWordCountList.Clear();
            foreach (string k in WordCounter.Keys)
                SortedWordCountList.Add(new PairCount(k, WordCounter[k]));
            SortedWordCountList.Sort();
            
            lstWordsCount.Items.Clear();
            string keyWords = "";
            int count = 0;
            foreach (PairCount pc in SortedWordCountList)
            {
                ListViewItem lvi = new ListViewItem(pc.Text);
                lvi.SubItems.Add(pc.Count.ToString());
                lstWordsCount.Items.Add(lvi);
                if (count < 5)
                {
                    keyWords += pc.Text + ", ";
                    count++;
                }
            }
            if (CurrentObject != null)
            {
                ((TextPublish)CurrentObject).Keywords = keyWords;
                OnChangeSelectedObjectProperty(null, null);
            }
        }
        private void OnPreviewTimer(object sender, EventArgs e)
        {
            if (!NeedRefresh)
                return;
            if (pnlTextEditor.Visible == true)
                UpdateTextInfoData();
            NeedRefresh = false;
        }
        public override void OnActivate()
        {
            if (TextEditor == null)
            {
                // text edit
                TextEditor = new ScintillaNET.Scintilla();
                pnlTextEditor.Panel1.Controls.Add(TextEditor);
                TextEditor.Dock = DockStyle.Fill;
                PrepareScintilla(TextEditor);
                pnlTextEditor.Dock = DockStyle.Fill;
            }

            // refac lista de builduri
            comboBuild.Items.Clear();
            comboBuild.Items.Add("All");
            foreach (GenericBuildConfiguration gb in Context.Prj.BuildConfigurations)
                comboBuild.Items.Add(gb.Name);
            comboBuild.SelectedIndex = 0;

            UpdatePublishObjectList();
            OnChangeSelectedPublishObjects(null, null);

            OnRefreshWorkGroupFiles(null, null);
        }

        public override void OnCommand(Command cmd, object parameters = null)
        {
            switch (cmd)
            {
                case Command.SelectAll:
                    if (TextEditor.Visible)
                        TextEditor.Selection.SelectAll();
                    break;
            }
        }

        private void OnChangeSelectedObjectProperty(object s, PropertyValueChangedEventArgs e)
        {
            foreach (ListViewItem lvi in lstPublishObjects.SelectedItems)
                UpdatePublishItem(lvi);
        }

        private void OnSaveCurrentObject(object sender, EventArgs e)
        {
            if (CurrentObject.GetType()==typeof(TextPublish))
            {
                if (Disk.SaveFile(CurrentObject.GetObjectFile(), TextEditor.Text, Context.Prj.EC) == false)
                    Context.Prj.ShowErrors();
            }
            btnSave.Enabled = false;
        }

        public void SaveCurrentContent(bool enablePreviewTimer)
        {
            if ((btnSave.Enabled) && (CurrentObject != null))
                OnSaveCurrentObject(null, null);
            PreviewTimer.Enabled = enablePreviewTimer;
        }

        private void OnAddImage(object sender, EventArgs e)
        {
            NewImagePublishObject dlg = new NewImagePublishObject(null,Context.Prj);
            if (dlg.ShowDialog() == DialogResult.OK)
                OnNewObjectAdded(dlg.Result);
        }

        private void OnEditCurrentObject(object sender, EventArgs e)
        {
            if (CurrentObject!=null)
            {
                string source = CurrentObject.GetObjectSource();
                if (source.Length>0)
                {
                    if (source.EndsWith(".svg"))
                    {
                        Context.Prj.RunCommand(Context.Settings.InskapePath, source, "Inskape", true, false);
                        Context.Prj.SVGtoPNG(source, CurrentObject.GetObjectFile(), 90.0f, -1, -1, 1.0f, true);
                        PreviewObject(CurrentObject);
                        return;
                    }
                    MessageBox.Show("Don't know how to edit: "+source);
                }
                else
                {
                    MessageBox.Show("No source object associated with the current object !");
                }

            }
        }

        private void OnPublishObjectDblClicked(object sender, EventArgs e)
        {
            OnEditCurrentObject(null, null);
        }

        private void OnRebuildPublishImage(object sender, EventArgs e)
        {
            if (CurrentObject != null)
            {
                string source = CurrentObject.GetObjectSource();
                if (source.Length > 0)
                {
                    if (source.EndsWith(".svg"))
                    {
                        Context.Prj.SVGtoPNG(source, CurrentObject.GetObjectFile(), 90.0f, -1, -1, 1.0f, true);
                        PreviewObject(CurrentObject);
                        return;
                    }
                    MessageBox.Show("Don't know how to rebuild: " + source);
                }
                else
                {
                    MessageBox.Show("No source object associated with the current object !");
                }

            }
        }

        private void OnDeletePublishObjects(object sender, EventArgs e)
        {
            if (lstPublishObjects.SelectedItems.Count==0)
            {
                MessageBox.Show("Please select at least one item for deletion !");
                return;
            }
            if (MessageBox.Show("Delete " + lstPublishObjects.SelectedItems.Count.ToString() + " items ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (ListViewItem lvi in lstPublishObjects.SelectedItems)
                {
                    PublishObject po = (PublishObject)lvi.Tag;
                    Context.Prj.PublishData.Remove(po);
                    Disk.DeleteDirectory(po.GetObjectFolder(), Context.Prj.EC);
                }
                if (Context.Prj.EC.HasErrors())
                    Context.Prj.ShowErrors();
                UpdatePublishObjectList();
            }

            
        }

        private void OnTextFilterChanged(object sender, EventArgs e)
        {
            UpdatePublishObjectList();
        }

        private void OnOpenFolder(object sender, EventArgs e)
        {
            if (CurrentObject != null)
            {
                if (File.Exists(CurrentObject.GetObjectFile()))
                    Process.Start("explorer.exe", string.Format("/select,\"{0}\"", CurrentObject.GetObjectFile()));
                else
                    MessageBox.Show("File :" + CurrentObject.GetObjectFile() + " is missing !\nRebuild it and try again !");
            }
        }

        private void OnCopyPath(object sender, EventArgs e)
        {
            if (CurrentObject != null)
            {
                if (File.Exists(CurrentObject.GetObjectFile()))
                    System.Windows.Forms.Clipboard.SetText(CurrentObject.GetObjectFile());
                else
                    MessageBox.Show("File :" + CurrentObject.GetObjectFile() + " is missing !\nRebuild it and try again !");
            }
        }

        private void OnFIlterConditionChanged(object sender, EventArgs e)
        {
            UpdatePublishObjectList();
        }

        private void OnAddVideo(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Video files|*.mp4;|All Files|*.*";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                VideoPublish tp = new VideoPublish();
                if (tp.Create(Context.Prj) == false)
                {
                    Context.Prj.ShowErrors();
                    return;
                }
                // fac si un fisier text
                if (Disk.Copy(dlg.FileName,tp.GetObjectFile(),Context.Prj.EC)==false)
                {
                    Context.Prj.ShowErrors();
                    return;
                }
                // am creat obiectul - il adaug
                tp.Name = "New video object";
                Context.Prj.PublishData.Add(tp);
                Context.Prj.PublishData.Sort();
                OnNewObjectAdded(tp);
            }
        }

        #region Work Group

        class ListViewItemComparer : IComparer
        {
            public int col = -1;
            public bool ascendent = true;

            public ListViewItemComparer(int column)
            {
                col = column;
                ascendent = true;
            }
            public int Compare(object x, object y)
            {
                if (ascendent)
                    return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                else
                    return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
            }
        }

        private void OnSendToWorkingGroup(object sender, EventArgs e)
        {
            if (lstPublishObjects.SelectedItems.Count==0)
            {
                MessageBox.Show("Please select some objects first !");
                return;
            }
            foreach (ListViewItem lvi in lstPublishObjects.SelectedItems)
            {
                PublishObject po = (PublishObject)lvi.Tag;
                string source = po.GetObjectFile();
                string dest = po.GetObjectType()+"__"+po.Name;
                dest = dest + "__[" + po.GetInformation() + "]";
                if (po.Builds.Trim().Length > 0)
                    dest = dest + "__[" + po.Builds.Trim() + "]";
                dest = dest + "__[" + po.Lang.ToString() + "]";
                dest = dest.Replace(":", " ").Replace("/", " ").Replace("\\", " ").Replace("*", " ").Replace("?", " ").Replace("\"", " ").Replace("<", " ").Replace("|", " ");
                dest = dest + "." + po.FileID;
                dest = dest + Path.GetExtension(source);
                if (File.Exists(dest))
                    continue;
                if (Disk.CreateHardLink(Path.Combine(Context.Prj.GetProjectPublishMaterialsWorkGroupFolder(),dest), source, null) == false)
                    MessageBox.Show("Can not link to: " + dest);
            }
            OnRefreshWorkGroupFiles(null, null);
        }
        private string SizeToString(long sz)
        {
            string s = "";
            int count = 0;
            while (sz > 0)
            {
                s = (char)((sz % 10) + '0') + s;
                sz = sz / 10;
                count++;
                if ((count == 3) && (sz > 0))
                {
                    s = "." + s;
                    count = 0;
                }
            }
            if (s.Length == 0)
                s = "0";
            while (s.Length < 20)
                s = " " + s;
            return s;
        }
        public void ReadWorkGroupFiles()
        {
            WorkGroupFiles.Clear();
            try
            {
                string[] fnames = Directory.GetFiles(Context.Prj.GetProjectPublishMaterialsWorkGroupFolder());
                foreach (string fname in fnames)
                {
                    FileInfo f = new FileInfo(fname);
                    FileInformation finf = new FileInformation();
                    finf.FileName = Path.GetFileName(fname);
                    string ext = Path.GetExtension(fname).ToLower();
                    if (extensionAssociations.ContainsKey(ext))
                        finf.Type = extensionAssociations[ext];
                    else
                        finf.Type = "? (" + ext + ")";
                    finf.Size = SizeToString(f.Length);
                    finf.Date = f.LastWriteTime.ToString("yyyy-MM-dd  HH:mm:ss", CultureInfo.InvariantCulture);
                    WorkGroupFiles.Add(finf);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to read files from " + Context.Prj.GetProjectPublishMaterialsWorkGroupFolder() + "\n" + e.ToString());
            }
        }
        public void PopulateWorkGroupList()
        {
            lstWorkGroupFiles.Items.Clear();
            lstWorkGroupFiles.BeginUpdate();
            string filter = txGroupFolderFiler.Text.ToLower();
            foreach (FileInformation fi in WorkGroupFiles)
            {
                if ((cbGroupFilterImages.Checked == false) && (fi.Type == "Image"))
                    continue;
                if ((cbGroupFilterTextFiles.Checked == false) && (fi.Type == "Text"))
                    continue;
                if ((cbGroupFilterVideous.Checked == false) && (fi.Type == "Video"))
                    continue;
                if ((cbGroupFilterOtherTypes.Checked == false) && (fi.Type.StartsWith("?")))
                    continue;
                bool add = false;
                if (filter.Length>0)
                {
                    if ((!add) && (fi.FileName.ToLower().Contains(filter)))
                        add = true;
                    if ((!add) && (fi.Date.ToLower().Contains(filter)))
                        add = true;
                } else {
                    add = true;
                }
                if (add)
                {
                    ListViewItem lvi = new ListViewItem(Path.GetFileNameWithoutExtension(fi.FileName));
                    lvi.SubItems.Add(fi.Type);
                    lvi.SubItems.Add(fi.Date);
                    lvi.SubItems.Add(fi.Size);
                    lvi.Tag = fi;
                    if (fi.Type.StartsWith("?"))
                        lvi.ForeColor = Color.Gray;
                    lstWorkGroupFiles.Items.Add(lvi);
                }
            }
            lstWorkGroupFiles.EndUpdate();
        }
        private void OnRefreshWorkGroupFiles(object sender, EventArgs e)
        {
            ReadWorkGroupFiles();
            PopulateWorkGroupList();
        }
        private void OnGroupFolderTextFilterChanged(object sender, EventArgs e)
        {
            PopulateWorkGroupList();
        }
        private void OnOpenGroupFolder(object sender, EventArgs e)
        {
            Process.Start(Context.Prj.GetProjectPublishMaterialsWorkGroupFolder());
        }
        private void OnGroupFolderDeleteFiles(object sender, EventArgs e)
        {
            if (lstWorkGroupFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select some objects first !");
                return;
            }
            if (MessageBox.Show("Delete " + lstWorkGroupFiles.SelectedItems.Count.ToString() + " file(s) ?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (ListViewItem lvi in lstWorkGroupFiles.SelectedItems)
                {
                    FileInformation fi = (FileInformation)lvi.Tag;
                    if (Disk.DeleteFile(Path.Combine(Context.Prj.GetProjectPublishMaterialsWorkGroupFolder(), fi.FileName), null) == false)
                        MessageBox.Show("Failed to delete: " + fi.FileName);
                }
                OnRefreshWorkGroupFiles(null, null);
            }
        }

        private void OnEmptyGroupFolder(object sender, EventArgs e)
        {
            if (WorkGroupFiles.Count==0)
            {
                MessageBox.Show("Work group is already empty !");
                return;
            }
            if (MessageBox.Show("Empty all " + WorkGroupFiles.Count.ToString() + " file(s) from Work group?", "Empty", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (FileInformation fi in WorkGroupFiles)
                {
                    if (Disk.DeleteFile(Path.Combine(Context.Prj.GetProjectPublishMaterialsWorkGroupFolder(), fi.FileName), null) == false)
                        MessageBox.Show("Failed to delete: " + fi.FileName);
                }
                OnRefreshWorkGroupFiles(null, null);
            }
        }
        private void OnSortGroupFiles(object sender, ColumnClickEventArgs e)
        {
            if (this.lstWorkGroupFiles.ListViewItemSorter != null)
            {
                ListViewItemComparer lvic = (ListViewItemComparer)this.lstWorkGroupFiles.ListViewItemSorter;
                if (lvic.col == e.Column)
                    lvic.ascendent = !lvic.ascendent;
                else
                {
                    lvic.ascendent = true;
                    lvic.col = e.Column;
                }
                this.lstWorkGroupFiles.Sort();
            }
            else
            {
                this.lstWorkGroupFiles.ListViewItemSorter = new ListViewItemComparer(e.Column);
            }
        }        
        
        #endregion

        private void OnShowTemplates(object sender, EventArgs e)
        {
            TemplateImages dlg = new TemplateImages(true);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string resultName = Path.Combine(Context.Prj.ProjectPath, "temp", "Temp_" + System.Environment.TickCount.ToString() + ".svg");
                if (Disk.Copy(dlg.TemplatePath, resultName,Context.Prj.EC)==false)
                {
                    Context.Prj.ShowErrors();
                    return;
                }
                if ((Context.Settings.InskapePath!=null) && (Context.Settings.InskapePath.Length>0))
                {
                    Context.Prj.RunCommand(Context.Settings.InskapePath, resultName, "Inkscape", false, false);
                }
                else
                {
                    MessageBox.Show("Please set the path to Inkscape in settings !");
                }                
            }
        }


    }
}
