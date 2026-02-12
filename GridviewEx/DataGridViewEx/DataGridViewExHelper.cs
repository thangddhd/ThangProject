using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;

namespace coms.COMMON.ui
{
    public static class DataGridViewExHelper
    {
        /// <summary>
        /// ロード時専用
        /// </summary>
        public static void AddInitialEmptyRow<T>(BindingSource bs, Action<T> initializer = null) where T : new()
        {
            if (bs?.List is BindingList<T> bl)
            {
                T newItem = new T();
                initializer?.Invoke(newItem);
                bl.Add(newItem);
            }
        }

        /// <summary>
        /// セル値変更された専用
        /// </summary>
        public static void HandleCellValueChanged<T>(
        DataGridViewEx dgv,
        BindingSource bs,
        int rowIndex,
        int columnIndex,
        Action<T> initializer = null) where T : new()
        {
            if (dgv == null || bs == null) return;
            if (!(bs.List is BindingList<T> bl)) return;

            if (rowIndex == dgv.Rows.Count - 1)
            {
                var cell = dgv.Rows[rowIndex].Cells[columnIndex];
                if (cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    T newItem = new T();
                    initializer?.Invoke(newItem);
                    bl.Add(newItem);
                }
            }
        }

        public static void RemoveItem<T>(BindingSource bs, T item)
        {
            if (bs == null || item == null) return;

            if (bs.List is BindingList<T> bl)
            {
                bl.Remove(item);
                return;
            }

            if (bs.List is IList<T> list)
            {
                list.Remove(item);
                bs.ResetBindings(false);
            }
        }

        public static bool SingleHorizontalBorderAdded(this DataGridView dataGridView)
        {
            return !dataGridView.ColumnHeadersVisible &&
                (dataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single ||
                 dataGridView.CellBorderStyle == DataGridViewCellBorderStyle.SingleHorizontal);
        }

        public static bool SingleVerticalBorderAdded(this DataGridView dataGridView)
        {
            return !dataGridView.RowHeadersVisible &&
                (dataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single ||
                 dataGridView.CellBorderStyle == DataGridViewCellBorderStyle.SingleVertical);
        }

        public static Point GetGridPointFromCellMouseEvent(
            DataGridView grid,
            DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return Point.Empty;

            // Cell rectangle in grid coordinates
            Rectangle cellRect = grid.GetCellDisplayRectangle(
                e.ColumnIndex,
                e.RowIndex,
                false);

            // Mouse point relative to grid
            return new Point(
                cellRect.Left + e.X,
                cellRect.Top + e.Y);
        }

        public static Point GetGridPointFromCustomCellEvent(
            DataGridView grid,
            CustomRowCellClickEventArgs e)
        {
            if (grid == null)
                return Point.Empty;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return Point.Empty;

            // Cell rectangle in grid coordinates
            Rectangle cellRect = grid.GetCellDisplayRectangle(
                e.ColumnIndex,
                e.RowIndex,
                false);

            if (cellRect == Rectangle.Empty)
                return Point.Empty;

            // Mouse point relative to grid
            return new Point(
                cellRect.Left + e.X,
                cellRect.Top + e.Y);
        }
    }
}
