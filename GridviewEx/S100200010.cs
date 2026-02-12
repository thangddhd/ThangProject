using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using coms.COMMON;
using coms.COMMON.ui;
using coms.COMSSService;
using coms.COMMONService;
using coms.COMSS.business;
using coms.COMMON.Utility;

namespace coms.COMSS.ui.S100200
{
    public partial class S100200010 : BaseReportForm
    {
        string screenID = "S100200010";

        string[] PATTERN2_COLUMN = new string[] { "作成", "協力会社", "組合名", "営業担当", "棟名", "作業ID", "作業", "作業名", "予定年月", "予定日", "点検入力", "予定時刻", "原価（税込））", "確認書/請書", "捺印済み確認書請書", "協力会社毎予定表", "捺印済み建物毎予定表", "協力会社宛請求書/請書" };
        int[] PATTTERN2_COLUMN_INDEX = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

        string[] PATTERN1_COLUMN = new string[] { "作成", "協力会社", "組合名", "営業担当", "棟名", "作業ID", "作業", "作業名", "予定年月", "予定日", "点検入力", "予定時刻", "原価（税込）", "確認書/請書", "捺印済み確認書請書", "建物毎予定表", "捺印済み建物毎予定表", "協力会社宛請求書/請書" };
        int[] PATTTERN1_COLUMN_INDEX = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

        string[] PATTERN3_COLUMN = new string[] { "作成", "協力会社", "組合名", "営業担当", "棟名", "作業ID", "作業", "作業名", "予定年月", "予定日", "点検入力", "予定時刻", "原価（税込）", "確認書/請書", "捺印済み確認書請書", "建物毎予定表", "捺印済み建物毎予定表", "協力会社宛請求書/請書" };
        int[] PATTTERN3_COLUMN_INDEX = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

        int total;
        int searchTypeIndex = 0; 

        public PATTERNINDEX patternIndex;
        public bool isB0070 = false;

        List<COMSSService.CommisionWorkPlanInfoResult> list;        
        COMSSService.CommisionWorkPlanInfoCondition condtion;
        WorkListDisplayPattern displayPattern;

        public DateTime targetMonth;
        public string defaultObjectType = string.Empty;
        HashSet<string> ignoreAutoFormatColumns;
        bool dataLoaded = true;

        public S100200010()
        {
            InitializeComponent();
            this.InitialOtherInfo();
        }

        private void InitialOtherInfo()
        {
            dgvCommon.ButtonIconNeeded += dgvCommon_ButtonIconNeeded;
            dgvCommon.CellBackColorNeeded += dgvCommon_CellBackColorNeeded;
            dgvCommon.TextBoxIconNeeded += dgvCommon_TextBoxIconNeeded;
        }

        private void BindDate(DateTime dateTime, bool isFrom)
        {
            if (isFrom)
            {
                dtpDueDateFrom.Value = dateTime;
            }
            else
            {
                dtpDueDateTo.Value = dateTime;
            }
        }
        private void btnPreviousMonth_Click(object sender, EventArgs e)
        {
            if (cmbSearchType.SelectedIndex == 0)
            {
                DateTime dateTime = Helper.GetBeginDayOfPreviousMonth();
                BindDate(dateTime, true);
                dateTime = Helper.GetEndDayOfPreviousMonth();
                BindDate(dateTime, false);
            }
            else
            {
                DateTime dateTime = DateTime.Now.AddMonths(-3);
                dtpDueDateFrom.Value = Helper.GetBeginDayOfMonth(dateTime);
                dateTime = Helper.GetEndDayOfMonth(dateTime);
                dtpDueDateTo.Value = dateTime;
            }
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            if (cmbSearchType.SelectedIndex == 0)
            {
                DateTime dateTime = Helper.GetBeginDayOfThisMonth();
                BindDate(dateTime, true);
                dateTime = Helper.GetEndDayOfThisMonth();
                BindDate(dateTime, false);
            }
            else
            {
                DateTime dateTime = DateTime.Now.AddMonths(-2);
                dtpDueDateFrom.Value = Helper.GetBeginDayOfMonth(dateTime);
                dateTime = Helper.GetEndDayOfMonth(dateTime);
                dtpDueDateTo.Value = dateTime;
            }
        }

        private void btnNextMonth_Click(object sender, EventArgs e)
        {
            if (cmbSearchType.SelectedIndex == 0)
            {
                DateTime dateTime = Helper.GetBeginDayOfNextMonth();
                BindDate(dateTime, true);
                dateTime = Helper.GetEndDayOfNextMonth();
                BindDate(dateTime, false);
            }
            else
            {
                DateTime dateTime = Helper.GetBeginDayOfPreviousMonth();
                dtpDueDateFrom.Value = dateTime;
                dateTime = Helper.GetEndDayOfPreviousMonth();
                dtpDueDateTo.Value = dateTime;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "CSV File (*.csv)|*.csv";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //string fileName = dlg.FileName;
                    //KumarinK.business.K100001.K100001BL bl = new KumarinK.business.K100001.K100001BL();
                    //bl.ImportData(fileName);

                    //SearchData();
                }

            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex);
            }

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "CSV File (*.csv)|*.csv|All files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dlg.FileName;
                    //KumarinK.business.K100001.K100001BL bl = new KumarinK.business.K100001.K100001BL();

                    //if (this.list != null && this.list.Length > 0)
                    //{
                    //    bl.ExportData(fileName, list);
                    //}
                }
            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string error = IsValid();
            if (error == string.Empty)
            {
                searchTypeIndex = cmbSearchType.SelectedIndex;
                itemIndexList.Clear();
                onFlg = false;
                //field = " businessConnectionsMstPid, KumiaiInfoPid, UserName, buildingName ";
                SearchDataAjax();
            }
            else
            {                
                MessageBox.Show(error, "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string IsValid()
        {
            if (cmbSearchType.SelectedIndex < 0)
            {
                return "日付区別を選択してください。";                
            }
            else if (dtpDueDateFrom.Value == DateTime.MinValue || dtpDueDateTo.Value == DateTime.MinValue)
            {
                return "予定年月日もしくは実施年月日を入力して下さい。";
            }
            return string.Empty;
        }


        public void SearchDataAjax()
        {
            if (!this.backgroundWorker1.IsBusy)
            {
                condtion = GetCondition();
                Helper.ShowLoadingPopup();
                this.backgroundWorker1.RunWorkerAsync();
            }

        }
        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Search();
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataLoaded = false;
            BindData();
            //ReBindingSelectedItem();
            Helper.HideLoadingPopup();
            dataLoaded = true;
        }

        private void ReBindingSelectedItem()
        {
            foreach (int i in itemIndexList)
            {
                //(dgvCommon.GetRow(i) as CommisionWorkPlanInfoResult).CreateFile = true;
                //dgvCommon.SetRowCellValue(i, clCreate, true);
                //dgvCommon.Invalidate();
            }
            dgvCommon.Invalidate();
            dgvCommon.Refresh();
        }

        private COMSSService.CommisionWorkPlanInfoCondition GetCondition()
        {
            COMSSService.CommisionWorkPlanInfoCondition condition = new COMSSService.CommisionWorkPlanInfoCondition();
            condition.BusinessConnectionsBranchMstPid = this.ctrBusinessConnections.CurrentBusinessConnectionsBranchMst != null ? this.ctrBusinessConnections.CurrentBusinessConnectionsBranchMst.Pid : long.MinValue;
            condition.KumiaiInfoPid = ctrKumiai.CurrentKumiaiInfo != null ? ctrKumiai.CurrentKumiaiInfo.Pid : long.MinValue;
            condition.PlanPid = Helper.ParseToLong(txtPlanPid.Text.Trim());            
            condition.TeamCode = ctrStaff.TeamCode;
            condition.UserMstPid = ctrStaff.UserId;

            DateTime dateto = dtpDueDateTo.Value;
            if (dateto != DateTime.MinValue)
            {
                dateto = (dateto < DateTime.MaxValue.AddDays(-1)) ? dateto.AddDays(1) : DateTime.MaxValue;
                condition.WorkPlanDueDateTo = dateto;
            }

            if (cmbSearchType.SelectedIndex == 0)
            {
                condition.WorkPlanDueDateFrom = dtpDueDateFrom.Value;
                condition.WorkPlanDueDateTo = dateto;
            }
            else
            {
                condition.WorkResultDateFrom = dtpDueDateFrom.Value;
                condition.WorkResultDateTo = dateto;
            }
            condition.Pattern = Constant.BC_PATTERN;
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える 20200416 kubota
            //condition.PlanType = cmbPlanType.GetSelectedValues();
            condition.ObjectType = cmbObjectType.SelectedValue.ToString();
            condition.WorkType = cmbWorkType.SelectedValue.ToString();
            condition.ContractType = cmbContactType.SelectedValue.ToString();

            condition.IsNotUpload = chkUpload.Checked;
            condition.IsPlanResultDateNull = chkNotFinish.Checked;
            condition.NotInputDate = chkNotInputDate.Checked;

            //condition.OwnerType = cmbOwnerType.GetSelectedValues();
            //condition.BlockCode = cmbBlockCode.GetSelectedValues();
            condition.WorkFlg = (cmbWorkFlg.SelectedIndex == 1 ? "1" : cmbWorkFlg.SelectedIndex == 2 ? "0" : "");
            condition.PlanType = cmbPlanTypeDEV.GetSelectedValues();
            condition.OwnerType = cmbOwnerTypeDEV.GetSelectedValues();
            condition.BlockCode = cmbBlockCodeDEV.GetSelectedValues();
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える end
            return condition;
        }
        private void Search()
        {

            int startIndex = ctrPage1.CurrentPageIndex * ctrPage1.PageSize + 1;
            try
            {
                list = (new COMSS.business.S100001BL()).SearchCommisionWorkPlanInfoByCondition(condtion, startIndex, ctrPage1.PageSize, out total);
            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
            }
        }

        private void BindData()
        {
            BindingList<CommisionWorkPlanInfoResultEx> subList = new BindingList<CommisionWorkPlanInfoResultEx>();
            if (list != null)
            {
                var searchResultEx = ObjectExtensions.MakeCopyList(list);
                foreach (var item in searchResultEx)
                    subList.Add(item);

            }

            BindingSource bd = new BindingSource();
            bd.DataSource = subList;
            bd.ResetBindings(false);
            int index = dgvCommon.FirstDisplayedScrollingRowIndex;
            int focusedRow = dgvCommon.CurrentCell?.RowIndex ?? -1;
            dgvCommon.DataSource = bd;

            try
            {
                dgvCommon.FirstDisplayedScrollingRowIndex = index;
                dgvCommon.CurrentCell = dgvCommon.Rows[focusedRow].Cells[0];
            }
            catch { }

            ctrPage1.Visible = true;
            ctrPage1.Total = total;
            ctrPage1.InitPager();
        }        

        void BindDisplayPattern(WorkListDisplayPattern pattern)
        {
            foreach (DataGridViewColumn col in dgvCommon.Columns)
            {                
                long displayOrder = DisplayColumn(col, pattern);
                if (displayOrder == -1)
                {
                    col.Visible = false;
                }              
            }
            foreach (WorkListDisplayPatternDetail detail in pattern.DetailList)
            {
                DataGridViewColumn col = GetDisplayColumn(detail);
                if (col != null)
                {
                    //2013/0206 misawa CHANGE↓ MK-300 CoMSのパターン登録が正しく動作しない
                    col.DisplayIndex = int.Parse(detail.DisplayOrder.ToString());
                    col.Visible = true;
                    //2013/0206 misawa CHANGE↑ MK-300 CoMSのパターン登録が正しく動作しない
                }
            }
           
        }

        private DataGridViewColumn GetDisplayColumn(WorkListDisplayPatternDetail detail)
        {
            foreach (DataGridViewColumn col in dgvCommon.Columns )
            {
                if (col.HeaderText == detail.Title)
                    return col;
            }
            return null;
        }

        private long DisplayColumn(DataGridViewColumn col, WorkListDisplayPattern pattern)
        {
            foreach (WorkListDisplayPatternDetail detail in pattern.DetailList)
            {
                if (col.HeaderText == detail.Title)
                    return detail.DisplayOrder;
            }
            return -1;
        }

        private void S100001010_Load(object sender, EventArgs e)
        {
            btnMoney.SendToBack();
            this.clickEvent += new click(btnAjaxSearch_Click);

            cmbSearchType.SelectedIndex = 0;
            cmbWorkFlg.SelectedIndex = 0;
            SetDefaultDateTime();

            SetPatternColumn();
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える 20200416 kubota
            //Helper.BindCodeMstToCheckboxList(Constant.COMISIONPLAN_TYPE_CODE, cmbPlanType);
            Helper.BindCodeMstToDropDownList(Constant.WORK_MASTER_TYPE, cmbObjectType, true);
            Helper.BindCodeMstToDropDownList(Constant.CONTRACT_WORK_TYPE, cmbWorkType, true);
            Helper.BindCodeMstToDropDownList(Constant.CONTRACT_TYPE, cmbContactType, true);
            //Helper.BindCodeMstToCheckboxList(Constant.BUILDINGGROUP_OWNERTYPE, cmbOwnerType);            

            //InitComboboxBlockCode();
            Helper.BindCodeMstToCheckboxList(Constant.COMISIONPLAN_TYPE_CODE, cmbPlanTypeDEV);
            Helper.BindCodeMstToCheckboxList(Constant.BUILDINGGROUP_OWNERTYPE, cmbOwnerTypeDEV);
            Helper.InitComboboxBlockCode(cmbBlockCodeDEV);
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える end
            if (defaultObjectType != string.Empty) { cmbObjectType.SelectedValue = defaultObjectType; }
            this.ctrStaff.BindData();
            this.ctrKumiai.BindData();
            this.ctrBusinessConnections.BindData();

            SetDisplayPattern((new S100001BL()).GetDefaultPattern(Helper.loginUserInfo.Pid, this.screenID, (int)this.patternIndex));
            // disable sort and filter
            this.dgvCommon.DisabledFilterColumns.Add(this.clCreate.Name);
            this.dgvCommon.DisabledFilterColumns.Add(this.clSB0120.Name);
            this.dgvCommon.DisabledFilterColumns.Add(this.clSB0020.Name);
            this.dgvCommon.DisabledFilterColumns.Add(this.clSB0220.Name);
            this.dgvCommon.DisabledFilterColumns.Add(this.clSB0225.Name);

            if (targetMonth != null && targetMonth != DateTime.MinValue)
            {
                dtpDueDateFrom.Value = Helper.GetBeginDayOfMonth(targetMonth);
                dtpDueDateTo.Value = Helper.GetEndDayOfMonth(targetMonth);
                if (isB0070)
                {
                    cmbSearchType.SelectedIndex = 1;
                }
                btnSearch_Click(null, null);
            }

            ignoreAutoFormatColumns = new HashSet<string>(new[] { clCompanyCode.Name, clWorkID.Name, clKumiaCode.Name, clSpecificID.Name });
            dgvCommon.IgnoreAutoFormatColumns = ignoreAutoFormatColumns;
        }
        //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える 20200416 kubota
        //private void InitComboboxBlockCode()
        //{
        //    COMSD.business.D100001BL business = new coms.COMSD.business.D100001BL();
        //    List<COMSDService.BlockMst> listBlock = business.ListBlockMst();

        //    foreach (COMSDService.BlockMst block in listBlock)
        //    {
        //        cmbBlockCode.Items.Add(new coms.COMMON.ui.CCBoxItem(block.BlockName, block.BlockCode));
        //    }
        //    cmbBlockCode.ValueSeparator = ", ";
        //    cmbBlockCode.DisplayMember = "Name";

        //}
        //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える end
        private void SetDefaultDateTime()
        {
            dtpDueDateFrom.Value = DateTime.Now;
            dtpDueDateTo.Value = DateTime.Now;
            dtpDueDateFrom.Value = DateTime.MinValue;
            dtpDueDateTo.Value = DateTime.MinValue;          
        }

        private void SetPatternColumn()
        {
            List<string> patternColumn = new List<string>();
            int[] columnIndex = null;
            if (patternIndex == PATTERNINDEX.PATTERN1)
            {
                patternColumn.AddRange(PATTERN1_COLUMN);
                columnIndex = PATTTERN1_COLUMN_INDEX;
            }
            else if (patternIndex == PATTERNINDEX.PATTERN2)
            {
                patternColumn.AddRange(PATTERN2_COLUMN);
                columnIndex = PATTTERN2_COLUMN_INDEX;
            }
            else if (patternIndex == PATTERNINDEX.PATTERN3)
            {
                patternColumn.AddRange(PATTERN3_COLUMN);
                columnIndex = PATTTERN3_COLUMN_INDEX;
            }

            int colIndex = 0;
            while (colIndex < dgvCommon.Columns.Count)
            {
                DataGridViewColumn col = dgvCommon.Columns[colIndex];
                if (!patternColumn.Contains(col.HeaderText))
                {
                    col.DisplayIndex = 0;
                    //dgvCommon.Columns.Remove(col);
                }
                colIndex++;                
            }

            foreach (DataGridViewColumn col in dgvCommon.Columns)
            {
                int index = patternColumn.IndexOf(col.HeaderText);
                if (index >= 0)
                {
                    col.Visible = true;
                    col.DisplayIndex = columnIndex[index] - 1;
                }
            }            
            
        }       

        void CustomizationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            chkListCustom.Checked = false;
        }

        public void SetDisplayPattern(WorkListDisplayPattern pattern)
        {
            displayPattern = pattern;
            if (displayPattern != null)
            {
                BindDisplayPattern(displayPattern);
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            COMSS.ui.S100001.S100001020 ui = new COMSS.ui.S100001.S100001020();
            ui.Show();

        }

        private void btnMaster_Click(object sender, EventArgs e)
        {
            this.Close();
            coms.COMSS.ui.common.menu masterMenu = new coms.COMSS.ui.common.menu();
            masterMenu.Show();
        }
        

        private void ctrPage1_PageIndexChanged(object sender, EventArgs e)
        {
            SearchDataAjax();
        }

        private void ctrPage1_PageSizeChanged(object sender, EventArgs e)
        {
            ctrPage1.CurrentPageIndex = 0;
            SearchDataAjax();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPlanPid.Text = "";
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える 20200416 kubota
            //cmbPlanType.Clear();
            cmbObjectType.SelectedIndex = 0;
            ctrBusinessConnections.Clear();
            SetDefaultDateTime();
            ctrKumiai.Clear();
            ctrStaff.Reset();
            cmbWorkType.SelectedIndex = 0;
            cmbContactType.SelectedIndex = 0;
            cmbWorkFlg.SelectedIndex = 0;
            chkNotInputDate.Checked = false;
            chkNotFinish.Checked = false;
            chkUpload.Checked = false;
            //cmbBlockCode.Clear();
            //cmbOwnerType.Clear();
            cmbPlanTypeDEV.Clear();
            cmbBlockCodeDEV.Clear();
            cmbOwnerTypeDEV.Clear();
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える end
        }

        private void btnPattern_Click(object sender, EventArgs e)
        {
            S100001.S100001100 frmPattern = new S100001.S100001100();
            frmPattern.parentScreenID = this.screenID;
            frmPattern.patternIndex = this.patternIndex;
            DialogResult dlg = frmPattern.ShowDialog();
            if (dlg == DialogResult.OK)
            {
                long pid = frmPattern.displayPatternPid;
                if (pid != long.MinValue)
                {
                    SetDisplayPattern((new S100001BL()).ReadWorkListDisplayPattern(pid));                    
                }
            }
        }

        COMSSService.CommisionWorkPlanInfoResult commisionWorkPlanInfoDelete;
        string selectedColumn = string.Empty;

        void OpenDocument(string documentInfo)
        {
            //普通ファイル      
            string[] val = documentInfo.Split(',');
            coms.COMSPService.DocumentInfoSearch docInfo = new coms.COMSPService.DocumentInfoSearch();
            docInfo.DocumentPid = int.Parse(val[1]);
            docInfo.WorkDocumentInfoPid = int.Parse(val[0]);
            S100001.S100001900 frmDoc = new coms.COMSS.ui.S100001.S100001900(docInfo, null);
            frmDoc.Show();
        }

        private void btnColumnRegister_Click(object sender, EventArgs e)
        {
            SaveDisplayPattern();
            S100001.S100001120 frmPattern = new S100001.S100001120();            
            frmPattern.SetDisplayPattern(this.displayPattern);
            DialogResult dlg = frmPattern.ShowDialog();

            if (dlg == DialogResult.Yes)
            {
                SetDisplayPattern(frmPattern.GetDisplayPattern());
            }
        }

        CodeMst[] patternCodeList = (new COMMON.business.CommonBL()).SearchCodeMstByCode(Constant.WORKLIST_DISPLAY_PATTERN);
        private void SaveDisplayPattern()
        {
            if (displayPattern == null)
            {
                displayPattern = new WorkListDisplayPattern();
                displayPattern.UserMstPid = Helper.loginUserInfo.Pid;
                displayPattern.ScreenID = this.screenID;
                displayPattern.PatternIndex = (int)this.patternIndex;
            }

            displayPattern.UpdateDateTime = DateTime.Now;

            List<WorkListDisplayPatternDetail> detailList = new List<WorkListDisplayPatternDetail>();
            foreach (DataGridViewColumn col in dgvCommon.Columns)
            {
                if (col.Visible)
                {
                    CodeMst codeMst = GetCodeMstByTitle(col.HeaderText);
                    if (codeMst != null)
                    {
                        WorkListDisplayPatternDetail del = new WorkListDisplayPatternDetail();
                        del.DisplayOrder = col.DisplayIndex;
                        del.Code = codeMst.Number;
                        del.Title = codeMst.Title;
                        del.WorkListDisplayPatternPid = displayPattern.Pid;
                        detailList.Add(del);
                    }
                }
            }
            displayPattern.DetailList = detailList.ToArray();
        }

        CodeMst GetCodeMstByTitle(string title)
        {
            foreach (CodeMst item in patternCodeList)
            {
                if (item.Title == title)
                    return item;
            }
            return null;
        }

        private void chkListCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (chkListCustom.Checked)
            {
                _chooserPopup = new ColumnChooserPopup(dgvCommon);

                _chooserPopup.ColumnReAddRequested += col =>
                {
                    if (dgvCommon.Columns.Contains(col))
                    {
                        col.Visible = true;
                        try { col.DisplayIndex = dgvCommon.Columns.Count - 1; } catch { }
                    }
                    _chooserPopup.RemoveColumnByName(col.Name);
                };

                // ===== PATCH: sync checkbox when popup closed =====
                _chooserPopup.PopupClosed += () =>
                {
                    chkListCustom.CheckedChanged -= chkListCustom_CheckedChanged;
                    chkListCustom.Checked = false;
                    chkListCustom.CheckedChanged += chkListCustom_CheckedChanged;

                    _chooserPopup = null;
                };

                var bottomRight = new Point(
                    this.Bounds.Right - _chooserPopup.Width - 10,
                    this.Bounds.Bottom - _chooserPopup.Height - 10
                );
                bottomRight = this.PointToScreen(this.PointToClient(bottomRight));
                _chooserPopup.Location = bottomRight;

                _chooserPopup.Show(this);
            }
            else
            {
                _chooserPopup?.Close();
                _chooserPopup = null;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            if (searchTypeIndex == 1)
            {
                MessageBox.Show("セコムの帳票をエクスポートする場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //printableComponentLink1.CreateDocument(true);
            //printableComponentLink1.ShowPreview();
            List<COMSSService.CommisionWorkPlanInfoResult> reportList = new List<CommisionWorkPlanInfoResult>();
            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                CommisionWorkPlanInfoResultEx obj = dgvCommon.GetRow(i) as CommisionWorkPlanInfoResultEx;
                //if (obj.BusinessConnectionsMstPid > 0 && obj.WorkType == Constant.WORK_TYPE_0010)
                if (obj.WorkType == Constant.WORK_TYPE_0010 && obj.WorkFlg == Constant.WORK_OPERATION)
                {
                    var baseObj = new COMSSService.CommisionWorkPlanInfoResult();
                    baseObj.CopyObjectData(obj);
                    reportList.Add(baseObj);
                }
            }
            
            if (reportList.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                //随時検索条件
                CommisionWorkPlanInfoCondition con = new CommisionWorkPlanInfoCondition();
                con.KumiaiInfoPid = (ctrKumiai.CurrentKumiaiInfo != null ? ctrKumiai.CurrentKumiaiInfo.Pid : long.MinValue);
                con.BusinessConnectionMstPid = (ctrBusinessConnections.CurrentBusinessConnectionsMst != null ? ctrBusinessConnections.CurrentBusinessConnectionsMst.Pid : long.MinValue);
                con.WorkPlanDueDateFrom = dtpDueDateFrom.Value;
                con.WorkPlanDueDateTo = dtpDueDateTo.Value;

                COMSSService.FileEntry fileEntry = new business.S100001BL().CreateSecomReport(reportList.ToArray(), con);
                if (fileEntry != null)
                {
                    Helper.SaveToExcel(fileEntry);
                }
                this.Cursor = Cursors.Default;
            }
            pnlSC.Visible = false;
        }

        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            
        }

        private void S100001010_SizeChanged(object sender, EventArgs e)
        {
            pnlSearch.Width = this.Width - 30;
            dgvCommon.Size = new Size(this.Width - 30, this.Height - dgvCommon.Location.Y - 50);
        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {            
            //DevExpress.XtraEditors.BaseEdit edit = (sender as DevExpress.XtraGrid.Views.Base.BaseView).ActiveEditor;    
            //(edit as DevExpress.XtraEditors.ButtonEdit).Properties.Buttons[0].Enabled = false;
        }

        private void btnCreateFile_Click(object sender, EventArgs e)
        {
            pnlReportType.Visible = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlReportType.Visible = false;
        }        

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex < 0)
            {
                MessageBox.Show("帳票種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if(searchTypeIndex == 0 && cmbReportType.SelectedIndex > 2)
            {
                if (cmbReportType.SelectedIndex == 3)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0210_TITLE + "を作成する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0215_TITLE + "を作成する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            else if (searchTypeIndex == 1 && cmbReportType.SelectedIndex < 3)
            {
                if (cmbReportType.SelectedIndex == 0)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0310_TITLE + "を作成する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (cmbReportType.SelectedIndex == 1)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0110_TITLE + "を作成する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0115_TITLE + "を作成する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }            

            if (cmbReportType.SelectedIndex >= 0)
            {
                itemIndexList.Clear();

                List<COMSSService.DictionaryEntry> planList = new List<DictionaryEntry>();
                List<string> planIDList = new List<string>();

                string documentNo = string.Empty;
                string documentTitle = string.Empty;
                //【元請】確認書/請書
                if (cmbReportType.SelectedIndex == 1)
                {
                    documentNo = Constant.DOCUMENTNO_SB0110;
                    documentTitle = Constant.DOCUMENTNO_SB0110_TITLE;
                }
                //【直発注】確認書/請書
                if (cmbReportType.SelectedIndex == 2)
                {
                    documentNo = Constant.DOCUMENTNO_SB0115;
                    documentTitle = Constant.DOCUMENTNO_SB0115_TITLE;
                }
                //【請求書/請書
                else if (cmbReportType.SelectedIndex == 3)
                {
                    documentNo = Constant.DOCUMENTNO_SB0210;
                    documentTitle = Constant.DOCUMENTNO_SB0210_TITLE;
                }
                //協力会社毎予定表
                else if (cmbReportType.SelectedIndex == 0)
                {
                    documentNo = Constant.DOCUMENTNO_SB0310;
                    documentTitle = Constant.DOCUMENTNO_SB0310_TITLE;
                }
                //協力会社宛請求書
                else if (cmbReportType.SelectedIndex == 4)
                {
                    documentNo = Constant.DOCUMENTNO_SB0215;
                    documentTitle = Constant.DOCUMENTNO_SB0215_TITLE;
                }

                for (int i = 0; i < dgvCommon.RowCount; i++)
                {
                    if (dgvCommon.GetRowCellValue(i, clCreate.Name).Equals(true))
                    {
                        itemIndexList.Add(i);
                    }
                }

                for (int i = 0; i < dgvCommon.RowCount; i++)
                {
                    if (dgvCommon.GetRowCellValue(i, clCreate.Name).Equals(true))
                    {                        

                        CommisionWorkPlanInfoResultEx objEx = dgvCommon.GetRow(i) as CommisionWorkPlanInfoResultEx;
                        COMSSService.CommisionWorkPlanInfoResult obj = new CommisionWorkPlanInfoResult();
                        obj.CopyObjectData(objEx);

                        string month = (searchTypeIndex == 0 ? GetMonth(obj.WorkPlanDueDate, true) : GetMonth(obj.WorkResultDate, false));

                        bool notCreate = (cmbReportType.SelectedIndex == 0 && obj.DocumentSB0310 == string.Empty) ||
                            (cmbReportType.SelectedIndex == 1 && obj.DocumentSB0110 == string.Empty) ||
                            (cmbReportType.SelectedIndex == 2 && obj.DocumentSB0115 == string.Empty) ||
                            (cmbReportType.SelectedIndex == 3 && obj.DocumentSB0210 == string.Empty) ||
                            (cmbReportType.SelectedIndex == 4 && obj.DocumentSB0215 == string.Empty);

                        string key = obj.BusinessConnectionsMstPid.ToString() + "|" + month;

                        if (!planIDList.Contains(key) && notCreate)
                        {
                            planIDList.Add(key);
                            COMSSService.DictionaryEntry entry = new DictionaryEntry();
                            entry.Key = obj.BusinessConnectionsMstPid;                            

                            int counter = 0;

                            int reportIndex = cmbReportType.SelectedIndex;

                            List<COMSSService.CommisionWorkPlanInfoResult> valList = new List<CommisionWorkPlanInfoResult>();

                            bool isCreate = IsCreateDocument(documentNo, obj);

                            if (isCreate)
                            {
                                valList.Add(obj);
                            }                            

                            for (int j = i + 1; j < dgvCommon.RowCount; j++)
                            {
                                CommisionWorkPlanInfoResultEx childObjEx = dgvCommon.GetRow(j) as CommisionWorkPlanInfoResultEx;
                                COMSSService.CommisionWorkPlanInfoResult childObj = new CommisionWorkPlanInfoResult();
                                childObj.CopyObjectData(childObjEx);

                                string tempMonth = (searchTypeIndex == 0 ? GetMonth(childObj.WorkPlanDueDate, true) : GetMonth(childObj.WorkResultDate, false));
                                string tempKey = childObj.BusinessConnectionsMstPid + "|" + tempMonth;

                                if (tempKey == key)
                                {
                                    isCreate = IsCreateDocument(documentNo, childObj);
                                    if (isCreate)
                                    {
                                        valList.Add(childObj);
                                    }                                    
                                    counter++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            i += counter;
                            if (valList != null && valList.Count > 0 && obj.BusinessConnectionsMstPid > 0)
                            {
                                if (documentNo != Constant.DOCUMENTNO_SB0215)
                                {
                                    entry.Value = valList;
                                    planList.Add(entry);
                                }

                                //SB0215協力会社宛請求書の場合、受領日をすべてセットしないと、帳票を作らない
                                else 
                                {
                                    //全件を取り込む
                                    List<CommisionWorkPlanInfoResult> subList = list.FindAll(delegate(CommisionWorkPlanInfoResult info)
                                    {
                                        string tempMonth = (searchTypeIndex == 0 ? GetMonth(info.WorkPlanDueDate, true) : GetMonth(info.WorkResultDate, false));
                                        string tempKey = info.BusinessConnectionsMstPid + "|" + tempMonth;

                                        return (tempKey == key && info.ContractType == Constant.CONTRACT_TYPE_0110);

                                    });
                                    if (subList.Count == valList.Count)
                                    {
                                        entry.Value = valList;
                                        planList.Add(entry);
                                    }
                                }
                            }
                        }
                        
                    }
                }               

                if (planList.Count > 0)
                {                    

                    if (MessageBox.Show("作成しますか？         ", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;

                    this.Cursor = Cursors.WaitCursor;

                    StartBalloon();
                    if (AddReport(documentNo, documentTitle))
                    {
                        (new S100001BL()).CreateBCDocumentInfo(planList.ToArray(), Helper.loginUserInfo.Pid, documentNo);
                    }
                    else
                    {
                        MessageBox.Show("帳票を作成中です。他の種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    this.Cursor = Cursors.Default;
                    pnlReportType.Visible = false;
                }
                else
                {
                    string msg = string.Format("対象が選択されていない、{0}もしくは選択されている対象は既に作成されている。{0}対象を確認して下さい。", Environment.NewLine);
                    MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex < 0)
            {
                MessageBox.Show("帳票種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("削除しますか？         ", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            if (cmbReportType.SelectedIndex >= 0)
            {
                List<string> planIDList = new List<string>();
                itemIndexList.Clear();
                for (int i = 0; i < dgvCommon.RowCount; i++)
                {
                    if (dgvCommon.GetRowCellValue(i, clCreate.Name).Equals(true))
                    {                        
                        COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult;

                        //【元請】確認書/請書
                        if (cmbReportType.SelectedIndex == 1 && obj.DocumentSB0110 != string.Empty && obj.DocumentSB0110.EndsWith(Constant.DOCUMENT_ENABLE_STATUS))
                        {
                            if (!planIDList.Contains(obj.DocumentSB0110))
                            {
                                planIDList.Add(obj.DocumentSB0110);                                
                            }
                        }
                        //【直発注】確認書/請書
                        if (cmbReportType.SelectedIndex == 2 && obj.DocumentSB0115 != string.Empty && obj.DocumentSB0115.EndsWith(Constant.DOCUMENT_ENABLE_STATUS))
                        {
                            if (!planIDList.Contains(obj.DocumentSB0115))
                            {
                                planIDList.Add(obj.DocumentSB0115);
                            }
                        }
                        //請求書/請書
                        else if (cmbReportType.SelectedIndex == 3 && obj.DocumentSB0210 != string.Empty && obj.DocumentSB0210.EndsWith(Constant.DOCUMENT_ENABLE_STATUS))
                        {
                            if (!planIDList.Contains(obj.DocumentSB0210))
                            {
                                planIDList.Add(obj.DocumentSB0210);
                            }
                        }
                        //協力会社毎予定表
                        else if (cmbReportType.SelectedIndex == 0 && obj.DocumentSB0310 != string.Empty && obj.DocumentSB0310.EndsWith(Constant.DOCUMENT_ENABLE_STATUS))
                        {
                            if (!planIDList.Contains(obj.DocumentSB0310))
                            {
                                planIDList.Add(obj.DocumentSB0310);
                            }
                        }
                        //協力会社宛請求書/請書
                        else if (cmbReportType.SelectedIndex == 4 && obj.DocumentSB0215 != string.Empty && obj.DocumentSB0215.EndsWith(Constant.DOCUMENT_ENABLE_STATUS))
                        {
                            if (!planIDList.Contains(obj.DocumentSB0215))
                            {
                                planIDList.Add(obj.DocumentSB0215);
                            }
                        }
                        itemIndexList.Add(i);
                    }
                }
                if (planIDList.Count > 0)
                {
                    (new S100001BL()).DeleteBCDocumentInfo(planIDList.ToArray());
                    SearchDataAjax();
                }
            }
            pnlReportType.Visible = false;
        }

        bool onFlg = false;
        private void dgvCommon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var view = sender as DataGridViewEx;
            var hit = view.HitTest(e.X, e.Y);
            if (hit.Type != DataGridViewHitTestType.ColumnHeader) return;

            DataGridViewColumn col = view.Columns[hit.ColumnIndex];
            if (col == clCreate)
            {
                onFlg = !onFlg;
                for (int i = 0; i < dgvCommon.RowCount; i++)
                {
                    dgvCommon.SetRowCellValue(i, clCreate.Name, onFlg);
                }
            };
        }

        private void mnuDecide_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("資料を確定しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string[] vals = GetDocument(commisionWorkPlanInfoDelete).Split(',');

                List<long> idList = new List<long>();
                idList.Add(long.Parse(vals[0]));
                (new S100001BL()).UpdateSBusinessconnectionDocumenStatus(idList, Constant.DOCUMENT_DISABLE_STATUS);
                btnSearch_Click(null, null);
            }
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("資料を解除しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string[] vals = GetDocument(commisionWorkPlanInfoDelete).Split(',');
                List<long> idList = new List<long>();
                idList.Add(long.Parse(vals[0]));
                (new S100001BL()).UpdateSBusinessconnectionDocumenStatus(idList, Constant.DOCUMENT_ENABLE_STATUS);
                btnSearch_Click(null, null);
            }
        }

        private void mnuPublish_Click(object sender, EventArgs e)
        {
            string[] vals = GetDocument(commisionWorkPlanInfoDelete).Split(',');

            List<long> idList = new List<long>();
            idList.Add(long.Parse(vals[0]));

            S100500.S100500Publish frm = new coms.COMSS.ui.S100500.S100500Publish(idList, Constant.BC_PATTERN);
            frm.ShowDialog();
        }

        string GetDocument(CommisionWorkPlanInfoResult obj)
        {
            string ret = string.Empty;
            if (selectedColumn == clSB0110.Name)
            {
                ret = obj.DocumentSB0110;
            }
            if (selectedColumn == clSB0115.Name)
            {
                ret = obj.DocumentSB0115;
            }
            else if (selectedColumn == clSB0210.Name)
            {
                ret = obj.DocumentSB0210;
            }
            else if (selectedColumn == clSB0310.Name)
            {
                ret = obj.DocumentSB0310;
            }
            else if (selectedColumn == clSB0215.Name)
            {
                ret = obj.DocumentSB0215;
            }
            return ret;
        }

        string GetDocumentInfo(CommisionWorkPlanInfoResult obj)
        {
            string documentInfo = string.Empty;
            if (cmbReportType.SelectedIndex == 1) documentInfo = obj.DocumentSB0110; //【元請】確認書/請書
            if (cmbReportType.SelectedIndex == 2) documentInfo = obj.DocumentSB0115; //【直発注】確認書/請書
            if (cmbReportType.SelectedIndex == 3) documentInfo = obj.DocumentSB0210; //【請求書/請書
            if (cmbReportType.SelectedIndex == 0) documentInfo = obj.DocumentSB0310; //協力会社毎予定表
            if (cmbReportType.SelectedIndex == 4) documentInfo = obj.DocumentSB0215; //協力会社宛請求書
            return documentInfo;
        }

        bool ContainsKey(List<coms.COMSSService.DictionaryEntry> docList, string key)
        {
            foreach (DictionaryEntry entry in docList)
            {
                if (entry.Key.ToString() == key)
                    return true;
            }
            return false;
        }

        private void mnuItemSendMail_Click(object sender, EventArgs e)
        {

            List<DictionaryEntry> businessHash = new List<DictionaryEntry>();
            List<COMSPService.Document> docList = new List<coms.COMSPService.Document>();

            if ((selectedColumn == clSB0110.Name && commisionWorkPlanInfoDelete.DocumentSB0110 != string.Empty) ||
                (selectedColumn == clSB0115.Name && commisionWorkPlanInfoDelete.DocumentSB0115 != string.Empty) ||
                (selectedColumn == clSB0210.Name && commisionWorkPlanInfoDelete.DocumentSB0210 != string.Empty) ||
                (selectedColumn == clSB0310.Name && commisionWorkPlanInfoDelete.DocumentSB0310 != string.Empty) ||
                (selectedColumn == clSB0215.Name && commisionWorkPlanInfoDelete.DocumentSB0215 != string.Empty))
            {

                if (!ContainsKey(businessHash, commisionWorkPlanInfoDelete.BusinessConnectionsMstPid.ToString()))
                {
                    DictionaryEntry entry = new DictionaryEntry();
                    entry.Key = commisionWorkPlanInfoDelete.BusinessConnectionsMstPid;
                    entry.Value = string.Format("{0} {1}", commisionWorkPlanInfoDelete.CompanyCode, commisionWorkPlanInfoDelete.CompanyName);
                    businessHash.Add(entry);
                }

                COMSPService.Document doc = new coms.COMSPService.Document();

                string[] vals = GetDocument(commisionWorkPlanInfoDelete).Split(',');
                doc.Pid = long.Parse(vals[1]);
                doc.FileName = Helper.SplitFileName(vals[3]);
                doc.HistoryPid = commisionWorkPlanInfoDelete.BusinessConnectionsMstPid;
                docList.Add(doc);
            }


            if (businessHash.Count > 0)
            {
                S100500.S100500Email popMail = new coms.COMSS.ui.S100500.S100500Email(businessHash, docList);
                popMail.month = dtpDueDateFrom.Value;
                string title = (cmbObjectType.SelectedItem as CodeMst).Title;
                if (title == "先組") popMail.isSaki = true;
                else if (title == "後組") popMail.isAto = true;

                if (selectedColumn == clSB0210.Name) popMail.isSeikyu = true;
                popMail.LoadMailTemplate();
                popMail.Show();
            }
        }       

        List<int> itemIndexList = new List<int>();
        private void btnDecise_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex < 0)
            {
                MessageBox.Show("帳票種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            itemIndexList.Clear();
            List<long> idList = new List<long>();
            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (dgvCommon.GetRowCellValue(i, clCreate.Name).Equals(true))
                {
                    COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult;                    
                    string documentInfo = GetDocumentInfo(obj);

                    if (documentInfo != string.Empty && documentInfo.EndsWith(Constant.DOCUMENT_ENABLE_STATUS))
                    {
                        string[] val = documentInfo.Split(',');
                        idList.Add(long.Parse(val[0]));                        
                    }
                    itemIndexList.Add(i);
                }
            }

            if (idList.Count > 0)
            {
                if (MessageBox.Show("資料を確定しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new S100001BL().UpdateSBusinessconnectionDocumenStatus(idList, Constant.DOCUMENT_DISABLE_STATUS);
                    SearchDataAjax();
                    MessageBox.Show("確定しました。         ", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pnlReportType.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("項目を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex < 0)
            {
                MessageBox.Show("帳票種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<long> idList = new List<long>();
            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (dgvCommon.GetRowCellValue(i, clCreate.Name).Equals(true))
                {
                    COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult;
                    string documentInfo = GetDocumentInfo(obj);

                    if (documentInfo != string.Empty)
                    {
                        string[] val = documentInfo.Split(',');
                        long pid = long.Parse(val[0]);
                        if(!idList.Contains(pid))
                            idList.Add(pid);
                    }
                }
            }
            if (idList.Count > 0)
            {
                S100500.S100500Publish frm = new coms.COMSS.ui.S100500.S100500Publish(idList, Constant.BC_PATTERN);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("発行しました。         ", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pnlReportType.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("項目を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnSendMail_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex < 0)
            {
                MessageBox.Show("帳票種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<DictionaryEntry> businessHash = new List<DictionaryEntry>();
            List<COMSPService.Document> docList = new List<coms.COMSPService.Document>();

            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (dgvCommon.GetRowCellValue(i, clCreate.Name).Equals(true))
                {
                    COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult;
                    string documentInfo = GetDocumentInfo(obj);

                    if (documentInfo != string.Empty)
                    {

                        if (!ContainsKey(businessHash, obj.BusinessConnectionsMstPid.ToString()))
                        {
                            DictionaryEntry entry = new DictionaryEntry();
                            entry.Key = obj.BusinessConnectionsMstPid;
                            entry.Value = string.Format("{0} {1}", obj.CompanyCode, obj.CompanyName);
                            businessHash.Add(entry);
                        }
                                                
                        string[] val = documentInfo.Split(',');
                        long docPid = long.Parse(val[1]);
                        if (!ContainsDoc(docList, docPid))
                        {
                            COMSPService.Document doc = new coms.COMSPService.Document();
                            doc.Pid = docPid;
                            doc.FileName = Helper.SplitFileName(val[3]);
                            doc.HistoryPid = obj.BusinessConnectionsMstPid;
                            docList.Add(doc);
                        }
                    }
                }
            }

            if (businessHash.Count > 0)
            {
                S100500.S100500Email popMail = new coms.COMSS.ui.S100500.S100500Email(businessHash, docList);
                popMail.month = dtpDueDateFrom.Value;
                string title = (cmbObjectType.SelectedItem as CodeMst).Title;
                if (title == "先組") popMail.isSaki = true;
                else if (title == "後組") popMail.isAto = true;
                else if (cmbReportType.SelectedIndex == 1) popMail.isMoto = true;
                else if (cmbReportType.SelectedIndex == 2) popMail.isChyoku = true;
                else if (cmbReportType.SelectedIndex == 3) popMail.isSeikyu = true;
                popMail.LoadMailTemplate();
                popMail.Show();
            }
        }

        private bool ContainsDoc(List<coms.COMSPService.Document> docList, long docPid)
        {
            foreach(coms.COMSPService.Document doc in docList)
            {
                if (doc.Pid == docPid)
                    return true;
            }
            return false;
        }

        private void mnuItemDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("削除しますか？         ", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            List<string> idList = new List<string>();

            string documentInfo = GetDocument(commisionWorkPlanInfoDelete);

            if (documentInfo != string.Empty && documentInfo.EndsWith(Constant.DOCUMENT_ENABLE_STATUS))
            {
                idList.Add(documentInfo);
                (new S100001BL()).DeleteBCDocumentInfo(idList.ToArray());
                SearchDataAjax();
            }
        }

        string importFileName = string.Empty;
        private void btnImport_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel (*.xlsx)|*.xlsx|CSV (*.csv)|*.csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("インポートしますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question)  == DialogResult.Yes)
                {
                    importFileName = dlg.FileName;

                    Helper.ShowLoadingPopup();
                    bgImport.RunWorkerAsync();
                }
            }
        }       

        string importResult = string.Empty;
        private void bgImport_DoWork(object sender, DoWorkEventArgs e)
        {
            COMSSService.FileEntry fileEntry = new coms.COMSSService.FileEntry();
            fileEntry.Content = Helper.ReadFile(importFileName);
            fileEntry.FileName = Helper.SplitFileName(importFileName);
            importResult = (new S100001BL()).ImportCommissionPlanInfo(fileEntry);            
        }

        private void bgImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Helper.HideLoadingPopup();
            if (importResult == string.Empty)
            {
                MessageBox.Show("インポートしました。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSearch_Click(null, null);
            }
            else
            {
                MessageBox.Show(importResult, "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSearch_Click(null, null);
            }
        }

        private void btnNext2Month_Click(object sender, EventArgs e)
        {
            if (cmbSearchType.SelectedIndex == 0)
            {
                DateTime dateTime = DateTime.Now.AddMonths(2);
                dateTime = Helper.GetBeginDayOfMonth(dateTime);
                BindDate(dateTime, true);
                dateTime = Helper.GetEndDayOfMonth(dateTime);
                BindDate(dateTime, false);
            }
            else
            {
                DateTime dateTime = Helper.GetBeginDayOfThisMonth();
                dtpDueDateFrom.Value = dateTime;
                dateTime = Helper.GetEndDayOfThisMonth();
                dtpDueDateTo.Value = dateTime;
            }
        }

        private void btnNext3Month_Click(object sender, EventArgs e)
        {
            if (cmbSearchType.SelectedIndex == 0)
            {
                DateTime dateTime = DateTime.Now.AddMonths(3);
                dateTime = Helper.GetBeginDayOfMonth(dateTime);
                BindDate(dateTime, true);
                dateTime = Helper.GetEndDayOfMonth(dateTime);
                BindDate(dateTime, false);
            }
            else
            {
                DateTime dateTime = Helper.GetBeginDayOfNextMonth();
                dtpDueDateFrom.Value = dateTime;
                dateTime = Helper.GetEndDayOfNextMonth();
                dtpDueDateTo.Value = dateTime;
            }
        }        

        private void mnuDownload_Click(object sender, EventArgs e)
        {
            if (commisionWorkPlanInfoDelete != null)
            {
                string documentInfo = GetDocument(commisionWorkPlanInfoDelete);

                string[] vals = documentInfo.Split(',');
                this.Cursor = Cursors.WaitCursor;
                string fileName = Helper.SaveFile(long.Parse(vals[1]), vals[3]);
                this.Cursor = Cursors.Default;

                if (System.IO.File.Exists(fileName))
                {
                    System.Diagnostics.Process.Start(fileName);
                }
            }
        }

        private void mnuCreate_Click(object sender, EventArgs e)
        {
            string documentNo = string.Empty;
            string documentTitle = string.Empty;

            //【元請】確認書/請書
            if (selectedColumn == clSB0110.Name)
            {
                if (searchTypeIndex != 0)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0110_TITLE + "を作成する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    documentNo = Constant.DOCUMENTNO_SB0110;
                    documentTitle = Constant.DOCUMENTNO_SB0110_TITLE;
                }
            }
            //【直発注】確認書/請書
            else if (selectedColumn == clSB0115.Name)
            {
                if (searchTypeIndex != 0)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0115_TITLE + "を作成する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    documentNo = Constant.DOCUMENTNO_SB0115;
                    documentTitle = Constant.DOCUMENTNO_SB0115_TITLE;
                }
            }
            //請求書/請書
            else if (selectedColumn == clSB0210.Name)
            {
                if (searchTypeIndex != 1)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0210_TITLE + "を作成する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    documentNo = Constant.DOCUMENTNO_SB0210;
                    documentTitle = Constant.DOCUMENTNO_SB0210_TITLE;
                }
            }
            //協力会社毎予定表
            else if (selectedColumn == clSB0310.Name)
            {
                if (searchTypeIndex != 0)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0310_TITLE + "を作成する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    documentNo = Constant.DOCUMENTNO_SB0310;
                    documentTitle = Constant.DOCUMENTNO_SB0310_TITLE;
                }
            }
            //協力会社宛請求書
            else if (selectedColumn == clSB0215.Name)
            {
                if (searchTypeIndex != 1)
                {
                    MessageBox.Show(Constant.DOCUMENTNO_SB0215_TITLE + "を作成する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    documentNo = Constant.DOCUMENTNO_SB0215;
                    documentTitle = Constant.DOCUMENTNO_SB0215_TITLE;
                }
            }

            this.Cursor = Cursors.WaitCursor;

            List<COMSSService.DictionaryEntry> planList = new List<DictionaryEntry>();

           
            List<COMSSService.CommisionWorkPlanInfoResult> valList = new List<CommisionWorkPlanInfoResult>();
            //valList.Add(commisionWorkPlanInfoDelete);

            COMSSService.DictionaryEntry entry = new DictionaryEntry();
            entry.Key = commisionWorkPlanInfoDelete.BusinessConnectionsMstPid;

            //月を取り込む
            string month = (searchTypeIndex == 0 ? GetMonth(commisionWorkPlanInfoDelete.WorkPlanDueDate, true) : GetMonth(commisionWorkPlanInfoDelete.WorkResultDate, false));

            for (int i = 0; i < dgvCommon.RowCount; i++)
            {                    
                COMSSService.CommisionWorkPlanInfoResult objEx = dgvCommon.GetRow(i) as CommisionWorkPlanInfoResultEx;
                var obj = new CommisionWorkPlanInfoResult();
                obj.CopyObjectData(objEx);

                string tempMonth = (searchTypeIndex == 0 ? GetMonth(obj.WorkPlanDueDate, true) : GetMonth(obj.WorkResultDate, false));
                
                //同じ協力会社、同じ月
                if (obj.BusinessConnectionsMstPid.Equals(commisionWorkPlanInfoDelete.BusinessConnectionsMstPid) &&  tempMonth.Equals(month))
                {
                    bool isCreate = IsCreateDocument(documentNo, obj);

                    //作成条件を会うの場合
                    if (isCreate)
                    {
                        valList.Add(obj);
                    }
                    //協力会社宛請求書
                    else if (documentNo == Constant.DOCUMENTNO_SB0215 && obj.ContractType == Constant.CONTRACT_TYPE_0110)
                    {
                        string msg = string.Format("【直発注】請求書受領日に値が入っていないか、{0}作業状態が正常となっていません。{0}確認してください。", Environment.NewLine);
                        MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }                                            
            }
            if (valList != null && valList.Count > 0)
            {
                entry.Value = valList;
                planList.Add(entry);
            }

            if (planList.Count > 0)
            {
                if (MessageBox.Show("作成しますか？         ", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                StartBalloon();
                if (AddReport(documentNo, documentTitle))
                {
                    (new S100001BL()).CreateBCDocumentInfo(planList.ToArray(), Helper.loginUserInfo.Pid, documentNo);
                }
                else
                {
                    MessageBox.Show("帳票を作成中です。他の種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                string msg = string.Format("対象が選択されていない、{0}もしくは選択されている対象は既に作成されている。{0}対象を確認して下さい。", Environment.NewLine);
                MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            this.Cursor = Cursors.Default;
        }

        bool IsCreateDocument(string documentNo, CommisionWorkPlanInfoResult obj)
        {
            bool isCreate = false;
            if (obj.StatusTitle == Constant.COMMISSIONWORKPLANINFO_SEIJYOU_STATUSTITLE)
            {
                //SB0110 【元請】確認書/請書 -> 定期
                if (documentNo == Constant.DOCUMENTNO_SB0110 && obj.WorkType == Constant.WORK_TYPE_0010
                    && obj.ContractType == Constant.CONTRACT_TYPE_0010 && obj.WorkFlg == Constant.WORK_OPERATION)
                {
                    isCreate = true;
                }
                //SB0115 【直発注】確認書/請書 -> 定期
                else if (documentNo == Constant.DOCUMENTNO_SB0115 && obj.WorkType == Constant.WORK_TYPE_0010
                    && obj.ContractType == Constant.CONTRACT_TYPE_0110 && obj.WorkFlg == Constant.WORK_OPERATION)
                {
                    isCreate = true;
                }
                //SB0210管理会社宛請求書/請書
                else if (documentNo == Constant.DOCUMENTNO_SB0210 && obj.ApprovalDate != DateTime.MinValue && obj.ContractType == Constant.CONTRACT_TYPE_0010)
                {
                    isCreate = true;
                }
                //SB0215協力会社宛請求書/請書
                else if (documentNo == Constant.DOCUMENTNO_SB0215 && obj.WorkBillDate != DateTime.MinValue && obj.ContractType == Constant.CONTRACT_TYPE_0110)
                {
                    isCreate = true;
                }
                //SB0310 協力会社毎予定表
                else if (documentNo == Constant.DOCUMENTNO_SB0310 && obj.WorkType == Constant.WORK_TYPE_0010 && obj.WorkFlg == Constant.WORK_OPERATION)
                {
                    isCreate = true;
                }
            }
            return isCreate;
        }

        string GetMonth(string val, bool isFirst)
        {
            if (isFirst)
            {
                return val.Contains("\n") ? val.Substring(0, val.IndexOf("\n")) : val;
            }
            else
            {
                return val.Contains("\n") ? val.Substring(val.LastIndexOf("\n") + "\n".Length) : val;
            }
        }        

        private void btnIEport_Click(object sender, EventArgs e)
        {
            pnlSC.Visible = true;
        }

        private void btnSCClose_Click(object sender, EventArgs e)
        {
            pnlSC.Visible = false;
        }

        string scImportResult = string.Empty;
        private void bgSCImport_DoWork(object sender, DoWorkEventArgs e)
        {
            COMSSService.FileEntry fileEntry = new coms.COMSSService.FileEntry();
            fileEntry.Content = Helper.ReadFile(scImportFileName);
            fileEntry.FileName = Helper.SplitFileName(scImportFileName);
            scImportResult = (new S100001BL()).ImportSCCommissionPlanInfo(fileEntry);
        }

        private void bgSCImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Helper.HideLoadingPopup();
            if (scImportResult == string.Empty)
            {
                MessageBox.Show("インポートしました。       ", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pnlSC.Visible = false;
                SearchDataAjax();
            }
            else
            {
                MessageBox.Show(scImportResult, "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SearchDataAjax();
            }
        }

        string scImportFileName = string.Empty;
        private void btnSCImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel (*.xlsx)|*.xlsx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("インポートしますか？       ", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                scImportFileName = dlg.FileName;

                Helper.ShowLoadingPopup();
                bgSCImport.RunWorkerAsync();
            }
        }

        private void cmbSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchType.SelectedIndex == 0)
            {
                btnPreviousMonth.Text = "先月";
                btnPreviousMonth.Width = 35;
                btnPreviousMonth.Location = new Point(295, 31);

                btnThisMonth.Text = "今月";
                btnThisMonth.Width = 35;
                btnThisMonth.Location = new Point(331, 31);

                btnNextMonth.Text = "来月";
                btnNextMonth.Width = 35;
                btnNextMonth.Location = new Point(368, 31);

                btnNext2Month.Text = "2ヵ月後";
                btnNext2Month.Width = 50;
                btnNext2Month.Location = new Point(406, 31);

                btnNext3Month.Text = "3ヵ月後";
                btnNext3Month.Width = 50;
                btnNext3Month.Location = new Point(459, 31);

            }
            else if (cmbSearchType.SelectedIndex == 1)
            {
                btnPreviousMonth.Text = "3ヵ月前";
                btnPreviousMonth.Width = 50;
                btnPreviousMonth.Location = new Point(295, 31);

                btnThisMonth.Text = "2ヵ月前";
                btnThisMonth.Width = 50;
                btnThisMonth.Location = new Point(347, 31);

                btnNextMonth.Text = "先月";
                btnNextMonth.Width = 35;
                btnNextMonth.Location = new Point(400, 31);

                btnNext2Month.Text = "今月";
                btnNext2Month.Width = 35;
                btnNext2Month.Location = new Point(437, 31);

                btnNext3Month.Text = "来月";
                btnNext3Month.Width = 35;
                btnNext3Month.Location = new Point(475, 31);
            }
        }

        private void btnDownLoad_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex < 0)
            {
                MessageBox.Show("帳票種別を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            #region 選択された項目を取得
            System.Collections.Hashtable docList = new System.Collections.Hashtable();
            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (dgvCommon.GetRowCellValue(i, clCreate.Name).Equals(true))
                {
                    COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult;
                    string documentInfo = GetDocumentInfo(obj);
                    string date = string.Empty;
                    if (cmbReportType.SelectedIndex == 0 || cmbReportType.SelectedIndex == 1)
                    {
                        date = obj.WorkPlanDueDate.Contains("\n") ? obj.WorkPlanDueDate.Substring(0, obj.WorkPlanDueDate.IndexOf("\n")) : obj.WorkPlanDueDate;
                    }
                    else
                    {
                        date = obj.WorkResultDate.Contains("\n") ? obj.WorkResultDate.Substring(0, obj.WorkResultDate.IndexOf("\n")) : obj.WorkResultDate;
                    }
                    date = date.Replace("/", "");

                    if (documentInfo != string.Empty)
                    {
                        string key = obj.CompanyCode + "_" + date;
                        if (!docList.ContainsKey(key))
                        {
                            string[] vals = documentInfo.Split(',');
                            
                            COMSPService.Document document = new COMSPService.Document();
                            document.FileName = vals[3];
                            document.Pid = long.Parse(vals[1]);
                            docList.Add(key, document);
                        }
                    }
                }
            }

            #endregion 選択された項目を取得

            if (docList.Count > 0)
            {
                Helper.DownLoadListFile(docList);
            }
            else
            {
                MessageBox.Show("項目を選択してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            this.Cursor = Cursors.Default;
        }

        private void btnAjaxSearch_Click(object sender, EventArgs e)
        {
            SearchDataAjax();
        }

        private void mnuApprove_Click(object sender, EventArgs e)
        {

            if (searchTypeIndex == 1)
            {
                MessageBox.Show("確認書請書受領日を確認する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            var rowIdx = dgvCommon.CurrentCell != null ? dgvCommon.CurrentCell.RowIndex : -1;
            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(rowIdx) as COMSSService.CommisionWorkPlanInfoResult;
            if (obj.BusinessConnectionsMstPid <= 0) return;
            DateTime approvalDate = DateTime.MinValue;
            S100200040 frmDate;

            if (obj.ApprovalDate.HasValue)
            {
                //obj.ApprovalDate = ((DateTime)obj.ApprovalDate);
                //S100200040 frmDate = (DateTime)new S100200040(obj.ApprovalDate);
                frmDate = new S100200040(((DateTime)obj.ApprovalDate));

            }
            else
            {

                frmDate = new S100200040();
            }
            
            if (frmDate.ShowDialog() == DialogResult.OK)
            {
                approvalDate = frmDate.GetDate();
            }
            else
            {
                return;
            }

            if (MessageBox.Show("確認しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;

            List<COMSSService.DictionaryEntry> planList = new List<DictionaryEntry>();
            COMSSService.DictionaryEntry entry = new DictionaryEntry();
            entry.Key = obj.BusinessConnectionsMstPid;
            entry.Value = obj.WorkPlanDueDate.Contains("\n") ? obj.WorkPlanDueDate.Substring(0, obj.WorkPlanDueDate.IndexOf("\n")) : obj.WorkPlanDueDate;

            entry.Value = entry.Value.ToString() + "|" + approvalDate.ToString(Constant.SHOWING_DATETIME_FORMAT);
            if (obj.ContractType == Constant.CONTRACT_TYPE_0010)
            {
                entry.Value += "|1";
            }
            else
            {
                entry.Value += "|2";
            }

            planList.Add(entry);

            (new S100001BL()).ApproveBusinessConnectionMst(planList.ToArray());

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        private void mnuUnApprove_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 1)
            {
                MessageBox.Show("確認書請書受領日を解除する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("確認を解除しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;
            var rowIdx = dgvCommon.CurrentCell != null ? dgvCommon.CurrentCell.RowIndex : -1;
            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(rowIdx) as COMSSService.CommisionWorkPlanInfoResult;

            string date = obj.WorkPlanDueDate.Contains("\n") ? obj.WorkPlanDueDate.Substring(0, obj.WorkPlanDueDate.IndexOf("\n")) : obj.WorkPlanDueDate;

            (new S100001BL()).DeleteBusinessConnectionMstApproval(obj.BusinessConnectionsMstPid, date, obj.ContractType);

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        void GetMotoukeList(int index, ref List<CommisionWorkPlanInfoResult> fixList, ref List<CommisionWorkPlanInfoResult> continueList)
        {

            CommisionWorkPlanInfoResultEx objEx = dgvCommon.GetRow(index) as CommisionWorkPlanInfoResultEx;
            COMSSService.CommisionWorkPlanInfoResult obj = new CommisionWorkPlanInfoResult();
            obj.CopyObjectData(objEx);

            //定期
            if (obj.WorkType == Constant.WORK_TYPE_0010)
            {
                fixList.Add(obj);
            }
            //継続
            else
            {
                continueList.Add(obj);
            }

            string month = GetMonth(obj.WorkResultDate, false);

            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                CommisionWorkPlanInfoResultEx itemEx = dgvCommon.GetRow(i) as CommisionWorkPlanInfoResultEx;
                COMSSService.CommisionWorkPlanInfoResult item = new CommisionWorkPlanInfoResult();
                item.CopyObjectData(itemEx);

                if (i != index && item.BusinessConnectionsMstPid == obj.BusinessConnectionsMstPid
                    && month == GetMonth(item.WorkResultDate, false) && obj.ContractType == item.ContractType)
                {

                    //定期
                    if (item.WorkType == Constant.WORK_TYPE_0010)
                    {
                        fixList.Add(item);
                    }
                    //継続
                    else
                    {
                        continueList.Add(item);
                    }
                }
            }
        }

        private void mnuBillDateSet_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 0)
            {
                MessageBox.Show("【元請】請求書受領日を設定する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える 20200416 kubota
            //if (cmbOwnerType.GetSelectedValues() != string.Empty || cmbBlockCode.GetSelectedValues() != string.Empty ||
            //    cmbWorkType.SelectedValue.ToString() != string.Empty || cmbObjectType.SelectedValue.ToString() != string.Empty || cmbWorkFlg.SelectedIndex > 0)
            if (cmbOwnerTypeDEV.GetSelectedValues() != string.Empty || cmbBlockCodeDEV.GetSelectedValues() != string.Empty ||
            cmbWorkType.SelectedValue.ToString() != string.Empty || cmbObjectType.SelectedValue.ToString() != string.Empty || cmbWorkFlg.SelectedIndex > 0)
                //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える end
            {
                string msg = string.Format(@"【{1}】{0}【{2}】{0}【{3}】{0}【{4}】{0}【{5}】{6}検索条件を指定せず操作を行ってください。"
                    , "、", "オーナータイプ", "ブロック", "業務種別", "作業種別", "作業実施", Environment.NewLine);
                MessageBox.Show(msg, Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(dgvCommon.FocusedRowHandle) as COMSSService.CommisionWorkPlanInfoResult;
            if (obj.BusinessConnectionsMstPid <= 0) return;

            DateTime billDate = DateTime.MinValue;
            DateTime paymentDate = DateTime.MinValue;
            S100200070 frmDate = new S100200070(obj.BillDate, obj.PaymentDate);
            if (frmDate.ShowDialog() == DialogResult.OK)
            {
                billDate = frmDate.GetDate();
                paymentDate = frmDate.GetPaymentDate();
            }
            else
            {
                return;
            }

            if (MessageBox.Show("設定しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;

            MonthlyBusinessConnectionApproval approval = new MonthlyBusinessConnectionApproval();
            approval.BusinessConnectionsMstPid = obj.BusinessConnectionsMstPid;
            approval.Date = obj.WorkResultDate.Contains("\n") ? obj.WorkResultDate.Substring(0, obj.WorkResultDate.IndexOf("\n")) : obj.WorkResultDate;
            approval.BillDate = billDate;
            approval.PaymentDate = paymentDate;
            

            List<CommisionWorkPlanInfoResult> fixList = new List<CommisionWorkPlanInfoResult>();
            List<CommisionWorkPlanInfoResult> continueList = new List<CommisionWorkPlanInfoResult>();
            GetMotoukeList(dgvCommon.FocusedRowHandle, ref fixList, ref continueList);



            (new S100001BL()).SetBusinessConnectionMstBillDate(approval, fixList, continueList);

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        private void mnuBillDateClear_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 0)
            {
                MessageBox.Show("【元請】請求書受領日を解除する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("解除しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;
            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(dgvCommon.FocusedRowHandle) as COMSSService.CommisionWorkPlanInfoResult;

            string date = obj.WorkResultDate.Contains("\n") ? obj.WorkResultDate.Substring(0, obj.WorkResultDate.IndexOf("\n")) : obj.WorkResultDate;

            (new S100001BL()).DeleteBusinessConnectionMstBillDate(obj.BusinessConnectionsMstPid, date);

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        private void btnReceptDate_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 0)
            {
                MessageBox.Show("請求書受領日を設定する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える 20200416 kubota
            //if (cmbOwnerType.GetSelectedValues() != string.Empty || cmbBlockCode.GetSelectedValues() != string.Empty ||
            if (cmbOwnerTypeDEV.GetSelectedValues() != string.Empty || cmbBlockCodeDEV.GetSelectedValues() != string.Empty ||


                cmbWorkType.SelectedValue.ToString() != string.Empty || cmbObjectType.SelectedValue.ToString() != string.Empty || cmbWorkFlg.SelectedIndex > 0)
            {
                string errorMsg = string.Format(@"【{1}】{0}【{2}】{0}【{3}】{0}【{4}】{0}【{5}】{6}検索条件を指定せず操作を行ってください。"
                    , "、", "オーナータイプ", "ブロック", "業務種別", "作業種別", "作業実施", Environment.NewLine);
                MessageBox.Show(errorMsg, Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //MK-1129 【Windows10対応】チェック付きプルダウンをDevExpress製品に入れ替える end

            bool hasChoose = false;
            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (dgvCommon.GetRowCellValue(i, clCreate).Equals(true) && (dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult).ContractType == Constant.CONTRACT_TYPE_0010)
                {
                    hasChoose = true;
                    break;
                }
            }

            if (!hasChoose)
            {
                MessageBox.Show("項目を選択してください。", Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }            

            DateTime billDate = DateTime.MinValue;
            DateTime paymentDate = DateTime.MinValue;
            S100200070 frmDate = new S100200070(billDate, paymentDate);
            if (frmDate.ShowDialog() == DialogResult.OK)
            {
                billDate = frmDate.GetDate();
                paymentDate = frmDate.GetPaymentDate();
            }
            else
            {
                return;
            }

            bool isMoto = frmDate.isMoto;
            string msg = string.Format("{0}請求書受領日を一括で登録しますか？", isMoto ? "【元請】" : "【直発注】");

            if (MessageBox.Show(msg, "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.Default;
            itemIndexList.Clear();

            #region 元請受領日をセット

            if (isMoto)
            {

                System.Collections.Hashtable planIDList = new System.Collections.Hashtable();

                for (int i = 0; i < dgvCommon.RowCount; i++)
                {
                    if (dgvCommon.GetRowCellValue(i, clCreate).Equals(true))
                    {
                        itemIndexList.Add(i);

                        COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult;
                        if (obj.BusinessConnectionsMstPid <= 0 || obj.ContractType != Constant.CONTRACT_TYPE_0010) continue;

                        string month = GetMonth(obj.WorkResultDate, true);
                        string key = obj.BusinessConnectionsMstPid.ToString() + "|" + month;

                        if (!planIDList.Contains(key))
                        {
                            planIDList.Add(key, i);
                        }

                    }
                }

                S100001BL business = new S100001BL();

                foreach (string key in planIDList.Keys)
                {
                    int index = int.Parse(planIDList[key].ToString());
                    CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(index) as CommisionWorkPlanInfoResult;

                    MonthlyBusinessConnectionApproval approval = new MonthlyBusinessConnectionApproval();
                    approval.BusinessConnectionsMstPid = obj.BusinessConnectionsMstPid;
                    approval.Date = obj.WorkResultDate.Contains("\n") ? obj.WorkResultDate.Substring(0, obj.WorkResultDate.IndexOf("\n")) : obj.WorkResultDate;
                    approval.BillDate = billDate;
                    approval.PaymentDate = paymentDate;


                    List<CommisionWorkPlanInfoResult> fixList = new List<CommisionWorkPlanInfoResult>();
                    List<CommisionWorkPlanInfoResult> continueList = new List<CommisionWorkPlanInfoResult>();
                    GetMotoukeList(index, ref fixList, ref continueList);

                    business.SetBusinessConnectionMstBillDate(approval, fixList, continueList);
                }
            }

            #endregion

            #region 直発注

            else
            {
                for (int i = 0; i < dgvCommon.RowCount; i++)
                {
                    if (dgvCommon.GetRowCellValue(i, clCreate).Equals(true))
                    {
                        itemIndexList.Add(i);
                        CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as CommisionWorkPlanInfoResult;
                        if (obj.ContractType != Constant.CONTRACT_TYPE_0010)
                        {
                            (new S100001BL()).SetWorkBillDate(obj.CommisionWorkPlanInfoPid, billDate);
                        }                        
                    }
                }
            }

            #endregion

            this.Cursor = Cursors.Default;

            SearchDataAjax();

        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            PrintPreviewForm frm = new PrintPreviewForm(this.dgvCommon, ignoreAutoFormatColumns);
            frm.ShowDialog();
        }

        private void btnSecomCheck_Click(object sender, EventArgs e)
        {
            S100200050 frm = new S100200050();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                pnlSC.Visible = false;
                SearchDataAjax();
            }
        }

        private void mnuWorkBillDateSet_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 0)
            {
                MessageBox.Show("【直発注】請求書受領日を設定する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }



            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(dgvCommon.FocusedRowHandle) as COMSSService.CommisionWorkPlanInfoResult;

            DateTime billDate = DateTime.MinValue;
            S100200040 frmDate = new S100200040(obj.BillDate);
            if (frmDate.ShowDialog() == DialogResult.OK)
            {
                billDate = frmDate.GetDate();
            }
            else
            {
                return;
            }

            if (MessageBox.Show("設定しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;           

            (new S100001BL()).SetWorkBillDate(obj.CommisionWorkPlanInfoPid, billDate);

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        private void mnuWorkBillDateClear_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 0)
            {
                MessageBox.Show("【直発注】請求書受領日を解除する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("解除しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;
            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(dgvCommon.FocusedRowHandle) as COMSSService.CommisionWorkPlanInfoResult;
            (new S100001BL()).DeleteWorkBillDate(obj.CommisionWorkPlanInfoPid);

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        long GetKBPriceTotal(ref List<CommisionWorkPlanInfoResult> dataList)
        {

            CommisionWorkPlanInfoResultEx objEx = dgvCommon.GetRow(dgvCommon.FocusedRowHandle) as CommisionWorkPlanInfoResultEx;
            COMSSService.CommisionWorkPlanInfoResult obj = new CommisionWorkPlanInfoResult();
            obj.CopyObjectData(objEx);
            long priceTotal = (obj.PriceTotal != long.MinValue ? obj.PriceTotal : 0);            
            
            dataList.Add(obj);            

            string month = GetMonth(obj.WorkResultDate, false);

            for (int i = 0; i < dgvCommon.RowCount; i++)
            {                
                CommisionWorkPlanInfoResultEx itemEx = dgvCommon.GetRow(i) as CommisionWorkPlanInfoResultEx;
                COMSSService.CommisionWorkPlanInfoResult item = new CommisionWorkPlanInfoResult();
                item.CopyObjectData(itemEx);

                if (i != dgvCommon.FocusedRowHandle && item.BusinessConnectionsMstPid == obj.BusinessConnectionsMstPid 
                    && month == GetMonth(item.WorkResultDate, false) && obj.ContractType == item.ContractType)
                {
                    priceTotal += (item.PriceTotal != long.MinValue ? item.PriceTotal : 0);

                    dataList.Add(item); 
                }
            }
            return priceTotal;
        }

        private void mnuKBSetDate_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 0)
            {
                MessageBox.Show("【直発注】入金日を設定する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(dgvCommon.FocusedRowHandle) as COMSSService.CommisionWorkPlanInfoResult;
            if (obj.BusinessConnectionsMstPid <= 0) return;

            MonthlyBusinessRequestExecInfo mbRequest = new MonthlyBusinessRequestExecInfo();

            DateTime dt = obj.KBPaymentDate != string.Empty ? DateTime.Parse(obj.KBPaymentDate) : DateTime.MinValue;

            List<CommisionWorkPlanInfoResult> dataList = new List<CommisionWorkPlanInfoResult>();
            long sumFix = 0;
            long sum = GetKBPriceTotal(ref dataList);

            S100200060 frmDate = new S100200060(dt, obj.KBTotal, sum, obj.KBCommision);
            if (frmDate.ShowDialog() == DialogResult.OK)
            {
                mbRequest.PaymentDate = frmDate.GetDate();
                mbRequest.Total = frmDate.GetTotal();
                mbRequest.Commission = frmDate.GetCommision();
            }
            else
            {
                return;
            }

            if (MessageBox.Show("設定しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;

            
            mbRequest.BusinessConnectionsMstPid = obj.BusinessConnectionsMstPid;
            mbRequest.Date = obj.WorkResultDate.Contains("\n") ? obj.WorkResultDate.Substring(0, obj.WorkResultDate.IndexOf("\n")) : obj.WorkResultDate;

            (new S100001BL()).SetMonthlyBusinessRequestExecInfo(mbRequest, dataList);

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        private void mnuKBClearDate_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 0)
            {
                MessageBox.Show("【直発注】入金日を解除する場合は、検索条件で実施年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("解除しますか？", "確認ダイアログ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.Cursor = Cursors.WaitCursor;
            COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(dgvCommon.FocusedRowHandle) as COMSSService.CommisionWorkPlanInfoResult;

            string date = obj.WorkResultDate.Contains("\n") ? obj.WorkResultDate.Substring(0, obj.WorkResultDate.IndexOf("\n")) : obj.WorkResultDate;

            (new S100001BL()).DeleteMonthlyBusinessRequestExecInfo(obj.BusinessConnectionsMstPid, date);

            this.Cursor = Cursors.Default;

            SearchDataAjax();
        }

        private void btnMoney_Click(object sender, EventArgs e)
        {
            S100200080 frm = new S100200080();
            frm.ShowDialog();
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            // TODO
            //if (e.Column == clNoImage)
            //{
            //    COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(e.RowHandle) as COMSSService.CommisionWorkPlanInfoResult;
            //    if (obj.BusinessConnectionsMstPid != long.MinValue)
            //    {
            //        e.RepositoryItem = repNoMail;
            //    }
            //    else
            //    {
            //        e.RepositoryItem = repBlank;
            //    }
            //}
        }

        bool isEnter = false;
        private void gridView1_MouseMove(object sender, MouseEventArgs e)
        {
            // TODO
            //GridView view = (GridView)sender;
            //Point pt = view.GridControl.PointToClient(Control.MousePosition);
            //GridHitInfo info = view.CalcHitInfo(pt);
            //if (info.InRowCell && info.Column == clNoImage)
            //{
            //    if (!isEnter)
            //    {
            //        isEnter = true;

            //        COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(info.RowHandle) as COMSSService.CommisionWorkPlanInfoResult;
            //        if (obj.BusinessConnectionsMstPid != long.MinValue && obj.Email == string.Empty)
            //        {
            //            toolTip1.SetToolTip(dgvCommon, "メールアドレスを登録して下さい。");
            //        }
            //        else
            //        {
            //            toolTip1.RemoveAll();
            //        }
            //    }
            //}
            //else
            //{
            //    isEnter = false;
            //    toolTip1.RemoveAll();
            //}
        }

        private void gridView1_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            // TODO
            //try
            //{
            //    if (e.Column == clKumiaiCode || e.Column == clCompanyCode)
            //    {
            //        e.Handled = true;
            //        e.Result = int.Parse(e.Value1.ToString()).CompareTo(int.Parse(e.Value2.ToString()));
            //    }
            //}
            //catch { }
        }

        private void mnuDocument_Opening(object sender, CancelEventArgs e)
        {

        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            if (searchTypeIndex == 1)
            {
                MessageBox.Show("確認書受領日を設定する場合は、検索条件で予定年月日を指定してください。", "確認ダイアログ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool hasChoose = false;
            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (dgvCommon.GetRowCellValue(i, clCreate).Equals(true))
                {
                    hasChoose = true;
                    break;
                }
            }

            if (!hasChoose)
            {
                MessageBox.Show("項目を選択してください。", Constant.CONFIRM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int selectedType = 0;
            DateTime? approvalDate = DateTime.MinValue;
            S100200900 frmApproval = new S100200900();
            if (frmApproval.ShowDialog() != DialogResult.OK) return;

            selectedType = frmApproval.SelectedType;
            approvalDate = frmApproval.ApprovalDate;            

            this.Cursor = Cursors.Default;
            itemIndexList.Clear();

            System.Collections.Hashtable planIDList = new System.Collections.Hashtable();
            List<COMSSService.DictionaryEntry> planList = new List<DictionaryEntry>();

            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (dgvCommon.GetRowCellValue(i, clCreate).Equals(true))
                {
                    itemIndexList.Add(i);

                    COMSSService.CommisionWorkPlanInfoResult obj = dgvCommon.GetRow(i) as COMSSService.CommisionWorkPlanInfoResult;
                    if (obj.BusinessConnectionsMstPid <= 0) continue;
                    if (obj.ContractType == Constant.CONTRACT_TYPE_0010 && selectedType == 1) continue; //直発注設定
                    if (obj.ContractType != Constant.CONTRACT_TYPE_0010 && selectedType == 0) continue; //元請設定

                    string month = GetMonth(obj.WorkPlanDueDate, true);
                    string key = obj.BusinessConnectionsMstPid.ToString() + "|" + month;

                    if (!planIDList.Contains(key))
                    {
                        planIDList.Add(key, i);

                        COMSSService.DictionaryEntry entry = new DictionaryEntry();
                        entry.Key = obj.BusinessConnectionsMstPid;
                        entry.Value = obj.WorkPlanDueDate.Contains("\n") ? obj.WorkPlanDueDate.Substring(0, obj.WorkPlanDueDate.IndexOf("\n")) : obj.WorkPlanDueDate;

                        //entry.Value = entry.Value.ToString() + "|" + approvalDate.ToString(Constant.SHOWING_DATETIME_FORMAT);
                        entry.Value = entry.Value.ToString() + "|" + approvalDate.ToString();
                        entry.Value += string.Format("|{0}", selectedType + 1);  

                        planList.Add(entry);

                    }

                }
            }

            (new S100001BL()).ApproveBusinessConnectionMst(planList.ToArray());
           

            this.Cursor = Cursors.Default;

            SearchDataAjax();
    
        }

        private void S100200010_Activated(object sender, EventArgs e)
        {
            Helper.ActivateLoadingPopup();
        }

        #region grid event

        private void dgvCommon_ButtonIconNeeded(object sender, ButtonIconNeededEventArgs e)
        {
            if (e.Column == clSB0120 || e.Column == clSB0020 || e.Column == clSB0220 || e.Column == clSB0225)
            {
                COMSSService.CommisionWorkPlanInfoResult obj = e.DataBoundItem as COMSSService.CommisionWorkPlanInfoResult;
                bool hasDocument = false;
                if (e.Column == clSB0120 && obj.DocumentSB0120Count > 0) hasDocument = true;
                else if (e.Column == clSB0020 && obj.DocumentSB0020Count > 0) hasDocument = true;
                else if (e.Column == clSB0220 && obj.DocumentSB0220Count > 0) hasDocument = true;
                else if (e.Column == clSB0225 && obj.DocumentSB0225Count > 0) hasDocument = true;

                if (hasDocument)
                {
                    e.Icon = Properties.Resources.folder_blue;

                }
                else
                {
                    e.Icon = Properties.Resources.folder_red;
                }
            }
        }

        private void dgvCommon_CellBackColorNeeded(object sender, CellBackColorNeededEventArgs e)
        {
            var obj = e.DataBoundItem as CommisionWorkPlanInfoResult;

            if (e.Column == clSB0110)
            {
                if (obj.DocumentSB0110.EndsWith(Constant.DOCUMENT_DISABLE_STATUS))
                {
                    e.BackColor = Color.Lavender;
                }
                if (obj.WorkType != Constant.WORK_TYPE_0010 || obj.ContractType != Constant.CONTRACT_TYPE_0010)
                {
                    e.BackColor = Color.LightGray;
                }
            }
            else if (e.Column == clSB0115)
            {
                if (obj.DocumentSB0115.EndsWith(Constant.DOCUMENT_DISABLE_STATUS))
                {
                    e.BackColor = Color.Lavender;
                }

                if (obj.WorkType != Constant.WORK_TYPE_0010 || obj.ContractType != Constant.CONTRACT_TYPE_0110)
                {
                    e.BackColor = Color.LightGray;
                }
            }
            else if (e.Column == clSB0210)
            {
                if (obj.DocumentSB0210.EndsWith(Constant.DOCUMENT_DISABLE_STATUS))
                {
                    e.BackColor = Color.Lavender;
                }
                if (obj.ContractType != Constant.CONTRACT_TYPE_0010)
                {
                    e.BackColor = Color.LightGray;
                }
            }
            else if (e.Column == clSB0310)
            {
                if (obj.DocumentSB0310.EndsWith(Constant.DOCUMENT_DISABLE_STATUS))
                {
                    e.BackColor = Color.Lavender;
                }
            }
            else if (e.Column == clSB0215)
            {
                if (obj.ContractType != Constant.CONTRACT_TYPE_0110)
                {
                    e.BackColor = Color.LightGray;
                }
                if (obj.DocumentSB0215.EndsWith(Constant.DOCUMENT_DISABLE_STATUS))
                {
                    e.BackColor = Color.Lavender;
                }
            }
            else if (e.Column == clApprovalDate)
            {
                if (obj.ContractType != Constant.CONTRACT_TYPE_0010)
                {
                    e.BackColor = Color.LightGray;
                }
                else if (obj.ApprovalDate == DateTime.MinValue)
                {
                    e.BackColor = Color.FromArgb(255, 192, 192);
                }
            }
            else if (e.Column == clApprovalDate2)
            {
                if (obj.ContractType == Constant.WORK_TYPE_0010)
                {
                    e.BackColor = Color.LightGray;
                }
                else if (obj.ApprovalDate2 == DateTime.MinValue)
                {
                    e.BackColor = Color.FromArgb(255, 192, 192);
                }
            }
            else if (e.Column == clBillDate || e.Column == clPaymentDate)
            {
                if (obj.ContractType == Constant.CONTRACT_TYPE_0010)
                {
                    if (obj.BillDate == DateTime.MinValue)
                    {
                        e.BackColor = Color.FromArgb(255, 192, 192);
                    }
                }
                else
                {
                    e.BackColor = Color.LightGray;
                }
            }
            else if (e.Column == clWorkBillDate)
            {
                if (obj.ContractType != Constant.CONTRACT_TYPE_0010)
                {
                    if (obj.WorkBillDate == DateTime.MinValue)
                    {
                        e.BackColor = Color.FromArgb(255, 192, 192);
                    }
                }
                else
                {
                    e.BackColor = Color.LightGray;
                }
            }
            else if (e.Column == clWorkType)
            {
                if (obj.WorkType == "0020")
                {
                    e.BackColor = Color.Aqua;
                }
            }
            else if (e.Column == clStatusTitle)
            {
                string status = obj.Status;
                if (status == "0020")
                {
                    e.BackColor = Color.Yellow;
                }
                else if (status == "0030")
                {
                    e.BackColor = Color.Pink;
                }
            }
            //【直発注】入金日
            else if (e.Column == clKBPaymentDate || e.Column == clKBTotal || e.Column == clKBCommision)
            {
                if (obj.ContractType == Constant.CONTRACT_TYPE_0110)
                {
                    e.BackColor = Color.FromArgb(255, 192, 192);
                }
                else
                {
                    e.BackColor = Color.LightGray;
                }
            }
        }

        private void dgvCommon_TextBoxIconNeeded(object sender, TextBoxIconNeededEventArgs e)
        {
            if (e.Column == clNoImage)
            {
                COMSSService.CommisionWorkPlanInfoResult obj = e.DataBoundItem as COMSSService.CommisionWorkPlanInfoResult;
                if (obj.BusinessConnectionsMstPid != long.MinValue)
                {
                    if (obj.Email == string.Empty) e.Icon = Properties.Resources.nomailimage;
                    else e.Icon = Properties.Resources.email;
                }
            }
        }

        private void dgvCommon_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                var grid = sender as DataGridViewEx;
                if (grid == null) return;

                Cursor.Current = Cursors.WaitCursor;

                var objEx = grid.Rows[e.RowIndex].DataBoundItem as CommisionWorkPlanInfoResultEx;
                if (objEx == null) return;
                var obj = new CommisionWorkPlanInfoResult();
                obj.CopyObjectData(objEx);

                var col = grid.Columns[e.ColumnIndex];
                var (top, bottom) = grid.GetMergeRangeForCell(col, e.ColumnIndex, e.RowIndex);
                if (e.RowIndex != top)
                    return;

                Point pt = DataGridViewExHelper.GetGridPointFromCellMouseEvent(dgvCommon, e);
                if (e.Button == MouseButtons.Right && (col == clSB0110 || col == clSB0115 || col == clSB0210
                || col == clSB0310 || col == clSB0215))
                {
                    if (obj.BusinessConnectionsMstPid <= 0)
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }

                    //【元請】確認書/青書(定期)
                    if (col == clSB0110 && (obj.WorkType != Constant.WORK_TYPE_0010 || obj.ContractType != Constant.CONTRACT_TYPE_0010))
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }

                    //【直発注】確認書/青書(定期)
                    if (col == clSB0115 && (obj.WorkType != Constant.WORK_TYPE_0010 || obj.ContractType != Constant.CONTRACT_TYPE_0110))
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }

                    //協力会社宛請求書(直発注A）
                    if (col == clSB0215 && obj.ContractType != Constant.CONTRACT_TYPE_0110)
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }

                    //請求書/請書（元請）
                    if (col == clSB0210 && obj.ContractType != Constant.CONTRACT_TYPE_0010)
                    {
                        Cursor.Current = Cursors.Default;
                        return;
                    }

                    commisionWorkPlanInfoDelete = obj;
                    selectedColumn = col.Name;
                    string documentInfo = (col == clSB0110 ? obj.DocumentSB0110
                        : col == clSB0115 ? obj.DocumentSB0115
                        : col == clSB0210 ? obj.DocumentSB0210
                        : col == clSB0310 ? obj.DocumentSB0310
                        : obj.DocumentSB0215);

                    if (documentInfo != string.Empty)
                    {
                        mnuDownload.Enabled = true;
                        mnuItemSendMail.Enabled = true;
                        mnuPublish.Enabled = true;
                        mnuCreate.Enabled = false;

                        string[] vals = documentInfo.Split(',');
                        if (vals[4] == Constant.DOCUMENT_DISABLE_STATUS)
                        {
                            mnuDecide.Enabled = false;
                            mnuCancel.Enabled = true;
                            mnuItemDelete.Enabled = false;
                        }
                        else
                        {
                            mnuDecide.Enabled = true;
                            mnuCancel.Enabled = false;
                            mnuItemDelete.Enabled = true;
                        }
                        mnuDocument.Show(dgvCommon, pt);
                    }
                    else
                    {
                        mnuDownload.Enabled = false;
                        mnuDecide.Enabled = false;
                        mnuCancel.Enabled = false;
                        mnuItemDelete.Enabled = false;
                        mnuItemSendMail.Enabled = false;
                        mnuPublish.Enabled = false;
                        mnuCreate.Enabled = true;
                        mnuDocument.Show(dgvCommon, pt);
                    }
                }
                else if (col == clSubject || col == clWorkID)
                {
                    coms.COMSDService.KumiaiInfo kumiaiInfo = new coms.COMSDService.KumiaiInfo();
                    kumiaiInfo.Pid = obj.KumiaiInfoPid;
                    coms.COMSDService.CommisionPlanInfo planInfo = new coms.COMSDService.CommisionPlanInfo();
                    planInfo.Pid = obj.PlanPid;
                    COMSD.business.D100001BL bl = new COMSD.business.D100001BL();
                    List<coms.COMSDService.CommisionContractInfo> listContraction = bl.SearchCommisionContractInfo_IncludeChilds_ByKumiaiInfoPid(kumiaiInfo.Pid);

                    coms.COMSD.ui.D100001.D100001060 frm = new coms.COMSD.ui.D100001.D100001060(kumiaiInfo, planInfo, listContraction, false);
                    frm.isCanUpdateDB = true;
                    frm.formParent = this;
                    frm.Show();
                }
                else if (col == clKumiai && obj.KumiaiInfoPid > 0)
                {
                    COMSDService.KumiaiInfo kumi = new coms.COMSDService.KumiaiInfo();
                    kumi.Pid = obj.KumiaiInfoPid;

                    COMSD.ui.D100001.D100001020 frm = new coms.COMSD.ui.D100001.D100001020(kumi);
                    frm.formParent = this;
                    frm.Show();
                }
                else if (col == clSB0120 || col == clSB0020 || col == clSB0220 || col == clSB0225)
                {
                    if (obj.BusinessConnectionsMstPid > 0)
                    {
                        String month1 = obj.WorkPlanDueDate;
                        if (month1.Contains("\n"))
                        {
                            month1 = month1.Substring(0, month1.IndexOf("\n"));
                        }

                        string documentNo = string.Empty;
                        if (col == clSB0120) documentNo = Constant.SEIKYUUSYO_NO;
                        else if (col == clSB0020) documentNo = Constant.NATSUINZUMI_TATEMONO_NO;
                        else if (col == clSB0220) documentNo = Constant.NATSUINZUMI_SEIKYUUSYO_NO;
                        else documentNo = Constant.NATSUINZUMI_KYOURYOKUGAISYA_SEIKYUUSYO_NO;

                        S100001.S100001130 frm = new S100001.S100001130(obj.BusinessConnectionsMstPid, documentNo, month1);
                        frm.frmBusinessConnectionParent = this;
                        frm.Show();
                    }
                }

                else if (col == clSB0110 && obj.DocumentSB0110 != string.Empty ||
                    col == clSB0115 && obj.DocumentSB0115 != string.Empty ||
                    col == clSB0210 && obj.DocumentSB0210 != string.Empty ||
                    col == clSB0215 && obj.DocumentSB0215 != string.Empty)
                {
                    string documentInfo = col == clSB0110 ? obj.DocumentSB0110
                        : col == clSB0115 ? obj.DocumentSB0115
                        : col == clSB0210 ? obj.DocumentSB0210
                        : obj.DocumentSB0215;
                    string[] vals = documentInfo.Split(',');
                    COMSPService.Document document = new COMSPService.Document();
                    document.FileName = vals[3];
                    document.Pid = long.Parse(vals[1]);
                    COMSPService.FileEntry fileEntry = (new COMSP.business.P100007BL()).LoadFileFromServer(document);
                    SaveFileDialog fsDialog = new SaveFileDialog();
                    string fileExe = document.FileName.Substring(document.FileName.LastIndexOf(".") + 1);
                    fsDialog.Filter = fileExe + " file|*." + fileExe;
                    if (fsDialog.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = fsDialog.FileName;
                        if (Helper.SaveFile(fileName, fileEntry))
                        {
                            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo(fileName);
                            System.Diagnostics.Process.Start(processInfo);
                        }
                    }
                }
                else if (col == clInputInspection)
                {
                    //if (obj.InspectionDocument != string.Empty)
                    {
                        COMSSService.CommisionWorkPlanInfo workPlanInfo = (new S100001BL()).GetCompositeWorkPlanInfo(obj.CommisionWorkPlanInfoPid, Helper.loginUserInfo.Pid);
                        if (workPlanInfo != null)
                        {

                            COMSS.ui.S100500.InspectionForm frm = COMSS.ui.S100500.InspectionUtility.GenerateInputScreen(obj.PlanType);
                            if (frm != null)
                            {
                                frm.TypeTitle = obj.PlanTypeTitle;
                                frm.Subject = obj.PlanSubject;
                                frm.workPlanInfo = workPlanInfo;
                                frm.planInfoPid = obj.PlanPid;
                                frm.kumiaiInfoPid = obj.KumiaiInfoPid;
                                frm.planResultInfoPid = obj.PlanResultInfoPid;
                                frm.frmParent = this;
                                frm.Show();
                            }
                        }
                    }
                }
                else if (col == clSB0310)
                {
                    if (obj.DocumentSB0310 != string.Empty)
                    {
                        OpenDocument(obj.DocumentSB0310);
                    }
                }

                else if (col == clCompanyName)
                {
                    String month = dgvCommon.GetRowCellValue(e.RowIndex, clDueDatePlan).ToString();
                    if (month.Contains("\n"))
                    {
                        month = month.Substring(0, month.IndexOf("\n"));
                    }

                    S100200020 frmComment = new S100200020(obj.BusinessConnectionsMstPid, dgvCommon.GetRowCellValue(e.RowIndex, clCompanyName).ToString(), month);
                    frmComment.Show();
                }

                //【元請】確認受領日
                else if (e.Button == MouseButtons.Right && col == clApprovalDate)
                {
                    if (obj.ContractType != Constant.CONTRACT_TYPE_0010) return;
                    if (obj.ApprovalDate != DateTime.MinValue)
                    {
                        mnuUnApprove.Enabled = true;
                    }
                    else
                    {
                        mnuUnApprove.Enabled = false;
                    }
                    mnuApproval.Show(dgvCommon, pt);
                }
                //【直発注】確認受領日
                else if (e.Button == MouseButtons.Right && col == clApprovalDate2)
                {
                    if (obj.ContractType == Constant.CONTRACT_TYPE_0010) return;
                    if (obj.ApprovalDate2 != DateTime.MinValue)
                    {
                        mnuUnApprove.Enabled = true;
                    }
                    else
                    {
                        mnuUnApprove.Enabled = false;
                    }
                    mnuApproval.Show(dgvCommon, pt);
                }

                //【元請】請求書受領日
                else if (e.Button == MouseButtons.Right && col == clBillDate)
                {
                    if (obj.ContractType != Constant.CONTRACT_TYPE_0010) return;
                    if (obj.BillDate != DateTime.MinValue)
                    {
                        mnuBillDateClear.Enabled = true;
                    }
                    else
                    {
                        mnuBillDateClear.Enabled = false;
                    }
                    mnuBillDate.Show(dgvCommon, pt);
                }
                //【直発注】請求書受領日
                else if (e.Button == MouseButtons.Right && col == clWorkBillDate)
                {
                    if (obj.ContractType == Constant.CONTRACT_TYPE_0010) return;
                    if (obj.WorkBillDate != DateTime.MinValue)
                    {
                        mnuWorkBillDateClear.Enabled = true;
                    }
                    else
                    {
                        mnuWorkBillDateClear.Enabled = false;
                    }
                    mnuWorkBillDate.Show(dgvCommon, pt);
                }
                //【直発注】入金日
                else if (e.Button == MouseButtons.Right && col == clKBPaymentDate)
                {
                    if (obj.ContractType != Constant.CONTRACT_TYPE_0110) return;
                    if (obj.KBPaymentDate != string.Empty)
                    {
                        mnuKBClearDate.Enabled = true;
                    }
                    else
                    {
                        mnuKBClearDate.Enabled = false;
                    }
                    mnuKBPaymentDate.Show(dgvCommon, pt);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void dgvCommon_CellMerge(object sender, CellMergeEventArgs e)
        {
            try
            {
                DataGridViewEx view = sender as DataGridViewEx;

                String month1 = (searchTypeIndex == 0 ? view.GetRowCellValue(e.RowIndex1, clDueDatePlan).ToString() : view.GetRowCellValue(e.RowIndex1, clDateResult).ToString());
                if (month1.Length > 7)
                {
                    month1 = month1.Substring(0, 7);
                }
                String month2 = (searchTypeIndex == 0 ? view.GetRowCellValue(e.RowIndex2, clDueDatePlan).ToString() : view.GetRowCellValue(e.RowIndex2, clDateResult).ToString());
                if (month2.Length > 7)
                {
                    month2 = month2.Substring(0, 7);
                }

                if (e.Column == clSubject || e.Column == clWorkID || e.Column == clSubjectPlan || e.Column == clContractType || e.Column == clWorkType
                    || e.Column == clSumPrice || e.Column == clSumCost || e.Column == clSumSale || e.Column == clNote)
                {
                    String workID1 = view.GetRowCellValue(e.RowIndex1, clWorkID).ToString();
                    String workID2 = view.GetRowCellValue(e.RowIndex2, clWorkID).ToString();

                    e.Merge = (workID1 == workID2);
                    e.Handled = true;

                }

                else if (e.Column == clCompanyName || e.Column == clNoImage || e.Column == clCreate || e.Column == clKumiai
                    || e.Column == clDocumentList || e.Column == clBuildingName
                    || e.Column == clSB0310 || e.Column == clSB0020 || e.Column == clSB0110 || e.Column == clSB0115
                    || e.Column == clSB0120 || e.Column == clSB0210 || e.Column == clSB0215
                    || e.Column == clSB0220 || e.Column == clSB0225 || e.Column == clApprovalDate || e.Column == clApprovalDate2
                    || e.Column == clBillDate || e.Column == clPaymentDate || e.Column == clKBPaymentDate || e.Column == clKBTotal || e.Column == clKBCommision)
                {
                    String company1 = view.GetRowCellValue(e.RowIndex1, clCompanyName).ToString();
                    String company2 = view.GetRowCellValue(e.RowIndex2, clCompanyName).ToString();
                    string workType1 = view.GetRowCellValue(e.RowIndex1, clWorkType).ToString();
                    string workType2 = view.GetRowCellValue(e.RowIndex2, clWorkType).ToString();
                    string contractType1 = view.GetRowCellValue(e.RowIndex1, clContractType).ToString();
                    string contractType2 = view.GetRowCellValue(e.RowIndex2, clContractType).ToString();

                    if (e.Column == clCompanyName || e.Column == clNoImage || e.Column == clCreate
                        || e.Column == clSB0020 || e.Column == clSB0120
                        || e.Column == clSB0220 || e.Column == clSB0225
                        || e.Column == clApprovalDate || e.Column == clApprovalDate2
                        || e.Column == clBillDate || e.Column == clPaymentDate
                        || e.Column == clKBPaymentDate || e.Column == clKBTotal
                        || e.Column == clKBCommision || e.Column == clSB0215 || e.Column == clSB0210
                        || e.Column == clSB0110 || e.Column == clSB0115)
                    {
                        if (e.Column == clSB0110 || e.Column == clSB0115)
                        {
                            e.Merge = (month1 == month2 && company1 == company2 && workType1 == workType2 && contractType1 == contractType2);
                            e.Handled = true;
                        }
                        else if (e.Column != clBillDate && e.Column != clPaymentDate && e.Column != clKBPaymentDate
                            && e.Column != clApprovalDate && e.Column != clApprovalDate2
                            && e.Column != clKBTotal && e.Column != clKBCommision && e.Column != clSB0215 && e.Column != clSB0210)
                        {
                            e.Merge = (month1 == month2 && company1 == company2);
                            e.Handled = true;
                        }
                        else if (e.Column == clApprovalDate2) //【直発注】確認書受領日
                        {
                            e.Merge = (month1 == month2 && company1 == company2
                                && (contractType1 == contractType2 || (contractType1 != Constant.CONTRACT_TYPE_0010_TITLE && contractType2 != Constant.CONTRACT_TYPE_0010_TITLE)));
                            e.Handled = true;
                        }
                        else
                        {
                            e.Merge = (month1 == month2 && company1 == company2 && contractType1 == contractType2);
                            e.Handled = true;
                        }
                    }

                    else if (e.Column == clSB0310 || e.Column == clSB0215)
                    {

                        bool equal = false;
                        CommisionWorkPlanInfoResult ret1 = view.GetRow(e.RowIndex1) as CommisionWorkPlanInfoResult;
                        CommisionWorkPlanInfoResult ret2 = view.GetRow(e.RowIndex2) as CommisionWorkPlanInfoResult;

                        if (e.Column == clSB0310 && ret1.DocumentSB0310 == ret2.DocumentSB0310) equal = true;
                        else if (e.Column == clSB0110 && ret1.DocumentSB0110 == ret2.DocumentSB0110) equal = true;
                        else if (e.Column == clSB0115 && ret1.DocumentSB0115 == ret2.DocumentSB0115) equal = true;
                        else if (e.Column == clSB0215 && ret1.DocumentSB0215 == ret2.DocumentSB0215) equal = true;

                        e.Merge = (month1 == month2 && company1 == company2 && equal);
                        e.Handled = true;
                    }

                    else if (e.Column == clKumiai)
                    {
                        String kumiai1 = (String)view.GetRowCellValue(e.RowIndex1, e.Column);
                        String kumiai2 = (String)view.GetRowCellValue(e.RowIndex2, e.Column);

                        e.Merge = (month1 == month2 && company1 == company2 && kumiai1 == kumiai2);
                        e.Handled = true;
                    }
                    else
                    {
                        String kumiai1 = (String)view.GetRowCellValue(e.RowIndex1, clKumiai);
                        String kumiai2 = (String)view.GetRowCellValue(e.RowIndex2, clKumiai);

                        String val1 = (String)view.GetRowCellValue(e.RowIndex1, e.Column);
                        String val2 = (String)view.GetRowCellValue(e.RowIndex2, e.Column);

                        e.Merge = (month1 == month2 && company1 == company2 && kumiai1 == kumiai2 && val1 == val2);
                        e.Handled = true;
                    }
                }
                else if (e.Column == clBlockName || e.Column == clKumiaCode || e.Column == clCompanyCode)
                {
                    e.Merge = view.IsEqual(e.RowIndex1, e.Column, e.RowIndex2, e.Column);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                int x = 2;
            }
        }
        #endregion

        private void dgvCommon_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!dataLoaded) return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var dgvCommon = sender as DataGridViewEx;
            if (dgvCommon == null)
                return;

            var col = dgvCommon.Columns[e.ColumnIndex];

            // =========================================
            // ONLY HANDLE clCreate CHECKBOX
            // =========================================
            if (col != clCreate)
                return;

            // ===== value is already updated =====
            bool check = false;
            var cellValue = dgvCommon.GetRowCellValue(e.RowIndex, clCreate);
            if (cellValue is bool b)
                check = b;
            else if (bool.TryParse(cellValue?.ToString(), out bool parsed))
                check = parsed;

            // ===== get company name =====
            string companyName =
                dgvCommon.GetRowCellValue(e.RowIndex, clCompanyName)?.ToString();

            if (string.IsNullOrEmpty(companyName))
                return;

            // ===== get month of current row =====
            int monthColIndex = (searchTypeIndex == 0)
                ? clDueDatePlan.Index
                : clDateResult.Index;

            string month1 =
                dgvCommon.GetRowCellValue(e.RowIndex, dgvCommon.Columns[monthColIndex])?.ToString();

            if (!string.IsNullOrEmpty(month1) && month1.Contains("\n"))
                month1 = month1.Substring(0, month1.IndexOf("\n"));

            // =========================================
            // APPLY TO RELATED ROWS
            // =========================================
            for (int i = 0; i < dgvCommon.RowCount; i++)
            {
                if (i == e.RowIndex) continue;

                string company2 =
                    dgvCommon.GetRowCellValue(i, clCompanyName)?.ToString();

                if (company2 != companyName)
                    continue;

                string month2 =
                    dgvCommon.GetRowCellValue(i, dgvCommon.Columns[monthColIndex])?.ToString();

                if (!string.IsNullOrEmpty(month2) && month2.Contains("\n"))
                    month2 = month2.Substring(0, month2.IndexOf("\n"));

                if (month2 == month1)
                {
                    var rowObj = dgvCommon.GetRow(i) as CommisionWorkPlanInfoResult;
                    if (rowObj != null)
                    {
                        rowObj.CreateFile = check;
                    }

                    // Optional: sync cell if needed
                    dgvCommon.SetRowCellValue(i, clCreate.Name, check);
                }
            }

            dgvCommon.Invalidate();
            dgvCommon.Refresh();
        }
    }
}