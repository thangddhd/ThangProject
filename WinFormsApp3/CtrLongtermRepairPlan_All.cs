using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using coms.COMMON.ui;
using coms.COMSK.common;
using coms.COMSKService;

/// <history>
/// #001 ducthang 20120801
///     ◆長期修繕計画の周期起算年を変更できるように修正します
/// </history>
namespace coms.COMSK.ui.common
{
	/// <summary>
	/// セルの値変更イベントハンドラ
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The <see cref="coms.COMSK.common.CustomEventArgs&lt;coms.COMSK.common.LongRepairPlanData&gt;"/> instance containing the event data.</param>
	public delegate void LongRepairPlanDataEventHandler(object sender, CustomEventArgs<LongRepairPlanData> e);

	/// <summary>
	/// 長期修繕計画表用（全体）のグリッドコントロール
	/// </summary>
	public partial class CtrLongtermRepairPlan_All : UserControl
	{
		#region イベント

		/// <summary>
		/// セルの値が変更されたときに呼び出される
		/// </summary>
		public event LongRepairPlanDataEventHandler CellValueChanged;

		/// <summary>
		/// 実績年度が変更されたときに呼び出される
		/// </summary>
		public event LongRepairPlanDataEventHandler EffectedYearChanged;

        /// <summary>
        /// 周期起算年が変更されたときに呼び出される
        /// </summary>
        public event LongRepairPlanDataEventHandler DatumYearChanged;

		/// <summary>
		/// 履歴参照を開く際に呼び出される
		/// </summary>
		public event LongRepairPlanDataEventHandler OpenHistory;

		/// <summary>
		/// 詳細を開く際に呼び出される
		/// </summary>
		public event LongRepairPlanDataEventHandler OpenDetail;

		/// <summary>
		/// 登録を開く際に呼び出される
		/// </summary>
		public event LongRepairPlanDataEventHandler OpenRegistering;

		/// <summary>
		/// ドラッグ完了時に呼び出される
		/// </summary>
		public event EventHandler<RowCellsDragEventArgs> RowCellsDragCompleted;

		#endregion

		#region メンバ変数

		/// <summary>
		/// 列マージ制御クラス
		/// </summary>
		private MyCellMergeHelper m_Helper;

		/// <summary>
		/// ドラッグドロップヘルパ
		/// </summary>
		DnDHelper dndHelper;

		/// <summary>
		/// 表示開始する会計期のインデックス
		/// </summary>
		private int startPeriodIndex = 0;

		/// <summary>
		/// ドラッグセル編集許可フラグ
		/// </summary>
		private bool allowEdit = false;
        public bool result; //集計FLG

		#endregion メンバ変数

		#region プロパティ

		/// <summary>
		/// 実際のデータが始まるカラムのインデックス
		/// </summary>
		public const int DATA_START_COLUMN = 17;

		LongRepairPlanData selectedRow = null;
		Dictionary<string, List<List<int>>> lstMergedCols = new Dictionary<string, List<List<int>>>();
		Dictionary<string, int> dictBandWidth = new Dictionary<string, int>();
		private int zoomFactor = 100; // Initial zoom factor
		private K300030020 parentForm;
		private int accountStartPeriodIdx = 0;  // 表の開始年変更場合これも変更される
		private int originalAccountStartPeriodIdx = 0;  // 作成時の開始年

		private LongRepairGridView<LongRepairPlanData> _grid;
		private BindingSource _bs = new BindingSource();

		private const string COL_ConstructionType = "bgcolConstructionType";
		private const string COL_ConstructionItem = "bgcolConstructionItem";
		private const string COL_ConstructionCategory = "bgcolConstructionCategory";
		private const string COL_ConstructionPosition = "bgcolConstructionPosition";
		private const string COL_ConstructionRegion = "bgcolConstructionRegion";
		private const string COL_ConstructionSpecification = "bgcolConstructionSpecification";
		private const string COL_ConstructionDivision = "bgcolConstructionDivision";
		private const string COL_RepairConstructionContent = "bgcolRepairConstructionContent";
		private const string COL_SubTotal = "bgcolSubTotal";
		private const string COL_DatumYear = "bgcolDatumYear";
		private const string COL_Amount = "bgcolAmount";
		private const string COL_RepairPeriod = "bgcolRepairPeriod";
		private const string COL_Unit = "bgcolUnit";
		private const string COL_EffectedYear = "bgcolEffectedYear";
		/// <summary>
		/// データソース
		/// </summary>
		[Browsable(false)]
		public List<LongRepairPlanData> DataSource
		{
			get
			{
				return _bs.DataSource as List<LongRepairPlanData>;
			}
			set
			{
				//  渡されたリストをコピー
				List<LongRepairPlanData> list = new List<LongRepairPlanData>();
				if (value != null)
				{
					//  工事分類別にソート
					list = (from item in value
							orderby item.ConstructionTypePid ascending
							select item).ToList();

					//  集計行を作成
					// 小計(A) 物価上昇率(C) 消費税(B) 想定修繕工事費年度合計(D) 想定修繕工事費累計(E)
					list.Add(LongRepairPlanData.CreateCalcAData());
					list.Add(LongRepairPlanData.CreateCalcCData());
					list.Add(LongRepairPlanData.CreateCalcBData());
					list.Add(LongRepairPlanData.CreateCalcDData());
					list.Add(LongRepairPlanData.CreateCalcEData());

				}

				//  セット
				_bs = new BindingSource();
				_bs.DataSource = list;
				_grid.DataSource = _bs;
				//BindData();

				// do merge
				_grid.RebuildMerges();

				this.GetMergedCells();
			}
		}

		/// <summary>
		/// 長計プロパティ
		/// </summary>
		[Browsable(false)]
		public MaintenancePlanConst MntPlanConst { get; set; }

		#endregion プロパティ

		#region 初期化・コンストラクタ
		public void SetParentForm(K300030020 frmInstance)
        {
			this.parentForm = frmInstance;
        }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CtrLongtermRepairPlan_All()
		{
			m_Helper = null;

			InitializeComponent();
			this.initGridView();

			//  DnD ヘルパを初期化
			//dndHelper = new DnDHelper(gridvLongtermRepairPlan);
			//dndHelper.DragCompleted += new DnDHelper.DragCompletedHandler(dndHelper_DragCompleted);
		}

		private void initGridView()
        {
			_grid = new LongRepairGridView<LongRepairPlanData>();
            _grid.Dock = DockStyle.Fill;

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

			// create columns
			AddColumnsNonYearly(_grid);

			// Configure merge behavior
			_grid.SetVerticalMergeColumns(new[] {
				"bgcolConstructionType",
				"bgcolConstructionItem",
				"bgcolConstructionCategory",
				"bgcolConstructionPosition",
				"bgcolConstructionRegion",
				"bgcolConstructionSpecification"
			});
			_grid.VerticalMergeProvider = new LongRPAllMergeProvider();

			// Left columns list (for calc-row horizontal merge) - not used for testObj unless you set IsCalcRow
			_grid.SetLeftColumnNames(new[] {
				"bgcolConstructionType",
				"bgcolConstructionItem",
				"bgcolConstructionCategory",
				"bgcolConstructionPosition",
				"bgcolConstructionRegion",
				"bgcolConstructionSpecification",
				"bgcolConstructionDivision",
				"bgcolRepairConstructionContent"
			});
			// No calc rows in testObj by default, so keep null (or set false)
			var calcTypes = COMSKCommon.calcTypes();
			_grid.IsCalcRow = m => m != null && calcTypes.Contains(m.Row);

            _grid.CanDragCell = (g, rowIndex, col, model) =>
            {
                // old behavior: only RepairPlan rows + column.Tag must match
                if (model == null) return false;
                //if (model.Row != "BBB") return false;
                return (col.Tag as string) == COMSKCommon.TAG_DRAGGABLE_CELL;
            };

            // Drag completed event
            _grid.RowCellsDragCompleted += Grid_RowCellsDragCompleted;

			// cell value changed
			_grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;
			_grid.CellValueChanged += Grid_CellValueChanged;

			// cell mouse click
			_grid.CellMouseClick += Grid_CellMouseClick;

			// mouse wheel
			_grid.MouseWheel += Grid_MouseWheel;

			// readonly
			_grid.CellReadOnlyNeeded += Grid_CellReadOnlyNeeded;

			// format text
			_grid.CellDisplayTextNeeded += Grid_CellDisplayTextNeeded;

			// cell style
			_grid.CellStyleNeeded += Grid_CellStyleNeeded;

			// button style
			_grid.ButtonCellStyleNeeded += Grid_ButtonCellStyleNeeded;

			// editing
			_grid.CellBeginEditRule += Grid_CellBeginEditRule;
			_grid.EditingControlRule += Grid_EditingControlRule;
			_grid.MouseDoubleClick += grid_MouseDoubleClick;

			// freeze
			FreezeLeftColumns();

            // Example header layout (2 rows)
            _grid.SetHeaderLayout(BuildBandedHeaderNonYearly());

            // Add to form
            this.Controls.Add(_grid);
        }

		private void AddColumnsNonYearly(DataGridView grid)
		{
			grid.Columns.Clear();
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolConstructionType", "ConstructionTypeName", "工事分類", 60));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolConstructionItem", "ConstructionItemName", "工事項目", 90));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolConstructionCategory", "ConstructionCategoryName", "工事種別", 90));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolConstructionPosition", "ConstructionPositionName", "位置", 90));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolConstructionRegion", "ConstructionRegionName", "部位", 90));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolConstructionSpecification", "ConstructionSpecificationName", "仕様", 90));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolConstructionDivision", "ConstructionDivisionName", "工事区分", 90));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolRepairConstructionContent", "RepairConstructionContentName", "(修繕工事内容)", 100));
			var col = COMSKCommon.CreateColTextNormal("bgcolRepairPeriod", "Cycle", "周期", 40);
			col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			grid.Columns.Add(col);
			grid.Columns.Add(COMSKCommon.CreateColButtonNormal("bgcolRepairPlan", null, "修繕計画", "開く", 40));
			grid.Columns.Add(COMSKCommon.CreateColButtonNormal("bgcolHistory", null, "変更履歴", "開く", 36));
			grid.Columns.Add(COMSKCommon.CreateColButtonNormal("bgcolRegister", null, "登録", "開く", 40));
			col = COMSKCommon.CreateColTextNormal("bgcolAmount", "Amount", "数量", 41);
			col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			col.DefaultCellStyle.Format = "N0";
			grid.Columns.Add(col);
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolUnit", "UnitName", "単位", 40));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolDatumYear", "DatumYear", "周期起算年", 49));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolEffectedYear", "ResultYear", "修繕実施年度", 75));
			grid.Columns.Add(COMSKCommon.CreateColTextNormal("bgcolSubTotal", "SubTotal", "小計", 75));
		}

		private void FreezeLeftColumns()
		{
			_grid.Columns["bgcolConstructionType"].Frozen = true;
			_grid.Columns["bgcolConstructionItem"].Frozen = true;
			_grid.Columns["bgcolConstructionCategory"].Frozen = true;
			_grid.Columns["bgcolConstructionPosition"].Frozen = true;

			_grid.Columns["bgcolConstructionRegion"].Frozen = true;
			_grid.Columns["bgcolConstructionSpecification"].Frozen = true;
			_grid.Columns["bgcolConstructionDivision"].Frozen = true;
			_grid.Columns["bgcolRepairConstructionContent"].Frozen = true;

			_grid.Columns["bgcolRepairPeriod"].Frozen = true;
			_grid.Columns["bgcolRepairPlan"].Frozen = true;
			_grid.Columns["bgcolHistory"].Frozen = true;
			_grid.Columns["bgcolRegister"].Frozen = true;

			_grid.Columns["bgcolAmount"].Frozen = true;
			_grid.Columns["bgcolUnit"].Frozen = true;
			_grid.Columns["bgcolDatumYear"].Frozen = true;
			_grid.Columns["bgcolEffectedYear"].Frozen = true;
		}

		private HeaderBandLayout BuildBandedHeaderNonYearly()
		{
			var layout = new HeaderBandLayout();
			layout.HeaderRowCount = 3;
			layout.HeaderRowHeight = 22;
			// ===== Row 0 (full height columns)
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionType", "工事分類", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionItem", "工事項目", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionCategory", "工事種別", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionPosition", "位置", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionRegion", "部位", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionSpecification", "仕様", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionDivision", "工事区分", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolRepairConstructionContent", "(修繕工事内容)", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolRepairPeriod", "周期", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolRepairPlan", "修繕\n基準", true));
			// ===== 変更理由 (2 columns)
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 1,
				new[] { "bgcolHistory", "bgcolRegister" }, "変更理由", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(1, 2, "bgcolHistory", "参照", false));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(1, 2, "bgcolRegister", "登録", false));
			// ===== normal columns
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolAmount", "数量", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolUnit", "単位", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolDatumYear", "周期\n起算年", true));
			// ===== 築年 (nested 3 level)
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 1,
				new[] { "bgcolEffectedYear" }, "築年", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(1, 1,
				new[] { "bgcolEffectedYear" }, "会計期", false));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(2, 1,
				"bgcolEffectedYear", "会計年度", false));
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

				if (columnName == "bgcolConstructionType") return row.ConstructionTypeName == nextRow.ConstructionTypeName;
				if (columnName == "bgcolConstructionItem") return row.ConstructionItemName == nextRow.ConstructionItemName;
				if (columnName == "bgcolConstructionCategory") return row.ConstructionCategoryName == nextRow.ConstructionCategoryName;
				if (columnName == "bgcolConstructionPosition") return row.ConstructionPositionName == nextRow.ConstructionPositionName;
				if (columnName == "bgcolConstructionRegion") return row.ConstructionRegionName == nextRow.ConstructionRegionName;
				if (columnName == "bgcolConstructionSpecification") return row.ConstructionSpecificationName == nextRow.ConstructionSpecificationName;

				return false;
			}
		}
		#endregion 初期化・コンストラクタ

		#region public メソッド


		/// <summary>
		/// セルでーたを変更できるかどうかプロバティ
		/// </summary>
		private bool isDisabled = false;

        public void SetDisable()
        {
			// TODO
            /*this.isDisabled = true;
            gridvLongtermRepairPlan.OptionsBehavior.Editable = false;*/
        }

		/// <summary>
		/// 年次カラムを作成する
		/// </summary>
		public void CreateYearColumns(int accountStartPeriod, int displayStartPeriod, int count, KumiaiTermInfo[] termInfo, int oAccountStartPeriod)
		{
			try
			{
				var hdLayout = BuildBandedHeaderNonYearly();
				// re-create header layout
				COMSKCommon.CreateYearColumns(_grid, accountStartPeriod, displayStartPeriod, count, termInfo, true, ref hdLayout);
				_grid.SetHeaderLayout(hdLayout);

				//  create columns (not use hdLayout)
				COMSKCommon.CreateYearColumns(_grid, accountStartPeriod, displayStartPeriod, count, termInfo, false, ref hdLayout);
				startPeriodIndex = displayStartPeriod;
				this.accountStartPeriodIdx = accountStartPeriod;
				this.originalAccountStartPeriodIdx = oAccountStartPeriod;

				//  データソースリフレッシュ
				_grid.Refresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// 表示データを更新する
		/// </summary>
		public void RefreshData()
		{
			_grid.Refresh();
		}

		public void ClearSelectedRow()
		{
			this.selectedRow = null;
		}

		public void GetBandedWidthDict(int currentRate, bool reCreatedYearly)
        {
			this.dictBandWidth = XtraGridUtil.GetBandedWidthDict(this._grid, currentRate, reCreatedYearly);
		}

		private void GetMergedCells()
		{
			// TODO
			/*List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColMerge = new List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>(
			new[] {
				this.bgcolConstructionType
				, this.bgcolConstructionItem
				, this.bgcolConstructionCategory
				, this.bgcolConstructionPosition
				, this.bgcolConstructionRegion
				, this.bgcolConstructionSpecification
			});

			this.lstMergedCols = XtraGridUtil.GetRepairPlanMergedCells(this.gridvLongtermRepairPlan, lstColMerge);
			// colbutton
			this.lstColButton = new List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>(new[] { bgcolHistory, bgcolRepairPlan, bgcolRegister });*/
		}
		#region ユーザーコントロール終了処理

		/// <summary>
		/// ユーザーコントロール終了処理
		/// <para>本コントロールを使用している画面が終了時に呼び出すこと。</para>
		/// <para>リソースの解放を行う</para>
		/// </summary>
		public void Close()
		{
		}

		#endregion ユーザーコントロール終了処理

		#endregion public メソッド

		#region privateメソッド

		#region データをグリッドコントロールへバインド

		/// <summary>
		/// パラメータで指定されたデータをグリッドコントロールへバインド
		/// </summary>
		private void BindData()
		{
			// not use need check to del TODO
			/*try
			{
				if (gridvLongtermRepairPlan != null && DataSource != null)
				{
					if (m_Helper == null)
					{
						m_Helper = new MyCellMergeHelper(gridvLongtermRepairPlan);
					}

					m_Helper.Clear();
					for (int i = 0; i < DataSource.Count; i++)
					{
						LongRepairPlanData data = DataSource[i];

						switch (data.Row)
						{
							case LongRepairPlanData.RowType.CalcA:
							case LongRepairPlanData.RowType.CalcB:
							case LongRepairPlanData.RowType.CalcC:
							case LongRepairPlanData.RowType.CalcD:
							case LongRepairPlanData.RowType.CalcE:
								m_Helper.AddMergedCell(i, 0, 7, data.ConstructionTypeName);
								break;
							default:
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}*/
		}

		#endregion

		#endregion privateメソッドecllva

		#region イベント
		private void grid_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			try
			{
				var grid = sender as coms.COMSK.ui.common.LongRepairGridView<LongRepairPlanData>;
				if (grid == null) return;

				var hit = grid.HitTest(e.X, e.Y);
				if (hit.Type != DataGridViewHitTestType.Cell || hit.RowIndex < 0 || hit.ColumnIndex < 0)
					return;

				var col = grid.Columns[hit.ColumnIndex];
				if ((col.Tag as string) != COMSKCommon.TAG_DRAGGABLE_CELL)
					return;

				allowEdit = true;
				grid.CurrentCell = grid[hit.ColumnIndex, hit.RowIndex];

				// Do NOT call BeginEdit here; LongRepairGridView.OnCellDoubleClick will do it.
			}
			catch { }
		}

		private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			// Forces CellValueChanged to fire immediately for checkbox/combobox etc.
			if (_grid.IsCurrentCellDirty)
				_grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}

		private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

				var data = _grid.Rows[e.RowIndex].DataBoundItem as LongRepairPlanData;
				if (data == null) return;

				var col = _grid.Columns[e.ColumnIndex];
				if (col == null) return;

				// Equivalent of DevExpress: Columns.IndexOf(e.Column)
				// Use DisplayIndex if you want visible order; use Index if you want collection order.
				int columnIndex = col.DisplayIndex;
				data.PeriodIndex = columnIndex - DATA_START_COLUMN;

				// EffectedYear column
				if (col.Name == "bgcolEffectedYear")
				{
					EffectedYearChanged?.Invoke(this, new CustomEventArgs<LongRepairPlanData>(data));
				}
				// DatumYear column
				else if (col.Name == "bgcolDatumYear")
				{
					DatumYearChanged?.Invoke(this, new CustomEventArgs<LongRepairPlanData>(data));
				}
				// draggable cell (year columns etc.)
				else if ((col.Tag as string) == COMSKCommon.TAG_DRAGGABLE_CELL)
				{
					object v = _grid[e.ColumnIndex, e.RowIndex].Value;
					if (v != null)
						data.NewValue = Convert.ToInt32(v);

					CellValueChanged?.Invoke(this, new CustomEventArgs<LongRepairPlanData>(data));
				}
			}
			catch
			{
				// keep behavior consistent with your old code (swallow exceptions)
			}
		}

		private void Grid_ButtonCellStyleNeeded(object sender, coms.COMMON.ui.ReserveButtonCellStyleNeededEventArgs e)
        {
			var obj = e.RowData as LongRepairPlanData;
			if (obj == null) return;

			if (e.ButtonColumn.Name == "bgcolHistory")
			{
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					e.Visible = true;
					e.Text = "開く";

					// red style if has history
					if (obj.UpdateFlg == COMSKCommon.HAS_HISTORY_FLG_ON || (obj.WorkUpdateReasonList?.Count ?? 0) > 0)
					{
						e.BackColor = Color.Red;
						e.ForeColor = Color.White;
					}
					else
					{
						e.BackColor = Color.White;
						e.ForeColor = Color.Black;
					}
				}
				else
				{
					e.Visible = false; // hide for non RepairPlan rows
				}
			}

			// REPAIR PLAN button column
			if (e.ButtonColumn.Name == "bgcolRepairPlan")
			{
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					if (this.result)
					{
						e.Visible = true;
						e.Text = "開く";
						e.DisabledStyle = true; // visual only
						e.BackColor = Color.LightGray;
						e.ForeColor = Color.DarkGray;
					}
					else
					{
						if (obj.ConstructionSpecificationName == COMSKCommon.REPAIR_HISTORY_GROUP_SPEC_TEXT)
						{
							e.Visible = false; // repEmpty
						}
						else
						{
							e.Visible = true;
							e.Text = "開く";
						}
					}
				}
				else
				{
					e.Visible = false;
				}
			}

			// REGISTER button column
			if (e.ButtonColumn.Name == "bgcolRegister")
			{
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					e.Visible = true;
					e.Text = "開く";

					if (this.result)
					{
						e.DisabledStyle = true; // visual only
						e.BackColor = Color.LightGray;
						e.ForeColor = Color.DarkGray;
					}
				}
				else
				{
					e.Visible = false;
				}
			}
		}

		/// <summary>
		/// Row Cell のスタイル変更をするタイミングで発生するイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Grid_CellStyleNeeded(object sender, coms.COMMON.ui.ReserveCellStyleNeededEventArgs e)
		{
			try
			{
				var grid = sender as DataGridView;
				if (grid == null) return;

				var obj = e.RowData as LongRepairPlanData;
				if (obj == null) return;

				// Resolve column
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

				// -----------------------------
				// 1) RowStyle (row base color)
				// -----------------------------
				// Only applies to RepairPlan rows in old code
				// Priority: UpdateContentList > ModifiedAtLongRepairPlanFlg > ImportFlg
				Color? baseRowBackColor = null;

				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					if (obj.UpdateContentList != null && obj.UpdateContentList.Count > 0)
					{
						baseRowBackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_NO_REASON;
					}
					else if (obj.ModifiedAtLongRepairPlanFlg == COMSKCommon.HAS_HISTORY_FLG_ON)
					{
						baseRowBackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_MODIFIED_AT_LONGTERM_REPAIR_PLAN;
					}
					else if (obj.ImportFlg != COMSKCommon.IMPORT_FLG_NONE)
					{
						if (obj.ImportFlg == COMSKCommon.IMPORT_FLG_REPAIR_PLAN)
						{
							baseRowBackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_PLAN;
						}
						else if (obj.ImportFlg == COMSKCommon.IMPORT_FLG_REPAIR_HISTORY)
						{
							baseRowBackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_HISTORY;
						}
					}
				}

				// apply base row color first
				if (baseRowBackColor.HasValue)
					e.BackColor = baseRowBackColor.Value;

				// ---------------------------------------
				// 2) RowCellStyle (cell/column overrides)
				// ---------------------------------------
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					// * 修繕項目 * column-based base colors
					if (colName == COL_ConstructionType)
					{
						e.BackColor = Color.Silver;
					}
					else if (colName == COL_ConstructionItem)
					{
						e.BackColor = Color.LightGray;
					}
					else if (colName == COL_ConstructionCategory ||
							 colName == COL_ConstructionPosition ||
							 colName == COL_ConstructionRegion ||
							 colName == COL_ConstructionSpecification ||
							 colName == COL_ConstructionDivision ||
							 colName == COL_RepairConstructionContent)
					{
						e.BackColor = Color.White;
					}

					// ドラッグセルなら (tag check)
					string tagStr = null;
					try { tagStr = col != null ? col.Tag as string : null; } catch { }

					if (tagStr == COMSKCommon.TAG_DRAGGABLE_CELL)
					{
						// In DevExpress:
						// columnIndex = grid.Columns.IndexOf(e.Column) - DATA_START_COLUMN;
						//
						// In WinForms: use DisplayIndex or Index depending on how you define DATA_START_COLUMN.
						// If DATA_START_COLUMN was based on visible order, use DisplayIndex.
						int columnIndex = -1;
						try
						{
							// If you already have DATA_START_COLUMN defined as "first year column display index"
							// then DisplayIndex is closer to DevExpress "visible order".
							columnIndex = (col != null ? col.DisplayIndex : -1) - DATA_START_COLUMN;
						}
						catch { }

						if (columnIndex >= 0)
						{
							int valueIndex = startPeriodIndex + columnIndex;
							if (valueIndex < LongRepairPlanData.VALID_VALUE_COUNT)
							{
								if (obj.GetDragged(valueIndex) == COMSKCommon.DRAGGED_FLG_ON)
								{
									e.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_DRAGGED;
								}
							}
						}
					}
				}
				else if (obj.Row == LongRepairPlanData.RowType.CalcA ||
						 obj.Row == LongRepairPlanData.RowType.CalcB ||
						 obj.Row == LongRepairPlanData.RowType.CalcC)
				{
					e.BackColor = Color.LightSeaGreen;
				}
				else if (obj.Row == LongRepairPlanData.RowType.CalcD ||
						 obj.Row == LongRepairPlanData.RowType.CalcE)
				{
					e.BackColor = Color.LightGreen;
				}

				// -----------------------------------------
				// 3) Selection range highlight (Lavender)
				// -----------------------------------------
				// DevExpress had dndHelper.IsInSelection(e) => Lavender
				// In LongRepairGridView, selection highlight is already applied in OnCellFormatting,
				// which runs BEFORE this event.
				//
				// If you still want to force Lavender here too, you can do:
				// if (SomeSelectionHelper != null && SomeSelectionHelper.IsInSelection(e.RowIndex, e.ColumnIndex)) e.BackColor = Color.Lavender;
				//
				// But if you rely on the built-in highlight in LongRepairGridView, DO NOTHING here.
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// Row Cell をクリックした際に発生するイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				// DataGridView: header click is RowIndex == -1 / ColumnIndex == -1
				if (e.RowIndex < 0 || e.ColumnIndex < 0)
				{
					this.selectedRow = null;
					return;
				}

				this.selectedRow = _grid.Rows[e.RowIndex].DataBoundItem as LongRepairPlanData;
			}
			catch (Exception ex)
			{
				this.selectedRow = null;
			}

			try
			{
				// 左ボタン以外は何もしない。
				if (e.Button == MouseButtons.Left)
				{
					// Rowデータが予期していないデータ型の場合は何もしない。
					LongRepairPlanData obj = _grid.Rows[e.RowIndex].DataBoundItem as LongRepairPlanData;
					if (obj == null) return;

					DataGridViewColumn col = _grid.Columns[e.ColumnIndex];
					if (col == null) return;

					//
					// クリックされた列・行により処理を切り分け
					//
					if (col.Name == "bgcolHistory")
					{
						//  変更履歴
						if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
						{
							if (OpenHistory != null)
							{
								OpenHistory(this, new CustomEventArgs<LongRepairPlanData>(obj));
							}
						}
					}

					if (!this.isDisabled)
					{
						if (col.Name == "bgcolRepairPlan")
						{
							if (!this.result)
							{
								//  詳細
								if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
								{
									//  修繕履歴実績集約行でなければ
									if (obj.ConstructionSpecificationName != COMSKCommon.REPAIR_HISTORY_GROUP_SPEC_TEXT)
									{
										if (OpenDetail != null)
										{
											OpenDetail(this, new CustomEventArgs<LongRepairPlanData>(obj));
										}
									}
								}
							}
						}
						else if (col.Name == "bgcolRegister")
						{
							//  登録
							if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
							{
								if (OpenRegistering != null)
								{
									OpenRegistering(this, new CustomEventArgs<LongRepairPlanData>(obj));
								}
							}
						}
					}
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
			try
			{
				var grid = sender as coms.COMSK.ui.common.LongRepairGridView<LongRepairPlanData>;
				if (grid == null) return;

				// Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = e.RowData as LongRepairPlanData;
				if (obj == null) return;

				// current column name
				string colName = null;
				try
				{
					colName = (e.ColumnIndex >= 0 && e.ColumnIndex < grid.Columns.Count)
						? grid.Columns[e.ColumnIndex].Name
						: null;
				}
				catch { }

				// 行タイプで分岐
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					// カラムで分岐
					if (colName == COL_DatumYear)
					{
						// 基準年
						e.DisplayText = (obj.DatumYear == 0) ? string.Empty : obj.DatumYear.ToString();
						return;
					}

					// else if (e.Column.Equals(bgcolSubTotal) == false && e.Column.Equals(bgcolAmount) == false && e.Column != bgcolRepairPeriod)
					if (colName != COL_SubTotal &&
						colName != COL_Amount &&
						colName != COL_RepairPeriod)
					{
						// 対象行か確認
						if (e.Value is long vLong)
						{
							e.DisplayText = COMSKCommon.ConvertToLongRepairPlanText(this.MntPlanConst.ViewUnit, vLong);
							return;
						}
						// If sometimes the value comes as int, you can optionally support it:
						if (e.Value is int vInt)
						{
							e.DisplayText = COMSKCommon.ConvertToLongRepairPlanText(this.MntPlanConst.ViewUnit, (long)vInt);
							return;
						}
					}
					else if (colName == COL_Amount)
					{
						if (obj.Amount == double.MinValue)
						{
							e.DisplayText = string.Empty;
							return;
						}
					}
					else if (colName == COL_RepairPeriod)
					{
						if (obj.Cycle == int.MinValue)
						{
							e.DisplayText = string.Empty;
							return;
						}
					}
				}
				else if (obj.Row == LongRepairPlanData.RowType.CalcA ||
						 obj.Row == LongRepairPlanData.RowType.CalcB ||
						 obj.Row == LongRepairPlanData.RowType.CalcC ||
						 obj.Row == LongRepairPlanData.RowType.CalcD ||
						 obj.Row == LongRepairPlanData.RowType.CalcE)
				{
					string dispText = null;

					// 特定のカラムは無視
					if (colName == COL_RepairPeriod ||
						colName == COL_Amount ||
						colName == COL_Unit ||
						colName == COL_EffectedYear ||
						colName == COL_DatumYear)
					{
						dispText = string.Empty;
					}
					else if (colName == COL_ConstructionType)
					{
						dispText = e.Value as string;
					}

					// 想定累計列なら (CalcE)
					if (obj.Row == LongRepairPlanData.RowType.CalcE)
					{
						// 小計は空欄
						if (colName == COL_SubTotal)
						{
							dispText = string.Empty;
						}
					}

					if (dispText == null)
					{
						if (e.Value != null)
						{
							if (e.Value is int || e.Value is long)
							{
								long v = (e.Value is long)
									? (long)e.Value
									: (long)(int)e.Value;

								dispText = COMSKCommon.ConvertToLongRepairPlanText(MntPlanConst.ViewUnit, v);
							}

							if (dispText == string.Empty)
							{
								dispText = "0";
							}
						}
						else
						{
							dispText = string.Empty;
						}
					}

					e.DisplayText = dispText;
					return;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
		{
			// TODO
			/*try
			{
				DevExpress.XtraGrid.Columns.GridColumn gc = gridvLongtermRepairPlan.FocusedColumn;

				if ((gc.Tag is string) && (gc.Tag as string == COMSKCommon.TAG_DRAGGABLE_CELL))
				{
					if (e.Value == null)
					{
						e.Value = 0;
						e.Valid = true;
					}
					else if (e.Value is string)
					{
						string s = e.Value as string;
						if (s.Trim() == string.Empty)
						{
							e.Value = 0;
							e.Valid = true;
						}
					}
				}
			}
			catch (Exception)
			{
			}*/
		}

		private void gridvLongtermRepairPlan_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e)
		{
			COMSKCommon.CustomDrawBandHeader(e, this.originalAccountStartPeriodIdx);
		}

		/// <summary>
		/// ドラッグドロップ完了
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		private void Grid_RowCellsDragCompleted(object sender, RowCellsDragEventArgs e)
		{
			// bubble up to parent
			RowCellsDragCompleted?.Invoke(this, e);
		}

		private void gridvLongtermRepairPlan_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			// TODO
			/*LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
			// マージセールがボーダーしない
			if (this.selectedRow != null && e.RowHandle == gridvLongtermRepairPlan.FocusedRowHandle)
			{
				//var isMergedCell = this.IsMergedCell(e);
				var isMergedCell = XtraGridUtil.IsMergedCell(this.gridvLongtermRepairPlan, e, lstMergedCols);
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan && !isMergedCell)
				{
					XtraGridUtil.RepairPlanDrawCell(e, obj, this.lstColButton, bgcolHistory, this.isDisabled);
				}
			}
			else
            {
				COMSKCommon.CustomDrawCostCell(e, this.originalAccountStartPeriodIdx, obj.Row);
			}*/
		}

		public void SetZoom(int rate, bool reCreatedYearly = false)
		{
			if (this.dictBandWidth.Keys.Count == 0)
			{
				this.GetBandedWidthDict(this.zoomFactor, reCreatedYearly);
			}
			this.zoomFactor = rate; // set new rate
			List<string> keepSizeBand = new List<string>(new[]
			{
				"bgcolConstructionType",
				"bgcolConstructionItem",
				"bgcolConstructionCategory",
				"bgcolConstructionPosition",
				"bgcolConstructionRegion",
				"bgcolConstructionSpecification",
				"bgcolConstructionDivision",
				"bgcolRepairConstructionContent",
				"bgcolRepairPeriod",
				"bgcolRepairPlan",
				"bgcolHistory",
				"bgcolRegister",
				"bgcolAmount",
				"bgcolUnit",
				"bgcolDatumYear",
				"bgcolEffectedYear",
			});
			XtraGridUtil.SetZoom(this._grid, this.dictBandWidth, rate, keepSizeBand);
		}

		public void RestoreAllBanded()
        {
			// TODO can not show/hide columns
			//XtraGridUtil.RestoreAllBanded(this._grid);
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
				XtraGridUtil.DoScroll(this._grid, e);
				((HandledMouseEventArgs)e).Handled = true;
			}
		}

		private void Grid_CellReadOnlyNeeded(object sender, coms.COMMON.ui.ReserveCellReadOnlyNeededEventArgs e)
		{
			var readonlyCols = new List<string>
			{
				"bgcolConstructionType",
				"bgcolConstructionItem",
				"bgcolConstructionCategory",
				"bgcolConstructionPosition",

				"bgcolConstructionRegion",
				"bgcolConstructionSpecification",
				"bgcolConstructionDivision",
				"bgcolRepairConstructionContent",

				"bgcolRepairPeriod",
				"bgcolRepairPlan",
				"bgcolHistory",
				"bgcolRegister",

				"bgcolAmount",
				"bgcolUnit",
			};
			if (e.ColumnIndex >= 0)
            {
				DataGridViewColumn col = _grid.Columns[e.ColumnIndex];
				if (readonlyCols.Contains(col.Name))
				{
					e.ReadOnly = true;
				}
                else
                {
					var calcTypes = COMSKCommon.calcTypes();
					LongRepairPlanData obj = e.RowData as LongRepairPlanData;
					if (calcTypes.Contains(obj.Row)) e.ReadOnly = true;
				}
			}
		}

		private void Grid_CellBeginEditRule(object sender, coms.COMMON.ui.ReserveCellBeginEditEventArgs e)
        {
			try
			{
				DataGridViewColumn col2 = _grid.Columns[e.ColumnIndex];
				// normalize before DataGridView creates the editing control
				if (col2 != null && (col2.Tag as string) == COMSKCommon.TAG_DRAGGABLE_CELL)
				{
					var cell = _grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

					if (cell.Value == null || cell.Value == DBNull.Value)
					{
						// choose the safe default for your model
						cell.Value = 0L;
					}
				}

				var grid = sender as coms.COMSK.ui.common.LongRepairGridView<LongRepairPlanData>;
				if (grid == null)
					return;

				// Like: selectedRow = gridvLongtermRepairPlan.GetRow(FocusedRowHandle)
				try
				{
					this.selectedRow = e.RowData as LongRepairPlanData;
				}
				catch
				{
					this.selectedRow = null;
				}

				// Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = e.RowData as LongRepairPlanData;
				if (obj == null)
				{
					e.Cancel = true;
					return;
				}

				// Default: 編集不可
				e.Cancel = true;

				// Column name
				string colName = null;
				DataGridViewColumn col = null;
				try
				{
					if (e.ColumnIndex >= 0 && e.ColumnIndex < grid.Columns.Count)
					{
						col = grid.Columns[e.ColumnIndex];
						colName = col.Name;
					}
				}
				catch { }

				// 実績年度列なら (EffectedYear / DatumYear) => allow edit
				if (colName == COL_EffectedYear || colName == COL_DatumYear)
				{
					e.Cancel = false;
				}
				// ドラッグカラムなら (RepairPlan row + allowEdit flag)
				else if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					e.Cancel = false;
				}

				// If editing is allowed, and it's a draggable cell, store PrevValue
				if (e.Cancel == false)
				{
					string tagStr = null;
					try { tagStr = col != null ? col.Tag as string : null; } catch { }

					if (tagStr == COMSKCommon.TAG_DRAGGABLE_CELL)
					{
						object value = null;
						try
						{
							// WinForms equivalent of GetRowCellValue(FocusedRowHandle, FocusedColumn)
							value = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
						}
						catch { value = null; }

						long currValue = 0;
						if (value != null)
						{
							long.TryParse(Convert.ToString(value), out currValue);
						}

						obj.PrevValue = currValue;
					}
				}
			}
			catch (Exception ex)
			{
				e.Cancel = true;
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void Grid_EditingControlRule(object sender, coms.COMMON.ui.ReserveEditingControlShowingEventArgs e)
		{
			try
			{
				var grid = sender as coms.COMSK.ui.common.LongRepairGridView<LongRepairPlanData>;
				if (grid == null) return;

				var obj = e.RowData as LongRepairPlanData;
				if (obj == null) return;

				if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

				// Column + tag check (draggable cell)
				DataGridViewColumn col = null;
				try { col = grid.Columns[e.ColumnIndex]; } catch { col = null; }

				string tagStr = null;
				try { tagStr = col != null ? col.Tag as string : null; } catch { }

				if (tagStr != COMSKCommon.TAG_DRAGGABLE_CELL)
					return;

				int focusedColumnIndex = -1;
				try
				{
					focusedColumnIndex = col != null ? col.DisplayIndex : e.ColumnIndex;
				}
				catch { focusedColumnIndex = e.ColumnIndex; }

				int index = focusedColumnIndex - DATA_START_COLUMN + startPeriodIndex;

				if (index >= 0)
				{
					if (obj.GetValue(index) == 0)
					{
						if (e.TextBox != null)
						{
							e.TextBox.Text = string.Empty;
							e.TextBox.SelectionStart = 0;
							e.TextBox.SelectionLength = 0;
						}
					}
				}
			}
			catch (Exception ex)
			{
				var kkk = 1;
				// keep same behavior: swallow
			}
		}
		#endregion
	}
}
