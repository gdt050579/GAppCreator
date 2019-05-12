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
    public partial class FontCharacterSelectorFromStrings : Form
    {
        
        Project prj;
        Dictionary<Language, bool> SelectedLanguages = new Dictionary<Language, bool>();
        public Dictionary<int, bool> SelectedCodes = new Dictionary<int, bool>();
        public FontGlyphCreator.SelectAction Action = FontGlyphCreator.SelectAction.None;
        public string CharactersToBeAdded;
        public bool FilterSelectedCharacters = true;
        public FontCharacterSelectorFromStrings(Project _prj,bool forAddingCharacters)
        {
            InitializeComponent();
            prj = _prj;
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                SelectedLanguages[l] = false;
                foreach (StringValues sv in prj.Strings)
                {
                    if (sv.Get(l).Length > 0)
                    {
                        SelectedLanguages[l] = true;
                        break;
                    }
                }
            }
            if (forAddingCharacters)
            {
                comboAction.Items.Add("Add + Select New");
                comboAction.Items.Add("Add + Select All");
                comboAction.Items.Add("Add");
            }
            else
            {
                comboAction.Items.Add("Select all characters that are NOT in the selected strings");
                comboAction.Items.Add("Keep only the characters that are in the selected strings");
            }
            comboAction.SelectedIndex = 0;
            UpdateDataGridFromStringValues();
            OnSelectionChanged(null, null);
        }
        private void UpdateDataGridFromStringValues()
        {
            DataGridViewTextBoxColumn dgc;

            dgStrings.Rows.Clear();
            dgStrings.Columns.Clear();

            // strings
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                dgc = new DataGridViewTextBoxColumn();
                dgc.HeaderText = l.ToString();
                dgc.Tag = l;
                if (SelectedLanguages[l] == false)
                    dgc.Visible = false;
                dgStrings.Columns.Add(dgc);
            }

            if ((prj != null) && (prj.Strings != null))
            {
                foreach (StringValues sv in prj.Strings)
                {
                    int index = dgStrings.Rows.Add();
                    dgStrings.Rows[index].HeaderCell.Value = sv.GetVariableNameWithArray();  
                    
                    int count = 0;
                    foreach (Language l in Enum.GetValues(typeof(Language)))
                    {
                        dgStrings.Rows[index].Cells[count].Value = sv.Get(l);
                        count++;
                    }
                }
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (dgStrings.SelectedCells.Count == 0)
            {
                lbInfo.Text = "No cell selected !";
            }
            else
            {
                lbInfo.Text = dgStrings.SelectedCells.Count.ToString() + " strings selected !";
            }
            if ((dgStrings.SelectedCells.Count == 1) && (dgStrings.SelectedCells[0].Value!=null))
            {             
                lbText.Text = dgStrings.SelectedCells[0].Value.ToString();
            } else {
                lbText.Text = "";

            }
        }

        private void OnSelectAll(object sender, EventArgs e)
        {
            dgStrings.SelectAll();
        }

        private void OnClearSelection(object sender, EventArgs e)
        {
            dgStrings.ClearSelection();
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (dgStrings.SelectedCells.Count == 0)
            {
                MessageBox.Show("You have to select at least one string !");
                return;
            }
            foreach (DataGridViewCell cell in dgStrings.SelectedCells)
            {
                if (cell.Value!=null)
                {
                    string ss = cell.Value.ToString();
                    foreach (char ch in ss)
                    {
                        if (ch>32)
                            SelectedCodes[(int)ch] = true;
                    }
                }                
            }
            CharactersToBeAdded = "";
            foreach (int key in SelectedCodes.Keys)
                CharactersToBeAdded += (char)key;

            Action = FontGlyphCreator.SelectAction.None;
            FilterSelectedCharacters = true;
            if (comboAction.SelectedIndex == 0)
            {
                Action = FontGlyphCreator.SelectAction.SelectOnlyNew;
                FilterSelectedCharacters = false;
            }
            if (comboAction.SelectedIndex == 1)
                Action = FontGlyphCreator.SelectAction.SelectAll;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
