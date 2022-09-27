using System.Collections.Generic;

namespace Assistant
{
    public class RazorTreeNode
    {
        private string text;
        private RazorTreeNode parent;
        private List<RazorTreeNode> nodes;
        private object userData;
        private int index; // Index onto the child node array

        public RazorTreeNode(string text)
        {
            this.text = text;
        }
        
        public List<RazorTreeNode> Nodes
        {
            get
            {
                if (nodes is not null) return nodes;
                nodes = new List<RazorTreeNode> { this };
                return nodes;
            }
        }
        
        public object Tag
        {
            get => userData;
            set => userData = value;
        }
        
        public string Text
        {
            get => text ?? "";
            set => text = value;
        }
        
        public RazorTreeNode NextNode
        {
            get
            {
                if (nodes == null || index + 1 >= nodes.Count) return null;
                index++;
                return nodes[index];
            }
        }
        
        public int GetNodeCount(bool includeSubTrees)
        {
            int total = nodes.Count;
            if (includeSubTrees)
            {
                for (int i = 0; i < total; i++)
                {
                    total += nodes[i].GetNodeCount(true);
                }
            }

            return total;
        }
        
        public RazorTreeNode FirstNode
        {
            get
            {
                if (nodes.Count == 0)
                {
                    return null;
                }

                return nodes[0];
            }
        }
        
        public void Remove()
        {
            Remove(true);
        }

        private void Remove(bool notify)
        {
            // unlink our children
            //
            foreach (var n in nodes)
            {
                n.Remove(false);
            }

            // children = null;
            // unlink ourself
            if (notify && parent is not null)
            {
                for (int i = index; i < parent.nodes.Count - 1; ++i)
                {
                    (parent.nodes[i] = parent.nodes[i + 1]).index = i;
                }

                parent.nodes[^1] = null;
                parent = null;
            }
        }
    }
}