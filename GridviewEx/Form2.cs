using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using coms.COMSK.ui.common;

namespace GridviewEx
{
    public partial class Form2 : coms.MyForm
    {
        private LongRepairGridView<testObj> _grid;
        private BindingSource _bs;

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
            _grid.SetVerticalMergeColumns(new[] { "Column1", "Column2", "Column3", "Column4" });
            _grid.VerticalMergeProvider = new SimpleTestObjMergeProvider();

            // Left columns list (for calc-row horizontal merge) - not used for testObj unless you set IsCalcRow
            _grid.SetLeftColumnNames(new[] { "Column1", "Column2", "Column3", "Column4" });

            // No calc rows in testObj by default, so keep null (or set false)
            _grid.IsCalcRow = null;

            // Drag completed event
            _grid.RowCellsDragCompleted += Grid_RowCellsDragCompleted;

            // freeze
            FreezeLeftColumns();

            // Example header layout (2 rows)
            _grid.SetHeaderLayout(BuildHeaderLikeImage_Correct2());

            // Add to form
            this.Controls.Add(_grid);

            // If you want it below existing controls, use a panel instead of Controls.Add directly.
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

            this.CreateColYearAll(grid);

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
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionType", "工事分類", true));
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolConstructionItem", "工事項目", true));
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

            this.CreateBandedYearAll(ref layout);

            // ===== 築年 (nested 3 level)
            layout.Cells.Add(MakeHeaderCell(0, 1,
                new[] { "bgcolEffectedYear" }, "築年", true));

            layout.Cells.Add(MakeHeaderCell(1, 1,
                new[] { "bgcolEffectedYear" }, "会計期", false));

            layout.Cells.Add(MakeHeaderCell(2, 1,
                "bgcolEffectedYear", "会計年度", false));

            // ===== subtotal
            layout.Cells.Add(MakeHeaderCell(0, 3, "bgcolSubTotal", "小計", true));

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

        private void CreateBandedYearAll(ref HeaderBandLayout layout)
        {
            for (int idx = 0; idx < 60; idx++)
            {
                string colName = "Y_" + idx.ToString();
                string yearIdx = (idx + 1).ToString() + "年目";
                string termIdx = (idx + 1).ToString() + "期";
                string year = (idx + 2001).ToString() + "年";
                this.CreateBandedYear(ref layout, colName, yearIdx, termIdx, year);
            }
        }

        private void CreateColYearAll(DataGridView grid)
        {
            for (int idx = 0; idx < 60; idx++)
            {
                string colName = "Y_" + idx.ToString();
                string fieldName = "Y_" + idx.ToString();
                grid.Columns.Add(CreateCol(colName, fieldName, "yearData", 120));
            }
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
            list.Add(new testObj("A", "A1", "X", "K", 10, 40, 20, 30));
            list.Add(new testObj("A", "A1", "X", "K", 11, 41, 21, 31));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "X", "K", 10, 40, 20, 30));
            list.Add(new testObj("A", "A1", "X", "K", 11, 41, 21, 31));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "X", "K", 10, 40, 20, 30));
            list.Add(new testObj("A", "A1", "X", "K", 11, 41, 21, 31));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "X", "K", 10, 40, 20, 30));
            list.Add(new testObj("A", "A1", "X", "K", 11, 41, 21, 31));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "X", "K", 10, 40, 20, 30));
            list.Add(new testObj("A", "A1", "X", "K", 11, 41, 21, 31));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "X", "K", 10, 40, 20, 30));
            list.Add(new testObj("A", "A1", "X", "K", 11, 41, 21, 31));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));
            list.Add(new testObj("A", "A1", "X", "K", 10, 40, 20, 30));
            list.Add(new testObj("A", "A1", "X", "K", 11, 41, 21, 31));
            list.Add(new testObj("A", "A1", "Y", "K", 12, 42, 22, 32));
            list.Add(new testObj("B", "B1", "Y", "K", 13, 43, 23, 33));
            list.Add(new testObj("B", "B1", "Y", "Z", 14, 44, 24, 34));

            _bs = new BindingSource();
            _bs.DataSource = list;

            _grid.DataSource = _bs;

            // IMPORTANT: rebuild merges after binding (and after any data changes)
            _grid.RebuildMerges();
        }

        private void Grid_RowCellsDragCompleted(object sender, RowCellsDragEventArgs e)
        {
            // Example: show the drag result
            this.Text = string.Format("Drag row {0}-{3}: {1} -> {2}", e.StartRowIndex, e.FromColumnName, e.ToColumnName, e.StartRowIndex);
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
    }

    // Example vertical merge provider: merges Column1..Column4 when equal.
    internal sealed class SimpleTestObjMergeProvider : IVerticalMergeProvider<testObj>
    {
        public bool MergeWithNextRow(DataGridView grid, testObj row, testObj nextRow, string columnName, int rowIndex)
        {
            if (row == null || nextRow == null) return false;

            if (columnName == "Column1") return row.Column1 == nextRow.Column1;
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

        public int Y_0 { get; set; }
        public int Y_1 { get; set; }
        public int Y_2 { get; set; }
        public int Y_3 { get; set; }

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
        }
    } 
}
