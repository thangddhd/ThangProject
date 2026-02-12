using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gridview_opens
{
    public partial class Form3 : Form
    {
        public Form3()
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
            customDataGridView1.DataSource = bs;

            customDataGridView1.AutoMergeColumn(0);
            customDataGridView1.AutoMergeColumn(1);
            customDataGridView1.AutoMergeColumn(5);
        }
    }
}
