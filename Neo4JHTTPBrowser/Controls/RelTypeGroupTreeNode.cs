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

            if (isOutgoing)
            {
                ImageKey = UIHelper.ObjectExplorerImageKeys.RelTypeOutgoing;
            }
            else
            {
                ImageKey = UIHelper.ObjectExplorerImageKeys.RelTypeIncoming;
            }

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
            Nodes.Clear();

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
                        var relTypeNodes = new List<RelTypeTreeNode>();

                        foreach (var row in rows)
                        {
                            var labels = (row["LABEL"] as JArray).ToObject<IEnumerable<string>>();
                            var type = row["TYPE"] as string;

                            foreach (var label in labels)
                            {
                                if (IsOutgoing)
                                {
                                    relTypeNodes.Add(new RelTypeTreeNode(NodeLabel, label, type));
                                }
                                else
                                {
                                    relTypeNodes.Add(new RelTypeTreeNode(label, NodeLabel, type));
                                }
                            }
                        }

                        relTypeNodes = relTypeNodes.OrderBy(n => n.Text).ToList();

                        Nodes.AddRange(relTypeNodes.ToArray());
                    }
                }
            }
            finally
            {
                base.Text = ToString();
                Expand();
            }
        }
    }
}
