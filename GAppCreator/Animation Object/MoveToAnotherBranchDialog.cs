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
    public partial class MoveToAnotherBranchDialog : Form
    {
        public AnimO.GenericTransformation SelectedNewParent = null;
        AnimO.GenericTransformation transToMove;
        public MoveToAnotherBranchDialog(ImageList icons, AnimO.AnimationObject anim, AnimO.GenericTransformation _transToMove)
        {
            InitializeComponent();
            treeTransformations.ImageList = icons;
            transToMove = _transToMove;
            CreateTreeNode(null, anim.Main);
            treeTransformations.ExpandAll();
        }

        private void UpdateTreeNode(TreeNode node)
        {
            if (node == null)
                return;
            AnimO.GenericTransformation trans = (AnimO.GenericTransformation)node.Tag;
            if (trans != null)
            {
                if ((trans.BranchName != null) && (trans.BranchName.Length > 0))
                    node.Text = "[" + trans.BranchName + "] - " + trans.GetName();
                else
                    node.Text = trans.GetName();
                node.ImageKey = trans.GetIconKey();
            }
            else
            {
                node.Text = "???";
                node.ImageKey = "";
            }
            node.SelectedImageKey = node.ImageKey;
        }
        private void CreateTreeNode(TreeNode parent, AnimO.GenericTransformation trans)
        {
            if (trans == transToMove)
                return;
            List<AnimO.GenericTransformation> copii = trans.GetBlockTransformations();
            if (copii == null)
                return;

            TreeNode tn = new TreeNode();
            tn.Tag = trans;
            if (parent == null)
                treeTransformations.Nodes.Add(tn);
            else
                parent.Nodes.Add(tn);
            foreach (AnimO.GenericTransformation c_trans in copii)
                CreateTreeNode(tn, c_trans);

            UpdateTreeNode(tn);
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (treeTransformations.SelectedNode == null)
            {
                MessageBox.Show("Please select a parent !");
                return;
            }
            SelectedNewParent = (AnimO.GenericTransformation)treeTransformations.SelectedNode.Tag;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void treeTransformations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeTransformations.SelectedNode != null)
                OnOK(null, null);
        }
    }
}
