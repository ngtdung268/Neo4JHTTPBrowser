using System;

namespace Neo4JHTTPBrowser.Helpers
{
    public class CypherQueryEventArgs : EventArgs
    {
        public string Query { get; private set; }

        public CypherQueryEventArgs(string query)
        {
            Query = query;
        }
    }
}
