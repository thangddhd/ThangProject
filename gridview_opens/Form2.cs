using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using gridview_opens.controls;

namespace gridview_opens
{
    public class AABB
    {
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }
        public int? MathScore { get; set; }
        public int? EnglishScore { get; set; }
        public string Rank { get; set; }

        public List<AABB> MakeDataTest()
        {
            List<AABB> ret = new List<AABB>();
            ret.Add(this.makePuple("NQ", "A", "Thang", 1, 2, "kem"));
            ret.Add(this.makePuple("NQ", "A", "Thang", 2, 3, "kem"));
            ret.Add(this.makePuple("NQ", "A", "Thang", 3, 4, "kem"));
            ret.Add(this.makePuple("NQ", "A", "Cheo", 1, 2, "kem"));
            ret.Add(this.makePuple("NQ", "A", "Cheo", 2, 3, "kem"));
            ret.Add(this.makePuple("NQ", "A", "Cheo", 3, 4, "kem"));
            ret.Add(this.makePuple("NQ", "B", "Chi pheo", 1, 2, "kem"));
            ret.Add(this.makePuple("NQ", "B", "Chi pheo", 2, 3, "kem"));
            ret.Add(this.makePuple("NQ", "B", "Chi pheo", 3, 4, "kem"));
            ret.Add(this.makePuple("TH", "A", "Thang B", 1, 2, "kem"));
            ret.Add(this.makePuple("TH", "A", "Thang B", 2, 3, "kem"));
            ret.Add(this.makePuple("TH", "A", "Thang B", 3, 4, "kem"));
            ret.Add(this.makePuple("TH", "A", "Cheo B", 1, 2, "kem"));
            ret.Add(this.makePuple("TH", "A", "Cheo B", 2, 3, "kem"));
            ret.Add(this.makePuple("TH", "A", "Cheo B", 3, 4, "kem"));
            ret.Add(this.makePuple("BX", "A", "Thang", 1, 2, "kem"));
            ret.Add(this.makePuple("BX", "A", "Thang", 2, 3, "kem"));
            ret.Add(this.makePuple("BX", "A", "Thang", 3, 4, "kem"));
            ret.Add(this.makePuple("BX", "A", "Cheo", 1, 2, "kem"));
            ret.Add(this.makePuple("BX", "A", "Cheo", 2, 3, "kem"));
            ret.Add(this.makePuple("BX", "A", "Cheo", 3, 4, "ok"));
            ret.Add(this.makePuple("BX", "B", "Chi pheo", 1, 2, "ok"));
            ret.Add(this.makePuple("BX", "B", "Chi pheo", 2, 3, "ok"));
            ret.Add(this.makePuple("BX", "B", "Chi pheo", 3, 4, "ok"));
            return ret;
        }

        private AABB makePuple(string sName, string cName, string name, int mScore, int eScore, string rank)
        {
            AABB pp = new AABB();
            pp.SchoolName = sName;
            pp.ClassName = cName;
            pp.Name = name;
            pp.MathScore = mScore;
            pp.EnglishScore = eScore;
            pp.Rank = rank;

            return pp;
        }
    }

    public partial class Form2 : Form
    {
        public ColumnChooserPopup _chooserPopup;
        public Form2()
        {
            InitializeComponent();

            AABB tobj = new AABB();
            var dt = tobj.MakeDataTest();
            var bs = new BindingSource { DataSource = dt };
            groupableDataGridView1.DataSource = bs;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                _chooserPopup = new ColumnChooserPopup(groupableDataGridView1);
                _chooserPopup.ColumnReAddRequested += col =>
                {
                    if (groupableDataGridView1.Columns.Contains(col))
                    {
                        col.Visible = true;
                        try { col.DisplayIndex = groupableDataGridView1.Columns.Count - 1; } catch { }
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
    }
}
