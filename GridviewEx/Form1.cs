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
            dataGridViewEx1.DataSource = lstSrc;
            dataGridViewEx1.SortAsNumberColumns.Add(Column2.Name);
        }
    }

    public class DataTest
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
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
            ret.Column3 = "Column3_" + index.ToString();
            ret.Column4 = "Column4_" + index.ToString();
            ret.Column5 = "Column5_" + index.ToString();
            ret.Column6 = "Column6_" + index.ToString();
            ret.Column7 = "Column7_" + index.ToString();
            return ret;
        }
    }
}
