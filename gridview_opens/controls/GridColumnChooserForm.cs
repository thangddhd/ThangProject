using System;
using System.Linq;
using System.Windows.Forms;

namespace gridview_opens.controls
{
    public partial class GridColumnChooserForm : Form
    {
        private GroupableDataGridView grid;

        public GridColumnChooserForm(GroupableDataGridView grid)
        {
            InitializeComponent();
            this.grid = grid;
            LoadColumns();
        }

        private void LoadColumns()
        {
            checkedListBox1.Items.Clear();
            foreach (DataGridViewColumn c in grid.Columns)
            {
                checkedListBox1.Items.Add(c.HeaderText, c.Visible);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < grid.Columns.Count && i < checkedListBox1.Items.Count; i++)
            {
                grid.Columns[i].Visible = checkedListBox1.GetItemChecked(i);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region Designer
        private CheckedListBox checkedListBox1;
        private Button btnOk;
        private Button btnCancel;
        private void InitializeComponent()
        {
            this.checkedListBox1 = new CheckedListBox();
            this.btnOk = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();
            // checkedListBox1
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 12);
            this.checkedListBox1.Size = new System.Drawing.Size(360, 340);
            // btnOk
            this.btnOk.Location = new System.Drawing.Point(216, 360);
            this.btnOk.Size = new System.Drawing.Size(75, 30);
            this.btnOk.Text = "OK";
            this.btnOk.Click += new EventHandler(this.btnOk_Click);
            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(297, 360);
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            // Form
            this.ClientSize = new System.Drawing.Size(384, 402);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Column Chooser";
            this.ResumeLayout(false);
        }
        #endregion
    }
}