using Newtonsoft.Json;

namespace Neo4JHTTPBrowser.DTOs
{
    internal class ProcedureDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("worksOnSystem")]
        public bool WorksOnSystem { get; set; }
    }
}
