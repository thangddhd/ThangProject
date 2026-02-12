using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Imaging;
using Zuby.ADGV;

namespace coms.COMMON.ui
{
    [Designer("System.Windows.Forms.Design.DataGridViewDesigner, System.Design", typeof(IDesigner))]
    public partial class DataGridViewEx : AdvancedDataGridView
    {
        //private HitTestInfo _lastHitTest;
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

        [Category("Behavior")]
        [Description("Keep filter and sort state when DataSource is rebound.")]
        [DefaultValue(false)]
        public bool KeepFilterAndSort { get; set; } = true;

        public event Action<DataGridViewRow> ApplyRowStyle;
        public event EventHandler<CellMergeEventArgs> CellMerge;
        public event EventHandler<ButtonIconNeededEventArgs> ButtonIconNeeded;
        public event EventHandler<CellBackColorNeededEventArgs> CellBackColorNeeded;
        public event EventHandler<RowBackColorNeededEventArgs> RowBackColorNeeded;
        public event EventHandler<TextBoxIconNeededEventArgs> TextBoxIconNeeded;
        public event EventHandler<LinkColumnEditEventArgs> LinkColumnEdit;
        public event EventHandler<ComboboxColumnEditEventArgs> ComboboxColumnEdit;
        //--------------------------
        public event EventHandler<CustomRowCellEventArgs> CustomRowCell;
        //XtraGridのCustomColumnDisplayTextイベントを再現する
        public event EventHandler<CustomColumnDisplayTextEventArgs> CustomColumnDisplayText;
        //XtraGridのCustomRowCellEditイベントを再現する
        public event EventHandler<CustomRowCellEditEventArgs> CustomRowCellEdit;
        //XtraGridのCustomRowCellEditイベントを再現する
        public event EventHandler<CustomColumnDataEventArgs> CustomColumnData;
        //CellMouseClick（マウスクリックする際にCellのRowIndex、ColumnIndex、クリックしたマウスのボタン...取得できる）と
        //CellContentClick(ボタンにクリックする場合)より提供するサービスが多い。
        //XtraGridのRowCellClickイベントを再現する
        public event EventHandler<CustomRowCellClickEventArgs> CustomRowCellClick;
        //XtraGridのInitNewRowイベントを再現する新しい行追加する時
        public event EventHandler<CustomInitNewRowEventArgs> CustomInitNewRow;
        //XtraGridのCellValueChangedイベントを再現する：Cell値編集する時
        public event EventHandler<CustomCellValueChangedEventArgs> CustomCellValueChanged; //for CellValueChanged
        //
        public event EventHandler<DataGridViewCellEventArgs> ButtonEditClick;

        private static readonly Image IconUpload_ActiveNew = SystemIcons.Hand.ToBitmap();
        private static readonly Image IconUpload_Disabled = SystemIcons.Information.ToBitmap();
        private static readonly Image IconUpload_Active = SystemIcons.Information.ToBitmap();

        //RepositoryItems
        private static readonly CustomButtonEditorTemplate TemplateUpdloadActive = new CustomButtonEditorTemplate(IconUpload_ActiveNew);
        private static readonly CustomButtonEditorTemplate TemplateUpdloadDisabled = new CustomButtonEditorTemplate(IconUpload_Disabled);
        private static readonly CustomButtonEditorTemplate TemplateUpdloadDefault = new CustomButtonEditorTemplate(IconUpload_Active);

        //--------------------------

        private string _groupColumn = null;
        private readonly List<GroupRowInfo> _groups = new List<GroupRowInfo>();
        private int _hoverRowIndex = -1;
        private Image _filterIcon = GridviewEx.Properties.Resources.icons8_filter_16;
        private int _hoverColumnIndex = -1;
        private object _oldCellValue;
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
        private bool _isCalculating = false; //calc value of cell

        // icon 処理
        private Dictionary<string, Image> _buttonColumnIcons = new Dictionary<string, Image>();
        [Browsable(false)]
        public Dictionary<string, Image> ButtonColumnIcons => _buttonColumnIcons;

        private List<DisplayRow> _displayRows = new List<DisplayRow>();
        private Dictionary<string, CollapsedGroupCache> _collapsedGroups
            = new Dictionary<string, CollapsedGroupCache>();
        // フィルターしない列の設定
        public HashSet<string> DisabledFilterColumns { get; set; }
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // disable sort all columns will ignore all sort event if not NotSortable
        public bool DisabledSortAll { get; set; } = false;
        // disable filter all columns will ignore all sort event if not in DisabledFilterAll
        public bool DisabledFilterAll { get; set; } = false;
        private bool _suppressSortOnce = false;
        private bool _isResizingColumn = false;
        public HashSet<string> IgnoreAutoFormatColumns { get; set; } = new HashSet<string>();
        public DataGridViewEx()
        {
            this.DoubleBuffered = true;
            this.EnableHeadersVisualStyles = false;
            this.RowHeadersVisible = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.AllowUserToAddRows = false;
            this.AllowDrop = true;
            this.AllowUserToOrderColumns = true;
            this.AutoGenerateColumns = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.ColumnHeadersHeight = 25;

            this.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            this.DragDrop += GroupableDataGridView_DragDrop;
            this.DragOver += GroupableDataGridView_DragOver;
            this.Paint += GroupableDataGridView_Paint;

            this.CellPainting += GroupableDataGridView_CellPainting;
            this.CellMouseEnter += GroupableDataGridView_CellMouseEnter;
            this.CellMouseLeave += GroupableDataGridView_CellMouseLeave;
            this.CellMouseClick += GroupableDataGridView_CellMouseClick;
            this.DataBindingComplete += GroupableDataGridView_DataBindingComplete;
            //this.CellContentClick += DataGridViewEx_CellContentClick;

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

            _clearFilterButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _clearFilterButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            _clearFilterButton.TextImageRelation = TextImageRelation.ImageBeforeText;

            _clearFilterButton.Image = GridviewEx.Properties.Resources.delete2_16;

            _clearFilterButton.Click += (s, e) =>
            {
                ClearAllFilters();
            };
            this.Controls.Add(_clearFilterButton);
            this.Resize += (s, e) => PositionClearFilterButton();
            this.Scroll += (s, e) => PositionClearFilterButton();
        }

        public int FocusedRowHandle
        {
            get
            {
                return this.CurrentRow?.Index ?? -1;
            }
            set
            {
                if (value >= 0 && value < this.Rows.Count)
                {
                    int col = this.CurrentCell?.ColumnIndex ?? 0;
                    if (col >= this.Columns.Count) col = 0;

                    this.CurrentCell = this.Rows[value].Cells[col];
                }
            }
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
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("ClearAllFilters: " + ex.Message); }
        }

        internal void RaiseComboboxColumnEdit(ComboboxColumnEditEventArgs args)
        {
            ComboboxColumnEdit?.Invoke(this, args);
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
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine("ReapplyColumnDisplayOrder: " + ex.Message); }
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
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("GroupableDataGridView_DragDrop: " + ex.Message); }

            // 位置赤い線を消すの設定
            _insertionMarkIndex = -1;
            _insertionMarkX = -1;
            this.Invalidate();

            // Cập nhật popup
            var parentForm = this.FindForm();
            if (parentForm is coms.MyForm prForm && prForm._chooserPopup != null)
                prForm._chooserPopup.RemoveColumnByName(colName);

            UpdateVisibleColumnOrder();
            ReapplyColumnDisplayOrder();
            this.Refresh();
        }

        public T GetRowObject<T>(int rowIndex) where T : class
        {
            if (rowIndex < 0 || rowIndex >= this.Rows.Count)
                return null;

            var row = this.Rows[rowIndex];

            // grouping: DisplayRow assign to Tag
            if (row.Tag != null)
            {
                var prop = row.Tag.GetType().GetProperty("DataItem");
                if (prop != null)
                {
                    var val = prop.GetValue(row.Tag);
                    if (val is T tObj)
                        return tObj;
                }
            }

            // グループではない場合 DataBoundItem 使います
            return row.DataBoundItem as T;
        }

        public void RefreshDataRowsFromDisplayModel()
        {
            for (int i = 0; i < this.Rows.Count; i++)
            {
                var tag = this.Rows[i].Tag;
                if (tag == null) continue;

                var prop = tag.GetType().GetProperty("DataItem");
                if (prop == null) continue;

                var dataItem = prop.GetValue(tag);
                if (dataItem == null) continue;

                BindRowValues(this.Rows[i], dataItem);
            }

            this.Invalidate();
        }
        #region Public API

        public List<T> CloneCurrentList<T>()
        {
            var result = new List<T>();

            foreach (DataGridViewRow row in this.Rows)
            {
                if (row.DataBoundItem is T item)
                {
                    result.Add(DeepClone(item));
                }
            }

            return result;
        }

        public static T DeepClone<T>(T obj)
        {
            if (obj == null) return default(T);

            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
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
            if (string.IsNullOrEmpty(_groupColumn))
                return;

            // オリジナルDataSource保管
            CaptureOriginalDataSourceIfNeeded();

            IEnumerable<object> items = ExtractEnumerableItemsFromDataSource();
            if (items == null)
                return;

            var grouped = items
                .GroupBy(x => GetGroupKey(x, _groupColumn))
                .OrderBy(g => g.Key);

            _displayRows.Clear();
            _collapsedGroups.Clear();

            foreach (var group in grouped)
            {
                // Header row
                _displayRows.Add(new DisplayRow
                {
                    IsGroup = true,
                    GroupKey = group.Key,
                    Collapsed = false,
                    DataItem = null
                });

                // Data rows
                foreach (var item in group)
                {
                    _displayRows.Add(new DisplayRow
                    {
                        IsGroup = false,
                        GroupKey = group.Key,
                        Collapsed = false,
                        DataItem = item
                    });
                }
            }

            // データソース関係なく再画く
            this.DataSource = null;

            RefreshDisplayRowsUI();
        }

        private IEnumerable<object> ExtractEnumerableItemsFromDataSource()
        {
            if (this.DataSource is BindingSource bs)
                return bs.Cast<object>();

            if (this.DataSource is IEnumerable<object> en)
                return en;

            if (this.DataSource is DataTable dt)
                return dt.AsEnumerable().Cast<object>();

            return null;
        }

        private string GetGroupKey(object item, string column)
        {
            if (item is DataRow dr)
                return dr[column]?.ToString() ?? "";

            if (item is DataRowView drv)
                return drv[column]?.ToString() ?? "";

            // object T → for Reflection
            var prop = item.GetType().GetProperty(column);
            return prop?.GetValue(item)?.ToString() ?? "";
        }

        private void RebuildUIFromDisplayRows()
        {
            this.Rows.Clear();
            this.SuspendLayout();

            foreach (var r in _displayRows)
            {
                if (r.IsGroup)
                {
                    int idx = this.Rows.Add();
                    var row = this.Rows[idx];
                    row.Tag = r;
                    row.ReadOnly = true;

                    row.DefaultCellStyle.BackColor = StyleSettings.GroupRowBackColor;
                    row.DefaultCellStyle.ForeColor = StyleSettings.GroupRowTextColor;
                    row.Cells[0].Value = r.Collapsed ? $"▸ {r.GroupKey}" : $"▾ {r.GroupKey}";
                }
                else
                {
                    int idx = this.Rows.Add();
                    var row = this.Rows[idx];
                    row.Tag = r;

                    BindRowValues(row, r.DataItem);
                }
            }

            this.ResumeLayout();
        }

        private void BindRowValues(DataGridViewRow row, object item)
        {
            foreach (DataGridViewColumn col in this.Columns)
            {
                var prop = item.GetType().GetProperty(col.DataPropertyName);
                if (prop == null) continue;

                object value = prop.GetValue(item);

                // ---- FIX TYPE SAFETY ----
                if (col is DataGridViewCheckBoxColumn)
                {
                    row.Cells[col.Index].Value = ConvertToBool(value);
                }
                else if (col.ValueType == typeof(bool))
                {
                    row.Cells[col.Index].Value = ConvertToBool(value);
                }
                else
                {
                    row.Cells[col.Index].Value = value ?? DBNull.Value;
                }
            }
        }

        private bool ConvertToBool(object val)
        {
            if (val == null) return false;

            if (val is bool b) return b;

            string s = val.ToString().Trim().ToLower();

            if (s == "1" || s == "true" || s == "yes" || s == "y" || s == "on")
                return true;

            if (s == "0" || s == "false" || s == "no" || s == "n" || s == "off")
                return false;

            // 例外なしでfalse返す
            return false;
        }

        /// <summary>
        /// Group the grid by the specified column name.
        /// </summary>
        public void GroupBy(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                ClearGrouping();
                return;
            }

            if (!this.Columns.Contains(columnName))
                throw new ArgumentException("Column not found", nameof(columnName));

            var grpField = this.Columns[columnName]?.DataPropertyName;
            if (string.IsNullOrEmpty(grpField))
            {
                ClearGrouping();
                return;
            }

            _groupColumn = grpField;
            RebuildGrouping();
        }

        public void ClearGrouping()
        {
            _groupColumn = null;
            _groups.Clear();
            _displayRows.Clear();
            _collapsedGroups.Clear();

            // Restore original DataSource
            RebindDataSourceWithoutGrouping();
        }

        public void ToggleGroup(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _displayRows.Count)
                return;

            var header = _displayRows[rowIndex];

            if (!header.IsGroup)
                return;

            string key = header.GroupKey;

            // Expanded → COLLAPSE
            if (!header.Collapsed)
            {
                header.Collapsed = true;

                // グループの物を探す
                var cache = new CollapsedGroupCache { GroupKey = key };

                int i = rowIndex + 1;
                while (i < _displayRows.Count && !_displayRows[i].IsGroup)
                {
                    if (_displayRows[i].GroupKey == key)
                    {
                        cache.CachedRows.Add(_displayRows[i]);
                        _displayRows.RemoveAt(i);
                        continue;
                    }
                    break;
                }

                _collapsedGroups[key] = cache;

                // UIリフレッシュ
                RefreshDisplayRowsUI();
                return;
            }

            // Collapsed → EXPAND
            header.Collapsed = false;

            if (_collapsedGroups.TryGetValue(key, out var groupCache))
            {
                int insertIndex = rowIndex + 1;

                foreach (var item in groupCache.CachedRows)
                    _displayRows.Insert(insertIndex++, item);

                // 再インサートしないようにキャッシュを消す
                _collapsedGroups.Remove(key);
            }

            RefreshDisplayRowsUI();
        }

        public void ExpandAllGroups()
        {
            if (_displayRows == null || _displayRows.Count == 0)
                return;

            foreach (var r in _displayRows)
            {
                if (r.IsGroup)
                    r.Collapsed = false;
            }
            foreach (var kvp in _collapsedGroups.ToList())
            {
                string key = kvp.Key;
                var cache = kvp.Value;

                int headerIndex = _displayRows.FindIndex(r => r.IsGroup && r.GroupKey == key);
                if (headerIndex < 0)
                    continue;

                int insertIndex = headerIndex + 1;
                foreach (var item in cache.CachedRows)
                {
                    _displayRows.Insert(insertIndex++, item);
                }
            }
            _collapsedGroups.Clear();
            RefreshDisplayRowsUI();
        }

        public void CollapseAllGroups()
        {
            if (_displayRows == null || _displayRows.Count == 0)
                return;

            _collapsedGroups.Clear();

            int index = 0;
            while (index < _displayRows.Count)
            {
                var header = _displayRows[index];

                if (!header.IsGroup)
                {
                    index++;
                    continue;
                }
                header.Collapsed = true;
                var cache = new CollapsedGroupCache
                {
                    GroupKey = header.GroupKey
                };
                int j = index + 1;
                while (j < _displayRows.Count && !_displayRows[j].IsGroup)
                {
                    cache.CachedRows.Add(_displayRows[j]);
                    _displayRows.RemoveAt(j);
                }
                _collapsedGroups[header.GroupKey] = cache;
                index++;
            }
            RefreshDisplayRowsUI();
        }

        private void RefreshDisplayRowsUI()
        {
            this.Rows.Clear();
            this.SuspendLayout();

            foreach (var r in _displayRows)
            {
                if (r.IsGroup)
                {
                    int idx = this.Rows.Add();
                    var row = this.Rows[idx];
                    row.Tag = r;
                    row.ReadOnly = true;

                    row.DefaultCellStyle.BackColor = StyleSettings.GroupRowBackColor;
                    row.DefaultCellStyle.ForeColor = StyleSettings.GroupRowTextColor;
                    row.Cells[0].Value = r.Collapsed ? $"▸ {r.GroupKey}" : $"▾ {r.GroupKey}";
                }
                else
                {
                    int idx = this.Rows.Add();
                    var row = this.Rows[idx];
                    row.Tag = r;

                    BindRowValues(row, r.DataItem);
                }
            }

            this.ResumeLayout();
        }

        private string GetComboDisplayText(DataGridViewComboBoxCell cell)
        {
            if (cell == null)
                return string.Empty;

            var value = cell.Value;
            if (value == null)
                return string.Empty;

            var ds = cell.DataSource;
            if (ds == null)
                return Convert.ToString(value);

            if (!(ds is System.Collections.IEnumerable enumerable) || ds is string)
                return Convert.ToString(value);

            string displayMember = cell.DisplayMember;
            string valueMember = cell.ValueMember;

            PropertyDescriptor displayProp = null;
            PropertyDescriptor valueProp = null;
            bool propsInitialized = false;

            foreach (var item in enumerable)
            {
                if (item == null) continue;

                if (!propsInitialized)
                {
                    var props = TypeDescriptor.GetProperties(item);

                    if (!string.IsNullOrEmpty(displayMember))
                        displayProp = props.Find(displayMember, false);

                    if (!string.IsNullOrEmpty(valueMember))
                        valueProp = props.Find(valueMember, false);

                    propsInitialized = true;
                }

                object itemValue;

                if (valueProp != null)
                    itemValue = valueProp.GetValue(item);
                else
                    itemValue = item;

                if ((itemValue == null && value == null) ||
                    (itemValue != null && itemValue.Equals(value)))
                {
                    if (displayProp != null)
                    {
                        var disp = displayProp.GetValue(item);
                        return disp != null ? disp.ToString() : string.Empty;
                    }

                    return item.ToString();
                }
            }

            return Convert.ToString(value);
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
            try { isGroup = tagObj.IsGroup; } catch (Exception ex) { isGroup = false; System.Diagnostics.Debug.WriteLine("GroupableDataGridView_CellMouseClick: " + ex.Message); }

            if (isGroup)
            {
                // toggle collapse on header click
                string key = tagObj.GroupKey;
                ToggleGroup(e.RowIndex);
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

        //private void DataGridViewEx_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        //    var col = this.Columns[e.ColumnIndex];
        //    if (col is DataGridViewCheckBoxColumn)
        //    {
        //        this.NotifyCurrentCellDirty(true);
        //        this.CommitEdit(DataGridViewDataErrorContexts.Commit);
        //        return;
        //    }
        //}

        private void GroupableDataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            _hoverRowIndex = -1;
            this.Invalidate();
        }

        private void GroupableDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; // skip row header or column header
            var column = this.Columns[e.ColumnIndex];
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
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("GroupableDataGridView_CellPainting: " + ex.Message); }
            }

            var (top, bottom) = GetMergeRangeForCell(column, e.ColumnIndex, e.RowIndex);

            // --- Base colors ---
            Color back = StyleSettings.RowBackColor;
            if (UsingSeparateRowStyle)
                back = (e.RowIndex % 2 == 0) ? StyleSettings.RowBackColor : StyleSettings.AlternatingRowBackColor;

            Color fore = StyleSettings.RowTextColor;

            // --- Group row ---
            if (isGroupRow)
            {
                e.Handled = true;
                return;
            }
            // --- Hover row ---
            else if (e.RowIndex == _hoverRowIndex && UsingRowMouseOverStyle)
            {
                back = StyleSettings.HoverBackColor;
            }

            // --- Selected row ---
            if (row.Selected && UsingRowSelectedStyle)
            {
                back = StyleSettings.SelectedBackColor;
                fore = StyleSettings.SelectedTextColor;
            }

            // --- Row-level event ---
            if (RowBackColorNeeded != null && !isGroupRow) // グループ対象外
            {
                var rowArgs = new RowBackColorNeededEventArgs(row, row.DataBoundItem, back);
                RowBackColorNeeded.Invoke(this, rowArgs);
                back = rowArgs.BackColor;
            }

            // --- Cell-level event overrides row ---
            if (CellBackColorNeeded != null)
            {
                var cellArgs = new CellBackColorNeededEventArgs(column, row, row.DataBoundItem, back);
                CellBackColorNeeded.Invoke(this, cellArgs);
                if (cellArgs.BackColor != Color.Empty)
                    back = cellArgs.BackColor; // セルが優先
            }

            // --- Fill cell background ---
            using (var brush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(brush, e.CellBounds);
            }

            // --- Draw cell borders ---
            Color borderColor = ColorTranslator.FromHtml("#EAE6DB");
            using (var pen = new Pen(borderColor, 0.5f))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                var r = e.CellBounds;

                e.Graphics.DrawLine(pen, r.Right - 1, r.Top, r.Right - 1, r.Bottom - 1);
                if (e.RowIndex == bottom)
                {
                    e.Graphics.DrawLine(pen, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);
                }
            }

            // --- Draw current cell border ---
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

            // --- Draw top cell for merged cells ---
            if (e.RowIndex == top)
            {
                this.DrawTopCell(e, column, back, fore);
            }

            e.Handled = true;

            ApplyRowStyle?.Invoke(row);
        }

        private void DrawGroupHeader(DataGridViewCellPaintingEventArgs e, DataGridViewRow row, string groupKey)
        {
            // Determine expand/collapse text
            string prefix = "▾ ";
            try
            {
                dynamic tagObj = row.Tag;
                if (tagObj.Collapsed) prefix = "▸ ";
            }
            catch { }

            string text = prefix + (groupKey ?? "");
            // FULL ROW RECTANGLE with proper scrolling
            Rectangle fullRowRect = this.GetRowDisplayRectangle(e.RowIndex, false);

            // Adjust for horizontal scroll
            fullRowRect.X = e.CellBounds.X;

            // Clip to visible area to avoid black region when scrolled
            fullRowRect.Width = (int)e.ClipBounds.Width;

            // Fill background
            using (var b = new SolidBrush(StyleSettings.GroupRowBackColor))
                e.Graphics.FillRectangle(b, fullRowRect);

            // Draw text
            TextRenderer.DrawText(
                e.Graphics,
                text,
                this.Font,
                fullRowRect,
                StyleSettings.GroupRowTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix
            );
        }

        private void DrawTopCell(DataGridViewCellPaintingEventArgs e, DataGridViewColumn column, Color back, Color fore)
        {
            if (column is DataGridViewButtonColumn)
            {
                this.DrawButtonColumn(e, column, fore);
            }
            else if (column is DataGridViewLinkColumn)
            {
                this.DrawLinkColumn(e, column, fore);
            }
            else if (column is DataGridViewCheckBoxColumn)
            {
                this.DrawCheckBoxColumn(e, column, fore);
            }
            else if (column is DataGridViewComboBoxColumn)
            {
                this.DrawComboBoxColumn(e, column, back, fore);
            }
            else if (column is DataGridViewTimeColumn)
            {
                this.DrawTimeColumn(e, column, fore);
            }
            else  // 現在デフォールトセル
            {
                this.DrawTextBoxColumn(e, column, fore);
            }
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

        public object GetRowCellValue(int rowHandle, DataGridViewColumn col)
        {
            if (rowHandle < 0 || rowHandle >= this.Rows.Count)
                return null;

            if (col == null)
                return null;

            return this.Rows[rowHandle].Cells[col.Index].Value;
        }

        public string GetRowCellValueStr(int rowHandle, DataGridViewColumn col)
        {
            var obj = GetRowCellValue(rowHandle, col);

            if (obj == null) return string.Empty;

            return obj.ToString();
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

        public (int topRow, int bottomRow) GetMergeRangeForCell(DataGridViewColumn column, int col, int row)
        {
            int top = row;
            int bottom = row;

            for (int r = row - 1; r >= 0; r--)
            {
                var args = new CellMergeEventArgs(column, col, r, r + 1);
                CellMerge?.Invoke(this, args);
                if (args.Handled && args.Merge)
                    top = r;
                else break;
            }

            for (int r = row + 1; r < this.Rows.Count; r++)
            {
                var args = new CellMergeEventArgs(column, col, r - 1, r);
                CellMerge?.Invoke(this, args);
                if (args.Handled && args.Merge)
                    bottom = r;
                else break;
            }

            return (top, bottom);
        }

        public bool IsEqual(int rowIdx1, DataGridViewColumn col1, int rowIdx2, DataGridViewColumn col2)
        {
            var val1 = this.GetRowCellValue(rowIdx1, col1.Name);
            var val2 = this.GetRowCellValue(rowIdx2, col2.Name);
            // どっちかNULLまたは両方NULLの場合　比較しない
            if (val1 == null || val2 == null) return false;

            return val1.ToString() == val2.ToString();
        }
        #endregion

        #region Filter & Sort for List<T>
        private void GroupableDataGridView_FilterStringChanged(object sender, EventArgs e)
        {
            foreach (var fld in _filteredColumns)
            {
                if (this.DisabledFilterAll || DisabledFilterColumns.Contains(fld))
                {
                    // reset filter & redownload datasource if needed
                    this.CleanFilterAndSort();
                    _filteredColumns.Clear();
                    _lastFilterString = "";
                    _clearFilterButton.Visible = false;
                    return;
                }
            }

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
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("UpdateFilteredColumnsFromFilterString: " + ex.Message); }
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

            // Trim
            filterString = filterString.Trim();

            // Remove square brackets around column names
            filterString = filterString.Replace("[", "").Replace("]", "");

            // Replace Convert(<expr>, 'System.String') -> Convert.ToString(<expr>)
            filterString = Regex.Replace(filterString,
                @"Convert\(\s*(?<expr>[^,]+?)\s*,\s*('|"")?System\.String('|"")?\s*\)",
                m => $"Convert.ToString({m.Groups["expr"].Value.Trim()})",
                RegexOptions.IgnoreCase);

            // Normalize consecutive whitespace to single space (keeps parentheses)
            filterString = Regex.Replace(filterString, @"\s+", " ");

            // Pattern for a field: either Convert.ToString(...) or a simple identifier/path
            var fieldPattern = @"(?<field>(?:Convert\.ToString\([^)]*\)|[A-Za-z0-9_\.]+))";

            // === NOT LIKE handlers (specific for % patterns) ===
            // NOT LIKE '%x%' => !Contains(...)
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+NOT\s+LIKE\s+'%(?<val>[^']*)%'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"(!{fieldExpr}.Contains(\"{v}\"))";
                }, RegexOptions.IgnoreCase);

            // NOT LIKE 'x%' => !StartsWith(...)
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+NOT\s+LIKE\s+'(?<val>[^'%]+)%'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"(!{fieldExpr}.StartsWith(\"{v}\"))";
                }, RegexOptions.IgnoreCase);

            // NOT LIKE '%x' => !EndsWith(...)
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+NOT\s+LIKE\s+'%(?<val>[^']+)'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"(!{fieldExpr}.EndsWith(\"{v}\"))";
                }, RegexOptions.IgnoreCase);

            // NOT LIKE 'x' fallback => !=
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+NOT\s+LIKE\s+'(?<val>[^']*)'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"({fieldExpr} != \"{v}\")";
                }, RegexOptions.IgnoreCase);

            // === LIKE handlers ===
            // LIKE '%x%' => Contains(...)
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+LIKE\s+'%(?<val>[^']*)%'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"({fieldExpr}.Contains(\"{v}\"))";
                }, RegexOptions.IgnoreCase);

            // LIKE 'x%' => StartsWith(...)
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+LIKE\s+'(?<val>[^'%]+)%'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"({fieldExpr}.StartsWith(\"{v}\"))";
                }, RegexOptions.IgnoreCase);

            // LIKE '%x' => EndsWith(...)
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+LIKE\s+'%(?<val>[^']+)'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"({fieldExpr}.EndsWith(\"{v}\"))";
                }, RegexOptions.IgnoreCase);

            // LIKE 'x' (no %) => equality on string
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+LIKE\s+'(?<val>[^'%]+)'",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var v = m.Groups["val"].Value.Replace("\"", "\\\"");
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"({fieldExpr} == \"{v}\")";
                }, RegexOptions.IgnoreCase);

            // IN (...) -> new[] { "a", "b" }.Contains(Convert.ToString(field))
            filterString = Regex.Replace(filterString,
                fieldPattern + @"\s+IN\s*\((?<vals>[^)]+)\)",
                m =>
                {
                    var f = m.Groups["field"].Value;
                    var vals = m.Groups["vals"].Value
                        .Split(',')
                        .Select(v => v.Trim().Trim('\'', '"'))
                        .Where(v => v.Length > 0)
                        .Select(v => $"\"{v.Replace("\"", "\\\"")}\"");
                    var joined = string.Join(", ", vals);
                    var fieldExpr = f.StartsWith("Convert.ToString(", StringComparison.OrdinalIgnoreCase) ? f : $"Convert.ToString({f})";
                    return $"(new[] {{ {joined} }}.Contains({fieldExpr}))";
                }, RegexOptions.IgnoreCase);

            // Convert SQL '<>' to '!='
            filterString = Regex.Replace(filterString, @"<>", "!=", RegexOptions.IgnoreCase);

            // Replace standalone = with ==, avoiding >=, <=, != and other operators
            filterString = Regex.Replace(filterString, @"(?<!(?:<|>|!|=))\s=\s(?!(?:=|<|>|!))", " == ", RegexOptions.None);

            // (Optional) if two parenthesized expressions appear adjacent without operator, insert &&:
            filterString = Regex.Replace(filterString, @"\)\s*\(", ") && (");

            // Convert single-quoted string literals to double-quoted (for Dynamic LINQ)
            filterString = Regex.Replace(filterString, @"'([^']*)'", m =>
            {
                var s = m.Groups[1].Value.Replace("\"", "\\\"");
                return $"\"{s}\"";
            });

            // Final trim
            filterString = filterString.Trim();

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

        private void DrawGroupHeaderFullRow(DataGridViewRowPostPaintEventArgs e, DataGridViewRow row)
        {
            string prefix = "▾ ";
            dynamic tag = row.Tag;
            try { if (tag.Collapsed) prefix = "▸ "; } catch { }

            string key = "";
            try { key = tag.GroupKey ?? ""; } catch { }

            string text = prefix + key;

            // Full row bounds
            Rectangle fullRowRect = this.GetRowDisplayRectangle(e.RowIndex, false);

            // ClipBounds trong RowPostPaint chính xác 100%
            fullRowRect.Intersect(Rectangle.Ceiling(e.Graphics.ClipBounds));

            using (Brush b = new SolidBrush(StyleSettings.GroupRowBackColor))
                e.Graphics.FillRectangle(b, fullRowRect);

            TextRenderer.DrawText(
                e.Graphics, text, this.Font,
                fullRowRect,
                StyleSettings.GroupRowTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
        #endregion

        #region draw merged columns
        private void DrawCheckBoxColumn(DataGridViewCellPaintingEventArgs e, DataGridViewColumn column, Color fore)
        {
            bool? isChecked = null;
            if (e.Value is bool b)
                isChecked = b;
            else if (e.Value is bool?)
                isChecked = (bool?)e.Value;
            else if (e.Value != null)
            {
                if (bool.TryParse(e.Value.ToString(), out bool parsed))
                    isChecked = parsed;
            }

            var state = System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;
            if (isChecked == true)
                state = System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal;
            else if (isChecked == null)
                state = System.Windows.Forms.VisualStyles.CheckBoxState.MixedNormal;

            Size checkBoxSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
            int cx = e.CellBounds.Left + (e.CellBounds.Width - checkBoxSize.Width) / 2;
            int cy = e.CellBounds.Top + (e.CellBounds.Height - checkBoxSize.Height) / 2;
            Rectangle checkBoxRect = new Rectangle(cx, cy, checkBoxSize.Width, checkBoxSize.Height);

            CheckBoxRenderer.DrawCheckBox(e.Graphics, checkBoxRect.Location, state);
        }

        private void DrawButtonColumn(DataGridViewCellPaintingEventArgs e, DataGridViewColumn column, Color fore)
        {
            var btnCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

            // ボタンがセル値優先でなけらばテキスト
            string btnText = (btnCell?.Value?.ToString())
                             ?? (column is DataGridViewButtonColumn btnCol && btnCol.UseColumnTextForButtonValue
                                 ? btnCol.Text
                                 : "");

            bool enabled = true;
            Image icon = null;
            Color backColor = Color.Empty;

            // if has default icon
            if (_buttonColumnIcons.TryGetValue(column.Name, out var defaultIcon))
                icon = defaultIcon;

            // イベントでアイコン設定の場合
            if (ButtonIconNeeded != null)
            {
                var args = new ButtonIconNeededEventArgs(column, this.Rows[e.RowIndex], this.Rows[e.RowIndex].DataBoundItem);
                args.Icon = icon; // デフォルトicon一応設定
                args.ForeColor = fore; //default text color
                ButtonIconNeeded.Invoke(this, args);
                // Invokeしたら新Iconが来る
                icon = args.Icon;
                enabled = args.Enabled;
                backColor = args.BackColor;
                fore = args.ForeColor;
                btnText = args.Text ?? btnText;
            }

            var state = enabled ? System.Windows.Forms.VisualStyles.PushButtonState.Normal
                        : System.Windows.Forms.VisualStyles.PushButtonState.Disabled;

            if (backColor != Color.Empty)
            {
                this.DrawButtonColumnWithBackColor(e, icon, btnText, backColor, enabled, fore);
            }
            else
            {
                this.DrawButtonColumnWithoutBackColor(e, icon, btnText, state, fore);
            }
        }

        private void DrawButtonColumnWithBackColor(DataGridViewCellPaintingEventArgs e, Image icon, string btnText, Color backColor, bool enabled, Color fore)
        {
            Rectangle rect = e.CellBounds;
            Color fillColor = backColor != Color.Empty
                                ? backColor
                                : (enabled ? Color.White : Color.LightGray);

            using (var brush = new SolidBrush(fillColor))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Border
            using (var pen = new Pen(Color.Gray))
            {
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            // === ICON ===
            if (icon != null)
            {
                int padding = 4;
                int iconSize = Math.Min(rect.Height - padding * 2, icon.Height);

                Rectangle iconRect = new Rectangle(
                    rect.Left + (rect.Width - iconSize) / 2,
                    rect.Top + (rect.Height - iconSize) / 2,
                    iconSize, iconSize);

                if (enabled)
                    e.Graphics.DrawImage(icon, iconRect);
                else
                    ControlPaint.DrawImageDisabled(e.Graphics, icon, iconRect.X, iconRect.Y, Color.Transparent);
            }

            // === TEXT ===
            if (!string.IsNullOrEmpty(btnText))
            {
                Color textColor = enabled ? fore : Color.DarkGray;

                TextRenderer.DrawText(
                    e.Graphics,
                    btnText,
                    this.Font,
                    rect,
                    textColor,
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.EndEllipsis
                );
            }
        }

        private void DrawButtonColumnWithoutBackColor(DataGridViewCellPaintingEventArgs e, Image icon, string btnText, System.Windows.Forms.VisualStyles.PushButtonState state, Color fore)
        {
            if (icon != null)
            {
                // テキスト無しボタンを画く
                ButtonRenderer.DrawButton(e.Graphics, e.CellBounds, state);
                int padding = 4;
                int iconSize = Math.Min(e.CellBounds.Height - 2 * padding, icon.Height);

                // 横・縦の真中
                Rectangle iconRect = new Rectangle(
                    e.CellBounds.Left + (e.CellBounds.Width - iconSize) / 2,
                    e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2,
                    iconSize,
                    iconSize
                );

                e.Graphics.DrawImage(icon, iconRect);

                if (!string.IsNullOrEmpty(btnText))
                {
                    Rectangle textRect = new Rectangle(iconRect.Right + 2, e.CellBounds.Top, e.CellBounds.Width - iconRect.Right - padding, e.CellBounds.Height);
                    TextRenderer.DrawText(
                        e.Graphics,
                        btnText,
                        this.Font,
                        textRect,
                        fore,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
            else
            {
                // アイコン無し
                ButtonRenderer.DrawButton(e.Graphics, e.CellBounds, btnText, this.Font, false, state);
            }
        }

        private void DrawLinkColumn(DataGridViewCellPaintingEventArgs e, DataGridViewColumn column, Color fore)
        {
            if (this.LinkColumnEdit != null)
            {
                var args = new LinkColumnEditEventArgs(column, this.Rows[e.RowIndex], this.Rows[e.RowIndex].DataBoundItem);
                this.LinkColumnEdit.Invoke(this, args);
                if (args.EditType == GridColumnType.TextBox)
                {
                    // draw linkbutton as textbox
                    this.DrawTextBoxColumn(e, column, fore);
                    return;
                }
                // 他タイプも出来ます
            }

            var linkCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewLinkCell;
            string text = linkCell?.Value?.ToString() ?? "";
            text = string.IsNullOrEmpty(text) ? linkCell.FormattedValue.ToString() : text;

            using (Font linkFont = new Font(this.Font, FontStyle.Underline))
            {
                Color linkColor = Color.Blue;

                DataGridViewContentAlignment align = column.DefaultCellStyle.Alignment;
                TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

                switch (align)
                {
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.BottomCenter:
                        flags |= TextFormatFlags.HorizontalCenter;
                        break;

                    case DataGridViewContentAlignment.MiddleRight:
                    case DataGridViewContentAlignment.TopRight:
                    case DataGridViewContentAlignment.BottomRight:
                        flags |= TextFormatFlags.Right;
                        break;

                    default:
                        flags |= TextFormatFlags.Left;
                        break;
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    linkFont,
                    e.CellBounds,
                    linkColor,
                    flags
                );
            }
        }

        private void DrawTextBoxColumn(DataGridViewCellPaintingEventArgs e, DataGridViewColumn column, Color fore)
        {
            bool isNumeric = false;
            bool isDate = false;
            string format = string.Empty;
            Type underlyingType = null;

            Type valType = column.ValueType ?? (e.Value?.GetType());
            if (valType != null)
            {
                underlyingType = Nullable.GetUnderlyingType(valType) ?? valType;
                format = column.DefaultCellStyle.Format;

                isNumeric = new[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }
                    .Contains(underlyingType);

                isDate = underlyingType == typeof(DateTime);
            }

            string text = (e.Value ?? "").ToString();
            text = (e.FormattedValue ?? text).ToString();
            // ===== Numeric format =====
            if (string.IsNullOrEmpty(format) && (IgnoreAutoFormatColumns == null || !IgnoreAutoFormatColumns.Contains(column.Name)))
            {
                if (isNumeric)
                {
                    if (underlyingType == typeof(int) || underlyingType == typeof(long))
                        format = "N0";
                    else if (underlyingType == typeof(float) || underlyingType == typeof(decimal) || underlyingType == typeof(double))
                        format = "N2";
                }
                else if (isDate) format = "yyyy/MM/dd";
            }

            if (!string.IsNullOrEmpty(format))
            {
                if (isNumeric)
                {
                    if (e.Value != null && double.TryParse(e.Value.ToString(), out double num))
                        text = num.ToString(format);
                    else
                        text = string.Empty;
                }
                else if (isDate)
                {
                    if (e.Value is DateTime dt && dt > DateTime.MinValue)
                        text = dt.ToString(format);
                    else
                        text = string.Empty;
                }
            }

            // ========== NEW ICON SUPPORT ==========
            Image icon = null;
            bool? newReadOnly = null;
            int iconSize = 16;

            if (TextBoxIconNeeded != null)
            {
                var args = new TextBoxIconNeededEventArgs(column, this.Rows[e.RowIndex], this.Rows[e.RowIndex].DataBoundItem);
                TextBoxIconNeeded.Invoke(this, args);

                icon = args.Icon;
                iconSize = args.IconSize;
                newReadOnly = args.ReadOnly;

                if (args.TextValue != null) text = args.TextValue;
                if (newReadOnly != null)
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = newReadOnly.Value;
            }
            // =======================================


            // ========== MULTILINE SUPPORT ==========
            bool isMultiline = (column.DefaultCellStyle.WrapMode == DataGridViewTriState.True);

            if (isMultiline)
            {
                text = (e.FormattedValue ?? "").ToString();
                Size preferred = TextRenderer.MeasureText(
                    text,
                    this.Font,
                    new Size(e.CellBounds.Width, int.MaxValue),
                    TextFormatFlags.WordBreak
                );

                int needed = preferred.Height + 6;
                if (needed > this.Rows[e.RowIndex].Height)
                    this.Rows[e.RowIndex].Height = needed;
            }
            // ========================================


            // ========== TEXT ALIGNMENT ==========
            DataGridViewContentAlignment align = column.DefaultCellStyle.Alignment;
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            bool isCenter = false, isRight = false;

            switch (align)
            {
                case DataGridViewContentAlignment.MiddleCenter:
                case DataGridViewContentAlignment.TopCenter:
                case DataGridViewContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter;
                    isCenter = true;
                    break;

                case DataGridViewContentAlignment.MiddleRight:
                case DataGridViewContentAlignment.TopRight:
                case DataGridViewContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right;
                    isRight = true;
                    break;

                default:
                    flags |= TextFormatFlags.Left;
                    break;
            }

            if (align == DataGridViewContentAlignment.NotSet)
                flags |= TextFormatFlags.HorizontalCenter;

            if (isMultiline)
                flags |= TextFormatFlags.WordBreak;
            // =======================================


            Rectangle cell = e.CellBounds;
            Rectangle textRect = cell;


            // ===== ICON POSITION BY ALIGNMENT =====
            if (icon != null)
            {
                int iconX = cell.X + 4;
                int iconY = cell.Y + (cell.Height - iconSize) / 2;

                if (isCenter)
                    iconX = cell.X + (cell.Width - iconSize) / 2;

                if (isRight)
                    iconX = cell.Right - iconSize - 4;

                Rectangle iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
                e.Graphics.DrawImage(icon, iconRect);

                if (!isCenter)
                    textRect.X += iconSize + 6;

                textRect.Width -= iconSize + 6;
            }

            // ===== DRAW TEXT =====
            TextRenderer.DrawText(
                e.Graphics,
                text,
                this.Font,
                textRect,
                fore,
                flags
            );
        }

        private void DrawComboBoxColumn(
            DataGridViewCellPaintingEventArgs e,
            DataGridViewColumn column,
            Color back,
            Color fore
            )
        {
            var cell = this.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
            if (cell == null)
            {
                DrawTextBoxColumn(e, column, fore);
                return;
            }

            string text = GetComboDisplayText(cell);

            Rectangle rect = e.CellBounds;

            using (var b = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(b, rect);
            }

            TextFormatFlags flags =
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.Left |
                TextFormatFlags.EndEllipsis;

            Rectangle textRect = new Rectangle(rect.X + 3, rect.Y, rect.Width - 20, rect.Height);

            TextRenderer.DrawText(
                e.Graphics,
                text ?? string.Empty,
                this.Font,
                textRect,
                fore,
                flags
            );

            Rectangle dropRect = new Rectangle(
                rect.Right - 18,
                rect.Y + (rect.Height - 16) / 2,
                16,
                16
            );

            ControlPaint.DrawComboButton(
                e.Graphics,
                dropRect,
                ButtonState.Normal
            );
        }

        private void DrawTimeColumn(DataGridViewCellPaintingEventArgs e, DataGridViewColumn column, Color fore)
        {
            string text = (e.Value ?? "").ToString();
            bool isPlaceholder = string.IsNullOrEmpty(text);

            Rectangle cell = e.CellBounds;

            if (isPlaceholder)
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    "HH:mm (例: 23:59)",
                    this.Font,
                    cell,
                    Color.Gray,
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.Left |
                    TextFormatFlags.EndEllipsis
                );
            }
            else
            {
                // vẽ giá trị thật
                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    this.Font,
                    cell,
                    fore,
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.Left |
                    TextFormatFlags.EndEllipsis
                );
            }
        }
        #endregion

        #region override function

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseDown(e);

            if (e.Button != MouseButtons.Right ||
                e.RowIndex < 0 ||
                e.ColumnIndex < 0)
                return;

            try
            {
                // If editing — don't disturb editor
                if (this.IsCurrentCellInEditMode)
                    return;

                var target = this.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (this.CurrentCell != target)
                    this.CurrentCell = target;

                if (!this.Rows[e.RowIndex].Selected)
                {
                    this.ClearSelection();
                    this.Rows[e.RowIndex].Selected = true;
                }

                this.InvalidateCell(target);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Right-click select patch: " + ex.Message);
            }
        }

        /// <summary>
        /// for enter key to make new row on multiple rows cell
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (this.EditingControl is DataGridViewTextBoxEditingControl tb)
                {
                    if (tb.Multiline)
                    {
                        tb.SelectedText = "\r\n";
                        return true; // グリッドのエンターキイベント無視する
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        /// <summary>
        /// for draw group header row
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);

            var row = this.Rows[e.RowIndex];
            if (row?.Tag == null) return;

            // detect group row
            dynamic tag = row.Tag;
            bool isGroup = false;
            try { isGroup = tag.IsGroup; } catch { }

            if (!isGroup) return;

            DrawGroupHeaderFullRow(e, row);
        }
        /// <summary>
        /// header style like xtragrid
        /// </summary>
        /// <param name="e"></param>
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
        /// <summary>
        /// header style like xtragrid
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellMouseLeave(DataGridViewCellEventArgs e)
        {
            base.OnCellMouseLeave(e);

            if (e.RowIndex == -1 && _hoverColumnIndex != -1)
            {
                _hoverColumnIndex = -1;
                this.Invalidate();
            }
        }
        /// <summary>
        /// checkboxエディター無効化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            base.OnCellBeginEdit(e);
            //// checkboxエディター無効化
            //if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            //{
            //    e.Cancel = true;
            //    return;
            //}
            //--------------------------
            // Event Fire: 確認
            // CustomRowCellEdit
            //--------------------------
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                _oldCellValue = this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            else
            {
                _oldCellValue = null;
            }
            //--------------------------
        }

        /// <summary>
        /// sort by click header
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (_suppressSortOnce)
            {
                _suppressSortOnce = false;
                return;
            }

            base.OnColumnHeaderMouseClick(e);

            if (e.Button != MouseButtons.Left || e.ColumnIndex < 0)
                return;

            var col = this.Columns[e.ColumnIndex];
            if (col == null)
                return;

            if (this.DisabledSortAll || col.SortMode == DataGridViewColumnSortMode.NotSortable)
                return;

            var propName = string.IsNullOrEmpty(col.DataPropertyName)
                ? col.Name
                : col.DataPropertyName;

            ListSortDirection newDirection;
            if (string.Equals(_lastSortedColumnName, propName, StringComparison.OrdinalIgnoreCase))
            {
                newDirection = _lastSortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                newDirection = ListSortDirection.Ascending;
            }

            foreach (DataGridViewColumn c in this.Columns)
            {
                if (c.SortMode != DataGridViewColumnSortMode.NotSortable)
                    c.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            col.HeaderCell.SortGlyphDirection =
                (newDirection == ListSortDirection.Ascending)
                    ? SortOrder.Ascending
                    : SortOrder.Descending;

            _lastSortedColumnName = propName;
            _lastSortDirection = newDirection;

            _lastSortString = $"[{propName}] {(newDirection == ListSortDirection.Ascending ? "ASC" : "DESC")}";

            ApplyListFilterAndSort();
        }
        /// <summary>
        /// prevent error when has grouping
        /// </summary>
        /// <param name="displayErrorDialogIfNoHandler"></param>
        /// <param name="e"></param>
        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            // Ngăn popup mặc định của DataGridView
            e.ThrowException = false;
            e.Cancel = true;
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                // This is the critical fix
                this.Invalidate(new Rectangle(0, 0, this.Width, this.ColumnHeadersHeight));
            }
        }

        /// <summary>
        /// draw line like xtragridview to separate cells on grid
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // ADGV drawing

            for (int i = 0; i < this.Columns.Count; i++)
            {
                var rect = this.GetCellDisplayRectangle(i, -1, true);
                if (rect.Width <= 0 || rect.Height <= 0)
                    continue;

                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                           rect, Color.White, Color.LightGray, 90f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // ===== 2. BORDER =====
                using (var pen = new Pen(Color.Gray))
                {
                    e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }

                // ===== 3. HOVER HEADER =====
                if (i == _hoverColumnIndex)
                {
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(254, 211, 102)))
                    {
                        e.Graphics.FillRectangle(hoverBrush, rect);
                    }
                }

                // ===== 4. FIELD =====
                string propName = this.Columns[i].DataPropertyName;
                if (string.IsNullOrEmpty(propName))
                    propName = this.Columns[i].Name;

                // ===== 5. DISABLE FILTER =====
                string colName = this.Columns[i].Name;
                bool isFilterDisabled = this.DisabledFilterAll || DisabledFilterColumns.Contains(colName);

                // =====  FILTER =====
                bool isFiltered = (!isFilterDisabled && _filteredColumns.Contains(propName));

                // ===== 7. ICON FILTER 
                bool shouldDrawFilterIcon =
                        isFiltered || i == _hoverColumnIndex;

                if (shouldDrawFilterIcon)
                    this.DrawFilterIcon(
                        e.Graphics,
                        rect,
                        !isFilterDisabled   // true = normal, false = disabled (alpha)
                    );

                // ===== 8.HEADER TEXT =====
                TextRenderer.DrawText(
                    e.Graphics,
                    this.Columns[i].HeaderText,
                    this.Columns[i].HeaderCell.Style.Font ?? this.Font,
                    rect,
                    this.Columns[i].HeaderCell.Style.ForeColor,
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.HorizontalCenter
                );
            }
        }
        /// <summary>
        /// drag and drop column
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // ✅ CHECK IF MOUSE IS ON COLUMN RESIZE GRIP
            if (e.Button == MouseButtons.Left && IsMouseOnColumnResizeGrip(e.Location))
            {
                _isResizingColumn = true;  // ✅ Set flag
                _draggingStarted = false;  // ✅ Prevent drag/drop
                base.OnMouseDown(e);
                return;
            }

            // Reset resize flag
            _isResizingColumn = false;

            // Drag start detect
            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = e.Location;
                _draggingStarted = false;
            }

            var hit = this.HitTest(e.X, e.Y);

            if (hit.Type != DataGridViewHitTestType.ColumnHeader || hit.ColumnIndex < 0)
            {
                base.OnMouseDown(e);
                return;
            }

            var col = this.Columns[hit.ColumnIndex];
            if (col == null)
            {
                base.OnMouseDown(e);
                return;
            }

            // ✅ Filter allow
            bool filterEnabled =
                !this.DisabledFilterAll &&
                !DisabledFilterColumns.Contains(col.Name);

            try
            {
                var headerRect = this.GetCellDisplayRectangle(hit.ColumnIndex, -1, true);
                if (headerRect.Width > 0 && headerRect.Height > 0)
                {
                    int iconSize = 16;
                    int iconLeft = headerRect.Right - iconSize - 4;
                    var iconArea = new Rectangle(
                        iconLeft,
                        headerRect.Top + 3,
                        iconSize,
                        iconSize
                    );

                    if (!filterEnabled && iconArea.Contains(e.Location))
                    {
                        return;
                    }

                    if (iconArea.Contains(e.Location))
                    {
                        _suppressSortOnce = true;
                        base.OnMouseDown(e);
                        return;
                    }
                }
            }
            catch
            {
                base.OnMouseDown(e);
                return;
            }

            base.OnMouseDown(e);
        }
        /// <summary>
        /// Detects if the mouse is positioned over a column resize grip (divider between columns).
        /// </summary>
        private bool IsMouseOnColumnResizeGrip(Point mouseLocation)
        {
            var hit = this.HitTest(mouseLocation.X, mouseLocation.Y);

            if (hit.Type != DataGridViewHitTestType.ColumnHeader || hit.ColumnIndex < 0)
                return false;

            var col = this.Columns[hit.ColumnIndex];

            // Can't resize if column resizing is disabled
            if (!this.AllowUserToResizeColumns || col.Resizable == DataGridViewTriState.False)
                return false;

            var headerRect = this.GetCellDisplayRectangle(hit.ColumnIndex, -1, true);

            if (headerRect == Rectangle.Empty)
                return false;

            // Resize grip is typically 4-6 pixels wide
            int gripWidth = 4;

            // Check RIGHT edge of current column
            bool nearRightEdge = (mouseLocation.X >= headerRect.Right - gripWidth &&
                                  mouseLocation.X <= headerRect.Right + gripWidth);

            // Check LEFT edge of current column (only if not first column)
            bool nearLeftEdge = false;
            if (hit.ColumnIndex > 0)
            {
                nearLeftEdge = (mouseLocation.X >= headerRect.Left - gripWidth &&
                                mouseLocation.X <= headerRect.Left + gripWidth);
            }

            return nearRightEdge || nearLeftEdge;
        }
        /// <summary>
        /// drag and drop column
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // ✅ Show resize cursor when hovering over column divider
            if (IsMouseOnColumnResizeGrip(e.Location))
            {
                this.Cursor = Cursors.VSplit;
            }
            else if (this.Cursor == Cursors.VSplit)
            {
                this.Cursor = Cursors.Default;
            }

            base.OnMouseMove(e);

            // ✅ SKIP drag/drop logic if currently resizing
            if (_isResizingColumn)
                return;

            if (e.Button == MouseButtons.Left && !_draggingStarted)
            {
                var dx = Math.Abs(e.X - _dragStartPoint.X);
                var dy = Math.Abs(e.Y - _dragStartPoint.Y);
                var dragThreshold = SystemInformation.DragSize;

                if (dx >= dragThreshold.Width || dy >= dragThreshold.Height)
                {
                    // ✅ Check if drag STARTED on a column header (not just current position)
                    var startHit = this.HitTest(_dragStartPoint.X, _dragStartPoint.Y);

                    if (startHit.Type == DataGridViewHitTestType.ColumnHeader && startHit.ColumnIndex >= 0)
                    {
                        var col = this.Columns[startHit.ColumnIndex];
                        if (col != null)
                        {
                            var data = new System.Windows.Forms.DataObject();
                            data.SetData(typeof(string), col.Name);
                            try
                            {
                                data.SetData(typeof(DataGridViewColumn), col);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("OnMouseMove: " + ex.Message);
                            }

                            _draggingStarted = true;
                            this.DoDragDrop(data, DragDropEffects.Move);
                        }
                    }
                }
            }

            // ✅ Existing hover logic for column headers (using HitTest, not e.RowIndex)
            var hit = this.HitTest(e.X, e.Y);

            if (hit.RowIndex == -1) // header row
            {
                if (_hoverColumnIndex != hit.ColumnIndex)
                {
                    _hoverColumnIndex = hit.ColumnIndex;
                    this.Invalidate();
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
        /// <summary>
        /// OnMouseUp: reset flag ( drag & drop column)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _draggingStarted = false;
            _isResizingColumn = false;  // ✅ Reset resize flag
        }

        private void DrawFilterIcon(
            Graphics g,
            Rectangle headerRect,
            bool enabled)
        {
            if (_filterIcon == null)
                return;

            int iconSize = 16;
            int x = headerRect.Right - iconSize - 4;
            int y = headerRect.Top + (headerRect.Height - iconSize) / 2;

            var destRect = new Rectangle(x, y, iconSize, iconSize);

            if (enabled)
            {
                // ✅ Normal icon
                g.DrawImage(_filterIcon, destRect);
            }
            else
            {
                // ✅ Disabled icon (alpha)
                using (var ia = new ImageAttributes())
                {
                    var matrix = new ColorMatrix
                    {
                        Matrix33 = 0.35f // alpha (0 = transparent, 1 = opaque)
                    };
                    ia.SetColorMatrix(matrix);

                    g.DrawImage(
                        _filterIcon,
                        destRect,
                        0, 0,
                        _filterIcon.Width,
                        _filterIcon.Height,
                        GraphicsUnit.Pixel,
                        ia
                    );
                }
            }
        }

        /// <summary>
        /// Filter and and sort(inside page)
        /// </summary>
        /// <param name="e"></param>
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

            // === KEEP FILTER & SORT AFTER REBIND ===
            if (KeepFilterAndSort &&
                _originalListData != null &&
                (!string.IsNullOrWhiteSpace(_lastFilterString) ||
                 !string.IsNullOrWhiteSpace(_lastSortString)))
            {
                try
                {
                    ReapplyCurrentFilterAndSort();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("KeepFilterAndSort apply failed: " + ex.Message);
                }
            }
        }

        protected override void OnCurrentCellDirtyStateChanged(EventArgs e)
        {
            base.OnCurrentCellDirtyStateChanged(e);

            // for checkbox
            if ((this.CurrentCell is DataGridViewCheckBoxCell || this.CurrentCell.OwningColumn is DataGridViewCheckBoxColumn) && this.IsCurrentCellDirty)
            {
                this.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        /// <summary>
        /// when has grouping need update bindingsource original to get true updated value on parent form 
        /// for CellValueChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            base.OnCellValueChanged(e);

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // For Grid has Mix Datasource
            DataGridViewRow row = this.Rows[e.RowIndex];
            if (row.Tag != null)
            {
                var tag = row.Tag;
                var prop = tag.GetType().GetProperty("DataItem");
                if (prop == null) return;

                var dataItem = prop.GetValue(tag);
                if (dataItem == null) return;

                var col = this.Columns[e.ColumnIndex];
                var modelProp = dataItem.GetType().GetProperty(col.DataPropertyName);
                if (modelProp == null) return;

                // update original object in BindingSource
                modelProp.SetValue(dataItem, row.Cells[e.ColumnIndex].Value);
            }
            else
            {

                // for simple datasource
                // calculating cell
                if (_isCalculating) return; // calculating do not stuff

                try
                {
                    _isCalculating = true; // calculating

                    DataGridViewCell cell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    object newValue = cell.Value;

                    DataGridViewColumn column = this.Columns[e.ColumnIndex];

                    //--------------------------
                    // Set Value from originary Grid Event 
                    //--------------------------
                    var args = new CustomCellValueChangedEventArgs(e.RowIndex, e.ColumnIndex, _oldCellValue, newValue, column);
                    OnCustomCellValueChanged(args);

                    _oldCellValue = null;

                    //this.NotifyCurrentCellDirty(false);
                    //--------------------------
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    _isCalculating = false; // calculated
                }
            }

        }

        //--------------------------
        //-- 追加
        //--------------------------
        protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                try
                {
                    Rectangle cellRect = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    Point gridPoint = new Point(cellRect.X + e.X, cellRect.Y + e.Y);

                    var (top, bottom) = GetMergeRangeForCell(this.Columns[e.ColumnIndex], e.ColumnIndex, e.RowIndex);

                    var glyphRect = GetCheckBoxGlyphRect(e.ColumnIndex, top);

                    if (glyphRect != Rectangle.Empty && glyphRect.Contains(gridPoint))
                    {
                        ToggleCheckBoxCellAt(e.ColumnIndex, e.RowIndex);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("OnCellMouseClick glyph check: " + ex.Message);
                }
            }

            base.OnCellMouseClick(e);

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewColumn clickedColumn = this.Columns[e.ColumnIndex];
                Rectangle rcell = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                Point gp = new Point(rcell.X + e.X, rcell.Y + e.Y);
                GridHitInfo hitInfo = CalcHitInfo(gp);
                var args = new CustomRowCellClickEventArgs(e, hitInfo, clickedColumn);
                OnCustomRowCellClick(args);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //this.DoubleBuffered = true;
            //this.UpdateStyles();
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            // data row only
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewColumn currentColumn = this.Columns[e.ColumnIndex];
                // init EventArgs
                var args = new CustomColumnDisplayTextEventArgs(e.ColumnIndex, e.RowIndex, e.Value, currentColumn);
                
                // run event
                OnCustomColumnDisplayText(args);

                // Show data to DataGridView
                if (args.DisplayText != null)
                {
                    e.Value = args.DisplayText;
                    e.FormattingApplied = true;
                }
            }

            base.OnCellFormatting(e);
        }
        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="topRowIndex"></param>
        /// <returns></returns>
        private Rectangle GetCheckBoxGlyphRect(int columnIndex, int topRowIndex)
        {
            if (columnIndex < 0 || topRowIndex < 0) return Rectangle.Empty;

            Rectangle topRect = this.GetCellDisplayRectangle(columnIndex, topRowIndex, false);
            if (topRect == Rectangle.Empty) return Rectangle.Empty;

            using (Graphics g = this.CreateGraphics())
            {
                var glyphSize = CheckBoxRenderer.GetGlyphSize(g, System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
                int cx = topRect.Left + (topRect.Width - glyphSize.Width) / 2;
                int cy = topRect.Top + (topRect.Height - glyphSize.Height) / 2;
                return new Rectangle(cx, cy, glyphSize.Width, glyphSize.Height);
            }
        }

        private void ToggleCheckBoxCellAt(int colIndex, int rowIndex)
        {
            if (colIndex < 0 || rowIndex < 0) return;
            var col = this.Columns[colIndex];
            if (!(col is DataGridViewCheckBoxColumn)) return;

            DataGridViewRow row = this.Rows[rowIndex];
            if (row == null) return;

            var cell = row.Cells[colIndex];

            object oldValObj = cell.Value;
            bool oldVal = ConvertToBool(oldValObj);
            bool newVal = !oldVal;

            // Set cell value (visual)
            cell.Value = newVal;

            // Update underlying model if possible (grouped via Tag.DataItem or DataBoundItem)
            try
            {
                string dp = col.DataPropertyName;

                if (row.Tag != null)
                {
                    var tag = row.Tag;
                    var prop = tag.GetType().GetProperty("DataItem");
                    if (prop != null)
                    {
                        var dataItem = prop.GetValue(tag);
                        if (dataItem != null && !string.IsNullOrEmpty(dp))
                        {
                            if (dataItem is DataRowView drv)
                                drv[dp] = newVal;
                            else if (dataItem is DataRow dr)
                                dr[dp] = newVal;
                            else
                            {
                                var modelProp = dataItem.GetType().GetProperty(dp);
                                if (modelProp != null && modelProp.CanWrite)
                                    modelProp.SetValue(dataItem, newVal);
                            }
                        }
                    }
                }
                else if (row.DataBoundItem != null && !string.IsNullOrEmpty(dp))
                {
                    var dataBound = row.DataBoundItem;
                    if (dataBound is DataRowView drv2)
                        drv2[dp] = newVal;
                    else
                    {
                        var modelProp = dataBound.GetType().GetProperty(dp);
                        if (modelProp != null && modelProp.CanWrite)
                            modelProp.SetValue(dataBound, newVal);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ToggleCheckBoxCellAt: update model failed: " + ex.Message);
            }

            // Commit (so databinding / CellValueChanged flows happen)
            try
            {
                this.CurrentCell = cell;
                this.NotifyCurrentCellDirty(true);
                this.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            catch { }

            // Raise custom CellValueChanged so parent can listen
            var args = new CustomCellValueChangedEventArgs(rowIndex, colIndex, oldValObj, newVal, col);
            OnCustomCellValueChanged(args);

            // Repaint
            this.InvalidateCell(cell);
        }

        public void DisableFilterAndSortForButtonColumns(bool disabled = true)
        {
            foreach (DataGridViewColumn col in this.Columns)
            {
                if (col is DataGridViewButtonColumn)
                {
                    if (disabled)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                        //this.DisableFilterAndSort(col);
                    }
                    else
                    {
                        col.SortMode = DataGridViewColumnSortMode.Programmatic;
                        //this.EnableFilterAndSort(col);
                    }
                }
            }
            this.Invalidate();
        }

        public bool DeleteRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= this.Rows.Count || this.Rows[rowIndex].IsNewRow)
            {
                return false; // 削除のデータ行じゃない場合
            }

            object dataItem = this.Rows[rowIndex].DataBoundItem;

            if (dataItem is DataRowView drv)
            {
                // DataRowStateを'Deleted'に変わる
                // DataGridView行の消す
                drv.Delete();
                return true;
            }
            else if (this.DataSource is BindingSource bs && rowIndex < bs.Count)
            {
                /// データソース：BindingSourceの場合
                bs.RemoveAt(rowIndex);
                return true;
            }

            return false;
        }

        internal void AddNewRow()
        {
            //get Datasource
            if (this.DataSource == null) return;
            BindingSource bd = this.DataSource as BindingSource;

            bd.AddNew();
            bd.MoveLast();

            int newRowIndex = this.NewRowIndex != -1 ? this.NewRowIndex - 1 : this.Rows.Count - 1;
            if (this.AllowUserToAddRows == false) newRowIndex = this.Rows.Count - 1;

            this.CurrentCell = this.Rows[newRowIndex].Cells[0];
            this.BeginEdit(true);

            var args = new CustomInitNewRowEventArgs(newRowIndex);
            this.OnCustomInitNewRow(args);
        }

        protected override void OnDefaultValuesNeeded(DataGridViewRowEventArgs e)
        {
            base.OnDefaultValuesNeeded(e);

            var args = new CustomInitNewRowEventArgs(e.Row.Index);
            this.OnCustomInitNewRow(args);
        }

        //for LayoutChanged
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        //for CellValueChanged
        protected virtual void OnCustomCellValueChanged(CustomCellValueChangedEventArgs e)
        {
            CustomCellValueChanged?.Invoke(this, e);
        }

        //--------------------------
        // Customize Event functions
        //--------------------------
        #region Customize Event functions
        public void OnCustomColumnDisplayText(CustomColumnDisplayTextEventArgs e)
        {
            CustomColumnDisplayText?.Invoke(this, e);
        }

        public void OnCustomRowCellEdit(CustomRowCellEditEventArgs e)
        {
            CustomRowCellEdit?.Invoke(this, e);
        }

        public void OnCustomColumnData(CustomColumnDataEventArgs e)
        {
            CustomColumnData?.Invoke(this, e);
        }

        protected void OnCustomRowCellClick(CustomRowCellClickEventArgs e)
        {
            CustomRowCellClick?.Invoke(this, e);
        }

        protected virtual void OnCustomInitNewRow(CustomInitNewRowEventArgs e)
        {
            CustomInitNewRow?.Invoke(this, e);
        }

        public void OnButtonEditClick(DataGridViewCellEventArgs e)
        {
            ButtonEditClick?.Invoke(this, e);
        }
        #endregion Customize Event functions

        public GridHitInfo CalcHitInfo(Point pt)
        {
            DataGridView.HitTestInfo hitTest = this.HitTest(pt.X, pt.Y);
            GridHitInfo hitInfo = new GridHitInfo();

            hitInfo.RowIndex = hitTest.RowIndex;
            hitInfo.ColumnIndex = hitTest.ColumnIndex;

            // HitTestType DataGridView HitTestType
            switch (hitTest.Type)
            {
                case DataGridViewHitTestType.Cell:
                    hitInfo.HitType = GridHitTest.RowCell;
                    hitInfo.Description = $"Cell ({hitTest.ColumnIndex}, {hitTest.RowIndex})";
                    break;

                case DataGridViewHitTestType.RowHeader:
                    hitInfo.HitType = GridHitTest.RowHeader;
                    hitInfo.Description = $"Row Header ({hitTest.RowIndex})";
                    break;

                case DataGridViewHitTestType.ColumnHeader:
                    hitInfo.HitType = GridHitTest.ColumnHeader;
                    hitInfo.Description = $"Column Header ({hitTest.ColumnIndex})";
                    break;

                case DataGridViewHitTestType.TopLeftHeader:
                    hitInfo.HitType = GridHitTest.TopLeftHeader;
                    hitInfo.Description = "Top Left Header";
                    break;

                case DataGridViewHitTestType.HorizontalScrollBar:
                case DataGridViewHitTestType.VerticalScrollBar:
                    hitInfo.HitType = GridHitTest.Scrollbar;
                    hitInfo.Description = "Scroll Area";
                    break;

                case DataGridViewHitTestType.None:
                    // is Empty area
                    if (hitTest.RowIndex == -1 && hitTest.ColumnIndex == -1)
                    {
                        hitInfo.HitType = GridHitTest.EmptyRow;
                        hitInfo.Description = "Empty Area / None";
                    }
                    break;

                default:
                    hitInfo.HitType = GridHitTest.None;
                    hitInfo.Description = "Unknown Area";
                    break;
            }

            return hitInfo;
        }

        /// <summary>
        /// Re-apply current filter & sort state to newly bound data.
        /// Caller (Form A) decides WHEN to call this.
        /// </summary>
        public void ReapplyCurrentFilterAndSort()
        {
            if (_originalListData == null)
                return;

            try
            {
                ApplyListFilterAndSort();
                UpdateClearFilterButtonVisibility();
                this.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ReapplyCurrentFilterAndSort: " + ex.Message);
            }
        }

        /// <summary>
        /// Clear filter/sort state explicitly (caller-controlled).
        /// </summary>
        public void ClearFilterState(bool clearUI = true)
        {
            _filteredColumns.Clear();
            _lastFilterString = "";
            _lastSortString = "";

            if (clearUI)
            {
                this.CleanFilterAndSort();
                _clearFilterButton.Visible = false;
                this.Invalidate();
            }
        }

        public (string filter, string sort) CaptureFilterState()
        {
            return (_lastFilterString, _lastSortString);
        }

        public void RestoreFilterState(string filter, string sort)
        {
            _lastFilterString = filter ?? "";
            _lastSortString = sort ?? "";

            UpdateFilteredColumnsFromFilterString(_lastFilterString);
            ReapplyCurrentFilterAndSort();
        }
    }
}