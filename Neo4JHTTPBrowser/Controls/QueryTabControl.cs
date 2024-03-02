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

        private const int TextMargin = 2;
        private const int CloseButtonNormalIndex = 0;
        private const int CloseButtonHoverIndex = 1;
        private readonly Dictionary<string, Rectangle> closeButtons = new Dictionary<string, Rectangle>();
        private static readonly Point CloseImageLocation = new Point(20, 5);

        [Browsable(true)]
        [Description("Indicate when this workspace is supported close button."), Category("Appearance")]
        public bool SupportCloseButton { get; set; }

        public QueryTabControl()
        {
            ImageList = new ImageList();
            ImageList.Images.Add(Properties.Resources.Close16);
            ImageList.Images.Add(Properties.Resources.CloseHover16);

            DoubleBuffered = true;
            DrawMode = TabDrawMode.OwnerDrawFixed;
            DrawItem += OnDrawItem;

            MouseClick += OnMouseClick;
            MouseLeave += OnMouseLeave;
            MouseMove += OnMouseMove;
        }

        public QueryTabPage AddTabPage(string query = null, bool selectTab = false)
        {
            currentTabNumber++;

            var newTab = new QueryTabPage
            {
                ImageIndex = 0,
                Name = $"{nameof(QueryTabPage)}-{Guid.NewGuid()}",
                TabIndex = currentTabNumber,
                Text = $"Query {currentTabNumber}",
                UseVisualStyleBackColor = true,
            };

            if (query != null)
            {
                newTab.View.Query = query;
            }

            Controls.Add(newTab);

            if (selectTab)
            {
                SelectedTab = newTab;
            }

            return newTab;
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            var page = TabPages[e.Index] as QueryTabPage;
            var rect = GetTabRect(e.Index);
            var graphics = e.Graphics;

            if (e.Index == SelectedIndex)
            {
                graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            }
            else
            {
                graphics.FillRectangle(new SolidBrush(SystemColors.Control), e.Bounds);
            }

            graphics.DrawString(page.Text, Font, new SolidBrush(ForeColor), rect.X + TextMargin, rect.Y + TextMargin);

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
    }
}
