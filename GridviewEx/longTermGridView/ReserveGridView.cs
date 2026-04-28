namespace coms.COMMON.ui
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class ReserveGridView : DataGridView
    {
        public ReserveGridView()
        {
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            ColumnHeadersVisible = false;
            RowHeadersVisible = false;

            SelectionMode = DataGridViewSelectionMode.CellSelect;
            MultiSelect = false;

            AutoGenerateColumns = true;

            // CHANGE THIS:
            // EditMode = DataGridViewEditMode.EditOnEnter;
            EditMode = DataGridViewEditMode.EditProgrammatically;

            CellFormatting += ReserveGridView_CellFormatting;
            CellPainting += ReserveGridView_CellPainting;
            CellBeginEdit += ReserveGridView_CellBeginEdit;
            EditingControlShowing += ReserveGridView_EditingControlShowing;
            CellParsing += ReserveGridView_CellParsing;
            DataError += ReserveGridView_DataError;
        }

        private ReserveGridViewMode _mode = ReserveGridViewMode.Editable;
        private bool _suppressNextBeginEdit;

        [DefaultValue(ReserveGridViewMode.Editable)]
        public ReserveGridViewMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                Invalidate();
            }
        }

        public Color? FocusedCellBackColor { get; set; }
        public Color? FocusedReadOnlyCellBackColor { get; set; }
        public Color? FocusedCellForeColor { get; set; }
        public Color? FocusedReadOnlyCellForeColor { get; set; }

        public event EventHandler<ReserveCellDisplayTextNeededEventArgs> CellDisplayTextNeeded;
        public event EventHandler<ReserveCellReadOnlyNeededEventArgs> CellReadOnlyNeeded;
        public event EventHandler<ReserveCellBeginEditEventArgs> CellBeginEditRule;
        public event EventHandler<ReserveEditingControlShowingEventArgs> EditingControlRule;
        public event EventHandler<ReserveCellStyleNeededEventArgs> CellStyleNeeded;

        private object GetRowDataOrNull(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count) return null;
            return Rows[rowIndex].DataBoundItem;
        }

        private bool IsCellReadOnlyEffective(int rowIndex, int colIndex)
        {
            if (Mode == ReserveGridViewMode.ReadOnly) return true;

            if (rowIndex >= 0 && colIndex >= 0 &&
                rowIndex < Rows.Count && colIndex < Columns.Count)
            {
                var cell = Rows[rowIndex].Cells[colIndex];
                if (cell.ReadOnly) return true;
                if (cell.OwningColumn != null && cell.OwningColumn.ReadOnly) return true;
            }

            if (CellReadOnlyNeeded != null)
            {
                var args = new ReserveCellReadOnlyNeededEventArgs(rowIndex, colIndex, GetRowDataOrNull(rowIndex));
                CellReadOnlyNeeded(this, args);
                if (args.ReadOnly.HasValue) return args.ReadOnly.Value;
            }

            return false;
        }

        private void ReserveGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (IsCellReadOnlyEffective(e.RowIndex, e.ColumnIndex))
            {
                e.Cancel = true;
                return;
            }

            if (CellBeginEditRule != null)
            {
                var args = new ReserveCellBeginEditEventArgs(e.RowIndex, e.ColumnIndex, GetRowDataOrNull(e.RowIndex));
                CellBeginEditRule(this, args);
                if (args.Cancel) e.Cancel = true;
            }
        }

        private void ReserveGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var rowIndex = CurrentCell != null ? CurrentCell.RowIndex : -1;
            var colIndex = CurrentCell != null ? CurrentCell.ColumnIndex : -1;

            if (e.Control is TextBox tb && rowIndex >= 0 && colIndex >= 0)
            {
                try
                {
                    var raw = this[colIndex, rowIndex].Value;
                    tb.Text = raw == null ? string.Empty : Convert.ToString(raw);
                    tb.SelectionStart = tb.TextLength;
                    tb.SelectionLength = 0;
                }
                catch (Exception)
                {
                }
            }

            if (EditingControlRule != null)
            {
                var args = new ReserveEditingControlShowingEventArgs(
                    rowIndex,
                    colIndex,
                    GetRowDataOrNull(rowIndex),
                    e.Control,
                    e.Control as TextBox);

                EditingControlRule(this, args);
            }
        }

        private void ReserveGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var rowData = GetRowDataOrNull(e.RowIndex);

            bool isCurrentCell = (CurrentCell != null &&
                                  CurrentCell.RowIndex == e.RowIndex &&
                                  CurrentCell.ColumnIndex == e.ColumnIndex);

            bool isReadOnly = IsCellReadOnlyEffective(e.RowIndex, e.ColumnIndex);
            bool disableSelectingCelStyle = false;
            if (CellStyleNeeded != null)
            {
                var styleArgs = new ReserveCellStyleNeededEventArgs(
                    e.RowIndex,
                    e.ColumnIndex,
                    rowData,
                    e.Value,
                    isCurrentCell,
                    isReadOnly);

                CellStyleNeeded(this, styleArgs);

                if (styleArgs.BackColor.HasValue) e.CellStyle.BackColor = styleArgs.BackColor.Value;
                if (styleArgs.ForeColor.HasValue) e.CellStyle.ForeColor = styleArgs.ForeColor.Value;
                disableSelectingCelStyle = styleArgs.DisableFocusedStyle;
                // in this case selection style will apply cell format needed style
                if (disableSelectingCelStyle)
                {
                    if (styleArgs.BackColor.HasValue) e.CellStyle.SelectionBackColor = styleArgs.BackColor.Value;
                    if (styleArgs.ForeColor.HasValue) e.CellStyle.SelectionForeColor = styleArgs.ForeColor.Value;
                }
            }

            if (isCurrentCell && disableSelectingCelStyle == false)
            {
                Color? focusBack = null;
                Color? focusFore = null;

                if (isReadOnly)
                {
                    if (FocusedReadOnlyCellBackColor.HasValue) focusBack = FocusedReadOnlyCellBackColor.Value;
                    if (FocusedReadOnlyCellForeColor.HasValue) focusFore = FocusedReadOnlyCellForeColor.Value;
                }

                if (!focusBack.HasValue && FocusedCellBackColor.HasValue) focusBack = FocusedCellBackColor.Value;
                if (!focusFore.HasValue && FocusedCellForeColor.HasValue) focusFore = FocusedCellForeColor.Value;

                if (focusBack.HasValue) e.CellStyle.SelectionBackColor = focusBack.Value;
                if (focusFore.HasValue) e.CellStyle.SelectionForeColor = focusFore.Value;
            }
        }

        private void ReserveGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.Handled) return;

            if (IsCurrentCellInEditMode &&
                CurrentCell != null &&
                CurrentCell.RowIndex == e.RowIndex &&
                CurrentCell.ColumnIndex == e.ColumnIndex)
            {
                e.Handled = false;
                return;
            }

            if (CellDisplayTextNeeded == null) return;

            var rowData = GetRowDataOrNull(e.RowIndex);

            bool isCurrentCell = (CurrentCell != null &&
                                  CurrentCell.RowIndex == e.RowIndex &&
                                  CurrentCell.ColumnIndex == e.ColumnIndex);

            bool isReadOnly = IsCellReadOnlyEffective(e.RowIndex, e.ColumnIndex);

            object rawValue = null;
            try
            {
                rawValue = this[e.ColumnIndex, e.RowIndex].Value;
            }
            catch (Exception)
            {
            }

            var displayArgs = new ReserveCellDisplayTextNeededEventArgs(
                e.RowIndex,
                e.ColumnIndex,
                rowData,
                rawValue,
                isCurrentCell,
                isReadOnly);

            CellDisplayTextNeeded(this, displayArgs);

            if (displayArgs.DisplayText == null) return;

            e.PaintBackground(e.ClipBounds, true);
            e.Paint(e.ClipBounds, DataGridViewPaintParts.Border);

            var textColor = e.State.HasFlag(DataGridViewElementStates.Selected)
                ? e.CellStyle.SelectionForeColor
                : e.CellStyle.ForeColor;

            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            switch (e.CellStyle.Alignment)
            {
                case DataGridViewContentAlignment.BottomRight:
                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.TopRight:
                    flags |= TextFormatFlags.Right;
                    break;

                case DataGridViewContentAlignment.BottomCenter:
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.TopCenter:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;

                default:
                    flags |= TextFormatFlags.Left;
                    break;
            }

            TextRenderer.DrawText(
                e.Graphics,
                displayArgs.DisplayText,
                e.CellStyle.Font,
                e.CellBounds,
                textColor,
                flags);

            e.Handled = true;
        }

        private void ReserveGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (IsCellReadOnlyEffective(e.RowIndex, e.ColumnIndex)) return;

            var col = this.Columns[e.ColumnIndex];

            // determine numeric type (ValueType can be null)
            var valueType = col.ValueType ?? this.Rows[e.RowIndex].Cells[e.ColumnIndex].ValueType;

            bool isNumeric =
                valueType == typeof(int) || valueType == typeof(long) ||
                valueType == typeof(float) || valueType == typeof(double) ||
                valueType == typeof(decimal);

            if (!isNumeric) return;

            string text = Convert.ToString(e.Value)?.Trim() ?? "";

            // empty OR non-number => force 0
            // (TryParse rules can be customized for comma, etc.)
            bool ok =
                (valueType == typeof(int) && int.TryParse(text, out _)) ||
                (valueType == typeof(long) && long.TryParse(text, out _)) ||
                (valueType == typeof(float) && float.TryParse(text, out _)) ||
                (valueType == typeof(double) && double.TryParse(text, out _)) ||
                (valueType == typeof(decimal) && decimal.TryParse(text, out _));

            if (text.Length == 0 || !ok)
            {
                if (valueType == typeof(int)) e.Value = 0;
                else if (valueType == typeof(long)) e.Value = 0L;
                else if (valueType == typeof(float)) e.Value = 0f;
                else if (valueType == typeof(double)) e.Value = 0d;
                else if (valueType == typeof(decimal)) e.Value = 0m;

                e.ParsingApplied = true;   // THIS is the key
            }
        }

        private void ReserveGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // suppress the default DataGridView error dialog
            e.ThrowException = false;
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);

            if (e.Column != null && !(e.Column is DataGridViewTextBoxColumn))
            {
                int index = e.Column.Index;
                var old = e.Column;

                var col = new DataGridViewTextBoxColumn
                {
                    Name = old.Name,
                    HeaderText = old.HeaderText,
                    DataPropertyName = old.DataPropertyName,
                    Width = old.Width,
                    MinimumWidth = old.MinimumWidth,
                    ReadOnly = old.ReadOnly,
                    Visible = old.Visible,
                    Frozen = old.Frozen,
                    AutoSizeMode = old.AutoSizeMode,
                    DefaultCellStyle = old.DefaultCellStyle,
                    SortMode = old.SortMode
                };

                Columns.RemoveAt(index);
                Columns.Insert(index, col);
            }
        }

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            // Right click should NEVER start editing
            if (e.Button == MouseButtons.Right)
                _suppressNextBeginEdit = true;

            // If any edit is active/pending, stop it before changing cell
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    // Ends/cancels any active editor so right-click behaves consistently while focused
                    EndEdit(DataGridViewDataErrorContexts.Commit);
                    CancelEdit();
                }
                catch { }
            }

            base.OnCellMouseDown(e);

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // select clicked cell (both buttons)
            CurrentCell = this[e.ColumnIndex, e.RowIndex];

            // left click => enter edit explicitly
            if (e.Button == MouseButtons.Left)
            {
                if (!IsCellReadOnlyEffective(e.RowIndex, e.ColumnIndex))
                    BeginEdit(true);
            }
        }

        // Add this override in ReserveGridView class:
        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            // If a right-click caused/preceded this edit, cancel it.
            if (_suppressNextBeginEdit)
            {
                e.Cancel = true;
                _suppressNextBeginEdit = false;
                return;
            }

            base.OnCellBeginEdit(e);
        }

        // Optional but helps: reset suppression when leaving the grid
        protected override void OnLeave(EventArgs e)
        {
            _suppressNextBeginEdit = false;
            base.OnLeave(e);
        }
    }

    public enum ReserveGridViewMode
    {
        Editable = 0,
        ReadOnly = 1
    }
}