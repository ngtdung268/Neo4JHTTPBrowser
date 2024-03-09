using Microsoft.Practices.CompositeUI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls.ObjectExplorer
{
    internal abstract class BaseObjectTreeNode : TreeNode
    {
        protected WorkItem rootWorkItem;

        public BaseObjectTreeNode(WorkItem workItem, string imageKey)
        {
            rootWorkItem = workItem;

            ImageKey = imageKey;
            SelectedImageKey = imageKey;
        }

        public virtual IEnumerable<MenuItem> GetContextMenuItems()
        {
            return Enumerable.Empty<MenuItem>();
        }

        public virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(Text);
            }
        }
    }
}
