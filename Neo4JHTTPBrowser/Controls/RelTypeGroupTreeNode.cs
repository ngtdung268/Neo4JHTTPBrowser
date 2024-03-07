using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class RelTypeGroupTreeNode : TreeNode
    {
        public string NodeLabel { get; set; }

        public bool IsOutgoing { get; set; }

        public new string Text => ToString();

        private bool isRelTypesLoaded = false;
        private readonly BackgroundWorker relTypesLoadingWorker;

        public RelTypeGroupTreeNode(string nodeLabel, bool isOutgoing)
        {
            NodeLabel = nodeLabel;
            IsOutgoing = isOutgoing;

            base.Text = ToString();

            Nodes.Add(new EmptyTreeNode());

            relTypesLoadingWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            relTypesLoadingWorker.DoWork += RelTypesLoadingWorker_DoWork;
            relTypesLoadingWorker.RunWorkerCompleted += RelTypesLoadingWorker_Completed;
        }

        public override string ToString()
        {
            var result = IsOutgoing ? "Outgoing Relationship Types" : "Incoming Relationship Types";
            if (isRelTypesLoaded)
            {
                result += $" ({Nodes.Count})";
            }
            return result;
        }

        public void LoadRelationshipTypes()
        {
            if (isRelTypesLoaded || relTypesLoadingWorker.IsBusy)
            {
                return;
            }

            base.Text = ToString() + " (expanding...)";

            relTypesLoadingWorker.RunWorkerAsync();
        }

        private void RelTypesLoadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var statement = IsOutgoing ? $"MATCH (s:{NodeLabel})-[r]->(n)" : $"MATCH (n)-[r]->(s:{NodeLabel})";
            statement += " RETURN DISTINCT labels(n) AS `LABEL`, type(r) AS `TYPE`";

            var payload = new QueryRequestDTO
            {
                Statements = new List<QueryRequestDTO.StatementDTO>
                {
                    new QueryRequestDTO.StatementDTO
                    {
                        Statement = statement,
                    }
                }
            };

            e.Result = Neo4JApiService.Instance.QueryAsync(payload).Result;
            isRelTypesLoaded = true;
        }

        private void RelTypesLoadingWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Nodes.Clear();

                if (e.Error != null)
                {
                    var cause = ExceptionHelper.GetCause(e.Error);
                    Nodes.Add(cause.Message);
                    return;
                }

                var response = e.Result as QueryResponseDTO;
                if (response.Errors != null && response.Errors.Any())
                {
                    Nodes.Add(Neo4JHelper.GetErrorText(response));
                    return;
                }

                if (response.Results != null && response.Results.Any())
                {
                    var rows = Neo4JHelper.GetRows(response.Results.First());
                    if (rows.Any())
                    {
                        var relationshipTypes = new List<string>();

                        foreach (var row in rows)
                        {
                            var labels = (row["LABEL"] as JArray).ToObject<IEnumerable<string>>();
                            var type = row["TYPE"] as string;

                            foreach (var label in labels)
                            {
                                if (IsOutgoing)
                                {
                                    relationshipTypes.Add($"({NodeLabel})-[{type}]->({label})");
                                }
                                else
                                {
                                    relationshipTypes.Add($"({label})-[{type}]->({NodeLabel})");
                                }
                            }
                        }

                        relationshipTypes.Sort();

                        Nodes.AddRange(relationshipTypes.Select(t => new TreeNode(t)).ToArray());
                    }
                }
            }
            finally
            {
                base.Text = ToString();
            }
        }
    }
}
