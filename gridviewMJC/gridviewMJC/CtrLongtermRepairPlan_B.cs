using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

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
		public event DnDHelper.DragCompletedHandler DragCompleted;

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
				return gridcLongtermRepairPlan.DataSource as List<LongRepairPlanData>;
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
				gridcLongtermRepairPlan.DataSource = list;
				if (value != null)
				{
					BindData();

					this.GetMergedCells();
				}
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
			m_Helper = null;

			InitializeComponent();

			//  工事分類 = 設備の場合のみ、ラベルを書き換える
			if (ConstrType == COMSKCommon.ConstructionType.Equipment)
			{
				gbPosition.Caption = "部位 (ユニット)";
				gbRegion.Caption = "部品";
			}

			//  DnD ヘルパを初期化
			dndHelper = new DnDHelper(gridvLongtermRepairPlan);
			dndHelper.DragCompleted += new DnDHelper.DragCompletedHandler(dndHelper_DragCompleted);

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
            gridvLongtermRepairPlan.OptionsBehavior.Editable = false;
            bgcolRepairPlan.ColumnEdit = repOpenBtnDisabled;
            bgcolRegister.ColumnEdit = repOpenBtnDisabled;
            bgcolHistory.ColumnEdit = repOpenBtnDisabled;
        }

		private void GetMergedCells()
        {
			List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColMerge = new List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>(
			new[] {
				this.bgcolConstructionItem
				, this.bgcolConstructionCategory
				, this.bgcolConstructionPosition
				, this.bgcolConstructionRegion
				, this.bgcolConstructionSpecification
			});
			this.lstMergedCols = XtraGridUtil.GetRepairPlanMergedCells(this.gridvLongtermRepairPlan, lstColMerge);
			// colbutton
			this.lstColButton = new List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>(new[] { bgcolHistory, bgcolRepairPlan, bgcolRegister });
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
				//  既存の年度カラムを削除する
				List<DevExpress.XtraGrid.Columns.GridColumn> tempColumns = new List<DevExpress.XtraGrid.Columns.GridColumn>();
				foreach (DevExpress.XtraGrid.Columns.GridColumn gc in gridvLongtermRepairPlan.Columns)
				{
					if (gc.Tag is string)
					{
						if (gc.Tag as string == COMSKCommon.TAG_DRAGGABLE_CELL)
						{
							tempColumns.Add(gc);
						}
					}
				}
				foreach (DevExpress.XtraGrid.Columns.GridColumn gc in tempColumns)
				{
					gridvLongtermRepairPlan.Columns.Remove(gc);
				}

				//  既存の年度バンドを削除する
				List<DevExpress.XtraGrid.Views.BandedGrid.GridBand> tempBands = new List<DevExpress.XtraGrid.Views.BandedGrid.GridBand>();
				foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand gb in gridvLongtermRepairPlan.Bands)
				{
					if (gb.Tag is string)
					{
						string tagStr = gb.Tag as string;

						if (tagStr == COMSKCommon.TAG_DRAGGABLE_CELL || tagStr == COMSKCommon.TAG_ADD_CONTROL_CALENDARYEAR)
						{
							tempBands.Add(gb);
						}
					}
				}
				foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand gb in tempBands)
				{
					gridvLongtermRepairPlan.Bands.Remove(gb);
				}

				//  新たに挿入
				COMSKCommon.CreateYearColumns(gridvLongtermRepairPlan, accountStartPeriod, displayStartPeriod, count, termInfo);
				startPeriodIndex = displayStartPeriod;
				this.accountStartPeriodIdx = accountStartPeriod;
				this.originalAccountStartPeriodIdx = oAccountStartPeriod;

				// 集計列を最後尾へ移動
				gridvLongtermRepairPlan.Bands.MoveTo(gridvLongtermRepairPlan.Bands.Count, gridBandSubTotal);
				//gridvLongtermRepairPlan.Bands.MoveTo(gridvLongtermRepairPlan.Bands.Count, gridBandConsumptionTax);
				//gridvLongtermRepairPlan.Bands.MoveTo(gridvLongtermRepairPlan.Bands.Count, gridBandTotal);

				//  データソースリフレッシュ
				gridvLongtermRepairPlan.RefreshData();
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
			gridvLongtermRepairPlan.RefreshData();
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
			try
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
			}
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
			try
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
			}
		}


		/// <summary>
		/// セルの値変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
		{
			try
			{
				//  行データを取得
				LongRepairPlanData data = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;

				//  会計期インデックス
				int columnIndex = gridvLongtermRepairPlan.Columns.IndexOf(e.Column);
				data.PeriodIndex = (columnIndex - DATA_START_COLUMN);

				//  実績年度カラムなら
				if (e.Column == bgcolEffectedYear)
				{
					if (EffectedYearChanged != null)
					{
						//  イベント着火
						EffectedYearChanged(this, new CustomEventArgs<LongRepairPlanData>(data));
					}
				}
                //　周期起算年カラムなら
                else if (e.Column == bgcolDatumYear)
                {
                    if (DatumYearChanged != null)
                    {
                        //　イベント着火
                        DatumYearChanged(this, new CustomEventArgs<LongRepairPlanData>(data));
                    }
                }
                //  ドラッグセルなら
                else if ((e.Column.Tag is string) && (e.Column.Tag as string == COMSKCommon.TAG_DRAGGABLE_CELL))
                {
                    if (CellValueChanged != null)
                    {
                        //  変更後値をセット
                        data.NewValue = int.Parse(e.Value.ToString());

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
			try
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
			}
		}

		/// <summary>
		/// 行のデザイン変更時に呼び出されるイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
		{
			try
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
			}
		}

		/// <summary>
		/// Row Cell をクリックした際に発生するイベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridvLongtermRepairPlan_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
		{
            try { this.selectedRow = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData; }
			catch (Exception ex) { this.selectedRow = null; }
			
			gridvLongtermRepairPlan.RefreshRow(e.RowHandle);
			if (isDisabled) return;

			try
			{
				// 左ボタン以外は何もしない。
				if (e.Button == MouseButtons.Left)
				{
					// Rowデータが予期していないデータ型の場合は何もしない。
					LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;


					//
					// クリックされた列・行により処理を切り分け
					//
					if (e.Column.Equals(bgcolHistory) == true)
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
					else if (e.Column.Equals(bgcolRepairPlan) == true)
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
					else if (e.Column.Equals(bgcolRegister) == true)
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
			try
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
			}
		}

		/// <summary>
		/// Cellの編集をさせたくない場合に設定処理を行うタイミングに発生するイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridvLongtermRepairPlan_ShowingEditor(object sender, CancelEventArgs e)
		{
			try { this.selectedRow = gridvLongtermRepairPlan.GetRow(gridvLongtermRepairPlan.FocusedRowHandle) as LongRepairPlanData; }
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
			}
		}

		/// <summary>
		/// セルにリポジトリコントロールを動的に設定する場合に使用するイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridvLongtermRepairPlan_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
		{
			if (isDisabled) return;

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
			}

		}

		/// <summary>
		/// 行をマージ制御するタイミングで発生するイベントである
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridvLongtermRepairPlan_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
		{
			try
			{
				LongRepairPlanData row1 = gridvLongtermRepairPlan.GetRow(e.RowHandle1) as LongRepairPlanData;
				LongRepairPlanData row2 = gridvLongtermRepairPlan.GetRow(e.RowHandle2) as LongRepairPlanData;

				//  デフォルトではマージ OFF
				e.Merge = false;
				e.Handled = true;

				if (row1.Row == LongRepairPlanData.RowType.CalcA)
				{
					if (e.RowHandle1 == e.RowHandle2)
					{
						e.Merge = true;

					}
				}
				//  列で分岐
				if (e.Column == bgcolConstructionItem)
				{
					e.Merge = (row1.ConstructionItemName == row2.ConstructionItemName);
				}
				else if (e.Column == bgcolConstructionCategory)
				{
					e.Merge = (row1.ConstructionCategoryName == row2.ConstructionCategoryName);
				}
				else if (e.Column == bgcolConstructionPosition)
				{
					e.Merge = (row1.ConstructionPositionName == row2.ConstructionPositionName);
				}
				else if (e.Column == bgcolConstructionRegion)
				{
					e.Merge = (row1.ConstructionRegionName == row2.ConstructionRegionName);
				}
				else if (e.Column == bgcolConstructionSpecification)
				{
					e.Merge = (row1.ConstructionSpecificationName == row2.ConstructionSpecificationName);
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs"/> instance containing the event data.</param>
		private void gridvLongtermRepairPlan_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
		{
			try
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
			}
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
			try
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
			}
		}


		/// <summary>
		/// ドラッグドロップ完了
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		void dndHelper_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (DragCompleted != null)
			{
				int columnIndex1 = gridvLongtermRepairPlan.Columns.IndexOf(e.From.Column);
				int columnIndex2 = gridvLongtermRepairPlan.Columns.IndexOf(e.To.Column);

				if (columnIndex1 != columnIndex2)
				{
					//  絶対位置に変換
					columnIndex1 -= DATA_START_COLUMN;
					columnIndex2 -= DATA_START_COLUMN;

					e.FromColumnIndex = columnIndex1;
					e.ToColumnIndex = columnIndex2;

					DragCompleted(sender, e);
				}
			}
		}

		private void gridvLongtermRepairPlan_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
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
			}
		}

		public void GetBandedWidthDict(int currentRate, bool reCreatedYearly)
		{
			this.dictBandWidth = XtraGridUtil.GetBandedWidthDict(this.gridvLongtermRepairPlan, currentRate, reCreatedYearly);
		}

		public void SetZoom(int rate, bool reCreatedYearly = false)
		{
			if (this.dictBandWidth.Keys.Count == 0)
			{
				this.GetBandedWidthDict(this.zoomFactor, reCreatedYearly);
			}
			this.zoomFactor = rate;  // set new zoom rate
			List<string> keepSizeBand = new List<string>(new[]
				{
					gridBand4.Name
					, gbPosition.Name
					, gridBand2.Name
					, gridBand16.Name
					, gridBand18.Name
					, gridBand17.Name
					, gridBand15.Name
					, gridBand9.Name
				});
			XtraGridUtil.SetZoom(this.gridvLongtermRepairPlan, dictBandWidth, rate, keepSizeBand);
		}

		public void RestoreAllBanded()
		{
			XtraGridUtil.RestoreAllBanded(this.gridvLongtermRepairPlan);
		}

		public void ResetBandedDict()
		{
			this.dictBandWidth = new Dictionary<string, int>();
		}

		private void gridvLongtermRepairPlan_MouseWheel(object sender, MouseEventArgs e)
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
				XtraGridUtil.DoScroll(this.gridvLongtermRepairPlan, e);
				((HandledMouseEventArgs)e).Handled = true;
			}
		}
		#endregion
	}
}
