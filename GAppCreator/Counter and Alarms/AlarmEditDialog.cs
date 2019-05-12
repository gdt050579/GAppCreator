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
    public partial class AlarmEditDialog : Form
    {
        Alarm alarm;
        Project prj;
        ProjectContext pContext;
        public AlarmEditDialog(Alarm _alarm, ProjectContext _pContext)
        {
            alarm = _alarm;
            pContext = _pContext;
            prj = pContext.Prj;
            InitializeComponent();
            // initializez combo-urile
            foreach (string s in Alarm.alarmTypes)
                comboAlertType.Items.Add(s);
            foreach (GenericBuildConfiguration gb in prj.BuildConfigurations)
                lstBuilds.Items.Add(gb.Name);
            List<string> lst;
            lst = Alarm.GetDurationValues();
            foreach (string s in lst)
                comboDuration.Items.Add(s);

            // updatez
            if (alarm != null)
                UpdateValues();
            comboAlertType_SelectedIndexChanged(null, null);
        }

        private void UpdateValues()
        {
            comboAlertType.SelectedIndex = alarm.AlarmType;
            txName.Text = alarm.Name;
            string duration = Alarm.ConvertValueToStringDuration(alarm.Duration);
            for (int tr=0; tr < comboDuration.Items.Count; tr++)
                if (duration.Equals(comboDuration.Items[tr].ToString()))
                {
                    comboDuration.SelectedIndex = tr;
                    break;
                }
            cbEnabled.Checked = alarm.Enabled;
            cbOneTimeOnly.Checked = alarm.OneTimeOnly;
            cbPushNotifications.Checked = alarm.PushNotification;
            txPushNotificationResource.Text = alarm.PushNotificationString;
            cbPushNotifications_CheckedChanged(null, null);
            Dictionary<string, string> d = Project.StringListToDict(alarm.Builds);
            foreach (ListViewItem lvi in lstBuilds.Items)
                if (d.ContainsKey(lvi.Text.ToLower()))
                    lvi.Checked = true;
            // updatez si valorile
            switch (alarm.AlarmType)
            {
                case 0:
                    dtExactDateTime.Value = new DateTime(alarm.Year, alarm.Month, alarm.Day, alarm.Hour, alarm.Minute, 0);
                    break;
                case 1:
                    dtMonthDayHourMinute.Value = new DateTime(2000, alarm.Month, alarm.Day, alarm.Hour, alarm.Minute, 0);
                    break;
                case 2:
                    nmDayFromMonth.Value = alarm.Day;
                    break;
                case 3:
                    comboDayOfWeek.SelectedIndex = alarm.Day;
                    break;
                case 4:
                    dtHourMinute.Value = new DateTime(2000, 1, 1, alarm.Hour, alarm.Minute, 0);
                    break;
                case 5:
                    nmEveryXMinutes.Value = alarm.Minute;
                    break;
                case 6:
                    nmEveryXHours.Value = alarm.Hour;
                    break;
                case 7:
                    nmEveryXDays.Value = alarm.Day;
                    break;
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            Alarm newAlert = new Alarm();
            // numele
            newAlert.Name = txName.Text.Trim();
            if (Project.ValidateVariableNameCorectness(newAlert.Name)==false)
            {
                MessageBox.Show("Invalid name for an allert (must contain [A-Za-z0-9_] and must start with a capital letter !");
                txName.Focus();
                return;
            }
            foreach (Alarm a in prj.Alarms)
            {
                // ma skipez pe mine daca e cazul
                if (a == alarm) 
                    continue;
                if (a.Name == newAlert.Name)
                {
                    MessageBox.Show("There is already an alert name '"+a.Name+"' in this project. Rename this new alert !");
                    txName.Focus();
                    return;
                }
            }
            // tipul de alerta
            if (comboAlertType.SelectedIndex<0)
            {
                MessageBox.Show("Please select an alert type !");
                comboAlertType.Focus();
                return;
            }
            newAlert.AlarmType = comboAlertType.SelectedIndex;

            // duration
            if (comboDuration.SelectedIndex < 0)
            {
                MessageBox.Show("Please select the duration of this alert !");
                comboDuration.Focus();
                return;
            }
            newAlert.Duration = Alarm.ConvertStringDurationToValue(comboDuration.Items[comboDuration.SelectedIndex].ToString());
            // enabled
            newAlert.Enabled = cbEnabled.Checked;
            // one time only
            newAlert.OneTimeOnly = cbOneTimeOnly.Checked;
            // push notifications
            newAlert.PushNotification = cbPushNotifications.Checked;
            if (newAlert.PushNotification)
            {
                Dictionary<string,StringValues> strRes = prj.GetStringResources();
                newAlert.PushNotificationString = txPushNotificationResource.Text.Trim();
                if (newAlert.PushNotificationString.Length ==0)
                {
                    MessageBox.Show("Please select a push notification string first !");
                    btnSelectTextResources.Focus();
                    return;
                }
                if (strRes.ContainsKey(newAlert.PushNotificationString)==false)
                {
                    MessageBox.Show("Invalid resource: '"+newAlert.PushNotificationString+"' ! Please select a push notification string first !");
                    btnSelectTextResources.Focus();
                    return;
                }
            }
            // build-uri
            List<string> lst= new List<string>();
            foreach (ListViewItem lvi in lstBuilds.Items)
                if (lvi.Checked)
                    lst.Add(lvi.Text);
            if (lst.Count==0)
            {
                MessageBox.Show("You have to select at least one build for this alert !");
                lstBuilds.Focus();
                return;
            }
            newAlert.Builds = Project.ListToStringList(lst);
            // validarea valorii
            newAlert.Year = 0;
            newAlert.Month = 0;
            newAlert.Day = 0;
            newAlert.Hour = 0;
            newAlert.Minute = 0;
            switch (newAlert.AlarmType)
            {
                case 0:
                    newAlert.Year = dtExactDateTime.Value.Year;
                    newAlert.Month = dtExactDateTime.Value.Month;
                    newAlert.Day = dtExactDateTime.Value.Day;
                    newAlert.Hour = dtExactDateTime.Value.Hour;
                    newAlert.Minute = dtExactDateTime.Value.Minute;
                    break;
                case 1:
                    newAlert.Month = dtMonthDayHourMinute.Value.Month;
                    newAlert.Day = dtMonthDayHourMinute.Value.Day;
                    newAlert.Hour = dtMonthDayHourMinute.Value.Hour;
                    newAlert.Minute = dtMonthDayHourMinute.Value.Minute;
                    break;
                case 2:
                    newAlert.Day = (int)nmDayFromMonth.Value;
                    break;
                case 3:
                    newAlert.Day = comboDayOfWeek.SelectedIndex;
                    if (newAlert.Day < 0)
                    {
                        MessageBox.Show("Please select a day of the week !");
                        comboDayOfWeek.Focus();
                        return;
                    }
                    break;
                case 4:
                    newAlert.Hour = dtHourMinute.Value.Hour;
                    newAlert.Minute = dtHourMinute.Value.Minute;
                    break;
                case 5:
                    newAlert.Minute = (int)nmEveryXMinutes.Value;
                    break;
                case 6:
                    newAlert.Hour = (int)nmEveryXHours.Value;
                    break;
                case 7:
                    newAlert.Day = (int)nmEveryXDays.Value;
                    break;
            }
            // last check
            prj.EC.Reset();
            if (newAlert.Validate(prj)==false)
            {
                prj.ShowErrors();
                prj.EC.Reset();
                return;
            }
            // all is ok
            if (alarm == null)
            {
                prj.Alarms.Add(newAlert);
                alarm = newAlert;
            }
            else
            {
                alarm.CopyFrom(newAlert);
            }
            prj.Alarms.Sort();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void comboAlertType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = comboAlertType.SelectedIndex;
            pnlExactDateTime.Visible = (idx == 0);
            pnlMonthDayTime.Visible = (idx == 1);
            pnlDayFromMonth.Visible = (idx == 2);
            pnlDayFromWeek.Visible = (idx == 3);
            pnlEveryDayAtASpecificTime.Visible = (idx == 4);
            pnlEveryXMinutes.Visible = (idx == 5);
            pnlEveryXHours.Visible = (idx == 6);
            pnlEveryXDays.Visible = (idx == 7);

            cbOneTimeOnly.Enabled = true;
            comboDuration.Enabled = true;
            // alte euristici legate de selectie
            switch (idx)
            {
                case 0: /* "Exact date (Year / Month / Day / Hour / Minute)" */
                    cbOneTimeOnly.Enabled = false;
                    cbOneTimeOnly.Checked = true;
                    break;
                case 2: /* "On a specific day of the month (from 00:00 to 23:59)" */
                case 3: /* "On a specific day of a week (from 00:00 to 23:59)" */
                    comboDuration.Enabled = false;
                    comboDuration.SelectedIndex = -1;
                    break;
            }
        }

        public Alarm GetCurrentAlarm()
        {
            return alarm;
        }

        private void cbPushNotifications_CheckedChanged(object sender, EventArgs e)
        {
            btnSelectTextResources.Enabled = cbPushNotifications.Checked;
            txPushNotificationResource.Enabled = cbPushNotifications.Checked;
        }

        private void OnSelectStringResources(object sender, EventArgs e)
        {
            ResourceSelectDialog dlg = new ResourceSelectDialog(pContext, ResourcesConstantType.String,true,false);                    
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txPushNotificationResource.Text = dlg.SelectedResource;
            }
        }

        private void process1_Exited(object sender, EventArgs e)
        {

        }
    }
}
