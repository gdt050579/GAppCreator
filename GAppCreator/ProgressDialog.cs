using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GAppCreator
{
    public partial class ProgressDialog : Form
    {
        enum ProgressActions
        {
            ImportFiles,
            FontGlyphCreator,
            FontGlyphCreatorFromSVG,
            FontGlyphBuild,
            
        };
        ProgressActions pa;
        Project prj;
        
        
        


        public ProgressDialog(Project p)
        {
            InitializeComponent();
            prj = p;            
        }

        #region Import files
        
        private string[] ImportFiles_files;
        string ImportFiles_toPath;

        public void SetImportFiles(string []list)
        {
            pa = ProgressActions.ImportFiles;
            ImportFiles_files = list;
            progBar.Minimum = 0;
            progBar.Maximum = list.Length;
            progBar.Value = 0;
            ImportFiles_toPath = prj.GetProjectResourceSourceFolder();
            this.Text = "Import resources";
        }
        private void PerformWork_ImportFiles()
        {
            for (int tr = 0; (tr < ImportFiles_files.Length) && (worker.CancellationPending==false); tr++)
            {
                worker.ReportProgress(tr, Path.GetFileName(ImportFiles_files[tr]));
                Disk.Copy(ImportFiles_files[tr], Path.Combine(ImportFiles_toPath, Path.GetFileName(ImportFiles_files[tr])), null);
            }
        }
        #endregion


        #region Font Glyph Creator
        List<int> FontGlyphCreator_code;
        string FontImageCreator_svg;
        FontResource FontGlyphCreator_fnt;
        public void SetFontGlyphCreator(List<int> codes,string svg,FontResource fnt)
        {
            pa = ProgressActions.FontGlyphCreator;
            FontGlyphCreator_code = codes;
            FontGlyphCreator_fnt = fnt;
            FontImageCreator_svg = svg;
            progBar.Minimum = 0;
            progBar.Maximum = codes.Count;
            progBar.Value = 0;
            this.Text = "Creating font SVGs";
        }
        public void PerformWork_FontGlyphCreator()
        {
            FontGlyphCreator_code.Sort();
            int index = 0;
            for (int tr=0;tr<FontGlyphCreator_code.Count;tr++)
            {
                int code = FontGlyphCreator_code[tr];
                if (worker.CancellationPending)
                    break;

                string output = FontGlyphCreator_fnt.GetGlyphSourcePath(code);
                worker.ReportProgress(index, "Character: '"+(char)code+"' Dec:"+code.ToString());
                index++;
                if (Disk.SaveFile(output, FontImageCreator_svg.Replace("$$CHAR$$", "&#" + code.ToString() + ";"), prj.EC) == false)
                    continue;
                if (prj.ResizeSVGToDrawing(output, true) == false)
                    FontGlyphCreator_code[tr] = 0;
            }
        }
        #endregion

        #region Font Glyph Creator from SVGs templates
        Dictionary<int, string> FontGlyphCreatorFromSVGsTemplates_code;
        FontResource FontGlyphCreatorFromSVGsTemplates_fnt;
        public void SetFontGlyphCreatorFromSVGsTemplates(Dictionary<int,string> codes, FontResource fnt)
        {
            pa = ProgressActions.FontGlyphCreatorFromSVG;
            FontGlyphCreatorFromSVGsTemplates_code = codes;
            FontGlyphCreatorFromSVGsTemplates_fnt = fnt;
            progBar.Minimum = 0;
            progBar.Maximum = codes.Count;
            progBar.Value = 0;
            this.Text = "Creating font SVGs";
        }
        public void PerformWork_FontGlyphCreatorFromSVGsTemplates()
        {
            int index = 0;
            foreach (int code in FontGlyphCreatorFromSVGsTemplates_code.Keys)
            {
                if (worker.CancellationPending)
                    break;

                string output = FontGlyphCreatorFromSVGsTemplates_fnt.GetGlyphSourcePath(code);
                worker.ReportProgress(index, "Character: '" + (char)code + "' Dec:" + code.ToString());
                index++;
                if (Disk.SaveFile(output, FontGlyphCreatorFromSVGsTemplates_code[code].Replace("$$CHAR$$", "&#" + code.ToString() + ";"), prj.EC) == false)
                    continue;
                if (prj.ResizeSVGToDrawing(output, true) == false)
                    FontGlyphCreatorFromSVGsTemplates_code[code] = null;
            }
        }
        #endregion


        #region Font Glyph Build
        List<int> FontGlyphBuild_code;
        FontResource FontGlyphBuild_fnt;
        public void SetFontGlyphBuild(List<int> codes, FontResource fnt)
        {
            pa = ProgressActions.FontGlyphBuild;
            FontGlyphBuild_code = codes;
            FontGlyphBuild_fnt = fnt;
            progBar.Minimum = 0;
            progBar.Maximum = codes.Count;
            progBar.Value = 0;
            this.Text = "Creating font images";
        }
        public void PerformWork_FontGlyphBuild()
        {
            FontGlyphBuild_code.Sort();
            int index = 0;
            Dictionary<int,Glyph> d = FontGlyphBuild_fnt.GetCharSet();
            for (int tr = 0; tr < FontGlyphBuild_code.Count; tr++)
            {
                int code = FontGlyphBuild_code[tr];
                if (worker.CancellationPending)
                    break;
                if ((code<32) || (d.ContainsKey(code)==false))
                {
                    index++;
                    continue;
                }
                Glyph g = d[code];
                Glyph.GlyphVersionInfo vi = g.GetVersion();
                foreach (string rez in vi.Rezolutions.Keys)
                {
                    string output = FontGlyphBuild_fnt.GetGlyphOutputPath(code, rez);
                    if (output!=null)
                    {
                        worker.ReportProgress(index, "Character: '" + (char)code + "' Dec:" + code.ToString()+" -> "+rez);
                        FontGlyphBuild_fnt.BuildGlyph(g, rez);
                    }
                }
                index++;
            }
        }
        #endregion

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            switch (pa)
            {
                case ProgressActions.ImportFiles:
                    PerformWork_ImportFiles();
                    break;
                case ProgressActions.FontGlyphCreator:
                    PerformWork_FontGlyphCreator();
                    break;
                case ProgressActions.FontGlyphCreatorFromSVG:
                    PerformWork_FontGlyphCreatorFromSVGsTemplates();
                    break;
                case ProgressActions.FontGlyphBuild:
                    PerformWork_FontGlyphBuild();
                    break;
            }
        }

        #region Task functions

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progBar.Value = e.ProgressPercentage;
            lbFile.Text = (String)e.UserState;
        }

        private void OnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            worker.RunWorkerAsync();
        }

        private void OnCancelWork(object sender, EventArgs e)
        {
            try
            {
                worker.CancelAsync();
                btnCancel.Enabled = false;
            }
            catch (Exception)
            {
            }
        }

        public void Start()
        {
            this.ShowDialog();
            prj.ShowErrors();
        }

        #endregion
    }
}
