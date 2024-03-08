using Neo4JHTTPBrowser.Helpers;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class NodeLabelTreeNode : TreeNode
    {
        public string Label { get; set; }

        public int Count { get; set; }

        public new string Text => ToString();

        public NodeLabelTreeNode(string label, int count)
        {
            Label = label?.Trim();
            Count = count;

            base.Text = ToString();

            ImageKey = UIHelper.ObjectExplorerImageKeys.NodeLabel;
            SelectedImageKey = UIHelper.ObjectExplorerImageKeys.NodeLabel;

            if (!string.IsNullOrEmpty(Label))
            {
                Nodes.Add(new RelTypeGroupTreeNode(Label, false));
                Nodes.Add(new RelTypeGroupTreeNode(Label, true));
            }
        }

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Label) ? "(blank)" : Label)} ({Count})";
        }
    }
}
