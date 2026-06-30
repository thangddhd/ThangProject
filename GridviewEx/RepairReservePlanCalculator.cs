using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using coms.COMSKService;

namespace coms.COMSK.common
{
	/// <summary>
	/// 修繕積立金計算ヘルパ
	/// </summary>
	public class RepairReservePlanCalculator
	{

		#region 定数

		/// <summary>
		/// データ長
		/// </summary>
		//private const int MAX_DATA_LENGTH = 100;
        //public static readonly int MAX_REPAIR_RESERVE_YEAR = 60;
		// in some try got that
		//   the zoomin width = 7750, out = 5240 => calc the rate
		private double rateUp = 7750.0 / 5240.0;
		private double rateDown = 5240.0 / 7750.0;
		#endregion

		#region 自動計算情報

		/// <summary>
		/// 自動計算情報
		/// </summary>
		public class AutoCalcInfo
		{
			public int FirstRevisionIndex { get; set; }
			public double CostIncreaseBand { get; set; }
			public int RevisionPitch { get; set; }

			public string CostShortCode { get; set; }
			public string CostShortCodeBefore { get; set; }

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AutoCalcInfo()
			{
				FirstRevisionIndex = int.MinValue;
				CostIncreaseBand = double.MinValue;
				RevisionPitch = int.MinValue;
				CostShortCode = string.Empty;
				CostShortCodeBefore = string.Empty;
			}

			/// <summary>
			/// データが不正でないかチェック
			/// </summary>
			/// <returns>
			///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
			/// </returns>
			public bool IsValid()
			{
				if ((FirstRevisionIndex == int.MinValue) ||
					(CostIncreaseBand == double.MinValue) ||
					(RevisionPitch == int.MinValue) ||
					(CostShortCode == string.Empty))
				{
					return false;
				}

				if (FirstRevisionIndex < 0 || FirstRevisionIndex >= COMSKCommon.MAX_VISIBLE_YEAR)
				{
					return false;
				}

				//  All OK
				return true;
			}

		}

		#endregion

		#region プロパティ

		#region 金額配列データ

		/// <summary>
		/// 累計工事費
		/// </summary>
		public Data30Period TotalRepairCost { get; set; }
		/// <summary>
		/// 積立金累計
		/// </summary>
		public Data30Period TotalReserveCost { get; set; }
		/// <summary>
        /// 次年度繰越金
		/// </summary>
		public Data30Period DiffConstructionCost { get; set; }
		/// <summary>
		/// 年度内修繕積立金
		/// </summary>
		public Data30Period YearRepairReserveCost { get; set; }
		/// <summary>
		/// 年度内住戸積立金
		/// </summary>
		public Data30Period YearHouseReserveCost { get; set; }
		/// <summary>
		/// 年度内住戸積立金（手入力または以降反映）
		/// </summary>
		public Data30Period YearHouseReserveCostLastInput { get; set; }
		/// <summary>
		/// 修繕積立金繰越額
		/// </summary>
		public Data30Period CarryOverCost { get; set; }
		/// <summary>
		/// 管理費振替額
		/// </summary>
		public Data30Period TransfarCost { get; set; }
		/// <summary>
		/// その他収入金
		/// </summary>
		public Data30Period OtherInCost { get; set; }
		/// <summary>
		/// その他支出金
		/// </summary>
		public Data30Period OtherOutCost { get; set; }
		/// <summary>
		/// その他会計口への繰入金
		/// </summary>
		public Data30Period OtherOutTransfarCost { get; set; }
		/// <summary>
		/// 住戸月額積立金単価（専有面積）
		/// </summary>
		public Data30Period HouseMonthReserveCost { get; set; }
		/// <summary>
		/// 住戸一時負担金単価（専有面積）
		/// </summary>
		public Data30Period HouseLumpsumCost { get; set; }
		/// <summary>
		/// 住戸月額積立金単価（共有持分）
		/// </summary>
		public Data30Period HouseSharedMonthReserveCost { get; set; }
		/// <summary>
		/// 住戸一時負担金単価（共有持分）
		/// </summary>
		public Data30Period HouseSharedLumpsumCost { get; set; }
		/// <summary>
		/// 年度内月額積立金
		/// </summary>
		public Data30Period YearMonthReserveCost { get; set; }
        /// <summary>
        /// 年度内一時負担金
        /// </summary>
        public Data30Period YearLumpsumCost { get; set; }



		#endregion

		#region 計算方法等

		/// <summary>
		/// 自動計算方法
		/// </summary>
		public string CalcDivision { get; set; }
		/// <summary>
		/// 月額端数単位
		/// </summary>
		public string MonthlyFractionUnit { get; set; }
		/// <summary>
		/// 月額端数処理
		/// </summary>
		public string MonthlyFractionProcess { get; set; }
		/// <summary>
		/// 一時金端数単位
		/// </summary>
		public string LumpsumFractionUnit { get; set; }
		/// <summary>
		/// 一時金端数処理
		/// </summary>
		public string LumpsumFractionProcess { get; set; }

		/// <summary>
		/// タイプ一覧
		/// </summary>
		public List<TypeMst> Types { get; set; }

		/// <summary>
		/// 決算・改定
		/// </summary>
		public int AccountAndRevision { get; set; }
		/// <summary>
		/// 現状の積立金単価
		/// </summary>
		public double CurrentHouseMonthlyReserveCost { get; set; }

		/// <summary>
		/// 最低工事費差額
		/// </summary>
		public long MinDiffConstructionCost { get; set; }

		/// <summary>
		/// 初年度計算月数 (基本は 12)
		/// </summary>
		public int FirstYearCalcMonth { get; set; }
		/// <summary>
		/// 帳票用総専有面積
		/// </summary>
		public double totalAppropriationArea { get; set; }

		public double totalOwnership { get; set; }
		#endregion

		#region 出力

		/// <summary>
		/// 年額
		/// </summary>
		public int YearlyCost { get; private set; }

        /// <summary>
        /// 月額
        /// </summary>
        public int MonthlyCost { get; private set; }

        /// <summary>
        /// 一時金
        /// </summary>
        public int LumpsumCost { get; private set; }

        #endregion

        #region 計算元データ

        /// <summary>
        /// 住戸月額積立金単価・専有面積
        /// </summary>
        public Data30Period MonthlyHouseReserveCost { get; set; }
        /// 住戸一時金積立金単価・専有面積
        /// </summary>
        public Data30Period LumpsumHouseReserveCost { get; set; }
		/// <summary>
		/// 住戸月額積立金単価・共有持分
		/// </summary>
		public Data30Period MonthlyHouseSharedReserveCost { get; set; }
		/// 住戸一時金積立金単価・共有持分
		/// </summary>
		public Data30Period LumpsumHouseSharedReserveCost { get; set; }

		#endregion
		#endregion

		#region メンバ

		#endregion

		#region コンストラクタ

		public RepairReservePlanCalculator()
		{
			HouseMonthReserveCost = new Data30Period();
			HouseLumpsumCost = new Data30Period();
			HouseSharedMonthReserveCost = new Data30Period();
			HouseSharedLumpsumCost = new Data30Period();
			CarryOverCost = new Data30Period();

			FirstYearCalcMonth = 12;
		}

		#endregion
		
		#region Public

        /// <summary>
        /// タイプ別の金額を計算する
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="typeMst">The type MST.</param>
        public void Calc(int index, TypeMst typeMst)
        {
            //  計算基本値
            double calcValue = GetCalcValue(typeMst);

			var targetCalcMonthly = this.GetTargetUnitPrice();
			var targetCalcLumpsum = this.GetTargetHouseLumpsum();

			//20200710 MJC_DEV-804 【プログラム修正】【長計システム】積立金計算不正 (No.366) start
			//  月額金
			MonthlyCost = (int)CollectFraction(System.Convert.ToDecimal(targetCalcMonthly.GetValue(index)) * System.Convert.ToDecimal(calcValue), MonthlyFractionUnit, MonthlyFractionProcess);

            //  一時金
            LumpsumCost = (int)CollectFraction(System.Convert.ToDecimal(targetCalcLumpsum.GetValue(index)) * System.Convert.ToDecimal(calcValue), LumpsumFractionUnit, LumpsumFractionProcess);

            //  年額
            {
                //  当年の月額単価
                double currMonthlyReserveCost = targetCalcMonthly.GetValue(index);
                //  前年の月額単価 (無ければ CurrentReservedCost)
                double prevMonthlyReserveCost = CurrentHouseMonthlyReserveCost;
                if (index > 0)
                {
                    prevMonthlyReserveCost = targetCalcMonthly.GetValue(index - 1);
                }

                //  年額を計算
                double currYearlyReserveCost = CollectFraction(System.Convert.ToDecimal(currMonthlyReserveCost * calcValue), MonthlyFractionUnit, MonthlyFractionProcess) * (12 - AccountAndRevision);
                double prevYearlyReserveCost = CollectFraction(System.Convert.ToDecimal(prevMonthlyReserveCost * calcValue), MonthlyFractionUnit, MonthlyFractionProcess) * AccountAndRevision;
                //20200710 MJC_DEV-804 【プログラム修正】【長計システム】積立金計算不正 (No.366)
                //  月額平均を出す
                double avgMonthReserveCost = Math.Round((prevYearlyReserveCost + currYearlyReserveCost) / 12, MidpointRounding.AwayFromZero);

                //  年額を出す
                YearlyCost = (int)((avgMonthReserveCost * 12 + LumpsumCost) * typeMst.RoomNumber);
            }
        }

		/// <summary>
		/// 自動計算オプションを設定
		/// </summary>
		/// <param name="planConst">The plan const.</param>
		public void SetCalculateOption(KumiaiLongRepairPlan kumiaiLongRepairPlan)
		{
			MaintenancePlanConst planConst = kumiaiLongRepairPlan.MaintenancePlanInfo;

			CalcDivision = planConst.CalcDivision;
			MonthlyFractionUnit = planConst.MonthlyFractionUnit;
			MonthlyFractionProcess = planConst.MonthlyFractionProcess;
			LumpsumFractionUnit = planConst.LumpsumFractionUnit;
			LumpsumFractionProcess = planConst.LumpsumFractionProcess;
			Types = planConst.Types.ToList();
			AccountAndRevision = planConst.MonRevisedAccounting;
			this.totalAppropriationArea = kumiaiLongRepairPlan.ReportBasicInfo.TotalAppropriationArea;
			this.totalOwnership = Types.Where(t => t.Ownership > double.MinValue).Sum(t => t.Ownership * t.RoomNumber);
			CurrentHouseMonthlyReserveCost = kumiaiLongRepairPlan.CurrentReservedCost;
			if (kumiaiLongRepairPlan.FundCost != double.MinValue)
			{
				if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
                {
					HouseLumpsumCost.SetValue(0, kumiaiLongRepairPlan.FundCost);
				}
                else
                {
					HouseSharedLumpsumCost.SetValue(0, kumiaiLongRepairPlan.FundCost);
				}
			}
			if ((MinDiffConstructionCost = kumiaiLongRepairPlan.MinimumDiffRepairCost) == long.MinValue)
			{
				MinDiffConstructionCost = 0;
			}

			//  新規長計なら
			if (kumiaiLongRepairPlan.TypeCode == COMSKCommon.COMSK_LONG_REPAIR_PLAN_TYPE_NEW)
			{
				try
				{
					//  初年度の会計年度のSTART/ENDを取得
                    //Linh 20170404 https://reci.backlog.jp/view/MJC_DEV-604
                    //DateTime dtStart = kumiaiLongRepairPlan.TermInfo[0].FromDate;
                    //DateTime dtEnd = kumiaiLongRepairPlan.TermInfo[0].ToDate;

                    var termInfo = kumiaiLongRepairPlan.TermInfo.Where(c => c.Term == 1).Select(c => new { fromDate = c.FromDate, toDate = c.ToDate }).FirstOrDefault();
                    DateTime dtStart = termInfo.fromDate;
                    DateTime dtEnd = termInfo.toDate;
                    //End Linh 20170404 https://reci.backlog.jp/view/MJC_DEV-604

					//  月換算する
					int monStart = dtStart.Year * 12 + (dtStart.Month - 1);
					int monEnd = dtEnd.Year * 12 + (dtEnd.Month - 1);

					//  初年度の積立金の計算月数
					FirstYearCalcMonth = monEnd - monStart + 1;
				}
				catch (Exception)
				{
				}
			}
		}

		/// <summary>
		/// 全て再計算
		/// </summary>
        public void CalcAll(KumiaiLongRepairPlan kumiaiLongRepairPlan, KumiaiRepairReservePlanDraft draft, AutoCalcInfo info, bool autoCalcDiff, bool autoCalc)
		{
			
			if (autoCalcDiff)
			{
                //次年度繰越金のみ自動計算
				CalcDiffConstructionCostOnly(0,kumiaiLongRepairPlan);
			}
			else
			{
				long updateVal = 0;

				#region ■  現状の積立金単価
				//  現状の積立金単価で更新
				for (int i = 0; i < COMSKCommon.YEAR100; i++)
				{
					if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
                    {
						HouseMonthReserveCost.SetValue(i, CurrentHouseMonthlyReserveCost);
					}
					else
                    {
						HouseSharedMonthReserveCost.SetValue(i, CurrentHouseMonthlyReserveCost);
					}
				}
				#endregion

				#region ■  修繕積立金繰越額
				//仕様変更により必ず設定する必要が有るため、呼び出し元(CtrRepairReservePlan.cs)へ移動
				////  初年度のみ数字を入れ、他は 0
				//if (kumiaiLongRepairPlan.CarryOverCost != long.MinValue)
				//{
				//    updateVal = kumiaiLongRepairPlan.CarryOverCost;
				//}
				//else
				//{
				//    updateVal = 0;
				//}

				//CarryOverCost.SetValue(0, updateVal);
				//for (int i = 1; i < MAX_DATA_LENGTH; i++)
				//{
				//    CarryOverCost.SetValue(i, 0);
				//}

				#endregion

				#region ■  管理費振替額

				//  オプションによって数字を入れる
				if (kumiaiLongRepairPlan.TransfarCost != long.MinValue)
				{
					updateVal = kumiaiLongRepairPlan.TransfarCost;
				}
				else
				{
					updateVal = 0;
				}

				if (kumiaiLongRepairPlan.TransfarCostCode == COMSKCommon.COMSK_TRANSFAR_COST_STARTING)
				{
					//  初年度のみ
					TransfarCost.SetValue(0, updateVal);
					for (int i = 1; i < COMSKCommon.YEAR100; i++)
					{
						TransfarCost.SetValue(i, 0);
					}
				}
				else if (kumiaiLongRepairPlan.TransfarCostCode == COMSKCommon.COMSK_TRANSFAR_COST_EVERYYEAR)
				{
					//  毎年
					for (int i = 0; i < COMSKCommon.YEAR100; i++)
					{
						TransfarCost.SetValue(i, updateVal);
					}
				}
				else
				{
					//  ピッチ指定
					int pitch = 0;
					if (kumiaiLongRepairPlan.TransfarCostPitch != int.MinValue)
					{
						pitch = kumiaiLongRepairPlan.TransfarCostPitch;
					}

					if (pitch != 0)
					{
						//  一回に置く数字
						for (int i = 0; i < COMSKCommon.YEAR100; i++)
						{
							if ((i % pitch) == 0)
							{
								TransfarCost.SetValue(i, updateVal);
							}
							else
							{
								TransfarCost.SetValue(i, 0);
							}
						}
					}
					else
					{
						//  0除算になるので全て 0
						for (int i = 0; i < COMSKCommon.YEAR100; i++)
						{
							TransfarCost.SetValue(i, 0);
						}
					}
				}

				#endregion

				#region ■　その他収入

				//  初年度のみ数字を入れ、他は 0
				if (kumiaiLongRepairPlan.OtherInCost != long.MinValue)
				{
					updateVal = kumiaiLongRepairPlan.OtherInCost;
				}
				else
				{
					updateVal = 0;
				}

				OtherInCost.SetValue(0, updateVal);
				for (int j = 1; j < COMSKCommon.YEAR100; j++)
				{
					OtherInCost.SetValue(j, 0);
				}

				#endregion

				#region ■  その他支出

				if (kumiaiLongRepairPlan.OtherOutCost != long.MinValue)
				{
					updateVal = kumiaiLongRepairPlan.OtherOutCost;
				}
				else
				{
					updateVal = 0;
				}

				OtherOutCost.SetValue(0, updateVal);
				for (int j = 1; j < COMSKCommon.YEAR100; j++)
				{
					OtherOutCost.SetValue(j, 0);
				}

				#endregion

				#region ■  その他会計口繰入金

				if (kumiaiLongRepairPlan.OtherOutTransfarCost != long.MinValue)
				{
					updateVal = kumiaiLongRepairPlan.OtherOutTransfarCost;
				}
				else
				{
					updateVal = 0;
				}

				OtherOutTransfarCost.SetValue(0, updateVal);
				for (int j = 1; j < COMSKCommon.YEAR100; j++)
				{
					OtherOutTransfarCost.SetValue(j, 0);
				}

				#endregion

				//  自動計算パラメータがあれば
				if (info != null)
				{
					CalcAuto(info);
				}

				//  最後に工事費累計を再計算
				CalcTotal(0);
			}

		}

		/// <summary>
		/// 年度内住戸積立金より上を再計算する
		/// </summary>
		/// <param name="index">The index.</param>
		public void CalcAllUpperYearHouseReserveCost(int index)
		{
			//  年度内積立金を計算
			YearRepairReserveCost.SetValue(index,
				YearHouseReserveCost.GetValue(index) +
				GetYearOtherCost(index));

			//  以降を計算
			CalcReserveCostAllNextYear(index);
		}
		/// <summary>
		/// 積立金詳細設定で繰入金～住戸一時負担金単価の変更した場合
		/// 当年度が全部再計算しますが次年度から累計のみを再計算する
		/// </summary>
		/// <param name="index"></param>
		public void CalcReserveByValueChanged(int index, bool keepYearMonthReserveCost, bool alsoNextYear = true)
		{
			//  年度内積立金を計算
			CalcDiffConstructionCost(index, keepYearMonthReserveCost);

			// 1期が２年にあるので（例：2025/4/1～2026/3/31）
			// 次期も再計算します
			if (index + 1 < COMSKCommon.YEAR100 && alsoNextYear)
				CalcDiffConstructionCost(index + 1, false);

			//  以降を計算
			CalcReserveCostAllNextYear(index);
		}
		/// <summary>
		/// 次年度の累計を再計算する
		/// </summary>
		/// <param name="index"></param>
		private void CalcReserveCostAllNextYear(int index)
        {
			//  以降を計算
			for (int i = index; i < COMSKCommon.YEAR100; i++)
			{
				//  前年の積立金累計
				double val = 0;
				if (i > 0)
				{
					val = TotalReserveCost.GetValue(i - 1);
				}

				//  積立金累計 = 前年度の積立金累計 + 当年の年度内修繕積立金 + 繰越額
				double sumVal = val + YearRepairReserveCost.GetValue(i) + CarryOverCost.GetValue(i);
				TotalReserveCost.SetValue(i, sumVal);

				//  次年度繰越金を計算
				DiffConstructionCost.SetValue(i,
					TotalReserveCost.GetValue(i) -
					TotalRepairCost.GetValue(i));
			}
		}

		/// <summary>
		/// 指定インデックス以降の累計を再計算
		/// </summary>
		/// <param name="index">The index.</param>
		public void CalcTotal(int index)
		{
            //  次年度繰越金を計算
			for (int i = index; i < COMSKCommon.YEAR100; i++)
			{
				CalcDiffConstructionCost(i, false);
			}
		}

		/// <summary>
		/// 工事費累計から逆方向に月額金を求め、再計算
		/// </summary>
		/// <param name="fromIndex">From index.</param>
		/// <param name="toIndex">To index.</param>
		public void CalcAllReverseMonthly(int fromIndex, int toIndex)
		{
			//  逆計算
			CalcMonthlyHouseReserveCostReverse(fromIndex, toIndex);

			//  累計積立金を再計算
			for (int i = fromIndex; i < COMSKCommon.YEAR100; i++)
			{
				CalcTotal(i);
			}
		}

		/// <summary>
		/// 工事費累計から逆方向に一時金を求め、再計算
		/// </summary>
		/// <param name="index">The index.</param>
		public void CalcAllReverseLumpsum(int index)
		{
			//  逆計算
			CalcMonthlyHouseLumpsumCostReverse(index);

			//  累計積立金を再計算
			for (int i = index; i < COMSKCommon.YEAR100; i++)
			{
				CalcTotal(i);
			}
		}

		/// <summary>
		/// 自動計算をシミュレートする
		/// </summary>
		/// <param name="info">The info.</param>
		public void CalcAuto(AutoCalcInfo info, Dictionary<string, List<int>> valueChangedCells = null)
		{
			//  増加分 (最初は 0 にしておく)
			double incrCost = 0;

			//  改定ピッチ
			int pitchCounter = -info.FirstRevisionIndex;

			for (int i = 0; i < COMSKCommon.YEAR100; i++)
			{
				//  ピッチが 0 年なら
				if (info.RevisionPitch == 0)
				{
					//  常に CostIncreaseBand
					incrCost = info.CostIncreaseBand;
				}
				//  カウンタが負の場合は
				else if (pitchCounter < 0)
				{
					//  常に 0
					incrCost = 0;
				}
				else
				{
					//  改定年なら
					if ((pitchCounter % info.RevisionPitch) == 0)
					{
						//  増加分をインクリメント
						incrCost += info.CostIncreaseBand;
					}
				}

				//  住戸月額積立金単価をインクリメント
                //tho 20170224 https://reci.backlog.jp/view/MJC_DEV-602 upd
                //HouseMonthReserveCost.SetValue(i, CurrentHouseMonthlyReserveCost + incrCost);
				if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
                {
					HouseMonthReserveCost.SetValue(i, (double)((decimal)CurrentHouseMonthlyReserveCost + (decimal)incrCost));
				}
				else
                {
					HouseSharedMonthReserveCost.SetValue(i, (double)((decimal)CurrentHouseMonthlyReserveCost + (decimal)incrCost));
				}

				//  一時金は 0 にする
				//  ただし、初年度を除く
				var isShortDiff = !string.IsNullOrEmpty(info.CostShortCodeBefore) && info.CostShortCode != info.CostShortCodeBefore;  // 一時金からそのままに変更して再計算するはリセットする
				if (i != 0 && (info.CostShortCode == "0002" || isShortDiff))  // そのままの場合クリアしません
				{
					// 一時金または変更箇所なければ必ずリセット
					var needReset = info.CostShortCode == "0002" || valueChangedCells == null;
					if (!needReset)
                    {
						var detailCode = COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST;
						var hasChangedVal = valueChangedCells.ContainsKey(detailCode) && valueChangedCells[detailCode].Contains(i);
						detailCode = COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST;
						hasChangedVal = hasChangedVal || valueChangedCells.ContainsKey(detailCode) && valueChangedCells[detailCode].Contains(i);

						needReset = !hasChangedVal;
					}

					if (needReset)
                    {
						HouseLumpsumCost.SetValue(i, 0);
						HouseSharedLumpsumCost.SetValue(i, 0);
					}
				}

				//  累計を計算
				CalcTotal(i);

				//  資金ショート時: 一時金なら
				if (info.CostShortCode == "0002")
				{
					//  資金ショートをチェック (差額が 負の値か？)
					double diffConstrCost = DiffConstructionCost.GetValue(i);
					if (diffConstrCost <= MinDiffConstructionCost)
					{
						//  資金ショート対応
						CalcSafeCostShort(i, (double)(MinDiffConstructionCost - diffConstrCost));

						//  再計算
						CalcTotal(i);
					}
				}

				//  ピッチカウンタをインクリメント
				pitchCounter++;
			}
		}

		/// <summary>
		/// 戸当たり平均増加額を求める
		/// </summary>
		/// <param name="costIncrBand">The cost incr band.</param>
		/// <returns></returns>
		public long CalcAvgIncreaseCost(double costIncrBand)
		{
			try
			{
				//  タイプがあれば
				if (Types != null)
				{
					//  (総専有面積 or 総共有持分) * 戸数の総和
					double sumCalcSourceValue = 0;
					foreach (TypeMst typeMst in Types)
					{
						sumCalcSourceValue += GetCalcTypeSourceValue(typeMst) * typeMst.RoomNumber;
					}

					//  0 より大きければ
					if (sumCalcSourceValue > 0)
					{
						//  (総専有面積 or 総共有持分) * 増額幅 / 総戸数
						return (long)Math.Floor(costIncrBand * sumCalcSourceValue / GetTotalRoomNumber());
					}
					else
					{
						//  無効
						return long.MinValue;
					}
				}
				else
				{
					//  無効
					return long.MinValue;
				}
			}
			catch (Exception)
			{
				//  無効
				return long.MinValue;
			}
		}

		/// <summary>
		/// 年度内修繕積立金から月額積立金単価を求める
		/// </summary>
		/// <param name="costIncrBand">The cost incr band.</param>
		/// <returns></returns>
		public double CalcHouseMonthReserveCostByYearRepairCost(long repairReserveCost)
		{
			double houseMonthReserveCost = double.MinValue;

			if (repairReserveCost != long.MinValue)
			{
				houseMonthReserveCost = GetMonthReserveCostReverse(repairReserveCost);
			}

			return houseMonthReserveCost;
		}


		#endregion

        /// <summary>
        /// タイプ別積立金を再計算する。
        /// </summary>
        /// <param name="draftInfo"></param>
        /// <param name="lstTypes"></param>
        public void ReCalcKumiaiRepairReservePlanTypeDetails(KumiaiRepairReservePlanDraftInfo draftInfo, TypeMst[] lstTypes,List<Data30Period> dataList, int index)
        {
            KumiaiRepairReservePlanDraft draft = draftInfo.Draft[index];

            List<KumiaiRepairReservePlanTypeDetail> ret = new List<KumiaiRepairReservePlanTypeDetail>();
            #region タイプ別金額計算準備

            //  月額積立金単価、一時金単価をセット
            for (int i = 0; i < draftInfo.Summary.Count(); i++)
            {
                if (dataList[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST)
                {
                    this.MonthlyHouseReserveCost = dataList[i];
                }
                else if (draftInfo.Summary[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST)
                {
                    this.LumpsumHouseReserveCost = dataList[i];
                }
				else if (draftInfo.Summary[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST)
				{
					this.MonthlyHouseSharedReserveCost = dataList[i];
				}
				else if (draftInfo.Summary[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST)
				{
					this.LumpsumHouseSharedReserveCost = dataList[i];
				}
			}


            #endregion

            #region タイプ別金額作成

            //  30 年分ループ
            for (int j = 0; j < COMSKCommon.YEAR100; j++)
            {
                                //  タイプの数だけループ
                for (int i = 0; i < lstTypes.Count(); i++)
                {
                    //  登録用データ
                    KumiaiRepairReservePlanTypeDetail objTypeDetail = new KumiaiRepairReservePlanTypeDetail()
                    {
                        KumiaiRepairReservePlanDraftPid = draft.Pid,
                        InsertUserMstPid = draft.UpdateUserMstPid,
                        UpdateUserMstPid = draft.UpdateUserMstPid,
                    };

                    TypeMst objTypeMst = lstTypes[i];

                    //  会計期インデックスをセット
                    objTypeDetail.AccountPeriodIndex = j;

                    //  タイプ別金額を計算
                    this.Calc(j, objTypeMst);

                    //  値をセット
                    objTypeDetail.YearlyCost = this.YearlyCost;
                    objTypeDetail.MonthlyCost = this.MonthlyCost;
                    objTypeDetail.LumpSum = this.LumpsumCost;

                    //  カウンタをセット
                    objTypeDetail.Counter = i + 1;

                    //  その他パラメータセット
                    objTypeDetail.TypeName = objTypeMst.Typename;
                    objTypeDetail.HouseNumber = objTypeMst.RoomNumber.ToString();

                    //  専有面積・共有持分どちらを使うか
                    double calcValue = 0;
                    if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
                    {
                        //  専有面積
                        calcValue = objTypeMst.AppropriationArea;
                    }
                    else
                    {
                        //  共有持分
                        calcValue = objTypeMst.Ownership;
                    }
                    //20160924 ↓以前からそのようだが、要確認
                    objTypeDetail.AreaPerHoldings = calcValue.ToString();
                    objTypeDetail.OccupancyArea = calcValue.ToString();

                    ret.Add(objTypeDetail);

                }
                
            }
            //画面で使用しない値なので直接draftを書き換える
            draft.TypeDetail = ret.ToArray();
            draft.TypeDetailReCalcFlg = true;
        }


		#endregion

        /// <summary>
        /// タイプ別積立金を再計算する。
        /// </summary>
        /// <param name="draftInfo"></param>
        /// <param name="lstTypes"></param>
        public void ReCalcKumiaiRepairReservePlanTypeDetailsOld(KumiaiRepairReservePlanDraftInfo draftInfo, TypeMst[] lstTypes, List<Data30Period> dataList, int index,string typeCode)
        {
            KumiaiRepairReservePlanDraft draft = draftInfo.Draft[index];

            List<KumiaiRepairReservePlanTypeDetail> ret = new List<KumiaiRepairReservePlanTypeDetail>();
            #region タイプ別金額計算準備

            //  月額積立金単価、一時金単価をセット
            for (int i = 0; i < draftInfo.Summary.Count(); i++)
            {
                if (dataList[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST)
                {
                    this.MonthlyHouseReserveCost = dataList[i];
                }
                else if (draftInfo.Summary[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST)
                {
                    this.LumpsumHouseReserveCost = dataList[i];
                }
				else if (draftInfo.Summary[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST)
				{
					this.MonthlyHouseSharedReserveCost = dataList[i];
				}
				else if (draftInfo.Summary[i].ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST)
				{
					this.LumpsumHouseSharedReserveCost = dataList[i];
				}
			}


            #endregion

            #region タイプ別金額作成

            var typeMstOld = new TypeMst();

            var oldType = draftInfo.Draft[0].TypeDetail.Where(r => r.AccountPeriodIndex == 0).ToList();
            for (int i = 0; i < lstTypes.Count();i++ )
            {
				//lstTypes[i].AppropriationArea = double.Parse(oldType[i].AreaPerHoldings)/100;
				// 自動計算しない場合、事前の保存したデータタイプがそのまま取得する
				var oTObj = oldType.Where(t => t.TypeName == lstTypes[i].Typename).FirstOrDefault();
				if (oTObj == null) continue;

                if (typeCode == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
                {
                    lstTypes[i].AppropriationArea = double.Parse(oTObj.AreaPerHoldings);
                }else
                {
                    lstTypes[i].AppropriationArea = double.Parse(oTObj.AreaPerHoldings)/100;
                    lstTypes[i].Ownership = double.Parse(oTObj.OccupancyArea);
                }
            }

            //  100 年分ループ
            for (int j = 0; j < COMSKCommon.YEAR100; j++)
            {


                //  タイプの数だけループ
                for (int i = 0; i < lstTypes.Count(); i++)
                {
                    //  登録用データ
                    KumiaiRepairReservePlanTypeDetail objTypeDetail = new KumiaiRepairReservePlanTypeDetail()
                    {
                        KumiaiRepairReservePlanDraftPid = draft.Pid,
                        InsertUserMstPid = draft.UpdateUserMstPid,
                        UpdateUserMstPid = draft.UpdateUserMstPid,
                    };

                    TypeMst objTypeMst = lstTypes[i];



                    //  会計期インデックスをセット
                    objTypeDetail.AccountPeriodIndex = j;

                    //  タイプ別金額を計算
                    this.Calc(j, objTypeMst);

                    //  値をセット
                    objTypeDetail.YearlyCost = this.YearlyCost;
                    objTypeDetail.MonthlyCost = this.MonthlyCost;
                    objTypeDetail.LumpSum = this.LumpsumCost;

                    //  カウンタをセット
                    objTypeDetail.Counter = i + 1;

                    //  その他パラメータセット
                    objTypeDetail.TypeName = objTypeMst.Typename;
                    objTypeDetail.HouseNumber = objTypeMst.RoomNumber.ToString();

                    //  専有面積・共有持分どちらを使うか
                    double calcValue = 0;
                    if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
                    {
                        //  専有面積
                        calcValue = objTypeMst.AppropriationArea;
                    }
                    else
                    {
                        //  共有持分
                        calcValue = objTypeMst.Ownership;
                    }
                    //20160924 ↓以前からそのようだが、要確認
                    objTypeDetail.AreaPerHoldings = calcValue.ToString();
                    objTypeDetail.OccupancyArea = calcValue.ToString();

                    ret.Add(objTypeDetail);

                }

            }
            //画面で使用しない値なので直接draftを書き換える
            draft.TypeDetail = ret.ToArray();
            draft.TypeDetailReCalcFlg = true;
        }


            #endregion

		#region Private

        /// <summary>
        /// 共有持分 / 専有面積を判断して値を返す
        /// </summary>
        /// <returns></returns>
        private double GetCalcValue(TypeMst typeMst)
        {
            if (CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
            {
                //  専有面積
                return typeMst.AppropriationArea;
            }
            else
            {
                //  共有持分
                return typeMst.Ownership;
            }
        }

		#region 住戸月額積立金単価変更→累計反映

        /// <summary>
        /// 次年度繰越金のみを計算
        /// </summary>
        /// <param name="index">The index.</param>
        public void CalcDiffConstructionCostOnly(int index, KumiaiLongRepairPlan kumiaiLongRepairPlan) //LINH ADD 20140725 MJC_DEV-165
        {
            for (int i = 1; i < COMSKCommon.YEAR100; i++)
            {
                CarryOverCost.SetValue(i, 0);
            }

            #endregion
            long updateVal = 0;

            //  次年度繰越金を計算
            for (int i = index; i < COMSKCommon.YEAR100; i++)
            {
                //  積立金累計 - 工事費累計
                double val = TotalReserveCost.GetValue(i) - TotalRepairCost.GetValue(i);
                DiffConstructionCost.SetValue(i, val);
            }

            #region ■  修繕積立金繰越額

            //  初年度のみ数字を入れ、他は 0
            if (kumiaiLongRepairPlan.CarryOverCost != long.MinValue)
            {
                updateVal = kumiaiLongRepairPlan.CarryOverCost;
            }
            else
            {
                updateVal = 0;
            }

            CarryOverCost.SetValue(0, updateVal);

        }

		/// <summary>
        /// 次年度繰越金を計算
		/// </summary>
		/// <param name="index">The index.</param>
		private void CalcDiffConstructionCost(int index, bool keepYearMonthReserveCost)
		{
			//  積立金累計を計算
			CalcTotalReserveCost(index, keepYearMonthReserveCost);

			//  積立金累計 - 工事費累計
			double val = TotalReserveCost.GetValue(index) - TotalRepairCost.GetValue(index);
			DiffConstructionCost.SetValue(index, val);
		}

		/// <summary>
		/// 累計積立金を計算
		/// </summary>
		/// <param name="index">The index.</param>
		private void CalcTotalReserveCost(int index, bool keepYearMonthReserveCost)
		{
			//  年度内修繕積立金を計算
			CalcYearRepairReserveCost(index, keepYearMonthReserveCost);

			//  前年の積立金累計
			double val = 0;
			if (index > 0)
			{
				val = TotalReserveCost.GetValue(index - 1);
			}

			//  積立金累計 = 前年度の積立金累計 + 当年の年度内修繕積立金 + 当年の繰越額
			double totalRepairReserveCost = val + YearRepairReserveCost.GetValue(index) + CarryOverCost.GetValue(index);
			TotalReserveCost.SetValue(index, totalRepairReserveCost);
		}

		/// <summary>
		/// 年度内修繕積立金を計算
		/// </summary>
		private void CalcYearRepairReserveCost(int index, bool keepYearMonthReserveCost)
		{
			//  年度内住戸積立金を計算
			CalcYearHouseReserveCost(index, keepYearMonthReserveCost);

			//  年度内修繕積立金を計算
			double val = YearHouseReserveCost.GetValue(index) + GetYearOtherCost(index);
			YearRepairReserveCost.SetValue(index, val);
		}

		/// <summary>
		/// 年度内住戸積立金を再計算
		/// </summary>
		/// <param name="index">The index.</param>
		private void CalcYearHouseReserveCost(int index, bool keepYearMonthReserveCost)
		{
			double sum = 0;
            double yearlyCostsum = 0;
            //double yearlyCostAreaSum = 0;
			double lumpSum = 0;
			foreach (TypeMst typeMst in Types)
			{
				CalcTypeTempCost(index, typeMst);
				sum += typeMst.ReserveCost;
                yearlyCostsum += typeMst.YearlyCost;
				//yearlyCostAreaSum += typeMst.YearlyCostArea;
				lumpSum += typeMst.LumpSum;
			}

			// 画面で年度内住戸積立金を手動変更した場合 →　変更した値を基にして住戸一時負担金を加算
			if (keepYearMonthReserveCost)
            {
				var newVal = YearHouseReserveCostLastInput.GetValue(index) + lumpSum;
				YearHouseReserveCost.SetValue(index, newVal);
			} 
			else
            {
				YearHouseReserveCost.SetValue(index, sum);
			}
			YearMonthReserveCost.SetValue(index, yearlyCostsum);
            YearLumpsumCost.SetValue(index, lumpSum);
			// 計算区分が共有持分の場合面積用の計算を行う
			if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_SHARED)
            {
				this.CalcUnitPriceForArea(index);
				this.CalclumpSumForArea(index);
			}
		}
		/// <summary>
		/// 計算区分が共有持分の場合の専有面積用単価の計算
		/// </summary>
		/// <param name="yearlyCostAreaSum"></param>
		public void CalcUnitPriceForArea(int index)
        {
			Data30Period targetCalcSrc = this.GetTargetUnitPrice();
			var currMonthReserveCost = targetCalcSrc.GetValue(index);
			if (currMonthReserveCost > double.MinValue && this.totalAppropriationArea > 0)
            {
				var monthlyUnit = Math.Round(currMonthReserveCost * this.totalOwnership / this.totalAppropriationArea);  // 四捨五入
				HouseMonthReserveCost.SetValue(index, monthlyUnit);
			}
		}

		public void CalclumpSumForArea(int index)
		{
			Data30Period targetLumsum = this.GetTargetHouseLumpsum();
			var lumpPrice = targetLumsum.GetValue(index);
			if (lumpPrice > double.MinValue && this.totalAppropriationArea > 0)
            {
				var lumpSumUnitPrice = Math.Round(lumpPrice * this.totalOwnership / this.totalAppropriationArea);
				HouseLumpsumCost.SetValue(index, lumpSumUnitPrice);
			}
		}

		/// <summary>
		/// タイプ別一時計算
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="typeMst">The type MST.</param>
		/// <returns></returns>
		private void CalcTypeTempCost(int index, TypeMst typeMst)
		{
			//  年額平均
			double YearAvgCost = CalcTypeMonthAvgReserveCost(index, typeMst, 1);

			// 年額平均・専有面積用
			//double YearAvgCostArea = CalcTypeMonthAvgReserveCostArea(index, typeMst, 1);

			//  一時金
			double lumpsumCost = CalcTypeLumpsumCost(index, typeMst);

			//  計算 (タイプ別積立金月額平均 * 12ヶ月 + タイプ別積立金一時金) * 戸数
			//NOTE: ただし、index=0 のときは FirstYearCalcMonth を使用する
			int calcMonth = 12;
			if (index == 0)
			{
				calcMonth = FirstYearCalcMonth;
			}
            typeMst.ReserveCost = (YearAvgCost * calcMonth / 12 + lumpsumCost) * typeMst.RoomNumber;
            typeMst.YearlyCost = YearAvgCost * calcMonth / 12 * typeMst.RoomNumber;
            //typeMst.YearlyCostArea = YearAvgCostArea * calcMonth / 12 * typeMst.RoomNumber;
			typeMst.LumpSum = lumpsumCost * typeMst.RoomNumber;
		}

        /// <summary>
        /// タイプ別積立金月額平均を計算
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="typeMst">The type MST.</param>
        /// <param name="dev">戻り値の除算値</param>
        /// <returns>偶数丸め後の数値</returns>
		private double CalcTypeMonthAvgReserveCost(int index, TypeMst typeMst,int dev)
		{
            //20200710 MJC_DEV-804 【プログラム修正】【長計システム】積立金計算不正 (No.366)
            //小数点誤差対策のためdecimak型で計算
			//  前年の住戸月額積立金単価
            decimal prevMonthReserveCost = 0;
			Data30Period targetCalcSrc = this.GetTargetUnitPrice();
			decimal currMonthReserveCost = System.Convert.ToDecimal(targetCalcSrc.GetValue(index));
			if (index > 0)
			{
                prevMonthReserveCost = System.Convert.ToDecimal(targetCalcSrc.GetValue(index - 1));
			}
			else
			{
				prevMonthReserveCost = System.Convert.ToDecimal(CurrentHouseMonthlyReserveCost);
			}

			//double prevYearValue = (prevMonthReserveCost * GetCalcTypeSourceValue(typeMst)) * AccountAndRevision;
			//double currYearValue = (currMonthReserveCost * GetCalcTypeSourceValue(typeMst)) * (12 - AccountAndRevision);
			//return CollectFraction(prevYearValue + currYearValue, MonthlyFractionUnit, MonthlyFractionProcess);
			double prevYearValue = CollectFraction(prevMonthReserveCost * System.Convert.ToDecimal(GetCalcTypeSourceValue(typeMst)), MonthlyFractionUnit, MonthlyFractionProcess) * AccountAndRevision;
			double currYearValue = CollectFraction(currMonthReserveCost * System.Convert.ToDecimal(GetCalcTypeSourceValue(typeMst)), MonthlyFractionUnit, MonthlyFractionProcess) * (12 - AccountAndRevision);
            var temp = CollectFraction(350.0m * 80.10m, MonthlyFractionUnit, MonthlyFractionProcess) * AccountAndRevision;
            return Math.Round((prevYearValue + currYearValue) / dev);
		}
		/// <summary>
		/// 自動計算区分が共有持分の場合、専有面積用計算が決算月関係なく一年間内同じ単価で計算
		/// </summary>
		/// <param name="index"></param>
		/// <param name="typeMst"></param>
		/// <param name="dev"></param>
		/// <returns></returns>
		private double CalcTypeMonthAvgReserveCostArea(int index, TypeMst typeMst, int dev)
		{
			// 計算区分が専有面積の場合→計算不要
			if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA) return 0;

			Data30Period targetCalcSrc = this.GetTargetUnitPrice();
			decimal currMonthReserveCost = System.Convert.ToDecimal(targetCalcSrc.GetValue(index));
			double currYearValue = CollectFraction(currMonthReserveCost * System.Convert.ToDecimal(GetCalcTypeSourceValue(typeMst)), MonthlyFractionUnit, MonthlyFractionProcess) * 12;
			return Math.Round(currYearValue / dev);
		}

		public Data30Period GetTargetUnitPrice()
        {
			if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
            {
				return HouseMonthReserveCost;
			} 
			else
            {
				return HouseSharedMonthReserveCost;
			}
        }

		public Data30Period GetTargetHouseLumpsum()
		{
			if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
			{
				return HouseLumpsumCost;
			}
			else
			{
				return HouseSharedLumpsumCost;
			}
		}

		/// <summary>
		/// タイプ別積立金一時金を計算
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="typeMst">The type MST.</param>
		/// <returns></returns>
		private double CalcTypeLumpsumCost(int index, TypeMst typeMst)
		{
			Data30Period targetLumsum = this.GetTargetHouseLumpsum();
			//  住戸一時負担金単価 * 共有持分
			return CollectFraction(System.Convert.ToDecimal(targetLumsum.GetValue(index)) * System.Convert.ToDecimal(GetCalcTypeSourceValue(typeMst)), LumpsumFractionUnit, LumpsumFractionProcess);
            //20200710 MJC_DEV-804 【プログラム修正】【長計システム】積立金計算不正 (No.366)
        }

		#endregion

		#region 累計→住戸一時負担金単価反映

		/// <summary>
		/// 累計から一時負担金単価を再計算する
		/// </summary>
		/// <param name="index">The index.</param>
		private void CalcMonthlyHouseLumpsumCostReverse(int index)
		{
			//  前年の積立金累計額
			double prevTotalReserveCost = 0;
			if (index > 0)
			{
				prevTotalReserveCost = TotalReserveCost.GetValue(index - 1);
			}

			//  今年度の新しい積立金累計 - 前年分で、今年の工事費を出す
			double currTotalRepairCost = TotalReserveCost.GetValue(index) - prevTotalReserveCost;

			//  そこから管理費振り替え等を引くと、年度内住戸積立金が出る
			double yearOtherCost = GetYearOtherCost(index);
			double yearHouseReserveCost = currTotalRepairCost - yearOtherCost;
				

			//  タイプ別月額積立金単価を計算し、作業用データに入れる
			double totalHouseMonthReserveCost = 0;
			foreach (TypeMst typeMst in Types)
			{
				double val = CalcTypeMonthAvgReserveCost(index, typeMst , 12);

				//  val は月額平均なので、12 をかける
				val *= 12;

				//  そのタイプの年度内の積立金が出たので、それを加算
				totalHouseMonthReserveCost += val;
			}

			//  totalHouseMonthReserveCost を引くとタイプ別一時負担金の総和になる
			//  この金額を分配することになる
			double totalLumpsumCost = yearHouseReserveCost - totalHouseMonthReserveCost;

			//  合計の(専有面積 or 共有持分) * 総戸数を計算
			double totalSourceValueMulRoomNumber = 0;
			foreach (TypeMst typeMst in Types)
			{
				totalSourceValueMulRoomNumber += GetCalcTypeSourceValue(typeMst) * typeMst.RoomNumber;
			}

			//  分配金額を totalSourceValueMulRoomNumber で割ると一時負担金単価となる
			if (totalSourceValueMulRoomNumber <= 0)
			{
				totalLumpsumCost = 0;
			}
			else
			{
				totalLumpsumCost /= totalSourceValueMulRoomNumber;
			}

			//  その結果、一時負担金単価となる
			if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
            {
				HouseLumpsumCost.SetValue(index, totalLumpsumCost);
			}
			else
            {
				HouseSharedLumpsumCost.SetValue(index, totalLumpsumCost);
			}
		}

		#endregion

		#region 累計→住戸月額積立金反映

		/// <summary>
		/// 累計→住戸月額積立金反映
		/// </summary>
		/// <param name="fromIndex">From index.</param>
		/// <param name="toIndex">To index.</param>
		private void CalcMonthlyHouseReserveCostReverse(int fromIndex, int toIndex)
		{
			//  fromIndex 年度の積立金累計を取得
			double fromTotalReserveCost = TotalReserveCost.GetValue(fromIndex);

			//  toIndex 年度の積立金累計を取得
			double toTotalReserveCost = TotalReserveCost.GetValue(toIndex);

			//  fromIndex + 1 ～ toIndex 年度の管理費振替や一時金の総価格を計算
			double totalOtherCost = 0;
			for (int i = fromIndex + 1; i <= toIndex; i++)
			{
				//  住戸一時負担金単価を求める
				double monthLumpsumCost = 0;
				foreach (TypeMst typeMst in Types)
				{
					//  タイプ毎の一時金を取得
					double typeLumpsumCost = CalcTypeLumpsumCost(i, typeMst);

					//  足しこむ
					monthLumpsumCost += typeLumpsumCost;
				}

				//  足しこむ
				totalOtherCost += monthLumpsumCost;

				//  管理費振替額等を足しこむ
				totalOtherCost += GetYearOtherCost(i);
			}

			//  toTotalReserveCost から totalOtherCost を引くと、
			//  住戸からの積立金累計となる
			double totalHouseReserveCost = toTotalReserveCost - totalOtherCost;

			//  totalHouseReserveCost から fromTotalReserveCost を引くことで、
			//  純粋に住戸から集める積立金の累計となる
			double totalCollectCost = totalHouseReserveCost - fromTotalReserveCost;

			//  toTotalReserveCost を各年度に割り振る
			//  この金額が年度内の住戸積立金となる
			totalCollectCost /= (toIndex - fromIndex + 1);

			//  fromIndex 年度に対して、住戸月額積立金単価を求める
			double monthReserveCost = GetMonthReserveCostReverse(totalCollectCost);

			//  最後の年度までその値を適用
			for (int i = fromIndex; i < COMSKCommon.YEAR100; i++)
			{
				if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
                {
					HouseMonthReserveCost.SetValue(i, monthReserveCost);
				}
				else
                {
					HouseSharedMonthReserveCost.SetValue(i, monthReserveCost);
				}
			}

		}

		/// <summary>
		/// タイプ別積立金一時金 * 戸数を計算する
		/// </summary>
		/// <returns></returns>
		private double CalcHouseLumpsumCostMulRoomNumber(int index, TypeMst typeMst)
		{
			return CalcTypeLumpsumCost(index, typeMst) * typeMst.RoomNumber;
		}


		/// <summary>
		/// 住戸月額積立金単価を分配する
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="distrCost">The distr cost.</param>
		private void DistributeMonthlyHouseReserveCost(int index, TypeMst typeMst, double distrCost)
		{
			//NOTE: 式変形メモ
			//  前年の住戸月額積立金単価 * 共有持分 * 決算・改定 + x * 共有持分 * (12 - 決算・改定) = distrCost
			//
			//  共有持分 * (前年の住戸月額積立金単価 * 決算・改定 + x * (12 - 決算・改定)) = distrCost
			//
			//  前年の住戸月額積立金単価 * 決算・改定 + x * (12 - 決算・改定) = distrCost / 共有持分
			//
			//  x * (12 - 決算・改定) = (distrCost / 共有持分) - (前年の住戸月額積立金単価 * 決算・改定)
			//
			//  x = ((distrCost / 共有持分) - (前年の住戸月額積立金単価 * 決算・改定)) / (12 - 決算・改定)
			//
			//  x: 当年度の住戸月額積立金単価

			//  月額に直すため、12で割る
			distrCost /= 12;

			//  前年の住戸月額積立金単価
			double prevMonthReserveCost = 0;
			if (index > 0)
			{
				prevMonthReserveCost = HouseMonthReserveCost.GetValue(index - 1);
			}
			else
			{
				prevMonthReserveCost = CurrentHouseMonthlyReserveCost;
			}

			//  計算
			double val = ((distrCost / GetCalcTypeSourceValue(typeMst)) - (prevMonthReserveCost * AccountAndRevision)) / (12 - AccountAndRevision);

			//  小数点二桁まで切り詰め
			double roundedVal = ((int)(val * 100)) / 100.0;
			HouseMonthReserveCost.SetValue(index, roundedVal);

			System.Diagnostics.Debug.WriteLine(string.Format("▼ New HouseMonthReserveCost={0}", roundedVal));
		}

		/// <summary>
		/// タイプの計算元となる値 (専有面積 or 共通持分) を返す
		/// </summary>
		/// <returns></returns>
		private double GetCalcTypeSourceValue(TypeMst typeMst)
		{
			if (CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
			{
				//  専有面積
				return typeMst.AppropriationArea;
			}
			else
			{
				//  共有持分
				return typeMst.Ownership;
			}
		}

		/// <summary>
		/// タイプの計算元となる値 (専有面積 or 共通持分) の総和を返す
		/// </summary>
		/// <returns></returns>
		private double GetSumCalcTypeSourceValue()
		{
			if (CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
			{
				//  専有面積
				return (from item in Types
						select item.AppropriationArea).Sum();
			}
			else
			{
				//  共有持分
				return (from item in Types
						select item.Ownership).Sum();
			}
		}

        //20200710 MJC_DEV-804 【プログラム修正】【長計システム】積立金計算不正 (No.366)
		/// <summary>
		/// 端数処理・端数金額を元に数値を編集して返す
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="fracUnit">The frac unit.</param>
		/// <param name="fracProcess">The frac process.</param>
		/// <returns></returns>
		private double CollectFraction(decimal value, string fracUnit, string fracProcess)
		{
			double newValue = 0;
			decimal workVal = 1m;
            //value = (int)value;
            //value = Math.Floor(value);
			//  単位
			if (fracUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_ONE)
			{
				//  1 円
				workVal = 1;
			}
			else if (fracUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_TEN)
			{
				//  10 円
				workVal = 10;
			}
			else if (fracUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_HUNDRED)
			{
				//  100 円
				workVal = 100;
			}
			else if (fracUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_THOUSAND)
			{
				//  1000 円
				workVal = 1000;
			}

			if (fracProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND)
			{
				//  四捨五入
				newValue = (double)(((int)(value / workVal + 0.5m)) * workVal);
			}
			else if (fracProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_UP)
			{
				//  切り上げ
                newValue = (double)(Math.Ceiling(value / workVal) * workVal);
			}
			else if (fracProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_DOWN)
			{
				//  切り捨て
                newValue = (double)((Math.Floor(value / workVal)) * workVal);
                //20200710 MJC_DEV-804 【プログラム修正】【長計システム】積立金計算不正 (No.366)
			}

			//  OK
			//System.Diagnostics.Debug.WriteLine(string.Format("★ value={0}, newVal={1}", value, newValue));
			return newValue;
		}

		/// <summary>
		/// 全タイプの総戸数を返す
		/// </summary>
		/// <returns></returns>
		private long GetTotalRoomNumber()
		{
			return (from item in Types
					select item.RoomNumber).Sum();
		}

		#endregion

		#region その他

		/// <summary>
		/// 指定年度の管理費振替額等の総計を取得する
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		private double GetYearOtherCost(int index)
		{
			return TransfarCost.GetValue(index) +
				OtherInCost.GetValue(index) +
				OtherOutCost.GetValue(index) +
				OtherOutTransfarCost.GetValue(index);
		}

		/// <summary>
		/// 住戸月額積立金単価を逆計算
		/// </summary>
		/// <param name="distrCost">The distr cost.</param>
		/// <returns></returns>
		private double GetMonthReserveCostReverse(double distrCost)
		{
			double ret = 0;

			//  全てのタイプを回す
			double total = 0;
			foreach (TypeMst typeMst in Types)
			{
				//  (専有面積 or 共有持分) * 戸数を足しこむ
				total += GetCalcTypeSourceValue(typeMst) * typeMst.RoomNumber;
			}

			//  total で distrCost を割る
			if (total > 0)
			{
				ret = distrCost / total;

				//  月額なのでさらに 12 で割る
				ret /= 12;
			}

			//  OK
			return ret;
		}

		#endregion

		#region 資金ショート対応

		/// <summary>
		/// 資金ショート時、一時金を押し上げる
		/// </summary>
		/// <param name="index">The index.</param>
		private void CalcSafeCostShort(int index, double cost)
		{
			//  SUM ((専有面積 or 共有持分) * 戸数) を出す
			double sum = 0;
			foreach (TypeMst typeMst in Types)
			{
				sum += this.GetCalcTypeSourceValue(typeMst) * typeMst.RoomNumber;
			}

			//  sum で金額を割る
			cost /= sum;

			//  それが一時金となる
			if (this.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
            {
				HouseLumpsumCost.SetValue(index, cost);
			}
			else
            {
				HouseSharedLumpsumCost.SetValue(index, cost);
			}

		}

		#endregion

		public double CalcColumnWidth(double chartWidth, COMSKCommon.RepairReserveViewMode vMode)
        {
			////  チャートサイズ = 1041 の場合、グラフ表示エリアは 871
			////  その際、フィットするカラムの幅は 28
			double k = 1.2;
			//double masterRatio = 2980.0 / 3330.0;

			////  現在のクライアントサイズからグラフ表示エリアサイズを計算
			//double areaWidth = chartWidth * masterRatio * k;

			////  比率からカラムの幅を計算
			//return areaWidth * 28.0 / 2980.0;
			// chart left = 9%, right = 91%
			// データグリッドがチャートより1年目の真ん中からスタートする
			var diffWidth = chartWidth * 0.92 * k / 60;
			return (chartWidth * 0.92 - diffWidth) / 60;
		}

		public double CalcGridLeftWidth(double chartWidth)
        {
			// chart left = 9%, right = 91%
			var diffWidth = chartWidth * 0.92 / 60 / 2;
			return chartWidth * 0.08 + diffWidth;
		}

		#endregion

		public double CalcReserveGridColWidth(int chartWidth)
        {
			//  チャートサイズ = 1041 の場合、グラフ表示エリアは 871
			//  その際、フィットするカラムの幅は 28
			//chartArea1.InnerPlotPosition.Width = 93.5F;
			//chartArea1.InnerPlotPosition.X = 6F;
			double k = 0.982;
			double masterRatio = 94.5 / 100;

			//  現在のクライアントサイズからグラフ表示エリアサイズを計算
			double areaWidth = chartWidth * masterRatio * k;

			//  比率からカラムの幅を計算
			return areaWidth / 60;
		}

		public int CalcReserveLeftPartWidth(int chartWidth, double colWidth)
        {
			//chartArea1.InnerPlotPosition.Width = 89.5F;
			//chartArea1.InnerPlotPosition.X = 10F;
			double k = 0.985;
			double masterRatio = 5.0 / 100;
			//  現在のクライアントサイズからグラフ表示エリアサイズを計算
			double areaWidth = chartWidth * masterRatio * k;
			//  比率からカラムの幅を計算
			return (int)(areaWidth + colWidth / 2);
		}

		public int CalcReserveChartWidth(int chartWidth, COMSKCommon.RepairReserveViewMode vMode)
        {
			if (vMode == COMSKCommon.RepairReserveViewMode.Standard)
            {
				return (int)(chartWidth * rateDown);
			}

			return (int)(chartWidth * rateUp);
		}

		public int CalcNewScrollPos(int currPos, COMSKCommon.RepairReserveViewMode nextMode)
        {
			if (nextMode == COMSKCommon.RepairReserveViewMode.Standard)
			{
				return (int)(currPos * rateDown);
			}

			return (int)(currPos * rateUp);
		}

		public int CalcReserveWidthDiff(COMSKCommon.RepairReserveViewMode vMode, bool isReserveCtr = false)
		{
			if (vMode == COMSKCommon.RepairReserveViewMode.Standard)
			{
				// 10ピクセル調整
				if (isReserveCtr) return (int)(COMSKCommon.RESERVE_DIFF_WITH_CHART * rateDown) - 6;

				return (int)(COMSKCommon.RESERVE_DIFF_WITH_CHART * rateDown);
			}

			return COMSKCommon.RESERVE_DIFF_WITH_CHART;
		}

		public void SetLastYearHouseReserveCost(int index)
        {
			YearHouseReserveCostLastInput.SetValue(index, YearHouseReserveCost.GetValue(index));
		}
	}
}
