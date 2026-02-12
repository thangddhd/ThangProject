using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coms.COMMON.ui
{
    partial class DataGridViewEx
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.BackgroundColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Name = "GroupableDataGridView";

            this.ResumeLayout(false);
        }
    }
}
