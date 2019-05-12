using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class BaseProjectContainer : UserControl
    {
        protected ProjectContext Context;
        public BaseProjectContainer()
        {
            InitializeComponent();
        }
        public virtual void OnCommand(Command cmd,object parameters=null)
        {
           
        }
        public virtual void OnActivate()
        {

        }
        public virtual void OnOpenNewProject(bool newProject)
        {

        }
        public virtual void OnSetContext()
        {

        }
        public void SetContext(ProjectContext context)
        {
            Context = context;
            if (Context != null)
                OnSetContext();
        }
        protected void ProcessContextMenuCommand(object sender, object parameters = null)
        {
            var snd = sender as ToolStripMenuItem;
            if (snd == null)
            {
                MessageBox.Show("Current object is not a menu !!!\n");
                return;
            }
            if (snd.Tag == null)
            {
                MessageBox.Show("'Tag' parameter was not set for current menu item !");
                return;
            }
            Command cmd;
            if ((Enum.TryParse<Command>(snd.Tag.ToString(), out cmd) == false))
            {
                MessageBox.Show("Unkwnon commnad for current menu item: " + snd.Tag.ToString());
                return;
            }
            this.OnCommand(cmd,parameters);
        }
    }
}
