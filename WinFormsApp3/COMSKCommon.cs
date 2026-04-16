using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using coms.COMSKService;
using coms.COMMONService;
using coms.COMMON;
using coms.COMSK.ui.common;

namespace coms.COMSK.common
{
	/// <summary>
	/// ID と名前を保持し、ToString で名前を返すクラス
	/// </summary>
	public class TypeValue<T>
	{
		public T Key { get; set; }
		public string Name { get; set; }

		public TypeValue(T key, string name)
		{
			Key = key;
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// 独自のパラメータ型を保持する EventArgs
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CustomEventArgs<T> : EventArgs
	{
		public T Param { get; set; }

		public CustomEventArgs(T t)
		{
			Param = t;
		}
	}

	/// <summary>
	/// ドラッグドロップ完了時イベント引数
	/// </summary>
	public class DragCompletedEventArgs : EventArgs
	{
		public DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo From { get; set; }
		public DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo To { get; set; }

		public int FromColumnIndex { get; set; }
		public int ToColumnIndex { get; set; }

		public List<LongRepairPlanData> DataList { get; set; }

	}

	/// <summary>
	/// CoMS-K 共通クラス
	/// </summary>
	public static class COMSKCommon
	{
		public const int YEAR30 = 30;
		public const int YEAR60 = 60;
		public const int YEAR100 = 100;  // 積立金データ100年分作成
		public const int MAX_VISIBLE_YEAR = 60;  // 画面にの表示年数
		// グラフの広さがグリッドより差があるので
		// スクロール同期為差部分がグリッドに増加
		public const int RESERVE_DIFF_WITH_CHART = 65;
		#region 列挙型

		/// <summary>
		/// 工事分類区分け
		/// </summary>
		public enum ConstructionType
		{
			/// <summary>
			/// 仮設
			/// </summary>
			Temp = 1,
			/// <summary>
			/// 建築
			/// </summary>
			Building = 2,
			/// <summary>
			/// 設備
			/// </summary>
			Equipment = 3,
			/// <summary>
			/// 外構
			/// </summary>
			Outer = 4,
			/// <summary>
			/// その他
			/// </summary>
			Other = 5,

		}

		/// <summary>
		/// 修繕積立金表示モード
		/// </summary>
		public enum RepairReserveViewMode
		{
			/// <summary>
			/// 標準
			/// </summary>
			Standard = 0,
			/// <summary>
			/// 拡大
			/// </summary>
			Full,
		}


		#endregion

		#region 各種フラグ

		#region 履歴ありフラグ
		public const string HAS_HISTORY_FLG_ON = "01";
		public const string HAS_HISTORY_FLG_OFF = "00";
		#endregion

		#region 履歴取得フラグ
		public const string TAKE_HISTORY_FLG_ON = "01";
		public const string TAKE_HISTORY_FLG_OFF = "00";
		#endregion

		#region 選択フラグ
		public const string SELECT_FLG_ON = "01";
		public const string SELECT_FLG_OFF = "00";
		#endregion

		#region 長計取り込みフラグ
		public const string IMPORT_LONG_REPAIR_PLAN_FLG_ON = "01";
		public const string IMPORT_LONG_REPAIR_PLAN_FLG_OFF = "00";
		#endregion

		#region 以後反映フラグ
		public const string APPLY_ALL_LONG_REPAIR_PLAN_FLG_ON = "01";
		public const string APPLY_ALL_LONG_REPAIR_PLAN_FLG_OFF = "00";
		#endregion

		#region ドラッグ済みフラグ
		public const string DRAGGED_FLG_ON = "01";
		public const string DRAGGED_FLG_OFF = "00";
		#endregion

		#region 取込フラグ
		/// <summary>
		/// 未取込
		/// </summary>
		public const string IMPORT_FLG_NONE = "00";
		/// <summary>
		/// 作成基準取込
		/// </summary>
		public const string IMPORT_FLG_REPAIR_PLAN = "01";
		/// <summary>
		/// 修繕履歴取込
		/// </summary>
		public const string IMPORT_FLG_REPAIR_HISTORY = "02";
		#endregion

		#endregion

		#region コードマスタコード区分
		/// <summary>
		/// 単位コードマスタ
		/// </summary>
		public const string COMSK_UNIT_TYPE_CODE = "n001";
		/// <summary>
		/// 長計タイプマスタ
		/// </summary>
		public const string COMSK_LONGREPAIRPLAN_TYPE_CODE = "n002";
		/// <summary>
		/// 長計状態マスタ
		/// </summary>
		public const string COMSK_LONGREPAIRPLAN_STATUS_CODE = "n003";
		/// <summary>
		/// 長計提出先マスタ
		/// </summary>
		public const string COMSK_LONGREPAIRPLAN_PRESENTATION_CODE = "n004";
		/// <summary>
		/// 受注区分マスタ
		/// </summary>
		public const string COMSK_ORDER_CODE = "n005";
		/// <summary>
		/// 会計区分マスタ
		/// </summary>
		public const string COMSK_ACCOUNT_CODE = "n006";
		/// <summary>
		/// 変更理由マスタ
		/// </summary>
		public const string COMSK_UPDATE_REASON_CODE = "n007";
		/// <summary>
		/// 監理会社マスタ
		/// </summary>
		public const string COMSK_MANAGE_COMPANY_CODE = "n008";
		/// <summary>
		/// 長計取込マスタ
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_IMPORT_CODE = "n009";
		/// <summary>
		/// 点検種別
		/// </summary>
		public const string COMSK_INSPECTION_CODE = "n010";
		/// <summary>
		/// 劣化の状況／指摘
		/// </summary>
		public const string COMSK_CONDITION_CODE = "n011";
		/// <summary>
		/// 修繕積立金明細種別
		/// </summary>
		public const string COMSK_RESERVE_PLAN_DETAIL_CODE = "n013";
		/// <summary>
		/// 資金ショートした場合の処理マスタ
		/// </summary>
		public const string COMSK_COST_SHORT_CODE = "n014";
		/// <summary>
		/// 管理費等振り替えマスタ
		/// </summary>
		public const string COMSK_TRANSFAR_COST_CODE = "n015";
        /// <summary>
        /// 集約状態
        /// </summary>
        public const string COMSK_INTENSIVEFLG_CODE = "n016";


		#endregion

		#region コードマスタ定数
		/// <summary>
		/// 消費税率
		/// </summary>
		public const string COMSK_TAXMST_CONSUMPTION = "0001";
		/// <summary>
		/// 物価上昇率
		/// </summary>
		public const string COMSK_TAXMST_PRICEINCREASE = "0002";
		/// <summary>
		/// 管理費振り替え等: 開始年度のみ
		/// </summary>
		public const string COMSK_TRANSFAR_COST_STARTING = "0001";
		/// <summary>
		/// 管理費振り替え等: 毎年
		/// </summary>
		public const string COMSK_TRANSFAR_COST_EVERYYEAR = "0002";
		/// <summary>
		/// 管理費振り替え等: ピッチ指定
		/// </summary>
		public const string COMSK_TRANSFAR_COST_PITCH = "0003";
		/// <summary>
		/// 長計金額表示単位: 千円
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_THOUSAND = "0010";
		/// <summary>
		/// 長計金額表示単位: 万円
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_10_THOUSAND = "0020";

		/// <summary>
		/// 長計タイプ: 新規
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_TYPE_NEW = "0001";
		/// <summary>
		/// 長計タイプ: 見直
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_TYPE_REEXAM = "0002";
		/// <summary>
		/// 長計タイプ: 決算
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_TYPE_ACCOUNT = "0003";

		/// <summary>
		/// 長計ステータス: 案
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_STATUS_DRAFT = "0001";
		/// <summary>
		/// 長計ステータス: 長計確定
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_STATUS_DECIDE_REPAIRPLAN = "0002";
		/// <summary>
		/// 長計ステータス: 積立金確定
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_STATUS_DECIDE_RESERVEPLAN = "0003";
		/// <summary>
		/// 長計ステータス: 没
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_STATUS_DROPPED = "0004";
        //20140812 y-hoshino start
        /// <summary>
        /// 長計ステータス: 決算確定
        /// </summary>
        public const string COMSK_LONG_REPAIR_PLAN_STATUS_SETTLE_RESERVEPLAN = "0005";
        //20140812 y-hoshino end

		/// <summary>
		/// 長計変更理由: 金額変更のため
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_UPDATE_REASON_MODIFY_COST = "0001";
		/// <summary>
		/// 長計変更理由: 実施間隔変更のため
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_UPDATE_REASON_DRAGGED = "0002";
		/// <summary>
		/// 長計変更理由: 修繕基準取込のため
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_UPDATE_REASON_IMPORT_REPAIR_PLAN = "0010";
		/// <summary>
		/// 長計変更理由: 修繕履歴実績取込のため
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_UPDATE_REASON_IMPORT_REPAIR_HISTORY = "0011";

		/// <summary>
		/// 長計自動計算方法: 専有面積
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA = "0010";
		/// <summary>
		/// 長計自動計算方法: 共有持分
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_SHARED = "0020";

		/// <summary>
		/// 長計自動計算端数単位: 1 円
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_ONE = "0010";
		/// <summary>
		/// 長計自動計算端数単位: 10 円
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_TEN = "0020";
		/// <summary>
		/// 長計自動計算端数単位: 100 円
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_HUNDRED = "0030";
		/// <summary>
		/// 長計自動計算端数単位: 1000 円
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_THOUSAND = "0040";

		/// <summary>
		/// 長計自動計算端数処理: 四捨五入
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND = "0010";
		/// <summary>
		/// 長計自動計算端数処理: 切り上げ
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_UP = "0020";
		/// <summary>
		/// 長計自動計算端数処理: 切り捨て
		/// </summary>
		public const string COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_DOWN = "0030";

		/// <summary>
		/// 積立金計画明細種別: 積立金累計
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TOTAL_RESERVE_COST = "0100";
		/// <summary>
        /// 積立金計画明細種別: 次年度繰越金
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_DIFF_CONSTRUCTION_COST = "0200";
		/// <summary>
		/// 積立金計画明細種別: 年度内修繕積立金
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_REPAIR_RESERVE_COST = "0300";
		/// <summary>
		/// 積立金計画明細種別: 年度内住戸積立金
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_HOUSE_RESERVE_COST = "0450";
		/// <summary>
		/// 積立金計画明細種別: 管理費振替額
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TRANSFAR_COST = "0500";
		/// <summary>
		/// 積立金計画明細種別: その他収入金
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_IN_COST = "0600";
		/// <summary>
		/// 積立金計画明細種別: その他支出金
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_COST = "0650";
		/// <summary>
		/// 積立金計画明細種別: その他会計口への繰入金
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_OTHER_OUT_TRANSFAR_COST = "0700";
		/// <summary>
		/// 積立金計画明細種別: 住戸月額積立金単価（専有面積）
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_MONTH_RESERVE_COST = "0800";
		/// <summary>
		/// 積立金計画明細種別: 住戸月額積立金単価（共有持分）
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_MONTH_RESERVE_COST = "0850";
		/// <summary>
		/// 積立金計画明細種別: 住戸一時負担金単価（専有面積）
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_LUMPSUM_COST = "0900";
		/// <summary>
		/// 積立金計画明細種別: 住戸一時負担金単価（共有持分）
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_HOUSE_SHARED_LUMPSUM_COST = "0950";
		/// <summary>
		/// 積立金計画明細種別: 年度内月額積立金
		/// </summary>
		public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_MONTH_RESERVE_COST = "1000";
        /// <summary>
        /// 積立金計画明細種別: 年度内一時負担金
        /// </summary>
        public const string COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_YEAR_LUMPSUM_COST = "1100";
		/// <summary>
		/// 共有持分の場合㎡用のタイトルにこれを先頭に付ける
		/// </summary>
		public static readonly string RESERVE_PLAN_DETAIL_CODE_PREFIX = "【参考】";
		/// <summary>
		/// 計画のpresentationCode「n004」
		/// </summary>
		public const string COMSK_PRESENTCODE_OWNER = "0005";
		/// <summary>
		/// 計画表の表紙シートにあるロゴ名
		/// </summary>
		public const string COMSK_PLAN_REPORT_MJC_LOGO_NAME = "Picture 34";
		#endregion

		#region コードマスタ読み込み

		/// <summary>
		/// 単位コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetUnitCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_UNIT_TYPE_CODE);
		}

		/// <summary>
		/// 長計タイプコードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetLongRepairPlanTypeCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_LONGREPAIRPLAN_TYPE_CODE);
		}

		/// <summary>
		/// 長計状態コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetLongRepairPlanStatusCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_LONGREPAIRPLAN_STATUS_CODE);
		}

		/// <summary>
		/// 長計提出先コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetLongRepairPlanPresentationCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_LONGREPAIRPLAN_PRESENTATION_CODE);
		}

		/// <summary>
		/// 受注区分コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetOrderCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_ORDER_CODE);
		}

		/// <summary>
		/// 会計区分コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetAccountCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_ACCOUNT_CODE);
		}

		/// <summary>
		/// 変更理由コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetUpdateReasonCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_UPDATE_REASON_CODE);
		}

		/// <summary>
		/// 監理会社コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetManageCompanyCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_MANAGE_COMPANY_CODE);
		}

		/// <summary>
		/// 長計取込コードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetLongRepairPlanImportCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_LONG_REPAIR_PLAN_IMPORT_CODE);
		}

		/// <summary>
		/// 修繕積立金明細種別マスタ
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetReservePlanDetailCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_RESERVE_PLAN_DETAIL_CODE);
		}

		/// <summary>
		/// 資金ショートした場合の処理マスタ
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetCostShortCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_COST_SHORT_CODE);
		}

		/// <summary>
		/// 管理費等振り替えタイプコードマスタを読み込む
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetTransfarCostCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_TRANSFAR_COST_CODE);
		}
		/// <summary>
		/// 点検種別コードマスタ
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetInspectionCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_INSPECTION_CODE);
		}

		/// <summary>
		/// 劣化の状況コードマスタ
		/// </summary>
		/// <returns></returns>
		public static CodeMst[] GetConditionCodeMst()
		{
			COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
			return commonBL.SearchCodeMstByCode(COMSK_CONDITION_CODE);
		}
        /// <summary>
        /// 集約状態
        /// </summary>
        public static CodeMst[] GetIntensiveFlgCode()
        {
            COMMON.business.CommonBL commonBL = new coms.COMMON.business.CommonBL();
            return commonBL.SearchCodeMstByCode(COMSK_INTENSIVEFLG_CODE);

        }


		#endregion

		#region その他文字列定数

		/// <summary>
		/// 工事名称新規作成
		/// </summary>
		public const string CONSTRUCTION_NEW_ITEM = "(新規)";

		/// <summary>
		/// 新規作成時の変更履歴テキスト
		/// </summary>
		public const string UPDATE_REASON_NEW = "新規作成";

		/// <summary>
		/// 仮箱から追加した際の変更履歴テキスト
		/// </summary>
		public const string UPDATE_REASON_APPLY_TEMPBOX = "参考基準から追加";

		/// <summary>
		/// 修繕履歴集約の仕様
		/// </summary>
		public const string REPAIR_HISTORY_GROUP_SPEC_TEXT = "その他(修繕履歴集約)";

		/// <summary>
		/// コードマスタでその他を示すコード
		/// </summary>
		public const string CODEMST_INVALID_CODE = "9999";

		/// <summary>
		/// スタッフ種別: 営業
		/// </summary>
		public const string STAFF_TYPE_FRONT = "10";

		/// <summary>
		/// スタッフ種別: 営業 サブ
		/// </summary>
		public const string STAFF_TYPE_FRONT_SUB = "15";

		/// <summary>
		/// スタッフ種別: 長計作成担当
		/// </summary>
		public const string STAFF_TYPE_CREATOR = "16";


		#region タグ

		public const string TAG_DRAGGABLE_CELL = "LongtermRepairPlan_DraggableCell";
		public const string TAG_ADD_CONTROL_CALENDARYEAR = "LongtermRepairPlan_CalendarYear";

		#endregion

		/// <summary>
		/// 選択全解除メッセージ
		/// </summary>
		public const string CONFIRM_DESELECT_ALL = "全ての選択を取り消してもよろしいですか？";

		#endregion

		#region 凡例用色定数

		#region 長計
		public static readonly Color LONGTERM_REPAIR_PLAN_COLOR_DEFAULT = Color.White;
		public static readonly Color LONGTERM_REPAIR_PLAN_COLOR_MODIFIED_AT_LONGTERM_REPAIR_PLAN = Color.PaleGreen;
		public static readonly Color LONGTERM_REPAIR_PLAN_COLOR_DRAGGED = Color.LimeGreen;
		public static readonly Color LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_PLAN = Color.LightSkyBlue;
		public static readonly Color LONGTERM_REPAIR_PLAN_COLOR_IMPORT_REPAIR_HISTORY = Color.Plum;
		public static readonly Color LONGTERM_REPAIR_PLAN_COLOR_NO_REASON = Color.Salmon;
		#endregion

		#region 作成基準との比較
		public static readonly Color DIFF_COLOR_MODIFIED = Color.PaleGreen;
		public static readonly Color DIFF_COLOR_IN_STANDARD = Color.LightSkyBlue;
		public static readonly Color DIFF_COLOR_IN_KUMIAI = Color.LightSalmon;
		public static readonly Color DIFF_COLOR_DELETED = Color.LightGray;
		#endregion

		#endregion

		#region 工事ツリー関係

		/// <summary>
		/// 工事ツリー一覧から指定した PID の ConstructionTree を検索する
		/// </summary>
		/// <param name="constructionTree">The construction tree.</param>
		/// <param name="pid">The pid.</param>
		/// <returns></returns>
		public static ConstructionTree FindConstructionTree(ConstructionTree[] constructionTree, long pid)
		{
			ConstructionTree ret = null;

			ConstructionTree[] itemList = (from item in constructionTree
										   where item.Pid == pid
										   select item).ToArray<ConstructionTree>();

			if (itemList != null)
			{
				if (itemList.Length > 0)
				{
					ret = itemList[0];
				}
			}

			return ret;
		}

		//public static void SetConstructionItemToComboBox(ComboBox cmb, ConstructionTree[] list, long pid)
		//{
		//    SetConstructionItemToComboBox(cmb, list, pid, long.MinValue);
		//}

		/// <summary>
		/// 工事ツリーの項目一覧をコンボボックスに設定する
		/// </summary>
		/// <param name="cmb">The CMB.</param>
		/// <param name="list">The list.</param>
		/// <param name="pid">The pid.</param>
		public static void SetConstructionItemToComboBox(ComboBox cmb, ConstructionTree[] list, long pid, long rewritePid)
		{
			cmb.Items.Clear();

			//  空白を挿入
			cmb.Items.Add(new TypeValue<long>(long.MinValue, string.Empty));

			//  一覧を追加
			if (list != null)
			{
				foreach (ConstructionTree item in list)
				{
					string name = item.Name;
					if (item.IsInTempBox)
					{
						name = "●" + name;
					}
					cmb.Items.Add(new TypeValue<long>(item.Pid, name));
				}
			}

			//  新規を追加
			cmb.Items.Add(new TypeValue<long>(0, COMSKCommon.CONSTRUCTION_NEW_ITEM));

			//  rewritePid が MinValue でなければ
			if (rewritePid != long.MinValue)
			{
				//  指定アイテムを選択
				SelectComboBox<long>(cmb, rewritePid);
			}
			else
			{
				//  pid が 0 なら
				if (pid == 0)
				{
					//  (新規) を選択
					cmb.SelectedIndex = cmb.Items.Count - 1;
				}
				//  それ以外
				else
				{
					//  先頭を選択
					cmb.SelectedIndex = 0;
				}
			}
		}

		/// <summary>
		/// 工事ツリーの親の項目一覧をコンボボックスに設定する
		/// </summary>
		/// <param name="cmb">The CMB.</param>
		/// <param name="ignorePid">The ignore pid.</param>
		/// <param name="list">The list.</param>
		public static void SetConstructionParentItemToComboBox(ComboBox cmb, long ignorePid, ConstructionTree[] list)
		{
			cmb.Items.Clear();

			//  空白を挿入
			cmb.Items.Add(new TypeValue<long>(long.MinValue, string.Empty));

			//  一覧を追加
			if (list != null)
			{
				foreach (ConstructionTree item in list)
				{
					//  Pid が ignorePid なら除外
					if (item.Pid != ignorePid)
					{
						string name = item.Name;
						if (item.IsInTempBox)
						{
							name = "●" + name;
						}
						cmb.Items.Add(new TypeValue<long>(item.Pid, name));
					}
				}
			}

			//  先頭を選択
			cmb.SelectedIndex = 0;
		}

		/// <summary>
		/// 親指定の修繕工事内容Cmb一覧をセットする
		/// </summary>
		/// <param name="cmb">The CMB.</param>
		/// <param name="ignorePid">The ignore pid.</param>
		/// <param name="list">The list.</param>
		public static void SetRepairConstructionContentParentItemToComboBox(ComboBox cmb, long ignorePid, ConstructionTree[] list)
		{
			cmb.Items.Clear();

			//  無効な値をセット
			cmb.Items.Add(new TypeValue<long>(long.MinValue, string.Empty));

			//  一覧を追加
			if (list != null)
			{
				foreach (ConstructionTree item in list)
				{
					//  Pid が ignorePid なら除外
					if (item.Pid != ignorePid)
					{
						cmb.Items.Add(new TypeValue<long>(item.Pid, item.Name));
					}
				}
			}

			//  先頭を選択
			cmb.SelectedIndex = 0;
		}

		#endregion

		#region コンボボックスユーティリティ

		/// <summary>
		/// コンボボックスの選択を設定する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmb">The CMB.</param>
		/// <param name="value">The value.</param>
		public static void SelectComboBox<T>(ComboBox cmb, T value)
		{
			foreach (var item in cmb.Items)
			{
				if (item is TypeValue<T>)
				{
					TypeValue<T> val = item as TypeValue<T>;
					if (val.Key.Equals(value))
					{
						cmb.SelectedItem = item;
						break;
					}
				}
			}
		}

		/// <summary>
		/// コンボボックスの選択を取得する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmb">The CMB.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public static T GetSelectedComboBox<T>(ComboBox cmb, T defaultValue)
		{
			T ret = defaultValue;

			if (cmb.SelectedItem is TypeValue<T>)
			{
				TypeValue<T> val = cmb.SelectedItem as TypeValue<T>;
				ret = val.Key;
			}

			//  OK
			return ret;
		}

		/// <summary>
		/// コードマスタ一覧をコンボボックスに設定する
		/// </summary>
		/// <param name="cmb">The CMB.</param>
		/// <param name="codeList">The code list.</param>
		internal static void SetCodeMstToComboBox(ComboBox cmb, CodeMst[] codeList)
		{
			cmb.Items.Clear();
			foreach (CodeMst codeMst in codeList)
			{
				cmb.Items.Add(new TypeValue<string>(codeMst.Number, codeMst.Title));
			}
		}

		/// <summary>
		/// チェックコンボにコード一覧を設定する
		/// </summary>
		/// <param name="cmb">The CMB.</param>
		/// <param name="codeList">The code list.</param>
		public static void SetCodeMstToCheckedComboBox(coms.COMMON.ui.CheckedComboBox cmb, CodeMst[] codeList)
		{
			cmb.Items.Clear();
			foreach (CodeMst code in codeList)
			{
				cmb.Items.Add(new coms.COMMON.ui.CCBoxItem(code.Title, code.Title));
			}
		}

		/// <summary>
		/// 選択フィルタコンボボックスのアイテム設定
		/// </summary>
		/// <param name="cmbFilter">The CMB filter.</param>
		public static void BindSelectionToCheckboxList(coms.COMMON.ui.CheckedComboBox cmbFilter)
		{
			cmbFilter.Items.Add(new coms.COMMON.ui.CCBoxItem("選択済", "1"));
			cmbFilter.Items.Add(new coms.COMMON.ui.CCBoxItem("未選択", "2"));
			cmbFilter.DisplayMember = "Name";
		}
		/// <summary>
		/// 選択フィルタコンボボックスのアイテム設定
		/// </summary>
		/// <param name="cmbFilter">The CMB filter.</param>
		public static void BindSelectionToCheckboxList(DevExpress.XtraEditors.CheckedComboBoxEdit cmbEdit)
		{
            var CCItemList = new List<coms.COMMON.ui.CCBoxItem>();
            //cmbEdit.Properties.Items.Add((new DevExpress.XtraEditors.Controls.ListBoxItem(1,"選択済"));


            //cmbEdit.Properties.Items.Add(new coms.COMMON.ui.CCBoxItem("選択済", "1"));
            //cmbEdit.Properties.Items.Add(new coms.COMMON.ui.CCBoxItem("未選択", "2"));
            //cmbEdit.Properties.DisplayMember = "Name";
            //cmbEdit.Properties.ValueMember = "Value";

		}
		/// <summary>
		/// 指定したチェックコンボボックスの、選択されている項目を文字列の配列で返す
		/// </summary>
		/// <param name="cmbBox">The CMB box.</param>
		public static string[] GetSelectedItemsAsArrayOfCheckedCombobox(coms.COMMON.ui.CheckedComboBox cmbBox)
		{
			string selectedValues = cmbBox.GetSelectedValues();

			//  セパレータでばらす
			string[] list = selectedValues.Split(new string[] { cmbBox.ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
            
			//  ' を取り除く
			for (int i = 0; i < list.Length; i++)
			{
				list[i] = list[i].Replace("'", "");
			}

			//  そのリストを返す
			return list;
		}

		public static string GetSelectedItemsAsStrCheckedCombobox(coms.COMMON.ui.CheckedComboBox cmbBox)
		{
			string selectedValues = cmbBox.GetSelectedValues();

			// セパレータでばらす
			string[] list = selectedValues.Split(
				new string[] { cmbBox.ValueSeparator },
				StringSplitOptions.RemoveEmptyEntries);

			// ' を取り除く
			for (int i = 0; i < list.Length; i++)
			{
				list[i] = list[i].Replace("'", "");
			}

			// DevExpress と同じように , で連結して返す
			return string.Join(",", list);
		}

		/// <summary>
		/// 指定したチェックコンボボックスの、選択されている項目を文字列の配列で返す
		/// </summary>
		/// <param name="cmbBox">The CMB box.</param>
		public static string[] GetSelectedItemsAsArrayOfCheckedCombobox(DevExpress.XtraEditors.CheckedComboBoxEdit cmbBox)
        {
            string selectedValues = cmbBox.Properties.GetCheckedItems().ToString();

            //  セパレータでばらす
            string[] list = selectedValues.Split(new string[] { cmbBox.Properties.SeparatorChar.ToString() }, StringSplitOptions.RemoveEmptyEntries);

            //  ' を取り除く
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = list[i].Replace("'", "").Replace(" ", "");
            }

            //  そのリストを返す
            return list;
        }

		#endregion

		#region 文字列ユーティリティ

		/// <summary>
		/// 指定した文字列の改行コードを表示用に直す
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string ReplaceNewline(string text)
		{
			return text.Replace("\n", Environment.NewLine);
		}

		/// <summary>
		/// A numerical text is edited into comma separated values. 
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns></returns>
		public static string EditCommaSeparate(string s)
		{
			long val = Helper.ParseToLong(s);
			if (val == long.MinValue)
			{
				return s;
			}
			else
			{
				return string.Format("{0:N0}", val);
			}
		}

		/// <summary>
		/// 文字列を double に変換して返す
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns></returns>
		public static string EditDoubleValue(string s)
		{
			double val = Helper.ParseToDouble(s);
			if (val == double.MinValue)
			{
				return s;
			}
			else
			{
				return val.ToString();
			}
		}

		/// <summary>
		/// 数量を表示用文字列にフォーマットして返す
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <returns></returns>
		public static string FormatAmount(double amount)
		{
			return amount.ToString();
		}

		#endregion

		#region 長計処理用

		/// <summary>
		/// 年次カラムを作成する
		/// </summary>
		/// <param name="gridvLongtermRepairPlan">The gridv longterm repair plan.</param>
		/// <param name="startPeriod">The start period.</param>
		/// <param name="count">The count.</param>
		/// <param name="termInfo">The term info.</param>
		public static void CreateYearColumns(
			DataGridView grid, 
			int startPeriod, 
			int displayStartPeriod, 
			int count, coms.COMSKService.KumiaiTermInfo[] termInfo, 
			bool createHeader, 
			ref HeaderBandLayout headerLayout
		)
		{
			try
			{
				int nWidth = 40;
				string SUBTOTAL_COL = "bgcolSubTotal";  // last col
				//  カラムを作る
				var subTotalDisplayIndex = grid.Columns.Contains(SUBTOTAL_COL)
					? grid.Columns[SUBTOTAL_COL].DisplayIndex
					: grid.Columns.Count;
				for (int i = 0; i < count; i++)
				{
					int period = startPeriod + i;
					COMSKService.KumiaiTermInfo info = null;
					if (period < termInfo.Length)
					{
						info = termInfo[period];
					}
					string colName = "bgcolValue" + period.ToString();

					// create header layout
					if (createHeader)
                    {
						string yearIdx = info.TikuYear + "年目";
						string termIdx = info.Term + "期";
						string year = info.FiscalYear + "年";
						CreateBandedYear(ref headerLayout, colName, yearIdx, termIdx, year);
					}
					//  カラム
					else
                    {
						string fieldName = "Value" + (displayStartPeriod + i + 1).ToString();
						var yearCol = CreateCol(colName, fieldName, string.Empty, nWidth);
						yearCol.DisplayIndex = subTotalDisplayIndex++;
						grid.Columns.Add(yearCol);
					}
				}
				// total col's index reset if re-create column
				if (createHeader == false && grid.Columns.Contains(SUBTOTAL_COL))
					grid.Columns[SUBTOTAL_COL].DisplayIndex = subTotalDisplayIndex;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static DataGridViewColumn CreateCol(string name, string field, string caption, int width)
		{
			var col = new DataGridViewTextBoxColumn()
			{
				Name = name,
				DataPropertyName = field,
				HeaderText = caption,
				Width = width,
				Tag = COMSKCommon.TAG_DRAGGABLE_CELL
			};
			col.DefaultCellStyle.Format = "N0";
			col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			return col;
		}

		public static HeaderBandCellByName MakeHeaderCell(int bandRow, int bandRowSpan, string[] colNames, string text, bool isTopBand)
		{
			var c = new HeaderBandCellByName();
			c.BandRow = bandRow;
			c.BandRowSpan = bandRowSpan;
			c.Text = text;

			c.BackColor = Color.FromArgb(236, 231, 217);
			c.ForeColor = Color.Black;
			c.BorderColor = Color.Gray;
			c.BorderThickness = 1;

			for (int i = 0; i < colNames.Length; i++)
				c.ColumnNames.Add(colNames[i]);

			return c;
		}

		public static HeaderBandCellByName MakeHeaderCell(int bandRow, int bandRowSpan, string colName, string text, bool isTopBand)
		{
			return MakeHeaderCell(bandRow, bandRowSpan, new[] { colName }, text, isTopBand);
		}

		public static void CreateBandedYear(ref HeaderBandLayout layout, string colName, string yearIdx, string termIdx, string year)
		{
			// Row 0: "築年" spans all year columns
			layout.Cells.Add(MakeHeaderCell(0, 1, new[] { colName }, yearIdx, true));
			// Row 1: "42年目", "43年目", "44年目"
			layout.Cells.Add(MakeHeaderCell(1, 1, colName, termIdx, false));
			// Row 2: two-line text per year column: "42期" + "2009年"
			layout.Cells.Add(MakeHeaderCell(2, 1, colName, year, false));
		}

		public static void CustomDrawBandHeader(DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e, int accountStartPeriodIdx)
		{
			if (e.Band == null) return;
			var needPro = IsNeedChangeBGBand(e.Band, accountStartPeriodIdx);
			if (!needPro) return;
			// TODO
			/*using (GraphicsCache cache = new GraphicsCache(e.Graphics))
			{
				e.Info.Appearance.FillRectangle(cache, e.Bounds);
				e.Painter.DrawObject(e.Info);
				using (Pen pen = new Pen(Color.Black, 4))
				{
					e.Graphics.DrawLine(pen,
						e.Bounds.Right - 1, e.Bounds.Top,
						e.Bounds.Right - 1, e.Bounds.Bottom);
				}
				e.Handled = true;
			}*/
		}

		public static void CustomDrawCostCell(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e, int accountStartPeriodIdx, LongRepairPlanData.RowType rType)
		{
			string columnName = e.Column.Name;
			if (!columnName.StartsWith("bgcolValue")) return;

			var colIdx = int.Parse(columnName.Replace("bgcolValue", ""));
			if (colIdx == accountStartPeriodIdx + 29)
            {
				//if (rType == LongRepairPlanData.RowType.RepairPlan || rType == LongRepairPlanData.RowType.GroupCategory || rType == LongRepairPlanData.RowType.GroupItem)
				if (rType == LongRepairPlanData.RowType.RepairPlan || rType == LongRepairPlanData.RowType.GroupCategory)
					{
					using (Pen pen = new Pen(Color.Black, 2))
					{
						e.Graphics.DrawLine(pen,
							e.Bounds.Right - 1, e.Bounds.Top,
							e.Bounds.Right - 1, e.Bounds.Bottom);
					}
				}
				else
                {
					// TODO
					/*using (GraphicsCache cache = new GraphicsCache(e.Graphics))
					{
						Rectangle fillRect = e.Bounds;
						fillRect.Width += 1;
						fillRect.Height += 1;
						e.Graphics.FillRectangle(new SolidBrush(e.Appearance.BackColor), fillRect);

						TextRenderer.DrawText(
							e.Graphics,
							e.DisplayText,
							e.Appearance.Font,
							e.Bounds,
							e.Appearance.ForeColor,
							TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

						using (Pen pen = new Pen(Color.Black, 2))
						{
							e.Graphics.DrawLine(pen,
								e.Bounds.Right - 1, e.Bounds.Top,
								e.Bounds.Right - 1, e.Bounds.Bottom);
						}

						e.Handled = true;
					}*/
				}
			}
		}

		public static bool IsNeedChangeBGBand(DevExpress.XtraGrid.Views.BandedGrid.GridBand band, int accountStartPeriodIdx)
		{
			var bandName = band.Name;
			string bandNo = "0";
			if (bandName.StartsWith("gbCalendarYear"))
			{
				bandNo = bandName.Replace("gbCalendarYear", "");
			}
			else if (bandName.StartsWith("gbYear"))
			{
				bandNo = bandName.Replace("gbYear", "");
			}
			else if (bandName.StartsWith("gbPlanYear"))
			{
				bandNo = bandName.Replace("gbPlanYear", "");
			}
			var bno = int.Parse(bandNo);
			var compareIdx = accountStartPeriodIdx + 29;  // 固定60年間ので半分がaccountStartPeriodIdx + 29
			if (bno == compareIdx) return true;

			return false;
		}

		/// <summary>
		/// 変更理由が記入済みかどうか調べる
		/// </summary>
		/// <param name="kumiaiLongRepairPlanDetailHistory">The kumiai long repair plan detail history.</param>
		/// <returns></returns>
		//public static bool UpdateReasonIsWritten(KumiaiLongRepairPlanDetailHistory kumiaiLongRepairPlanDetailHistory)
		//{
		//    if (kumiaiLongRepairPlanDetailHistory.UpdateReasonCode == null)
		//    {
		//        return false;
		//    }
		//    else if (kumiaiLongRepairPlanDetailHistory.UpdateReasonCode == string.Empty)
		//    {
		//        return false;
		//    }
		//    else if (kumiaiLongRepairPlanDetailHistory.UpdateReasonCode != COMSKCommon.CODEMST_INVALID_CODE)
		//    {
		//        return true;
		//    }
		//    else
		//    {
		//        if (kumiaiLongRepairPlanDetailHistory.UpdateReasonText.Trim() == string.Empty)
		//        {
		//            return false;
		//        }
		//        else
		//        {
		//            return true;
		//        }
		//    }
		//}

		/// <summary>
		/// 変更内容取得 (ドラッグ)
		/// </summary>
		/// <param name="periodFrom">開始会計期.</param>
		/// <param name="periodTo">終了会計期.</param>
		/// <param name="updateLator">true なら以後反映.</param>
		/// <returns></returns>
		public static string GetUpdateContent_Drag(int periodFrom, int periodTo, bool updateLator)
		{
			string s = updateLator ? "以後反映" : "以後非反映";
			return string.Format("会計期 {0} → {1}、{2}", periodFrom, periodTo, s);
		}

		/// <summary>
		/// 変更内容取得 (数字変更)
		/// </summary>
		/// <param name="accountYear">The account year.</param>
		/// <param name="prevValue">The prev value.</param>
		/// <param name="newValue">The new value.</param>
		/// <returns></returns>
		public static string GetUpdateContent_Change(int accountYear, long prevValue, long newValue)
		{
			return string.Format("会計期 {0} 金額 {1:N0} → {2:N0}", accountYear, prevValue, newValue);
		}

		/// <summary>
		/// 実績年度書き換え
		/// </summary>
		/// <param name="resultYear">The result year.</param>
		/// <returns></returns>
		public static string GetUpdateContent_WriteEffectedYear(string resultYear)
		{
            return string.Format("工事実施年度記入 「{0}」", resultYear);
		}

        /// <summary>
        /// 周期起算年書き換え
        /// </summary>
        /// <param name="resultYear">The datum year.</param>
        /// <returns></returns>
        public static string GetUpdateContent_WriteDatumYear(string datumYear)
        {
            return string.Format("周期起算年修正 「{0}」", datumYear);
        }

		/// <summary>
		/// 会計期データ、消費税データ、年から対象の消費税を取得する
		/// </summary>
		/// <param name="kumiaiTermInfo">組合会計期データ配列.</param>
		/// <param name="consumptionTaxList">消費税データ配列.</param>
		/// <param name="periodIndex">会計期インデックス.</param>
		/// <returns></returns>
		public static double GetConsumptionTaxRateFromList(coms.COMSKService.KumiaiTermInfo[] kumiaiTermInfo, List<KumiaiLongRepairPlanTaxMst> consumptionTaxList, int periodIndex)
		{
			double ret = 0;

			if (periodIndex < kumiaiTermInfo.Length)
			{
				COMSKService.KumiaiTermInfo info = kumiaiTermInfo[periodIndex];

				int year = 0;
				if (int.TryParse(info.FiscalYear, out year))
				{
					//  消費税データ配列から目的の年を取得
					KumiaiLongRepairPlanTaxMst taxInfo = (from item in consumptionTaxList
														  where item.StartYear <= year && year <= item.EndYear
														  orderby item.StartYear ascending
														  select item).FirstOrDefault();

					if (taxInfo != null)
					{
						//  最終的な消費税率
						ret = taxInfo.Tax;
					}
				}
			}

			return ret;
		}

		/// <summary>
		/// 会計期データ、消費税データ、年から対象の物価上昇率を取得する
		/// </summary>
		/// <param name="kumiaiTermInfo">組合会計期データ配列.</param>
		/// <param name="costIncrList">The cost incr list.</param>
		/// <param name="periodIndex">会計期インデックス.</param>
		/// <returns></returns>
		public static double GetCostIncreaseRateFromList(coms.COMSKService.KumiaiTermInfo[] kumiaiTermInfo, List<KumiaiLongRepairPlanTaxMst> costIncrList, int periodIndex)
		{
			double ret = 0;

			if (periodIndex < kumiaiTermInfo.Length)
			{
				COMSKService.KumiaiTermInfo info = kumiaiTermInfo[periodIndex];

				int year = 0;
				if (int.TryParse(info.FiscalYear, out year))
				{
					//  物価上昇率データ配列から目的の年を取得
					KumiaiLongRepairPlanTaxMst taxInfo = (from item in costIncrList
														  where item.StartYear <= year && year <= item.EndYear
														  orderby item.StartYear ascending
														  select item).FirstOrDefault();

					if (taxInfo != null)
					{
						//  最終的な消費税率
						ret = taxInfo.Tax;
					}
				}
			}

			return ret;
		}

		/// <summary>
		/// 組合毎の表示単位 (千円 or 万円) を考慮して数字の表示文字列を決定する
		/// </summary>
		/// <param name="viewUnitCode">The view unit code.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static long ConvertToLongRepairPlanCost(string viewUnitCode, long value)
		{
			int divValue = 1000;
			if (viewUnitCode == COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_THOUSAND)
			{
				//  千円単位
				divValue = 1000;
			}
			else if (viewUnitCode == COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_10_THOUSAND)
			{
				//  万円単位
				divValue = 10000;
			}
			else
			{
				//  不明
				divValue = 1000;
			}

			return (long)Math.Round(((double)value) / divValue, MidpointRounding.AwayFromZero);
		}

		/// <summary>
		/// ConvertToLongRepairPlanCost の結果を表示文字列に変換する
		/// </summary>
		/// <param name="viewUnitCode">The view unit code.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string ConvertToLongRepairPlanText(string viewUnitCode, long value)
		{
			if (value == 0)
			{
				return string.Empty;
			}
			else
			{
				value = ConvertToLongRepairPlanCost(viewUnitCode, value);
				return string.Format("{0:N0}", value);
			}
		}

		/// <summary>
		/// 単価と数量から長計集計用値を算出する
		/// </summary>
		/// <param name="cost">The cost.</param>
		/// <param name="amount">The amount.</param>
		/// <returns></returns>
		public static long RoundUpToLongRepairPlanImportCost(long cost, double amount)
		{
			double val = amount * cost;

			return (long)Math.Ceiling((val / 100000)) * 100000;
		}

		#endregion

		#region 会計期ユーティリティ

		/// <summary>
		/// 会計期から会計年度を取得する
		/// </summary>
		/// <param name="accountPeriod">The account period.</param>
		/// <returns></returns>
		public static int GetAccountYearFromAccountPeriod(COMSKService.KumiaiTermInfo[] termInfo, int accountPeriod)
		{
			COMSKService.KumiaiTermInfo info = (from item in termInfo
												where item.Term == accountPeriod
												select item).FirstOrDefault();

			if (info == null)
			{
				return 0;
			}
			else
			{
				return int.Parse(info.FiscalYear);
			}
		}

		/// <summary>
		/// 会計年度から決算期の配列を検索する。
		/// </summary>
		/// <param name="accountYear">The account year.</param>
		/// <returns></returns>
		public static int[] GetAccountPeriodFromAccountYear(COMSKService.KumiaiTermInfo[] termInfo, int accountYear)
		{
			if (termInfo == null) return new int[0];
			return (from item in termInfo
					where item.FiscalYear == accountYear.ToString()
					select item.Term).ToArray();
		}

		/// <summary>
		/// 会計期から会計期インデックスを取得する
		/// </summary>
		/// <param name="termInfo">The term info.</param>
		/// <param name="accountPeriod">The account period.</param>
		/// <returns></returns>
		public static int GetAccountPeriodIndexFromAccountPeriod(COMSKService.KumiaiTermInfo[] termInfo, int accountPeriod)
		{
			for (int i = 0; i < termInfo.Length; i++)
			{
				if (termInfo[i].Term == accountPeriod)
				{
					return i;
				}
			}

			return -1;
		}

		#endregion

		#region 修繕積立金 各案の色

		/// <summary>
		/// 各案の色配列
		/// </summary>
		public static readonly Color[] REPAIR_RESERVE_PLAN_COLORS = new Color[]
		{
			Color.Orange,
			Color.LightBlue,
			Color.Plum,
			Color.LightGray,
			Color.Pink,
		};

		#endregion

		#region 必要なデータが揃っているか確認

		/// <summary>
		/// 長計表示時
		/// </summary>
		/// <returns></returns>
		public static bool CheckReadyLongRepairPlanData(long kumiaiInfoPid, int needMaxTerm)
		{
			business.K300000BL bl = new coms.COMSK.business.K300000BL();
			string msg = bl.CheckReadyLongRepairPlanData(kumiaiInfoPid, needMaxTerm);
			if (msg == string.Empty)
			{
				return true;
			}
			else
			{
				MessageBox.Show(msg, coms.COMMON.Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
		}

		/// <summary>
		/// 積立金表示時
		/// </summary>
		/// <returns></returns>
		public static bool CheckReadyRepairReservePlanData(long kumiaiInfoPid, int needMaxTerm)
		{
			business.K300000BL bl = new coms.COMSK.business.K300000BL();
			bool noTypes = false;
			string msg = bl.CheckReadyRepairReservePlanData(kumiaiInfoPid, needMaxTerm, out noTypes);
			if (msg == string.Empty)
			{
				return true;
			}
			else
			{
				if (noTypes == true)
				{
					MessageBox.Show(msg, coms.COMMON.Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
					//if (MessageBox.Show(msg, coms.COMMON.Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					//{
					//    return true;
					//}
					//else
					//{
					//    return false;
					//}
				}
				else
				{
					MessageBox.Show(msg, coms.COMMON.Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
			}
		}

		#endregion

		#region その他メソッド

		/// <summary>
		/// plusビットマップの取得（サイズ調整有）
		/// </summary>
		/// <returns></returns>
		public static Bitmap GetPlusBitmap()
		{
			using (Bitmap bmp = new Bitmap(coms.Properties.Resources.plus))
			{
				return bmp.Clone(new Rectangle(5, 5, 9, 9), System.Drawing.Imaging.PixelFormat.Format64bppArgb);
			}
		}

		public static List<LongRepairPlanData.RowType> calcTypes()
        {
			return new List<LongRepairPlanData.RowType>() {
				LongRepairPlanData.RowType.CalcA,
				LongRepairPlanData.RowType.CalcB,
				LongRepairPlanData.RowType.CalcC,
				LongRepairPlanData.RowType.CalcD,
				LongRepairPlanData.RowType.CalcE
			};
		}

		public static DataGridViewColumn CreateColTextNormal(string name, string field, string caption, int width)
		{
			return new DataGridViewTextBoxColumn()
			{
				Name = name,
				DataPropertyName = field,
				HeaderText = caption,
				Width = width
			};
		}

		public static DataGridViewColumn CreateColButtonNormal(string name, string field, string caption, string btnText, int width)
		{
			return new DataGridViewButtonColumn()
			{
				Name = name,
				HeaderText = caption,
				Width = width,
				Text = btnText,
				UseColumnTextForButtonValue = true
			};
		}
		#endregion

	}
}
