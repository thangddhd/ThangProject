using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class chartFixLeft : Form
    {
        private Chart chartYAxis;
        private Chart chartMain;
        private Panel panelChart;
        public chartFixLeft()
        {
            InitializeComponent();
        }

        private void chartFixLeft_Load(object sender, EventArgs e)
        {
            Panel panelContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 300,
                AutoScroll = true
            };
            this.Controls.Add(panelContainer);

            Chart chartFull = new Chart
            {
                Width = 1200, // Đặt chiều rộng lớn để scroll ngang
                Height = 300
            };
            panelContainer.Controls.Add(chartFull);

            ChartArea area = new ChartArea("MainArea");

            // Trục X
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;

            // Trục Y
            area.AxisY.Title = "金額\n千円";
            area.AxisY.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Arial", 9);
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.LineWidth = 2;

            // Quan trọng: Giữ phần trục Y bên trái, phần còn lại dịch phải
            area.Position = new ElementPosition(0, 0, 100, 100);      // Vị trí tổng thể
            area.InnerPlotPosition = new ElementPosition(15, 10, 80, 80); // Dịch vùng vẽ sang phải

            chartFull.ChartAreas.Add(area);

            chartFull.Series.Clear();

            // Cột cam: 工事費累計
            Series s1 = new Series("工事費累計")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Orange,
                BorderColor = Color.DarkOrange
            };

            // Đường tròn tím: 改定第二案
            Series s2 = new Series("改定第二案")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.MediumOrchid,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8
            };

            // Giả sử 12 năm
            for (int i = 0; i < 12; i++)
            {
                string year = (2044 + i).ToString() + "年";
                int value1 = 5000 + i * 5000;
                int value2 = 2000 + i * 3000;

                s1.Points.AddXY(year, value1);
                s2.Points.AddXY(year, value2);
            }

            chartFull.Series.Add(s1);
            chartFull.Series.Add(s2);
        }

        private void InitializeChartYAxis()
        {
            ChartArea yArea = new ChartArea("YAxisArea");

            // Ẩn trục X
            yArea.AxisX.LineWidth = 0;
            yArea.AxisX.LabelStyle.Enabled = false;
            yArea.AxisX.MajorTickMark.Enabled = false;
            yArea.AxisX.MajorGrid.Enabled = false;

            // Cấu hình trục Y (có title như hình gốc của bạn)
            yArea.AxisY.LabelStyle.Font = new Font("Arial", 9);
            yArea.AxisY.Title = "金額\n千円"; // Ghi theo chiều dọc
            yArea.AxisY.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            yArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            yArea.AxisY.Minimum = 0;
            yArea.AxisY.Maximum = 80000;
            yArea.AxisY.Interval = 20000; // Đặt theo hình gốc của bạn

            chartYAxis.ChartAreas.Clear();
            chartYAxis.ChartAreas.Add(yArea);

            // Thêm dummy series để chart hiển thị
            Series dummy = new Series("dummy")
            {
                ChartType = SeriesChartType.Point,
                Color = Color.Transparent
            };
            dummy.Points.AddY(0);
            dummy.Points.AddY(80000); // Tạo 2 điểm để xác định phạm vi trục
            chartYAxis.Series.Clear();
            chartYAxis.Series.Add(dummy);
        }

        private void InitializeChartMain()
        {
            ChartArea mainArea = new ChartArea("MainArea");
            mainArea.AxisY.LabelStyle.Enabled = false; // Ẩn trục Y bên này
            mainArea.AxisY.MajorGrid.Enabled = false;
            mainArea.AxisY.LineWidth = 0;

            mainArea.AxisX.Interval = 1;
            mainArea.AxisX.LabelStyle.Angle = -45;

            chartMain.ChartAreas.Add(mainArea);

            Series series = new Series("工事費累計")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Orange
            };

            // Dữ liệu mẫu
            int[] data = { 220, 7590, 11660, 28490, 73370, 78540 };
            string[] years = { "2044年", "2046年", "2049年", "2052年", "2055年", "2058年" };

            for (int i = 0; i < data.Length; i++)
            {
                series.Points.AddXY(years[i], data[i]);
            }

            chartMain.Series.Add(series);
        }
    }
}
