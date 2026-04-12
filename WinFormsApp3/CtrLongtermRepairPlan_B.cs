using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using coms.COMSK.common;
using coms.COMSKService;

/// #001 ducthang 20120801
///     ◆長期修繕計画の周期起算年を変更できるように修正します
namespace coms.COMSK.ui.common
{
	/// <summary>
	/// 長期修繕計画表用（建築）のグリッドコントロール
	/// </summary>
	public partial class CtrLongtermRepairPlan_B : UserControl
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

		#endregion メンバ変数

		#region プロパティ

		/// <summary>
		/// 実際のデータが始まるカラムのインデックス
		/// </summary>
		public const int DATA_START_COLUMN = 16;

		LongRepairPlanData selectedRow = null;
		Dictionary<string, List<List<int>>> lstMergedCols = new Dictionary<string, List<List<int>>>();
		Dictionary<string, int> dictBandWidth = new Dictionary<string, int>();
		List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColButton = new List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>();
		private int zoomFactor = 100; // Initial zoom factor
		private K300030020 parentForm;
		private int accountStartPeriodIdx = 0;  // 表の開始年変更場合これも変更される
		private int originalAccountStartPeriodIdx = 0;  // 作成時の開始年

		private LongRepairGridView<LongRepairPlanData> _grid;
		private BindingSource _bs = new BindingSource();

		/// <summary>
		/// 表示する工事分類。最初に設定しておく必要がある
		/// </summary>
		[Browsable(false)]
		public COMSKCommon.ConstructionType ConstrType { get; set; }

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
				//  データを選り抜く
				List<LongRepairPlanData> list;

				if (value == null)
				{
					list = new List<LongRepairPlanData>();
				}
				else
				{
					list = (from item in value
							where item.ConstructionTypePid == (long)this.ConstrType
							select item).ToList();
				}

				//  集計行を追加
				// 小計(A) 物価上昇率(C) 消費税(B) 想定修繕工事費年度合計(D) 想定修繕工事費累計(E)
				list.Add(LongRepairPlanData.CreateCalcAData());
				list.Add(LongRepairPlanData.CreateCalcCData());
				list.Add(LongRepairPlanData.CreateCalcBData());
				list.Add(LongRepairPlanData.CreateCalcDData());
				list.Add(LongRepairPlanData.CreateCalcEData());

				//  セット
				_bs = new BindingSource();
				_bs.DataSource = list;
				_grid.DataSource = _bs;
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
		public CtrLongtermRepairPlan_B()
		{
			//m_Helper = null;

			InitializeComponent();
			this.initGridView();

			//  工事分類 = 設備の場合のみ、ラベルを書き換える

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
				"bgcolConstructionItem",
				"bgcolConstructionCategory",
				"bgcolConstructionPosition",
				"bgcolConstructionRegion",
				"bgcolConstructionSpecification"
			});
			_grid.VerticalMergeProvider = new LongRPAllMergeProvider();

			// Left columns list (for calc-row horizontal merge) - not used for testObj unless you set IsCalcRow
			_grid.SetLeftColumnNames(new[] {
				"bgcolConstructionItem",
				"bgcolConstructionCategory",
				"bgcolConstructionPosition",
				"bgcolConstructionRegion",
				"bgcolConstructionSpecification",
				"bgcolConstructionDivision",
				"bgcolRepairConstructionContent"
			});
			// No calc rows in testObj by default, so keep null (or set false)
			var calcTypes = new List<LongRepairPlanData.RowType>() {
				LongRepairPlanData.RowType.CalcA,
				LongRepairPlanData.RowType.CalcB,
				LongRepairPlanData.RowType.CalcC,
				LongRepairPlanData.RowType.CalcD,
				LongRepairPlanData.RowType.CalcE
			};
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
			grid.Columns.Add(CreateCol("bgcolConstructionItem", "ConstructionItemName", "工事項目", 90));
			grid.Columns.Add(CreateCol("bgcolConstructionCategory", "ConstructionCategoryName", "工事種別", 90));

			string posText = "位置";
			string regText = "部位";
			if (ConstrType == COMSKCommon.ConstructionType.Equipment)
			{
				posText = "部位 (ユニット)";
				regText = "部品";
			}
			grid.Columns.Add(CreateCol("bgcolConstructionPosition", "ConstructionPositionName", posText, 90));
			grid.Columns.Add(CreateCol("bgcolConstructionRegion", "ConstructionRegionName", regText, 90));

			grid.Columns.Add(CreateCol("bgcolConstructionSpecification", "ConstructionSpecificationName", "仕様", 90));
			grid.Columns.Add(CreateCol("bgcolConstructionDivision", "ConstructionDivisionName", "工事区分", 90));
			grid.Columns.Add(CreateCol("bgcolRepairConstructionContent", "RepairConstructionContentName", "(修繕工事内容)", 100));
			grid.Columns.Add(CreateCol("bgcolRepairPeriod", "Cycle", "周期", 40));
			grid.Columns.Add(CreateCol("bgcolRepairPlan", null, "修繕計画", 40));
			grid.Columns.Add(CreateCol("bgcolHistory", null, "変更履歴", 36));
			grid.Columns.Add(CreateCol("bgcolRegister", null, "登録", 40));
			grid.Columns.Add(CreateCol("bgcolAmount", "Amount", "数量", 41));
			grid.Columns.Add(CreateCol("bgcolUnit", "UnitName", "単位", 40));
			grid.Columns.Add(CreateCol("bgcolDatumYear", "DatumYear", "周期起算年", 49));
			grid.Columns.Add(CreateCol("bgcolEffectedYear", "ResultYear", "修繕実施年度", 75));
			grid.Columns.Add(CreateCol("bgcolSubTotal", "SubTotal", "小計", 75));
		}

		private DataGridViewColumn CreateCol(string name, string field, string caption, int width)
		{
			return new DataGridViewTextBoxColumn()
			{
				Name = name,
				DataPropertyName = field,
				HeaderText = caption,
				Width = width
			};
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
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionItem", "工事項目", true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionCategory", "工事種別", true));

			string posText = "位置";
			string regText = "部位";
			if (ConstrType == COMSKCommon.ConstructionType.Equipment)
			{
				posText = "部位 (ユニット)";
				regText = "部品";
			}
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionPosition", posText, true));
			layout.Cells.Add(COMSKCommon.MakeHeaderCell(0, 3, "bgcolConstructionRegion", regText, true));

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
				if (row == null || nextRow == null) return false;

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
            this.isDisabled = true;
			// TODO
            /*gridvLongtermRepairPlan.OptionsBehavior.Editable = false;
            bgcolRepairPlan.ColumnEdit = repOpenBtnDisabled;
            bgcolRegister.ColumnEdit = repOpenBtnDisabled;
            bgcolHistory.ColumnEdit = repOpenBtnDisabled;*/
        }

		private void GetMergedCells()
        {
			// TODO
			/*List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColMerge = new List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>(
			new[] {
				this.bgcolConstructionItem
				, this.bgcolConstructionCategory
				, this.bgcolConstructionPosition
				, this.bgcolConstructionRegion
				, this.bgcolConstructionSpecification
			});
			this.lstMergedCols = XtraGridUtil.GetRepairPlanMergedCells(this.gridvLongtermRepairPlan, lstColMerge);
			// colbutton
			this.lstColButton = new List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>(new[] { bgcolHistory, bgcolRepairPlan, bgcolRegister });*/
		}

		public void ClearSelectedRow()
        {
			this.selectedRow = null;
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
			this._grid.Refresh();
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
			// not use any more but need check for sure
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
								m_Helper.AddMergedCell(i, 0, 6, data.ConstructionTypeName);
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

		#endregion privateメソッド

		#region イベント

		/// <summary>
		/// マウスボタンダウンイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_MouseDown(object sender, MouseEventArgs e)
		{
			//  左ボタンで、かつキーが押されていなければ
			if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.None)
			{
				//  マウス押下処理
				dndHelper.DispatchMouseDown(new Point(e.X, e.Y));

			}

		}

		/// <summary>
		/// マウス移動イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_MouseMove(object sender, MouseEventArgs e)
		{
			//  左ボタン押下中で、ヒットテストが有効なら
			if (e.Button == MouseButtons.Left)
			{
				dndHelper.DispatchMouseMove(new Point(e.X, e.Y));
			}
		}

		/// <summary>
		/// マウスボタンリリースイベント
		/// </summary>
		/// <remarks>非ドラッグ中のみ発生。ドラッグ中は発生しない</remarks>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_MouseUp(object sender, MouseEventArgs e)
		{
			dndHelper.DispatchMouseUp(new Point(e.X, e.Y));
		}

		/// <summary>
		/// ドラッグ中イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		private void gridcLongtermRepairPlan_DragOver(object sender, DragEventArgs e)
		{
			dndHelper.DispatchDragOver(sender, e);
		}

		/// <summary>
		/// ドロップイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		private void gridcLongtermRepairPlan_DragDrop(object sender, DragEventArgs e)
		{
			dndHelper.DispatchDragDrop(sender, e);
		}

		/// <summary>
		/// グリッドダブルクリックイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void gridcLongtermRepairPlan_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// TODO
			/*try
			{
				//  カーソル位置を取得
				GridHitInfo hi = gridvLongtermRepairPlan.CalcHitInfo(e.Location) as GridHitInfo;

				//  セルなら
				if (hi.HitTest == GridHitTest.RowCell)
				{
					//  ドラッグセルなら
					if ((hi.Column.Tag is string) && (hi.Column.Tag as string == COMSKCommon.TAG_DRAGGABLE_CELL))
					{
						//  編集許可
						hi.Column.OptionsColumn.AllowEdit = true;
						allowEdit = true;

						//  エディタ表示
						gridvLongtermRepairPlan.ShowEditorByMouse();
					}
				}
			}
			catch (Exception)
			{
			}*/
		}

		private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			// Forces CellValueChanged to fire immediately for checkbox/combobox etc.
			if (_grid.IsCurrentCellDirty)
				_grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}

		/// <summary>
		/// セルの値変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs"/> instance containing the event data.</param>
		private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

				//  行データを取得
				LongRepairPlanData data = _grid.Rows[e.RowIndex].DataBoundItem as LongRepairPlanData;
				if (data == null) return;

				DataGridViewColumn col = _grid.Columns[e.ColumnIndex];
				if (col == null) return;

				//  会計期インデックス
				int columnIndex = col.DisplayIndex; // 見た目順(DevExpressのIndexOfに近い用途)
				data.PeriodIndex = (columnIndex - DATA_START_COLUMN);

				//  実績年度カラムなら
				if (col.Name == "bgcolEffectedYear")
				{
					if (EffectedYearChanged != null)
					{
						//  イベント着火
						EffectedYearChanged(this, new CustomEventArgs<LongRepairPlanData>(data));
					}
				}
				//　周期起算年カラムなら
				else if (col.Name == "bgcolDatumYear")
				{
					if (DatumYearChanged != null)
					{
						//　イベント着火
						DatumYearChanged(this, new CustomEventArgs<LongRepairPlanData>(data));
					}
				}
				//  ドラッグセルなら
				else if ((col.Tag is string) && ((col.Tag as string) == COMSKCommon.TAG_DRAGGABLE_CELL))
				{
					if (CellValueChanged != null)
					{
						//  変更後値をセット
						object v = _grid[e.ColumnIndex, e.RowIndex].Value;
						if (v != null)
						{
							// DevExpressの e.Value 相当
							data.NewValue = Convert.ToInt32(v);
						}

						//  イベント着火
						CellValueChanged(this, new CustomEventArgs<LongRepairPlanData>(data));
					}
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Row Cell のスタイル変更をするタイミングで発生するイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridvLongtermRepairPlan_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			// TODO
			/*try
			{
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;

				//  行タイプで分岐
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					//  * 修繕項目 *
					if (e.Column.Equals(bgcolConstructionItem) == true)
					{
						e.Appearance.BackColor = Color.LightGray;
					}
					else if ((e.Column == bgcolConstructionCategory) ||
						(e.Column == bgcolConstructionPosition) ||
						(e.Column == bgcolConstructionRegion) ||
						(e.Column == bgcolConstructionSpecification) ||
						(e.Column == bgcolConstructionDivision) ||
						(e.Column == bgcolRepairConstructionContent))
					{
						e.Appearance.BackColor = Color.White;
					}

					//  ドラッグセルなら
					if ((e.Column.Tag is string) && (e.Column.Tag as string == COMSKCommon.TAG_DRAGGABLE_CELL))
					{
						//  カラムインデックス
						int columnIndex = gridvLongtermRepairPlan.Columns.IndexOf(e.Column) - DATA_START_COLUMN;

						//  フラグが立っていたら
						if ((startPeriodIndex + columnIndex) < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							if (obj.GetDragged(startPeriodIndex + columnIndex) == COMSKCommon.DRAGGED_FLG_ON)
							{
								//  色を変える
								e.Appearance.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_DRAGGED;
							}
						}
					}
				}
				else if (obj.Row == LongRepairPlanData.RowType.CalcA || obj.Row == LongRepairPlanData.RowType.CalcB || obj.Row == LongRepairPlanData.RowType.CalcC)
				{
					e.Appearance.BackColor = Color.LightSeaGreen;
				}
				else if (obj.Row == LongRepairPlanData.RowType.CalcD || obj.Row == LongRepairPlanData.RowType.CalcE)
				{
					e.Appearance.BackColor = Color.LightGreen;
				}

				//  ヘルパがあれば
				if (dndHelper != null)
				{
					//  選択範囲内なら
					if (dndHelper.IsInSelection(e))
					{
						//  色を変える
						e.Appearance.BackColor = Color.Lavender;
					}
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}*/
		}

		/// <summary>
		/// 行のデザイン変更時に呼び出されるイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
		{
			// TODO
			/*try
			{
				//  対象のレコードを取得
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
				if (obj != null)
				{
					//  データタイプが RepairPlan なら
					if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
					{
						//  色を決定 (if 文が上にいくほど優先順位が高い)

						//  変更点があるなら
						if (obj.UpdateContentList.Count > 0)
						{
							e.Appearance.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_NO_REASON;

						}
						//  長計側で変更されているか
						else if (obj.ModifiedAtLongRepairPlanFlg == COMSKCommon.HAS_HISTORY_FLG_ON)
						{
							e.Appearance.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_MODIFIED_AT_LONGTERM_REPAIR_PLAN;
						}
						//  取り込みフラグを見る
						else if (obj.ImportFlg != COMSKCommon.IMPORT_FLG_NONE)
						{
							if (obj.ImportFlg == COMSKCommon.IMPORT_FLG_REPAIR_PLAN)
							{
								e.Appearance.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_PLAN;
							}
							else if (obj.ImportFlg == COMSKCommon.IMPORT_FLG_REPAIR_HISTORY)
							{
								e.Appearance.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_HISTORY;
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
			}*/
		}

		/// <summary>
		/// Row Cell をクリックした際に発生するイベントハンドラ
		/// </summary>Grid_CellMouseClick
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				if (e.RowIndex >= 0)
					this.selectedRow = this._grid.Rows[e.RowIndex].DataBoundItem as LongRepairPlanData;
				else
					this.selectedRow = null;
			}
			catch (Exception ex)
			{
				this.selectedRow = null;
			}

			// DevExpress: gridvLongtermRepairPlan.RefreshRow(e.RowHandle);
			// DataGridView: InvalidateRow で再描画
			if (e.RowIndex >= 0)
				this._grid.InvalidateRow(e.RowIndex);

			if (isDisabled) return;

			try
			{
				// 左ボタン以外は何もしない。
				if (e.Button == MouseButtons.Left)
				{
					// Rowデータが予期していないデータ型の場合は何もしない。
					if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

					LongRepairPlanData obj = this._grid.Rows[e.RowIndex].DataBoundItem as LongRepairPlanData;
					if (obj == null) return;

					DataGridViewColumn col = this._grid.Columns[e.ColumnIndex];
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
					else if (col.Name == "bgcolRepairPlan")
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
		private void gridvLongtermRepairPlan_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
			// TODO
			/*try
			{
				// Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;

				//  行タイプで分岐
				if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					//  カラムで分岐
					if (e.Column == bgcolDatumYear)
					{
						//  基準年
						if (obj.DatumYear == 0)
						{
							e.DisplayText = string.Empty;
						}
						else
						{
							e.DisplayText = obj.DatumYear.ToString();
						}
					}
					//else if (e.Column.Equals(bgcolSubTotal) == false &&
					//    e.Column.Equals(bgcolConsumptionTax) == false &&
					//    e.Column.Equals(bgcolTotal) == false &&
					//    e.Column.Equals(bgcolAmount) == false &&
					//    e.Column != bgcolRepairPeriod)
					else if (e.Column.Equals(bgcolSubTotal) == false &&
						e.Column.Equals(bgcolAmount) == false &&
						e.Column != bgcolRepairPeriod)
					{
						// 対象行か確認
						if (e.Value is long)
						{
							e.DisplayText = COMSKCommon.ConvertToLongRepairPlanText(this.MntPlanConst.ViewUnit, (long)e.Value);
						}
					}
					else if (e.Column == bgcolAmount)
					{
						if (obj.Amount == double.MinValue)
						{
							e.DisplayText = string.Empty;
						}
					}
					else if (e.Column == bgcolRepairPeriod)
					{
						if (obj.Cycle == int.MinValue)
						{
							e.DisplayText = string.Empty;
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

					//  特定のカラムは無視
					if (e.Column == bgcolRepairPeriod ||
						e.Column == bgcolAmount ||
						e.Column == bgcolUnit ||
						e.Column == bgcolEffectedYear ||
						e.Column == bgcolDatumYear)
					{
						dispText = string.Empty;
					}
					else if (e.Column == bgcolConstructionItem)
					{
						dispText = e.Value as string;
					}

					//  想定累計列なら
					if (obj.Row == LongRepairPlanData.RowType.CalcE)
					{
						//  小計 / 消費税 / 累計は空欄
						//if (e.Column == bgcolSubTotal ||
						//    e.Column == bgcolConsumptionTax ||
						//    e.Column == bgcolTotal)
						if (e.Column == bgcolSubTotal)
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
								dispText = COMSKCommon.ConvertToLongRepairPlanText(MntPlanConst.ViewUnit, (long)e.Value);
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
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}*/
		}

		/// <summary>
		/// Cellの編集をさせたくない場合に設定処理を行うタイミングに発生するイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridvLongtermRepairPlan_ShowingEditor(object sender, CancelEventArgs e)
		{
			// TODO
			/*try { this.selectedRow = gridvLongtermRepairPlan.GetRow(gridvLongtermRepairPlan.FocusedRowHandle) as LongRepairPlanData; }
			catch (Exception ex) { this.selectedRow = null; }
			gridvLongtermRepairPlan.RefreshData();
			try
			{
				// Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(gridvLongtermRepairPlan.FocusedRowHandle) as LongRepairPlanData;

				//  デフォルトは編集不可
				e.Cancel = true;

				//  カラム
				DevExpress.XtraGrid.Columns.GridColumn gc = gridvLongtermRepairPlan.FocusedColumn;

				//  実績年度列なら
				if (gc == bgcolEffectedYear || gc == bgcolDatumYear)
				{
					//  履歴実績取込がなされていなければ
					if (obj.ImportFlg != COMSKCommon.IMPORT_FLG_REPAIR_HISTORY)
					{
						//  編集可能
						e.Cancel = false;
					}
				}
				//  ドラッグカラムなら
				else if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					//  編集許可なら
					if (allowEdit == true)
					{
						//  編集可能
						e.Cancel = false;

						//  フラグをクリア
						allowEdit = false;
					}
				}

				//  編集可能なら
				if (e.Cancel == false)
				{
					//  ドラッグセルなら
					if ((gc.Tag is string) && (gc.Tag as string == COMSKCommon.TAG_DRAGGABLE_CELL))
					{
						//  セルの値を取得
						object value = gridvLongtermRepairPlan.GetRowCellValue(gridvLongtermRepairPlan.FocusedRowHandle,
							gridvLongtermRepairPlan.FocusedColumn);

						long currValue = 0;
						if (value != null)
						{
							currValue = long.Parse(value.ToString());
						}

						//  変更前値をセット
						obj.PrevValue = currValue;
					}
				}
			}
			catch (Exception ex)
			{
				e.Cancel = true;
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}*/
		}

		/// <summary>
		/// セルにリポジトリコントロールを動的に設定する場合に使用するイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridvLongtermRepairPlan_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			// TODO
			/*if (isDisabled) return;

			try
			{
				// Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;

				// 対象列か確認
				DevExpress.XtraEditors.Repository.RepositoryItem repItem = null;
				if (e.Column == bgcolHistory)
				{
					if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
					{
						//  変更履歴があれば赤、それ以外は通常ボタン
						if (obj.UpdateFlg == COMSKCommon.HAS_HISTORY_FLG_ON || obj.WorkUpdateReasonList.Count > 0)
						{
							repItem = repBtnOpenRed;
						}
						else
						{
							repItem = repBtnOpen;
						}
					}
				}
				else if (e.Column == bgcolRepairPlan)
				{
					if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
					{
						//  修繕履歴集約行でなければ
						if (obj.ConstructionSpecificationName == COMSKCommon.REPAIR_HISTORY_GROUP_SPEC_TEXT)
						{
							repItem = repEmpty;
						}
						else
						{
							repItem = repBtnOpen;
						}
					}
				}
				else if (e.Column == bgcolRegister)
				{
					if (obj.Row == LongRepairPlanData.RowType.RepairPlan)
					{
						repItem = repBtnOpen;
					}
				}

				if (repItem != null)
				{
					if (e.RepositoryItem.Equals(repItem) == false)
					{
						e.RepositoryItem = repItem;
					}
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}*/

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
		/// エディタ表示完了後イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_ShownEditor(object sender, EventArgs e)
		{
			// TODO
			/*try
			{
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(gridvLongtermRepairPlan.FocusedRowHandle) as LongRepairPlanData;

				if (obj != null)
				{
					DevExpress.XtraGrid.Columns.GridColumn gc = gridvLongtermRepairPlan.FocusedColumn;
					if ((gc.Tag is string) && (gc.Tag as string == COMSKCommon.TAG_DRAGGABLE_CELL))
					{
						int focusedColumnIndex = gridvLongtermRepairPlan.Columns.IndexOf(gridvLongtermRepairPlan.FocusedColumn);
						int index = focusedColumnIndex - DATA_START_COLUMN + startPeriodIndex;
						if (obj.GetValue(index) == 0)
						{
							gridvLongtermRepairPlan.EditingValue = string.Empty;
						}
					}
				}
			}
			catch (Exception)
			{
			}*/
		}


		private void Grid_RowCellsDragCompleted(object sender, RowCellsDragEventArgs e)
		{
			// bubble up to parent
			RowCellsDragCompleted?.Invoke(this, e);
		}

		private void gridvLongtermRepairPlan_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			// TODO - maybe not use
			/*LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
			// マージセールがボーダーしない
			if (this.selectedRow != null && e.RowHandle == gridvLongtermRepairPlan.FocusedRowHandle)
			{
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
			this.zoomFactor = rate; // set new rate
			List<string> keepSizeBand = new List<string>(new[]
			{
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
			//XtraGridUtil.RestoreAllBanded(this.gridvLongtermRepairPlan);
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
		#endregion
	}
}
