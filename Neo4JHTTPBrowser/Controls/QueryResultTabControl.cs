using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class QueryResultTabControl : TabControl
    {
        //private readonly TabPage messageTabPage;
        //private readonly TextBox messageTextBox;

        private readonly QueryResultTabPage firstTabPage;
        private readonly List<QueryResultTabPage> otherTabPages;

        public QueryResultTabControl()
        {
            otherTabPages = new List<QueryResultTabPage>();

            if (!DesignMode)
            {
                firstTabPage = new QueryJsonResultTabPage { Text = "Result" };
                TabPages.Add(firstTabPage);
                SelectedTab = firstTabPage;
            }
        }

        public void ShowResult(QueryResponseDTO response)
        {
            if (response.Errors != null && response.Errors.Any())
            {
                firstTabPage.ShowError(Neo4JHelper.GetErrorText(response));
            }
            else if (response.Results != null && response.Results.Any())
            {
                firstTabPage.ShowResult(response.Results[0]);

                if (response.Results.Count > 1)
                {
                    firstTabPage.Text = $"Result 1";

                    for (int i = 1, j = 0; i < response.Results.Count; i++, j++)
                    {
                        QueryResultTabPage page;
                        if (otherTabPages.Count > j)
                        {
                            page = otherTabPages[j];
                        }
                        else
                        {
                            page = new QueryJsonResultTabPage();
                            otherTabPages.Add(page);
                        }

                        page.Text = $"Result {i + 1}";
                        page.ShowResult(response.Results[i]);

                        TabPages.Add(page);
                        SelectedTab = page;
                    }
                }
            }
        }

        public void ResetDisplay()
        {
            firstTabPage.ResetDisplay();
            firstTabPage.Text = "Result";

            foreach (var page in otherTabPages)
            {
                page.ResetDisplay();
                TabPages.Remove(page);
            }
        }
    }
}
