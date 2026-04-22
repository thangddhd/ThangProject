using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using coms.COMSKService;
using coms.COMSK.common;
using coms.COMMON;

namespace coms.COMSK.ui.common
{
	/// <summary>
	/// 修繕積立金計画のタブ内のコントロール
	/// </summary>
	public partial class CtrRepairReservePlan : UserControl
	{
		#region イベント

		/// <summary>
		/// 積立金累計変更イベント
		/// </summary>
		public event EventHandler TotalReserveChanged;

		public event EventHandler ScrollChanged;

		/// <summary>
		/// 案名変更イベント
		/// </summary>
		public event EventHandler DraftNameChanged;

		/// <summary>
		/// 使用するChk変更イベント
		/// </summary>
		public event EventHandler UseChanged;

        /// <summary>
        /// https://reci.backlog.jp/view/MJC_DEV-524
        /// ユーザーが行った最後の編集を記録
        /// </summary>
        public Object preSender = null;
        public EventArgs preEvent = null;

        /// <summary>
        /// ユーザーが行った編集を反映しているときにtrue
        /// </summary>
        private bool preMode = false;

        /// <summary>
        /// 表の値変更イベントでユーザーが行った編集値
        /// </summary>
        private double preVal;

		public bool scrollingByParent { get; set; }
		#endregion

		#region メンバ

		/// <summary>
		/// 全体のマスタデータ
		/// </summary>
		private List<Data30Period> dataList = new List<Data30Period>();

		/// <summary>
		/// 案の番号
		/// </summary>
		private int draftIndex = 0;

		/// <summary>
		/// 表示単位
		/// </summary>
		private string viewUnit = string.Empty;

		/// <summary>
		/// 工事費差額最低額
		/// </summary>
		private long minDiffRepairCost = 0;


		/// <summary>
		/// 計算ヘルパ
		/// </summary>
		private RepairReservePlanCalculator calculator = new RepairReservePlanCalculator();

		/// <summary>
		/// 以後反映コード
		/// </summary>
		private string applyDetailCode = string.Empty;

		/// <summary>
		/// 以後反映行番号
		/// </summary>
		private int applyRowIndex = 0;

		/// <summary>
		/// 以後反映開始カラム
		/// </summary>
		private int applyColumnIndex = 0;

		private Dictionary<string, List<int>> valueChangedCells = new Dictionary<string, List<int>>();
		private List<string> valueChangedCellCheckCodes = new List<string>()
		{
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TRANSFAR_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_IN_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_TRANSFAR_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_MONTH_RESERVE_COST,
			COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_LUMPSUM_COST
		};
		private string calcShortBefore = "";
		bool isArea = false;
		#endregion

		#region プロパティ

		/// <summary>
		/// 積立金累計データ
		/// </summary>
		[Browsable(false)]
		public Data30Period TotalReserveData
		{
			get
			{
				return calculator.TotalReserveCost;
			}
		}

		/// <summary>
		/// 案の名前
		/// </summary>
		[Browsable(false)]
		public string DraftName
		{
			get
			{
				return txtDraftName.Text;
			}
			set
			{
				txtDraftName.Text = value;
			}
		}

		/// <summary>
		/// 案番号
		/// </summary>
		[Browsable(false)]
		public int DraftIndex
		{
			get
			{
				return draftIndex;
			}
			set
			{
				if (value != draftIndex)
				{
					draftIndex = value;

					//  panelForLabels 内のラベルの先頭
					if (panelForLabels.Controls.Count > 0)
					{
						Label lbl = panelForLabels.Controls[0] as Label;
						if (lbl != null)
						{
							//  色を変える
							lbl.BackColor = COMSKCommon.REPAIR_RESERVE_PLAN_COLORS[draftIndex];

						}
					}
				}
			}
		}

		/// <summary>
		/// 明細のサマリ
		/// </summary>
		[Browsable(false)]
		public KumiaiRepairReservePlanDetailSummary[] DetailSummary { get; set; }

		/// <summary>
		/// 現在の表示モード
		/// </summary>
		private COMSKCommon.RepairReserveViewMode ViewMode = COMSKCommon.RepairReserveViewMode.Full;

		/// <summary>
		/// 工事費最低差額
		/// </summary>
		[Browsable(false)]
		public long MinimumDiffRepairCost { get; set; }

		/// <summary>
		/// 会計期情報
		/// </summary>
		[Browsable(false)]
		public KumiaiTermInfo[] TermInfo { get; set; }

		/// <summary>
		/// 開始会計期
		/// </summary>
		[Browsable(false)]
		public int AccountPeriod { get; set; }

		/// <summary>
		/// 使用するかどうか
		/// </summary>
		[Browsable(false)]
		public bool Uses
		{
			get
			{
				return chkUse.Checked;
			}
		}

		#endregion

		#region Public

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CtrRepairReservePlan()
		{
			InitializeComponent();
			this.scrollingByParent = false;
		}

		/// <summary>
		/// 案を設定する
		/// </summary>
		/// <param name="kumiaiRepairReservePlanDraft">The kumiai repair reserve plan draft.</param>
		public void SetDraft(KumiaiLongRepairPlan kumiaiLongRepairPlan, Data30Period totalRepairCost, KumiaiRepairReservePlanDetailSummary[] summary, KumiaiRepairReservePlanDraft draft)
		{
			isArea = kumiaiLongRepairPlan.MaintenancePlanInfo.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA;
			//  フィールドセット
			#region フィールドセット
			//  案サマリを設定
			SetDraftSummary(draft);
			COMSKCommon.SelectComboBox<string>(cmbCostShort, draft.CostShortCode);
			#endregion

			//  データソースを設定
			#region データソースを設定
			dataList.Clear();
			for (int i = 0; i < summary.Length; i++)
			{
				//  サマリーを取得
				KumiaiRepairReservePlanDetailSummary objSummary = summary[i];

				//  表示するか確定
				bool visibleTest = TestVisible(objSummary.ReservePlanDetailCode, kumiaiLongRepairPlan);

				//  値の配列
				MiniKumiaiRepairReservePlanDetail[] arrDetails = draft.Rows[i];

				//  値の配列を取得
				double[] d = (from item in arrDetails
							  select item.Value).ToArray();

				//  名前の後ろに付けるラベルを取得する
				string suffixLabel = GetDetailSuffixLabelFromCode(objSummary.ReservePlanDetailCode,
					kumiaiLongRepairPlan.MaintenancePlanInfo.ViewUnit,
					kumiaiLongRepairPlan.MaintenancePlanInfo.CalcDivision);

				//  表示形式
				Data30Period.Format displayFormat = GetData30PeriodDisplayFormat(objSummary.ReservePlanDetailCode);

				// 【参考】文字付ける
				string areaPrefix = "";
				if (objSummary.ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST
					|| objSummary.ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST)
                {
					areaPrefix = isArea ? "" : COMSKCommon.RESERVE_PLAN_DETAIL_CODE_PREFIX;
				}

				//  データを作成
				Data30Period data = new Data30Period(d)
				{
					DisplayFormat = displayFormat,
					Name = (areaPrefix + objSummary.ReservePlanDetailName + suffixLabel),
                    ReservePlanDetailCode = objSummary.ReservePlanDetailCode,
					Visible = visibleTest,
				};

				dataList.Add(data);
			}

			//  データソース設定 -- no show invisible
			gcData30Period.DataSource = dataList.Where(item => item.Visible).ToList();

			try
			{
				// IMPORTANT: ensure no editing control is active, otherwise CellPainting may skip repaint
				gcData30Period.EndEdit(DataGridViewDataErrorContexts.Commit);
				gcData30Period.CancelEdit();
			}
			catch { }

			// Reset current cell (prevents edit-mode state/cached painting)
			gcData30Period.CurrentCell = null;
			gcData30Period.ClearSelection();

			// now force full repaint
			gcData30Period.Invalidate(true);
			gcData30Period.Update();

			#endregion

			//  計算ヘルパにプロパティを設定
			#region 計算ヘルパにプロパティを設定
			calculator.YearHouseReserveCostLastInput = new Data30Period();
			calculator.TotalRepairCost = totalRepairCost;
			calculator.TotalReserveCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TOTAL_RESERVE_COST);
			calculator.DiffConstructionCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_DIFF_CONSTRUCTION_COST);
			calculator.YearRepairReserveCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_REPAIR_RESERVE_COST);
			calculator.YearHouseReserveCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST);
			calculator.TransfarCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TRANSFAR_COST);
			calculator.OtherInCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_IN_COST);
			calculator.OtherOutCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_COST);
			calculator.OtherOutTransfarCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_TRANSFAR_COST);
			calculator.HouseMonthReserveCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST);
			calculator.HouseLumpsumCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST);
			calculator.YearMonthReserveCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_MONTH_RESERVE_COST);
            calculator.YearLumpsumCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_LUMPSUM_COST);
			if (kumiaiLongRepairPlan.MaintenancePlanInfo.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_SHARED)
			{
				calculator.HouseSharedMonthReserveCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST);
				calculator.HouseSharedLumpsumCost = GetData30Period(summary, dataList, COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST);
			}
			calculator.SetCalculateOption(kumiaiLongRepairPlan);
            if (kumiaiLongRepairPlan.CarryOverCost != long.MinValue)
            {
                calculator.CarryOverCost.SetValue(0, kumiaiLongRepairPlan.CarryOverCost);
            }
            else
            {
                calculator.CarryOverCost.SetValue(0, 0);
            }
            
			#endregion

			//  戸当たり平均増額幅を計算
			CalcAvgCostIncreaseBand();

			//  表示単位だけ覚えておく
			viewUnit = kumiaiLongRepairPlan.MaintenancePlanInfo.ViewUnit;
			minDiffRepairCost = kumiaiLongRepairPlan.MinimumDiffRepairCost;

			//  ラベルを作成
			CreateLabels(draft);

			//  データリフレッシュ
			//gcData30Period.Refresh();
			//COMSKCommon.ForceRepaintGrid(gcData30Period);

			//  名前変更イベントを飛ばす
			if (DraftNameChanged != null)
			{
				DraftNameChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 案データを取得する
		/// </summary>
		/// <param name="draft">The draft.</param>
		public void GetDraft(KumiaiRepairReservePlanDraft draft)
		{
			//  ヘッダ情報を取得
			draft.Name = txtDraftName.Text.Trim();
			if (chkUse.Checked == true)
			{
				draft.SelectFlg = COMSKCommon.SELECT_FLG_ON;
			}
			else
			{
				draft.SelectFlg = COMSKCommon.SELECT_FLG_OFF;
			}
			draft.FirstRevisionYear = Helper.ParseToInt(txtFirstRevisionYear.Text);
			draft.RevisionPitch = Helper.ParseToInt(txtRevisionPitch.Text);
			draft.CostIncreaseBand = Helper.ParseToDouble(txtCostIncreaseBand.Text);
			draft.AvgIncreaseCostPerHouse = Helper.ParseToDouble(txtAvgIncreaseCostPerHouse.Text);
			draft.CostShortCode = COMSKCommon.GetSelectedComboBox<string>(cmbCostShort, null);

			//  その他情報
			draft.UpdateUserMstPid = Helper.loginUserInfo.Pid;

			//  行データを取得
			for (int i = 0; i < dataList.Count; i++)
			{
				for (int j = 0; j < COMSKCommon.YEAR100; j++)
				{
					MiniKumiaiRepairReservePlanDetail detail = draft.Rows[i][j];
					detail.AccountPeriodIndex = j;
					detail.Value = dataList[i].GetValue(j);
				}
			}
		}

		/// <summary>
		/// 案の設定情報を設定する
		/// </summary>
		/// <param name="draft">The draft.</param>
		public void SetDraftSummary(KumiaiRepairReservePlanDraft draft)
		{
			chkUse.Checked = (draft.SelectFlg == COMSKCommon.SELECT_FLG_ON);
			txtDraftName.Text = draft.Name;
			if (draft.FirstRevisionYear != int.MinValue)
			{
				txtFirstRevisionYear.Text = draft.FirstRevisionYear.ToString();
			}
			if (draft.RevisionPitch != int.MinValue)
			{
				txtRevisionPitch.Text = draft.RevisionPitch.ToString();
			}
			if (draft.CostIncreaseBand != double.MinValue)
			{
				txtCostIncreaseBand.Text = string.Format("{0:F3}", draft.CostIncreaseBand);
			}
			if (txtCostIncreaseBand.Text != string.Empty)
			{
				CalcAvgCostIncreaseBand();
			}
		}

		/// <summary>
		/// バリデーションを行う
		/// </summary>
		/// <param name="fillAll">if set to <c>true</c> [fill all].</param>
		/// <returns></returns>
		public bool ValidateControls(bool fillAll)
		{
			bool ret = true;

			ValidationChecker.CheckValidRangeInteger(2000, 3000, txtFirstRevisionYear, errSummary, string.Format(ValidationChecker.WRONG_FORMAT_MESSAGE, "改定初年度"));
			ValidationChecker.CheckValidRangeInteger(0, COMSKCommon.MAX_VISIBLE_YEAR, txtRevisionPitch, errSummary, string.Format(ValidationChecker.WRONG_FORMAT_MESSAGE, "改定ピッチ"));
            ValidationChecker.CheckValidDoubleControl(txtCostIncreaseBand, errSummary, string.Format(ValidationChecker.WRONG_FORMAT_MESSAGE, "単価増額幅"));

			//  fillAll が true なら全ての値を埋めていなければならない
			if (fillAll)
			{
				if (errSummary.GetError(txtFirstRevisionYear) == string.Empty)
				{
					ValidationChecker.CheckRequireControl(txtFirstRevisionYear, errSummary, string.Format(ValidationChecker.REQUIRED_MESSAGE, "改定初年度"));
				}
				if (errSummary.GetError(txtRevisionPitch) == string.Empty)
				{
					ValidationChecker.CheckRequireControl(txtRevisionPitch, errSummary, string.Format(ValidationChecker.REQUIRED_MESSAGE, "改定ピッチ"));
				}
				if (errSummary.GetError(txtCostIncreaseBand) == string.Empty)
				{
                    ValidationChecker.CheckRequireControl(txtCostIncreaseBand, errSummary, string.Format(ValidationChecker.REQUIRED_MESSAGE, "単価増額幅"));
				}
			}

			foreach (Control c in panelHeader.Controls)
			{
				if (errSummary.GetError(c) != string.Empty)
				{
					ret = false;
					break;
				}
			}

			//  OK
			return ret;
		}


		/// <summary>
		/// 資金ショート候補をセットする
		/// </summary>
		/// <param name="codeMst">The code MST.</param>
		public void SetCostShortCodeMst(coms.COMMONService.CodeMst[] codeMst)
		{
			COMSKCommon.SetCodeMstToComboBox(cmbCostShort, codeMst);
		}

		/// <summary>
		/// 指定した年度の積立金累計を更新する
		/// </summary>
		/// <param name="fromIndex">From index.</param>
		/// <param name="toIndex">To index.</param>
		/// <param name="deltaValue">The delta value.</param>
		public void UpdateValue(int fromIndex, int toIndex, long val)
		{
			//  値を更新
			TotalReserveData.SetValue(toIndex, val);

			//  再計算
			calculator.CalcAllReverseMonthly(fromIndex, toIndex);

			//  データを更新
			gcData30Period.Refresh();

            //  累計変更着火
            FireTotalReserveChanged();

			// 手入力リセット
			List<string> lstCode = new List<string>() {
				COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST,
				COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST,
				COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST };
			List<int> numbers = Enumerable.Range(fromIndex, toIndex - fromIndex + 1).ToList();
			this.ResetChangedCells(lstCode, numbers);
		}

		/// <summary>
		/// 指定した年度の積立金累計を更新する (一時金)
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="val">The val.</param>
		public void UpdateValueTemp(int index, long val)
		{
			//  現在の値
			double currValue = TotalReserveData.GetValue(index);

			//  差分を計算
			double diffValue = val - currValue;

			//  更新
			TotalReserveData.SetValue(index, currValue + diffValue);

			//  再計算
			calculator.CalcAllReverseLumpsum(index);

			//  データを更新
			gcData30Period.Refresh();

			//  累計変更着火
			FireTotalReserveChanged();

			// 手入力リセット
			List<string> lstCode = new List<string>() {
				COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST,
				COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST,
				COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST };
			List<int> numbers = new List<int>() { index };
			this.ResetChangedCells(lstCode, numbers);
		}

		/// <summary>
		/// 再計算を行う
		/// </summary>
        public void Recalc(KumiaiLongRepairPlan kumiaiLongRepairPlan, KumiaiTermInfo[] arrTermInfo, KumiaiRepairReservePlanDraft draft, bool autoCalcDiff, bool autoCalc)
		{
			//  自動計算用パラメータを設定
			RepairReservePlanCalculator.AutoCalcInfo info = new RepairReservePlanCalculator.AutoCalcInfo()
			{
				CostIncreaseBand = draft.CostIncreaseBand,
				RevisionPitch = draft.RevisionPitch,
				CostShortCode = draft.CostShortCode,
				FirstRevisionIndex = draft.FirstRevisionYear,
			};

			//  会計年度から会計期インデックスを求める
			int[] accountPeriods = COMSKCommon.GetAccountPeriodFromAccountYear(arrTermInfo, draft.FirstRevisionYear);
			if (accountPeriods.Length > 0)
			{
				info.FirstRevisionIndex = accountPeriods[0] - kumiaiLongRepairPlan.AccountPeriod;

			}

			if (!info.IsValid())
			{
				info = null;
			}

			//  自動計算
            calculator.CalcAll(kumiaiLongRepairPlan, draft, info, autoCalcDiff, autoCalc);

			//  累計工事費変更イベント着火
            FireTotalReserveChanged();
		}

        /// <summary>
        /// タイプ別積立金の再計算を行う
        /// </summary>
        public void ReCalcKumiaiRepairReservePlanTypeDetails(KumiaiRepairReservePlanDraftInfo draftInfo, TypeMst[] lstTypes)
        {
            calculator.ReCalcKumiaiRepairReservePlanTypeDetails(draftInfo, lstTypes,dataList,DraftIndex);

        }

        /// <summary>
        /// タイプ別積立金の再計算を行う
        /// </summary>
        public void ReCalcKumiaiRepairReservePlanTypeDetailsOld(KumiaiRepairReservePlanDraftInfo draftInfo, TypeMst[] lstTypes, string  typeCode)
        {
            calculator.ReCalcKumiaiRepairReservePlanTypeDetailsOld(draftInfo, lstTypes, dataList, DraftIndex,typeCode);

        }

		/// <summary>
		/// 表示モード設定
		/// </summary>
		/// <param name="viewMode">The view mode.</param>
		/// <param name="font">The font.</param>
		public void SetViewMode(COMSKCommon.RepairReserveViewMode viewMode, Font font)
		{
			//  モードを設定
			ViewMode = viewMode;

			//  カラムを設定
			foreach (DataGridViewColumn gc in gcData30Period.Columns)
			{
				gc.DefaultCellStyle.Font = font;
			}

			//  更新
			gcData30Period.Invalidate();
		}

		/// <summary>
		/// コントロール、カラムをリサイズする
		/// </summary>
		/// <param name="chartWidth">Width of the chart.</param>
		public void ResizeControl(int chartWidth, COMSKCommon.RepairReserveViewMode currVM)
		{
			//  比率からカラムの幅を計算
			double columnWidth = calculator.CalcReserveGridColWidth(chartWidth);

			//  セット
			double sumColumnDoubleWidth = 0;
			int sumColumnIntWidth = 0;
			foreach (DataGridViewColumn gc in gcData30Period.Columns)
			{
				//  幅を計算
				sumColumnDoubleWidth += columnWidth;

				//  幅を設定
				int width = (int)(sumColumnDoubleWidth - sumColumnIntWidth);
				gc.Width = width;

				//  整数値幅を足しこむ
				sumColumnIntWidth += width;
			}

			gcData30Period.Width = gcData30Period.Columns
				.Cast<DataGridViewColumn>()
				.Where(col => col.Visible)
				// TODO
				//.Sum(col => col.VisibleWidth) + gcData30Period.IndicatorWidth + calculator.CalcReserveWidthDiff(currVM, true);
				.Sum(col => col.Width) + calculator.CalcReserveWidthDiff(currVM, true);

			// グリッドの左パネル
			this.tableLayoutRoot.ColumnStyles[0].Width = calculator.CalcReserveLeftPartWidth(chartWidth, columnWidth);
		}

		/// <summary>
		/// 決算以外の場合、年度内修繕積立金単価、住戸月額金積立金単価は表示しない
		/// </summary>
		/// <param name="typeCode"></param>
		public void displayChange(string typeCode)
		{
			if (typeCode != COMSKCommon.COMSK_LONG_REPAIR_PLAN_TYPE_ACCOUNT)
			{
				label5.Visible = false;
				txtRepairReserveCost.Visible = false;
				label7.Visible = false;
				label4.Visible = false;
				txtHouseMonthReserveCost.Visible = false;
				label6.Visible = false;
			}
			else
			{
				label5.Visible = true;
				txtRepairReserveCost.Visible = true;
				label7.Visible = true;
				label4.Visible = true;
				txtHouseMonthReserveCost.Visible = true;
				label6.Visible = true;
			}
		}



		#endregion

		#region イベント

		/// <summary>
		/// セルに表示する文字のカスタマイズイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs"/> instance containing the event data.</param>
		private void gcData30Period_CellDisplayTextNeeded(object sender, coms.COMMON.ui.ReserveCellDisplayTextNeededEventArgs e)
		{
			try
			{
				int columnIndex = e.ColumnIndex;
				int rowHandle = e.RowIndex;
				var grid = sender as DataGridView;
				var data = grid?.Rows[rowHandle]?.DataBoundItem as Data30Period;
				if (data == null) return;
				string detailCode = DetailSummary[rowHandle].ReservePlanDetailCode;

				if (columnIndex < COMSKCommon.MAX_VISIBLE_YEAR)
				{
					double val = data.GetValue(columnIndex);

					//専有面積チェック
					bool isAppropriationArea = (data.Name.IndexOf("㎡") >= 0) ? true : false;

					//  特定の行なら
					if ((detailCode != COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST) &&
						(detailCode != COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST) &&
						(detailCode != COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST) &&
						(detailCode != COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST))
					{
						//  表示単位で割る
						val = RoundByViewUnit(val, viewUnit);
					}

					string str = string.Format("{0:D}", (long)val);

					//  標準モードなら
					if (ViewMode == COMSKCommon.RepairReserveViewMode.Standard)
					{
						//  フォーマットタイプで分岐
						if (data.DisplayFormat == Data30Period.Format.Yen)
						{
							//  \ フォーマット
							if (isAppropriationArea)
							{
								str = string.Format("{0}", val);
							}
							else
							{
								str = string.Format("{0:F02}", val);
							}
						}
					}
					//  フルモードなら
					else
					{
						//  フォーマットタイプで分岐
						if (data.DisplayFormat == Data30Period.Format.Comma)
						{
							//  カンマ区切り
							str = string.Format("{0:#,0}", val);
						}
						else if (data.DisplayFormat == Data30Period.Format.Yen)
						{
							//  \ フォーマット
							if (isAppropriationArea)
							{
								str = string.Format("\\{0}", val);
							}
							else
							{
								str = string.Format("\\{0:F02}", val);
							}
						}
					}

					//  最後にセット
					e.DisplayText = str;
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// セル編集可否設定イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		private void gcData30Period_CellBeginEditRule(object sender, coms.COMMON.ui.ReserveCellBeginEditEventArgs e)
		{
			try
			{
				//  行番号
				int rowHandle = e.RowIndex;

				//  サマリから編集可能か調べる
				if (DetailSummary[rowHandle].Editable == true)
				{
					e.Cancel = false;
				}
				else
				{
					e.Cancel = true;
				}
			}
			catch (Exception)
			{
			}
		}

		private void gcData30Period_EditingControlRule(object sender, coms.COMMON.ui.ReserveEditingControlShowingEventArgs e)
		{
			try
			{
				if (e.TextBox != null)
				{
					e.TextBox.MaxLength = 14;
					e.TextBox.ImeMode = ImeMode.Disable;
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// セルのスタイル変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs"/> instance containing the event data.</param>
		private void gcData30Period_CellStyleNeeded(object sender, coms.COMMON.ui.ReserveCellStyleNeededEventArgs e)
		{
			try
			{
				//  カラムインデックスを取得
				int columnIndex = e.ColumnIndex;
				if (0 <= columnIndex && columnIndex < COMSKCommon.MAX_VISIBLE_YEAR)
				{
					//  明細コードを取得
					int rowHandle = e.RowIndex;
					string detailCode = DetailSummary[rowHandle].ReservePlanDetailCode;

					//  明細ごとに分岐
					if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_DIFF_CONSTRUCTION_COST)
					{
						//  工事費差額を取得
						double diffConstructionCost = calculator.DiffConstructionCost.GetValue(columnIndex);

						//  負の数なら
						if (diffConstructionCost < 0)
						{
							e.BackColor = Color.Salmon;
						}
						//  工事費差額最低額を割っていたら
						else if (diffConstructionCost < minDiffRepairCost)
						{
							e.BackColor = Color.Yellow;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TOTAL_RESERVE_COST)
					{
						if (calculator.TotalReserveCost.GetValue(columnIndex) < 0)
						{
							e.BackColor = Color.Salmon;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST)
					{
						if (calculator.YearHouseReserveCost.GetValue(columnIndex) < 0)
						{
							e.BackColor = Color.Salmon;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_REPAIR_RESERVE_COST)
					{
						if (calculator.YearHouseReserveCost.GetValue(columnIndex) < 0)
						{
							e.BackColor = Color.Salmon;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TRANSFAR_COST)
					{
						if (calculator.TransfarCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST)
					{
						if (calculator.HouseMonthReserveCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST)
					{
						if (calculator.HouseSharedMonthReserveCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST)
					{
						if (calculator.HouseLumpsumCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST)
					{
						if (calculator.HouseSharedLumpsumCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_IN_COST)
					{
						if (calculator.OtherInCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_COST)
					{
						if (calculator.OtherOutCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}
					else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_TRANSFAR_COST)
					{
						if (calculator.OtherOutTransfarCost.GetValue(columnIndex) < 0)
						{
							e.ForeColor = Color.Red;
						}
					}

					var cellValChanged = this.IsValueChangedCell(detailCode, columnIndex);
					if (cellValChanged) e.BackColor = Color.LightGoldenrodYellow;
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// コントロールサイズ変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void CtrRepairReservePlan_SizeChanged(object sender, EventArgs e)
		{
			//  推奨高さ
			int height = gcData30Period.RowTemplate.Height * gcData30Period.RowCount + 10 +
				(panelHeader.Margin.Top + panelHeader.Margin.Bottom + 80);

			//  幅はタブの分を差し引いて計算
			int width = this.Width - 15;

			//  移動
			tableLayoutRoot.Location = new Point(0, 0);
			tableLayoutRoot.Size = new Size(width, height);
		}

		/// <summary>
		/// 案名ロストフォーカスイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void txtDraftName_Leave(object sender, EventArgs e)
		{
			//  名前が空白ならデフォルトをセット
			if (txtDraftName.Text.Trim() == string.Empty)
			{
				txtDraftName.Text = string.Format("案{0}", DraftIndex + 1);
			}

			if (DraftNameChanged != null)
			{
				DraftNameChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 再計算ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnRecalc_Click(object sender, EventArgs e)
		{
            preSender = sender;
            preEvent = e;
			//  バリデーション
			if (!ValidateControls(true))
			{
				return;
			}

			// 変更セル履歴をクリア
			this.ResetChangedCells(true);

			//  自動計算情報
			RepairReservePlanCalculator.AutoCalcInfo info = new RepairReservePlanCalculator.AutoCalcInfo()
			{
				RevisionPitch = int.Parse(txtRevisionPitch.Text),
				CostShortCode = COMSKCommon.GetSelectedComboBox(cmbCostShort, "0001"),
				CostIncreaseBand = double.Parse(txtCostIncreaseBand.Text),
				CostShortCodeBefore = this.calcShortBefore
			};

			//  改定初年度
			int firstRevisionYear = int.Parse(txtFirstRevisionYear.Text);

			//  会計期インデックスを求める
			int[] accountPeriods = COMSKCommon.GetAccountPeriodFromAccountYear(TermInfo, firstRevisionYear);
			if (accountPeriods.Length == 0)
			{
				MessageBox.Show("改定初年度が不正です。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			else
			{
				//int accountPeriodIndex = COMSKCommon.GetAccountPeriodIndexFromAccountPeriod(TermInfo, accountPeriods[0]);
				int accountPeriodIndex = accountPeriods[0] - AccountPeriod;

				//  会計期インデックスが範囲外ならエラー
				if (!((0 <= accountPeriodIndex) && (accountPeriodIndex < COMSKCommon.MAX_VISIBLE_YEAR)))
				{
					MessageBox.Show("改定初年度が不正です。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}

				//  開始インデックスをセット
				info.FirstRevisionIndex = accountPeriodIndex;
			}

			//  自動計算実行
			calculator.CalcAuto(info, this.valueChangedCells);

			//  画面を更新
			gcData30Period.Refresh();

			//  累計変更着火
			FireTotalReserveChanged();

			// 最終的前回になる
			this.calcShortBefore = COMSKCommon.GetSelectedComboBox(cmbCostShort, "0001");
		}

		/// <summary>
		/// 単価増額幅テキスト変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void txtCostIncreaseBand_TextChanged(object sender, EventArgs e)
		{
			CalcAvgCostIncreaseBand();
		}

		/// <summary>
		/// 以後反映処理
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void mnuApplyToAll_Click(object sender, EventArgs e)
		{
            preSender = sender;
            preEvent = e;
			//  対象の Data30Period を取得
			Data30Period data = dataList[applyRowIndex];

			//  適用する値
			double val = data.GetValue(applyColumnIndex);

			//  適用
			if (applyDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST)
			{
				//  年額なら別処理
				for (int i = applyColumnIndex + 1; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
				{
					data.SetValue(i, val);

					//  累計を再計算
					calculator.CalcAllUpperYearHouseReserveCost(i);
				}
			}
			else
			{
				for (int i = applyColumnIndex + 1; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
				{
					data.SetValue(i, val);

					//  累計を再計算
					calculator.CalcTotal(i);
				}
			}

			// 変更箇所
			for (int i = applyColumnIndex + 1; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
			{
				this.SetValueChangedCells(applyDetailCode, 0, i);
				if (applyDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST)
                {
					calculator.SetLastYearHouseReserveCost(i);
				}
			}

			//  データ更新
			gcData30Period.Refresh();

			//  累計変更着火
			FireTotalReserveChanged();

		}

		/// <summary>
		/// 使用するチェック変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void chkUse_CheckedChanged(object sender, EventArgs e)
		{
			if (UseChanged != null)
			{
				UseChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 年度内修繕積立金単価変更時に計算
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtRepairReserveCost_TextChanged(object sender, EventArgs e)
		{
			calcHouseMonthReserveCost();
		}

		/// <summary>
		/// 年度内修繕積立金単価変更時に計算
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtRepairReserveCost_Leave(object sender, EventArgs e)
		{
			txtRepairReserveCost.Text = COMSKCommon.EditCommaSeparate(txtRepairReserveCost.Text);
		}

		#endregion

		#region Private

		/// <summary>
		/// ラベルを作成する
		/// </summary>
		private void CreateLabels(KumiaiRepairReservePlanDraft draft)
		{
			//  既存をクリア
			panelForLabels.Controls.Clear();

			//  ラベルの高さ
			int labelHeight = 23 + 1;

			//  Y 位置
			int y = 0;

			//  リストを回す
			for (int i = 0; i < dataList.Count; i++)
			{

				Data30Period data30 = dataList[i];

				//  非表示なら無視
				if (data30.Visible == false)
				{
				    continue;
				}

				//  ラベルを作成
				Label label = new Label()
				{
					Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
					Dock = DockStyle.None,
					Margin = new Padding(0),
					BackColor = Color.White,
					Text = data30.Name,
					TextAlign = ContentAlignment.MiddleLeft,
					Font = new Font("MS UI Gothic", 8),
					Location = new Point(0, y),
					Size = new Size(panelForLabels.Width, labelHeight),
					BorderStyle = BorderStyle.FixedSingle,
				};

				//  パネルに追加
				panelForLabels.Controls.Add(label);

				//  先頭なら
				if (i == 0)
				{
					//  色をバインド
					label.BackColor = COMSKCommon.REPAIR_RESERVE_PLAN_COLORS[draftIndex];
				}

				//  次へ
				y += labelHeight;
			}
		}

		/// <summary>
		/// 明細コードから Data30Period を検索して返す
		/// </summary>
		/// <param name="summary">The summary.</param>
		/// <param name="dataList">The data list.</param>
		/// <param name="detailCode">The detail code.</param>
		/// <returns></returns>
		private Data30Period GetData30Period(KumiaiRepairReservePlanDetailSummary[] summary, List<Data30Period> dataList, string detailCode)
		{
			for (int i = 0; i < summary.Length; i++)
			{
				if (summary[i].ReservePlanDetailCode == detailCode)
				{
					return dataList[i];
				}
			}

			throw new Exception(string.Format("見つかりません (Code={0})", detailCode));
		}

		/// <summary>
		/// 修繕積立金累計の値変更イベントを着火する
		/// </summary>
		private void FireTotalReserveChanged()
		{
			if (TotalReserveChanged != null)
			{
				TotalReserveChanged(this, EventArgs.Empty);
			}
		}

		private void FireScroll()
		{
			if (ScrollChanged != null)
			{
				ScrollChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 明細行の後ろにつけるラベル文字列を判定して返す
		/// </summary>
		/// <param name="detailCode">The detail code.</param>
		/// <param name="viewUnit">The view unit.</param>
		/// <param name="calcDivision">The calc division.</param>
		/// <returns></returns>
		private string GetDetailSuffixLabelFromCode(string detailCode, string viewUnit, string calcDivision)
		{
			string label = string.Empty;

			if ((detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST))
			{
				label = "円／㎡";
			}
			else if ((detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST))
			{
				label = "円／共有持分";
			}
			else
			{
				if (viewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_THOUSAND)
				{
					label = "千円";
				}
				else
				{
					label = "万円";
				}
			}

			if (label == string.Empty)
			{
				return label;
			}
			else
			{
				return "(" + label + ")";
			}
		}

		/// <summary>
		/// 表示単位に応じて val を割って返す
		/// </summary>
		/// <param name="val">The val.</param>
		/// <param name="viewUnit">The view unit.</param>
		/// <returns></returns>
		private double RoundByViewUnit(double val, string viewUnit)
		{
			if (viewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_THOUSAND)
			{
				//  千円単位
				val /= 1000;
			}
			else
			{
				//  万円単位
				val /= 10000;
			}

			//  四捨五入
			return Math.Round(val, MidpointRounding.AwayFromZero);
		}

		/// <summary>
		/// 明細コードから表示フォーマットを返す
		/// </summary>
		/// <param name="detailCode">The detail code.</param>
		/// <returns></returns>
		private Data30Period.Format GetData30PeriodDisplayFormat(string detailCode)
		{
			if ((detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST))
			{
				return Data30Period.Format.Yen;
			}
			else
			{
				return Data30Period.Format.Comma;
			}
		}

		/// <summary>
		/// 明細コードとパラメータからその行を表示するか調べる
		/// </summary>
		/// <param name="detailCode">The detail code.</param>
		/// <param name="kumiaiLongRepairPlan">The kumiai long repair plan.</param>
		/// <returns></returns>
		private bool TestVisible(string detailCode, KumiaiLongRepairPlan kumiaiLongRepairPlan)
		{
			bool ret = true;

            if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_MONTH_RESERVE_COST)
            {
                ret = false;
            }
            else if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_LUMPSUM_COST)
            {
                ret = false;
            }

			//  OK
			return ret;
		}

		/// <summary>
		/// 戸当たり平均増加額を計算
		/// </summary>
		private void CalcAvgCostIncreaseBand()
		{
            //  単価増額幅
			double costIncrBand = Helper.ParseToDouble(txtCostIncreaseBand.Text);
			long avgIncrCost = long.MinValue;
			if (costIncrBand != double.MinValue)
			{
				//  計算
				avgIncrCost = calculator.CalcAvgIncreaseCost(costIncrBand);
			}

			if (avgIncrCost == long.MinValue)
			{
				txtAvgIncreaseCostPerHouse.Text = string.Empty;
			}
			else
			{
				txtAvgIncreaseCostPerHouse.Text = string.Format("{0:N0}", avgIncrCost);
			}
		}

		/// <summary>
		/// 住戸月額積立金単価を計算する
		/// </summary>
		private void calcHouseMonthReserveCost()
		{
            //  単価増額幅
			long repairReserveCost = Helper.ParseToLong(txtRepairReserveCost.Text);
			double houseMonthReserveCost = double.MinValue;
			if (repairReserveCost != long.MinValue)
			{
				//  計算
				houseMonthReserveCost = calculator.CalcHouseMonthReserveCostByYearRepairCost(repairReserveCost);
			}

			if (houseMonthReserveCost == double.MinValue)
			{
				txtHouseMonthReserveCost.Text = string.Empty;
			}
			else
			{
				txtHouseMonthReserveCost.Text = string.Format("{0:F2}", houseMonthReserveCost);
			}
		}


		#endregion


        public void CallPreSender()
        {
            preMode = true;
            if (preSender is Button)
            {
                this.btnRecalc_Click(preSender,preEvent);
            }
            else if (preSender is DataGridView)
            {
                this.gcData30Period_CellValueChanged(preSender, (DataGridViewCellEventArgs)preEvent);
            }
            else if (preSender is System.Windows.Forms.ToolStripMenuItem)
            {
                this.mnuApplyToAll_Click(preSender, preEvent);
            }
            preMode = false;
            preSender = null;
            preEvent = null;
        }

        private void pnScrollDraft_Scroll(object sender, ScrollEventArgs e)
        {
			if (!this.scrollingByParent)
				this.FireScroll();
		}

		private void pnScrollDraft_MouseWheel(object sender, MouseEventArgs e)
		{
			this.FireScroll();
		}

		public int GetScrollPos()
        {
			return -pnScrollDraft.AutoScrollPosition.X;
		}

		public void ScrollByParentPanel(int parentNewPos)
        {
			pnScrollDraft.AutoScrollPosition = new Point(parentNewPos, -pnScrollDraft.AutoScrollPosition.X);
		}

		public void ResetChangedCells(bool keepChanedCell = false)
		{
			if (keepChanedCell)
            {
				var calcShort = COMSKCommon.GetSelectedComboBox(cmbCostShort, "0001");
				List<string> keepChangedCellCode = new List<string>()
				{
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TRANSFAR_COST,
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_IN_COST,
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_COST,
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_TRANSFAR_COST
				};
				// そのままの場合　→　入力した一時金単価がのこして計算します
				if (calcShort == "0001")
                {
					keepChangedCellCode.Add(COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST);
					keepChangedCellCode.Add(COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST);
				}

				var changedCell = new Dictionary<string, List<int>>();
				keepChangedCellCode.ForEach(cellCode =>
				{
					if (this.valueChangedCells.Keys.Contains(cellCode))
					{
						changedCell[cellCode] = this.valueChangedCells[cellCode];
					}
				});
				this.valueChangedCells = changedCell;
			} else
            {
				this.valueChangedCells = new Dictionary<string, List<int>>();
			}
		}

		private void SetValueChangedCells(string detailCode, int rowIdx, int colIdx)
		{
			var needAdd = this.IsValueChangedCheckCell(detailCode);
			if (needAdd)
			{
				if (!this.valueChangedCells.ContainsKey(detailCode)) this.valueChangedCells[detailCode] = new List<int>();
				this.valueChangedCells[detailCode].Add(colIdx);
			}
		}

		private bool IsValueChangedCell(string detailCode, int colIdx)
		{
			return this.valueChangedCells.ContainsKey(detailCode) && this.valueChangedCells[detailCode].Contains(colIdx);
		}

		private bool IsValueChangedCheckCell(string detailCode)
		{
			return this.valueChangedCellCheckCodes.Contains(detailCode);
		}

		public void ResetChangedCells(List<string> keycodes, List<int> indexs)
		{
			foreach (var key in keycodes)
			{
				if (this.valueChangedCells.TryGetValue(key, out var list))
				{
					list.RemoveAll(i => indexs.Contains(i));
				}
			}
		}

		private void gcData30Period_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right) return;
			if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

			var grid = (DataGridView)sender;

			int rowHandle = e.RowIndex;
			string detailCode = DetailSummary[rowHandle].ReservePlanDetailCode;

			bool canEditByCalcDivition = isArea && (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST ||
				detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST);

			if ((detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TRANSFAR_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_IN_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_TRANSFAR_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST) ||
				canEditByCalcDivition ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST) ||
				(detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST))
			{
				applyDetailCode = detailCode;
				applyRowIndex = rowHandle;
				applyColumnIndex = e.ColumnIndex;

				// optional: ensure the clicked cell becomes current (like many grids)
				grid.CurrentCell = grid[e.ColumnIndex, e.RowIndex];

				// Show exactly at mouse cursor position (screen coords)
				cmsApply.Show(Cursor.Position);
			}
		}

		private void gcData30Period_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
			preSender = sender;
			preEvent = e;
			try
			{
				//  現在行の種別を取得
				int rowHandle = e.RowIndex;
				var grid = sender as DataGridView;
				var rowData = grid.Rows[rowHandle].DataBoundItem as coms.COMSK.common.Data30Period;
				string detailCode = rowData?.ReservePlanDetailCode;

				//  行番号を取得
				int index = e.ColumnIndex;

				if (preMode) rowData.SetValue(index, preVal);
				else preVal = rowData.GetValue(index);

				if (!preMode) this.SetValueChangedCells(detailCode, rowHandle, index);

				//  コードで分岐
				List<string> onlyCalcUpperCodes = new List<string>()
				{
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TRANSFAR_COST,
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_IN_COST,
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_COST,
					COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_TRANSFAR_COST
				};

				// 年度内住戸積立金
				if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST)
				{
					calculator.SetLastYearHouseReserveCost(index);
				}

				bool onlyCalcUpper = detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST;

				// その他収入等変更時に年度内住戸積立金既に変更した場合その金額で年度内修繕積立金～積立金累計がその変更した年度内住戸積立金値で計算します
				if (!onlyCalcUpper)
				{
					onlyCalcUpper = this.IsValueChangedCell(COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST, index);
					onlyCalcUpper = onlyCalcUpper && onlyCalcUpperCodes.Contains(detailCode);
				}

				if (onlyCalcUpper)
				{
					//  積立金累計から逆計算
					calculator.CalcAllUpperYearHouseReserveCost(index);

					//  再表示
					grid.Refresh();

					//  イベント着火
					FireTotalReserveChanged();
				}
				else
				{
					// 次年度の年度内住戸積立金が手入力した場合再計算しません
					var nextYearAmountChanged = this.IsValueChangedCell(COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST, index + 1);
					var currYearAmountChanged = this.IsValueChangedCell(COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST, index);

					// 【住戸月額積立金単価】を変更する場合年度内住戸積立金が必ず再計算する
					if (currYearAmountChanged)
					{
						if (detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST ||
							detailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST)
						{
							currYearAmountChanged = false;
							this.ResetChangedCells(
								new List<string>() { COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST },
								new List<int>() { index });
						}
					}

					//  全て再計算
					//calculator.CalcTotal(index);
					calculator.CalcReserveByValueChanged(index, currYearAmountChanged, !(nextYearAmountChanged));

					//  再表示
					grid.Refresh();

					//  チャート更新
					FireTotalReserveChanged();
				}
			}
			catch (Exception)
			{
			}
		}
    }
}
