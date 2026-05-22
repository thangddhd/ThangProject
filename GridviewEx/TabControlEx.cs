using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace coms.COMMON.ui
{
    public class TabControlEx : TabControl
    {
        private int hoverTabIndex = -1; //current tab when mouse hover
        private Color _selectedTextColor = Color.FromArgb(25, 25, 25); // selected text color: black 
        private Color _indicatorColor = Color.FromArgb(255, 135, 0); // Under line of selectedTab
        private Color _hoverColor = Color.FromArgb(255, 235, 140);   // hover tab backcolor 230, 240, 250;
        private Color _backColor = Color.FromArgb(245, 245, 245);    // normail tab backgroundColor

        public TabControlEx()
        {
            //this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.ItemSize = new Size(60, 18);
            this.SizeMode = TabSizeMode.Normal;
            this.Padding = new Point(6, 3);
            //this.Appearance = TabAppearance.Normal;
            //this.DoubleBuffered = true;
            //this.ResizeRedraw = true;
            //this.Multiline = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int index = -1;
            for (int i = 0; i < TabCount; i++)
            {
                if (GetTabRect(i).Contains(e.Location))
                {
                    index = i;
                    break;
                }
            }
            if (index != hoverTabIndex)
            {
                hoverTabIndex = index;
                Invalidate(); // Vẽ lại
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hoverTabIndex = -1;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. Draw TabControl background
            g.Clear(this.Parent != null ? this.Parent.BackColor : SystemColors.Control);

            // 2. Draw tab Item
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle rect = GetTabRect(i);
                DrawTabItem(g, i, rect);
            }

            // 4. Content Area border
            if (TabCount > 0)
            {
                Rectangle contentRect = this.ClientRectangle;
                // tab header area
                int headerHeight = GetTabRect(0).Bottom;
                using (Pen p = new Pen(Color.FromArgb(220, 220, 220))) //Tab body color: Gainsboro 
                {
                    g.DrawRectangle(p, 0, headerHeight, contentRect.Width - 1, contentRect.Height - headerHeight - 1);
                }
            }
        }

        private void DrawTabItem(Graphics g, int index, Rectangle rect)
        {
            TabPage tabPage = this.TabPages[index];
            bool isSelected = (index == SelectedIndex);
            bool isHover = (index == hoverTabIndex);

            // current Tab color
            Color currentBackColor = _backColor;
            Color currentTextColor = Color.FromArgb(80, 80, 80); //Text color

            if (isSelected)
            {
                currentBackColor = Color.White;
                currentTextColor = _selectedTextColor;
            }
            else if (isHover)
            {
                currentBackColor = _hoverColor;
            }

            using (GraphicsPath path = GetTabPath(rect, 5))
            {
                // Tab background
                using (SolidBrush brush = new SolidBrush(currentBackColor))
                {
                    g.FillPath(brush, path);
                }

                // Tab border color
                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220)))
                {
                    g.DrawPath(pen, path);
                }

                // Indicator under selected tab
                if (isSelected)
                {
                    using (SolidBrush indicatorBrush = new SolidBrush(_indicatorColor))
                    {
                        // Indicator color 3px
                        g.FillRectangle(indicatorBrush, rect.X + 1, rect.Bottom - 3, rect.Width - 1, 3);
                    }

                    // Tab and content conecting color
                    using (Pen p = new Pen(Color.White, 2))
                    {
                        g.DrawLine(p, rect.X + 1, rect.Bottom, rect.X + rect.Width - 1, rect.Bottom);
                    }
                }
            }

            // 3. Text
            TextRenderer.DrawText(g, tabPage.Text, this.Font, rect, currentTextColor,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private GraphicsPath GetTabPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + radius);
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddLine(rect.Left + radius, rect.Top, rect.Right - radius, rect.Top);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);
            return path;
        }
    }
}
