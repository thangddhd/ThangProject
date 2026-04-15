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
            EditMode = DataGridViewEditMode.EditOnEnter;

            CellFormatting += ReserveGridView_CellFormatting;
            CellPainting += ReserveGridView_CellPainting;
            CellBeginEdit += ReserveGridView_CellBeginEdit;
            EditingControlShowing += ReserveGridView_EditingControlShowing;
        }

        private ReserveGridViewMode _mode = ReserveGridViewMode.Editable;

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

            if (isCurrentCell)
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
    }

    public enum ReserveGridViewMode
    {
        Editable = 0,
        ReadOnly = 1
    }
}