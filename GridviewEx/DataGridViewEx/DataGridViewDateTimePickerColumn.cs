using System;
using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class DataGridViewDateTimePickerColumn : DataGridViewColumn
    {
        public DataGridViewDateTimePickerColumn()
            : base(new DataGridViewDateTimePickerCell())
        {
            this.ValueType = typeof(DateTime);
        }

        //public DataGridViewDateTimePickerColumn() : base()
        //{
        //    this.CellTemplate = new DataGridViewDateTimePickerCell();
        //    this.ValueType = typeof(DateTime);
        //}
    }
}
