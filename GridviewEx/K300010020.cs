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
using coms.COMSK.business;
using coms.COMSK.common;

namespace coms.COMSK.ui
{
	/// <summary>
	/// 標準修繕計画
	/// </summary>
	public partial class K300010020 : MyForm
	{

		#region イベント

		public event EventHandler UpdatedAndClosed;
		public K300020010 childFrom;

		#endregion

		#region メンバ

		/// <summary>
		/// 対象の標準修繕計画PID
		/// </summary>
		private long pid = 0;

		/// <summary>
		/// 標準修繕計画
		/// </summary>
		private StandardRepairPlan stdRepairPlan = null;

		#endregion


		#region Public

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public K300010020(long pid)
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
		private void K200010100_Load(object sender, EventArgs e)
		{
			try
			{
				//  権限設定
				SetAuthorities();

				K300010BL business = new K300010BL();

				//  標準修繕計画を取得する
				stdRepairPlan = business.GetStandardRepairPlan(this.pid);

				//  明細を取得する
				List<StandardRepairPlanDetail> list = business.GetAllStandardRepairPlanDetail(stdRepairPlan.Pid);

				//  名前等設定
				txtPlanName.Text = stdRepairPlan.StandardRepairPlanName;
				txtNote.Text = stdRepairPlan.Note;
				if (stdRepairPlan.HistoryFlg == coms.COMSK.common.COMSKCommon.HAS_HISTORY_FLG_ON)
				{
					chkTakeChangeHistory.Checked = true;
				}
				else
				{
					chkTakeChangeHistory.Checked = false;
				}
				

				//  データ読み込み
				LoadDatas(list);

				//  イベントを設定
				ctrRepairPlan_T.Deleting += new EventHandler<coms.COMSK.common.CustomEventArgs<StandardRepairPlanDetail>>(ctrRepairPlan_Deleting);
				ctrRepairPlan_B.Deleting += new EventHandler<coms.COMSK.common.CustomEventArgs<StandardRepairPlanDetail>>(ctrRepairPlan_Deleting);
				ctrRepairPlan_E.Deleting += new EventHandler<coms.COMSK.common.CustomEventArgs<StandardRepairPlanDetail>>(ctrRepairPlan_Deleting);
				ctrRepairPlan_Out.Deleting += new EventHandler<coms.COMSK.common.CustomEventArgs<StandardRepairPlanDetail>>(ctrRepairPlan_Deleting);
				ctrRepairPlan_Other.Deleting += new EventHandler<coms.COMSK.common.CustomEventArgs<StandardRepairPlanDetail>>(ctrRepairPlan_Deleting);

				//  レイアウト更新
				UpdateLayout();
			}
			catch (Exception ex)
			{
				//  エラー
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.Close();
			}
		}

		/// <summary>
		/// メニューへ戻るボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
			
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
				//  作成
				K300010BL business = new K300010BL();
				FileEntry ret = business.ExportStandardRepairPlan(pid);

				//  保存
				Helper.SaveFile(ret, this);
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// 登録ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			//  バリデーション
			if (!ValidateControls())
			{
				return;
			}

			//  保存
			if (MessageBox.Show(Constant.CONFIRM_REGISTER_TITLE, Constant.CONFIRM_TITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				UpdateData(false);

				//  イベント着火
				FireUpdatedAndClosed();

				//  閉じる
				this.Close();
			}
			
		}

		/// <summary>
		/// 仮箱ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnTempBox_Click(object sender, EventArgs e)
		{
			coms.COMSK.common.COMSKCommon.ConstructionType constrType;
			if (tabControl1.SelectedIndex == 0)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Temp;
			}
			else if (tabControl1.SelectedIndex == 1)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Building;
			}
			else if (tabControl1.SelectedIndex == 2)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Equipment;
			}
			else if (tabControl1.SelectedIndex == 3)
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Outer;
			}
			else
			{
				constrType = coms.COMSK.common.COMSKCommon.ConstructionType.Other;
			}

			//20140821 y-hoshino start
			//  現在のタブの工事分類を取得
			childFrom = new K300020010(constrType, pid)
            //20140821 y-hoshino end
			{
				StdRepairPlanPid = stdRepairPlan.Pid,
				AdminMode = true,
				Approved = (this.stdRepairPlan.ApprovalUserMstPid != long.MinValue),
			};

			childFrom.parentForm = this;
			childFrom.Show();
		}

		public void LoadStandardRepairPlanDetail(K300020010 frm, coms.COMSK.common.COMSKCommon.ConstructionType constrType)
		{
			
			try
			{
				//  推奨されるアイテムを持ってくる
				K300020BL bl20 = new K300020BL();
				KumiaiRepairPlanDetail sourceDetail = bl20.GetRecommendedReserveBoxItem(frm.ReserveBoxItem.Pid);

				//  新規インスタンスを作成
				StandardRepairPlanDetail detail = new StandardRepairPlanDetail()
				{
					Pid = long.MinValue,

					ConstructionTypePid = frm.ReserveBoxItem.ConstructionTypePid,
					ConstructionItemPid = frm.ReserveBoxItem.ConstructionItemPid,
					ConstructionCategoryPid = frm.ReserveBoxItem.ConstructionCategoryPid,
					ConstructionPositionPid = frm.ReserveBoxItem.ConstructionPositionPid,
					ConstructionRegionPid = frm.ReserveBoxItem.ConstructionRegionPid,
					ConstructionSpecificationPid = frm.ReserveBoxItem.ConstructionSpecificationPid,
					ConstructionDivisionPid = frm.ReserveBoxItem.ConstructionDivisionPid,
					RepairConstructionContentPid = long.MinValue,

					CurrentSpecification = sourceDetail.CurrentSpecification,
					Cost = sourceDetail.Cost,
					Cycle = sourceDetail.Cycle,
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

					DeleteFlg = Constant.DELETEFLG_OFF,
					ConvertFlg = "00",
					UpdateUserMstPid = Helper.loginUserInfo.Pid,
					InsertUserMstPid = Helper.loginUserInfo.Pid,
					StandardRepairPlanPid = stdRepairPlan.Pid,
					UpdateFlg = "01",

					ConstructionTypeName = coms.COMSK.common.ConstructionTreeMngr.Instance.GetConstructionTypeName(constrType),

				};

				//  詳細画面を開く
				bool ok = false;
				if (constrType == coms.COMSK.common.COMSKCommon.ConstructionType.Equipment)
				{
					//  設備
					K300010022 frmDetail = new K300010022(detail)
					{
						DontRegister = true,
					};
					if (frmDetail.ShowDialog() == DialogResult.OK)
					{
						ok = true;
					}
				}
				else
				{
					//  設備以外
					K300010021 frmDetail = new K300010021(detail)
					{
						DontRegister = true,
					};
					if (frmDetail.ShowDialog() == DialogResult.OK)
					{
						ok = true;
					}
				}

				//  OK なら
				if (ok)
				{
					//  昇格 (仮箱から引用時は変更履歴を常に取る)
					string errMsg;
					long newPid = bl20.ApplyRepairPlanReserveBoxToStandard(frm.ReserveBoxItem.Pid, detail, true, out errMsg);
					if (newPid == long.MinValue)
					{
						throw new Exception(errMsg);
					}

					//  引いてくる
					K300010BL bl30 = new K300010BL();
					StandardRepairPlanDetail newStdRepairPlanDetail = bl30.GetStandardRepairPlanDetail(newPid);

					//  追加
					if (constrType == COMSKCommon.ConstructionType.Temp)
					{
						ctrRepairPlan_T.Add(newStdRepairPlanDetail);
					}
					else if (constrType == COMSKCommon.ConstructionType.Building)
					{
						ctrRepairPlan_B.Add(newStdRepairPlanDetail);
					}
					else if (constrType == COMSKCommon.ConstructionType.Equipment)
					{
						ctrRepairPlan_E.Add(newStdRepairPlanDetail);
					}
					else if (constrType == COMSKCommon.ConstructionType.Outer)
					{
						ctrRepairPlan_Out.Add(newStdRepairPlanDetail);
					}
					else
					{
						ctrRepairPlan_Other.Add(newStdRepairPlanDetail);
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
		/// 再表示ボタンイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnRefresh_Click(object sender, EventArgs e)
		{
			ctrRepairPlan_B.ApplyViewSequence();
			ctrRepairPlan_T.ApplyViewSequence();
			ctrRepairPlan_E.ApplyViewSequence();
			ctrRepairPlan_Out.ApplyViewSequence();
			ctrRepairPlan_Other.ApplyViewSequence();
		}

		/// <summary>
		/// 承認ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnApprove_Click(object sender, EventArgs e)
		{
			//  バリデーション
			if (!ValidateControls())
			{
				return;
			}

			//  承認済みなら
			if (stdRepairPlan.ApprovalUserMstPid != long.MinValue)
			{
				//  承認取り消し確認
				if (MessageBox.Show("承認状態を取り消してもよろしいですか？", Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					UpdateData(false);

					//  イベント着火
					FireUpdatedAndClosed();

					//  閉じる
					this.Close();
				}
			}
			else
			{
				//  承認確認
				if (MessageBox.Show("この標準修繕計画を承認し、組合毎修繕計画から選べるようにしますか？", Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					UpdateData(true);

					//  イベント着火
					FireUpdatedAndClosed();

					//  閉じる
					this.Close();
				}
			}
		}

		/// <summary>
		/// 論理削除も表示チェック変更時イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void chkDisplayDeleted_CheckedChanged(object sender, EventArgs e)
		{
			ctrRepairPlan_B.DisplayData(chkDisplayDeleted.Checked);
			ctrRepairPlan_T.DisplayData(chkDisplayDeleted.Checked);
			ctrRepairPlan_E.DisplayData(chkDisplayDeleted.Checked);
			ctrRepairPlan_Out.DisplayData(chkDisplayDeleted.Checked);
			ctrRepairPlan_Other.DisplayData(chkDisplayDeleted.Checked);
		}

		/// <summary>
		/// 変更履歴chkイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void chkTakeChangeHistory_CheckedChanged(object sender, EventArgs e)
		{
			ctrRepairPlan_T.TakeHistory = chkTakeChangeHistory.Checked;
			ctrRepairPlan_B.TakeHistory = chkTakeChangeHistory.Checked;
			ctrRepairPlan_E.TakeHistory = chkTakeChangeHistory.Checked;
			ctrRepairPlan_Out.TakeHistory = chkTakeChangeHistory.Checked;
			ctrRepairPlan_Other.TakeHistory = chkTakeChangeHistory.Checked;
		}

		/// <summary>
		/// 削除確認
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="coms.COMSK.common.CustomEventArgs&lt;coms.COMSKService.StandardRepairPlanDetail&gt;"/> instance containing the event data.</param>
		void ctrRepairPlan_Deleting(object sender, coms.COMSK.common.CustomEventArgs<StandardRepairPlanDetail> e)
		{
			StandardRepairPlanDetail data = e.Param;


			if (data == null || data.Pid < 0) return;

			//  変更履歴取得Flgによって分岐
			bool doVerb = false;
			bool takeHistory = chkTakeChangeHistory.Checked;
			string updateReasonText = string.Empty;
			string deleteFlg = string.Empty;

			//  削除済みなら
			if (data.DeleteFlg == Constant.DELETEFLG_ON)
			{
				if (takeHistory == true)
				{
					//  復元確認
					K300100011 frm = new K300100011();
					if (frm.ShowDialog() == DialogResult.OK)
					{
						doVerb = true;
						updateReasonText = frm.Reason;
					}
				}
				else
				{
					//  メッセージボックスで確認
					string msg = "この項目の削除を取り消しますか？";
					if (MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						doVerb = true;
					}
				}

				//  復活させる
				deleteFlg = Constant.DELETEFLG_OFF;
			}
			//  それ以外は
			else
			{
				if (takeHistory == true)
				{
					// 削除確認
					K300100010 frm = new K300100010();
					if (frm.ShowDialog() == DialogResult.OK)
					{
						doVerb = true;
						updateReasonText = frm.Reason;
					}
				}
				else
				{
					//  メッセージボックスで確認
					string msg = "本当に削除してもよろしいですか？";
					if (MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						doVerb = true;
					}
				}

				//  削除する
				deleteFlg = Constant.DELETEFLG_ON;
			}

			// 削除 OK なら
			if (doVerb == true)
			{
				//  削除
				data.UpdateReason = updateReasonText;
				data.DeleteFlg = deleteFlg;

				try
				{
					K300010BL business = new K300010BL();
					business.UpdateStandardRepairPlanDetail(data, takeHistory);

					//  変更履歴不取得なら
					if (takeHistory == false)
					{
						#region 削除モードならデータを削除
						if (deleteFlg == Constant.DELETEFLG_ON)
						{
							//  データを削除
							if (sender is common.CtrBasicRepairPlan_B)
							{
								(sender as common.CtrBasicRepairPlan_B).RemoveItem(data);
							}
							else if (sender is common.CtrBasicRepairPlan_E)
							{
								(sender as common.CtrBasicRepairPlan_E).RemoveItem(data);
							}
						}
						#endregion

					}

					//  変更理由をクリア
					data.UpdateReason = string.Empty;
				}
				catch (Exception ex)
				{
					//  エラー処理
					Helper.WriteLog(ex);
					MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					//  フラグを元に戻す
					data.DeleteFlg = Constant.DELETEFLG_OFF;
				}

				//  再表示
				chkDisplayDeleted_CheckedChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 新規登録イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairPlan_Adding(object sender, EventArgs e)
		{
			common.CtrBasicRepairPlan_B ctr = sender as common.CtrBasicRepairPlan_B;

			//  先に工事ツリーを初期化
			coms.COMSK.common.ConstructionTreeMngr.Instance.UpdateTree();

			// 詳細画面表示
			StandardRepairPlanDetail detail = new StandardRepairPlanDetail()
			{
				Pid = long.MinValue,
				StandardRepairPlanPid = stdRepairPlan.Pid,
				ConstructionTypePid = (long)ctr.ConstrType,
				ConstructionItemPid = long.MinValue,
				ConstructionCategoryPid = long.MinValue,
				ConstructionPositionPid = long.MinValue,
				ConstructionRegionPid = long.MinValue,
				ConstructionSpecificationPid = long.MinValue,
				ConstructionDivisionPid = long.MinValue,
				ConstructionTypeName = coms.COMSK.common.ConstructionTreeMngr.Instance.GetConstructionTypeName(ctr.ConstrType),
				InsertUserMstPid = Helper.loginUserInfo.Pid,
				ParentPid = long.MinValue,
				DeleteFlg = Constant.DELETEFLG_OFF,
			};

			K300010021 frm = new K300010021(detail)
			{
				AddNew = true,
				TakeHistory = chkTakeChangeHistory.Checked,
			};
			if (this.stdRepairPlan.ApprovalUserMstPid != long.MinValue)
			{
				frm.SetApprovalMode();
			}
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//  追加
				ctr.Add(frm.WorkStdRepairPlanDetail);
			}
		}

		/// <summary>
		/// 新規登録イベント (設備)
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairPlan_E_Adding(object sender, EventArgs e)
		{
			//  先に工事ツリーを初期化
			coms.COMSK.common.ConstructionTreeMngr.Instance.UpdateTree();

			// 詳細画面表示
			StandardRepairPlanDetail detail = new StandardRepairPlanDetail()
			{
				Pid = long.MinValue,
				StandardRepairPlanPid = stdRepairPlan.Pid,
				ConstructionTypePid = (long)COMSK.common.COMSKCommon.ConstructionType.Equipment,
				ConstructionItemPid = long.MinValue,
				ConstructionCategoryPid = long.MinValue,
				ConstructionPositionPid = long.MinValue,
				ConstructionRegionPid = long.MinValue,
				ConstructionSpecificationPid = long.MinValue,
				ConstructionDivisionPid = long.MinValue,
				ConstructionTypeName = coms.COMSK.common.ConstructionTreeMngr.Instance.GetConstructionTypeName(COMSK.common.COMSKCommon.ConstructionType.Equipment),
				InsertUserMstPid = Helper.loginUserInfo.Pid,
				ParentPid = long.MinValue,
				DeleteFlg = Constant.DELETEFLG_OFF,
			};

			K300010022 frm = new K300010022(detail)
			{
				AddNew = true,
				TakeHistory = chkTakeChangeHistory.Checked,
			};
			if (this.stdRepairPlan.ApprovalUserMstPid != long.MinValue)
			{
				frm.SetApprovalMode();
			}
			if (frm.ShowDialog() == DialogResult.OK)
			{
				//  追加
				ctrRepairPlan_E.Add(frm.WorkStdRepairPlanDetail);
			}
		}

		#endregion

		#region Privates

		/// <summary>
		/// コントロールバリデーション
		/// </summary>
		/// <returns></returns>
		private bool ValidateControls()
		{
			bool ret = true;

			errSummary.Clear();

			ValidationChecker.CheckRequireControl(txtPlanName, errSummary, string.Format(ValidationChecker.REQUIRED_MESSAGE, "標準作成基準名"));

			foreach (Control c in panelSearch.Controls)
			{
				if (errSummary.GetError(c) != string.Empty)
				{
					ret = false;
				}
			}

			//  OK
			return ret;
		}

		/// <summary>
		/// 承認状態に応じて状態を設定
		/// </summary>
		private void UpdateLayout()
		{
			//  承認済みなら
			if (stdRepairPlan.ApprovalUserMstPid != long.MinValue)
			{
				//  ボタン名称を変える
				btnApprove.Text = "承認取消";

				//  他コントロール状態設定
				txtPlanName.Enabled = false;
				txtNote.Enabled = false;
				chkTakeChangeHistory.Enabled = false;

				//  削除不可にする
				ctrRepairPlan_T.SetApprovalMode();
				ctrRepairPlan_B.SetApprovalMode();
				ctrRepairPlan_E.SetApprovalMode();
				ctrRepairPlan_Out.SetApprovalMode();
				ctrRepairPlan_Other.SetApprovalMode();

			}
		}

		/// <summary>
		/// 各タブにデータをロードさせる
		/// </summary>
		/// <param name="list">The list.</param>
		private void LoadDatas(List<StandardRepairPlanDetail> list)
		{
			// 仮設
			long pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Temp;
			List<StandardRepairPlanDetail> subList = (from item in list
													  where item.ConstructionTypePid == pidType
													  select item).ToList<StandardRepairPlanDetail>();
			ctrRepairPlan_T.LoadData(subList);
			ctrRepairPlan_T.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Temp;
			ctrRepairPlan_T.StdRepairPlanPid = pid;

			// 建築
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Building;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<StandardRepairPlanDetail>();
			ctrRepairPlan_B.LoadData(subList);
			ctrRepairPlan_B.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Building;
			ctrRepairPlan_B.StdRepairPlanPid = pid;

			// 設備
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Equipment;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<StandardRepairPlanDetail>();
			ctrRepairPlan_E.LoadData(subList);
			//ctrRepairPlan_E.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Equipment;
			ctrRepairPlan_E.StdRepairPlanPid = pid;

			// 外構
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Outer;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<StandardRepairPlanDetail>();
			ctrRepairPlan_Out.LoadData(subList);
			ctrRepairPlan_Out.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Outer;
			ctrRepairPlan_Out.StdRepairPlanPid = pid;

			// その他
			pidType = (long)coms.COMSK.common.COMSKCommon.ConstructionType.Other;
			subList = (from item in list
					   where item.ConstructionTypePid == pidType
					   select item).ToList<StandardRepairPlanDetail>();
			ctrRepairPlan_Other.LoadData(subList);
			ctrRepairPlan_Other.ConstrType = coms.COMSK.common.COMSKCommon.ConstructionType.Other;
			ctrRepairPlan_Other.StdRepairPlanPid = pid;

		}

		/// <summary>
		/// データを登録する
		/// </summary>
		/// <param name="approve">true だと承認</param>
		/// <returns></returns>
		private bool UpdateData(bool approveFlg)
		{
			bool ret = false;

			try
			{
				K300010BL business = new K300010BL();

				//  データを回収
				stdRepairPlan.StandardRepairPlanName = txtPlanName.Text;
				stdRepairPlan.Note = txtNote.Text;
				if (chkTakeChangeHistory.Checked)
				{
					stdRepairPlan.HistoryFlg = coms.COMSK.common.COMSKCommon.HAS_HISTORY_FLG_ON;
				}
				else
				{
					stdRepairPlan.HistoryFlg = coms.COMSK.common.COMSKCommon.HAS_HISTORY_FLG_OFF;
				}
				if (approveFlg)
				{
					stdRepairPlan.ApprovalUserMstPid = Helper.loginUserInfo.Pid;
				}
				else
				{
					stdRepairPlan.ApprovalUserMstPid = long.MinValue;
				}
				stdRepairPlan.UpdateUserMstPid = Helper.loginUserInfo.Pid;
				business.UpdateStandardRepairPlan(stdRepairPlan);


				//  送信用一時データ
				List<StandardRepairPlanDetail> list = new List<StandardRepairPlanDetail>();

				//  データを回収する
				list = list.Concat(ctrRepairPlan_T.SortedDataSource)
					.Concat(ctrRepairPlan_B.SortedDataSource)
					.Concat(ctrRepairPlan_E.SortedDataSource)
					.Concat(ctrRepairPlan_Out.SortedDataSource)
					.Concat(ctrRepairPlan_Other.SortedDataSource)
					.ToList();

				//  送信5
                business.UpdateStandardRepairPlanDetailViewSequence(stdRepairPlan.Pid, list, chkTakeChangeHistory.Checked);

				//  OK
				ret = true;
			}
			catch (Exception ex)
			{
				//  エラー
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			//  OK
			return ret;
		}

		/// <summary>
		/// 保存イベント着火
		/// </summary>
		private void FireUpdatedAndClosed()
		{
			if (UpdatedAndClosed != null)
			{
				UpdatedAndClosed(this, EventArgs.Empty);
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
			"    AI.partsCode LIKE '%btnK300010020%'";

			COMSD.business.D100109BL bl109 = new coms.COMSD.business.D100109BL();
			COMSDService.AuthorityInfo[] authorities = bl109.SearchAuthorityInfo(sql, "", "");
			
			btnApprove.Enabled = Helper.GetAuthority(authorities, "btnK300010020001", userPostCode, userPositionCode);
			btnOutputRepairPlan.Enabled = Helper.GetAuthority(authorities, "btnK300010020002", userPostCode, userPositionCode);
			btnUpdate.Enabled = Helper.GetAuthority(authorities, "btnK300010020003", userPostCode, userPositionCode);
		}
		#endregion

		private void K300010020_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (childFrom != null)
			{
				childFrom.Close();
			}
		}

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (childFrom != null)
			{
				childFrom.Close();
			}
		}
    }
}
