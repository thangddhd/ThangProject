using coms.COMSK.common;
using coms.COMSKService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace coms.COMSK.ui.common
{
    /// <summary>
    /// 長期修繕計画総括表用のグリッドコントロール
    /// </summary>
    public partial class CtrLongtermRepairPlan_Total<T> : UserControl
	{
		#region メンバ変数

		/// <summary>
        /// 列マージ制御クラス
        /// </summary>
        private MyCellMergeHelper m_Helper;

        /// <summary>
        /// グループ管理クラス
        /// </summary>
        private GridGroupMgr m_mgrGroup;

		/// <summary>
		/// 表示開始する会計期のインデックス
		/// </summary>
		private int startPeriodIndex = 0;

		Dictionary<string, int> dictBandWidth = new Dictionary<string, int>();
		private int zoomFactor = 100; // Initial zoom factor
		private K300030020 parentForm;
		private int accountStartPeriodIdx = 0;  // 表の開始年変更場合これも変更される
		private int originalAccountStartPeriodIdx = 0;  // 作成時の開始年

		private LongRepairGridView<LongRepairPlanData> _grid;
		private BindingSource _bs = new BindingSource();

		private LongRepairGridView<LongRepairPlanData> _gridRight;

        private Panel panelContent = new Panel();
        private Panel panelContainer = new Panel();
        private Panel panelBottom = new Panel();

        private bool _syncingScroll = false;
        private bool _syncingSelection = false;

        private HScrollBar hScroll = new HScrollBar();
		private Color _gridBorderColor = Color.FromArgb(140, 116, 100);

        #endregion メンバ変数

        #region プロパティ

        /// <summary>
        /// データソース
        /// </summary>
        private List<LongRepairPlanData> data = null;
		[Browsable(false)]
		public List<LongRepairPlanData> DataSource
		{
			get
			{
				return _bs.DataSource as List<LongRepairPlanData>;
			}
			set
			{
				if (value != null)
				{
					data = ConvertLongRepairPlanDatas(value);
				}
				else
				{
					data = new List<LongRepairPlanData>();
				}
				//_bs = new BindingSource();
				_grid.SuspendCurrentCellChanged();
				_gridRight.SuspendCurrentCellChanged();
				try
				{
					_bs.DataSource = data;
					_grid.DataSource = _bs;
					_gridRight.DataSource = _bs;

					// do merge
					_grid.RebuildMerges();

					_grid.ClearInitialSelectionState();
					_gridRight.ClearInitialSelectionState();

					UpdateRightPanelWidth();
					UpdateScrollBar();
				}
				finally
				{
					_grid.ResumeCurrentCellChanged();
					_gridRight.ResumeCurrentCellChanged();
				}
			}
		}

        /// <summary>
        /// 長計プロパティ
        /// </summary>
        [Browsable(false)]
		public MaintenancePlanConst MntPlanConst { get; set; }

        /// <summary>
        /// セルでーたを変更できるかどうかプロバティ
        /// </summary>
        public bool Editable
        {
			get;
			set;
			//TODO
            /*get { return gridvLongtermRepairPlan.OptionsBehavior.Editable; }
            set { gridvLongtermRepairPlan.OptionsBehavior.Editable = value; }*/
        }

		#endregion プロパティ

        #region コンストラクタ
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CtrLongtermRepairPlan_Total()
        {
			//m_Helper = null;
			//m_mgrGroup = new GridGroupMgr();

			InitializeComponent();
			InitGridLayout();

            this.initGridView();

			this.Load += CtrLongtermRepairPlan_Total_Load;
            
            //HookEvents();
            // TODO grouping
            // repLinkPlusのPlus画像の調整
            //repLinkPlus.Image = COMSKCommon.GetPlusBitmap();
        }

        // ////////////////////////////////////////////
        #region Fit Right Column

        private void InitGridLayout()
        {
            int scrollH = SystemInformation.HorizontalScrollBarHeight;
            //
            hScroll.Dock = DockStyle.Bottom;
            hScroll.Height = scrollH;
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = scrollH + 1;
            panelBottom.Controls.Add(hScroll);
            panelContainer.Dock = DockStyle.Fill;

            _grid = new LongRepairGridView<LongRepairPlanData>();
			_gridRight = new LongRepairGridView<LongRepairPlanData>();
			
            _gridRight.Dock = DockStyle.Right;
            _gridRight.Width = 100;

            _grid.Dock = DockStyle.Fill;
            
            panelContent.Dock = DockStyle.Fill;
            panelContent.Controls.Add(_gridRight);
            panelContent.Controls.Add(_grid);
            _grid.BringToFront();

            panelContainer.BackColor = _gridBorderColor;
            panelContainer.Padding = new Padding(2, 1, 2, 1);
            panelContainer.Controls.Add(panelContent);
            panelContainer.Controls.Add(panelBottom);
            this.Controls.Add(panelContainer);
        }
        private void initGridView()
		{
            // Important for manual columns
            _grid.AutoGenerateColumns = false;

            // Optional baseline settings
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
			_grid.RowHeadersVisible = false;
			_grid.DefaultCellStyle.BackColor = Color.White;
			_grid.DefaultCellStyle.ForeColor = Color.Black;
			_grid.BackgroundColor = Color.White;
			_grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _grid.ScrollBars = ScrollBars.None;
			_grid.AllowUserToResizeRows = false;
            _grid.BorderStyle = BorderStyle.None;

            _gridRight.ScrollBars = ScrollBars.Vertical;
            _gridRight.AllowUserToResizeRows = false;
            _gridRight.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _gridRight.AutoGenerateColumns = false;
            _gridRight.AllowUserToAddRows = false;
            _gridRight.AllowUserToDeleteRows = false;
            _gridRight.RowHeadersVisible = false;
            _gridRight.DefaultCellStyle.BackColor = Color.White;
            _gridRight.DefaultCellStyle.ForeColor = Color.Black;
            _gridRight.BackgroundColor = Color.White;
            _gridRight.BorderStyle = BorderStyle.None;
            // create columns
            AddColumnsNonYearly(_grid);
			
            // Configure merge behavior
            _grid.SetVerticalMergeColumns(new[] {
				"bgcolType",
				"bgcolItem"
			});
			_grid.VerticalMergeProvider = new LongRPAllMergeProvider();

			// Left columns list (for calc-row horizontal merge) - not used for testObj unless you set IsCalcRow
			_grid.SetLeftColumnNames(new[] {
				"bgcolType",
				"bgcolItem"
			});
            
            _grid.DrawMergeTextColumnNames = new[] { "bgcolType"};

            // No calc rows in testObj by default, so keep null (or set false)
            var calcTypes = COMSKCommon.calcTypes();
            // ----------------
            _grid.IsCalcRow = m => m != null && calcTypes.Contains(m.Row);

			// mouse wheel
			_grid.MouseWheel += Grid_MouseWheel;

			// readonly
			_grid.CellReadOnlyNeeded += Grid_CellReadOnlyNeeded;

			// format text
			_grid.CellDisplayTextNeeded += Grid_CellDisplayTextNeeded;

			// cell style
			_grid.CellStyleNeeded += Grid_CellStyleNeeded;

			_grid.CanDragCell = (g, rowIndex, col, model) =>
			{
				// old behavior: only RepairPlan rows + column.Tag must match
				if (model == null) return false;
				//if (model.Row != "BBB") return false;
				return (col.Tag as string) == COMSKCommon.TAG_DRAGGABLE_CELL;
			};

            //開閉グループのクリックイベント
            _grid.CellClick += Grid_OnCellClick;
            // ----------------
            _gridRight.IsCalcRow = m => m != null && calcTypes.Contains(m.Row);

			// mouse wheel
			_gridRight.MouseWheel += Grid_MouseWheel;

			// readonly
			_gridRight.CellReadOnlyNeeded += Grid_CellReadOnlyNeeded;

			// format text
			_gridRight.CellDisplayTextNeeded += Grid_CellDisplayTextNeeded;

			// cell style
			_gridRight.CellStyleNeeded += Grid_CellStyleNeeded;

			_gridRight.CanDragCell = (g, rowIndex, col, model) =>
			{
				// old behavior: only RepairPlan rows + column.Tag must match
				if (model == null) return false;
				//if (model.Row != "BBB") return false;
				return (col.Tag as string) == COMSKCommon.TAG_DRAGGABLE_CELL;
			};

			// ----------------
			// freeze
			FreezeLeftColumns();

			// Example header layout (2 rows)
			//_grid.SetHeaderLayout(BuildBandedHeaderNonYearly());

            HookEvents();
        }
        private void CtrLongtermRepairPlan_Total_Load(object sender, EventArgs e)
        {
            this.PerformLayout();
            UpdateRightPanelWidth();
            UpdateScrollBar();
        }

        private void HookEvents()
        {
            _grid.Scroll += SyncVerticalScroll;
            _gridRight.Scroll += SyncVerticalScroll;
            InitScroll();

            _grid.SelectionChanged += SyncSelectionMain;
            _gridRight.SelectionChanged += SyncSelectionRight;

            _grid.ColumnWidthChanged += (s, e) =>
            {
                UpdateScrollBar();
                SyncColumnWidth(e.Column);
            };

			_gridRight.ColumnWidthChanged += (s, e) => { SyncColumnWidth(e.Column); UpdateRightPanelWidth(); };
            _gridRight.DataBindingComplete += (s, e) => { UpdateRightPanelWidth(); };

            this.Resize += (s, e) => { UpdateRightPanelWidth(); UpdateScrollBar(); };
            
        }
        

        // Fixed RIGHT (expression)
        public void SetRightColumns(params string[] cols)
        {
			_gridRight.Columns.Clear();

			// reset visible
			foreach (DataGridViewColumn col in _grid.Columns)
				col.Visible = true;

			foreach (var name in cols)
            {
                if (!_grid.Columns.Contains(name)) continue;

                var col = _grid.Columns[name];

                var newCol = (DataGridViewColumn)col.Clone();
                newCol.DataPropertyName = col.DataPropertyName;
                newCol.HeaderText = col.HeaderText;
				newCol.Width = col.Width;
				newCol.DefaultCellStyle = col.DefaultCellStyle;
				newCol.DisplayIndex = col.DisplayIndex;
				newCol.Resizable = col.Resizable;
				newCol.ValueType = col.ValueType;
				newCol.Tag = col.Tag;
                _gridRight.Columns.Add(newCol);

				_grid.Columns[name].Visible = false;
			}

            UpdateRightPanelWidth();
            UpdateScrollBar();
        }

        // Sync scroll
        private void SyncVerticalScroll(object sender, ScrollEventArgs e)
        {
			if (_syncingScroll) return;
			if (e.ScrollOrientation != ScrollOrientation.VerticalScroll) return;

            _syncingScroll = true;

			int index = ((DataGridView)sender).FirstDisplayedScrollingRowIndex;

			SetScroll(_grid, index); _grid.Update();
			SetScroll(_gridRight, index); _gridRight.Update();

			_syncingScroll = false;
        }

        private void SetScroll(DataGridView dgv, int idx)
		{
            if (idx < 0 || idx >= dgv.RowCount) return;
            try
			{
				dgv.FirstDisplayedScrollingRowIndex = idx;
			}
			catch { }
		}

		// Sync selection
		private void SyncSelectionMain(object sender, EventArgs e)
        {
            if (_syncingSelection) return;
            if (_grid.CurrentRow == null) return;

            _syncingSelection = true;

            int idx = _grid.CurrentRow.Index;

            _gridRight.ClearSelection();
            if (idx >= 0 && idx < _gridRight.Rows.Count)
                _gridRight.Rows[idx].Selected = true;

            _syncingSelection = false;
        }

        private void SyncSelectionRight(object sender, EventArgs e)
        {
            if (_syncingSelection) return;
            if (_gridRight.CurrentRow == null) return;

            _syncingSelection = true;

            int idx = _gridRight.CurrentRow.Index;

            _grid.ClearSelection();
            if (idx >= 0 && idx < _grid.Rows.Count)
                _grid.Rows[idx].Selected = true;

            _syncingSelection = false;
        }

        // ※ Sync column width
        private void SyncColumnWidth(DataGridViewColumn col)
        {
            if (_gridRight.Columns.Contains(col.Name))
            {
                _gridRight.Columns[col.Name].Width = col.Width;
                //_gridRight.Refresh();
            }
        }

        private void UpdateRightPanelWidth()
        {
            int totalWidth = _gridRight.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .Sum(c => c.Width);
            bool isVScrollVisible = _gridRight.Rows.Count * _gridRight.RowTemplate.Height > _gridRight.ClientSize.Height;
            int scrollw = isVScrollVisible ? SystemInformation.VerticalScrollBarWidth + 2 : 0;
            _gridRight.Width = totalWidth + scrollw;
        }

        private void UpdateScrollBar()
        {
			int totalWidth = _grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
			int visibleWidth = _grid.ClientSize.Width - (_grid.RowHeadersVisible ? _grid.RowHeadersWidth : 0) - 70;
			if (totalWidth > visibleWidth)
			{
				hScroll.Enabled = true;
				hScroll.Maximum = totalWidth;
				hScroll.LargeChange = visibleWidth;
				hScroll.SmallChange = 10; //
				hScroll.Value = _grid.HorizontalScrollingOffset;
			}
			else
			{
				hScroll.Enabled = false;
				hScroll.Value = 0;
			}
		}

        private void InitScroll()
        {
            hScroll.Scroll += (s, e) =>
            {
                _grid.HorizontalScrollingOffset = hScroll.Value;
            };
        }

        #endregion Frozen Right Column
        // ////////////////////////////////////////////

        private void AddColumnsNonYearly(DataGridView grid)
		{
			grid.Columns.Clear();
            grid.Columns.Add(CreateCol("bgcolType", "ConstructionTypeName", "区分", 70));
			grid.Columns.Add(CreateCol("bgcolItem", "ConstructionCategoryName", "項目", 200));
			grid.Columns.Add(CreateCol("bgcolDummy", "", "Dummy", 90));
            grid.Columns.Add(CreateCol("bgcolSubTotal", "SubTotal", "小計", 75, true));
		}

		private DataGridViewColumn CreateCol(string name, string field, string caption, int width, bool isNumberCol = false)
		{
			var col = new DataGridViewTextBoxColumn()
            {
                Name = name,
                DataPropertyName = field,
                HeaderText = caption,
                Width = width
            };
			if (isNumberCol)
			{
                col.DefaultCellStyle.Format = "N0";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
			return col;
		}

		private void FreezeLeftColumns()
		{
			_grid.Columns["bgcolType"].Frozen = true;
			_grid.Columns["bgcolItem"].Frozen = true;
		}

		private HeaderBandLayout BuildBandedHeaderNonYearly()
		{
			var layout = new HeaderBandLayout();
			layout.HeaderRowCount = 3;
			layout.HeaderRowHeight = 22;
			// ===== Row 0 (full height columns)
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3,
				new[] { "bgcolType", "bgcolItem" }, "想定修繕工事項目", true));
			// ===== 築年 (nested 3 level)
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 1,
				new[] { "bgcolDummy" }, "築年", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(1, 1,
				new[] { "bgcolDummy" }, "会計期", false));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(2, 1,
				"bgcolDummy", "会計年度", false));
			// ===== subtotal
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolSubTotal", "小計", true));

			return layout;
		}

		internal sealed class LongRPAllMergeProvider : IVerticalMergeProvider<LongRepairPlanData>
		{
			public bool MergeWithNextRow(DataGridView grid, LongRepairPlanData row, LongRepairPlanData nextRow, string columnName, int rowIndex)
			{
				var calcTypes = COMSKCommon.calcTypes();
				if (row == null || nextRow == null || calcTypes.Contains(row.Row) || calcTypes.Contains(nextRow.Row)) return false;

				if (columnName == "bgcolType") return row.ConstructionTypePid == nextRow.ConstructionTypePid;
				if (columnName == "bgcolItem") return row.ConstructionTypePid == nextRow.ConstructionTypePid && row.ConstructionItemPid == nextRow.ConstructionItemPid;

				return false;
			}
		}

		#endregion コンストラクタ

		#region public メソッド
		public void SetParentForm(K300030020 frmInstance)
		{
			this.parentForm = frmInstance;
		}

		/// <summary>
		/// ユーザーコントロール終了処理
		/// <para>本コントロールを使用している画面が終了時に呼び出すこと。</para>
		/// <para>リソースの解放を行う</para>
		/// </summary>
		public void Close()
        {
			// TODO
            //
            // プラス画像を加工したオブジェクトの解放
            // 画像はサイズが大きいため、解放しておく。
            //
            /*if (repLinkPlus.Image != null)
            {
                repLinkPlus.Image.Dispose();
                repLinkPlus.Image = null;
            }*/
        }

		/// <summary>
		/// 年カラムのクリア
		/// </summary>
		public void CreateYearColumns(int accountStartPeriod, int displayStartPeriod, int count, COMSKService.KumiaiTermInfo[] termInfo, int oAccountStartPeriod)
		{
			_grid.SuspendCurrentCellChanged();
			_grid.SuspendLayout();

			_gridRight.SuspendCurrentCellChanged();
			_gridRight.SuspendLayout();

			try
			{
				if (_grid.CurrentCell != null)
				{
					_grid.CurrentCell = null;
				}

				if (_gridRight.CurrentCell != null)
				{
					_gridRight.CurrentCell = null;
				}

				// 既存築年列を削除
				COMSKCommon.RemoveYearlyCol(_grid);

				var hdLayout = BuildBandedHeaderNonYearly();

				// re-create header layout
				COMSKCommon.CreateYearColumns(_grid, accountStartPeriod, displayStartPeriod, count, termInfo, true, ref hdLayout);
				_grid.SetHeaderLayout(hdLayout);
				_gridRight.SetHeaderLayout(hdLayout);

				// create columns (not use hdLayout)
				COMSKCommon.CreateYearColumns(_grid, accountStartPeriod, displayStartPeriod, count, termInfo, false, ref hdLayout);

				startPeriodIndex = displayStartPeriod;
				this.accountStartPeriodIdx = accountStartPeriod;
				this.originalAccountStartPeriodIdx = oAccountStartPeriod;

				// 30Th year right-border
				var col30Th = COMSKCommon.Get30thColName(this.originalAccountStartPeriodIdx);
				_grid.SetRightBorderColumns(new[] { col30Th, "bgcolItem" });
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				_grid.ResumeLayout();
				_grid.ResumeCurrentCellChanged();

				_gridRight.ResumeLayout();
				_gridRight.ResumeCurrentCellChanged();

				_grid.Refresh();
				_gridRight.Refresh();
			}
		}

		/// <summary>
		/// 表示データを更新する
		/// </summary>
		public void RefreshData()
		{
			_grid.Refresh();
		}

		public void DisabledDrawDragRectange()
		{
			_grid.IsDrawDragRectange = false;
			_gridRight.IsDrawDragRectange = false;
        }
        #endregion pulic メソッド

        #region privateメソッド

        /// <summary>
        /// 渡された行データを表示用に変形する
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private List<LongRepairPlanData> ConvertLongRepairPlanDatas(List<LongRepairPlanData> list)
		{
			List<LongRepairPlanData> ret = new List<LongRepairPlanData>();

			try
			{
				//  グループ ID
				int groupID = 1;

				foreach (LongRepairPlanData data in list)
				{
					//  Row == RepairPlan でなければ無視
					if (data.Row != LongRepairPlanData.RowType.RepairPlan)
					{
						continue;
					}

					//  既存 (ret) に工事分類・工事項目が一致するレコードがあるか調べる
					LongRepairPlanData topParentData = null;
					foreach (LongRepairPlanData itrData in ret)
					{
						//NOTE: itrData.Row == LongRepairPlanData.RowType.Group のレコードには、
						//  工事種別に工事項目が入っている点に注意
						if ((itrData.Row == LongRepairPlanData.RowType.GroupItem) &&
							(itrData.ConstructionTypePid == data.ConstructionTypePid) &&
							(itrData.ConstructionItemPid == data.ConstructionItemPid))
						{
							topParentData = itrData;
							break;
						}
					}

					//  存在しなければ
					if (topParentData == null)
					{
						//  新規作成
						LongRepairPlanData newData = new LongRepairPlanData()
						{
							Row = LongRepairPlanData.RowType.GroupItem,
							ConstructionTypePid = data.ConstructionTypePid,
							ConstructionTypeName = data.ConstructionTypeName,
							ConstructionItemPid = data.ConstructionItemPid,
							ConstructionItemName = data.ConstructionItemName,
							ConstructionCategoryPid = data.ConstructionItemPid,
							//  表示の都合上、工事種別に工事項目を入れる
							ConstructionCategoryName = data.ConstructionItemName,
							GroupID = groupID++,
						};
						ret.Add(newData);

						//  親として次へ
						topParentData = newData;
					}

					//  既存 (ret) に工事分類・工事項目・工事種別が一致するレコードがあるか調べる
					LongRepairPlanData parentData = null;
					foreach (LongRepairPlanData itrData in ret)
					{
						if ((itrData.Row == LongRepairPlanData.RowType.GroupCategory) &&
							(itrData.ConstructionTypePid == data.ConstructionTypePid) &&
							(itrData.ConstructionItemPid == data.ConstructionItemPid) &&
							(itrData.ConstructionCategoryPid == data.ConstructionCategoryPid))
						{
							parentData = itrData;
							break;
						}
					}

					//  存在しなければ
					if (parentData == null)
					{
						//  新規作成
						LongRepairPlanData newData = new LongRepairPlanData()
						{
							Row = LongRepairPlanData.RowType.GroupCategory,
							ConstructionTypePid = data.ConstructionTypePid,
							ConstructionTypeName = data.ConstructionTypeName,
							ConstructionItemPid = data.ConstructionItemPid,
							ConstructionItemName = data.ConstructionItemName,
							ConstructionCategoryPid = data.ConstructionCategoryPid,
							ConstructionCategoryName = data.ConstructionCategoryName,
						};
						ret.Add(newData);

						//  親として次へ
						parentData = newData;
					}

					//  グループ ID を設定
					parentData.GroupID = topParentData.GroupID;

					//  所属に追加
					topParentData.BelongsList.Add(data);
					parentData.BelongsList.Add(data);

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			//  工事分類順にソート
			ret = (from item in ret
				   orderby item.ConstructionTypePid ascending
				   select item).ToList();

			//  集計行を作成
			// 小計(A) 物価上昇率(C) 消費税(B) 想定修繕工事費年度合計(D) 想定修繕工事費累計(E)
			ret.Add(LongRepairPlanData.CreateCalcAData());
			ret.Add(LongRepairPlanData.CreateCalcCData());
			ret.Add(LongRepairPlanData.CreateCalcBData());
			ret.Add(LongRepairPlanData.CreateCalcDData());
			ret.Add(LongRepairPlanData.CreateCalcEData());

			//  OK
			return ret;
		}

		#endregion

		#region イベントハンドラ

		/// <summary>
		/// Row Cell のスタイル変更をするタイミングで発生するイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Grid_CellStyleNeeded(object sender, coms.COMMON.ui.ReserveCellStyleNeededEventArgs e)
		{
			const string COL_Type = "bgcolType";
			try
			{
				// Use LongRepairGridView sender (optional, but clearer)
				var grid = sender as coms.COMSK.ui.common.LongRepairGridView<LongRepairPlanData>;
				if (grid == null) return;

				// obj comes from DataBoundItem (the same as DevExpress GetRow(e.RowHandle))
				LongRepairPlanData obj = e.RowData as LongRepairPlanData;
				if (obj == null) return;

				// column name
				string colName = null;
				try
				{
					if (e.ColumnIndex >= 0 && e.ColumnIndex < grid.Columns.Count)
						colName = grid.Columns[e.ColumnIndex].Name;
				}
				catch { }

				// データタイプ別に背景色調整
				switch (obj.Row)
				{
					case LongRepairPlanData.RowType.GroupItem:
						// 区分列は何もしない
						if (colName != COL_Type)
						{
							e.BackColor = Color.LightGray;
						}
						break;

					case LongRepairPlanData.RowType.CalcA:
					case LongRepairPlanData.RowType.CalcB:
					case LongRepairPlanData.RowType.CalcC:
						e.BackColor = Color.LightSeaGreen;
						break;

					case LongRepairPlanData.RowType.CalcD:
					case LongRepairPlanData.RowType.CalcE:
						e.BackColor = Color.LightGreen;
						break;

					default:
						break;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
        

		/// <summary>
        /// 表示テキスト変更したいときに使用するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Grid_CellDisplayTextNeeded(object sender, coms.COMMON.ui.ReserveCellDisplayTextNeededEventArgs e)
		{
			const string COL_SubTotal = "bgcolSubTotal";
			try
			{
				var grid = sender as DataGridView;
				if (grid == null) return;

				// Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = e.RowData as LongRepairPlanData;
				if (obj == null) return;

				// column + name
				DataGridViewColumn col = null;
				string colName = null;
				try
				{
					if (e.ColumnIndex >= 0 && e.ColumnIndex < grid.Columns.Count)
					{
						col = grid.Columns[e.ColumnIndex];
						colName = col.Name;
					}
				}
				catch { }

				// タグ (DevExpress e.Column.Tag -> WinForms DataGridViewColumn.Tag)
				string tagStr = string.Empty;
				try
				{
					if (col != null && col.Tag is string)
						tagStr = (string)col.Tag;
				}
				catch { }

				// 年度列なら (draggable/year columns)
				if (tagStr == COMSKCommon.TAG_DRAGGABLE_CELL)
				{
					// 集計行でなければ
					if (obj.Row == LongRepairPlanData.RowType.RepairPlan ||
						obj.Row == LongRepairPlanData.RowType.GroupItem ||
						obj.Row == LongRepairPlanData.RowType.GroupCategory)
					{
						// 年次データ
						// NOTE: original comment: sometimes e.Value becomes null
						if (e.Value is long vLong)
						{
							e.DisplayText = COMSKCommon.ConvertToLongRepairPlanText(this.MntPlanConst.ViewUnit, vLong);
							return;
						}

						// optional: handle int too
						if (e.Value is int vInt)
						{
							e.DisplayText = COMSKCommon.ConvertToLongRepairPlanText(this.MntPlanConst.ViewUnit, (long)vInt);
							return;
						}
					}
				}

				// 想定累計列なら (CalcE)
				if (obj.Row == LongRepairPlanData.RowType.CalcE)
				{
					// 小計は空欄
					if (colName == COL_SubTotal)
					{
						e.DisplayText = string.Empty;
						return;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

        #endregion イベントハンドラ

		public void GetBandedWidthDict(int currentRate, bool reCreatedYearly)
		{
			this.dictBandWidth = XtraGridUtil.GetBandedWidthDict(this._grid, currentRate, reCreatedYearly);
		}

		public void SetZoom(int rate, bool reCreatedYearly = false)
		{
			if (this.dictBandWidth.Keys.Count == 0)
			{
				this.GetBandedWidthDict(this.zoomFactor, reCreatedYearly);
			}
			this.zoomFactor = rate;
			List<string> keepSizeBand = new List<string>(new[]
			{
				""
				//"bgcolType", "bgcolItem"
			});
			XtraGridUtil.SetZoom(this._grid, dictBandWidth, rate, keepSizeBand);
            XtraGridUtil.SetZoom(this._gridRight, dictBandWidth, rate, keepSizeBand);
        }

		public void RestoreAllBanded()
		{
			_grid.RestoreColumns(_grid.Columns.Cast<DataGridViewColumn>().Select(c => c.Name));
		}

		public void ResetBandedDict()
		{
			this.dictBandWidth = new Dictionary<string, int>();
		}

		private void Grid_MouseWheel(object sender, MouseEventArgs e)
		{
			// Check if the Ctrl key is pressed
			if (Control.ModifierKeys == Keys.Control)
			{
				bool doZoom = false;
				int newZoom = XtraGridUtil.CalcZoomFactor(e, ref doZoom, this.zoomFactor);
				if (doZoom)
				{
					this.SetZoom(newZoom);
					this.parentForm.ChangeZoomRateFromChildTab(newZoom);
				}

				((HandledMouseEventArgs)e).Handled = true;
			}
			else if (Control.ModifierKeys == Keys.Shift)
			{
                XtraGridUtil.DoScroll(this._grid, e, this.hScroll);
                ((HandledMouseEventArgs)e).Handled = true;
			}
			else
			{
				// -----------------------
				//左Gridスクロールしたら、右グリッドもスクロールさせる
				HandledMouseEventArgs hme = e as HandledMouseEventArgs;
				if (hme != null) hme.Handled = true;

				if (_syncingScroll) return;
				_syncingScroll = true;
				int scrollLines = SystemInformation.MouseWheelScrollLines;
				int direction = e.Delta > 0 ? -scrollLines : scrollLines;

				int currentIndex = _gridRight.FirstDisplayedScrollingRowIndex;
				int newIndex = currentIndex + direction;

				newIndex = Math.Max(0, Math.Min(newIndex, _gridRight.RowCount - 1));

				if (newIndex >= 0 && newIndex < _gridRight.RowCount)
				{
					_grid.FirstDisplayedScrollingRowIndex = newIndex;
					_gridRight.FirstDisplayedScrollingRowIndex = newIndex;

					_grid.Update();
					_gridRight.Update();
				}

				_syncingScroll = false;
				// -----------------------
			}
        }

        private void Grid_CellReadOnlyNeeded(object sender, coms.COMMON.ui.ReserveCellReadOnlyNeededEventArgs e)
		{
			e.ReadOnly = true;
		}

        /// <summary>
        /// 開閉グループのクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_OnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as LongRepairGridView<T>;
            if (grid == null) return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
			//MergeCell
			MergeRegion region;
			var isGroup = _grid.TryGetMergeRegion(e.RowIndex, e.ColumnIndex, out region);

			if (isGroup && region != null && region.RowSpan > 1 && e.RowIndex == region.RowStart && !region.AllowDrawMergeGroup)
			{
				ToggleRowsVisibility(grid, region);

				// re paint grid
				Invalidate();
			}
		}

        /// <summary>
        ///  開閉グループのクリック
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="region"></param>
        private void ToggleRowsVisibility(LongRepairGridView<T> grid, MergeRegion region)
        {
            // 開閉ステータス
            bool visible = !region.IsCollapsed;
			region.IsCollapsed = visible;

            // RowStart + 1 -> RowStart + RowSpan - 1
            for (int r = region.RowStart + 1; r < region.RowStart + region.RowSpan; r++)
            {
                if (r < grid.RowCount)
                {
                    visible = !region.IsCollapsed;
					grid.Rows[r].Visible = visible;
                    _gridRight.Rows[r].Visible = visible;

                }
            }
        }
    }
}
