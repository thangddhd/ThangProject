using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridviewEx
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            reserveGridView1.DataSource = MakeSrc(20);
        }

        private List<objtest> MakeSrc(int rowNum)
        {
            List<objtest> ret = new List<objtest>();
            Random rnd = new Random();
            for (int i = 0; i < rowNum; i++)
            {
                objtest obj = new objtest();
                obj.Column2 = rnd.Next(1200, 120001);
                obj.Column3 = rnd.Next(1200, 120001);
                obj.Column4 = rnd.Next(1200, 120001);
                obj.Column5 = rnd.Next(1200, 120001);
                obj.Column6 = rnd.Next(1200, 120001);
                obj.Column7 = rnd.Next(1200, 120001);
                obj.Column8 = rnd.Next(1200, 120001);
                ret.Add(obj);
            }
            return ret;
        }

        private int GetRand()
        {
            Random rnd = new Random();
            return rnd.Next(1200, 120001);
        }

        private void reserveGridView1_CellDisplayTextNeeded(object sender, coms.COMMON.ui.ReserveCellDisplayTextNeededEventArgs e)
        {
            try
            {
                int rowHandle = e.RowIndex;
                objtest data = e.RowData as objtest;
                if (e.ColumnIndex ==0)
                {
                    var str = string.Format("\\{0:F02}", data.Column2);
                    e.DisplayText = str;
                }
            }
            catch (Exception)
            {
            }
        }

        private void reserveGridView1_CellReadOnlyNeeded(object sender, coms.COMMON.ui.ReserveCellReadOnlyNeededEventArgs e)
        {
            if (e.RowIndex < 3)
            {
                e.ReadOnly = true;
            }
        }

        private void reserveGridView1_CellStyleNeeded(object sender, coms.COMMON.ui.ReserveCellStyleNeededEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                e.BackColor = Color.Red;
            }
        }

        private void reserveGridView1_CellBeginEditRule(object sender, coms.COMMON.ui.ReserveCellBeginEditEventArgs e)
        {

        }

        private void reserveGridView1_EditingControlRule(object sender, coms.COMMON.ui.ReserveEditingControlShowingEventArgs e)
        {

        }
    }

    public class objtest
    {
        public int Column2 { get; set; }
        public int Column3 { get; set; }
        public int Column4 { get; set; }
        public int Column5 { get; set; }
        public int Column6 { get; set; }
        public int Column7 { get; set; }
        public int Column8 { get; set; }
    }
}
