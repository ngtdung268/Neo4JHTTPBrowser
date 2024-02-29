using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class EmptyTreeNode : TreeNode
    {
        public new string Text => string.Empty;

        public EmptyTreeNode()
        {
            base.Text = string.Empty;
        }
    }
}
