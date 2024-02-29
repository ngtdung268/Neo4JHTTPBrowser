using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Helpers
{
    internal static class MessageBoxHelper
    {
        public static DialogResult Info(IWin32Window parent, string message)
        {
            if (parent == null)
            {
                return MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else
            {
                return MessageBox.Show(parent, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        }

        public static DialogResult Error(IWin32Window parent, string message)
        {
            if (parent == null)
            {
                return MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                return MessageBox.Show(parent, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        public static DialogResult Warn(IWin32Window parent, string message)
        {
            if (parent == null)
            {
                return MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            else
            {
                return MessageBox.Show(parent, message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        public static DialogResult Confirm(IWin32Window parent, string message)
        {
            if (parent == null)
            {
                return MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }
            else
            {
                return MessageBox.Show(parent, message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }
        }
    }
}
