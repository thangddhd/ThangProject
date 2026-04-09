using System;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

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
}