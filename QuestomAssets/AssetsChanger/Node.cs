using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace QuestomAssets.AssetsChanger
{
    public class Node
    {
        public enum FindType
        {
            Text = 1,
            TypeName = 2,
            ObjectValues = 4,
            All = 255
        }
        private bool FindNode(string text, ref Node afterNode, FindType findType, Stack<int> path)
        {
            if (afterNode == null)
            {
                if ((findType & FindType.Text) > 0 && (this.Text?.ToLower()?.Contains(text.ToLower()) ?? false))
                {
                    return true;
                }
                if ((findType & FindType.TypeName) > 0 && (this.TypeName?.ToLower()?.Contains(text.ToLower()) ?? false))
                {
                    return true;
                }
                if ((findType & FindType.ObjectValues) > 0 && (this.Obj?.ToString()?.ToLower()?.Contains(text.ToLower()) ?? false))
                {
                    return true;
                }
            }
            if (afterNode == this)
                afterNode = null;
            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                
                if (node.FindNode(text, ref afterNode, findType, path))
                {
                    path.Push(i);
                    return true;
                }
            }
            return false;
        }
        public Stack<int> SearchNodes(string text, Node afterNode = null, FindType findType = FindType.Text)
        {
            Node nodecheck = afterNode;
            Stack<int> path = new Stack<int>();
            if (FindNode(text, ref afterNode, findType, path))
                return path;
            else
                return null;
        }

        private bool FindNodePath(Node targetNode, Stack<int> path)
        {
            if (targetNode == this)
                return true;
            
            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];

                if (node.FindNodePath(targetNode, path))
                {
                    path.Push(i);
                    return true;
                }
            }
            return false;
        }
        public Stack<int> GetNodePath(Node node)
        {
            Stack<int> path = new Stack<int>();
            FindNodePath(node, path);
            return path;
        }
        public static Node MakeNode(AssetsManager manager)
        {
            var trackedObjects = new Dictionary<object, Node>();
            var node = new Node() { Text = "All Assets", TypeName = manager.GetType().Name, Obj = manager, Depth = 0};
            foreach (var f in manager.OpenFiles.OrderBy(x => x.AssetsFilename))
                node.AddNode(MakeNode(f, 0, trackedObjects));
            return node;
        }
        public static Node MakeNode(AssetsFile file, int depth = 0, Dictionary<object, Node> trackedObjects = null)
        {
            if (trackedObjects == null)
            {
                trackedObjects = new Dictionary<object, Node>();
            }
            else
            {
                depth += 1;
            }
            var node = new Node() { Text = file.AssetsFilename, TypeName = file.GetType().Name, Obj = file, Depth = depth };
            foreach (var o in file.Metadata.ObjectInfos.Select(x => x.Object))
                node.AddNode(MakeNode(o, depth, trackedObjects));
            return node;
        }
        public static void MakeNodeAndTypeName(object o, out string nodeName, out string typeName)
        {
            Type t = o.GetType();
            if (typeof(IObjectInfo<AssetsObject>).IsAssignableFrom(t))
            {
                o = (o as IObjectInfo<AssetsObject>).Object;
                t = o.GetType();
            }
            if (t.IsValueType)
            {
                nodeName = $"{o}";
                typeName = t.Name;
            }
            else if (t == typeof(string))
            {
                nodeName = $"\"{o}\"";
                typeName = t.Name;
            }
            else if (t.IsArray && (t.GetElementType().IsValueType || t.GetElementType() == typeof(string)))
            {
                nodeName = $"{o}";
                typeName = t.Name;
            }
            else if (typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(t))
            {
                ISmartPtr<AssetsObject> ptr = o as ISmartPtr<AssetsObject>;

                nodeName = $"PtrTo F:{ptr.FileID} P:{ptr.PathID} ({o.GetType().GetGenericArguments()[0].Name})";
                typeName = o.GetType().GetGenericArguments()[0].Name;
                
            }
            else if (typeof(IEnumerable).IsAssignableFrom(t))
            {
                int i = 0;
                var thisNodeName = "";
                if (t.IsGenericType)
                {
                    thisNodeName = "[List<";
                    //half assed, only 1 generic arg to keep it simple
                    var genArg = t.GetGenericArguments()[0];

                    if (genArg.IsGenericType)
                    {
                        thisNodeName += genArg.GetGenericTypeDefinition().Name;

                        thisNodeName = thisNodeName.Substring(0, thisNodeName.LastIndexOf('`'));
                        thisNodeName += "<";
                        thisNodeName += genArg.GetGenericArguments()[0].Name;
                        thisNodeName += ">";
                        //nodeName = nodeName.Substring(0, nodeName.Length - 1);
                        //nodeName += string.Join(", ", t.GetGenericArguments().Select(x => x.Name));
                        //nodeName += ">";
                    }
                    else
                    {
                        thisNodeName += genArg.Name;
                    }
                    thisNodeName += ">]";
                }
                else
                {
                    thisNodeName = "[List]";
                }
                nodeName = thisNodeName;
                typeName = t.Name;

            }
            else if (t.GetProperties().Count() > 0)
            {
                if (typeof(AssetsObject).IsAssignableFrom(t))
                {
                    var ao = o as AssetsObject;
                    nodeName = $"{ao.ObjectInfo.ObjectID,5} {t.Name} {((o is IHaveName) ? (": " + (o as IHaveName)?.Name) : "")}";
                    typeName = ao.GetType().Name;
                }
                else
                {
                    nodeName = o.GetType().Name;
                    typeName = o.GetType().Name;
                }
            }
            else
            {
                nodeName = $"{o}";
                typeName = t.Name;
            }
        }
        public static Node MakeNode(object o, int depth = 0, Dictionary<object, Node> trackedObjects = null)
        {
            if (trackedObjects == null)
            {
                trackedObjects = new Dictionary<object, Node>();
            }
            else
            {
                depth += 1;
            }

            Node node = null;
            if (o == null)
            {
                node = new Node() { Text = "(null)", TypeName = null, Obj = null, Depth = depth };
                return node;
            }
            Type t = o.GetType();
            if (typeof(IObjectInfo<AssetsObject>).IsAssignableFrom(t))
            {
                o = (o as IObjectInfo<AssetsObject>).Object;
                t = o.GetType();
            }
                
            node = new Node(o, depth);
            if (!t.IsValueType && t != typeof(string) && !(t.IsArray && (t.GetElementType().IsValueType || t.GetElementType() == typeof(string)))
                && trackedObjects.ContainsKey(o))
            {
                var otherNode = trackedObjects[o];
                node.Text = "";
                node.StubToNode = otherNode;
                
                
                if (otherNode.Depth > depth)
                {
                    node.Depth = otherNode.Depth;
                    node.Parent = otherNode.Parent;
                    node.ParentPropertyName = otherNode.ParentPropertyName;
                    int otherIndex = otherNode.Parent.Nodes.IndexOf(otherNode);
                    otherNode.Parent.Nodes.RemoveAt(otherIndex);
                    otherNode.Parent.Nodes.Insert(otherIndex, node);
                    otherNode.Depth = depth;
                    otherNode.Parent = null;
                    
                    node = otherNode;
                }

                return node;
            }
            else
            {
                if (!t.IsValueType && t != typeof(string) && !(t.IsArray && (t.GetElementType().IsValueType || t.GetElementType() == typeof(string))))
                {
                    trackedObjects.Add(o, node);
                }
            }
            string nodeName1;
            string typeName1;
            MakeNodeAndTypeName(o, out nodeName1, out typeName1);
            node.Set(nodeName1, typeName1);
            if (typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(t))
            {
                ISmartPtr<AssetsObject> ptr = o as ISmartPtr<AssetsObject>;

                var targetNode = MakeNode(ptr.Target.Object, depth, trackedObjects);
                //targetNode.Text =  targetNode.Text;
                //targetNode.TypeName = o.GetType().GetGenericArguments()[0].Name;
                node.AddNode(targetNode);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(t))
            {
                int i = 0;

                foreach (object obj in o as IEnumerable)
                {
                    Node childNode = MakeNode(obj, depth, trackedObjects);
                    childNode.Text = $"{("[" + i.ToString() + "]").PadRight(3)} {childNode?.Text}";
                    node.AddNode(childNode);
                    i++;
                }
            }
            else if (t.GetProperties().Count() > 0)
            {
                List<PropertyInfo> props = new List<PropertyInfo>(t.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    object propValue = null;
                    try
                    {
                        //filters just to stop the exceptions, even though they're handled
                        if (!(prop.Name == "Data" && o.GetType() != typeof(AssetsObject)) &&
                            !(prop.Name == "ScriptParametersData" && o.GetType() != typeof(MonoBehaviourObject)))

                        {
                            propValue = prop.GetValue(o, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        QuestomAssets.Log.LogErr($"Failed loading property {prop.Name} on object type {o.GetType().Name}", ex);
                        node.AddNode(new Node() { Text = $"{prop.Name}: (Inaccessible)", TypeName = "", Depth = depth, ParentPropertyName = prop.Name});
                        continue;
                    }

                    if (propValue != null && typeof(IObjectInfo<AssetsObject>).IsAssignableFrom(propValue.GetType()) && prop.Name == nameof(AssetsObject.ObjectInfo))
                    {
                        continue;
                    }

                    if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string) || (prop.PropertyType.IsArray && (prop.PropertyType.GetElementType().IsValueType || prop.PropertyType.GetElementType() == typeof(string))))
                    {
                        continue;
                    }
                    var childNode = MakeNode(propValue, depth, trackedObjects);
                    childNode.ParentPropertyName = prop.Name;
                    //stupid place for this
                    if (typeof(IEnumerable).IsAssignableFrom(propValue?.GetType()))
                    {
                        foreach ( Node n in childNode.Nodes)
                        {
                            if (string.IsNullOrWhiteSpace(n.ParentPropertyName))
                                n.ParentPropertyName = prop.Name;
                        }
                    }
                    if (string.IsNullOrEmpty(childNode.Text))
                    {
                        childNode.Text = $"{prop.Name}: {propValue?.GetType()?.Name ?? "((null)"}";
                    }
                    else
                    {
                        childNode.Text = $"{prop.Name}: {childNode.Text}";
                    }
                    node.AddNode(childNode);
                }
            }
            else
            {
                node.Set($"{o}", t.Name);
            }

            return node;
        }
        public Node()
        { }

        public Node(object obj, int depth)
        {
            Obj = obj;
            Depth = depth;
        }
        public void Set(string text, string typeName)
        {
            Text = text;
            TypeName = typeName;
        }
        public string TypeName { get; set; }
        public string Text { get; set; }
        public object Obj { get; set; }
        public Node StubToNode { get; set; }
        public int Depth { get; set; }
        public object ExtRef { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();


        public string ParentPropertyName { get; set; }
        

        public Node Parent { get; set; }
        public void AddNode(Node n)
        {
            this.Nodes.Add(n);
            n.Parent = this;
        }
    }
}
