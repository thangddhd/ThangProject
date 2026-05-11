using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using coms.COMMON.ui;

namespace coms.COMSK.ui.common
{
    public class LongRepairGridView<T> : DataGridView
    {
        public HeaderBandLayout HeaderLayout { get; private set; }

        public bool MergingEnabled { get; set; }

        public IVerticalMergeProvider<T> VerticalMergeProvider { get; set; }

        public Func<T, bool> IsCalcRow { get; set; }

        public string[] YearColumnNamePrefixes { get; set; }

        public int HighlightedRowIndex { get; private set; }

        public Color RowHighlightTopBorderColor { get; set; }
        public Color RowHighlightBottomBorderColor { get; set; }
        public int RowHighlightBorderThickness { get { return _rowBorderThickness; } set { _rowBorderThickness = Math.Max(1, value); } }

        public event EventHandler<RowCellsDragEventArgs> RowCellsDragCompleted;

        public IReadOnlyList<string> LeftColumnNames { get { return _leftColumnNames; } }
        public IReadOnlyList<string> VerticalMergeColumnNames { get { return _verticalMergeColumnNames; } }

        // --------- ReserveGridView merged APIs ----------
        // NOTE:
        // - Keep old LongRepairGridView behavior: always neutral selection colors.
        // - CellReadOnlyNeeded is re-added per request (business rule readonly).
        public Color? FocusedCellBackColor { get; set; }
        public Color? FocusedReadOnlyCellBackColor { get; set; }
        public Color? FocusedCellForeColor { get; set; }
        public Color? FocusedReadOnlyCellForeColor { get; set; }

        public event EventHandler<ReserveCellDisplayTextNeededEventArgs> CellDisplayTextNeeded;
        public event EventHandler<ReserveCellBeginEditEventArgs> CellBeginEditRule;
        public event EventHandler<ReserveEditingControlShowingEventArgs> EditingControlRule;
        public event EventHandler<ReserveCellStyleNeededEventArgs> CellStyleNeeded;
        public event EventHandler<ReserveButtonCellStyleNeededEventArgs> ButtonCellStyleNeeded;

        // NEW: merge from ReserveGridView
        public event EventHandler<ReserveCellReadOnlyNeededEventArgs> CellReadOnlyNeeded;
        // -----------------------------------------------

        private readonly MergeStore _mergeStore = new MergeStore();

        private List<string> _leftColumnNames = new List<string>();
        private List<string> _verticalMergeColumnNames = new List<string>();

        private readonly Dictionary<Tuple<int, string>, CellStyleOverride> _cellStyleOverrides =
            new Dictionary<Tuple<int, string>, CellStyleOverride>();

        private readonly HashSet<CellKey> _editPermit = new HashSet<CellKey>();

        private bool _dragging;
        private CellKey _dragStart = new CellKey(-1, -1);
        private CellKey _dragEnd = new CellKey(-1, -1);
        private Cursor _normalCursor = null;
        private Point _mouseDownPoint;

        // selection range highlight (lavender)
        private bool _hasDragSelection;
        private int _selDisplayColMin = -1;
        private int _selDisplayColMax = -1;

        private int _rowBorderThickness = 2;

        private CellKey _hoverButtonCell = new CellKey(-1, -1);
        private CellKey _pressedButtonCell = new CellKey(-1, -1);

        /// <summary>
        /// Optional: business-rule filter for whether a cell is draggable/selectable.
        /// If null, default rule is: year column only (+ optional DraggableColumnTag).
        /// </summary>
        public Func<LongRepairGridView<T>, int, DataGridViewColumn, T, bool> CanDragCell { get; set; }

        private enum DragMode
        {
            None,
            VerticalRangeSelect,  // same column, different rows
            HorizontalMove        // same row, different columns
        }
        private DragMode _dragMode = DragMode.None;
        // Range selection (same column)
        private bool _hasRowRangeSelection;
        private int _rangeAnchorRow = -1;  // first row user pointed to
        private int _rangeEndRow = -1;     // current end row
        private int _rangeColumnIndex = -1; // the column being selected (must stay same)
        private int _activeMoveRow = -1; // row currently used for horizontal dragging (must be inside selected range)

        // ---- Header drag-to-hide support ----
        private bool _headerBandDragging;
        private HeaderBandCellByName _dragBandCell;
        private Point _dragStartPoint;
        private Point _lastMousePoint;
        private List<string> _lastHiddenColumnNames = new List<string>();

        /// <summary>
        /// Returns column names hidden by the last drag-to-hide action.
        /// </summary>
        public IReadOnlyList<string> LastHiddenColumnNames => _lastHiddenColumnNames.AsReadOnly();
        private readonly HashSet<string> _rightBorderColumnNames = new HashSet<string>(StringComparer.Ordinal);
        public LongRepairGridView()
        {
            HeaderLayout = null;
            MergingEnabled = true;

            YearColumnNamePrefixes = new[] { "Y_", "Period_" };

            HighlightedRowIndex = -1;

            RowHighlightTopBorderColor = Color.Red;
            RowHighlightBottomBorderColor = Color.Red;

            MultiSelect = false;
            EnableHeadersVisualStyles = false;

            EnableDoubleBuffering();

            // events
            CellFormatting += OnCellFormatting;
            CellPainting += OnCellPainting;
            RowPostPaint += OnRowPostPaint;

            CellMouseDown += OnCellMouseDown;
            CellDoubleClick += OnCellDoubleClick;
            CellBeginEdit += OnCellBeginEdit;
            CellEndEdit += OnCellEndEdit;
            EditingControlShowing += OnEditingControlShowing;

            Scroll += OnScrollInvalidate;
            ColumnWidthChanged += OnColumnLayoutChanged;
            ColumnDisplayIndexChanged += OnColumnLayoutChanged;
            ColumnStateChanged += OnColumnLayoutChanged;

            MouseDown += OnMouseDownDrag;
            MouseMove += OnMouseMoveDrag;
            MouseUp += OnMouseUpDrag;

            CellMouseMove += OnCellMouseMoveButtonHover;
            CellMouseLeave += OnCellMouseLeaveButtonHover;
            MouseDown += OnMouseDownButtonPress;
            MouseUp += OnMouseUpButtonPress;

            MouseDown += OnMouseDownHeaderBandDrag;
            MouseMove += OnMouseMoveHeaderBandDrag;
            MouseUp += OnMouseUpHeaderBandDrag;
        }

        private void EnableDoubleBuffering()
        {
            // DataGridView.DoubleBuffered is protected; use reflection safely
            try
            {
                typeof(DataGridView).InvokeMember(
                    "DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null,
                    this,
                    new object[] { true });
            }
            catch { }
        }

        // -------- ReserveGridView merged helpers --------
        private object GetRowDataOrNull(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count) return null;
            return Rows[rowIndex].DataBoundItem;
        }

        /// <summary>
        /// Effective readonly calculation:
        /// - merged cell => always readonly (keeps LongRepairGridView behavior)
        /// - otherwise: DataGridView's readonly flags + optional CellReadOnlyNeeded override
        /// </summary>
        private bool IsCellReadOnlyByRule(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || columnIndex < 0) return true;
            if (rowIndex >= Rows.Count || columnIndex >= Columns.Count) return true;

            // merged => readonly (existing behavior)
            CellKey owner;
            if (MergingEnabled && _mergeStore.TryGetOwner(rowIndex, columnIndex, out owner))
            {
                return true;
            }

            bool isReadOnly = false;
            try
            {
                var cell = Rows[rowIndex].Cells[columnIndex];
                isReadOnly =
                    this.ReadOnly ||
                    (Rows[rowIndex].ReadOnly) ||
                    (cell != null && cell.ReadOnly) ||
                    (cell != null && cell.OwningColumn != null && cell.OwningColumn.ReadOnly);
            }
            catch { }

            // business rule hook (separate from drag logic)
            if (CellReadOnlyNeeded != null)
            {
                var args = new ReserveCellReadOnlyNeededEventArgs(rowIndex, columnIndex, GetRowDataOrNull(rowIndex));
                CellReadOnlyNeeded(this, args);
                if (args.ReadOnly.HasValue) isReadOnly = args.ReadOnly.Value;
            }

            return isReadOnly;
        }
        // -----------------------------------------------

        // -------- Public APIs --------
        public void SetHeaderLayout(HeaderBandLayout layout)
        {
            HeaderLayout = layout;
            ApplyHeaderHeight();
            Invalidate();
        }

        public void SetLeftColumnNames(IEnumerable<string> leftColumnNames)
        {
            _leftColumnNames = NormalizeDistinct(leftColumnNames);
        }

        public void SetVerticalMergeColumns(IEnumerable<string> columnNames)
        {
            _verticalMergeColumnNames = NormalizeDistinct(columnNames);
        }

        public void SetCellBackColor(int rowIndex, string columnName, Color color)
        {
            if (rowIndex < 0) return;
            if (string.IsNullOrEmpty(columnName)) return;

            DataGridViewColumn col = Columns[columnName];
            if (col == null) return;

            _cellStyleOverrides[Tuple.Create(rowIndex, columnName)] = new CellStyleOverride { BackColor = color };
            InvalidateCell(col.Index, rowIndex);
        }

        public void ClearCellStyle(int rowIndex, string columnName)
        {
            var key = Tuple.Create(rowIndex, columnName);
            if (_cellStyleOverrides.Remove(key))
            {
                DataGridViewColumn col = Columns[columnName];
                if (col != null) InvalidateCell(col.Index, rowIndex);
            }
        }

        public void ClearAllCellStyles()
        {
            _cellStyleOverrides.Clear();
            Invalidate();
        }

        public void ClearMerges()
        {
            _mergeStore.Clear();
            Invalidate();
        }

        public void RebuildMerges()
        {
            _mergeStore.Clear();

            if (!MergingEnabled) { Invalidate(); return; }
            if (RowCount <= 0 || ColumnCount <= 0) { Invalidate(); return; }

            BuildVerticalMerges();
            BuildCalcRowHorizontalMerges();

            Invalidate();
        }

        public void SetHighlightedRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowCount) return;
            if (HighlightedRowIndex == rowIndex) return;

            int old = HighlightedRowIndex;
            HighlightedRowIndex = rowIndex;

            if (old >= 0 && old < RowCount) InvalidateRow(old);
            InvalidateRow(HighlightedRowIndex);
        }

        // -------- Merge building --------
        private void BuildVerticalMerges()
        {
            if (VerticalMergeProvider == null) return;
            if (_verticalMergeColumnNames == null || _verticalMergeColumnNames.Count == 0) return;

            foreach (string colName in _verticalMergeColumnNames)
            {
                DataGridViewColumn col = Columns[colName];
                if (col == null || !col.Visible) continue;

                int colIndex = col.Index;
                int start = -1;

                for (int r = 0; r < RowCount - 1; r++)
                {
                    T row1, row2;
                    if (!TryGetRowModel(r, out row1) || !TryGetRowModel(r + 1, out row2))
                    {
                        CloseRegionIfAny(ref start, r, colIndex);
                        continue;
                    }

                    bool mergeNext = VerticalMergeProvider.MergeWithNextRow(this, row1, row2, colName, r);

                    if (mergeNext)
                    {
                        if (start < 0) start = r;
                    }
                    else
                    {
                        CloseRegionIfAny(ref start, r, colIndex);
                    }
                }

                if (start >= 0)
                {
                    int endRow = RowCount - 1;
                    AddRegion(new MergeRegion
                    {
                        RowStart = start,
                        RowSpan = endRow - start + 1,
                        ColumnIndexes = new[] { colIndex }
                    });
                }
            }
        }

        private void CloseRegionIfAny(ref int start, int currentRow, int colIndex)
        {
            if (start < 0) return;

            int endRow = currentRow;
            int span = endRow - start + 1;
            if (span >= 2)
            {
                AddRegion(new MergeRegion
                {
                    RowStart = start,
                    RowSpan = span,
                    ColumnIndexes = new[] { colIndex }
                });
            }
            start = -1;
        }

        private void BuildCalcRowHorizontalMerges()
        {
            if (_leftColumnNames == null || _leftColumnNames.Count == 0) return;
            if (IsCalcRow == null) return;

            // strict resolve
            var resolved = new List<DataGridViewColumn>();
            foreach (string name in _leftColumnNames)
            {
                DataGridViewColumn col = Columns[name];
                if (col == null || !col.Visible) return; // strict: skip entirely
                resolved.Add(col);
            }

            var ordered = resolved.OrderBy(c => c.DisplayIndex).ToArray();
            int[] colIndexes = ordered.Select(c => c.Index).ToArray();
            if (colIndexes.Length <= 1) return;

            for (int r = 0; r < RowCount; r++)
            {
                T model;
                if (!TryGetRowModel(r, out model)) continue;
                if (!IsCalcRow(model)) continue;

                AddRegion(new MergeRegion
                {
                    RowStart = r,
                    RowSpan = 1,
                    ColumnIndexes = colIndexes
                });
            }
        }

        private void AddRegion(MergeRegion region)
        {
            if (region == null) return;
            if (region.ColumnIndexes == null || region.ColumnIndexes.Length == 0) return;
            if (region.RowSpan <= 1 && region.ColumnIndexes.Length <= 1) return;

            int ownerR = region.OwnerRow;
            int ownerC = region.OwnerCol;
            if (ownerC < 0) return;

            // overlap validation
            for (int r = region.RowStart; r < region.RowStart + region.RowSpan; r++)
            {
                if (r < 0 || r >= RowCount) continue;

                for (int i = 0; i < region.ColumnIndexes.Length; i++)
                {
                    int c = region.ColumnIndexes[i];
                    if (c < 0 || c >= ColumnCount) continue;

                    var key = new CellKey(r, c);
                    if (_mergeStore.OwnerByCell.ContainsKey(key))
                    {
                        // Overlap: ignore later region (safe behavior)
                        return;
                    }
                }
            }

            var ownerKey = new CellKey(ownerR, ownerC);
            _mergeStore.RegionByOwner[ownerKey] = region;

            for (int r = region.RowStart; r < region.RowStart + region.RowSpan; r++)
            {
                for (int i = 0; i < region.ColumnIndexes.Length; i++)
                {
                    int c = region.ColumnIndexes[i];
                    _mergeStore.OwnerByCell[new CellKey(r, c)] = ownerKey;
                }
            }
        }

        private bool TryGetRowModel(int rowIndex, out T model)
        {
            model = default(T);

            if (rowIndex < 0 || rowIndex >= RowCount) return false;

            object item = Rows[rowIndex].DataBoundItem;
            if (item is T)
            {
                model = (T)item;
                return true;
            }
            return false;
        }

        // -------- Formatting / Painting --------
        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewColumn col = Columns[e.ColumnIndex];
            string colName = col != null ? col.Name : null;

            // =========================
            // 1. Programmatic overrides
            // =========================
            if (!string.IsNullOrEmpty(colName))
            {
                CellStyleOverride ov;
                if (_cellStyleOverrides.TryGetValue(Tuple.Create(e.RowIndex, colName), out ov) && ov != null)
                {
                    if (ov.BackColor.HasValue) e.CellStyle.BackColor = ov.BackColor.Value;
                    if (ov.ForeColor.HasValue) e.CellStyle.ForeColor = ov.ForeColor.Value;
                    if (ov.Font != null) e.CellStyle.Font = ov.Font;
                }
            }

            // =========================
            // 2. Detect selection state (ONLY MARK, don't apply yet)
            // =========================
            bool isSelectedByDrag = false;

            // vertical range (same column)
            if (_hasRowRangeSelection && e.ColumnIndex == _rangeColumnIndex)
            {
                int r1 = Math.Min(_rangeAnchorRow, _rangeEndRow);
                int r2 = Math.Max(_rangeAnchorRow, _rangeEndRow);

                if (e.RowIndex >= r1 && e.RowIndex <= r2)
                    isSelectedByDrag = true;
            }
            // horizontal drag selection
            else if (_hasDragSelection && _selDisplayColMin >= 0 && _selDisplayColMax >= 0 && _hasRowRangeSelection)
            {
                int r1 = Math.Min(_rangeAnchorRow, _rangeEndRow);
                int r2 = Math.Max(_rangeAnchorRow, _rangeEndRow);

                if (e.RowIndex >= r1 && e.RowIndex <= r2)
                {
                    int di = col.DisplayIndex;
                    if (di >= _selDisplayColMin && di <= _selDisplayColMax)
                        isSelectedByDrag = true;
                }
            }

            // =========================
            // 3. CellStyleNeeded (business rule)
            // =========================
            if (CellStyleNeeded != null)
            {
                var rowData = GetRowDataOrNull(e.RowIndex);

                bool isCurrentCell = (CurrentCell != null &&
                                      CurrentCell.RowIndex == e.RowIndex &&
                                      CurrentCell.ColumnIndex == e.ColumnIndex);

                bool isReadOnly = IsCellReadOnlyByRule(e.RowIndex, e.ColumnIndex);

                var styleArgs = new ReserveCellStyleNeededEventArgs(
                    e.RowIndex,
                    e.ColumnIndex,
                    rowData,
                    e.Value,
                    isCurrentCell,
                    isReadOnly);

                CellStyleNeeded(this, styleArgs);

                if (styleArgs.BackColor.HasValue)
                    e.CellStyle.BackColor = styleArgs.BackColor.Value;

                if (styleArgs.ForeColor.HasValue)
                    e.CellStyle.ForeColor = styleArgs.ForeColor.Value;
            }

            // =========================
            // 4. Button style (giữ nguyên logic cũ)
            // =========================
            try
            {
                var btnArgs = RaiseButtonCellStyleNeeded(e.RowIndex, e.ColumnIndex, e.Value);
                if (btnArgs != null)
                {
                    if (btnArgs.Visible.HasValue && btnArgs.Visible.Value == false)
                    {
                        e.Value = string.Empty;
                    }
                    else if (btnArgs.Text != null)
                    {
                        e.Value = btnArgs.Text;
                    }

                    if (btnArgs.BackColor.HasValue) e.CellStyle.BackColor = btnArgs.BackColor.Value;
                    if (btnArgs.ForeColor.HasValue) e.CellStyle.ForeColor = btnArgs.ForeColor.Value;

                    if (btnArgs.DisabledStyle)
                    {
                        if (!btnArgs.BackColor.HasValue) e.CellStyle.BackColor = Color.Gainsboro;
                        if (!btnArgs.ForeColor.HasValue) e.CellStyle.ForeColor = Color.DimGray;
                    }
                }
            }
            catch { }

            // =========================
            // 5. 🔥 FINAL OVERRIDE: selection MUST WIN
            // =========================
            if (isSelectedByDrag)
            {
                e.CellStyle.BackColor = Blend(Color.Lavender, Color.Black, 0.1f);
            }

            // =========================
            // 6. Keep neutral selection behavior
            // =========================
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
        }

        private void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            bool needsRightBorder = false;
            try { needsRightBorder = HasRightBorder(e.ColumnIndex); } catch { }

            // If current cell is in edit mode, don't custom paint it.
            // BUT: we still want the right border => draw via grid line? (not safe).
            // Best: do nothing here; the editing control will cover the cell anyway.
            if (IsCurrentCellInEditMode &&
                CurrentCell != null &&
                CurrentCell.RowIndex == e.RowIndex &&
                CurrentCell.ColumnIndex == e.ColumnIndex)
            {
                return;
            }

            // --- 1) Button rendering path ---
            try
            {
                if (Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    var args = RaiseButtonCellStyleNeeded(e.RowIndex, e.ColumnIndex, null);
                    if (args != null)
                    {
                        // visible=false => draw nothing (just background + border)
                        if (args.Visible.HasValue && args.Visible.Value == false)
                        {
                            e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
                            if (needsRightBorder) PaintRightBorderIfNeeded(e);
                            e.Handled = true;
                            return;
                        }

                        bool wants =
                            args.DisabledStyle ||
                            args.BackColor.HasValue ||
                            args.ForeColor.HasValue ||
                            args.Text != null;

                        if (wants)
                        {
                            string btnText = args.Text;
                            if (btnText == null)
                            {
                                try { btnText = Convert.ToString(this[e.ColumnIndex, e.RowIndex].FormattedValue); }
                                catch { btnText = string.Empty; }
                            }

                            bool hovered = _hoverButtonCell.Row == e.RowIndex && _hoverButtonCell.Col == e.ColumnIndex;
                            bool pressed = _pressedButtonCell.Row == e.RowIndex && _pressedButtonCell.Col == e.ColumnIndex;

                            Color back = args.BackColor ?? Color.White;
                            Color fore = args.ForeColor ?? Color.Black;

                            DrawRoundedGradientButton(
                                e,
                                btnText,
                                back,
                                fore,
                                args.DisabledStyle,
                                hovered,
                                pressed,
                                radius: 4);

                            if (needsRightBorder) PaintRightBorderIfNeeded(e);
                            // DrawRoundedGradientButton already sets Handled=true
                            return;
                        }
                    }
                }
            }
            catch { }

            // --- 2) Detect merged cell ---
            bool isMergedCell = false;
            try
            {
                CellKey tmpOwner;
                if (MergingEnabled && _mergeStore.TryGetOwner(e.RowIndex, e.ColumnIndex, out tmpOwner))
                    isMergedCell = true;
            }
            catch { }

            // --- 3) Custom display text path (non-merged only) ---
            if (!isMergedCell && CellDisplayTextNeeded != null)
            {
                try
                {
                    var rowData = GetRowDataOrNull(e.RowIndex);

                    bool isCurrentCell = (CurrentCell != null &&
                                          CurrentCell.RowIndex == e.RowIndex &&
                                          CurrentCell.ColumnIndex == e.ColumnIndex);

                    bool isReadOnly = IsCellReadOnlyByRule(e.RowIndex, e.ColumnIndex);

                    object rawValue = null;
                    try { rawValue = this[e.ColumnIndex, e.RowIndex].Value; } catch { }

                    var displayArgs = new ReserveCellDisplayTextNeededEventArgs(
                        e.RowIndex,
                        e.ColumnIndex,
                        rowData,
                        rawValue,
                        isCurrentCell,
                        isReadOnly);

                    CellDisplayTextNeeded(this, displayArgs);

                    if (displayArgs.DisplayText != null)
                    {
                        // paint everything normally first
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

                        if (needsRightBorder) PaintRightBorderIfNeeded(e);

                        e.Handled = true;
                        return;
                    }
                }
                catch { }
            }

            // --- 4) Merged cell paint path ---
            if (MergingEnabled)
            {
                CellKey owner;
                if (_mergeStore.TryGetOwner(e.RowIndex, e.ColumnIndex, out owner))
                {
                    MergeRegion region;
                    if (_mergeStore.TryGetRegionByOwner(owner, out region) && region != null)
                    {
                        Rectangle mergedRect = GetMergedRect(region);
                        if (!mergedRect.IsEmpty)
                        {
                            // Always paint something for this cell to avoid black and missing visuals
                            e.PaintBackground(e.CellBounds, true);

                            // Fill merged background (only the part being repainted)
                            Rectangle paintRect = Rectangle.Intersect(mergedRect, e.CellBounds);
                            if (!paintRect.IsEmpty)
                            {
                                using (SolidBrush back = new SolidBrush(e.CellStyle.BackColor))
                                    e.Graphics.FillRectangle(back, paintRect);
                            }

                            // Draw text from owner value, but clip to current cell repaint area
                            object val = this[owner.Col, owner.Row].FormattedValue;
                            string text = Convert.ToString(val) ?? string.Empty;

                            Rectangle textRect = Rectangle.Inflate(mergedRect, -2, -2);

                            Region oldClip = e.Graphics.Clip;
                            try
                            {
                                e.Graphics.SetClip(e.CellBounds);

                                TextRenderer.DrawText(
                                    e.Graphics,
                                    text,
                                    e.CellStyle.Font ?? Font,
                                    textRect,
                                    e.CellStyle.ForeColor,
                                    TextFormatFlags.Left |
                                    TextFormatFlags.VerticalCenter |
                                    TextFormatFlags.EndEllipsis |
                                    TextFormatFlags.NoClipping);
                            }
                            finally
                            {
                                e.Graphics.Clip = oldClip;
                                if (oldClip != null) oldClip.Dispose();
                            }

                            DrawMergedOuterBorderIfNeeded(e, region, mergedRect);

                            if (needsRightBorder) PaintRightBorderIfNeeded(e);

                            e.Handled = true;
                            return;
                        }
                    }
                }
            }

            // --- 5) Default cells: if we need a right border, we must fully paint then overlay border ---
            if (needsRightBorder)
            {
                // paint default cell completely, then overlay the right border line
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
                PaintRightBorderIfNeeded(e);
                e.Handled = true;
                return;
            }

            // else: do nothing => default painting
        }

        private Rectangle GetMergedRect(MergeRegion region)
        {
            Rectangle mergedRect = Rectangle.Empty;

            for (int r = region.RowStart; r < region.RowStart + region.RowSpan; r++)
            {
                for (int i = 0; i < region.ColumnIndexes.Length; i++)
                {
                    int c = region.ColumnIndexes[i];
                    Rectangle cellRect = GetCellDisplayRectangle(c, r, true);
                    if (cellRect.Width <= 0 || cellRect.Height <= 0) continue;

                    mergedRect = mergedRect.IsEmpty ? cellRect : Rectangle.Union(mergedRect, cellRect);
                }
            }

            return mergedRect;
        }

        private void DrawMergedOuterBorderIfNeeded(DataGridViewCellPaintingEventArgs e, MergeRegion region, Rectangle mergedRect)
        {
            int topRow = region.RowStart;
            int bottomRow = region.RowStart + region.RowSpan - 1;

            int leftCol = region.ColumnIndexes
                .Select(ci => this.Columns[ci])
                .OrderBy(c => c.DisplayIndex)
                .First().Index;

            int rightCol = region.ColumnIndexes
                .Select(ci => this.Columns[ci])
                .OrderByDescending(c => c.DisplayIndex)
                .First().Index;

            Rectangle cellRect = e.CellBounds;
            if (cellRect.Width <= 0 || cellRect.Height <= 0) return;

            int left = cellRect.Left;
            int top = cellRect.Top;
            int right = cellRect.Right - 1;
            int bottom = cellRect.Bottom - 1;

            using (Pen pen = new Pen(this.GridColor, 1f))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                // top border
                if (e.RowIndex == topRow)
                {
                    e.Graphics.DrawLine(pen, left, top, right, top);
                }

                // bottom border
                if (e.RowIndex == bottomRow)
                {
                    e.Graphics.DrawLine(pen, left, bottom, right, bottom);
                }

                // left border
                if (e.ColumnIndex == leftCol)
                {
                    e.Graphics.DrawLine(pen, left, top, left, bottom);
                }

                // right border
                if (e.ColumnIndex == rightCol)
                {
                    e.Graphics.DrawLine(pen, right, top, right, bottom);
                }
            }
        }

        private void OnRowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex != HighlightedRowIndex) return;

            Rectangle rowRect = GetRowDisplayRectangle(e.RowIndex, true);
            if (rowRect.Width <= 0 || rowRect.Height <= 0) return;

            int thickness = _rowBorderThickness;

            int left = 0;
            int right = ClientSize.Width - 1;

            int topY = rowRect.Top;
            int bottomY = rowRect.Bottom - thickness;

            using (var topBrush = new SolidBrush(RowHighlightTopBorderColor))
            using (var bottomBrush = new SolidBrush(RowHighlightBottomBorderColor))
            {
                e.Graphics.FillRectangle(topBrush, left, topY, right - left, thickness);
                e.Graphics.FillRectangle(bottomBrush, left, bottomY, right - left, thickness);
            }
        }

        private void OnCellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            SetHighlightedRow(e.RowIndex);
        }

        private void OnCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewColumn col = Columns[e.ColumnIndex];
            if (col == null) return;

            // merged => no edit (existing behavior)
            CellKey owner;
            if (MergingEnabled && _mergeStore.TryGetOwner(e.RowIndex, e.ColumnIndex, out owner))
                return;

            // NEW: readonly comes from CellReadOnlyNeeded + DataGridView flags (not drag rule)
            if (IsCellReadOnlyByRule(e.RowIndex, e.ColumnIndex))
                return;

            // Normalize DBNull/null BEFORE BeginEdit
            try
            {
                var cell = this[e.ColumnIndex, e.RowIndex];
                if (cell != null && (cell.Value == null || cell.Value == DBNull.Value))
                    cell.Value = 0L; // or 0, depending on your property type
            }
            catch { }

            // keep the "permit on double click" behavior (but no longer tied to drag-allowed)
            _editPermit.Add(new CellKey(e.RowIndex, e.ColumnIndex));

            CurrentCell = this[e.ColumnIndex, e.RowIndex];
            BeginEdit(true);
        }

        private void OnCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // merged => read-only (existing behavior)
            CellKey owner;
            if (MergingEnabled && _mergeStore.TryGetOwner(e.RowIndex, e.ColumnIndex, out owner))
            {
                e.Cancel = true;
                return;
            }

            // NEW: readonly comes from CellReadOnlyNeeded + DataGridView flags (not drag rule)
            if (IsCellReadOnlyByRule(e.RowIndex, e.ColumnIndex))
            {
                e.Cancel = true;
                return;
            }

            // ReserveGridView merged: begin edit rule hook (kept)
            if (CellBeginEditRule != null)
            {
                var args = new ReserveCellBeginEditEventArgs(e.RowIndex, e.ColumnIndex, GetRowDataOrNull(e.RowIndex));
                CellBeginEditRule(this, args);
                if (args.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // IMPORTANT CHANGE:
            // Remove "readonly/edit permit" behavior that depended on IsDragAllowedAt.
            // Editing permission is now independent from dragging.
            if (!_editPermit.Contains(new CellKey(e.RowIndex, e.ColumnIndex)))
            {
                // This keeps your old UX: user must double-click to edit.
                // If you want single-click edit allowed, remove this block.
                e.Cancel = true;
                return;
            }
        }

        private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _editPermit.Remove(new CellKey(e.RowIndex, e.ColumnIndex));
        }

        private void OnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // keep existing behavior
            e.Control.BackColor = Color.White;

            // ReserveGridView merged: textbox init + EditingControlRule
            var rowIndex = CurrentCell != null ? CurrentCell.RowIndex : -1;
            var colIndex = CurrentCell != null ? CurrentCell.ColumnIndex : -1;

            if (e.Control is TextBox tb && rowIndex >= 0 && colIndex >= 0)
            {
                try
                {
                    tb.SelectionStart = 0;
                    tb.SelectionLength = tb.TextLength;
                }
                catch { }
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

        private void OnMouseDownDrag(object sender, MouseEventArgs e)
        {
            // Do not start drag selection/move while editing (allows textbox mouse selection)
            if (IsCurrentCellInEditMode) return;

            if (e.Button != MouseButtons.Left) return;
            if (Control.ModifierKeys != Keys.None) return;

            var hit = HitTest(e.X, e.Y);
            if (hit.Type != DataGridViewHitTestType.Cell) return;

            // only year columns participate (drag function)
            if (!IsDragAllowedAt(hit.RowIndex, hit.ColumnIndex)) return;

            _dragging = true;
            Capture = true;

            _normalCursor = this.Cursor;
            _mouseDownPoint = new Point(e.X, e.Y);

            // Start vertical range selection anchored at this cell
            _dragMode = DragMode.VerticalRangeSelect;

            _rangeAnchorRow = hit.RowIndex;
            _rangeEndRow = hit.RowIndex;
            _rangeColumnIndex = hit.ColumnIndex;
            _hasRowRangeSelection = true;

            // Keep your "highlighted row" = first row user pointed to
            SetHighlightedRow(_rangeAnchorRow);

            // reset horizontal selection visuals
            _hasDragSelection = false;

            Invalidate();
        }

        private void OnMouseMoveDrag(object sender, MouseEventArgs e)
        {
            if (IsCurrentCellInEditMode) return;

            if (!_dragging) return;
            if ((Control.MouseButtons & MouseButtons.Left) == 0) return;

            var hit = HitTest(e.X, e.Y);
            if (hit.Type != DataGridViewHitTestType.Cell) return;

            // If user moved to non-year column during our drag: show No
            if (!IsDragAllowedAt(hit.RowIndex, hit.ColumnIndex))
            {
                this.Cursor = Cursors.No;
                _hasDragSelection = false;
                Invalidate();
                return;
            }

            // Selected row range boundaries
            int r1 = Math.Min(_rangeAnchorRow, _rangeEndRow);
            int r2 = Math.Max(_rangeAnchorRow, _rangeEndRow);
            bool inSelectedRowRange = (hit.RowIndex >= r1 && hit.RowIndex <= r2);

            // If we started vertical select, allow vertical motion only in SAME column
            if (_dragMode == DragMode.VerticalRangeSelect)
            {
                if (hit.ColumnIndex == _rangeColumnIndex)
                {
                    // update row range (same column)
                    if (hit.RowIndex != _rangeEndRow)
                    {
                        _rangeEndRow = hit.RowIndex;
                        this.Cursor = Cursors.SizeNS;
                        Invalidate();
                    }
                    return;
                }

                // Column changed:
                // Allow switching to horizontal move if on ANY row in selected row range
                if (inSelectedRowRange)
                {
                    _dragMode = DragMode.HorizontalMove;

                    _activeMoveRow = hit.RowIndex;

                    // Start/end columns are still based on the original selected column and current hit column
                    _dragStart = new CellKey(_activeMoveRow, _rangeColumnIndex);
                    _dragEnd = new CellKey(_activeMoveRow, hit.ColumnIndex);

                    _hasDragSelection = true;
                    UpdateSelectionRange(_dragStart.Col, _dragEnd.Col);

                    // cursor based on dx
                    int dx = e.X - _mouseDownPoint.X;
                    if (dx > 0) this.Cursor = Cursors.PanEast;
                    else if (dx < 0) this.Cursor = Cursors.PanWest;
                    else this.Cursor = _normalCursor ?? Cursors.Default;

                    Invalidate();
                }
                else
                {
                    // not in selected range -> not allowed
                    this.Cursor = Cursors.No;
                    _hasDragSelection = false;
                    Invalidate();
                }

                return;
            }

            // Horizontal move mode
            if (_dragMode == DragMode.HorizontalMove)
            {
                // Must stay inside selected row range
                if (!inSelectedRowRange)
                {
                    this.Cursor = Cursors.No;
                    _hasDragSelection = false;
                    Invalidate();
                    return;
                }

                // Update active row if user moves within the selected range
                if (_activeMoveRow != hit.RowIndex)
                {
                    _activeMoveRow = hit.RowIndex;
                    // keep dragStart/dragEnd row consistent with current row for hit-testing
                    _dragStart = new CellKey(_activeMoveRow, _rangeColumnIndex);
                    _dragEnd = new CellKey(_activeMoveRow, _dragEnd.Col);
                }

                // update end column
                if (hit.ColumnIndex != _dragEnd.Col)
                {
                    _dragEnd = new CellKey(_activeMoveRow, hit.ColumnIndex);
                    UpdateSelectionRange(_dragStart.Col, _dragEnd.Col);
                    Invalidate();
                }

                int dx = e.X - _mouseDownPoint.X;
                if (dx > 0) this.Cursor = Cursors.PanEast;
                else if (dx < 0) this.Cursor = Cursors.PanWest;
                else this.Cursor = _normalCursor ?? Cursors.Default;

                return;
            }
        }

        private void OnMouseUpDrag(object sender, MouseEventArgs e)
        {
            if (IsCurrentCellInEditMode) return;

            if (!_dragging) return;

            _dragging = false;
            Capture = false;

            // restore cursor
            this.Cursor = _normalCursor ?? Cursors.Default;
            _normalCursor = null;

            bool didHorizontalMove = (_dragMode == DragMode.HorizontalMove);

            // clear horizontal visuals; keep row-range selection stored
            _hasDragSelection = false;

            // If a horizontal move happened, fire multi-row event
            if (didHorizontalMove)
            {
                bool canFire =
                    _rangeAnchorRow >= 0 &&
                    _rangeEndRow >= 0 &&
                    _dragStart.Col >= 0 &&
                    _dragEnd.Col >= 0 &&
                    _dragStart.Col != _dragEnd.Col &&
                    IsDragAllowedAt(_dragStart.Row, _dragStart.Col) &&
                    IsDragAllowedAt(_dragEnd.Row, _dragEnd.Col);

                if (canFire)
                {
                    var from = Columns[_dragStart.Col];
                    var to = Columns[_dragEnd.Col];

                    if (RowCellsDragCompleted != null)
                    {
                        int r1 = Math.Min(_rangeAnchorRow, _rangeEndRow);
                        int r2 = Math.Max(_rangeAnchorRow, _rangeEndRow);

                        var dataList = new List<T>();
                        for (int r = r1; r <= r2; r++)
                        {
                            T m;
                            if (TryGetRowModel(r, out m))
                                dataList.Add(m);
                        }

                        RowCellsDragCompleted(this, new RowCellsDragEventArgs(_rangeAnchorRow, _rangeEndRow, from, to, dataList));
                    }
                }
            }

            _dragMode = DragMode.None;

            Invalidate();
        }

        private void UpdateSelectionRange(int colIndexA, int colIndexB)
        {
            if (colIndexA < 0 || colIndexB < 0) return;

            int d1 = Columns[colIndexA].DisplayIndex;
            int d2 = Columns[colIndexB].DisplayIndex;

            _selDisplayColMin = Math.Min(d1, d2);
            _selDisplayColMax = Math.Max(d1, d2);
        }

        // -------- Header paint --------
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (HeaderLayout == null) return;
            if (HeaderLayout.Cells == null || HeaderLayout.Cells.Count == 0) return;

            int headerRowHeight = Math.Max(16, HeaderLayout.HeaderRowHeight);

            foreach (HeaderBandCellByName cell in HeaderLayout.Cells)
            {
                if (cell == null) continue;
                if (cell.ColumnNames == null || cell.ColumnNames.Count == 0) continue;

                // strict resolve
                var cols = new List<DataGridViewColumn>();
                bool ok = true;
                foreach (string name in cell.ColumnNames)
                {
                    DataGridViewColumn c = Columns[name];
                    if (c == null || !c.Visible) { ok = false; break; }
                    cols.Add(c);
                }
                if (!ok) continue;

                Rectangle rect = Rectangle.Empty;
                foreach (DataGridViewColumn c in cols.OrderBy(x => x.DisplayIndex))
                {
                    Rectangle r = GetCellDisplayRectangle(c.Index, -1, true);
                    if (r.Width <= 0 || r.Height <= 0) continue;
                    rect = rect.IsEmpty ? r : Rectangle.Union(rect, r);
                }
                if (rect.IsEmpty) continue;

                int y = rect.Top + cell.BandRow * headerRowHeight;
                int h = cell.BandRowSpan * headerRowHeight;
                Rectangle bandRect = new Rectangle(rect.Left, y, rect.Width, h);

                using (var back = new SolidBrush(cell.BackColor))
                    e.Graphics.FillRectangle(back, bandRect);

                using (var pen = new Pen(cell.BorderColor, Math.Max(1, cell.BorderThickness)))
                    e.Graphics.DrawRectangle(pen, bandRect.X, bandRect.Y, bandRect.Width - 1, bandRect.Height - 1);

                Font font = cell.Font ?? ColumnHeadersDefaultCellStyle.Font ?? Font;

                DrawHeaderTextExplicitLines(
                    e.Graphics,
                    cell.Text ?? "",
                    font,
                    cell.ForeColor,
                    bandRect,
                    center: true);

                // thick right border for band header
                try
                {
                    // if any column in this band is marked, draw a right border at the band's right edge
                    bool bandHasRightBorder = false;
                    foreach (var cn in cell.ColumnNames)
                    {
                        if (_rightBorderColumnNames.Contains(cn))
                        {
                            bandHasRightBorder = true;
                            break;
                        }
                    }

                    if (bandHasRightBorder)
                    {
                        using (var pen = new Pen(Color.Black, 2))
                        {
                            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                            int x = bandRect.Right - 1;
                            e.Graphics.DrawLine(pen, x, bandRect.Top, x, bandRect.Bottom);
                        }
                    }
                }
                catch { }

                // Drag indicator (optional)
                if (_headerBandDragging && _dragBandCell != null)
                {
                    using (var pen = new Pen(Color.Red, 2))
                    {
                        // draw a line at the top edge to hint "drop above to hide"
                        e.Graphics.DrawLine(pen, 0, 0, this.ClientSize.Width, 0);
                    }
                    // draw current mouse location marker
                    using (var b = new SolidBrush(Color.FromArgb(60, Color.Red)))
                    {
                        e.Graphics.FillEllipse(b, _lastMousePoint.X - 6, _lastMousePoint.Y - 6, 12, 12);
                    }
                }
            }
        }

        private static void DrawHeaderTextExplicitLines(
            Graphics g,
            string text,
            Font font,
            Color foreColor,
            Rectangle bounds,
            bool center)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // padding
            Rectangle r = Rectangle.Inflate(bounds, -2, -2);
            if (r.Width <= 0 || r.Height <= 0) return;

            // explicit lines only (no auto wrap)
            string[] lines = text.Replace("\r\n", "\n").Split('\n');

            int lineHeight = TextRenderer.MeasureText(
                g,
                "Ag",
                font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Height;

            if (lineHeight <= 0) lineHeight = font.Height;
            if (lineHeight <= 0) return;

            int maxLinesByHeight = Math.Max(1, r.Height / lineHeight);
            int visibleLines = Math.Min(lines.Length, maxLinesByHeight);
            bool truncatedByHeight = (lines.Length > visibleLines);

            // Vertically center the entire visible block
            int blockHeight = visibleLines * lineHeight;
            int startY = r.Top + Math.Max(0, (r.Height - blockHeight) / 2);

            TextFormatFlags flagsBase =
                TextFormatFlags.SingleLine |
                TextFormatFlags.NoPadding |
                TextFormatFlags.PreserveGraphicsClipping |
                TextFormatFlags.VerticalCenter |      // vertical center per-line rect
                TextFormatFlags.EndEllipsis;          // ellipsis if width not enough

            flagsBase |= center ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;

            for (int i = 0; i < visibleLines; i++)
            {
                bool isLastVisibleLine = (i == visibleLines - 1);

                Rectangle lineRect = new Rectangle(
                    r.Left,
                    startY + i * lineHeight,
                    r.Width,
                    lineHeight);

                string lineText = lines[i] ?? string.Empty;

                // If height is truncated, force a visual hint on the last shown line.
                // EndEllipsis only triggers when width is tight, so we append '…' in this case.
                if (truncatedByHeight && isLastVisibleLine)
                {
                    if (!lineText.EndsWith("…"))
                        lineText += "…";
                }

                TextRenderer.DrawText(g, lineText, font, lineRect, foreColor, flagsBase);
            }
        }

        private void ApplyHeaderHeight()
        {
            if (HeaderLayout == null) return;

            int rows = Math.Max(1, HeaderLayout.HeaderRowCount);
            int rh = Math.Max(16, HeaderLayout.HeaderRowHeight);

            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            ColumnHeadersHeight = rows * rh;
        }

        private void OnScrollInvalidate(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        private void OnColumnLayoutChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private static List<string> NormalizeDistinct(IEnumerable<string> names)
        {
            var ret = new List<string>();
            if (names == null) return ret;

            foreach (string s in names)
            {
                if (string.IsNullOrWhiteSpace(s)) continue;
                if (!ret.Contains(s)) ret.Add(s);
            }
            return ret;
        }

        private bool IsDragAllowedAt(int rowIndex, int colIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowCount) return false;
            if (colIndex < 0 || colIndex >= ColumnCount) return false;

            var col = Columns[colIndex];
            if (col == null || !col.Visible) return false;

            // need model to apply business rules
            T model;
            if (!TryGetRowModel(rowIndex, out model)) return false;

            // custom business rules hook
            if (CanDragCell != null)
                return CanDragCell(this, rowIndex, col, model);

            return false;
        }

        private void ApplyStyleNeededToCellStyle(int rowIndex, int columnIndex, object valueForRule, DataGridViewCellStyle style)
        {
            if (style == null) return;

            // Apply programmatic overrides (same behavior as OnCellFormatting)
            try
            {
                DataGridViewColumn col = (columnIndex >= 0 && columnIndex < Columns.Count) ? Columns[columnIndex] : null;
                string colName = col != null ? col.Name : null;

                if (!string.IsNullOrEmpty(colName))
                {
                    CellStyleOverride ov;
                    if (_cellStyleOverrides.TryGetValue(Tuple.Create(rowIndex, colName), out ov) && ov != null)
                    {
                        if (ov.BackColor.HasValue) style.BackColor = ov.BackColor.Value;
                        if (ov.ForeColor.HasValue) style.ForeColor = ov.ForeColor.Value;
                        if (ov.Font != null) style.Font = ov.Font;
                    }
                }
            }
            catch { }

            // Apply CellStyleNeeded rule so OnCellPainting sees the same colors as OnCellFormatting
            try
            {
                if (CellStyleNeeded != null)
                {
                    bool isCurrentCell = (CurrentCell != null &&
                                          CurrentCell.RowIndex == rowIndex &&
                                          CurrentCell.ColumnIndex == columnIndex);

                    bool isReadOnly = IsCellReadOnlyByRule(rowIndex, columnIndex);

                    var args = new ReserveCellStyleNeededEventArgs(
                        rowIndex,
                        columnIndex,
                        GetRowDataOrNull(rowIndex),
                        valueForRule,
                        isCurrentCell,
                        isReadOnly);

                    CellStyleNeeded(this, args);

                    if (args.BackColor.HasValue) style.BackColor = args.BackColor.Value;
                    if (args.ForeColor.HasValue) style.ForeColor = args.ForeColor.Value;
                }
            }
            catch { }

            // keep LongRepairGridView behavior: always neutral selection colors
            style.SelectionBackColor = style.BackColor;
            style.SelectionForeColor = style.ForeColor;
        }

        private ReserveButtonCellStyleNeededEventArgs RaiseButtonCellStyleNeeded(int rowIndex, int columnIndex, object value)
        {
            if (ButtonCellStyleNeeded == null) return null;
            if (rowIndex < 0 || columnIndex < 0) return null;
            if (rowIndex >= Rows.Count || columnIndex >= Columns.Count) return null;

            var btnCol = Columns[columnIndex] as DataGridViewButtonColumn;
            if (btnCol == null) return null;

            bool isCurrentCell = (CurrentCell != null &&
                                  CurrentCell.RowIndex == rowIndex &&
                                  CurrentCell.ColumnIndex == columnIndex);

            bool isReadOnly = IsCellReadOnlyByRule(rowIndex, columnIndex);

            var args = new ReserveButtonCellStyleNeededEventArgs(
                rowIndex,
                columnIndex,
                GetRowDataOrNull(rowIndex),
                value,
                isCurrentCell,
                isReadOnly,
                btnCol);

            ButtonCellStyleNeeded(this, args);
            return args;
        }

        private static System.Drawing.Drawing2D.GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color Blend(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            int r = (int)(a.R + (b.R - a.R) * t);
            int g = (int)(a.G + (b.G - a.G) * t);
            int bl = (int)(a.B + (b.B - a.B) * t);
            return Color.FromArgb(255, r, g, bl);
        }

        private void DrawRoundedGradientButton(
            DataGridViewCellPaintingEventArgs e,
            string text,
            Color baseBack,
            Color fore,
            bool disabledStyle,
            bool hovered,
            bool pressed,
            int radius = 4)
        {
            e.PaintBackground(e.ClipBounds, true);
            e.Paint(e.ClipBounds, DataGridViewPaintParts.Border);

            // Bigger button (less padding)
            int padX = 1;
            int padY = 1;
            Rectangle r = Rectangle.Inflate(e.CellBounds, -padX, -padY);

            if (r.Width <= 6 || r.Height <= 6)
            {
                e.Handled = true;
                return;
            }

            // Prevent pill shape: cap radius by height
            // For ~23px row height: (23-2)/5 ≈ 4
            int maxByHeight = Math.Max(3, (r.Height - 2) / 5);
            radius = Math.Max(3, Math.Min(radius, maxByHeight));

            // Disabled overrides (visual only)
            if (disabledStyle)
            {
                baseBack = Color.Gainsboro;
                fore = SystemColors.GrayText;
            }

            // Hover: slightly brighter
            if (!disabledStyle && hovered)
                baseBack = Blend(baseBack, Color.White, 0.15f);

            // Pressed: slightly darker + invert gradient
            if (!disabledStyle && pressed)
                baseBack = Blend(baseBack, Color.Black, 0.08f);

            // Stronger 3-stop gradient
            Color top = disabledStyle ? Color.Gainsboro : Blend(baseBack, Color.White, 0.78f);
            Color mid = disabledStyle ? Color.Silver : Blend(baseBack, Color.White, 0.25f);
            Color bottom = disabledStyle ? Color.Silver : Blend(baseBack, Color.FromArgb(185, 185, 185), 0.35f);

            if (pressed && !disabledStyle)
            {
                var tmp = top; top = bottom; bottom = tmp;
            }

            // Border colors (keep your current good-looking blue border + hover thickness)
            Color border = disabledStyle ? Color.Gray : Color.FromArgb(60, 120, 215);
            Color innerBorder = disabledStyle ? Color.DarkGray : Color.FromArgb(140, Color.White);

            var oldSmoothing = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            try
            {
                using (var path = GetRoundedRect(r, radius))
                {
                    // Fill with a 3-stop gradient (top->mid->bottom)
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(r, top, bottom, 90f))
                    {
                        var blend = new System.Drawing.Drawing2D.ColorBlend
                        {
                            Colors = new[] { top, mid, bottom },
                            Positions = new[] { 0f, 0.55f, 1f }
                        };
                        brush.InterpolationColors = blend;

                        e.Graphics.FillPath(brush, path);
                    }

                    // Outer border
                    using (var pen = new Pen(border, hovered && !disabledStyle ? 2f : 1f))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }

                    // Inner border (crispness)
                    Rectangle inner = Rectangle.Inflate(r, -2, -2);
                    using (var innerPath = GetRoundedRect(inner, Math.Max(2, radius - 2)))
                    using (var pen2 = new Pen(innerBorder, 1f))
                    {
                        e.Graphics.DrawPath(pen2, innerPath);
                    }

                    // Top shine band
                    if (!disabledStyle)
                    {
                        Rectangle shine = new Rectangle(r.X + 2, r.Y + 2, r.Width - 4, (r.Height - 4) / 2);
                        using (var shinePath = GetRoundedRect(shine, Math.Max(2, radius - 2)))
                        using (var sb = new SolidBrush(Color.FromArgb(70, Color.White)))
                        {
                            e.Graphics.FillPath(sb, shinePath);
                        }
                    }
                }
            }
            finally
            {
                e.Graphics.SmoothingMode = oldSmoothing;
            }

            TextRenderer.DrawText(
                e.Graphics,
                text ?? "",
                e.CellStyle.Font ?? Font,
                r,
                fore,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.SingleLine);

            e.Handled = true;
        }

        private void OnCellMouseMoveButtonHover(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            bool isBtn = false;
            try { isBtn = Columns[e.ColumnIndex] is DataGridViewButtonColumn; } catch { }

            var newKey = isBtn ? new CellKey(e.RowIndex, e.ColumnIndex) : new CellKey(-1, -1);
            if (!_hoverButtonCell.Equals(newKey))
            {
                var old = _hoverButtonCell;
                _hoverButtonCell = newKey;

                if (old.Row >= 0 && old.Col >= 0) InvalidateCell(old.Col, old.Row);
                if (_hoverButtonCell.Row >= 0 && _hoverButtonCell.Col >= 0) InvalidateCell(_hoverButtonCell.Col, _hoverButtonCell.Row);
            }
        }

        private void OnCellMouseLeaveButtonHover(object sender, DataGridViewCellEventArgs e)
        {
            if (_hoverButtonCell.Row >= 0 && _hoverButtonCell.Col >= 0)
            {
                var old = _hoverButtonCell;
                _hoverButtonCell = new CellKey(-1, -1);
                InvalidateCell(old.Col, old.Row);
            }
        }

        private void OnMouseDownButtonPress(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var hit = HitTest(e.X, e.Y);
            if (hit.Type != DataGridViewHitTestType.Cell) return;

            if (!(Columns[hit.ColumnIndex] is DataGridViewButtonColumn)) return;

            _pressedButtonCell = new CellKey(hit.RowIndex, hit.ColumnIndex);
            InvalidateCell(hit.ColumnIndex, hit.RowIndex);
        }

        private void OnMouseUpButtonPress(object sender, MouseEventArgs e)
        {
            if (_pressedButtonCell.Row >= 0 && _pressedButtonCell.Col >= 0)
            {
                var old = _pressedButtonCell;
                _pressedButtonCell = new CellKey(-1, -1);
                InvalidateCell(old.Col, old.Row);
            }
        }

        /// <summary>
        /// Hide columns by name. Ignores names that don't exist.
        /// </summary>
        public void HideColumns(IEnumerable<string> columnNames)
        {
            if (columnNames == null) return;

            var names = columnNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (names.Count == 0) return;

            foreach (var name in names)
            {
                if (this.Columns.Contains(name))
                    this.Columns[name].Visible = false;
            }

            _lastHiddenColumnNames = names;
            Invalidate();
        }

        /// <summary>
        /// Restore columns by name (set Visible=true).
        /// </summary>
        public void RestoreColumns(IEnumerable<string> columnNames)
        {
            if (columnNames == null) return;

            var names = columnNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (names.Count == 0) return;

            foreach (var name in names)
            {
                if (this.Columns.Contains(name))
                    this.Columns[name].Visible = true;
            }

            Invalidate();
        }

        /// <summary>
        /// Restore columns hidden by the last drag-to-hide action.
        /// </summary>
        public void RestoreLastHiddenColumns()
        {
            RestoreColumns(_lastHiddenColumnNames);
        }

        private bool TryHitTestHeaderBandCell(Point clientPoint, out HeaderBandCellByName cell, out Rectangle bandRect)
        {
            cell = null;
            bandRect = Rectangle.Empty;

            if (HeaderLayout == null || HeaderLayout.Cells == null || HeaderLayout.Cells.Count == 0)
                return false;

            // Must be inside header area
            if (clientPoint.Y < 0 || clientPoint.Y > this.ColumnHeadersHeight)
                return false;

            int headerRowHeight = Math.Max(16, HeaderLayout.HeaderRowHeight);

            foreach (var c in HeaderLayout.Cells)
            {
                if (c == null || c.ColumnNames == null || c.ColumnNames.Count == 0) continue;

                // resolve columns (skip hidden columns)
                var cols = new List<DataGridViewColumn>();
                bool ok = true;
                foreach (string name in c.ColumnNames)
                {
                    if (!this.Columns.Contains(name)) { ok = false; break; }
                    var col = this.Columns[name];
                    if (col == null || !col.Visible) { ok = false; break; }
                    cols.Add(col);
                }
                if (!ok || cols.Count == 0) continue;

                // union rect across columns (header row = -1)
                Rectangle rect = Rectangle.Empty;
                foreach (var col in cols.OrderBy(x => x.DisplayIndex))
                {
                    Rectangle r = GetCellDisplayRectangle(col.Index, -1, true);
                    if (r.Width <= 0 || r.Height <= 0) continue;
                    rect = rect.IsEmpty ? r : Rectangle.Union(rect, r);
                }
                if (rect.IsEmpty) continue;

                int y = rect.Top + c.BandRow * headerRowHeight;
                int h = c.BandRowSpan * headerRowHeight;

                var rBand = new Rectangle(rect.Left, y, rect.Width, h);

                if (rBand.Contains(clientPoint))
                {
                    cell = c;
                    bandRect = rBand;
                    return true;
                }
            }

            return false;
        }

        private bool IsPointAboveGridTop(Point clientPoint)
        {
            // "outside" condition requested: drag to outside/top of grid
            return clientPoint.Y < 0;
        }

        private void OnMouseDownHeaderBandDrag(object sender, MouseEventArgs e)
        {
            // Only left mouse, and only when not editing
            if (IsCurrentCellInEditMode) return;
            if (e.Button != MouseButtons.Left) return;

            // Only allow starting drag in the header area over a band cell
            HeaderBandCellByName hitCell;
            Rectangle hitRect;
            if (!TryHitTestHeaderBandCell(e.Location, out hitCell, out hitRect))
                return;

            _headerBandDragging = true;
            _dragBandCell = hitCell;
            _dragStartPoint = e.Location;
            _lastMousePoint = e.Location;

            // capture mouse so we keep receiving move/up even when outside
            this.Capture = true;

            // Optional: change cursor to indicate dragging
            this.Cursor = Cursors.SizeAll;

            Invalidate(); // to draw drag indicator if you want
        }

        private void OnMouseMoveHeaderBandDrag(object sender, MouseEventArgs e)
        {
            if (!_headerBandDragging) return;

            _lastMousePoint = e.Location;

            // Cursor feedback
            if (IsPointAboveGridTop(e.Location))
                this.Cursor = Cursors.No;     // means "drop here to hide"
            else
                this.Cursor = Cursors.SizeAll;

            Invalidate();
        }

        private void OnMouseUpHeaderBandDrag(object sender, MouseEventArgs e)
        {
            if (!_headerBandDragging) return;

            _headerBandDragging = false;
            this.Capture = false;
            this.Cursor = Cursors.Default;

            var cell = _dragBandCell;
            _dragBandCell = null;

            if (cell == null) { Invalidate(); return; }

            // If dropped above the grid => hide
            if (IsPointAboveGridTop(e.Location))
            {
                // Hide all columns in that band cell
                HideColumns(cell.ColumnNames);
            }

            Invalidate();
        }

        /// <summary>
        /// Set column names that should have a thick black RIGHT border (header + cells).
        /// </summary>
        public void SetRightBorderColumns(IEnumerable<string> columnNames)
        {
            _rightBorderColumnNames.Clear();
            if (columnNames != null)
            {
                foreach (var n in columnNames)
                {
                    if (!string.IsNullOrWhiteSpace(n))
                        _rightBorderColumnNames.Add(n);
                }
            }
            Invalidate();
        }

        private bool HasRightBorder(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Columns.Count) return false;
            var c = Columns[columnIndex];
            if (c == null) return false;
            return _rightBorderColumnNames.Contains(c.Name);
        }

        private void PaintRightBorderIfNeeded(DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (!HasRightBorder(e.ColumnIndex)) return;

                // draw on top of everything
                int thickness = 2; // adjust if you want thicker
                int x = e.CellBounds.Right - 1;

                using (var pen = new Pen(Color.Black, thickness))
                {
                    // make sure the line is inside the cell bounds
                    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    e.Graphics.DrawLine(pen, x, e.CellBounds.Top, x, e.CellBounds.Bottom);
                }
            }
            catch { }
        }
    }
}