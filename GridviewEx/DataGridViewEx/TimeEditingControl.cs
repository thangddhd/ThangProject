using System;
using System.Drawing;
using System.Windows.Forms;

namespace coms.COMMON.ui
{
    public class TimeEditingControl : TextBox, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        bool valueChanged = false;
        int rowIndex;

        public string PlaceholderText { get; set; } = "HH:mm (例: 23:59)";
        private bool _isPlaceholderActive = false;

        public TimeEditingControl()
        {
            this.ForeColor = Color.Black;
            ShowPlaceholder();
        }

        public void ShowPlaceholder()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                _isPlaceholderActive = true;
                this.Text = PlaceholderText;
                this.ForeColor = Color.Gray;
            }
        }

        public void HidePlaceholder()
        {
            if (_isPlaceholderActive)
            {
                _isPlaceholderActive = false;
                // Only clear text if it's still showing the placeholder
                if (this.Text == PlaceholderText)
                {
                    this.Text = "";
                }
                this.ForeColor = Color.Black;
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            HidePlaceholder();
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
                ShowPlaceholder();

            base.OnLeave(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (_isPlaceholderActive)
                HidePlaceholder();

            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ':')
            {
                e.Handled = true;
                return;
            }

            if (!char.IsControl(e.KeyChar))
            {
                int len = this.Text.Length;

                if (this.SelectionLength > 0)
                    len -= this.SelectionLength;

                if (len >= 5)
                {
                    e.Handled = true;
                    return;
                }
            }

            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (_isPlaceholderActive) return;

            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
            base.OnTextChanged(e);
        }

        // IDataGridViewEditingControl
        public object EditingControlFormattedValue
        {
            get => _isPlaceholderActive ? "" : this.Text;
            set => this.Text = value?.ToString();
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            => EditingControlFormattedValue;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle style)
        {
            this.Font = style.Font;
            this.BackColor = style.BackColor;
            this.ForeColor = _isPlaceholderActive ? Color.Gray : style.ForeColor;
        }

        public int EditingControlRowIndex { get => rowIndex; set => rowIndex = value; }

        public bool EditingControlWantsInputKey(Keys key, bool gridWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    return true;
                default:
                    return !gridWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            HidePlaceholder();
            if (selectAll && !_isPlaceholderActive)
                this.SelectAll();
        }

        public bool RepositionEditingControlOnValueChange => false;

        public DataGridView EditingControlDataGridView
        { get => dataGridView; set => dataGridView = value; }

        public bool EditingControlValueChanged
        { get => valueChanged; set => this.valueChanged = value; }

        public Cursor EditingPanelCursor => Cursors.IBeam;
    }
}
