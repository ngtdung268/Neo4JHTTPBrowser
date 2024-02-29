using Neo4JHttpBrowser;
using Neo4JHttpBrowser.DTOs;
using Neo4JHttpBrowser.Helpers;
using Newtonsoft.Json;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

            SetupResultEditor();

            cmbResultDisplayTypes.SelectedIndex = 0;

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
            var statement = queryEditor.Text.Trim();
            if (statement.Length == 0)
            {
                return;
            }

            statement = Neo4JHelper.ExcludeComments(statement);

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

            resultEditor.ReadOnly = false;
            {
                if (response.Errors != null && response.Errors.Any())
                {
                    resultEditor.Text = Neo4JHelper.GetErrorText(response);
                    statusLabel.Text = "Query completed with errors";
                    statusLabel.ForeColor = Color.Red;
                }
                else if (response.Results != null && response.Results.Any())
                {
                    // We only support executing 1 query per editor for now.
                    var rows = Neo4JHelper.GetRows(response.Results.First());
                    resultEditor.Text = SerializeResultRows(rows);

                    var rowsCount = response.Results?.FirstOrDefault().Data?.Count;
                    if (rowsCount != null)
                    {
                        rowsCountLabel.Text = $"Found {rowsCount} row(s).";
                    }

                    statusLabel.Text = "Query executed successfully";
                    statusLabel.ForeColor = Color.Green;
                }
            }
            resultEditor.ReadOnly = true;

            elapsedDelimiterLabel.Text = "|";
            elapsedLabel.Text = elapsed.ToString("g");
        }

        private void ResetExecutionLabels(string message)
        {
            // In ReadOnly mode, Scintilla does not allow to set a Text value.
            resultEditor.ReadOnly = false;
            resultEditor.Text = string.Empty;
            resultEditor.ReadOnly = true;

            statusLabel.Text = message;
            statusLabel.ForeColor = SystemColors.ControlText;

            rowsCountLabel.Text = string.Empty;

            elapsedLabel.Text = string.Empty;
            elapsedDelimiterLabel.Text = string.Empty;
        }

        private static string SerializeResultRows(IEnumerable<Dictionary<string, object>> rows)
        {
            var sb = new StringBuilder(256);
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            var serializer = JsonSerializer.CreateDefault();

            using (var writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = ' ';
                writer.Indentation = 4;
                serializer.Serialize(writer, rows);
            }

            return sw.ToString();
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

        private void SetupResultEditor()
        {
            resultEditor.CaretLineVisible = true;
            resultEditor.CaretLineBackColor = Color.FromArgb(232, 232, 255);

            // Configure the default styles.
            resultEditor.StyleResetDefault();
            resultEditor.Styles[Style.Default].Font = "Consolas";
            resultEditor.Styles[Style.Default].Size = 10;
            resultEditor.Styles[Style.Default].BackColor = Color.White;
            resultEditor.Styles[Style.Default].ForeColor = Color.Black;
            resultEditor.StyleClearAll();

            // Configure the JSON lexer styles.
            resultEditor.Lexer = Lexer.Json;

            resultEditor.Styles[Style.Json.Default].ForeColor = Color.Silver;
            resultEditor.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            resultEditor.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0);  // Green
            resultEditor.Styles[Style.Json.Number].ForeColor = Color.Olive;
            resultEditor.Styles[Style.Json.PropertyName].ForeColor = Color.Blue;
            resultEditor.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21);     // Red
            resultEditor.Styles[Style.Json.StringEol].BackColor = Color.Pink;
            resultEditor.Styles[Style.Json.Operator].ForeColor = Color.Purple;

            // Enable code folding.
            resultEditor.SetProperty("fold", "1");
            resultEditor.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols.
            resultEditor.Margins[3].Type = MarginType.Symbol;
            resultEditor.Margins[3].Mask = Marker.MaskFolders;
            resultEditor.Margins[3].Sensitive = true;
            resultEditor.Margins[3].Width = 20;
            resultEditor.SetFoldMarginHighlightColor(true, Color.Silver);

            // Set colors for all folding markers.
            for (int i = 25; i < 31; i++)
            {
                resultEditor.Markers[i].SetForeColor(Color.White);
                resultEditor.Markers[i].SetBackColor(Color.FromArgb(41, 57, 85));
            }

            // Configure folding markers with respective symbols.
            resultEditor.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            resultEditor.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            resultEditor.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            resultEditor.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            resultEditor.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            resultEditor.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            resultEditor.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding.
            resultEditor.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
        }
    }
}
