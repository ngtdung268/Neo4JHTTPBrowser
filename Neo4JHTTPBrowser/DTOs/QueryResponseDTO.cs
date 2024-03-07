﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Neo4JHTTPBrowser.DTOs
{
    internal class QueryResponseDTO
    {
        [JsonProperty("results")]
        public List<ResultDTO> Results { get; set; }

        [JsonProperty("errors")]
        public List<ErrorDTO> Errors { get; set; }

        [JsonProperty("lastBookmarks")]
        public List<string> LastBookmarks { get; set; }

        internal class ResultDTO
        {
            [JsonProperty("columns")]
            public List<string> Columns { get; set; }

            [JsonProperty("data")]
            public List<ResultDataDTO> Data { get; set; }

            [JsonProperty("plan")]
            public Dictionary<string, Dictionary<string, object>> Plan { get; set; }
        }

        internal class ResultDataDTO
        {
            [JsonProperty("row")]
            public List<object> Row { get; set; }

            [JsonProperty("meta")]
            public List<ResultDataMetaDTO> Meta { get; set; }
        }

        internal class ResultDataMetaDTO
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("elementId")]
            public string ElementId { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("deleted")]
            public bool Deleted { get; set; }
        }

        internal class ErrorDTO
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
