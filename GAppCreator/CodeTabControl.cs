using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    class CodeTabControl: TabControl
    {
        public Project prj = null;
        List<TabPage> TabOrder = new List<TabPage>();
        TabSelector Selector = new TabSelector();

        public CodeTabControl(): base()
        {
            Selector.Show();
            Selector.Visible = false;
            this.KeyUp += CodeTabControl_KeyUp;
            this.SelectedIndexChanged += CodeTabControl_SelectedIndexChanged;
            this.ControlAdded += CodeTabControl_ControlAdded;
            this.ControlRemoved += CodeTabControl_ControlRemoved;
        }

        void CodeTabControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control.GetType() == typeof(TabPage))
            {
                TabOrder.Remove((TabPage)e.Control);
            }
        }

        void CodeTabControl_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control.GetType() == typeof(TabPage))
            {
                TabOrder.Insert(0, (TabPage)e.Control);
            }
        }

        void CodeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage tp = this.SelectedTab;
            if (tp!=null)
            {
                TabOrder.Remove(tp);
                TabOrder.Insert(0, tp);
            }
        }

        void CodeTabControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                if (Selector.Visible)
                {
                    Selector.Visible = false;
                    int index = Selector.GetSelectedTabIndex();
                    if ((index >= 0) && (index<=TabOrder.Count))
                        this.SelectTab(TabOrder[index]);
                }
            }
        }
        private void ShowSelector()
        {
            if (Selector.Visible == false)
            {
                Point p = this.PointToScreen(new Point(this.Width / 2, this.Height / 2));
                p.X -= Selector.Width / 2;
                p.Y -= Selector.Height / 2;
                Selector.Location = p;
                Selector.UpdateItems(TabOrder);
                Selector.Visible = true;
                Focus();
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == (Keys.Control | Keys.Tab)))// || (keyData == (Keys.Control | Keys.Down)))
            {
                ShowSelector();
                Selector.SelectNext(1);
                return true;
            }

            //if ((keyData == (Keys.Control | Keys.Shift | Keys.Tab))|| (keyData == (Keys.Control | Keys.Up)))
            //{
            //    ShowSelector();
            //    Selector.SelectNext(-1);
            //    return true;
            //}
            if (keyData == Keys.None)
            {
                Selector.Visible = false;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
