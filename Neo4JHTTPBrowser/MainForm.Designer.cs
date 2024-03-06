namespace Neo4JHttpBrowser
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.executeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.objectExplorerTreeView = new System.Windows.Forms.TreeView();
            this.objectExplorerHeaderLabel = new System.Windows.Forms.Label();
            this.queriesTabControl = new Neo4JHTTPBrowser.Controls.QueryTabControl();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.newQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsMenuItem,
            this.newQueryMenuItem,
            this.executeMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(784, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(68, 20);
            this.optionsMenuItem.Text = "&OPTIONS";
            this.optionsMenuItem.Visible = false;
            // 
            // executeMenuItem
            // 
            this.executeMenuItem.Image = global::Neo4JHTTPBrowser.Properties.Resources.Play16;
            this.executeMenuItem.Name = "executeMenuItem";
            this.executeMenuItem.Size = new System.Drawing.Size(82, 20);
            this.executeMenuItem.Text = "&EXECUTE";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(57)))), ((int)(((byte)(85)))));
            this.panel1.Controls.Add(this.splitContainer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(784, 537);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(4, 4);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.queriesTabControl);
            this.splitContainer.Size = new System.Drawing.Size(776, 529);
            this.splitContainer.SplitterDistance = 300;
            this.splitContainer.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(155)))), ((int)(((byte)(188)))));
            this.panel2.Controls.Add(this.objectExplorerTreeView);
            this.panel2.Controls.Add(this.objectExplorerHeaderLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(1);
            this.panel2.Size = new System.Drawing.Size(300, 529);
            this.panel2.TabIndex = 0;
            // 
            // objectExplorerTreeView
            // 
            this.objectExplorerTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(249)))), ((int)(((byte)(254)))));
            this.objectExplorerTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectExplorerTreeView.Location = new System.Drawing.Point(1, 26);
            this.objectExplorerTreeView.Name = "objectExplorerTreeView";
            this.objectExplorerTreeView.Size = new System.Drawing.Size(298, 502);
            this.objectExplorerTreeView.TabIndex = 1;
            // 
            // objectExplorerHeaderLabel
            // 
            this.objectExplorerHeaderLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(96)))), ((int)(((byte)(130)))));
            this.objectExplorerHeaderLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.objectExplorerHeaderLabel.ForeColor = System.Drawing.Color.White;
            this.objectExplorerHeaderLabel.Location = new System.Drawing.Point(1, 1);
            this.objectExplorerHeaderLabel.Name = "objectExplorerHeaderLabel";
            this.objectExplorerHeaderLabel.Size = new System.Drawing.Size(298, 25);
            this.objectExplorerHeaderLabel.TabIndex = 0;
            this.objectExplorerHeaderLabel.Text = "Object Explorer";
            this.objectExplorerHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // queriesTabControl
            // 
            this.queriesTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queriesTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.queriesTabControl.Location = new System.Drawing.Point(0, 0);
            this.queriesTabControl.Name = "queriesTabControl";
            this.queriesTabControl.SelectedIndex = 0;
            this.queriesTabControl.Size = new System.Drawing.Size(472, 529);
            this.queriesTabControl.SupportCloseButton = true;
            this.queriesTabControl.TabIndex = 0;
            // 
            // newQueryMenuItem
            // 
            this.newQueryMenuItem.Image = global::Neo4JHTTPBrowser.Properties.Resources.Add16;
            this.newQueryMenuItem.Name = "newQueryMenuItem";
            this.newQueryMenuItem.Size = new System.Drawing.Size(101, 20);
            this.newQueryMenuItem.Text = "&NEW QUERY";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Neo4J HTTP Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
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

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem optionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newQueryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem executeMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label objectExplorerHeaderLabel;
        private System.Windows.Forms.TreeView objectExplorerTreeView;
        private Neo4JHTTPBrowser.Controls.QueryTabControl queriesTabControl;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

