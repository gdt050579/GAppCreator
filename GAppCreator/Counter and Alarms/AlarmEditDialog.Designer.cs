namespace GAppCreator
{
    partial class AlarmEditDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboAlertType = new System.Windows.Forms.ComboBox();
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txName = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlEveryXDays = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.nmEveryXDays = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.pnlEveryXHours = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.nmEveryXHours = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.pnlEveryXMinutes = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.nmEveryXMinutes = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.pnlEveryDayAtASpecificTime = new System.Windows.Forms.Panel();
            this.dtHourMinute = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.pnlDayFromWeek = new System.Windows.Forms.Panel();
            this.comboDayOfWeek = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlDayFromMonth = new System.Windows.Forms.Panel();
            this.nmDayFromMonth = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlMonthDayTime = new System.Windows.Forms.Panel();
            this.dtMonthDayHourMinute = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlExactDateTime = new System.Windows.Forms.Panel();
            this.dtExactDateTime = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.comboDuration = new System.Windows.Forms.ComboBox();
            this.cbPushNotifications = new System.Windows.Forms.CheckBox();
            this.txPushNotificationResource = new System.Windows.Forms.TextBox();
            this.btnSelectTextResources = new System.Windows.Forms.Button();
            this.lstBuilds = new System.Windows.Forms.ListView();
            this.label16 = new System.Windows.Forms.Label();
            this.cbOneTimeOnly = new System.Windows.Forms.CheckBox();
            this.process1 = new System.Diagnostics.Process();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlEveryXDays.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmEveryXDays)).BeginInit();
            this.pnlEveryXHours.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmEveryXHours)).BeginInit();
            this.pnlEveryXMinutes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmEveryXMinutes)).BeginInit();
            this.pnlEveryDayAtASpecificTime.SuspendLayout();
            this.pnlDayFromWeek.SuspendLayout();
            this.pnlDayFromMonth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmDayFromMonth)).BeginInit();
            this.pnlMonthDayTime.SuspendLayout();
            this.pnlExactDateTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 357);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(684, 58);
            this.panel1.TabIndex = 8;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(590, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(507, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OnOK);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Alarm Type";
            // 
            // comboAlertType
            // 
            this.comboAlertType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboAlertType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAlertType.FormattingEnabled = true;
            this.comboAlertType.Location = new System.Drawing.Point(74, 42);
            this.comboAlertType.Name = "comboAlertType";
            this.comboAlertType.Size = new System.Drawing.Size(591, 21);
            this.comboAlertType.TabIndex = 10;
            this.comboAlertType.SelectedIndexChanged += new System.EventHandler(this.comboAlertType_SelectedIndexChanged);
            // 
            // cbEnabled
            // 
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Location = new System.Drawing.Point(16, 146);
            this.cbEnabled.Name = "cbEnabled";
            this.cbEnabled.Size = new System.Drawing.Size(152, 17);
            this.cbEnabled.TabIndex = 12;
            this.cbEnabled.Text = "Alarm is enabled by default";
            this.cbEnabled.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Name";
            // 
            // txName
            // 
            this.txName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txName.Location = new System.Drawing.Point(74, 10);
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(591, 20);
            this.txName.TabIndex = 15;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.pnlEveryXDays);
            this.panel2.Controls.Add(this.pnlEveryXHours);
            this.panel2.Controls.Add(this.pnlEveryXMinutes);
            this.panel2.Controls.Add(this.pnlEveryDayAtASpecificTime);
            this.panel2.Controls.Add(this.pnlDayFromWeek);
            this.panel2.Controls.Add(this.pnlDayFromMonth);
            this.panel2.Controls.Add(this.pnlMonthDayTime);
            this.panel2.Controls.Add(this.pnlExactDateTime);
            this.panel2.Location = new System.Drawing.Point(16, 309);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(649, 42);
            this.panel2.TabIndex = 16;
            // 
            // pnlEveryXDays
            // 
            this.pnlEveryXDays.Controls.Add(this.label13);
            this.pnlEveryXDays.Controls.Add(this.nmEveryXDays);
            this.pnlEveryXDays.Controls.Add(this.label14);
            this.pnlEveryXDays.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEveryXDays.Location = new System.Drawing.Point(0, 280);
            this.pnlEveryXDays.Name = "pnlEveryXDays";
            this.pnlEveryXDays.Size = new System.Drawing.Size(649, 40);
            this.pnlEveryXDays.TabIndex = 7;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(620, 12);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 13);
            this.label13.TabIndex = 18;
            this.label13.Text = "days";
            // 
            // nmEveryXDays
            // 
            this.nmEveryXDays.Location = new System.Drawing.Point(58, 10);
            this.nmEveryXDays.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nmEveryXDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmEveryXDays.Name = "nmEveryXDays";
            this.nmEveryXDays.Size = new System.Drawing.Size(542, 20);
            this.nmEveryXDays.TabIndex = 18;
            this.nmEveryXDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(-3, 13);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 13);
            this.label14.TabIndex = 17;
            this.label14.Text = "Every";
            // 
            // pnlEveryXHours
            // 
            this.pnlEveryXHours.Controls.Add(this.label11);
            this.pnlEveryXHours.Controls.Add(this.nmEveryXHours);
            this.pnlEveryXHours.Controls.Add(this.label12);
            this.pnlEveryXHours.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEveryXHours.Location = new System.Drawing.Point(0, 240);
            this.pnlEveryXHours.Name = "pnlEveryXHours";
            this.pnlEveryXHours.Size = new System.Drawing.Size(649, 40);
            this.pnlEveryXHours.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(613, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(33, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = "hours";
            // 
            // nmEveryXHours
            // 
            this.nmEveryXHours.Location = new System.Drawing.Point(58, 10);
            this.nmEveryXHours.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this.nmEveryXHours.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmEveryXHours.Name = "nmEveryXHours";
            this.nmEveryXHours.Size = new System.Drawing.Size(542, 20);
            this.nmEveryXHours.TabIndex = 18;
            this.nmEveryXHours.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(-3, 13);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(34, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "Every";
            // 
            // pnlEveryXMinutes
            // 
            this.pnlEveryXMinutes.Controls.Add(this.label10);
            this.pnlEveryXMinutes.Controls.Add(this.nmEveryXMinutes);
            this.pnlEveryXMinutes.Controls.Add(this.label9);
            this.pnlEveryXMinutes.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEveryXMinutes.Location = new System.Drawing.Point(0, 200);
            this.pnlEveryXMinutes.Name = "pnlEveryXMinutes";
            this.pnlEveryXMinutes.Size = new System.Drawing.Size(649, 40);
            this.pnlEveryXMinutes.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(606, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "minutes";
            // 
            // nmEveryXMinutes
            // 
            this.nmEveryXMinutes.Location = new System.Drawing.Point(58, 10);
            this.nmEveryXMinutes.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nmEveryXMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmEveryXMinutes.Name = "nmEveryXMinutes";
            this.nmEveryXMinutes.Size = new System.Drawing.Size(542, 20);
            this.nmEveryXMinutes.TabIndex = 18;
            this.nmEveryXMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(-3, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Every";
            // 
            // pnlEveryDayAtASpecificTime
            // 
            this.pnlEveryDayAtASpecificTime.Controls.Add(this.dtHourMinute);
            this.pnlEveryDayAtASpecificTime.Controls.Add(this.label8);
            this.pnlEveryDayAtASpecificTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEveryDayAtASpecificTime.Location = new System.Drawing.Point(0, 160);
            this.pnlEveryDayAtASpecificTime.Name = "pnlEveryDayAtASpecificTime";
            this.pnlEveryDayAtASpecificTime.Size = new System.Drawing.Size(649, 40);
            this.pnlEveryDayAtASpecificTime.TabIndex = 4;
            // 
            // dtHourMinute
            // 
            this.dtHourMinute.CustomFormat = "HH:mm";
            this.dtHourMinute.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtHourMinute.Location = new System.Drawing.Point(58, 9);
            this.dtHourMinute.Name = "dtHourMinute";
            this.dtHourMinute.ShowUpDown = true;
            this.dtHourMinute.Size = new System.Drawing.Size(591, 20);
            this.dtHourMinute.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(-3, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Time";
            // 
            // pnlDayFromWeek
            // 
            this.pnlDayFromWeek.Controls.Add(this.comboDayOfWeek);
            this.pnlDayFromWeek.Controls.Add(this.label7);
            this.pnlDayFromWeek.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDayFromWeek.Location = new System.Drawing.Point(0, 120);
            this.pnlDayFromWeek.Name = "pnlDayFromWeek";
            this.pnlDayFromWeek.Size = new System.Drawing.Size(649, 40);
            this.pnlDayFromWeek.TabIndex = 3;
            // 
            // comboDayOfWeek
            // 
            this.comboDayOfWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDayOfWeek.FormattingEnabled = true;
            this.comboDayOfWeek.Items.AddRange(new object[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"});
            this.comboDayOfWeek.Location = new System.Drawing.Point(57, 10);
            this.comboDayOfWeek.Name = "comboDayOfWeek";
            this.comboDayOfWeek.Size = new System.Drawing.Size(591, 21);
            this.comboDayOfWeek.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(-3, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Week Day";
            // 
            // pnlDayFromMonth
            // 
            this.pnlDayFromMonth.Controls.Add(this.nmDayFromMonth);
            this.pnlDayFromMonth.Controls.Add(this.label6);
            this.pnlDayFromMonth.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDayFromMonth.Location = new System.Drawing.Point(0, 80);
            this.pnlDayFromMonth.Name = "pnlDayFromMonth";
            this.pnlDayFromMonth.Size = new System.Drawing.Size(649, 40);
            this.pnlDayFromMonth.TabIndex = 2;
            // 
            // nmDayFromMonth
            // 
            this.nmDayFromMonth.Location = new System.Drawing.Point(58, 10);
            this.nmDayFromMonth.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nmDayFromMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmDayFromMonth.Name = "nmDayFromMonth";
            this.nmDayFromMonth.Size = new System.Drawing.Size(590, 20);
            this.nmDayFromMonth.TabIndex = 18;
            this.nmDayFromMonth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(-3, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Month Day";
            // 
            // pnlMonthDayTime
            // 
            this.pnlMonthDayTime.Controls.Add(this.dtMonthDayHourMinute);
            this.pnlMonthDayTime.Controls.Add(this.label5);
            this.pnlMonthDayTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMonthDayTime.Location = new System.Drawing.Point(0, 40);
            this.pnlMonthDayTime.Name = "pnlMonthDayTime";
            this.pnlMonthDayTime.Size = new System.Drawing.Size(649, 40);
            this.pnlMonthDayTime.TabIndex = 1;
            // 
            // dtMonthDayHourMinute
            // 
            this.dtMonthDayHourMinute.CustomFormat = "dd/MMMM ,   HH:mm";
            this.dtMonthDayHourMinute.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMonthDayHourMinute.Location = new System.Drawing.Point(58, 9);
            this.dtMonthDayHourMinute.Name = "dtMonthDayHourMinute";
            this.dtMonthDayHourMinute.ShowUpDown = true;
            this.dtMonthDayHourMinute.Size = new System.Drawing.Size(591, 20);
            this.dtMonthDayHourMinute.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-3, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Date/Time";
            // 
            // pnlExactDateTime
            // 
            this.pnlExactDateTime.Controls.Add(this.dtExactDateTime);
            this.pnlExactDateTime.Controls.Add(this.label4);
            this.pnlExactDateTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlExactDateTime.Location = new System.Drawing.Point(0, 0);
            this.pnlExactDateTime.Name = "pnlExactDateTime";
            this.pnlExactDateTime.Size = new System.Drawing.Size(649, 40);
            this.pnlExactDateTime.TabIndex = 0;
            // 
            // dtExactDateTime
            // 
            this.dtExactDateTime.CustomFormat = "dddd, yyyy-MMMM-dd ,   HH:mm";
            this.dtExactDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtExactDateTime.Location = new System.Drawing.Point(58, 9);
            this.dtExactDateTime.Name = "dtExactDateTime";
            this.dtExactDateTime.Size = new System.Drawing.Size(591, 20);
            this.dtExactDateTime.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-3, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Exact date";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(13, 77);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(47, 13);
            this.label15.TabIndex = 17;
            this.label15.Text = "Duration";
            // 
            // comboDuration
            // 
            this.comboDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDuration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDuration.FormattingEnabled = true;
            this.comboDuration.Location = new System.Drawing.Point(73, 73);
            this.comboDuration.Name = "comboDuration";
            this.comboDuration.Size = new System.Drawing.Size(589, 21);
            this.comboDuration.TabIndex = 18;
            // 
            // cbPushNotifications
            // 
            this.cbPushNotifications.AutoSize = true;
            this.cbPushNotifications.Location = new System.Drawing.Point(16, 169);
            this.cbPushNotifications.Name = "cbPushNotifications";
            this.cbPushNotifications.Size = new System.Drawing.Size(296, 17);
            this.cbPushNotifications.TabIndex = 19;
            this.cbPushNotifications.Text = "Alarm produces push-notifications outside the applicattion";
            this.cbPushNotifications.UseVisualStyleBackColor = true;
            this.cbPushNotifications.CheckedChanged += new System.EventHandler(this.cbPushNotifications_CheckedChanged);
            // 
            // txPushNotificationResource
            // 
            this.txPushNotificationResource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txPushNotificationResource.Enabled = false;
            this.txPushNotificationResource.Location = new System.Drawing.Point(74, 192);
            this.txPushNotificationResource.Name = "txPushNotificationResource";
            this.txPushNotificationResource.ReadOnly = true;
            this.txPushNotificationResource.Size = new System.Drawing.Size(554, 20);
            this.txPushNotificationResource.TabIndex = 20;
            // 
            // btnSelectTextResources
            // 
            this.btnSelectTextResources.Enabled = false;
            this.btnSelectTextResources.Location = new System.Drawing.Point(632, 192);
            this.btnSelectTextResources.Name = "btnSelectTextResources";
            this.btnSelectTextResources.Size = new System.Drawing.Size(32, 20);
            this.btnSelectTextResources.TabIndex = 21;
            this.btnSelectTextResources.Text = "...";
            this.btnSelectTextResources.UseVisualStyleBackColor = true;
            this.btnSelectTextResources.Click += new System.EventHandler(this.OnSelectStringResources);
            // 
            // lstBuilds
            // 
            this.lstBuilds.CheckBoxes = true;
            this.lstBuilds.Location = new System.Drawing.Point(73, 221);
            this.lstBuilds.Name = "lstBuilds";
            this.lstBuilds.Size = new System.Drawing.Size(589, 82);
            this.lstBuilds.TabIndex = 22;
            this.lstBuilds.UseCompatibleStateImageBehavior = false;
            this.lstBuilds.View = System.Windows.Forms.View.List;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 221);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(35, 13);
            this.label16.TabIndex = 23;
            this.label16.Text = "Builds";
            // 
            // cbOneTimeOnly
            // 
            this.cbOneTimeOnly.AutoSize = true;
            this.cbOneTimeOnly.Location = new System.Drawing.Point(16, 123);
            this.cbOneTimeOnly.Name = "cbOneTimeOnly";
            this.cbOneTimeOnly.Size = new System.Drawing.Size(118, 17);
            this.cbOneTimeOnly.TabIndex = 13;
            this.cbOneTimeOnly.Text = "One-time only alarm";
            this.cbOneTimeOnly.UseVisualStyleBackColor = true;
            // 
            // process1
            // 
            this.process1.StartInfo.Domain = "";
            this.process1.StartInfo.LoadUserProfile = false;
            this.process1.StartInfo.Password = null;
            this.process1.StartInfo.StandardErrorEncoding = null;
            this.process1.StartInfo.StandardOutputEncoding = null;
            this.process1.StartInfo.UserName = "";
            this.process1.SynchronizingObject = this;
            this.process1.Exited += new System.EventHandler(this.process1_Exited);
            // 
            // AlarmEditDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(684, 415);
            this.Controls.Add(this.cbOneTimeOnly);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lstBuilds);
            this.Controls.Add(this.btnSelectTextResources);
            this.Controls.Add(this.txPushNotificationResource);
            this.Controls.Add(this.cbPushNotifications);
            this.Controls.Add(this.comboDuration);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.txName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbEnabled);
            this.Controls.Add(this.comboAlertType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AlarmEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alarm editor";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlEveryXDays.ResumeLayout(false);
            this.pnlEveryXDays.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmEveryXDays)).EndInit();
            this.pnlEveryXHours.ResumeLayout(false);
            this.pnlEveryXHours.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmEveryXHours)).EndInit();
            this.pnlEveryXMinutes.ResumeLayout(false);
            this.pnlEveryXMinutes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmEveryXMinutes)).EndInit();
            this.pnlEveryDayAtASpecificTime.ResumeLayout(false);
            this.pnlEveryDayAtASpecificTime.PerformLayout();
            this.pnlDayFromWeek.ResumeLayout(false);
            this.pnlDayFromWeek.PerformLayout();
            this.pnlDayFromMonth.ResumeLayout(false);
            this.pnlDayFromMonth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmDayFromMonth)).EndInit();
            this.pnlMonthDayTime.ResumeLayout(false);
            this.pnlMonthDayTime.PerformLayout();
            this.pnlExactDateTime.ResumeLayout(false);
            this.pnlExactDateTime.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboAlertType;
        private System.Windows.Forms.CheckBox cbEnabled;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnlExactDateTime;
        private System.Windows.Forms.DateTimePicker dtExactDateTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlMonthDayTime;
        private System.Windows.Forms.DateTimePicker dtMonthDayHourMinute;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlDayFromWeek;
        private System.Windows.Forms.ComboBox comboDayOfWeek;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pnlDayFromMonth;
        private System.Windows.Forms.NumericUpDown nmDayFromMonth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlEveryDayAtASpecificTime;
        private System.Windows.Forms.DateTimePicker dtHourMinute;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel pnlEveryXMinutes;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nmEveryXMinutes;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel pnlEveryXDays;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nmEveryXDays;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel pnlEveryXHours;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nmEveryXHours;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboDuration;
        private System.Windows.Forms.CheckBox cbPushNotifications;
        private System.Windows.Forms.TextBox txPushNotificationResource;
        private System.Windows.Forms.Button btnSelectTextResources;
        private System.Windows.Forms.ListView lstBuilds;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox cbOneTimeOnly;
        private System.Diagnostics.Process process1;
    }
}