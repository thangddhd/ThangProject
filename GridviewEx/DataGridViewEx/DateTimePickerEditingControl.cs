using System;
using System.Windows.Forms;
using System.Drawing;

namespace coms.COMMON.ui
{
    public class DateTimePickerEditingControl
        : DateTimePicker2, IDataGridViewEditingControl
    {
        DataGridView _dataGridView;
        private bool _valueChanged = false;
        int rowIndex;

        public DateTimePickerEditingControl()
        {
            this.Format = DateTimePickerFormat.Custom;
            this.CustomFormat = "yyyy/MM/dd";
        }

        #region IDataGridViewEditingControl
        public object EditingControlFormattedValue
        {
            get {
                if (IsNull) return string.Empty; 
                return this.Value.ToString(this.DisplayFormat);
            }
            set
            {
                if (value is string s)
                {
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        if (AllowNull) ClearToNull();
                        return;
                    }
                    else if (DateTime.TryParse(s, out var dt))
                    {
                        SetValueNullable(dt);
                    }
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            => EditingControlFormattedValue;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange => false;
        public DataGridView EditingControlDataGridView
        {
            get => _dataGridView;
            set => _dataGridView = value;
        }

        public bool EditingControlValueChanged
        {
            get => _valueChanged;
            set => _valueChanged = value;
        }

        public Cursor EditingPanelCursor => base.Cursor;
        #endregion

        protected override void OnValueChanged(EventArgs eventargs)
        {
            _valueChanged = true;
            this._dataGridView?.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            // When clear value form base, notify for the value has been changed 
            if (e.Handled)
            {
                _valueChanged = true;
                _dataGridView?.NotifyCurrentCellDirty(true);
            }
        }
    }
}
