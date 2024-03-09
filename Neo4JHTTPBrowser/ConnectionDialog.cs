using Neo4JHTTPBrowser.Helpers;
using Neo4JHTTPBrowser.Properties;
using System;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser
{
    public partial class ConnectionDialog : Form
    {
        public string Url { get; private set; }

        public ConnectionDialog()
        {
            InitializeComponent();

            addBtn.Click += AddBtn_Click;
            removeBtn.Click += RemoveBtn_Click;
            connectBtn.Click += ConnectBtn_Click;

            savedUrlsListView.MouseDoubleClick += SavedUrlsListView_MouseDoubleClick;
            savedUrlsListView.SelectedIndexChanged += SavedUrlsListView_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadSavedUrls();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (savedUrlsListView.Items.Count != 0)
            {
                savedUrlsListView.Focus();
            }
            else
            {
                urlTextBox.Focus();
            }
        }

        private void LoadSavedUrls()
        {
            foreach (var url in Settings.Default.Neo4JSavedUrls)
            {
                var item = new ListViewItem
                {
                    Selected = url == Settings.Default.Neo4JBaseUrl,
                    Text = url,
                };
                savedUrlsListView.Items.Add(item);
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            var url = urlTextBox.Text.Trim();
            if (url.Length == 0)
            {
                MessageBoxHelper.Error(this, "Pleave provide the URL.");
                urlTextBox.Focus();
                return;
            }
            if (!ValidateUrl(url))
            {
                MessageBoxHelper.Error(this, "The URL is not valid.");
                urlTextBox.SelectAll();
                urlTextBox.Focus();
                return;
            }
            if (Settings.Default.Neo4JSavedUrls.Contains(url))
            {
                MessageBoxHelper.Error(this, "The URL has already been added.");
                urlTextBox.SelectAll();
                urlTextBox.Focus();
                return;
            }

            var item = new ListViewItem
            {
                Selected = true,
                Text = url,
            };

            urlTextBox.Text = string.Empty;

            savedUrlsListView.SelectedItems.Clear();
            savedUrlsListView.Items.Add(item);
            savedUrlsListView.Focus();

            Settings.Default.Neo4JSavedUrls.Add(url);
            Settings.Default.Save();
        }

        private static bool ValidateUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            if (savedUrlsListView.SelectedItems.Count == 1)
            {
                var item = savedUrlsListView.SelectedItems[0];

                Settings.Default.Neo4JSavedUrls.Remove(item.Text);
                Settings.Default.Save();

                savedUrlsListView.SelectedItems.Clear();

                if (item.Index != 0)
                {
                    savedUrlsListView.Items[item.Index - 1].Selected = true;
                    savedUrlsListView.Focus();
                }
                else if (savedUrlsListView.Items.Count > item.Index + 1)
                {
                    savedUrlsListView.Items[item.Index + 1].Selected = true;
                    savedUrlsListView.Focus();
                }
                else
                {
                    urlTextBox.Focus();
                }

                savedUrlsListView.Items.Remove(item);
            }
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (savedUrlsListView.SelectedItems.Count != 1)
            {
                return;
            }

            var selectedUrl = savedUrlsListView.SelectedItems[0].Text;
            if (selectedUrl != Settings.Default.Neo4JBaseUrl)
            {
                Settings.Default.Neo4JBaseUrl = selectedUrl;
                Settings.Default.Save();
            }

            DialogResult = DialogResult.OK;

            Hide();
        }

        private void SavedUrlsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ConnectBtn_Click(this, EventArgs.Empty);
        }

        private void SavedUrlsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeBtn.Enabled = connectBtn.Enabled = savedUrlsListView.SelectedIndices.Count == 1;
        }
    }
}