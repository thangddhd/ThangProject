
namespace coms.COMSK.ui
{
	partial class K300010020
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(K300010020));
            this.splitContainerRepairList = new System.Windows.Forms.SplitContainer();
            this.chkDisplayDeleted = new System.Windows.Forms.CheckBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnTempBox = new System.Windows.Forms.Button();
            this.btnOutputRepairPlan = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.btnApprove = new System.Windows.Forms.Button();
            this.chkTakeChangeHistory = new System.Windows.Forms.CheckBox();
            this.txtPlanName = new System.Windows.Forms.TextBox();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.xtraTabPage_Temp1 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_T = new coms.COMSK.ui.common.CtrBasicRepairPlan_B();
            this.xtraTabPage_Building1 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_B = new coms.COMSK.ui.common.CtrBasicRepairPlan_B();
            this.xtraTabPage_Equipment1 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_E = new coms.COMSK.ui.common.CtrBasicRepairPlan_E();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_Out = new coms.COMSK.ui.common.CtrBasicRepairPlan_B();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.ctrRepairPlan_Other = new coms.COMSK.ui.common.CtrBasicRepairPlan_B();
            this.errSummary = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRepairList)).BeginInit();
            this.splitContainerRepairList.Panel1.SuspendLayout();
            this.splitContainerRepairList.Panel2.SuspendLayout();
            this.splitContainerRepairList.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.xtraTabPage_Temp1.SuspendLayout();
            this.xtraTabPage_Building1.SuspendLayout();
            this.xtraTabPage_Equipment1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errSummary)).BeginInit();
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
            this.splitContainerRepairList.Panel1.Controls.Add(this.chkDisplayDeleted);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnUpdate);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnRefresh);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnTempBox);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnOutputRepairPlan);
            this.splitContainerRepairList.Panel1.Controls.Add(this.btnClose);
            this.splitContainerRepairList.Panel1.Controls.Add(this.panelSearch);
            // 
            // splitContainerRepairList.Panel2
            // 
            this.splitContainerRepairList.Panel2.Controls.Add(this.tabControl1);
            this.splitContainerRepairList.Size = new System.Drawing.Size(984, 715);
            this.splitContainerRepairList.SplitterDistance = 141;
            this.splitContainerRepairList.TabIndex = 0;
            // 
            // chkDisplayDeleted
            // 
            this.chkDisplayDeleted.AutoSize = true;
            this.chkDisplayDeleted.Location = new System.Drawing.Point(12, 112);
            this.chkDisplayDeleted.Name = "chkDisplayDeleted";
            this.chkDisplayDeleted.Size = new System.Drawing.Size(105, 16);
            this.chkDisplayDeleted.TabIndex = 0;
            this.chkDisplayDeleted.Text = "削除項目も表示";
            this.chkDisplayDeleted.UseVisualStyleBackColor = true;
            this.chkDisplayDeleted.CheckedChanged += new System.EventHandler(this.chkDisplayDeleted_CheckedChanged);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnUpdate.Location = new System.Drawing.Point(726, 106);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(120, 23);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "保存";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(142, 106);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "再表示";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnTempBox
            // 
            this.btnTempBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnTempBox.Location = new System.Drawing.Point(392, 106);
            this.btnTempBox.Name = "btnTempBox";
            this.btnTempBox.Size = new System.Drawing.Size(120, 23);
            this.btnTempBox.TabIndex = 2;
            this.btnTempBox.Text = "参考基準";
            this.btnTempBox.UseVisualStyleBackColor = true;
            this.btnTempBox.Click += new System.EventHandler(this.btnTempBox_Click);
            // 
            // btnOutputRepairPlan
            // 
            this.btnOutputRepairPlan.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOutputRepairPlan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnOutputRepairPlan.Location = new System.Drawing.Point(518, 106);
            this.btnOutputRepairPlan.Name = "btnOutputRepairPlan";
            this.btnOutputRepairPlan.Size = new System.Drawing.Size(120, 23);
            this.btnOutputRepairPlan.TabIndex = 3;
            this.btnOutputRepairPlan.Text = "Excel出力";
            this.btnOutputRepairPlan.UseVisualStyleBackColor = false;
            this.btnOutputRepairPlan.Click += new System.EventHandler(this.btnOutputRepairPlan_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(852, 106);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelSearch
            // 
            this.panelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSearch.Controls.Add(this.btnApprove);
            this.panelSearch.Controls.Add(this.chkTakeChangeHistory);
            this.panelSearch.Controls.Add(this.txtPlanName);
            this.panelSearch.Controls.Add(this.txtNote);
            this.panelSearch.Controls.Add(this.label3);
            this.panelSearch.Controls.Add(this.label2);
            this.panelSearch.Location = new System.Drawing.Point(12, 12);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(1039, 88);
            this.panelSearch.TabIndex = 0;
            // 
            // btnApprove
            // 
            this.btnApprove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApprove.BackColor = System.Drawing.Color.Red;
            this.btnApprove.ForeColor = System.Drawing.Color.White;
            this.btnApprove.Location = new System.Drawing.Point(839, 7);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(120, 23);
            this.btnApprove.TabIndex = 3;
            this.btnApprove.Text = "承認";
            this.btnApprove.UseVisualStyleBackColor = false;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // chkTakeChangeHistory
            // 
            this.chkTakeChangeHistory.AutoSize = true;
            this.chkTakeChangeHistory.Location = new System.Drawing.Point(498, 14);
            this.chkTakeChangeHistory.Name = "chkTakeChangeHistory";
            this.chkTakeChangeHistory.Size = new System.Drawing.Size(105, 16);
            this.chkTakeChangeHistory.TabIndex = 2;
            this.chkTakeChangeHistory.Text = "変更履歴を取得";
            this.chkTakeChangeHistory.UseVisualStyleBackColor = true;
            this.chkTakeChangeHistory.CheckedChanged += new System.EventHandler(this.chkTakeChangeHistory_CheckedChanged);
            // 
            // txtPlanName
            // 
            this.txtPlanName.BackColor = System.Drawing.Color.Pink;
            this.txtPlanName.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.txtPlanName.Location = new System.Drawing.Point(111, 11);
            this.txtPlanName.Name = "txtPlanName";
            this.txtPlanName.Size = new System.Drawing.Size(364, 19);
            this.txtPlanName.TabIndex = 1;
            // 
            // txtNote
            // 
            this.txtNote.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.txtNote.Location = new System.Drawing.Point(111, 36);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNote.Size = new System.Drawing.Size(848, 47);
            this.txtNote.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "標準作成基準名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "備考";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.xtraTabPage_Temp1);
            this.tabControl1.Controls.Add(this.xtraTabPage_Building1);
            this.tabControl1.Controls.Add(this.xtraTabPage_Equipment1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(984, 570);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // xtraTabPage_Temp1
            // 
            this.xtraTabPage_Temp1.BackColor = System.Drawing.Color.Transparent;
            this.xtraTabPage_Temp1.Controls.Add(this.ctrRepairPlan_T);
            this.xtraTabPage_Temp1.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_Temp1.Name = "xtraTabPage_Temp1";
            this.xtraTabPage_Temp1.Padding = new System.Windows.Forms.Padding(3);
            this.xtraTabPage_Temp1.Size = new System.Drawing.Size(976, 544);
            this.xtraTabPage_Temp1.TabIndex = 0;
            this.xtraTabPage_Temp1.Text = "仮設";
            // 
            // ctrRepairPlan_T
            // 
            this.ctrRepairPlan_T.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_T.Location = new System.Drawing.Point(3, 3);
            this.ctrRepairPlan_T.Name = "ctrRepairPlan_T";
            this.ctrRepairPlan_T.Size = new System.Drawing.Size(970, 538);
            this.ctrRepairPlan_T.StdRepairPlanPid = ((long)(0));
            this.ctrRepairPlan_T.TabIndex = 0;
            this.ctrRepairPlan_T.TakeHistory = false;
            this.ctrRepairPlan_T.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            // 
            // xtraTabPage_Building1
            // 
            this.xtraTabPage_Building1.Controls.Add(this.ctrRepairPlan_B);
            this.xtraTabPage_Building1.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_Building1.Name = "xtraTabPage_Building1";
            this.xtraTabPage_Building1.Padding = new System.Windows.Forms.Padding(3);
            this.xtraTabPage_Building1.Size = new System.Drawing.Size(976, 544);
            this.xtraTabPage_Building1.TabIndex = 1;
            this.xtraTabPage_Building1.Text = "建築";
            this.xtraTabPage_Building1.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_B
            // 
            this.ctrRepairPlan_B.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolTip;
            this.ctrRepairPlan_B.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_B.Location = new System.Drawing.Point(3, 3);
            this.ctrRepairPlan_B.Name = "ctrRepairPlan_B";
            this.ctrRepairPlan_B.Size = new System.Drawing.Size(970, 538);
            this.ctrRepairPlan_B.StdRepairPlanPid = ((long)(0));
            this.ctrRepairPlan_B.TabIndex = 0;
            this.ctrRepairPlan_B.TakeHistory = false;
            this.ctrRepairPlan_B.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            // 
            // xtraTabPage_Equipment1
            // 
            this.xtraTabPage_Equipment1.Controls.Add(this.ctrRepairPlan_E);
            this.xtraTabPage_Equipment1.Location = new System.Drawing.Point(4, 22);
            this.xtraTabPage_Equipment1.Name = "xtraTabPage_Equipment1";
            this.xtraTabPage_Equipment1.Size = new System.Drawing.Size(575, 383);
            this.xtraTabPage_Equipment1.TabIndex = 2;
            this.xtraTabPage_Equipment1.Text = "設備";
            this.xtraTabPage_Equipment1.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_E
            // 
            this.ctrRepairPlan_E.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_E.Location = new System.Drawing.Point(0, 0);
            this.ctrRepairPlan_E.Name = "ctrRepairPlan_E";
            this.ctrRepairPlan_E.Size = new System.Drawing.Size(575, 383);
            this.ctrRepairPlan_E.StdRepairPlanPid = ((long)(0));
            this.ctrRepairPlan_E.TabIndex = 0;
            this.ctrRepairPlan_E.TakeHistory = false;
            this.ctrRepairPlan_E.Adding += new System.EventHandler(this.ctrRepairPlan_E_Adding);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.ctrRepairPlan_Out);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(575, 383);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "外構";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_Out
            // 
            this.ctrRepairPlan_Out.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_Out.Location = new System.Drawing.Point(0, 0);
            this.ctrRepairPlan_Out.Name = "ctrRepairPlan_Out";
            this.ctrRepairPlan_Out.Size = new System.Drawing.Size(575, 383);
            this.ctrRepairPlan_Out.StdRepairPlanPid = ((long)(0));
            this.ctrRepairPlan_Out.TabIndex = 0;
            this.ctrRepairPlan_Out.TakeHistory = false;
            this.ctrRepairPlan_Out.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ctrRepairPlan_Other);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(575, 383);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "その他";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // ctrRepairPlan_Other
            // 
            this.ctrRepairPlan_Other.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrRepairPlan_Other.Location = new System.Drawing.Point(0, 0);
            this.ctrRepairPlan_Other.Name = "ctrRepairPlan_Other";
            this.ctrRepairPlan_Other.Size = new System.Drawing.Size(575, 383);
            this.ctrRepairPlan_Other.StdRepairPlanPid = ((long)(0));
            this.ctrRepairPlan_Other.TabIndex = 0;
            this.ctrRepairPlan_Other.TakeHistory = false;
            this.ctrRepairPlan_Other.Adding += new System.EventHandler(this.ctrRepairPlan_Adding);
            // 
            // errSummary
            // 
            this.errSummary.ContainerControl = this;
            // 
            // K300010020
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 715);
            this.Controls.Add(this.splitContainerRepairList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "K300010020";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "標準作成基準";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.K300010020_FormClosed);
            this.Load += new System.EventHandler(this.K200010100_Load);
            this.splitContainerRepairList.Panel1.ResumeLayout(false);
            this.splitContainerRepairList.Panel1.PerformLayout();
            this.splitContainerRepairList.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRepairList)).EndInit();
            this.splitContainerRepairList.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.xtraTabPage_Temp1.ResumeLayout(false);
            this.xtraTabPage_Building1.ResumeLayout(false);
            this.xtraTabPage_Equipment1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errSummary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerRepairList;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnOutputRepairPlan;
        private System.Windows.Forms.TextBox txtPlanName;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Button btnTempBox;
		private coms.COMSK.ui.common.CtrBasicRepairPlan_B ctrRepairPlan_B;
		private coms.COMSK.ui.common.CtrBasicRepairPlan_B ctrRepairPlan_T;
		private coms.COMSK.ui.common.CtrBasicRepairPlan_E ctrRepairPlan_E;
		private coms.COMSK.ui.common.CtrBasicRepairPlan_B ctrRepairPlan_Out;
		private coms.COMSK.ui.common.CtrBasicRepairPlan_B ctrRepairPlan_Other;
		private System.Windows.Forms.CheckBox chkDisplayDeleted;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.CheckBox chkTakeChangeHistory;
		private System.Windows.Forms.Button btnApprove;
		private System.Windows.Forms.ErrorProvider errSummary;
        private System.Windows.Forms.TabPage xtraTabPage_Temp1;
        private System.Windows.Forms.TabPage xtraTabPage_Building1;
        private System.Windows.Forms.TabPage xtraTabPage_Equipment1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabControl tabControl1;
    }
}