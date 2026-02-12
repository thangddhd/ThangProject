using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class DataGridViewTimeColumn : DataGridViewColumn
    {
        public DataGridViewTimeColumn()
            : base(new DataGridViewTimeCell())
        {
            this.ValueType = typeof(string);
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }
}
