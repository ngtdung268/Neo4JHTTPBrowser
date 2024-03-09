using Microsoft.Practices.CompositeUI;
using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls.ObjectExplorer
{
    internal class ProcedureTreeNode : BaseObjectTreeNode
    {
        public ProcedureDTO Procedure { get; set; }

        public new string Text => ToString();

        private readonly List<MenuItem> contextMenuItems;

        public ProcedureTreeNode(WorkItem workItem, ProcedureDTO procedure) : base(workItem, UIHelper.ObjectExplorerImageKeys.Procedure)
        {
            Procedure = procedure;
            ToolTipText = Procedure?.Description;
            base.Text = ToString();

            contextMenuItems = new List<MenuItem>
            {
                new MenuItem("Copy", CopyTextItem_Click),
                new MenuItem("Run", RunItem_Click),
            };
        }

        public override string ToString() => Procedure?.Name;

        public override IEnumerable<MenuItem> GetContextMenuItems()
        {
            return contextMenuItems ?? Enumerable.Empty<MenuItem>();
        }

        private void CopyTextItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Procedure?.Name);
        }

        private void RunItem_Click(object sender, EventArgs e)
        {
        }

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(Procedure?.Name);
            }
        }
    }
}
