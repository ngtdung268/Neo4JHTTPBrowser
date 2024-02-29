using Neo4JHttpBrowser.DTOs;
using Neo4JHttpBrowser.Helpers;
using Newtonsoft.Json;
using ScintillaNET;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class QueryJsonResultTabPage : QueryResultTabPage
    {
        private readonly Scintilla editor = new Scintilla();

        public QueryJsonResultTabPage()
        {
            SetupResultEditor();

            editor.Dock = DockStyle.Fill;
            Controls.Add(editor);
        }

        public override void ShowResult(QueryResponseDTO.ResultDTO result)
        {
            editor.ReadOnly = false;
            {
                // We only support executing 1 query per editor for now.
                var rows = Neo4JHelper.GetRows(result);
                editor.Text = SerializeResultRows(rows);

                var rowsCount = result.Data?.Count;
                if (rowsCount != null)
                {
                    //rowsCountLabel.Text = $"Found {rowsCount} row(s).";
                }

            }
            editor.ReadOnly = true;
        }

        public override void ShowError(string error)
        {
            editor.ReadOnly = false;
            editor.Text = error;
            editor.ReadOnly = true;
        }

        public override void ResetDisplay()
        {
            // In ReadOnly mode, Scintilla does not allow to set a Text value.
            editor.ReadOnly = false;
            editor.Text = string.Empty;
            editor.ReadOnly = true;
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

        private void SetupResultEditor()
        {
            editor.ReadOnly = true;
            editor.WrapMode = WrapMode.Word;

            editor.CaretLineVisible = true;
            editor.CaretLineBackColor = Color.FromArgb(232, 232, 255);

            // Configure the default styles.
            editor.StyleResetDefault();
            editor.Styles[Style.Default].Font = "Consolas";
            editor.Styles[Style.Default].Size = 10;
            editor.Styles[Style.Default].BackColor = Color.White;
            editor.Styles[Style.Default].ForeColor = Color.Black;
            editor.StyleClearAll();

            // Configure the JSON lexer styles.
            editor.Lexer = Lexer.Json;

            editor.Styles[Style.Json.Default].ForeColor = Color.Silver;
            editor.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            editor.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0);  // Green
            editor.Styles[Style.Json.Number].ForeColor = Color.Olive;
            editor.Styles[Style.Json.PropertyName].ForeColor = Color.Blue;
            editor.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21);     // Red
            editor.Styles[Style.Json.StringEol].BackColor = Color.Pink;
            editor.Styles[Style.Json.Operator].ForeColor = Color.Purple;

            // Enable code folding.
            editor.SetProperty("fold", "1");
            editor.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols.
            editor.Margins[3].Type = MarginType.Symbol;
            editor.Margins[3].Mask = Marker.MaskFolders;
            editor.Margins[3].Sensitive = true;
            editor.Margins[3].Width = 20;
            editor.SetFoldMarginHighlightColor(true, Color.Silver);

            // Set colors for all folding markers.
            for (int i = 25; i < 31; i++)
            {
                editor.Markers[i].SetForeColor(Color.White);
                editor.Markers[i].SetBackColor(Color.FromArgb(41, 57, 85));
            }

            // Configure folding markers with respective symbols.
            editor.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            editor.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            editor.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            editor.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            editor.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            editor.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            editor.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding.
            editor.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
        }
    }
}
