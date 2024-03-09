using Microsoft.Practices.CompositeUI;
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
        private readonly WorkItem rootWorkItem;

        private readonly BackgroundWorker executionWorker;

        private int editorLastCaretPosition = 0;

        public string Query
        {
            get
            {
                return editor.Text;
            }
            set
            {
                editor.Text = value;
                editor.SelectionStart = editor.Text.Length;
            }
        }

        public QueryTabView(WorkItem rootWorkItem)
        {
            this.rootWorkItem = rootWorkItem;

            InitializeComponent();

            executionWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            executionWorker.DoWork += ExecutionWorker_DoWork;
            executionWorker.RunWorkerCompleted += ExecutionWorker_Completed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetupQueryEditorStyle();

            editor.KeyPress += OnEditorKeyPress;
            editor.UpdateUI += OnEditorUpdateUI;

            ResetExecutionLabels("Ready");
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            editor.Focus();
            editor.ScrollCaret();
        }

        public void ExecuteQuery()
        {
            var inputText = editor.SelectedText.Trim();
            if (inputText.Length == 0)
            {
                inputText = editor.Text.Trim();
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
            var response = rootWorkItem.Services.Get<Neo4JApiService>().QueryAsync(payload).Result;
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

        private void SetupQueryEditorStyle()
        {
            editor.SetSelectionBackColor(true, Color.FromArgb(153, 201, 239));

            editor.CaretLineVisible = true;
            editor.CaretLineBackColor = Color.FromArgb(232, 232, 255);

            // Configure the default styles.
            editor.StyleResetDefault();
            editor.Styles[Style.Default].Font = "Consolas";
            editor.Styles[Style.Default].Size = 10;
            editor.Styles[Style.Default].BackColor = Color.White;
            editor.Styles[Style.Default].ForeColor = Color.Black;
            editor.StyleClearAll();

            // Configure the SQL lexer styles.
            editor.Lexer = Lexer.Sql;

            editor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(255, 128, 128, 128);    // Dark Gray
            editor.Styles[Style.LineNumber].BackColor = Color.FromArgb(255, 228, 228, 228);    // Light Gray

            editor.Styles[Style.BraceBad].ForeColor = Color.Red;
            editor.Styles[Style.BraceBad].BackColor = Color.White;
            editor.Styles[Style.BraceLight].ForeColor = Color.Red;
            editor.Styles[Style.BraceLight].BackColor = Color.White;

            editor.Styles[Style.Sql.Comment].ForeColor = Color.Green;
            editor.Styles[Style.Sql.CommentLine].ForeColor = Color.Green;
            editor.Styles[Style.Sql.CommentLineDoc].ForeColor = Color.Green;
            editor.Styles[Style.Sql.Number].ForeColor = Color.Maroon;
            editor.Styles[Style.Sql.Word].ForeColor = Color.Blue;
            editor.Styles[Style.Sql.Word2].ForeColor = Color.Fuchsia;
            editor.Styles[Style.Sql.User1].ForeColor = Color.Gray;
            editor.Styles[Style.Sql.User2].ForeColor = Color.FromArgb(255, 00, 128, 192);      // Medium Blue-Green
            editor.Styles[Style.Sql.String].ForeColor = Color.Red;
            editor.Styles[Style.Sql.Character].ForeColor = Color.Red;
            editor.Styles[Style.Sql.Operator].ForeColor = Color.Black;

            editor.SetKeywords(0, string.Join(" ", Neo4JHelper.ReservedKeywords.Clauses.Concat(Neo4JHelper.ReservedKeywords.SubClauses).Concat(Neo4JHelper.ReservedKeywords.Schema).Concat(Neo4JHelper.ReservedKeywords.Hints)).ToLowerInvariant());
            editor.SetKeywords(1, string.Join(" ", Neo4JHelper.ReservedKeywords.Modifiers.Concat(Neo4JHelper.ReservedKeywords.Expressions)).ToLowerInvariant());
            editor.SetKeywords(4, string.Join(" ", Neo4JHelper.ReservedKeywords.Operators.Concat(Neo4JHelper.ReservedKeywords.FutureUse)).ToLowerInvariant());
            editor.SetKeywords(5, string.Join(" ", Neo4JHelper.ReservedKeywords.Literals).ToLowerInvariant());

            // Show line numbers.
            var margin = editor.Margins[0];
            margin.Width = 20;
            margin.Type = MarginType.Number;
            margin.Sensitive = true;
        }

        private void OnEditorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 32)
            {
                // Prevent control characters from getting inserted into the text buffer.
                e.Handled = true;
                return;
            }
        }

        // https://github.com/jacobslusser/ScintillaNET/wiki/Brace-Matching
        private void OnEditorUpdateUI(object sender, UpdateUIEventArgs e)
        {
            // Has the caret changed position?
            var caretPosition = editor.CurrentPosition;
            if (editorLastCaretPosition != caretPosition)
            {
                editorLastCaretPosition = caretPosition;

                // Is there a brace to the left or right?
                var openBracePosition = -1;
                if (caretPosition > 0 && IsBrace(editor.GetCharAt(caretPosition - 1)))
                {
                    openBracePosition = caretPosition - 1;
                }
                else if (IsBrace(editor.GetCharAt(caretPosition)))
                {
                    openBracePosition = caretPosition;
                }

                if (openBracePosition >= 0)
                {
                    // Find the matching brace.
                    var closeBracePosition = editor.BraceMatch(openBracePosition);
                    if (closeBracePosition == Scintilla.InvalidPosition)
                    {
                        editor.BraceBadLight(openBracePosition);
                    }
                    else
                    {
                        editor.BraceHighlight(openBracePosition, closeBracePosition);
                    }
                }
                else
                {
                    // Turn off brace matching.
                    editor.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                }
            }
        }

        private static bool IsBrace(int character)
        {
            switch (character)
            {
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                    return true;
            }

            return false;
        }
    }
}
