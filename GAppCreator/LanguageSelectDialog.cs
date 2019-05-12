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
    public partial class LanguageSelectDialog : Form
    {
        Dictionary<Language, bool> dLang;
        public LanguageSelectDialog(Dictionary<Language, bool> lng)
        {
            InitializeComponent();
            dLang = lng;
            if (dLang == null)
                dLang = new Dictionary<Language, bool>();

            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                ListViewItem lvi = new ListViewItem(l.ToString());
                lvi.Tag = l;
                if (dLang.ContainsKey(l))
                    lvi.Checked = dLang[l];
                else
                    lvi.Checked = true;
                lstLanguages.Items.Add(lvi);
            }
            UpdateSelectionInfo();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        public void UpdateSelectionInfo()
        {
            int count = 0;
            foreach (ListViewItem lvi in lstLanguages.Items)
                if (lvi.Checked)
                    count++;
            if (count == 0)
                lbInfo.Text = "No language";
            else
                lbInfo.Text = count.ToString() + " selected";
        }
        private void OnSelectAll(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLanguages.Items)
                lvi.Checked = true;
            UpdateSelectionInfo();
        }

        private void OnClearAll(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLanguages.Items)
                lvi.Checked = false;
            UpdateSelectionInfo();
        }


        public Dictionary<Language, bool> GetLanguageDict()
        {
            return dLang;
        }
        public List<Language> GetSelectedLanguages()
        {
            List<Language> l = new List<Language>();
            foreach (Language ll in dLang.Keys)
                if (dLang[ll])
                    l.Add(ll);
            return l;
        }

        private void OnOK(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLanguages.Items)
                dLang[(Language)lvi.Tag] = lvi.Checked;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnRevertSelection(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstLanguages.Items)
                lvi.Checked = !lvi.Checked;
            UpdateSelectionInfo();
        }

        private void lstLanguages_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateSelectionInfo();
        }
    }
}
