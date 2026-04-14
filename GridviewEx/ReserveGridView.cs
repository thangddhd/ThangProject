// Namespace requested by user
namespace coms.COMMON.ui
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Minimal customized DataGridView base control.
    /// - TextBox columns only (no merge)
    /// - Cell-level: display text, readonly, style, editor config
    /// - Two modes: Editable / ReadOnly
    /// - Uses DataGridView built-in behaviors as much as possible
    /// </summary>
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    public class ReserveGridView : DataGridView
    {
        public ReserveGridView()
        {
            // Minimal defaults (only those you already requested/confirmed)
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            ColumnHeadersVisible = false;
            RowHeadersVisible = false;

            SelectionMode = DataGridViewSelectionMode.CellSelect;
            MultiSelect = false;

            // Typical “grid-like” behavior; safe defaults
            AutoGenerateColumns = true; // You can set false in forms if you prefer manual columns
            EditMode = DataGridViewEditMode.EditOnEnter;

            // Wire once here so parent forms don't repeat wiring logic
            CellFormatting += ReserveGridView_CellFormatting;
            CellBeginEdit += ReserveGridView_CellBeginEdit;
            EditingControlShowing += ReserveGridView_EditingControlShowing;
        }

        // -----------------------------
        // Mode
        // -----------------------------

        private ReserveGridViewMode _mode = ReserveGridViewMode.Editable;

        [DefaultValue(ReserveGridViewMode.Editable)]
        public ReserveGridViewMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                // Do NOT force DataGridView.ReadOnly here, because we want cell-level control.
                // Just refresh so formatting (focused readonly, etc.) updates immediately.
                Invalidate();
            }
        }

        // Optional focus colors (keep null to preserve DataGridView defaults)
        public Color? FocusedCellBackColor { get; set; }
        public Color? FocusedReadOnlyCellBackColor { get; set; }

        // -----------------------------
        // Public rule events
        // -----------------------------

        /// <summary>
        /// Display-only text override (DevExpress-like CustomColumnDisplayText).
        /// Fired from CellFormatting.
        /// </summary>
        public event EventHandler<ReserveCellDisplayTextNeededEventArgs> CellDisplayTextNeeded;

        /// <summary>
        /// Cell-level read-only rule. Parent can mark the specific cell as readonly.
        /// </summary>
        public event EventHandler<ReserveCellReadOnlyNeededEventArgs> CellReadOnlyNeeded;

        /// <summary>
        /// Fired before editing begins. Parent can cancel editing dynamically.
        /// </summary>
        public event EventHandler<ReserveCellBeginEditEventArgs> CellBeginEditRule;

        /// <summary>
        /// Fired when the editing control (TextBox) is shown. Parent can set MaxLength, ImeMode, etc.
        /// </summary>
        public event EventHandler<ReserveEditingControlShowingEventArgs> EditingControlRule;

        /// <summary>
        /// Cell-level style rule (BackColor/ForeColor). Fired from CellFormatting.
        /// </summary>
        public event EventHandler<ReserveCellStyleNeededEventArgs> CellStyleNeeded;

        // -----------------------------
        // Internal helpers
        // -----------------------------

        private object GetRowDataOrNull(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count) return null;
            return Rows[rowIndex].DataBoundItem;
        }

        private bool IsCellReadOnlyEffective(int rowIndex, int colIndex)
        {
            // Grid-level mode wins
            if (Mode == ReserveGridViewMode.ReadOnly) return true;

            // DataGridView built-in per-cell/per-column readonly settings
            if (rowIndex >= 0 && colIndex >= 0 &&
                rowIndex < Rows.Count && colIndex < Columns.Count)
            {
                var cell = Rows[rowIndex].Cells[colIndex];
                if (cell.ReadOnly) return true;
                if (cell.OwningColumn != null && cell.OwningColumn.ReadOnly) return true;
            }

            // Ask parent form (cell-level)
            if (CellReadOnlyNeeded != null)
            {
                var args = new ReserveCellReadOnlyNeededEventArgs(rowIndex, colIndex, GetRowDataOrNull(rowIndex));
                CellReadOnlyNeeded(this, args);
                if (args.ReadOnly.HasValue) return args.ReadOnly.Value;
            }

            return false;
        }

        // -----------------------------
        // DataGridView event handlers
        // -----------------------------

        private void ReserveGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // First: block edit if readonly
            if (IsCellReadOnlyEffective(e.RowIndex, e.ColumnIndex))
            {
                e.Cancel = true;
                return;
            }

            // Second: allow parent to cancel dynamically (ShowingEditor equivalent)
            if (CellBeginEditRule != null)
            {
                var args = new ReserveCellBeginEditEventArgs(e.RowIndex, e.ColumnIndex, GetRowDataOrNull(e.RowIndex));
                CellBeginEditRule(this, args);
                if (args.Cancel) e.Cancel = true;
            }
        }

        private void ReserveGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Only textbox columns are expected. If not textbox, we still safely expose Control.
            var rowIndex = CurrentCell?.RowIndex ?? -1;
            var colIndex = CurrentCell?.ColumnIndex ?? -1;

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

            // Compute current/focused cell info
            bool isCurrentCell = (CurrentCell != null &&
                                  CurrentCell.RowIndex == e.RowIndex &&
                                  CurrentCell.ColumnIndex == e.ColumnIndex);

            bool isReadOnly = IsCellReadOnlyEffective(e.RowIndex, e.ColumnIndex);

            // 1) Display text (display-only)
            if (CellDisplayTextNeeded != null)
            {
                var displayArgs = new ReserveCellDisplayTextNeededEventArgs(
                    e.RowIndex,
                    e.ColumnIndex,
                    rowData,
                    e.Value,
                    isCurrentCell,
                    isReadOnly);

                CellDisplayTextNeeded(this, displayArgs);

                if (displayArgs.DisplayText != null)
                {
                    e.Value = displayArgs.DisplayText;
                    e.FormattingApplied = true;
                }
            }

            // 2) Style
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
            }

            // 3) Focus highlight (minimal, no custom draw)
            // We implement it by adjusting selection backcolor, because DataGridView paints selected/current cell
            // using selection colors.
            if (isCurrentCell)
            {
                Color? focusColor = null;

                if (isReadOnly && FocusedReadOnlyCellBackColor.HasValue)
                    focusColor = FocusedReadOnlyCellBackColor.Value;
                else if (FocusedCellBackColor.HasValue)
                    focusColor = FocusedCellBackColor.Value;

                if (focusColor.HasValue)
                {
                    e.CellStyle.SelectionBackColor = focusColor.Value;
                    // Keep default selection forecolor unless you want custom later
                }
            }
        }

        // -----------------------------
        // Enforce textbox-only columns (minimal guard)
        // -----------------------------
        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);

            // C# 7.3 compatible: use !(x is Type) instead of "is not"
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
    }

    public enum ReserveGridViewMode
    {
        Editable = 0,
        ReadOnly = 1
    }

    // -----------------------------
    // EventArgs
    // -----------------------------

    public sealed class ReserveCellReadOnlyNeededEventArgs : EventArgs
    {
        public ReserveCellReadOnlyNeededEventArgs(int rowIndex, int columnIndex, object rowData)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }

        /// <summary>
        /// Parent sets this to true/false to force readonly state.
        /// Null means "no opinion".
        /// </summary>
        public bool? ReadOnly { get; set; }
    }

    public sealed class ReserveCellBeginEditEventArgs : EventArgs
    {
        public ReserveCellBeginEditEventArgs(int rowIndex, int columnIndex, object rowData)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }

        public bool Cancel { get; set; }
    }

    public sealed class ReserveEditingControlShowingEventArgs : EventArgs
    {
        public ReserveEditingControlShowingEventArgs(
            int rowIndex,
            int columnIndex,
            object rowData,
            Control editingControl,
            TextBox textBox)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
            EditingControl = editingControl;
            TextBox = textBox;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }

        public Control EditingControl { get; }
        public TextBox TextBox { get; }
    }

    public sealed class ReserveCellDisplayTextNeededEventArgs : EventArgs
    {
        public ReserveCellDisplayTextNeededEventArgs(
            int rowIndex,
            int columnIndex,
            object rowData,
            object value,
            bool isCurrentCell,
            bool isReadOnly)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
            Value = value;
            IsCurrentCell = isCurrentCell;
            IsReadOnly = isReadOnly;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }
        public object Value { get; }
        public bool IsCurrentCell { get; }
        public bool IsReadOnly { get; }

        /// <summary>
        /// Parent sets this to override the displayed text (display-only).
        /// Null means "use default formatting".
        /// </summary>
        public string DisplayText { get; set; }
    }

    public sealed class ReserveCellStyleNeededEventArgs : EventArgs
    {
        public ReserveCellStyleNeededEventArgs(
            int rowIndex,
            int columnIndex,
            object rowData,
            object value,
            bool isCurrentCell,
            bool isReadOnly)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
            Value = value;
            IsCurrentCell = isCurrentCell;
            IsReadOnly = isReadOnly;
        }

        public int RowIndex { get; }
        public int ColumnIndex { get; }
        public object RowData { get; }
        public object Value { get; }
        public bool IsCurrentCell { get; }
        public bool IsReadOnly { get; }

        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
    }
}