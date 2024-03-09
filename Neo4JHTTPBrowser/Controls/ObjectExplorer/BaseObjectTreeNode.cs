using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls.ObjectExplorer
{
    internal abstract class BaseObjectTreeNode : TreeNode
    {
        public BaseObjectTreeNode(string imageKey)
        {
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
