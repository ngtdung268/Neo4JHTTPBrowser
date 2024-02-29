namespace Neo4JHTTPBrowser.Controls
{
    partial class QueryTabView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.elapsedDelimiterLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.elapsedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.queryEditor = new ScintillaNET.Scintilla();
            this.resultEditor = new ScintillaNET.Scintilla();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rowsCountLabel = new System.Windows.Forms.Label();
            this.cmbResultDisplayTypes = new System.Windows.Forms.ComboBox();
            this.statusStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.toolStripStatusLabel1,
            this.elapsedDelimiterLabel,
            this.elapsedLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 478);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(500, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 17);
            this.statusLabel.Text = "Ready";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(387, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "          ";
            // 
            // elapsedDelimiterLabel
            // 
            this.elapsedDelimiterLabel.ForeColor = System.Drawing.Color.Silver;
            this.elapsedDelimiterLabel.Name = "elapsedDelimiterLabel";
            this.elapsedDelimiterLabel.Size = new System.Drawing.Size(10, 17);
            this.elapsedDelimiterLabel.Text = "|";
            // 
            // elapsedLabel
            // 
            this.elapsedLabel.Name = "elapsedLabel";
            this.elapsedLabel.Size = new System.Drawing.Size(49, 17);
            this.elapsedLabel.Text = "00:00:00";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(219)))), ((int)(((byte)(233)))));
            this.panel1.Controls.Add(this.splitContainer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 478);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.queryEditor);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.resultEditor);
            this.splitContainer.Panel2.Controls.Add(this.panel2);
            this.splitContainer.Size = new System.Drawing.Size(500, 478);
            this.splitContainer.SplitterDistance = 250;
            this.splitContainer.TabIndex = 2;
            // 
            // queryEditor
            // 
            this.queryEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryEditor.Lexer = ScintillaNET.Lexer.Sql;
            this.queryEditor.Location = new System.Drawing.Point(0, 0);
            this.queryEditor.Name = "queryEditor";
            this.queryEditor.Size = new System.Drawing.Size(500, 250);
            this.queryEditor.TabIndex = 0;
            this.queryEditor.WrapMode = ScintillaNET.WrapMode.Word;
            // 
            // resultEditor
            // 
            this.resultEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultEditor.Lexer = ScintillaNET.Lexer.Json;
            this.resultEditor.Location = new System.Drawing.Point(0, 27);
            this.resultEditor.Name = "resultEditor";
            this.resultEditor.Size = new System.Drawing.Size(500, 197);
            this.resultEditor.TabIndex = 1;
            this.resultEditor.WrapMode = ScintillaNET.WrapMode.Word;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rowsCountLabel);
            this.panel2.Controls.Add(this.cmbResultDisplayTypes);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.panel2.Size = new System.Drawing.Size(500, 27);
            this.panel2.TabIndex = 0;
            // 
            // rowsCountLabel
            // 
            this.rowsCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rowsCountLabel.Location = new System.Drawing.Point(0, 0);
            this.rowsCountLabel.Name = "rowsCountLabel";
            this.rowsCountLabel.Size = new System.Drawing.Size(379, 23);
            this.rowsCountLabel.TabIndex = 1;
            this.rowsCountLabel.Text = "Found 1 row(s).";
            this.rowsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbResultDisplayTypes
            // 
            this.cmbResultDisplayTypes.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmbResultDisplayTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResultDisplayTypes.FormattingEnabled = true;
            this.cmbResultDisplayTypes.Items.AddRange(new object[] {
            "JSON"});
            this.cmbResultDisplayTypes.Location = new System.Drawing.Point(379, 0);
            this.cmbResultDisplayTypes.Name = "cmbResultDisplayTypes";
            this.cmbResultDisplayTypes.Size = new System.Drawing.Size(121, 23);
            this.cmbResultDisplayTypes.TabIndex = 0;
            // 
            // QueryTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "QueryTabView";
            this.Size = new System.Drawing.Size(500, 500);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel elapsedLabel;
        private System.Windows.Forms.ToolStripStatusLabel elapsedDelimiterLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer;
        private ScintillaNET.Scintilla queryEditor;
        private ScintillaNET.Scintilla resultEditor;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmbResultDisplayTypes;
        private System.Windows.Forms.Label rowsCountLabel;
    }
}
