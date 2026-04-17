
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace coms.COMSK.common
{
    public class XtraGridUtil
    {
		private static float LongPlanFontSizeNormal = 9.0f;
		private static int scrollAmount = 20;
		private static int maxPosRight = 2500;// this got from debug when can not scroll right any more
		private static int zoomStep = 10; // Zoom in/out step
		private static int zoomMaxRate = 150;
		private static int zoomMinRate = 50;

		public static Dictionary<string, List<List<int>>> GetRepairPlanMergedCells(
			DevExpress.XtraGrid.Views.BandedGrid.BandedGridView gridView
			, List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColCheck)
		{
			var result = new Dictionary<string, List<List<int>>>();
			foreach (var col in lstColCheck)
			{
				result[col.Name] = new List<List<int>>();
			}
			// Sub
			Dictionary<string, List<int>> subList = new Dictionary<string, List<int>>();
			foreach (var col in lstColCheck)
			{
				subList[col.Name] = new List<int>();
			}
			for (int rowHandle = 0; rowHandle < gridView.RowCount - 1; rowHandle++)
			{
				var obj = gridView.GetRow(rowHandle) as LongRepairPlanData;
				if (obj.Row != LongRepairPlanData.RowType.RepairPlan) continue;

				foreach (var colCheck in lstColCheck)
				{
					var colCheckName = colCheck.Name;
					if (!subList[colCheckName].Contains(rowHandle))
					{
						subList[colCheckName].Add(rowHandle);
					}

					var currentValue = gridView.GetRowCellValue(rowHandle, colCheck);
					var nextValue = gridView.GetRowCellValue(rowHandle + 1, colCheck);
					if (currentValue != null && currentValue.Equals(nextValue))
					{
						subList[colCheckName].Add(rowHandle + 1);
					}
					else
					{
						result[colCheckName].Add(subList[colCheckName]);
						subList[colCheckName] = new List<int>();
					}
				}
			}

			return result;
		}

		public static bool IsMergedCell(
			DevExpress.XtraGrid.Views.BandedGrid.BandedGridView gridView
			, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e
			, Dictionary<string, List<List<int>>> lstMergedCols)
		{
			if (lstMergedCols.Keys.Contains(e.Column.Name))
			{
				var lstMergedRows = lstMergedCols[e.Column.Name];
				List<int> currFocusRowMergedIdx = new List<int>();
				foreach (var lstRowId in lstMergedRows)
				{
					if (lstRowId.Contains(gridView.FocusedRowHandle))
					{
						currFocusRowMergedIdx = lstRowId;
						break;
					}
				}
				return currFocusRowMergedIdx.Contains(e.RowHandle) && currFocusRowMergedIdx.Count > 1;
			}
			return false;
		}
		/// <summary>
		/// ※※※　築年の以外バンドのNameがbgcolValue、gbCalendarYear、gbYear、gbPlanYear
		/// 　　その値入れないで下さい
		/// ※※※　reCreatedYearlyがTRUEの場合バンドが作り直されたために「originalWidth」に戻されてる
		/// </summary>
		/// <param name="gridView"></param>
		/// <param name="currentRate"></param>
		/// <param name="reCreatedYearly"></param>
		/// <returns></returns>
		public static Dictionary<string, int> GetBandedWidthDict(DataGridView gridView, int currentRate, bool reCreatedYearly)
		{
			var ret = new Dictionary<string, int>();
			foreach (DataGridViewColumn col in gridView.Columns)
			{
				var originalWidth = (int)Math.Ceiling(col.Width * 100.0f / currentRate);
				if (col.Name.Contains("bgcolValue"))
				{
					if (reCreatedYearly) originalWidth = col.Width;
				}
				ret.Add(col.Name, originalWidth);
			}
			return ret;
		}

		public static void SetZoom(
			DataGridView gridView,
			Dictionary<string, int> dictBandWidth,
			int rate,
			List<string> keepSizeBand)
		{
			if (gridView == null) return;

			// DevExpress版と同様にフォントサイズを計算
			var fontSize = (float)Math.Round(rate * LongPlanFontSizeNormal / 100.0f, 2);
			Font font = new Font("Tahoma", fontSize);

			//  更新
			// DataGridView: row/header font equivalents
			gridView.DefaultCellStyle.Font = font;
			gridView.ColumnHeadersDefaultCellStyle.Font = font;

			// KeepSize list null safety
			keepSizeBand = keepSizeBand ?? new List<string>();

			// DevExpress Bands => DataGridView Columns
			foreach (DataGridViewColumn col in gridView.Columns)
			{
				if (col == null) continue;
				if (!col.Visible) continue;

				// DevExpress: if (!keepSizeBand.Contains(band.Name))
				if (!keepSizeBand.Contains(col.Name))
				{
					// DevExpress: dictBandWidth[band.Name]
					int originalWidth;
					if (dictBandWidth == null || !dictBandWidth.TryGetValue(col.Name, out originalWidth))
					{
						// if missing, treat current width as original
						originalWidth = col.Width;
					}

					int newWidth = (int)Math.Ceiling(originalWidth * rate / 100.0f);
					col.Width = newWidth;
				}
			}
			gridView.Invalidate();
		}

		public static void RepairPlanDrawCell(
			DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e
			, LongRepairPlanData obj
			, List<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn> lstColButton
			, DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn hisCol
			, bool isDisabled)
        {
			e.Handled = true;
			e.Appearance.DrawBackground(e.Cache, e.Bounds);
			e.Graphics.DrawLine(new Pen(Color.Red, 4), e.Bounds.Left, e.Bounds.Top, e.Bounds.Right, e.Bounds.Top);
			e.Graphics.DrawLine(new Pen(Color.Red, 4), e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
			e.Appearance.DrawString(e.Cache, e.DisplayText, e.Bounds);

			if (obj.Row == LongRepairPlanData.RowType.RepairPlan && lstColButton.Contains((DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn)e.Column))
			{
				var bgColor = Brushes.Silver;
				var textColor = Brushes.Gray;
				if (!isDisabled)
				{
					bgColor = new SolidBrush(Color.FromArgb(255, (byte)238, (byte)234, (byte)221));
					if (e.Column == hisCol && (obj.UpdateFlg == COMSKCommon.HAS_HISTORY_FLG_ON || obj.WorkUpdateReasonList.Count > 0))
					{
						bgColor = new SolidBrush(Color.FromArgb(255, (byte)203, (byte)0, (byte)0));
						textColor = Brushes.White;
					}
				}
				// Draw cell background
				e.Appearance.DrawBackground(e.Cache, e.Bounds);

				// Draw a button within the cell bounds
				Rectangle buttonRect = e.Bounds;
				buttonRect.Inflate(-2, -2); // Optional: adjust button size

				DevExpress.Utils.Drawing.GraphicsCache cache = e.Cache;
				cache.Graphics.FillRectangle(bgColor, buttonRect); // Button background color
				cache.Graphics.DrawRectangle(Pens.Gray, buttonRect); // Button border color

				// Draw button text
				StringFormat sf = new StringFormat
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				};
				e.Cache.Graphics.DrawString("開く", e.Appearance.Font, textColor, buttonRect, sf);
			}
		}

		public static void RestoreAllBanded(DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bandedGridView)
        {
			foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand band in bandedGridView.Bands)
			{
				if (!band.Visible) band.Visible = true;
				if (band.HasChildren)
				{
					// child level 1
					foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand cband in band.Children)
					{
						if (!cband.Visible) cband.Visible = true;
						if (cband.HasChildren)
						{
							// child level 2
							foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand cband2 in cband.Children)
							{
								if (!cband2.Visible) cband2.Visible = true;
							}
						}
					}
				}
			}
		}

		public static void DoScroll(coms.COMSK.ui.common.LongRepairGridView<LongRepairPlanData> gridView, MouseEventArgs e)
		{
			if (gridView == null) return;

			// Get the horizontal scroll bar of the GridControl
			// DataGridView には DevExpress の HScrollBar が無いので、HorizontalScrollingOffset を使う
			int current = gridView.HorizontalScrollingOffset;

			// DataGridView は Maximum/Minimum を直接持たないため、
			// Minimum=0, Maximum=maxPosRight として元のロジックを再現する
			int minimum = 0;
			int maximum = Math.Max(0, maxPosRight);

			// Scroll left if Delta is positive, right if Delta is negative
			if (e.Delta > 0) // Scroll left
			{
				//if (hScrollBar.Value - scrollAmount >= hScrollBar.Minimum)
				gridView.HorizontalScrollingOffset = Math.Max(minimum, current - scrollAmount);
			}
			else if (e.Delta < 0) // Scroll right
			{
				if ((current) <= maxPosRight)
				{
					gridView.HorizontalScrollingOffset = Math.Min(maximum, current + scrollAmount);
				}
				//if (hScrollBar.Value + scrollAmount <= hScrollBar.Value)
			}
		}

		public static int CalcZoomFactor(MouseEventArgs e, ref bool doZoom, int zoomFactor)
        {
			int newZoom = zoomFactor;
			// Increase or decrease zoom factor based on scroll direction
			if (e.Delta > 0)
			{
				if (zoomFactor < zoomMaxRate)
				{
					newZoom += zoomStep;
					doZoom = true;
				}
			}
			else if (e.Delta < 0)
			{
				if (zoomFactor > zoomMinRate)
				{
					newZoom -= zoomStep;
					doZoom = true;
				}
			}

			return newZoom;
		}
	}
}
