using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ScintillaNET;

namespace GAppCreator
{
    public class GACEditor: ScintillaNET.Scintilla
    {
        public enum FindResult
        {
            Found,
            EndOfDocument,
            NotFound
        }
        public class FindReplaceSettings
        {
            public string TextToFind;
            public string ReplaceText;
            public bool MatchCase;
            public bool WholeWord;
            public ScintillaNET.SearchFlags Options;
        }
        class PositionError
        {
            public int start,end;
            public string text;
            public PositionError()
            {
                start = LineErrorAnalyzer.StartPosition;
                end = LineErrorAnalyzer.EndPosition;
                text = LineErrorAnalyzer.Content;
                if (text.Contains("in line"))
                    text = text.Replace("in line", "\r\nLine:");
            }
        };
        public string fullFilePath;
        Project prj;
        GACParser parser;
        TabPage tabPage;
        string[] autocompleteWords = new string[32];
        List<PositionError> posErrors = new List<PositionError>();
        public bool HasTextBeenModified = false;
        public bool HasTextBeenModifiedAfterIntelliSense = true;
        private List<string> AutoCompleteList = new List<string>();
        
        private static string lexTypes = "";
        private static string lexConstants = "";



        public GACEditor(Project p, string fileName, GACParser gac_parser,TabPage tab_page,ContextMenuStrip menu )
        {
            prj = p;
            parser = gac_parser;
            HasTextBeenModified = false;
            tabPage = tab_page;

            this.Commands.RemoveBinding(Keys.F, Keys.Control, BindableCommand.ShowFind);
            this.Commands.RemoveBinding(Keys.H, Keys.Control, BindableCommand.ShowReplace);
            this.ContextMenuStrip = menu;
            if (fileName.ToLower().EndsWith(".gac"))
            {
                InitGACEditor(fileName);                
                return;
            }
            if ((fileName.ToLower().EndsWith(".h")) || (fileName.ToLower().EndsWith(".cpp")))
            {
                InitCPPEditor(fileName);
                return;
            }



        }


        void InitCPPEditor(string fileName)
        {
            fullFilePath = Path.Combine(prj.ProjectPath, "CppProject", fileName);
            this.Text = Disk.ReadFileAsString(fullFilePath, prj.EC);
            
            this.Margins[0].Width = 20;
            this.Margins[1].Width = 20;
            this.Margins[1].IsFoldMargin = true;
            this.Margins[1].IsMarkerMargin = false;
            this.Margins[1].IsClickable = true;

            this.Indentation.TabWidth = 6;
            this.ConfigurationManager.Language = "cpp";
            
            Caret.HighlightCurrentLine = true;
            Caret.CurrentLineBackgroundColor = System.Drawing.Color.FromArgb(255, 212, 212);        

            this.IsReadOnly = true;
        }
        public void SetSyntaxColorsForGAC()
        {
            this.BackColor = System.Drawing.Color.White;
            foreach (string s in this.Lexing.StyleNameMap.Keys)
                Styles[this.Lexing.StyleNameMap[s]].BackColor = this.BackColor;
            Styles[this.Lexing.StyleNameMap["COMMENT"]].ForeColor = System.Drawing.Color.Gray;
            Styles[this.Lexing.StyleNameMap["COMMENTLINE"]].ForeColor = System.Drawing.Color.Gray;
            Styles[this.Lexing.StyleNameMap["WORD"]].ForeColor = System.Drawing.Color.Blue;
            Styles[this.Lexing.StyleNameMap["WORD"]].Bold = true;
            Styles[this.Lexing.StyleNameMap["STRING"]].ForeColor = System.Drawing.Color.Red;
            Styles[this.Lexing.StyleNameMap["CHARACTER"]].ForeColor = System.Drawing.Color.DarkRed;
            Styles[this.Lexing.StyleNameMap["CONTROLCHAR"]].ForeColor = System.Drawing.Color.DarkRed;
            Styles[this.Lexing.StyleNameMap["PREPROCESSOR"]].ForeColor = System.Drawing.Color.Green;
            Styles[this.Lexing.StyleNameMap["OPERATOR"]].ForeColor = System.Drawing.Color.Black;
            Styles[this.Lexing.StyleNameMap["WORD2"]].ForeColor = System.Drawing.Color.DarkCyan;
            Styles[this.Lexing.StyleNameMap["NUMBER"]].ForeColor = System.Drawing.Color.DarkMagenta;
            Styles[this.Lexing.StyleNameMap["NUMBER"]].Bold = true;
            Styles[this.Lexing.StyleNameMap["BRACEBAD"]].ForeColor = System.Drawing.Color.White;
            Styles[this.Lexing.StyleNameMap["BRACEBAD"]].BackColor = System.Drawing.Color.DarkRed;
            Styles[this.Lexing.StyleNameMap["BRACEBAD"]].Bold = true;
            Styles[this.Lexing.StyleNameMap["BRACELIGHT"]].ForeColor = System.Drawing.Color.Yellow;
            Styles[this.Lexing.StyleNameMap["BRACELIGHT"]].BackColor = System.Drawing.Color.Green;
            Styles[this.Lexing.StyleNameMap["GLOBALCLASS"]].ForeColor = System.Drawing.Color.DarkMagenta;
            Styles[this.Lexing.StyleNameMap["GLOBALCLASS"]].Bold = true;
            Styles[this.Lexing.StyleNameMap["IDENTIFIER"]].ForeColor = System.Drawing.Color.Black;

            Caret.HighlightCurrentLine = true;
            Caret.CurrentLineBackgroundColor = System.Drawing.Color.AliceBlue;
            this.Indicators[0].Style = IndicatorStyle.RoundBox;
            this.Indicators[0].Color = System.Drawing.Color.Red;
            this.Indicators[1].Style = IndicatorStyle.Squiggle;
            this.Indicators[1].Color = System.Drawing.Color.Red;
            this.Selection.BackColorUnfocused = System.Drawing.Color.DarkGreen;
            this.Selection.ForeColorUnfocused = System.Drawing.Color.Yellow;


            System.Drawing.Font fnt = new System.Drawing.Font("Consolas", 10);
            for (int tr = 0; tr < Styles.GetEndStyled(); tr++)
                Styles[tr].Font = fnt;

            this.UpdateStyles();
            this.Lexing.Colorize();
        }
        public void EnableDisableEditor(bool enable)
        {
            //if (enable)
            //    this.BackColor = System.Drawing.Color.White;
            //else
            //    this.BackColor = System.Drawing.Color.LightGray;
        }
        //public void GraySyntaxColorForGAC()
        //{
        //    this.BackColor = System.Drawing.Color.LightGray;
        //    System.Drawing.Color col = System.Drawing.Color.Gray;
        //    Styles[this.Lexing.StyleNameMap["COMMENT"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["COMMENTLINE"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["WORD"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["STRING"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["CHARACTER"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["CONTROLCHAR"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["PREPROCESSOR"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["OPERATOR"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["WORD2"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["NUMBER"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["BRACEBAD"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["BRACELIGHT"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["GLOBALCLASS"]].ForeColor = col;
        //    Styles[this.Lexing.StyleNameMap["IDENTIFIER"]].ForeColor = col;
        //    foreach (string s in this.Lexing.StyleNameMap.Keys)
        //        Styles[this.Lexing.StyleNameMap[s]].BackColor = this.BackColor;
        //    Caret.HighlightCurrentLine = true;
        //    Caret.CurrentLineBackgroundColor = System.Drawing.Color.Yellow;
        //    this.Indicators[0].Style = IndicatorStyle.RoundBox;
        //    this.Indicators[0].Color = System.Drawing.Color.Blue;
        //    this.Indicators[1].Style = IndicatorStyle.Squiggle;
        //    this.Indicators[1].Color = System.Drawing.Color.Gray;
        //    this.Selection.BackColorUnfocused = System.Drawing.Color.Yellow;
        //    this.Selection.ForeColorUnfocused = System.Drawing.Color.Black;
            

        //    System.Drawing.Font fnt = new System.Drawing.Font("Consolas", 10);
        //    for (int tr = 0; tr < Styles.GetEndStyled(); tr++)
        //        Styles[tr].Font = fnt;

        //    this.UpdateStyles();
        //    this.Lexing.Colorize();
        //}
        void InitGACEditor(string fileName)
        {
            fullFilePath = Path.Combine(prj.ProjectPath, "Sources", fileName);
            tabPage.ImageKey = "";
            this.Text = Disk.ReadFileAsString(fullFilePath, prj.EC).Replace("\r\n", "\n").Replace("\r", "\n");
            
            this.Margins[0].Width = 30;
            this.Margins[1].Width = 12;
            this.Margins[1].Type = MarginType.Symbol;
            this.Margins[2].Width = 20;
            this.Margins[2].IsFoldMargin = true;
            this.Margins[2].IsMarkerMargin = false;
            this.Margins[2].IsClickable = true;


            this.EndOfLine.Mode = EndOfLineMode.LF;

            //this.ConfigurationManager.Language = "c#";
            this.IsBraceMatching = true;

            this.Indentation.TabWidth = 6;
            this.Indentation.ShowGuides = true;
            this.Indentation.SmartIndentType = SmartIndent.CPP;

            this.Folding.IsEnabled = true;
            this.Folding.UseCompactFolding = true;

            this.Lexing.Lexer = ScintillaNET.Lexer.Cpp;


            SetSyntaxColorsForGAC();
            //GraySyntaxColorForGAC();


            

            this.Lexing.SetKeywords(0, GACParser.Compiler.GetKeywordsList());

            // tipuri cunoscute
            UpdateWithLocalTypes();

            this.Lexing.SetKeywords(1, lexTypes);
            this.Lexing.SetKeywords(3, lexConstants);



            this.Markers[0].Symbol = MarkerSymbol.Circle;
            this.Markers[0].BackColor = System.Drawing.Color.Red;

            this.SelectionChanged += new EventHandler(GACEditor_SelectionChanged);

            this.AutoComplete.IsCaseSensitive = false;
            this.AutoComplete.DropRestOfWord = false;
            //this.AutoComplete.FillUpCharacters = ".";
            this.AutoCompleteAccepted += new EventHandler<AutoCompleteAcceptedEventArgs>(GACEditor_AutoCompleteAccepted);           

            for (int tr = 0; tr < 32; tr++)
                autocompleteWords[tr] = "";
            this.CharAdded += new EventHandler<CharAddedEventArgs>(GACEditor_CharAdded);
            

            this.AutoComplete.RegisterImages(GACParser.AutocompleteIcons,System.Drawing.Color.Magenta);

            this.GotFocus += new EventHandler(GACEditor_GotFocus);

            this.CallTip.OverloadList = new OverloadList(16);

            this.DwellStart += new EventHandler<ScintillaMouseEventArgs>(GACEditor_DwellStart);
            this.NativeInterface.SetMouseDwellTime(1000);

            this.TextInserted += new EventHandler<TextModifiedEventArgs>(GACEditor_TextInserted);
            this.TextDeleted += new EventHandler<TextModifiedEventArgs>(GACEditor_TextDeleted);
        }

        void GACEditor_AutoCompleteAccepted(object sender, AutoCompleteAcceptedEventArgs e)
        {
            //Console.WriteLine("AutoComplete -> " + e.Text);
            //if (PunctApasat)
            //{
            //    e.Cancel = true;
            //    ShowAutoComplete();
            //    PunctApasat = false;
            //}
        }

        void OnTextHasBeenModified()
        {
            HasTextBeenModifiedAfterIntelliSense = true;
            if (HasTextBeenModified == false)
            {
                tabPage.ImageKey = "save";;
                HasTextBeenModified = true;
            }
        }
        void GACEditor_TextDeleted(object sender, TextModifiedEventArgs e)
        {
            OnTextHasBeenModified();
        }

        void GACEditor_TextInserted(object sender, TextModifiedEventArgs e)
        {
            OnTextHasBeenModified();
        }
        void GACEditor_GotFocus(object sender, EventArgs e)
        {
            this.Lexing.SetKeywords(1, lexTypes);
            this.Lexing.SetKeywords(3, lexConstants);            
        }
        public void ClearSyntaxError()
        {
            this.GetRange().ClearIndicator(1);
            for (int tr = 0; tr < Lines.Count; tr++)
                Lines[tr].DeleteMarker(0);
            posErrors.Clear();
        }
        public void AddSyntaxError(string error)
        {
            LineErrorAnalyzer.Analyze(error);
            if (LineErrorAnalyzer.StartPosition >= 0)
            {
                Range r = new Range(LineErrorAnalyzer.StartPosition, LineErrorAnalyzer.EndPosition, this);
                r.SetIndicator(1);
                Lines[LineErrorAnalyzer.Line-1].AddMarker(this.Markers[0]);
                posErrors.Add(new PositionError());
            }
        }
        public static void UpdateWithLocalTypes()
        {
            lexTypes = GACParser.Compiler.GetTypesList();
            lexConstants = GACParser.Compiler.GetConstantsList();
        }
        private int ParseLine(string ss, int cursorPoz)
        {
            int startPoz = cursorPoz - 1;
            int wIndex = 0;
            int endPoz = cursorPoz;
            if (startPoz < 0)
                return 0;
            while (startPoz >= 0)
            {
                char ch = ss[startPoz];
                if (((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z')) || (ch == '_') || (ch == '.') || ((ch >= '0') && (ch <= '9')))
                {
                    if (ch == '.')
                    {
                        autocompleteWords[wIndex] = ss.Substring(startPoz + 1, endPoz - (startPoz + 1));
                        endPoz = startPoz;
                        wIndex++;
                    }
                    startPoz--;
                    continue;
                }
                break;
            }
            if (startPoz + 1 < endPoz)
            {
                // daca chiar am ceva acolo in buffer
                autocompleteWords[wIndex] = ss.Substring(startPoz + 1, endPoz - (startPoz + 1));
                wIndex++;
            }
            return wIndex;
        }
        public void ShowCallTipList(GACParser.Member member)
        {
            if ((member.Overrides == null) || (member.Overrides.Count == 0))
            {
                this.CallTip.Message = member.GetToolTip(Parent.Name);
                this.CallTip.Show();
            }
            else
            {
                if (this.CallTip.OverloadList == null)
                    this.CallTip.OverloadList = new ScintillaNET.OverloadList();
                this.CallTip.OverloadList.Clear();
                this.CallTip.OverloadList.Add(member.GetToolTip(Parent.Name));
                foreach (GACParser.Member mb in member.Overrides)
                    this.CallTip.OverloadList.Add(mb.GetToolTip(Parent.Name));
                this.CallTip.ShowOverload();
            }
        }
        void ShowFunctionCallTip()
        {
            //*
            this.CallTip.Hide();
            int cursorPoz = 0;
            GACParser.Module c = null;
            string ss = this.GetCurrentLine(out cursorPoz);
            int wIndex = ParseLine(ss, cursorPoz-1);
            if (wIndex <= 0)
                return;

            // verific sa nu fie un constructor
            c = GACParser.Compiler.GetModule(autocompleteWords[0]);
            if (c != null)
            {
                GACParser.Member f = c.GetMember("");
                if (f != null)
                    ShowCallTipList(f);
                return;
            }

            // caut pozitia mea in fisier
            GACParser.Module cls = null;
            GACParser.Member fnc = null;
            if (parser.CursorToFunction(this.CurrentPos, out cls, out fnc) == false)
            {
                cls = null;
                fnc = null;
            }
            if (wIndex == 1)
            {
                if (cls != null)
                {
                    GACParser.Member f = cls.GetMember(autocompleteWords[0]);
                    if (f != null)
                        ShowCallTipList(f);
                }
                return;
            }
            
            // verific "this"
            if (autocompleteWords[1] == "this")
            {
                if (cls != null)
                {
                    GACParser.Member f = cls.GetMember(autocompleteWords[0]);
                    if (f != null)
                        ShowCallTipList(f);
                }
            }
            // vad tipul lui - fac doar la 2 nivele
            c = GACParser.Compiler.GetModule(autocompleteWords[1]);
            if (c == null)
            {
                // nu e o clasa statica - este o variabila locala.functie()
                if (fnc != null)
                {
                    GACParser.Member localVar = fnc.GetLocalVariableOrParameter(autocompleteWords[1]);
                    if (localVar == null)
                        localVar = cls.GetMember(autocompleteWords[1]);
                    if (localVar != null)
                    {
                        c = GACParser.Compiler.GetModule(localVar.DataType);
                        if (c != null)
                        {
                            GACParser.Member f = c.GetMember(autocompleteWords[0]);
                            if (f != null)
                                ShowCallTipList(f);
                        }
                    }
                }
            }
            else
            {
                // e o clasa statica - 
                GACParser.Member f = c.GetMember(autocompleteWords[0]);
                ShowCallTipList(f);
            }

             //*/
        }
        public void GetCurrentLocation(GACParser.Location location)
        {
            location.CursorPositionInLine = 0;
            location.Line = GetCurrentLine(out location.CursorPositionInLine);
            location.module = null;
            location.member = null;
            parser.CursorToFunction(this.CurrentPos, out location.module, out location.member);
        }
        public GACParser.Location GetCurrentLocation()
        {
            GACParser.Location location = new GACParser.Location();
            GetCurrentLocation(location);
            return location;
        }
        void ShowAutoComplete()
        {
            //*
            int cursorPoz = 0;
            string ss = this.GetCurrentLine(out cursorPoz);
            int wIndex = ParseLine(ss, cursorPoz);

            // caut pozitia mea in fisier
            GACParser.Module cls = null;
            GACParser.Member fnc = null;
            if (parser.CursorToFunction(this.CurrentPos, out cls, out fnc) == false)
            {
                cls = null;
                fnc = null;
            }

            // aleg lista in functie de ce cuvinte sunt
            if (wIndex == 1)
            {
                AutoCompleteList.Clear();
                AutoCompleteList.AddRange(GACParser.Compiler.AutoCompleteList);
                // pun si variabilele interne
                if (cls != null)
                    AutoCompleteList.AddRange(cls.AutoCompleteListNonStatic);
                if (fnc != null)
                {
                    List<GACParser.Member> Parameters = fnc.GetParameters();
                    if (Parameters != null)
                    {
                        foreach (GACParser.Member mb in Parameters)
                            AutoCompleteList.Add(mb.Name + "?" + ((int)GACParser.GACIcon.Variable).ToString());
                    }
                    if (fnc.LocalVariables != null)
                    {
                        foreach (string name in fnc.LocalVariables.Keys)
                            AutoCompleteList.Add(name + "?" + ((int)GACParser.GACIcon.Variable).ToString());
                    }
                }

                AutoCompleteList.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
                this.AutoComplete.List = AutoCompleteList;
                this.AutoComplete.Show(autocompleteWords[0].Length);
            }
            else
            {
                // daca e o clasa/namespace/enum
                GACParser.Module m = GACParser.Compiler.GetModule(autocompleteWords[1]);

                if (m != null)
                {
                    if (m.Name == "Global")
                        this.AutoComplete.List = m.AutoCompleteListNonStatic;
                    else { 
                        this.AutoComplete.List = m.AutoCompleteListStatic;
                        // verific daca nu am ceva si in extra comenzi
                        if (GACParser.ExtraConstants.ContainsKey(m.Name))
                        {
                            foreach (string key in GACParser.ExtraConstants[m.Name].Keys)
                                this.AutoComplete.List.Add(key+"?4");
                            this.AutoComplete.List.Sort();
                        }
                    }
                    this.AutoComplete.Show(autocompleteWords[0].Length);
                    return;
                }

                // vad tipul lui - fac doar la 2 nivele
                if (autocompleteWords[1].Equals("this") && (cls!=null))
                {
                    this.AutoComplete.List = cls.AutoCompleteListNonStatic;
                    this.AutoComplete.Show(autocompleteWords[0].Length);
                    return;
                }
                // vad daca nu cumva e un membru al clasei / functiei
                GACParser.Member obj = null;
                if (cls != null)
                {
                    obj = cls.GetMember(autocompleteWords[1]);
                    if ((obj == null) && (fnc != null))
                        obj = fnc.GetLocalVariableOrParameter(autocompleteWords[1]);
                }
                if (obj != null)
                {
                    GACParser.Module mobj = GACParser.Compiler.GetModule(obj.DataType);
                    if (mobj != null)
                    {
                        this.AutoComplete.List = mobj.AutoCompleteListNonStatic;
                        this.AutoComplete.Show(autocompleteWords[0].Length);
                        return;
                    }
                }
                // testez si nivelul 2 Clasa.Variabila.ceva
                if (wIndex >= 3)
                {
                    m = GACParser.Compiler.GetModule(autocompleteWords[2]);
                    if (m != null)
                    {
                        GACParser.Member mb = m.GetMember(autocompleteWords[1]);
                        if (mb != null)
                        {
                            m = GACParser.Compiler.GetModule(mb.DataType);
                            if (m != null)
                            {
                                this.AutoComplete.List = m.AutoCompleteListNonStatic;
                                this.AutoComplete.Show(autocompleteWords[0].Length);
                            }
                        }
                    }
                }
            }

            //*/
        }
        void ShowPrecompileCommands()
        {            
            this.AutoComplete.List = GACParser.preprocessAutoCompleteList;
            this.AutoComplete.Show(1);
        }
        void GACEditor_CharAdded(object sender, CharAddedEventArgs e)
        {
            //if (e.Ch == '[')
            //{
            //    this.InsertText(this.CurrentPos, "]");
            //}
            if (e.Ch == '(')
            {
                bool add = this.CurrentPos>=this.TextLength;
                if (!add)
                {
                    if (this.CharAt(this.CurrentPos) < 32)
                        add = true;
                }
                if (add)
                    this.InsertText(this.CurrentPos, ")");
            }
            if (e.Ch == '{')
            {
                string cline = this.GetCurrentLine();
                int sz = 0;
                for (; (sz < cline.Length) && (cline[sz] <= 32); sz++);
                this.InsertText(this.CurrentPos, "\n"+cline.Substring(0, sz) + "\t\n" + cline.Substring(0, sz) + "}\n");
                this.CurrentPos += sz + 2;    
            }
            //PunctApasat = (e.Ch == '.');
            if (((e.Ch >= 'a') && (e.Ch <= 'z')) || ((e.Ch >= 'A') && (e.Ch <= 'Z')) || (e.Ch == '_') || (e.Ch == '.'))
            {
                //if (e.Ch == '.')
                //    Console.WriteLine("punct apasat");
                
                /*
                if ((e.Ch == '.') && (this.AutoComplete.IsActive == true))
                {
                    this.AutoComplete.Accept();
                    return;
                }
                 */
                //if (PunctApasat && (this.AutoComplete.IsActive == true))
                //    PunctApasat = false; 
                if ((this.AutoComplete.IsActive == false) || (e.Ch == '.'))
                //if (this.AutoComplete.IsActive == false)
                    ShowAutoComplete();
            }
            else if (e.Ch == '(')
            {
                ShowFunctionCallTip();
            }
            else if (e.Ch == '#')
            {
                ShowPrecompileCommands();
            }
            else
            {
                if (this.AutoComplete.IsActive)
                    this.AutoComplete.Cancel();
            }            
        }
        void GACEditor_SelectionChanged(object sender, EventArgs e)
        {
            this.GetRange().ClearIndicator(0);
            if (this.Selection.Start < this.Selection.End)
            {
                this.FindReplace.Flags = ScintillaNET.SearchFlags.WholeWord;
                foreach (Range r in this.FindReplace.FindAll(this.Selection.Text))
                    if ((r.Start!=Selection.Start) || (r.End!=Selection.End))
                        r.SetIndicator(0);
            }            
        }
        void GACEditor_DwellStart(object sender, ScintillaMouseEventArgs e)
        {
            if (posErrors.Count > 0)
            {
                int pos = this.PositionFromPoint(e.X, e.Y);
                foreach (PositionError pe in posErrors)
                    if ((pe.start<=pos) && (pe.end>=pos))
                    {
                        this.CallTip.Show(pe.text,pos);
                        return;
                    }
                this.CallTip.Hide();
            }
        }
        public bool SaveFile()
        {
            // verific daca trebuei sa salvez
            string txt = Disk.ReadFileAsString(fullFilePath, prj.EC);
            bool aceleasiFisiere = (txt.Replace("\n", "").Replace("\r", "") == Text.Replace("\n", "").Replace("\r", ""));
            if ((!aceleasiFisiere) || (prj.EC.HasErrors()))
            {
                if (Disk.SaveFile(fullFilePath, Text, prj.EC) == true)
                {
                    HasTextBeenModified = false;
                    tabPage.ImageKey = "";
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (aceleasiFisiere)
            {
                HasTextBeenModified = false;
                tabPage.ImageKey = "";
                return true;
            }
            return false;
        }
        public void Reload()
        {
            this.Text = Disk.ReadFileAsString(fullFilePath, prj.EC).Replace("\r\n", "\n").Replace("\r", "\n");
            ClearSyntaxError();
            HasTextBeenModified = false;
            tabPage.ImageKey = "";
        }
        public void FoldAll()
        {
            foreach (ScintillaNET.Line linear in this.Lines)
            {
                if (linear.IsFoldPoint && linear.FoldExpanded)
                {
                    linear.ToggleFoldExpanded();
                }
            }
        }
        public void UnfoldAll()
        {
            foreach (ScintillaNET.Line linear in this.Lines)
            {
                if (linear.IsFoldPoint && !linear.FoldExpanded)
                {
                    linear.ToggleFoldExpanded();
                }
            }
        }
        public void CollapseToDefinitions()
        {
            
            foreach (ScintillaNET.Line linear in this.Lines)
            {
                if ((linear.IsFoldPoint) && (linear.FoldExpanded) && (linear.FoldLevel>1024))
                {
                    linear.ToggleFoldExpanded();
                }
            }
        }
        public void GoToRange(Range r)
        {
            Line cLine = r.StartingLine;
            while (cLine != null)
            {
                if (cLine.FoldExpanded == false)
                    cLine.ToggleFoldExpanded();
                cLine = cLine.FoldParent;
            }
            Caret.Goto(r.Start);
            r.ShowLines();
            r.Select();
        }
        public void GoToLine(int lineNo)
        {
            if ((lineNo>=0) && (lineNo<Lines.Count))
            {
                GoToRange(new Range(Lines[lineNo].StartPosition,Lines[lineNo].StartPosition,this));
            }
        }
        public FindResult FindText(FindReplaceSettings settings, bool next, bool notifyWhenEndOfDocumentIsReached)
        {
            Range r;
            if (next) 
                r = FindReplace.FindNext(settings.TextToFind,settings.Options);
            else
                r = FindReplace.FindPrevious(settings.TextToFind, settings.Options);
            
            if (r!=null)
            {
                if ((r.Start < this.CurrentPos) && (next) && (notifyWhenEndOfDocumentIsReached))
                    return FindResult.EndOfDocument;
                if ((r.Start > this.CurrentPos) && (!next) && (notifyWhenEndOfDocumentIsReached))
                    return FindResult.EndOfDocument;
                GoToRange(r);
                return FindResult.Found;
            }
            return FindResult.NotFound;
        }
        public string GetSelectionOrCurrentWord()
        {
            if (Selection.Length>0)
                return Selection.Text;
            string s = this.GetWordFromPosition(this.CurrentPos);
            if ((s == null) || (s.Length == 0))
                return null;
            return s;
        }
        public void CommenOrUnCommentSelection(bool comment)
        {
            int orig_start,start, end;
            if (Selection.Length==0)
            {
                start = end = Lines.Current.Number;
            }
            else
            {

                start = Lines.FromPosition(Selection.Start).Number;
                end = Lines.FromPosition(Selection.End).Number;
            }
            // comentez liniile
            orig_start = start;
            while (start<=end)
            {
                string s = Lines[start].Text.Trim();
                if (s.Length>0)
                {
                    if ((comment) && (s.StartsWith("//")==false))
                    {
                        Lines[start].Text = "//" + Lines[start].Text.TrimEnd();
                    }
                    if ((comment==false) && (s.StartsWith("//") == true))
                    {
                        s = Lines[start].Text.TrimEnd();
                        int index = s.IndexOf("//");
                        int end_index = index;
                        while ((end_index + 2 <= s.Length) && (s[end_index] == '/') && (s[end_index + 1] == '/'))
                            end_index += 2;
                        Lines[start].Text = s.Substring(0, index) + s.Substring(end_index);
                    }
                }
                start++;
            }
            Selection.Start = Lines[orig_start].StartPosition;
            Selection.End = Lines[end].EndPosition;
        }
    }
}
