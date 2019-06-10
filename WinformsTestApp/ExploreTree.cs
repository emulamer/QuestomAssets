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
using QuestomAssets;
using QuestomAssets.Utils;

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

        public static Node ClipData { get; set; }

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
                                SelectedNode = null;
                                SelectedNode = tn;
                                tn.EnsureVisible();                                
                            }
                        }));
                    }
                    else
                    {
                        cm.MenuItems.Add(new MenuItem("Copy", (o, ea) =>
                        {
                            Clipboard.Clear();
                            Clipboard.SetText(n.GetHashCode().ToString(), TextDataFormat.Text);
                            ClipData = n;
                        }));
                    }
                }
                if (n.StubToNode == null)
                {
                    if (CanPaste(n))
                    {
                        cm.MenuItems.Add(new MenuItem("Paste Clone", (o, ea) =>
                        {
                            DoPaste(n);
                        }));
                    }
                    if (CanPastePointer(n))
                    {
                        cm.MenuItems.Add(new MenuItem("Paste Pointer", (o, ea) =>
                        {
                            DoPastePointer(n);
                        }));
                    }
                }

                if (cm.MenuItems.Count > 0)
                    cm.Show(sender as Control, (sender as Control).PointToClient(Cursor.Position));
            }
        }

        private Node GetClipData()
        {
            var clipObj = Clipboard.GetDataObject()?.GetData(DataFormats.Text) as string;
            int clipHash = 0;

            if (clipObj != null && ClipData != null && Int32.TryParse(clipObj, out clipHash) && ClipData.GetHashCode() == clipHash)
            {
                return ClipData;
            }
            return null;
        }

        private bool CanPastePointer(Node targetNode)
        {
            try
            {
                var target = targetNode?.Obj;
                if (target == null)
                    return false;

                var source = GetClipData();

                var sourceObj = source?.Obj as AssetsObject;
                if (sourceObj == null)
                    return false;

                if (target.GetType() == typeof(AssetsFile))
                    return false;

                Type assetType = null;
                var ptrList = target as IEnumerable<ISmartPtr<AssetsObject>>;
                if (ptrList != null)
                {
                    //it's a list of some kind of pointers... can possibly paste (add) into here
                    assetType = target.GetType().GetGenericArguments()[0].GetGenericArguments()[0];
                }

                var ptr = target as ISmartPtr<AssetsObject>;
                if (ptr != null)
                {
                    //if it's a pointer we can maybe replace it if it isn't in a collection.
                    if (ReflectionHelper.IsPropNameAssignableToType(ptr.Owner, targetNode.ParentPropertyName, typeof(IEnumerable<ISmartPtr<AssetsObject>>)))
                        return false;

                    assetType = target.GetType().GetGenericArguments()[0];
                    if (string.IsNullOrWhiteSpace(targetNode.ParentPropertyName))
                    {
                        Log.LogErr($"Tried to paste, but type {target.GetType().Name} on node '{source.Text}' has no ParentPropertyName set.  Check MakeNode to figure out why.");
                        return false;
                    }
                }


                if (assetType == null)
                    return false;

                if (assetType.IsAssignableFrom(source.Obj.GetType()))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception trying to see if paste is allowed.", ex);
                return false;
            }
        }

        private bool CanPaste(Node targetNode)
        {
            try
            {
                var target = targetNode?.Obj;
                if (target == null)
                    return false;

                var source = GetClipData();

                if (source?.Obj == null)
                    return false;
                if (target.GetType() == typeof(AssetsFile))
                    return true;

                return CanPastePointer(targetNode);
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception trying to see if paste is allowed.", ex);
                return false;
            }
        }

        private Node FindFirstParent(Node node)
        {
            if (node?.Obj == null)
                return null;

            if (typeof(AssetsObject).IsAssignableFrom(node.Obj.GetType()))
                return node;

            return FindFirstParent(node.Parent);
        }

        private List<CloneExclusion> GetExclusionsForObject(AssetsObject o, AssetsFile targetAssetsFile)
        {
            List<CloneExclusion> exclusions = new List<CloneExclusion>();

            //exclude any monobehaviors that the script type can't be found for
            exclusions.Add(new CloneExclusion(ExclusionMode.Remove)
            {
                Filter = (ptr, propInfo) =>
                {
                    var res = ptr != null && ptr.Object is MonoBehaviourObject && targetAssetsFile.Manager.GetScriptObject(ptr.Target.Type.TypeHash) == null;
                    if (res)
                    {
                        Log.LogMsg($"Removing MonoBehaviour object during cloning because type hash '{ptr.Target.Type.TypeHash}' doesn't exist in the target.");
                    }
                    return res;
                }

            });
            exclusions.Add(new CloneExclusion(ExclusionMode.Remove)
            {
                Filter = (ptr, propInfo) =>
                {
                    var res = ptr != null && ptr.Target.Type.ClassID == AssetsConstants.ClassID.AnimationClassID;
                    if (res)
                    {
                        Log.LogMsg($"Removing Animation object during cloning because it isn't supported yet.");
                    }
                    return res;
                }
            });
            if (typeof(Transform).IsAssignableFrom(o.GetType()))
            {
                exclusions.Add(new CloneExclusion(ExclusionMode.LeaveRef, propertyName: "Father", pointerTarget: ((Transform)o).Father.Target.Object));
            }
            return exclusions;
        }

        //todo: refactor
        private void DoPaste(Node targetNode)
        {
            try
            {
                var node = GetClipData();
                var sourceObj = node?.Obj as AssetsObject;
                var targetObj = targetNode?.Obj;

                AssetsFile targetFile = null;
                Node targetOwnerNode = null;
                AssetsObject targetOwnerObj = null;
                object targetDirectParentObj = null;

                if (node == null || node.Obj == null || node.StubToNode != null || sourceObj == null || targetObj == null)
                    return;
                bool isFile = false;
                if (targetObj is AssetsFile)
                {
                    isFile = true;
                    targetFile = targetObj as AssetsFile;
                    targetOwnerNode = targetNode.Parent;
                }
                else
                {
                    targetOwnerNode = FindFirstParent(targetNode);
                    targetOwnerObj = targetOwnerNode?.Obj as AssetsObject;
                    targetDirectParentObj = targetNode?.Parent?.Obj;
                    if (targetOwnerObj == null)
                    {
                        Log.LogErr($"Tried to pase, but couldn't find the assetsobject owner on node '{targetNode.Text}'");
                        return;
                    }
                    if (targetDirectParentObj == null)
                    {
                        Log.LogErr($"Tried to paste, but couldn't find the actual parent on node '{targetNode.Text}'");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(targetNode.ParentPropertyName))
                    {
                        Log.LogErr($"Tried to paste, but parent property name was null on node '{targetNode.Text}'");
                        return;
                    }
                    targetFile = targetOwnerObj.ObjectInfo.ParentFile;
                }
                List<AssetsObject> addedObjects = new List<AssetsObject>();
                AssetsObject cloned = null;
                try
                {
                    var exclus = GetExclusionsForObject(sourceObj, targetFile);
                    cloned = sourceObj.ObjectInfo.DeepClone(targetFile, addedObjects: addedObjects, exclusions: exclus);
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Exception trying to clone object of type {sourceObj.GetType().Name}!", ex);
                    try
                    {
                        foreach (var ao in addedObjects)
                        {
                            targetFile.DeleteObject(ao);
                        }
                    }
                    catch (Exception ex2)
                    {
                        Log.LogErr("Failed to clean up after bad clone!", ex2);
                        MessageBox.Show("A clone failed and the rollback failed too.  The assets files are in an unknown state!");
                    }
                    return;
                }
                bool updated = false;
                if (isFile)
                {
                    updated= true;
                }
                else
                {
                    var ptrList = targetObj as IEnumerable<ISmartPtr<AssetsObject>>;
                    if (ptrList != null)
                    {
                        try
                        {
                            var pointer = ReflectionHelper.MakeTypedPointer(targetOwnerObj, cloned);
                            ReflectionHelper.AddObjectToEnum(pointer, ptrList);
                            updated = true;
                        }
                        catch (Exception ex)
                        {
                            Log.LogErr($"Adding object to collection failed!", ex);
                            MessageBox.Show("Object was created, but could not be attached to the collection!");
                            return;
                        }
                    }

                    var ptr = targetObj as ISmartPtr<AssetsObject>;
                    if (ptr != null)
                    {
                        try
                        {
                            var pointer = ReflectionHelper.MakeTypedPointer(targetOwnerObj, cloned);
                            var oldPointer = ReflectionHelper.GetPtrFromPropName(targetDirectParentObj, targetNode.ParentPropertyName);
                            ReflectionHelper.AssignPtrToPropName(targetDirectParentObj, targetNode.ParentPropertyName, pointer);
                            oldPointer.Dispose();
                            updated = true;
                        }
                        catch (Exception ex)
                        {
                            Log.LogErr($"Replacing pointer failed on {targetDirectParentObj?.GetType()?.Name}.{targetNode.ParentPropertyName}!");
                        }
                    }
                }
                if (updated)
                {
                    var res = (tvExplorer.Nodes[0].Tag as Node).GetNodePath(targetNode);
                    //update node, hopefully we won't have to repopulate the entire thing?
                    
                    

                    if (!isFile)
                    {
                        Node newNode = Node.MakeNode(targetOwnerObj);
                        var targetOwnerParentNode = targetOwnerNode.Parent;
                        var idx = targetOwnerNode.Parent.Nodes.IndexOf(targetOwnerNode);
                        targetOwnerParentNode.Nodes.RemoveAt(idx);
                        targetOwnerParentNode.Nodes.Insert(idx, newNode);
                        newNode.Parent = targetOwnerParentNode;
                        newNode.ParentPropertyName = targetOwnerNode.ParentPropertyName;
                    }
                    else
                    {
                        Node newNode = Node.MakeNode((AssetsFile)targetObj);
                        var idx = targetNode.Parent.Nodes.IndexOf(targetNode);
                        targetNode.Parent.Nodes.RemoveAt(idx);
                        targetNode.Parent.Nodes.Insert(idx, newNode);
                        newNode.Parent = targetNode;
                        newNode.ParentPropertyName = null;
                    }

                    //TODO: find a better way to refresh only the altered tree node and not the whole thing

                    var ds = DataSource;
                    DataSource = null;
                    DataSource = ds;

                    TreeNode tn = tvExplorer.Nodes[0];
                    while (res.Count > 0)
                    {
                        tn = tn.Nodes[res.Pop()] as TreeNode;
                    }
                    tn.EnsureVisible();
                    SelectedNode = tn;
                    tn.Expand();

                    return;
                }
            }
            catch (Exception ex)
            {
                Log.LogErr($"Exception trying to pase object!", ex);
                MessageBox.Show("Failed to pase object!");
            }
        }

        private void DoPastePointer(Node targetNode)
        {
            try
            {
                //todo: lots of copy/paste that could be cleaned up and deduped with DoPaste
                var node = GetClipData();
                var sourceObj = node?.Obj as AssetsObject;
                var targetObj = targetNode?.Obj;
                var targetOwnerNode = FindFirstParent(targetNode);
                var targetOwnerObj = targetOwnerNode?.Obj as AssetsObject;
                var targetDirectParentObj = targetNode?.Parent?.Obj;

                if (node == null || node.Obj == null || node.StubToNode != null || sourceObj == null || targetObj == null)
                    return;
                if (targetOwnerObj == null)
                {
                    Log.LogErr($"Tried to pase, but couldn't find the assetsobject owner on node '{targetNode.Text}'");
                    return;
                }
                if (targetDirectParentObj == null)
                {
                    Log.LogErr($"Tried to paste, but couldn't find the actual parent on node '{targetNode.Text}'");
                    return;
                }
                if (string.IsNullOrWhiteSpace(targetNode.ParentPropertyName))
                {
                    Log.LogErr($"Tried to paste, but parent property name was null on node '{targetNode.Text}'");
                    return;
                }
                if (sourceObj.ObjectInfo.ParentFile.Manager != targetOwnerObj.ObjectInfo.ParentFile.Manager)
                {
                    //can't paste a pointer to a different assets manager instance
                    return;
                }
                    

               
                bool updated = false;
                var ptrList = targetObj as IEnumerable<ISmartPtr<AssetsObject>>;
                if (ptrList != null)
                {
                    try
                    {
                        var pointer = ReflectionHelper.MakeTypedPointer(targetOwnerObj, sourceObj);
                        ReflectionHelper.AddObjectToEnum(pointer, ptrList);
                        updated = true;
                    }
                    catch (Exception ex)
                    {
                        Log.LogErr($"Adding object to collection failed!", ex);
                        return;
                    }
                }

                var ptr = targetObj as ISmartPtr<AssetsObject>;
                if (ptr != null)
                {
                    try
                    {
                        var pointer = ReflectionHelper.MakeTypedPointer(targetOwnerObj, sourceObj);
                        var oldPointer = ReflectionHelper.GetPtrFromPropName(targetDirectParentObj, targetNode.ParentPropertyName);
                        ReflectionHelper.AssignPtrToPropName(targetDirectParentObj, targetNode.ParentPropertyName, pointer);
                        oldPointer.Dispose();
                        updated = true;
                    }
                    catch (Exception ex)
                    {
                        Log.LogErr($"Replacing pointer failed on {targetDirectParentObj?.GetType()?.Name}.{targetNode.ParentPropertyName}!");
                    }
                }
                if (updated)
                {
                    var res = (tvExplorer.Nodes[0].Tag as Node).GetNodePath(targetNode);
                    //update node, hopefully we won't have to repopulate the entire thing?
                    Node newnode = Node.MakeNode(targetOwnerObj);
                    var targetOwnerParentNode = targetOwnerNode.Parent;
                    var idx = targetOwnerNode.Parent.Nodes.IndexOf(targetOwnerNode);
                    targetOwnerParentNode.Nodes.RemoveAt(idx);
                    targetOwnerParentNode.Nodes.Insert(idx, newnode);
                    newnode.Parent = targetOwnerParentNode;
                    newnode.ParentPropertyName = targetOwnerNode.ParentPropertyName;

                    //TODO: find a better way to refresh only the altered tree node and not the whole thing


                    var ds = DataSource;
                    DataSource = null;
                    DataSource = ds;

                    TreeNode tn = tvExplorer.Nodes[0];
                    while (res.Count > 0)
                    {
                        tn = tn.Nodes[res.Pop()] as TreeNode;
                    }
                    tn.EnsureVisible();
                    SelectedNode = tn;
                    tn.Expand();

                    return;

                }
            }
            catch (Exception ex)
            {
                Log.LogErr($"Exception trying to pase object!", ex);
                MessageBox.Show("Failed to pase object!");
            }
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
