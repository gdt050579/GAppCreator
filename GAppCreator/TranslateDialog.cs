using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class TranslateDialog : Form
    {
        protected enum BuildTask
        {
            SelectIcon,
        };
        private class ResultInfo
        {
            public BuildTask Action;
            public string Value;
            public int i1;
            public string s1;
        };
        private class TranslateItem
        {
            public StringValues sv;
            public Language Lang;
            public DataGridViewCell Cell;
        };
        List<TranslateItem> items = new List<TranslateItem>();
        ResultInfo tmp = new ResultInfo();
        Random rnd = new Random();
        bool stop = false;
        string error = "";
        private bool sync;
        private Language FromLanguage;
        public TranslateDialog(Project prj,Language l, DataGridView dg)
        {
            InitializeComponent();
            lstTranslate.Columns[1].Text = "From " + l.ToString();
            foreach (DataGridViewCell cell in dg.SelectedCells)
            {
                TranslateItem ti = new TranslateItem();
                ti.Cell = cell;
                ti.Lang = (Language)Enum.Parse(typeof(Language),dg.Columns[cell.ColumnIndex].HeaderText);
                ti.sv = prj.Strings[cell.RowIndex];
                items.Add(ti);
                // adaug si in listview
                ListViewItem lvi = new ListViewItem(ti.sv.GetVariableNameWithArray());
                lvi.SubItems.Add(ti.sv.Get(l));
                lvi.SubItems.Add(ti.Lang.ToString());
                lvi.SubItems.Add("");
                lvi.ForeColor = Color.Gray;
                lstTranslate.Items.Add(lvi);
            }
            prog.Minimum = 0;
            prog.Maximum = items.Count-1;
            prog.Value = 0;
            FromLanguage = l;
            
        }
        private string GetTranslate(WebClient Client,int index)
        {
            TranslateItem ti = items[index];
            error = "";
            string l_from = Project.LanguageTo2DigitsSymbol(FromLanguage);
            if (l_from.Length==0)
            {
                error = "Unable to translate " + FromLanguage.ToString() + " to two digit code !";
                return null;
            }
            string l_to = Project.LanguageTo2DigitsSymbol(ti.Lang);
            if (l_to.Length == 0)
            {
                error = "Unable to translate " + ti.Lang.ToString() + " to two digit code !";
                return null;
            }
            string phrase = ti.sv.Get(FromLanguage).Trim();
            if ((phrase == null) || (phrase.Length==0))
            {
                error = "Skipping because value for "+FromLanguage.ToString()+" is not defined !";
                return null;
            }
            phrase = phrase.Replace(" ", "%20");            
            string url = string.Format("https://translate.google.com/translate_a/single?client=t&sl={0}&tl={1}&hl=ro&dt=bd&dt=ex&dt=ld&dt=md&dt=qc&dt=rw&dt=rm&dt=ss&dt=t&dt=at&ie=UTF-8&oe=UTF-8&prev=bh&ssel=0&tsel=0&tk=517707|938966&q={2}", l_from, l_to, phrase);
            try
            {
                byte[] b = Client.DownloadData(url);
                if ((b==null) || (b.Length==0))
                {
                    error = "Invalid response from server !";
                    return null;
                }
                string result = System.Text.Encoding.UTF8.GetString(b).Trim();
                if ((result == null) || (result.Length==0))
                {
                    error = "Unable to convert binary result from server to string !";
                    return null;
                }
                if (result.StartsWith("[[[\"")==false)
                {
                    error = "Incorrect response format: "+result;
                    return null;
                }
                result = result.Substring(4).Trim();
                if (result.Contains("\"")==false)
                {
                    error = "Incorrect response format: "+result;
                    return null;
                }
                return result.Substring(0, result.IndexOf("\""));
                
            }
            catch (Exception e)
            {
                error = e.ToString();
                return null;
            }
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            WebClient Client = new WebClient();
            Client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36 OPR/26.0.1656.60");
            for (int tr=0;(tr<items.Count) && (stop==false);tr++)
            {
                SendCommand(BuildTask.SelectIcon, "Run", tr,"Translating ...");
                string res = GetTranslate(Client, tr);
                if (res==null)
                {
                    if (error.StartsWith("Skip"))
                        SendCommand(BuildTask.SelectIcon, "Skipped", tr,error);
                    else
                        SendCommand(BuildTask.SelectIcon, "Error", tr, error);
                }
                else
                {
                    SendCommand(BuildTask.SelectIcon, "Ok", tr, res);
                }
                
                System.Threading.Thread.Sleep(rnd.Next(500, 2000));
            }

        }
        private void SendCommand(BuildTask action,string sValue,int iValue,string s1)
        {
            tmp.Action = action;
            tmp.Value = sValue;
            tmp.i1 = iValue;
            tmp.s1 = s1;
            sync = true;
            Worker.ReportProgress(0, tmp);
            while (sync) { };
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                switch (tmp.Action)
                {
                    case BuildTask.SelectIcon:
                        lstTranslate.Items[tmp.i1].ImageKey = tmp.Value;
                        lstTranslate.Items[tmp.i1].SubItems[3].Text = tmp.s1;
                        lstTranslate.Items[tmp.i1].EnsureVisible();
                        switch (tmp.Value)
                        {
                            case "Run": lstTranslate.Items[tmp.i1].ForeColor = Color.Black; prog.Value = tmp.i1; break;
                            case "Error": lstTranslate.Items[tmp.i1].ForeColor = Color.Red; break;
                            case "Ok": 
                                lstTranslate.Items[tmp.i1].ForeColor = Color.Green;
                                items[tmp.i1].Cell.Value = tmp.s1;
                                break;
                            case "Skipped": lstTranslate.Items[tmp.i1].ForeColor = Color.Orange; break;
                        }
                        break;
                }
            }
            sync = false;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStartStop.Enabled = false;
            btnClose.Enabled = true;
        }

        private void OnBegin(object sender, EventArgs e)
        {
            if (btnStartStop.Text == "Start")
            {
                stop = false;
                Worker.RunWorkerAsync();
                btnStartStop.Text = "Stop";
                btnClose.Enabled = false;
                return;
            }
            if (btnStartStop.Text == "Stop")
            {
                stop = true;
                btnStartStop.Enabled = false;
                return;
            }
        }
    }
}
