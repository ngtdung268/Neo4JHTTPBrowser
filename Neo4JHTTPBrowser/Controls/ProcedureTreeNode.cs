using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class ProcedureTreeNode : TreeNode
    {
        public ProcedureDTO Procedure { get; set; }

        public new string Text => ToString();

        public ProcedureTreeNode(ProcedureDTO procedure)
        {
            Procedure = procedure;

            ToolTipText = Procedure?.Description;

            ImageKey = UIHelper.ObjectExplorerImageKeys.Procedure;
            SelectedImageKey = UIHelper.ObjectExplorerImageKeys.Procedure;

            base.Text = ToString();
        }

        public override string ToString() => Procedure?.Name;
    }
}
