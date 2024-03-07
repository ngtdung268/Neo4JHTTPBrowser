using Neo4JHTTPBrowser.DTOs;
using Neo4JHTTPBrowser.Helpers;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    public partial class QueryTabView : UserControl
    {
        private readonly BackgroundWorker executionWorker;

        public string Query
        {
            get
            {
                return queryEditor.Text;
            }
            set
            {
                queryEditor.Text = value;
                queryEditor.SelectionStart = queryEditor.Text.Length;
            }
        }

        public QueryTabView()
        {
            InitializeComponent();

            executionWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            executionWorker.DoWork += ExecutionWorker_DoWork;
            executionWorker.RunWorkerCompleted += ExecutionWorker_Completed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetupQueryEditor();

            ResetExecutionLabels("Ready");
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            queryEditor.Focus();
            queryEditor.ScrollCaret();
        }

        public void ExecuteQuery()
        {
            var inputText = queryEditor.SelectedText.Trim();
            if (inputText.Length == 0)
            {
                inputText = queryEditor.Text.Trim();
            }
            if (inputText.Length == 0)
            {
                return;
            }

            inputText = Neo4JHelper.RemoveComments(inputText);

            var payload = new QueryRequestDTO { Statements = new List<QueryRequestDTO.StatementDTO>() };
            var lines = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var statement = string.Empty;

            foreach (var line in lines)
            {
                statement += line.Trim();

                if (statement.EndsWith(";"))
                {
                    payload.Statements.Add(new QueryRequestDTO.StatementDTO { Statement = statement });
                    statement = string.Empty;
                }
                else
                {
                    statement += Environment.NewLine;
                }
            }

            if (!string.IsNullOrWhiteSpace(statement))
            {
                payload.Statements.Add(new QueryRequestDTO.StatementDTO { Statement = statement });
            }

            if (!executionWorker.IsBusy)
            {
                executionWorker.RunWorkerAsync(payload);
                ResetExecutionLabels("Executing...");
            }
        }

        private void ExecutionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var payload = e.Argument as QueryRequestDTO;

            var stopwatch = Stopwatch.StartNew();
            var response = Neo4JApiService.Instance.QueryAsync(payload).Result;
            var elapsed = stopwatch.Elapsed;

            e.Result = Tuple.Create(response, elapsed);
        }

        private void ExecutionWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            var (response, elapsed) = e.Result as Tuple<QueryResponseDTO, TimeSpan>;

            resultsTabControl.ShowResult(response);

            if (response.Errors != null && response.Errors.Any())
            {
                statusLabel.Text = "Query completed with errors";
                statusLabel.ForeColor = Color.Red;
            }
            else
            {
                statusLabel.Text = "Query executed successfully";
                statusLabel.ForeColor = Color.Green;
            }

            elapsedDelimiterLabel.Text = "|";
            elapsedLabel.Text = elapsed.ToString("g");
        }

        private void ResetExecutionLabels(string message)
        {
            resultsTabControl.ResetDisplay();

            statusLabel.Text = message;
            statusLabel.ForeColor = SystemColors.ControlText;

            elapsedLabel.Text = string.Empty;
            elapsedDelimiterLabel.Text = string.Empty;
        }

        private void SetupQueryEditor()
        {
            queryEditor.SetSelectionBackColor(true, Color.FromArgb(153, 201, 239));

            queryEditor.CaretLineVisible = true;
            queryEditor.CaretLineBackColor = Color.FromArgb(232, 232, 255);

            // Configure the default styles.
            queryEditor.StyleResetDefault();
            queryEditor.Styles[Style.Default].Font = "Consolas";
            queryEditor.Styles[Style.Default].Size = 10;
            queryEditor.Styles[Style.Default].BackColor = Color.White;
            queryEditor.Styles[Style.Default].ForeColor = Color.Black;
            queryEditor.StyleClearAll();

            // Configure the SQL lexer styles.
            queryEditor.Lexer = Lexer.Sql;

            queryEditor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(255, 128, 128, 128);    // Dark Gray
            queryEditor.Styles[Style.LineNumber].BackColor = Color.FromArgb(255, 228, 228, 228);    // Light Gray
            queryEditor.Styles[Style.Sql.Comment].ForeColor = Color.Green;
            queryEditor.Styles[Style.Sql.CommentLine].ForeColor = Color.Green;
            queryEditor.Styles[Style.Sql.CommentLineDoc].ForeColor = Color.Green;
            queryEditor.Styles[Style.Sql.Number].ForeColor = Color.Maroon;
            queryEditor.Styles[Style.Sql.Word].ForeColor = Color.Blue;
            queryEditor.Styles[Style.Sql.Word2].ForeColor = Color.Fuchsia;
            queryEditor.Styles[Style.Sql.User1].ForeColor = Color.Gray;
            queryEditor.Styles[Style.Sql.User2].ForeColor = Color.FromArgb(255, 00, 128, 192);      // Medium Blue-Green
            queryEditor.Styles[Style.Sql.String].ForeColor = Color.Red;
            queryEditor.Styles[Style.Sql.Character].ForeColor = Color.Red;
            queryEditor.Styles[Style.Sql.Operator].ForeColor = Color.Black;

            queryEditor.SetKeywords(0, string.Join(" ", Neo4JHelper.ReservedKeywords.Clauses.Concat(Neo4JHelper.ReservedKeywords.SubClauses).Concat(Neo4JHelper.ReservedKeywords.Schema).Concat(Neo4JHelper.ReservedKeywords.Hints)).ToLowerInvariant());
            queryEditor.SetKeywords(1, string.Join(" ", Neo4JHelper.ReservedKeywords.Modifiers.Concat(Neo4JHelper.ReservedKeywords.Expressions)).ToLowerInvariant());
            queryEditor.SetKeywords(4, string.Join(" ", Neo4JHelper.ReservedKeywords.Operators.Concat(Neo4JHelper.ReservedKeywords.FutureUse)).ToLowerInvariant());
            queryEditor.SetKeywords(5, string.Join(" ", Neo4JHelper.ReservedKeywords.Literals).ToLowerInvariant());

            // Show line numbers.
            var margin = queryEditor.Margins[0];
            margin.Width = 20;
            margin.Type = MarginType.Number;
            margin.Sensitive = true;
        }
    }
}
