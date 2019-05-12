namespace GAppCreator
{
    partial class FontGlyphCreator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontGlyphCreator));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.charactersByCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.charactersFromTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.comboLanguage = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbHideExisting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cbShowDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowNonDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.cbShowUpper = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowLower = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowDigits = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowPunctiation = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowOtherTypes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cbShowCustomChars = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.lbSelected = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.selectFromExistingTemplatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useABlankImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.createNewTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbTemplate = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lstChars = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.charactersUsedInApplicationStringsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripButton3,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.comboLanguage,
            this.toolStripDropDownButton2,
            this.toolStripSeparator3,
            this.lbSelected,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.lbTemplate,
            this.toolStripSeparator4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(748, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.charactersByCodeToolStripMenuItem,
            this.charactersFromTextToolStripMenuItem,
            this.charactersUsedInApplicationStringsToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(42, 35);
            this.toolStripDropDownButton1.Text = "Add";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // charactersByCodeToolStripMenuItem
            // 
            this.charactersByCodeToolStripMenuItem.Name = "charactersByCodeToolStripMenuItem";
            this.charactersByCodeToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.charactersByCodeToolStripMenuItem.Text = "Characters by code";
            this.charactersByCodeToolStripMenuItem.Click += new System.EventHandler(this.OnAddCharactersByCode);
            // 
            // charactersFromTextToolStripMenuItem
            // 
            this.charactersFromTextToolStripMenuItem.Name = "charactersFromTextToolStripMenuItem";
            this.charactersFromTextToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.charactersFromTextToolStripMenuItem.Text = "Characters from text";
            this.charactersFromTextToolStripMenuItem.Click += new System.EventHandler(this.OnAddCharactersFromText);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(55, 35);
            this.toolStripButton3.Text = "Clear All";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton3.Click += new System.EventHandler(this.OnClearAll);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(59, 35);
            this.toolStripLabel1.Text = "Language";
            // 
            // comboLanguage
            // 
            this.comboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboLanguage.Name = "comboLanguage";
            this.comboLanguage.Size = new System.Drawing.Size(121, 38);
            this.comboLanguage.SelectedIndexChanged += new System.EventHandler(this.OnChangeFilterLanguage);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbHideExisting,
            this.toolStripMenuItem2,
            this.cbShowDefault,
            this.cbShowNonDefault,
            this.toolStripMenuItem3,
            this.cbShowUpper,
            this.cbShowLower,
            this.cbShowDigits,
            this.cbShowPunctiation,
            this.cbShowOtherTypes,
            this.toolStripMenuItem4,
            this.cbShowCustomChars});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(46, 35);
            this.toolStripDropDownButton2.Text = "Filter";
            this.toolStripDropDownButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbHideExisting
            // 
            this.cbHideExisting.Checked = true;
            this.cbHideExisting.CheckOnClick = true;
            this.cbHideExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHideExisting.Name = "cbHideExisting";
            this.cbHideExisting.Size = new System.Drawing.Size(394, 22);
            this.cbHideExisting.Text = "Hide existing characters";
            this.cbHideExisting.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(391, 6);
            // 
            // cbShowDefault
            // 
            this.cbShowDefault.Checked = true;
            this.cbShowDefault.CheckOnClick = true;
            this.cbShowDefault.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowDefault.Name = "cbShowDefault";
            this.cbShowDefault.Size = new System.Drawing.Size(394, 22);
            this.cbShowDefault.Text = "Show Default characters for all languages";
            this.cbShowDefault.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // cbShowNonDefault
            // 
            this.cbShowNonDefault.Checked = true;
            this.cbShowNonDefault.CheckOnClick = true;
            this.cbShowNonDefault.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowNonDefault.Name = "cbShowNonDefault";
            this.cbShowNonDefault.Size = new System.Drawing.Size(394, 22);
            this.cbShowNonDefault.Text = "Show non-Default characters";
            this.cbShowNonDefault.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(391, 6);
            // 
            // cbShowUpper
            // 
            this.cbShowUpper.Checked = true;
            this.cbShowUpper.CheckOnClick = true;
            this.cbShowUpper.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowUpper.Name = "cbShowUpper";
            this.cbShowUpper.Size = new System.Drawing.Size(394, 22);
            this.cbShowUpper.Text = "Show Upper-case characters";
            this.cbShowUpper.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // cbShowLower
            // 
            this.cbShowLower.Checked = true;
            this.cbShowLower.CheckOnClick = true;
            this.cbShowLower.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowLower.Name = "cbShowLower";
            this.cbShowLower.Size = new System.Drawing.Size(394, 22);
            this.cbShowLower.Text = "Show Lower-case characters";
            this.cbShowLower.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // cbShowDigits
            // 
            this.cbShowDigits.Checked = true;
            this.cbShowDigits.CheckOnClick = true;
            this.cbShowDigits.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowDigits.Name = "cbShowDigits";
            this.cbShowDigits.Size = new System.Drawing.Size(394, 22);
            this.cbShowDigits.Text = "Show digits";
            this.cbShowDigits.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // cbShowPunctiation
            // 
            this.cbShowPunctiation.Checked = true;
            this.cbShowPunctiation.CheckOnClick = true;
            this.cbShowPunctiation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPunctiation.Name = "cbShowPunctiation";
            this.cbShowPunctiation.Size = new System.Drawing.Size(394, 22);
            this.cbShowPunctiation.Text = "Show punctuation marks";
            this.cbShowPunctiation.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // cbShowOtherTypes
            // 
            this.cbShowOtherTypes.Checked = true;
            this.cbShowOtherTypes.CheckOnClick = true;
            this.cbShowOtherTypes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowOtherTypes.Name = "cbShowOtherTypes";
            this.cbShowOtherTypes.Size = new System.Drawing.Size(394, 22);
            this.cbShowOtherTypes.Text = "Show other type characters";
            this.cbShowOtherTypes.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(391, 6);
            // 
            // cbShowCustomChars
            // 
            this.cbShowCustomChars.Checked = true;
            this.cbShowCustomChars.CheckOnClick = true;
            this.cbShowCustomChars.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowCustomChars.Name = "cbShowCustomChars";
            this.cbShowCustomChars.Size = new System.Drawing.Size(394, 22);
            this.cbShowCustomChars.Text = "Show custom characters (not from the existing character set)";
            this.cbShowCustomChars.Click += new System.EventHandler(this.OnChangeFilterOption);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // lbSelected
            // 
            this.lbSelected.AutoSize = false;
            this.lbSelected.Name = "lbSelected";
            this.lbSelected.Size = new System.Drawing.Size(130, 35);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectFromExistingTemplatesToolStripMenuItem,
            this.useABlankImageToolStripMenuItem,
            this.toolStripMenuItem1,
            this.createNewTemplateToolStripMenuItem});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(70, 35);
            this.toolStripButton1.Text = "Template";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // selectFromExistingTemplatesToolStripMenuItem
            // 
            this.selectFromExistingTemplatesToolStripMenuItem.Name = "selectFromExistingTemplatesToolStripMenuItem";
            this.selectFromExistingTemplatesToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.selectFromExistingTemplatesToolStripMenuItem.Text = "Select from existing templates";
            this.selectFromExistingTemplatesToolStripMenuItem.Click += new System.EventHandler(this.OnUseExistingTemplate);
            // 
            // useABlankImageToolStripMenuItem
            // 
            this.useABlankImageToolStripMenuItem.Name = "useABlankImageToolStripMenuItem";
            this.useABlankImageToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.useABlankImageToolStripMenuItem.Text = "Use a blank image";
            this.useABlankImageToolStripMenuItem.Click += new System.EventHandler(this.OnUseBlank);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(229, 6);
            // 
            // createNewTemplateToolStripMenuItem
            // 
            this.createNewTemplateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createNewTemplateToolStripMenuItem.Image")));
            this.createNewTemplateToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.createNewTemplateToolStripMenuItem.Name = "createNewTemplateToolStripMenuItem";
            this.createNewTemplateToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.createNewTemplateToolStripMenuItem.Text = "Create new Template";
            this.createNewTemplateToolStripMenuItem.Click += new System.EventHandler(this.OnCreateNewTemplate);
            // 
            // lbTemplate
            // 
            this.lbTemplate.AutoSize = false;
            this.lbTemplate.Name = "lbTemplate";
            this.lbTemplate.Size = new System.Drawing.Size(150, 35);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 38);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(661, 457);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 30);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(580, 457);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnOK);
            // 
            // lstChars
            // 
            this.lstChars.CheckBoxes = true;
            this.lstChars.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader6,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lstChars.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstChars.FullRowSelect = true;
            this.lstChars.GridLines = true;
            this.lstChars.Location = new System.Drawing.Point(0, 38);
            this.lstChars.Name = "lstChars";
            this.lstChars.Size = new System.Drawing.Size(748, 405);
            this.lstChars.TabIndex = 4;
            this.lstChars.UseCompatibleStateImageBehavior = false;
            this.lstChars.View = System.Windows.Forms.View.Details;
            this.lstChars.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.OnCheckItem);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Character";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Dec";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 50;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Hex";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 55;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Languages";
            this.columnHeader3.Width = 380;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Default";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 50;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            this.columnHeader5.Width = 120;
            // 
            // charactersUsedInApplicationStringsToolStripMenuItem
            // 
            this.charactersUsedInApplicationStringsToolStripMenuItem.Name = "charactersUsedInApplicationStringsToolStripMenuItem";
            this.charactersUsedInApplicationStringsToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.charactersUsedInApplicationStringsToolStripMenuItem.Text = "Characters used in application strings";
            this.charactersUsedInApplicationStringsToolStripMenuItem.Click += new System.EventHandler(this.OnAddCharactersUsedInAppStrings);
            // 
            // FontGlyphCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 499);
            this.ControlBox = false;
            this.Controls.Add(this.lstChars);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FontGlyphCreator";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Glyph Creator";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lbTemplate;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem selectFromExistingTemplatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useABlankImageToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView lstChars;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox comboLanguage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem charactersByCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem charactersFromTextToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem createNewTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem cbHideExisting;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem cbShowDefault;
        private System.Windows.Forms.ToolStripMenuItem cbShowNonDefault;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem cbShowUpper;
        private System.Windows.Forms.ToolStripMenuItem cbShowLower;
        private System.Windows.Forms.ToolStripMenuItem cbShowDigits;
        private System.Windows.Forms.ToolStripMenuItem cbShowPunctiation;
        private System.Windows.Forms.ToolStripMenuItem cbShowOtherTypes;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cbShowCustomChars;
        private System.Windows.Forms.ToolStripLabel lbSelected;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ToolStripMenuItem charactersUsedInApplicationStringsToolStripMenuItem;
    }
}