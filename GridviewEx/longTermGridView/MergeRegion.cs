namespace coms.COMSK.ui.common
{
    public sealed class MergeRegion
    {
        public int RowStart { get; set; }
        public int RowSpan { get; set; }
        public int[] ColumnIndexes { get; set; }
        public bool IsCollapsed { get; set; } // 開閉状態
        // グループの
        public bool AllowDrawMergeGroup { get; set; }

        public MergeRegion()
        {
            RowSpan = 1;
            ColumnIndexes = new int[0];
        }

        public int OwnerRow => RowStart;
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