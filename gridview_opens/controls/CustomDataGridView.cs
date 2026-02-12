using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace gridview_opens.controls
{
    public class CustomDataGridView : DataGridView
    {
        private Dictionary<int, List<(int StartRow, int EndRow)>> mergeRanges = new Dictionary<int, List<(int StartRow, int EndRow)>>();
        private BindingSource bindingSource;

        public CustomDataGridView()
        {
            this.DoubleBuffered = true;
            this.AllowUserToOrderColumns = true;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.CellPainting += AdvancedMergeDataGridView_CellPainting;
        }

        #region Merge Cell

        public void AutoMergeColumn(int colIndex)
        {
            if (this.DataSource == null || colIndex < 0 || colIndex >= this.Columns.Count) return;

            mergeRanges[colIndex] = new List<(int StartRow, int EndRow)>();

            int startRow = 0;
            string lastValue = this.Rows[0].Cells[colIndex].Value?.ToString();

            for (int i = 1; i < this.Rows.Count; i++)
            {
                string currentValue = this.Rows[i].Cells[colIndex].Value?.ToString();
                if (currentValue != lastValue)
                {
                    if (i - 1 > startRow)
                        mergeRanges[colIndex].Add((startRow, i - 1));
                    startRow = i;
                    lastValue = currentValue;
                }
            }
            if (this.Rows.Count - 1 > startRow)
                mergeRanges[colIndex].Add((startRow, this.Rows.Count - 1));

            this.Invalidate();
        }

        private void AdvancedMergeDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (!mergeRanges.TryGetValue(e.ColumnIndex, out var ranges)) return;

            var range = ranges.FirstOrDefault(r => e.RowIndex >= r.StartRow && e.RowIndex <= r.EndRow);
            if (range == default) return;

            e.Handled = true;

            int height = 0;
            for (int i = range.StartRow; i <= range.EndRow; i++)
                height += this.Rows[i].Height;

            Rectangle mergeRect = new Rectangle(
                e.CellBounds.X,
                this.GetRowDisplayRectangle(range.StartRow, true).Y,
                e.CellBounds.Width,
                height
            );

            using (Brush backBrush = new SolidBrush(e.CellStyle.BackColor))
                e.Graphics.FillRectangle(backBrush, mergeRect);

            string text = this.Rows[range.StartRow].Cells[e.ColumnIndex].Value?.ToString();
            TextRenderer.DrawText(e.Graphics, text ?? "", e.CellStyle.Font, mergeRect, e.CellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            using (Pen pen = new Pen(this.GridColor))
                e.Graphics.DrawRectangle(pen, mergeRect);
        }

        #endregion

        #region Filter

        public void SetDataSource(object dataSource)
        {
            bindingSource = new BindingSource();
            bindingSource.DataSource = dataSource;
            this.DataSource = bindingSource;
        }

        public void ApplyFilter(string columnName, string filterValue)
        {
            if (bindingSource == null) return;

            if (string.IsNullOrEmpty(filterValue))
                bindingSource.RemoveFilter();
            else
                bindingSource.Filter = $"{columnName} LIKE '%{filterValue.Replace("'", "''")}%'";

            // Recalculate merge after filter
            for (int i = 0; i < this.Columns.Count; i++)
            {
                if (mergeRanges.ContainsKey(i))
                    AutoMergeColumn(i);
            }
        }

        #endregion

        #region Hide/Show Column

        public void HideColumn(int colIndex)
        {
            if (colIndex >= 0 && colIndex < this.Columns.Count)
                this.Columns[colIndex].Visible = false;
        }

        public void ShowColumn(int colIndex)
        {
            if (colIndex >= 0 && colIndex < this.Columns.Count)
                this.Columns[colIndex].Visible = true;
        }

        #endregion
    }
}
