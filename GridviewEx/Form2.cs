using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using coms.COMMON.ui;
using coms.COMSK.ui.common;

namespace GridviewEx
{
    public partial class Form2 : coms.MyForm
    {
        private LongRepairGridView<testObj> _grid;
        private BindingSource _bs;
        int _yearStart = 2010;

        public Form2()
        {
            InitializeComponent();
            BuildGrid();
            BindData();
        }

        private void BuildGrid()
        {
            _grid = new LongRepairGridView<testObj>();
            _grid.Dock = DockStyle.Fill;

            // Important for manual columns
            _grid.AutoGenerateColumns = false;

            // Optional baseline settings
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.RowHeadersVisible = false;
            _grid.DefaultCellStyle.BackColor = Color.White;
            _grid.DefaultCellStyle.ForeColor = Color.Black;
            _grid.BackgroundColor = Color.White;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Add columns exactly like your snippet
            AddColumns2(_grid);

            // Configure merge behavior
            _grid.SetVerticalMergeColumns(new[] {
                "bgcolConstructionType", 
                "bgcolConstructionItem", 
                "bgcolConstructionCategory", 
                "bgcolConstructionPosition",
                "bgcolConstructionRegion",
                "bgcolConstructionSpecification",
                "bgcolConstructionDivision",
                "bgcolRepairConstructionContent"
            });
            _grid.VerticalMergeProvider = new SimpleTestObjMergeProvider();

            // Left columns list (for calc-row horizontal merge) - not used for testObj unless you set IsCalcRow
            _grid.SetLeftColumnNames(new[] {
                 "bgcolConstructionType",
                "bgcolConstructionItem",
                "bgcolConstructionCategory",
                "bgcolConstructionPosition",
                "bgcolConstructionRegion",
                "bgcolConstructionSpecification",
                "bgcolConstructionDivision",
                "bgcolRepairConstructionContent"
            });

            // No calc rows in testObj by default, so keep null (or set false)
            _grid.IsCalcRow = m => m != null && !string.IsNullOrEmpty(m.RowType) && "T1,T2,T3,T4".Contains(m.RowType);

            _grid.CanDragCell = (g, rowIndex, col, model) =>
            {
                // old behavior: only RepairPlan rows + column.Tag must match
                if (model == null) return false;
                if (model.Row != "BBB") return false;
                return (col.Tag as string) == "AAA";
            };

            // Drag completed event
            _grid.RowCellsDragCompleted += Grid_RowCellsDragCompleted;

            _grid.CellStyleNeeded += new System.EventHandler<coms.COMMON.ui.ReserveCellStyleNeededEventArgs>(this.Grid_CellStyleNeeded);

            // freeze
            FreezeLeftColumns();

            // Example header layout (2 rows)
            _grid.SetHeaderLayout(BuildHeaderLikeImage_Correct2());

            // Add to form
            this.splitContainer1.Panel2.Controls.Add(_grid);

            // If you want it below existing controls, use a panel instead of Controls.Add directly.
        }

        private void Grid_CellStyleNeeded(object sender, coms.COMMON.ui.ReserveCellStyleNeededEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.BackColor = Color.Gray;
            }
            else if (e.ColumnIndex == 1)
            {
                e.BackColor = Color.Blue;
            }
        }

        private void AddColumns2(DataGridView grid)
        {
            grid.Columns.Clear();

            grid.Columns.Add(CreateCol("bgcolConstructionType", "ConstructionTypeName", "工事分類", 60));
            grid.Columns.Add(CreateCol("bgcolConstructionItem", "ConstructionItemName", "工事項目", 90));
            grid.Columns.Add(CreateCol("bgcolConstructionCategory", "ConstructionCategoryName", "工事種別", 90));
            grid.Columns.Add(CreateCol("bgcolConstructionPosition", "ConstructionPositionName", "位置", 90));
            grid.Columns.Add(CreateCol("bgcolConstructionRegion", "ConstructionRegionName", "部位", 90));
            grid.Columns.Add(CreateCol("bgcolConstructionSpecification", "ConstructionSpecificationName", "仕様", 90));
            grid.Columns.Add(CreateCol("bgcolConstructionDivision", "ConstructionDivisionName", "工事区分", 90));
            grid.Columns.Add(CreateCol("bgcolRepairConstructionContent", "RepairConstructionContentName", "(修繕工事内容)", 100));

            grid.Columns.Add(CreateCol("bgcolRepairPeriod", "Cycle", "周期", 40));
            grid.Columns.Add(CreateCol("bgcolRepairPlan", null, "修繕計画", 40));

            grid.Columns.Add(CreateCol("bgcolHistory", null, "変更履歴", 36));
            grid.Columns.Add(CreateCol("bgcolRegister", null, "登録", 40));

            grid.Columns.Add(CreateCol("bgcolAmount", "Amount", "数量", 41));
            grid.Columns.Add(CreateCol("bgcolUnit", "UnitName", "単位", 40));

            grid.Columns.Add(CreateCol("bgcolDatumYear", "DatumYear", "周期起算年", 49));
            grid.Columns.Add(CreateCol("bgcolEffectedYear", "ResultYear", "修繕実施年度", 75));

            this.CreateColYearAll(grid, _yearStart);

            grid.Columns.Add(CreateCol("bgcolSubTotal", "SubTotal", "小計", 75));
        }

        private DataGridViewColumn CreateCol(string name, string field, string caption, int width)
        {
            return new DataGridViewTextBoxColumn()
            {
                Name = name,
                DataPropertyName = field,
                HeaderText = caption,
                Width = width
            };
        }

        private void AddColumns(DataGridView grid)
        {
            // Column1
            var Column1 = new DataGridViewTextBoxColumn();
            Column1.DataPropertyName = "Column1";
            Column1.HeaderText = "Column1";
            Column1.Name = "Column1";
            Column1.Width = 120;

            // Column2
            var Column2 = new DataGridViewTextBoxColumn();
            Column2.DataPropertyName = "Column2";
            Column2.HeaderText = "Column2";
            Column2.Name = "Column2";
            Column2.Width = 120;

            // Column3
            var Column3 = new DataGridViewTextBoxColumn();
            Column3.DataPropertyName = "Column3";
            Column3.HeaderText = "Column3";
            Column3.Name = "Column3";
            Column3.Width = 120;

            // Column4
            var Column4 = new DataGridViewTextBoxColumn();
            Column4.DataPropertyName = "Column4";
            Column4.HeaderText = "Column4";
            Column4.Name = "Column4";
            Column4.Width = 120;

            // Y_0
            var Y_0 = new DataGridViewTextBoxColumn();
            Y_0.DataPropertyName = "Y_0";
            Y_0.HeaderText = "Y_0";
            Y_0.Name = "Y_0";
            Y_0.Width = 220;

            // Y_1
            var Y_1 = new DataGridViewTextBoxColumn();
            Y_1.DataPropertyName = "Y_1";
            Y_1.HeaderText = "Y_1";
            Y_1.Name = "Y_1";
            Y_1.Width = 220;

            // Y_2
            var Y_2 = new DataGridViewTextBoxColumn();
            Y_2.DataPropertyName = "Y_2";
            Y_2.HeaderText = "Y_2";
            Y_2.Name = "Y_2";
            Y_2.Width = 220;

            // Y_3
            var Y_3 = new DataGridViewTextBoxColumn();
            Y_3.DataPropertyName = "Y_3";
            Y_3.HeaderText = "Y_3";
            Y_3.Name = "Y_3";
            Y_3.Width = 220;

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                Column1, Column2, Column3, Column4,
                Y_0, Y_1, Y_2, Y_3
            });
        }

        private HeaderBandLayout BuildHeaderLikeImage_Correct2()
        {
            var layout = new HeaderBandLayout();
            layout.HeaderRowCount = 3;
            layout.HeaderRowHeight = 22;

            // ===== Row 0 (full height columns)
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolSubTotal", "小計", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, new[] { "bgcolConstructionType", "bgcolConstructionItem" }, "工事分類 工事項目", true));
            //layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionItem", "工事項目", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionCategory", "工事種別", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionPosition", "位置", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionRegion", "部位", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionSpecification", "仕様", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionDivision", "工事区分", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolRepairConstructionContent", "(修繕工事内容)", true));

            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolRepairPeriod", "周期", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolRepairPlan", "修繕\n基準", true));

            // ===== 変更理由 (2 columns)
            layout.Cells.Add(MakeHeaderCell(0, 1,
                new[] { "bgcolHistory", "bgcolRegister" }, "変更理由", true));

            layout.Cells.Add(MakeHeaderCell(1, 2, "bgcolHistory", "参照", false));
            layout.Cells.Add(MakeHeaderCell(1, 2, "bgcolRegister", "登録", false));

            // ===== normal columns
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolAmount", "数量", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolUnit", "単位", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolDatumYear", "周期\n起算年", true));


            // ===== 築年 (nested 3 level)
            layout.Cells.Add(MakeHeaderCell(0, 1,
                new[] { "bgcolEffectedYear" }, "築年", true));

            layout.Cells.Add(MakeHeaderCell(1, 1,
                new[] { "bgcolEffectedYear" }, "会計期", false));

            layout.Cells.Add(MakeHeaderCell(2, 1,
                "bgcolEffectedYear", "会計年度", false));


            // ===== subtotal
            this.CreateBandedYearAll(ref layout, _yearStart);

            return layout;
        }

        private void CreateBandedYear(ref HeaderBandLayout layout, string colName, string yearIdx, string termIdx, string year)
        {
            // Row 0: "築年" spans all year columns
            layout.Cells.Add(MakeHeaderCell(0, 1, new[] { colName }, yearIdx, true));
            // Row 1: "42年目", "43年目", "44年目"
            layout.Cells.Add(MakeHeaderCell(1, 1, colName, termIdx, false));
            // Row 2: two-line text per year column: "42期" + "2009年"
            layout.Cells.Add(MakeHeaderCell(2, 1, colName, year, false));
        }

        private void CreateBandedYearAll(ref HeaderBandLayout layout, int yearStart, int count = 60)
        {
            for (int i = 0; i < count; i++)
            {
                int year = yearStart + i;
                string colName = "Y_" + year.ToString();

                string yearIdx = (i + 1).ToString() + "年目";
                string termIdx = (i + 1).ToString() + "期";
                string yearText = year.ToString() + "年";

                this.CreateBandedYear(ref layout, colName, yearIdx, termIdx, yearText);
            }
        }

        private void CreateColYearAll(DataGridView grid, int yearStart, int count = 60)
        {
            const string YEAR_TAG = "AAA"; // or COMSKCommon.TAG_DRAGGABLE_CELL
            const string SUBTOTAL_COL = "bgcolSubTotal";

            // Find where subtotal currently is (by DisplayIndex)
            int subTotalDisplayIndex = grid.Columns.Contains(SUBTOTAL_COL)
                ? grid.Columns[SUBTOTAL_COL].DisplayIndex
                : grid.Columns.Count;

            // 1) remove existing year columns
            var remove = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn c in grid.Columns)
            {
                if ((c.Tag as string) == YEAR_TAG)
                    remove.Add(c);
            }
            foreach (var c in remove)
                grid.Columns.Remove(c);

            // subtotal display index may shift after removals, recompute safely
            subTotalDisplayIndex = grid.Columns.Contains(SUBTOTAL_COL)
                ? grid.Columns[SUBTOTAL_COL].DisplayIndex
                : grid.Columns.Count;

            // 2) add new year columns
            var newYearCols = new List<DataGridViewColumn>();
            for (int i = 0; i < count; i++)
            {
                int year = yearStart + i;
                string colName = "Y_" + year;

                var col = CreateCol(colName, colName, "yearData", 120);
                col.Tag = YEAR_TAG;

                grid.Columns.Add(col);
                newYearCols.Add(col);
            }

            // 3) force the year columns to sit immediately before subtotal, preserving order
            // Example: if subtotal display index is 15, years become 15..74, subtotal becomes 75.
            int di = subTotalDisplayIndex;
            foreach (var c in newYearCols)
            {
                c.DisplayIndex = di++;
            }

            // ensure subtotal is last among that group
            if (grid.Columns.Contains(SUBTOTAL_COL))
                grid.Columns[SUBTOTAL_COL].DisplayIndex = di;
        }

        private HeaderBandLayout BuildHeaderLikeImage_Correct()
        {
            var layout = new HeaderBandLayout();
            layout.HeaderRowCount = 3;
            layout.HeaderRowHeight = 22;

            // Row 0: Column1 & Column2 span 3 rows
            layout.Cells.Add(MakeHeaderCell(0, 3, "Column1", "周期", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "Column2", "修繕\n基準", true));

            // Row 0: "変更理由" spans Column3 + Column4
            layout.Cells.Add(MakeHeaderCell(0, 1, new[] { "Column3", "Column4" }, "変更理由", true));

            // Row 1: Column3/4 span 2 rows (rows 1-2)
            layout.Cells.Add(MakeHeaderCell(1, 2, "Column3", "参照", false));
            layout.Cells.Add(MakeHeaderCell(1, 2, "Column4", "登録", false));

            // Row 0: "築年" spans all year columns
            layout.Cells.Add(MakeHeaderCell(0, 1, new[] { "Y_0" }, "築年1", true));
            layout.Cells.Add(MakeHeaderCell(0, 1, new[] { "Y_1" }, "築年2", true));
            layout.Cells.Add(MakeHeaderCell(0, 1, new[] { "Y_2" }, "築年3", true));
            layout.Cells.Add(MakeHeaderCell(0, 1, new[] { "Y_3" }, "築年4", true));

            // Row 1: "42年目", "43年目", "44年目"
            layout.Cells.Add(MakeHeaderCell(1, 1, "Y_0", "42年目", false));
            layout.Cells.Add(MakeHeaderCell(1, 1, "Y_1", "43年目", false));
            layout.Cells.Add(MakeHeaderCell(1, 1, "Y_2", "44年目", false));
            layout.Cells.Add(MakeHeaderCell(1, 1, "Y_3", "44年目", false));

            // Row 2: two-line text per year column: "42期" + "2009年"
            layout.Cells.Add(MakeHeaderCell(2, 1, "Y_0", "2009年", false));
            layout.Cells.Add(MakeHeaderCell(2, 1, "Y_1", "2010年", false));
            layout.Cells.Add(MakeHeaderCell(2, 1, "Y_2", "2011年", false));
            layout.Cells.Add(MakeHeaderCell(2, 1, "Y_3", "2011年", false));

            return layout;
        }

        private HeaderBandCellByName MakeHeaderCell(int bandRow, int bandRowSpan, string colName, string text, bool isTopBand)
        {
            return MakeHeaderCell(bandRow, bandRowSpan, new[] { colName }, text, isTopBand);
        }

        private HeaderBandCellByName MakeHeaderCell(int bandRow, int bandRowSpan, string[] colNames, string text, bool isTopBand)
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

        private void BindData()
        {
            var list = new BindingList<testObj>();
            var obj = new testObj("A", "A1", "X", "K", 10, 40, 20, 30);
            obj.ConstructionTypeName = "仮設";
            list.Add(obj);
            obj = new testObj("A", "A1", "X", "K", 11, 41, 21, 31);
            obj.ConstructionTypeName = "仮設";
            list.Add(obj);
            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "仮設";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "仮設";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "仮設";
            list.Add(obj);

            obj = new testObj("A", "A1", "X", "K", 10, 40, 20, 30);
            obj.ConstructionTypeName = "仮設";
            list.Add(obj);
            obj = new testObj("A", "A1", "X", "K", 11, 41, 21, 31);
            obj.ConstructionTypeName = "建築";
            list.Add(obj);
            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "建築";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "建築";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "建築";
            list.Add(obj);

            obj = new testObj("A", "A1", "X", "K", 10, 40, 20, 30);
            obj.ConstructionTypeName = "建築";
            list.Add(obj);
            obj = new testObj("A", "A1", "X", "K", 11, 41, 21, 31);
            obj.ConstructionTypeName = "建築";
            list.Add(obj);
            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "建築";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "設備";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "設備";
            list.Add(obj);

            obj = new testObj("A", "A1", "X", "K", 10, 40, 20, 30);
            obj.ConstructionTypeName = "設備";
            list.Add(obj);
            obj = new testObj("A", "A1", "X", "K", 11, 41, 21, 31);
            obj.ConstructionTypeName = "設備";
            list.Add(obj);
            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "設備";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "設備";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "設備";
            list.Add(obj);

            obj = new testObj("A", "A1", "X", "K", 10, 40, 20, 30);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("A", "A1", "X", "K", 11, 41, 21, 31);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);

            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);

            obj = new testObj("A", "A1", "X", "K", 10, 40, 20, 30);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("A", "A1", "X", "K", 11, 41, 21, 31);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);

            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.ConstructionTypeName = "その他";
            list.Add(obj);

            obj = new testObj("A", "A1", "X", "K", 10, 40, 20, 30);
            list.Add(obj);

            obj = new testObj("A", "A1", "X", "K", 11, 41, 21, 31);
            obj.RowType = "T1";
            list.Add(obj);
            obj = new testObj("A", "A1", "Y", "K", 12, 42, 22, 32);
            obj.RowType = "T2";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "K", 13, 43, 23, 33);
            obj.RowType = "T3";
            list.Add(obj);
            obj = new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34);
            obj.RowType = "T4";
            list.Add(obj);

            _bs = new BindingSource();
            _bs.DataSource = list;

            _grid.DataSource = _bs;

            // IMPORTANT: rebuild merges after binding (and after any data changes)
            _grid.RebuildMerges();
        }

        private void Grid_RowCellsDragCompleted(object sender, RowCellsDragEventArgs e)
        {
            // Example: show the drag result
            this.Text = string.Format("Drag row {0}-{3}: {1} -> {2}", e.StartRowIndex, e.FromColumnName, e.ToColumnName, e.EndRowIndex);
            var rows = e.DataList.Cast<testObj>().ToList();
        }

        private void FreezeLeftColumns()
        {
            _grid.Columns["bgcolConstructionType"].Frozen = true;
            _grid.Columns["bgcolConstructionItem"].Frozen = true;
            _grid.Columns["bgcolConstructionCategory"].Frozen = true;
            _grid.Columns["bgcolConstructionPosition"].Frozen = true;

            _grid.Columns["bgcolConstructionRegion"].Frozen = true;
            _grid.Columns["bgcolConstructionSpecification"].Frozen = true;
            _grid.Columns["bgcolConstructionDivision"].Frozen = true;
            _grid.Columns["bgcolRepairConstructionContent"].Frozen = true;

            _grid.Columns["bgcolRepairPeriod"].Frozen = true;
            _grid.Columns["bgcolRepairPlan"].Frozen = true;
            _grid.Columns["bgcolHistory"].Frozen = true;
            _grid.Columns["bgcolRegister"].Frozen = true;

            _grid.Columns["bgcolAmount"].Frozen = true;
            _grid.Columns["bgcolUnit"].Frozen = true;
            _grid.Columns["bgcolDatumYear"].Frozen = true;
            _grid.Columns["bgcolEffectedYear"].Frozen = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _yearStart = int.Parse(this.textBox1.Text);
            // rebuild year columns (names + DataPropertyName)
            CreateColYearAll(_grid, _yearStart, 60);

            // rebuild banded header layout to reference new names
            _grid.SetHeaderLayout(BuildHeaderLikeImage_Correct2());

            // If you rely on merges after changing columns:
            _grid.RebuildMerges();

            _grid.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }

    // Example vertical merge provider: merges Column1..Column4 when equal.
    internal sealed class SimpleTestObjMergeProvider : IVerticalMergeProvider<testObj>
    {
        public bool MergeWithNextRow(DataGridView grid, testObj row, testObj nextRow, string columnName, int rowIndex)
        {
            if (row == null || nextRow == null) return false;

            if (columnName == "bgcolConstructionType") return row.ConstructionTypeName == nextRow.ConstructionTypeName;
            if (columnName == "Column2") return row.Column2 == nextRow.Column2;
            if (columnName == "Column3") return row.Column3 == nextRow.Column3;
            if (columnName == "Column4") return row.Column4 == nextRow.Column4;

            return false;
        }

    }
    public class testObj
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Row { get; set; }

        public string ConstructionTypeName { get; set; }
        public string ConstructionItemName { get; set; }
        public string ConstructionCategoryName { get; set; }
        public string ConstructionPositionName { get; set; }
        public string ConstructionRegionName { get; set; }
        public string ConstructionSpecificationName { get; set; }
        public string ConstructionDivisionName { get; set; }
        public string RepairConstructionContentName { get; set; }

        public int Y_0 { get; set; }
        public int Y_1 { get; set; }
        public int Y_2 { get; set; }
        public int Y_3 { get; set; }
        public string RowType { get; set; }

        public testObj(string val1, string val2, string val3, string val4, int val5, int val8, int val6, int val7)
        {
            this.Column1 = val1;
            this.Column2 = val2;
            this.Column3 = val3;
            this.Column4 = val4;
            this.Y_0 = val5;
            this.Y_1 = val6;
            this.Y_2 = val7;
            this.Y_3 = val8;
            this.Row = "BBB";
        }
    } 
}
