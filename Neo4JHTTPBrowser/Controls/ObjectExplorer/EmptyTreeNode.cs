namespace Neo4JHTTPBrowser.Controls.ObjectExplorer
{
    internal class EmptyTreeNode : BaseObjectTreeNode
    {
        public new string Text => string.Empty;

        public EmptyTreeNode() : base(null, null)
        {
            base.Text = string.Empty;
        }
    }
}
