using Neo4JHTTPBrowser.DTOs;
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

            base.Text = ToString();
        }

        public override string ToString() => Procedure?.Name;
    }
}
