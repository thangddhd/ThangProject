using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using coms.COMMON;
using coms.COMSK.ui.common;
using coms.COMMONService;
using coms.COMSKService;
using coms.COMSK.common;
using coms.COMSK.business;


namespace coms.COMSK.ui
{
	/// <summary>
	/// 長計作成画面
	/// </summary>
	public partial class K300030020 : MyForm
	{
		#region プロパティ

		/// <summary>
		/// 表示する最大年数
		/// </summary>
		//public const int MAX_VISIBLE_YEAR = 60;
        private bool result;
        private string errorMsg = "";

		/// <summary>
		/// 対象の長計データ
		/// </summary>
		/// <value>
		/// The long repair plan.
		/// </value>
		public KumiaiLongRepairPlan LongRepairPlan { get; set; }

		private int maxTerm = 0;
        
		#endregion

		#region メンバ

		/// <summary>
		/// 対象の長計の PID
		/// </summary>
		private long pid;

		/// <summary>
		/// 組合の PID
		/// </summary>
		private long kumiaiInfoPid = long.MinValue;

		/// <summary>
		/// 変更理由コードマスタ
		/// </summary>
		private CodeMst[] updateReasonCodeMst = null;

		/// <summary>
		/// 100年分の消費税率
		/// </summary>
		private double[] consumptionTaxList = null;

		/// <summary>
		/// 物価上昇率
		/// </summary>
		private double[] priceIncreaseList = null;

		/// <summary>
		/// 会計期オフセット
		/// </summary>
		private int AccountPeriodIndex = 0;

		///// <summary>
		///// 会計期オフセット(現長計会計期起算)
		///// </summary>
		//private int CurrentAccountPeriodIndex = 0;

		/// <summary>
		/// 会計期オフセット (会計期算出用)
		/// </summary>
		private int TermInfoAccountPeriodIndex = 0;

		/// <summary>
		/// 初回取得フラグ
		/// </summary>
		private bool initialFlg = true;

		/// <summary>
		/// 変更フラグ
		/// </summary>
		private bool modifyFlg = false;
		/// <summary>
		/// 基準フォントサイズ
		/// </summary>
		private int? zoomValue = null;
		private bool ignoreZoomChangedEvent = false;
		private int[] zoomLst = new int[] { 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150 };
		private Dictionary<string, bool> dictRecreatedYearlyProcessed = new Dictionary<string, bool>();
		
		#endregion

		#region コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pid">The pid.</param>
		public K300030020(long pid, bool result)
		{
			this.pid = pid;
            this.result = result; //20140822 Linh ADD - MJC_DEV-211

			InitializeComponent();

			this.ctrLongtermPlan_All.SetParentForm(this);
			this.ctrLongtermPlan_Total.SetParentForm(this);
			this.ctrLongtermPlan_Temp.SetParentForm(this);
			this.ctrLongtermPlan_B.SetParentForm(this);
			this.ctrLongtermPlan_E.SetParentForm(this);
			this.ctrLongtermPlan_Out.SetParentForm(this);
			this.ctrLongtermPlan_Others.SetParentForm(this);
		}


		#endregion

		#region Public

		/// <summary>
		/// 実績取込があるかチェック
		/// </summary>
		public void CheckImportStatus()
		{
			try
			{
				K300030BL bl = new K300030BL();
				KumiaiLongRepairPlan data = bl.GetKumiaiLongRepairPlan(this.pid);

				//  作成基準取込があれば
				if (data.HasImportRepairPlanDetail)
				{
					//  強調させる
					EmphasizeButton(btnImportRepairPlan, true && btnRepairPlan.Enabled);
				}
				else
				{
					//  解除
					EmphasizeButton(btnImportRepairPlan, false);
				}

				//  修繕履歴取込があれば
				if (data.HasImportRepairHistory)
				{
					//  強調させる
					EmphasizeButton(btnImportRepairHistory, true && btnRepairHistory.Enabled);
				}
				else
				{
					//  解除
					EmphasizeButton(btnImportRepairHistory, false);
				}

			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		#endregion

		#region Private

		#region 再計算

		/// <summary>
		/// 集計行を再計算する
		/// </summary>
		private void Recalc()
		{
			//  全体の一覧
			RecalcSub(ctrLongtermPlan_All.DataSource);
			RecalcSub_Total(ctrLongtermPlan_Total.DataSource);
			RecalcSub(ctrLongtermPlan_B.DataSource);
			RecalcSub(ctrLongtermPlan_Temp.DataSource);
			RecalcSub(ctrLongtermPlan_E.DataSource);
			RecalcSub(ctrLongtermPlan_Out.DataSource);
			RecalcSub(ctrLongtermPlan_Others.DataSource);

			//  再描画
			RefreshCurrentTab();
		}

		/// <summary>
		/// 集計行再計算 2 (総括表以外)
		/// </summary>
		/// <param name="list">The list.</param>
		private void RecalcSub(List<LongRepairPlanData> list)
		{
			//  割る単位
			string viewUnit = this.LongRepairPlan.MaintenancePlanInfo.ViewUnit;

			//  実際に割る値
			int viewUnitValue = 1000;
			if (viewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_10_THOUSAND)
			{
				viewUnitValue = 10000;
			}

			//  会計期オフセット
			//int termInfoOffset = COMSKCommon.GetAccountPeriodIndexFromAccountPeriod(this.LongRepairPlan.TermInfo, this.LongRepairPlan.CreateAccountPeriod);
			int termInfoOffset = this.LongRepairPlan.AccountPeriod - this.LongRepairPlan.CreateAccountPeriod;

			//  一時保存用リスト
			double[] consumptionTaxList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			double[] priceIncreaseList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			double[] totalList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			double[] subtotalList = new double[LongRepairPlanData.VALID_VALUE_COUNT];

			//  列ごとの総計を出す
			for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
			{
				long subtotalPrice = 0;

				foreach (LongRepairPlanData data in list)
				{
					//  Row == RepairPlan なら
					if ((data.Row == LongRepairPlanData.RowType.RepairPlan))
					{
						//  加算
						subtotalPrice += data.GetValue(i);
					}
				}

				//  小計を保存
				subtotalList[i] = subtotalPrice;

				//  当年の消費税を取得
				double consumptionTaxRate = this.consumptionTaxList[i];

				//  物価上昇率を取得
				double costIncrRate = GetPriceIncreaseRate(termInfoOffset, i - termInfoOffset);

				//  物価上昇金額を計算
				double costIncrCostD = subtotalPrice * costIncrRate;
				priceIncreaseList[i] = costIncrCostD;

				//  消費税を考慮して計算
				double consumptionTaxD = (subtotalPrice + costIncrCostD) * consumptionTaxRate;
				consumptionTaxList[i] = consumptionTaxD;

				//  各タブにセット
				foreach (LongRepairPlanData data in list)
				{
					if (data.Row == LongRepairPlanData.RowType.CalcA)
					{
						data.SetValue(i, RoundToLong((double)subtotalPrice));
					}
					else if (data.Row == LongRepairPlanData.RowType.CalcB)
					{
						data.SetValue(i, RoundToLong(consumptionTaxD));
					}
					else if (data.Row == LongRepairPlanData.RowType.CalcC)
					{
						data.SetValue(i, RoundToLong(costIncrCostD));
					}
					else if (data.Row == LongRepairPlanData.RowType.CalcD)
					{
						totalList[i] = ((double)subtotalPrice + consumptionTaxD + costIncrCostD);
						data.SetValue(i, RoundToLong(totalList[i]));
					}
				}
			}

			//  推定修繕工事費累計を出す
			{
				double sumTotalPrice = 0;
				for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
				{
					int index = AccountPeriodIndex + i;

					if (index < LongRepairPlanData.VALID_VALUE_COUNT)
					{
						//  足しこむ
						sumTotalPrice += totalList[index];

						//  累計行を取得
						LongRepairPlanData data = (from item in list
												   where item.Row == LongRepairPlanData.RowType.CalcE
												   select item).FirstOrDefault();
						if (data != null)
						{
							data.SetValue(index, RoundToLong(sumTotalPrice));
						}
					}
				}
			}

			//  行ごとの総計を出す
			foreach (LongRepairPlanData data in list)
			{
				//  明細行なら
				if (data.Row == LongRepairPlanData.RowType.RepairPlan)
				{
					long subtotalPrice = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{

							long val = data.GetValue(index);

							//  加算
							subtotalPrice += val;
						}
					}

					//  セット
					data.SubTotal = RoundToLong((double)subtotalPrice / viewUnitValue);
				}
				//  小計行なら
				else if (data.Row == LongRepairPlanData.RowType.CalcA)
				{
					double subtotalPrice = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							double val = subtotalList[index];

							//  加算
							subtotalPrice += val;
						}
					}

					//  セット
					data.SubTotal = RoundToLong(subtotalPrice);
				}
				//  消費税率なら
				else if (data.Row == LongRepairPlanData.RowType.CalcB)
				{
					double consumptionTaxCost = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							//  加算
							consumptionTaxCost += consumptionTaxList[index];
						}
					}

					//  設定
					data.SubTotal = RoundToLong(consumptionTaxCost);
				}
				//  物価上昇率なら
				else if (data.Row == LongRepairPlanData.RowType.CalcC)
				{
					double priceIncreaseCost = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;

						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							//  加算
							priceIncreaseCost += priceIncreaseList[index];
						}
					}

					//  設定
					data.SubTotal = RoundToLong(priceIncreaseCost);
				}
				//  CalcD (合計) なら
				else if (data.Row == LongRepairPlanData.RowType.CalcD)
				{
					double subtotalPrice = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							subtotalPrice += totalList[index];
						}
					}

					//  セット
					data.SubTotal = RoundToLong(subtotalPrice);
				}
			}

		}

		/// <summary>
		/// 集計行再計算 2 (総括表)
		/// </summary>
		/// <param name="list">The list.</param>
		private void RecalcSub_Total(List<LongRepairPlanData> list)
		{
			//  割る単位
			string viewUnit = this.LongRepairPlan.MaintenancePlanInfo.ViewUnit;

			//  実際に割る値
			int viewUnitValue = 1000;
			if (viewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_10_THOUSAND)
			{
				viewUnitValue = 10000;
			}

			//  マトリクスを走査し、所属の合計を出す
			for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
			{
				//  全行回す
				foreach (LongRepairPlanData data in list)
				{
					long total = 0;

					//  所属リストを回す
					foreach (LongRepairPlanData belongData in data.BelongsList)
					{
						total += belongData.GetValue(i);
					}

					data.SetValue(i, total);
				}
			}

			//  会計期のオフセット
			//int termInfoOffset = COMSKCommon.GetAccountPeriodIndexFromAccountPeriod(this.LongRepairPlan.TermInfo, this.LongRepairPlan.CreateAccountPeriod);
			int termInfoOffset = this.LongRepairPlan.AccountPeriod - this.LongRepairPlan.CreateAccountPeriod;

			//  一時保存用リスト
			double[] consumptionTaxList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			double[] priceIncreaseList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			double[] totalList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			double[] subtotalList = new double[LongRepairPlanData.VALID_VALUE_COUNT];

			//  列ごとの総計を出す
			for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
			{
				long subtotalPrice = 0;

				foreach (LongRepairPlanData data in list)
				{
					//  Row == GroupItem なら
					if (data.Row == LongRepairPlanData.RowType.GroupItem)
					{
						long val = data.GetValue(i);

						subtotalPrice += val;
					}
				}

				//  小計を保存
				subtotalList[i] = subtotalPrice;

				//  当年の消費税を取得
				double consumptionTaxRate = this.consumptionTaxList[i];

				//  物価上昇率を取得
				double costIncrRate = GetPriceIncreaseRate(termInfoOffset, i - termInfoOffset);

				//  物価上昇金額を計算
				double costIncrCostD = subtotalPrice * costIncrRate;
				priceIncreaseList[i] = costIncrCostD;
				long costIncrCost = RoundToLong(costIncrCostD / viewUnitValue);

				//  消費税を考慮して計算
				double consumptionTaxD = (subtotalPrice + costIncrCostD) * consumptionTaxRate;
				consumptionTaxList[i] = consumptionTaxD;
				long consumptionTax = RoundToLong(consumptionTaxD / viewUnitValue);

				//  各タブにセット
				foreach (LongRepairPlanData data in list)
				{
					if (data.Row == LongRepairPlanData.RowType.CalcA)
					{
						data.SetValue(i, RoundToLong((double)subtotalPrice / viewUnitValue));
					}
					else if (data.Row == LongRepairPlanData.RowType.CalcB)
					{
						data.SetValue(i, consumptionTax);
					}
					else if (data.Row == LongRepairPlanData.RowType.CalcC)
					{
						data.SetValue(i, costIncrCost);
					}
					else if (data.Row == LongRepairPlanData.RowType.CalcD)
					{
						totalList[i] = ((double)subtotalPrice + consumptionTaxD + costIncrCostD);
						data.SetValue(i, (long)RoundToLong(totalList[i] / viewUnitValue));
					}
				}
			}

			//  推定修繕工事費累計を出す
			{
				double sumTotalPrice = 0;
				for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
				{
					int index = AccountPeriodIndex + i;

					if (index < LongRepairPlanData.VALID_VALUE_COUNT)
					{
						//  足しこむ
						sumTotalPrice += totalList[index];

						//  累計行を取得
						LongRepairPlanData data = (from item in list
												   where item.Row == LongRepairPlanData.RowType.CalcE
												   select item).FirstOrDefault();
						if (data != null)
						{
							data.SetValue(index, RoundToLong(sumTotalPrice / viewUnitValue));
						}
					}
				}
			}

			//  行ごとの総計を出す
			foreach (LongRepairPlanData data in list)
			{
				//  GroupItem なら
				if ((data.Row == LongRepairPlanData.RowType.GroupItem) ||
					(data.Row == LongRepairPlanData.RowType.GroupCategory))
				{
					long subtotalPrice = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							long val = data.GetValue(index);

							//  加算
							subtotalPrice += val;
						}
					}

					//  セット
					data.SubTotal = RoundToLong((double)subtotalPrice / viewUnitValue);

				}
				////  GroupCategory なら
				//else if (data.Row == LongRepairPlanData.RowType.GroupCategory)
				//{
				//    double subtotalPrice = 0;

				//    //  年次データを回す
				//    for (int i = 0; i < MAX_VISIBLE_YEAR; i++)
				//    {
				//        int index = AccountPeriodIndex + i;
				//        double val = sumCategoryList[index];

				//        //  加算
				//        subtotalPrice += val;
				//    }

				//    //  セット
				//    data.SubTotal = RoundToLong(subtotalPrice / viewUnitValue);

				//}
				//  小計行なら
				else if (data.Row == LongRepairPlanData.RowType.CalcA)
				{
					double subtotalPrice = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							double val = subtotalList[index];

							//  加算
							subtotalPrice += val;
						}
					}

					//  セット
					data.SubTotal = RoundToLong((double)subtotalPrice / viewUnitValue);

				}
				//  消費税率なら
				else if (data.Row == LongRepairPlanData.RowType.CalcB)
				{
					double consumptionTaxCost = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							//  加算
							consumptionTaxCost += consumptionTaxList[index];
						}
					}

					//  設定
					data.SubTotal = RoundToLong(consumptionTaxCost / viewUnitValue);
				}
				//  物価上昇率なら
				else if (data.Row == LongRepairPlanData.RowType.CalcC)
				{
					double priceIncreaseCost = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							//  加算
							priceIncreaseCost += priceIncreaseList[index];
						}
					}

					//  設定
					data.SubTotal = RoundToLong(priceIncreaseCost / viewUnitValue);
				}
				//  CalcD (合計) なら
				else if (data.Row == LongRepairPlanData.RowType.CalcD)
				{
					double subtotalPrice = 0;

					//  年次データを回す
					for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
					{
						int index = AccountPeriodIndex + i;
						if (index < LongRepairPlanData.VALID_VALUE_COUNT)
						{
							subtotalPrice += totalList[index];
						}
					}

					//  セット
					data.SubTotal = RoundToLong(subtotalPrice / viewUnitValue);
				}
			}

		}

		/// <summary>
		/// 物価上昇率を取得する
		/// </summary>
		/// <param name="accountPeriodIndex">Index of the account period.</param>
		/// <param name="count">The count.</param>
		/// <returns></returns>
		private double GetPriceIncreaseRate(int accountPeriodIndex, int count)
		{
			double ret = 0.0;
			double priceIncrRate = 1.0;

			for (int i = 0; i <= count; i++)
			{
				if (accountPeriodIndex + i < LongRepairPlanData.VALID_VALUE_COUNT)
					priceIncrRate *= (1.0 + this.priceIncreaseList[accountPeriodIndex + i]);
			}

			//  基準の 1 を引く
			ret = priceIncrRate - 1;

			////  accountPeriodIndex から表示インデックスをさし引き、左端からのインデックスとする
			//int periodIndex = COMSKCommon.GetAccountPeriodIndexFromAccountPeriod(this.LongRepairPlan.TermInfo, this.LongRepairPlan.AccountPeriod);
			//int index = accountPeriodIndex - this.AccountPeriodIndex;
			//int index2 = accountPeriodIndex + this.CurrentAccountPeriodIndex - this.AccountPeriodIndex;

			////  表示範囲内なら
			//if ((0 <= index) && (index < MAX_VISIBLE_YEAR))
			//{
			//    //  物価上昇率初期値
			//    double priceIncrRate = 1.0;
				
			//    for (int i = 0; i <= index2; i++)
			//    {
			//        if (i >= LongRepairPlanData.VALID_VALUE_COUNT)
			//        {
			//            break;
			//        }
			//        //  物価上昇率をかける
			//        priceIncrRate *= (1.0 + this.priceIncreaseList[periodIndex + i]);
			//    }

			//    //  基準の 1 を引く
			//    ret = priceIncrRate - 1.0;
			//}

			//  OK
			return ret;
		}

		#endregion

		/// <summary>
		/// 表示開始年を設定する
		/// </summary>
		/// <param name="startAccountPeriod">The start account period.</param>
		private void ChangeStartYear(int startAccountPeriod)
		{
			try
			{
				//  ウェイトカーソル
				Cursor.Current = Cursors.WaitCursor;

				//  年数を設定
				//CurrentAccountPeriodIndex = startAccountPeriod - LongRepairPlan.AccountPeriod;
				AccountPeriodIndex = startAccountPeriod - LongRepairPlan.CreateAccountPeriod;

				// 作成開始年度での位置
				var adjustIdx = LongRepairPlan.CreateAccountPeriod - this.LongRepairPlan.TermInfo[0].Term;
				//var adjustIdx = 0;
				var originalAccountPeriodIndex = this.LongRepairPlan.AccountPeriod - LongRepairPlan.CreateAccountPeriod + adjustIdx;

				//  会計期の先頭からのオフセット
				TermInfoAccountPeriodIndex = startAccountPeriod - this.LongRepairPlan.TermInfo[0].Term;
				ctrLongtermPlan_All.CreateYearColumns(TermInfoAccountPeriodIndex, AccountPeriodIndex, COMSKCommon.MAX_VISIBLE_YEAR, this.LongRepairPlan.TermInfo, originalAccountPeriodIndex);
				ctrLongtermPlan_Total.CreateYearColumns(TermInfoAccountPeriodIndex, AccountPeriodIndex, COMSKCommon.MAX_VISIBLE_YEAR, this.LongRepairPlan.TermInfo, originalAccountPeriodIndex);
				ctrLongtermPlan_Temp.CreateYearColumns(TermInfoAccountPeriodIndex, AccountPeriodIndex, COMSKCommon.MAX_VISIBLE_YEAR, this.LongRepairPlan.TermInfo, originalAccountPeriodIndex);
				ctrLongtermPlan_B.CreateYearColumns(TermInfoAccountPeriodIndex, AccountPeriodIndex, COMSKCommon.MAX_VISIBLE_YEAR, this.LongRepairPlan.TermInfo, originalAccountPeriodIndex);
				ctrLongtermPlan_E.CreateYearColumns(TermInfoAccountPeriodIndex, AccountPeriodIndex, COMSKCommon.MAX_VISIBLE_YEAR, this.LongRepairPlan.TermInfo, originalAccountPeriodIndex);
				ctrLongtermPlan_Out.CreateYearColumns(TermInfoAccountPeriodIndex, AccountPeriodIndex, COMSKCommon.MAX_VISIBLE_YEAR, this.LongRepairPlan.TermInfo, originalAccountPeriodIndex);
				ctrLongtermPlan_Others.CreateYearColumns(TermInfoAccountPeriodIndex, AccountPeriodIndex, COMSKCommon.MAX_VISIBLE_YEAR, this.LongRepairPlan.TermInfo, originalAccountPeriodIndex);

				//  再計算
				Recalc();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			finally
			{
				Cursor.Current = Cursors.Arrow;
			}
		}

		/// <summary>
		/// セルドラッグ処理
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="updateLator">if set to <c>true</c> [update lator].</param>
		private void MoveCells(List<LongRepairPlanData> list, int fromColumn, int toColumn, bool updateLator)
		{
			//  後方か前方かで分岐
			if (toColumn > fromColumn)
			{
				//  ■  後方へ
				int deltaOffset = toColumn - fromColumn;

				//  全行に対してまわす
				List<LongRepairPlanData> exceptList = new List<LongRepairPlanData>();
				foreach (LongRepairPlanData info in list)
				{
					//  Row が RepairPlan でなければ無視
					if (info.Row != LongRepairPlanData.RowType.RepairPlan)
					{
						continue;
					}

					//  適用する値
					long val = 0;
					{
						object _val = info.GetValue(fromColumn);
						if (_val is long)
						{
							val = (long)_val;
						}
					}

					//  0 なら何もしない
					if (val == 0)
					{
						//  除外リストに追加
						exceptList.Add(info);
						continue;
					}

					//  値をコピー
					info.SetValue(toColumn, val);

					//  fromColumn ～ toColumn を 0 クリア
					//  加えてドラッグフラグを付加
					for (int i = 0; i < deltaOffset; i++)
					{
						info.SetValue(fromColumn + i, 0);
						info.SetDragged(fromColumn + i, COMSKCommon.DRAGGED_FLG_ON);
					}
					info.SetDragged(fromColumn + deltaOffset, COMSKCommon.DRAGGED_FLG_ON);
					info.PrevValue = 1;

					//  周期再計算
					RecalcPeriodForward(info, fromColumn, toColumn, deltaOffset, val, updateLator);

				}

				//  適用されなかったデータを除外する
				List<LongRepairPlanData> validList = list.Except(exceptList).ToList();

				//  list 内にある項目を親に持つ子工事を全て移動させる
				foreach (LongRepairPlanData info in this.ctrLongtermPlan_All.DataSource)
				{
					//  親指定があれば
					if (info.ParentPid != long.MinValue)
					{
						//  親は一つしかいないはず
						LongRepairPlanData parentData = (from item in validList
														 where item.Pid == info.ParentPid
														 select item).FirstOrDefault();

						//  親を見つけたら
						if (parentData != null)
						{
							//  長計用金額
							long val = COMSKCommon.RoundUpToLongRepairPlanImportCost(info.Cost, info.Amount);

							//  周期を再計算
							RecalcPeriodForward(info, fromColumn, toColumn, deltaOffset, val, updateLator);

							//  加えて、toColumn 位置をクリア
                            if (info.GetValue(toColumn) != 0) ChildDataDeleted(info, toColumn);
							info.SetDragged(toColumn, COMSKCommon.DRAGGED_FLG_ON);
							info.PrevValue = 1;

							//  結果リストに追加
							if (list.Contains(info) == false)
							{
								list.Add(info);
							}
						}
					}
				}

			}
			else
			{
				//  ■  前方へ
				int deltaOffset = fromColumn - toColumn;

				//  全行に対してまわす
				List<LongRepairPlanData> exceptList = new List<LongRepairPlanData>();
				foreach (LongRepairPlanData info in list)
				{
					//  DataType が Child でなければ無視
					if (info.Row != LongRepairPlanData.RowType.RepairPlan)
					{
						continue;
					}

					//  適用する値
					long val = 0;
					{
						object _val = info.GetValue(fromColumn);
						if (_val is long)
						{
							val = (long)_val;
						}
					}

					//  0 なら何もしない
					if (val == 0)
					{
						//  除外リストに追加
						exceptList.Add(info);
						continue;
					}

					//  値をコピー
					info.SetValue(toColumn, val);

					//  toColumn ～ fromColumn を 0 クリア
					//  加えてドラッグフラグを付加
					for (int i = 0; i < deltaOffset; i++)
					{
						info.SetValue(toColumn + i + 1, 0);
						info.SetDragged(toColumn + i, COMSKCommon.DRAGGED_FLG_ON);
					}
					info.SetDragged(toColumn + deltaOffset, COMSKCommon.DRAGGED_FLG_ON);
					info.PrevValue = 1;

					//  後ろを反映させる
					RecalcPeriodBackward(info, fromColumn, toColumn, deltaOffset, val, updateLator);

				}

				//  適用されなかったデータを除外する
				List<LongRepairPlanData> validList = list.Except(exceptList).ToList();

				//  list 内にある項目を親に持つ子工事を全て移動させる
				foreach (LongRepairPlanData info in this.ctrLongtermPlan_All.DataSource)
				{
					//  親指定があれば
					if (info.ParentPid != long.MinValue)
					{
						//  親は一つしかいないはず
						LongRepairPlanData parentData = (from item in validList
														 where item.Pid == info.ParentPid
														 select item).FirstOrDefault();

						//  親を見つけたら
						if (parentData != null)
						{
							//  長計用金額
							long val = COMSKCommon.RoundUpToLongRepairPlanImportCost(info.Cost, info.Amount);

							//  周期を再計算
							RecalcPeriodBackward(info, fromColumn, toColumn, deltaOffset, val, updateLator);

							//  加えて、toColumn 位置をクリア
                            if (info.GetValue(toColumn) != 0) ChildDataDeleted(info, toColumn);
							info.SetDragged(toColumn, COMSKCommon.DRAGGED_FLG_ON);
							info.PrevValue = 1;

							//  結果リストに追加
							if (list.Contains(info) == false)
							{
								list.Add(info);
							}
						}
					}
				}

			}
            ShowDialogChildDataDeleted();

		}

		/// <summary>
		/// 親工事と重なる子工事を削除する
		/// </summary>
		/// <param name="list">The list.</param>
		private void DeleteIsOnTopData(List<LongRepairPlanData> list)
		{
            
			//  列挙
			foreach (LongRepairPlanData itrData in this.ctrLongtermPlan_All.DataSource)
			{
				//  子工事なら
                if (itrData.ParentPid != long.MinValue && itrData.ParentPid != 0)
				{
					//  親は一つしかいないはず
					LongRepairPlanData parentData = (from item in list
													 where item.Pid == itrData.ParentPid
													 select item).FirstOrDefault();

					//  親を見つけたら
					if (parentData != null)
					{
						//  回す
						for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
						{
							//  親の年次データの数字が 0 でなければ
							if (parentData.GetValue(i) != 0 && itrData.GetValue(i) != 0)
							{
                                ChildDataDeleted(itrData,i);
							}
						}

					}
				}
			}
		}

        

		/// <summary>
		/// 周期を再計算する (前方)
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="fromColumn">From column.</param>
		/// <param name="toColumn">To column.</param>
		/// <param name="deltaOffset">The delta offset.</param>
		/// <param name="val">The val.</param>
		/// <param name="updateLator">if set to <c>true</c> [update lator].</param>
		private void RecalcPeriodForward(LongRepairPlanData data, int fromColumn, int toColumn, int deltaOffset, long val, bool updateLator)
		{
			//  後ろを反映させる
			if (updateLator)
			{
				data.RecalcPeriod(toColumn, val, this.maxTerm);
			}
			else
			{
				//  反映範囲を更新
				for (int i = 0; i <= deltaOffset; i++)
				{
					data.SetDragged(fromColumn + i, COMSKCommon.DRAGGED_FLG_ON);
				}
			}
		}

		/// <summary>
		/// 周期を再計算する (前方)
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="fromColumn">From column.</param>
		/// <param name="toColumn">To column.</param>
		/// <param name="deltaOffset">The delta offset.</param>
		/// <param name="val">The val.</param>
		/// <param name="updateLator">if set to <c>true</c> [update lator].</param>
		private void RecalcPeriodBackward(LongRepairPlanData data, int fromColumn, int toColumn, int deltaOffset, long val, bool updateLator)
		{
			//  後ろを反映させる
			if (updateLator)
			{
				data.RecalcPeriod(toColumn, val, this.maxTerm);
			}
			else
			{
				for (int i = 0; i <= deltaOffset; i++)
				{
					data.SetDragged(toColumn + i, COMSKCommon.DRAGGED_FLG_ON);
				}
			}
		}

		/// <summary>
		/// 明細を読み込む
		/// </summary>
		private void LoadDetails()
		{
			try
			{
				//  明細一覧を取得
				K300030BL business = new K300030BL();
				List<KumiaiLongRepairPlanDetail> list = business.GetAllKumiaiLongRepairPlanDetails(LongRepairPlan.Pid, Helper.loginUserInfo.Pid, initialFlg);
				List<LongRepairPlanData> convertedList = LongRepairPlanData.FromKumiaiLongRepairPlanDetails(list);
                //20140822 Linh ADD - MJC_DEV-211
                K300030020 frm = new K300030020(pid, result);
                if (frm.result)
                {
                    this.btnUpdate.Enabled = false;
                    this.btnRepairPlan.Enabled = false;
                }//END - MJC_DEV-211

				//  データソースに設定
                ctrLongtermPlan_All.result = frm.result;
				ctrLongtermPlan_All.DataSource = convertedList;
				ctrLongtermPlan_Temp.DataSource = convertedList;
				ctrLongtermPlan_B.DataSource = convertedList;
				ctrLongtermPlan_E.DataSource = convertedList;
				ctrLongtermPlan_Out.DataSource = convertedList;
				ctrLongtermPlan_Others.DataSource = convertedList;
				ctrLongtermPlan_Total.DataSource = convertedList;

				//  子工事を潰す
				DeleteIsOnTopData(convertedList);

				//NOTE: CellValueChanged は、デザイン時に張ると画面表示時に負荷が掛かるので、
				//  初期値を設定し終わった後に手動で張る
				ctrLongtermPlan_All.CellValueChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_CellValueChanged);
				ctrLongtermPlan_Temp.CellValueChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_CellValueChanged);
				ctrLongtermPlan_B.CellValueChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_CellValueChanged);
				ctrLongtermPlan_E.CellValueChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_CellValueChanged);
				ctrLongtermPlan_Out.CellValueChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_CellValueChanged);
				ctrLongtermPlan_Others.CellValueChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_CellValueChanged);

				//  その他タブも
				ctrLongtermPlan_All.EffectedYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_EffectedYearChanged);
                ctrLongtermPlan_All.DatumYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_DatumYearChanged);
				ctrLongtermPlan_Temp.EffectedYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_EffectedYearChanged);
                ctrLongtermPlan_Temp.DatumYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_DatumYearChanged);
				ctrLongtermPlan_B.EffectedYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_EffectedYearChanged);
                ctrLongtermPlan_B.DatumYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_DatumYearChanged);
				ctrLongtermPlan_E.EffectedYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_EffectedYearChanged);
                ctrLongtermPlan_E.DatumYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_DatumYearChanged);
				ctrLongtermPlan_Out.EffectedYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_EffectedYearChanged);
                ctrLongtermPlan_Out.DatumYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_DatumYearChanged);
				ctrLongtermPlan_Others.EffectedYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_EffectedYearChanged);
                ctrLongtermPlan_Others.DatumYearChanged += new coms.COMSK.ui.common.LongRepairPlanDataEventHandler(ctrLongtermPlan_DatumYearChanged);

				//  表示開始年を設定
				ChangeStartYear(LongRepairPlan.AccountPeriod);

				// 30年度の列が再生成されたの為
				this.UpdateDictRecreatedYearlyProcessed("", true);

				//  集計行を再計算
				Recalc();

				// 拡大処理
				this.DoZoomSubTab();

				//  初回取得フラグを折る
				initialFlg = false;

				//  変更フラグを OFF
				modifyFlg = false;
                ShowDialogChildDataDeleted();
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// ボタンの強調を設定、または解除する
		/// </summary>
		/// <param name="btn">The BTN.</param>
		/// <param name="b">if set to <c>true</c> [b].</param>
		private void EmphasizeButton(Button btn, bool b)
		{
			if (b == true)
			{
				btn.Enabled = true;
				btn.BackColor = Color.Red;
				btn.ForeColor = Color.White;
			}
			else
			{
				btn.Enabled = false;
				btn.BackColor = Color.White;
				btn.ForeColor = Color.Black;
			}
		}

		/// <summary>
		/// 消費税率を計算する
		/// </summary>
		/// <param name="taxList">The tax list.</param>
		private void CreateConsumptionTaxList(List<KumiaiLongRepairPlanTaxMst> taxList)
		{
			this.consumptionTaxList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			int accountPeriodIndex = this.LongRepairPlan.CreateAccountPeriod - this.LongRepairPlan.TermInfo[0].Term;
			for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
			{
				this.consumptionTaxList[i] = COMSKCommon.GetConsumptionTaxRateFromList(this.LongRepairPlan.TermInfo, taxList, accountPeriodIndex + i);
			}
		}

		/// <summary>
		/// 物価上昇率を計算する
		/// </summary>
		/// <param name="taxList">The tax list.</param>
		private void CreatePriceIncreaseList(List<KumiaiLongRepairPlanTaxMst> taxList)
		{
			this.priceIncreaseList = new double[LongRepairPlanData.VALID_VALUE_COUNT];
			int accountPeriodIndex = this.LongRepairPlan.CreateAccountPeriod - this.LongRepairPlan.TermInfo[0].Term;
			for (int i = 0; i < LongRepairPlanData.VALID_VALUE_COUNT; i++)
			{
				this.priceIncreaseList[i] = COMSKCommon.GetCostIncreaseRateFromList(this.LongRepairPlan.TermInfo, taxList, accountPeriodIndex + i);
			}
		}

		/// <summary>
		/// 会計期コンボボックスに会計期をセットする
		/// </summary>
		/// <param name="accountPeriods">The account periods.</param>
		private void SetAccountPeriodList(int[] accountPeriods)
		{
			cmbAccountPeriod.Items.Clear();

			foreach (int accountPeriod in accountPeriods)
			{
				//  追加
				TypeValue<int> val = new TypeValue<int>(accountPeriod,
					string.Format("{0}期", accountPeriod));
				cmbAccountPeriod.Items.Add(val);
			}

			cmbAccountPeriod.SelectedIndex = 0;
		}

		/// <summary>
		/// 現在のタブを再描画する
		/// </summary>
		private void RefreshCurrentTab(bool tabChanged = false)
		{
			try
			{
				int rate = this.zoomValue.HasValue ? this.zoomValue.Value : 100;
				var keyCtr = xtraTabControl_LongRepairPlan.SelectedTabPage.Controls[0].Name;
				bool processByRecreatedYearly = this.dictRecreatedYearlyProcessed[keyCtr];

				if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_All)
				{
					ctrLongtermPlan_All.RefreshData();
					if (tabChanged)
                    {
						ctrLongtermPlan_All.ClearSelectedRow();
						if (this.zoomValue.HasValue)
						{
							ctrLongtermPlan_All.SetZoom(rate, processByRecreatedYearly);
						}
					}
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Total)
				{
					ctrLongtermPlan_Total.RefreshData();
					if (this.zoomValue.HasValue && tabChanged)
					{
						ctrLongtermPlan_Total.SetZoom(rate, processByRecreatedYearly);
					}
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_TempConstruction)
				{
					ctrLongtermPlan_Temp.RefreshData();
					if (tabChanged)
                    {
						ctrLongtermPlan_Temp.ClearSelectedRow();
						if (this.zoomValue.HasValue)
						{
							ctrLongtermPlan_Temp.SetZoom(rate, processByRecreatedYearly);
						}
					}
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Building)
				{
					ctrLongtermPlan_B.RefreshData();
					if (tabChanged)
                    {
						ctrLongtermPlan_B.ClearSelectedRow();
						if (this.zoomValue.HasValue)
						{
							ctrLongtermPlan_B.SetZoom(rate, processByRecreatedYearly);
						}
					}
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Equipment)
				{
					ctrLongtermPlan_E.RefreshData();
					if (tabChanged)
                    {
						ctrLongtermPlan_E.ClearSelectedRow();
						if (this.zoomValue.HasValue)
						{
							ctrLongtermPlan_E.SetZoom(rate, processByRecreatedYearly);
						}
					}
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_OutwardAppearance)
				{
					ctrLongtermPlan_Out.RefreshData();
					if (tabChanged)
                    {
						ctrLongtermPlan_Out.ClearSelectedRow();
						if (this.zoomValue.HasValue)
						{
							ctrLongtermPlan_Out.SetZoom(rate, processByRecreatedYearly);
						}
					}
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Others)
				{
					ctrLongtermPlan_Others.RefreshData();
					if (tabChanged)
                    {
						ctrLongtermPlan_Others.ClearSelectedRow();
						if (this.zoomValue.HasValue)
						{
							ctrLongtermPlan_Others.SetZoom(rate, processByRecreatedYearly);
						}
					}
				}

				if (!string.IsNullOrEmpty(keyCtr)) this.UpdateDictRecreatedYearlyProcessed(keyCtr, false);
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// 累計工事費を取得する
		/// </summary>
		/// <returns></returns>
		private long[] GetTotalRepairCost()
		{
			//  戻りデータ
			long[] ret = new long[COMSKCommon.YEAR100];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = 0;
			}

			//  積立金累計額一覧
			LongRepairPlanData data = (from item in ctrLongtermPlan_All.DataSource
									   where item.Row == LongRepairPlanData.RowType.CalcE
									   select item).FirstOrDefault();

			if (data != null)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					//  会計期インデックスを考慮してデータを設定
					if (AccountPeriodIndex + i < LongRepairPlanData.VALID_VALUE_COUNT)  // AccountPeriodIndex + i=期目
						ret[i] = data.GetValue(AccountPeriodIndex + i);
				}
			}

			return ret;
		}

		/// <summary>
		/// 正しく四捨五入する
		/// </summary>
		/// <param name="val">The val.</param>
		/// <returns></returns>
		private long RoundToLong(double val)
		{
			return (long)Math.Round(val, MidpointRounding.AwayFromZero);
		}


		#endregion

		#region イベント

		#region 雑多

		/// <summary>
		/// フォームロードイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void K200010030_Load(object sender, EventArgs e)
		{
			try
			{
				//  権限設定
				SetAuthorities();

				//  コードマスタを引いてくる
				updateReasonCodeMst = COMSKCommon.GetUpdateReasonCodeMst();

				//  長計サマリーを引いてくる
				K300030BL business = new K300030BL();
				this.LongRepairPlan = business.GetKumiaiLongRepairPlan(this.pid);
				if (this.LongRepairPlan == null)
				{
					throw new Exception("長計データを正しく取得できませんでした。");
				}
				this.kumiaiInfoPid = this.LongRepairPlan.KumiaiInfoPid;
				this.maxTerm = this.LongRepairPlan.TermInfo.Max(t => t.Term);

				//  プロパティをセット
				txtKumiai.Text = LongRepairPlan.KumiaiName;
				txtLongRepairPlanName.Text = LongRepairPlan.Name;
				if (LongRepairPlan.NextNotifyDate != DateTime.MinValue)
				{
					txtNextNotifyDate.Text = LongRepairPlan.NextNotifyDate.ToString("yyyy年MM月dd日");
				}
				txtPresentationType.Text = LongRepairPlan.PresentationName;

				if (LongRepairPlan.AccountMonth != int.MinValue)
				{
					txtAccountMonth.Text = string.Format("{0}月", LongRepairPlan.AccountMonth);
				}
				txtType.Text = LongRepairPlan.TypeName;
				txtStatus.Text = LongRepairPlan.StatusName;

				if (LongRepairPlan.MaintenancePlanInfo.ViewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_THOUSAND)
				{
					//  千円
					lblViewUnit.Text = "表示単位：千円";
				}
				else
				{
					//  万円
					lblViewUnit.Text = "表示単位：万円";
				}

				//  長計設定を設定
				ctrLongtermPlan_All.MntPlanConst = this.LongRepairPlan.MaintenancePlanInfo;
				ctrLongtermPlan_Total.MntPlanConst = this.LongRepairPlan.MaintenancePlanInfo;
				ctrLongtermPlan_Temp.MntPlanConst = this.LongRepairPlan.MaintenancePlanInfo;
				ctrLongtermPlan_B.MntPlanConst = this.LongRepairPlan.MaintenancePlanInfo;
				ctrLongtermPlan_E.MntPlanConst = this.LongRepairPlan.MaintenancePlanInfo;
				ctrLongtermPlan_Out.MntPlanConst = this.LongRepairPlan.MaintenancePlanInfo;
				ctrLongtermPlan_Others.MntPlanConst = this.LongRepairPlan.MaintenancePlanInfo;

				//  基準年
				{
					//  基準年を引いてくる
					int accountYear = COMSKCommon.GetAccountYearFromAccountPeriod(this.LongRepairPlan.TermInfo, LongRepairPlan.AccountPeriod);

					//  設定
					txtDatumYear.Text = accountYear.ToString();
				}

				//  消費税率・物価上昇率を引いてくる
				List<KumiaiLongRepairPlanTaxMst> taxList = business.GetConsumptionTaxList(this.LongRepairPlan.Pid);
				CreateConsumptionTaxList(taxList);
				taxList = business.GetPriceIncreaseList(this.LongRepairPlan.Pid);
				CreatePriceIncreaseList(taxList);


				//  凡例を設定
				lblColorModifiedAtLongtermRepairPlan.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_MODIFIED_AT_LONGTERM_REPAIR_PLAN;
				lblColorDragged.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_DRAGGED;
				lblColorImportRepairPlan.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_PLAN;
				lblColorImportRepairHistory.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_HISTORY;
				lblColorNoReason.BackColor = COMSKCommon.LONGTERM_REPAIR_PLAN_COLOR_NO_REASON;


				//  長計データをロード
				LoadDetails();

				//  取込チェック
				CheckImportStatus();

				//  登録
				FormMngr.Instance.RegisterKumiaiLongRepairPlanP(this.pid, this);
				FormMngr.Instance.RegisterKumiaiLongRepairPlanH(this.kumiaiInfoPid, this);

                //MJC_DEV-227
                if (this.LongRepairPlan.IntensiveFlg == "0010")//集約
                {
                    ctrLongtermPlan_All.SetDisable();
                    ctrLongtermPlan_Total.Editable = false;
                    ctrLongtermPlan_Temp.SetDisable();
                    ctrLongtermPlan_B.SetDisable();
                    ctrLongtermPlan_E.SetDisable();
                    ctrLongtermPlan_Out.SetDisable();
                    ctrLongtermPlan_Others.SetDisable();
					btnTaxRate.Enabled = false;//Diem add https://reci.backlog.jp/view/MJC_DEV-232
					
                }
                this.Text += "_" + LongRepairPlan.Name;

				this.SetZoomList();
				this.UpdateDictRecreatedYearlyProcessed("", false);
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.Close();
			}
		}

		/// <summary>
		/// フォームクロージング
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		private void K200010030_FormClosing(object sender, FormClosingEventArgs e)
		{
			ctrLongtermPlan_Temp.Close();
			ctrLongtermPlan_B.Close();
			ctrLongtermPlan_E.Close();
			ctrLongtermPlan_Out.Close();
			ctrLongtermPlan_Others.Close();
			ctrLongtermPlan_Total.Close();

			//  登録解除
			FormMngr.Instance.UnregisterKumiaiLongRepairPlanP(this.pid, this);
			FormMngr.Instance.UnregisterKumiaiLongRepairPlanH(this.kumiaiInfoPid, this);
		}

		/// <summary>
		/// タブ変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraTab.TabPageChangedEventArgs"/> instance containing the event data.</param>
		private void xtraTabControl_LongRepairPlan_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
		{
			RefreshCurrentTab(true);
		}

		/// <summary>
		/// 表示年変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void txtDatumYear_TextChanged(object sender, EventArgs e)
		{
			bool ok = false;
			string s = txtDatumYear.Text.Trim();

			//  4 桁なら
			if (s.Length == 4)
			{
				//  int にできれば
				int datumYear = 0;
				if (int.TryParse(s, out datumYear))
				{
					//  年度から会計期リストを取得する
					int[] accountPeriods = COMSKCommon.GetAccountPeriodFromAccountYear(this.LongRepairPlan.TermInfo, datumYear);

					//  あれば
					if (accountPeriods.Length > 1)
					{
						//  会計期を設定
						SetAccountPeriodList(accountPeriods);

						//  表示
						cmbAccountPeriod.Visible = true;

						//  OK
						ok = true;
					}
				}
			}

			//  失敗なら
			if (ok == false)
			{
				//  非表示
				cmbAccountPeriod.Visible = false;
			}
		}

		#endregion

		#region ボタン操作

		/// <summary>
		/// 長計出力ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnOutputLongtermRepairPlan_Click(object sender, EventArgs e)
		{
			//  ウェイトカーソル
			this.Cursor = Cursors.WaitCursor;

			try
			{
				K300030BL business = new K300030BL();

				//  作業用データからDB用データに変換
				List<LongRepairPlanData> list = ctrLongtermPlan_Total.DataSource;
				list = (from item in list
						where item.Row == LongRepairPlanData.RowType.GroupItem
						select item).ToList();
				List<KumiaiLongRepairPlanDetail> convertedList = LongRepairPlanData.ToKumiaiLongRepairPlanDetails(list);

				// サービス
				COMSKService.FileEntry fileEntry = business.ExportLongRepairPlanSummary(convertedList, pid);

				//  ファイル保存
				Helper.SaveFile(fileEntry, this);
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			finally
			{
				this.Cursor = Cursors.Arrow;
			}
		}

		/// <summary>
		/// 修繕計画出力ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnOutputRepairPlan_Click(object sender, EventArgs e)
		{
            // MOD 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↓
            //K300030026 frm = new K300030026();
            //
            //  長計データを引いてくる
            K300030BL business = new K300030BL();
            KumiaiLongRepairPlan tempData = business.GetKumiaiLongRepairPlan(this.LongRepairPlan.Pid);
            //
            K300030026 frm = new K300030026(tempData.TypeCode);
            // MOD 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↑
            if (frm.ShowDialog() == DialogResult.OK)
			{
				//  ウェイトカーソル
				this.Cursor = Cursors.WaitCursor;

				// 長計出力
				try
                {
                    // DEL 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↓
                    //K300030BL business = new K300030BL();

                    ////  長計データを引いてくる
                    //KumiaiLongRepairPlan tempData = business.GetKumiaiLongRepairPlan(this.LongRepairPlan.Pid);
                    // DEL 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↑

					//  現在のインデックスを覚えておく
					int tempAccountPeriodIndex = this.AccountPeriodIndex;

					//  一時的に書き換え
					AccountPeriodIndex = this.LongRepairPlan.AccountPeriod - LongRepairPlan.CreateAccountPeriod;

					//  再計算
					RecalcSub(this.ctrLongtermPlan_All.DataSource);

					//  元に戻す
					this.AccountPeriodIndex = tempAccountPeriodIndex;

					//  作業用データからDB用データに変換
					List<LongRepairPlanData> list = ctrLongtermPlan_All.DataSource;
					//list = (from item in list
					//        where item.Row == LongRepairPlanData.RowType.GroupItem
					//        select item).ToList();
					List<KumiaiLongRepairPlanDetail> convertedList = LongRepairPlanData.ToKumiaiLongRepairPlanDetails(list);

                    //ADDED 2012/08/22 Hieu↓
                    //  作業用データからDB用データに変換
                    List<LongRepairPlanData> subList = ctrLongtermPlan_Total.DataSource;
                    subList = (from item in subList where item.Row == LongRepairPlanData.RowType.GroupItem select item).ToList();
                    List<KumiaiLongRepairPlanDetail> groupedList = LongRepairPlanData.ToKumiaiLongRepairPlanDetails(subList);
                    //ADDED 2012/08/22 Hieu↑

                    //  もう一度再計算
					RecalcSub(this.ctrLongtermPlan_All.DataSource);

					//  出力
                    // MOD 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↓
                    //COMSKService.FileEntry fileEntry = business.ExportKumiaiLongRepairPlan(tempData, convertedList);
                    //
                    //bool bolIsKokukousyou = !frm.IsExport;
                    //yamagata20150220
                    bool bolIsKokukousyou = frm.IsExport;
                    //yamagata20150220end
                    COMSKService.FileEntry fileEntry = null;
                    if (bolIsKokukousyou)
                        fileEntry = business.ExportKumiaiLongRepairPlan(tempData, convertedList, groupedList, Helper.loginUserInfo.Pid);    //EDITED 2012/08/22 Hieu
                    else
                    {
						fileEntry = business.ExportKumiaiLongRepairPlan(tempData, convertedList);
						var isModifyOwner = this.IsModifyOwnerPlan(tempData);
						if (isModifyOwner)
                        {
							fileEntry = ReportCommon.RemoveShape(fileEntry, 1, COMSKCommon.COMSK_PLAN_REPORT_MJC_LOGO_NAME);
						}
					}
                    // MOD 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↑

					//  ファイル保存
					Helper.SaveFile(fileEntry, this);

				}
				catch (Exception ex)
				{
					Helper.WriteLog(ex);
					MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				finally
				{
					this.Cursor = Cursors.Arrow;
				}
			}
		}

		private bool IsModifyOwnerPlan(KumiaiLongRepairPlan planObj)
        {
			return planObj.TypeCode == COMSKCommon.COMSK_LONG_REPAIR_PLAN_TYPE_REEXAM && planObj.PresentationCode == COMSKCommon.COMSK_PRESENTCODE_OWNER;
		}

		/// <summary>
		/// 修繕計画ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnRepairPlan_Click(object sender, EventArgs e)
		{
			K300040010 frmRepairPlan = new K300040010(LongRepairPlan.Pid);
			frmRepairPlan.Show();
		}

		/// <summary>
		/// 修繕履歴ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnRepairHistory_Click(object sender, EventArgs e)
		{
			K300050020 frmRepairHistory = new K300050020(LongRepairPlan.KumiaiInfoPid);
			frmRepairHistory.Show();
		}

		/// <summary>
		/// 作成開始年度設定ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnSetBaseYear_Click(object sender, EventArgs e)
		{
			this.RestoreAllBanded();
			// banded リスト変わるのでDICT再生成
			this.ResetBandedDict();
			try
			{
				//  表示する会計期を割り出す
				int accountPeriod = int.MinValue;

				// 最終期
				var lastViewableStartTerm = this.maxTerm - COMSKCommon.MAX_VISIBLE_YEAR + 1;

				//  会計期コンボボックスが表示されていたら
				if (cmbAccountPeriod.Visible == true)
				{
					//  ダイレクトに持ってくる
					TypeValue<int> val = cmbAccountPeriod.SelectedItem as TypeValue<int>;
					if (val != null)
					{
						accountPeriod = val.Key;
					}
				}
				else
				{
					int datumYear;
					if (int.TryParse(txtDatumYear.Text, out datumYear))
					{
						//  会計期を検索
						int[] arrAccountPeriod = COMSKCommon.GetAccountPeriodFromAccountYear(LongRepairPlan.TermInfo, datumYear);

						//  一本に絞れているはず
						if (arrAccountPeriod.Length == 1)
						{
							int tempAccountPeriod = arrAccountPeriod[0];

							//  index が許容範囲内かどうか
							if ((this.LongRepairPlan.CreateAccountPeriod <= tempAccountPeriod) &&
								(tempAccountPeriod <= lastViewableStartTerm))
							{
								//  それを設定
								accountPeriod = tempAccountPeriod;
							}
						}
					}
				}

				if (accountPeriod != int.MinValue)
				{
					ChangeStartYear(accountPeriod);
					this.UpdateDictRecreatedYearlyProcessed("", true);
					this.DoZoomSubTab();
				}
				else
				{
					//  設定が無効
					int validStartYear = COMSKCommon.GetAccountYearFromAccountPeriod(this.LongRepairPlan.TermInfo, this.LongRepairPlan.CreateAccountPeriod);
					int validEndYear = COMSKCommon.GetAccountYearFromAccountPeriod(this.LongRepairPlan.TermInfo, lastViewableStartTerm);
					//string validStartYear = this.LongRepairPlan.TermInfo[this.LongRepairPlan.AccountPeriod].FiscalYear;
					//string validEndYear = this.LongRepairPlan.TermInfo[this.LongRepairPlan.AccountPeriod + this.LongRepairPlan.TermInfo.Length - MAX_VISIBLE_YEAR].FiscalYear;

					string msg = string.Format("作成開始年度設定が無効です。{0}～{1}年の間で入力してください。", validStartYear, validEndYear);
					MessageBox.Show(msg, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			catch (Exception ex)
			{
			}
		}

		/// <summary>
		/// 登録ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //20140815 y-hoshino start
            //if (MessageBox.Show(Constant.CONFIRM_REGISTER_TITLE, Constant.CONFIRM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            if ((LongRepairPlan.StatusCode == "0002") || (LongRepairPlan.StatusCode == "0003") || (LongRepairPlan.StatusCode == "0005"))
            {
                if (MessageBox.Show(Constant.CONFIRM_KAKUTEIHOZON_TITLE, Constant.CONFIRM_KAKUTEI_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show(Constant.CONFIRM_REGISTER_TITLE, Constant.CONFIRM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }
            //20140815 y-hoshino end
            //  ウェイトカーソル
            this.Cursor = Cursors.WaitCursor;
            try
            {
                K300030BL business = new K300030BL();

                //  作業用データからDB用データに変換
                List<LongRepairPlanData> list = ctrLongtermPlan_All.DataSource;
                errorMsg = "";
                DeleteIsOnTopData(list);//Diem add 20140115: https://reci.backlog.jp/view/MJC_DEV-264
                List<KumiaiLongRepairPlanDetail> convertedList = LongRepairPlanData.ToKumiaiLongRepairPlanDetails(list);

                //  年次データを登録
                if (!business.UpdateKumiaiLongRepairPlanDetails(this.pid, convertedList))
                {
                    throw new Exception("長計データの保存に失敗しました。");
                }

                //  変更理由・変更内容をマージする
                {
                    foreach (LongRepairPlanData data in list)
                    {
                        data.Histories = data.Histories.Concat(data.WorkUpdateReasonList).ToArray();
                        if (data.Histories.Length > 0)
                        {
                            data.UpdateFlg = COMSKCommon.HAS_HISTORY_FLG_ON;
                        }
                        data.WorkUpdateReasonList = new List<KumiaiLongRepairPlanDetailHistory>();
                    }
                }

                //  変更フラグを OFF
                modifyFlg = false;
                //  保存完了
                MessageBox.Show(Constant.SAVE_COMPLETE_TITLE, Constant.INFO_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowDialogChildDataDeleted();

                RefreshCurrentTab();

            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
                MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        #region 子工事の削除機能
        private void ChildDataDeleted(LongRepairPlanData itrData, int i)
        {
            long prevValue = itrData.GetValue(i);
            //  子のデータを削除
            itrData.SetValue(i, 0);
            itrData.UpdateContentList.Add(new KumiaiLongRepairPlanDetailUpdateContent()
            {
                Pid = long.MinValue,
                UpdateContent = COMSKCommon.GetUpdateContent_Change(i + 1, prevValue, 0),
                InsertUserMstPid = Helper.loginUserInfo.Pid,
                InsertDateTime = DateTime.Now,
            });

            errorMsg += "工事分類:" + itrData.ConstructionTypeName + "、工事種別:" + itrData.ConstructionCategoryName + "、工事区分:" + itrData.ConstructionDivisionName + "の" +
                this.LongRepairPlan.TermInfo[i].Term + "期の金額を削除しました。" + Environment.NewLine;
        }

        private void ShowDialogChildDataDeleted()
        {
            if (errorMsg != "")
            {
                K300100040 frm = new K300100040("子工事金額削除確認ダイアログ", errorMsg);
                frm.ShowDialog();
                errorMsg = "";
            }
        }
        #endregion 子工事の削除機能

        /// <summary>
		/// 戻るボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnClose_Click(object sender, EventArgs e)
		{
			//  変更フラグ ON なら
			if (modifyFlg == true)
			{
				//  問い合わせ
				string msg = "データは変更されています。保存せずに閉じてもよろしいですか？";
				if (MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					//  閉じずにリターン
					return;
				}
			}

			//  閉じる
			this.Close();
		}

		/// <summary>
		/// プレビューボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnPreview_Click(object sender, EventArgs e)
		{
			var needMaxTern = this.LongRepairPlan.AccountPeriod + COMSKCommon.MAX_VISIBLE_YEAR - 1;
			//  積立金データチェック
			if (!COMSKCommon.CheckReadyRepairReservePlanData(this.LongRepairPlan.KumiaiInfoPid, needMaxTern))
			{
				return;
			}

			K300070011 frm = new K300070011()
			{
				KumiaiLongRepairPlanPid = this.LongRepairPlan.Pid,
			};
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//  画面更新
				txtStatus.Text = frm.LongRepairPlan.StatusName;

				//  工事費累計を計算する
				long[] totalRepairCost = GetTotalRepairCost();

				//  表示
				K300070010 frmReservePlan = new K300070010()
				{
					LongRepairPlan = this.LongRepairPlan,
					TempTotalRepairCost = totalRepairCost,
					AutoCalc = frm.AutoCalc,
					AutoCalcDiff = frm.AutoCalcDiff,
				};
				frmReservePlan.Show();
			}
		}

		/// <summary>
		/// 消費税率ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnTaxRate_Click(object sender, EventArgs e)
		{
			K300030025 frm = new K300030025(LongRepairPlan.Pid);
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//  税率を再計算する
				CreateConsumptionTaxList(frm.ConsumptionTaxList);
				CreatePriceIncreaseList(frm.PriceIncreaseList);

				//  再計算
				Recalc();
			}
		}

		/// <summary>
		/// 長計設定ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnLongRepairPlanSettings_Click(object sender, EventArgs e)
		{
			K300030030 frm = new K300030030(this.LongRepairPlan);
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//  プロパティを更新する
				txtKumiai.Text = LongRepairPlan.KumiaiName;
				txtLongRepairPlanName.Text = LongRepairPlan.Name;
				txtNextNotifyDate.Text = LongRepairPlan.NextNotifyDate.ToString("yyyy年MM月dd日");
				txtPresentationType.Text = LongRepairPlan.PresentationName;

				txtType.Text = LongRepairPlan.TypeName;
				txtStatus.Text = LongRepairPlan.StatusName;

				//  取込再チェック
				CheckImportStatus();
			}
		}

		/// <summary>
		/// 積立金設定ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnReservePlanSettings_Click(object sender, EventArgs e)
		{
			K300070011 frm = new K300070011()
			{
				KumiaiLongRepairPlanPid = this.LongRepairPlan.Pid,
				SettingOnly = true,
			};
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//  画面更新
				txtStatus.Text = frm.LongRepairPlan.StatusName;
			}
		}


		/// <summary>
		/// 点検・調査履歴ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnInspection_Click(object sender, EventArgs e)
		{
			K300060020 frm = new K300060020(LongRepairPlan.KumiaiInfoPid);
			frm.Show();
		}

		/// <summary>
		/// 修繕基準取込
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnImportRepairPlan_Click(object sender, EventArgs e)
		{
			//  確認
			if (MessageBox.Show("この操作を行うと、保存されていない変更は全て失われます。よろしいですか？", Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				try
				{
					K300030BL business = new K300030BL();
					List<KumiaiLongRepairPlanDetail> list = business.GetAllKumiaiLongRepairPlanDetails(LongRepairPlan.Pid, Helper.loginUserInfo.Pid, initialFlg);
					var minimumDatumYear = list.Count == 0 ? 0 : list.Min(r => r.DatumYear);
					var zeroTermFiscalYear = Int32.Parse(this.LongRepairPlan.TermInfo[0].FiscalYear);
					if (minimumDatumYear != 0 && minimumDatumYear < (zeroTermFiscalYear - 1))
					{
						var errMessage = "以下の操作を行い、期情報を修正してください。\n　①期情報画面で{0}年に該当する会計期を追加してください。\n　②長期修繕計画画面で再度修繕基準取込ボタンを押してください。";
						errMessage = errMessage.Replace("{0}", (minimumDatumYear + 1).ToString());
						MessageBox.Show(errMessage, "エラーダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}										if (business.ImportKumiaiRepairPlanDetails(this.LongRepairPlan.Pid, Helper.loginUserInfo.Pid))
					{
						//  一度イベントをはがす
						ctrLongtermPlan_All.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_Temp.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_B.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_E.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_Out.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_Others.CellValueChanged -= ctrLongtermPlan_CellValueChanged;

						ctrLongtermPlan_All.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_All.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
						ctrLongtermPlan_Temp.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_Temp.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
						ctrLongtermPlan_B.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_B.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
						ctrLongtermPlan_E.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_E.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
						ctrLongtermPlan_Out.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_Out.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
						ctrLongtermPlan_Others.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_Others.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;

						// 行選択をクリア
						this.ClearRowSelected();

						//  再読み込み
						LoadDetails();

						//  強調を解除する
						EmphasizeButton(btnImportRepairPlan, false);
					}
					else
					{
						MessageBox.Show("修繕基準の取込に失敗しました。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
				catch (Exception ex)
				{
					Helper.WriteLog(ex);
					MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		private void ClearRowSelected()
        {
			this.ctrLongtermPlan_All.ClearSelectedRow();
			this.ctrLongtermPlan_Temp.ClearSelectedRow();
			this.ctrLongtermPlan_B.ClearSelectedRow();
			this.ctrLongtermPlan_E.ClearSelectedRow();
			this.ctrLongtermPlan_Out.ClearSelectedRow();
			this.ctrLongtermPlan_Others.ClearSelectedRow();
		}

		/// <summary>
		/// 修繕履歴実績取込
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnImportRepairHistory_Click(object sender, EventArgs e)
		{
			//  確認
			if (MessageBox.Show("この操作を行うと、保存されていない変更は全て失われます。よろしいですか？", Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				try
				{
					K300030BL business = new K300030BL();
					if (business.ImportKumiaiRepairHistories(this.LongRepairPlan.Pid, Helper.loginUserInfo.Pid))
					{
						//  一度イベントをはがす
						ctrLongtermPlan_All.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_Temp.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_B.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_E.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_Out.CellValueChanged -= ctrLongtermPlan_CellValueChanged;
						ctrLongtermPlan_Others.CellValueChanged -= ctrLongtermPlan_CellValueChanged;

                        ctrLongtermPlan_All.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_All.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
                        ctrLongtermPlan_Temp.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_Temp.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
                        ctrLongtermPlan_B.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_B.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
                        ctrLongtermPlan_E.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_E.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
                        ctrLongtermPlan_Out.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_Out.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;
                        ctrLongtermPlan_Others.EffectedYearChanged -= ctrLongtermPlan_EffectedYearChanged;
                        ctrLongtermPlan_Others.DatumYearChanged -= ctrLongtermPlan_DatumYearChanged;

						// 行選択をクリア
						this.ClearRowSelected();

						//  再読み込み
						LoadDetails();

						//  強調を解除する
						EmphasizeButton(btnImportRepairHistory, false);
					}
					else
					{
						MessageBox.Show("修繕履歴実績取込の取込に失敗しました。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
				catch (Exception ex)
				{
					Helper.WriteLog(ex);
					MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		#endregion

		#region グリッドからの通知

		/// <summary>
		/// セルの値が変更されたときに呼び出される
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrLongtermPlan_CellValueChanged(object sender, CustomEventArgs<LongRepairPlanData> e)
		{
			//  値が変わっていたら
			if (e.Param.NewValue != e.Param.PrevValue)
			{
				//  変更フラグを立てる
				e.Param.ModifiedAtLongRepairPlanFlg = COMSKCommon.HAS_HISTORY_FLG_ON;

				//  会計期
				int period = LongRepairPlan.TermInfo[this.TermInfoAccountPeriodIndex + e.Param.PeriodIndex].Term;

				//  変更前/変更後の値
				long prevValue = e.Param.PrevValue;
				long newValue = e.Param.NewValue;

				//  変更内容に追加
				e.Param.UpdateContentList.Add(new KumiaiLongRepairPlanDetailUpdateContent()
				{
					Pid = long.MinValue,
					UpdateContent = COMSKCommon.GetUpdateContent_Change(period, prevValue, newValue),
					InsertUserMstPid = Helper.loginUserInfo.Pid,
					InsertDateTime = DateTime.Now,
				});

				//  変更フラグ ON
				modifyFlg = true;

				//  集計行を再計算
				Recalc();
			}
		}

		/// <summary>
		/// 実績年度変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="coms.COMSK.common.CustomEventArgs&lt;coms.COMSK.common.LongRepairPlanData&gt;"/> instance containing the event data.</param>
		private void ctrLongtermPlan_EffectedYearChanged(object sender, CustomEventArgs<LongRepairPlanData> e)
		{
			//  変更フラグを立てる
			e.Param.ModifiedAtLongRepairPlanFlg = COMSKCommon.HAS_HISTORY_FLG_ON;

			//  変更内容に追加
			e.Param.UpdateContentList.Add(new KumiaiLongRepairPlanDetailUpdateContent()
			{
				Pid = long.MinValue,
				UpdateContent = COMSKCommon.GetUpdateContent_WriteEffectedYear(e.Param.ResultYear),
				InsertUserMstPid = Helper.loginUserInfo.Pid,
				InsertDateTime = DateTime.Now,
			});

			//  変更フラグ ON
			modifyFlg = true;

			//  集計行を再計算
			Recalc();
		}

        /// <summary>
        /// 実績年度変更イベント
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="coms.COMSK.common.CustomEventArgs&lt;coms.COMSK.common.LongRepairPlanData&gt;"/> instance containing the event data.</param>
        private void ctrLongtermPlan_DatumYearChanged(object sender, CustomEventArgs<LongRepairPlanData> e)
        {
            //  変更フラグを立てる
            e.Param.ModifiedAtLongRepairPlanFlg = COMSKCommon.HAS_HISTORY_FLG_ON;

            //  変更内容に追加
            e.Param.UpdateContentList.Add(new KumiaiLongRepairPlanDetailUpdateContent()
            {
                Pid = long.MinValue,
                UpdateContent = COMSKCommon.GetUpdateContent_WriteDatumYear(e.Param.DatumYear.ToString()),
                InsertUserMstPid = Helper.loginUserInfo.Pid,
                InsertDateTime = DateTime.Now,
            });

            //  変更フラグ ON
            modifyFlg = true;

            //  集計行を再計算
            Recalc();
        }

		/// <summary>
		/// 履歴登録イベントハンドラ
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="coms.COMSK.common.CustomEventArgs&lt;coms.COMSK.common.LongRepairPlanData&gt;"/> instance containing the event data.</param>
		private void ctrLongtermPlan_OpenRegistering(object sender, CustomEventArgs<LongRepairPlanData> e)
		{
			K300030023 frmRegister = new K300030023(e.Param,LongRepairPlan.Name)
			{
				UpdateReasonCode = updateReasonCodeMst,
			};
			if (frmRegister.ShowDialog() == DialogResult.OK)
			{
				//  変更フラグ ON
				modifyFlg = true;

				//  リフレッシュ
				Refresh();
			}
		}

		/// <summary>
		/// 変更履歴表示イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="coms.COMSK.common.CustomEventArgs&lt;coms.COMSK.common.LongRepairPlanData&gt;"/> instance containing the event data.</param>
		private void ctrLongtermPlan_OpenHistory(object sender, CustomEventArgs<LongRepairPlanData> e)
		{
			//  今までの変更履歴 (DB から取ってきたもの) に、今回の変更分を追加する
			KumiaiLongRepairPlanDetailHistory[] histories = e.Param.Histories.Concat(e.Param.WorkUpdateReasonList).ToArray();
			K300030021 frmHistory = new K300030021(e.Param, histories,LongRepairPlan.Name);
			frmHistory.ShowDialog();
		}

		/// <summary>
		/// 手引き表示
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="coms.COMSK.common.CustomEventArgs&lt;coms.COMSK.common.LongRepairPlanData&gt;"/> instance containing the event data.</param>
		private void ctrLongtermPlan_OpenDetail(object sender, CustomEventArgs<LongRepairPlanData> e)
		{
			K300030027 frm = new K300030027(e.Param.KumiaiRepairPlanDetailPid,LongRepairPlan.Name);
			frm.ShowDialog();
		}

		/// <summary>
		/// ドラッグ完了イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="coms.COMSK.common.DragCompletedEventArgs"/> instance containing the event data.</param>
		private void ctrLongtermPlan_DragCompleted(object sender, RowCellsDragEventArgs e)
		{
			//Diem add: https://reci.backlog.jp/view/MJC_DEV-233
			if (this.LongRepairPlan.IntensiveFlg == "0010")//集約
			{
				return;
			}
			//  カーソル位置を取得
			Point cursorPos = Cursor.Position;

			//  会計期
			int startIndex = this.TermInfoAccountPeriodIndex + e.FromColumnIndex;
			int endIndex = this.TermInfoAccountPeriodIndex + e.ToColumnIndex;
			int startPeriod = LongRepairPlan.TermInfo[startIndex].Term;
			int endPeriod = LongRepairPlan.TermInfo[endIndex].Term;

			//  フォームを表示
			K300030024 frmPopup = new K300030024();
			frmPopup.Location = new Point(cursorPos.X - frmPopup.Size.Width, cursorPos.Y);
			frmPopup.MoveStartAccountYear = startPeriod;
			frmPopup.MoveEndAccountYear = endPeriod;
			frmPopup.Show();
			frmPopup.FormClosed += (object sender2, FormClosedEventArgs e2) =>
			{
				//  OK で閉じられたら
				if (frmPopup.PopupResult == true)
				{
					//  実際に移動
					try
					{
						//  e.DataList 内の LongRepairPlanData の PrevValue を 0 にしておく
						//  実際に移動された行の PrevValue には 1 が設定される
						foreach (LongRepairPlanData data in e.DataList)
						{
							data.PrevValue = 0;
						}

						//  値を移動
						int startIndexAccount = AccountPeriodIndex + e.FromColumnIndex;
						int endIndexAccount = AccountPeriodIndex + e.ToColumnIndex;
						var dtl = e.DataList as List<LongRepairPlanData>;
						MoveCells(dtl, startIndexAccount, endIndexAccount, frmPopup.UpdateLator);

						//  子工事を削除
						DeleteIsOnTopData(dtl);
                        errorMsg = ""; //周期再計算後はエラーメッセージ不要
						//  再計算
						Recalc();

						//  変更内容を追加
						string updateContent = COMSKCommon.GetUpdateContent_Drag(startPeriod, endPeriod, frmPopup.UpdateLator);
						long insertUserPid = Helper.loginUserInfo.Pid;
						foreach (LongRepairPlanData data in e.DataList)
						{
							//  移動されていたら
							if (data.PrevValue != 0)
							{
								//  変更内容を追加
								data.UpdateContentList.Add(new KumiaiLongRepairPlanDetailUpdateContent()
								{
									Pid = long.MinValue,
									UpdateContent = updateContent,
									InsertUserMstPid = insertUserPid,
									InsertDateTime = DateTime.Now,
								});

								//  長計で変更フラグも立てる
								data.ModifiedAtLongRepairPlanFlg = COMSKCommon.HAS_HISTORY_FLG_ON;
							}
						}

						//  変更フラグ ON
						modifyFlg = true;
                        

					}
					catch (Exception ex)
					{
						Helper.WriteLog(ex);
						MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}

				}
			};
		}

		private bool NeedProcessZoom()
        {
			// サブタブから拡大率変更した場合イベント発生しません。
			if (this.ignoreZoomChangedEvent)
            {
				this.ignoreZoomChangedEvent = false;
				return false;
			}

			int newZoomRate = 100;
			if (cmbZoomRate.SelectedItem != null)
            {
				var selectedObj = (TypeValue<int>)cmbZoomRate.SelectedItem;
				newZoomRate = int.Parse(selectedObj.Name);
			}
			// 拡大率変わるまたは最初100ではない場合処理
			if ((this.zoomValue.HasValue && newZoomRate != this.zoomValue) || (!this.zoomValue.HasValue && newZoomRate != 100))
			{
				return true;
			}

			return false;
		}

		private void cmbZoomRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			var doZoom = this.NeedProcessZoom();
			if (doZoom)
			{
				int newZoomRate = 100;
				if (cmbZoomRate.SelectedItem != null)
				{
					var selectedObj = (TypeValue<int>)cmbZoomRate.SelectedItem;
					newZoomRate = int.Parse(selectedObj.Name);
				}
				this.zoomValue = newZoomRate;
				this.DoZoomSubTab();
			}
		}

		private void DoZoomSubTab()
        {
			try
			{
				var keyCtr = xtraTabControl_LongRepairPlan.SelectedTabPage.Controls[0].Name;
				bool reCreatedYearly = this.dictRecreatedYearlyProcessed[keyCtr];

				if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_All)
				{
					ctrLongtermPlan_All.SetZoom(this.zoomValue.Value, reCreatedYearly);
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Total)
				{
					ctrLongtermPlan_Total.SetZoom(this.zoomValue.Value, reCreatedYearly);
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_TempConstruction)
				{
					ctrLongtermPlan_Temp.SetZoom(this.zoomValue.Value, reCreatedYearly);
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Building)
				{
					ctrLongtermPlan_B.SetZoom(this.zoomValue.Value, reCreatedYearly);
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Equipment)
				{
					ctrLongtermPlan_E.SetZoom(this.zoomValue.Value, reCreatedYearly);
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_OutwardAppearance)
				{
					ctrLongtermPlan_Out.SetZoom(this.zoomValue.Value, reCreatedYearly);
				}
				else if (xtraTabControl_LongRepairPlan.SelectedTabPage == xtraTabPage_Others)
				{
					ctrLongtermPlan_Others.SetZoom(this.zoomValue.Value, reCreatedYearly);
				}
				if (!string.IsNullOrEmpty(keyCtr)) this.UpdateDictRecreatedYearlyProcessed(keyCtr, false);
			}
			catch (Exception)
			{
			}
		}

		private void SetZoomList()
		{
			cmbZoomRate.Items.Clear();
			foreach (int zomVal in zoomLst)
			{
				TypeValue<int> val = new TypeValue<int>(zomVal, zomVal.ToString());
				cmbZoomRate.Items.Add(val);
			}
			cmbZoomRate.SelectedIndex = 5;
		}

		private void RestoreAllBanded()
		{
			ctrLongtermPlan_All.RestoreAllBanded();
			ctrLongtermPlan_Total.RestoreAllBanded();
			ctrLongtermPlan_Temp.RestoreAllBanded();
			ctrLongtermPlan_B.RestoreAllBanded();
			ctrLongtermPlan_E.RestoreAllBanded();
			ctrLongtermPlan_Out.RestoreAllBanded();
			ctrLongtermPlan_Others.RestoreAllBanded();
		}

		private void ResetBandedDict()
        {
			ctrLongtermPlan_All.ResetBandedDict();
			ctrLongtermPlan_Total.ResetBandedDict();
			ctrLongtermPlan_Temp.ResetBandedDict();
			ctrLongtermPlan_B.ResetBandedDict();
			ctrLongtermPlan_E.ResetBandedDict();
			ctrLongtermPlan_Out.ResetBandedDict();
			ctrLongtermPlan_Others.ResetBandedDict();
		}

		public void ChangeZoomRateFromChildTab(int rate)
        {
			this.zoomValue = rate;
			var idx = Array.FindIndex(zoomLst, r => r == rate);
			this.ignoreZoomChangedEvent = true;
			this.cmbZoomRate.SelectedIndex = idx;
		}

		private void UpdateDictRecreatedYearlyProcessed(string keyName, bool Val)
        {
			if (string.IsNullOrEmpty(keyName))
            {
				dictRecreatedYearlyProcessed["ctrLongtermPlan_All"] = Val;
				dictRecreatedYearlyProcessed["ctrLongtermPlan_Total"] = Val;
				dictRecreatedYearlyProcessed["ctrLongtermPlan_Temp"] = Val;
				dictRecreatedYearlyProcessed["ctrLongtermPlan_B"] = Val;
				dictRecreatedYearlyProcessed["ctrLongtermPlan_E"] = Val;
				dictRecreatedYearlyProcessed["ctrLongtermPlan_Out"] = Val;
				dictRecreatedYearlyProcessed["ctrLongtermPlan_Others"] = Val;
			}
            else
            {
				dictRecreatedYearlyProcessed[keyName] = Val;
			}
        }
		#endregion

		#endregion

		#region 権限
		/// <summary>
		/// 権限設定
		/// </summary>
		private void SetAuthorities()
		{
			//TODO: CoMS-K_Authority
			string userPostCode = Helper.loginUserInfo.PostCode;
			string userPositionCode = Helper.loginUserInfo.PositionCode;
			string sql =
			"    AI.partsCode LIKE '%btnK300030020%'" +
			" OR AI.partsCode LIKE '%btnK300030010%'";

			COMSD.business.D100109BL bl109 = new coms.COMSD.business.D100109BL();
			COMSDService.AuthorityInfo[] authorities = bl109.SearchAuthorityInfo(sql, "", "");

			btnOutputLongtermRepairPlan.Enabled = Helper.GetAuthority(authorities, "btnK300030020001", userPostCode, userPositionCode);
			btnOutputRepairPlan.Enabled = Helper.GetAuthority(authorities, "btnK300030020002", userPostCode, userPositionCode);
			btnUpdate.Enabled = Helper.GetAuthority(authorities, "btnK300030020003", userPostCode, userPositionCode);

			//  修繕基準
			btnRepairPlan.Enabled = Helper.GetAuthority(authorities, "btnK300030010003", userPostCode, userPositionCode);
			//  修繕履歴
			btnRepairHistory.Enabled = Helper.GetAuthority(authorities, "btnK300030010004", userPostCode, userPositionCode);
			//  点検・調査履歴
			btnInspection.Enabled = Helper.GetAuthority(authorities, "btnK300030010002", userPostCode, userPositionCode);

		}
        #endregion
    }
}
