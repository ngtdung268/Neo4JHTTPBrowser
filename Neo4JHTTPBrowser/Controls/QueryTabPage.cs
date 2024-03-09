using Microsoft.Practices.CompositeUI;
using System;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class QueryTabPage : TabPage
    {
        private readonly WorkItem rootWorkItem;

        private const string UnsavedIndicator = "*";

        public QueryTabView View { get; private set; }

        public QueryTabPage(WorkItem rootWorkItem)
        {
            this.rootWorkItem = rootWorkItem;

            Name = $"{nameof(QueryTabPage)}-{Guid.NewGuid()}";

            View = new QueryTabView(rootWorkItem)
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
