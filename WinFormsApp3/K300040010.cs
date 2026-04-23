using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using coms.COMMON;
using coms.COMSKService;
using coms.COMSK.common;
using coms.COMSK.business;


/// <history>
/// #001 ducthang 20121005
///     修繕基準数量明細「参考基準」反映する時に登録します
/// </history>
namespace coms.COMSK.ui
{
	/// <summary>
	/// 修繕計画
	/// </summary>
	public partial class K300040010 : MyForm
	{

		#region プロパティ

		public KumiaiLongRepairPlan LongRepairPlan { get; set; }
		#endregion

		#region メンバ

		/// <summary>
		/// 対象の長計の PID
		/// </summary>
		private long pid = 0;

		public K300020010 childFrom ;

		#endregion

		#region コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public K300040010(long pid)
		{
			this.pid = pid;
			InitializeComponent();
		}

		#endregion

		#region イベント

		/// <summary>
		/// フォームロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void K200010040_Load(object sender, EventArgs e)
		{
			
			try
			{
				//  権限設定
				SetAuthorities();

				//  長計サマリーデータ取得
				K300030BL bl300030 = new K300030BL();
				LongRepairPlan = bl300030.GetKumiaiLongRepairPlan(pid);
				if (this.LongRepairPlan == null)
				{
					throw new Exception("長計データを正しく取得できませんでした。");
				}

				//  明細一覧を取得
				K300040BL bl300040 = new K300040BL();
				List<KumiaiRepairPlanDetail> list = bl300040.GetAllKumiaiRepairPlanDetails(pid).ToList();

				//  データ流し込み
				LoadDatas(list);

				//  画面にプロパティをセット
				txtKumiaiName.Text = LongRepairPlan.KumiaiName;
				txtStandardRepairPlanName.Text = LongRepairPlan.StandardRepairPlanName;
				txtKumiaiRepairPlanName.Text = LongRepairPlan.Name;

				//  作成年度
				int createYear = COMSKCommon.GetAccountYearFromAccountPeriod(LongRepairPlan.TermInfo, LongRepairPlan.AccountPeriod);
				txtDatumYear.Text = string.Format("{0}年", createYear);

				//  フィルタをセット
				bool[] filter = new bool[] { true, false, };
				ctrRepairPlan_T.Filter = filter;
				ctrRepairPlan_B.Filter = filter;
				ctrRepairPlan_E.Filter = filter;
				ctrRepairPlan_Out.Filter = filter;
				ctrRepairPlan_Other.Filter = filter;

				//  フィルタコンボボックスを初期化
                //COMSKCommon.BindSelectionToCheckboxList(cmbFilter);

                ////  デフォルトで両方にチェック
                //for (int i = 0; i < cmbFilter.Items.Count; i++)
                //{
                //    cmbFilter.SetItemChecked(i, true);
                //}

				//  登録
				FormMngr.Instance.RegisterKumiaiRepairPlan(this.pid, this);
                this.Text += "_" + LongRepairPlan.Name;
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// フォームクロージング
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		private void K300040010_FormClosing(object sender, FormClosingEventArgs e)
		{
			//  登録解除
			FormMngr.Instance.UnregisterKumiaiRepairPlan(this.pid, this);
			if (childFrom != null)
			{
				childFrom.Close();
			}
			
		}

		private void xtraTabControl_RepairList_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
		{
			if (childFrom != null)
			{
				childFrom.Close();
			}
		}

		/// <summary>
		/// メニューへ戻るボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSave_Click(object sender, EventArgs e)
		{
            //20140818 y-hoshino start
            if ((LongRepairPlan.StatusCode == "0002") || (LongRepairPlan.StatusCode == "0003") || (LongRepairPlan.StatusCode == "0005"))
            {
                if (MessageBox.Show(Constant.CONFIRM_KAKUTEIHOZON_TITLE, Constant.CONFIRM_KAKUTEI_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                //  確認
                if (MessageBox.Show("登録しますか？", Constant.CONFIRM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }
            //20140818 y-hoshino end
            Save();
           
		}

		/// <summary>
		/// 帳票出力ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOutputRepairPlan_Click(object sender, EventArgs e)
		{
			try
			{
				// pidではなくリストを渡す必要がある。
				//  送信用一時データ
				List<KumiaiRepairPlanDetail> list = new List<KumiaiRepairPlanDetail>();

				//  データを回収する
				list = list.Concat(ctrRepairPlan_T.SortedDataSource)
					.Concat(ctrRepairPlan_B.SortedDataSource)
					.Concat(ctrRepairPlan_E.SortedDataSource)
					.Concat(ctrRepairPlan_Out.SortedDataSource)
					.Concat(ctrRepairPlan_Other.SortedDataSource)
					.ToList();

				K300040BL business = new K300040BL();
				FileEntry ret = business.ExportScheduleRepairPlan(list);
				Helper.SaveFile(ret, this);
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// 修繕計画登録ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnUpdate_Click(object sender, EventArgs e)
		{
            //20140818 y-hoshino start
            //if (MessageBox.Show("登録しますか？", Constant.CONFIRM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            if ((LongRepairPlan.StatusCode == "0002") || (LongRepairPlan.StatusCode == "0003") || (LongRepairPlan.StatusCode == "0005"))
            {
                if (MessageBox.Show(Constant.CONFIRM_KAKUTEIHOZON_TITLE, Constant.CONFIRM_KAKUTEI_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                //  確認
                if (MessageBox.Show("登録しますか？", Constant.CONFIRM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            //20140818 y-hoshino end
			}
            if (Save())
            {
                this.Close();
            }
			
		}

		/// <summary>
		/// 仮箱表示ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnTempBox_Click(object sender, EventArgs e)
		{
			#region 現在のタブから工事分類を確定
			COMSK.common.COMSKCommon.ConstructionType constrType;
			if (xtraTabControl_RepairList2.SelectedIndex == 0)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Temp;
			}
			else if (xtraTabControl_RepairList2.SelectedIndex == 1)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Building;
			}
			else if (xtraTabControl_RepairList2.SelectedIndex == 2)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Equipment;
			}
			else if (xtraTabControl_RepairList2.SelectedIndex == 3)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Outer;
			}
			else
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Other;
			}
			#endregion

			
			
            childFrom = new K300020010(constrType, pid)
			{
				KumiaiInfoPid = LongRepairPlan.KumiaiInfoPid,
				KumiaiLongRepairPlanPid = LongRepairPlan.Pid,
				AdminMode = false,
				Approved = false,
			};
			childFrom.parentForm = this;
			childFrom.Show();
			
		}

		public void LoadDataKumiaiRepairPlanDetail(K300020010 frm, COMSK.common.COMSKCommon.ConstructionType constrType)
		{
			try
			{
				//  推奨されるアイテムを持ってくる
				K300020BL bl20 = new K300020BL();
				KumiaiRepairPlanDetail sourceDetail = bl20.GetRecommendedReserveBoxItem(frm.ReserveBoxItem.Pid);

				#region 新規インスタンスを作成
				KumiaiRepairPlanDetail detail = new KumiaiRepairPlanDetail()
				{
					Pid = long.MinValue,
					KumiaiLongRepairPlanPid = LongRepairPlan.Pid,
					StandardRepairPlanDetailPid = long.MinValue,

					ConstructionTypePid = frm.ReserveBoxItem.ConstructionTypePid,
					ConstructionItemPid = frm.ReserveBoxItem.ConstructionItemPid,
					ConstructionCategoryPid = frm.ReserveBoxItem.ConstructionCategoryPid,
					ConstructionPositionPid = frm.ReserveBoxItem.ConstructionPositionPid,
					ConstructionRegionPid = frm.ReserveBoxItem.ConstructionRegionPid,
					ConstructionSpecificationPid = frm.ReserveBoxItem.ConstructionSpecificationPid,
					ConstructionDivisionPid = frm.ReserveBoxItem.ConstructionDivisionPid,
					RepairConstructionContentPid = long.MinValue,
					ConstructionPositionUnit = string.Empty,
					ConstructionPartsNo = string.Empty,

					CurrentSpecification = sourceDetail.CurrentSpecification,
					Cost = sourceDetail.Cost,
					Cycle = sourceDetail.Cycle,
					Amount = 0,
					UnitCode = sourceDetail.UnitCode,
					CostUnitCode = sourceDetail.CostUnitCode,
					ResultCostA = sourceDetail.ResultCostA,
					ResultCostB = sourceDetail.ResultCostB,
					ReliableApplication = sourceDetail.ReliableApplication,
					Specification = sourceDetail.Specification,
					Attention = sourceDetail.Attention,
					ReferenceData = sourceDetail.ReferenceData,
					Memo = sourceDetail.Memo,

					ParentPid = long.MinValue,
					ParentSpecificationPid = long.MinValue,
					ParentDivisionPid = long.MinValue,
					ParentRepairConstructionContentPid = long.MinValue,

					Select = true,
					SelectFlg = COMSKCommon.SELECT_FLG_ON,
					ConvertFlg = "00",
					UpdateUserMstPid = Helper.loginUserInfo.Pid,
					InsertUserMstPid = Helper.loginUserInfo.Pid,
					UpdateFlg = COMSKCommon.HAS_HISTORY_FLG_ON,

				};
				#endregion

				#region 詳細画面を開く

				bool ok = false;
				List<KumiaiRepairPlanDetailAmount> amountList;
				if (constrType == coms.COMSK.common.COMSKCommon.ConstructionType.Equipment)
				{
					//  設備
                    K300040012 frmDetail = new K300040012(detail)
					{
						DontRegister = true,
					};
					if (frmDetail.ShowDialog() == DialogResult.OK)
					{
						ok = true;
					}
					amountList = frmDetail.GetAmountList();
				}
				else
				{
					//  設備以外
					K300040011 frmDetail = new K300040011(detail)
					{
						DontRegister = true,
					};
					if (frmDetail.ShowDialog() == DialogResult.OK)
					{
						ok = true;
					}
					amountList = frmDetail.GetAmountList();
				}
				#endregion

				//  OK なら
				if (ok)
				{
					//  昇格
					string errMsg;
					detail.Pid = bl20.ApplyRepairPlanReserveBoxToKumiai(frm.ReserveBoxItem.Pid, detail, out errMsg);
					if (detail.Pid == long.MinValue)
					{
						throw new Exception(errMsg);
					}

					//#001
					if (amountList != null)
					{
						K300040BL business = new K300040BL();
						business.ResetKumiaiRepairPlanDetailAmount(detail, amountList);
						if (COMMON.BusinessExceptionHandler.businessHasError)
						{
							COMMON.BusinessExceptionHandler.businessHasError = false;
							return;
						}
					}

					//  変更履歴をクリア
					detail.UpdateReason = string.Empty;

					//  追加
					if (constrType == COMSKCommon.ConstructionType.Equipment)
					{
						ctrRepairPlan_E.Add(detail);
					}
					else
					{
						common.CtrRepairPlan_B ctr = xtraTabControl_RepairList2.SelectedTab.Controls[0] as common.CtrRepairPlan_B;
						ctr.Add(detail);
					}
				}

			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// 再表示ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnRefresh_Click(object sender, EventArgs e)
		{
            //チェックコンボボックス部品差し替え
            string[] selectedValues = COMSKCommon.GetSelectedItemsAsArrayOfCheckedCombobox(chkFilterDEV);

			List<bool> list = new List<bool>();

			foreach (string s in selectedValues)
			{
				if (s == "1")
				{
					list.Add(true);
				}
				else if (s == "2")
				{
					list.Add(false);
				}
			}

			bool[] filters = list.ToArray();
			ctrRepairPlan_B.Filter = filters;
			ctrRepairPlan_E.Filter = filters;
			ctrRepairPlan_T.Filter = filters;
			ctrRepairPlan_Out.Filter = filters;
			ctrRepairPlan_Other.Filter = filters;

			ctrRepairPlan_B.ApplyViewSequence();
			ctrRepairPlan_E.ApplyViewSequence();
			ctrRepairPlan_T.ApplyViewSequence();
			ctrRepairPlan_Out.ApplyViewSequence();
			ctrRepairPlan_Other.ApplyViewSequence();

			ctrRepairPlan_B.RefreshData();
			ctrRepairPlan_E.RefreshData();
			ctrRepairPlan_T.RefreshData();
			ctrRepairPlan_Out.RefreshData();
			ctrRepairPlan_Other.RefreshData();
		}

		/// <summary>
		/// 差異表示
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnDiff_Click(object sender, EventArgs e)
		{

			//  確認メッセージ
			if (MessageBox.Show("作成基準を取り込むには、先に保存する必要があります。保存してもよろしいですか？", Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}

			//  保存
			if (!Save())
			{
				return;
			}

			//  比較を開く
			K300040030 frm = new K300040030(pid, xtraTabControl_RepairList2.SelectedIndex);
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//  明細一覧を取得
				K300040BL bl300040 = new K300040BL();
				List<KumiaiRepairPlanDetail> list = bl300040.GetAllKumiaiRepairPlanDetails(pid).ToList();

				//  データ流し込み
				LoadDatas(list);

			}
		}


		/// <summary>
		/// 新規登録
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairPlan_Adding(object sender, EventArgs e)
		{
			//  先に工事ツリーを初期化
			coms.COMSK.common.ConstructionTreeMngr.Instance.UpdateTree();

			common.CtrRepairPlan_B ctr = sender as common.CtrRepairPlan_B;

			// 詳細画面表示
			KumiaiRepairPlanDetail detail = new KumiaiRepairPlanDetail()
			{
				Pid = long.MinValue,
				KumiaiLongRepairPlanPid = this.LongRepairPlan.Pid,
				StandardRepairPlanDetailPid = this.LongRepairPlan.StandardRepairPlanPid,
				ConstructionTypePid = (long)ctr.ConstrType,
				ConstructionItemPid = long.MinValue,
				ConstructionCategoryPid = long.MinValue,
				ConstructionPositionPid = long.MinValue,
				ConstructionRegionPid = long.MinValue,
				ConstructionSpecificationPid = long.MinValue,
				ConstructionDivisionPid = long.MinValue,
				ConstructionTypeName = coms.COMSK.common.ConstructionTreeMngr.Instance.GetConstructionTypeName(ctr.ConstrType),
				InsertUserMstPid = Helper.loginUserInfo.Pid,
				SelectFlg = coms.COMSK.common.COMSKCommon.SELECT_FLG_ON,
				ParentPid = long.MinValue,
			};

			K300040011 frm = new K300040011(detail)
			{
				AddNew = true,
			};
			if (frm.ShowDialog() == DialogResult.OK)
			{
				ctr.Add(frm.WorkRepairPlanDetail);
			}
		}

		/// <summary>
		/// 新規登録 (設備)
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairPlan_E_Adding(object sender, EventArgs e)
		{
			//  先に工事ツリーを初期化
			coms.COMSK.common.ConstructionTreeMngr.Instance.UpdateTree();

			// 詳細画面表示
			KumiaiRepairPlanDetail detail = new KumiaiRepairPlanDetail()
			{
				Pid = long.MinValue,
				KumiaiLongRepairPlanPid = this.LongRepairPlan.Pid,
				StandardRepairPlanDetailPid = this.LongRepairPlan.StandardRepairPlanPid,
				ConstructionTypePid = (long)COMSKCommon.ConstructionType.Equipment,
				ConstructionItemPid = long.MinValue,
				ConstructionCategoryPid = long.MinValue,
				ConstructionPositionPid = long.MinValue,
				ConstructionRegionPid = long.MinValue,
				ConstructionSpecificationPid = long.MinValue,
				ConstructionDivisionPid = long.MinValue,
				ConstructionTypeName = coms.COMSK.common.ConstructionTreeMngr.Instance.GetConstructionTypeName(COMSKCommon.ConstructionType.Equipment),
				InsertUserMstPid = Helper.loginUserInfo.Pid,
				SelectFlg = coms.COMSK.common.COMSKCommon.SELECT_FLG_ON,
				ParentPid = long.MinValue,
			};

			K300040012 frm = new K300040012(detail)
			{
				AddNew = true,
			};
			if (frm.ShowDialog() == DialogResult.OK)
			{
				ctrRepairPlan_E.Add(frm.WorkRepairPlanDetail);
			}
		}

		#endregion

		#region Privates

		/// <summary>
		/// 各タブにデータをロードさせる
		/// </summary>
		/// <param name="list">The list.</param>
		private void LoadDatas(List<KumiaiRepairPlanDetail> list)
		{
			// 仮設
			long pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Temp;
			List<KumiaiRepairPlanDetail> subList = (from item in list
													where item.ConstructionTypePid == pidType
													select item).ToList<KumiaiRepairPlanDetail>();
			ctrRepairPlan_T.DataSource = subList;
			ctrRepairPlan_T.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Temp;

			// 建築
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Building;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<KumiaiRepairPlanDetail>();
			ctrRepairPlan_B.DataSource = subList;
			ctrRepairPlan_B.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Building;

			// 設備
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Equipment;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<KumiaiRepairPlanDetail>();
			ctrRepairPlan_E.DataSource = subList;
			//ctrRepairPlan_E.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Equipment;

			// 外構
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Outer;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<KumiaiRepairPlanDetail>();
			ctrRepairPlan_Out.DataSource = subList;
			ctrRepairPlan_Out.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Outer;

			// その他
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Other;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<KumiaiRepairPlanDetail>();
			ctrRepairPlan_Other.DataSource = subList;
			ctrRepairPlan_Other.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Other;

		}

		/// <summary>
		/// 表示順序と選択状況を保存する
		/// </summary>
		/// <returns></returns>
		private bool Save()
		{
			//  送信用一時データ
			List<KumiaiRepairPlanDetail> list = new List<KumiaiRepairPlanDetail>();

		
			//  データを回収する
			list = list.Concat(ctrRepairPlan_T.SortedDataSource)
				.Concat(ctrRepairPlan_B.SortedDataSource)
				.Concat(ctrRepairPlan_E.SortedDataSource)
				.Concat(ctrRepairPlan_Out.SortedDataSource)
				.Concat(ctrRepairPlan_Other.SortedDataSource)
				.ToList();

			//  送信
			try
			{
				K300040BL business = new K300040BL();
				if (!business.UpdateKumiaiRepairPlanDetailViewSequence(list))
				{
					throw new Exception("データの登録に失敗しました。");
				}

                //  明細一覧を取得
                K300040BL bl300040 = new K300040BL();
                List<KumiaiRepairPlanDetail> newlist = bl300040.GetAllKumiaiRepairPlanDetails(pid).ToList();

                //  データ流し込み
                LoadDatas(newlist);
                
				//  OK
				return true;
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				return false;
			}
		}

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
			"    AI.partsCode LIKE '%btnK300040011%'" +
			" OR AI.partsCode LIKE '%btnK300040010%'";

			COMSD.business.D100109BL bl109 = new coms.COMSD.business.D100109BL();
			COMSDService.AuthorityInfo[] authorities = bl109.SearchAuthorityInfo(sql, "", "");

			btnDiff.Enabled = Helper.GetAuthority(authorities, "btnK300040010001", userPostCode, userPositionCode);
			btnTempBox.Enabled = Helper.GetAuthority(authorities, "btnK300040010002", userPostCode, userPositionCode);
			btnOutputRepairPlan.Enabled = Helper.GetAuthority(authorities, "btnK300040010003", userPostCode, userPositionCode);
			btnUpdate.Enabled = Helper.GetAuthority(authorities, "btnK300040010004", userPostCode, userPositionCode);

			bool allowAdd = Helper.GetAuthority(authorities, "btnK300040011001", userPostCode, userPositionCode);
			ctrRepairPlan_T.AllowAdd = allowAdd;
			ctrRepairPlan_B.AllowAdd = allowAdd;
			ctrRepairPlan_E.AllowAdd = allowAdd;
			ctrRepairPlan_Other.AllowAdd = allowAdd;
			ctrRepairPlan_Out.AllowAdd = allowAdd;
		}
		#endregion

		private void ctrRepairPlan_T_Load(object sender, EventArgs e)
		{

		}

        private void xtraTabControl_RepairList2_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (childFrom != null)
			{
				childFrom.Close();
			}
		}
    }
}
