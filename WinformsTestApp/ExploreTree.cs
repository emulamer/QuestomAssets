using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using QuestomAssets.AssetsChanger;

namespace WinformsTestApp
{

    public partial class ExploreTree : UserControl
    {
        public ExploreTree()
        {
            InitializeComponent();
        }
        public event EventHandler<TreeNodeMouseClickEventArgs> NodeRightClicked;

        public TreeNode SelectedNode
        {
            get
            {
                return tvExplorer.SelectedNode;
            }
            set
            {
                tvExplorer.SelectedNode = value;
                if (value != null)
                    value.EnsureVisible();
            }
        }

        public TreeNodeCollection Nodes
        {
            get
            {
                return tvExplorer.Nodes;
            }
        }
        public bool AutoExpand { get; set; } = true;
        public Color HighlightColor { get; set; } = Color.AliceBlue;
        private Node _dataSource;
        public Node DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (_dataSource != value)
                {
                    tvExplorer.Nodes.Clear();
                    if (value != null)
                    {
                        tvExplorer.Nodes.Add(MakeTreeNode(value as Node));
                    }
                    else
                    {
                        tvExplorer.Nodes.Clear();
                    }
                }
                _dataSource = value;
            }
        }

        private TreeNode MakeTreeNode(Node n, int depthLimit = Int32.MaxValue, bool isClone = false)
        {
            TreeNode node = new TreeNode(n.Text);
            if (n.StubToNode != null)
            {
                //node.Text += ""
                node.ForeColor = Color.DarkBlue;
                node.Text = node.Text + " " + n.StubToNode.Text;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;
                if (n.StubToNode.Nodes.Count > 0)
                {
                    var stubnode = new TreeNode("STUB");
                    stubnode.Tag = node;
                    node.Nodes.Add(stubnode);
                }
            }
            else if (isClone)
            {
                node.ForeColor = Color.DarkBlue;
                
            }
            else
            {
               node.ImageIndex = -1;
                node.SelectedImageIndex = -1;
            }

            n.ExtRef = node;
            node.ToolTipText = n.TypeName;
            node.Tag = n;
            foreach (var cn in n.Nodes)
            {
                if (depthLimit < 1)
                {
                    node.Nodes.Add(new TreeNode("STUB"));
                }
                node.Nodes.Add(MakeTreeNode(cn, depthLimit - 1, isClone));
            }
            return node;
        }
        private const int DUPE_DEPTH_LIMIT = 5;
        private void TvExplorer_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var thisNode = e.Node.Tag as Node;
            
            if (thisNode != null && thisNode.StubToNode != null && thisNode.StubToNode.ExtRef != null)
            {                
                e.Node.Nodes.Clear();
                foreach (var n in MakeTreeNode(thisNode.StubToNode, DUPE_DEPTH_LIMIT, true).Nodes.OfType<TreeNode>().ToList())
                {
                    e.Node.Nodes.Add(n);
                }
            }
            else if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "STUB")
            {
                var n = e.Node.Tag as Node;
                if (n != null)
                {
                    e.Node.Nodes.Clear();
                    foreach (var cn in MakeTreeNode(n.StubToNode??n, DUPE_DEPTH_LIMIT, true).Nodes.OfType<TreeNode>())
                    {
                        e.Node.Nodes.Add(cn);
                    }
                }
            }
        }

        private void TvExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node.Tag as Node;

            foreach (var node in tvExplorer.Nodes.OfType<TreeNode>())
            {
                HighlightNodesByTag(node, selectedNode);
            }
        }

        private void HighlightNodesByTag(TreeNode node, Node tag)
        {
            var treetag = node.Tag as Node;
            if (tag != null && treetag != null && tag.Obj == treetag.Obj)
            {
                node.BackColor = HighlightColor;
            }
            else
            {
                node.BackColor = Color.White;
            }
            foreach(var n in node.Nodes)
            {
                HighlightNodesByTag(n as TreeNode, tag);
            }
        }

        private void TvExplorer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Node.Tag != null)
            {
                tvExplorer.SelectedNode = e.Node;
                if (NodeRightClicked == null)
                { DoRightClick(sender, e); }
                else
                { NodeRightClicked?.Invoke(this, e); }
            }
            else if (e.Button == MouseButtons.Left)
            {

            }
        }

        private bool DoFind()
        {
            if (Nodes.Count < 1)
                return false;
            TreeNode firstTreeNode = Nodes[0];

            Node firstNode = firstTreeNode.Tag as Node;
            if (firstNode == null)
                return false;
            TreeNode startNode = SelectedNode;
            var node = startNode?.Tag as Node;
            if (!string.IsNullOrWhiteSpace(txtFind.Text))
            {
                var res = firstNode.SearchNodes(txtFind.Text, node, Node.FindType.Text);
                if (res == null || res.Count < 1)
                {
                    SelectedNode = null;
                    return false;
                }
                TreeNode tn = firstTreeNode;
                while (res.Count > 0)
                {
                    tn = tn.Nodes[res.Pop()] as TreeNode;
                }
                tn.EnsureVisible();
                SelectedNode = tn;
                return true;
            }
            return false;
        }


        private void DoRightClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var cm = new ContextMenu();
            var n = e.Node.Tag as Node;
            if (n != null)
            {
                var ao = n.Obj as AssetsObject;
                if (ao != null)
                {
                    if (n.StubToNode != null)
                    {
                        cm.MenuItems.Add(new MenuItem("Go to Object", (o, ea) =>
                        {
                            var tn = n.StubToNode.ExtRef as TreeNode;
                            if (tn != null)
                            {
                                tn.EnsureVisible();
                                SelectedNode = tn;
                            }
                        }));
                    }
                }
            }

            if (cm.MenuItems.Count > 0)
                cm.Show(sender as Control, PointToClient(Cursor.Position));
        }

        private void BtnFind_Click(object sender, EventArgs e)
        {
            DoFind();
        }

        private void TxtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                if (DoFind())
                    e.SuppressKeyPress = true;
        }

    }
}
