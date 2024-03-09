using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Neo4JHTTPBrowser.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Neo4JHTTPBrowser.Controls
{
    internal class QueryTabControl : TabControl
    {
        private int currentTabNumber = 0;

        private const int TextMargin = 3;
        private const int CloseButtonNormalIndex = 0;
        private const int CloseButtonHoverIndex = 1;
        private readonly Dictionary<string, Rectangle> closeButtons = new Dictionary<string, Rectangle>();
        private static readonly Point CloseImageLocation = new Point(20, 5);

        private static readonly Color ActiveTabBackColor = Color.FromArgb(214, 219, 233);

        [Browsable(true)]
        [Description("Indicate when this workspace is supported close button."), Category("Appearance")]
        public bool SupportCloseButton { get; set; }

        [ServiceDependency]
        public WorkItem RootWorkItem { get; set; }

        public QueryTabControl()
        {
            ImageList = new ImageList();
            ImageList.Images.Add(Properties.Resources.close_16);
            ImageList.Images.Add(Properties.Resources.close_hover_16);

            DoubleBuffered = true;
            DrawMode = TabDrawMode.OwnerDrawFixed;
            DrawItem += OnDrawItem;

            MouseClick += OnMouseClick;
            MouseLeave += OnMouseLeave;
            MouseMove += OnMouseMove;
        }

        public QueryTabPage AddTabPage(string query = null, bool runQuery = false, bool reuseCurrentBlankTab = false, bool focusTab = false)
        {
            QueryTabPage tabPage;
            if (reuseCurrentBlankTab && SelectedTab != null && SelectedTab is QueryTabPage queryTabPage && string.IsNullOrWhiteSpace(queryTabPage.View.Query))
            {
                tabPage = queryTabPage;
            }
            else
            {
                currentTabNumber++;

                tabPage = new QueryTabPage(RootWorkItem)
                {
                    ImageIndex = 0,
                    Name = $"{nameof(QueryTabPage)}-{Guid.NewGuid()}",
                    TabIndex = currentTabNumber,
                    Text = $"Query {currentTabNumber}",
                    UseVisualStyleBackColor = true,
                };

                Controls.Add(tabPage);
            }

            if (query != null)
            {
                tabPage.View.Query = query;
            }

            if (focusTab)
            {
                SelectedTab = tabPage;
            }

            if (runQuery)
            {
                tabPage.View.ExecuteQuery();
            }

            return tabPage;
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            var page = TabPages[e.Index] as QueryTabPage;
            var rect = GetTabRect(e.Index);
            var graphics = e.Graphics;

            if (e.Index == SelectedIndex)
            {
                using (var brush = new SolidBrush(ActiveTabBackColor))
                {
                    graphics.FillRectangle(brush, e.Bounds);
                }
            }
            else
            {
                using (var brush = new SolidBrush(SystemColors.Control))
                {
                    graphics.FillRectangle(brush, e.Bounds);
                }
            }

            using (var brush = new SolidBrush(ForeColor))
            {
                TextRenderer.DrawText(graphics, page.Text, Font, new Point(rect.X + TextMargin, rect.Y + TextMargin), ForeColor);
            }

            if (SupportCloseButton)
            {
                var closeImage = ImageList.Images[page.ImageIndex];
                var closeImageRect = new Rectangle(new Point(rect.X + (rect.Width - CloseImageLocation.X), CloseImageLocation.Y), closeImage.Size);
                graphics.DrawImage(closeImage, closeImageRect);

                if (!closeButtons.ContainsKey(page.Name))
                {
                    closeButtons.Add(page.Name, closeImageRect);
                }
                else
                {
                    closeButtons[page.Name] = closeImageRect;
                }
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            var page = GetPageByPoint(e.Location);

            if (e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && closeButtons.ContainsKey(page.Name) && closeButtons[page.Name].Contains(e.Location)))
            {
                if (page == null)
                {
                    return;
                }
                if (!closeButtons.ContainsKey(page.Name))
                {
                    return;
                }

                TabPages.Remove(page);
                closeButtons.Remove(page.Name);
            }
        }

        private TabPage GetPageByPoint(Point point)
        {
            for (var i = 0; i < TabPages.Count; i++)
            {
                var page = TabPages[i];

                if (GetTabRect(i).Contains(point))
                {
                    return page;
                }
            }
            return null;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            foreach (TabPage page in TabPages)
            {
                if (page.ImageIndex != CloseButtonNormalIndex)
                {
                    page.ImageIndex = CloseButtonNormalIndex;
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
            {
                return;
            }

            if (TabPages.Count > 0)
            {
                foreach (var kvp in closeButtons)
                {
                    var page = TabPages[kvp.Key];
                    if (page == null)
                    {
                        continue;
                    }

                    if (kvp.Value.Contains(e.Location))
                    {
                        if (page.ImageIndex != CloseButtonHoverIndex)
                        {
                            page.ImageIndex = CloseButtonHoverIndex;
                            return;
                        }
                    }
                    else if (page.ImageIndex != CloseButtonNormalIndex)
                    {
                        page.ImageIndex = CloseButtonNormalIndex;
                        return;
                    }
                }
            }
        }

        [EventSubscription(CABEventTopics.QueryExecutionRequested)]
        public void QueryExecutionRequestedHandler(object sender, EventArgs e)
        {
            if (e is QueryExecutionEventArgs qe)
            {
                AddTabPage(qe.Query, runQuery: true, reuseCurrentBlankTab: true, focusTab: true);
            }
        }
    }
}
