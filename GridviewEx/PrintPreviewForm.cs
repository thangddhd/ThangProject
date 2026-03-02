using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using PdfSharp.Drawing;

using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using coms.COMMON.Utility;

namespace coms.COMMON.ui
{
    public partial class PrintPreviewForm : MyForm
    {
        // Grid ソース
        public DataGridViewEx Grid { get; }

        private readonly PrintDocument printDoc;

        // フォント GDI+
        private readonly System.Drawing.Font normalFont = new System.Drawing.Font("Segoe UI", 9f);
        private readonly System.Drawing.Font headerFont = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Bold);

        // データソース
        private List<PrintCol> cols;
        // 出力の場合ボタンも出す
        private List<PrintCol> exportCols;
        private List<object> rows;

        // プレビュー中行
        private int currentRowIndex;

        // ヘッダー (theo GDI)
        private readonly int headerHeight = 28;

        // 全列　→　１パージ (横スケール)
        private float scaleFactor = 1f;

        // ページ数
        private int totalPages = 0;

        private bool isLandscape = false;
        private string pSize = "A4";

        // ===== PATCH: row height cache =====
        private class ColumnMeasureInfo
        {
            public float OneLineHeight;
            public int CharsPerLine;
            public bool IsDoubleByte;
        }
        private Dictionary<int, ColumnMeasureInfo> _columnMeasureCache = new Dictionary<int, ColumnMeasureInfo>();
        private Dictionary<int, float> _columnHeightAdjust = new Dictionary<int, float>();
        private bool _layoutDirty = true;
        private float ROW_HEIGHT_BUFFER = -12f;
        // ===== PATCH: page number =====
        private readonly System.Drawing.Font pageNumberFont = new System.Drawing.Font("Segoe UI", 8f);
        private const float PAGE_NUMBER_MARGIN = 6f;
        private const float PREVIEW_MIN_COLUMN_WIDTH = 70f;
        // ===== PATCH: cached StringFormat =====
        private readonly StringFormat _cellTextFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near,
            Trimming = StringTrimming.Word,
            FormatFlags = 0
        };

        private static readonly Pen ThinBorderPen =
            new Pen(System.Drawing.Color.Black, 1f)
            {
                Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
            };

        private readonly StringFormat _headerTextNoWrapFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None,
            FormatFlags =
                StringFormatFlags.NoWrap |
                StringFormatFlags.FitBlackBox
        };

        // ===== PATCH: export overlay =====
        private Panel _exportOverlay;
        private Label _exportOverlayLabel;
        private readonly HashSet<string> _ignoreFormatColumns;
        /// <summary>
        /// ignoreFormatColumns -> list of column names ignore format number (like company-code, kumiai-code ...)
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="ignoreFormatColumns"></param>
        public PrintPreviewForm(DataGridViewEx grid, HashSet<string> ignoreFormatColumns = null)
        {
            if (ignoreFormatColumns == null) ignoreFormatColumns = new HashSet<string>();

            InitializeComponent();

            Grid = grid ?? throw new ArgumentNullException(nameof(grid));

            _ignoreFormatColumns = ignoreFormatColumns;

            printDoc = new PrintDocument();
            printDoc.BeginPrint += PrintDoc_BeginPrint;
            printDoc.EndPrint += PrintDoc_EndPrint;
            printDoc.PrintPage += PrintDoc_PrintPage;

            BuildPrintData();

            previewControl.Document = printDoc;
            previewControl.AutoZoom = false;
            previewControl.Zoom = 1.0;
            previewControl.UseAntiAlias = true;
            previewControl.Dock = DockStyle.Fill;
            previewControl.Rows = 1;
            previewControl.Columns = 1;

            // フォーカスしてイベントハンドル
            this.Shown += (s, e) => previewControl.Focus();

            HookEvents();
        }

        #region Hook events

        private void HookEvents()
        {
            // Zoom +
            btnZoomIn.Click += (s, e) =>
            {
                previewControl.Zoom += 0.1;
            };

            // Zoom -
            btnZoomOut.Click += (s, e) =>
            {
                previewControl.Zoom -= 0.1;
                if (previewControl.Zoom < 0.1)
                    previewControl.Zoom = 0.1;
            };

            cboZoom.SelectedIndexChanged += (s, e) =>
            {
                if (cboZoom.SelectedItem == null) return;
                var text = cboZoom.SelectedItem.ToString().Trim().TrimEnd('%');
                if (int.TryParse(text, out var percent))
                {
                    previewControl.Zoom = percent / 100.0;
                }
            };

            // Print
            btnPrint.Click += (s, e) =>
            {
                using (var dlg = new PrintDialog())
                {
                    dlg.Document = printDoc;
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        currentRowIndex = 0;
                        printDoc.Print();
                    }
                }
            };

            // Export PDF
            btnExportPdf.Click += (s, e) => ExportPdfViaExcel();

            // Export Excel (Interop)
            //btnExportExcel.Click += (s, e) => ExportExcel_Interop();
            btnExportExcel.Click += (s, e) => ExportExcel_OpenXml_Raw();
            btnExportCSV.Click += (s, e) => ExportCsv();

            // Close
            btnClose.Click += (s, e) => Close();
        }

        #endregion

        #region BeginPrint / EndPrint (đếm số trang)

        private void PrintDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            // 毎回プレビューする場合最初ページ、トタルリセット
            totalPages = 0;
            currentRowIndex = 0;
        }

        private void PrintDoc_EndPrint(object sender, PrintEventArgs e)
        {

        }

        #endregion

        #region Chuẩn bị dữ liệu in

        private void BuildPrintData()
        {
            cols = new List<PrintCol>();
            exportCols = new List<PrintCol>();

            // 表示中のボタン列以外
            var visibleColumns = Grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible && !(c is DataGridViewButtonColumn))
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            foreach (var c in visibleColumns)
            {
                var prCol = new PrintCol
                {
                    Header = c.HeaderText,
                    Width = c.Width,
                    Property = c.DataPropertyName,
                    ColumnIndex = c.Index,   // Index for get cell
                    ColumnName = c.Name
                };
                exportCols.Add(prCol);
                if (!(c is DataGridViewButtonColumn))
                    cols.Add(prCol);
            }

            rows = new List<object>();
            foreach (DataGridViewRow r in Grid.Rows)
            {
                if (!r.IsNewRow)
                    rows.Add(r.DataBoundItem ?? r);
            }

            // 列すべて1ページピッタリ
            var page = printDoc.DefaultPageSettings;
            float pageWidth = page.Bounds.Width - page.Margins.Left - page.Margins.Right;

            float totalWidth = 0;
            foreach (var c in cols) totalWidth += c.Width;

            if (totalWidth <= 0)
            {
                scaleFactor = 1f;
            }
            else
            {
                scaleFactor = pageWidth / totalWidth;
                if (scaleFactor > 1f) scaleFactor = 1f;
            }

            currentRowIndex = 0;
        }

        #endregion

        #region PrintPage (preview & in)

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            totalPages++;

            // ===== 1. PREPARE =====
            int colCount = cols.Count;
            float pageWidth = e.MarginBounds.Width;

            // ===== 2. CHECK MIN WIDTH POSSIBILITY =====
            float minTotalWidth = colCount * PREVIEW_MIN_COLUMN_WIDTH;

            if (minTotalWidth > pageWidth)
            {
                DrawPreviewTooManyColumnsMessage(e.Graphics, e.MarginBounds);
                e.HasMorePages = false;
                currentRowIndex = 0;
                return;
            }

            // ===== 3. CALCULATE scaleExtra =====
            float originalTotalWidth = cols.Sum(c => c.Width);
            float originalScalableWidth =
                originalTotalWidth - colCount * PREVIEW_MIN_COLUMN_WIDTH;

            float scaleExtra =
                originalScalableWidth > 0
                    ? (pageWidth - minTotalWidth) / originalScalableWidth
                    : 1f;

            // ===== 4. CALIBRATE ROW HEIGHT CACHE (ONCE) =====
            if (_layoutDirty)
            {
                BuildColumnMeasureCache(e.Graphics);
                _layoutDirty = false;
            }

            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            // ===== 5. PAGE NUMBER =====
            DrawPageNumber(e.Graphics, e.MarginBounds);

            // ===== 6. HEADER =====
            DrawHeader(e.Graphics, x, ref y, scaleExtra);

            // ===== 7. DATA ROWS =====
            while (currentRowIndex < rows.Count)
            {
                var rowObj = rows[currentRowIndex];

                float rowHeight = 0f;

                // ---- calculate row height ----
                foreach (var col in cols)
                {
                    float w = GetPreviewColumnWidth(col, scaleExtra);
                    //string text = GetCellValue(rowObj, col)?.ToString() ?? "";
                    object rawValue = this.GetCellValue(rowObj, col);
                    string text =
                        this.IsCheckboxColumn(col)
                            ? this.ConvertToExcelText(rawValue, col)
                            : this.FormatCellText(rawValue, col);

                    var info = _columnMeasureCache[col.ColumnIndex];
                    int lineCount = CalculateLineCount(text, info);

                    float h = lineCount * info.OneLineHeight;

                    if (_columnHeightAdjust != null &&
                        _columnHeightAdjust.TryGetValue(col.ColumnIndex, out float adjust))
                    {
                        h += adjust;
                    }

                    if (h > rowHeight)
                        rowHeight = h;
                }

                if (currentRowIndex > 0)
                    rowHeight += ROW_HEIGHT_BUFFER;

                // ---- page break ----
                float nextY = y + rowHeight;
                if (nextY > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                // ---- draw cells ----
                float px = x;

                foreach (var col in cols)
                {
                    float w = GetPreviewColumnWidth(col, scaleExtra);

                    RectangleF rect = new RectangleF(px, y, w, rowHeight);

                    bool isFirstColumn = (col == cols[0]);
                    bool isFirstRow = (currentRowIndex == 0);

                    DrawThinBorder(e.Graphics, ThinBorderPen, rect, isFirstColumn, isFirstRow);

                    object rawValue = GetCellValue(rowObj, col);
                    if (this.IsCheckboxColumn(col))
                    {
                        bool checkedValue = this.IsChecked(rawValue);
                        this.DrawPreviewCheckbox(e.Graphics, rect, checkedValue);
                    }
                    else
                    {
                        string text = this.FormatCellText(rawValue, col);
                        if (!string.IsNullOrEmpty(text))
                        {
                            e.Graphics.DrawString(
                                text,
                                normalFont,
                                Brushes.Black,
                                rect,
                                _cellTextFormat
                            );
                        }
                    }

                    px += w;
                }

                y = nextY;
                currentRowIndex++;
            }

            // ===== 8. FINISH =====
            e.HasMorePages = false;
            currentRowIndex = 0;
        }

        private void DrawPreviewTooManyColumnsMessage(Graphics g, Rectangle bounds)
        {
            string msg = "列数がプレビューサイズを超えているため、プレビューできません。";

            using (var font = new System.Drawing.Font("MS UI Gothic", 14f, FontStyle.Bold))
            using (var brush = new SolidBrush(System.Drawing.Color.Black))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };

                g.DrawString(msg, font, brush, bounds, sf);
            }
        }

        private float GetPreviewColumnWidth(PrintCol col, float scaleExtra)
        {
            return
                PREVIEW_MIN_COLUMN_WIDTH +
                Math.Max(0, col.Width - PREVIEW_MIN_COLUMN_WIDTH) * scaleExtra;
        }

        /// <summary>
        /// border ダブルしないためにセル位置応じて対策違う
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="rect"></param>
        /// <param name="isFirstColumn"></param>
        /// <param name="isFirstRow"></param>
        private void DrawThinBorder(Graphics g, Pen pen, RectangleF rect, bool isFirstColumn, bool isFirstRow)
        {
            if (isFirstRow)
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);

            if (isFirstColumn)
                g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);

            // right
            g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);

            // bottom
            g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
        }

        private void DrawHeader(
            Graphics g,
            float startX,
            ref float y,
            float scaleExtra)
        {
            float x = startX;

            float totalWidth = 0f;
            foreach (var col in cols)
            {
                totalWidth += GetPreviewColumnWidth(col, scaleExtra);
            }

            RectangleF headerRect =
                new RectangleF(startX, y, totalWidth, headerHeight);

            using (var backBrush =
                new SolidBrush(System.Drawing.Color.LightGray))
            {
                g.FillRectangle(backBrush, headerRect);
            }

            foreach (var col in cols)
            {
                float w = GetPreviewColumnWidth(col, scaleExtra);

                RectangleF rect = new RectangleF(x, y, w, headerHeight);

                g.DrawRectangle(
                    Pens.Black,
                    rect.X,
                    rect.Y,
                    rect.Width,
                    rect.Height
                );

                // text
                g.DrawString(
                    col.Header ?? "",
                    headerFont,
                    Brushes.Black,
                    rect,
                    _headerTextNoWrapFormat
                );

                x += w;
            }

            y += headerHeight + 4f;
        }

        private object GetCellValue(object rowObj, PrintCol col)
        {
            if (rowObj == null) return null;

            // DataGridViewRowの場合
            if (rowObj is DataGridViewRow dgvRow)
            {
                if (col.ColumnIndex >= 0 && col.ColumnIndex < dgvRow.Cells.Count)
                    return dgvRow.Cells[col.ColumnIndex].Value;
                return null;
            }

            // オブジェクトの場合
            if (!string.IsNullOrEmpty(col.Property))
            {
                var prop = rowObj.GetType().GetProperty(col.Property);
                if (prop != null)
                    return prop.GetValue(rowObj, null);
            }

            return null;
        }

        #endregion

        #region Mouse wheel

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            var pt = previewControl.PointToClient(MousePosition);
            if (!previewControl.ClientRectangle.Contains(pt))
                return;

            if (totalPages <= 0)
                return;

            if (e.Delta < 0)
            {
                if (previewControl.StartPage < totalPages - 1)
                    previewControl.StartPage++;
            }
            else if (e.Delta > 0)
            {
                if (previewControl.StartPage > 0)
                    previewControl.StartPage--;
            }
        }

        #endregion

        #region Export PDF

        private void ExportPdfViaExcel()
        {
            bool exportSuccess = false;
            Exception exportError = null;
            string pdfPath = string.Empty;

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "PDF File (*.pdf)|*.pdf";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                ShowExportOverlay("Exporting PDF...");
                try
                {
                    pdfPath = dlg.FileName;

                    // temp　EXCEL作成
                    string tempExcel = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xlsx");

                    var excel = new Excel.Application();
                    excel.Visible = false;
                    excel.DisplayAlerts = false;

                    Excel.Workbook wb = excel.Workbooks.Add();
                    Excel.Worksheet ws = (Excel.Worksheet)wb.ActiveSheet;

                    // Font
                    string jpFont = "MS UI Gothic";

                    int rowIndex = 1;
                    int colIndex = 1;

                    // ============= HEADER =============
                    foreach (var c in cols)
                    {
                        var cell = (Excel.Range)ws.Cells[rowIndex, colIndex];
                        cell.Value = c.Header;
                        cell.Font.Bold = true;
                        cell.Font.Name = jpFont;
                        cell.WrapText = true;
                        cell.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        // Border
                        cell.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        cell.Borders.Weight = Excel.XlBorderWeight.xlHairline;

                        colIndex++;
                    }

                    // ============= DATA ROWS =============
                    rowIndex = 2;
                    foreach (var rowObj in rows)
                    {
                        colIndex = 1;

                        foreach (var c in cols)
                        {
                            var cell = (Excel.Range)ws.Cells[rowIndex, colIndex];
                            object value = GetCellValue(rowObj, c);

                            cell.Value = value?.ToString() ?? "";
                            cell.Font.Name = jpFont;
                            cell.WrapText = true;

                            // Border
                            cell.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            cell.Borders.Weight = Excel.XlBorderWeight.xlHairline;

                            colIndex++;
                        }
                        rowIndex++;
                    }

                    // ============= AUTOFIT =============
                    ws.Columns.AutoFit();
                    ws.Rows.AutoFit();

                    // ============= LƯU EXCEL TẠM =============
                    wb.SaveAs(tempExcel);

                    // ================= FIT ALL COLUMNS TO ONE PAGE WIDTH =================
                    ws.PageSetup.Zoom = false;            // 必須
                    ws.PageSetup.FitToPagesWide = 1;      // 横全列1ページ内
                    ws.PageSetup.FitToPagesTall = false;  // 縦自由

                    // margin đẹp hơn
                    ws.PageSetup.LeftMargin = excel.InchesToPoints(0.25);
                    ws.PageSetup.RightMargin = excel.InchesToPoints(0.25);
                    ws.PageSetup.TopMargin = excel.InchesToPoints(0.5);
                    ws.PageSetup.BottomMargin = excel.InchesToPoints(0.5);

                    // orientation by PaperSize form preview
                    ws.PageSetup.Orientation =
                        (printDoc.DefaultPageSettings.Landscape)
                        ? Excel.XlPageOrientation.xlLandscape
                        : Excel.XlPageOrientation.xlPortrait;

                    // ============= PDF =============
                    ws.ExportAsFixedFormat(
                        Excel.XlFixedFormatType.xlTypePDF,
                        pdfPath,
                        Excel.XlFixedFormatQuality.xlQualityStandard,
                        IncludeDocProperties: true,
                        IgnorePrintAreas: false,
                        OpenAfterPublish: false
                    );

                    wb.Close(false);
                    excel.Quit();

                    // Clean Excel COM objects
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                    exportSuccess = true;
                }
                catch(Exception ex)
                {
                    exportError = ex;
                }
            }

            HideExportOverlay();

            if (exportSuccess)
            {
                MessageBox.Show(this,
                    "PDF出力完了しました:\n" + pdfPath,
                    "PDF",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this,
                    "PDF出力に失敗しました。\n\n" + exportError?.Message,
                    "PDF",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Export Excel (Interop)

        private void ExportExcel_Interop()
        {
            bool exportSuccess = false;
            Exception exportError = null;

            try
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Excel File (*.xlsx)|*.xlsx";
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;

                    ShowExportOverlay("Exporting Excel...");

                    var excel = new Excel.Application();
                    excel.Visible = false;
                    excel.DisplayAlerts = false;

                    Excel.Workbook wb = excel.Workbooks.Add();
                    Excel.Worksheet ws = (Excel.Worksheet)wb.ActiveSheet;
                    ws.Cells.Font.Name = "MS UI Gothic";

                    int rowIndex = 1;
                    int colIndex = 1;

                    // Header
                    foreach (var c in cols)
                    {
                        ws.Cells[rowIndex, colIndex] = c.Header;
                        var cell = (Excel.Range)ws.Cells[rowIndex, colIndex];
                        cell.Font.Bold = true;
                        cell.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                        colIndex++;
                    }

                    // Data
                    rowIndex = 2;
                    foreach (var rowObj in rows)
                    {
                        colIndex = 1;
                        foreach (var c in cols)
                        {
                            object value = GetCellValue(rowObj, c);
                            ws.Cells[rowIndex, colIndex] = value;
                            colIndex++;
                        }
                        rowIndex++;
                    }

                    ws.Columns.AutoFit();

                    wb.SaveAs(dlg.FileName);
                    wb.Close();
                    excel.Quit();

                    exportSuccess = true;
                }
            }
            catch (Exception ex)
            {
                exportError = ex;
            }

            HideExportOverlay();

            if (exportSuccess)
            {
                MessageBox.Show(this, "Excel出力完了しました.", "Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "Excel出力エラー: " + exportError.Message, "Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helper class

        private class PrintCol
        {
            public string Header { get; set; }
            public string Property { get; set; }
            public int Width { get; set; }
            public int ColumnIndex { get; set; }
            public string ColumnName { get; set; }
        }

        #endregion

        private void btnPortrait_Click(object sender, EventArgs e)
        {
            if (this.isLandscape == false) return;

            this.isLandscape = false;
            printDoc.DefaultPageSettings.Landscape = false;

            // ページリセット
            currentRowIndex = 0;
            totalPages = 0;

            // スケール再計算
            BuildPrintData();
            _layoutDirty = true;

            previewControl.StartPage = 0;
            previewControl.InvalidatePreview();
            previewControl.Refresh();
            previewControl.Focus();
        }

        private void btnLandscape_Click(object sender, EventArgs e)
        {
            if (this.isLandscape) return;

            this.isLandscape = true;
            printDoc.DefaultPageSettings.Landscape = true;

            currentRowIndex = 0;
            totalPages = 0;

            BuildPrintData();
            _layoutDirty = true;

            previewControl.StartPage = 0;
            previewControl.InvalidatePreview();
            previewControl.Refresh();
            previewControl.Focus();
        }

        private void cboPaperSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPaperSize.SelectedItem == null) return;

            string paper = cboPaperSize.SelectedItem.ToString();
            if (paper == pSize) return;
            pSize = paper;

            PaperSize ps = null;

            if (paper == "A4")
            {
                // A4 = 210 × 297 mm
                ps = new PaperSize("A4", 827, 1169); // 単位: 1/100 inch
                previewControl.Zoom = 1.0;
                cboZoom.SelectedItem = "100%";
            }
            else if (paper == "A3")
            {
                // A3 = 297 × 420 mm
                ps = new PaperSize("A3", 1169, 1654);
                previewControl.Zoom = 0.75; // A3のデフォールト
                cboZoom.SelectedItem = "75%";
            }

            if (ps != null)
            {
                printDoc.DefaultPageSettings.PaperSize = ps;

                currentRowIndex = 0;
                totalPages = 0;

                // 紙サイズ応じて再画く
                BuildPrintData();
                _layoutDirty = true;

                previewControl.StartPage = 0;
                previewControl.InvalidatePreview();
                previewControl.Refresh();
                previewControl.Focus();
            }
        }

        private void PrintPreviewForm_Shown(object sender, EventArgs e)
        {
            cboZoom.SelectedItem = "100%";
            cboPaperSize.SelectedItem = "A4";

            // ===== PATCH: pre-create export overlay to avoid first-time flicker =====
            EnsureExportOverlay();
        }

        private float MeasureCellHeight(Graphics g, string text, float cellWidth, System.Drawing.Font font)
        {
            if (string.IsNullOrEmpty(text))
                return normalFont.Height + 4;

            // 改行あり前提行の高さを計算
            var stringFormat = new StringFormat(StringFormatFlags.LineLimit)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None,
                FormatFlags = 0
            };

            SizeF size = g.MeasureString(text, font, (int)cellWidth, stringFormat);

            return size.Height + 6;
        }

        private void DrawPdfWrappedText(XGraphics gfx, string text, XFont font, XRect rect)
        {
            double lineHeight = font.GetHeight();
            double x = rect.X;
            double y = rect.Y;
            double maxWidth = rect.Width;

            // Split Japanese or non-space text
            bool isJapanese = System.Text.Encoding.UTF8.GetByteCount(text) != text.Length;
            List<string> lines = new List<string>();

            if (isJapanese)
            {
                string line = "";
                foreach (char ch in text)
                {
                    string test = line + ch;
                    if (gfx.MeasureString(test, font).Width > maxWidth)
                    {
                        lines.Add(line);
                        line = ch.ToString();
                    }
                    else
                    {
                        line = test;
                    }
                }
                lines.Add(line);
            }
            else
            {
                string[] words = text.Split(' ');
                string line = "";
                foreach (var word in words)
                {
                    string test = (line.Length == 0) ? word : line + " " + word;

                    if (gfx.MeasureString(test, font).Width > maxWidth)
                    {
                        lines.Add(line);
                        line = word;
                    }
                    else
                    {
                        line = test;
                    }
                }
                lines.Add(line);
            }

            foreach (string line in lines)
            {
                gfx.DrawString(line, font, XBrushes.Black, new XRect(x + 2, y, maxWidth, lineHeight),
                    new XStringFormat
                    {
                        Alignment = XStringAlignment.Near,
                        LineAlignment = XLineAlignment.Near
                    });

                y += lineHeight;
            }
        }

        // ===== PATCH: build column measure cache =====
        private void BuildColumnMeasureCache(Graphics g)
        {
            _columnMeasureCache = new Dictionary<int, ColumnMeasureInfo>();

            foreach (var col in cols)
            {
                float cellWidth = col.Width * scaleFactor;

                // 1-line height (by DPI of printer)
                float oneLineHeight = g.MeasureString(
                    "Ag",
                    normalFont,
                    (int)cellWidth,
                    StringFormat.GenericTypographic
                ).Height;

                // detect double byte (sample first non-empty cell)
                bool isDoubleByte = false;
                foreach (var r in rows)
                {
                    var v = GetCellValue(r, col)?.ToString();
                    if (!string.IsNullOrEmpty(v))
                    {
                        isDoubleByte = System.Text.Encoding.UTF8.GetByteCount(v) != v.Length;
                        break;
                    }
                }

                string probe = isDoubleByte
                    ? new string('あ', 100)
                    : new string('A', 100);

                float probeWidth = g.MeasureString(
                    probe,
                    normalFont,
                    int.MaxValue,
                    StringFormat.GenericTypographic
                ).Width;

                float avgCharWidth = probeWidth / probe.Length;
                int charsPerLine = Math.Max(1, (int)(cellWidth / avgCharWidth));

                _columnMeasureCache[col.ColumnIndex] = new ColumnMeasureInfo
                {
                    OneLineHeight = oneLineHeight,
                    CharsPerLine = charsPerLine,
                    IsDoubleByte = isDoubleByte
                };
            }

            _columnHeightAdjust = new Dictionary<int, float>();
            if (rows.Count > 0)
            {
                var firstRow = rows[0];

                foreach (var col in cols)
                {
                    float cellWidth = col.Width * scaleFactor;
                    string text = GetCellValue(firstRow, col)?.ToString() ?? "";

                    float realHeight = MeasureCellHeight(g, text, cellWidth, normalFont);

                    var info = _columnMeasureCache[col.ColumnIndex];
                    int lineCount = CalculateLineCount(text, info);
                    float logicHeight = lineCount * info.OneLineHeight;

                    float delta = realHeight - logicHeight;
                    //if (delta < 0) delta = 0;
                    _columnHeightAdjust[col.ColumnIndex] = delta;
                }
            }
        }

        // ===== PATCH: calculate line count =====
        private int CalculateLineCount(string text, ColumnMeasureInfo info)
        {
            if (string.IsNullOrEmpty(text))
                return 1;

            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            int total = 0;

            foreach (var line in lines)
            {
                int len = info.IsDoubleByte
                    ? System.Text.Encoding.UTF8.GetByteCount(line) / 2
                    : line.Length;

                int count = (int)Math.Ceiling((double)len / info.CharsPerLine);
                total += Math.Max(1, count);
            }

            return total;
        }

        // ===== PATCH: draw page number =====
        private void DrawPageNumber(Graphics g, Rectangle marginBounds)
        {
            if (totalPages <= 0) return;

            string text = $"ページ　{totalPages}";

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            // TOP (center)
            RectangleF topRect = new RectangleF(
                marginBounds.Left,
                marginBounds.Top - pageNumberFont.Height - PAGE_NUMBER_MARGIN,
                marginBounds.Width,
                pageNumberFont.Height
            );

            g.DrawString(text, pageNumberFont, Brushes.Gray, topRect, sf);

            // BOTTOM (center)
            RectangleF bottomRect = new RectangleF(
                marginBounds.Left,
                marginBounds.Bottom + PAGE_NUMBER_MARGIN,
                marginBounds.Width,
                pageNumberFont.Height
            );

            g.DrawString(text, pageNumberFont, Brushes.Gray, bottomRect, sf);
        }

        // ===== PATCH: overlay creation does NOT set text =====
        private void EnsureExportOverlay()
        {
            if (_exportOverlay != null)
                return;

            _exportOverlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(120, System.Drawing.Color.Gray),
                Visible = false
            };

            _exportOverlayLabel = new Label
            {
                AutoSize = true,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 12f, FontStyle.Bold),
                BackColor = System.Drawing.Color.Transparent
            };

            _exportOverlay.Controls.Add(_exportOverlayLabel);
            Controls.Add(_exportOverlay);
            _exportOverlay.BringToFront();

            _exportOverlay.Resize += (s, e) =>
            {
                _exportOverlayLabel.Left =
                    (_exportOverlay.Width - _exportOverlayLabel.Width) / 2;
                _exportOverlayLabel.Top =
                    (_exportOverlay.Height - _exportOverlayLabel.Height) / 2;
            };
        }

        // ===== PATCH: dynamic overlay text =====
        private void ShowExportOverlay(string overlayText)
        {
            EnsureExportOverlay();

            _exportOverlayLabel.Text = overlayText;

            // recenter after text change
            _exportOverlayLabel.Left =
                (_exportOverlay.Width - _exportOverlayLabel.Width) / 2;
            _exportOverlayLabel.Top =
                (_exportOverlay.Height - _exportOverlayLabel.Height) / 2;

            _exportOverlay.Visible = true;
            _exportOverlay.BringToFront();
        }

        private void HideExportOverlay()
        {
            if (_exportOverlay != null)
                _exportOverlay.Visible = false;
        }

        private void ExportExcel_OpenXml_Raw()
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Excel File (*.xlsx)|*.xlsx";
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                string filePath = dlg.FileName;

                try
                {
                    using (SpreadsheetDocument doc =
                        SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
                    {
                        // ================= Workbook =================
                        WorkbookPart workbookPart = doc.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        // ================= Styles =================
                        WorkbookStylesPart stylesPart =
                            workbookPart.AddNewPart<WorkbookStylesPart>();
                        stylesPart.Stylesheet = CreateStylesheet();
                        stylesPart.Stylesheet.Save();

                        // ================= Worksheet =================
                        WorksheetPart worksheetPart =
                            workbookPart.AddNewPart<WorksheetPart>();

                        SheetData sheetData = new SheetData();
                        Worksheet worksheet = new Worksheet();

                        // ================= Columns (GRID WIDTH – NOT PREVIEW) =================
                        Columns columns = new Columns();

                        for (int i = 0; i < cols.Count; i++)
                        {
                            // DataGridView width (pixel) → Excel column width
                            double excelWidth =
                                Math.Max(3.0, cols[i].Width / 7.0); // 7 = empirical Excel factor

                            columns.Append(new Column
                            {
                                Min = (uint)(i + 1),
                                Max = (uint)(i + 1),
                                Width = excelWidth,
                                CustomWidth = true
                            });
                        }

                        worksheet.Append(columns);
                        worksheet.Append(sheetData);
                        worksheetPart.Worksheet = worksheet;

                        // ================= Sheets =================
                        Sheets sheets =
                            workbookPart.Workbook.AppendChild(new Sheets());

                        sheets.Append(new Sheet
                        {
                            Id = workbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = "Sheet1"
                        });

                        // ================= Header =================
                        Row headerRow = new Row();
                        foreach (var c in cols)
                        {
                            headerRow.Append(new Cell
                            {
                                DataType = CellValues.InlineString,
                                StyleIndex = 1, // header wrap
                                InlineString = new InlineString(
                                    new Text
                                    {
                                        Text = c.Header ?? "",
                                        Space = SpaceProcessingModeValues.Preserve
                                    })
                            });
                        }
                        sheetData.Append(headerRow);

                        // ================= Data =================
                        foreach (var rowObj in rows)
                        {
                            Row row = new Row();

                            foreach (var c in cols)
                            {
                                object rawValue = GetCellValue(rowObj, c);

                                Cell cell;

                                if (IsCheckboxColumn(c))
                                {
                                    string text = ConvertToExcelText(rawValue, c);

                                    cell = new Cell
                                    {
                                        DataType = CellValues.InlineString,
                                        StyleIndex = 3,
                                        InlineString = new InlineString(new Text(text ?? ""))
                                    };
                                }
                                else
                                {
                                    cell = CreateCell(rawValue, c);
                                }

                                row.Append(cell);
                            }

                            // ❗ no set Row.Height
                            // Excel auto height when file was opened
                            sheetData.Append(row);
                        }

                        workbookPart.Workbook.Save();
                    }

                    MessageBox.Show(this,
                        "OpenXML Excel 出力完了しました。",
                        "OpenXML",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        "OpenXML Excel 出力エラー:\n" + ex.Message,
                        "OpenXML",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private Stylesheet CreateStylesheet()
        {
            var numberingFormats = new NumberingFormats(
                new NumberingFormat   // 164 decimal
                {
                    NumberFormatId = 164,
                    FormatCode = "#,##0.00"
                },
                new NumberingFormat   // 165 integer
                {
                    NumberFormatId = 165,
                    FormatCode = "#,##0"
                },
                new NumberingFormat   // 166 date
                {
                    NumberFormatId = 166,
                    FormatCode = "yyyy/mm/dd"
                }
            );
            numberingFormats.Count = 3;

            // ===== FONTS =====
            var fonts = new Fonts(
                new DocumentFormat.OpenXml.Spreadsheet.Font(), // 0 default

                new DocumentFormat.OpenXml.Spreadsheet.Font(   // 1 header
                    new Bold(),
                    new FontSize { Val = 9 },
                    new FontName { Val = "Segoe UI" }
                ),

                new DocumentFormat.OpenXml.Spreadsheet.Font(   // 2 normal
                    new FontSize { Val = 9 },
                    new FontName { Val = "Segoe UI" }
                )
            );
            fonts.Count = 3;

            // ===== FILLS =====
            var fills = new Fills(
                new Fill(new PatternFill { PatternType = PatternValues.None }),
                new Fill(new PatternFill { PatternType = PatternValues.Gray125 }),
                new Fill(
                    new PatternFill(
                        new ForegroundColor { Rgb = HexBinaryValue.FromString("D9D9D9") }
                    )
                    { PatternType = PatternValues.Solid }
                )
            );
            fills.Count = 3;

            // ===== BORDERS =====
            var borders = new Borders(
                new Border(), // 0 default
                new Border(   // 1 thin
                    new LeftBorder { Style = BorderStyleValues.Thin },
                    new RightBorder { Style = BorderStyleValues.Thin },
                    new TopBorder { Style = BorderStyleValues.Thin },
                    new BottomBorder { Style = BorderStyleValues.Thin }
                )
            );
            borders.Count = 2;

            // ===== CELL FORMATS =====
            var cellFormats = new CellFormats(
                new CellFormat(), // 0 default

                new CellFormat    // 1 header (wrap)
                {
                    FontId = 1,
                    FillId = 2,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyFill = true,
                    ApplyBorder = true,
                    Alignment = new Alignment
                    {
                        Horizontal = HorizontalAlignmentValues.Left,
                        Vertical = VerticalAlignmentValues.Center,
                        WrapText = true
                    }
                },

                new CellFormat    // 2 normal NO WRAP (🔥 quan trọng)
                {
                    FontId = 2,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyBorder = true,
                    Alignment = new Alignment
                    {
                        Horizontal = HorizontalAlignmentValues.Left,
                        Vertical = VerticalAlignmentValues.Top,
                        WrapText = false
                    }
                },

                new CellFormat    // 3 normal WRAP
                {
                    FontId = 2,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyBorder = true,
                    Alignment = new Alignment
                    {
                        Horizontal = HorizontalAlignmentValues.Left,
                        Vertical = VerticalAlignmentValues.Top,
                        WrapText = true
                    }
                },

                new CellFormat    // 4 INTEGER
                {
                    FontId = 2,
                    BorderId = 1,
                    NumberFormatId = 165,
                    ApplyNumberFormat = true,
                    ApplyFont = true,
                    ApplyBorder = true,
                    Alignment = new Alignment
                    {
                        Horizontal = HorizontalAlignmentValues.Right,
                        Vertical = VerticalAlignmentValues.Top
                    }
                },

                new CellFormat    // 5 DECIMAL
                {
                    FontId = 2,
                    BorderId = 1,
                    NumberFormatId = 164,
                    ApplyNumberFormat = true,
                    ApplyFont = true,
                    ApplyBorder = true,
                    Alignment = new Alignment
                    {
                        Horizontal = HorizontalAlignmentValues.Right,
                        Vertical = VerticalAlignmentValues.Top
                    }
                },

                new CellFormat    // 6 DATE
                {
                    FontId = 2,
                    BorderId = 1,
                    NumberFormatId = 166,
                    ApplyNumberFormat = true,
                    ApplyFont = true,
                    ApplyBorder = true,
                    Alignment = new Alignment
                    {
                        Horizontal = HorizontalAlignmentValues.Left,
                        Vertical = VerticalAlignmentValues.Top
                    }
                },

                new CellFormat    // 7 NUMERIC GENERAL (no format)
                {
                    FontId = 2,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyBorder = true,
                    Alignment = new Alignment
                    {
                        Horizontal = HorizontalAlignmentValues.Right,
                        Vertical = VerticalAlignmentValues.Top
                    }
                }
            );
            cellFormats.Count = 8;

            return new Stylesheet(numberingFormats, fonts, fills, borders, cellFormats);
        }

        private string ConvertToExcelText(object value, PrintCol col)
        {
            var isCheckbox = this.IsCheckboxColumn(col);
            if (isCheckbox)
            {
                var isChecked = this.IsChecked(value);
                // Fake checkbox
                return isChecked ? "☑" : "☐";
            }
            // Normal column
            return value?.ToString() ?? "";
        }

        private void DrawPreviewCheckbox(Graphics g, RectangleF cellRect, bool isChecked)
        {
            // Checkbox size (fixed, independent of column width)
            float boxSize = 10f;

            // Center checkbox in cell
            float x = cellRect.Left + (cellRect.Width - boxSize) / 2f;
            float y = cellRect.Top + (cellRect.Height - boxSize) / 2f;

            RectangleF box = new RectangleF(x, y, boxSize, boxSize);

            // Draw box
            g.DrawRectangle(Pens.Black, box.X, box.Y, box.Width, box.Height);

            if (isChecked)
            {
                // Simple check mark (✔)
                using (var pen = new Pen(System.Drawing.Color.Black, 1.5f))
                {
                    g.DrawLine(pen,
                        box.Left + 2, box.Top + box.Height / 2,
                        box.Left + box.Width / 2, box.Bottom - 2);

                    g.DrawLine(pen,
                        box.Left + box.Width / 2, box.Bottom - 2,
                        box.Right - 2, box.Top + 2);
                }
            }
        }

        private bool IsCheckboxColumn(PrintCol col)
        {
            return col.ColumnIndex >= 0
                && col.ColumnIndex < Grid.Columns.Count
                && Grid.Columns[col.ColumnIndex] is DataGridViewCheckBoxColumn;
        }

        private bool IsChecked(object value)
        {
            bool isChecked = false;

            if (value is bool b)
                isChecked = b;
            else if (value != null)
            {
                // handle 0/1, "true"/"false"
                bool.TryParse(value.ToString(), out isChecked);
            }

            return isChecked;
        }

        private string FormatCellText(object value, PrintCol col)
        {
            if (value == null)
                return string.Empty;

            // Checkbox handled elsewhere
            if (IsCheckboxColumn(col))
                return string.Empty;

            var dgvCol = Grid.Columns[col.ColumnIndex];

            bool isNumeric = false;
            bool isDate = false;
            string format = string.Empty;
            Type underlyingType = null;

            Type valType = dgvCol.ValueType ?? value.GetType();
            if (valType != null)
            {
                underlyingType = Nullable.GetUnderlyingType(valType) ?? valType;
                format = dgvCol.DefaultCellStyle?.Format;

                isNumeric = new[]
                {
                    typeof(int), typeof(long),
                    typeof(float), typeof(double), typeof(decimal)
                }.Contains(underlyingType);

                isDate = underlyingType == typeof(DateTime);
            }

            // Ignore numeric formatting if column in ignore list
            if (isNumeric && _ignoreFormatColumns.Contains(col.ColumnName))
                return value.ToString();

            // Default format
            if (string.IsNullOrEmpty(format))
            {
                if (isNumeric)
                {
                    if (underlyingType == typeof(int) || underlyingType == typeof(long))
                        format = "N0";
                    else
                        format = "N2";
                }
                else if (isDate)
                {
                    format = "yyyy/MM/dd";
                }
            }

            // Apply format
            if (!string.IsNullOrEmpty(format))
            {
                if (isNumeric)
                {
                    if (double.TryParse(value.ToString(), out double num))
                        return num.ToString(format);
                    return string.Empty;
                }
                else if (isDate)
                {
                    if (value is DateTime dt && dt > DateTime.MinValue)
                        return dt.ToString(format);
                    return string.Empty;
                }
            }

            return value.ToString();
        }

        private Cell CreateCell(object rawValue, PrintCol col)
        {
            if (rawValue == null)
            {
                return new Cell
                {
                    DataType = CellValues.InlineString,
                    StyleIndex = 3,   // style có border
                    InlineString = new InlineString(new Text(""))
                };
            }

            var dgvCol = Grid.Columns[col.ColumnIndex];

            Type valType = dgvCol.ValueType ?? rawValue.GetType();
            Type underlying = Nullable.GetUnderlyingType(valType) ?? valType;

            bool isNumberMinValue = false;
            bool isNumeric = Number.IsNumeric(rawValue, underlying, ref isNumberMinValue);

            bool isDate = underlying == typeof(DateTime);

            // ===== NUMBER =====
            if (isNumeric)
            {
                uint style;
                if (underlying == typeof(int) || underlying == typeof(long))
                    style = 4;     // integer
                else
                    style = 5;     // decimal

                string formatedVal = string.Empty;
                if (_ignoreFormatColumns.Contains(col.ColumnName))
                {
                    style = 7;
                    formatedVal = rawValue.ToString();
                }
                else if (!isNumberMinValue && double.TryParse(rawValue.ToString(), out double num))
                {
                    formatedVal = num.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                return new Cell
                {
                    DataType = CellValues.Number,
                    CellValue = new CellValue(formatedVal),
                    StyleIndex = style // number style
                };
            }

            // ===== DATE =====
            if (isDate)
            {
                if (rawValue is DateTime dt && dt > DateTime.MinValue)
                {
                    return new Cell
                    {
                        DataType = CellValues.Number,
                        CellValue = new CellValue(
                        dt.ToOADate().ToString(System.Globalization.CultureInfo.InvariantCulture)
                    ),
                        StyleIndex = 6 // date style
                    };
                }
                return new Cell
                {
                    DataType = CellValues.InlineString,
                    StyleIndex = 3,
                    InlineString = new InlineString(
                        new Text("")
                    )
                };
            }

            // ===== TEXT =====
            return new Cell
            {
                DataType = CellValues.InlineString,
                StyleIndex = 3,
                InlineString = new InlineString(
                    new Text(rawValue.ToString() ?? "")
                )
            };
        }

        private void ExportCsv()
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV File (*.csv)|*.csv";
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    using (var writer = new StreamWriter(
                        dlg.FileName,
                        false,
                        System.Text.Encoding.GetEncoding(932))) // Shift-JIS (CP932)
                    {
                        // ===== HEADER =====
                        writer.WriteLine(string.Join(",",
                            cols.Select(c => TextUtility.CsvEscape(c.Header ?? ""))));

                        // ===== DATA =====
                        foreach (var rowObj in rows)
                        {
                            var values = new List<string>();

                            foreach (var c in cols)
                            {
                                object rawValue = GetCellValue(rowObj, c);

                                string text;

                                // CHECKBOX → space
                                if (IsCheckboxColumn(c))
                                {
                                    text = " ";
                                }
                                else
                                {
                                    text = FormatCellText(rawValue, c) ?? "";
                                }

                                values.Add(TextUtility.CsvEscape(text));
                            }

                            writer.WriteLine(string.Join(",", values));
                        }
                    }

                    MessageBox.Show(this,
                        "CSV出力完了しました。",
                        "CSV",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        "CSV出力エラー:\n" + ex.Message,
                        "CSV",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
