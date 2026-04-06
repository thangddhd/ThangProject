using System;

namespace coms.COMSK.ui.common
{
    internal sealed class MergeRegion
    {
        public int RowStart { get; set; }
        public int RowSpan { get; set; }
        public int[] ColumnIndexes { get; set; }

        public MergeRegion()
        {
            RowSpan = 1;
            ColumnIndexes = new int[0];
        }

        public int OwnerRow
        {
            get { return RowStart; }
        }

        public int OwnerCol
        {
            get
            {
                return (ColumnIndexes != null && ColumnIndexes.Length > 0)
                    ? ColumnIndexes[0]
                    : -1;
            }
        }
    }
}