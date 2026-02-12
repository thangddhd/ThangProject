using System;
using System.Globalization;
using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class NumericEditingControl : TextBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public bool AllowDecimal { get; set; } = false;

        public NumericEditingControl()
        {
            this.BorderStyle = BorderStyle.None;
            this.TextAlign = HorizontalAlignment.Right;
        }

        // VALIDATE INPUT
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                base.OnKeyPress(e);
                return;
            }

            if (char.IsDigit(e.KeyChar))
            {
                base.OnKeyPress(e);
                return;
            }

            if (!AllowDecimal && e.KeyChar == '.')
            {
                e.Handled = true;
                return;
            }

            if (AllowDecimal && e.KeyChar == '.' && !this.Text.Contains("."))
            {
                base.OnKeyPress(e);
                return;
            }

            e.Handled = true;
        }

        // NOTIFY DIRTY WHEN USER TYPES
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Focused)
            {
                valueChanged = true;
                dataGridView?.NotifyCurrentCellDirty(true);
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
        }

        // IDataGridViewEditingControl
        public object EditingControlFormattedValue
        {
            get => this.Text;
            set => this.Text = value?.ToString() ?? "";
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            => this.Text;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle style)
        {
            this.Font = style.Font;
            this.BackColor = style.BackColor;
            this.ForeColor = style.ForeColor;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
            => true;

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
                this.SelectAll();
        }

        public bool RepositionEditingControlOnValueChange => false;

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        public Cursor EditingPanelCursor => Cursors.IBeam;
    }
}
