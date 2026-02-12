using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using gridview_opens.controls;

namespace gridview_opens
{



    public partial class Form1 : Form
    {
        private GroupableDataGridView groupableGrid1;
        private Button btnGroupByDept;
        private Button btnClearGroup;
        public ColumnChooserPopup _chooserPopup;
        private List<DataGridViewColumn> Columns;


        public Form1()
        {
            InitializeComponent();
            SetupDemo();
        }

        private void SetupDemo()
        {
            AABB tobj = new AABB();
            var dt = tobj.MakeDataTest();
            // Bind via BindingSource to keep ADGV features working
            var bs = new BindingSource { DataSource = dt };
            groupableGrid1.DataSource = bs;
        }

        private void btnGroupByDept_Click(object sender, EventArgs e)
        {
            groupableGrid1.GroupBy("Department");
        }

        private void btnClearGroup_Click(object sender, EventArgs e)
        {
            //groupableGrid1.ClearGrouping();
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void groupableGrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                _chooserPopup = new ColumnChooserPopup(groupableGrid1);
                _chooserPopup.ColumnReAddRequested += col =>
                {
                    if (groupableGrid1.Columns.Contains(col))
                    {
                        col.Visible = true;
                        try { col.DisplayIndex = groupableGrid1.Columns.Count - 1; } catch { }
                    }
                    _chooserPopup.RemoveColumnByName(col.Name);
                };
                var screenPoint = this.PointToScreen(checkBox1.Location);
                screenPoint.Y += checkBox1.Height;
                _chooserPopup.Location = screenPoint;
                _chooserPopup.Show(this);
            }
            else
            {
                _chooserPopup?.Close();
            }
        }

        public string GetHiddenColumnsJson()
        {
            var hidden = this.Columns.Cast<DataGridViewColumn>()
                .Where(c => !c.Visible)
                .Select(c => c.Name)
                .ToList();
            return string.Join(",", hidden);
        }

        public void RestoreHiddenColumnsFromJson(string csv)
        {
            if (string.IsNullOrEmpty(csv)) return;
            var cols = csv.Split(',').Select(s => s.Trim()).ToHashSet();
            foreach (DataGridViewColumn c in this.Columns)
            {
                c.Visible = !cols.Contains(c.Name);
            }
        }

        //private void groupableGrid1_CellMerge(object sender, CellMergeEventArgs e)
        //{
        //    GroupableDataGridView view = sender as GroupableDataGridView;
        //    //var needMerge = 
        //    if (e.Column == truong || e.Column == lop || e.Column == ten || e.Column == toan || e.Column == tienganh || e.Column == rank)
        //    {
        //        String workID1 = view.GetRowCellValue(e.RowIndex1, e.Column.Name).ToString();
        //        String workID2 = view.GetRowCellValue(e.RowIndex2, e.Column.Name).ToString();
        //        e.Merge = (workID1 == workID2);
        //        e.Handled = true;
        //    }


        //}

        //private bool needMerge(CellMergeEventArgs e)
        //{
        //    return true;
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    Form3 f = new Form3();
        //    f.Show();
        //}

        //private void groupableGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0 || e.ColumnIndex < 0)
        //        return;

        //    var grid = sender as GroupableDataGridView;
        //    var column = grid.Columns[e.ColumnIndex];
        //    var cell = grid[e.ColumnIndex, e.RowIndex];

        //    // Kiểm tra có merge không
        //    var (top, bottom) = grid.GetMergeRangeForCell(column, e.ColumnIndex, e.RowIndex);

        //    // Nếu ô nằm trong vùng merge giả => chỉ cho phép click ở ô đầu (top)
        //    if (e.RowIndex != top)
        //        return;

        //    // ---- Button column ----
        //    if (column is DataGridViewButtonColumn)
        //    {
        //        string buttonText = cell.Value?.ToString() ?? "";
        //        MessageBox.Show($"Click button '{buttonText}' tại dòng {e.RowIndex + 1}");
        //        return;
        //    }

        //    // ---- Link column ----
        //    if (column is DataGridViewLinkColumn)
        //    {
        //        string linkText = cell.Value?.ToString() ?? "";
        //        MessageBox.Show($"Click link '{linkText}' tại dòng {e.RowIndex + 1}");
        //        return;
        //    }

        //}
    }
}
