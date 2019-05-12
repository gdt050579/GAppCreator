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
    public partial class ResourceSelectDialog : Form,ITerminateEdit
    {
        private ResourceSelectControl rsc;
        public string SelectedResource = "";
        public ResourceSelectDialog(ProjectContext pContext, ResourcesConstantType rType, bool disableFilterButton,bool enableNullResourceButton)
        {
            InitializeComponent();
            ResourceSelectControl.InitControl(pContext, disableFilterButton, enableNullResourceButton);
            rsc = new ResourceSelectControl();
            rsc.Dock = DockStyle.Fill;
            rsc.Init(new KeyValuePair<ResourcesConstantType, string>(rType, ""), this);
            pnl.Controls.Add(rsc);
            if (enableNullResourceButton)
                btnNone.Visible = true;
        }

        private void OnOK(object sender, EventArgs e)
        {
            if ((rsc.SelectedResource.Length==0) && (btnNone.Visible == false))
            {
                MessageBox.Show("Please select a resource first !");
                return;
            }
            this.Tag = SelectedResource = rsc.SelectedResource;               
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        public void FinishEdit()
        {
            OnOK(null, null);
        }

        private void OnNULL(object sender, EventArgs e)
        {
            this.Tag = SelectedResource = "";
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
