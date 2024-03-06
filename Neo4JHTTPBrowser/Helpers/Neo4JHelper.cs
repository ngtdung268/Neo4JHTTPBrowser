using Neo4JHttpBrowser.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Neo4JHttpBrowser.Helpers
{
    internal static class Neo4JHelper
    {
        private static readonly string NonCommentsPattern = string.Join("|", new string[]
        {
            // Block comments.
            @"/\*(.*?)\*/",
            // Line comments (double splashes).
            @"//(.*?)\r?\n",
            // Line comments (double hyphens).
            @"--(.*?)\r?\n",
            // Strings.
            @"'((\\[^\n]|[^'\n])*)'",
            // Verbatim strings.
            @"@('[^']*')+"
        });

        public static string RemoveComments(string statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            // Ensure statement is ended with a new line.
            // It's a trick to make "line comment" patterns work.
            statement += Environment.NewLine;

            return Regex
                .Replace(
                    statement,
                    NonCommentsPattern,
                    match =>
                    {
                        if (match.Value.StartsWith("/*") || match.Value.StartsWith("//") || match.Value.StartsWith("--"))
                        {
                            return string.Empty;
                        }
                        return match.Value;
                    },
                    RegexOptions.Singleline
                )
                .Trim();
        }

        public static IEnumerable<Dictionary<string, object>> GetRows(QueryResponseDTO.ResultDTO result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.Plan != null && result.Plan.Any())
            {
                return new List<Dictionary<string, object>>() { result.Plan["root"] };
            }
            else
            {
                var cols = result.Columns ?? new List<string>(0);
                if (cols.Count == 0)
                {
                    return Enumerable.Empty<Dictionary<string, object>>();
                }

                return result
                    .Data?
                    .Select(d =>
                    {
                        if (d.Row.Count != cols.Count)
                        {
                            return null;
                        }

                        var dict = new Dictionary<string, object>();
                        for (var i = 0; i < d.Row.Count; i++)
                        {
                            dict.Add(cols[i], d.Row[i]);
                        }

                        return dict;
                    })
                    .Where(r => r != null)
                    .ToList();
            }
        }

        public static IEnumerable<T> GetRows<T>(QueryResponseDTO.ResultDTO result) where T : class
        {
            var dicts = GetRows(result);
            var json = JsonConvert.SerializeObject(dicts);
            return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
        }

        public static string GetErrorText(QueryResponseDTO response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.Errors == null || !response.Errors.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var error in response.Errors)
            {
                sb.AppendLine(error.Code);
                sb.AppendLine(error.Message.Replace("\n", Environment.NewLine));
                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }

        public static class ReservedKeywords
        {
            public static string[] Clauses = new string[]
            {
                "CALL",
                "CREATE",
                "DELETE",
                "DETACH",
                "FOREACH",
                "LOAD",
                "MATCH",
                "MERGE",
                "OPTIONAL",
                "REMOVE",
                "RETURN",
                "SET",
                "START",
                "UNION",
                "UNWIND",
                "WITH",
            };

            public static string[] SubClauses = new string[]
            {
                "LIMIT",
                "ORDER",
                "SKIP",
                "WHERE",
                "YIELD",
            };

            public static string[] Modifiers = new string[]
            {
                "ASC",
                "ASCENDING",
                "ASSERT",
                "BY",
                "CSV",
                "DESC",
                "DESCENDING",
                "ON",
            };

            public static string[] Expressions = new string[]
            {
                "ALL",
                "CASE",
                "COUNT",
                "ELSE",
                "END",
                "EXISTS",
                "THEN",
                "WHEN",
            };

            public static string[] Operators = new string[]
            {
                "AND",
                "AS",
                "CONTAINS",
                "DISTINCT",
                "ENDS",
                "IN",
                "IS",
                "NOT",
                "OR",
                "STARTS",
                "XOR",
            };

            public static string[] Schema = new string[]
            {
                "CONSTRAINT",
                "CREATE",
                "DROP",
                "EXISTS",
                "INDEX",
                "NODE",
                "KEY",
                "UNIQUE",
            };

            public static string[] Hints = new string[]
            {
                "INDEX",
                "JOIN",
                "SCAN",
                "USING",
            };

            public static string[] Literals = new string[]
            {
                "false",
                "null",
                "true",
            };

            public static string[] FutureUse = new string[]
            {
                "ADD",
                "DO",
                "FOR",
                "MANDATORY",
                "OF",
                "REQUIRE",
                "SCALAR",
            };
        }
    }
}
