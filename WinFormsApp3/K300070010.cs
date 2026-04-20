using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using coms.COMMON;
using coms.COMSK.business;
using coms.COMSK.common;
using coms.COMSKService;

/// <history>
/// ducthang 20120912 国交省対応
/// </history>
namespace coms.COMSK.ui
{
	/// <summary>
	/// 修繕積立金計画画面
	/// </summary>
    public partial class K300070010 : MyForm
	{

		#region 定数

		/// <summary>
		/// 案の最大数
		/// </summary>
		public static readonly int MAX_REPAIR_RESERVE_PLAN = 5;

		/// <summary>
		/// デフォルトのマーカーサイズ
		/// </summary>
		private static readonly int MARKAR_SIZE_DEFAULT = 10;

		/// <summary>
		/// デフォルトのマーカーサイズ
		/// </summary>
		private static readonly int MARKAR_SIZE_EMPHASIS = 18;

        /// <summary>
        /// 表示する最大年数
        /// </summary>
        //public const int MAX_VISIBLE_YEAR = 60;

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

        /// <summary>
        /// 金額更新フラグ
        /// </summary>
        private bool isUpdated = false;
		#endregion

		#region プロパティ

		/// <summary>
		/// 対象の長計 PID。
		/// 試算表示からの場合は MinValue にする。
		/// 長計の PID を使いたい場合は LongRepairPlan.Pid を使う。
		/// </summary>
		[Browsable(false)]
		public long LongRepairPlanPid { get; set; }

		/// <summary>
		/// 対象の長計データ
		/// </summary>
		[Browsable(false)]
		public KumiaiLongRepairPlan LongRepairPlan { get; set; }

		/// <summary>
		/// 試算表示に使う累計工事費
		/// </summary>
		[Browsable(false)]
		public long[] TempTotalRepairCost { get; set; }

		/// <summary>
		/// 自動計算フラグ
		/// </summary>
		[Browsable(false)]
		public bool AutoCalc { get; set; }
        /// <summary>
        /// 
        /// 工事費差額のみ自動計算フラグ
        /// </summary>
        [Browsable(false)]
        public bool AutoCalcDiff { get; set; }

        /// <summary>
        /// タイプ別積立金とタイプマスタの差異があるかどうかのフラグ。タイプ別積立金対応。
        /// </summary>
        [Browsable(false)]
        public bool TypeDetailDifferenceFlg { get; set; }

        /// <summary>
        /// 画面読込完了フラグ。タイプ別積立金対応。
        /// </summary>
        [Browsable(false)]
        public bool isLoadEnd { get; set; }
		#endregion

		#region メンバ

		/// <summary>
		/// 築年等データ
		/// </summary>
		private List<Data30Period> totalRepairCost = new List<Data30Period>();

		#region ドラッグドロップ用
		private DataPoint selectedDataPoint = null;
		private DataPoint prevSelectedDataPoint = null;
		private int pointIndex = int.MinValue;
		private int prevPointIndex = int.MinValue;
		private double startValue = int.MinValue;
		//private Data30Period startValues = new Data30Period();
		private bool multiSelectDrag = false;
		private Series selectedSeries = null;
		private Point startCursorPoint = new Point();
		#endregion

		/// <summary>
		/// 現在の表示モード
		/// </summary>
		private COMSKCommon.RepairReserveViewMode currViewMode = COMSKCommon.RepairReserveViewMode.Full;

		/// <summary>
		/// 標準モード用フォント
		/// </summary>
		private Font fntStandard = new Font("Tahoma", 6);
		/// <summary>
		/// 拡大時用フォント
		/// </summary>
		private Font fntFull = new Font("Tahoma", 9);

		/// <summary>
		/// 拡大時のビューの幅
		/// </summary>
		private const int FULL_PANEL_WIDTH = 2512;

		/// <summary>
		/// 案データ情報
		/// </summary>
		private KumiaiRepairReservePlanDraftInfo draftInfo = null;

		/// <summary>
		/// 各案の配列
		/// </summary>
		private coms.COMSK.ui.common.CtrRepairReservePlan[] repairReservePlans = null;

		/// <summary>
		/// 既に表示されたかどうか
		/// 既に表示された場合スクロールが自動出来るが
		/// 初期表示だとスクロール設定必須
		/// </summary>
		private List<common.CtrRepairReservePlan> lstViewedPlan = new List<common.CtrRepairReservePlan>();
		/// <summary>
		/// 画面全体（グラフ・工事費・各プラン）現時点のスクロール位置
		/// </summary>
		private int currScrollPos = 0;

		/// <summary>
		/// 表示単位
		/// </summary>
		private int viewUnitValue = 1000;

		/// <summary>
		/// 工事費累計の配列
		/// </summary>
		private long[] totalRepairCostArray = null;

        /// <summary>
        /// 組合タイプマスタ情報
        /// </summary>
        private TypeMst[] lstTypes = null;

        //private List<KumiaiRepairReservePlanDraft> totalRepairCostArray = null;

        /// <summary>
        /// https://reci.backlog.jp/view/MJC_DEV-524
        /// ユーザーが行った最後の編集を記録
        /// </summary>
        private Object preSender = null;
        private EventArgs preEvent = null;

        /// <summary>
        /// ユーザーが行った編集を反映しているときにtrue
        /// </summary>
        private bool preMode = false;

        /// <summary>
        /// グラフドラッグイベントでユーザーが行った編集値
        /// </summary>
        private double preVal;
		RepairReservePlanCalculator calc = new RepairReservePlanCalculator();
		#endregion

		#region コンストラクタ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public K300070010()
        {
            InitializeComponent();

			this.LongRepairPlanPid = long.MinValue;
			this.LongRepairPlan = null;
			this.TempTotalRepairCost = null;

			//  タブの配列を作っておく
			repairReservePlans = new coms.COMSK.ui.common.CtrRepairReservePlan[5] {
				ctrRepairReservePlan1,
				ctrRepairReservePlan2,
				ctrRepairReservePlan3,
				ctrRepairReservePlan4,
				ctrRepairReservePlan5,
			};

			//this.AddMorePoint();
		}

		private void AddMorePoint()
        {
			foreach(var srObj in chartGraph.Series)
            {
				for(int i=0; i < 30; i++)
                {
					System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 0D);
					srObj.Points.Add(dataPoint);
				}
			}

			this.chartGraph.ChartAreas[0].AxisX.Maximum = 61D;
		}
		#endregion

		#region イベント

		#region フォームイベント

		/// <summary>
		/// フォームロードイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void K300070010_Load(object sender, EventArgs e)
		{
			//  権限設定
			SetAuthorities();

			//  データ読み込み
            isLoadEnd = false;
			LoadData();
            isLoadEnd = true;
		}

		#endregion

		#region データグリッドイベント
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
				var data = e.RowData as Data30Period;
				if (data == null) return;

				if (0 <= columnIndex && columnIndex < COMSKCommon.MAX_VISIBLE_YEAR)
				{
					if (currViewMode == COMSKCommon.RepairReserveViewMode.Full)
					{
						// Zoom (Full) mode
						if (e.RowIndex == 0)
						{
							// 築年
							e.DisplayText = string.Format("{0}年目", data.GetValue(columnIndex));
						}
						else if (e.RowIndex == 1)
						{
							// 会計期
							e.DisplayText = string.Format("{0}期", data.GetValue(columnIndex));
						}
						else if (e.RowIndex == 2)
						{
							// 会計年度
							e.DisplayText = string.Format("{0}年", data.GetValue(columnIndex));
						}
						else if (e.RowIndex == 3)
						{
							// 工事費累計
							e.DisplayText = string.Format(
								"{0:#,0}",
								RoundByViewUnit(data.GetValue(columnIndex), viewUnitValue));
						}
					}
					else if (currViewMode == COMSKCommon.RepairReserveViewMode.Standard)
					{
						// Standard mode
						if (e.RowIndex == 3)
						{
							// 工事費累計
							long val = RoundByViewUnit(data.GetValue(columnIndex), viewUnitValue);
							e.DisplayText = val.ToString();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void gcData30Period_CellReadOnlyNeeded(object sender, coms.COMMON.ui.ReserveCellReadOnlyNeededEventArgs e)
		{
			var colIdx = e.ColumnIndex;
			e.ReadOnly = true;
		}

		private void gcData30Period_CellStyleNeeded(object sender, coms.COMMON.ui.ReserveCellStyleNeededEventArgs e)
		{
			var colIdx = e.ColumnIndex;
			if (e.RowIndex < 3)
			{
				e.BackColor = Color.Yellow;
			}
		}
		#endregion

		#region チャートイベント

		/// <summary>
		/// マウスダウンイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void chartGraph_MouseDown(object sender, MouseEventArgs e)
		{
			//  ヒットテスト
			HitTestResult hitResult = chartGraph.HitTest(e.X, e.Y);

			//  マウス位置がデータポイントで、棒グラフでなければ
			selectedDataPoint = null;
			if (hitResult.ChartElementType == ChartElementType.DataPoint && hitResult.Series != chartGraph.Series[0])
			{
				//  対象のデータポイントを保存
				selectedDataPoint = (DataPoint)hitResult.Object;

				//  Ctrl キーが押されていたら
				multiSelectDrag = false;
				if ((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control)
				{
					//  prevPointIndex が有効なら
					if (prevPointIndex != int.MinValue)
					{
						//  今回の点が前の点より大きければ
						if (hitResult.PointIndex > prevPointIndex)
						{
							multiSelectDrag = true;

							//  前回データポイントをセット
							prevSelectedDataPoint = hitResult.Series.Points[prevPointIndex];
						}
					}
				}

				//  位置と種別を保存
				pointIndex = hitResult.PointIndex;
				selectedSeries = hitResult.Series;
				startCursorPoint = new Point(e.X, e.Y);

				//  ラベルを表示する
				selectedDataPoint.IsValueShownAsLabel = true;

				//  カーソルを変更
				chartGraph.Cursor = Cursors.SizeNS;

				//  初期値を決定
				startValue = selectedDataPoint.YValues[0];

				//  マルチセレクトでなければ
				if (multiSelectDrag == false)
				{
					//  今回の位置を覚えておく
					prevPointIndex = pointIndex;
				}
				else
				{
					//for (int i = 0; i < MAX_REPAIR_RESERVE_PLAN; i++)
					//{
					//    startValues.SetValue(i, selectedSeries.Points[i].YValues[0]);
					//}

					//  ポイントを強調
					selectedDataPoint.MarkerSize = MARKAR_SIZE_EMPHASIS;
					prevSelectedDataPoint.MarkerSize = MARKAR_SIZE_EMPHASIS;
				}

			}
		}

		/// <summary>
		/// マウスアップイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void chartGraph_MouseUp(object sender, MouseEventArgs e)
		{
            this.preSender = sender;
            this.preEvent = e as EventArgs;
			//  ドラッグ中なら
			if (selectedDataPoint != null)
			{
				//  マウス位置がコントロールサイズの外に出ないように調整
				int coordinate = e.Y;
				if (coordinate < 0)
				{
					coordinate = 0;
				}
				else if (coordinate >= chartGraph.Size.Height)
				{
					coordinate = chartGraph.Size.Height - 1;
				}

				//  カーソル位置から新しい Y 位置を計算する
				double yValue = chartGraph.ChartAreas["Default"].AxisY.PixelPositionToValue(coordinate);
				yValue = Math.Min(yValue, chartGraph.ChartAreas["Default"].AxisY.Maximum);
				yValue = Math.Max(yValue, chartGraph.ChartAreas["Default"].AxisY.Minimum);
				if (yValue <= 0)
				{
					yValue = 0;
				}
                if (preMode) yValue = preVal;
                else preVal = yValue;
                
				//  開始位置と異なれば
				if (startCursorPoint.X != e.X || startCursorPoint.Y != e.Y)
				{
					//  更新する案を確定
					int chartIndex = 0;
					common.CtrRepairReservePlan targetPlan = ctrRepairReservePlan1;
					if (selectedSeries == chartGraph.Series[1])
					{
						targetPlan = ctrRepairReservePlan1;
						chartIndex = 1;
					}
					else if (selectedSeries == chartGraph.Series[2])
					{
						targetPlan = ctrRepairReservePlan2;
						chartIndex = 2;
					}
					else if (selectedSeries == chartGraph.Series[3])
					{
						targetPlan = ctrRepairReservePlan3;
						chartIndex = 3;
					}
					else if (selectedSeries == chartGraph.Series[4])
					{
						targetPlan = ctrRepairReservePlan4;
						chartIndex = 4;
					}
					else if (selectedSeries == chartGraph.Series[5])
					{
						targetPlan = ctrRepairReservePlan5;
						chartIndex = 5;
					}

					//  表示単位を考慮して数字を調整
					long val = (long)(yValue * viewUnitValue + 0.5);

					//  マルチセレクトなら
					if (multiSelectDrag)
					{
						//  住戸月額積立金単価を更新
						targetPlan.UpdateValue(prevPointIndex, pointIndex, val);
					}
					else
					{
						////  Y 位置を更新
						//selectedDataPoint.YValues[0] = yValue;

						//  一時金単価を更新
						targetPlan.UpdateValueTemp(pointIndex, val);
					}

					//  チャートデータを更新
					SetChartData(chartIndex, targetPlan.TotalReserveData);

				}

				//  ラベルを隠す
                if (selectedDataPoint != null)
                {
                    selectedDataPoint.IsValueShownAsLabel = false;
                    selectedDataPoint.MarkerSize = MARKAR_SIZE_DEFAULT;
                }

				//  ポイントサイズを元に戻す
				
				if (prevSelectedDataPoint != null)
				{
					prevSelectedDataPoint.MarkerSize = MARKAR_SIZE_DEFAULT;
				}

				// ドラッグ終了
				selectedDataPoint = null;
				selectedSeries = null;

				//  再描画
				chartGraph.Invalidate();

				//  カーソルを戻す
				chartGraph.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// マウス移動イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void chartGraph_MouseMove(object sender, MouseEventArgs e)
		{
			//  ドラッグ中なら
			if (selectedDataPoint != null)
			{
				//  マウス位置がコントロールサイズの外に出ないように調整
				int coordinate = e.Y;
				if (coordinate < 0)
				{
					coordinate = 0;
				}
				else if (coordinate >= chartGraph.Size.Height)
				{
					coordinate = chartGraph.Size.Height - 1;
				}

				//  カーソル位置から新しい Y 位置を計算する
				double yValue = chartGraph.ChartAreas["Default"].AxisY.PixelPositionToValue(coordinate);
				yValue = Math.Min(yValue, chartGraph.ChartAreas["Default"].AxisY.Maximum);
				yValue = Math.Max(yValue, chartGraph.ChartAreas["Default"].AxisY.Minimum);
				if (yValue <= 0)
				{
					yValue = 0;
				}

				////  マルチセレクトなら
				//if (multiSelectDrag == true)
				//{
				//    //  差分を出す
				//    double deltaValue = yValue - startValue;

				//    //  その他のポイントの値も変更
				//    if (prevPointIndex < pointIndex)
				//    {
				//        for (int i = prevPointIndex; i <= pointIndex; i++)
				//        {
				//            //  加算
				//            selectedSeries.Points[i].YValues[0] = startValues.GetValue(i) + deltaValue;
				//        }
				//    }
				//    else
				//    {
				//        for (int i = pointIndex; i <= prevPointIndex; i++)
				//        {
				//            //  加算
				//            selectedSeries.Points[i].YValues[0] = startValues.GetValue(i) + deltaValue;
				//        }
				//    }
				//}
				//else
				//{
					//  通常更新
					selectedDataPoint.YValues[0] = yValue;

				//}

				//  再描画
				chartGraph.Invalidate();
				chartGraph.Update();
			}
			else
			{
				// Set different shape of cursor over the data points
				HitTestResult hitResult = chartGraph.HitTest(e.X, e.Y);
				if ((hitResult.ChartElementType == ChartElementType.DataPoint) &&
					(hitResult.Series != chartGraph.Series[0]))
				{
					chartGraph.Cursor = Cursors.Hand;
				}
				else
				{
					chartGraph.Cursor = Cursors.Default;
				}
			}
		}

		/// <summary>
		/// サイズ変更イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void chartGraph_SizeChanged(object sender, EventArgs e)
		{
			double columnWidth = calc.CalcReserveGridColWidth(this.chartGraph.Width);

			//  セット
			double sumColumnDoubleWidth = 0;
			int sumColumnIntWidth = 0;
			for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
			{
				DataGridViewColumn gc = gcData30Period.Columns[i];

				//  幅を計算
				sumColumnDoubleWidth += columnWidth;

				//  幅を設定
				int width = (int)(sumColumnDoubleWidth - sumColumnIntWidth);
				gc.Width = width;

				//  整数値幅を足しこむ
				sumColumnIntWidth += width;

			}
			// grid control
			gcData30Period.Width = gcData30Period.Columns
				.Cast<DataGridViewColumn>()
				.Where(col => col.Visible)
				// TODO
				//.Sum(col => col.VisibleWidth) + gvData30Period.IndicatorWidth + calc.CalcReserveWidthDiff(this.currViewMode);
				.Sum(col => col.Width) + calc.CalcReserveWidthDiff(this.currViewMode);

			//  各タブも処理
			ctrRepairReservePlan1.ResizeControl(chartGraph.Width, this.currViewMode);
			ctrRepairReservePlan2.ResizeControl(chartGraph.Width, this.currViewMode);
			ctrRepairReservePlan3.ResizeControl(chartGraph.Width, this.currViewMode);
			ctrRepairReservePlan4.ResizeControl(chartGraph.Width, this.currViewMode);
			ctrRepairReservePlan5.ResizeControl(chartGraph.Width, this.currViewMode);

			// mid lef area
			this.tableLayoutPanel3.ColumnStyles[0].Width = calc.CalcReserveLeftPartWidth(chartGraph.Width, columnWidth);
		}

		#endregion

		#region その他コントロールイベント
		/// <summary>
		/// タブクリックイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void xtraTabControl1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				Point pos = (sender as Control).PointToScreen(new Point(e.X, e.Y));
				mnuMoveTab.Show(pos);
			}
		}

		/// <summary>
		/// 積立金累計を手で編集した際に呼び出されるイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairReserve_TotalReserveChanged(object sender, EventArgs e)
		{
			//  CtrRepairReserve なら
			if (sender is common.CtrRepairReservePlan)
			{
				common.CtrRepairReservePlan ctrPlan = sender as common.CtrRepairReservePlan;
				if (ctrPlan == ctrRepairReservePlan1)
				{
					SetChartData(1, ctrPlan.TotalReserveData);
                    CallPreSender(ctrPlan);

				}
				else if (ctrPlan == ctrRepairReservePlan2)
				{
					SetChartData(2, ctrPlan.TotalReserveData);
                    CallPreSender(ctrPlan);
				}
				else if (ctrPlan == ctrRepairReservePlan3)
				{
					SetChartData(3, ctrPlan.TotalReserveData);
                    CallPreSender(ctrPlan);
				}
				else if (ctrPlan == ctrRepairReservePlan4)
				{
					SetChartData(4, ctrPlan.TotalReserveData);
                    CallPreSender(ctrPlan);
				}
                else if (ctrPlan == ctrRepairReservePlan5)
                {
                    SetChartData(5, ctrPlan.TotalReserveData);
                    CallPreSender(ctrPlan);
                }
                isUpdated = true;
			}
			chartGraph.Invalidate();
		}

		private void ctrRepairReserve_ScrollChanged(object sender, EventArgs e)
		{
			var chartHeight = this.chartGraph.Height;
			var midGridHeight = this.gcData30Period.Height;
			if (sender is common.CtrRepairReservePlan)
			{
				common.CtrRepairReservePlan ctrPlan = sender as common.CtrRepairReservePlan;
				var drtScrollPos = ctrPlan.GetScrollPos();
				this.currScrollPos = drtScrollPos;
				// Tạm thời tắt AutoScroll để gán lại vị trí
				//pnScrollTop.AutoScroll = false;
				pnScrollTop.AutoScrollPosition = new Point(drtScrollPos, -pnScrollTop.AutoScrollPosition.X);
				//pnScrollTop.AutoScroll = true;

				//pnScrollMid.AutoScroll = false;
				pnScrollMid.AutoScrollPosition = new Point(drtScrollPos, -pnScrollMid.AutoScrollPosition.X);
				//pnScrollMid.AutoScroll = true;
				//foreach (Control ctrl in pnScrollTop.Controls)
				//{
				//	ctrl.Dock = DockStyle.None;
				//	ctrl.Left = -drtScrollPos;
				//	//ctrl.Dock = DockStyle.Left;
				//}
				//foreach (Control ctrl in pnScrollMid.Controls)
				//{
				//	ctrl.Dock = DockStyle.None;
				//	ctrl.Left = -drtScrollPos;
				//	//ctrl.Dock = DockStyle.Left;
				//}
				//this.chartGraph.Height = chartHeight;
				//this.gcData30Period.Height = midGridHeight;
				this.ScrollAllSubTab(ctrPlan, drtScrollPos);
			}
		}

		private void CallPreSender(coms.COMSK.ui.common.CtrRepairReservePlan ctr)
        {
            try
            {
                if (isLoadEnd) //読込処理が終わっているならば
                {
                    preMode = true; //各イベントの反映モードを切り替えてユーザーが最後行った編集値を使用する。
                    if (TypeDetailDifferenceFlg && draftInfo.Draft.Any(c => c.TypeDetailReCalcFlg == false))
                    {
                        //1案のタイプ別タイプ別積立金のタイプ情報部分が差し替わるならばタイプ別積立金の全案を再計算する。
                        MessageBox.Show("表・グラフの変更に伴い、修繕積立金計画画面上の全案は新しいタイプ情報で再計算されます。", "");
                        this.AutoCalc = true;
                        this.AutoCalcDiff = false;
                        this.isLoadEnd = false;
                        isUpdated = true;
                        LoadData();

                        if (preSender is System.Windows.Forms.DataVisualization.Charting.Chart)
                        {
                            //このフォームのグラフオブジェクト編集が最終イベントならば
                            this.chartGraph_MouseUp(preSender, (MouseEventArgs)preEvent);
                        }
                        else
                        {
                            //子コントロール側を確認
                            ctr.CallPreSender();
                        }
                    }
                    else
                    {
                        //既にタイプ別積立金のタイプ情報と組合のタイプ情報が同じ、または既に全案再計算済みであればこの案のみ再計算
                        ctr.ReCalcKumiaiRepairReservePlanTypeDetails(draftInfo, lstTypes);
                    }
                    preSender = null;
                    preEvent = null;
                }
              
            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
                MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                preMode = false; //モードを元に戻す。
            }
        }

		/// <summary>
		/// 標準サイズラジオボタンクリックイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void rdbStandard_CheckedChanged(object sender, EventArgs e)
		{
			//  サイズを設定
			if (currViewMode == COMSKCommon.RepairReserveViewMode.Standard)
				SetViewMode(COMSKCommon.RepairReserveViewMode.Full);
		}

		/// <summary>
		/// 拡大サイズラジオボタンクリックイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void rdbZoom_CheckedChanged(object sender, EventArgs e)
		{
			//  サイズを設定
			if (currViewMode == COMSKCommon.RepairReserveViewMode.Full)
				SetViewMode(COMSKCommon.RepairReserveViewMode.Standard);
		}

		/// <summary>
		/// 閉じるボタンクリック
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
			//if (MessageBox.Show(Constant.CONFIRM_CLOSE_TITLE, Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			//{
			//    this.Close();
			//}
		}

		/// <summary>
		/// エクスポートボタンクリック
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnExport_Click(object sender, EventArgs e)
		{
			//  バリデーション
			if (!ValidateControls())
			{
				return;
			}

            // MOD 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↓
            //K300030026 frm = new K300030026();
            K300030026 frm = new K300030026(this.LongRepairPlan.TypeCode);
            //案入替のみの場合、タイプ更新は行わないため通常メッセージ
            if (TypeDetailDifferenceFlg && draftInfo.Draft.Any(c => c.TypeDetailReCalcFlg) && isUpdated == true)
            {
                //メッセージ変更
                frm.SetTypeDetailDifferenceLabel();
            }

            // MOD 2012/08/20 S.Igarashi 国交省ガイドライン・長期修繕計画表 ↑
            if (frm.ShowDialog() == DialogResult.OK)
			{
				this.Cursor = Cursors.WaitCursor;

				try
				{
					//  データを回収
					CollectData();

					// 長計出力
					K300030BL business = new K300030BL();
                    //bool isKokukousyou = !frm.IsExport;
                    //yamagata20150303
                    bool isKokukousyou = frm.IsExport;
                    //yamagata20150303end
                    COMSKService.FileEntry fileEntry = null;

                    if (isKokukousyou)
                    {
                        List<KumiaiLongRepairPlanTaxMst> taxList = business.GetConsumptionTaxList(this.LongRepairPlan.Pid);
                        CreateConsumptionTaxList(taxList);
                        taxList = business.GetPriceIncreaseList(this.LongRepairPlan.Pid);
                        CreatePriceIncreaseList(taxList);
                        AccountPeriodIndex = LongRepairPlan.AccountPeriod - LongRepairPlan.CreateAccountPeriod;
                        List<KumiaiLongRepairPlanDetail> list = business.GetAllKumiaiLongRepairPlanDetails(LongRepairPlan.Pid, Helper.loginUserInfo.Pid, true);

                        //Make RepairPlanTotal
                        List<LongRepairPlanData> subList = ConvertLongRepairPlanDatas(LongRepairPlanData.FromKumiaiLongRepairPlanDetails(list));
                        RecalcSub_Total(subList);
                        var temp = subList.Where(item => item.Row == LongRepairPlanData.RowType.GroupItem).Select(item => item).ToList();
                        subList = (from item in subList where item.Row == LongRepairPlanData.RowType.GroupItem select item).ToList();
                        List<KumiaiLongRepairPlanDetail> groupedList = LongRepairPlanData.ToKumiaiLongRepairPlanDetails(subList);

                        //Make RepairPlanAll
                        List<LongRepairPlanData> convertedList = LongRepairPlanData.FromKumiaiLongRepairPlanDetails(list);
                        convertedList = (from item in convertedList
                                         orderby item.ConstructionTypePid ascending
                                         select item).ToList();
                        convertedList.Add(LongRepairPlanData.CreateCalcAData());
                        convertedList.Add(LongRepairPlanData.CreateCalcCData());
                        convertedList.Add(LongRepairPlanData.CreateCalcBData());
                        convertedList.Add(LongRepairPlanData.CreateCalcDData());
                        convertedList.Add(LongRepairPlanData.CreateCalcEData());
                        RecalcSub(convertedList);
                        list = LongRepairPlanData.ToKumiaiLongRepairPlanDetails(convertedList);

                        //Export Excel
                        fileEntry = business.ExportKumiaiLongRepairPlan(this.LongRepairPlan.Pid, list, groupedList, draftInfo, Helper.loginUserInfo.Pid, AutoCalc);
                    }
                    else
                    {
                        if (AutoCalc == true )
                        {
                            //自動計算するの場合のみ
                            //出力時全案タイプ別再計算
                            for (int i = 0; i < repairReservePlans.Count(); i++)
                            {
                                repairReservePlans[i].ReCalcKumiaiRepairReservePlanTypeDetails(draftInfo, lstTypes);
                            }
                        }
                        else
                        {
                            //出力時全案タイプ別旧タイプで再計算
                            for (int i = 0; i < repairReservePlans.Count(); i++)
                            {
                                var typeCode = LongRepairPlan.MaintenancePlanInfo.CalcDivision;
                                repairReservePlans[i].ReCalcKumiaiRepairReservePlanTypeDetailsOld(draftInfo, lstTypes, typeCode);
                            }
                        }




                        //20200709帳票出力時に、案入替を反映
                        //var sorted = draftInfo;
                        var orgDraft = draftInfo.Draft;
                        //案順序をソート
                        var sortedDraft = draftInfo.Draft.OrderBy(r => r.ViewSequence).ToArray();
                        draftInfo.Draft = sortedDraft;
                        fileEntry = business.ExportKumiaiLongRepairPlan(this.LongRepairPlan.Pid, draftInfo, AutoCalc);
						var isModifyOwner = this.IsModifyOwnerPlan();
						if (isModifyOwner)
						{
							fileEntry = ReportCommon.RemoveShape(fileEntry, 1, COMSKCommon.COMSK_PLAN_REPORT_MJC_LOGO_NAME);
						}
						//並び替え前のデータを復元
						draftInfo.Draft = orgDraft;
                    }
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


			//TODO: 削除
#if false
			/*
			try
			{
				string strChartFileName = Application.StartupPath
					+ System.IO.Path.DirectorySeparatorChar
					+ "chart.jpg";
				if (System.IO.File.Exists(strChartFileName) == true)
				{
					System.IO.FileAttributes fas = System.IO.File.GetAttributes(strChartFileName);
					fas = fas & ~System.IO.FileAttributes.ReadOnly;
					System.IO.File.SetAttributes(strChartFileName, fas);

					System.IO.File.Delete(strChartFileName);
				}
				chartGraph.SaveImage(strChartFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

				saveFileDialog1.FileName = "修繕積立金計画グラフ.xls";
				if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

				string fileName = saveFileDialog1.FileName;

				string strTemplateFileName = Application.StartupPath
					+ System.IO.Path.DirectorySeparatorChar
					+ K200010Helper.DIRNAME_DOCUMENT
					+ System.IO.Path.DirectorySeparatorChar
					+ @"RepairPlanGraphTemplate.xls";
				if (System.IO.File.Exists(strTemplateFileName) == false)
				{
					MessageBox.Show("テンプレートファイルがありません。\n\n" + strTemplateFileName, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				AdvanceSoftware.VBReport7.Xls.Web.XlsWebReport xlsReport = new AdvanceSoftware.VBReport7.Xls.Web.XlsWebReport();
				xlsReport.FileName = strTemplateFileName;
				xlsReport.Report.Start();
				xlsReport.Report.File();

				xlsReport.Page.Start("template", "1");

				using (System.Drawing.Image img = System.Drawing.Bitmap.FromFile(strChartFileName))
				{
					xlsReport.Cell("A1").Drawing.AddImage(img, 600, 600 * img.Height / img.Width);
				}

			#region 積立金累計

				int startPos = 38;
				int colIndex = 1;
				for (int i = 0; i < 3; i++)
				{
					colIndex = 1;
					for (int j = 0; j <= 30; j++)
					{
						string name = ColumnName(colIndex);
						xlsReport.Cell(string.Format("{0}{1}", name, startPos + i)).Value = gvData30Period.GetRowCellValue(i, string.Format("Val{0}", j));
						colIndex++;
					}
				}

			#endregion

			#region Y.H 対応が面倒なので封印

			#region 段階組立案

				startPos = 42;
				for (int i = 0; i < 5; i++)
				{
					colIndex = 1;
					for (int j = 0; j <= 30; j++)
					{
						string name = ColumnName(colIndex);
						//xlsReport.Cell(string.Format("{0}{1}", name, startPos + i)).Value = gridView_Line1.GetRowCellValue(i, string.Format("Val{0}", j));
						colIndex++;
					}
				}

			#endregion 段階組立案

			#region 段階組立案（一時金）

				startPos = 49;
				for (int i = 0; i < 5; i++)
				{
					colIndex = 1;
					for (int j = 0; j <= 30; j++)
					{
						string name = ColumnName(colIndex);
						//xlsReport.Cell(string.Format("{0}{1}", name, startPos + i)).Value = gridView_Line2.GetRowCellValue(i, string.Format("Val{0}", j));
						colIndex++;
					}
				}

			#endregion 段階組立案（一時金）

			#region 均等組立案<<参考>>

				startPos = 56;
				for (int i = 0; i < 5; i++)
				{
					colIndex = 1;
					for (int j = 0; j <= 30; j++)
					{
						string name = ColumnName(colIndex);
						//xlsReport.Cell(string.Format("{0}{1}", name, startPos + i)).Value = gridView_Line3.GetRowCellValue(i, string.Format("Val{0}", j));
						colIndex++;
					}
				}

			#endregion 均等組立案<<参考>>

			#endregion

				xlsReport.Page.Name = "サンプル";
				xlsReport.Page.End();

				xlsReport.Report.End();

				xlsReport.Report.SaveAs(fileName);

				System.Diagnostics.Process.Start(fileName);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "例外", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			 * */
#endif
		}

		private bool IsModifyOwnerPlan()
		{
			return this.LongRepairPlan.TypeCode == COMSKCommon.COMSK_LONG_REPAIR_PLAN_TYPE_REEXAM && this.LongRepairPlan.PresentationCode == COMSKCommon.COMSK_PRESENTCODE_OWNER;
		}

		/// <summary>
		/// 登録ボタンイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnSave_Click(object sender, EventArgs e)
		{
			//  バリデーション
			if (!ValidateControls())
			{
				return;
			}
            //20140818 y-hoshino start
            if ((LongRepairPlan.StatusCode == "0002") || (LongRepairPlan.StatusCode == "0003") || (LongRepairPlan.StatusCode == "0005"))
            {
                if (MessageBox.Show(Constant.CONFIRM_KAKUTEIHOZON_TITLE, Constant.CONFIRM_KAKUTEI_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    return;
                }
            }
            //20140818 y-hoshino end
            else
            {
                if (TypeDetailDifferenceFlg && draftInfo.Draft.Any(c => c.TypeDetailReCalcFlg) && isUpdated == true)
                {
                    if(MessageBox.Show("新しいタイプ情報で再計算され、保存されます。よろしいですか？", Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                else if(MessageBox.Show("修繕積立金計画を登録しますか？", Constant.CONFIRM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

			//  カーソルを変更
			this.Cursor = Cursors.WaitCursor;

			// 変更セル履歴をクリア
			this.ResetChangedCells();

			//  情報を集める
			CollectData();
			try
			{
                //MJC_DEV-817 【長計：問合せ】タイプ別積立金額不正 (No.375)　本対応(プログラム修正)
                //保存時自動計算するなら、タイプ別再計算

                if (AutoCalc == true )
                {
                    for (int i = 0; i < repairReservePlans.Count(); i++)
                    {
                        repairReservePlans[i].ReCalcKumiaiRepairReservePlanTypeDetails(draftInfo, lstTypes);
                    }
                }
                else
                {
                    //保存時全案タイプ別旧タイプで再計算
                    for (int i = 0; i < repairReservePlans.Count(); i++)
                    {
                        var typeCode = LongRepairPlan.MaintenancePlanInfo.CalcDivision;
                        repairReservePlans[i].ReCalcKumiaiRepairReservePlanTypeDetailsOld(draftInfo, lstTypes, typeCode);
                    }
                }

				//  案を登録
				K300070BL bl = new K300070BL();
                if (!bl.UpdateKumiaiRepairReservePlanDraftInfo(this.LongRepairPlan.Pid, this.draftInfo, AutoCalc))
				{
					throw new Exception("積立金計画データの保存に失敗しました。");
				}

				//  閉じる
				this.Close();
					
				////  保存完了
				//MessageBox.Show(Constant.SAVE_COMPLETE_TITLE, Constant.INFO_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
		/// 積立金設定
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void btnSettings_Click(object sender, EventArgs e)
		{
			K300070011 frm = new K300070011()
			{
				KumiaiLongRepairPlanPid = this.LongRepairPlan.Pid,
                // ducthang 20160425 add
                SetDefaultAutoCalcAndDisable = true,
                // end ducthang 20160425 
			};  
			if (frm.ShowDialog() == DialogResult.OK)
			{
                AutoCalc = frm.AutoCalc;
                AutoCalcDiff = frm.AutoCalcDiff;

				if (frm.AutoCalc == true)
				{
					//  画面リロード
                    isLoadEnd = false;
					LoadData();
                    isLoadEnd = true;
				}
				else
				{
					//  設定を更新
					try
					{
						//  最新の案情報を取得
						K300070BL bl70 = new K300070BL();
						KumiaiRepairReservePlanDraftInfo draftInfo = bl70.GetKumiaiRepairReservePlanDraftInfo(this.LongRepairPlan.Pid);

						//  案を更新
						for (int i = 0; i < repairReservePlans.Length; i++)
						{
							repairReservePlans[i].SetDraftSummary(draftInfo.Draft[i]);
						}
					}
					catch (Exception ex)
					{
						Helper.WriteLog(ex);
						MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}

				// 入力箇所リセット
				this.ResetChangedCells();
			}
		}

		#endregion

		#region 積立金計画コントロールイベント

		/// <summary>
		/// 案名変更 - 1
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairReservePlan1_DraftNameChanged(object sender, EventArgs e)
		{
			tabPage1.Text = ctrRepairReservePlan1.DraftName;
			chartGraph.Series[1].LegendText = tabPage1.Text;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			ChangeCurrPage(tabControl1.SelectedTab);
		}

		/// <summary>
		/// 案名変更 - 2
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairReservePlan2_DraftNameChanged(object sender, EventArgs e)
		{
			tabPage2.Text = ctrRepairReservePlan2.DraftName;
			chartGraph.Series[2].LegendText = tabPage2.Text;
		}

		/// <summary>
		/// 案名変更 - 3
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairReservePlan3_DraftNameChanged(object sender, EventArgs e)
		{
			tabPage3.Text = ctrRepairReservePlan3.DraftName;
			chartGraph.Series[3].LegendText = tabPage3.Text;
		}

		/// <summary>
		/// 案名変更 - 4
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairReservePlan4_DraftNameChanged(object sender, EventArgs e)
		{
			tabPage4.Text = ctrRepairReservePlan4.DraftName;
			chartGraph.Series[4].LegendText = tabPage4.Text;
		}

		/// <summary>
		/// 案名変更 - 5
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairReservePlan5_DraftNameChanged(object sender, EventArgs e)
		{
			tabPage5.Text = ctrRepairReservePlan5.DraftName;
			chartGraph.Series[5].LegendText = tabPage5.Text;
		}

		/// <summary>
		/// 使用するチェック選択イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ctrRepairReservePlan_UseChanged(object sender, EventArgs e)
		{
			try
			{
				//  案コントロールを取得
				common.CtrRepairReservePlan plan = sender as common.CtrRepairReservePlan;

				//  切り替え
				ToggleChartEnable(plan);
			}
			catch (Exception)
			{
			}
		}

		#endregion

		#region コンテキストメニュー

		/// <summary>
		/// 「左へ移動」メニュークリックイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void mnuMoveLeft_Click(object sender, EventArgs e)
		{
			//  現在のページ
			int currPage = tabControl1.SelectedIndex;

			//MJC_DEV-797 【長計：問合せ】タイプ別積立金計算の金額について (No.357) 20200608
			var draft1 = draftInfo.Draft.Where(r => r.ViewSequence == currPage + 1).FirstOrDefault();
			var draft2 = draftInfo.Draft.Where(r => r.ViewSequence == currPage).FirstOrDefault();
			int draftnum1 = 0;
			int draftnum2 = 0;

			for (int i = 0; i < draftInfo.Draft.Count(); i++)
			{
				if (draftInfo.Draft[i].ViewSequence == currPage + 1)
				{
					draftnum1 = i;
				}

				if (draftInfo.Draft[i].ViewSequence == currPage)
				{
					draftnum2 = i;
				}
			}

			//  入れ替え
			KumiaiRepairReservePlanDraft temp = draftInfo.Draft[draftnum1];
			draftInfo.Draft[draftnum1] = draftInfo.Draft[draftnum2];
			draftInfo.Draft[draftnum2] = temp;

			//  ページも入れ替え
			TabPage page = tabControl1.TabPages[currPage];
			tabControl1.TabPages.RemoveAt(currPage);
			tabControl1.TabPages.Insert(currPage - 1, page);

			//  現在のタブを変更
			tabControl1.SelectedIndex = currPage - 1;

			var typeCode = LongRepairPlan.MaintenancePlanInfo.CalcDivision;
			repairReservePlans[draftnum1].ReCalcKumiaiRepairReservePlanTypeDetailsOld(draftInfo, lstTypes, typeCode);
			repairReservePlans[draftnum2].ReCalcKumiaiRepairReservePlanTypeDetailsOld(draftInfo, lstTypes, typeCode);
			//MJC_DEV-797 【長計：問合せ】タイプ別積立金計算の金額について (No.357) 20200608 end
		}

		/// <summary>
		/// 「右へ移動」メニュークリックイベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void mnuMoveRight_Click(object sender, EventArgs e)
		{
			//  現在のページ
			int currPage = tabControl1.SelectedIndex;

			//MJC_DEV-797 【長計：問合せ】タイプ別積立金計算の金額について (No.357) 20200608
			var draft1 = draftInfo.Draft.Where(r => r.ViewSequence == currPage + 1).FirstOrDefault();
			var draft2 = draftInfo.Draft.Where(r => r.ViewSequence == currPage + 2).FirstOrDefault();
			int draftnum1 = 0;
			int draftnum2 = 0;

			for (int i = 0; i < draftInfo.Draft.Count(); i++)
			{
				if (draftInfo.Draft[i].ViewSequence == currPage + 1)
				{
					draftnum1 = i;
				}

				if (draftInfo.Draft[i].ViewSequence == currPage + 2)
				{
					draftnum2 = i;
				}
			}

			//  入れ替え
			KumiaiRepairReservePlanDraft temp = draftInfo.Draft[draftnum1];
			draftInfo.Draft[draftnum1] = draftInfo.Draft[draftnum2];
			draftInfo.Draft[draftnum2] = temp;

			//  ページも入れ替え
			TabPage page = tabControl1.TabPages[currPage + 1];
			tabControl1.TabPages.RemoveAt(currPage + 1);
			tabControl1.TabPages.Insert(currPage, page);

			//  現在のタブを変更
			tabControl1.SelectedIndex = currPage + 1;

			var typeCode = LongRepairPlan.MaintenancePlanInfo.CalcDivision;
			repairReservePlans[draftnum1].ReCalcKumiaiRepairReservePlanTypeDetailsOld(draftInfo, lstTypes, typeCode);
			repairReservePlans[draftnum2].ReCalcKumiaiRepairReservePlanTypeDetailsOld(draftInfo, lstTypes, typeCode);
			//MJC_DEV-797 【長計：問合せ】タイプ別積立金計算の金額について (No.357) 20200608 end
		}

		/// <summary>
		/// コンテキストメニュー表示前イベント
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		private void mnuMoveTab_Opening(object sender, CancelEventArgs e)
		{
			mnuMoveLeft.Enabled = (tabControl1.SelectedIndex > 0);
			mnuMoveRight.Enabled = (tabControl1.SelectedIndex < (tabControl1.TabPages.Count - 1));
		}

		#endregion

		#endregion

		#region Private

		/// <summary>
		/// 積立金データを読み込む
		/// </summary>
		private void LoadData()
		{
			try
			{
				#region サーバからデータ取得

				//  Pid が指定されていたら長計データを取得
				//  それ以外は LongRepairPlan.Pid を使って再度取得
				if (this.LongRepairPlanPid != long.MinValue)
				{
					K300030BL bl30 = new K300030BL();
					this.LongRepairPlan = bl30.GetKumiaiLongRepairPlan(this.LongRepairPlanPid);
					if (this.LongRepairPlan == null)
					{
						throw new Exception("長計データを正しく取得できませんでした。");
					}
				}
				else
				{
					K300030BL bl30 = new K300030BL();
					this.LongRepairPlan = bl30.GetKumiaiLongRepairPlan(this.LongRepairPlan.Pid);
					if (this.LongRepairPlan == null)
					{
						throw new Exception("長計データを正しく取得できませんでした。");
					}
				}

				//  表示単位に応じて値を決定
				if (this.LongRepairPlan.MaintenancePlanInfo.ViewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_10_THOUSAND)
				{
					viewUnitValue = 10000;
				}
				else
				{
					viewUnitValue = 1000;
				}

				//  データを取得
				//  取得データは登録時に明細タイプおよび PID を使うため、覚えておく
				K300070BL bl70 = new K300070BL();
				draftInfo = bl70.GetKumiaiRepairReservePlanDraftInfo(this.LongRepairPlan.Pid);

				//  5 案あるはず
				if (draftInfo.Draft.Length != MAX_REPAIR_RESERVE_PLAN)
				{
					throw new Exception(string.Format("データが不正です: 案が 5 個ではありません ({0})", draftInfo.Draft.Length));
				}
				////  専有面積等が取れなかったら
				//else if (draftInfo.AppropriationArea == double.MinValue || draftInfo.Ownership == double.MinValue)
				//{
				//    string msg = "専有面積、もしくは共有持分を取得できませんでした。\n修繕積立金を正しく計算できない可能性があります。";
				//    MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				//}

				#region TODO: デバッグ
				{
					string val;
					if (LongRepairPlan.MaintenancePlanInfo.CalcDivision == COMSKCommon.COMSK_LONG_REPAIR_PLAN_CALC_DIVISION_AREA)
					{
						val = "㎡";
                        //val = "専有面積";
					}
					else
					{
						val = "共有持分";
					}
					System.Diagnostics.Debug.WriteLine("■  計算方法: " + val);

					string fracUnit = string.Empty;
					string fracProc = string.Empty;
					if (LongRepairPlan.MaintenancePlanInfo.LumpsumFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_ONE)
					{
						fracUnit = "１円";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.LumpsumFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_TEN)
					{
						fracUnit = "１０円";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.LumpsumFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_HUNDRED)
					{
						fracUnit = "１００円";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.LumpsumFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_THOUSAND)
					{
						fracUnit = "１０００円";
					}

					if (LongRepairPlan.MaintenancePlanInfo.LumpsumFractionProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND)
					{
						fracProc = "四捨五入";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.LumpsumFractionProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_UP)
					{
						fracProc = "切り上げ";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.LumpsumFractionProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_UP)
					{
						fracProc = "切り捨て";
					}
					System.Diagnostics.Debug.WriteLine(String.Format("■  一時金端数処理: {0}, {1}", fracUnit, fracProc));

					if (LongRepairPlan.MaintenancePlanInfo.MonthlyFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_ONE)
					{
						fracUnit = "１円";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.MonthlyFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_TEN)
					{
						fracUnit = "１０円";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.MonthlyFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_HUNDRED)
					{
						fracUnit = "１００円";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.MonthlyFractionUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_UNIT_THOUSAND)
					{
						fracUnit = "１０００円";
					}

					if (LongRepairPlan.MaintenancePlanInfo.MonthlyFractionProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND)
					{
						fracProc = "四捨五入";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.MonthlyFractionProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_UP)
					{
						fracProc = "切り上げ";
					}
					else if (LongRepairPlan.MaintenancePlanInfo.MonthlyFractionProcess == COMSKCommon.COMSK_LONG_REPAIR_PLAN_FRACTION_PROCESS_ROUND_UP)
					{
						fracProc = "切り捨て";
					}
					System.Diagnostics.Debug.WriteLine(String.Format("■  月額端数処理: {0}, {1}", fracUnit, fracProc));

					long totalRoom = (from item in LongRepairPlan.MaintenancePlanInfo.Types
									  select item.RoomNumber).Sum();
					System.Diagnostics.Debug.WriteLine(string.Format("■  タイプ数={0}, 総戸数={1}",
						LongRepairPlan.MaintenancePlanInfo.Types.Length, totalRoom));

				}
				#endregion

				//  工事費累計を取得
				if (LongRepairPlanPid == long.MinValue)
				{
					//  工事費累計は渡されたデータを使う
					totalRepairCostArray = new long[COMSKCommon.YEAR100];
					for (int i = 0; i < TempTotalRepairCost.Length; i++)
					{
						totalRepairCostArray[i] = TempTotalRepairCost[i];
					}

					////  viewUnit で割られた形なので、viewUnitValue 倍する
					//for (int i = 0; i < totalRepairCostArray.Length; i++)
					//{
					//    totalRepairCostArray[i] *= viewUnitValue;
					//}
				}
				else
				{
					//  工事費累計はサーバから取得して使う
					totalRepairCostArray = bl70.GetTotalRepairCost(LongRepairPlan.Pid);
				}

				//  資金ショートコードマスタを取得
				COMMONService.CodeMst[] codeMst = COMSKCommon.GetCostShortCodeMst();

                //タイプマスタを取得
                GetTypeMst();
                TypeDetailDifferenceFlg = !bl70.GetTypeDetailDifference(this.LongRepairPlan.Pid);
				#endregion

				#region フォーム情報セット

				//  フォーム基礎情報をセット
				this.txtKumiai.Text = this.LongRepairPlan.KumiaiName;

				//  金額単位をセット
				if (this.LongRepairPlan.MaintenancePlanInfo.ViewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_THOUSAND)
				{
					//chartGraph.ChartAreas[0].AxisY.Title = "金額　千円";
					this.lbChartLeft.Text = "金\r\n額\r\n\r\n千\r\n円";
				}
				else if (this.LongRepairPlan.MaintenancePlanInfo.ViewUnit == COMSKCommon.COMSK_LONG_REPAIR_PLAN_VIEW_UNIT_10_THOUSAND)
				{
					//chartGraph.ChartAreas[0].AxisY.Title = "金額　万円";
					this.lbChartLeft.Text = "金\r\n額\r\n\r\n万\r\n円";
				}

				#endregion

				//  会計期データをセット
				Data30Period data30Period = new Data30Period(totalRepairCostArray);
				int accountPeriodIndex = COMSKCommon.GetAccountPeriodIndexFromAccountPeriod(LongRepairPlan.TermInfo, LongRepairPlan.AccountPeriod);
				SetAccountPeriods(LongRepairPlan.TermInfo, data30Period, accountPeriodIndex);

				//  工事費累計を設定
				SetChartData(0, data30Period);

				//  タブに設定
				for (int i = 0; i < draftInfo.Draft.Length; i++)
				{
					repairReservePlans[i].DraftIndex = i;
					repairReservePlans[i].DetailSummary = draftInfo.Summary;
					repairReservePlans[i].TermInfo = LongRepairPlan.TermInfo;
					repairReservePlans[i].AccountPeriod = LongRepairPlan.AccountPeriod;

					//  資金ショート候補セット
					repairReservePlans[i].SetCostShortCodeMst(codeMst);

					//  案データをセット
					repairReservePlans[i].SetDraft(LongRepairPlan, data30Period, draftInfo.Summary, draftInfo.Draft[i]);

					//  自動計算が ON なら
					if (AutoCalc == true || AutoCalcDiff == true)
					{
						//  自動計算
						//  計算後、チャートデータ変更イベントが発生する
						repairReservePlans[i].Recalc(this.LongRepairPlan, LongRepairPlan.TermInfo, draftInfo.Draft[i], AutoCalcDiff, AutoCalc);
                        if (AutoCalc == true) repairReservePlans[i].ReCalcKumiaiRepairReservePlanTypeDetails(draftInfo, lstTypes);
					}
					else
					{
						//  修繕積立金累計をチャートにアサイン
						SetChartData(i + 1, repairReservePlans[i].TotalReserveData);

					}

					//  有効無効切り替え
					ToggleChartEnable(repairReservePlans[i]);

                    //MJC_DEV-860 【長計問合せ】確認：金額チェックのNGデータについて (11/16) (No.388)
                    //ロード時に自動計算する以外金額更新フラグリセット
                    if (AutoCalc == false)
                    {
                        isUpdated = false;
                    }
				}

				//  Pid が未指定→保存できないようにする
				//if (this.LongRepairPlanPid == long.MinValue)
				//{
				//	btnSave.Enabled = false;
				//}

				//  グラフの最大値をセット
				SetChartMaxValue();

				//  リサイズ処理
				chartGraph_SizeChanged(null, EventArgs.Empty);

				// 決算以外の場合、年度内修繕積立金単価、住戸月額金積立金単価は表示しない
				foreach (coms.COMSK.ui.common.CtrRepairReservePlan plan in repairReservePlans)
				{
					plan.displayChange(this.LongRepairPlan.TypeCode);
				}

                //Linh 20170405 https://reci.backlog.jp/view/MJC_DEV-541
                //this.Text += "_" + LongRepairPlan.Name;
                if (this.Text.IndexOf("_") < 0)
                {
                    this.Text += "_" + LongRepairPlan.Name;
                }
                //End Linh 20170405 https://reci.backlog.jp/view/MJC_DEV-541
			}
			catch (Exception ex)
			{
				Helper.WriteLog(ex);
				MessageBox.Show(ex.Message, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.Close();
			}
		}

        /// <summary>
        /// 組合タイプマスタをセットする
        /// </summary>
        private void GetTypeMst()
        {
            K300000BL bl = new K300000BL();
            this.lstTypes = bl.ListTypeMst(this.LongRepairPlan.KumiaiInfoPid);
        }
        
		/// <summary>
		/// 会計期データをセットする
		/// </summary>
		/// <param name="termInfo">The term info.</param>
		/// <param name="repairConstructionTotal">The repair construction total.</param>
		/// <param name="startIndex">The start index.</param>
		private void SetAccountPeriods(KumiaiTermInfo[] termInfo, Data30Period repairConstructionTotal, int startIndex)
		{
			Data30Period dataBuildYear = new Data30Period();
			Data30Period dataAccountPeriod = new Data30Period();
			Data30Period dataAccountYear = new Data30Period();

			for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
			{
				KumiaiTermInfo info = termInfo[startIndex + i];

				dataBuildYear.SetValue(i, int.Parse(info.TikuYear));
				dataAccountPeriod.SetValue(i, info.Term);
				dataAccountYear.SetValue(i, int.Parse(info.FiscalYear));
			}

			//  リストを作成
			totalRepairCost.Add(dataBuildYear);
			totalRepairCost.Add(dataAccountPeriod);
			totalRepairCost.Add(dataAccountYear);
			totalRepairCost.Add(repairConstructionTotal);

			//  データソースを設定
			gcData30Period.DataSource = totalRepairCost;
		}

		/// <summary>
		/// タブの内容を設定する
		/// </summary>
		/// <param name="xtraTabPage">The xtra tab page.</param>
		private void ChangeCurrPage(TabPage xtraTabPage)
		{
			var ctrPlan = xtraTabPage.Controls[0];
			if (ctrPlan is common.CtrRepairReservePlan)
            {
				var planObj = ctrPlan as common.CtrRepairReservePlan;
				if (!lstViewedPlan.Contains(planObj)) { 
					lstViewedPlan.Add(planObj);
					if (this.currScrollPos != 0)
						this.ScrollSubTab(planObj, this.currScrollPos);
				}
			} 
		}

		/// <summary>
		/// チャートにデータを設定する
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="data">The data.</param>
		private void SetChartData(int index, Data30Period data)
		{
			System.Windows.Forms.DataVisualization.Charting.Series s = chartGraph.Series[index];
			for (int i = 0; i < s.Points.Count; i++)
			{
				double val = 0;
				if (i < COMSKCommon.MAX_VISIBLE_YEAR) val = data.GetValue(i) / viewUnitValue;
				if (val == double.MinValue)
				{
					val = 0;
				}
				s.Points[i].SetValueY(val);

			}

			//  最大値を再計算
			SetChartMaxValue();
		}

		/// <summary>
		/// グラフの最大値を変更する
		/// </summary>
		private void SetChartMaxValue()
		{
			//  工事費累計の最大を取得
			double maxY = GetChartMaxValue(totalRepairCostArray, draftInfo, viewUnitValue);

			//  グラフの最高値に設定
			chartGraph.ChartAreas[0].AxisY.Maximum = maxY;
		}

		/// <summary>
		/// 表示モード切替
		/// </summary>
		/// <param name="viewMode">The view mode.</param>
		private void SetViewMode(COMSKCommon.RepairReserveViewMode viewMode)
		{
			currViewMode = viewMode;
			var targetFont = fntFull;
			int chartWidth = 0;

			if (viewMode == COMSKCommon.RepairReserveViewMode.Standard)
			{
				targetFont = fntStandard;
				chartWidth = calc.CalcReserveChartWidth(this.chartGraph.Width, COMSKCommon.RepairReserveViewMode.Standard);
			}
			else
			{
				targetFont = fntFull;
				chartWidth = calc.CalcReserveChartWidth(this.chartGraph.Width, COMSKCommon.RepairReserveViewMode.Full);
			}

			this.chartGraph.Width = chartWidth;

			//  フォントを差し替え
			for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
			{
				gcData30Period.Columns[i].DefaultCellStyle.Font = targetFont;
			}
			gcData30Period.Invalidate();

			//  グリッドアップデート
			ctrRepairReservePlan1.SetViewMode(viewMode, targetFont);
			ctrRepairReservePlan2.SetViewMode(viewMode, targetFont);
			ctrRepairReservePlan3.SetViewMode(viewMode, targetFont);
			ctrRepairReservePlan4.SetViewMode(viewMode, targetFont);
			ctrRepairReservePlan5.SetViewMode(viewMode, targetFont);

			// reset scroll position
			if (this.currScrollPos != 0)
            {
				this.currScrollPos = calc.CalcNewScrollPos(this.currScrollPos, currViewMode);
				pnScrollTop.AutoScrollPosition = new Point(this.currScrollPos, -pnScrollTop.AutoScrollPosition.X);
				pnScrollMid.AutoScrollPosition = new Point(this.currScrollPos, -pnScrollMid.AutoScrollPosition.X);
				foreach(var ctrReserve in this.lstViewedPlan)
                {
					this.ScrollSubTab(ctrReserve, this.currScrollPos);
				}
			}
		}

		/// <summary>
		/// バリデーション
		/// </summary>
		/// <returns></returns>
		private bool ValidateControls()
		{
			bool ret = true;
			string errDraftList = string.Empty;

			foreach (common.CtrRepairReservePlan c in repairReservePlans)
			{
				if (!c.ValidateControls(false))
				{
					errDraftList += string.Format("{0}\r\n", c.DraftName);
					ret = false;
				}
			}

			if (ret == true)
			{
				return true;
			}
			else
			{
				string msg = string.Format("{0}\r\nに入力エラーがあります。", errDraftList);
				MessageBox.Show(msg, Constant.ERROR_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
		}

		/// <summary>
		/// 登録データを回収
		/// </summary>
		private void CollectData()
		{
			for (int i = 0; i < repairReservePlans.Length; i++)
			{
				repairReservePlans[i].GetDraft(draftInfo.Draft[i]);
			}
		}

		/// <summary>
		/// グラフの最大値を求める
		/// </summary>
		/// <returns></returns>
		private long GetChartMaxValue(long[] totalRepairCost, KumiaiRepairReservePlanDraftInfo draftInfo, int viewUnitValue)
		{
			//  累計工事費の最大値を求める
			//  (普通は一番後ろが最大値だが、手でいじることもできる)
			double maxY = totalRepairCost[COMSKCommon.MAX_VISIBLE_YEAR - 1];

			//  明細コードを探す
			int detailCodeIndex = 0;
			for (int i = 0; i < draftInfo.Summary.Length; i++)
			{
				KumiaiRepairReservePlanDetailSummary summary = draftInfo.Summary[i];

				if (summary.ReservePlanDetailCode == COMSKCommon.COMSK_REPAIR_RESERVE_PLAN_DETAIL_CODE_TOTAL_RESERVE_COST)
				{
					detailCodeIndex = i;
					break;
				}
			}

			//  案を回す
			foreach (common.CtrRepairReservePlan plan in repairReservePlans)
			{
				//  値の束を取得
				Data30Period data = plan.TotalReserveData;
				if (data == null)
				{
					continue;
				}

				//  最大値を取得
				double maxY2 = data.MaxValue(COMSKCommon.MAX_VISIBLE_YEAR);

				//  maxY と比較して大きいほうを採用
				if (maxY2 > maxY)
				{
					maxY = maxY2;
				}
			}

			//  それの 1.2 倍
			maxY *= 1.2;

			//  表示単位に合わせる
			maxY /= viewUnitValue;

			//  五千円/五万円単位で切り上げ
			maxY = Math.Ceiling(maxY / viewUnitValue) * viewUnitValue;

			//  最低でも 1
			if (maxY < 1)
			{
				maxY = 1;
			}

			//  OK
			return (long)(maxY);
		}

		/// <summary>
		/// チャートのグラフの表示を切り替える
		/// </summary>
		/// <param name="plan">The plan.</param>
		private void ToggleChartEnable(common.CtrRepairReservePlan plan)
		{
			//  案インデックスを取得し、+1
			int draftIndex = plan.DraftIndex + 1;

			//  有効無効切り替え
			if (plan.Uses)
			{
				chartGraph.Series[draftIndex].Enabled = true;
			}
			else
			{
				chartGraph.Series[draftIndex].Enabled = false;
			}
		}

		/// <summary>
		/// 表示単位で割り、四捨五入したものを返す
		/// </summary>
		/// <param name="val">The val.</param>
		/// <param name="viewUnitValue">The view unit value.</param>
		/// <returns></returns>
		private long RoundByViewUnit(double val, int viewUnitValue)
		{
			return (long)Math.Round(val / viewUnitValue, MidpointRounding.AwayFromZero);
		}

		private void ResetChangedCells()
		{
			for (int i = 0; i < repairReservePlans.Length; i++)
			{
				repairReservePlans[i].ResetChangedCells();
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
			"    AI.partsCode LIKE '%btnK300070010%'";

			COMSD.business.D100109BL bl109 = new coms.COMSD.business.D100109BL();
			COMSDService.AuthorityInfo[] authorities = bl109.SearchAuthorityInfo(sql, "", "");

			btnExport.Enabled = Helper.GetAuthority(authorities, "btnK300070010001", userPostCode, userPositionCode);
			btnSave.Enabled = Helper.GetAuthority(authorities, "btnK300070010002", userPostCode, userPositionCode);
		}
		#endregion

        #region copy from K300030020 for CALC
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
                for (int i = 0; i < COMSKCommon.MAX_VISIBLE_YEAR; i++)
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
        /// 正しく四捨五入する
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        private long RoundToLong(double val)
        {
            return (long)Math.Round(val, MidpointRounding.AwayFromZero);
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
                priceIncrRate *= (1.0 + this.priceIncreaseList[accountPeriodIndex + i]);
            }

            //  基準の 1 を引く
            ret = priceIncrRate - 1;
            return ret;
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

		private void pnScrollTop_MouseWheel(object sender, MouseEventArgs e)
		{
			this.FireScroll(this.pnScrollTop);
		}

		private void pnScrollMid_MouseWheel(object sender, MouseEventArgs e)
		{
			this.FireScroll(this.pnScrollMid);
		}

		private void FireScroll(coms.COMSK.ui.common.NoScrollPanel sender)
        {
			var newPos = -sender.AutoScrollPosition.X;
			this.currScrollPos = newPos;
			if (sender == this.pnScrollTop)
            {
				pnScrollMid.AutoScrollPosition = new Point(newPos, -pnScrollMid.AutoScrollPosition.X);
			}
			else
            {
				pnScrollTop.AutoScrollPosition = new Point(newPos, -pnScrollTop.AutoScrollPosition.X);
			}

			this.ScrollAllSubTab(null, newPos);
		}

		private void ScrollSubTab(common.CtrRepairReservePlan targetPlan, int newPos)
		{
			targetPlan.scrollingByParent = true;
			targetPlan.ScrollByParentPanel(newPos);
			targetPlan.scrollingByParent = false;
		}

		private void ScrollAllSubTab(common.CtrRepairReservePlan sender, int newPos)
        {
			foreach(var tab in this.repairReservePlans)
            {
				if (sender == null || sender != tab)
                {
					this.ScrollSubTab(tab, newPos);
				}
            }
        }
		#endregion
	}
}
