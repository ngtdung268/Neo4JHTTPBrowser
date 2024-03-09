using System;

namespace Neo4JHTTPBrowser.Helpers
{
    public class QueryExecutionEventArgs : EventArgs
    {
        public string Query { get; private set; }

        public QueryExecutionEventArgs(string query)
        {
            Query = query;
        }
    }
}
