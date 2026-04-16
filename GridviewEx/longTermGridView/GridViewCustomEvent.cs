using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

namespace coms.COMMON.ui
{
    public sealed class RowCellsDragEventArgs : EventArgs
    {
        public int StartRowIndex { get; private set; }
        public int EndRowIndex { get; private set; }

        public string FromColumnName { get; private set; }
        public string ToColumnName { get; private set; }

        public int FromColumnIndex { get; private set; }
        public int ToColumnIndex { get; private set; }

        public IReadOnlyList<object> DataList { get; private set; }

        public RowCellsDragEventArgs(int startRowIndex, int endRowIndex, DataGridViewColumn from, DataGridViewColumn to, IList dataList)
        {
            StartRowIndex = Math.Min(startRowIndex, endRowIndex);
            EndRowIndex = Math.Max(startRowIndex, endRowIndex);

            FromColumnName = from != null ? from.Name : null;
            ToColumnName = to != null ? to.Name : null;
            FromColumnIndex = from != null ? from.Index : -1;
            ToColumnIndex = to != null ? to.Index : -1;

            var list = new List<object>();
            if (dataList != null)
            {
                foreach (var x in dataList) list.Add(x);
            }
            DataList = list;
        }
    }

    public sealed class ReserveCellReadOnlyNeededEventArgs : EventArgs
    {
        public ReserveCellReadOnlyNeededEventArgs(int rowIndex, int columnIndex, object rowData)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }

        public bool? ReadOnly { get; set; }
    }

    public sealed class ReserveCellBeginEditEventArgs : EventArgs
    {
        public ReserveCellBeginEditEventArgs(int rowIndex, int columnIndex, object rowData)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }

        public bool Cancel { get; set; }
    }

    public sealed class ReserveEditingControlShowingEventArgs : EventArgs
    {
        public ReserveEditingControlShowingEventArgs(
            int rowIndex,
            int columnIndex,
            object rowData,
            Control editingControl,
            TextBox textBox)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
            EditingControl = editingControl;
            TextBox = textBox;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }

        public Control EditingControl { get; }
        public TextBox TextBox { get; }
    }

    public sealed class ReserveCellDisplayTextNeededEventArgs : EventArgs
    {
        public ReserveCellDisplayTextNeededEventArgs(
            int rowIndex,
            int columnIndex,
            object rowData,
            object value,
            bool isCurrentCell,
            bool isReadOnly)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
            Value = value;
            IsCurrentCell = isCurrentCell;
            IsReadOnly = isReadOnly;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }
        public object Value { get; }
        public bool IsCurrentCell { get; }
        public bool IsReadOnly { get; }

        public string DisplayText { get; set; }
    }

    public sealed class ReserveCellStyleNeededEventArgs : EventArgs
    {
        public ReserveCellStyleNeededEventArgs(
            int rowIndex,
            int columnIndex,
            object rowData,
            object value,
            bool isCurrentCell,
            bool isReadOnly)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
            Value = value;
            IsCurrentCell = isCurrentCell;
            IsReadOnly = isReadOnly;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }
        public object Value { get; }
        public bool IsCurrentCell { get; }
        public bool IsReadOnly { get; }

        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
    }

    public sealed class ReserveButtonCellStyleNeededEventArgs : EventArgs
    {
        public ReserveButtonCellStyleNeededEventArgs(
            int rowIndex,
            int columnIndex,
            object rowData,
            object value,
            bool isCurrentCell,
            bool isReadOnly,
            DataGridViewButtonColumn buttonColumn)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
            Value = value;
            IsCurrentCell = isCurrentCell;
            IsReadOnly = isReadOnly;
            ButtonColumn = buttonColumn;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }
        public object Value { get; }
        public bool IsCurrentCell { get; }
        public bool IsReadOnly { get; }
        public DataGridViewButtonColumn ButtonColumn { get; }

        // What parent can control
        public bool? Visible { get; set; }          // false => visually hide (blank)
        public string Text { get; set; }            // optional override caption
        public Color? BackColor { get; set; }       // optional
        public Color? ForeColor { get; set; }       // optional
        public bool DisabledStyle { get; set; }     // visual only (click still fires)
    }
}
