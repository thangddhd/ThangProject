using System;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;

namespace coms.COMMON.ui
{
    public class DataGridViewNumericTextBoxCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(NumericEditingControl);
        public override Type ValueType => typeof(double);
        public override object DefaultNewRowValue => 0;

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
                                                     DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            var ctl = DataGridView.EditingControl as NumericEditingControl;
            var col = OwningColumn as DataGridViewNumericColumn;

            string raw = this.Value?.ToString() ?? "";

            ctl.Text = raw;
            ctl.AllowDecimal = col.AllowDecimal;
        }

        protected override object GetFormattedValue(object value, int rowIndex,
            ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter,
            TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (value == null || value == DBNull.Value)
                return "";

            var col = this.OwningColumn as DataGridViewNumericColumn;
            bool allowDecimal = col?.AllowDecimal ?? false;

            if (!double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double n))
                return "";

            if (!allowDecimal)
            {
                long iv = (long)n;
                return iv.ToString("#,##0", CultureInfo.InvariantCulture);
            }

            return n.ToString("#,##0.##########", CultureInfo.InvariantCulture);
        }
    }
}
