using Neo4JHTTPBrowser.DTOs;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal abstract class QueryResultTabPage : TabPage
    {
        public abstract void ShowResult(QueryResponseDTO.ResultDTO result);

        public abstract void ShowError(string error);

        public abstract void ResetDisplay();
    }
}
