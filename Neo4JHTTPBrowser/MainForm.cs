using Neo4JHTTPBrowser.Controls;
using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser
{
    public partial class MainForm : Form
    {
        private readonly BackgroundWorker objectsLoadingWorker;

        public MainForm()
        {
            InitializeComponent();

            objectsLoadingWorker = new BackgroundWorker() { WorkerSupportsCancellation = true };
            objectsLoadingWorker.DoWork += ObjectsLoadingWorker_DoWork;
            objectsLoadingWorker.RunWorkerCompleted += ObjectsLoadingWorker_Completed;

            newQueryMenuItem.Click += NewQueryMenuItem_Click;
            executeMenuItem.Click += ExecuteMenuItem_Click;

            queriesTabControl.SelectedIndexChanged += QueriesTabControl_SelectedIndexChanged;

            objectExplorerTreeView.BeforeExpand += ObjectExplorerTreeView_BeforeExpand;
            objectExplorerTreeView.NodeMouseDoubleClick += ObjectExplorerTreeView_NodeMouseDoubleClick;
            objectExplorerTreeView.KeyDown += ObjectExplorerTreeView_KeyDown;

            var objectIcons = new ImageList();
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.Category, Properties.Resources.folder_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.NodeLabel, Properties.Resources.label_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.Procedure, Properties.Resources.code_file_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.ProcedureMode, Properties.Resources.folder_program_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.RelType, Properties.Resources.connect_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.RelTypeIncoming, Properties.Resources.enter_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.RelTypeOutgoing, Properties.Resources.logout_16);
            objectExplorerTreeView.ImageList = objectIcons;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetupConnection();
        }

        private void SetupConnection()
        {
            using (var connDialog = new ConnectionForm())
            {
                if (connDialog.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            objectExplorerTreeView.Nodes.Add("Loading...");

            objectsLoadingWorker.RunWorkerAsync();
        }

        private void ObjectsLoadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var payload = new QueryRequestDTO
            {
                Statements = new List<QueryRequestDTO.StatementDTO>
                {
                    new QueryRequestDTO.StatementDTO
                    {
                        Statement = "MATCH (n) RETURN DISTINCT labels(n) AS `LABELS`, count(*) AS `COUNT`",
                    },
                    new QueryRequestDTO.StatementDTO
                    {
                        Statement = "SHOW PROCEDURES",
                    },
                }
            };

            e.Result = Neo4JApiService.Instance.QueryAsync(payload).Result;
        }

        private void ObjectsLoadingWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            objectExplorerTreeView.Nodes.Clear();

            if (e.Error != null)
            {
                var cause = ExceptionHelper.GetCause(e.Error);
                MessageBoxHelper.Error(this, cause.Message);

                if (cause is SocketException)
                {
                    SetupConnection();
                }

                return;
            }

            var response = e.Result as QueryResponseDTO;
            if (response.Errors != null && response.Errors.Any())
            {
                MessageBoxHelper.Error(this, Neo4JHelper.GetErrorText(response));
                return;
            }

            if (response.Results != null)
            {
                var firstNodeLevelFont = new Font(DefaultFont, FontStyle.Bold);
                var ObjectExplorerFirstNodeLevelForeColor = Color.Navy;

                var nodeLabelsTreeNode = new TreeNode()
                {
                    ForeColor = ObjectExplorerFirstNodeLevelForeColor,
                    ImageKey = UIHelper.ObjectExplorerImageKeys.Category,
                    NodeFont = firstNodeLevelFont,
                    SelectedImageKey = UIHelper.ObjectExplorerImageKeys.Category,
                };
                objectExplorerTreeView.Nodes.Add(nodeLabelsTreeNode);
                AddNodeLabelsToObjectExplorer(nodeLabelsTreeNode, response.Results[0]);
                nodeLabelsTreeNode.Text = "Node Labels";

                var proceduresTreeNode = new TreeNode()
                {
                    ForeColor = ObjectExplorerFirstNodeLevelForeColor,
                    ImageKey = UIHelper.ObjectExplorerImageKeys.Category,
                    NodeFont = firstNodeLevelFont,
                    SelectedImageKey = UIHelper.ObjectExplorerImageKeys.Category,
                };
                objectExplorerTreeView.Nodes.Add(proceduresTreeNode);
                AddProceduresToObjectExplorer(proceduresTreeNode, response.Results[1]);
                proceduresTreeNode.Text = "Procedures";
            }

            MakeReadyToQuery();
        }

        private void AddNodeLabelsToObjectExplorer(TreeNode parentNode, QueryResponseDTO.ResultDTO result)
        {
            var nodes = new List<NodeLabelTreeNode>();

            foreach (var item in Neo4JHelper.GetRows(result))
            {
                nodes.Add(new NodeLabelTreeNode((item["LABELS"] as JArray)?.FirstOrDefault()?.ToString(), Convert.ToInt32(item["COUNT"])));
            }

            nodes = nodes.OrderBy(n => n.Text).ToList();

            foreach (var node in nodes)
            {
                parentNode.Nodes.Add(node);
            }

            parentNode.Text += $" ({nodes.Count})";
            parentNode.Expand();
        }

        private void AddProceduresToObjectExplorer(TreeNode parentNode, QueryResponseDTO.ResultDTO result)
        {
            var procedures = Neo4JHelper.GetRows<ProcedureDTO>(result);

            foreach (var group in procedures.GroupBy(p => p.Mode).OrderBy(g => g.Key))
            {
                var modeTreeNode = new TreeNode(group.Key)
                {
                    ImageKey = UIHelper.ObjectExplorerImageKeys.ProcedureMode,
                    SelectedImageKey = UIHelper.ObjectExplorerImageKeys.ProcedureMode
                };
                parentNode.Nodes.Add(modeTreeNode);

                foreach (var item in group.OrderBy(i => i.Name))
                {
                    modeTreeNode.Nodes.Add(new ProcedureTreeNode(item));
                }
            }

            parentNode.Expand();
        }

        private void MakeReadyToQuery()
        {
            newQueryMenuItem.Enabled = true;
            executeMenuItem.Enabled = true;
            NewQueryMenuItem_Click(this, EventArgs.Empty);
        }

        private void NewQueryMenuItem_Click(object sender, EventArgs e)
        {
            AddQueryTabPage();
        }

        private void AddQueryTabPage(string query = null)
        {
            queriesTabControl.AddTabPage(query, selectTab: true);
            QueriesTabControl_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void ExecuteMenuItem_Click(object sender, EventArgs e)
        {
            if (queriesTabControl.SelectedTab != null && (queriesTabControl.SelectedTab is QueryTabPage queryTab))
            {
                queryTab.View.ExecuteQuery();
            }
        }

        private void QueriesTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (queriesTabControl.SelectedIndex == -1)
            {
                executeMenuItem.Enabled = false;
            }
            else
            {
                executeMenuItem.Enabled = true;
                (queriesTabControl.SelectedTab as QueryTabPage).Focus();
            }
        }

        private void ObjectExplorerTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is RelTypeGroupTreeNode node)
            {
                node.LoadRelationshipTypes();
            }
        }

        private void ObjectExplorerTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is ProcedureTreeNode node)
            {
                var query = $"-- {node.Procedure.Description}";
                query += Environment.NewLine;
                query += Environment.NewLine;
                query += $"CALL {node.Procedure.Name}";

                if (queriesTabControl.SelectedTab != null && queriesTabControl.SelectedTab is QueryTabPage page && string.IsNullOrWhiteSpace(page.View.Query))
                {
                    page.View.Query = query;
                    page.Focus();
                }
                else
                {
                    AddQueryTabPage(query);
                }
            }
        }

        private void ObjectExplorerTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C && objectExplorerTreeView.SelectedNode != null)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Clipboard.SetText(objectExplorerTreeView.SelectedNode.Text);
            }
        }
    }
}
