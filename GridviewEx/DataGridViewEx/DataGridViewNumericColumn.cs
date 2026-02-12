using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class DataGridViewNumericColumn : DataGridViewColumn
    {
        public bool AllowDecimal { get; set; } = false;

        public DataGridViewNumericColumn()
            : base(new DataGridViewNumericTextBoxCell())
        {
        }

        public override object Clone()
        {
            var col = (DataGridViewNumericColumn)base.Clone();
            col.AllowDecimal = this.AllowDecimal;
            return col;
        }
    }
}
