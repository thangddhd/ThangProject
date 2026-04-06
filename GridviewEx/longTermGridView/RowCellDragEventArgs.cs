using System;
using System.Windows.Forms;

namespace coms.COMSK.ui.common
{
    public sealed class RowCellDragEventArgs : EventArgs
    {
        public int RowIndex { get; private set; }
        public string FromColumnName { get; private set; }
        public string ToColumnName { get; private set; }
        public int FromColumnIndex { get; private set; }
        public int ToColumnIndex { get; private set; }

        public RowCellDragEventArgs(int rowIndex, DataGridViewColumn from, DataGridViewColumn to)
        {
            RowIndex = rowIndex;
            FromColumnName = from != null ? from.Name : null;
            ToColumnName = to != null ? to.Name : null;
            FromColumnIndex = from != null ? from.Index : -1;
            ToColumnIndex = to != null ? to.Index : -1;
        }
    }
}