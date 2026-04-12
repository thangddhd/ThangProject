using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using coms.COMSK.common;
using coms.COMSKService;

namespace coms.COMSK.ui.common
{
    /// <summary>
    /// 長期修繕計画総括表用のグリッドコントロール
    /// </summary>
    public partial class CtrLongtermRepairPlan_Total : UserControl
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
		#endregion メンバ変数

		#region プロパティ

		/// <summary>
		/// データソース
		/// </summary>
		private List<LongRepairPlanData> dataSource = null;
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
					dataSource = ConvertLongRepairPlanDatas(value);
				}
				else
				{
					dataSource = new List<LongRepairPlanData>();
				}
				_bs = new BindingSource();
				_bs.DataSource = dataSource;
				_grid.DataSource = _bs;
				//m_mgrGroup.SetOpenAll();
				//BindData();
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
			this.initGridView();
			// TODO grouping
			// repLinkPlusのPlus画像の調整
			//repLinkPlus.Image = COMSKCommon.GetPlusBitmap();
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
				"bgcolType",
				"bgcolItem"
			});
			_grid.VerticalMergeProvider = new LongRPAllMergeProvider();

			// Left columns list (for calc-row horizontal merge) - not used for testObj unless you set IsCalcRow
			_grid.SetLeftColumnNames(new[] {
				"bgcolType",
				"bgcolItem"
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

			// mouse wheel
			_grid.MouseWheel += Grid_MouseWheel;

			_grid.CanDragCell = (g, rowIndex, col, model) =>
			{
				// old behavior: only RepairPlan rows + column.Tag must match
				if (model == null) return false;
				//if (model.Row != "BBB") return false;
				return (col.Tag as string) == COMSKCommon.TAG_DRAGGABLE_CELL;
			};

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
			grid.Columns.Add(CreateCol("bgcolType", "ConstructionTypeName", "区分", 60));
			grid.Columns.Add(CreateCol("bgcolItem", "ConstructionCategoryName", "項目", 90));
			grid.Columns.Add(CreateCol("bgcolDummy", "", "Dummy", 90));
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
			_grid.Columns["bgcolType"].Frozen = true;
			_grid.Columns["bgcolItem"].Frozen = true;
			_grid.Columns["bgcolDummy"].Frozen = true;
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
				if (row == null || nextRow == null) return false;

				if (columnName == "bgcolType") return row.ConstructionTypeName == nextRow.ConstructionTypeName;
				if (columnName == "bgcolItem") return row.Row == nextRow.Row && row.ConstructionItemName == nextRow.ConstructionItemName;

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

		private void gridvLongtermRepairPlan_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e)
		{
			COMSKCommon.CustomDrawBandHeader(e, this.originalAccountStartPeriodIdx);
		}

		/// <summary>
		/// 表示データを更新する
		/// </summary>
		public void RefreshData()
		{
			_grid.Refresh();
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

		private void gridvLongtermRepairPlan_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			// TODO
			/*LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
			COMSKCommon.CustomDrawCostCell(e, this.originalAccountStartPeriodIdx, obj.Row);*/
		}

		/// <summary>
		/// パラメータで指定されたデータをグリッドコントロールへバインド
		/// </summary>
		private void BindData()
		{
			// TODO check if need
			/*try
			{
				if (gridvLongtermRepairPlan != null && gridvLongtermRepairPlan.DataSource != null)
				{
					if (m_Helper == null)
					{
						m_Helper = new MyCellMergeHelper(gridvLongtermRepairPlan);
					}
					m_Helper.Clear();

					List<LongRepairPlanData> listDataTemp = new List<LongRepairPlanData>();
					foreach (LongRepairPlanData data in dataSource)
					{
						if (data.Row == LongRepairPlanData.RowType.GroupCategory)
						{
							if (m_mgrGroup.IsDisplay(data.GroupID) == false)
							{
								continue;
							}
						}
						listDataTemp.Add(data);
					}

					//  データソースを設定
					gridcLongtermRepairPlan.DataSource = listDataTemp;

					// 集計行の列マージのクリア
					m_Helper.MergedCells.Clear();

					// データソースへ設定
					// フォーカス位置、スクロール位置が先頭行へ戻らないようにも制御
					int pos = gridvLongtermRepairPlan.TopRowIndex;
					int currentRow = gridvLongtermRepairPlan.FocusedRowHandle;
					this.gridcLongtermRepairPlan.DataSource = listDataTemp;
					this.gridcLongtermRepairPlan.RefreshDataSource();
					gridvLongtermRepairPlan.TopRowIndex = pos;
					gridvLongtermRepairPlan.FocusedRowHandle = currentRow;

					// 集計行の列マージ
					for (int i = 0; i < listDataTemp.Count; i++)
					{
						LongRepairPlanData data = listDataTemp[i];

						switch (data.Row)
						{
							case LongRepairPlanData.RowType.CalcA:
							case LongRepairPlanData.RowType.CalcB:
							case LongRepairPlanData.RowType.CalcC:
							case LongRepairPlanData.RowType.CalcD:
							case LongRepairPlanData.RowType.CalcE:
								m_Helper.AddMergedCell(i, 0, 1, data.ConstructionTypeName);
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

        #region イベントハンドラ

        /// <summary>
        /// Row Cell のスタイル変更をするタイミングで発生するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandedGridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
			// TODO
            /*try
            {
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
				if (obj != null)
				{
					//
					// データタイプ別に背景色調整
					//
					switch (obj.Row)
					{
						case LongRepairPlanData.RowType.GroupItem:
							// 区分列は何もしない
							if (e.Column.Equals(bgcolType) == false)
							{
								e.Appearance.BackColor = Color.LightGray;
							}
							break;

						case LongRepairPlanData.RowType.CalcA:
						case LongRepairPlanData.RowType.CalcB:
						case LongRepairPlanData.RowType.CalcC:
							e.Appearance.BackColor = Color.LightSeaGreen;
							break;

						case LongRepairPlanData.RowType.CalcD:
						case LongRepairPlanData.RowType.CalcE:
							e.Appearance.BackColor = Color.LightGreen;
							break;

						default:
							break;
					}
				}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }*/
        }

        /// <summary>
        /// Row Cell をクリックした際に発生するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandedGridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
			// TODO
            /*try
            {
                // 左ボタン以外は何もしない。
                if (e.Button != MouseButtons.Left)
                {
                    return;
                }

                // 項目列以外の場合は何もしない。
                if (e.Column.Equals(bgcolItem) != true)
                {
                    return;
                }

                // Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
                if (obj == null)
                {
                    return;
                }

                //
                // 親データタイプ行がクリックされた場合、
                // 属する子データを表示にするためめ非表示グループリストへ追加or削除を行う。
                //
                if (obj.Row == LongRepairPlanData.RowType.GroupItem)
                {
                    m_mgrGroup.SetReverse(obj.GroupID, obj.ConstructionItemName);
                    BindData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }*/
        }
        
        /// <summary>
        /// 表示テキスト変更したいときに使用するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void bandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
			// TODO
			/*try
			{
				// Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
				if (obj != null)
				{
					//  タグ
					string tagStr = string.Empty;
					if (e.Column.Tag is string)
					{
						tagStr = e.Column.Tag as string;
					}

					//  年度列なら
					if (tagStr == COMSKCommon.TAG_DRAGGABLE_CELL)
					{
						//  集計行でなければ
						if ((obj.Row == LongRepairPlanData.RowType.RepairPlan) ||
							(obj.Row == LongRepairPlanData.RowType.GroupItem) ||
							(obj.Row == LongRepairPlanData.RowType.GroupCategory))
						{
							//  年次データ
							//TODO: なぜこのタブだけ e.Value が null になる？
							if (e.Value is long)
							{
								long value = (long)e.Value;
								e.DisplayText = COMSKCommon.ConvertToLongRepairPlanText(this.MntPlanConst.ViewUnit, (long)e.Value);
								//if (value != 0)
								//{
								//    e.DisplayText = string.Format("{0:N0}", value);
								//}
								//else
								//{
								//    e.DisplayText = string.Empty;
								//}
							}
						}
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
							e.DisplayText = string.Empty;
						}
					}

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}*/
		}

        /// <summary>
        /// セルにリポジトリコントロールを動的に設定する場合に使用するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandedGridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
			// TODO
            /*try
            {
                // Rowデータが予期していないデータ型の場合は何もしない。
				LongRepairPlanData obj = gridvLongtermRepairPlan.GetRow(e.RowHandle) as LongRepairPlanData;
                if (obj == null)
                {
                    return;
                }

                //
                // Parent行の開閉表示
                //

                // 対象列か確認
                if (e.Column.Equals(bgcolItem) == true)
                {
                    // 対象行か確認
					if (obj.Row == LongRepairPlanData.RowType.GroupItem)
                    {
                        if (m_mgrGroup.IsDisplay(obj.GroupID) == false)
                        {
                            e.RepositoryItem = repLinkPlus;
                        }
                        else
                        {
                            e.RepositoryItem = repLinkMinus;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }*/
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
				"bgcolType", "bgcolItem"
			});
			XtraGridUtil.SetZoom(this._grid, dictBandWidth, rate, keepSizeBand);
		}

		public void RestoreAllBanded()
		{
			// TODO
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
    }
}
