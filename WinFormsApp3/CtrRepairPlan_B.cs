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
	/// 組合毎修繕計画グリッドコントロール
	/// </summary>
	public partial class CtrRepairPlan_B : UserControl
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
				return gcRepairList_Building.DataSource as List<KumiaiRepairPlanDetail>;
			}
			set
			{
				gcRepairList_Building.DataSource = value;
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
		public CtrRepairPlan_B()
		{
			InitializeComponent();

			try
			{
				if (!DesignMode)
				{
					var bl = new coms.COMMON.business.CommonBL();
					coms.COMMONService.CodeMst[] dataSource = bl.SearchCodeMstByCode("n001");

					clUnit_B.DisplayMember = "Title";
					clUnit_B.ValueMember = "Number";
					clUnit_B.DataSource = dataSource;

					clUnit_B.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
					clUnit_B.FlatStyle = FlatStyle.Flat;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			gcRepairList_Building.IgnoreAutoFormatColumns = new HashSet<string>(new[] { clPrice_B.Name });
		}

		#endregion

		#region Public

		/// <summary>
		/// データを再描画する
		/// </summary>
		public void RefreshData()
		{
			gcRepairList_Building.Refresh();
		}

		/// <summary>
		/// 明細を追加する
		/// </summary>
		/// <param name="detail">The detail.</param>
		public void Add(KumiaiRepairPlanDetail detail)
		{
			if (detail == null) return;
			if (DataSource == null) DataSource = new List<KumiaiRepairPlanDetail>();

			var grid = gcRepairList_Building;

			// 1) current selected row object (like FocusedRowHandle)
			KumiaiRepairPlanDetail current = null;
			int currRowIndex = -1;

			try
			{
				currRowIndex = grid?.FocusedRowHandle ?? -1;
				if (currRowIndex >= 0)
					current = grid.GetRowObject<KumiaiRepairPlanDetail>(currRowIndex);
			}
			catch
			{
				currRowIndex = -1;
				current = null;
			}

			// 2) compute ViewSequenceDisplay
			if (current == null)
				detail.ViewSequenceDisplay = 1;
			else
				detail.ViewSequenceDisplay = current.ViewSequenceDisplay + 0.1;

			// 3) insert into list after the current item
			if (current == null)
			{
				DataSource.Add(detail);
			}
			else
			{
				int indexInList = DataSource.IndexOf(current);
				if (indexInList < 0)
				{
					// fallback: append
					DataSource.Add(detail);
				}
				else
				{
					DataSource.Insert(indexInList + 1, detail);
				}
			}

			// 4) refresh the grid
			// safest way: rebind the list so the grid rebuilds rows
			try
			{
				// Re-assigning DataSource forces UI refresh reliably in WinForms binding.
				this.DataSource = DataSource.ToList();
				grid.Refresh();
			}
			catch
			{
				// last resort
				grid?.Invalidate();
			}
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

		private void gcRepairList_Building_ButtonIconNeeded(object sender, COMMON.ui.ButtonIconNeededEventArgs e)
		{
			try
			{
				if (e.Column == clUpdateHistory_B)
				{
					KumiaiRepairPlanDetail obj = gcRepairList_Building.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;
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

		private void gcRepairList_Building_RowBackColorNeeded(object sender, COMMON.ui.RowBackColorNeededEventArgs e)
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
		private void gvRepairList_Building_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
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

				gcRepairList_Building.Refresh();
			}
		}

		#endregion


		private void gcRepairList_Building_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				var col = gcRepairList_Building.Columns[e.ColumnIndex];

				//  行データを取得
				KumiaiRepairPlanDetail obj = gcRepairList_Building.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;

				// 該当列で無い場合は何もしない。
				if (col.Equals(clRepairPeriod_B) || col.Equals(clPrice_B) || col.Equals(clAmount_B) || col.Equals(clUnit_B) || col.Equals(clRemarks_B))
				{
					obj.ChengeValueFlg = true; //修繕基準の変更フラグを立てる
					obj.UpdateUserMstPid = Helper.loginUserInfo.Pid;
					//obj.UpdateDateTime = DateTime.Now; ローカルの時計がおかしい可能性を考慮しサーバー側で設定
				}
			}
			catch (Exception)
			{
			}
		}

		private void gcRepairList_Building_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				if (e.Button == MouseButtons.Left)
				{
					if (e.RowIndex < 0) return;

					var grid = gcRepairList_Building;

					// 行データ取得
					KumiaiRepairPlanDetail obj =
						grid.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;

					if (obj == null) return;

					DataGridViewColumn col = grid.Columns[e.ColumnIndex];

					// 変更履歴
					if (col == clUpdateHistory_B)
					{
						K300040013 frmUpdateHistory = new K300040013(obj);
						frmUpdateHistory.ShowDialog();
					}

					// メモ
					else if (col == clRemarks_B)
					{
						K300040014 frm = new K300040014(obj);
						frm.ShowDialog();

						obj.Memo = frm.kumiaiRepairPlanDetail.Memo;

						grid.Refresh();
					}

					// 詳細
					else if (col == clDetail_B)
					{
						K300040011 frm = new K300040011(obj);

						if (frm.ShowDialog() == DialogResult.OK)
						{
							if (frm.AddNew)
							{
								if (e.RowIndex == -1)
								{
									DataSource.Add(frm.WorkRepairPlanDetail);
								}
								else
								{
									int index = DataSource.IndexOf(obj);
									DataSource.Insert(index + 1, frm.WorkRepairPlanDetail);
								}
							}

							if (frm.bolChildUpdte)
							{
								this.RefleshChildRepairPlanDetail(obj);
							}
						}

						grid.Refresh();
					}

					// 数量
					else if (col == clAmount_B)
					{
						if (obj.Amount == double.MinValue)
						{
							obj.Amount = 0;
							grid.Refresh();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void gcRepairList_Building_CustomColumnDisplayText(object sender, COMMON.ui.CustomColumnDisplayTextEventArgs e)
		{
			try
			{
				//  行データを取得
				KumiaiRepairPlanDetail obj = gcRepairList_Building.GetRow(e.RowIndex) as KumiaiRepairPlanDetail;

				//  特定のカラムなら分岐
				if (e.Column == clTempBox_B)
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
				else if (e.Column == clChild_B)
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
				else if (e.Column == clPrice_B)
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
				else if (e.Column == clAmount_B)
				{
					if (obj.Amount == double.MinValue)
					{
						e.DisplayText = string.Empty;
					}
				}
				else if (e.Column == clRepairPeriod_B)
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
	}
}
