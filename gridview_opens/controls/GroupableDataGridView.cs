using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using Zuby.ADGV;

namespace gridview_opens.controls
{
    public class GridViewStyle
    {
        public Color RowBackColor { get; set; } = Color.White;
        public Color AlternatingRowBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color RowTextColor { get; set; } = Color.Black;

        public Color SelectedBackColor { get; set; } = Color.FromArgb(51, 153, 255);
        public Color SelectedTextColor { get; set; } = Color.White;

        public Color GroupRowBackColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color GroupRowTextColor { get; set; } = Color.Black;

        public Color HoverBackColor { get; set; } = Color.FromArgb(235, 245, 255);

        public Color CellBorderColor { get; set; } = Color.LightGray;
        public Color FocusedCellBorderColor { get; set; } = Color.DodgerBlue;
    }

    public class GroupRowInfo
    {
        public string GroupKey { get; set; }
        public int GroupHeaderRowIndex { get; set; }
        public List<int> MemberRowIndexes { get; set; } = new List<int>();
        public bool Collapsed { get; set; } = false;
    }

    public class CellMergeEventArgs : EventArgs
    {
        public int ColumnIndex { get; }
        public int RowIndex1 { get; }
        public int RowIndex2 { get; }
        public bool Handled { get; set; }
        public bool Merge { get; set; }

        public CellMergeEventArgs(int columnIndex, int rowIndex1, int rowIndex2)
        {
            ColumnIndex = columnIndex;
            RowIndex1 = rowIndex1;
            RowIndex2 = rowIndex2;
        }
    }

    [Designer("System.Windows.Forms.Design.DataGridViewDesigner, System.Design", typeof(IDesigner))]
    public partial class GroupableDataGridView : AdvancedDataGridView
    {
        private HitTestInfo _lastHitTest;
        public GridViewStyle StyleSettings { get; set; } = new GridViewStyle();
        [Category("Appearance")]
        [Description("行の背景を交互に色分けするかどうかを指定します。")]
        [DefaultValue(false)]
        public bool UsingSeparateRowStyle { get; set; } = false;

        [Category("Appearance")]
        [Description("選択行の背景スタイルを使用するかどうかを指定します。")]
        [DefaultValue(false)]
        public bool UsingRowSelectedStyle { get; set; } = false;

        [Category("Appearance")]
        [Description("マウスオーバー行の背景スタイルを使用するかどうかを指定します。")]
        [DefaultValue(false)]
        public bool UsingRowMouseOverStyle { get; set; } = false;
        [Category("Appearance")]
        [Description("セル選択の枠フラグ。")]
        [DefaultValue(true)]
        public bool DrawCellBorderOnSelect { get; set; } = true;
        public event Action<DataGridViewRow> ApplyRowStyle;
        public event EventHandler<CellMergeEventArgs> CellMerge;

        private string _groupColumn = null;
        private readonly List<GroupRowInfo> _groups = new List<GroupRowInfo>();
        private int _hoverRowIndex = -1;
        private Image _filterIcon = Properties.Resources.arrow_down_blue;
        private int _hoverColumnIndex = -1;

        private ContextMenuStrip headerContext;
        private ToolStripMenuItem miHideColumn;
        private ToolStripMenuItem miShowAllColumns;
        private ToolStripMenuItem miColumnChooser;

        private int _insertionMarkIndex = -1;
        private int _insertionMarkX = -1;

        private IEnumerable<object> _originalListData;
        private string _lastFilterString = "";
        private string _lastSortString = "";

        private string _lastSortedColumnName = null;
        private ListSortDirection? _lastSortDirection = null;

        private Point _dragStartPoint;
        private bool _draggingStarted = false;

        private List<string> _visibleColumnOrder = new List<string>();
        private bool _suppressColumnReorder = false;

        private HashSet<string> _filteredColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Button _clearFilterButton;

        public GroupableDataGridView()
        {
            DoubleBuffered = true;
            EnableHeadersVisualStyles = false;
            RowHeadersVisible = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AllowUserToAddRows = false;
            AllowDrop = true;
            AllowUserToOrderColumns = true;
            AutoGenerateColumns = false;
            this.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            this.DragDrop += GroupableDataGridView_DragDrop;
            this.EnableHeadersVisualStyles = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ColumnHeadersHeight = 30;

            this.DragOver += GroupableDataGridView_DragOver;
            this.Paint += GroupableDataGridView_Paint;

            this.CellPainting += GroupableDataGridView_CellPainting;
            this.CellMouseEnter += GroupableDataGridView_CellMouseEnter;
            this.CellMouseLeave += GroupableDataGridView_CellMouseLeave;
            this.CellMouseClick += GroupableDataGridView_CellMouseClick;
            //this.ColumnHeaderMouseClick += GroupableDataGridView_ColumnHeaderMouseClick;
            this.DataBindingComplete += GroupableDataGridView_DataBindingComplete;

            this.ColumnDisplayIndexChanged += GroupableDataGridView_ColumnDisplayIndexChanged;
            this.ColumnStateChanged += GroupableDataGridView_ColumnStateChanged;
            this.initialOtherControl();
        }

        private void initialOtherControl()
        {
            _clearFilterButton = new Button();
            _clearFilterButton.Text = "フィルターを解除";
            _clearFilterButton.Font = new Font("MS UI Gothic", 9f, FontStyle.Regular);
            _clearFilterButton.Visible = false;
            _clearFilterButton.AutoSize = true;
            _clearFilterButton.BackColor = Color.FromArgb(240, 240, 240);
            _clearFilterButton.FlatStyle = FlatStyle.Flat;
            _clearFilterButton.FlatAppearance.BorderColor = Color.Gray;
            _clearFilterButton.Cursor = Cursors.Hand;

            _clearFilterButton.ImageAlign = ContentAlignment.MiddleLeft;
            _clearFilterButton.TextAlign = ContentAlignment.MiddleRight;
            _clearFilterButton.TextImageRelation = TextImageRelation.ImageBeforeText;

            _clearFilterButton.Image = Properties.Resources.delete2_16;

            _clearFilterButton.Click += (s, e) =>
            {
                ClearAllFilters();
            };
            this.Controls.Add(_clearFilterButton);
            this.Resize += (s, e) => PositionClearFilterButton();
            this.Scroll += (s, e) => PositionClearFilterButton();
        }

        public void ClearAllFilters()
        {
            try
            {
                this.CleanFilterAndSort();

                _filteredColumns.Clear();
                _lastFilterString = "";
                _lastSortString = "";

                ApplyListFilterAndSort();

                _clearFilterButton.Visible = false;
                this.Refresh();
            }
            catch { }
        }

        private void UpdateClearFilterButtonVisibility()
        {
            _clearFilterButton.Visible = _filteredColumns.Count > 0;
            if (_clearFilterButton.Visible)
                PositionClearFilterButton();
        }

        private void PositionClearFilterButton()
        {
            if (_clearFilterButton == null) return;
            int x = 5;
            int y = this.Height - _clearFilterButton.Height - 5;
            _clearFilterButton.Location = new Point(x, y);
            _clearFilterButton.BringToFront();
        }

        private void GroupableDataGridView_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (_suppressColumnReorder) return;
            UpdateVisibleColumnOrder();
        }

        private void GroupableDataGridView_ColumnStateChanged(object sender, DataGridViewColumnStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Visible)
            {
                UpdateVisibleColumnOrder();
                ReapplyColumnDisplayOrder();
            }
        }

        private void UpdateVisibleColumnOrder()
        {
            _visibleColumnOrder = this.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .Select(c => c.Name)
                .ToList();
        }

        protected override void OnCellMouseMove(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseMove(e);

            if (e.RowIndex == -1) // header row
            {
                if (_hoverColumnIndex != e.ColumnIndex)
                {
                    _hoverColumnIndex = e.ColumnIndex;
                    this.Invalidate(); // redraw header
                }
            }
            else
            {
                if (_hoverColumnIndex != -1)
                {
                    _hoverColumnIndex = -1;
                    this.Invalidate();
                }
            }
        }

        protected override void OnCellMouseLeave(DataGridViewCellEventArgs e)
        {
            base.OnCellMouseLeave(e);

            if (e.RowIndex == -1 && _hoverColumnIndex != -1)
            {
                _hoverColumnIndex = -1;
                this.Invalidate();
            }
        }

        private void ReapplyColumnDisplayOrder()
        {
            if (_suppressColumnReorder) return;
            _suppressColumnReorder = true;

            int idx = 0;
            foreach (var colName in _visibleColumnOrder)
            {
                if (this.Columns.Contains(colName))
                {
                    try
                    {
                        this.Columns[colName].DisplayIndex = idx++;
                    }
                    catch { /* ignore out of range */ }
                }
            }

            _suppressColumnReorder = false;
        }

        private void GroupableDataGridView_DragOver(object sender, DragEventArgs e)
        {
            Point clientPoint = this.PointToClient(new Point(e.X, e.Y));
            var hit = this.HitTest(clientPoint.X, clientPoint.Y);

            if (hit.Type == DataGridViewHitTestType.ColumnHeader && hit.ColumnIndex >= 0)
            {
                var hitCol = this.Columns[hit.ColumnIndex];
                _insertionMarkIndex = hitCol.DisplayIndex;
                Rectangle rect = this.GetCellDisplayRectangle(hit.ColumnIndex, -1, true);

                if (clientPoint.X > rect.Left + rect.Width / 2)
                {
                    _insertionMarkIndex++;
                    _insertionMarkX = rect.Right;
                }
                else
                {
                    _insertionMarkX = rect.Left;
                }
            }
            else
            {
                _insertionMarkIndex = -1;
                _insertionMarkX = -1;
            }

            this.Invalidate();
        }


        private void GroupableDataGridView_Paint(object sender, PaintEventArgs e)
        {
            if (_insertionMarkIndex < 0 || _insertionMarkX < 0)
                return;

            using (var pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawLine(pen,
                    _insertionMarkX,
                    2,
                    _insertionMarkX,
                    this.ColumnHeadersHeight - 2);
            }
        }

        private void GroupableDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            string colName = null;

            if (e.Data.GetDataPresent(typeof(string)))
                colName = e.Data.GetData(typeof(string)) as string;
            else if (e.Data.GetDataPresent(typeof(DataGridViewColumn)))
                colName = ((DataGridViewColumn)e.Data.GetData(typeof(DataGridViewColumn))).Name;

            if (string.IsNullOrEmpty(colName)) return;
            if (!this.Columns.Contains(colName)) return;

            var col = this.Columns[colName];
            col.Visible = true;

            var visibleCols = this.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible && c != col)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            int targetDisplayIndex = visibleCols.Count; // 最終に列を戻す
            if (_insertionMarkIndex >= 0 && _insertionMarkIndex < visibleCols.Count)
            {
                var targetCol = visibleCols[_insertionMarkIndex];
                targetDisplayIndex = targetCol.DisplayIndex;
            }

            try
            {
                col.DisplayIndex = targetDisplayIndex;
            }
            catch { }

            // 位置赤い線を消すの設定
            _insertionMarkIndex = -1;
            _insertionMarkX = -1;
            this.Invalidate();

            // Cập nhật popup
            var parentForm = this.FindForm();
            if (parentForm is Form1 form1 && form1._chooserPopup != null)
                form1._chooserPopup.RemoveColumnByName(colName);

            UpdateVisibleColumnOrder();
            ReapplyColumnDisplayOrder();
            this.Refresh();
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnColumnHeaderMouseClick(e);

            if (e.Button != MouseButtons.Left || e.ColumnIndex < 0) return;

            var col = this.Columns[e.ColumnIndex];
            if (col == null) return;

            // 列のDataPropertyNameまたはName
            var propName = string.IsNullOrEmpty(col.DataPropertyName) ? col.Name : col.DataPropertyName;

            // ソート
            ListSortDirection newDirection;
            if (string.Equals(_lastSortedColumnName, propName, StringComparison.OrdinalIgnoreCase))
            {
                // toggle
                newDirection = _lastSortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                newDirection = ListSortDirection.Ascending;
            }

            // glyph更新
            foreach (DataGridViewColumn c in this.Columns)
                c.HeaderCell.SortGlyphDirection = SortOrder.None;

            col.HeaderCell.SortGlyphDirection = (newDirection == ListSortDirection.Ascending)
                ? SortOrder.Ascending
                : SortOrder.Descending;

            // 現在状態保存
            _lastSortedColumnName = propName;
            _lastSortDirection = newDirection;

            // ソートクエリ
            _lastSortString = $"[{propName}] {(newDirection == ListSortDirection.Ascending ? "ASC" : "DESC")}";

            // ソート実装
            ApplyListFilterAndSort();
        }
        #region Public API
        /// <summary>
        /// Group the grid by the specified column name.
        /// </summary>
        public void GroupBy(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) ClearGrouping();
            if (!this.Columns.Contains(columnName)) throw new ArgumentException("Column not found", nameof(columnName));

            _groupColumn = columnName;
            RebuildGrouping();
        }

        public void ClearGrouping()
        {
            _groupColumn = null;
            _groups.Clear();
            // Restore original DataSource if bound to DataTable or BindingSource
            RebindDataSourceWithoutGrouping();
        }

        /// <summary>
        /// Toggle collapse/expand for a given group key.
        /// </summary>
        public void ToggleGroup(string key)
        {
            var g = _groups.FirstOrDefault(x => x.GroupKey == key);
            if (g == null) return;
            g.Collapsed = !g.Collapsed;
            ApplyCollapseState(g);
        }
        #endregion

        #region Binding helpers
        private object _origDataSource = null;
        private void RebindDataSourceWithoutGrouping()
        {
            // If the grid is bound to DataTable via BindingSource, try to reassign original data source
            if (_origDataSource != null)
            {
                this.DataSource = null;
                this.Rows.Clear();
                this.DataSource = _origDataSource;
            }
        }

        private void CaptureOriginalDataSourceIfNeeded()
        {
            if (_origDataSource == null)
            {
                _origDataSource = this.DataSource;
            }
        }
        #endregion

        #region Grouping Implementation
        private void RebuildGrouping()
        {
            if (string.IsNullOrEmpty(_groupColumn)) return;
            CaptureOriginalDataSourceIfNeeded();

            // Only support data from DataTable (or BindingSource -> DataTable) or unbound rows
            DataTable dt = ExtractDataTableFromDataSource();
            if (dt == null)
            {
                // fallback: if unbound (Rows already present), group on current rows
                BuildGroupsFromCurrentRows();
                return;
            }

            // Build groups
            var rows = dt.AsEnumerable().OrderBy(r => r[_groupColumn]).ToList();
            this.DataSource = null;
            this.Rows.Clear();
            _groups.Clear();

            int currentDisplayRowIndex = 0;
            string currentKey = null;
            GroupRowInfo currentGroup = null;

            foreach (var dr in rows)
            {
                string key = dr[_groupColumn]?.ToString() ?? "(null)";
                if (currentKey == null || key != currentKey)
                {
                    // create group header row
                    var headerRow = new DataGridViewRow();
                    headerRow.CreateCells(this);
                    headerRow.ReadOnly = true;
                    headerRow.DefaultCellStyle.BackColor = StyleSettings.GroupRowBackColor;
                    headerRow.DefaultCellStyle.ForeColor = StyleSettings.GroupRowTextColor;
                    headerRow.Tag = new { IsGroup = true, GroupKey = key };

                    // set first cell text as group title, leave others empty
                    headerRow.Cells[0].Value = $"▾ {key}"; // default expanded
                    this.Rows.Add(headerRow);

                    currentGroup = new GroupRowInfo
                    {
                        GroupKey = key,
                        GroupHeaderRowIndex = this.Rows.Count - 1,
                        Collapsed = false
                    };
                    _groups.Add(currentGroup);
                    currentKey = key;
                    currentDisplayRowIndex++;
                }

                // create data row
                var dataRow = new DataGridViewRow();
                dataRow.CreateCells(this);
                for (int c = 0; c < dt.Columns.Count && c < this.Columns.Count; c++)
                {
                    var colName = dt.Columns[c].ColumnName;
                    if (this.Columns.Contains(colName))
                        dataRow.Cells[this.Columns[colName].Index].Value = dr[colName];
                    else
                        dataRow.Cells[c].Value = dr[c];
                }

                dataRow.Tag = new { IsGroup = false, GroupKey = currentKey };
                this.Rows.Add(dataRow);
                currentGroup.MemberRowIndexes.Add(this.Rows.Count - 1);
                currentDisplayRowIndex++;
            }

            // Repaint & style
            this.Invalidate();
        }

        private DataTable ExtractDataTableFromDataSource()
        {
            if (this.DataSource == null) return null;
            if (this.DataSource is BindingSource bs)
            {
                if (bs.DataSource is DataTable dt) return dt;
                if (bs.DataSource is DataView dv) return dv.Table;
                if (bs.Current is DataRowView drv) return drv.Row.Table;
            }
            if (this.DataSource is DataTable dt2) return dt2;
            if (this.DataSource is DataView dv2) return dv2.Table;
            return null;
        }

        private void BuildGroupsFromCurrentRows()
        {
            // assume the grid currently has Rows with values, group them
            _groups.Clear();
            if (this.RowCount == 0) return;

            string currKey = null;
            GroupRowInfo currentGroup = null;
            var allRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow r in this.Rows)
                allRows.Add(r);

            this.DataSource = null;
            this.Rows.Clear();

            foreach (var r in allRows)
            {
                var cell = r.Cells[_groupColumn];
                string key = cell?.Value?.ToString() ?? "(null)";
                if (currKey == null || key != currKey)
                {
                    var header = new DataGridViewRow();
                    header.CreateCells(this);
                    header.ReadOnly = true;
                    header.DefaultCellStyle.BackColor = StyleSettings.GroupRowBackColor;
                    header.DefaultCellStyle.ForeColor = StyleSettings.GroupRowTextColor;
                    header.Tag = new { IsGroup = true, GroupKey = key };
                    header.Cells[0].Value = $"▾ {key}";
                    this.Rows.Add(header);

                    currentGroup = new GroupRowInfo
                    {
                        GroupKey = key,
                        GroupHeaderRowIndex = this.Rows.Count - 1,
                        Collapsed = false
                    };
                    _groups.Add(currentGroup);
                    currKey = key;
                }

                var dataRow = new DataGridViewRow();
                dataRow.CreateCells(this);
                for (int i = 0; i < r.Cells.Count && i < this.Columns.Count; i++)
                    dataRow.Cells[i].Value = r.Cells[i].Value;

                dataRow.Tag = new { IsGroup = false, GroupKey = currKey };
                this.Rows.Add(dataRow);
                currentGroup.MemberRowIndexes.Add(this.Rows.Count - 1);
            }
        }

        private void ApplyCollapseState(GroupRowInfo g)
        {
            foreach (var rowIdx in g.MemberRowIndexes)
            {
                if (rowIdx >= 0 && rowIdx < this.Rows.Count)
                    this.Rows[rowIdx].Visible = !g.Collapsed;
            }

            // update header symbol
            if (g.GroupHeaderRowIndex >= 0 && g.GroupHeaderRowIndex < this.Rows.Count)
            {
                var hr = this.Rows[g.GroupHeaderRowIndex];
                if (hr != null)
                {
                    hr.Cells[0].Value = (g.Collapsed ? "▸ " : "▾ ") + g.GroupKey;
                }
            }
            this.Invalidate();
        }
        #endregion

        #region Events & Painting
        private void GroupableDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // When binding completes and grouping is set, rebuild grouping
            if (!string.IsNullOrEmpty(_groupColumn))
                RebuildGrouping();
        }

        private void GroupableDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = this.Rows[e.RowIndex];
            if (row.Tag is null) return;
            var tagObj = row.Tag as dynamic;
            bool isGroup = false;
            try { isGroup = tagObj.IsGroup; } catch { isGroup = false; }

            if (isGroup)
            {
                // toggle collapse on header click
                string key = tagObj.GroupKey;
                ToggleGroup(key);
            }
        }

        private void GroupableDataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _hoverRowIndex = e.RowIndex;
                this.InvalidateRow(e.RowIndex);
            }
        }

        private void GroupableDataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            _hoverRowIndex = -1;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // ADG

            for (int i = 0; i < this.Columns.Count; i++)
            {
                var rect = this.GetCellDisplayRectangle(i, -1, true);
                if (rect.Width <= 0 || rect.Height <= 0) continue;

                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                           rect, Color.White, Color.LightGray, 90f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                using (var pen = new Pen(Color.Gray))
                {
                    e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }

                bool isFiltered = false;
                string colName = this.Columns[i].DataPropertyName;
                if (String.IsNullOrEmpty(colName))
                    colName = this.Columns[i].Name;
                if (!String.IsNullOrEmpty(colName) && _filteredColumns.Contains(colName))
                    isFiltered = true;

                if (i == _hoverColumnIndex)
                {
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(254, 211, 102)))
                    {
                        e.Graphics.FillRectangle(hoverBrush, rect);
                    }
                }
                if (isFiltered || i == _hoverColumnIndex)
                {
                    if (_filterIcon != null)
                    {
                        int iconSize = rect.Height - 6;
                        e.Graphics.DrawImage(_filterIcon,
                                             rect.Right - iconSize - 4,
                                             rect.Top + 3,
                                             iconSize, iconSize);
                    }
                }
                TextRenderer.DrawText(
                    e.Graphics,
                    this.Columns[i].HeaderText,
                    this.Columns[i].HeaderCell.Style.Font ?? this.Font,
                    rect,
                    this.Columns[i].HeaderCell.Style.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
        }

        private void GroupableDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return; // skip header

            var row = this.Rows[e.RowIndex];
            bool isGroupRow = false;
            string groupKey = null;
            if (row.Tag != null)
            {
                try
                {
                    dynamic tagObj = row.Tag;
                    isGroupRow = tagObj.IsGroup;
                    groupKey = tagObj.GroupKey;
                }
                catch { }
            }

            if (!isGroupRow && PaintMergedCell(e))
                return;

            Color back = StyleSettings.RowBackColor;
            if (UsingSeparateRowStyle)
                back = (e.RowIndex % 2 == 0) ? StyleSettings.RowBackColor : StyleSettings.AlternatingRowBackColor;

            Color fore = StyleSettings.RowTextColor;

            if (isGroupRow)
            {
                back = StyleSettings.GroupRowBackColor;
                fore = StyleSettings.GroupRowTextColor;
            }
            else if (e.RowIndex == _hoverRowIndex && UsingRowMouseOverStyle)
            {
                back = StyleSettings.HoverBackColor;
            }

            if (row.Selected && UsingRowSelectedStyle)
            {
                back = StyleSettings.SelectedBackColor;
                fore = StyleSettings.SelectedTextColor;
            }

            using (var brush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(brush, e.CellBounds);
            }

            if (isGroupRow)
            {
                if (e.ColumnIndex == 0)
                {
                    var text = (e.Value ?? "").ToString();
                    TextRenderer.DrawText(e.Graphics, text, this.Font, e.CellBounds, fore, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                }
                e.Handled = true;
                return;
            }

            Color borderColor = ColorTranslator.FromHtml("#EAE6DB");
            using (var pen = new Pen(borderColor, 0.5f))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                var r = e.CellBounds;

                e.Graphics.DrawLine(pen, r.Right - 1, r.Top, r.Right - 1, r.Bottom - 1);
                e.Graphics.DrawLine(pen, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);
            }

            bool isCurrentCell = this.CurrentCell != null &&
                                 this.CurrentCell.RowIndex == e.RowIndex &&
                                 this.CurrentCell.ColumnIndex == e.ColumnIndex;

            if (isCurrentCell && DrawCellBorderOnSelect)
            {
                using (var pen = new Pen(Color.Black))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    var rect = e.CellBounds;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }

            TextRenderer.DrawText(e.Graphics, (e.Value ?? "").ToString(), this.Font, e.CellBounds, fore, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            e.Handled = true;

            ApplyRowStyle?.Invoke(row);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e); // ADGV のソート、フィルター、ヘッダークリック
            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = e.Location;
                _draggingStarted = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left && !_draggingStarted)
            {
                var dx = Math.Abs(e.X - _dragStartPoint.X);
                var dy = Math.Abs(e.Y - _dragStartPoint.Y);
                var dragThreshold = SystemInformation.DragSize;
                if (dx >= dragThreshold.Width || dy >= dragThreshold.Height)
                {
                    var hit = this.HitTest(e.X, e.Y);
                    if (hit.Type == DataGridViewHitTestType.ColumnHeader && hit.ColumnIndex >= 0)
                    {
                        var col = this.Columns[hit.ColumnIndex];
                        if (col != null)
                        {
                            var data = new System.Windows.Forms.DataObject();
                            data.SetData(typeof(string), col.Name);
                            try
                            {
                                data.SetData(typeof(DataGridViewColumn), col);
                            }
                            catch
                            { }

                            _draggingStarted = true;
                            this.DoDragDrop(data, DragDropEffects.Move);
                        }
                    }
                }
            }
        }

        // OnMouseUp: reset flag
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _draggingStarted = false;
        }

        public object GetRow(int rowHandle)
        {
            if (rowHandle < 0 || rowHandle >= this.Rows.Count)
                return null;

            if (this.DataSource is BindingSource bs)
            {
                if (rowHandle < bs.Count)
                    return bs[rowHandle];
            }
            else if (this.DataSource is DataTable dt)
            {
                if (rowHandle < dt.Rows.Count)
                    return dt.Rows[rowHandle];
            }
            else if (this.DataSource is DataView dv)
            {
                if (rowHandle < dv.Count)
                    return dv[rowHandle];
            }
            else if (this.DataSource is System.Collections.IList list)
            {
                if (rowHandle < list.Count)
                    return list[rowHandle];
            }

            return null;
        }

        public object GetRowCellValue(int rowHandle, string columnFieldName)
        {
            if (rowHandle < 0 || rowHandle >= this.Rows.Count)
                return null;

            var col = this.Columns[columnFieldName];
            if (col == null)
                return null;

            return this.Rows[rowHandle].Cells[col.Index].Value;
        }

        public void SetRowCellValue(int rowHandle, string columnFieldName, object value)
        {
            if (rowHandle < 0 || rowHandle >= this.Rows.Count)
                return;

            var col = this.Columns[columnFieldName];
            if (col == null)
                return;

            this.Rows[rowHandle].Cells[col.Index].Value = value;
        }
        #endregion

        #region Filter & Sort for List<T>
        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);

            if (this.DataSource is BindingSource bs && bs.DataSource is IEnumerable<object> list)
            {
                _originalListData = ((IEnumerable<object>)bs.DataSource).ToList();
            }
            else if (this.DataSource is IEnumerable<object> list2)
            {
                _originalListData = list2.ToList();
            }
            else
            {
                _originalListData = null;
            }

            this.FilterStringChanged -= GroupableDataGridView_FilterStringChanged;
            this.SortStringChanged -= GroupableDataGridView_SortStringChanged;
            this.FilterStringChanged += GroupableDataGridView_FilterStringChanged;
            this.SortStringChanged += GroupableDataGridView_SortStringChanged;
        }

        private void GroupableDataGridView_FilterStringChanged(object sender, EventArgs e)
        {
            _lastFilterString = this.FilterString;
            UpdateFilteredColumnsFromFilterString(_lastFilterString);
            ApplyListFilterAndSort();
        }

        private void GroupableDataGridView_SortStringChanged(object sender, EventArgs e)
        {
            _lastSortString = this.SortString;
            ApplyListFilterAndSort();
        }
        // filter by fieldname
        private void UpdateFilteredColumnsFromFilterString(string filter)
        {
            var newSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                try
                {
                    var rxBracket = new System.Text.RegularExpressions.Regex(@"\[([^\]]+)\]");
                    foreach (System.Text.RegularExpressions.Match m in rxBracket.Matches(filter))
                    {
                        var name = m.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(name))
                            newSet.Add(name);
                    }

                    var rxSimple = new System.Text.RegularExpressions.Regex(@"\b([A-Za-z_]\w*)\b\s*(?:LIKE|NOT\s+LIKE|==|=|IN|IS|>=|<=|>|<)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    foreach (System.Text.RegularExpressions.Match m in rxSimple.Matches(filter))
                    {
                        var name = m.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(name))
                            newSet.Add(name);
                    }
                }
                catch
                {
                }
            }

            bool changed = !_filteredColumns.SetEquals(newSet);
            if (changed)
            {
                _filteredColumns = newSet;
                Invalidate(); // redraw header overlay
            }
        }

        private void ApplyListFilterAndSort()
        {
            if (_originalListData == null) return;
            if (!_originalListData.Any()) return;

            try
            {
                var query = _originalListData.AsQueryable();

                // Apply filter
                if (!string.IsNullOrWhiteSpace(_lastFilterString))
                {
                    string linqFilter = ConvertFilterToLinq(_lastFilterString);
                    if (!string.IsNullOrWhiteSpace(linqFilter))
                    {
                        query = query.Where(linqFilter);
                    }
                }

                // Apply sort
                if (!string.IsNullOrWhiteSpace(_lastSortString))
                {
                    string sortExpr = ConvertSortToLinq(_lastSortString);
                    if (!string.IsNullOrWhiteSpace(sortExpr))
                    {
                        query = query.OrderBy(sortExpr);
                    }
                }

                var result = query.ToList();

                if (this.DataSource is BindingSource bs)
                    bs.DataSource = result;
                else
                    this.DataSource = new BindingSource { DataSource = result };

                this.UpdateClearFilterButtonVisibility();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Filter/sort failed: " + ex.Message);
            }
        }

        private string ConvertFilterToLinq(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString))
                return null;

            // 1️⃣ ADGVの "Convert(...)" を外す
            filterString = filterString
                .Replace("[", "")
                .Replace("]", "")
                .Replace("Convert(", "")
                .Replace(",System.String)", "");

            // 2️⃣  "=" を "=="　に変換
            filterString = Regex.Replace(filterString, @"\s=\s", " == ", RegexOptions.IgnoreCase);

            // 3️⃣  IN (...)　の処理
            filterString = Regex.Replace(filterString, @"(\w+)\s+IN\s+\((.+?)\)", match =>
            {
                var field = match.Groups[1].Value.Trim();
                var values = match.Groups[2].Value
                    .Trim('(', ')', ' ')
                    .Split(',')
                    .Select(v => v.Trim().Trim('\'', '"'))
                    .ToArray();

                var joined = string.Join(",", values.Select(v => $"\"{v}\""));
                return $"new[] {{{joined}}}.Contains(Convert.ToString({field}))";
            }, RegexOptions.IgnoreCase);

            // 4️⃣  LIKEの処理
            filterString = Regex.Replace(filterString, @"(\w+)\s+LIKE\s+'%(.+?)%'", m =>
            {
                var field = m.Groups[1].Value.Trim();
                var value = m.Groups[2].Value;
                return $"Convert.ToString({field}).Contains(\"{value}\")";
            }, RegexOptions.IgnoreCase);

            // 5️⃣ NOT LIKEの処理
            filterString = Regex.Replace(filterString, @"(\w+)\s+NOT\s+LIKE\s+'%(.+?)%'", m =>
            {
                var field = m.Groups[1].Value.Trim();
                var value = m.Groups[2].Value;
                return $"!Convert.ToString({field}).Contains(\"{value}\")";
            }, RegexOptions.IgnoreCase);

            // 6️⃣　最終のLINQ用クエリ
            filterString = filterString.Replace("\\\"", "\"").Trim();

            return filterString;
        }

        private string EscapeQuotes(string input)
        {
            return input?.Replace("\"", "\\\"") ?? "";
        }

        private string ConvertSortToLinq(string adgvSort)
        {
            string result = adgvSort.Replace("[", "").Replace("]", "");
            result = result.Replace("ASC", "ascending").Replace("DESC", "descending");
            return result;
        }

        private (int topRow, int bottomRow) GetMergeRangeForCell(int col, int row)
        {
            int top = row;
            int bottom = row;

            for (int r = row - 1; r >= 0; r--)
            {
                var args = new CellMergeEventArgs(col, r, r + 1);
                CellMerge?.Invoke(this, args);
                if (args.Handled && args.Merge)
                    top = r;
                else break;
            }

            for (int r = row + 1; r < this.Rows.Count; r++)
            {
                var args = new CellMergeEventArgs(col, r - 1, r);
                CellMerge?.Invoke(this, args);
                if (args.Handled && args.Merge)
                    bottom = r;
                else break;
            }

            return (top, bottom);
        }

        /// <summary>
        /// Gọi trong CellPainting để xử lý merge cell.
        /// </summary>
        private bool PaintMergedCell(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return false;

            var (top, bottom) = GetMergeRangeForCell(e.ColumnIndex, e.RowIndex);
            if (top == bottom) return false; // không merge

            // Chỉ vẽ cell top
            if (e.RowIndex != top)
            {
                e.Handled = true;
                return true;
            }

            // Xác định vùng merge
            Rectangle rect = e.CellBounds;
            for (int r = e.RowIndex + 1; r <= bottom && r < this.Rows.Count; r++)
            {
                rect.Height += this.Rows[r].Height;
            }

            using (SolidBrush backBrush = new SolidBrush(e.CellStyle.BackColor))
                e.Graphics.FillRectangle(backBrush, rect);

            using (Pen pen = new Pen(this.GridColor))
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            var text = Convert.ToString(e.Value);
            if (!string.IsNullOrEmpty(text))
            {
                TextRenderer.DrawText(
                    e.Graphics, text, e.CellStyle.Font, rect,
                    e.CellStyle.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }

            e.Handled = true;
            return true;
        }
        #endregion
    }
}