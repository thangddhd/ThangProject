using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace coms.COMSK.ui.common
{
    public partial class CtrBasicRepairPlan_B : UserControl
    {
        public event EventHandler Adding;

        public CtrBasicRepairPlan_B()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Adding != null)
            {
                Adding(this, EventArgs.Empty);
            }

        }
    }
}
