
namespace coms.COMMON.ui
{
    partial class PrintPreviewForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripComboBox cboZoom;
        private System.Windows.Forms.ToolStripButton btnExportPdf;
        private System.Windows.Forms.ToolStripButton btnExportExcel;
        private System.Windows.Forms.ToolStripButton btnClose;

        private System.Windows.Forms.PrintPreviewControl previewControl;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintPreviewForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.cboZoom = new System.Windows.Forms.ToolStripComboBox();
            this.cboPaperSize = new System.Windows.Forms.ToolStripComboBox();
            this.btnPortrait = new System.Windows.Forms.ToolStripButton();
            this.btnLandscape = new System.Windows.Forms.ToolStripButton();
            this.btnExportPdf = new System.Windows.Forms.ToolStripButton();
            this.btnExportExcel = new System.Windows.Forms.ToolStripButton();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.previewControl = new System.Windows.Forms.PrintPreviewControl();
            this.btnExportCSV = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPrint,
            this.btnZoomIn,
            this.btnZoomOut,
            this.cboZoom,
            this.cboPaperSize,
            this.btnPortrait,
            this.btnLandscape,
            this.btnExportPdf,
            this.btnExportExcel,
            this.btnExportCSV,
            this.btnClose});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1473, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // btnPrint
            // 
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(35, 22);
            this.btnPrint.Text = "印刷";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(46, 22);
            this.btnZoomIn.Text = "拡大 +";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(43, 22);
            this.btnZoomOut.Text = "縮小 -";
            // 
            // cboZoom
            // 
            this.cboZoom.AutoSize = false;
            this.cboZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZoom.Items.AddRange(new object[] {
            "25%",
            "50%",
            "75%",
            "100%",
            "150%",
            "200%"});
            this.cboZoom.Name = "cboZoom";
            this.cboZoom.Size = new System.Drawing.Size(89, 23);
            // 
            // cboPaperSize
            // 
            this.cboPaperSize.AutoSize = false;
            this.cboPaperSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPaperSize.Items.AddRange(new object[] {
            "A4",
            "A3"});
            this.cboPaperSize.Name = "cboPaperSize";
            this.cboPaperSize.Size = new System.Drawing.Size(80, 23);
            this.cboPaperSize.SelectedIndexChanged += new System.EventHandler(this.cboPaperSize_SelectedIndexChanged);
            // 
            // btnPortrait
            // 
            this.btnPortrait.Name = "btnPortrait";
            this.btnPortrait.Size = new System.Drawing.Size(44, 22);
            this.btnPortrait.Text = "縦向き";
            this.btnPortrait.Click += new System.EventHandler(this.btnPortrait_Click);
            // 
            // btnLandscape
            // 
            this.btnLandscape.Name = "btnLandscape";
            this.btnLandscape.Size = new System.Drawing.Size(47, 22);
            this.btnLandscape.Text = "横方向";
            this.btnLandscape.Click += new System.EventHandler(this.btnLandscape_Click);
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(56, 22);
            this.btnExportPdf.Text = "PDF出力";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(62, 22);
            this.btnExportExcel.Text = "Excel出力";
            // 
            // btnClose
            // 
            this.btnClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(41, 22);
            this.btnClose.Text = "閉じる";
            // 
            // previewControl
            // 
            this.previewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewControl.Location = new System.Drawing.Point(0, 25);
            this.previewControl.Name = "previewControl";
            this.previewControl.Size = new System.Drawing.Size(1473, 812);
            this.previewControl.TabIndex = 0;
            // 
            // btnExportCSV
            // 
            this.btnExportCSV.Name = "btnExportCSV";
            this.btnExportCSV.Size = new System.Drawing.Size(55, 22);
            this.btnExportCSV.Text = "CSV出力";
            // 
            // PrintPreviewForm
            // 
            this.ClientSize = new System.Drawing.Size(1473, 837);
            this.Controls.Add(this.previewControl);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PrintPreviewForm";
            this.Text = "Print Preview";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.PrintPreviewForm_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ToolStripButton btnPortrait;
        private System.Windows.Forms.ToolStripButton btnLandscape;
        private System.Windows.Forms.ToolStripComboBox cboPaperSize;
        private System.Windows.Forms.ToolStripButton btnExportCSV;
    }
}