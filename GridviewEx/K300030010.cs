using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//MJC_DEV-724  追加 kubota 20181225 
using System.Reflection;
using coms.COMMON;
using coms.COMMONService;
using coms.COMSKService;
using coms.COMSK.business;
using coms.COMSK.common;

namespace coms.COMSK.ui
{
    /// <summary>
    /// 長期修繕一覧
    /// </summary>
    public partial class K300030010 : MyForm
	{

		#region プロパティ

		/// <summary>
		/// プリセットする組合
		/// </summary>
		[Browsable(false)]
		public long KumiaiInfoPid { get; set; }

		#endregion

		#region メンバ

		/// <summary>
		/// 表示データ
		/// </summary>
		//List<KumiaiLongRepairPlan2> DataSource = new List<KumiaiLongRepairPlan>();
        //MJC_DEV-724 【開発】No.321 集計機能の集計対象について チェックボックス列追加（このフォームでのみ使用）
        List<KumiaiLongRepairPlan2> DataSource = new List<KumiaiLongRepairPlan2>();

        /// <summary>
        /// 集計対象長計フラグデータ
        /// </summary>
        //List<KumiaiLongRepairPlanCheckedFlg> Flglist = new List<KumiaiLongRepairPlanCheckedFlg>();
        //MJC_DEV-724 【開発】No.321 集計機能の集計対象について チェックボックス列追加（このフォームでのみ使用）

        List<KumiaiLongRepairPlanCheckedFlg> Flglist = new List<KumiaiLongRepairPlanCheckedFlg>();

		/// <summary>
		/// ソートカラム
		/// </summary>
		private DataGridViewColumn sortColumn = null;

		/// <summary>
		/// ソート順序
		/// </summary>
		private string sortOrder = string.Empty;

		/// <summary>
		/// ソートフラグ
		/// </summary>
		private bool isSort = false;

		/// <summary>
		/// 検索条件
		/// </summary>
		KumiaiLongRepairPlanSearch search = new KumiaiLongRepairPlanSearch();

		/// <summary>
		/// セル内ボタンの権限
		/// key:ボタン、value:trueなら権限所持
		/// </summary>
		private Dictionary<string, bool> authority_dict = new Dictionary<string, bool>();

		/// <summary>
		/// 点検・履歴
		/// </summary>
		private readonly string AUTH_CHECK_HISTORY = "btnK300030010002";

		/// <summary>
		/// 修繕基準
		/// </summary>
		private readonly string AUTH_REPAIR_PLAN = "btnK300030010003";

		/// <summary>
		/// 修繕履歴
		/// </summary>
		private readonly string AUTH_REPAIR_HISTORY = "btnK300030010004";

		/// <summary>
		/// 長期修繕計画
		/// </summary>
		private readonly string AUTH_LONG_REPAIR_PLAN = "btnK300030010005";

		/// <summary>
		/// 修繕積立金計画
		/// </summary>
		private readonly string AUTH_REPAIR_RESERVE_PLAN = "btnK300030010006";

		/// <summary>
		/// コピー
		/// </summary>
		private readonly string AUTH_COPY = "btnK300030010008";

        //20140826 Linh ADD MJC_DEV-213
		public const string COMSK_INTENSIVEFLG_0010 = "0010";
        public const string COMSK_INTENSIVEFLG_0030 = "0030";
        // 集約状態
        public CodeMst[] intensiveFlg = COMSK.common.COMSKCommon.GetIntensiveFlgCode();
        //END MJC_DEV-213

		#endregion

		#region コンストラクタ

		/// <summary>
        /// コンストラクタ
        /// </summary>
		public K300030010()
		{
			InitializeComponent();
            //フラグ情報リストは保持しておく必要があるので外のリストに渡す
            K300030BL business = new K300030BL();
            Flglist = business.GetKumiaiLongRepairPlanCheckedFlg();
			KumiaiInfoPid = long.MinValue;
		}

		#endregion

		#region イベント

		#region その他

		/// <summary>
        /// フォームロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void K300030010_Load(object sender, EventArgs e)
        {
			//  コントロール初期化
            ctrTeamCreator.BindData();
            ctrTeamFront.BindData();
            
			//  権限設定
			SetAuthorities();

			//  組合プリセットがあれば
			if (KumiaiInfoPid != long.MinValue)
			{
				ctrKumiaiKana1.BindData(KumiaiInfoPid);
			}
			//  タイプマスタを設定
			Helper.BindCodeMstToCheckboxList(COMSKCommon.COMSK_LONGREPAIRPLAN_TYPE_CODE, chkTypeDEV);


			//for (int i = 0; i < chkType.Items.Count; i++)
			//{
			//    chkType.SetItemChecked(i, true);
			//}
			chkTypeDEV.SetAllItemCheckedFromZero(true);


			//  ステータスマスタを設定
			Helper.BindCodeMstToCheckboxList(COMSKCommon.COMSK_LONGREPAIRPLAN_STATUS_CODE, chkStatusDEV);

			//for (int i = 0; i < chkStatus.Items.Count; i++)
			//{
			//    chkStatus.SetItemChecked(i, true);
			//}
			chkStatusDEV.SetAllItemCheckedFromZero(true);
			//  ページャー初期化
			if (KumiaiInfoPid != long.MinValue)
			{
				//  検索
				LoadKumiaiLongRepairPlan();
			}
			else
			{
				ctrPageLongtermRepair.Total = 0;
				ctrPageLongtermRepair.InitPager();
			}

			this.ClearDateSearch();
		}

		private void ClearDateSearch()
        {
			dtpNextNotifyDateFrom.Value = DateTime.Now;
			dtpNextNotifyDateFrom.Value = DateTime.MinValue;
			dtpNextNotifyDateTo.Value = DateTime.Now;
			dtpNextNotifyDateTo.Value = DateTime.MinValue;
			dtpInsertDateFrom.Value = DateTime.Now;
			dtpInsertDateFrom.Value = DateTime.MinValue;
			dtpInsertDateTo.Value = DateTime.Now;
			dtpInsertDateTo.Value = DateTime.MinValue;
		}

		/// <summary>
		/// ページ変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrPageLongtermRepair_PageIndexChanged(object sender, EventArgs e)
		{
			LoadKumiaiLongRepairPlan();
		}

		/// <summary>
		/// ページ数変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrPageLongtermRepair_PageSizeChanged(object sender, EventArgs e)
		{
			LoadKumiaiLongRepairPlan();
		}

		#endregion

		#region ボタンイベント

		/// <summary>
        /// 検索ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
			ctrPageLongtermRepair.CurrentPageIndex = 0;
			sortColumn = null;
			sortOrder = Constant.SORT_DOWN;
			CollectSearchInfo();
			LoadKumiaiLongRepairPlan();
			//検索時、組合名絞り込みが有効の場合のみ集計対象チェックボックスを有効化する
			//20190124　集約関連長計表示フラグ有効時も、非活性の対象にする仕様を追加
			if ((ctrKumiaiKana1.cmbKumiaiName.SelectedIndex != 0)
				&& (ctrKumiaiKana1.cmbKumiaiName.Items.Count != 0)
				&& (ctrKumiaiKana1.cmbKumiaiName.SelectedIndex != -1)
				&& (radioButton2.Checked == false))
			{
				this.clCheckLongRepairPlan.ReadOnly = false;
				this.btnSaveKumiailongRepairPlanChackedFlg.Enabled = true;
			}
			else
			{
				this.clCheckLongRepairPlan.ReadOnly = true;
				this.btnSaveKumiailongRepairPlanChackedFlg.Enabled = false;
			}
		}

        /// <summary>
        /// メニューへ戻るボタンの押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		/// <summary>
		/// 検索条件クリア
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnClear_Click(object sender, EventArgs e)
        {
			ctrKumiaiKana1.Clear();
			ctrTeamFront.Reset();
			ctrTeamCreator.Reset();
			this.ClearDateSearch();
			radioButton1.Checked = true;
            radioButton2.Checked = false;

            //for (int i = 0; i < chkStatus.Items.Count; i++)
            //{
            //    chkStatus.SetItemChecked(i, true);
            //}
            //for (int i = 0; i < chkType.Items.Count; i++)
            //{
            //    chkType.SetItemChecked(i, true);
            //}
            chkTypeDEV.SetAllItemCheckedFromZero(true);
            chkStatusDEV.SetAllItemCheckedFromZero(true);

			CollectSearchInfo();
		}

		/// <summary>
		/// 新規作成ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnCreate_Click(object sender, EventArgs e)
		{
			//  組合が入っているかチェック
			COMMONService.KumiaiInfo info = ctrKumiaiKana1.CurrentKumiaiInfo;
			if (info == null || (info != null && info.KumiaiCode == null))
			{
				errSummary.SetError(ctrKumiaiKana1, "物件を選択してください");
				MessageBox.Show("物件を選択してください", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			//  データ確認
			// 作成開始年のチェックがあるのでここneedMaxTermが０で良い
			// 今後150期必ず作成必要ある場合　→　100から150に変更すればオケ
			if (!COMSKCommon.CheckReadyLongRepairPlanData(info.Pid, 0))
			{
				return;
			}

			//  もとにする標準修繕計画を選択
			K300100020 frmSelectPlan = new K300100020();
			if (frmSelectPlan.ShowDialog() == DialogResult.OK)
			{
				//  長計初期設定画面
				K300030030 frmSettings = new K300030030(info);
				if (frmSettings.ShowDialog() == DialogResult.OK)
				{
					//  作成したデータを取得
					KumiaiLongRepairPlan data = frmSettings.LongRepairPlan;

					//  標準修繕基準 PID
					data.StandardRepairPlanPid = frmSelectPlan.SelectedStandardRepairPlanPid;

					//  組合情報 PID をセット
					data.KumiaiInfoPid = info.Pid;

					//  ユーザ PID をセット
					data.FrontUserMstPid = Helper.loginUserInfo.Pid;
					data.InsertUserMstPid = Helper.loginUserInfo.Pid;

					try
					{
						//  作成
						K300030BL business = new K300030BL();
						long pid = business.CreateKumiaiLongRepairPlan(data);
						if (pid == long.MinValue)
						{
							throw new Exception("長期修繕計画データを作成できませんでした。");
						}
                        //ここで集計対象フラグを作成した長計につける　MJC_DEV-724 【開発】No.321 集計機能の集計対象について kubota-add 20100107
                        KumiaiLongRepairPlanCheckedFlg Flginfo = new KumiaiLongRepairPlanCheckedFlg();
                        Flginfo.KumiaiInfoPid = data.KumiaiInfoPid;
                        Flginfo.KumiaiLongRepairPlanPid = pid;
                        business.UpdateKumiaiLongRepairPlanCheckedFlg(Flginfo);
                        //更新した情報を画面に反映
                        Flglist = new List<KumiaiLongRepairPlanCheckedFlg>();
                        Flglist = business.GetKumiaiLongRepairPlanCheckedFlg();
                        //ここで集計対象フラグを作成した長計につける　MJC_DEV-724 【開発】No.321 集計機能の集計対象について kubota-add-end 20100107

						//  表示
						K300040010 frmRepairPlan = new K300040010(pid);
						frmRepairPlan.Show();

						//  再検索
						CollectSearchInfo();
						ctrPageLongtermRepair.CurrentPageIndex = 0;
						LoadKumiaiLongRepairPlan();
					}
					catch (Exception ex)
					{
						Helper.WriteLog(ex);
						MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}

				}

			}
		}

		/// <summary>
		/// 新旧比較ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnDiff_Click(object sender, EventArgs e)
		{
			//  二つ選択されているか調べる
			KumiaiLongRepairPlan[] list = (from item in DataSource
										   where item.Select
										   //select item).ToArray<KumiaiLongRepairPlan>();
                                           //MJC_DEV-724 【開発】No.321 集計機能の集計対象について
										   select item).ToArray<KumiaiLongRepairPlan2>();

            //tho add MJC_DEV-202
            if (list.Any(d => d.TypeCode != COMSKCommon.COMSK_LONG_REPAIR_PLAN_TYPE_REEXAM))
            {
                MessageBox.Show("見直長計以外が選択されています。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // end MJC_DEV-202

			if (list.Length != 2)
			{
				MessageBox.Show("比較する長期修繕計画を二つ選択してください。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			if (list[0].KumiaiInfoPid != list[1].KumiaiInfoPid)
			{
				MessageBox.Show("同一物件の長期修繕計画を選択してください。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			try
			{
				//  出力
				K300030BL business = new K300030BL();
				coms.COMSKService.FileEntry ret = business.ExportDiffLongRepairPlanDetails(list[0].Pid, list[1].Pid);

				// セルフォマット条件
				ret = ReportCommon.KumiaiLongRepairPlanReportDiff_CellFormat(ret);

				//  保存
				Helper.SaveFile(ret, this);
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		#endregion

		#region データグリッドイベント
		private void gcLongRepairList_CellBackColorNeeded(object sender, COMMON.ui.CellBackColorNeededEventArgs e)
		{
			if (e.Column == clSelect)
			{
				e.BackColor = Color.PaleGreen;
			}
		}

		/// <summary>
		/// エディタ表示イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		private void gvLongRepairList_ShowingEditor(object sender, CancelEventArgs e)
		{
            //tho 20140814 MJC_DEV-202　コンメントアウト↓
            //try
            //{
            //    //  デフォルトはキャンセル
            //    e.Cancel = true;

            //    //  行データ取得
            //    KumiaiLongRepairPlan obj = gvLongRepairList.GetRow(gvLongRepairList.FocusedRowHandle) as KumiaiLongRepairPlan;

            //    //  選択列なら
            //    if (gvLongRepairList.FocusedColumn == clSelect)
            //    {
            //        //  見直し長計なら
            //        if (obj.TypeCode == COMSKCommon.COMSK_LONG_REPAIR_PLAN_TYPE_REEXAM)
            //        {
            //            //  編集可能
            //            e.Cancel = false;
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //}
            //end MJC_DEV-202

		}

		#endregion

		#endregion

		#region Private

		/// <summary>
		/// 検索条件を集める
		/// </summary>
		private void CollectSearchInfo()
		{
			search.UserMstPid = Helper.loginUserInfo.Pid;
			search.NextNotifyDateFrom = DateTime.MinValue;
			search.NextNotifyDateTo = DateTime.MinValue;
			search.InsertDateFrom = DateTime.MinValue;
			search.InsertDateTo = DateTime.MinValue;

			//  物件
			if (ctrKumiaiKana1.CurrentKumiaiInfo != null)
			{
				search.KumiaiInfoPid = ctrKumiaiKana1.CurrentKumiaiInfo.Pid;
			}
			else
			{
				search.KumiaiInfoPid = long.MinValue;
			}

			//  管理担当
			search.FrontTeamPid = ctrTeamFront.TeamMstPid;
			search.FrontUserPid = ctrTeamFront.UserId;

			//  作成担当
			search.InsertTeamPid = ctrTeamCreator.TeamMstPid;
			search.InsertUserPid = ctrTeamCreator.UserId;

			//  次期作成通知日
			if (dtpNextNotifyDateFrom.Value > DateTime.MinValue)
			{
				search.NextNotifyDateFrom = dtpNextNotifyDateFrom.Value;
			}
			if (dtpNextNotifyDateTo.Value > DateTime.MinValue)
			{
				search.NextNotifyDateTo = dtpNextNotifyDateTo.Value;
			}

			//  作成日
			if (dtpInsertDateFrom.Value > DateTime.MinValue)
			{
				search.InsertDateFrom = dtpInsertDateFrom.Value;
			}
			if (dtpInsertDateTo.Value > DateTime.MinValue)
			{
				search.InsertDateTo = dtpInsertDateTo.Value;
			}

			//  種類
			//search.TypeList = COMSKCommon.GetSelectedItemsAsArrayOfCheckedCombobox(chkType);
            search.TypeList = COMSKCommon.GetSelectedTextAsArrayOfCheckedCombobox(chkTypeDEV);

			//  状態
			//search.StatusList = COMSKCommon.GetSelectedItemsAsArrayOfCheckedCombobox(chkStatus);
			search.StatusList = COMSKCommon.GetSelectedTextAsArrayOfCheckedCombobox(chkStatusDEV);
            //
            search.FlgDisplay = (radioButton2.Checked) ? true : false;
		}

		/// <summary>
		/// 検索条件に基づいて検索
		/// </summary>
		private void LoadKumiaiLongRepairPlan()
		{

			//  開始・終了ページ
			search.Start = ctrPageLongtermRepair.CurrentPageIndex * ctrPageLongtermRepair.PageSize;
			search.Count = ctrPageLongtermRepair.PageSize;

			string orderClause = string.Empty;
			if (sortColumn == null)
			{
				sortColumn = clInsertDateTime;
				sortOrder = Constant.SORT_DOWN;
				orderClause = "[Base].[insertDateTime] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clInsertDateTime)
			{
				orderClause = "[Base].[insertDateTime] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clNextNotifyDate)
			{
				orderClause = "[PI].[nextNotifyDate] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clType)
			{
				orderClause = "[TypeCodeMst].[title] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clStatus)
			{
				orderClause = "[StatusCodeMst].[title] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			//Diem add https://reci.backlog.jp/view/MJC_DEV-234
			else if (sortColumn == clIntensiveFlg)
			{
				orderClause = "[Base].[intensiveFlg] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}

			else if (sortColumn == clKumiaiName)
			{
				orderClause = "[KumiaiInfo].[kumiaiName] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clName)
			{
				orderClause = "[Base].[name] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clTrusteesProposalDate)
			{
				orderClause = "[Base].[trusteesProposalDate] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clTrusteesApprovalDate)
			{
				orderClause = "[Base].[trusteesApprovalDate] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clAssemblyApprovalDate)
			{
				orderClause = "[Base].[assemblyApprovalDate] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clInsertUserName)
			{
				orderClause = "[UM_C].[userName] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			else if (sortColumn == clCustomerUserName)
			{
				orderClause = "[UM_F].[userName] " + sortOrder + ", [Base].[pid] " + sortOrder;
			}
			search.Order = orderClause;

			//  検索
			K300030BL business = new K300030BL();
			int total = 0;
            //MJC_DEV-724 【開発】No.321 集計機能の集計対象について
            var kumiaiiLongRepairPlanList = business.SearchKumiaiLongRepairPlan(search, out total);


            //ここに集計対象長計ＩＤリストに入っているかどうかを判定し、KumiaiLongRepairPlan2型のインスタンスに読み込む
            //作成した一時データをDataSorceに読み込む
            List<KumiaiLongRepairPlan2> kumiaiiLongRepairPlanList2 = new List<KumiaiLongRepairPlan2> ();
            foreach (var item in kumiaiiLongRepairPlanList)
            {
                KumiaiLongRepairPlan2 tempdata = new KumiaiLongRepairPlan2();
                PropertyInfo[] infoArray = item.GetType().GetProperties();
                PropertyInfo[] infoArray2 = tempdata.GetType().GetProperties();

                //拡張クラスインスタンスのkumiaiiLongRepairPlanList2に、長計集計フラグ情報を追加して
                //全要素コピーを行う
                foreach (PropertyInfo info in infoArray)
                {
                    foreach (PropertyInfo info2 in infoArray2)
                    {
                        //同名プロパティ値をコピー
                        if (info.Name == info2.Name)
                        {
                            var val = info.GetValue(item, null);
                            //info.SetValue(tempdata, 200);
                            info2.SetValue(tempdata,val,null);
                            //集計対象長計ＩＤリスト内に、長計IDがあるかどうかを判断し、true/falseを追加する
                            if(info.Name == "Pid")
                            {
                                try
                                {

                                    foreach (var Flginfo in Flglist)
                                    {
                                        if (Flginfo.KumiaiLongRepairPlanPid == (long)val)
                                        {
                                            tempdata.KumiaiLongRepairPlanCheckedFlg = true;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Helper.WriteLog(ex);
                                    MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }


                        
                        }
                    }
                }

               // tempdata = (KumiaiLongRepairPlan2)item;




                kumiaiiLongRepairPlanList2.Add(tempdata);
                
            }
            DataSource = kumiaiiLongRepairPlanList2;
            gcLongRepairList.SetData(DataSource);

			//  ページャーをセット
			ctrPageLongtermRepair.Total = total;
			ctrPageLongtermRepair.InitPager();

			//NOTE: 参考
			//K200010030 frmLongtermRepairPlan = new K200010030();
			//// 上部への値設定
			//if (infBase != null)
			//{
			//    frmLongtermRepairPlan.KumialName = infBase.Association;
			//    frmLongtermRepairPlan.RepairPlanName = infBase.RepairPlanName;
			//    frmLongtermRepairPlan.Kind = infBase.Kind;
			//    frmLongtermRepairPlan.LongtermPlanName = infBase.LongtermRepairPlanName;
			//    frmLongtermRepairPlan.Status = infBase.Status;
			//    frmLongtermRepairPlan.NextDate = infBase.NextDate;
			//}
			//if (string.IsNullOrEmpty(strOverWriteRepairPlanName) == false)
			//{
			//    frmLongtermRepairPlan.RepairPlanName = strOverWriteRepairPlanName;
			//}
			//if (string.IsNullOrEmpty(strOverWriteKind) == false)
			//{
			//    frmLongtermRepairPlan.Kind = strOverWriteKind;
			//}
			//if (bIsAdd == true)
			//{
			//    frmLongtermRepairPlan.NextDate = DateTime.Now.AddYears(1);
			//    frmLongtermRepairPlan.Status = "案";
			//}

			//frmLongtermRepairPlan.AddMode = bIsAdd;
			//frmLongtermRepairPlan.Show();

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
			"    AI.partsCode LIKE '%btnK300030010%'";

			COMSD.business.D100109BL bl109 = new coms.COMSD.business.D100109BL();
			COMSDService.AuthorityInfo[] authorities = bl109.SearchAuthorityInfo(sql, "", "");
			
			btnCreate.Enabled = Helper.GetAuthority(authorities, "btnK300030010001", userPostCode, userPositionCode);
			btnDiff.Enabled = Helper.GetAuthority(authorities, "btnK300030010007", userPostCode, userPositionCode);
			authority_dict.Add(AUTH_CHECK_HISTORY, Helper.GetAuthority(authorities, AUTH_CHECK_HISTORY, userPostCode, userPositionCode));
			authority_dict.Add(AUTH_REPAIR_PLAN, Helper.GetAuthority(authorities, AUTH_REPAIR_PLAN, userPostCode, userPositionCode));
			authority_dict.Add(AUTH_REPAIR_HISTORY, Helper.GetAuthority(authorities, AUTH_REPAIR_HISTORY, userPostCode, userPositionCode));
			authority_dict.Add(AUTH_LONG_REPAIR_PLAN, Helper.GetAuthority(authorities, AUTH_LONG_REPAIR_PLAN, userPostCode, userPositionCode));
			authority_dict.Add(AUTH_REPAIR_RESERVE_PLAN, Helper.GetAuthority(authorities, AUTH_REPAIR_RESERVE_PLAN, userPostCode, userPositionCode));
			authority_dict.Add(AUTH_COPY, Helper.GetAuthority(authorities, AUTH_COPY, userPostCode, userPositionCode));
		}
		#endregion

        private void btnCreate2_Click(object sender, EventArgs e)
        {
            //MK-898 明和管理のコムズ動作環境にOffice2013を加えてほしい
            //  二つ選択されているか調べる
            //KumiaiLongRepairPlan[] list = (from item in DataSource
            //                               where item.Select
            //                               select item).ToArray<KumiaiLongRepairPlan>();
            KumiaiLongRepairPlan[] list = (from item in DataSource
                                           where item.Select
                                           select item).ToArray<KumiaiLongRepairPlan2>();
            if (list.Length == 0)
            {
                MessageBox.Show("データを選択してください。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            List<long?> kumiaiInfoPids = list.Select(d => (long?)d.KumiaiInfoPid).Distinct().ToList();
			List<string> fiscalYearList = new List<string>();
            K300030BL business = new K300030BL();

            if(kumiaiInfoPids.Count > 0)
            {
                if (kumiaiInfoPids.Count > 1 && !business.CheckKumiaiTermInfo_FiscalYear(kumiaiInfoPids))
                {
                    MessageBox.Show("会計期マスタが一致しません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (list.Select(d=>d.TypeCode).Distinct().Count() > 1)
                {
                    MessageBox.Show("長計の種類が一致しません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (kumiaiInfoPids.Count > 1 && !business.CheckKumiaiLongRepairPlanTaxMst_TaxRate(list.Select(d => (long?)d.Pid).ToList()))
                {
                    MessageBox.Show("消費税率・物価上昇率が一致しません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
				fiscalYearList = business.CheckKumiaiLongRepairPlanfiscalYear(list.Select(d => (long?)d.Pid).ToList());
				bool fiscalYearReult = fiscalYearList.Count != 0 && fiscalYearList.Count == fiscalYearList.Where(d => d == fiscalYearList[0].ToString()).Count();
				if (kumiaiInfoPids.Count > 1 && !fiscalYearReult)
                {
                    MessageBox.Show("作成開始年度が一致しません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

			//  長計初期設定画面
			bool allChildIs60 = !list.Any(lrp => lrp.ReportYearCount != 60);
			K300030031 frmSettings = new K300030031(list.Select(d => d.Pid).ToList(), list.Select(d => d.TypeCode).FirstOrDefault(), fiscalYearList[0], allChildIs60);

            if (frmSettings.ShowDialog() == DialogResult.OK)
            {
                //  再検索
                CollectSearchInfo();
                ctrPageLongtermRepair.CurrentPageIndex = 0;
                LoadKumiaiLongRepairPlan();

                frmSettings.Close();
            }
        }

        private void ctrKumiaiKana1_Kumiai_Changed(object sender, EventArgs e)
        {
            if (ctrKumiaiKana1.cmbKumiaiName.SelectedIndex != 0)
            {
                //this.clCheckLongRepairPlan.OptionsColumn.AllowEdit = true;

                radioButton2.Checked = true;
            }
            else
            {
                //this.clCheckLongRepairPlan.OptionsColumn.AllowEdit = false;
            }
        }

        #region 集計対象長計保存
        /// <summary>
        /// 集計対象長計保存
        /// </summary>
        private void SaveKumiailongRepairPlanChackedFlg_Click(object sender, EventArgs e)
        {
            KumiaiLongRepairPlanCheckedFlg Flginfo = new KumiaiLongRepairPlanCheckedFlg();
            try
            {
                DialogResult result = MessageBox.Show("保存しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    K300030BL business = new K300030BL();
                    //画面の長計チェック数のカウント
                    int checkCount = DataSource
                        .Where(x => x.KumiaiLongRepairPlanCheckedFlg == true)
                        .Count();

                    //表示されている組合のIDを特定する
                    Flginfo.KumiaiInfoPid = DataSource
                    .Select(x => x.KumiaiInfoPid)
                    .Distinct()
                    .FirstOrDefault()
                    ;

                    //チェックがついている長計（複数ある場合は一つ目）のIDを取得する
                    Flginfo.KumiaiLongRepairPlanPid = DataSource
                    .Where(x => x.KumiaiLongRepairPlanCheckedFlg == true)
                    .Select(x => x.Pid)
                    .FirstOrDefault();

                    var checkedKumiaiLongRepairPlanPid = Flglist.Where(x => x.KumiaiInfoPid == Flginfo.KumiaiInfoPid)
                        .Select(x => x.KumiaiLongRepairPlanPid).FirstOrDefault();
                    //絞り込み対象の組合IDがフラグテーブル内の組合IDと一致する長計情報が存在する？
                    if (Flglist
                        .Where(x => x.KumiaiInfoPid == Flginfo.KumiaiInfoPid)
                        .Any())
                    {
                        //画面内にフラグ付き長計があるかないかで分岐
                        //画面内にフラグテーブル内の長計IDと一致する長計情報が存在する？
                        if (!DataSource
                            .Where(x => x.Pid == checkedKumiaiLongRepairPlanPid)
                            .Any())
                        {
                            checkCount++;
                            //画面外にチェック付き長計が存在するため更新用の長計IDにその長計のIDをセット
                            Flginfo.KumiaiLongRepairPlanPid = checkedKumiaiLongRepairPlanPid;
                        }


                        //一致する組合情報あり
                        if (checkCount == 0)
                        {
                            //フラグテーブルから現在絞り込みを行っている組合の情報を削除する(長計ID0を入力すると入力した組合のデータを削除する）
                            Flginfo.KumiaiLongRepairPlanPid = 0;
                            UpdateKumiaiLongRepairPlanCheckedFlg(Flginfo);
                            return;
                        }
                        else if (checkCount == 1)
                        {
                            if (DataSource
                                .Where(x => x.KumiaiLongRepairPlanCheckedFlg == true)
                                .Select(x => x.IntensiveFlg)
                                .FirstOrDefault() == "0010")
                            {
                                MessageBox.Show("集約長計は選択できません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            UpdateKumiaiLongRepairPlanCheckedFlg(Flginfo);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("集計対象は1物件に対し複数選択はできません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    else
                    {
                        //現在集計対象外の組合の場合の分岐
                        
                        if (checkCount == 0)
                        {
                            //①の分岐
                            return;
                        }
                        else if (checkCount == 1)
                        {
                            if(DataSource
                            .Where(x => x.KumiaiLongRepairPlanCheckedFlg == true)
                            .Select(x => x.IntensiveFlg)
                            .FirstOrDefault() == "0010"
                            )
                            {
                                MessageBox.Show("集約長計は選択できません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }

                            //④の分岐 チェックされた長計情報をフラグテーブルに追加する
                            Flginfo.KumiaiLongRepairPlanPid = DataSource
                                .Where(x => x.KumiaiLongRepairPlanCheckedFlg == true)
                                .Select(x => x.Pid)
                                .FirstOrDefault();
                            UpdateKumiaiLongRepairPlanCheckedFlg(Flginfo);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("集計対象は1物件に対し複数選択はできません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }

                    //画面内にフラグテーブル内の長計IDと一致する長計情報が存在する？
                    if (!DataSource
                        .Where(x => x.Pid == checkedKumiaiLongRepairPlanPid)
                        .Any())
                    {
                        checkCount++;
                    }

                    if (checkCount != 0)
                    {
                        //複数チェック時はポップアップを出して操作をキャンセル
                        if (checkCount >= 2)
                        {
                            MessageBox.Show("集計対象は1物件に対し複数選択はできません。", Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
                MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region 集計フラグ情報更新
        /// <summary>
        /// 集計フラグ情報更新
        /// </summary>
        private void UpdateKumiaiLongRepairPlanCheckedFlg(KumiaiLongRepairPlanCheckedFlg Flginfo)
        {
           // KumiaiLongRepairPlanCheckedFlg Flginfo = new KumiaiLongRepairPlanCheckedFlg();
            try
            {
                K300030BL business = new K300030BL();
                business.UpdateKumiaiLongRepairPlanCheckedFlg(Flginfo);
                //更新後のフラグ情報再取得
                Flglist = new List<KumiaiLongRepairPlanCheckedFlg>();
                Flglist = business.GetKumiaiLongRepairPlanCheckedFlg();
                return;
            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
                MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        private void gcLongRepairList_MouseUp_1(object sender, MouseEventArgs e)
        {
			try
			{
				if (isSort == true)
				{
					var hit = gcLongRepairList.HitTest(e.X, e.Y);

					if (hit.Type == DataGridViewHitTestType.ColumnHeader)
					{
						DataGridViewColumn column = gcLongRepairList.Columns[hit.ColumnIndex];

						// 特定のカラムなら
						if (column == clInsertDateTime ||
							column == clNextNotifyDate ||
							column == clIntensiveFlg ||
							column == clType ||
							column == clStatus ||
							column == clKumiaiName ||
							column == clName ||
							column == clTrusteesProposalDate ||
							column == clTrusteesApprovalDate ||
							column == clAssemblyApprovalDate ||
							column == clInsertUserName ||
							column == clCustomerUserName)
						{
							if (sortColumn != column)
							{
								sortOrder = Constant.SORT_UP;
								sortColumn = column;
							}
							else
							{
								if (sortOrder == Constant.SORT_UP)
								{
									sortOrder = Constant.SORT_DOWN;
								}
								else
								{
									sortOrder = Constant.SORT_UP;
								}
							}

							sortColumn = column;
							ctrPageLongtermRepair.CurrentPageIndex = 0;
							LoadKumiaiLongRepairPlan();
						}
					}

					isSort = false;
				}
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

        private void gcLongRepairList_MouseDown_1(object sender, MouseEventArgs e)
        {
			isSort = true;
		}

        private void gcLongRepairList_MouseMove_1(object sender, MouseEventArgs e)
        {
			isSort = false;
		}

        private void gcLongRepairList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
			try
			{
				if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

				// 左ボタンなら
				if (e.Button == MouseButtons.Left)
				{
					// 行データ取得
					KumiaiLongRepairPlan obj = gcLongRepairList.Rows[e.RowIndex].DataBoundItem as KumiaiLongRepairPlan;
					if (obj == null)
						return;

					var needMaxTerm = obj.AccountPeriod + COMSKCommon.MAX_VISIBLE_YEAR - 1;

					DataGridViewColumn column = gcLongRepairList.Columns[e.ColumnIndex];

					if (column == clRepairPlan && authority_dict[AUTH_REPAIR_PLAN])
					{
						// 修繕計画画面の表示
						if (obj.Result == false)
						{
							K300040010 frmRepairPlan = new K300040010(obj.Pid);
							frmRepairPlan.Show();
						}
					}
					else if (column == clRepairHistory && authority_dict[AUTH_REPAIR_HISTORY])
					{
						// 修繕履歴画面の表示
						K300050020 frmRepairHistory = new K300050020(obj.KumiaiInfoPid);
						frmRepairHistory.Show();
					}
					else if (column == clLongRepairPlan && authority_dict[AUTH_LONG_REPAIR_PLAN])
					{
						// 必要なデータが揃っているかチェック
						if (!COMSKCommon.CheckReadyLongRepairPlanData(obj.KumiaiInfoPid, needMaxTerm))
						{
							return;
						}

						// 長期修繕計画を表示
						K300030020 frm = new K300030020(obj.Pid, obj.Result);
						frm.Show();
					}
					else if (column == clCheckHistory && authority_dict[AUTH_CHECK_HISTORY])
					{
						// 点検・調査履歴を表示
						K300060020 frmCheckHistory = new K300060020(obj.KumiaiInfoPid);
						frmCheckHistory.Show();
					}
					else if (column == clRepairReservePlan && authority_dict[AUTH_REPAIR_RESERVE_PLAN])
					{
						// 必要なデータが揃っているかチェック
						if (!COMSKCommon.CheckReadyRepairReservePlanData(obj.KumiaiInfoPid, needMaxTerm))
						{
							return;
						}

						// 修繕積立金設定画面を出す
						K300070011 frmSettings = new K300070011()
						{
							KumiaiLongRepairPlanPid = obj.Pid,
						};

						if (frmSettings.ShowDialog() == DialogResult.OK)
						{
							// 修繕積み立て金計画を表示
							K300070010 frm = new K300070010()
							{
								LongRepairPlanPid = obj.Pid,
								AutoCalc = frmSettings.AutoCalc,
								AutoCalcDiff = frmSettings.AutoCalcDiff,
							};
							frm.Show();
						}
					}
					else if (column == clCopy && authority_dict[AUTH_COPY])
					{
						if (obj.Result == false)
						{
							// コピー
							if (MessageBox.Show(obj.Name + " をコピーして、新たに長期修繕計画を作成しますか？",
								Constant.CONFIRM_TITLE,
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Question) == DialogResult.Yes)
							{
								// 組合情報を便宜上作成
								COMMONService.KumiaiInfo kumiaiInfo = new COMMONService.KumiaiInfo()
								{
									Pid = obj.KumiaiInfoPid,
									KumiaiName = obj.KumiaiName,
								};

								// 設定画面を開く
								K300030030 frmSettings = new K300030030(kumiaiInfo)
								{
									CopySourceCreateAccountPeriod = obj.CreateAccountPeriod,
									CopySourceAccountPeriod = obj.AccountPeriod,
									CopySourceTypeCode = obj.TypeCode,
								};

								if (frmSettings.ShowDialog() == DialogResult.OK)
								{
									KumiaiLongRepairPlan data = frmSettings.LongRepairPlan;

									data.Pid = obj.Pid;
									data.StandardRepairPlanPid = long.MinValue;
									data.KumiaiInfoPid = kumiaiInfo.Pid;
									data.FrontUserMstPid = Helper.loginUserInfo.Pid;

									try
									{
										K300030BL business = new K300030BL();
										long newPid = business.CopyKumiaiLongRepairPlan(data);

										KumiaiLongRepairPlanCheckedFlg Flginfo = new KumiaiLongRepairPlanCheckedFlg();
										Flginfo.KumiaiLongRepairPlanPid = newPid;
										Flginfo.KumiaiInfoPid = data.KumiaiInfoPid;

										this.UpdateKumiaiLongRepairPlanCheckedFlg(Flginfo);

										if (newPid != long.MinValue)
										{
											K300040010 frm = new K300040010(newPid);
											frm.Show();

											LoadKumiaiLongRepairPlan();
										}
										else
										{
											throw new Exception("長期修繕計画を正常に作成できませんでした。");
										}
									}
									catch (Exception ex)
									{
										Helper.WriteLog(ex);
										MessageBox.Show(ex.Message,
											Constant.ERROR_TITLE,
											MessageBoxButtons.OK,
											MessageBoxIcon.Exclamation);
									}
								}
							}
						}
					}
					else if (column == clMemo)
					{
						// メモを開く
						K300100030 frm = new K300100030(obj.KumiaiInfoPid, obj);
						frm.ShowDialog();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "例外");
			}
		}

        private void gcLongRepairList_CustomColumnDisplayText(object sender, COMMON.ui.CustomColumnDisplayTextEventArgs e)
        {
			try
			{
				KumiaiLongRepairPlan obj = gcLongRepairList.GetRow(e.RowIndex) as KumiaiLongRepairPlan;

				//  日付は、NULL (Ticks = 0) なら表示しない
				if (e.Column == clTrusteesProposalDate)
				{
					//  理事会提案日
					if (obj.TrusteesProposalDate.Ticks == 0)
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clTrusteesApprovalDate)
				{
					//  理事会承認日
					if (obj.TrusteesApprovalDate.Ticks == 0)
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clAssemblyApprovalDate)
				{
					//  総会承認日
					if (obj.AssemblyApprovalDate.Ticks == 0)
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clInsertDateTime)
				{
					//  作成日
					if (obj.InsertDateTime.Ticks == 0)
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clNextNotifyDate)
				{
					//  次期作成開始通知日
					if ((obj.NextNotifyDate.Ticks == 0) ||
						(obj.NextNotifyDate == DateTime.MinValue))
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clSelect)
				{
					e.DisplayText = string.Empty;
				}
				//20140826 Linh ADD - MJC_DEV-213
				else if (e.Column == clIntensiveFlg)
				{
					//集約状態
					string temp = string.IsNullOrEmpty(obj.IntensiveFlg) ? COMSK_INTENSIVEFLG_0030 : obj.IntensiveFlg;
					e.DisplayText = intensiveFlg.Where(a => a.Number == temp).Select(a => a.Title).FirstOrDefault();
				}//END - MJC_DEV-213
			}
			catch (Exception Ex)
			{
				var error = true;
			}
		}

        private void gcLongRepairList_ButtonIconNeeded(object sender, COMMON.ui.ButtonIconNeededEventArgs e)
        {
			try
			{
				KumiaiLongRepairPlan obj = gcLongRepairList.GetRow(e.RowIndex) as KumiaiLongRepairPlan;
				if (e.Column == clRepairPlan || e.Column == clCopy)
				{
					e.Enabled = obj.Result == false;
				}
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
    }
}
