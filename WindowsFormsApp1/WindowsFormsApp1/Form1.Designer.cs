
namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gcKumiai = new DevExpress.XtraGrid.GridControl();
            this.bandedGridView1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
            this.gridBand2 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clDetail = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand3 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clKumiaiCode = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand4 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clKumiaiName = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand5 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clZipCode = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand6 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clRoomCount = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.WideLatin = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBand12 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clTel = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand8 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clAddress = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand9 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clTerm = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand10 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clTeamName = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clPostName = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand11 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.clOwnerType = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gcKumiai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // gcKumiai
            // 
            this.gcKumiai.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gcKumiai.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gcKumiai.Location = new System.Drawing.Point(13, 196);
            this.gcKumiai.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gcKumiai.LookAndFeel.UseWindowsXPTheme = true;
            this.gcKumiai.MainView = this.bandedGridView1;
            this.gcKumiai.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gcKumiai.Name = "gcKumiai";
            this.gcKumiai.Size = new System.Drawing.Size(1663, 652);
            this.gcKumiai.TabIndex = 77;
            this.gcKumiai.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.bandedGridView1});
            this.gcKumiai.DragDrop += new System.Windows.Forms.DragEventHandler(this.gcKumiai_DragDrop);
            this.gcKumiai.MouseHover += new System.EventHandler(this.gcKumiai_MouseHover);
            // 
            // bandedGridView1
            // 
            this.bandedGridView1.Appearance.FocusedCell.BackColor = System.Drawing.Color.CornflowerBlue;
            this.bandedGridView1.Appearance.FocusedCell.BorderColor = System.Drawing.Color.Red;
            this.bandedGridView1.Appearance.FocusedCell.Options.UseBackColor = true;
            this.bandedGridView1.Appearance.FocusedCell.Options.UseBorderColor = true;
            this.bandedGridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.CornflowerBlue;
            this.bandedGridView1.Appearance.FocusedRow.BorderColor = System.Drawing.Color.Red;
            this.bandedGridView1.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.bandedGridView1.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Black;
            this.bandedGridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.bandedGridView1.Appearance.FocusedRow.Options.UseBorderColor = true;
            this.bandedGridView1.Appearance.FocusedRow.Options.UseFont = true;
            this.bandedGridView1.Appearance.FocusedRow.Options.UseForeColor = true;
            this.bandedGridView1.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.bandedGridView1.Appearance.OddRow.BackColor = System.Drawing.Color.AliceBlue;
            this.bandedGridView1.Appearance.OddRow.Options.UseBackColor = true;
            this.bandedGridView1.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            this.gridBand2,
            this.gridBand3,
            this.gridBand4,
            this.gridBand5,
            this.gridBand6,
            this.WideLatin,
            this.gridBand10,
            this.gridBand1,
            this.gridBand11});
            this.bandedGridView1.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[] {
            this.clDetail,
            this.clKumiaiCode,
            this.clKumiaiName,
            this.clRoomCount,
            this.clZipCode,
            this.clAddress,
            this.clTel,
            this.clTerm,
            this.clTeamName,
            this.clPostName,
            this.clOwnerType});
            this.bandedGridView1.GridControl = this.gcKumiai;
            this.bandedGridView1.IndicatorWidth = 20;
            this.bandedGridView1.Name = "bandedGridView1";
            this.bandedGridView1.OptionsCustomization.AllowFilter = false;
            this.bandedGridView1.OptionsCustomization.AllowGroup = false;
            this.bandedGridView1.OptionsCustomization.AllowSort = false;
            this.bandedGridView1.OptionsView.ColumnAutoWidth = false;
            this.bandedGridView1.OptionsView.EnableAppearanceOddRow = true;
            this.bandedGridView1.OptionsView.RowAutoHeight = true;
            this.bandedGridView1.OptionsView.ShowColumnHeaders = false;
            this.bandedGridView1.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.bandedGridView1.OptionsView.ShowGroupPanel = false;
            this.bandedGridView1.OptionsView.ShowIndicator = false;
            this.bandedGridView1.CustomDrawBandHeader += new DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventHandler(this.bandedGridView1_CustomDrawBandHeader);
            this.bandedGridView1.DragObjectOver += new DevExpress.XtraGrid.Views.Base.DragObjectOverEventHandler(this.bandedGridView1_DragObjectOver);
            this.bandedGridView1.ColumnPositionChanged += new System.EventHandler(this.bandedGridView1_ColumnPositionChanged);
            this.bandedGridView1.LayoutUpgrade += new DevExpress.Utils.LayoutUpgadeEventHandler(this.bandedGridView1_LayoutUpgrade);
            this.bandedGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bandedGridView1_MouseUp);
            this.bandedGridView1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseWheel);
            this.bandedGridView1.MouseLeave += new System.EventHandler(this.bandedGridView1_MouseLeave);
            this.bandedGridView1.LostFocus += new System.EventHandler(this.bandedGridView1_LostFocus);
            // 
            // gridBand2
            // 
            this.gridBand2.Caption = "gridBand2";
            this.gridBand2.Columns.Add(this.clDetail);
            this.gridBand2.Name = "gridBand2";
            this.gridBand2.Width = 50;
            // 
            // clDetail
            // 
            this.clDetail.AppearanceHeader.BackColor = System.Drawing.Color.Red;
            this.clDetail.AppearanceHeader.BackColor2 = System.Drawing.Color.Red;
            this.clDetail.AppearanceHeader.Options.UseBackColor = true;
            this.clDetail.Caption = "詳細";
            this.clDetail.Name = "clDetail";
            this.clDetail.OptionsColumn.AllowEdit = false;
            this.clDetail.OptionsColumn.AllowShowHide = false;
            this.clDetail.OptionsColumn.FixedWidth = true;
            this.clDetail.OptionsColumn.ReadOnly = true;
            this.clDetail.Visible = true;
            this.clDetail.Width = 50;
            // 
            // gridBand3
            // 
            this.gridBand3.Caption = "gridBand3";
            this.gridBand3.Columns.Add(this.clKumiaiCode);
            this.gridBand3.Name = "gridBand3";
            this.gridBand3.Width = 80;
            // 
            // clKumiaiCode
            // 
            this.clKumiaiCode.AppearanceCell.Options.UseTextOptions = true;
            this.clKumiaiCode.AppearanceHeader.BackColor = System.Drawing.Color.SaddleBrown;
            this.clKumiaiCode.AppearanceHeader.BackColor2 = System.Drawing.Color.RosyBrown;
            this.clKumiaiCode.AppearanceHeader.Options.UseBackColor = true;
            this.clKumiaiCode.Caption = "組合コード";
            this.clKumiaiCode.FieldName = "KumiaiCode";
            this.clKumiaiCode.Name = "clKumiaiCode";
            this.clKumiaiCode.OptionsColumn.AllowEdit = false;
            this.clKumiaiCode.OptionsColumn.AllowShowHide = false;
            this.clKumiaiCode.OptionsColumn.ReadOnly = true;
            this.clKumiaiCode.Visible = true;
            this.clKumiaiCode.Width = 80;
            // 
            // gridBand4
            // 
            this.gridBand4.Caption = "gridBand4";
            this.gridBand4.Columns.Add(this.clKumiaiName);
            this.gridBand4.Name = "gridBand4";
            this.gridBand4.Width = 250;
            // 
            // clKumiaiName
            // 
            this.clKumiaiName.Caption = "組合名";
            this.clKumiaiName.FieldName = "KumiaiName";
            this.clKumiaiName.Name = "clKumiaiName";
            this.clKumiaiName.OptionsColumn.AllowEdit = false;
            this.clKumiaiName.OptionsColumn.AllowShowHide = false;
            this.clKumiaiName.OptionsColumn.ReadOnly = true;
            this.clKumiaiName.Visible = true;
            this.clKumiaiName.Width = 250;
            // 
            // gridBand5
            // 
            this.gridBand5.Caption = "gridBand5";
            this.gridBand5.Columns.Add(this.clZipCode);
            this.gridBand5.Name = "gridBand5";
            this.gridBand5.Width = 80;
            // 
            // clZipCode
            // 
            this.clZipCode.Caption = "郵便番号";
            this.clZipCode.Name = "clZipCode";
            this.clZipCode.OptionsColumn.AllowEdit = false;
            this.clZipCode.OptionsColumn.AllowShowHide = false;
            this.clZipCode.OptionsColumn.ReadOnly = true;
            this.clZipCode.Visible = true;
            this.clZipCode.Width = 80;
            // 
            // gridBand6
            // 
            this.gridBand6.Caption = "gridBand6";
            this.gridBand6.Columns.Add(this.clRoomCount);
            this.gridBand6.Name = "gridBand6";
            this.gridBand6.Width = 50;
            // 
            // clRoomCount
            // 
            this.clRoomCount.AppearanceCell.Options.UseTextOptions = true;
            this.clRoomCount.Caption = "戸数";
            this.clRoomCount.FieldName = "RoomCount";
            this.clRoomCount.Name = "clRoomCount";
            this.clRoomCount.OptionsColumn.AllowEdit = false;
            this.clRoomCount.OptionsColumn.AllowShowHide = false;
            this.clRoomCount.OptionsColumn.ReadOnly = true;
            this.clRoomCount.Visible = true;
            this.clRoomCount.Width = 50;
            // 
            // WideLatin
            // 
            this.WideLatin.AppearanceHeader.BackColor = System.Drawing.Color.Red;
            this.WideLatin.AppearanceHeader.BackColor2 = System.Drawing.Color.Red;
            this.WideLatin.AppearanceHeader.BorderColor = System.Drawing.Color.Red;
            this.WideLatin.AppearanceHeader.Font = new System.Drawing.Font("Wide Latin", 9F);
            this.WideLatin.AppearanceHeader.Options.UseBackColor = true;
            this.WideLatin.AppearanceHeader.Options.UseBorderColor = true;
            this.WideLatin.AppearanceHeader.Options.UseFont = true;
            this.WideLatin.Caption = "gridBand7";
            this.WideLatin.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            this.gridBand12,
            this.gridBand8,
            this.gridBand9});
            this.WideLatin.MinWidth = 123;
            this.WideLatin.Name = "WideLatin";
            this.WideLatin.Width = 975;
            // 
            // gridBand12
            // 
            this.gridBand12.Caption = "gridBand12";
            this.gridBand12.Columns.Add(this.clTel);
            this.gridBand12.MinWidth = 111;
            this.gridBand12.Name = "gridBand12";
            this.gridBand12.Width = 455;
            // 
            // clTel
            // 
            this.clTel.Caption = "TEL";
            this.clTel.Name = "clTel";
            this.clTel.OptionsColumn.AllowEdit = false;
            this.clTel.OptionsColumn.AllowShowHide = false;
            this.clTel.OptionsColumn.ReadOnly = true;
            this.clTel.Visible = true;
            this.clTel.Width = 455;
            // 
            // gridBand8
            // 
            this.gridBand8.Caption = "gridBand8";
            this.gridBand8.Columns.Add(this.clAddress);
            this.gridBand8.Name = "gridBand8";
            this.gridBand8.Width = 358;
            // 
            // clAddress
            // 
            this.clAddress.Caption = "所在地";
            this.clAddress.Name = "clAddress";
            this.clAddress.OptionsColumn.AllowEdit = false;
            this.clAddress.OptionsColumn.AllowShowHide = false;
            this.clAddress.OptionsColumn.ReadOnly = true;
            this.clAddress.Visible = true;
            this.clAddress.Width = 358;
            // 
            // gridBand9
            // 
            this.gridBand9.Caption = "gridBand9";
            this.gridBand9.Columns.Add(this.clTerm);
            this.gridBand9.Name = "gridBand9";
            this.gridBand9.Width = 162;
            // 
            // clTerm
            // 
            this.clTerm.Caption = "決算月";
            this.clTerm.FieldName = "Term";
            this.clTerm.Name = "clTerm";
            this.clTerm.OptionsColumn.AllowEdit = false;
            this.clTerm.OptionsColumn.AllowShowHide = false;
            this.clTerm.OptionsColumn.ReadOnly = true;
            this.clTerm.Visible = true;
            this.clTerm.Width = 162;
            // 
            // gridBand10
            // 
            this.gridBand10.Caption = "gridBand10";
            this.gridBand10.Columns.Add(this.clTeamName);
            this.gridBand10.Name = "gridBand10";
            this.gridBand10.Width = 304;
            // 
            // clTeamName
            // 
            this.clTeamName.Caption = "所属";
            this.clTeamName.FieldName = "TeamName";
            this.clTeamName.Name = "clTeamName";
            this.clTeamName.OptionsColumn.AllowEdit = false;
            this.clTeamName.OptionsColumn.AllowShowHide = false;
            this.clTeamName.OptionsColumn.ReadOnly = true;
            this.clTeamName.Visible = true;
            this.clTeamName.Width = 304;
            // 
            // gridBand1
            // 
            this.gridBand1.Caption = "gridBand1";
            this.gridBand1.Columns.Add(this.clPostName);
            this.gridBand1.Name = "gridBand1";
            this.gridBand1.Width = 286;
            // 
            // clPostName
            // 
            this.clPostName.Caption = "管理担当者";
            this.clPostName.Name = "clPostName";
            this.clPostName.OptionsColumn.AllowEdit = false;
            this.clPostName.OptionsColumn.AllowShowHide = false;
            this.clPostName.OptionsColumn.ReadOnly = true;
            this.clPostName.Visible = true;
            this.clPostName.Width = 286;
            // 
            // gridBand11
            // 
            this.gridBand11.Caption = "gridBand11";
            this.gridBand11.Columns.Add(this.clOwnerType);
            this.gridBand11.Name = "gridBand11";
            this.gridBand11.Width = 416;
            // 
            // clOwnerType
            // 
            this.clOwnerType.AppearanceCell.Options.UseTextOptions = true;
            this.clOwnerType.Caption = "オーナータイプ";
            this.clOwnerType.FieldName = "OwnerType";
            this.clOwnerType.Name = "clOwnerType";
            this.clOwnerType.OptionsColumn.AllowEdit = false;
            this.clOwnerType.OptionsColumn.AllowShowHide = false;
            this.clOwnerType.OptionsColumn.ReadOnly = true;
            this.clOwnerType.Visible = true;
            this.clOwnerType.Width = 416;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 58);
            this.button1.TabIndex = 78;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(28, 108);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 58);
            this.button2.TabIndex = 79;
            this.button2.Text = "show";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(28, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 26);
            this.comboBox1.TabIndex = 80;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1033, 208);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 36);
            this.button3.TabIndex = 81;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(13, 872);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1095, 161);
            this.tabControl1.TabIndex = 82;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1087, 129);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1087, 129);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 28);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1087, 129);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 28);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1087, 129);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 28);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1087, 129);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 28);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1087, 129);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "tabPage6";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Location = new System.Drawing.Point(511, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 57);
            this.panel1.TabIndex = 83;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(142, 16);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 34);
            this.button6.TabIndex = 2;
            this.button6.Text = "button6";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(468, 16);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 1;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(26, 16);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 34);
            this.button4.TabIndex = 0;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.button7);
            this.panel2.Controls.Add(this.button8);
            this.panel2.Controls.Add(this.button9);
            this.panel2.Location = new System.Drawing.Point(511, 117);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(370, 81);
            this.panel2.TabIndex = 84;
            this.panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panel2_Scroll);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(142, 16);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 33);
            this.button7.TabIndex = 2;
            this.button7.Text = "button7";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(468, 16);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 33);
            this.button8.TabIndex = 1;
            this.button8.Text = "button8";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(26, 16);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 33);
            this.button9.TabIndex = 0;
            this.button9.Text = "button9";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button10);
            this.panel3.Controls.Add(this.button11);
            this.panel3.Controls.Add(this.button12);
            this.panel3.Location = new System.Drawing.Point(511, 64);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(370, 54);
            this.panel3.TabIndex = 85;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(142, 16);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 31);
            this.button10.TabIndex = 2;
            this.button10.Text = "button10";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(468, 16);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(75, 23);
            this.button11.TabIndex = 1;
            this.button11.Text = "button11";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(26, 16);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(75, 31);
            this.button12.TabIndex = 0;
            this.button12.Text = "button12";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(1380, 56);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(136, 34);
            this.button13.TabIndex = 86;
            this.button13.Text = "chartFixLeft";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1689, 1045);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gcKumiai);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gcKumiai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcKumiai;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bandedGridView1;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clDetail;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clKumiaiCode;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clKumiaiName;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clRoomCount;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clZipCode;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clAddress;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clTel;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clTerm;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clTeamName;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clPostName;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn clOwnerType;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand2;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand3;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand4;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand5;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand6;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand WideLatin;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand12;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand8;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand9;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand10;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand1;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand11;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
    }
}