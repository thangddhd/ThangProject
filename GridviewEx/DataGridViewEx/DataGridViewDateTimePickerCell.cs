using System;
using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class DataGridViewDateTimePickerCell : DataGridViewTextBoxCell
    {
        public DataGridViewDateTimePickerCell()
        {
            this.Style.Format = "yyyy/MM/dd";
        }

        public override void InitializeEditingControl(
            int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            //DateTimePickerEditingControl ctl = DataGridView.EditingControl as DateTimePickerEditingControl;
            //var col = this.OwningColumn as DataGridViewDateTimePickerColumn;

            //ctl.CustomFormat = col?.CustomFormat ?? this.CustomFormat;

            //if (this.Value == null || this.Value == DBNull.Value)
            //    ctl.Value = DateTime.Today;
            //else
            //    ctl.Value = Convert.ToDateTime(this.Value);
        }

        public override Type EditType => typeof(DateTimePickerEditingControl);

        public override Type ValueType => typeof(DateTime);

        public override object DefaultNewRowValue => DateTime.Today;

        protected override object GetFormattedValue(object value,
                                                int rowIndex,
                                                ref DataGridViewCellStyle cellStyle,
                                                System.ComponentModel.TypeConverter valueTypeConverter,
                                                System.ComponentModel.TypeConverter formattedValueTypeConverter,
                                                DataGridViewDataErrorContexts context)
        {
            object baseFormattedValue = base.GetFormattedValue(value, rowIndex,
                                                                ref cellStyle,
                                                                valueTypeConverter,
                                                                formattedValueTypeConverter,
                                                                context);

            if (this.DataGridView is DataGridViewEx customGrid)
            {
                if (value == null || value == DBNull.Value)
                {
                    return string.Empty;
                }

                if (value is DateTime dateValue)
                {
                    if (dateValue == DateTime.MinValue || dateValue.Year < 1900)
                    {
                        return string.Empty;
                    }

                    var args = new CustomColumnDisplayTextEventArgs(rowIndex, value)
                    {
                        ColumnName = this.OwningColumn.Name,
                        Column = this.OwningColumn,
                        DisplayText = baseFormattedValue?.ToString() ?? string.Empty
                    };
                    //表示テキストする際にカスタイマイズフォーマットが必要ならばその関数を指定する
                    //例：gvKumiai_CustomColumnDisplayText関数に参考
                    customGrid.OnCustomColumnDisplayText(args);
                    //フォーマットした場合
                    if (args.Handled)
                    {
                        return args.DisplayText;
                    }
                }
            }

            return baseFormattedValue;
        }
    }
}
