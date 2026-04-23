using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using coms.COMMON;
using coms.COMSKService;
using coms.COMSK.business;

namespace coms.COMSK.ui.common
{
	/// <summary>
	/// 組合毎修繕計画グリッドコントロール (設備)
	/// </summary>
	public partial class CtrRepairPlan_E : UserControl
	{
		#region イベント

		/// <summary>
		/// 新規作成するタイミングで呼び出される
		/// </summary>
		public event EventHandler Adding;

		#endregion

		#region プロパティ

		/// <summary>
		/// データソース
		/// </summary>
		[Browsable(false)]
		public List<KumiaiRepairPlanDetail> DataSource
		{
			get
			{
				return gridcRepairList_Equipment.DataSource as List<KumiaiRepairPlanDetail>;
			}
			set
			{
				gridcRepairList_Equipment.DataSource = value;
			}
		}

		/// <summary>
		/// 工事分類
		/// </summary>
		[Browsable(false)]
		public COMSK.common.COMSKCommon.ConstructionType ConstrType { get; set; }

		/// <summary>
		/// 表示フィルタ
		/// </summary>
		[Browsable(false)]
		public bool[] Filter { get; set; }

		/// <summary>
		/// 最新の並び順にソートした状態でリストを返す
		/// </summary>
		[Browsable(false)]
		public List<KumiaiRepairPlanDetail> SortedDataSource
		{
			get
			{
				List<KumiaiRepairPlanDetail> list = (from item in DataSource
													 orderby item.ViewSequenceDisplay ascending
													 select item).ToList();
				for (int i = 0; i < list.Count; i++)
				{
					list[i].ViewSequence = i + 1;
				}

				return list;
			}
		}

		/// <summary>
		/// 新規登録できるかどうか
		/// </summary>
		/// <value>
		///   <c>true</c> if [allow add]; otherwise, <c>false</c>.
		/// </value>
		[Browsable(false)]
		public bool AllowAdd
		{
			set
			{
				btnAdd.Enabled = value;
			}
		}

		#endregion

		#region コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CtrRepairPlan_E()
		{
			InitializeComponent();

			try
			{
				if (!DesignMode)
				{
					var bl = new coms.COMMON.business.CommonBL();
					coms.COMMONService.CodeMst[] dataSource = bl.SearchCodeMstByCode("n001");

					clUnit_E.DisplayMember = "Title";
					clUnit_E.ValueMember = "Number";
					clUnit_E.DataSource = dataSource;

					clUnit_E.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
					clUnit_E.FlatStyle = FlatStyle.Flat;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		#endregion

		#region Public

		/// <summary>
		/// データを再描画する
		/// </summary>
		public void RefreshData()
		{
			gridcRepairList_Equipment.Refresh();
		}

		/// <summary>
		/// 明細を追加する
		/// </summary>
		/// <param name="detail">The detail.</param>
		public void Add(KumiaiRepairPlanDetail detail)
		{
			// TODO Add by bindingsource
			//  現在行を取得
			/*int currRow = gridvRepairList_Equipment.FocusedRowHandle;
			KumiaiRepairPlanDetail currRepairPlanDetail = gridvRepairList_Equipment.GetRow(currRow) as KumiaiRepairPlanDetail;

			//  ViewSequenceDisplay を設定
			if (currRepairPlanDetail == null)
			{
				detail.ViewSequenceDisplay = 1;
			}
			else
			{
				detail.ViewSequenceDisplay = currRepairPlanDetail.ViewSequenceDisplay + 0.1;
			}

			//  挿入
			if (currRow < 0)
			{
				DataSource.Add(detail);
			}
			else
			{
				DataSource.Insert(currRow + 1, detail);
			}

			//  リフレッシュ
			gridcRepairList_Equipment.RefreshDataSource();*/
		}

		/// <summary>
		/// 表示順序を更新する
		/// </summary>
		public void ApplyViewSequence()
		{
			List<KumiaiRepairPlanDetail> list = (from item in DataSource
												 orderby item.ViewSequenceDisplay ascending
												 select item).ToList();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].ViewSequence = i + 1;
				list[i].ViewSequenceDisplay = list[i].ViewSequence;
			}
			DataSource = list;
		}

		#endregion

		#region イベント

        // ADD 2012/08/07 S.Igarashi No.7 ↓
        private void RefleshChildRepairPlanDetail(KumiaiRepairPlanDetail detail)
        {
            for (int i = 0; i <= this.DataSource.Count - 1; i++)
            {
                if (this.DataSource[i].ParentPid == detail.Pid)
                {
                    //数量
                    this.DataSource[i].Amount = detail.Amount;
                    //更新フラグ
                    this.DataSource[i].UpdateFlg = COMSK.common.COMSKCommon.HAS_HISTORY_FLG_ON;
                }
            }
        }
        // ADD 2012/08/07 S.Igarashi No.7 ↑

		private void gridcRepairList_Equipment_ButtonIconNeeded(object sender, COMMON.ui.ButtonIconNeededEventArgs e)
		{
			try
			{
				if (e.Column == clUpdateHistory_E)
				{
					KumiaiRepairPlanDetail obj = gridcRepairList_Equipment.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;
					if (obj.UpdateFlg == COMSK.common.COMSKCommon.HAS_HISTORY_FLG_ON)
					{
						e.BackColor = Color.Red;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void gridcRepairList_Equipment_RowBackColorNeeded(object sender, COMMON.ui.RowBackColorNeededEventArgs e)
		{
			try
			{
				//  行データを取得
				KumiaiRepairPlanDetail obj = e.DataBoundItem as KumiaiRepairPlanDetail;

				//  未選択なら灰色に
				if (obj.Select == false)
				{
					e.BackColor = Color.Silver;
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// 行フィルタイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DevExpress.XtraGrid.Views.Base.RowFilterEventArgs"/> instance containing the event data.</param>
		private void gridvRepairList_Equipment_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
		{
			try
			{
				KumiaiRepairPlanDetail obj = DataSource[e.ListSourceRow] as KumiaiRepairPlanDetail;

				//  フィルタリング
				bool passed = false;
				foreach (bool b in Filter)
				{
					if (obj.Select == b)
					{
						passed = true;
					}
				}

				if (passed)
				{
					e.Visible = true;
				}
				else
				{
					e.Visible = false;
				}

				e.Handled = true;
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// 新規登録ボタンクリック
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (Adding != null)
			{
				Adding(this, EventArgs.Empty);

			}

		}

		/// <summary>
		/// 全て選択解除ボタン
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnDeselectAll_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(COMSK.common.COMSKCommon.CONFIRM_DESELECT_ALL, Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				foreach (KumiaiRepairPlanDetail detail in DataSource)
				{
					detail.Select = false;
				}

				gridcRepairList_Equipment.Refresh();
			}
		}

		#endregion


        private void gridcRepairList_Equipment_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
			try
			{
				DataGridViewColumn col = gridcRepairList_Equipment.Columns[e.ColumnIndex];

				//  左クリックなら
				if (e.Button == MouseButtons.Left)
				{
					//  行データを取得
					KumiaiRepairPlanDetail obj = gridcRepairList_Equipment.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;

					// 該当列で無い場合は何もしない。
					if (col.Equals(clUpdateHistory_E) == true)
					{
						// 変更履歴を表示
						K300040013 frmUpdateHistory = new K300040013(obj);
						frmUpdateHistory.ShowDialog();
					}
					else if (col == clRemarks_E)
					{
						K300040014 frm = new K300040014(obj);
						frm.ShowDialog();
						obj.Memo = frm.kumiaiRepairPlanDetail.Memo;
						gridcRepairList_Equipment.Refresh();
					}
					else if (col.Equals(clDetail_E) == true)
					{
						// 詳細画面表示
						K300040012 frm = new K300040012(obj)
						{
						};
						if (frm.ShowDialog() == DialogResult.OK)
						{
							//  新規挿入なら
							if (frm.AddNew)
							{
								//  現在行の下に挿入
								if (e.RowIndex == -1)
								{
									DataSource.Add(frm.WorkRepairPlanDetail);
								}
								else
								{
									//  e.RowHandle は表示されているリスト内でのインデックスとなるので、
									//  detail が DataSource の何番目にあるかを調べ、
									//  その一つ下に挿入する
									int index = DataSource.IndexOf(obj);
									DataSource.Insert(index + 1, frm.WorkRepairPlanDetail);
								}
							}
							// ADD 2012/08/07 S.Igarashi No.7 ↓
							if (frm.bolChildUpdte)
							{
								//  子データ更新
								this.RefleshChildRepairPlanDetail(obj);
							}
							// ADD 2012/08/07 S.Igarashi No.7 ↑
						}

						//  データソースを更新
						gridcRepairList_Equipment.Refresh();
					}
					else if (col.Equals(clAmount_E))
					{
						if (obj.Amount == double.MinValue)
						{
							obj.Amount = 0;
							gridcRepairList_Equipment.Refresh();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

        private void gridcRepairList_Equipment_CustomColumnDisplayText(object sender, COMMON.ui.CustomColumnDisplayTextEventArgs e)
        {
			try
			{
				//  行データを取得
				KumiaiRepairPlanDetail obj = gridcRepairList_Equipment.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;

				//  特定のカラムなら分岐
				if (e.Column == clTempBox_E)
				{
					//  IsInTempBox が true なら黒●表示
					if (obj.ReserveBoxPid != long.MinValue)
					{
						e.DisplayText = "●";
					}
					else
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clChild)
				{
					//  子項目なら黒●表示
					if (obj.ParentPid != long.MinValue)
					{
						e.DisplayText = "●";

					}
					else
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clPrice_E)
				{
					if (obj.Cost == long.MinValue)
					{
						e.DisplayText = string.Empty;
					}
					else
					{
						e.DisplayText = (obj.CostUnitName != string.Empty) ? string.Format("{0:N0}/{1}", obj.Cost, obj.CostUnitName) : string.Format("{0:N0}{1}", obj.Cost, obj.CostUnitName);
					}
				}
				else if (e.Column == clAmount_E)
				{
					if (obj.Amount == double.MinValue)
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clRepairPeriod_E)
				{
					if (obj.Cycle == int.MinValue)
					{
						e.DisplayText = string.Empty;
					}
				}
			}
			catch (Exception)
			{
			}
		}

        private void gridcRepairList_Equipment_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
			try
			{
				var col = gridcRepairList_Equipment.Columns[e.ColumnIndex];

				//  行データを取得
				KumiaiRepairPlanDetail obj = gridcRepairList_Equipment.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;

				// 該当列で無い場合は何もしない。
				if (col.Equals(clRepairPeriod_E) || col.Equals(clPrice_E) || col.Equals(clAmount_E) || col.Equals(clUnit_E) || col.Equals(clRemarks_E))
				{
					obj.ChengeValueFlg = true; //修繕基準の変更フラグを立てる
					obj.UpdateUserMstPid = Helper.loginUserInfo.Pid;
				}
			}
			catch (Exception)
			{
			}
		}
    }
}
