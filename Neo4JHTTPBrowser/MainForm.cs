using Microsoft.Practices.CompositeUI;
using Neo4JHTTPBrowser.Controls;
using Neo4JHTTPBrowser.Controls.ObjectExplorer;
using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using Neo4JHTTPBrowser.Properties;
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
        private readonly WorkItem rootWorkItem;
        private readonly BackgroundWorker objectsLoadingWorker;
        private readonly ContextMenu objectExplorerContextMenu;
        private readonly QueryTabControl queriesTabControl;

        public MainForm([ServiceDependency] WorkItem rootWorkItem)
        {
            this.rootWorkItem = rootWorkItem;

            InitializeComponent();

            objectsLoadingWorker = new BackgroundWorker() { WorkerSupportsCancellation = true };
            objectsLoadingWorker.DoWork += ObjectsLoadingWorker_DoWork;
            objectsLoadingWorker.RunWorkerCompleted += ObjectsLoadingWorker_Completed;

            objectExplorerContextMenu = new ContextMenu();

            newQueryMenuItem.Click += NewQueryMenuItem_Click;
            executeMenuItem.Click += ExecuteMenuItem_Click;

            queriesTabControl = new QueryTabControl
            {
                Dock = DockStyle.Fill,
                DrawMode = TabDrawMode.OwnerDrawFixed,
                SupportCloseButton = true,
            };
            queriesTabControl.SelectedIndexChanged += QueriesTabControl_SelectedIndexChanged;

            objectExplorerTreeView.BeforeExpand += ObjectExplorerTreeView_BeforeExpand;
            objectExplorerTreeView.NodeMouseClick += ObjectExplorerTreeView_NodeMouseClick;
            objectExplorerTreeView.NodeMouseDoubleClick += ObjectExplorerTreeView_NodeMouseDoubleClick;
            objectExplorerTreeView.KeyDown += ObjectExplorerTreeView_KeyDown;

            var objectIcons = new ImageList();
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.Category, Resources.folder_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.NodeLabel, Resources.label_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.Procedure, Resources.script_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.ProcedureMode, Resources.folder_program_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.RelType, Resources.connect_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.RelTypeIncoming, Resources.enter_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.RelTypeOutgoing, Resources.logout_16);
            objectIcons.Images.Add(UIHelper.ObjectExplorerImageKeys.SavedQuery, Resources.code_file_16);
            objectExplorerTreeView.ImageList = objectIcons;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            rootWorkItem.SmartParts.Add(objectExplorerTreeView);
            rootWorkItem.SmartParts.Add(queriesTabControl);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetupConnection();
        }

        private void SetupConnection()
        {
            using (var connDialog = rootWorkItem.Items.AddNew<ConnectionDialog>())
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

            e.Result = rootWorkItem.Services.Get<Neo4JApiService>().QueryAsync(payload).Result;
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

            splitContainer.Panel2.Controls.Add(queriesTabControl);

            if (response.Results != null)
            {
                var firstNodeLevelFont = new Font(DefaultFont, FontStyle.Bold);
                var ObjectExplorerFirstNodeLevelForeColor = Color.Navy;

                var nodeLabelsTreeNode = new CategoryTreeNode(rootWorkItem, UIHelper.ObjectExplorerImageKeys.Category)
                {
                    ForeColor = ObjectExplorerFirstNodeLevelForeColor,
                    NodeFont = firstNodeLevelFont,
                };
                objectExplorerTreeView.Nodes.Add(nodeLabelsTreeNode);
                nodeLabelsTreeNode.Text = "Node Labels";
                AddNodeLabelsToObjectExplorer(nodeLabelsTreeNode, response.Results[0]);

                var proceduresTreeNode = new CategoryTreeNode(rootWorkItem, UIHelper.ObjectExplorerImageKeys.Category)
                {
                    ForeColor = ObjectExplorerFirstNodeLevelForeColor,
                    NodeFont = firstNodeLevelFont,
                };
                objectExplorerTreeView.Nodes.Add(proceduresTreeNode);
                proceduresTreeNode.Text = "Procedures";
                AddProceduresToObjectExplorer(proceduresTreeNode, response.Results[1]);
            }

            MakeReadyToQuery();
        }

        private void AddNodeLabelsToObjectExplorer(TreeNode parentNode, QueryResponseDTO.ResultDTO result)
        {
            var nodes = new List<NodeLabelTreeNode>();

            foreach (var item in Neo4JHelper.GetRows(result))
            {
                nodes.Add(new NodeLabelTreeNode(rootWorkItem, (item["LABELS"] as JArray)?.FirstOrDefault()?.ToString(), Convert.ToInt32(item["COUNT"])));
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
                var modeTreeNode = new CategoryTreeNode(rootWorkItem, UIHelper.ObjectExplorerImageKeys.ProcedureMode)
                {
                    Text = group.Key
                };
                parentNode.Nodes.Add(modeTreeNode);

                foreach (var item in group.OrderBy(i => i.Name))
                {
                    modeTreeNode.Nodes.Add(new ProcedureTreeNode(rootWorkItem, item));
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

        private void AddQueryTabPage(string query = null, bool reuseCurrentBlankTab = false, bool focusTab = true)
        {
            queriesTabControl.AddTabPage(query, reuseCurrentBlankTab: reuseCurrentBlankTab, focusTab: focusTab);
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
                queriesTabControl.Visible = false;
                executeMenuItem.Enabled = false;
            }
            else
            {
                queriesTabControl.Visible = true;
                executeMenuItem.Enabled = true;
                (queriesTabControl.SelectedTab as QueryTabPage).Focus();
            }
        }

        private void ObjectExplorerTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is BaseObjectTreeNode node)
            {
                node.OnBeforeExpand(sender, e);
            }
        }

        private void ObjectExplorerTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            objectExplorerTreeView.SelectedNode = e.Node;

            if (e.Button == MouseButtons.Right && e.Node is BaseObjectTreeNode node)
            {
                var menuItems = node.GetContextMenuItems();
                if (menuItems == null || !menuItems.Any())
                {
                    return;
                }

                objectExplorerContextMenu.MenuItems.Clear();
                objectExplorerContextMenu.MenuItems.AddRange(menuItems.ToArray());
                objectExplorerContextMenu.Show(objectExplorerTreeView, e.Location);
            }
        }

        private void ObjectExplorerTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is BaseObjectTreeNode node)
            {
                node.OnNodeMouseDoubleClick(sender, e);
            }
        }

        private void ObjectExplorerTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (objectExplorerTreeView.SelectedNode != null && objectExplorerTreeView.SelectedNode is BaseObjectTreeNode node)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                node.OnKeyDown(sender, e);
            }
        }
    }
}
