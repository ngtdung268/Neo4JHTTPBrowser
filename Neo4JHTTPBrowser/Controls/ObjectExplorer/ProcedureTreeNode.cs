using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
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
        private readonly List<MenuItem> contextMenuItems;

        public ProcedureDTO Procedure { get; private set; }

        public new string Text => ToString();

        public ProcedureTreeNode(WorkItem workItem, ProcedureDTO procedure) : base(workItem, UIHelper.ObjectExplorerImageKeys.Procedure)
        {
            Procedure = procedure;
            ToolTipText = Procedure.Description;
            base.Text = ToString();

            contextMenuItems = new List<MenuItem>
            {
                new MenuItem("Copy", CopyProcedureItem_Click),
                new MenuItem("Load", LoadProcedureItem_Click),
            };
        }

        public override IEnumerable<MenuItem> GetContextMenuItems()
        {
            return contextMenuItems ?? Enumerable.Empty<MenuItem>();
        }

        private void CopyProcedureItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Procedure.Name);
        }

        private void LoadProcedureItem_Click(object sender, EventArgs e)
        {
            LoadProcedure();
        }

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(Procedure.Name);
            }
        }

        public override void OnNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            LoadProcedure();
        }

        private void LoadProcedure()
        {
            var query = $"-- {Procedure.Description}";
            query += Environment.NewLine;
            query += Environment.NewLine;
            query += $"CALL {Procedure.Name}";
            rootWorkItem.EventTopics[CABEventTopics.QueryDisplayRequested].Fire(this, new CypherQueryEventArgs(query), rootWorkItem, PublicationScope.Global);
        }

        public override string ToString() => Procedure.Name;
    }
}
