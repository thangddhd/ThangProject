namespace coms.COMSK.ui
{
    partial class K300040010
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(K300040010));
            this.splitContainerRepairList = new System.Windows.Forms.SplitContainer();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDiff = new System.Windows.Forms.Button();
            this.btnTempBox = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnOutputRepairPlan = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.txtKumiaiName = new System.Windows.Forms.TextBox();
            this.txtDatumYear = new System.Windows.Forms.TextBox();
            this.txtStandardRepairPlanName = new System.Windows.Forms.TextBox();
            this.txtKumiaiRepairPlanName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.xtraTabControl_RepairList2 = new System.Windows.Forms.TabControl();
            this.xtraTabPage_TempBuilding2 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_T = new coms.COMSK.ui.common.CtrRepairPlan_B();
            this.xtraTabPage_Building2 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_B = new coms.COMSK.ui.common.CtrRepairPlan_B();
            this.xtraTabPage_Equipment2 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_E = new coms.COMSK.ui.common.CtrRepairPlan_E();
            this.xtraTabPage_Outer2 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_Out = new coms.COMSK.ui.common.CtrRepairPlan_B();
            this.xtraTabPage_Other2 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_Other = new coms.COMSK.ui.common.CtrRepairPlan_B();
            this.chkFilterDEV = new coms.COMMON.ui.CheckedComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRepairList)).BeginInit();
            this.splitContainerRepairList.Panel1.SuspendLayout();
            this.splitContainerRepairList.Panel2.SuspendLayout();
            this.splitContainerRepairList.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.xtraTabControl_RepairList2.SuspendLayout();
            this.xtraTabPage_TempBuilding2.SuspendLayout();
            this.xtraTabPage_Building2.SuspendLayout();
            this.xtraTabPage_Equipment2.SuspendLayout();
            this.xtraTabPage_Outer2.SuspendLayout();
            this.xtraTabPage_Other2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerRepairList
            // 
            this.splitContainerRepairList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRepairList.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerRepairList.IsSplitterFixed = true;
            this.splitContainerRepairList.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRepairList.Name = "splitContainerRepairList";
            this.splitContainerRepairList.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerRepairList.Panel1
            // 
            this.splitContainerRepairList.Panel1.Controls.Add(this.chkFilterDEV);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnUpdate);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnDiff);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnTempBox);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnRefresh);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnOutputRepairPlan);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnClose);
            this.splitContainerRepairList.Panel1.Controls.Add(this.panelSearch);
            // 
            // splitContainerRepairList.Panel2
            // 
            this.splitContainerRepairList.Panel2.Controls.Add(this.xtraTabControl_RepairList2);
            this.splitContainerRepairList.Size = new System.Drawing.Size(948, 715);
            this.splitContainerRepairList.SplitterDistance = 141;
            this.splitContainerRepairList.TabIndex = 0;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnUpdate.Location = new System.Drawing.Point(823, 105);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(120, 23);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "保存して閉じる";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDiff
            // 
            this.btnDiff.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnDiff.Location = new System.Drawing.Point(277, 105);
            this.btnDiff.Name = "btnDiff";
            this.btnDiff.Size = new System.Drawing.Size(120, 23);
            this.btnDiff.TabIndex = 3;
            this.btnDiff.Text = "現作成基準との比較";
            this.btnDiff.UseVisualStyleBackColor = true;
            this.btnDiff.Click += new System.EventHandler(this.btnDiff_Click);
            // 
            // btnTempBox
            // 
            this.btnTempBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnTempBox.Location = new System.Drawing.Point(403, 105);
            this.btnTempBox.Name = "btnTempBox";
            this.btnTempBox.Size = new System.Drawing.Size(120, 23);
            this.btnTempBox.TabIndex = 4;
            this.btnTempBox.Text = "参考基準参照";
            this.btnTempBox.UseVisualStyleBackColor = true;
            this.btnTempBox.Click += new System.EventHandler(this.btnTempBox_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(139, 105);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(76, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "再表示";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnOutputRepairPlan
            // 
            this.btnOutputRepairPlan.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOutputRepairPlan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnOutputRepairPlan.Location = new System.Drawing.Point(529, 105);
            this.btnOutputRepairPlan.Name = "btnOutputRepairPlan";
            this.btnOutputRepairPlan.Size = new System.Drawing.Size(120, 23);
            this.btnOutputRepairPlan.TabIndex = 5;
            this.btnOutputRepairPlan.Text = "Excel出力";
            this.btnOutputRepairPlan.UseVisualStyleBackColor = false;
            this.btnOutputRepairPlan.Click += new System.EventHandler(this.btnOutputRepairPlan_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnClose.Location = new System.Drawing.Point(697, 105);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "保存";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panelSearch
            // 
            this.panelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSearch.Controls.Add(this.txtKumiaiName);
            this.panelSearch.Controls.Add(this.txtDatumYear);
            this.panelSearch.Controls.Add(this.txtStandardRepairPlanName);
            this.panelSearch.Controls.Add(this.txtKumiaiRepairPlanName);
            this.panelSearch.Controls.Add(this.label3);
            this.panelSearch.Controls.Add(this.label2);
            this.panelSearch.Controls.Add(this.label4);
            this.panelSearch.Controls.Add(this.label1);
            this.panelSearch.Location = new System.Drawing.Point(12, 12);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(931, 88);
            this.panelSearch.TabIndex = 0;
            // 
            // txtKumiaiName
            // 
            this.txtKumiaiName.Location = new System.Drawing.Point(111, 18);
            this.txtKumiaiName.MaxLength = 100;
            this.txtKumiaiName.Name = "txtKumiaiName";
            this.txtKumiaiName.ReadOnly = true;
            this.txtKumiaiName.Size = new System.Drawing.Size(297, 19);
            this.txtKumiaiName.TabIndex = 1;
            // 
            // txtDatumYear
            // 
            this.txtDatumYear.ImeMode = System.Windows.Forms.ImeMode.Close;
            this.txtDatumYear.Location = new System.Drawing.Point(528, 48);
            this.txtDatumYear.MaxLength = 4;
            this.txtDatumYear.Name = "txtDatumYear";
            this.txtDatumYear.ReadOnly = true;
            this.txtDatumYear.Size = new System.Drawing.Size(151, 19);
            this.txtDatumYear.TabIndex = 7;
            // 
            // txtStandardRepairPlanName
            // 
            this.txtStandardRepairPlanName.Location = new System.Drawing.Point(528, 18);
            this.txtStandardRepairPlanName.MaxLength = 100;
            this.txtStandardRepairPlanName.Name = "txtStandardRepairPlanName";
            this.txtStandardRepairPlanName.ReadOnly = true;
            this.txtStandardRepairPlanName.Size = new System.Drawing.Size(364, 19);
            this.txtStandardRepairPlanName.TabIndex = 3;
            // 
            // txtKumiaiRepairPlanName
            // 
            this.txtKumiaiRepairPlanName.Location = new System.Drawing.Point(111, 48);
            this.txtKumiaiRepairPlanName.MaxLength = 100;
            this.txtKumiaiRepairPlanName.Name = "txtKumiaiRepairPlanName";
            this.txtKumiaiRepairPlanName.ReadOnly = true;
            this.txtKumiaiRepairPlanName.Size = new System.Drawing.Size(297, 19);
            this.txtKumiaiRepairPlanName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(433, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "標準作成基準名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "長期修繕計画名";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(433, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "作成開始年度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "物件";
            // 
            // xtraTabControl_RepairList2
            // 
            this.xtraTabControl_RepairList2.Controls.Add(this.xtraTabPage_TempBuilding2);
            this.xtraTabControl_RepairList2.Controls.Add(this.xtraTabPage_Building2);
            this.xtraTabControl_RepairList2.Controls.Add(this.xtraTabPage_Equipment2);
            this.xtraTabControl_RepairList2.Controls.Add(this.xtraTabPage_Outer2);
            this.xtraTabControl_RepairList2.Controls.Add(this.xtraTabPage_Other2);
            this.xtraTabControl_RepairList2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl_RepairList2.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl_RepairList2.Name = "xtraTabControl_RepairList2";
            this.xtraTabControl_RepairList2.SelectedIndex = 0;
            this.xtraTabControl_RepairList2.Size = new System.Drawing.Size(948, 570);
            this.xtraTabControl_RepairList2.TabIndex = 1;
            this.xtraTabControl_RepairList2.SelectedIndexChanged += new System.EventHandler(this.xtraTabControl_RepairList2_SelectedIndexChanged);
            // 
            // xtraTabPage_TempBuilding2
            // 
            this.xtraTabPage_TempBuilding2.Controls.Add(this.ctrRepairPlan_T);
            this.xtraTabPage_TempBuilding2.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_TempBuilding2.Name = "xtraTabPage_TempBuilding2";
            this.xtraTabPage_TempBuilding2.Padding = new System.Windows.Forms.Padding(3);
            this.xtraTabPage_TempBuilding2.Size = new System.Drawing.Size(940, 544);
            this.xtraTabPage_TempBuilding2.TabIndex = 0;
            this.xtraTabPage_TempBuilding2.Text = "仮設";
            this.xtraTabPage_TempBuilding2.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_T
            // 
            this.ctrRepairPlan_T.DataSource = null;
            this.ctrRepairPlan_T.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_T.Filter = null;
            this.ctrRepairPlan_T.Location = new System.Drawing.Point(3, 3);
            this.ctrRepairPlan_T.Name = "ctrRepairPlan_T";
            this.ctrRepairPlan_T.Size = new System.Drawing.Size(934, 538);
            this.ctrRepairPlan_T.TabIndex = 0;
            this.ctrRepairPlan_T.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            this.ctrRepairPlan_T.Load += new System.EventHandler(this.ctrRepairPlan_T_Load);
            // 
            // xtraTabPage_Building2
            // 
            this.xtraTabPage_Building2.Controls.Add(this.ctrRepairPlan_B);
            this.xtraTabPage_Building2.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_Building2.Name = "xtraTabPage_Building2";
            this.xtraTabPage_Building2.Padding = new System.Windows.Forms.Padding(3);
            this.xtraTabPage_Building2.Size = new System.Drawing.Size(940, 544);
            this.xtraTabPage_Building2.TabIndex = 1;
            this.xtraTabPage_Building2.Text = "建築";
            this.xtraTabPage_Building2.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_B
            // 
            this.ctrRepairPlan_B.DataSource = null;
            this.ctrRepairPlan_B.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_B.Filter = null;
            this.ctrRepairPlan_B.Location = new System.Drawing.Point(3, 3);
            this.ctrRepairPlan_B.Name = "ctrRepairPlan_B";
            this.ctrRepairPlan_B.Size = new System.Drawing.Size(934, 538);
            this.ctrRepairPlan_B.TabIndex = 0;
            this.ctrRepairPlan_B.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            // 
            // xtraTabPage_Equipment2
            // 
            this.xtraTabPage_Equipment2.Controls.Add(this.ctrRepairPlan_E);
            this.xtraTabPage_Equipment2.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_Equipment2.Name = "xtraTabPage_Equipment2";
            this.xtraTabPage_Equipment2.Size = new System.Drawing.Size(940, 544);
            this.xtraTabPage_Equipment2.TabIndex = 2;
            this.xtraTabPage_Equipment2.Text = "設備";
            this.xtraTabPage_Equipment2.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_E
            // 
            this.ctrRepairPlan_E.DataSource = null;
            this.ctrRepairPlan_E.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_E.Filter = null;
            this.ctrRepairPlan_E.Location = new System.Drawing.Point(0, 0);
            this.ctrRepairPlan_E.Name = "ctrRepairPlan_E";
            this.ctrRepairPlan_E.Size = new System.Drawing.Size(940, 544);
            this.ctrRepairPlan_E.TabIndex = 0;
            this.ctrRepairPlan_E.Adding += new System.EventHandler(this.ctrRepairPlan_E_Adding);
            // 
            // xtraTabPage_Outer2
            // 
            this.xtraTabPage_Outer2.Controls.Add(this.ctrRepairPlan_Out);
            this.xtraTabPage_Outer2.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_Outer2.Name = "xtraTabPage_Outer2";
            this.xtraTabPage_Outer2.Size = new System.Drawing.Size(940, 544);
            this.xtraTabPage_Outer2.TabIndex = 3;
            this.xtraTabPage_Outer2.Text = "外構";
            this.xtraTabPage_Outer2.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_Out
            // 
            this.ctrRepairPlan_Out.DataSource = null;
            this.ctrRepairPlan_Out.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_Out.Filter = null;
            this.ctrRepairPlan_Out.Location = new System.Drawing.Point(0, 0);
            this.ctrRepairPlan_Out.Name = "ctrRepairPlan_Out";
            this.ctrRepairPlan_Out.Size = new System.Drawing.Size(940, 544);
            this.ctrRepairPlan_Out.TabIndex = 0;
            this.ctrRepairPlan_Out.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            // 
            // xtraTabPage_Other2
            // 
            this.xtraTabPage_Other2.Controls.Add(this.ctrRepairPlan_Other);
            this.xtraTabPage_Other2.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_Other2.Name = "xtraTabPage_Other2";
            this.xtraTabPage_Other2.Size = new System.Drawing.Size(940, 544);
            this.xtraTabPage_Other2.TabIndex = 4;
            this.xtraTabPage_Other2.Text = "その他";
            this.xtraTabPage_Other2.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_Other
            // 
            this.ctrRepairPlan_Other.DataSource = null;
            this.ctrRepairPlan_Other.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_Other.Filter = null;
            this.ctrRepairPlan_Other.Location = new System.Drawing.Point(0, 0);
            this.ctrRepairPlan_Other.Name = "ctrRepairPlan_Other";
            this.ctrRepairPlan_Other.Size = new System.Drawing.Size(940, 544);
            this.ctrRepairPlan_Other.TabIndex = 0;
            this.ctrRepairPlan_Other.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            // 
            // chkFilterDEV
            // 
            this.chkFilterDEV.CheckOnClick = true;
            this.chkFilterDEV.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.chkFilterDEV.DropDownHeight = 1;
            this.chkFilterDEV.FormattingEnabled = true;
            this.chkFilterDEV.IntegralHeight = false;
            this.chkFilterDEV.Location = new System.Drawing.Point(12, 107);
            this.chkFilterDEV.Name = "chkFilterDEV";
            this.chkFilterDEV.Size = new System.Drawing.Size(121, 20);
            this.chkFilterDEV.TabIndex = 8;
            this.chkFilterDEV.ValueSeparator = ", ";
            // 
            // K300040010
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(948, 715);
            this.Controls.Add(this.splitContainerRepairList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "K300040010";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修繕基準";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.K300040010_FormClosing);
            this.Load += new System.EventHandler(this.K200010040_Load);
            this.splitContainerRepairList.Panel1.ResumeLayout(false);
            this.splitContainerRepairList.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRepairList)).EndInit();
            this.splitContainerRepairList.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.xtraTabControl_RepairList2.ResumeLayout(false);
            this.xtraTabPage_TempBuilding2.ResumeLayout(false);
            this.xtraTabPage_Building2.ResumeLayout(false);
            this.xtraTabPage_Equipment2.ResumeLayout(false);
            this.xtraTabPage_Outer2.ResumeLayout(false);
            this.xtraTabPage_Other2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerRepairList;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOutputRepairPlan;
        private System.Windows.Forms.TextBox txtStandardRepairPlanName;
        private System.Windows.Forms.TextBox txtKumiaiRepairPlanName;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnUpdate;
		private coms.COMSK.ui.common.CtrRepairPlan_B ctrRepairPlan_B;
		private coms.COMSK.ui.common.CtrRepairPlan_E ctrRepairPlan_E;
		private coms.COMSK.ui.common.CtrRepairPlan_B ctrRepairPlan_T;
		private coms.COMSK.ui.common.CtrRepairPlan_B ctrRepairPlan_Out;
		private coms.COMSK.ui.common.CtrRepairPlan_B ctrRepairPlan_Other;
		private System.Windows.Forms.TextBox txtKumiaiName;
		private System.Windows.Forms.Button btnTempBox;
        private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.Button btnDiff;
		private System.Windows.Forms.TextBox txtDatumYear;
        private System.Windows.Forms.TabControl xtraTabControl_RepairList2;
        private System.Windows.Forms.TabPage xtraTabPage_TempBuilding2;
        private System.Windows.Forms.TabPage xtraTabPage_Building2;
        private System.Windows.Forms.TabPage xtraTabPage_Equipment2;
        private System.Windows.Forms.TabPage xtraTabPage_Outer2;
        private System.Windows.Forms.TabPage xtraTabPage_Other2;
        private COMMON.ui.CheckedComboBox chkFilterDEV;
    }
}