using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace GridviewEx
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<DataTest> lstSrc = new List<DataTest>();
            lstSrc.Add(DataTest.GetNew(1));
            lstSrc.Add(DataTest.GetNew(2));
            lstSrc.Add(DataTest.GetNew(3));
            lstSrc.Add(DataTest.GetNew(4));
            lstSrc.Add(DataTest.GetNew(5));
            lstSrc.Add(DataTest.GetNew(13));
            lstSrc.Add(DataTest.GetNew(17));
            lstSrc.Add(DataTest.GetNew(25));
            lstSrc.Add(DataTest.GetNew(35));
            lstSrc.Add(DataTest.GetNew(46));
            lstSrc.Add(DataTest.GetNew(57));
            BindingSource bds = new BindingSource();
            bds.DataSource = lstSrc;
            

            BindingList<DataTest> realList = new BindingList<DataTest>();
            if (lstSrc != null)
            {
                foreach (var item in lstSrc)
                {
                    realList.Add(item);
                }
            }
            bds.AllowNew = true;
            bds.DataSource = realList;
            bds.ResetBindings(true);
            dataGridViewEx1.DataSource = bds;
            dataGridViewEx1.SortAsNumberColumns.Add(Column2.Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
        }
    }

    public class DataTest
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public DateTime? Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }

        public static DataTest GetNew(int index)
        {
            DataTest ret = new DataTest();
            ret.Column1 = "Column1_" + index.ToString();
            var text = "Column2_" + index.ToString();
            if (index < 30)
            {
                text = (index + 7).ToString();
            }
            ret.Column2 = text;
            if (index == 2)
            {
                ret.Column3 = null;
            } else
            {
                ret.Column3 = DateTime.Now.AddDays(index);
            }
            ret.Column4 = "Column4_" + index.ToString();
            ret.Column5 = "Column5_" + index.ToString();
            ret.Column6 = "Column6_" + index.ToString();
            ret.Column7 = "Column7_" + index.ToString();
            return ret;
        }
    }
}
