namespace GAppCreator
{
    partial class GoToDefinitionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoToDefinitionDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txSearch = new System.Windows.Forms.ToolStripTextBox();
            this.lbInfo = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.cbScenes = new System.Windows.Forms.ToolStripMenuItem();
            this.cbClasses = new System.Windows.Forms.ToolStripMenuItem();
            this.cbMethods = new System.Windows.Forms.ToolStripMenuItem();
            this.cbParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.cbLocalVariables = new System.Windows.Forms.ToolStripMenuItem();
            this.cbMembers = new System.Windows.Forms.ToolStripMenuItem();
            this.cbConstructors = new System.Windows.Forms.ToolStripMenuItem();
            this.cbDestructor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cbSearchInDefinition = new System.Windows.Forms.ToolStripMenuItem();
            this.cbSearchInCurrentFileOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.lstItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbSearchInPrototype = new System.Windows.Forms.ToolStripMenuItem();
            this.cbShowPerfectMatches = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 303);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(734, 58);
            this.panel1.TabIndex = 2;
            this.panel1.TabStop = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(640, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(559, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.txSearch,
            this.lbInfo,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(734, 38);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.TabStop = true;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(63, 35);
            this.toolStripLabel1.Text = "&Search for ";
            // 
            // txSearch
            // 
            this.txSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txSearch.Name = "txSearch";
            this.txSearch.Size = new System.Drawing.Size(300, 38);
            this.txSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txSearch_KeyDown);
            this.txSearch.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = false;
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(280, 35);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbScenes,
            this.cbClasses,
            this.cbMethods,
            this.cbParameters,
            this.cbLocalVariables,
            this.cbMembers,
            this.cbConstructors,
            this.cbDestructor,
            this.toolStripMenuItem1,
            this.cbSearchInDefinition,
            this.cbSearchInCurrentFileOnly,
            this.cbSearchInPrototype,
            this.cbShowPerfectMatches});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(46, 35);
            this.toolStripButton1.Text = "Filter";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbScenes
            // 
            this.cbScenes.Checked = true;
            this.cbScenes.CheckOnClick = true;
            this.cbScenes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbScenes.Name = "cbScenes";
            this.cbScenes.Size = new System.Drawing.Size(217, 22);
            this.cbScenes.Text = "Scenes";
            this.cbScenes.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbClasses
            // 
            this.cbClasses.Checked = true;
            this.cbClasses.CheckOnClick = true;
            this.cbClasses.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbClasses.Name = "cbClasses";
            this.cbClasses.Size = new System.Drawing.Size(217, 22);
            this.cbClasses.Text = "Classes";
            this.cbClasses.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbMethods
            // 
            this.cbMethods.Checked = true;
            this.cbMethods.CheckOnClick = true;
            this.cbMethods.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMethods.Name = "cbMethods";
            this.cbMethods.Size = new System.Drawing.Size(217, 22);
            this.cbMethods.Text = "Methods";
            this.cbMethods.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbParameters
            // 
            this.cbParameters.Checked = true;
            this.cbParameters.CheckOnClick = true;
            this.cbParameters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbParameters.Name = "cbParameters";
            this.cbParameters.Size = new System.Drawing.Size(217, 22);
            this.cbParameters.Text = "Parameters";
            this.cbParameters.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbLocalVariables
            // 
            this.cbLocalVariables.Checked = true;
            this.cbLocalVariables.CheckOnClick = true;
            this.cbLocalVariables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLocalVariables.Name = "cbLocalVariables";
            this.cbLocalVariables.Size = new System.Drawing.Size(217, 22);
            this.cbLocalVariables.Text = "Local variables";
            this.cbLocalVariables.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbMembers
            // 
            this.cbMembers.Checked = true;
            this.cbMembers.CheckOnClick = true;
            this.cbMembers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMembers.Name = "cbMembers";
            this.cbMembers.Size = new System.Drawing.Size(217, 22);
            this.cbMembers.Text = "Members";
            this.cbMembers.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbConstructors
            // 
            this.cbConstructors.Checked = true;
            this.cbConstructors.CheckOnClick = true;
            this.cbConstructors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbConstructors.Name = "cbConstructors";
            this.cbConstructors.Size = new System.Drawing.Size(217, 22);
            this.cbConstructors.Text = "Constructors";
            this.cbConstructors.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbDestructor
            // 
            this.cbDestructor.Checked = true;
            this.cbDestructor.CheckOnClick = true;
            this.cbDestructor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDestructor.Name = "cbDestructor";
            this.cbDestructor.Size = new System.Drawing.Size(217, 22);
            this.cbDestructor.Text = "Destructors";
            this.cbDestructor.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(214, 6);
            // 
            // cbSearchInDefinition
            // 
            this.cbSearchInDefinition.CheckOnClick = true;
            this.cbSearchInDefinition.Name = "cbSearchInDefinition";
            this.cbSearchInDefinition.Size = new System.Drawing.Size(217, 22);
            this.cbSearchInDefinition.Text = "Search in definition";
            this.cbSearchInDefinition.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbSearchInCurrentFileOnly
            // 
            this.cbSearchInCurrentFileOnly.CheckOnClick = true;
            this.cbSearchInCurrentFileOnly.Name = "cbSearchInCurrentFileOnly";
            this.cbSearchInCurrentFileOnly.Size = new System.Drawing.Size(217, 22);
            this.cbSearchInCurrentFileOnly.Text = "Search in current file only";
            this.cbSearchInCurrentFileOnly.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // lstItems
            // 
            this.lstItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstItems.FullRowSelect = true;
            this.lstItems.GridLines = true;
            this.lstItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstItems.HideSelection = false;
            this.lstItems.Location = new System.Drawing.Point(0, 38);
            this.lstItems.MultiSelect = false;
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(734, 265);
            this.lstItems.TabIndex = 1;
            this.lstItems.UseCompatibleStateImageBehavior = false;
            this.lstItems.View = System.Windows.Forms.View.Details;
            this.lstItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstItems_KeyDown);
            this.lstItems.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstItems_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Definition";
            this.columnHeader3.Width = 400;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Prototype";
            this.columnHeader4.Width = 400;
            // 
            // cbSearchInPrototype
            // 
            this.cbSearchInPrototype.CheckOnClick = true;
            this.cbSearchInPrototype.Name = "cbSearchInPrototype";
            this.cbSearchInPrototype.Size = new System.Drawing.Size(217, 22);
            this.cbSearchInPrototype.Text = "Search in prototype";
            this.cbSearchInPrototype.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // cbShowPerfectMatches
            // 
            this.cbShowPerfectMatches.CheckOnClick = true;
            this.cbShowPerfectMatches.Name = "cbShowPerfectMatches";
            this.cbShowPerfectMatches.Size = new System.Drawing.Size(217, 22);
            this.cbShowPerfectMatches.Text = "Show only perfect matches";
            this.cbShowPerfectMatches.Click += new System.EventHandler(this.OnChangeFilter);
            // 
            // GoToDefinitionDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(734, 361);
            this.ControlBox = false;
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GoToDefinitionDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GoTo Definition";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GoToDefinition_KeyDown);
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txSearch;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem cbClasses;
        private System.Windows.Forms.ToolStripMenuItem cbMethods;
        private System.Windows.Forms.ToolStripMenuItem cbMembers;
        private System.Windows.Forms.ToolStripMenuItem cbScenes;
        private System.Windows.Forms.ToolStripMenuItem cbConstructors;
        private System.Windows.Forms.ToolStripMenuItem cbDestructor;
        private System.Windows.Forms.ListView lstItems;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripLabel lbInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cbSearchInDefinition;
        private System.Windows.Forms.ToolStripMenuItem cbLocalVariables;
        private System.Windows.Forms.ToolStripMenuItem cbSearchInCurrentFileOnly;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ToolStripMenuItem cbParameters;
        private System.Windows.Forms.ToolStripMenuItem cbSearchInPrototype;
        private System.Windows.Forms.ToolStripMenuItem cbShowPerfectMatches;
    }
}