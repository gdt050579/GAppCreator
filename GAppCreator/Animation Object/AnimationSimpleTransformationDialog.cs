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
    public partial class AnimationSimpleTransformationDialog : Form
    {
        public AnimO.GenericTransformation SelectedTransformation = null;
        public AnimationSimpleTransformationDialog(ImageList icons,bool blocks, bool actions)
        {
            InitializeComponent();
            lstTransformation.LargeImageList = icons;
            lstTransformation.SmallImageList = icons;
            this.Text = "Add";
            if (blocks)
                AddBlocks();
            if (actions)
                AddExtraItems();
        }

        private void AddBlocks()
        {
            lstTransformation.Groups.Add("blocks", "Blocks");
            AddItem("If..Else", "if_else", typeof(AnimO.IfElseBlock), "blocks");
            AddItem("Branch (switch loop)", "branch_block", typeof(AnimO.BranchBlock),"blocks");
            AddItem("Continous / Infinte loop", "infinite_loop_block", typeof(AnimO.ContinousBlock), "blocks");
            AddItem("Parallel", "parralel_block", typeof(AnimO.TransformationBlock), "blocks");
            AddItem("Repeat", "repeat_block", typeof(AnimO.RepeatBlock), "blocks");
            AddItem("Repeat ... Until", "repeat_until", typeof(AnimO.RepeatUntil), "blocks");
            AddItem("Do Once... Until", "do_until", typeof(AnimO.DoOnceUntil), "blocks");
            AddItem("Popup Loop", "popup_loop", typeof(AnimO.PopupLoop), "blocks");
            AddItem("Do Once... Popup Loop", "do_once_popup_loop", typeof(AnimO.DoOncePopupLoop), "blocks");
            AddItem("Sequance", "sequance_block", typeof(AnimO.TransformationBlock), "blocks");            
        }

        private void AddExtraItems()
        {
            lstTransformation.Groups.Add("actions", "Actions");
            AddItem("Event", "event", typeof(AnimO.EventTransformation), "actions");
            AddItem("Set Animation end Event", "animation_end_event", typeof(AnimO.AnimationEndEventTransformation), "actions");            
            AddItem("Sound", "sound", typeof(AnimO.SoundTransformation), "actions");
            AddItem("Stopper", "stop", typeof(AnimO.Stopper), "actions");
            AddItem("Wait until triggered", "wait_until", typeof(AnimO.WaitUntil), "actions");
            AddItem("Timer", "timer", typeof(AnimO.TimerTransformation), "actions");
            AddItem("Touch status", "touch_status", typeof(AnimO.TouchStatusTransformation), "actions");
            AddItem("Touch boundary", "touch_boundary", typeof(AnimO.TouchBoundaryTransformation), "actions"); 
            AddItem("Z-Order", "z_order", typeof(AnimO.ZOrderTransformation), "actions");
        }

        private void AddItem(string name,string icon, Type t, string groupName)
        {
            ListViewItem lvi = new ListViewItem(name);
            lvi.ImageKey = icon;
            lvi.Tag = t;
            lvi.Group = lstTransformation.Groups[groupName];
            lstTransformation.Items.Add(lvi);
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (lstTransformation.SelectedItems.Count!=1)
            {
                MessageBox.Show("Please select a transformation !");
                return;
            }
            SelectedTransformation = (AnimO.GenericTransformation)Activator.CreateInstance((Type)(lstTransformation.SelectedItems[0].Tag));
            if (SelectedTransformation.GetType() == typeof(AnimO.TransformationBlock))
            {
                if (lstTransformation.SelectedItems[0].Text.StartsWith("P"))
                    ((AnimO.TransformationBlock)SelectedTransformation).BlockType = AnimO.TransformationBlockType.Parallel;
                else
                    ((AnimO.TransformationBlock)SelectedTransformation).BlockType = AnimO.TransformationBlockType.Sequance;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void lstTransformation_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstTransformation.SelectedItems.Count == 1)
                OnOK(null,null);
        }

        private void lstTransformation_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
