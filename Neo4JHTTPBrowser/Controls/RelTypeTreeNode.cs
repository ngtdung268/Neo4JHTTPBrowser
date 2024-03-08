using Neo4JHTTPBrowser.Helpers;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class RelTypeTreeNode : TreeNode
    {
        public string SourceNodeLabel { get; set; }

        public string TargetNodeLabel { get; set; }

        public string RelationshipType { get; set; }

        public new string Text => ToString();

        public RelTypeTreeNode(string sourceNodeLabel, string targetNodeLabel, string relationshipType)
        {
            SourceNodeLabel = sourceNodeLabel;
            TargetNodeLabel = targetNodeLabel;
            RelationshipType = relationshipType;

            ImageKey = UIHelper.ObjectExplorerImageKeys.RelType;
            SelectedImageKey = UIHelper.ObjectExplorerImageKeys.RelType;

            base.Text = ToString();
        }

        public override string ToString()
        {
            return $"({SourceNodeLabel})-[{RelationshipType}]->({TargetNodeLabel})";
        }
    }
}
