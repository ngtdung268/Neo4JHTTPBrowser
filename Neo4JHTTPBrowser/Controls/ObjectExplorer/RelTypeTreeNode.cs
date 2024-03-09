using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Neo4JHTTPBrowser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls.ObjectExplorer
{
    internal class RelTypeTreeNode : BaseObjectTreeNode
    {
        public string SourceNodeLabel { get; set; }

        public string TargetNodeLabel { get; set; }

        public string RelationshipType { get; set; }

        public new string Text => ToString();

        private readonly List<MenuItem> contextMenuItems;

        public RelTypeTreeNode(WorkItem workItem, string sourceNodeLabel, string targetNodeLabel, string relationshipType) : base(workItem, UIHelper.ObjectExplorerImageKeys.RelType)
        {
            SourceNodeLabel = sourceNodeLabel;
            TargetNodeLabel = targetNodeLabel;
            RelationshipType = relationshipType;
            base.Text = ToString();

            contextMenuItems = new List<MenuItem>
            {
                new MenuItem("Copy", CopyTextItem_Click),
                new MenuItem("View First 20", ViewFirst20Item_Click)
            };
        }

        public override string ToString()
        {
            return GetRelTypeRepr();
        }

        private string GetRelTypeRepr()
        {
            return $"({SourceNodeLabel})-[{RelationshipType}]->({TargetNodeLabel})";
        }

        public override IEnumerable<MenuItem> GetContextMenuItems()
        {
            return contextMenuItems ?? Enumerable.Empty<MenuItem>();
        }

        private void CopyTextItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GetRelTypeRepr());
        }

        private void ViewFirst20Item_Click(object sender, EventArgs e)
        {
            rootWorkItem.EventTopics[CABEventTopics.QueryExecutionRequested].Fire(
                this,
                new CypherQueryEventArgs($"MATCH (s:{SourceNodeLabel})-[r:{RelationshipType}]->(t:{TargetNodeLabel}) RETURN s, t, r LIMIT 20"),
                rootWorkItem,
                PublicationScope.Global
            );
        }

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(GetRelTypeRepr());
            }
        }
    }
}
