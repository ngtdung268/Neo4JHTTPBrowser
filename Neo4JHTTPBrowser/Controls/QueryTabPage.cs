using System;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class QueryTabPage : TabPage
    {
        public QueryTabView View { get; private set; }

        public QueryTabPage()
        {
            Name = $"{nameof(QueryTabPage)}-{Guid.NewGuid()}";

            View = new QueryTabView
            {
                Dock = DockStyle.Fill,
                TabIndex = 0,
            };

            Controls.Add(View);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            View.Focus();
        }
    }
}
