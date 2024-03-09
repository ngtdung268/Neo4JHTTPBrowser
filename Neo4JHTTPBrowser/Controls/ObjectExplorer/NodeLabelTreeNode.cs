using Neo4JHTTPBrowser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls.ObjectExplorer
{
    internal class NodeLabelTreeNode : BaseObjectTreeNode
    {
        public string Label { get; set; }

        public int Count { get; set; }

        public new string Text => ToString();

        private readonly RelTypeGroupTreeNode incomingRelTypeGroupNode;
        private readonly RelTypeGroupTreeNode outgoingRelTypeGroupNode;
        private readonly List<MenuItem> contextMenuItems;

        public NodeLabelTreeNode(string label, int count) : base(UIHelper.ObjectExplorerImageKeys.NodeLabel)
        {
            Label = label?.Trim();
            Count = count;
            base.Text = ToString();

            if (!string.IsNullOrEmpty(Label))
            {
                incomingRelTypeGroupNode = new RelTypeGroupTreeNode(Label, false);
                Nodes.Add(incomingRelTypeGroupNode);

                outgoingRelTypeGroupNode = new RelTypeGroupTreeNode(Label, true);
                Nodes.Add(outgoingRelTypeGroupNode);

                contextMenuItems = new List<MenuItem>
                {
                    new MenuItem("Copy", CopyTextItem_Click),
                    new MenuItem("Refresh", RefreshItem_Click),
                    new MenuItem("View First 100", ViewFirst100Item_Click)
                };
            }
        }

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Label) ? "(blank)" : Label)} ({Count})";
        }

        public override IEnumerable<MenuItem> GetContextMenuItems()
        {
            return contextMenuItems ?? Enumerable.Empty<MenuItem>();
        }

        private void CopyTextItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Label);
        }

        private void RefreshItem_Click(object sender, EventArgs e)
        {
            incomingRelTypeGroupNode?.LoadRelationshipTypes(reload: true);
            outgoingRelTypeGroupNode?.LoadRelationshipTypes(reload: true);
        }

        private void ViewFirst100Item_Click(object sender, EventArgs e)
        {
            // TODO: Add CAB?
        }

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(Label);
            }
        }
    }
}
