using System;
using System.Drawing;
using System.Windows.Forms;

namespace GridviewEx
{
    public class LongRepairGridView : DataGridView
    {
        public int HighlightedRowIndex { get; private set; } = -1;

        public Color RowHighlightTopBorderColor { get; set; } = Color.Red;
        public Color RowHighlightBottomBorderColor { get; set; } = Color.Red;

        private int _rowHighlightBorderThickness = 2;
        public int RowHighlightBorderThickness
        {
            get => _rowHighlightBorderThickness;
            set => _rowHighlightBorderThickness = Math.Max(1, value);
        }

        public event EventHandler HighlightedRowChanged;

        public LongRepairGridView()
        {
            // Recommended defaults for your UI
            MultiSelect = false;
            EnableHeadersVisualStyles = false;

            CellMouseDown += OnCellMouseDown_SetHighlight;
            CellFormatting += OnCellFormatting_NeutralSelection;
            RowPostPaint += OnRowPostPaint_DrawRowBorders;
            EditingControlShowing += OnEditingControlShowing_WhiteEditor;
        }

        public void SetHighlightedRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowCount) return;
            if (HighlightedRowIndex == rowIndex) return;

            int old = HighlightedRowIndex;
            HighlightedRowIndex = rowIndex;

            if (old >= 0 && old < RowCount) InvalidateRow(old);
            InvalidateRow(HighlightedRowIndex);

            HighlightedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCellMouseDown_SetHighlight(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            SetHighlightedRow(e.RowIndex);
        }

        private void OnCellFormatting_NeutralSelection(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Your existing styling rules (row green, custom cell colors, etc.) should run BEFORE this.
            // Then force selection to be neutral:
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
        }

        private void OnEditingControlShowing_WhiteEditor(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Force white background for editing cell, like your screenshot.
            e.Control.BackColor = Color.White;

            // Optional: keep text readable
            // e.Control.ForeColor = Color.Black;
        }

        private void OnRowPostPaint_DrawRowBorders(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex != HighlightedRowIndex) return;

            // Get visible row rectangle in display coordinates
            Rectangle rowRect = GetRowDisplayRectangle(e.RowIndex, true);
            if (rowRect.Width <= 0 || rowRect.Height <= 0) return;

            int thickness = RowHighlightBorderThickness;

            // Span the full visible grid width
            int left = 0;
            int right = ClientSize.Width - 1;

            // If you want the border to start after row headers, use:
            // int left = RowHeadersVisible ? RowHeadersWidth : 0;

            int topY = rowRect.Top;
            int bottomY = rowRect.Bottom - thickness;

            using (var topBrush = new SolidBrush(RowHighlightTopBorderColor))
            using (var bottomBrush = new SolidBrush(RowHighlightBottomBorderColor))
            {
                e.Graphics.FillRectangle(topBrush, left, topY, right - left, thickness);
                e.Graphics.FillRectangle(bottomBrush, left, bottomY, right - left, thickness);
            }
        }
    }
}
