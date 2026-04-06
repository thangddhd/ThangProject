using System;
using System.Windows.Forms;

namespace coms.COMSK.ui.common
{
    public sealed class RowCellsDragEventArgs : EventArgs
    {
        public int StartRowIndex { get; private set; }
        public int EndRowIndex { get; private set; }

        public string FromColumnName { get; private set; }
        public string ToColumnName { get; private set; }

        public int FromColumnIndex { get; private set; }
        public int ToColumnIndex { get; private set; }

        public RowCellsDragEventArgs(int startRowIndex, int endRowIndex, DataGridViewColumn from, DataGridViewColumn to)
        {
            StartRowIndex = Math.Min(startRowIndex, endRowIndex);
            EndRowIndex = Math.Max(startRowIndex, endRowIndex);

            FromColumnName = from != null ? from.Name : null;
            ToColumnName = to != null ? to.Name : null;
            FromColumnIndex = from != null ? from.Index : -1;
            ToColumnIndex = to != null ? to.Index : -1;
        }
    }
}