using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private float zoomFactor = 1.0f; // Initial zoom factor
        private const float zoomStep = 0.1f; // Zoom in/out step
        private int scrollAmount = 20;
        private int maxPosRight = 1374;// this got from debug when can not scroll right any more
        private Color bgrCL = Color.FromArgb(206, 219, 226);
        public Form1()
        {
            InitializeComponent();

            var data = this.GetSrc();
            this.gcKumiai.DataSource = data;
        }

        public List<KumiaiInfo> GetSrc()
        {
            List<KumiaiInfo> lstData = new List<KumiaiInfo>();

            for(int i = 1; i < 60; i++)
            {
                KumiaiInfo obj1 = new KumiaiInfo();
                obj1.Pid = i;
                obj1.KumiaiName = "A" + i.ToString();
                lstData.Add(obj1);
            }
            return lstData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> hidBanded = new List<string>();
            List<string> hidColumn = new List<string>();
            foreach(DevExpress.XtraGrid.Views.BandedGrid.GridBand band in bandedGridView1.Bands)
            {
                if (!band.Visible) hidBanded.Add(band.Name);
                else
                {
                    if (band.HasChildren)
                    {
                        foreach(DevExpress.XtraGrid.Views.BandedGrid.GridBand cband in band.Children)
                        {
                            if (!cband.Visible) hidBanded.Add(cband.Name);
                        }
                    }
                }
            }
            foreach (DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn col in bandedGridView1.Columns)
            {
                if (!col.Visible) hidColumn.Add(col.Name);
            }
            string msg = "Band:" + string.Join(",", hidBanded.ToArray()) + "####### column: " + string.Join(",", hidColumn.ToArray());
            MessageBox.Show(msg);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand band in bandedGridView1.Bands)
            {
                if (!band.Visible) band.Visible = true;
                else
                {
                    if (band.HasChildren)
                    {
                        foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand cband in band.Children)
                        {
                            if (!cband.Visible) cband.Visible = true;
                        }
                    }
                }
            }
        }
        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            // Check if the Ctrl key is pressed
            if (Control.ModifierKeys == Keys.Control)
            {
                // Increase or decrease zoom factor based on scroll direction
                if (e.Delta > 0)
                {
                    zoomFactor += zoomStep;
                }
                else if (e.Delta < 0 && zoomFactor > zoomStep)
                {
                    zoomFactor -= zoomStep;
                }

                // Apply the zoom by scaling the form and its controls
                ApplyZoom();

                ((HandledMouseEventArgs)e).Handled = true;
            }
            else if (Control.ModifierKeys == Keys.Shift)
            {
                // Get the horizontal scroll bar of the GridControl
                var hScrollBar = bandedGridView1.GridControl.Controls
                    .OfType<DevExpress.XtraEditors.HScrollBar>()
                    .FirstOrDefault();

                if (hScrollBar != null)
                {
                    // Scroll left if Delta is positive, right if Delta is negative
                    if (e.Delta > 0) // Scroll left
                    {
                        //if (hScrollBar.Value - scrollAmount >= hScrollBar.Minimum)
                        hScrollBar.Value = Math.Max(hScrollBar.Minimum, hScrollBar.Value - scrollAmount);
                    }
                    else if (e.Delta < 0) // Scroll right
                    {
                        if ((hScrollBar.Value) <= maxPosRight)
                        {
                            hScrollBar.Value = Math.Min(hScrollBar.Maximum, hScrollBar.Value + scrollAmount);
                        }
                        //if (hScrollBar.Value + scrollAmount <= hScrollBar.Value)
                    }
                }

                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        private void ApplyZoom()
        {
            var fontSize = (float)Math.Round(zoomFactor * 9, 2);
            var font = new Font("Tahoma", fontSize);
            bandedGridView1.Appearance.Row.Font = font;
            bandedGridView1.Appearance.HeaderPanel.Font = font;
            /*
            // Set scale factor for form's AutoScaleDimensions
            this.AutoScaleDimensions = new SizeF(zoomFactor, zoomFactor);
            this.Scale(new SizeF(zoomFactor, zoomFactor));

            // Optionally refresh layout to apply scaling
            this.PerformLayout();
            */
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void gcKumiai_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void bandedGridView1_MouseLeave(object sender, EventArgs e)
        {
            
        }

        public void AAA()
        {
            bool hasInvisible = false;
            foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand band in bandedGridView1.Bands)
            {
                if (!band.Visible) hasInvisible = true;

                if (band.HasChildren)
                {
                    foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand cband in band.Children)
                    {
                        if (!cband.Visible) hasInvisible = true;
                    }
                }
                if (hasInvisible) break;
            }

            if (hasInvisible) this.button2.BackColor = Color.Black;
            else this.button2.BackColor = Color.White;
        }

        private void bandedGridView1_LayoutUpgrade(object sender, DevExpress.Utils.LayoutUpgadeEventArgs e)
        {
            var bb = 2;
        }

        private void bandedGridView1_DragObjectOver(object sender, DevExpress.XtraGrid.Views.Base.DragObjectOverEventArgs e)
        {
            var ccc = 1;
        }

        private void bandedGridView1_LostFocus(object sender, EventArgs e)
        {
            var bbb = 1;
        }

        private void bandedGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            var kkk = 1;
        }

        private void bandedGridView1_ColumnPositionChanged(object sender, EventArgs e)
        {
            var xyz = 111;
        }

        private void gcKumiai_MouseHover(object sender, EventArgs e)
        {
            this.AAA();
        }

        private void bandedGridView1_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e)
        {
            return;

            if (e.Band == null) return;
            e.Cache.FillRectangle(bgrCL, e.Bounds);

            Rectangle r = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, r, (e.Info.State == DevExpress.Utils.Drawing.ObjectState.Pressed ? Border3DStyle.SunkenOuter : Border3DStyle.RaisedInner));
            r.Inflate(-1, -1);
            e.Appearance.ForeColor = e.Info.Appearance.ForeColor;
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            e.Info.InnerElements.DrawObjects(e.Info, e.Info.Cache, Point.Empty);
            e.Handled = true;
        }

        private void panel2_Scroll(object sender, ScrollEventArgs e)
        {
            Point scrollPos = panel2.AutoScrollPosition;
            foreach (Control ctrl in panel1.Controls)
            {
                ctrl.Location = new Point(ctrl.Location.X, -scrollPos.Y);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            chartFixLeft frm = new chartFixLeft();
            frm.Show();
        }
    }

    public class KumiaiInfo
    {
        #region Private Members


        private long pid;
        private string kumiaiCode;
        private string kumiaiName;
        private string kumiaiNameKana;
        private string blockCode;
        private DateTime establishmentDay;
        private int term;
        private string url;
        private string activeFlg;
        private DateTime insertDateTime;
        private DateTime updateDateTime;
        private int deadline;
        private string teamName;
        #endregion // End of Private Members


        private long roomCount = int.MinValue;
        private string blockName = string.Empty;
        private string ownerType = string.Empty;
        private string timer = string.Empty;
        private long kumiaiCodeAsNumber = long.MinValue;

        #region Default ( Empty ) Class Constuctor
        public KumiaiInfo()
        {


            pid = long.MinValue;
            kumiaiCode = String.Empty;
            kumiaiName = String.Empty;
            kumiaiNameKana = String.Empty;
            blockCode = String.Empty;
            establishmentDay = DateTime.MinValue;
            term = int.MinValue;
            url = String.Empty;
            activeFlg = String.Empty;
            insertDateTime = DateTime.MinValue;
            updateDateTime = DateTime.MinValue;
            deadline = int.MinValue;
            teamName = string.Empty;
        }
        #endregion // End of Default ( Empty ) Class Constuctor

        #region Public Accessors 

        


        /// <summary>
        /// 
        /// </summary>		
        public long Pid
        {
            get { return pid; }
            set { pid = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public string KumiaiCode
        {
            get { return kumiaiCode; }
            set { kumiaiCode = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public string KumiaiName
        {
            get { return kumiaiName; }
            set { kumiaiName = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public string KumiaiNameKana
        {
            get { return kumiaiNameKana; }
            set { kumiaiNameKana = value; }
        }

        /// <summary>
        /// 
        /// </summary>		
        public string BlockCode
        {
            get { return blockCode; }
            set { blockCode = value; }
        }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime EstablishmentDay
        {
            get { return establishmentDay; }
            set { establishmentDay = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public int Term
        {
            get { return term; }
            set { term = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public string Url
        {
            get { return url; }
            set { url = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public string ActiveFlg
        {
            get { return activeFlg; }
            set { activeFlg = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public DateTime InsertDateTime
        {
            get { return insertDateTime; }
            set { insertDateTime = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public DateTime UpdateDateTime
        {
            get { return updateDateTime; }
            set { updateDateTime = value; }
        }


        /// <summary>
        /// 
        /// </summary>		
        public int Deadline
        {
            get { return deadline; }
            set { deadline = value; }
        }


        #endregion // End of Public Accessors 


        private long userMstPid = long.MinValue;
        /// <summary>
        /// 
        /// </summary>		
        public long UserMstPid
        {
            get { return userMstPid; }
            set { userMstPid = value; }
        }

        public long RoomCount
        {
            get { return roomCount; }
            set { roomCount = value; }
        }
        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }
        public string OwnerType
        {
            get { return ownerType; }
            set { ownerType = value; }
        }
        public string Timer
        {
            get { return timer; }
            set { timer = value; }
        }
        public long KumiaiCodeAsNumber
        {
            get { return kumiaiCodeAsNumber; }
            set { kumiaiCodeAsNumber = value; }
        }
        public string TeamName
        {
            get { return this.teamName; }
            set { this.teamName = value; }
        }
    }
}
