using Newtonsoft.Json;
using System.Collections.Generic;

namespace Neo4JHTTPBrowser.DTOs
{
    public class QueryRequestDTO
    {
        [JsonProperty("statements")]
        public List<StatementDTO> Statements { get; set; }

        public class StatementDTO
        {
            [JsonProperty("statement")]
            public string Statement { get; set; }

            [JsonProperty("parameters")]
            public Dictionary<string, object> Parameters { get; set; }
        }
    }
}
